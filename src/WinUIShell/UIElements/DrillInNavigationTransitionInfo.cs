using WinUIShell.Common;

namespace WinUIShell;

public class DrillInNavigationTransitionInfo : NavigationTransitionInfo
{
    internal DrillInNavigationTransitionInfo(ObjectId id)
    : base(id)
    {
    }

    public DrillInNavigationTransitionInfo()
    {
        Id = CommandClient.Get().CreateObject(
            "Microsoft.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo, Microsoft.WinUI",
            this);
    }
}
