using WinUIShell.Common;

namespace WinUIShell;

public class WindowVisibilityChangedEventArgs : WinUIShellObject
{
    public bool Handled
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(Handled))!;
        set => PropertyAccessor.SetAndWait(Id, nameof(Handled), value);
    }

    public bool Visible
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(Visible))!;
    }

    internal WindowVisibilityChangedEventArgs(ObjectId id)
        : base(id)
    {
    }
}
