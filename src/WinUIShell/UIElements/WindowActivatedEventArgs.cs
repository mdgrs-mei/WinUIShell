using WinUIShell.Common;

namespace WinUIShell;

public class WindowActivatedEventArgs : WinUIShellObject
{
    public bool Handled
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(Handled))!;
        set => PropertyAccessor.SetAndWait(Id, nameof(Handled), value);
    }

    public WindowActivationState WindowActivationState
    {
        get => PropertyAccessor.Get<WindowActivationState>(Id, nameof(WindowActivationState))!;
    }

    internal WindowActivatedEventArgs(ObjectId id)
        : base(id)
    {
    }
}
