using WinUIShell.Common;

namespace WinUIShell;

public class WindowSizeChangedEventArgs : WinUIShellObject
{
    public bool Handled
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(Handled))!;
        set => PropertyAccessor.SetAndWait(Id, nameof(Handled), value);
    }

    public Size Size
    {
        get => PropertyAccessor.Get<Size>(Id, nameof(Size))!;
    }

    internal WindowSizeChangedEventArgs(ObjectId id)
        : base(id)
    {
    }
}
