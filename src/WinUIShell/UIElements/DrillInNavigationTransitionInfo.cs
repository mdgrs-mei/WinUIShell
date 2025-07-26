using WinUIShell.Common;

namespace WinUIShell;

public class DrillInNavigationTransitionInfo : NavigationTransitionInfo
{
    public DrillInNavigationTransitionInfo()
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<DrillInNavigationTransitionInfo>(),
            this);
    }

    internal DrillInNavigationTransitionInfo(ObjectId id)
        : base(id)
    {
    }
}
