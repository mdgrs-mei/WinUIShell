using WinUIShell.Common;

namespace WinUIShell;

public class UIElement : WinUIShellObject
{
    public double Opacity
    {
        get => PropertyAccessor.Get<double>(Id, nameof(Opacity))!;
        set => PropertyAccessor.Set(Id, nameof(Opacity), value);
    }

    public double RasterizationScale
    {
        get => PropertyAccessor.Get<double>(Id, nameof(RasterizationScale))!;
        set => PropertyAccessor.Set(Id, nameof(RasterizationScale), value);
    }

    internal UIElement()
    {
    }

    internal UIElement(ObjectId id)
        : base(id)
    {
    }
}
