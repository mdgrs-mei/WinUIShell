using System.Management.Automation;

namespace WinUIShell;

public abstract class ButtonBase : ContentControl
{
    private readonly EventCallbackList _clickCallbacks = new();

    public void AddClick(ScriptBlock scriptBlock, object? argumentList = null)
    {
        AddClick(new EventCallback
        {
            ScriptBlock = scriptBlock,
            ArgumentList = argumentList
        });
    }
    public void AddClick(EventCallback eventCallback)
    {
        _clickCallbacks.Add(eventCallback, Id, nameof(AddClick));
    }

    internal void OnClick(int eventId, RoutedEventArgs eventArgs)
    {
        _clickCallbacks.Invoke(eventId, eventArgs);
    }
}
