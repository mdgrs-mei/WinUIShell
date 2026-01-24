namespace WinUIShell.Common;

public class ObjectStore : Singleton<ObjectStore>
{
    private readonly ClassTypeObjectStore _classTypeObjectStore = new();
    private readonly ValueTypeObjectStore _valueTypeObjectStore = new();

    public void SetObjectIdPrefix(string prefix)
    {
        ObjectIdGenerator.Get().SetPrefix(prefix);
    }

    public bool RegisterObject(object? obj, out ObjectId id)
    {
        if (obj is null)
        {
            id = ObjectIdGenerator.Get().Generate();
            return true;
        }

        if (obj.GetType().IsClass)
        {
            return _classTypeObjectStore.RegisterObject(obj, out id);
        }
        else
        {
            return _valueTypeObjectStore.RegisterObject(obj, out id);
        }
    }

    public bool RegisterObjectWithType(object? obj, out ObjectId id)
    {
        bool registered = RegisterObject(obj, out id);
        if (obj is not null)
        {
            _ = ObjectTypeMapping.Get().TryGetTargetTypeName(obj.GetType(), out string? targetTypeName);
            id.Type = targetTypeName ?? "";
        }
        return registered;
    }


    public void RegisterObject(ObjectId id, object obj)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(obj);

        if (obj.GetType().IsClass)
        {
            _classTypeObjectStore.RegisterObject(id, obj);
        }
        else
        {
            _valueTypeObjectStore.RegisterObject(id, obj);
        }
    }

    public bool UnregisterObject(ObjectId id)
    {
        ArgumentNullException.ThrowIfNull(id);

        if (_classTypeObjectStore.FindObject(id) is not null)
        {
            return _classTypeObjectStore.UnregisterObject(id);
        }
        else
        if (_valueTypeObjectStore.FindObject(id) is not null)
        {
            return _valueTypeObjectStore.UnregisterObject(id);
        }
        else
        {
            // Allow unregistering not registered object for null linkedObject cases.
            return false;
        }
    }

    public object GetObject(ObjectId id)
    {
        var obj = FindObject(id);
        if (obj is null)
        {
            throw new InvalidOperationException($"Object [{id}] not found.");
        }
        return obj;
    }

    public object? FindObject(ObjectId id)
    {
        if (id is null)
            return null;

        var obj = _classTypeObjectStore.FindObject(id);
        if (obj is null)
        {
            obj = _valueTypeObjectStore.FindObject(id);
        }
        return obj;
    }

    public ObjectId GetId(object obj)
    {
        var id = FindId(obj);
        if (id is null)
        {
            throw new InvalidOperationException($"Object [{obj}] not found.");
        }
        return id;
    }

    public ObjectId? FindId(object obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        if (obj.GetType().IsClass)
        {
            return _classTypeObjectStore.FindId(obj);
        }
        else
        {
            return _valueTypeObjectStore.FindId(obj);
        }
    }
}
