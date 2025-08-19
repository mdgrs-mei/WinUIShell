
namespace WinUIShell.Common;

public enum CommandQueueType
{
    Immediate,
    MainThread,
    ThreadPool,
    RunspaceId
}

public class CommandQueueId
{
    public CommandQueueType Type { get; set; } = CommandQueueType.MainThread;
    public int RunspaceId { get; set; } = Constants.InvalidRunspaceId;

    public CommandQueueId()
    {
    }

    public CommandQueueId(CommandQueueType type)
    {
        Type = type;
    }

    public CommandQueueId(int runspaceId)
    {
        Type = CommandQueueType.RunspaceId;
        RunspaceId = runspaceId;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as CommandQueueId);
    }

    public bool Equals(CommandQueueId? other)
    {
        if (other is null)
            return false;

        return (Type == other.Type) && (RunspaceId == other.RunspaceId);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Type, RunspaceId);
    }

    public static readonly CommandQueueId Immediate = new(CommandQueueType.Immediate);
    public static readonly CommandQueueId MainThread = new(CommandQueueType.MainThread);
    public static readonly CommandQueueId ThreadPool = new(CommandQueueType.ThreadPool);
}
