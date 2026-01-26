namespace WinUIShell.Common;

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
                var arguments = RpcValueConverter.ConvertRpcValueArrayToObjectArray(rpcArguments);
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
                    var arguments = RpcValueConverter.ConvertRpcValueArrayToObjectArray(rpcArguments);
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
                var arguments = RpcValueConverter.ConvertRpcValueArrayToObjectArray(rpcArguments);
                var obj = Invoker.Get().InvokeStaticMethod(className, methodName, arguments);
                if (obj is null)
                {
                    throw new InvalidOperationException($"Failed to create object with [{className}.{methodName}].");
                }
                ObjectStore.Get().RegisterObject(id, obj);
            });
    }

    public void DestroyObject(CommandQueueId queueId, ObjectId id)
    {
        _commandServer.AddCommand(
            queueId,
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
                var arguments = RpcValueConverter.ConvertRpcValueArrayToObjectArray(rpcArguments);
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
                    var arguments = RpcValueConverter.ConvertRpcValueArrayToObjectArray(rpcArguments);
                    _ = Invoker.Get().InvokeMethod(obj, methodName, arguments);

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
                    var arguments = RpcValueConverter.ConvertRpcValueArrayToObjectArray(rpcArguments);
                    object? result = Invoker.Get().InvokeMethod(obj, methodName, arguments);
                    taskCompletion.SetResult(RpcValueConverter.ConvertObjectToRpcValue(result));
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
                var arguments = RpcValueConverter.ConvertRpcValueArrayToObjectArray(rpcArguments);
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
                    var arguments = RpcValueConverter.ConvertRpcValueArrayToObjectArray(rpcArguments);
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
                    var arguments = RpcValueConverter.ConvertRpcValueArrayToObjectArray(rpcArguments);
                    object? result = Invoker.Get().InvokeStaticMethod(className, methodName, arguments);
                    taskCompletion.SetResult(RpcValueConverter.ConvertObjectToRpcValue(result));
                }
                catch (Exception e)
                {
                    taskCompletion.SetException(e);
                    throw;
                }
            });
        return taskCompletion.Task;
    }

    public void SetProperty(CommandQueueId queueId, ObjectId id, string propertyName, RpcValue rpcValue)
    {
        _commandServer.AddCommand(
            queueId,
            () =>
            {
                var obj = ObjectStore.Get().GetObject(id);
                var value = RpcValueConverter.ConvertRpcValueTo<object, object>(rpcValue);
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
                    var value = RpcValueConverter.ConvertRpcValueTo<object, object>(rpcValue);
                    Invoker.Get().SetProperty(obj, propertyName, value);

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
                var value = RpcValueConverter.ConvertRpcValueTo<object, object>(rpcValue);
                Invoker.Get().SetStaticProperty(className, propertyName, value);
            });
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
                    taskCompletion.SetResult(RpcValueConverter.ConvertObjectToRpcValue(value));
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
                    taskCompletion.SetResult(RpcValueConverter.ConvertObjectToRpcValue(value));
                }
                catch (Exception e)
                {
                    taskCompletion.SetException(e);
                    throw;
                }
            });
        return taskCompletion.Task;
    }

    public void SetIndexerProperty(ObjectId id, RpcValue rpcValue, RpcValue[]? rpcIndexArguments)
    {
        _commandServer.AddCommand(
            CommandQueueId.MainThread,
            () =>
            {
                var obj = ObjectStore.Get().GetObject(id);
                var value = RpcValueConverter.ConvertRpcValueTo<object, object>(rpcValue);
                var indexArguments = RpcValueConverter.ConvertRpcValueArrayToObjectArray(rpcIndexArguments);

                if (indexArguments is null)
                {
                    throw new InvalidOperationException("Index of Indexer property cannot be null.");
                }

                Invoker.Get().SetIndexerProperty(obj, value, indexArguments);
            });
    }

    public Task<RpcValue> GetIndexerPropertyAsync(ObjectId id, RpcValue[]? rpcIndexArguments)
    {
        var taskCompletion = new TaskCompletionSource<RpcValue>();
        _commandServer.AddCommand(
            CommandQueueId.MainThread,
            () =>
            {
                try
                {
                    var obj = ObjectStore.Get().GetObject(id);
                    var indexArguments = RpcValueConverter.ConvertRpcValueArrayToObjectArray(rpcIndexArguments);

                    if (indexArguments is null)
                    {
                        throw new InvalidOperationException("Index of Indexer property cannot be null.");
                    }

                    var value = Invoker.Get().GetIndexerProperty(obj, indexArguments);
                    taskCompletion.SetResult(RpcValueConverter.ConvertObjectToRpcValue(value));
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
