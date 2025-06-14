using Microsoft.UI.Xaml.Controls;
using WinUIShell.Common;

namespace WinUIShell.Server;

public partial class Page05 : Page, IPage
{
    public ObjectId Id { get; set; } = new();

    public Page05()
    {
        InitializeComponent();
        IPage.Initialize(this);
    }
}
