using WinUIShell.Common;

namespace WinUIShell;

public class WindowActivatedEventArgs : WinUIShellObject
{
    public bool Handled
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(Handled))!;
        set => PropertyAccessor.SetAndWait(Id, nameof(Handled), value);
    }

    public Microsoft.UI.Xaml.WindowActivationState WindowActivationState
    {
        get => PropertyAccessor.Get<Microsoft.UI.Xaml.WindowActivationState>(Id, nameof(WindowActivationState))!;
    }

    internal WindowActivatedEventArgs(ObjectId id)
        : base(id)
    {
    }
}
