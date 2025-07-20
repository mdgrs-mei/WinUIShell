using WinUIShell.Common;

namespace WinUIShell;

public class IconSource : WinUIShellObject
{
    public Brush Foreground
    {
        get => PropertyAccessor.Get<Brush>(Id, nameof(Foreground))!;
        set => PropertyAccessor.Set(Id, nameof(Foreground), value?.Id);
    }

    internal IconSource()
    {
    }

    internal IconSource(ObjectId id)
        : base(id)
    {
    }
}
