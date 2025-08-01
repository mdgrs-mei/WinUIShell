using WinUIShell.Common;

namespace WinUIShell;

public class Page : UserControl
{
    //public AppBar BottomAppBar

    public Frame? Frame
    {
        get => PropertyAccessor.Get<Frame>(Id, nameof(Frame));
    }

    //public NavigationCacheMode NavigationCacheMode
    //public AppBar TopAppBar

    public Page()
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<Page>(),
            this);
    }

    internal Page(ObjectId id)
    : base(id)
    {
    }
}
