using WinUIShell.Common;

namespace WinUIShell.Server;

internal sealed partial class Page03 : Page, IPage
{
    public ObjectId Id { get; set; } = new();

    public Page03()
    {
        InitializeComponent();
        IPage.Init(this);
    }
}
