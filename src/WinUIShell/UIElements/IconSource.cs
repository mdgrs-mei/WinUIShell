namespace WinUIShell;

public abstract class IconSource : WinUIShellObject
{
    public Brush Foreground
    {
        get => PropertyAccessor.Get<Brush>(Id, nameof(Foreground))!;
        set => PropertyAccessor.Set(Id, nameof(Foreground), value?.Id);
    }
}
