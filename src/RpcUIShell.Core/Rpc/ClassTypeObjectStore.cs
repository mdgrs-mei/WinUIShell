using System.Runtime.CompilerServices;

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

    private sealed class ReferenceComparer : IEqualityComparer<WeakReference<object>>
    {
        public bool Equals(WeakReference<object>? x, WeakReference<object>? y)
        {
            if (x is null && y is null)
                return true;

            if (x is null)
                return false;

            if (y is null)
                return false;

            if (ReferenceEquals(x, y))
                return true;

            if (x.TryGetTarget(out object? xTarget) && y.TryGetTarget(out object? yTarget))
            {
                return ReferenceEquals(xTarget, yTarget);
            }
            else
            {
                return false;
            }
        }
        public int GetHashCode(WeakReference<object> obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }

    private readonly object _lock = new();
    private readonly HashSet<object> _storedReferences = new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<string, RegisteredObject> _objects = [];
    private readonly Dictionary<WeakReference<object>, string> _ids = new(new ReferenceComparer());

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

            if (!_ids.TryAdd(robj.Reference, id.Id))
            {
                throw new InvalidOperationException($"Object [{id}] already exists.");
            }
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

            if (!_ids.TryAdd(robj.Reference, id.Id))
            {
                throw new InvalidOperationException($"Object [{id}] already exists.");
            }
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
            if (robj.Reference.TryGetTarget(out object? obj))
            {
                _ = _storedReferences.Remove(obj);
            }

            if (!_ids.Remove(robj.Reference, out string? _))
            {
                throw new InvalidOperationException($"Object [{robj}] not found.");
            }
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
            if (_ids.TryGetValue(new(obj), out string? id))
            {
                return new ObjectId(id);
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
