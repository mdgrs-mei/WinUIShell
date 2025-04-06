using Microsoft.UI.Xaml;
using Windows.Foundation;

namespace WinUIShell.Server;
internal static class WindowAccessor
{
    public static void AddClosed(
        Microsoft.UI.Xaml.Window target,
        int queueThreadId,
        int eventId,
        object?[]? disabledControlsWhileProcessing)
    {
        var callback = EventCallback.Create<WindowEventArgs>(
            target,
            "OnClosed",
            queueThreadId,
            eventId,
            disabledControlsWhileProcessing);

        target.Closed += new TypedEventHandler<object, WindowEventArgs>(callback);
    }
}
