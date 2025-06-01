using WinUIShell.Common;

namespace WinUIShell;

public class EntranceNavigationTransitionInfo : NavigationTransitionInfo
{
    public EntranceNavigationTransitionInfo()
    {
        Id = CommandClient.Get().CreateObject(
            "Microsoft.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo, Microsoft.WinUI",
            this);
    }
}
