namespace WinUIShell.Common;

internal sealed class RpcService
{
    private readonly CommandServer _commandServer;

    public RpcService(CommandServer commandServer)
    {
        _commandServer = commandServer;
    }

    public void CreateObject(CommandQueueId queueId, ObjectId id, string typeName, RpcValue[]? rpcArguments)
    {
        _commandServer.AddCommand(
            queueId,
            () =>
            {
                var arguments = ConvertArguments(rpcArguments);
                var obj = Invoker.Get().CreateObject(typeName, arguments);
                ObjectStore.Get().RegisterObject(id, obj);
            });
    }

    public Task CreateObjectWaitAsync(CommandQueueId queueId, ObjectId id, string typeName, RpcValue[]? rpcArguments)
    {
        var taskCompletion = new TaskCompletionSource();
        _commandServer.AddCommand(
            queueId,
            () =>
            {
                try
                {
                    var arguments = ConvertArguments(rpcArguments);
                    var obj = Invoker.Get().CreateObject(typeName, arguments);
                    ObjectStore.Get().RegisterObject(id, obj);

                    taskCompletion.SetResult();
                }
                catch (Exception e)
                {
                    taskCompletion.SetException(e);
                }
            });
        return taskCompletion.Task;
    }

    public void CreateObjectWithStaticMethod(
        ObjectId id,
        string className,
        string methodName,
        RpcValue[]? rpcArguments)
    {
        _commandServer.AddCommand(
            CommandQueueId.MainThread,
            () =>
            {
                var arguments = ConvertArguments(rpcArguments);
                var obj = Invoker.Get().InvokeStaticMethod(className, methodName, arguments);
                if (obj is null)
                {
                    throw new InvalidOperationException($"Failed to create object with [{className}.{methodName}].");
                }
                ObjectStore.Get().RegisterObject(id, obj);
            });
    }

    public void DestroyObject(ObjectId id)
    {
        _commandServer.AddCommand(
            CommandQueueId.MainThread,
            () =>
            {
                _ = ObjectStore.Get().UnregisterObject(id);
            });
    }

    public void InvokeMethod(ObjectId id, string methodName, RpcValue[]? rpcArguments)
    {
        _commandServer.AddCommand(
            CommandQueueId.MainThread,
            () =>
            {
                var obj = ObjectStore.Get().GetObject(id);
                var arguments = ConvertArguments(rpcArguments);
                _ = Invoker.Get().InvokeMethod(obj, methodName, arguments);
            });
    }

    public Task InvokeMethodWaitAsync(CommandQueueId queueId, ObjectId id, string methodName, RpcValue[]? rpcArguments)
    {
        var taskCompletion = new TaskCompletionSource();
        _commandServer.AddCommand(
            queueId,
            () =>
            {
                try
                {
                    var obj = ObjectStore.Get().GetObject(id);
                    var arguments = ConvertArguments(rpcArguments);
                    _ = Invoker.Get().InvokeMethod(obj, methodName, arguments);

                    taskCompletion.SetResult();
                }
                catch (Exception e)
                {
                    taskCompletion.SetException(e);
                }
            });
        return taskCompletion.Task;
    }

    public Task<RpcValue> InvokeMethodAndGetResultAsync(ObjectId id, string methodName, RpcValue[]? rpcArguments)
    {
        var taskCompletion = new TaskCompletionSource<RpcValue>();

        _commandServer.AddCommand(
            CommandQueueId.MainThread,
            () =>
            {
                try
                {
                    var obj = ObjectStore.Get().GetObject(id);
                    var arguments = ConvertArguments(rpcArguments);
                    object? result = Invoker.Get().InvokeMethod(obj, methodName, arguments);
                    taskCompletion.SetResult(ConvertObjectToRpcValue(result));
                }
                catch (Exception e)
                {
                    taskCompletion.SetException(e);
                }
            });
        return taskCompletion.Task;
    }

    public void InvokeStaticMethod(string className, string methodName, RpcValue[]? rpcArguments)
    {
        _commandServer.AddCommand(
            CommandQueueId.MainThread,
            () =>
            {
                var arguments = ConvertArguments(rpcArguments);
                _ = Invoker.Get().InvokeStaticMethod(className, methodName, arguments);
            });
    }

    public Task<RpcValue> InvokeStaticMethodAndGetResultAsync(string className, string methodName, RpcValue[]? rpcArguments)
    {
        var taskCompletion = new TaskCompletionSource<RpcValue>();

        _commandServer.AddCommand(
            CommandQueueId.MainThread,
            () =>
            {
                try
                {
                    var arguments = ConvertArguments(rpcArguments);
                    object? result = Invoker.Get().InvokeStaticMethod(className, methodName, arguments);
                    taskCompletion.SetResult(ConvertObjectToRpcValue(result));
                }
                catch (Exception e)
                {
                    taskCompletion.SetException(e);
                }
            });
        return taskCompletion.Task;
    }

    private object?[]? ConvertArguments(RpcValue[]? rpcArguments)
    {
        if (rpcArguments is null)
        {
            return null;
        }

        object?[] arguments = new object[rpcArguments.Length];
        for (int i = 0; i < arguments.Length; ++i)
        {
            arguments[i] = ConvertRpcValueToObject(rpcArguments[i]);
        }
        return arguments;
    }

    public void SetProperty(CommandQueueId queueId, ObjectId id, string propertyName, RpcValue rpcValue)
    {
        _commandServer.AddCommand(
            queueId,
            () =>
            {
                var obj = ObjectStore.Get().GetObject(id);
                var value = ConvertRpcValueToObject(rpcValue);
                Invoker.Get().SetProperty(obj, propertyName, value);
            });
    }

    public Task SetPropertyWaitAsync(CommandQueueId queueId, ObjectId id, string propertyName, RpcValue rpcValue)
    {
        var taskCompletion = new TaskCompletionSource();
        _commandServer.AddCommand(
            queueId,
            () =>
            {
                try
                {
                    var obj = ObjectStore.Get().GetObject(id);
                    var value = ConvertRpcValueToObject(rpcValue);
                    Invoker.Get().SetProperty(obj, propertyName, value);

                    taskCompletion.SetResult();
                }
                catch (Exception e)
                {
                    taskCompletion.SetException(e);
                }
            });
        return taskCompletion.Task;
    }

    public Task<RpcValue> GetPropertyAsync(ObjectId id, string propertyName)
    {
        var taskCompletion = new TaskCompletionSource<RpcValue>();
        _commandServer.AddCommand(
            CommandQueueId.MainThread,
            () =>
            {
                try
                {
                    var obj = ObjectStore.Get().GetObject(id);
                    var value = Invoker.Get().GetProperty(obj, propertyName);
                    taskCompletion.SetResult(ConvertObjectToRpcValue(value));
                }
                catch (Exception e)
                {
                    taskCompletion.SetException(e);
                }
            });
        return taskCompletion.Task;
    }

    public Task<RpcValue> GetStaticPropertyAsync(string className, string propertyName)
    {
        var taskCompletion = new TaskCompletionSource<RpcValue>();
        _commandServer.AddCommand(
            CommandQueueId.MainThread,
            () =>
            {
                try
                {
                    var value = Invoker.Get().GetStaticProperty(className, propertyName);
                    taskCompletion.SetResult(ConvertObjectToRpcValue(value));
                }
                catch (Exception e)
                {
                    taskCompletion.SetException(e);
                }
            });
        return taskCompletion.Task;
    }

    public void SetIndexerProperty(ObjectId id, RpcValue rpcIndex, RpcValue rpcValue)
    {
        _commandServer.AddCommand(
            CommandQueueId.MainThread,
            () =>
            {
                var obj = ObjectStore.Get().GetObject(id);
                var index = ConvertRpcValueToObject(rpcIndex);
                var value = ConvertRpcValueToObject(rpcValue);

                if (index is null)
                {
                    throw new InvalidOperationException("Index of Indexer property cannot be null.");
                }

                Invoker.Get().SetIndexerProperty(obj, index, value);
            });
    }

    public Task<RpcValue> GetIndexerPropertyAsync(ObjectId id, RpcValue rpcIndex)
    {
        var taskCompletion = new TaskCompletionSource<RpcValue>();
        _commandServer.AddCommand(
            CommandQueueId.MainThread,
            () =>
            {
                try
                {
                    var obj = ObjectStore.Get().GetObject(id);
                    var index = ConvertRpcValueToObject(rpcIndex);
                    if (index is null)
                    {
                        throw new InvalidOperationException("Index of Indexer property cannot be null.");
                    }

                    var value = Invoker.Get().GetIndexerProperty(obj, index);
                    taskCompletion.SetResult(ConvertObjectToRpcValue(value));
                }
                catch (Exception e)
                {
                    taskCompletion.SetException(e);
                }
            });
        return taskCompletion.Task;
    }

    public void WriteError(string message)
    {
        // Do not use CommandServer to show errors immediately without being blocked by other commands.
        Console.Error.WriteLine(message);
    }

    private object? ConvertRpcValueToEnum(RpcValue rpcValue)
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

    private object? ConvertRpcValueToObject(RpcValue rpcValue)
    {
        var enumValue = ConvertRpcValueToEnum(rpcValue);
        if (enumValue is not null)
        {
            return enumValue;
        }

        var value = rpcValue.GetObject();
        if (value is RpcValue[] array)
        {
            var objectArray = new object?[array.Length];
            for (int i = 0; i < array.Length; ++i)
            {
                objectArray[i] = ConvertRpcValueToObject(array[i]);
            }
            return objectArray;
        }
        else
        if (value is ObjectId objectId)
        {
            return ObjectStore.Get().GetObject(objectId);
        }
        else
        {
            return value;
        }
    }

    private RpcValue ConvertObjectToRpcValue(object? obj)
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
                _ = ObjectStore.Get().RegisterObject(obj!, out ObjectId id);
                return new RpcValue(id);
            }
        }
    }
}
