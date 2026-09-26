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
    private readonly Dictionary<string, WeakReference<object>> _objects = [];
    private readonly Dictionary<WeakReference<object>, string> _ids = new(new ObjectComparer());

    public void RegisterObject(ObjectId id, object obj)
    {
        lock (_lock)
        {
            if (!_objects.TryAdd(id.Id, new(obj)))
            {
                throw new InvalidOperationException($"Object [{id}] already exists.");
            }
            if (!_ids.TryAdd(new(obj), id.Id))
            {
                throw new InvalidOperationException($"Object [{id}] already exists.");
            }
        }
    }

    public void UnregisterObject(ObjectId id)
    {
        lock (_lock)
        {
            if (!_objects.TryGetValue(id.Id, out WeakReference<object>? robj))
            {
                throw new InvalidOperationException($"Object [{id}] not found.");
            }

            _ = _objects.Remove(id.Id);

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
}
