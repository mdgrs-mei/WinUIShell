using Microsoft.UI.Xaml.Controls;
using WinUIShell.Common;

namespace WinUIShell.Server;

public partial class Page09 : Page, IPage
{
    public ObjectId Id { get; set; } = new();

    public Page09()
    {
        InitializeComponent();
        IPage.Init(this);
    }
}
