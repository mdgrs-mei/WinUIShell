using WinUIShell.Common;

namespace WinUIShell;

public class SymbolIcon : IconElement
{
    public Microsoft.UI.Xaml.Controls.Symbol Symbol
    {
        get => PropertyAccessor.Get<Microsoft.UI.Xaml.Controls.Symbol>(Id, nameof(Symbol))!;
        set => PropertyAccessor.Set(Id, nameof(Symbol), value);
    }

    public SymbolIcon()
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<SymbolIcon>(),
            this);
    }

    public SymbolIcon(Microsoft.UI.Xaml.Controls.Symbol symbol)
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<SymbolIcon>(),
            this,
            symbol);
    }

    internal SymbolIcon(ObjectId id)
        : base(id)
    {
    }
}
