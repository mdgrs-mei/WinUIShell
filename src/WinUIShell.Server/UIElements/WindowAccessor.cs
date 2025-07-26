using Microsoft.UI.Xaml;
using Windows.Foundation;
using WinUIShell.Common;

namespace WinUIShell.Server;
internal static class WindowAccessor
{
    public static void RegisterWindow(
        Window window)
    {
        WindowStore.Get().RegisterWindow(window);

        window.Closed += async (sender, eventArgs) =>
        {
            WindowStore.Get().TerminateWindow(window);
            var id = ObjectStore.Get().GetId(window);
            await CommandClient.Get().SetPropertyAsync(CommandQueueId.Immediate, id, "IsClosed", true);
        };
    }

    public static void AddClosed(
        Window target,
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
