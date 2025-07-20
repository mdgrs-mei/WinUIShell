using WinUIShell.Common;

namespace WinUIShell;

public class RangeBase : Control
{
    public double Maximum
    {
        get => PropertyAccessor.Get<double>(Id, nameof(Maximum))!;
        set => PropertyAccessor.Set(Id, nameof(Maximum), value);
    }

    public double Minimum
    {
        get => PropertyAccessor.Get<double>(Id, nameof(Minimum))!;
        set => PropertyAccessor.Set(Id, nameof(Minimum), value);
    }

    public double Value
    {
        get => PropertyAccessor.Get<double>(Id, nameof(Value))!;
        set => PropertyAccessor.Set(Id, nameof(Value), value);
    }

    internal RangeBase()
    {
    }

    internal RangeBase(ObjectId id)
        : base(id)
    {
    }
}
