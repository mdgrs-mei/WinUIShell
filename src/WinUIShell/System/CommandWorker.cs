using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;

namespace WinUIShell;

internal sealed class CommandWorker
{
    private string _modulePath = "";
    private string _initializationScript = "";
    private PSHost? _streamingHost;
    private Thread? _thread;
    private Runspace? _runspace;
    private PowerShell? _powershell;
    private readonly Queue<Action> _commands = [];
    private bool _stopThread;

    public CommandWorker()
    {
    }

    public void Start(PSHost streamingHost, string modulePath, string initializationScript)
    {
        _streamingHost = streamingHost;
        _modulePath = modulePath;
        _initializationScript = initializationScript;
        _stopThread = false;

        _thread = new Thread(new ThreadStart(ThreadEntry))
        {
            IsBackground = true
        };
        _thread.Start();
    }

    public void Stop()
    {
        lock (_commands)
        {
            _stopThread = true;
            Monitor.Pulse(_commands);
        }
        _thread?.Join();
    }

    public void SetInitializationScript(ScriptBlock scriptBlock)
    {
        _ = scriptBlock;
    }

    private void ThreadEntry()
    {
        _runspace = RunspaceFactory.CreateRunspace(_streamingHost);
        _runspace.ThreadOptions = PSThreadOptions.UseCurrentThread;
        _runspace.Open();
        Runspace.DefaultRunspace = _runspace;

        _powershell = PowerShell.Create();
        _powershell.Runspace = _runspace;

        _ = _powershell.AddScript($"Import-Module '{_modulePath}'");
        if (!string.IsNullOrEmpty(_initializationScript))
        {
            _ = _powershell.AddScript(_initializationScript);
        }
        _ = _powershell.Invoke();
        _powershell.Commands.Clear();

        ProcessCommands();

        _powershell.Dispose();
        _runspace.Close();
        _runspace.Dispose();
    }

    public void ProcessCommands()
    {
        while (true)
        {
            Action? action = null;
            lock (_commands)
            {
                if (_stopThread && _commands.Count == 0)
                {
                    return;
                }

                if (_commands.Count == 0)
                {
                    _ = Monitor.Wait(_commands);
                }

                if (_commands.Count > 0)
                {
                    action = _commands.Dequeue();
                }
            }

            if (action is not null)
            {
                action();
            }
        }
    }

    public void AddCommand(Action action)
    {
        lock (_commands)
        {
            _commands.Enqueue(action);
            Monitor.Pulse(_commands);
        }
    }
}
