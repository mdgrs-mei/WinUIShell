using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using WinUIShell.Common;

namespace WinUIShell;

public static class Engine
{
    private sealed class RunspaceState
    {
        public bool IsInitialized { get; set; }
        public bool IsInUpdate { get; set; }
        public System.Timers.Timer? EventTimer;
        public PSEventSubscriber? TimerEventSubscriber;
    }

    private static readonly RunspaceLocal<RunspaceState> _thisRunspace = new(() => new RunspaceState());
    private static readonly object _lock = new();
    private static int _mainRunspaceId = Constants.InvalidRunspaceId;
    private static string _upstreamPipeName = "";
    private static string _downstreamPipeName = "";
    private static Process? _serverProcess;
    private static readonly CommandThreadPool _commandThreadPool = new();

    public static void InitRunspace(
        string serverExePath,
        PSHost? streamingHost,
        string modulePath,
        bool useTimerEvent)
    {
        var thisRunspace = _thisRunspace.Value;
        if (thisRunspace.IsInitialized)
            return;

        lock (_lock)
        {
            if (_mainRunspaceId == Constants.InvalidRunspaceId)
            {
#if DEBUG
                System.Diagnostics.Debugger.Launch();
#endif
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
                _mainRunspaceId = Runspace.DefaultRunspace.Id;
                InitCommandThreadPool(streamingHost, modulePath);
            }
        }

        if (useTimerEvent)
        {
            InitTimerEvent();
        }

        thisRunspace.IsInitialized = true;
    }

    public static void TermRunspace()
    {
        var thisRunspace = _thisRunspace.Value;
        if (!thisRunspace.IsInitialized)
            return;

        thisRunspace.IsInitialized = false;

        TermTimerEvent();

        lock (_lock)
        {
            if (Runspace.DefaultRunspace.Id == _mainRunspaceId)
            {
                TermCommandThreadPool();
                TermConnection();
                StopServerProcess();
                _mainRunspaceId = Constants.InvalidRunspaceId;
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
        _serverProcess = null;
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
        var thisRunspace = _thisRunspace.Value;

        // Register timer event to process the main command queue.
        // The timer event fires when commands are processed on the main runspace or when waiting for user inputs in interactive sessions.
        thisRunspace.EventTimer = new()
        {
            Interval = Constants.ClientTimerEventCommandPolingIntervalMillisecond,
            AutoReset = false,
            Enabled = false
        };

        ScriptBlock action = ScriptBlock.Create(@"
[WinUIShell.Engine]::IdleUpdateRunspace()
$engineUpdateTimer = $Sender
$engineUpdateTimer.Start()
"
        );

        thisRunspace.TimerEventSubscriber = Runspace.DefaultRunspace.Events.SubscribeEvent(
            source: thisRunspace.EventTimer,
            eventName: "Elapsed",
            sourceIdentifier: "",
            data: null,
            action: action,
            supportEvent: false,
            forwardEvent: false);

        thisRunspace.EventTimer.Start();
    }

    private static void TermTimerEvent()
    {
        var thisRunspace = _thisRunspace.Value;
        if (thisRunspace.EventTimer is null)
            return;

        thisRunspace.EventTimer.Stop();
        Runspace.DefaultRunspace.Events.UnsubscribeEvent(thisRunspace.TimerEventSubscriber);
    }

    private static void InitCommandThreadPool(PSHost? streamingHost, string modulePath)
    {
        _commandThreadPool.Init(streamingHost, modulePath);
    }

    private static void TermCommandThreadPool()
    {
        _commandThreadPool.Term();
    }

    public static void SetCommandThreadPoolInitializationScript(ScriptBlock? scriptBlock)
    {
        _commandThreadPool.SetInitializationScript(scriptBlock);
    }

    public static void IdleUpdateRunspace()
    {
        var thisRunspace = _thisRunspace.Value;
        if (!thisRunspace.IsInitialized)
            return;

        // Do not run commands inside other event callbacks.
        if (thisRunspace.IsInUpdate)
            return;

        var queueId = new CommandQueueId(Runspace.DefaultRunspace.Id);
        CommandServer.Get().ProcessCommands(queueId);
    }

    internal static void UpdateRunspace()
    {
        var thisRunspace = _thisRunspace.Value;
        if (!thisRunspace.IsInitialized)
            return;

        var queueId = new CommandQueueId(Runspace.DefaultRunspace.Id);
        if (!thisRunspace.IsInUpdate)
        {
            // Root update.
            thisRunspace.IsInUpdate = true;
            CommandServer.Get().ProcessCommands(queueId);
            thisRunspace.IsInUpdate = false;
        }
        else
        {
            // Recursive update.
            CommandServer.Get().ProcessCommands(queueId);
        }
    }
}
