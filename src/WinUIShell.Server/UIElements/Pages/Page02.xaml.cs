using WinUIShell.Common;

namespace WinUIShell.Server;

internal sealed partial class Page02 : Page, IPage
{
    public ObjectId Id { get; set; } = new();

    public Page02()
    {
        InitializeComponent();
        IPage.Init(this);
    }
}
