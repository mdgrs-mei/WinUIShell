namespace WinUIShell.Common;

public sealed class ObjectTypeMapping : Singleton<ObjectTypeMapping>
{
    private readonly Dictionary<string, string> _map = [];

    private readonly List<(string, string)> _list = [
        ("WinUIShell.Application, WinUIShell", "Microsoft.UI.Xaml.Application, Microsoft.WinUI"),
        ("WinUIShell.AppWindow, WinUIShell", "Microsoft.UI.Windowing.AppWindow, Microsoft.InteractiveExperiences.Projection"),
        ("WinUIShell.AppWindowPresenter, WinUIShell", "Microsoft.UI.Windowing.AppWindowPresenter, Microsoft.InteractiveExperiences.Projection"),
        ("WinUIShell.AppWindowTitleBar, WinUIShell", "Microsoft.UI.Windowing.AppWindowTitleBar, Microsoft.InteractiveExperiences.Projection"),
        ("WinUIShell.Brush, WinUIShell", "Microsoft.UI.Xaml.Media.Brush, Microsoft.WinUI"),
        ("WinUIShell.Button, WinUIShell", "Microsoft.UI.Xaml.Controls.Button, Microsoft.WinUI"),
        ("WinUIShell.ButtonBase, WinUIShell", "Microsoft.UI.Xaml.Controls.Primitives.ButtonBase, Microsoft.WinUI"),
        ("WinUIShell.Color, WinUIShell", "Windows.UI.Color, Microsoft.Windows.SDK.NET"),
        ("WinUIShell.Colors, WinUIShell", "Microsoft.UI.Colors, Microsoft.InteractiveExperiences.Projection"),
        ("WinUIShell.ColumnDefinition, WinUIShell", "Microsoft.UI.Xaml.Controls.ColumnDefinition, Microsoft.WinUI"),
        ("WinUIShell.ColumnDefinitionCollection, WinUIShell", "Microsoft.UI.Xaml.Controls.ColumnDefinitionCollection, Microsoft.WinUI"),
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
        ("WinUIShell.Grid, WinUIShell", "Microsoft.UI.Xaml.Controls.Grid, Microsoft.WinUI"),
        ("WinUIShell.GridLength, WinUIShell", "Microsoft.UI.Xaml.GridLength, Microsoft.WinUI"),
        ("WinUIShell.HyperlinkButton, WinUIShell", "Microsoft.UI.Xaml.Controls.HyperlinkButton, Microsoft.WinUI"),
        ("WinUIShell.IconElement, WinUIShell", "Microsoft.UI.Xaml.Controls.IconElement, Microsoft.WinUI"),
        ("WinUIShell.IconSource, WinUIShell", "Microsoft.UI.Xaml.Controls.IconSource, Microsoft.WinUI"),
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
        ("WinUIShell.WindowEventArgs, WinUIShell", "Microsoft.UI.Xaml.WindowEventArgs, Microsoft.WinUI"),
        ("WinUIShell.XamlReader, WinUIShell", "Microsoft.UI.Xaml.Markup.XamlReader, Microsoft.WinUI"),
    ];

    public ObjectTypeMapping()
    {
        foreach (var map in _list)
        {
            _map.Add(map.Item1, map.Item2);
            _map.Add(map.Item2, map.Item1);
        }
    }

    public bool TryGetValue(string sourceTypeName, out string? targetTypeName)
    {
        return _map.TryGetValue(sourceTypeName, out targetTypeName);
    }

    public string GetTargetTypeName<T>()
    {
        var type = typeof(T);
        var typeName = type.FullName;
        var assemblyName = type.Assembly.GetName().Name;
        var sourceTypeName = $"{typeName}, {assemblyName}";

        _ = TryGetValue(sourceTypeName, out string? targetTypeName);
        if (targetTypeName is null)
        {
            throw new InvalidOperationException($"Object type mapping not found for [{sourceTypeName}].");
        }
        return targetTypeName;
    }
}
