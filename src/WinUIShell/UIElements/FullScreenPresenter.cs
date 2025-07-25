using WinUIShell.Common;

namespace WinUIShell;

public class FullScreenPresenter : AppWindowPresenter
{
    public FullScreenPresenter()
    {
        Id = CommandClient.Get().CreateObjectWithStaticMethod(
            ObjectTypeMapping.Get().GetTargetTypeName<FullScreenPresenter>(),
            "Create",
            this);
    }

    internal FullScreenPresenter(ObjectId id)
        : base(id)
    {
    }
}
