namespace RpcUIShell.Core;

internal sealed class ValueTypeObjectStore
{
    private sealed class RegisteredObject
    {
        public object Obj { get; }
        public int ReferenceCount { get; set; } = 1;

        public RegisteredObject(object obj)
        {
            Obj = obj;
        }
    }

    private readonly object _lock = new();
    private readonly Dictionary<string, RegisteredObject> _objects = [];
    private readonly Dictionary<object, List<string>> _ids = [];

    public void StoreObject(ObjectId id, object obj)
    {
        RegisteredObject robj = new(obj);

        lock (_lock)
        {
            if (!_objects.TryAdd(id.Id, robj))
            {
                throw new InvalidOperationException($"Object [{id}] already exists.");
            }

            if (_ids.TryGetValue(robj.Obj, out List<string>? list))
            {
                list.Add(id.Id);
            }
            else
            {
                _ids[obj] = [id.Id];
            }
        }
    }

    public void RegisterObject(ObjectId id, object obj)
    {
        StoreObject(id, obj);
    }

    public void AddReference(ObjectId id)
    {
        lock (_lock)
        {
            if (!_objects.TryGetValue(id.Id, out RegisteredObject? robj))
            {
                throw new InvalidOperationException($"Object [{id}] not found.");
            }
            robj.ReferenceCount++;
        }
    }

    public void RemoveObject(ObjectId id)
    {
        lock (_lock)
        {
            if (!_objects.TryGetValue(id.Id, out RegisteredObject? robj))
            {
                throw new InvalidOperationException($"Object [{id}] not found.");
            }

            robj.ReferenceCount--;
            if (robj.ReferenceCount > 0)
            {
                return;
            }

            _ = _objects.Remove(id.Id);
            RemoveId(robj.Obj, id);
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
            if (_objects.TryGetValue(id.Id, out RegisteredObject? robj))
            {
                return robj.Obj;
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

    public bool ContainsObject(ObjectId id)
    {
        lock (_lock)
        {
            return _objects.ContainsKey(id.Id);
        }
    }
}
