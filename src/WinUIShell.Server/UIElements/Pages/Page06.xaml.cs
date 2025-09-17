using WinUIShell.Common;

namespace WinUIShell.Server;

internal sealed partial class Page06 : Page, IPage
{
    public ObjectId Id { get; set; } = new();

    public Page06()
    {
        InitializeComponent();
        IPage.Init(this);
    }
}
