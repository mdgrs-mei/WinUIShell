using WinUIShell.Common;

namespace WinUIShell;

public class EntranceNavigationTransitionInfo : NavigationTransitionInfo
{
    public EntranceNavigationTransitionInfo()
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<EntranceNavigationTransitionInfo>(),
            this);
    }

    internal EntranceNavigationTransitionInfo(ObjectId id)
        : base(id)
    {
    }
}
