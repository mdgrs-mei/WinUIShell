using WinUIShell.Common;

namespace WinUIShell;

public class SlideNavigationTransitionInfo : NavigationTransitionInfo
{
    public SlideNavigationTransitionEffect Effect
    {
        get => PropertyAccessor.Get<SlideNavigationTransitionEffect>(Id, nameof(Effect))!;
        set => PropertyAccessor.Set(Id, nameof(Effect), value);
    }

    internal SlideNavigationTransitionInfo(ObjectId id)
        : base(id)
    {
    }

    public SlideNavigationTransitionInfo()
    {
        Id = CommandClient.Get().CreateObject(
            "Microsoft.UI.Xaml.Media.Animation.SlideNavigationTransitionInfo, Microsoft.WinUI",
            this);
    }
}
