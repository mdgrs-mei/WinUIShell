using WinUIShell.Common;

namespace WinUIShell;

public class MicaBackdrop : SystemBackdrop
{
    public Microsoft.UI.Composition.SystemBackdrops.MicaKind Kind
    {
        get => PropertyAccessor.Get<Microsoft.UI.Composition.SystemBackdrops.MicaKind>(Id, nameof(Kind))!;
        set => PropertyAccessor.Set(Id, nameof(Kind), value);
    }

    public MicaBackdrop()
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<MicaBackdrop>(),
            this);
    }

    internal MicaBackdrop(ObjectId id)
        : base(id)
    {
    }
}
