using WinUIShell.Common;

namespace WinUIShell;

public class CompactOverlayPresenter : AppWindowPresenter
{
    public Microsoft.UI.Windowing.CompactOverlaySize InitialSize
    {
        get => PropertyAccessor.Get<Microsoft.UI.Windowing.CompactOverlaySize>(Id, nameof(InitialSize));
        set => PropertyAccessor.Set(Id, nameof(InitialSize), value);
    }

    public CompactOverlayPresenter()
    {
        Id = CommandClient.Get().CreateObjectWithStaticMethod(
            ObjectTypeMapping.Get().GetTargetTypeName<CompactOverlayPresenter>(),
            "Create",
            this);
    }

    internal CompactOverlayPresenter(ObjectId id)
        : base(id)
    {
    }
}
