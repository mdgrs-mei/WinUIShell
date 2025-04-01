using WinUIShell.Common;

namespace WinUIShell;

public class RoutedEventArgs
{
    public WinUIShellObject OriginalSource
    {
        get
        {
            var id = ObjectStore.Get().GetId(this);
            return PropertyAccessor.Get<WinUIShellObject>(id, nameof(OriginalSource))!;
        }
    }
}
