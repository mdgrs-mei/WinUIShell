using WinUIShell.Common;

namespace WinUIShell;

public class ColumnDefinition : WinUIShellObject
{
    public double ActualWidth
    {
        get => PropertyAccessor.Get<double>(Id, nameof(ActualWidth))!;
    }

    public double MaxWidth
    {
        get => PropertyAccessor.Get<double>(Id, nameof(MaxWidth))!;
        set => PropertyAccessor.Set(Id, nameof(MaxWidth), value);
    }

    public double MinWidth
    {
        get => PropertyAccessor.Get<double>(Id, nameof(MinWidth))!;
        set => PropertyAccessor.Set(Id, nameof(MinWidth), value);
    }

    public GridLength Width
    {
        get => PropertyAccessor.Get<GridLength>(Id, nameof(Width))!;
        set => PropertyAccessor.Set(Id, nameof(Width), value?.Id);
    }

    public ColumnDefinition()
    {
        Id = CommandClient.Get().CreateObject(
            "Microsoft.UI.Xaml.Controls.ColumnDefinition, Microsoft.WinUI",
            this);
    }
}
