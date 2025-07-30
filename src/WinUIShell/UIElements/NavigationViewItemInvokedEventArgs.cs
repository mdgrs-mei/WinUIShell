using WinUIShell.Common;

namespace WinUIShell;

public class NavigationViewItemInvokedEventArgs : WinUIShellObject
{
    public object? InvokedItem
    {
        get => PropertyAccessor.Get<object>(Id, nameof(InvokedItem));
    }

    public NavigationViewItemBase? InvokedItemContainer
    {
        get => PropertyAccessor.Get<NavigationViewItemBase>(Id, nameof(InvokedItemContainer));
    }

    public bool IsSettingsInvoked
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsSettingsInvoked))!;
        set => PropertyAccessor.SetAndWait(Id, nameof(IsSettingsInvoked), value);
    }

    public NavigationTransitionInfo RecommendedNavigationTransitionInfo
    {
        get => PropertyAccessor.Get<NavigationTransitionInfo>(Id, nameof(RecommendedNavigationTransitionInfo))!;
    }

    internal NavigationViewItemInvokedEventArgs(ObjectId id)
        : base(id)
    {
    }
}
