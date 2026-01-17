using System.Management.Automation.Runspaces;
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

        var copiedEventCallback = eventCallback.Copy();

        int eventId = 0;
        lock (_callbacks)
        {
            eventId = _callbacks.Count;
            _callbacks.Add(copiedEventCallback);
        }

        ObjectId[]? disabledControlIds = copiedEventCallback.GetDisabledControlIds();

        CommandClient.Get().InvokeStaticMethod(
            "WinUIShell.Server.EventCallback, WinUIShell.Server",
            "Add",
            targetObjectId,
            eventName,
            eventArgsTypeName,
            copiedEventCallback.RunspaceMode,
            Runspace.DefaultRunspace.Id,
            Id.Id,
            eventId,
            disabledControlIds);
    }

    public void AddStatic(
        string className,
        string eventName,
        string eventArgsTypeName,
        EventCallback? eventCallback)
    {
        if (eventCallback is null)
            return;

        var copiedEventCallback = eventCallback.Copy();

        int eventId = 0;
        lock (_callbacks)
        {
            eventId = _callbacks.Count;
            _callbacks.Add(copiedEventCallback);
        }

        ObjectId[]? disabledControlIds = copiedEventCallback.GetDisabledControlIds();

        CommandClient.Get().InvokeStaticMethod(
            "WinUIShell.Server.EventCallback, WinUIShell.Server",
            "AddStatic",
            className,
            eventName,
            eventArgsTypeName,
            copiedEventCallback.RunspaceMode,
            Runspace.DefaultRunspace.Id,
            Id.Id,
            eventId,
            disabledControlIds);
    }

    public void Invoke(int eventId, object? sender, object? eventArgs)
    {
        EventCallback? callback = null;
        lock (_callbacks)
        {
            if (eventId < 0 || eventId >= _callbacks.Count)
            {
                return;
            }
            callback = _callbacks[eventId];
        }
        callback.Invoke(sender, eventArgs);
    }

    public bool IsAllInvoked()
    {
        lock (_callbacks)
        {
            foreach (var callback in _callbacks)
            {
                if (!callback.IsInvoked)
                    return false;
            }
            return true;
        }
    }
}
