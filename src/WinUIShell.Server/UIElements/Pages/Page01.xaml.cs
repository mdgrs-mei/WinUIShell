using Microsoft.UI.Xaml.Controls;
using WinUIShell.Common;

namespace WinUIShell.Server;
public sealed partial class Page01 : Page
{
    private ObjectId _id = new();

    public Page01()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private async void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs eventArgs)
    {
        var pageProperty = PageStore.Get().GetPageProperty(typeof(Page01));
        var queueId = new CommandQueueId(pageProperty.CallbackQueueThreadId);

        _id = CommandClient.Get().CreateObjectWithId(
            queueId,
            "WinUIShell.Page, WinUIShell",
            this);

        var eventArgsId = CommandClient.Get().CreateObject(
            queueId,
            "WinUIShell.RoutedEventArgs, WinUIShell",
            eventArgs);

        await CommandClient.Get().InvokeStaticMethodWaitAsync(
            queueId,
            "WinUIShell.PageStore, WinUIShell",
            "OnLoaded",
            pageProperty.Name,
            _id,
            eventArgsId);

        CommandClient.Get().DestroyObject(eventArgsId);
    }

    private void OnUnloaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs eventArgs)
    {
        CommandClient.Get().DestroyObject(_id);
        _id = new();
    }
}
