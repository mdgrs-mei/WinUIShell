using WinUIShell.Common;
namespace WinUIShell;

internal static class EventCallback
{
    public static Action<object, TEventArgs> Create<TEventArgs>(
        object parentObjectAddress,
        string callbackName,
        int queueThreadId,
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

            var id = ObjectStore.Get().GetId(parentObjectAddress);
            var queueId = new CommandQueueId(queueThreadId);

            var eventArgsId = CommandClient.Get().CreateObject(
                queueId,
                $"WinUIShell.{typeof(TEventArgs).Name}, WinUIShell",
                eventArgs);
            await CommandClient.Get().InvokeMethodWaitAsync(queueId, id, callbackName, eventId, eventArgsId);
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
