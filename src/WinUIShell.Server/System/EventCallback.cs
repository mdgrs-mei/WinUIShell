using System.Diagnostics;
using System.Reflection;
using WinUIShell.Common;
namespace WinUIShell.Server;

internal static class EventCallback
{
    public static void Add(
        object target,
        string eventName,
        string eventArgsTypeName,
        EventCallbackRunspaceMode runspaceMode,
        int mainRunspaceId,
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
            runspaceMode,
            mainRunspaceId,
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
        EventCallbackRunspaceMode runspaceMode,
        int mainRunspaceId,
        string eventListId,
        int eventId,
        object?[]? disabledControlsWhileProcessing)
    {
        return async (object sender, TEventArgs eventArgs) =>
        {
            var parentWindow = WindowStore.Get().EnterEventCallbackAndGetParentWindow(sender);

            DisabledControlsHolder disabledControls = new(disabledControlsWhileProcessing);
            disabledControls.Disable();

            var senderId = ObjectStore.Get().GetId(sender);
            var temporaryQueueId = CommandClient.Get().CreateTemporaryQueueId();
            var processingQueueId = GetProcessingQueueId(runspaceMode, mainRunspaceId);

            Type eventArgsType = typeof(TEventArgs);
            var eventArgsTypeName = (eventArgsType == typeof(object)) ? "WinUIShellObject" : eventArgsType.Name;
            var eventArgsId = CommandClient.Get().CreateObjectWithId(
                temporaryQueueId,
                $"WinUIShell.{eventArgsTypeName}, WinUIShell",
                eventArgs);

            var invokeTask = CommandClient.Get().InvokeMethodWaitAsync(
                temporaryQueueId,
                new ObjectId(eventListId),
                "Invoke",
                eventId,
                senderId,
                eventArgsId);

            CommandClient.Get().ProcessTemporaryQueue(processingQueueId, temporaryQueueId);

            try
            {
                if (runspaceMode == EventCallbackRunspaceMode.MainRunspaceSyncUI)
                {
                    BlockingWaitTask(invokeTask);
                }
                else
                {
                    await invokeTask;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("EventCallback faild:");
                Debug.WriteLine(e);
                CommandClient.Get().WriteError("EventCallback faild:");
                CommandClient.Get().WriteException(e);
            }

            CommandClient.Get().DestroyObject(processingQueueId, eventArgsId);
            disabledControls.Enable();

            WindowStore.Get().ExitEventCallback(parentWindow);
        };
    }

    public static CommandQueueId GetProcessingQueueId(EventCallbackRunspaceMode runspaceMode, int mainRunspaceId)
    {
        if (runspaceMode == EventCallbackRunspaceMode.RunspacePoolAsyncUI)
        {
            return CommandQueueId.ThreadPool;
        }
        else
        {
            return new CommandQueueId(CommandQueueType.RunspaceId, mainRunspaceId);
        }
    }

    public static void BlockingWaitTask(Task task)
    {
        while (!task.IsCompleted)
        {
            App.ProcessCommands();
            Thread.Sleep(Constants.ServerSyncUICommandPolingIntervalMillisecond);
        }
        App.ProcessCommands();
    }

    private sealed class DisabledControlsHolder
    {
        private readonly List<Microsoft.UI.Xaml.Controls.Control>? _controls;

        public DisabledControlsHolder(object?[]? controls)
        {
            if (controls is null)
                return;

            _controls = [];
            foreach (var obj in controls)
            {
                if (obj is Microsoft.UI.Xaml.Controls.Control control)
                {
                    if (control.IsEnabled)
                    {
                        _controls.Add(control);
                    }
                }
            }
        }

        public void Disable()
        {
            if (_controls is null)
                return;

            foreach (var control in _controls)
            {
                control.IsEnabled = false;
            }
        }

        public void Enable()
        {
            if (_controls is null)
                return;

            foreach (var control in _controls)
            {
                control.IsEnabled = true;
            }
        }
    }
}
