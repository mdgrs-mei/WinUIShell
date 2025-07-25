using WinUIShell.Common;

namespace WinUIShell;

public class Button : ButtonBase
{
    public FlyoutBase? Flyout
    {
        get => PropertyAccessor.Get<FlyoutBase>(Id, nameof(Flyout));
        set => PropertyAccessor.Set(Id, nameof(Flyout), value?.Id);
    }

    public Button()
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<Button>(),
            this);
    }

    internal Button(ObjectId id)
        : base(id)
    {
    }
}
