using Microsoft.UI.Xaml;
using WinUIShell.Common;

namespace WinUIShell.Server;

internal static class ObjectValidator
{
    // In Windows App SDK 1.7, app crashes for some objects (Window.SystemBackdrop, ProgressBar, etc.) at object access after closing the window.
    // Skip any property or method access if XamlRoot is closed.
    public static void Init()
    {
        Invoker.Get().Validator = IsValid;
    }

    public static void Term()
    {
        Invoker.Get().Validator = null;
    }

    public static bool IsValid(object obj)
    {
        if (obj is UIElement uiElement)
        {
            if (uiElement.XamlRoot is null)
                // This element is not added or shown in window.
                return true;
            if (uiElement.XamlRoot.ContentIsland is null)
                // XamlRoot is terminated.
                return false;
            if (WindowStore.Get().GetParentWindow(uiElement) is null)
                // Window is Terminated.
                return false;
            return true;
        }
        else
        if (obj is Window window)
        {
            var property = WindowStore.Get().GetWindowProperty(window);
            if (property.IsTerminated)
                return false;
        }
        return true;
    }
}
