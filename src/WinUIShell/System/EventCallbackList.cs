using WinUIShell.Common;

namespace WinUIShell;

internal sealed class EventCallbackList : WinUIShellObject
{
    private readonly List<EventCallback> _callbacks = [];

    public EventCallbackList()
    {
        _ = ObjectStore.Get().RegisterObject(this, out ObjectId id);
        Id = id;
    }

    public void Add(
        ObjectId targetObjectId,
        string eventName,
        string eventArgsTypeName,
        EventCallback? eventCallback)
    {
        if (eventCallback is null)
            return;

        int eventId = 0;
        lock (_callbacks)
        {
            eventId = _callbacks.Count;
            _callbacks.Add(eventCallback);
        }

        ObjectId[]? disabledControlIds = null;
        if (eventCallback.DisabledControlsWhileProcessing is not null)
        {
            disabledControlIds = new ObjectId[eventCallback.DisabledControlsWhileProcessing.Length];
            for (int i = 0; i<eventCallback.DisabledControlsWhileProcessing.Length; ++i)
            {
                disabledControlIds[i] = eventCallback.DisabledControlsWhileProcessing[i].Id;
            }
        }

        CommandClient.Get().InvokeStaticMethod(
            "WinUIShell.Server.EventCallback, WinUIShell.Server",
            "Add",
            targetObjectId,
            eventName,
            eventArgsTypeName,
            Environment.CurrentManagedThreadId,
            Id.Id,
            eventId,
            disabledControlIds);
    }

    public void Invoke(int eventId, object? sender, object? eventArgs)
    {
        if (eventId < 0 || eventId >= _callbacks.Count)
        {
            return;
        }
        _callbacks[eventId].Invoke(sender, eventArgs);
    }

    public void ClearIsInvoked()
    {
        foreach (var callback in _callbacks)
        {
            callback.ClearIsInvoked();
        }
    }

    public bool IsAllInvoked()
    {
        foreach (var callback in _callbacks)
        {
            if (!callback.IsInvoked)
                return false;
        }

        return true;
    }
}
