using System.Diagnostics;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
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
        Init();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _updateTimer = DispatcherQueue.GetForCurrentThread().CreateTimer();
        _updateTimer.Interval = TimeSpan.FromMicroseconds(16);
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
#if DEBUG
        //System.Diagnostics.Debugger.Launch();
        TypeMappingPrinter.Print();
#endif
        DispatcherShutdownMode = DispatcherShutdownMode.OnExplicitShutdown;

        ParseArgs();
        InitializeComponent();
        ObjectStore.Get().SetObjectIdPrefix("s");
        CommandServer.Get().Init(_upstreamPipeName);
        CommandClient.Get().Init(_downstreamPipeName);

        // In Windows App SDK 1.7, app crashes for some objects (Window.SystemBackdrop, ProgressBar, etc.) at object access after closing the window.
        // Skip any property or method access if XamlRoot is closed.
        Invoker.Get().Validator = obj =>
        {
            if (obj is UIElement uiElement)
            {
                if (uiElement.XamlRoot is null)
                    return true;
                if (uiElement.XamlRoot.ContentIsland is null)
                    return false;
                return true;
            }
            else
            if (obj is Window window)
            {
                var property = WindowStore.Get().GetWindowProperty(window);
                if (property.IsTerminated)
                    return false;
            }
            return true;
        };
    }

    private void Term()
    {
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

        try
        {
            CommandServer.Get().ProcessCommands(CommandQueueId.MainThread);
        }
        catch (Exception e)
        {
            Debug.WriteLine("CommandServer.ProcessCommands faild:");
            Debug.WriteLine(e);

            // Show error on the client.
            CommandClient.Get().WriteError($"{e.GetType().FullName}: {e.Message}");
            if (e.InnerException is not null)
            {
                CommandClient.Get().WriteError($"-> {e.InnerException.GetType().FullName}: {e.InnerException.Message}");
            }
        }
        return true;
    }
}
#pragma warning restore CA1515
