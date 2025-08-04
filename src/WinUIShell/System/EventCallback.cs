using System.Management.Automation;

namespace WinUIShell;

public class EventCallback
{
    internal bool IsInvoked { get; private set; }

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
            e.ScriptBlock = ScriptBlock.Create(ScriptBlock.ToString());
        }
        return e;
    }

    internal void Invoke(object? sender, object? eventArgs)
    {
        if (ScriptBlock is not null)
        {
            _ = ScriptBlock.Invoke(ArgumentList, sender, eventArgs);
        }
        IsInvoked = true;
    }
}
