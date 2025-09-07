using Microsoft.UI.Xaml.Controls;
using WinUIShell.Common;

namespace WinUIShell.Server;

public partial class Page03 : Page, IPage
{
    public ObjectId Id { get; set; } = new();

    public Page03()
    {
        InitializeComponent();
        IPage.Init(this);
    }
}
