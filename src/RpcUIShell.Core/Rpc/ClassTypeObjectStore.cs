namespace RpcUIShell.Core;

internal sealed class ClassTypeObjectStore
{
    private sealed class RegisteredObject
    {
        public WeakReference<object> Reference { get; }
        public int ReferenceCount { get; set; } = 1;

        public RegisteredObject(object obj)
        {
            Reference = new WeakReference<object>(obj);
        }
    }

    private readonly object _lock = new();
    private readonly HashSet<object> _storedReferences = new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<string, RegisteredObject> _objects = [];

    public void StoreObject(ObjectId id, object obj)
    {
        RegisteredObject robj = new(obj);

        lock (_lock)
        {
            if (!_objects.TryAdd(id.Id, robj))
            {
                throw new InvalidOperationException($"Object [{id}] already exists.");
            }
            _ = _storedReferences.Add(obj);
        }
    }

    public void RegisterObject(ObjectId id, object obj)
    {
        RegisteredObject robj = new(obj);

        lock (_lock)
        {
            if (!_objects.TryAdd(id.Id, robj))
            {
                throw new InvalidOperationException($"Object [{id}] already exists.");
            }

            // Do not hold the reference so that the object can be garbage collected.
        }
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

    public bool RemoveObject(ObjectId id)
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
                return false;
            }

            _ = _objects.Remove(id.Id);
            if (robj.Reference.TryGetTarget(out object? obj))
            {
                _ = _storedReferences.Remove(obj);
            }
            return true;
        }
    }

    public object? FindObject(ObjectId id)
    {
        lock (_lock)
        {
            if (_objects.TryGetValue(id.Id, out RegisteredObject? robj))
            {
                if (robj.Reference.TryGetTarget(out object? target))
                {
                    return target;
                }
                else
                {
                    throw new InvalidOperationException($"Object [{id}] has been garbage collected.");
                }
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
            foreach (var it in _objects)
            {
                if (it.Value.Reference.TryGetTarget(out object? target) && ReferenceEquals(target, obj))
                {
                    return new ObjectId(it.Key);
                }
            }
            return null;
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
