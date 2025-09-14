using System.Management.Automation;
using WinUIShell.Common;

namespace WinUIShell;

public class Selector : ItemsControl
{
    private readonly EventCallbackList _callbacks = new();

    public bool? IsSynchronizedWithCurrentItem
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsSynchronizedWithCurrentItem));
        set => PropertyAccessor.Set(Id, nameof(IsSynchronizedWithCurrentItem), value);
    }

    public int SelectedIndex
    {
        get => PropertyAccessor.Get<int>(Id, nameof(SelectedIndex))!;
        set => PropertyAccessor.Set(Id, nameof(SelectedIndex), value);
    }

    public object? SelectedItem
    {
        get => PropertyAccessor.Get<object>(Id, nameof(SelectedItem));
        set
        {
            if (value is WinUIShellObject v)
            {
                PropertyAccessor.Set(Id, nameof(SelectedItem), v.Id);
            }
            else
            {
                PropertyAccessor.Set(Id, nameof(SelectedItem), value);
            }
        }
    }

    public object? SelectedValue
    {
        get => PropertyAccessor.Get<object>(Id, nameof(SelectedValue));
        set
        {
            if (value is WinUIShellObject v)
            {
                PropertyAccessor.Set(Id, nameof(SelectedValue), v.Id);
            }
            else
            {
                PropertyAccessor.Set(Id, nameof(SelectedValue), value);
            }
        }
    }

    public string SelectedValuePath
    {
        get => PropertyAccessor.Get<string>(Id, nameof(SelectedValuePath))!;
        set => PropertyAccessor.Set(Id, nameof(SelectedValuePath), value);
    }

    internal Selector()
    {
    }

    internal Selector(ObjectId id)
        : base(id)
    {
    }

    public void AddSelectionChanged(ScriptBlock scriptBlock, object? argumentList = null)
    {
        AddSelectionChanged(new EventCallback
        {
            ScriptBlock = scriptBlock,
            ArgumentList = argumentList
        });
    }
    public void AddSelectionChanged(EventCallback eventCallback)
    {
        _callbacks.Add(
            Id,
            "SelectionChanged",
            ObjectTypeMapping.Get().GetTargetTypeName<SelectionChangedEventArgs>(),
            eventCallback);
    }
}
