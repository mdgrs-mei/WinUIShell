using Microsoft.UI.Xaml.Controls;
using WinUIShell.Common;

namespace WinUIShell.Server;

public partial class Page07 : Page, IPage
{
    public ObjectId Id { get; set; } = new();

    public Page07()
    {
        InitializeComponent();
        IPage.Initialize(this);
    }
}
