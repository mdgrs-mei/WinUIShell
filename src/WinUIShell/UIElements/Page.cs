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

    internal Page()
    {
    }

    internal Page(ObjectId id)
    : base(id)
    {
    }
}
