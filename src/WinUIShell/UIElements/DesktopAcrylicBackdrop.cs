using WinUIShell.Common;

namespace WinUIShell;

public class DesktopAcrylicBackdrop : SystemBackdrop
{
    public DesktopAcrylicBackdrop()
    {
        Id = CommandClient.Get().CreateObject(
            "Microsoft.UI.Xaml.Media.DesktopAcrylicBackdrop, Microsoft.WinUI",
            this);
    }
}
