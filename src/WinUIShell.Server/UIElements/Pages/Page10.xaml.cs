using Microsoft.UI.Xaml.Controls;
using WinUIShell.Common;

namespace WinUIShell.Server;

public partial class Page10 : Page, IPage
{
    public ObjectId Id { get; set; } = new();

    public Page10()
    {
        InitializeComponent();
        IPage.Init(this);
    }
}
