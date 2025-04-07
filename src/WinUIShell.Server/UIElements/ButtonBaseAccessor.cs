using Microsoft.UI.Xaml;

namespace WinUIShell.Server;
internal static class ButtonBaseAccessor
{
    public static void AddClick(
        Microsoft.UI.Xaml.Controls.Primitives.ButtonBase target,
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
