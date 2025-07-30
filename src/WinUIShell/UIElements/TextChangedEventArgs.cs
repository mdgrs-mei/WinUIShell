using WinUIShell.Common;

namespace WinUIShell;

public class TextChangedEventArgs : RoutedEventArgs
{
    internal TextChangedEventArgs(ObjectId id)
        : base(id)
    {
    }
}
