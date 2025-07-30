using WinUIShell.Common;

namespace WinUIShell;

public class TextBoxBeforeTextChangingEventArgs : WinUIShellObject
{
    public bool Cancel
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(Cancel))!;
        set => PropertyAccessor.SetAndWait(Id, nameof(Cancel), value);
    }

    public string NewText
    {
        get => PropertyAccessor.Get<string>(Id, nameof(NewText))!;
    }

    internal TextBoxBeforeTextChangingEventArgs(ObjectId id)
        : base(id)
    {
    }
}
