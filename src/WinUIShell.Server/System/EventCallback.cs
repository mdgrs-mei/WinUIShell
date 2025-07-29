using System.Reflection;
using WinUIShell.Common;
namespace WinUIShell.Server;

internal static class EventCallback
{
    public static void Add(
        object target,
        string eventName,
        string eventArgsTypeName,
        int queueThreadId,
        string eventListId,
        int eventId,
        object?[]? disabledControlsWhileProcessing)
    {
        var targetType = target.GetType();

        var eventInfo = targetType.GetEvent(eventName);
        if (eventInfo is null)
        {
            throw new InvalidOperationException($"Event [{eventName}] not found in [{targetType.Name}].");
        }

        var eventArgsType = Type.GetType(eventArgsTypeName);
        if (eventArgsType is null)
        {
            throw new InvalidOperationException($"Type [{eventArgsTypeName}] not found.");
        }

        var callbackCreatorGeneric = typeof(EventCallback).GetMethod("Create", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)!;
        var callbackCreator = callbackCreatorGeneric.MakeGenericMethod(eventArgsType);

        var callback = callbackCreator.Invoke(null, [
            queueThreadId,
            eventListId,
            eventId,
            disabledControlsWhileProcessing])!;

        var callbackType = callback.GetType();
        var callbackTargetProperty = callbackType.GetProperty("Target", BindingFlags.Instance | BindingFlags.Public)!;
        var callbackTarget = callbackTargetProperty.GetValue(callback)!;
        var callbackMethodInfoProperty = callbackType.GetProperty("Method", BindingFlags.Instance | BindingFlags.Public)!;
        var callbackMethodInfo = (MethodInfo)callbackMethodInfoProperty.GetValue(callback)!;

        var handler = Delegate.CreateDelegate(eventInfo.EventHandlerType!, callbackTarget, callbackMethodInfo);
        eventInfo.AddEventHandler(target, handler);
    }

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
