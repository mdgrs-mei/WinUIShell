
namespace WinUIShell.Common;

public enum CommandQueueType
{
    Immediate,
    MainThread,
    ThreadPool,
    ThreadId
}

public class CommandQueueId
{
    public CommandQueueType Type { get; set; } = CommandQueueType.MainThread;
    public int ThreadId { get; set; } = Constants.InvalidThreadId;

    public CommandQueueId()
    {
    }

    public CommandQueueId(CommandQueueType type)
    {
        Type = type;
    }

    public CommandQueueId(int threadId)
    {
        Type = CommandQueueType.ThreadId;
        ThreadId = threadId;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as CommandQueueId);
    }

    public bool Equals(CommandQueueId? other)
    {
        if (other is null)
            return false;

        return (Type == other.Type) && (ThreadId == other.ThreadId);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Type, ThreadId);
    }

    public static readonly CommandQueueId Immediate = new(CommandQueueType.Immediate);
    public static readonly CommandQueueId MainThread = new(CommandQueueType.MainThread);
    public static readonly CommandQueueId ThreadPool = new(CommandQueueType.ThreadPool);
}
