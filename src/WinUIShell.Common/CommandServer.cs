using System.Collections.Concurrent;
using System.IO.Pipes;
using Microsoft.VisualStudio.Threading;
using StreamJsonRpc;

namespace WinUIShell.Common;

public class CommandServer : Singleton<CommandServer>
{
    private string _pipeName = "";
    private Thread? _thread;
    private JsonRpc? _rpc;
    private readonly object _rpcInitLock = new();
    private readonly JoinableTaskFactory _joinableTaskFactory = new(new JoinableTaskContext());

    private readonly ConcurrentDictionary<CommandQueueId, ConcurrentQueue<Action>> _commandQueues = new();

    public void Init(string pipeName)
    {
        _pipeName = pipeName;
        _thread = new Thread(new ThreadStart(ServerThreadEntry))
        {
            IsBackground = true
        };
        _thread.Start();
    }

    public void Term()
    {
        lock (_rpcInitLock)
        {
            if (_rpc is not null)
            {
                _rpc.Dispose();
            }
        }
        if (_thread is not null)
        {
            _thread.Join();
        }
    }

    private void ServerThreadEntry()
    {
        using var serverStream = new NamedPipeServerStream(
            pipeName: _pipeName,
            direction: PipeDirection.InOut,
            maxNumberOfServerInstances: NamedPipeServerStream.MaxAllowedServerInstances,
            transmissionMode: PipeTransmissionMode.Byte,
            options: PipeOptions.Asynchronous);

        serverStream.WaitForConnection();

        lock (_rpcInitLock)
        {
            _rpc = JsonRpc.Attach(serverStream, new RpcService(this));
        }
        _joinableTaskFactory.Run(async () =>
        {
            await _rpc.Completion;
        });
    }

    public void AddCommand(CommandQueueId queueId, Action action)
    {
        ArgumentNullException.ThrowIfNull(queueId);
        ArgumentNullException.ThrowIfNull(action);

        if (queueId.Equals(CommandQueueId.Immediate))
        {
            action();
            return;
        }

        var queue = _commandQueues.GetOrAdd(queueId, (key) => new ConcurrentQueue<Action>());
        queue.Enqueue(action);
    }

    public void ProcessCommands(CommandQueueId queueId)
    {
        _ = _commandQueues.TryGetValue(queueId, out ConcurrentQueue<Action>? queue);
        if (queue is null)
            return;

        while (queue.TryDequeue(out Action? action))
        {
            action();
        }
    }
}
