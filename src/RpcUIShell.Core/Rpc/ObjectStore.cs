namespace RpcUIShell.Core;

public class ObjectStore : Singleton<ObjectStore>
{
    private readonly ClassTypeObjectStore _classTypeObjectStore = new();
    private readonly ValueTypeObjectStore _valueTypeObjectStore = new();

    public void SetObjectIdPrefix(string prefix)
    {
        ObjectIdGenerator.Get().SetPrefix(prefix);
    }

    public void StoreObjectWithType(object obj, string? targetTypeName, out ObjectId id)
    {
        var newId = ObjectIdGenerator.Get().Generate();
        StoreObject(newId, obj);
        newId.Type = targetTypeName ?? "";
        id = newId;
    }

    // Add object and hold the reference.
    public void StoreObject(ObjectId id, object obj)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(obj);

        if (obj.GetType().IsClass)
        {
            _classTypeObjectStore.StoreObject(id, obj);
        }
        else
        {
            _valueTypeObjectStore.StoreObject(id, obj);
        }
    }

    // Add object but do not hold the reference.
    public void RegisterObject(object obj, out ObjectId id)
    {
        var newId = ObjectIdGenerator.Get().Generate();
        RegisterObject(newId, obj);
        id = newId;
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

    public void AddReference(ObjectId id)
    {
        ArgumentNullException.ThrowIfNull(id);

        if (_classTypeObjectStore.ContainsObject(id))
        {
            _classTypeObjectStore.AddReference(id);
        }
        else if (_valueTypeObjectStore.ContainsObject(id))
        {
            _valueTypeObjectStore.AddReference(id);
        }
        else
        {
            throw new InvalidOperationException($"Object [{id}] not found.");
        }
    }

    public bool RemoveObject(ObjectId id)
    {
        ArgumentNullException.ThrowIfNull(id);

        if (_classTypeObjectStore.ContainsObject(id))
        {
            return _classTypeObjectStore.RemoveObject(id);
        }
        else if (_valueTypeObjectStore.ContainsObject(id))
        {
            return _valueTypeObjectStore.RemoveObject(id);
        }
        else
        {
            throw new InvalidOperationException($"Object [{id}] not found.");
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
