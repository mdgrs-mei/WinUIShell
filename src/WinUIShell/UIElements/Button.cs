using WinUIShell.Common;

namespace WinUIShell;

public class Button : ButtonBase
{
    public Button()
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<Button>(),
            this);
    }
}
