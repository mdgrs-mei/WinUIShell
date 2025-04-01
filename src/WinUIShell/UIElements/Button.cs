using WinUIShell.Common;

namespace WinUIShell;

public class Button : ButtonBase
{
    public Button()
    {
        Id = CommandClient.Get().CreateObject(
            "WinUIShell.Server.Button, WinUIShell.Server",
            this);
    }
}
