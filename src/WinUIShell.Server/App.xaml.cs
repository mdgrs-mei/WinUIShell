using System.Diagnostics;
using Microsoft.UI.Dispatching;
using WinUIShell.Common;

namespace WinUIShell.Server;

#pragma warning disable CA1515 // Consider making public types internal
public partial class App : Application
{
    private Process? _parentProcess;
    private string _upstreamPipeName = "";
    private string _downstreamPipeName = "";
    private DispatcherQueueTimer? _updateTimer;

    public App()
    {
        InitializeComponent();
        Init();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _updateTimer = DispatcherQueue.GetForCurrentThread().CreateTimer();
        _updateTimer.Interval = TimeSpan.FromMicroseconds(Constants.ServerCommandPolingIntervalMillisecond);
        _updateTimer.IsRepeating = false;
        _updateTimer.Tick += (sender, eventArgs) =>
        {
            var continueUpdate = Update();
            if (continueUpdate)
            {
                _updateTimer.Start();
            }
        };
        _updateTimer.Start();
    }

    private void Init()
    {
#if USE_UNO
        var framework = Framework.Uno;
#else
        var framework = Framework.WinUI;
#endif

#if DEBUG
        //System.Diagnostics.Debugger.Launch();
        TypeMappingPrinter.Print();
#endif
        DispatcherShutdownMode = DispatcherShutdownMode.OnExplicitShutdown;

        ParseArgs();
        ObjectTypeMapping.Get().SetFramework(framework);
        ObjectStore.Get().SetObjectIdPrefix("s");
        CommandServer.Get().Init(_upstreamPipeName);
        CommandClient.Get().Init(_downstreamPipeName);
        ObjectValidator.Init();
    }

    private void Term()
    {
        ObjectValidator.Term();
        CommandClient.Get().Term();
        CommandServer.Get().Term();
    }

    private void ParseArgs()
    {
        string[] arguments = Environment.GetCommandLineArgs();
        if (arguments.Length != 4)
        {
            throw new ArgumentException($"Invalid arguments {arguments}");
        }
        _upstreamPipeName = arguments[1];
        _downstreamPipeName = arguments[2];

        var parentProcessId = int.Parse(arguments[3]);
        _parentProcess = Process.GetProcessById(parentProcessId);
    }

    private bool ParentProcessExited()
    {
        if (_parentProcess is null)
        {
            return true;
        }
        return _parentProcess.HasExited;
    }

    private bool Update()
    {
        if (ParentProcessExited())
        {
            Term();
            Exit();
            return false;
        }

        ProcessCommands();
        return true;
    }

    public static void ProcessCommands()
    {
        try
        {
            CommandServer.Get().ProcessCommands(CommandQueueId.MainThread);
        }
        catch (Exception e)
        {
            Debug.WriteLine("App.ProcessCommands faild:");
            Debug.WriteLine(e);
            CommandClient.Get().WriteError("App.ProcessCommands faild:");
            CommandClient.Get().WriteException(e);
        }
    }
}
#pragma warning restore CA1515
