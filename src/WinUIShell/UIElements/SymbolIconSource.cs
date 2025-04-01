using WinUIShell.Common;

namespace WinUIShell;

public class SymbolIconSource : IconSource
{
    public Symbol Symbol
    {
        get => PropertyAccessor.Get<Symbol>(Id, nameof(Symbol))!;
        set => PropertyAccessor.Set(Id, nameof(Symbol), value);
    }

    public SymbolIconSource()
    {
        Id = CommandClient.Get().CreateObject(
            "Microsoft.UI.Xaml.Controls.SymbolIconSource, Microsoft.WinUI",
            this);
    }
}
