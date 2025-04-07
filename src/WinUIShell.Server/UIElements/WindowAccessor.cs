using Microsoft.UI.Xaml;
using Windows.Foundation;

namespace WinUIShell.Server;
internal static class WindowAccessor
{
    public static void AddClosed(
        Microsoft.UI.Xaml.Window target,
        int queueThreadId,
        string eventListId,
        int eventId,
        object?[]? disabledControlsWhileProcessing)
    {
        var callback = EventCallback.Create<WindowEventArgs>(
            queueThreadId,
            eventListId,
            eventId,
            disabledControlsWhileProcessing);

        target.Closed += new TypedEventHandler<object, WindowEventArgs>(callback);
    }
}
