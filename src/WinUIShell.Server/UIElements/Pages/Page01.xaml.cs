using Microsoft.UI.Xaml.Controls;
using WinUIShell.Common;

namespace WinUIShell.Server;
public sealed partial class Page01 : Page
{
    public Page01()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var pageProperty = PageStore.Get().GetPageProperty(typeof(Page01));
        await CommandClient.Get().InvokeStaticMethodWaitAsync(
            new CommandQueueId(pageProperty.CallbackQueueThreadId),
            "WinUIShell.PageStore, WinUIShell",
            "OnLoaded",
            pageProperty.Name);
    }
}
