using Microsoft.UI.Xaml;
using Windows.Foundation;
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

    public void AddClosed(
        int queueThreadId,
        int eventId,
        object?[]? disabledControlsWhileProcessing)
    {
        var callback = EventCallback.Create<WindowEventArgs>(
            this,
            "OnClosed",
            queueThreadId,
            eventId,
            disabledControlsWhileProcessing);

        Closed += new TypedEventHandler<object, WindowEventArgs>(callback);
    }
}
