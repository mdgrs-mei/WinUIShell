using WinUIShell.Common;

namespace WinUIShell.Server;

internal sealed partial class Page04 : Page, IPage
{
    public ObjectId Id { get; set; } = new();

    public Page04()
    {
        InitializeComponent();
        IPage.Init(this);
    }
}
