namespace RpcUIShell.Core;

internal sealed class RpcService
{
    private readonly CommandServer _commandServer;

    public RpcService(CommandServer commandServer)
    {
        _commandServer = commandServer;
    }

    public void ProcessTemporaryQueue(CommandQueueId queueId, CommandQueueId temporaryQueueId)
    {
        _commandServer.AddCommand(
            queueId,
            () =>
            {
                _commandServer.ProcessCommands(temporaryQueueId);
                _commandServer.RemoveCommandQueue(temporaryQueueId);
            });
    }

    public void CreateObject(CommandQueueId queueId, ObjectId id, string typeName, RpcValue[]? rpcArguments)
    {
        _commandServer.AddCommand(
            queueId,
            () =>
            {
                var arguments = RpcValueConverter.Get().ConvertRpcValueArrayToObjectArray(rpcArguments);
                var obj = Invoker.Get().CreateObject(typeName, arguments);
                ObjectStore.Get().RegisterObject(id, obj);
            });
    }

    public void CreateObjectWithId(CommandQueueId queueId, ObjectId id, string typeName)
    {
        _commandServer.AddCommand(
            queueId,
            () =>
            {
                var obj = Invoker.Get().CreateObject(typeName, [id]);
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
                    var arguments = RpcValueConverter.Get().ConvertRpcValueArrayToObjectArray(rpcArguments);
                    var obj = Invoker.Get().CreateObject(typeName, arguments);
                    ObjectStore.Get().RegisterObject(id, obj);

                    taskCompletion.SetResult();
                }
                catch (Exception e)
                {
                    taskCompletion.SetException(e);
                    throw;
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
                var arguments = RpcValueConverter.Get().ConvertRpcValueArrayToObjectArray(rpcArguments);
                var obj = Invoker.Get().InvokeStaticMethod(className, methodName, arguments);
                if (obj is null)
                {
                    throw new InvalidOperationException($"Failed to create object with [{className}.{methodName}].");
                }
                ObjectStore.Get().RegisterObject(id, obj);
            });
    }

    public void RemoveObject(CommandQueueId queueId, ObjectId id)
    {
        _commandServer.AddCommand(
            queueId,
            () =>
            {
                ObjectStore.Get().UnregisterObject(id);
            });
    }

    public void RemoveAllObjects()
    {
        _commandServer.AddCommand(
            CommandQueueId.MainThread,
            () =>
            {
                ObjectStore.Get().UnregisterAllObjects();
            });
    }

    public void InvokeMethod(ObjectId id, string? typeName, string methodName, RpcValue[]? rpcArguments)
    {
        _commandServer.AddCommand(
            CommandQueueId.MainThread,
            () =>
            {
                var obj = ObjectStore.Get().GetObject(id);
                var arguments = RpcValueConverter.Get().ConvertRpcValueArrayToObjectArray(rpcArguments);
                _ = Invoker.Get().InvokeMethod(obj, typeName, methodName, arguments);
            });
    }

    public Task InvokeMethodWaitAsync(CommandQueueId queueId, ObjectId id, string? typeName, string methodName, RpcValue[]? rpcArguments)
    {
        var taskCompletion = new TaskCompletionSource();
        _commandServer.AddCommand(
            queueId,
            () =>
            {
                try
                {
                    var obj = ObjectStore.Get().GetObject(id);
                    var arguments = RpcValueConverter.Get().ConvertRpcValueArrayToObjectArray(rpcArguments);
                    _ = Invoker.Get().InvokeMethod(obj, typeName, methodName, arguments);

                    taskCompletion.SetResult();
                }
                catch (Exception e)
                {
                    taskCompletion.SetException(e);
                    throw;
                }
            });
        return taskCompletion.Task;
    }

    public Task<RpcValue> InvokeMethodAndGetResultAsync(ObjectId id, string? typeName, string methodName, RpcValue[]? rpcArguments)
    {
        var taskCompletion = new TaskCompletionSource<RpcValue>();

        _commandServer.AddCommand(
            CommandQueueId.MainThread,
            () =>
            {
                try
                {
                    var obj = ObjectStore.Get().GetObject(id);
                    var arguments = RpcValueConverter.Get().ConvertRpcValueArrayToObjectArray(rpcArguments);
                    object? result = Invoker.Get().InvokeMethod(obj, typeName, methodName, arguments);
                    taskCompletion.SetResult(RpcValueConverter.Get().ConvertObjectToRpcValue(result));
                }
                catch (Exception e)
                {
                    taskCompletion.SetException(e);
                    throw;
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
                var arguments = RpcValueConverter.Get().ConvertRpcValueArrayToObjectArray(rpcArguments);
                _ = Invoker.Get().InvokeStaticMethod(className, methodName, arguments);
            });
    }

    public Task InvokeStaticMethodWaitAsync(CommandQueueId queueId, string className, string methodName, RpcValue[]? rpcArguments)
    {
        var taskCompletion = new TaskCompletionSource();
        _commandServer.AddCommand(
            queueId,
            () =>
            {
                try
                {
                    var arguments = RpcValueConverter.Get().ConvertRpcValueArrayToObjectArray(rpcArguments);
                    _ = Invoker.Get().InvokeStaticMethod(className, methodName, arguments);

                    taskCompletion.SetResult();
                }
                catch (Exception e)
                {
                    taskCompletion.SetException(e);
                    throw;
                }
            });
        return taskCompletion.Task;
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
                    var arguments = RpcValueConverter.Get().ConvertRpcValueArrayToObjectArray(rpcArguments);
                    object? result = Invoker.Get().InvokeStaticMethod(className, methodName, arguments);
                    taskCompletion.SetResult(RpcValueConverter.Get().ConvertObjectToRpcValue(result));
                }
                catch (Exception e)
                {
                    taskCompletion.SetException(e);
                    throw;
                }
            });
        return taskCompletion.Task;
    }

    public void SetProperty(CommandQueueId queueId, ObjectId id, string? typeName, string propertyName, RpcValue rpcValue)
    {
        _commandServer.AddCommand(
            queueId,
            () =>
            {
                var obj = ObjectStore.Get().GetObject(id);
                var value = RpcValueConverter.Get().ConvertRpcValueTo<object>(rpcValue);
                Invoker.Get().SetProperty(obj, typeName, propertyName, value);
            });
    }

    public Task SetPropertyWaitAsync(CommandQueueId queueId, ObjectId id, string? typeName, string propertyName, RpcValue rpcValue)
    {
        var taskCompletion = new TaskCompletionSource();
        _commandServer.AddCommand(
            queueId,
            () =>
            {
                try
                {
                    var obj = ObjectStore.Get().GetObject(id);
                    var value = RpcValueConverter.Get().ConvertRpcValueTo<object>(rpcValue);
                    Invoker.Get().SetProperty(obj, typeName, propertyName, value);

                    taskCompletion.SetResult();
                }
                catch (Exception e)
                {
                    taskCompletion.SetException(e);
                    throw;
                }
            });
        return taskCompletion.Task;
    }

    public void SetStaticProperty(CommandQueueId queueId, string className, string propertyName, RpcValue rpcValue)
    {
        _commandServer.AddCommand(
            queueId,
            () =>
            {
                var value = RpcValueConverter.Get().ConvertRpcValueTo<object>(rpcValue);
                Invoker.Get().SetStaticProperty(className, propertyName, value);
            });
    }

    public Task<RpcValue> GetPropertyAsync(ObjectId id, string? typeName, string propertyName)
    {
        var taskCompletion = new TaskCompletionSource<RpcValue>();
        _commandServer.AddCommand(
            CommandQueueId.MainThread,
            () =>
            {
                try
                {
                    var obj = ObjectStore.Get().GetObject(id);
                    var value = Invoker.Get().GetProperty(obj, typeName, propertyName);
                    taskCompletion.SetResult(RpcValueConverter.Get().ConvertObjectToRpcValue(value));
                }
                catch (Exception e)
                {
                    taskCompletion.SetException(e);
                    throw;
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
                    taskCompletion.SetResult(RpcValueConverter.Get().ConvertObjectToRpcValue(value));
                }
                catch (Exception e)
                {
                    taskCompletion.SetException(e);
                    throw;
                }
            });
        return taskCompletion.Task;
    }

    public void SetIndexerProperty(ObjectId id, string? typeName, string indexerName, RpcValue rpcValue, RpcValue[]? rpcIndexArguments)
    {
        _commandServer.AddCommand(
            CommandQueueId.MainThread,
            () =>
            {
                var obj = ObjectStore.Get().GetObject(id);
                var value = RpcValueConverter.Get().ConvertRpcValueTo<object>(rpcValue);
                var indexArguments = RpcValueConverter.Get().ConvertRpcValueArrayToObjectArray(rpcIndexArguments);

                if (indexArguments is null)
                {
                    throw new InvalidOperationException("Index of Indexer property cannot be null.");
                }

                Invoker.Get().SetIndexerProperty(obj, typeName, indexerName, value, indexArguments);
            });
    }

    public Task<RpcValue> GetIndexerPropertyAsync(ObjectId id, string? typeName, string indexerName, RpcValue[]? rpcIndexArguments)
    {
        var taskCompletion = new TaskCompletionSource<RpcValue>();
        _commandServer.AddCommand(
            CommandQueueId.MainThread,
            () =>
            {
                try
                {
                    var obj = ObjectStore.Get().GetObject(id);
                    var indexArguments = RpcValueConverter.Get().ConvertRpcValueArrayToObjectArray(rpcIndexArguments);

                    if (indexArguments is null)
                    {
                        throw new InvalidOperationException("Index of Indexer property cannot be null.");
                    }

                    var value = Invoker.Get().GetIndexerProperty(obj, typeName, indexerName, indexArguments);
                    taskCompletion.SetResult(RpcValueConverter.Get().ConvertObjectToRpcValue(value));
                }
                catch (Exception e)
                {
                    taskCompletion.SetException(e);
                    throw;
                }
            });
        return taskCompletion.Task;
    }

    public void WriteError(string message)
    {
        // Do not use CommandServer to show errors immediately without being blocked by other commands.
        Console.Error.WriteLine(message);
    }
}
