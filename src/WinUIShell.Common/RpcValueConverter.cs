namespace WinUIShell.Common;

public static class RpcValueConverter
{
    public static T? ConvertRpcValueTo<T>(RpcValue rpcValue)
    {
        var obj = ConvertRpcValueToObject(rpcValue);
        if (obj is null)
            return default;

        if (obj is ObjectId objectId)
        {
            // Newly created object on the server side, and no type mapping was found.
            // Create the object on the client side with the return type. It needs to have a constructor from ObjectId.
            Type createType = typeof(T);
            if (createType == typeof(object))
            {
                throw new InvalidOperationException($"Object not found or unsupported object type. Id:[{objectId.Id}], Type:[{objectId.Type}].");
            }
            else if (createType.IsInterface)
            {
                var interfaceImplType = GetInterfaceImplType(createType);
                if (interfaceImplType is null)
                {
                    throw new InvalidOperationException($"Unsupported interface type [{createType.FullName}]. Id:[{objectId.Id}], Type:[{objectId.Type}].");
                }
                createType = interfaceImplType;
            }

            obj = Activator.CreateInstance(
                createType,
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public,
                null,
                [objectId],
                null);

            if (obj == null)
            {
                throw new InvalidOperationException($"Failed to create instance of type [{createType.FullName}].");
            }
            ObjectStore.Get().RegisterObject(objectId, obj);
        }

        return (T?)obj;
    }

    private static object? ConvertRpcValueToObject(RpcValue rpcValue)
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

                obj = Activator.CreateInstance(
                    targetType,
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public,
                    null,
                    [objectId],
                    null);

                if (obj == null)
                {
                    throw new InvalidOperationException($"Failed to create instance of type [{objectId.Type}].");
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

    private static Type? GetInterfaceImplType(Type interfaceType)
    {
        // Get interface Impl type fullname from interface type fullname.
        // fullName has a format like "WinUIShell.Namespace.Class`1+InnerClass+InnerMost`2[[WinUIShell.Namespace.GenericArgumentClass, WinUIShell, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]".
        var fullName = interfaceType.FullName!;

        // System interface types don't have "WinUIShell" namespace. Add it here as Impl classes are always under WinUIShell namespace.
        if (!fullName.StartsWith("WinUIShell.", StringComparison.Ordinal))
        {
            fullName = "WinUIShell." + fullName;
        }

        int insertIndex = fullName.Length;
        int firstGenericArgumentSeparator = fullName.IndexOf('[', StringComparison.Ordinal);
        if (firstGenericArgumentSeparator >= 0)
        {
            insertIndex = firstGenericArgumentSeparator;
        }

        int lastNestedClassSeparator = fullName.LastIndexOf('+', insertIndex - 1);
        int lastGenericTypeSeparator = fullName.LastIndexOf('`', insertIndex - 1);
        if (lastNestedClassSeparator >= 0)
        {
            if (lastNestedClassSeparator < lastGenericTypeSeparator)
            {
                insertIndex = lastGenericTypeSeparator;
            }
        }
        else if (lastGenericTypeSeparator >= 0)
        {
            insertIndex = lastGenericTypeSeparator;
        }

        string implTypeFullName = $"{fullName.Insert(insertIndex, "Impl")}, WinUIShell";
        return Type.GetType(implTypeFullName);
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

    public static object?[]? ConvertRpcValueArrayToObjectArray(RpcValue[]? rpcArray)
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
                _ = ObjectTypeMapping.Get().TryGetTargetTypeName(obj!.GetType(), out string? targetTypeName);
                _ = ObjectStore.Get().RegisterObjectWithType(obj!, targetTypeName, out ObjectId id);
                return new RpcValue(id);
            }
        }
    }

    public static RpcValue[]? ConvertObjectArrayToRpcArray(object?[]? objectArray)
    {
        return ConvertObjectArrayToRpcArray((Array?)objectArray);
    }

    public static RpcValue[]? ConvertObjectArrayToRpcArray(Array? objectArray)
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
