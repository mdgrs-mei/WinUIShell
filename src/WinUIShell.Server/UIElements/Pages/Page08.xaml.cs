using WinUIShell.Common;

namespace WinUIShell.Server;

internal sealed partial class Page08 : Page, IPage
{
    public ObjectId Id { get; set; } = new();

    public Page08()
    {
        InitializeComponent();
        IPage.Init(this);
    }
}
