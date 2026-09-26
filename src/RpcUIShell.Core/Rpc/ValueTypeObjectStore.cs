namespace RpcUIShell.Core;

internal sealed class ValueTypeObjectStore
{
    private readonly object _lock = new();
    private readonly Dictionary<string, object> _objects = [];
    private readonly Dictionary<object, List<string>> _ids = [];

    public void RegisterObject(object obj, out ObjectId id)
    {
        lock (_lock)
        {
            var newId = ObjectIdGenerator.Get().Generate();
            RegisterObject(newId, obj);
            id = newId;
        }
    }

    public void RegisterObject(ObjectId id, object obj)
    {
        lock (_lock)
        {
            if (!_objects.TryAdd(id.Id, obj))
            {
                throw new InvalidOperationException($"Object [{id}] already exists.");
            }

            if (_ids.TryGetValue(obj, out List<string>? list))
            {
                list.Add(id.Id);
            }
            else
            {
                _ids[obj] = [id.Id];
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
            RemoveId(obj, id);
        }
    }

    private void RemoveId(object obj, ObjectId id)
    {
        if (!_ids.TryGetValue(obj, out List<string>? list))
        {
            throw new InvalidOperationException($"Object [{obj}] not found.");
        }

        if (!list.Remove(id.Id))
        {
            throw new InvalidOperationException($"Object [{id}] not found.");
        }

        if (list.Count == 0)
        {
            _ = _ids.Remove(obj);
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
            if (_ids.TryGetValue(obj, out List<string>? ids))
            {
                return new ObjectId(ids[0]);
            }
            else
            {
                return null;
            }
        }
    }
}
