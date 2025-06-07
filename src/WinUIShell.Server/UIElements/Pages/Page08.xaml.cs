using Microsoft.UI.Xaml.Controls;
using WinUIShell.Common;

namespace WinUIShell.Server;

public partial class Page08 : Page, IPage
{
    public ObjectId Id { get; set; } = new();

    public Page08()
    {
        InitializeComponent();
        IPage.Initialize(this);
    }
}
