using WinUIShell.Common;

namespace WinUIShell;

public class NavigationViewItemSeparator : NavigationViewItemBase
{
    public NavigationViewItemSeparator()
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<NavigationViewItemSeparator>(),
            this);
    }
}
