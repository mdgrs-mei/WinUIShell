
using System.Diagnostics;

namespace WinUIShell.Common;

public enum CommandQueueType
{
    Immediate,
    MainThread,
    ThreadPool,
    RunspaceId,
    TemporaryQueueId
}

public class CommandQueueId
{
    public CommandQueueType Type { get; set; } = CommandQueueType.MainThread;
    public int Id { get; set; }

    public CommandQueueId()
    {
    }

    public CommandQueueId(CommandQueueType type)
    {
        Debug.Assert(type is not (CommandQueueType.RunspaceId or CommandQueueType.TemporaryQueueId));
        Type = type;
    }

    public CommandQueueId(CommandQueueType type, int id)
    {
        Debug.Assert(type is CommandQueueType.RunspaceId or CommandQueueType.TemporaryQueueId);
        Type = type;
        Id = id;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as CommandQueueId);
    }

    public bool Equals(CommandQueueId? other)
    {
        if (other is null)
            return false;

        return (Type == other.Type) && (Id == other.Id);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Type, Id);
    }

    public static readonly CommandQueueId Immediate = new(CommandQueueType.Immediate);
    public static readonly CommandQueueId MainThread = new(CommandQueueType.MainThread);
    public static readonly CommandQueueId ThreadPool = new(CommandQueueType.ThreadPool);
}
