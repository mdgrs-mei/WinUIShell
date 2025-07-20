using System.Management.Automation;
using WinUIShell.Common;

namespace WinUIShell;

public class ToggleSwitch : Control
{
    private const string _accessorClassName = "WinUIShell.Server.ToggleSwitchAccessor, WinUIShell.Server";
    private readonly EventCallbackList _toggledCallbacks = new(_accessorClassName);

    public object? Header
    {
        get => PropertyAccessor.Get<object>(Id, nameof(Header));
        set
        {
            if (value is WinUIShellObject v)
            {
                PropertyAccessor.Set(Id, nameof(Header), v.Id);
            }
            else
            {
                PropertyAccessor.Set(Id, nameof(Header), value);
            }
        }
    }

    public bool IsOn
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsOn))!;
        set => PropertyAccessor.Set(Id, nameof(IsOn), value);
    }

    public object? OffContent
    {
        get => PropertyAccessor.Get<object>(Id, nameof(OffContent));
        set
        {
            if (value is WinUIShellObject v)
            {
                PropertyAccessor.Set(Id, nameof(OffContent), v.Id);
            }
            else
            {
                PropertyAccessor.Set(Id, nameof(OffContent), value);
            }
        }
    }

    public object? OnContent
    {
        get => PropertyAccessor.Get<object>(Id, nameof(OnContent));
        set
        {
            if (value is WinUIShellObject v)
            {
                PropertyAccessor.Set(Id, nameof(OnContent), v.Id);
            }
            else
            {
                PropertyAccessor.Set(Id, nameof(OnContent), value);
            }
        }
    }

    public ToggleSwitch()
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<ToggleSwitch>(),
            this);
    }

    internal ToggleSwitch(ObjectId id)
        : base(id)
    {
    }

    public void AddToggled(ScriptBlock scriptBlock, object? argumentList = null)
    {
        AddToggled(new EventCallback
        {
            ScriptBlock = scriptBlock,
            ArgumentList = argumentList
        });
    }
    public void AddToggled(EventCallback eventCallback)
    {
        _toggledCallbacks.Add(
            nameof(AddToggled),
            Id,
            eventCallback);
    }
}
