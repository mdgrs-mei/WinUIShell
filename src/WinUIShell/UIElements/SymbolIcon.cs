using WinUIShell.Common;

namespace WinUIShell;

public class SymbolIcon : IconElement
{
    public Symbol Symbol
    {
        get => PropertyAccessor.Get<Symbol>(Id, nameof(Symbol))!;
        set => PropertyAccessor.Set(Id, nameof(Symbol), value);
    }

    public SymbolIcon()
    {
        Id = CommandClient.Get().CreateObject(
            "Microsoft.UI.Xaml.Controls.SymbolIcon, Microsoft.WinUI",
            this);
    }

    public SymbolIcon(Symbol symbol)
    {
        Id = CommandClient.Get().CreateObject(
            "Microsoft.UI.Xaml.Controls.SymbolIcon, Microsoft.WinUI",
            this,
            symbol);
    }
}
