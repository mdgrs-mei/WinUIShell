using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUIShell.Common;

namespace WinUIShell.Server;
public interface IPage
{
    ObjectId Id { get; set; }

    static void Init<TPage>(TPage page) where TPage : Page, IPage
    {
        ArgumentNullException.ThrowIfNull(page);

        PageStore.Get().RegisterPageInstance(page);
        var pageProperty = PageStore.Get().GetPageProperty(page.GetType());

        page.NavigationCacheMode = pageProperty.NavigationCacheMode;
        page.Loaded += CreateOnLoaded(page);
    }

    static void Term<TPage>(TPage page) where TPage : IPage
    {
        ArgumentNullException.ThrowIfNull(page);

        var property = PageStore.Get().GetPageProperty(page.GetType());
        var queueId = new CommandQueueId(CommandQueueType.RunspaceId, property.OnLoadedCallbackMainRunspaceId);
        CommandClient.Get().DestroyObject(queueId, page.Id);
        page.Id = new();
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

            CommandClient.Get().DestroyObject(queueId, eventArgsId);
        };
    }
}
