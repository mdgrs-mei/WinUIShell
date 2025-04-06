using System.Management.Automation;
using WinUIShell.Common;

namespace WinUIShell;

public class ToggleSwitch : Control
{
    private readonly EventCallbackList _toggledCallbacks = new();

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
            "Microsoft.UI.Xaml.Controls.ToggleSwitch, Microsoft.WinUI",
            this);
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
            "WinUIShell.Server.ToggleSwitchAccessor, WinUIShell.Server",
            nameof(AddToggled),
            Id,
            eventCallback);
    }
}
