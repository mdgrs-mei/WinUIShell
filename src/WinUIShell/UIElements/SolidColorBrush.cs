using WinUIShell.Common;

namespace WinUIShell;

public class SolidColorBrush : Brush
{
    public Color Color
    {
        get => PropertyAccessor.Get<Color>(Id, nameof(Color))!;
        set => PropertyAccessor.Set(Id, nameof(Color), value?.Id);
    }

    public SolidColorBrush()
    {
        Id = CommandClient.Get().CreateObject(
            "Microsoft.UI.Xaml.Media.SolidColorBrush, Microsoft.WinUI",
            this);
    }

    public SolidColorBrush(Color color)
    {
        Id = CommandClient.Get().CreateObject(
            "Microsoft.UI.Xaml.Media.SolidColorBrush, Microsoft.WinUI",
            this,
            color?.Id);
    }
}
