using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace WinUIShell.Server;
internal static class FrameAccessor
{
    public static bool Navigate(
        Frame frame,
        int queueThreadId,
        string pageName,
        NavigationCacheMode navigationCacheMode)
    {
        var pageType = PageStore.Get().RegisterPage(pageName, queueThreadId, navigationCacheMode);
        return frame.Navigate(pageType);
    }
}
