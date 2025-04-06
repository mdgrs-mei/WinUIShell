using WinUIShell.Common;

namespace WinUIShell.Server;
internal sealed class Window : Microsoft.UI.Xaml.Window
{
    public bool IsTerminated { get; private set; }

    public Window()
    {
        Closed += async (sender, eventArgs) =>
        {
            IsTerminated = true;
            var id = ObjectStore.Get().GetId(this);
            await CommandClient.Get().SetPropertyAsync(CommandQueueId.Immediate, id, "IsClosed", true);
        };
    }
}
