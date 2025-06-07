using WinUIShell.Common;

namespace WinUIShell;

public abstract class NavigationViewItemBase : ContentControl
{
    public bool IsSelected
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsSelected))!;
        set => PropertyAccessor.Set(Id, nameof(IsSelected), value);
    }

    internal NavigationViewItemBase()
    {
    }

    internal NavigationViewItemBase(ObjectId id)
        : base(id)
    {
    }
}
