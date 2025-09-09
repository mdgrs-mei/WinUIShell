using WinUIShell.Common;

namespace WinUIShell;

public class ItemIndexRange : WinUIShellObject
{
    public int FirstIndex
    {
        get => PropertyAccessor.Get<int>(Id, nameof(FirstIndex))!;
    }

    public int LastIndex
    {
        get => PropertyAccessor.Get<int>(Id, nameof(LastIndex))!;
    }

    public uint Length
    {
        get => PropertyAccessor.Get<uint>(Id, nameof(Length))!;
    }

    public ItemIndexRange(int firstIndex, uint length)
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<ItemIndexRange>(),
            this,
            firstIndex,
            length);
    }

    internal ItemIndexRange()
    {
    }

    internal ItemIndexRange(ObjectId id)
        : base(id)
    {
    }
}
