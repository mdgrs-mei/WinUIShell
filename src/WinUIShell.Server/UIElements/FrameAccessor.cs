using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;

namespace WinUIShell.Server;
internal static class FrameAccessor
{
    public static bool Navigate(
        Frame frame,
        EventCallbackThreadingMode onLoadedCallbackThreadingMode,
        int onLoadedCallbackMainThreadId,
        string pageName,
        NavigationTransitionInfo? transitionOverride,
        NavigationCacheMode navigationCacheMode)
    {
        var pageType = PageStore.Get().RegisterPageProperty(
            pageName,
            onLoadedCallbackThreadingMode,
            onLoadedCallbackMainThreadId,
            navigationCacheMode);
        return frame.Navigate(pageType, null, transitionOverride);
    }

    public static string GetSourcePageName(
        Frame frame)
    {
        if (frame.SourcePageType is null)
        {
            return "";
        }

        var property = PageStore.Get().GetPageProperty(frame.SourcePageType);
        return property.Name;
    }
}
