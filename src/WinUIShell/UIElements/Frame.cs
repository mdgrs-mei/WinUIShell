using System.Management.Automation;
using System.Management.Automation.Runspaces;
using WinUIShell.Common;
using WinUIShell.Generator;

namespace WinUIShell.Microsoft.UI.Xaml.Controls;

[SurpressGeneratorMethodByName("NavigateToType")]
public partial class Frame : ContentControl
{
    private const string _accessorClassName = "WinUIShell.Server.FrameAccessor, WinUIShell.Server";

    public string SourcePageName
    {
        get => CommandClient.Get().InvokeStaticMethodAndGetResult<string, string>(
            _accessorClassName,
            "GetSourcePageName",
            WinUIShellObjectId)!;
    }

    [SurpressGeneratorMethodByName]
    public bool Navigate(
        string pageName,
        Media.Animation.NavigationTransitionInfo? transitionOverride,
        Navigation.NavigationCacheMode cacheMode,
        ScriptBlock onLoaded,
        object? onLoadedArgumentList = null)
    {
        return Navigate(pageName, transitionOverride, cacheMode, new EventCallback
        {
            ScriptBlock = onLoaded,
            ArgumentList = onLoadedArgumentList
        });
    }

    public bool Navigate(
        string pageName,
        Media.Animation.NavigationTransitionInfo? transitionOverride,
        Navigation.NavigationCacheMode cacheMode,
        EventCallback onLoaded)
    {
        ArgumentNullException.ThrowIfNull(onLoaded);
        PageStore.Get().RegisterLoaded(pageName, onLoaded);
        return CommandClient.Get().InvokeStaticMethodAndGetResult<bool, bool>(
            _accessorClassName,
            nameof(Navigate),
            WinUIShellObjectId,
            onLoaded.RunspaceMode,
            Runspace.DefaultRunspace.Id,
            pageName,
            transitionOverride?.WinUIShellObjectId,
            cacheMode);
    }
}
