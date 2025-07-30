using Microsoft.UI.Xaml;
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
}
