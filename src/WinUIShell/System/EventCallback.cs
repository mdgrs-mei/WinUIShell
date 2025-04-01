using System.Management.Automation;

namespace WinUIShell;

public class EventCallback
{
    internal bool IsInvoked { get; private set; }

    public ScriptBlock? ScriptBlock { get; set; }

    public object? ArgumentList { get; set; }

    public Control[]? DisabledControlsWhileProcessing { get; set; }

    public EventCallback()
    {
    }

    internal void Invoke(object? sender, object? eventArgs)
    {
        if (ScriptBlock is not null)
        {
            _ = ScriptBlock.Invoke(ArgumentList, sender, eventArgs);
        }
        IsInvoked = true;
    }

    internal void ClearIsInvoked()
    {
        IsInvoked = false;
    }
}
