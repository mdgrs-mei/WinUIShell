using System.Management.Automation;

namespace WinUIShell;

public class EventCallback
{
    private int _isInvoked;
    internal bool IsInvoked
    {
        get => Interlocked.CompareExchange(ref _isInvoked, 0, 0) > 0;
        private set => Interlocked.Exchange(ref _isInvoked, value ? 1 : 0);
    }

    private string _scriptBlockString = "";
    public ScriptBlock? ScriptBlock { get; set; }

    public EventCallbackThreadingMode ThreadingMode { get; set; } = EventCallbackThreadingMode.MainThreadAsyncUI;

    public object? ArgumentList { get; set; }

    public Control[]? DisabledControlsWhileProcessing { get; set; }

    public EventCallback()
    {
    }

    internal EventCallback Copy()
    {
        EventCallback e = (EventCallback)MemberwiseClone();
        if (ThreadingMode == EventCallbackThreadingMode.ThreadPoolAsyncUI && ScriptBlock is not null)
        {
            e._scriptBlockString = ScriptBlock.ToString();
        }
        return e;
    }

    internal void Invoke(object? sender, object? eventArgs)
    {
        if (ScriptBlock is not null)
        {
            if (ThreadingMode == EventCallbackThreadingMode.ThreadPoolAsyncUI)
            {
                CommandWorker.InvokeScriptBlock(
                    _scriptBlockString,
                    ArgumentList,
                    sender,
                    eventArgs);
            }
            else
            {
                _ = ScriptBlock.Invoke(ArgumentList, sender, eventArgs);
            }
        }
        IsInvoked = true;
    }
}
