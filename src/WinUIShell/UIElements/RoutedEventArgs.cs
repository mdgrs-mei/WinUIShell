using WinUIShell.Common;

namespace WinUIShell;

public class RoutedEventArgs : WinUIShellObject
{
    public WinUIShellObject OriginalSource
    {
        get => PropertyAccessor.Get<WinUIShellObject>(Id, nameof(OriginalSource))!;
    }

    internal RoutedEventArgs(ObjectId id)
        : base(id)
    {
    }
}
