using Microsoft.UI.Xaml.Controls;
using WinUIShell.Common;

namespace WinUIShell.Server;

public partial class Page04 : Page, IPage
{
    public ObjectId Id { get; set; } = new();

    public Page04()
    {
        InitializeComponent();
        IPage.Initialize(this);
    }
}
