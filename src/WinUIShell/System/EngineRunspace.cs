using System.Management.Automation;
using System.Management.Automation.Runspaces;
using RpcUIShell.Core;

namespace WinUIShell;

internal sealed class EngineRunspace
{
    private bool _isInUpdate;
    private global::System.Timers.Timer? _eventTimer;
    private PSEventSubscriber? _timerEventSubscriber;

    public bool IsInitialized { get; set; }
    public bool IsMain { get; set; }

    public void Init(bool useTimerEvent, bool isMain)
    {
        if (IsInitialized)
            return;

        if (useTimerEvent)
        {
            InitTimerEvent();
        }

        IsMain = isMain;
        IsInitialized = true;
    }

    public void Term()
    {
        if (!IsInitialized)
            return;

        IsMain = false;
        IsInitialized = false;

        TermTimerEvent();
    }

    private void InitTimerEvent()
    {
        // Register timer event to process the main command queue.
        // The timer event fires when commands are processed on the main runspace or when waiting for user inputs in interactive sessions.
        _eventTimer = new()
        {
            Interval = Constants.ClientTimerEventCommandPolingIntervalMillisecond,
            AutoReset = false,
            Enabled = false
        };

        ScriptBlock action = ScriptBlock.Create(@"
[WinUIShell.Engine]::Get().IdleUpdateRunspace()
$engineUpdateTimer = $Sender
$engineUpdateTimer.Start()
"
        );

        _timerEventSubscriber = Runspace.DefaultRunspace.Events.SubscribeEvent(
            source: _eventTimer,
            eventName: "Elapsed",
            sourceIdentifier: "",
            data: null,
            action: action,
            supportEvent: false,
            forwardEvent: false);

        _eventTimer.Start();
    }

    private void TermTimerEvent()
    {
        if (_eventTimer is null)
            return;

        _eventTimer.Stop();
        Runspace.DefaultRunspace.Events.UnsubscribeEvent(_timerEventSubscriber);
    }

    public void IdleUpdate()
    {
        if (!IsInitialized)
            return;

        // Do not run commands inside other event callbacks.
        if (_isInUpdate)
            return;

        ProcessCommands();
    }

    public void Update()
    {
        if (!IsInitialized)
            return;

        if (!_isInUpdate)
        {
            // Root update.
            _isInUpdate = true;
            ProcessCommands();
            _isInUpdate = false;
        }
        else
        {
            // Recursive update.
            ProcessCommands();
        }
    }

    private void ProcessCommands()
    {
        var queueId = new CommandQueueId(CommandQueueType.RunspaceId, Runspace.DefaultRunspace.Id);
        try
        {
            CommandServer.Get().ProcessCommands(queueId);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine("Engine.ProcessCommands faild:");
            Console.Error.WriteLine($"{e.GetType().FullName}: {e.Message}");
            if (e.InnerException is not null)
            {
                Console.Error.WriteLine($"-> {e.InnerException.GetType().FullName}: {e.InnerException.Message}");
            }
        }
    }
}
