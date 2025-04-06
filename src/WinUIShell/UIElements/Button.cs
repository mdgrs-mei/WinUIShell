using WinUIShell.Common;

namespace WinUIShell;

public class Button : ButtonBase
{
    public Button()
    {
        Id = CommandClient.Get().CreateObject(
            "Microsoft.UI.Xaml.Controls.Button, Microsoft.WinUI",
            this);
    }
}
