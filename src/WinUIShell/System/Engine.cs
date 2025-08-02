using System.Diagnostics;
using System.Management.Automation;
using WinUIShell.Common;

namespace WinUIShell;

public static class Engine
{
    private static readonly ThreadLocal<bool> _isThisThreadStarted = new(() => false);
    private static readonly ThreadLocal<bool> _isThisThreadInUpdate = new(() => false);
    private static readonly object _lock = new();
    private static int _mainThreadId = Constants.InvalidThreadId;
    private static string _upstreamPipeName = "";
    private static string _downstreamPipeName = "";
    private static Process? _serverProcess;

    public static void Start(string serverExePath)
    {
        if (_isThisThreadStarted.Value)
            return;

        lock (_lock)
        {
            if (_mainThreadId == Constants.InvalidThreadId)
            {
                InitPipeNames();
                try
                {
                    StartServerProcess(serverExePath);
                    InitConnection();
                }
                catch (Exception)
                {
                    StopServerProcess();
                    Console.Error.WriteLine($"Failed to start server [{serverExePath}]");
                    throw;
                }
                _mainThreadId = Environment.CurrentManagedThreadId;
            }
        }

        InitTimerEvent();

        _isThisThreadStarted.Value = true;
    }

    public static void Stop()
    {
        if (!_isThisThreadStarted.Value)
            return;

        _isThisThreadStarted.Value = false;

        TermTimerEvent();

        lock (_lock)
        {
            if (Environment.CurrentManagedThreadId == _mainThreadId)
            {
                TermConnection();
                StopServerProcess();
                _mainThreadId = Constants.InvalidThreadId;
            }
        }
    }

    private static void InitPipeNames()
    {
        var processId = Environment.ProcessId.ToString();
        _upstreamPipeName = $"WinUIShell.ClientToServer.{processId}";
        _downstreamPipeName = $"WinUIShell.ServerToClient.{processId}";
    }

    private static void StartServerProcess(string path)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = path
        };
        startInfo.ArgumentList.Add(_upstreamPipeName);
        startInfo.ArgumentList.Add(_downstreamPipeName);
        startInfo.ArgumentList.Add(Environment.ProcessId.ToString());
        _serverProcess = Process.Start(startInfo);
    }

    private static void StopServerProcess()
    {
        if (_serverProcess is null)
            return;

        _serverProcess.Kill();
    }

    private static void InitConnection()
    {
        ObjectStore.Get().SetObjectIdPrefix("c");
        CommandServer.Get().Init(_downstreamPipeName);
        CommandClient.Get().Init(_upstreamPipeName);
    }

    private static void TermConnection()
    {
        CommandClient.Get().Term();
        CommandServer.Get().Term();
    }

    private static void InitTimerEvent()
    {
        // Use script block to make the timer action run on this thread.
        // It looks like only ScriptBlocks can call a private version of Runspace.DefaultRunspace.Events.SubscribeEvent that has 'shouldQueueAndProcessInExecutionThread' parameter.
        var script = @"
$script:engineUpdateTimer = New-Object Timers.Timer
$script:engineUpdateTimer.Interval = 8
$script:engineUpdateTimer.AutoReset = $false
$script:engineUpdateTimer.Enabled = $true
$script:engineUpdateJob = Register-ObjectEvent -InputObject $script:engineUpdateTimer -EventName 'Elapsed' -Action {
    [WinUIShell.Engine]::IdleUpdate()
    $engineUpdateTimer = $Sender
    $engineUpdateTimer.Start()
}
";
        var scriptBlock = ScriptBlock.Create(script);
        _ = scriptBlock.Invoke();
    }

    private static void TermTimerEvent()
    {
        var script = @"
$script:engineUpdateJob.StopJob()
";
        var scriptBlock = ScriptBlock.Create(script);
        _ = scriptBlock.Invoke();
    }

    public static void IdleUpdate()
    {
        if (!_isThisThreadStarted.Value)
            return;

        // Do not run commands inside other event callbacks.
        if (_isThisThreadInUpdate.Value)
            return;

        var queueId = new CommandQueueId(Environment.CurrentManagedThreadId);
        CommandServer.Get().ProcessCommands(queueId);
    }

    internal static void Update()
    {
        if (!_isThisThreadStarted.Value)
            return;

        var queueId = new CommandQueueId(Environment.CurrentManagedThreadId);
        if (!_isThisThreadInUpdate.Value)
        {
            // Root update.
            _isThisThreadInUpdate.Value = true;
            CommandServer.Get().ProcessCommands(queueId);
            _isThisThreadInUpdate.Value = false;
        }
        else
        {
            // Recursive update.
            CommandServer.Get().ProcessCommands(queueId);
        }
    }
}
