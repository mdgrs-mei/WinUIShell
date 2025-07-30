using WinUIShell.Common;

namespace WinUIShell;

public class NavigationViewBackRequestedEventArgs : WinUIShellObject
{
    internal NavigationViewBackRequestedEventArgs(ObjectId id)
        : base(id)
    {
    }
}
