using WinUIShell.Common;

namespace WinUIShell;

public class ItemClickEventArgs : RoutedEventArgs
{
    public object ClickedItem
    {
        get => PropertyAccessor.Get<object>(Id, nameof(ClickedItem))!;
    }

    internal ItemClickEventArgs(ObjectId id)
        : base(id)
    {
    }
}
