using System.Management.Automation;
using System.Management.Automation.Runspaces;
using WinUIShell.Common;

namespace WinUIShell;

public class Frame : ContentControl
{
    private const string _accessorClassName = "WinUIShell.Server.FrameAccessor, WinUIShell.Server";
    private readonly EventCallbackList _callbacks = new();

    //public IList<PageStackEntry> BackStack => IFrameMethods.get_BackStack(_objRef_global__Microsoft_UI_Xaml_Controls_IFrame);
    //public int BackStackDepth => IFrameMethods.get_BackStackDepth(_objRef_global__Microsoft_UI_Xaml_Controls_IFrame);

    public int CacheSize
    {
        get => PropertyAccessor.Get<int>(Id, nameof(CacheSize))!;
        set => PropertyAccessor.Set(Id, nameof(CacheSize), value);
    }

    public bool CanGoBack
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(CanGoBack))!;
    }

    public bool CanGoForward
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(CanGoForward))!;
    }

    //public Type CurrentSourcePageType => IFrameMethods.get_CurrentSourcePageType(_objRef_global__Microsoft_UI_Xaml_Controls_IFrame);
    //public IList<PageStackEntry> ForwardStack => IFrameMethods.get_ForwardStack(_objRef_global__Microsoft_UI_Xaml_Controls_IFrame);

    public bool IsNavigationStackEnabled
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsNavigationStackEnabled))!;
        set => PropertyAccessor.Set(Id, nameof(IsNavigationStackEnabled), value);
    }

    //public Type SourcePageType
    public string SourcePageName
    {
        get
        {
            return CommandClient.Get().InvokeStaticMethodAndGetResult<string>(
                _accessorClassName,
                "GetSourcePageName",
                Id)!;
        }
    }

    public Frame()
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<Frame>(),
            this);
    }

    internal Frame(ObjectId id)
        : base(id)
    {
    }

    public void GoBack()
    {
        CommandClient.Get().InvokeMethod(Id, nameof(GoBack));
    }

    public void GoForward()
    {
        CommandClient.Get().InvokeMethod(Id, nameof(GoForward));
    }

    public bool Navigate(
        string pageName,
        NavigationTransitionInfo? transitionOverride,
        Microsoft.UI.Xaml.Navigation.NavigationCacheMode cacheMode,
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
        NavigationTransitionInfo? transitionOverride,
        Microsoft.UI.Xaml.Navigation.NavigationCacheMode cacheMode,
        EventCallback onLoaded)
    {
        ArgumentNullException.ThrowIfNull(onLoaded);
        PageStore.Get().RegisterLoaded(pageName, onLoaded);
        return CommandClient.Get().InvokeStaticMethodAndGetResult<bool>(
            _accessorClassName,
            nameof(Navigate),
            Id,
            onLoaded.RunspaceMode,
            Runspace.DefaultRunspace.Id,
            pageName,
            transitionOverride?.Id,
            cacheMode);
    }

    //public bool NavigateToType(Type sourcePageType, object parameter, FrameNavigationOptions navigationOptions)
    //public string GetNavigationState()
    //public void SetNavigationState(string navigationState)
    //public void SetNavigationState(string navigationState, bool suppressNavigate)

    public void AddNavigated(ScriptBlock scriptBlock, object? argumentList = null)
    {
        AddNavigated(new EventCallback
        {
            ScriptBlock = scriptBlock,
            ArgumentList = argumentList
        });
    }
    public void AddNavigated(EventCallback eventCallback)
    {
        _callbacks.Add(
            Id,
            "Navigated",
            ObjectTypeMapping.Get().GetTargetTypeName<NavigationEventArgs>(),
            eventCallback);
    }
}
