using System.Management.Automation;
using WinUIShell.Common;

namespace WinUIShell;

public class ButtonBase : ContentControl
{
    private readonly EventCallbackList _callbacks = new();

    internal ButtonBase()
    {
    }

    internal ButtonBase(ObjectId id)
        : base(id)
    {
    }

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
        _callbacks.Add(
            Id,
            "Click",
            ObjectTypeMapping.Get().GetTargetTypeName<RoutedEventArgs>(),
            eventCallback);
    }
}
