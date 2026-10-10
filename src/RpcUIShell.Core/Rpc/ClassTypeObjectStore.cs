using System.Runtime.CompilerServices;

namespace RpcUIShell.Core;

internal sealed class ClassTypeObjectStore
{
    private sealed class ObjectComparer : IEqualityComparer<WeakReference<object>>
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
    private readonly Dictionary<string, WeakReference<object>> _objects = [];
    private readonly Dictionary<WeakReference<object>, string> _ids = new(new ObjectComparer());

    public void StoreObject(ObjectId id, object obj)
    {
        WeakReference<object> robj = new(obj);

        lock (_lock)
        {
            if (!_objects.TryAdd(id.Id, robj))
            {
                throw new InvalidOperationException($"Object [{id}] already exists.");
            }

            _ = _storedReferences.Add(obj);

            if (!_ids.TryAdd(robj, id.Id))
            {
                throw new InvalidOperationException($"Object [{id}] already exists.");
            }
        }
    }

    public void RegisterObject(ObjectId id, object obj)
    {
        WeakReference<object> robj = new(obj);

        lock (_lock)
        {
            if (!_objects.TryAdd(id.Id, robj))
            {
                throw new InvalidOperationException($"Object [{id}] already exists.");
            }

            // Do not hold the reference so that the object can be garbage collected.

            if (!_ids.TryAdd(robj, id.Id))
            {
                throw new InvalidOperationException($"Object [{id}] already exists.");
            }
        }
    }

    public void RemoveObject(ObjectId id)
    {
        lock (_lock)
        {
            if (!_objects.TryGetValue(id.Id, out WeakReference<object>? robj))
            {
                throw new InvalidOperationException($"Object [{id}] not found.");
            }

            _ = _objects.Remove(id.Id);
            if (robj.TryGetTarget(out object? obj))
            {
                _ = _storedReferences.Remove(obj);
            }

            if (!_ids.Remove(robj, out string? _))
            {
                throw new InvalidOperationException($"Object [{robj}] not found.");
            }
        }
    }

    public object? FindObject(ObjectId id)
    {
        lock (_lock)
        {
            if (_objects.TryGetValue(id.Id, out WeakReference<object>? robj))
            {
                if (robj.TryGetTarget(out object? target))
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
