using System.Diagnostics;
using System.Reflection;

namespace RpcUIShell.Core;

public class EventCallbackBinder<TDisabledControlsHolder> where TDisabledControlsHolder : IDisabledControlsHolder
{
    private static readonly MethodInfo s_callbackCreatorGeneric = typeof(EventCallbackBinder<TDisabledControlsHolder>).GetMethod(
        "Create",
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;

    private readonly string _defaultEventArgsTypeName;
    private readonly IWindowStore _windowStore;
    private readonly Action<Task> _blockingWaitTaskAction;

    public EventCallbackBinder(
        string defaultEventArgsTypeName,
        IWindowStore windowStore,
        Action<Task> blockingWaitTaskAction)
    {
        _defaultEventArgsTypeName = defaultEventArgsTypeName;
        _windowStore = windowStore;
        _blockingWaitTaskAction = blockingWaitTaskAction;
    }

    public void Add(
        object? target,
        Type targetType,
        string eventName,
        string eventArgsTypeName,
        EventCallbackRunspaceMode runspaceMode,
        int mainRunspaceId,
        string eventListId,
        int eventId,
        object?[]? disabledControlsWhileProcessing)
    {
        ArgumentNullException.ThrowIfNull(targetType);

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

        var callbackCreator = s_callbackCreatorGeneric.MakeGenericMethod(eventArgsType);

        var callback = callbackCreator.Invoke(this, [
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

    internal Action<object, TEventArgs> Create<TEventArgs>(
        EventCallbackRunspaceMode runspaceMode,
        int mainRunspaceId,
        string eventListId,
        int eventId,
        object?[]? disabledControlsWhileProcessing)
    {
        return async (sender, eventArgs) =>
        {
            var parentWindow = EnterEventCallbackAndGetParentWindow(sender);

            IDisabledControlsHolder disabledControls = CreateDisabledControlsHolder(disabledControlsWhileProcessing);
            disabledControls.Disable();

            var senderId = ObjectStore.Get().GetId(sender);
            var temporaryQueueId = CommandClient.Get().CreateTemporaryQueueId();
            var processingQueueId = GetProcessingQueueId(runspaceMode, mainRunspaceId);

            Type eventArgsType = typeof(TEventArgs);
            var eventArgsTypeName = (eventArgsType == typeof(object)) ?
                _defaultEventArgsTypeName :
                ObjectTypeMapping.Get().GetTargetTypeName(eventArgsType);

            ObjectId? eventArgsId = null;
            if (eventArgs is not null)
            {
                eventArgsId = CommandClient.Get().CreateObjectWithId(
                    temporaryQueueId,
                    eventArgsTypeName,
                    eventArgs);
            }

            var invokeTask = CommandClient.Get().InvokeMethodWaitAsync(
                temporaryQueueId,
                new ObjectId(eventListId),
                null,
                "Invoke",
                eventId,
                senderId,
                eventArgsId);

            CommandClient.Get().ProcessTemporaryQueue(processingQueueId, temporaryQueueId);

            await WaitEventCallbackAsync(runspaceMode, invokeTask);

            if (eventArgsId is not null)
            {
                CommandClient.Get().DestroyObject(processingQueueId, eventArgsId);
            }
            disabledControls.Enable();

            ExitEventCallback(parentWindow);
        };
    }

    public object? EnterEventCallbackAndGetParentWindow(object sender)
    {
        return _windowStore.EnterEventCallbackAndGetParentWindow(sender);
    }

    public void ExitEventCallback(object? parentWindow)
    {
        _windowStore.ExitEventCallback(parentWindow);
    }

    public IDisabledControlsHolder CreateDisabledControlsHolder(object?[]? controls)
    {
        return TDisabledControlsHolder.Create(controls);
    }

    public CommandQueueId GetProcessingQueueId(EventCallbackRunspaceMode runspaceMode, int mainRunspaceId)
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

    public async Task WaitEventCallbackAsync(EventCallbackRunspaceMode runspaceMode, Task invokeTask)
    {
        try
        {
            if (runspaceMode == EventCallbackRunspaceMode.MainRunspaceSyncUI)
            {
                _blockingWaitTaskAction(invokeTask);
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
    }
}
