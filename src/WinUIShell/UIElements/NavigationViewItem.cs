using WinUIShell.Common;

namespace WinUIShell;

public class NavigationViewItem : NavigationViewItemBase
{
    public double CompactPaneLength
    {
        get => PropertyAccessor.Get<double>(Id, nameof(CompactPaneLength))!;
        set => PropertyAccessor.Set(Id, nameof(CompactPaneLength), value);
    }

    public bool HasUnrealizedChildren
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(HasUnrealizedChildren))!;
        set => PropertyAccessor.Set(Id, nameof(HasUnrealizedChildren), value);
    }

    public IconElement? Icon
    {
        get => PropertyAccessor.Get<IconElement>(Id, nameof(Icon));
        set => PropertyAccessor.Set(Id, nameof(Icon), value?.Id);
    }

    //public InfoBadge InfoBadge

    public bool IsChildSelected
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsChildSelected))!;
        set => PropertyAccessor.Set(Id, nameof(IsChildSelected), value);
    }

    public bool IsExpanded
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsExpanded))!;
        set => PropertyAccessor.Set(Id, nameof(IsExpanded), value);
    }

    public WinUIShellObjectList<object> MenuItems
    {
        get => PropertyAccessor.Get<WinUIShellObjectList<object>>(Id, nameof(MenuItems))!;
    }

    //public object MenuItemsSource

    public bool SelectsOnInvoked
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(SelectsOnInvoked))!;
        set => PropertyAccessor.Set(Id, nameof(SelectsOnInvoked), value);
    }

    public NavigationViewItem()
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<NavigationViewItem>(),
            this);
    }

    internal NavigationViewItem(ObjectId id)
        : base(id)
    {
    }
}
