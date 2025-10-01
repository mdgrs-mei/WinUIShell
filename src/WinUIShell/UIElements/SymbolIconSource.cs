using WinUIShell.Common;

namespace WinUIShell;

public class SymbolIconSource : IconSource
{
    public Microsoft.UI.Xaml.Controls.Symbol Symbol
    {
        get => PropertyAccessor.Get<Microsoft.UI.Xaml.Controls.Symbol>(Id, nameof(Symbol))!;
        set => PropertyAccessor.Set(Id, nameof(Symbol), value);
    }

    public SymbolIconSource()
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<SymbolIconSource>(),
            this);
    }

    internal SymbolIconSource(ObjectId id)
        : base(id)
    {
    }
}
