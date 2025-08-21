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
            var queueId = new CommandQueueId(CommandQueueType.RunspaceId, pageProperty.OnLoadedCallbackMainRunspaceId);

            page.Id = CommandClient.Get().CreateObjectWithId(
                queueId,
                "WinUIShell.Page, WinUIShell",
                page);

            var eventArgsId = CommandClient.Get().CreateObjectWithId(
                queueId,
                "WinUIShell.RoutedEventArgs, WinUIShell",
                eventArgs);

            var invokeTask = CommandClient.Get().InvokeStaticMethodWaitAsync(
                queueId,
                "WinUIShell.PageStore, WinUIShell",
                "OnLoaded",
                pageProperty.Name,
                page.Id,
                eventArgsId);

            CommandClient.Get().DestroyObject(queueId, eventArgsId);

            if (pageProperty.OnLoadedCallbackThreadingMode == EventCallbackThreadingMode.MainThreadSyncUI)
            {
                while (!invokeTask.IsCompleted)
                {
                    App.ProcessCommands();
                    Thread.Sleep(Constants.ServerSyncUICommandPolingIntervalMillisecond);
                }
                App.ProcessCommands();
            }
            else
            {
                await invokeTask;
            }
        };
    }
}
