using WinUIShell.Common;
namespace WinUIShell;

internal static class EventCallback
{
    public static Action<object, TEventArgs> Create<TEventArgs>(
        int queueThreadId,
        string eventListId,
        int eventId,
        object?[]? disabledControlsWhileProcessing)
    {
        return async (object sender, TEventArgs eventArgs) =>
        {
            List<Microsoft.UI.Xaml.Controls.Control>? disabledControls = null;
            if (disabledControlsWhileProcessing is not null)
            {
                disabledControls = [];
                foreach (var obj in disabledControlsWhileProcessing)
                {
                    if (obj is Microsoft.UI.Xaml.Controls.Control control)
                    {
                        if (control.IsEnabled)
                        {
                            control.IsEnabled = false;
                            disabledControls.Add(control);
                        }
                    }
                }
            }

            var senderId = ObjectStore.Get().GetId(sender);
            var queueId = new CommandQueueId(queueThreadId);

            var eventArgsId = CommandClient.Get().CreateObject(
                queueId,
                $"WinUIShell.{typeof(TEventArgs).Name}, WinUIShell",
                eventArgs);

            await CommandClient.Get().InvokeMethodWaitAsync(
                queueId,
                new ObjectId(eventListId),
                "Invoke",
                eventId,
                senderId,
                eventArgsId);

            CommandClient.Get().DestroyObject(eventArgsId);

            if (disabledControls is not null)
            {
                foreach (var control in disabledControls)
                {
                    control.IsEnabled = true;
                }
            }
        };
    }
}
