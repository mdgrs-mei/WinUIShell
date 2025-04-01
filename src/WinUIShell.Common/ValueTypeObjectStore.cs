namespace WinUIShell.Common;

internal sealed class ValueTypeObjectStore
{
    private sealed class RegisteredObject
    {
        public int RegisterCount { get; set; }
        public object Obj { get; set; }

        public RegisteredObject(object obj)
        {
            RegisterCount = 1;
            Obj = obj;
        }
    }

    private readonly object _lock = new();
    private readonly Dictionary<string, RegisteredObject> _objects = [];
    private readonly Dictionary<object, List<string>> _ids = [];

    public bool RegisterObject(object obj, out ObjectId id)
    {
        lock (_lock)
        {
            if (_ids.TryGetValue(obj, out List<string>? list))
            {
                var idValue = list[0];
                _objects[idValue].RegisterCount++;
                id = new ObjectId(idValue);
                return false;
            }
            else
            {
                var newId = ObjectIdGenerator.Get().Generate();
                RegisterObject(newId, obj);
                id = newId;
                return true;
            }
        }
    }

    public void RegisterObject(ObjectId id, object obj)
    {
        lock (_lock)
        {
            if (!_objects.TryAdd(id.Id, new(obj)))
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

    public bool UnregisterObject(ObjectId id)
    {
        lock (_lock)
        {
            if (!_objects.TryGetValue(id.Id, out RegisteredObject? robj))
            {
                throw new InvalidOperationException($"Object [{id}] not found.");
            }

            robj.RegisterCount--;
            if (robj.RegisterCount > 0)
                return false;

            _ = _objects.Remove(id.Id);
            RemoveId(robj.Obj, id);
            return true;
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
}
