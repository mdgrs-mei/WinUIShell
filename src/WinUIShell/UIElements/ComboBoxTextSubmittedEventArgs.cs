using WinUIShell.Common;

namespace WinUIShell;

public class ComboBoxTextSubmittedEventArgs : WinUIShellObject
{
    public bool Handled
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(Handled))!;
        set => PropertyAccessor.SetAndWait(Id, nameof(Handled), value);
    }

    public string Text
    {
        get => PropertyAccessor.Get<string>(Id, nameof(Text))!;
    }

    internal ComboBoxTextSubmittedEventArgs(ObjectId id)
        : base(id)
    {
    }
}
