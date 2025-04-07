using System.Management.Automation;

namespace WinUIShell;

public abstract class ButtonBase : ContentControl
{
    private const string _accessorClassName = "WinUIShell.Server.ButtonBaseAccessor, WinUIShell.Server";
    private readonly EventCallbackList _clickCallbacks = new(_accessorClassName);

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
            nameof(AddClick),
            Id,
            eventCallback);
    }
}
