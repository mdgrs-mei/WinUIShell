using System.Management.Automation;
using WinUIShell.Common;

namespace WinUIShell;

public class NavigationView : ContentControl
{
    private const string _accessorClassName = "WinUIShell.Server.NavigationViewAccessor, WinUIShell.Server";
    private readonly EventCallbackList _itemInvokedCallbacks = new(_accessorClassName);
    private readonly EventCallbackList _backRequestedCallbacks = new(_accessorClassName);

    public bool AlwaysShowHeader
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(AlwaysShowHeader))!;
        set => PropertyAccessor.Set(Id, nameof(AlwaysShowHeader), value);
    }

    //public AutoSuggestBox AutoSuggestBox

    public double CompactModeThresholdWidth
    {
        get => PropertyAccessor.Get<double>(Id, nameof(CompactModeThresholdWidth))!;
        set => PropertyAccessor.Set(Id, nameof(CompactModeThresholdWidth), value);
    }

    public double CompactPaneLength
    {
        get => PropertyAccessor.Get<double>(Id, nameof(CompactPaneLength))!;
        set => PropertyAccessor.Set(Id, nameof(CompactPaneLength), value);
    }

    //public UIElement ContentOverlay
    public NavigationViewDisplayMode DisplayMode
    {
        get => PropertyAccessor.Get<NavigationViewDisplayMode>(Id, nameof(DisplayMode))!;
    }

    public double ExpandedModeThresholdWidth
    {
        get => PropertyAccessor.Get<double>(Id, nameof(ExpandedModeThresholdWidth))!;
        set => PropertyAccessor.Set(Id, nameof(ExpandedModeThresholdWidth), value);
    }

    //public IList<object> FooterMenuItems => INavigationViewMethods.get_FooterMenuItems(_objRef_global__Microsoft_UI_Xaml_Controls_INavigationView);
    //public object FooterMenuItemsSource

    public object? Header
    {
        get => PropertyAccessor.Get<object>(Id, nameof(Header));
        set
        {
            if (value is WinUIShellObject v)
            {
                PropertyAccessor.Set(Id, nameof(Header), v.Id);
            }
            else
            {
                PropertyAccessor.Set(Id, nameof(Header), value);
            }
        }
    }

    //public DataTemplate HeaderTemplate
    public NavigationViewBackButtonVisible IsBackButtonVisible
    {
        get => PropertyAccessor.Get<NavigationViewBackButtonVisible>(Id, nameof(IsBackButtonVisible))!;
        set => PropertyAccessor.Set(Id, nameof(IsBackButtonVisible), value);
    }

    public bool IsBackEnabled
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsBackEnabled))!;
        set => PropertyAccessor.Set(Id, nameof(IsBackEnabled), value);
    }

    public bool IsPaneOpen
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsPaneOpen))!;
        set => PropertyAccessor.Set(Id, nameof(IsPaneOpen), value);
    }

    public bool IsPaneToggleButtonVisible
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsPaneToggleButtonVisible))!;
        set => PropertyAccessor.Set(Id, nameof(IsPaneToggleButtonVisible), value);
    }

    public bool IsPaneVisible
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsPaneVisible))!;
        set => PropertyAccessor.Set(Id, nameof(IsPaneVisible), value);
    }

    public bool IsSettingsVisible
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsSettingsVisible))!;
        set => PropertyAccessor.Set(Id, nameof(IsSettingsVisible), value);
    }

    public bool IsTitleBarAutoPaddingEnabled
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsTitleBarAutoPaddingEnabled))!;
        set => PropertyAccessor.Set(Id, nameof(IsTitleBarAutoPaddingEnabled), value);
    }

    //public Style MenuItemContainerStyle
    //public StyleSelector MenuItemContainerStyleSelector
    //public DataTemplate MenuItemTemplate
    //public DataTemplateSelector MenuItemTemplateSelector

    public WinUIShellObjectList<object> MenuItems
    {
        get => PropertyAccessor.Get<WinUIShellObjectList<object>>(Id, nameof(MenuItems))!;
    }

    //public object MenuItemsSource
    //public double OpenPaneLength
    //public NavigationViewOverflowLabelMode OverflowLabelMode
    //public UIElement PaneCustomContent

    public NavigationViewPaneDisplayMode PaneDisplayMode
    {
        get => PropertyAccessor.Get<NavigationViewPaneDisplayMode>(Id, nameof(PaneDisplayMode))!;
        set => PropertyAccessor.Set(Id, nameof(PaneDisplayMode), value);
    }

    //public UIElement PaneFooter
    //public UIElement PaneHeader

    public string PaneTitle
    {
        get => PropertyAccessor.Get<string>(Id, nameof(PaneTitle))!;
        set => PropertyAccessor.Set(Id, nameof(PaneTitle), value);
    }

    //public Style PaneToggleButtonStyle

    public NavigationViewItem? SelectedItem
    {
        get => PropertyAccessor.Get<NavigationViewItem>(Id, nameof(SelectedItem));
        set => PropertyAccessor.Set(Id, nameof(SelectedItem), value?.Id);
    }

    //public NavigationViewSelectionFollowsFocus SelectionFollowsFocus

    public NavigationViewItem? SettingsItem
    {
        get => PropertyAccessor.Get<NavigationViewItem>(Id, nameof(SettingsItem));
    }

    //public NavigationViewShoulderNavigationEnabled ShoulderNavigationEnabled
    //public NavigationViewTemplateSettings TemplateSettings => INavigationView2Methods.get_TemplateSettings(_objRef_global__Microsoft_UI_Xaml_Controls_INavigationView2);

    public NavigationView()
    {
        Id = CommandClient.Get().CreateObject(
            ObjectTypeMapping.Get().GetTargetTypeName<NavigationView>(),
            this);
    }

    internal NavigationView(ObjectId id)
        : base(id)
    {
    }

    public void AddItemInvoked(ScriptBlock scriptBlock, object? argumentList = null)
    {
        AddItemInvoked(new EventCallback
        {
            ScriptBlock = scriptBlock,
            ArgumentList = argumentList
        });
    }
    public void AddItemInvoked(EventCallback eventCallback)
    {
        _itemInvokedCallbacks.Add(
            nameof(AddItemInvoked),
            Id,
            eventCallback);
    }

    public void AddBackRequested(ScriptBlock scriptBlock, object? argumentList = null)
    {
        AddBackRequested(new EventCallback
        {
            ScriptBlock = scriptBlock,
            ArgumentList = argumentList
        });
    }
    public void AddBackRequested(EventCallback eventCallback)
    {
        _backRequestedCallbacks.Add(
            nameof(AddBackRequested),
            Id,
            eventCallback);
    }
}
