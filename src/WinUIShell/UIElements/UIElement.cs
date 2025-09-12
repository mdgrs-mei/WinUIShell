using WinUIShell.Common;

namespace WinUIShell;

public class UIElement : WinUIShellObject
{
    public bool AllowDrop
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(AllowDrop))!;
        set => PropertyAccessor.Set(Id, nameof(AllowDrop), value);
    }

    public bool CanDrag
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(CanDrag))!;
        set => PropertyAccessor.Set(Id, nameof(CanDrag), value);
    }

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

    public Visibility Visibility
    {
        get => PropertyAccessor.Get<Visibility>(Id, nameof(Visibility))!;
        set => PropertyAccessor.Set(Id, nameof(Visibility), value);
    }

    internal UIElement()
    {
    }

    internal UIElement(ObjectId id)
        : base(id)
    {
    }
}
