using WinUIShell.Common;

namespace WinUIShell;

public class NavigationViewItemInvokedEventArgs
{
    public object? InvokedItem
    {
        get
        {
            var id = ObjectStore.Get().GetId(this);
            return PropertyAccessor.Get<object>(id, nameof(InvokedItem));
        }
    }

    public NavigationViewItemBase? InvokedItemContainer
    {
        get
        {
            var id = ObjectStore.Get().GetId(this);
            return PropertyAccessor.Get<NavigationViewItemBase>(id, nameof(InvokedItemContainer));
        }
    }

    public bool IsSettingsInvoked
    {
        get
        {
            var id = ObjectStore.Get().GetId(this);
            return PropertyAccessor.Get<bool>(id, nameof(IsSettingsInvoked))!;
        }
        set
        {
            var id = ObjectStore.Get().GetId(this);
            PropertyAccessor.SetAndWait(id, nameof(IsSettingsInvoked), value);
        }
    }

    public NavigationTransitionInfo RecommendedNavigationTransitionInfo
    {
        get
        {
            var id = ObjectStore.Get().GetId(this);
            return PropertyAccessor.Get<NavigationTransitionInfo>(id, nameof(RecommendedNavigationTransitionInfo))!;
        }
    }
}
