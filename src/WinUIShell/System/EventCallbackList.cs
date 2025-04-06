using WinUIShell.Common;

namespace WinUIShell;

internal sealed class EventCallbackList
{
    private readonly List<EventCallback> _callbacks = [];

    public EventCallbackList()
    {
    }

    public void Add(
        string accessorClassName,
        string addMethodName,
        ObjectId id,
        EventCallback? eventCallback)
    {
        if (eventCallback is null)
            return;

        int eventId = _callbacks.Count;
        _callbacks.Add(eventCallback);

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
            accessorClassName,
            addMethodName,
            id,
            Environment.CurrentManagedThreadId,
            eventId,
            disabledControlIds);
    }

    public void Invoke(int eventId, object? eventArgs)
    {
        if (eventId < 0 || eventId >= _callbacks.Count)
        {
            return;
        }
        _callbacks[eventId].Invoke(this, eventArgs);
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
