using Microsoft.UI.Xaml.Controls;
using RpcUIShell.Core;

namespace WinUIShell.Server;

internal sealed partial class Page05 : Page, IPage
{
    public ObjectId Id { get; set; } = new();

    public Page05()
    {
        InitializeComponent();
        IPage.Init(this);
    }
}
