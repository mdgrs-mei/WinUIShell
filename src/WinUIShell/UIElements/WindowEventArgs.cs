using WinUIShell.Common;

namespace WinUIShell;

public class WindowEventArgs : WinUIShellObject
{
    public bool Handled
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(Handled))!;
        set => PropertyAccessor.SetAndWait(Id, nameof(Handled), value);
    }

    internal WindowEventArgs(ObjectId id)
        : base(id)
    {
    }
}
