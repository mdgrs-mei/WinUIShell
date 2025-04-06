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
        _clickCallbacks.Add(
            "WinUIShell.Server.ButtonBaseAccessor, WinUIShell.Server",
            nameof(AddClick),
            Id,
            eventCallback);
    }
}
