using WinUIShell.Common;

namespace WinUIShell;

public class NavigationEventArgs : WinUIShellObject
{
    public object? Content
    {
        get => PropertyAccessor.Get<object>(Id, nameof(Content));
    }

    public Microsoft.UI.Xaml.Navigation.NavigationMode NavigationMode
    {
        get => PropertyAccessor.Get<Microsoft.UI.Xaml.Navigation.NavigationMode>(Id, nameof(NavigationMode))!;
    }

    public NavigationTransitionInfo? NavigationTransitionInfo
    {
        get => PropertyAccessor.Get<NavigationTransitionInfo>(Id, nameof(NavigationTransitionInfo));
    }

    //public object Parameter => INavigationEventArgsMethods.get_Parameter(_objRef_global__Microsoft_UI_Xaml_Navigation_INavigationEventArgs);
    //public Type SourcePageType => INavigationEventArgsMethods.get_SourcePageType(_objRef_global__Microsoft_UI_Xaml_Navigation_INavigationEventArgs);
    //public Uri Uri

    internal NavigationEventArgs(ObjectId id)
        : base(id)
    {
    }
}
