namespace WinUIShell.Common;

public sealed class ObjectTypeMapping : Singleton<ObjectTypeMapping>
{
    private readonly Dictionary<string, string> _map = [];

    private readonly List<(string, string)> _listWinUI = [
        ("WinUIShell.Application, WinUIShell", "Microsoft.UI.Xaml.Application, Microsoft.WinUI"),
        ("WinUIShell.AppWindow, WinUIShell", "Microsoft.UI.Windowing.AppWindow, Microsoft.InteractiveExperiences.Projection"),
        ("WinUIShell.AppWindowPresenter, WinUIShell", "Microsoft.UI.Windowing.AppWindowPresenter, Microsoft.InteractiveExperiences.Projection"),
        ("WinUIShell.AppWindowTitleBar, WinUIShell", "Microsoft.UI.Windowing.AppWindowTitleBar, Microsoft.InteractiveExperiences.Projection"),
        ("WinUIShell.BitmapImage, WinUIShell", "Microsoft.UI.Xaml.Media.Imaging.BitmapImage, Microsoft.WinUI"),
        ("WinUIShell.BitmapSource, WinUIShell", "Microsoft.UI.Xaml.Media.Imaging.BitmapSource, Microsoft.WinUI"),
        ("WinUIShell.Brush, WinUIShell", "Microsoft.UI.Xaml.Media.Brush, Microsoft.WinUI"),
        ("WinUIShell.Button, WinUIShell", "Microsoft.UI.Xaml.Controls.Button, Microsoft.WinUI"),
        ("WinUIShell.ButtonBase, WinUIShell", "Microsoft.UI.Xaml.Controls.Primitives.ButtonBase, Microsoft.WinUI"),
        ("WinUIShell.Color, WinUIShell", "Windows.UI.Color, Microsoft.Windows.SDK.NET"),
        ("WinUIShell.Colors, WinUIShell", "Microsoft.UI.Colors, Microsoft.InteractiveExperiences.Projection"),
        ("WinUIShell.ColumnDefinition, WinUIShell", "Microsoft.UI.Xaml.Controls.ColumnDefinition, Microsoft.WinUI"),
        ("WinUIShell.ColumnDefinitionCollection, WinUIShell", "Microsoft.UI.Xaml.Controls.ColumnDefinitionCollection, Microsoft.WinUI"),
        ("WinUIShell.ComboBox, WinUIShell", "Microsoft.UI.Xaml.Controls.ComboBox, Microsoft.WinUI"),
        ("WinUIShell.ComboBoxTextSubmittedEventArgs, WinUIShell", "Microsoft.UI.Xaml.Controls.ComboBoxTextSubmittedEventArgs, Microsoft.WinUI"),
        ("WinUIShell.CompactOverlayPresenter, WinUIShell", "Microsoft.UI.Windowing.CompactOverlayPresenter, Microsoft.InteractiveExperiences.Projection"),
        ("WinUIShell.ContentControl, WinUIShell", "Microsoft.UI.Xaml.Controls.ContentControl, Microsoft.WinUI"),
        ("WinUIShell.Control, WinUIShell", "Microsoft.UI.Xaml.Controls.Control, Microsoft.WinUI"),
        ("WinUIShell.CornerRadius, WinUIShell", "Microsoft.UI.Xaml.CornerRadius, Microsoft.WinUI"),
        ("WinUIShell.DesktopAcrylicBackdrop, WinUIShell", "Microsoft.UI.Xaml.Media.DesktopAcrylicBackdrop, Microsoft.WinUI"),
        ("WinUIShell.DrillInNavigationTransitionInfo, WinUIShell", "Microsoft.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo, Microsoft.WinUI"),
        ("WinUIShell.EntranceNavigationTransitionInfo, WinUIShell", "Microsoft.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo, Microsoft.WinUI"),
        ("WinUIShell.FlyoutBase, WinUIShell", "Microsoft.UI.Xaml.Controls.Primitives.FlyoutBase, Microsoft.WinUI"),
        ("WinUIShell.FontFamily, WinUIShell", "Microsoft.UI.Xaml.Media.FontFamily, Microsoft.WinUI"),
        ("WinUIShell.FontWeight, WinUIShell", "Windows.UI.Text.FontWeight, Microsoft.Windows.SDK.NET"),
        ("WinUIShell.FontWeights, WinUIShell", "Microsoft.UI.Text.FontWeights, Microsoft.WinUI"),
        ("WinUIShell.Frame, WinUIShell", "Microsoft.UI.Xaml.Controls.Frame, Microsoft.WinUI"),
        ("WinUIShell.FrameworkElement, WinUIShell", "Microsoft.UI.Xaml.FrameworkElement, Microsoft.WinUI"),
        ("WinUIShell.FullScreenPresenter, WinUIShell", "Microsoft.UI.Windowing.FullScreenPresenter, Microsoft.InteractiveExperiences.Projection"),
        ("WinUIShell.Grid, WinUIShell", "Microsoft.UI.Xaml.Controls.Grid, Microsoft.WinUI"),
        ("WinUIShell.GridLength, WinUIShell", "Microsoft.UI.Xaml.GridLength, Microsoft.WinUI"),
        ("WinUIShell.GridView, WinUIShell", "Microsoft.UI.Xaml.Controls.GridView, Microsoft.WinUI"),
        ("WinUIShell.HyperlinkButton, WinUIShell", "Microsoft.UI.Xaml.Controls.HyperlinkButton, Microsoft.WinUI"),
        ("WinUIShell.IconElement, WinUIShell", "Microsoft.UI.Xaml.Controls.IconElement, Microsoft.WinUI"),
        ("WinUIShell.IconSource, WinUIShell", "Microsoft.UI.Xaml.Controls.IconSource, Microsoft.WinUI"),
        ("WinUIShell.ImageIcon, WinUIShell", "Microsoft.UI.Xaml.Controls.ImageIcon, Microsoft.WinUI"),
        ("WinUIShell.ImageSource, WinUIShell", "Microsoft.UI.Xaml.Media.ImageSource, Microsoft.WinUI"),
        ("WinUIShell.ItemClickEventArgs, WinUIShell", "Microsoft.UI.Xaml.Controls.ItemClickEventArgs, Microsoft.WinUI"),
        ("WinUIShell.ItemCollection, WinUIShell", "Microsoft.UI.Xaml.Controls.ItemCollection, Microsoft.WinUI"),
        ("WinUIShell.ItemIndexRange, WinUIShell", "Microsoft.UI.Xaml.Data.ItemIndexRange, Microsoft.WinUI"),
        ("WinUIShell.ItemsControl, WinUIShell", "Microsoft.UI.Xaml.Controls.ItemsControl, Microsoft.WinUI"),
        ("WinUIShell.KeyRoutedEventArgs, WinUIShell", "Microsoft.UI.Xaml.Input.KeyRoutedEventArgs, Microsoft.WinUI"),
        ("WinUIShell.ListBox, WinUIShell", "Microsoft.UI.Xaml.Controls.ListBox, Microsoft.WinUI"),
        ("WinUIShell.ListView, WinUIShell", "Microsoft.UI.Xaml.Controls.ListView, Microsoft.WinUI"),
        ("WinUIShell.ListViewBase, WinUIShell", "Microsoft.UI.Xaml.Controls.ListViewBase, Microsoft.WinUI"),
        ("WinUIShell.MenuFlyout, WinUIShell", "Microsoft.UI.Xaml.Controls.MenuFlyout, Microsoft.WinUI"),
        ("WinUIShell.MenuFlyoutItem, WinUIShell", "Microsoft.UI.Xaml.Controls.MenuFlyoutItem, Microsoft.WinUI"),
        ("WinUIShell.MenuFlyoutItemBase, WinUIShell", "Microsoft.UI.Xaml.Controls.MenuFlyoutItemBase, Microsoft.WinUI"),
        ("WinUIShell.MicaBackdrop, WinUIShell", "Microsoft.UI.Xaml.Media.MicaBackdrop, Microsoft.WinUI"),
        ("WinUIShell.NavigationEventArgs, WinUIShell", "Microsoft.UI.Xaml.Navigation.NavigationEventArgs, Microsoft.WinUI"),
        ("WinUIShell.NavigationTransitionInfo, WinUIShell", "Microsoft.UI.Xaml.Media.Animation.NavigationTransitionInfo, Microsoft.WinUI"),
        ("WinUIShell.NavigationView, WinUIShell", "Microsoft.UI.Xaml.Controls.NavigationView, Microsoft.WinUI"),
        ("WinUIShell.NavigationViewBackRequestedEventArgs, WinUIShell", "Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs, Microsoft.WinUI"),
        ("WinUIShell.NavigationViewItem, WinUIShell", "Microsoft.UI.Xaml.Controls.NavigationViewItem, Microsoft.WinUI"),
        ("WinUIShell.NavigationViewItemBase, WinUIShell", "Microsoft.UI.Xaml.Controls.NavigationViewItemBase, Microsoft.WinUI"),
        ("WinUIShell.NavigationViewItemHeader, WinUIShell", "Microsoft.UI.Xaml.Controls.NavigationViewItemHeader, Microsoft.WinUI"),
        ("WinUIShell.NavigationViewItemInvokedEventArgs, WinUIShell", "Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs, Microsoft.WinUI"),
        ("WinUIShell.NavigationViewItemSeparator, WinUIShell", "Microsoft.UI.Xaml.Controls.NavigationViewItemSeparator, Microsoft.WinUI"),
        ("WinUIShell.OverlappedPresenter, WinUIShell", "Microsoft.UI.Windowing.OverlappedPresenter, Microsoft.InteractiveExperiences.Projection"),
        ("WinUIShell.Page, WinUIShell", "Microsoft.UI.Xaml.Controls.Page, Microsoft.WinUI"),
        ("WinUIShell.Panel, WinUIShell", "Microsoft.UI.Xaml.Controls.Panel, Microsoft.WinUI"),
        ("WinUIShell.PasswordBox, WinUIShell", "Microsoft.UI.Xaml.Controls.PasswordBox, Microsoft.WinUI"),
        ("WinUIShell.ProgressBar, WinUIShell", "Microsoft.UI.Xaml.Controls.ProgressBar, Microsoft.WinUI"),
        ("WinUIShell.ProgressRing, WinUIShell", "Microsoft.UI.Xaml.Controls.ProgressRing, Microsoft.WinUI"),
        ("WinUIShell.RangeBase, WinUIShell", "Microsoft.UI.Xaml.Controls.Primitives.RangeBase, Microsoft.WinUI"),
        ("WinUIShell.ResourceDictionary, WinUIShell", "Microsoft.UI.Xaml.ResourceDictionary, Microsoft.WinUI"),
        ("WinUIShell.RoutedEventArgs, WinUIShell", "Microsoft.UI.Xaml.RoutedEventArgs, Microsoft.WinUI"),
        ("WinUIShell.RowDefinition, WinUIShell", "Microsoft.UI.Xaml.Controls.RowDefinition, Microsoft.WinUI"),
        ("WinUIShell.RowDefinitionCollection, WinUIShell", "Microsoft.UI.Xaml.Controls.RowDefinitionCollection, Microsoft.WinUI"),
        ("WinUIShell.ScrollView, WinUIShell", "Microsoft.UI.Xaml.Controls.ScrollView, Microsoft.WinUI"),
        ("WinUIShell.SelectionChangedEventArgs, WinUIShell", "Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs, Microsoft.WinUI"),
        ("WinUIShell.Selector, WinUIShell", "Microsoft.UI.Xaml.Controls.Primitives.Selector, Microsoft.WinUI"),
        ("WinUIShell.Size, WinUIShell", "Windows.Foundation.Size, WinRT.Runtime"),
        ("WinUIShell.SlideNavigationTransitionInfo, WinUIShell", "Microsoft.UI.Xaml.Media.Animation.SlideNavigationTransitionInfo, Microsoft.WinUI"),
        ("WinUIShell.SolidColorBrush, WinUIShell", "Microsoft.UI.Xaml.Media.SolidColorBrush, Microsoft.WinUI"),
        ("WinUIShell.StackPanel, WinUIShell", "Microsoft.UI.Xaml.Controls.StackPanel, Microsoft.WinUI"),
        ("WinUIShell.Style, WinUIShell", "Microsoft.UI.Xaml.Style, Microsoft.WinUI"),
        ("WinUIShell.SymbolIcon, WinUIShell", "Microsoft.UI.Xaml.Controls.SymbolIcon, Microsoft.WinUI"),
        ("WinUIShell.SymbolIconSource, WinUIShell", "Microsoft.UI.Xaml.Controls.SymbolIconSource, Microsoft.WinUI"),
        ("WinUIShell.SystemBackdrop, WinUIShell", "Microsoft.UI.Xaml.Media.SystemBackdrop, Microsoft.WinUI"),
        ("WinUIShell.TextBlock, WinUIShell", "Microsoft.UI.Xaml.Controls.TextBlock, Microsoft.WinUI"),
        ("WinUIShell.TextBox, WinUIShell", "Microsoft.UI.Xaml.Controls.TextBox, Microsoft.WinUI"),
        ("WinUIShell.TextBoxBeforeTextChangingEventArgs, WinUIShell", "Microsoft.UI.Xaml.Controls.TextBoxBeforeTextChangingEventArgs, Microsoft.WinUI"),
        ("WinUIShell.TextChangedEventArgs, WinUIShell", "Microsoft.UI.Xaml.Controls.TextChangedEventArgs, Microsoft.WinUI"),
        ("WinUIShell.Thickness, WinUIShell", "Microsoft.UI.Xaml.Thickness, Microsoft.WinUI"),
        ("WinUIShell.TitleBar, WinUIShell", "Microsoft.UI.Xaml.Controls.TitleBar, Microsoft.WinUI"),
        ("WinUIShell.ToggleSwitch, WinUIShell", "Microsoft.UI.Xaml.Controls.ToggleSwitch, Microsoft.WinUI"),
        ("WinUIShell.UIElement, WinUIShell", "Microsoft.UI.Xaml.UIElement, Microsoft.WinUI"),
        ("WinUIShell.UIElementCollection, WinUIShell", "Microsoft.UI.Xaml.Controls.UIElementCollection, Microsoft.WinUI"),
        ("WinUIShell.Uri, WinUIShell", "System.Uri, System.Private.Uri"),
        ("WinUIShell.UserControl, WinUIShell", "Microsoft.UI.Xaml.Controls.UserControl, Microsoft.WinUI"),
        ("WinUIShell.Window, WinUIShell", "Microsoft.UI.Xaml.Window, Microsoft.WinUI"),
        ("WinUIShell.WindowActivatedEventArgs, WinUIShell", "Microsoft.UI.Xaml.WindowActivatedEventArgs, Microsoft.WinUI"),
        ("WinUIShell.WindowEventArgs, WinUIShell", "Microsoft.UI.Xaml.WindowEventArgs, Microsoft.WinUI"),
        ("WinUIShell.WindowSizeChangedEventArgs, WinUIShell", "Microsoft.UI.Xaml.WindowSizeChangedEventArgs, Microsoft.WinUI"),
        ("WinUIShell.WindowVisibilityChangedEventArgs, WinUIShell", "Microsoft.UI.Xaml.WindowVisibilityChangedEventArgs, Microsoft.WinUI"),
        ("WinUIShell.XamlReader, WinUIShell", "Microsoft.UI.Xaml.Markup.XamlReader, Microsoft.WinUI"),
    ];

    private readonly List<(string, string)> _listUno = [
        ("WinUIShell.Application, WinUIShell", "Microsoft.UI.Xaml.Application, Uno.UI"),
        ("WinUIShell.AppWindow, WinUIShell", "Microsoft.UI.Windowing.AppWindow, Uno"),
        ("WinUIShell.AppWindowPresenter, WinUIShell", "Microsoft.UI.Windowing.AppWindowPresenter, Uno"),
        ("WinUIShell.AppWindowTitleBar, WinUIShell", "Microsoft.UI.Windowing.AppWindowTitleBar, Uno"),
        ("WinUIShell.BitmapImage, WinUIShell", "Microsoft.UI.Xaml.Media.Imaging.BitmapImage, Uno.UI"),
        ("WinUIShell.BitmapSource, WinUIShell", "Microsoft.UI.Xaml.Media.Imaging.BitmapSource, Uno.UI"),
        ("WinUIShell.Brush, WinUIShell", "Microsoft.UI.Xaml.Media.Brush, Uno.UI"),
        ("WinUIShell.Button, WinUIShell", "Microsoft.UI.Xaml.Controls.Button, Uno.UI"),
        ("WinUIShell.ButtonBase, WinUIShell", "Microsoft.UI.Xaml.Controls.Primitives.ButtonBase, Uno.UI"),
        ("WinUIShell.Color, WinUIShell", "Windows.UI.Color, Uno"),
        ("WinUIShell.Colors, WinUIShell", "Microsoft.UI.Colors, Uno.UI"),
        ("WinUIShell.ColumnDefinition, WinUIShell", "Microsoft.UI.Xaml.Controls.ColumnDefinition, Uno.UI"),
        ("WinUIShell.ColumnDefinitionCollection, WinUIShell", "Microsoft.UI.Xaml.Controls.ColumnDefinitionCollection, Uno.UI"),
        ("WinUIShell.ComboBox, WinUIShell", "Microsoft.UI.Xaml.Controls.ComboBox, Uno.UI"),
        ("WinUIShell.ComboBoxTextSubmittedEventArgs, WinUIShell", "Microsoft.UI.Xaml.Controls.ComboBoxTextSubmittedEventArgs, Uno.UI"),
        ("WinUIShell.CompactOverlayPresenter, WinUIShell", "Microsoft.UI.Windowing.CompactOverlayPresenter, Uno"),
        ("WinUIShell.ContentControl, WinUIShell", "Microsoft.UI.Xaml.Controls.ContentControl, Uno.UI"),
        ("WinUIShell.Control, WinUIShell", "Microsoft.UI.Xaml.Controls.Control, Uno.UI"),
        ("WinUIShell.CornerRadius, WinUIShell", "Microsoft.UI.Xaml.CornerRadius, Uno.UI"),
        ("WinUIShell.DesktopAcrylicBackdrop, WinUIShell", "Microsoft.UI.Xaml.Media.DesktopAcrylicBackdrop, Uno.UI"),
        ("WinUIShell.DrillInNavigationTransitionInfo, WinUIShell", "Microsoft.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo, Uno.UI"),
        ("WinUIShell.EntranceNavigationTransitionInfo, WinUIShell", "Microsoft.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo, Uno.UI"),
        ("WinUIShell.FlyoutBase, WinUIShell", "Microsoft.UI.Xaml.Controls.Primitives.FlyoutBase, Uno.UI"),
        ("WinUIShell.FontFamily, WinUIShell", "Microsoft.UI.Xaml.Media.FontFamily, Uno.UI"),
        ("WinUIShell.FontWeight, WinUIShell", "Windows.UI.Text.FontWeight, Uno"),
        ("WinUIShell.FontWeights, WinUIShell", "Microsoft.UI.Text.FontWeights, Uno.UI"),
        ("WinUIShell.Frame, WinUIShell", "Microsoft.UI.Xaml.Controls.Frame, Uno.UI"),
        ("WinUIShell.FrameworkElement, WinUIShell", "Microsoft.UI.Xaml.FrameworkElement, Uno.UI"),
        ("WinUIShell.FullScreenPresenter, WinUIShell", "Microsoft.UI.Windowing.FullScreenPresenter, Uno"),
        ("WinUIShell.Grid, WinUIShell", "Microsoft.UI.Xaml.Controls.Grid, Uno.UI"),
        ("WinUIShell.GridLength, WinUIShell", "Microsoft.UI.Xaml.GridLength, Uno.UI"),
        ("WinUIShell.GridView, WinUIShell", "Microsoft.UI.Xaml.Controls.GridView, Uno.UI"),
        ("WinUIShell.HyperlinkButton, WinUIShell", "Microsoft.UI.Xaml.Controls.HyperlinkButton, Uno.UI"),
        ("WinUIShell.IconElement, WinUIShell", "Microsoft.UI.Xaml.Controls.IconElement, Uno.UI"),
        ("WinUIShell.IconSource, WinUIShell", "Microsoft.UI.Xaml.Controls.IconSource, Uno.UI"),
        ("WinUIShell.ImageIcon, WinUIShell", "Microsoft.UI.Xaml.Controls.ImageIcon, Uno.UI"),
        ("WinUIShell.ImageSource, WinUIShell", "Microsoft.UI.Xaml.Media.ImageSource, Uno.UI"),
        ("WinUIShell.ItemClickEventArgs, WinUIShell", "Microsoft.UI.Xaml.Controls.ItemClickEventArgs, Uno.UI"),
        ("WinUIShell.ItemCollection, WinUIShell", "Microsoft.UI.Xaml.Controls.ItemCollection, Uno.UI"),
        ("WinUIShell.ItemIndexRange, WinUIShell", "Microsoft.UI.Xaml.Data.ItemIndexRange, Uno.UI"),
        ("WinUIShell.ItemsControl, WinUIShell", "Microsoft.UI.Xaml.Controls.ItemsControl, Uno.UI"),
        ("WinUIShell.KeyRoutedEventArgs, WinUIShell", "Microsoft.UI.Xaml.Input.KeyRoutedEventArgs, Uno.UI"),
        ("WinUIShell.ListBox, WinUIShell", "Microsoft.UI.Xaml.Controls.ListBox, Uno.UI"),
        ("WinUIShell.ListView, WinUIShell", "Microsoft.UI.Xaml.Controls.ListView, Uno.UI"),
        ("WinUIShell.ListViewBase, WinUIShell", "Microsoft.UI.Xaml.Controls.ListViewBase, Uno.UI"),
        ("WinUIShell.MenuFlyout, WinUIShell", "Microsoft.UI.Xaml.Controls.MenuFlyout, Uno.UI"),
        ("WinUIShell.MenuFlyoutItem, WinUIShell", "Microsoft.UI.Xaml.Controls.MenuFlyoutItem, Uno.UI"),
        ("WinUIShell.MenuFlyoutItemBase, WinUIShell", "Microsoft.UI.Xaml.Controls.MenuFlyoutItemBase, Uno.UI"),
        ("WinUIShell.MicaBackdrop, WinUIShell", "Microsoft.UI.Xaml.Media.MicaBackdrop, Uno.UI"),
        ("WinUIShell.NavigationEventArgs, WinUIShell", "Microsoft.UI.Xaml.Navigation.NavigationEventArgs, Uno.UI"),
        ("WinUIShell.NavigationTransitionInfo, WinUIShell", "Microsoft.UI.Xaml.Media.Animation.NavigationTransitionInfo, Uno.UI"),
        ("WinUIShell.NavigationView, WinUIShell", "Microsoft.UI.Xaml.Controls.NavigationView, Uno.UI"),
        ("WinUIShell.NavigationViewBackRequestedEventArgs, WinUIShell", "Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs, Uno.UI"),
        ("WinUIShell.NavigationViewItem, WinUIShell", "Microsoft.UI.Xaml.Controls.NavigationViewItem, Uno.UI"),
        ("WinUIShell.NavigationViewItemBase, WinUIShell", "Microsoft.UI.Xaml.Controls.NavigationViewItemBase, Uno.UI"),
        ("WinUIShell.NavigationViewItemHeader, WinUIShell", "Microsoft.UI.Xaml.Controls.NavigationViewItemHeader, Uno.UI"),
        ("WinUIShell.NavigationViewItemInvokedEventArgs, WinUIShell", "Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs, Uno.UI"),
        ("WinUIShell.NavigationViewItemSeparator, WinUIShell", "Microsoft.UI.Xaml.Controls.NavigationViewItemSeparator, Uno.UI"),
        ("WinUIShell.OverlappedPresenter, WinUIShell", "Microsoft.UI.Windowing.OverlappedPresenter, Uno"),
        ("WinUIShell.Page, WinUIShell", "Microsoft.UI.Xaml.Controls.Page, Uno.UI"),
        ("WinUIShell.Panel, WinUIShell", "Microsoft.UI.Xaml.Controls.Panel, Uno.UI"),
        ("WinUIShell.PasswordBox, WinUIShell", "Microsoft.UI.Xaml.Controls.PasswordBox, Uno.UI"),
        ("WinUIShell.ProgressBar, WinUIShell", "Microsoft.UI.Xaml.Controls.ProgressBar, Uno.UI"),
        ("WinUIShell.ProgressRing, WinUIShell", "Microsoft.UI.Xaml.Controls.ProgressRing, Uno.UI"),
        ("WinUIShell.RangeBase, WinUIShell", "Microsoft.UI.Xaml.Controls.Primitives.RangeBase, Uno.UI"),
        ("WinUIShell.ResourceDictionary, WinUIShell", "Microsoft.UI.Xaml.ResourceDictionary, Uno.UI"),
        ("WinUIShell.RoutedEventArgs, WinUIShell", "Microsoft.UI.Xaml.RoutedEventArgs, Uno.UI"),
        ("WinUIShell.RowDefinition, WinUIShell", "Microsoft.UI.Xaml.Controls.RowDefinition, Uno.UI"),
        ("WinUIShell.RowDefinitionCollection, WinUIShell", "Microsoft.UI.Xaml.Controls.RowDefinitionCollection, Uno.UI"),
        ("WinUIShell.ScrollView, WinUIShell", "Microsoft.UI.Xaml.Controls.ScrollView, Uno.UI"),
        ("WinUIShell.SelectionChangedEventArgs, WinUIShell", "Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs, Uno.UI"),
        ("WinUIShell.Selector, WinUIShell", "Microsoft.UI.Xaml.Controls.Primitives.Selector, Uno.UI"),
        ("WinUIShell.Size, WinUIShell", "Windows.Foundation.Size, Uno.Foundation"),
        ("WinUIShell.SlideNavigationTransitionInfo, WinUIShell", "Microsoft.UI.Xaml.Media.Animation.SlideNavigationTransitionInfo, Uno.UI"),
        ("WinUIShell.SolidColorBrush, WinUIShell", "Microsoft.UI.Xaml.Media.SolidColorBrush, Uno.UI"),
        ("WinUIShell.StackPanel, WinUIShell", "Microsoft.UI.Xaml.Controls.StackPanel, Uno.UI"),
        ("WinUIShell.Style, WinUIShell", "Microsoft.UI.Xaml.Style, Uno.UI"),
        ("WinUIShell.SymbolIcon, WinUIShell", "Microsoft.UI.Xaml.Controls.SymbolIcon, Uno.UI"),
        ("WinUIShell.SymbolIconSource, WinUIShell", "Microsoft.UI.Xaml.Controls.SymbolIconSource, Uno.UI"),
        ("WinUIShell.SystemBackdrop, WinUIShell", "Microsoft.UI.Xaml.Media.SystemBackdrop, Uno.UI"),
        ("WinUIShell.TextBlock, WinUIShell", "Microsoft.UI.Xaml.Controls.TextBlock, Uno.UI"),
        ("WinUIShell.TextBox, WinUIShell", "Microsoft.UI.Xaml.Controls.TextBox, Uno.UI"),
        ("WinUIShell.TextBoxBeforeTextChangingEventArgs, WinUIShell", "Microsoft.UI.Xaml.Controls.TextBoxBeforeTextChangingEventArgs, Uno.UI"),
        ("WinUIShell.TextChangedEventArgs, WinUIShell", "Microsoft.UI.Xaml.Controls.TextChangedEventArgs, Uno.UI"),
        ("WinUIShell.Thickness, WinUIShell", "Microsoft.UI.Xaml.Thickness, Uno.UI"),
        ("WinUIShell.TitleBar, WinUIShell", "Microsoft.UI.Xaml.Controls.TitleBar, Uno.UI"),
        ("WinUIShell.ToggleSwitch, WinUIShell", "Microsoft.UI.Xaml.Controls.ToggleSwitch, Uno.UI"),
        ("WinUIShell.UIElement, WinUIShell", "Microsoft.UI.Xaml.UIElement, Uno.UI"),
        ("WinUIShell.UIElementCollection, WinUIShell", "Microsoft.UI.Xaml.Controls.UIElementCollection, Uno.UI"),
        ("WinUIShell.Uri, WinUIShell", "System.Uri, System.Private.Uri"),
        ("WinUIShell.UserControl, WinUIShell", "Microsoft.UI.Xaml.Controls.UserControl, Uno.UI"),
        ("WinUIShell.Window, WinUIShell", "Microsoft.UI.Xaml.Window, Uno.UI"),
        ("WinUIShell.WindowActivatedEventArgs, WinUIShell", "Microsoft.UI.Xaml.WindowActivatedEventArgs, Uno.UI"),
        ("WinUIShell.WindowEventArgs, WinUIShell", "Microsoft.UI.Xaml.WindowEventArgs, Uno.UI"),
        ("WinUIShell.WindowSizeChangedEventArgs, WinUIShell", "Microsoft.UI.Xaml.WindowSizeChangedEventArgs, Uno.UI"),
        ("WinUIShell.WindowVisibilityChangedEventArgs, WinUIShell", "Microsoft.UI.Xaml.WindowVisibilityChangedEventArgs, Uno.UI"),
        ("WinUIShell.XamlReader, WinUIShell", "Microsoft.UI.Xaml.Markup.XamlReader, Uno.UI"),
    ];

    public ObjectTypeMapping()
    {
    }

    public void SetFramework(Framework framework)
    {
        var list = (framework == Framework.WinUI) ? _listWinUI : _listUno;

        _map.Clear();
        foreach (var map in list)
        {
            _map.Add(map.Item1, map.Item2);
            _map.Add(map.Item2, map.Item1);
        }

        EnumTypeMapping.Get().SetFramework(framework);
    }

    public bool TryGetValue(string sourceTypeName, out string? targetTypeName)
    {
        return _map.TryGetValue(sourceTypeName, out targetTypeName);
    }

    public string GetTargetTypeName<T>()
    {
        Type type = typeof(T);
        var typeName = type.FullName;
        var assemblyName = type.Assembly.GetName().Name;
        var sourceTypeName = $"{typeName}, {assemblyName}";

        if (type == typeof(object))
        {
            return sourceTypeName;
        }

        _ = TryGetValue(sourceTypeName, out string? targetTypeName);
        if (targetTypeName is null)
        {
            throw new InvalidOperationException($"Object type mapping not found for [{sourceTypeName}].");
        }
        return targetTypeName;
    }
}
