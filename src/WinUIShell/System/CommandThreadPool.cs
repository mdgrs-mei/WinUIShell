using System.Management.Automation;
using System.Management.Automation.Host;
using WinUIShell.Common;

namespace WinUIShell;

internal sealed class CommandThreadPool
{
    private PSHost? _streamingHost;
    private string _modulePath = "";
    private Thread? _thread;
    private bool _stopThread;
    private readonly CommandWorker[] _workers = new CommandWorker[2];

    public CommandThreadPool()
    {
    }

    public void Init(PSHost? streamingHost, string modulePath)
    {
        _streamingHost = streamingHost;
        _modulePath = modulePath;
        Start("");
    }

    public void Term()
    {
        Stop();
    }

    private void Start(string initializationScript)
    {
        foreach (var worker in _workers)
        {
            worker.Start(_streamingHost, _modulePath, initializationScript);
        }

        _stopThread = false;
        _thread = new Thread(new ThreadStart(ThreadEntry))
        {
            IsBackground = true
        };
        _thread.Start();
    }

    private void Stop()
    {
        var commandQueue = CommandServer.Get().GetThreadPoolCommandQueue();
        lock (commandQueue)
        {
            _stopThread = true;
            Monitor.Pulse(commandQueue);
        }
        _thread?.Join();

        foreach (var worker in _workers)
        {
            worker.Stop();
        }
    }

    public void SetInitializationScript(ScriptBlock? scriptBlock)
    {
        Stop();
        Start(scriptBlock is null ? "" : scriptBlock.ToString());
    }

    private void ThreadEntry()
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
}
