using Microsoft.UI.Xaml.Controls;
using RpcUIShell.Core;

namespace WinUIShell.Server;

internal sealed partial class Page09 : Page, IPage
{
    public ObjectId Id { get; set; } = new();

    public Page09()
    {
        InitializeComponent();
        IPage.Init(this);
    }
}
