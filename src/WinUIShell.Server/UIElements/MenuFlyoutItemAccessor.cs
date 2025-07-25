using Microsoft.UI.Xaml;

namespace WinUIShell.Server;
internal static class MenuFlyoutItemAccessor
{
    public static void AddClick(
        Microsoft.UI.Xaml.Controls.MenuFlyoutItem target,
        int queueThreadId,
        string eventListId,
        int eventId,
        object?[]? disabledControlsWhileProcessing)
    {
        var callback = EventCallback.Create<RoutedEventArgs>(
            queueThreadId,
            eventListId,
            eventId,
            disabledControlsWhileProcessing);
        target.Click += new RoutedEventHandler(callback);
    }
}
