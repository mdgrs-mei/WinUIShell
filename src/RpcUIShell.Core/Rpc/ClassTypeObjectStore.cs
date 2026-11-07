namespace RpcUIShell.Core;

internal sealed class ClassTypeObjectStore
{
    private readonly object _lock = new();
    private readonly Dictionary<string, object> _objects = [];
    private readonly Dictionary<object, string> _ids = new(ReferenceEqualityComparer.Instance);

    public void RegisterObject(ObjectId id, object obj)
    {
        lock (_lock)
        {
            if (!_objects.TryAdd(id.Id, obj))
            {
                throw new InvalidOperationException($"Object [{id}] already exists.");
            }
            if (!_ids.TryAdd(obj, id.Id))
            {
                throw new InvalidOperationException($"Object [{id}] already exists.");
            }
        }
    }

    public void UnregisterObject(ObjectId id)
    {
        lock (_lock)
        {
            if (!_objects.TryGetValue(id.Id, out object? obj))
            {
                throw new InvalidOperationException($"Object [{id}] not found.");
            }

            _ = _objects.Remove(id.Id);

            if (!_ids.Remove(obj, out string? _))
            {
                throw new InvalidOperationException($"Object [{obj}] not found.");
            }
        }
    }

    public void UnregisterAllObjects()
    {
        lock (_lock)
        {
            _objects.Clear();
            _ids.Clear();
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
            if (_objects.TryGetValue(id.Id, out object? obj))
            {
                return obj;
            }
            else
            {
                return null;
            }
        }
    }

    public ObjectId? FindId(object obj)
    {
        lock (_lock)
        {
            if (_ids.TryGetValue(obj, out string? id))
            {
                return new ObjectId(id);
            }
            else
            {
                return null;
            }
        }
    }
}
