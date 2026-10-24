namespace RpcUIShell.Core;

public class ObjectStore : Singleton<ObjectStore>
{
    private readonly ClassTypeObjectStore _classTypeObjectStore = new();
    private readonly ValueTypeObjectStore _valueTypeObjectStore = new();

    public void SetObjectIdPrefix(string prefix)
    {
        ObjectIdGenerator.Get().SetPrefix(prefix);
    }

    public void RegisterObject(object obj, out ObjectId id)
    {
        ArgumentNullException.ThrowIfNull(obj);

        var newId = ObjectIdGenerator.Get().Generate();
        RegisterObject(newId, obj);
        id = newId;
    }

    public void RegisterObjectWithType(object obj, string? targetTypeName, out ObjectId id)
    {
        RegisterObject(obj, out id);
        id.Type = targetTypeName ?? "";
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

    public void UnregisterObject(ObjectId id)
    {
        ArgumentNullException.ThrowIfNull(id);

        if (_classTypeObjectStore.FindObject(id) is not null)
        {
            _classTypeObjectStore.UnregisterObject(id);
        }
        else if (_valueTypeObjectStore.FindObject(id) is not null)
        {
            _valueTypeObjectStore.UnregisterObject(id);
        }
        else
        {
            throw new InvalidOperationException($"Object [{id}] not found.");
        }
    }

    public void UnregisterAllObjects()
    {
        _classTypeObjectStore.UnregisterAllObjects();
        _valueTypeObjectStore.UnregisterAllObjects();
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
