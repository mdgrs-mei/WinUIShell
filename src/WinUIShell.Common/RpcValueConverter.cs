

namespace WinUIShell.Common;

public static class RpcValueConverter
{
    public static object? ConvertRpcValueToObject(RpcValue rpcValue)
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
        else
        if (value is ObjectId objectId)
        {
            object? obj = ObjectStore.Get().FindObject(objectId);
            if (obj is null)
            {
                // Newly created object on the server side. Create the corresponding object on the client side.
                if (string.IsNullOrEmpty(objectId.Type))
                    return objectId;

                var sourceTypeName = objectId.Type;
                _ = EnumTypeMapping.Get().TryGetValue(sourceTypeName, out string? targetTypeName);
                if (targetTypeName is null)
                    return objectId;

                Type? targetType = Type.GetType(targetTypeName);
                if (targetType is null)
                    return objectId;

                obj = Activator.CreateInstance(
                    targetType,
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public,
                    null,
                    [objectId],
                    null);

                if (obj == null)
                {
                    throw new InvalidOperationException($"Failed to create instance of type [{targetTypeName}].");
                }
                ObjectStore.Get().RegisterObject(objectId, obj);
            }

            return obj;
        }
        else
        {
            return value;
        }
    }

    public static T? ConvertRpcValueTo<T>(RpcValue rpcValue)
    {
        var obj = ConvertRpcValueToObject(rpcValue);
        if (obj is null)
            return default;

        if (obj is ObjectId objectId)
        {
            // Newly created object on the server side. Create the corresponding object on the client side.
            obj = Activator.CreateInstance(
                typeof(T),
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public,
                null,
                [objectId],
                null);

            if (obj == null)
            {
                throw new InvalidOperationException($"Failed to create instance of type [{typeof(T).Name}].");
            }
            ObjectStore.Get().RegisterObject(objectId, obj);
        }

        if (obj is T)
        {
            return (T?)obj;
        }
        else
        {
            // Call constructor for type conversion.
            obj = Activator.CreateInstance(
                typeof(T),
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public,
                null,
                [obj],
                null);

            if (obj == null)
            {
                throw new InvalidOperationException($"Failed to create instance of type [{typeof(T).Name}].");
            }

            return (T?)obj;
        }
    }

    private static object? ConvertRpcValueToEnum(RpcValue rpcValue)
    {
        var value = rpcValue.GetObject();
        if (value is null)
        {
            return null;
        }

        var sourceEnumName = rpcValue.GetEnumTypeName();
        if (sourceEnumName is null)
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

    public static object?[]? ConvertRpcValueArrayToObjectArray(RpcValue[]? rpcArray)
    {
        if (rpcArray is null)
        {
            return null;
        }

        var objectArray = new object?[rpcArray.Length];
        for (int i = 0; i < objectArray.Length; ++i)
        {
            objectArray[i] = ConvertRpcValueToObject(rpcArray[i]);
        }
        return objectArray;
    }

    public static RpcValue ConvertObjectToRpcValue(object? obj)
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
                _ = ObjectStore.Get().RegisterObjectWithType(obj!, out ObjectId id);
                return new RpcValue(id);
            }
        }
    }

    public static RpcValue[]? ConvertObjectArrayToRpcArray(object?[]? objectArray)
    {
        if (objectArray is null || objectArray.Length == 0)
        {
            return null;
        }

        var rpcArray = new RpcValue[objectArray.Length];
        for (int i = 0; i < objectArray.Length; ++i)
        {
            rpcArray[i] = new RpcValue(objectArray[i]);
        }
        return rpcArray;
    }
}
