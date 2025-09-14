using WinUIShell.Common;

namespace WinUIShell;

public class GridView : ListViewBase
{
    public GridView()
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<GridView>(),
            this);
    }

    internal GridView(ObjectId id)
        : base(id)
    {
    }
}
