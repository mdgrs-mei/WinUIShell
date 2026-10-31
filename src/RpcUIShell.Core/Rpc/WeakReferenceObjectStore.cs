namespace RpcUIShell.Core;

internal sealed class WeakReferenceObjectStore
{
    private readonly object _lock = new();
    private readonly Dictionary<string, WeakReference<object>> _objects = [];

    public void RegisterObject(ObjectId id, object obj)
    {
        lock (_lock)
        {
            if (!_objects.TryAdd(id.Id, new WeakReference<object>(obj)))
            {
                throw new InvalidOperationException($"Object [{id}] already exists.");
            }
        }
    }

    public void UnregisterObject(ObjectId id)
    {
        lock (_lock)
        {
            if (!_objects.Remove(id.Id))
            {
                throw new InvalidOperationException($"Object [{id}] not found.");
            }
        }
    }

    public void UnregisterAllObjects()
    {
        lock (_lock)
        {
            _objects.Clear();
        }
    }

    public bool ContainsObject(ObjectId id)
    {
        lock (_lock)
        {
            return _objects.ContainsKey(id.Id);
        }
    }

    public object? FindObject(ObjectId id)
    {
        lock (_lock)
        {
            if (_objects.TryGetValue(id.Id, out WeakReference<object>? objRef))
            {
                if (objRef.TryGetTarget(out object? obj))
                {
                    return obj;
                }
            }
            return null;
        }
    }

    public ObjectId? FindId(object obj)
    {
        lock (_lock)
        {
            foreach (var it in _objects)
            {
                if (it.Value.TryGetTarget(out object? target) && ReferenceEquals(target, obj))
                {
                    return new ObjectId(it.Key);
                }
            }
            return null;
        }
    }
}
