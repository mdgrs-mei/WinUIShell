using System.Runtime.CompilerServices;

namespace WinUIShell.Common;

internal sealed class ClassTypeObjectStore
{
    private sealed class ObjectComparer : IEqualityComparer<object>
    {
        public new bool Equals(object? x, object? y)
        {
            return ReferenceEquals(x, y);
        }
        public int GetHashCode(object obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }

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
    private readonly Dictionary<object, string> _ids = new(new ObjectComparer());

    public bool RegisterObject(object obj, out ObjectId id)
    {
        lock (_lock)
        {
            if (_ids.TryGetValue(obj, out string? idValue))
            {
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
            if (!_ids.TryAdd(obj, id.Id))
            {
                throw new InvalidOperationException($"Object [{id}] already exists.");
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
            if (!_ids.Remove(robj.Obj, out string? _))
            {
                throw new InvalidOperationException($"Object [{robj.Obj}] not found.");
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
