using WinUIShell.Common;

namespace WinUIShell;

public class MicaBackdrop : SystemBackdrop
{
    public MicaKind Kind
    {
        get => PropertyAccessor.Get<MicaKind>(Id, nameof(Kind))!;
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
