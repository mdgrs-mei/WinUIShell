using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace WinUIShell.Server;
internal static class NavigationViewAccessor
{
    public static void AddItemInvoked(
        NavigationView target,
        int queueThreadId,
        string eventListId,
        int eventId,
        object?[]? disabledControlsWhileProcessing)
    {
        var callback = EventCallback.Create<NavigationViewItemInvokedEventArgs>(
            queueThreadId,
            eventListId,
            eventId,
            disabledControlsWhileProcessing);

        target.ItemInvoked += new TypedEventHandler<NavigationView, NavigationViewItemInvokedEventArgs>(callback);
    }
}
