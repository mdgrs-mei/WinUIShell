using WinUIShell.Common;

namespace WinUIShell;

public class DesktopAcrylicBackdrop : SystemBackdrop
{
    public DesktopAcrylicBackdrop()
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<DesktopAcrylicBackdrop>(),
            this);
    }

    internal DesktopAcrylicBackdrop(ObjectId id)
        : base(id)
    {
    }
}
