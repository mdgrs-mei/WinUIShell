using WinUIShell.Common;

namespace WinUIShell;

public class NavigationEventArgs
{
    public object? Content
    {
        get
        {
            var id = ObjectStore.Get().GetId(this);
            return PropertyAccessor.Get<object>(id, nameof(Content));
        }
    }

    public NavigationMode NavigationMode
    {
        get
        {
            var id = ObjectStore.Get().GetId(this);
            return PropertyAccessor.Get<NavigationMode>(id, nameof(NavigationMode))!;
        }
    }

    public NavigationTransitionInfo? NavigationTransitionInfo
    {
        get
        {
            var id = ObjectStore.Get().GetId(this);
            return PropertyAccessor.Get<NavigationTransitionInfo>(id, nameof(NavigationTransitionInfo));
        }
    }

    //public object Parameter => INavigationEventArgsMethods.get_Parameter(_objRef_global__Microsoft_UI_Xaml_Navigation_INavigationEventArgs);
    //public Type SourcePageType => INavigationEventArgsMethods.get_SourcePageType(_objRef_global__Microsoft_UI_Xaml_Navigation_INavigationEventArgs);
    //public Uri Uri
}
