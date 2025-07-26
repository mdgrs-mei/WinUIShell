using WinUIShell.Common;

namespace WinUIShell;

public class MenuFlyout : FlyoutBase
{
    public WinUIShellObjectList<MenuFlyoutItemBase> Items
    {
        get => PropertyAccessor.Get<WinUIShellObjectList<MenuFlyoutItemBase>>(Id, nameof(Items))!;
    }

    public MenuFlyout()
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<MenuFlyout>(),
            this);
    }

    internal MenuFlyout(ObjectId id)
        : base(id)
    {
    }
}
