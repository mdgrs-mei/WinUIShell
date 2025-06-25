
using System.Diagnostics;
using System.IO.Pipes;
using System.Security.Principal;
using Microsoft.VisualStudio.Threading;
using StreamJsonRpc;

namespace WinUIShell.Common;

public class CommandClient : Singleton<CommandClient>
{
    private NamedPipeClientStream? _clientStream;
    private JsonRpc? _rpc;
    private readonly JoinableTaskFactory _joinableTaskFactory = new(new JoinableTaskContext());

    public void Init(string pipeName)
    {
        _clientStream = new NamedPipeClientStream(
            serverName: ".",
            pipeName: pipeName,
            direction: PipeDirection.InOut,
            options: PipeOptions.WriteThrough | PipeOptions.Asynchronous,
            impersonationLevel: TokenImpersonationLevel.Anonymous);

        _clientStream.Connect(Constants.CommandClientConnectTimeoutMillisecond);
        _rpc = JsonRpc.Attach(_clientStream);
    }

    public void Term()
    {
        _rpc?.Dispose();
        _clientStream?.Dispose();
    }

    public ObjectId CreateObject(string typeName, object? linkedObject, params object?[] arguments)
    {
        return CreateObject(CommandQueueId.MainThread, typeName, linkedObject, arguments);
    }

    public ObjectId CreateObject(CommandQueueId queueId, string typeName, object? linkedObject, params object?[] arguments)
    {
        Debug.Assert(_rpc is not null);
        ArgumentNullException.ThrowIfNull(arguments);

        if (!ObjectStore.Get().RegisterObject(linkedObject, out ObjectId id))
            return id;

        var rpcArguments = RpcValueConverter.ConvertObjectArrayToRpcArray(arguments);

        _joinableTaskFactory.Run(async () =>
        {
            await _rpc.InvokeAsync("CreateObject", queueId, id, typeName, rpcArguments);
        });
        return id;
    }

    public ObjectId CreateObjectWithId(string typeName, object? linkedObject)
    {
        return CreateObjectWithId(CommandQueueId.MainThread, typeName, linkedObject);
    }

    public ObjectId CreateObjectWithId(CommandQueueId queueId, string typeName, object? linkedObject)
    {
        Debug.Assert(_rpc is not null);

        if (!ObjectStore.Get().RegisterObject(linkedObject, out ObjectId id))
            return id;

        _joinableTaskFactory.Run(async () =>
        {
            await _rpc.InvokeAsync("CreateObjectWithId", queueId, id, typeName);
        });
        return id;
    }

    public Task<ObjectId> CreateObjectWaitAsync(string typeName, object? linkedObject, params object?[] arguments)
    {
        return CreateObjectWaitAsync(CommandQueueId.MainThread, typeName, linkedObject, arguments);
    }

    public async Task<ObjectId> CreateObjectWaitAsync(CommandQueueId queueId, string typeName, object? linkedObject, params object?[] arguments)
    {
        Debug.Assert(_rpc is not null);
        ArgumentNullException.ThrowIfNull(arguments);

        if (!ObjectStore.Get().RegisterObject(linkedObject, out ObjectId id))
            return id;

        var rpcArguments = RpcValueConverter.ConvertObjectArrayToRpcArray(arguments);

        await _rpc.InvokeAsync("CreateObjectWait", queueId, id, typeName, rpcArguments);

        return id;
    }

    public ObjectId CreateObjectWithStaticMethod(
        string className,
        string methodName,
        object? linkedObject,
        params object?[] arguments)
    {
        Debug.Assert(_rpc is not null);
        ArgumentNullException.ThrowIfNull(arguments);

        if (!ObjectStore.Get().RegisterObject(linkedObject, out ObjectId id))
            return id;

        var rpcArguments = RpcValueConverter.ConvertObjectArrayToRpcArray(arguments);

        _joinableTaskFactory.Run(async () =>
        {
            await _rpc.InvokeAsync("CreateObjectWithStaticMethod", id, className, methodName, rpcArguments);
        });
        return id;
    }

    public void DestroyObject(ObjectId id)
    {
        Debug.Assert(_rpc is not null);

        if (!ObjectStore.Get().UnregisterObject(id))
            return;

        _joinableTaskFactory.Run(async () =>
        {
            await _rpc.InvokeAsync("DestroyObject", id);
        });
    }

    public void InvokeMethod(ObjectId id, string methodName, params object?[] arguments)
    {
        Debug.Assert(_rpc is not null);
        ArgumentNullException.ThrowIfNull(arguments);

        var rpcArguments = RpcValueConverter.ConvertObjectArrayToRpcArray(arguments);

        _joinableTaskFactory.Run(async () =>
        {
            await _rpc.InvokeAsync("InvokeMethod", id, methodName, rpcArguments);
        });
    }

    public Task InvokeMethodWaitAsync(ObjectId id, string methodName, params object?[] arguments)
    {
        return InvokeMethodWaitAsync(CommandQueueId.MainThread, id, methodName, arguments);
    }

    public Task InvokeMethodWaitAsync(CommandQueueId queueId, ObjectId id, string methodName, params object?[] arguments)
    {
        Debug.Assert(_rpc is not null);
        ArgumentNullException.ThrowIfNull(arguments);

        RpcValue[]? rpcArguments = RpcValueConverter.ConvertObjectArrayToRpcArray(arguments);

        return _rpc.InvokeAsync("InvokeMethodWait", queueId, id, methodName, rpcArguments);
    }

    public T? InvokeMethodAndGetResult<T>(ObjectId id, string methodName, params object?[] arguments)
    {
        Debug.Assert(_rpc is not null);
        ArgumentNullException.ThrowIfNull(arguments);

        var rpcArguments = RpcValueConverter.ConvertObjectArrayToRpcArray(arguments);

        var rpcValue = _joinableTaskFactory.Run(async () =>
        {
            return await _rpc.InvokeAsync<RpcValue>("InvokeMethodAndGetResult", id, methodName, rpcArguments);
        });

        return RpcValueConverter.ConvertRpcValueTo<T>(rpcValue);
    }

    public void InvokeStaticMethod(string className, string methodName, params object?[] arguments)
    {
        Debug.Assert(_rpc is not null);
        ArgumentNullException.ThrowIfNull(arguments);

        var rpcArguments = RpcValueConverter.ConvertObjectArrayToRpcArray(arguments);

        _joinableTaskFactory.Run(async () =>
        {
            await _rpc.InvokeAsync("InvokeStaticMethod", className, methodName, rpcArguments);
        });
    }

    public Task InvokeStaticMethodWaitAsync(CommandQueueId queueId, string className, string methodName, params object?[] arguments)
    {
        Debug.Assert(_rpc is not null);
        ArgumentNullException.ThrowIfNull(arguments);

        var rpcArguments = RpcValueConverter.ConvertObjectArrayToRpcArray(arguments);

        return _rpc.InvokeAsync("InvokeStaticMethodWait", queueId, className, methodName, rpcArguments);
    }

    public T? InvokeStaticMethodAndGetResult<T>(string className, string methodName, params object?[] arguments)
    {
        Debug.Assert(_rpc is not null);
        ArgumentNullException.ThrowIfNull(arguments);

        var rpcArguments = RpcValueConverter.ConvertObjectArrayToRpcArray(arguments);

        var rpcValue = _joinableTaskFactory.Run(async () =>
        {
            return await _rpc.InvokeAsync<RpcValue>("InvokeStaticMethodAndGetResult", className, methodName, rpcArguments);
        });

        return RpcValueConverter.ConvertRpcValueTo<T>(rpcValue);
    }

    public void SetProperty(ObjectId id, string propertyName, object? value)
    {
        SetProperty(CommandQueueId.MainThread, id, propertyName, value);
    }
    public void SetProperty(CommandQueueId queueId, ObjectId id, string propertyName, object? value)
    {
        Debug.Assert(_rpc is not null);

        _joinableTaskFactory.Run(async () =>
        {
            await _rpc.InvokeAsync("SetProperty", queueId, id, propertyName, new RpcValue(value));
        });
    }

    public Task SetPropertyAsync(ObjectId id, string propertyName, object? value)
    {
        return SetPropertyAsync(CommandQueueId.MainThread, id, propertyName, value);
    }
    public Task SetPropertyAsync(CommandQueueId queueId, ObjectId id, string propertyName, object? value)
    {
        Debug.Assert(_rpc is not null);
        return _rpc.InvokeAsync("SetProperty", queueId, id, propertyName, new RpcValue(value));
    }

    public void SetPropertyWait(ObjectId id, string propertyName, object? value)
    {
        SetPropertyWait(CommandQueueId.MainThread, id, propertyName, value);
    }
    public void SetPropertyWait(CommandQueueId queueId, ObjectId id, string propertyName, object? value)
    {
        Debug.Assert(_rpc is not null);

        _joinableTaskFactory.Run(async () =>
        {
            await _rpc.InvokeAsync("SetPropertyWait", queueId, id, propertyName, new RpcValue(value));
        });
    }

    public T? GetProperty<T>(ObjectId id, string propertyName)
    {
        Debug.Assert(_rpc is not null);

        var rpcValue = _joinableTaskFactory.Run(async () =>
        {
            return await _rpc.InvokeAsync<RpcValue>("GetProperty", id, propertyName);
        });

        return RpcValueConverter.ConvertRpcValueTo<T>(rpcValue);
    }

    public T? GetStaticProperty<T>(string className, string propertyName)
    {
        Debug.Assert(_rpc is not null);

        var rpcValue = _joinableTaskFactory.Run(async () =>
        {
            return await _rpc.InvokeAsync<RpcValue>("GetStaticProperty", className, propertyName);
        });

        return RpcValueConverter.ConvertRpcValueTo<T>(rpcValue);
    }

    public void SetIndexerProperty(ObjectId id, object index, object? value)
    {
        Debug.Assert(_rpc is not null);

        _joinableTaskFactory.Run(async () =>
        {
            await _rpc.InvokeAsync("SetIndexerProperty", id, new RpcValue(index), new RpcValue(value));
        });
    }

    public T? GetIndexerProperty<T>(ObjectId id, object index)
    {
        Debug.Assert(_rpc is not null);

        var rpcValue = _joinableTaskFactory.Run(async () =>
        {
            return await _rpc.InvokeAsync<RpcValue>("GetIndexerProperty", id, new RpcValue(index));
        });

        return RpcValueConverter.ConvertRpcValueTo<T>(rpcValue);
    }

    public void WriteError(string message)
    {
        Debug.Assert(_rpc is not null);

        _joinableTaskFactory.Run(async () =>
        {
            await _rpc.InvokeAsync("WriteError", message);
        });
    }
}
