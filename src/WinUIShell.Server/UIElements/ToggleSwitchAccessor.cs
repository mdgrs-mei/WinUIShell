using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUIShell.Server;
internal static class ToggleSwitchAccessor
{
    public static void AddToggled(
        ToggleSwitch target,
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

        target.Toggled += new RoutedEventHandler(callback);
    }
}
