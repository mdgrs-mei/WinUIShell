using WinUIShell.Common;

namespace WinUIShell;

public class WinUIShellObjectList<T> : WinUIShellObject
{
    public int Count
    {
        get => PropertyAccessor.Get<int>(Id, nameof(Count))!;
    }

    public bool IsReadOnly
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsReadOnly))!;
    }

    public T this[int index]
    {
        get => PropertyAccessor.GetIndexer<T>(Id, index)!;
        set => PropertyAccessor.SetIndexer(Id, index, value);
    }

    internal WinUIShellObjectList(ObjectId id)
        : base(id)
    {
    }

    public int IndexOf(T item)
    {
        if (item is WinUIShellObject obj)
        {
            return CommandClient.Get().InvokeMethodAndGetResult<int>(Id, nameof(IndexOf), obj.Id);
        }
        else
        {
            return CommandClient.Get().InvokeMethodAndGetResult<int>(Id, nameof(IndexOf), item);
        }
    }

    public void Insert(int index, T item)
    {
        if (item is WinUIShellObject obj)
        {
            CommandClient.Get().InvokeMethod(Id, nameof(Insert), index, obj.Id);
        }
        else
        {
            CommandClient.Get().InvokeMethod(Id, nameof(Insert), index, item);
        }
    }

    public void RemoveAt(int index)
    {
        CommandClient.Get().InvokeMethod(Id, nameof(RemoveAt), index);
    }

    public void Add(T item)
    {
        if (item is WinUIShellObject obj)
        {
            CommandClient.Get().InvokeMethod(Id, nameof(Add), obj.Id);
        }
        else
        {
            CommandClient.Get().InvokeMethod(Id, nameof(Add), item);
        }
    }

    public void Clear()
    {
        CommandClient.Get().InvokeMethod(Id, nameof(Clear));
    }

    public bool Contains(T item)
    {
        if (item is WinUIShellObject obj)
        {
            return CommandClient.Get().InvokeMethodAndGetResult<bool>(Id, nameof(Contains), obj.Id);
        }
        else
        {
            return CommandClient.Get().InvokeMethodAndGetResult<bool>(Id, nameof(Contains), item);
        }
    }

    public bool Remove(T item)
    {
        if (item is WinUIShellObject obj)
        {
            return CommandClient.Get().InvokeMethodAndGetResult<bool>(Id, nameof(Remove), obj.Id);
        }
        else
        {
            return CommandClient.Get().InvokeMethodAndGetResult<bool>(Id, nameof(Remove), item);
        }
    }
}
