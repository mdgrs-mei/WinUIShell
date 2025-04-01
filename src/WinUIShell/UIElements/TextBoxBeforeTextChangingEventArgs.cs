using WinUIShell.Common;

namespace WinUIShell;

public class TextBoxBeforeTextChangingEventArgs
{
    public bool Cancel
    {
        get
        {
            var id = ObjectStore.Get().GetId(this);
            return PropertyAccessor.Get<bool>(id, nameof(Cancel))!;
        }
        set
        {
            var id = ObjectStore.Get().GetId(this);
            PropertyAccessor.SetAndWait(id, nameof(Cancel), value);
        }
    }

    public string NewText
    {
        get
        {
            var id = ObjectStore.Get().GetId(this);
            return PropertyAccessor.Get<string>(id, nameof(NewText))!;
        }
    }
}
