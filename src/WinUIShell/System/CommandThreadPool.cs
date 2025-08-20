using System.Management.Automation;
using System.Management.Automation.Host;
using WinUIShell.Common;

namespace WinUIShell;

internal sealed class CommandThreadPool
{
    private PSHost? _streamingHost;
    private string _modulePath = "";
    private CommandWorker[]? _workers;

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
        if (_workers is not null)
            return;

        _workers = new CommandWorker[Constants.ClientCommandThreadPoolDefaultThreadCount];
        for (int i = 0; i < _workers.Length; ++i)
        {
            var worker = new CommandWorker();
            worker.Start(_streamingHost, _modulePath, initializationScript);
            _workers[i] = worker;
        }
    }

    private void Stop()
    {
        if (_workers is null)
            return;

        foreach (var worker in _workers)
        {
            worker.Stop();
        }
        _workers = null;
    }

    public void SetInitializationScript(ScriptBlock? scriptBlock)
    {
        Stop();
        Start(scriptBlock is null ? "" : scriptBlock.ToString());
    }
}
