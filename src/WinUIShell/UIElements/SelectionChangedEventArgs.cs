using WinUIShell.Common;

namespace WinUIShell;

public class SelectionChangedEventArgs : RoutedEventArgs
{
    public WinUIShellObjectList<object> AddedItems
    {
        get => PropertyAccessor.Get<WinUIShellObjectList<object>>(Id, nameof(AddedItems))!;
    }

    public WinUIShellObjectList<object> RemovedItems
    {
        get => PropertyAccessor.Get<WinUIShellObjectList<object>>(Id, nameof(RemovedItems))!;
    }

    internal SelectionChangedEventArgs(ObjectId id)
        : base(id)
    {
    }
}
