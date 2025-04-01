using WinUIShell.Common;

namespace WinUIShell;

public class UIElementCollection : WinUIShellObject
{
    public int Count
    {
        get => PropertyAccessor.Get<int>(Id, nameof(Count))!;
    }

    public bool IsReadOnly
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsReadOnly))!;
    }

    public UIElement this[int index]
    {
        get => PropertyAccessor.GetIndexer<UIElement>(Id, index)!;
        set => PropertyAccessor.SetIndexer(Id, index, value);
    }

    internal UIElementCollection(ObjectId id)
        : base(id)
    {
    }

    public void Move(uint oldIndex, uint newIndex)
    {
        CommandClient.Get().InvokeMethod(Id, nameof(Move), oldIndex, newIndex);
    }

    public int IndexOf(UIElement item)
    {
        return CommandClient.Get().InvokeMethodAndGetResult<int>(Id, nameof(IndexOf), item?.Id);
    }

    public void Insert(int index, UIElement item)
    {
        CommandClient.Get().InvokeMethod(Id, nameof(Insert), index, item?.Id);
    }

    public void RemoveAt(int index)
    {
        CommandClient.Get().InvokeMethod(Id, nameof(RemoveAt), index);
    }

    public void Add(UIElement item)
    {
        CommandClient.Get().InvokeMethod(Id, nameof(Add), item?.Id);
    }

    public void Clear()
    {
        CommandClient.Get().InvokeMethod(Id, nameof(Clear));
    }

    public bool Contains(UIElement item)
    {
        return CommandClient.Get().InvokeMethodAndGetResult<bool>(Id, nameof(Contains), item?.Id);
    }

    public bool Remove(UIElement item)
    {
        return CommandClient.Get().InvokeMethodAndGetResult<bool>(Id, nameof(Remove), item?.Id);
    }
}
