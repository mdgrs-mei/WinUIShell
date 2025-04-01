using WinUIShell.Common;

namespace WinUIShell;

public class Brush : WinUIShellObject
{
    public double Opacity
    {
        get => PropertyAccessor.Get<double>(Id, nameof(Opacity))!;
        set => PropertyAccessor.Set(Id, nameof(Opacity), value);
    }

    internal Brush()
    {
    }
    internal Brush(ObjectId id)
    : base(id)
    {
    }
}
