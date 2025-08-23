using Microsoft.UI.Xaml.Controls;
using WinUIShell.Common;

namespace WinUIShell.Server;

public partial class Page01 : Page, IPage
{
    public ObjectId Id { get; set; } = new();

    public Page01()
    {
        InitializeComponent();
        IPage.Init(this);
    }
}
