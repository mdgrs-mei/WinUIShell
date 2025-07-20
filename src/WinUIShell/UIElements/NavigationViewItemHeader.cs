using WinUIShell.Common;

namespace WinUIShell;

public class NavigationViewItemHeader : NavigationViewItemBase
{
    public NavigationViewItemHeader()
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<NavigationViewItemHeader>(),
            this);
    }

    internal NavigationViewItemHeader(ObjectId id)
        : base(id)
    {
    }
}
