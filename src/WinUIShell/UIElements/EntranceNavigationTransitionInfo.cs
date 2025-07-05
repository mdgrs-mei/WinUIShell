using WinUIShell.Common;

namespace WinUIShell;

public class EntranceNavigationTransitionInfo : NavigationTransitionInfo
{
    internal EntranceNavigationTransitionInfo(ObjectId id)
        : base(id)
    {
    }

    public EntranceNavigationTransitionInfo()
    {
        Id = CommandClient.Get().CreateObject(
            "Microsoft.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo, Microsoft.WinUI",
            this);
    }
}
