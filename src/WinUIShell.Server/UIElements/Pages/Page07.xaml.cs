using WinUIShell.Common;

namespace WinUIShell.Server;

internal sealed partial class Page07 : Page, IPage
{
    public ObjectId Id { get; set; } = new();

    public Page07()
    {
        InitializeComponent();
        IPage.Init(this);
    }
}
