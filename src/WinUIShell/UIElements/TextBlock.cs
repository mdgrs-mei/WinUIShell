using WinUIShell.Common;

namespace WinUIShell;

public class TextBlock : FrameworkElement
{
    public string Text
    {
        get => PropertyAccessor.Get<string>(Id, nameof(Text))!;
        set => PropertyAccessor.Set(Id, nameof(Text), value);
    }

    public TextBlock()
    {
        Id = CommandClient.Get().CreateObject(
            "Microsoft.UI.Xaml.Controls.TextBlock, Microsoft.WinUI",
            this);
    }
}
