namespace WinUIShell;

public class NavigationViewItemBase : ContentControl
{
    public bool IsSelected
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsSelected))!;
        set => PropertyAccessor.Set(Id, nameof(IsSelected), value);
    }
}
