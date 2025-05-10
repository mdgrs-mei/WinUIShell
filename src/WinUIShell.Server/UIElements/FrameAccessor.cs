using Microsoft.UI.Xaml.Controls;

namespace WinUIShell.Server;
internal static class FrameAccessor
{
    public static bool Navigate(
        Frame frame,
        int queueThreadId,
        string pageName)
    {
        var pageType = PageStore.Get().RegisterPage(pageName, queueThreadId);
        return frame.Navigate(pageType);
    }
}
