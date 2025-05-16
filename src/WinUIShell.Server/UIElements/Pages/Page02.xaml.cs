using Microsoft.UI.Xaml.Controls;
using WinUIShell.Common;

namespace WinUIShell.Server;

public partial class Page02 : Page, IPage
{
    public ObjectId Id { get; set; } = new();

    public Page02()
    {
        InitializeComponent();
        Loaded += IPage.CreateOnLoaded(this);
        Unloaded += IPage.CreateOnUnloaded(this);
    }
}
