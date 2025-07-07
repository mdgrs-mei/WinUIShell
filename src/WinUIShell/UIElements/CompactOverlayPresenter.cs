using WinUIShell.Common;

namespace WinUIShell;

public class CompactOverlayPresenter : AppWindowPresenter
{
    public CompactOverlaySize InitialSize
    {
        get => PropertyAccessor.Get<CompactOverlaySize>(Id, nameof(InitialSize));
        set => PropertyAccessor.Set(Id, nameof(InitialSize), value);
    }

    public CompactOverlayPresenter()
    {
        Id = CommandClient.Get().CreateObjectWithStaticMethod(
            ObjectTypeMapping.Get().GetTargetTypeName<CompactOverlayPresenter>(),
            "Create",
            this);
    }
}
