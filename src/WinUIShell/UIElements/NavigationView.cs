using System.Management.Automation;
using WinUIShell.Common;

namespace WinUIShell;

public class NavigationView : ContentControl
{
    private const string _accessorClassName = "WinUIShell.Server.NavigationViewAccessor, WinUIShell.Server";
    private readonly EventCallbackList _itemInvokedCallbacks = new(_accessorClassName);

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
    //public NavigationViewDisplayMode DisplayMode => INavigationViewMethods.get_DisplayMode(_objRef_global__Microsoft_UI_Xaml_Controls_INavigationView);

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
    //public NavigationViewBackButtonVisible IsBackButtonVisible
    //public bool IsBackEnabled
    //public bool IsPaneOpen
    //public bool IsPaneToggleButtonVisible
    //public bool IsPaneVisible
    //public bool IsSettingsVisible
    //public bool IsTitleBarAutoPaddingEnabled
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
    //public NavigationViewPaneDisplayMode PaneDisplayMode
    //public UIElement PaneFooter
    //public UIElement PaneHeader

    public string PaneTitle
    {
        get => PropertyAccessor.Get<string>(Id, nameof(PaneTitle))!;
        set => PropertyAccessor.Set(Id, nameof(PaneTitle), value);
    }

    //public Style PaneToggleButtonStyle

    public object? SelectedItem
    {
        get => PropertyAccessor.Get<object>(Id, nameof(SelectedItem));
        set
        {
            if (value is WinUIShellObject v)
            {
                PropertyAccessor.Set(Id, nameof(SelectedItem), v.Id);
            }
            else
            {
                PropertyAccessor.Set(Id, nameof(SelectedItem), value);
            }
        }
    }

    //public NavigationViewSelectionFollowsFocus SelectionFollowsFocus
    //public object SettingsItem => INavigationViewMethods.get_SettingsItem(_objRef_global__Microsoft_UI_Xaml_Controls_INavigationView);
    //public NavigationViewShoulderNavigationEnabled ShoulderNavigationEnabled
    //public NavigationViewTemplateSettings TemplateSettings => INavigationView2Methods.get_TemplateSettings(_objRef_global__Microsoft_UI_Xaml_Controls_INavigationView2);

    public NavigationView()
    {
        Id = CommandClient.Get().CreateObject(
            "Microsoft.UI.Xaml.Controls.NavigationView, Microsoft.WinUI",
            this);
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
}
