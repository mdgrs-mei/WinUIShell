using WinUIShell.Common;

namespace WinUIShell;

public class NavigationViewItemHeader : NavigationViewItemBase
{
    public NavigationViewItemHeader()
    {
        Id = CommandClient.Get().CreateObject(
            "Microsoft.UI.Xaml.Controls.NavigationViewItemHeader, Microsoft.WinUI",
            this);
    }
}
