using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUIShell.Common;

namespace WinUIShell.Server;
public interface IPage
{
    ObjectId Id { get; set; }

    static void Initialize<TPage>(TPage page) where TPage : Page, IPage
    {
        ArgumentNullException.ThrowIfNull(page);

        PageStore.Get().RegisterPageInstance(page);
        var pageProperty = PageStore.Get().GetPageProperty(typeof(TPage));

        page.NavigationCacheMode = pageProperty.NavigationCacheMode;
        page.Loaded += CreateOnLoaded(page);
    }

    private static RoutedEventHandler CreateOnLoaded<TPage>(TPage page) where TPage : Page, IPage
    {
        return async (object sender, RoutedEventArgs eventArgs) =>
        {
            var pageProperty = PageStore.Get().GetPageProperty(typeof(TPage));
            var queueId = new CommandQueueId(pageProperty.CallbackQueueThreadId);

            page.Id = CommandClient.Get().CreateObjectWithId(
                queueId,
                "WinUIShell.Page, WinUIShell",
                page);

            var eventArgsId = CommandClient.Get().CreateObject(
                queueId,
                "WinUIShell.RoutedEventArgs, WinUIShell",
                eventArgs);

            await CommandClient.Get().InvokeStaticMethodWaitAsync(
                queueId,
                "WinUIShell.PageStore, WinUIShell",
                "OnLoaded",
                pageProperty.Name,
                page.Id,
                eventArgsId);

            CommandClient.Get().DestroyObject(eventArgsId);
        };
    }
}
