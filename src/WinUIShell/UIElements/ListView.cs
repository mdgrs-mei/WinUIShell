using WinUIShell.Common;

namespace WinUIShell;

public class ListView : ListViewBase
{
    public ListView()
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<ListView>(),
            this);
    }

    internal ListView(ObjectId id)
        : base(id)
    {
    }
}
