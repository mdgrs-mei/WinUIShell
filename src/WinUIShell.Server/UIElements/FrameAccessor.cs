using Microsoft.UI.Xaml.Controls;
using WinUIShell.Server.UIElements.Pages;

namespace WinUIShell.Server;
internal static class FrameAccessor
{
    public static bool Navigate(
        Frame frame,
        string pageName)
    {
        _ = pageName;
        return frame.Navigate(typeof(Page01));
    }
}
