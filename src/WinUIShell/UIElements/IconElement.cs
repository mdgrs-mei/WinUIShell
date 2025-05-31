using WinUIShell.Common;

namespace WinUIShell;

public class IconElement : FrameworkElement
{
    public Brush Foreground
    {
        get => PropertyAccessor.Get<Brush>(Id, nameof(Foreground))!;
        set => PropertyAccessor.Set(Id, nameof(Foreground), value?.Id);
    }

    internal IconElement()
    {
    }

    internal IconElement(ObjectId id)
        : base(id)
    {
    }
}
