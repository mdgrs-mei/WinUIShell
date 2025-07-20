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
            ObjectTypeMapping.Get().GetTargetTypeName<SymbolIcon>(),
            this);
    }

    public SymbolIcon(Symbol symbol)
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
