using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Host;
using RpcUIShell.Core;

namespace WinUIShell;

public class Engine
{
    private readonly RunspaceLocal<EngineRunspace> _thisRunspace = new(() => new EngineRunspace());
    private int _remainingMainRunspaceCount = 1;
    private string _upstreamPipeName = "";
    private string _downstreamPipeName = "";
    private Process? _serverProcess;
    private readonly CommandThreadPool _commandThreadPool = new();

    private static readonly Engine _instance = new();
    public static Engine Get()
    {
        return _instance;
    }

    public bool AcquireMainRunspace()
    {
        return Interlocked.Exchange(ref _remainingMainRunspaceCount, 0) > 0;
    }

    public void ReleaseMainRunspace()
    {
        _ = Interlocked.Exchange(ref _remainingMainRunspaceCount, 1);
    }

    public void InitMainRunspace(
        string serverExePath,
        PSHost? streamingHost,
        string modulePath,
        bool useTimerEvent)
    {
        var thisRunspace = _thisRunspace.Value;
        if (thisRunspace.IsInitialized)
            return;

#if DEBUG
        //System.Diagnostics.Debugger.Launch();
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
        InitCommandThreadPool(streamingHost, modulePath);

        thisRunspace.Init(useTimerEvent, isMain: true);
    }

    public void InitSubRunspace(bool useTimerEvent)
    {
        var thisRunspace = _thisRunspace.Value;
        thisRunspace.Init(useTimerEvent, isMain: false);
    }

    public void TermRunspace()
    {
        var thisRunspace = _thisRunspace.Value;
        if (!thisRunspace.IsInitialized)
            return;

        bool isMainRunspace = thisRunspace.IsMain;
        thisRunspace.Term();

        if (isMainRunspace)
        {
            TermCommandThreadPool();
            TermConnection();
            StopServerProcess();
        }
    }

    private void InitPipeNames()
    {
        var processId = Environment.ProcessId.ToString();
        _upstreamPipeName = $"WinUIShell.ClientToServer.{processId}";
        _downstreamPipeName = $"WinUIShell.ServerToClient.{processId}";
    }

    private void StartServerProcess(string path)
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

    private void StopServerProcess()
    {
        if (_serverProcess is null)
            return;

        _serverProcess.Kill();
        _serverProcess = null;
    }

    private void InitConnection()
    {
        ObjectStore.Get().SetObjectIdPrefix("c");
        TypeMappingInitializer.Init();
        RpcValueConverter.Get().DefaultObjectType = typeof(WinUIShellObject);
        CommandServer.Get().Init(_downstreamPipeName);
        CommandClient.Get().Init(_upstreamPipeName);
    }

    private void TermConnection()
    {
        CommandClient.Get().Term();
        CommandServer.Get().Term();
    }

    private void InitCommandThreadPool(PSHost? streamingHost, string modulePath)
    {
        _commandThreadPool.Init(streamingHost, modulePath, Constants.ClientCommandThreadPoolDefaultThreadCount);
    }

    private void TermCommandThreadPool()
    {
        _commandThreadPool.Term();
    }

    public void SetCommandThreadPoolOption(
        uint? threadCount,
        ScriptBlock? initializationScriptBlock,
        object?[]? initializationScriptBlockArgumentList)
    {
        _commandThreadPool.SetOption(
            threadCount,
            initializationScriptBlock,
            initializationScriptBlockArgumentList);
    }

    public void IdleUpdateRunspace()
    {
        var thisRunspace = _thisRunspace.Value;
        thisRunspace.IdleUpdate();
    }

    internal void UpdateRunspace()
    {
        var thisRunspace = _thisRunspace.Value;
        thisRunspace.Update();
    }

    public void RemoveWinUIShellObject(IWinUIShellObject? obj)
    {
        if (obj is null)
            return;

        CommandClient.Get().RemoveObject(obj.WinUIShellObjectId);
    }

    public void RemoveAllWinUIShellObjects()
    {
        CommandClient.Get().RemoveAllObjects();
    }
}
