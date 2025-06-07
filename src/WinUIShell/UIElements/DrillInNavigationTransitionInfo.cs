using WinUIShell.Common;

namespace WinUIShell;

public class DrillInNavigationTransitionInfo : NavigationTransitionInfo
{
    public DrillInNavigationTransitionInfo()
    {
        Id = CommandClient.Get().CreateObject(
            "Microsoft.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo, Microsoft.WinUI",
            this);
    }
}
