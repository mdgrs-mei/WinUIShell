namespace RpcUIShell.Core;

public class ObjectStore : Singleton<ObjectStore>
{
    private readonly ClassTypeObjectStore _classTypeObjectStore = new();
    private readonly WeakReferenceObjectStore _weakReferenceObjectStore = new();
    private readonly ValueTypeObjectStore _valueTypeObjectStore = new();

    public void SetObjectIdPrefix(string prefix)
    {
        ObjectIdGenerator.Get().SetPrefix(prefix);
    }

    public void RegisterObject(object obj, out ObjectId id, bool storeReference = true)
    {
        ArgumentNullException.ThrowIfNull(obj);

        var newId = ObjectIdGenerator.Get().Generate();
        RegisterObject(newId, obj, storeReference);
        id = newId;
    }

    public void RegisterObjectWithType(object obj, string? targetTypeName, out ObjectId id)
    {
        RegisterObject(obj, out id);
        id.Type = targetTypeName ?? "";
    }


    public void RegisterObject(ObjectId id, object obj, bool storeReference = true)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(obj);

        if (obj.GetType().IsClass)
        {
            if (storeReference)
            {
                _classTypeObjectStore.RegisterObject(id, obj);
            }
            else
            {
                _weakReferenceObjectStore.RegisterObject(id, obj);
            }
        }
        else
        {
            _valueTypeObjectStore.RegisterObject(id, obj);
        }
    }

    public void UnregisterObject(ObjectId id)
    {
        ArgumentNullException.ThrowIfNull(id);

        if (_classTypeObjectStore.ContainsObject(id))
        {
            _classTypeObjectStore.UnregisterObject(id);
        }
        else if (_weakReferenceObjectStore.ContainsObject(id))
        {
            _weakReferenceObjectStore.UnregisterObject(id);
        }
        else if (_valueTypeObjectStore.ContainsObject(id))
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
        _weakReferenceObjectStore.UnregisterAllObjects();
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
        if (obj is not null)
            return obj;

        obj = _weakReferenceObjectStore.FindObject(id);
        if (obj is not null)
            return obj;

        obj = _valueTypeObjectStore.FindObject(id);
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
            ObjectId? id = _classTypeObjectStore.FindId(obj);
            if (id is not null)
                return id;

            return _weakReferenceObjectStore.FindId(obj);
        }
        else
        {
            return _valueTypeObjectStore.FindId(obj);
        }
    }
}
