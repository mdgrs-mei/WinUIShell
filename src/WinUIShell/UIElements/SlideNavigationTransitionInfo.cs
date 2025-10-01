using WinUIShell.Common;

namespace WinUIShell;

public class SlideNavigationTransitionInfo : NavigationTransitionInfo
{
    public Microsoft.UI.Xaml.Media.Animation.SlideNavigationTransitionEffect Effect
    {
        get => PropertyAccessor.Get<Microsoft.UI.Xaml.Media.Animation.SlideNavigationTransitionEffect>(Id, nameof(Effect))!;
        set => PropertyAccessor.Set(Id, nameof(Effect), value);
    }

    public SlideNavigationTransitionInfo()
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<SlideNavigationTransitionInfo>(),
            this);
    }

    internal SlideNavigationTransitionInfo(ObjectId id)
        : base(id)
    {
    }
}
