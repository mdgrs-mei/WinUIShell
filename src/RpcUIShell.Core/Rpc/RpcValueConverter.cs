namespace RpcUIShell.Core;

public class RpcValueConverter : Singleton<RpcValueConverter>
{
    public Type? DefaultObjectType { get; set; }

    public T? ConvertRpcValueTo<T>(RpcValue rpcValue)
    {
        if (rpcValue is null)
            return default;

        object? obj = ConvertRpcValueToObject(rpcValue);
        if (obj is null)
            return default;

        if (obj is ObjectId objectId)
        {
            // Newly created object on the server side, and no type mapping was found.
            // Create the object on the client side with the return type. It needs to have a constructor from ObjectId.
            obj = CreateObject(objectId, typeof(T), registerObject: true);
            return (T?)obj;
        }
        else
        {
            try
            {
                return (T?)obj;
            }
            catch (InvalidCastException)
            {
                // 1. An object is not supported in the type mapping, and created on the client side as a base class or an interface type.
                // 2. Access the object as another base or interface type, then the cast will fail.
                // In this case, create a temporary object with the return type which is not registered.
                if (rpcValue.GetObject() is ObjectId existingId)
                {
                    obj = CreateObject(existingId, typeof(T), registerObject: false);
                    return (T?)obj;
                }
                throw new InvalidOperationException($"Failed to cast object of type [{obj.GetType().FullName}] to type [{typeof(T).FullName}].");
            }
        }
    }

    private object? ConvertRpcValueToObject(RpcValue rpcValue)
    {
        if (rpcValue is null)
            return null;

        var enumValue = ConvertRpcValueToEnum(rpcValue);
        if (enumValue is not null)
        {
            return enumValue;
        }

        var value = rpcValue.GetObject();
        if (value is RpcValue[] array)
        {
            return ConvertRpcValueArrayToObjectArray(array);
        }
        else if (value is ObjectId objectId)
        {
            object? obj = ObjectStore.Get().FindObject(objectId);
            if (obj is null)
            {
                // Newly created object on the server side. Create the corresponding object on the client side.
                if (string.IsNullOrEmpty(objectId.Type))
                    return objectId;

                Type? targetType = Type.GetType(objectId.Type);
                if (targetType is null)
                    return objectId;

                obj = CreateObject(objectId, targetType, registerObject: true);
            }

            return obj;
        }
        else
        {
            return value;
        }
    }

    private object? CreateObject(ObjectId objectId, Type type, bool registerObject)
    {
        object? obj;
        if (type == typeof(object))
        {
            if (DefaultObjectType is not null)
            {
                obj = Invoker.Get().CreateObject(DefaultObjectType, [objectId]);
            }
            else
            {
                obj = new object();
            }
        }
        else
        {
            obj = Invoker.Get().CreateObject(type, [objectId]);
        }

        if (registerObject)
        {
            ObjectStore.Get().RegisterObject(objectId, obj);
        }
        return obj;
    }

    private static object? ConvertRpcValueToEnum(RpcValue rpcValue)
    {
        var sourceEnumName = rpcValue.GetEnumTypeName();
        if (sourceEnumName is null)
        {
            return null;
        }

        var value = rpcValue.GetObject();
        if (value is null)
        {
            return null;
        }

        _ = EnumTypeMapping.Get().TryGetValue(sourceEnumName, out string? enumTargetName);
        if (enumTargetName is null)
        {
            throw new InvalidOperationException($"Enum mapping for [{sourceEnumName}] not found.");
        }

        var targetEnumType = Type.GetType(enumTargetName);
        if (targetEnumType == null)
        {
            throw new InvalidOperationException($"Type [{enumTargetName}] not found.");
        }

        return Enum.ToObject(targetEnumType, value);
    }

    public object?[]? ConvertRpcValueArrayToObjectArray(RpcValue[]? rpcArray)
    {
        if (rpcArray is null)
        {
            return null;
        }

        var objectArray = new object?[rpcArray.Length];
        for (int i = 0; i < objectArray.Length; ++i)
        {
            objectArray[i] = ConvertRpcValueTo<object>(rpcArray[i]);
        }
        return objectArray;
    }

    public RpcValue ConvertObjectToRpcValue(object? obj)
    {
        if (RpcValue.IsSupportedType(obj))
        {
            return new RpcValue(obj);
        }
        else
        {
            var valueObjectId = ObjectStore.Get().FindId(obj!);
            if (valueObjectId is not null)
            {
                return new RpcValue(valueObjectId);
            }
            else
            {
                // If the object is not a primitive type or a registered object, register it here.
                // The corresponding object needs to be created on the client side.
                _ = ObjectTypeMapping.Get().TryGetTargetTypeName(obj!.GetType(), out string? targetTypeName);
                ObjectStore.Get().StoreObjectWithType(obj!, targetTypeName, out ObjectId id);
                return new RpcValue(id);
            }
        }
    }

    public RpcValue[]? ConvertObjectArrayToRpcArray(object?[]? objectArray)
    {
        return ConvertObjectArrayToRpcArray((Array?)objectArray);
    }

    public RpcValue[]? ConvertObjectArrayToRpcArray(Array? objectArray)
    {
        if (objectArray is null || objectArray.Length == 0)
        {
            return null;
        }

        var rpcArray = new RpcValue[objectArray.Length];
        for (int i = 0; i < objectArray.Length; ++i)
        {
            rpcArray[i] = ConvertObjectToRpcValue(objectArray.GetValue(i));
        }
        return rpcArray;
    }

}
