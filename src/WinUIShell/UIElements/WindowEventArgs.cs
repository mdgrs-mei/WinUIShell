using WinUIShell.Common;

namespace WinUIShell;

public class WindowEventArgs
{
    public bool Handled
    {
        get
        {
            var id = ObjectStore.Get().GetId(this);
            return PropertyAccessor.Get<bool>(id, nameof(Handled))!;
        }
        set
        {
            var id = ObjectStore.Get().GetId(this);
            PropertyAccessor.SetAndWait(id, nameof(Handled), value);
        }
    }
}
