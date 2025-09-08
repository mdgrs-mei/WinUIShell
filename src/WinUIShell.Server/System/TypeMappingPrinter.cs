using System.Diagnostics;

namespace WinUIShell.Server;

internal static class TypeMappingPrinter
{
    public static void Print()
    {
        PrintObjectMapping();
        PrintEnumMapping();
    }

    private static void PrintObjectMapping()
    {
        Debug.WriteLine("----");
        Print(typeof(Microsoft.UI.Xaml.Application));
        Print(typeof(Microsoft.UI.Windowing.AppWindow));
        Print(typeof(Microsoft.UI.Windowing.AppWindowPresenter));
        Print(typeof(Microsoft.UI.Windowing.AppWindowTitleBar));
        Print(typeof(Microsoft.UI.Xaml.Media.Brush));
        Print(typeof(Microsoft.UI.Xaml.Controls.Button));
        Print(typeof(Microsoft.UI.Xaml.Controls.Primitives.ButtonBase));
        Print(typeof(Windows.UI.Color));
        Print(typeof(Microsoft.UI.Colors));
        Print(typeof(Microsoft.UI.Xaml.Controls.ColumnDefinition));
        Print(typeof(Microsoft.UI.Xaml.Controls.ColumnDefinitionCollection));
        Print(typeof(Microsoft.UI.Windowing.CompactOverlayPresenter));
        Print(typeof(Microsoft.UI.Xaml.Controls.ContentControl));
        Print(typeof(Microsoft.UI.Xaml.Controls.Control));
        Print(typeof(Microsoft.UI.Xaml.CornerRadius));
        Print(typeof(Microsoft.UI.Xaml.Media.DesktopAcrylicBackdrop));
        Print(typeof(Microsoft.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo));
        Print(typeof(Microsoft.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo));
        Print(typeof(Microsoft.UI.Xaml.Controls.Primitives.FlyoutBase));
        Print(typeof(Microsoft.UI.Xaml.Media.FontFamily));
        Print(typeof(Windows.UI.Text.FontWeight));
        Print(typeof(Microsoft.UI.Text.FontWeights));
        Print(typeof(Microsoft.UI.Xaml.Controls.Frame));
        Print(typeof(Microsoft.UI.Xaml.FrameworkElement));
        Print(typeof(Microsoft.UI.Windowing.FullScreenPresenter));
        Print(typeof(Microsoft.UI.Xaml.Controls.Grid));
        Print(typeof(Microsoft.UI.Xaml.GridLength));
        Print(typeof(Microsoft.UI.Xaml.Controls.HyperlinkButton));
        Print(typeof(Microsoft.UI.Xaml.Controls.IconElement));
        Print(typeof(Microsoft.UI.Xaml.Controls.IconSource));
        Print(typeof(Microsoft.UI.Xaml.Controls.ItemCollection));
        Print(typeof(Microsoft.UI.Xaml.Controls.ItemsControl));
        Print(typeof(Microsoft.UI.Xaml.Controls.MenuFlyout));
        Print(typeof(Microsoft.UI.Xaml.Controls.MenuFlyoutItem));
        Print(typeof(Microsoft.UI.Xaml.Controls.MenuFlyoutItemBase));
        Print(typeof(Microsoft.UI.Xaml.Media.MicaBackdrop));
        Print(typeof(Microsoft.UI.Xaml.Navigation.NavigationEventArgs));
        Print(typeof(Microsoft.UI.Xaml.Media.Animation.NavigationTransitionInfo));
        Print(typeof(Microsoft.UI.Xaml.Controls.NavigationView));
        Print(typeof(Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs));
        Print(typeof(Microsoft.UI.Xaml.Controls.NavigationViewItem));
        Print(typeof(Microsoft.UI.Xaml.Controls.NavigationViewItemBase));
        Print(typeof(Microsoft.UI.Xaml.Controls.NavigationViewItemHeader));
        Print(typeof(Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs));
        Print(typeof(Microsoft.UI.Xaml.Controls.NavigationViewItemSeparator));
        Print(typeof(Microsoft.UI.Windowing.OverlappedPresenter));
        Print(typeof(Microsoft.UI.Xaml.Controls.Page));
        Print(typeof(Microsoft.UI.Xaml.Controls.Panel));
        Print(typeof(Microsoft.UI.Xaml.Controls.PasswordBox));
        Print(typeof(Microsoft.UI.Xaml.Controls.ProgressBar));
        Print(typeof(Microsoft.UI.Xaml.Controls.ProgressRing));
        Print(typeof(Microsoft.UI.Xaml.Controls.Primitives.RangeBase));
        Print(typeof(Microsoft.UI.Xaml.ResourceDictionary));
        Print(typeof(Microsoft.UI.Xaml.RoutedEventArgs));
        Print(typeof(Microsoft.UI.Xaml.Controls.RowDefinition));
        Print(typeof(Microsoft.UI.Xaml.Controls.RowDefinitionCollection));
        Print(typeof(Microsoft.UI.Xaml.Controls.ScrollView));
        Print(typeof(Microsoft.UI.Xaml.Media.Animation.SlideNavigationTransitionInfo));
        Print(typeof(Microsoft.UI.Xaml.Media.SolidColorBrush));
        Print(typeof(Microsoft.UI.Xaml.Controls.StackPanel));
        Print(typeof(Microsoft.UI.Xaml.Style));
        Print(typeof(Microsoft.UI.Xaml.Controls.SymbolIcon));
        Print(typeof(Microsoft.UI.Xaml.Controls.SymbolIconSource));
        Print(typeof(Microsoft.UI.Xaml.Media.SystemBackdrop));
        Print(typeof(Microsoft.UI.Xaml.Controls.TextBlock));
        Print(typeof(Microsoft.UI.Xaml.Controls.TextBox));
        Print(typeof(Microsoft.UI.Xaml.Controls.TextBoxBeforeTextChangingEventArgs));
        Print(typeof(Microsoft.UI.Xaml.Controls.TextChangedEventArgs));
        Print(typeof(Microsoft.UI.Xaml.Thickness));
        Print(typeof(Microsoft.UI.Xaml.Controls.TitleBar));
        Print(typeof(Microsoft.UI.Xaml.Controls.ToggleSwitch));
        Print(typeof(Microsoft.UI.Xaml.UIElement));
        Print(typeof(Microsoft.UI.Xaml.Controls.UIElementCollection));
        Print(typeof(Uri));
        Print(typeof(Microsoft.UI.Xaml.Controls.UserControl));
        Print(typeof(Microsoft.UI.Xaml.Window));
        Print(typeof(Microsoft.UI.Xaml.WindowEventArgs));
        Print(typeof(Microsoft.UI.Xaml.Markup.XamlReader));
        Debug.WriteLine("----");
    }

    private static void PrintEnumMapping()
    {
        Debug.WriteLine("----");
        Print(typeof(Microsoft.UI.Xaml.Controls.BackgroundSizing));
        Print(typeof(Microsoft.UI.Xaml.Controls.CandidateWindowAlignment));
        Print(typeof(Microsoft.UI.Xaml.Controls.CharacterCasing));
        Print(typeof(Microsoft.UI.Windowing.CompactOverlaySize));
        Print(typeof(Microsoft.UI.Xaml.ElementTheme));
        Print(typeof(EventCallbackRunspaceMode));
        Print(typeof(Windows.UI.Text.FontStretch));
        Print(typeof(Windows.UI.Text.FontStyle));
        Print(typeof(Microsoft.UI.Xaml.GridUnitType));
        Print(typeof(Microsoft.UI.Xaml.HorizontalAlignment));
        Print(typeof(Microsoft.UI.Xaml.LineStackingStrategy));
        Print(typeof(Microsoft.UI.Composition.SystemBackdrops.MicaKind));
        Print(typeof(Microsoft.UI.Xaml.Navigation.NavigationCacheMode));
        Print(typeof(Microsoft.UI.Xaml.Navigation.NavigationMode));
        Print(typeof(Microsoft.UI.Xaml.Controls.NavigationViewBackButtonVisible));
        Print(typeof(Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode));
        Print(typeof(Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode));
        Print(typeof(Microsoft.UI.Xaml.OpticalMarginAlignment));
        Print(typeof(Microsoft.UI.Xaml.Controls.Orientation));
        Print(typeof(Microsoft.UI.Windowing.OverlappedPresenterState));
        Print(typeof(Microsoft.UI.Xaml.Controls.ScrollingChainMode));
        Print(typeof(Microsoft.UI.Xaml.Controls.ScrollingContentOrientation));
        Print(typeof(Microsoft.UI.Xaml.Controls.ScrollingRailMode));
        Print(typeof(Microsoft.UI.Xaml.Controls.ScrollingScrollBarVisibility));
        Print(typeof(Microsoft.UI.Xaml.Controls.ScrollingScrollMode));
        Print(typeof(Microsoft.UI.Xaml.Media.Animation.SlideNavigationTransitionEffect));
        Print(typeof(Microsoft.UI.Xaml.Controls.Symbol));
        Print(typeof(Microsoft.UI.Xaml.TextAlignment));
        Print(typeof(Windows.UI.Text.TextDecorations));
        Print(typeof(Microsoft.UI.Xaml.TextLineBounds));
        Print(typeof(Microsoft.UI.Xaml.TextReadingOrder));
        Print(typeof(Microsoft.UI.Xaml.TextTrimming));
        Print(typeof(Microsoft.UI.Xaml.TextWrapping));
        Print(typeof(Microsoft.UI.Windowing.TitleBarTheme));
        Print(typeof(Microsoft.UI.Xaml.VerticalAlignment));
        Debug.WriteLine("----");
    }

    private static void Print(Type type)
    {
        var fullname = type.FullName;
        var assemblyName = type.Assembly.GetName().Name;
        Debug.WriteLine($"(\"WinUIShell.{type.Name}, WinUIShell\", \"{fullname}, {assemblyName}\"),");
    }
}
