using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using WinUIShell.Common;

namespace WinUIShell;

internal sealed class CommandWorker
{
    private string _modulePath = "";
    private string _initializationScript = "";
    private PSHost? _streamingHost;
    private Thread? _thread;
    private bool _stopThread;

    private static readonly ThreadLocal<PowerShell?> _threadLocalPowerShell = new(() => null);

    public CommandWorker()
    {
    }

    public void Start(PSHost? streamingHost, string modulePath, string initializationScript)
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
        var commandQueue = CommandServer.Get().GetThreadPoolCommandQueue();
        lock (commandQueue)
        {
            _stopThread = true;
            Monitor.PulseAll(commandQueue);
        }
        _thread?.Join();
        _thread = null;
    }

    private void ThreadEntry()
    {
        var runspace = RunspaceFactory.CreateRunspace(_streamingHost);
        runspace.ThreadOptions = PSThreadOptions.UseCurrentThread;
        runspace.Open();

        var powershell = PowerShell.Create();
        powershell.Runspace = runspace;
        InitRunspace(powershell);

        _threadLocalPowerShell.Value = powershell;
        ProcessCommands();
        _threadLocalPowerShell.Value = null;

        powershell.Dispose();
        runspace.Close();
        runspace.Dispose();
    }

    private void InitRunspace(PowerShell powershell)
    {
        _ = powershell.AddScript($"Import-Module '{_modulePath}' -ArgumentList $false");
        if (!string.IsNullOrEmpty(_initializationScript))
        {
            _ = powershell.AddScript(_initializationScript);
        }
        _ = powershell.Invoke();
        powershell.Commands.Clear();
    }

    private void ProcessCommands()
    {
        var commandQueue = CommandServer.Get().GetThreadPoolCommandQueue();
        while (true)
        {
            Action? action = null;
            lock (commandQueue)
            {
                if (_stopThread && commandQueue.Count == 0)
                {
                    return;
                }

                if (commandQueue.Count == 0)
                {
                    _ = Monitor.Wait(commandQueue);
                }

                if (commandQueue.Count > 0)
                {
                    action = commandQueue.Dequeue();
                }
            }

            if (action is not null)
            {
                action();
            }
        }
    }

    public static void InvokeEventCallback(
        string scriptBlock,
        object? argumentList,
        object? sender,
        object? eventArgs)
    {
        var powershell = _threadLocalPowerShell.Value;
        ArgumentNullException.ThrowIfNull(powershell);

        _ = powershell.AddScript(scriptBlock);
        _ = powershell.AddArgument(argumentList);
        _ = powershell.AddArgument(sender);
        _ = powershell.AddArgument(eventArgs);
        _ = powershell.Invoke();

        powershell.Commands.Clear();
    }
}
