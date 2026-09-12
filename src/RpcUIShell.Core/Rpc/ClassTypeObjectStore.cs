using System.Runtime.CompilerServices;

namespace RpcUIShell.Core;

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
        public object Obj { get; set; }

        public RegisteredObject(object obj)
        {
            Obj = obj;
        }
    }

    private readonly object _lock = new();
    private readonly Dictionary<string, RegisteredObject> _objects = [];
    private readonly Dictionary<object, string> _ids = new(new ObjectComparer());

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

    public void UnregisterObject(ObjectId id)
    {
        lock (_lock)
        {
            if (!_objects.TryGetValue(id.Id, out RegisteredObject? robj))
            {
                throw new InvalidOperationException($"Object [{id}] not found.");
            }

            _ = _objects.Remove(id.Id);

            if (!_ids.Remove(robj.Obj, out string? _))
            {
                throw new InvalidOperationException($"Object [{robj.Obj}] not found.");
            }
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
