using WinUIShell.Common;

namespace WinUIShell;

public class RowDefinitionCollection : WinUIShellObject
{
    public int Count
    {
        get => PropertyAccessor.Get<int>(Id, nameof(Count))!;
    }

    public bool IsReadOnly
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsReadOnly))!;
    }

    public RowDefinition this[int index]
    {
        get => PropertyAccessor.GetIndexer<RowDefinition>(Id, index)!;
        set => PropertyAccessor.SetIndexer(Id, index, value);
    }

    internal RowDefinitionCollection(ObjectId id)
        : base(id)
    {
    }

    public int IndexOf(RowDefinition item)
    {
        return CommandClient.Get().InvokeMethodAndGetResult<int>(Id, nameof(IndexOf), item?.Id);
    }

    public void Insert(int index, RowDefinition item)
    {
        CommandClient.Get().InvokeMethod(Id, nameof(Insert), index, item?.Id);
    }

    public void RemoveAt(int index)
    {
        CommandClient.Get().InvokeMethod(Id, nameof(RemoveAt), index);
    }

    public void Add(RowDefinition item)
    {
        CommandClient.Get().InvokeMethod(Id, nameof(Add), item?.Id);
    }

    public void Clear()
    {
        CommandClient.Get().InvokeMethod(Id, nameof(Clear));
    }

    public bool Contains(RowDefinition item)
    {
        return CommandClient.Get().InvokeMethodAndGetResult<bool>(Id, nameof(Contains), item?.Id);
    }
}
