using Microsoft.UI.Xaml;
using WinUIShell.Common;

namespace WinUIShell.Server;
internal static class WindowAccessor
{
    public static void RegisterWindow(
        Window window)
    {
        var windowStore = WindowStore.Get();
        windowStore.RegisterWindow(window);

        window.Closed += async (sender, eventArgs) =>
        {
            var id = ObjectStore.Get().GetId(window);

            // Make sure that all running event callbacks including ones processed on the Runspace pool are completed
            // so that Window.WaitForClosed() can wait for all background event callbacks associated with the window.
            await windowStore.WaitForAllChildEventCallbacksFinishedAsync(window);
            await CommandClient.Get().SetPropertyAsync(CommandQueueId.Immediate, id, "IsClosed", true);

            windowStore.TerminateWindow(window);
        };
    }
}
