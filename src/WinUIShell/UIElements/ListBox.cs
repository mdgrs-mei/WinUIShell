using WinUIShell.Common;

namespace WinUIShell;

public class ListBox : Selector
{
    public WinUIShellObjectList<object> SelectedItems
    {
        get => PropertyAccessor.Get<WinUIShellObjectList<object>>(Id, nameof(SelectedItems))!;
    }

    public Microsoft.UI.Xaml.Controls.SelectionMode SelectionMode
    {
        get => PropertyAccessor.Get<Microsoft.UI.Xaml.Controls.SelectionMode>(Id, nameof(SelectionMode))!;
        set => PropertyAccessor.Set(Id, nameof(SelectionMode), value);
    }

    public bool SingleSelectionFollowsFocus
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(SingleSelectionFollowsFocus))!;
        set => PropertyAccessor.Set(Id, nameof(SingleSelectionFollowsFocus), value);
    }

    public ListBox()
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<ListBox>(),
            this);
    }

    internal ListBox(ObjectId id)
        : base(id)
    {
    }

    public void ScrollIntoView(object item)
    {
        CommandClient.Get().InvokeMethod(
            Id,
            nameof(ScrollIntoView),
            item);
    }

    public void SelectAll()
    {
        CommandClient.Get().InvokeMethod(
            Id,
            nameof(SelectAll));
    }
}
