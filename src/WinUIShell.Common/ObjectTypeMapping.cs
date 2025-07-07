namespace WinUIShell.Common;

public sealed class ObjectTypeMapping : Singleton<ObjectTypeMapping>
{
    private readonly Dictionary<string, string> _map = [];

    private readonly List<(string, string)> _list = [
        ("WinUIShell.Application, WinUIShell", "Microsoft.UI.Xaml.Application, Microsoft.WinUI"),
        ("WinUIShell.Button, WinUIShell", "Microsoft.UI.Xaml.Controls.Button, Microsoft.WinUI"),
        ("WinUIShell.ColumnDefinition, WinUIShell", "Microsoft.UI.Xaml.Controls.ColumnDefinition, Microsoft.WinUI"),
        ("WinUIShell.CompactOverlayPresenter, WinUIShell", "Microsoft.UI.Windowing.CompactOverlayPresenter, Microsoft.InteractiveExperiences.Projection"),
        ("WinUIShell.CornerRadius, WinUIShell", "Microsoft.UI.Xaml.CornerRadius, Microsoft.WinUI"),
        ("WinUIShell.DesktopAcrylicBackdrop, WinUIShell", "Microsoft.UI.Xaml.Media.DesktopAcrylicBackdrop, Microsoft.WinUI"),
        ("WinUIShell.DrillInNavigationTransitionInfo, WinUIShell", "Microsoft.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo, Microsoft.WinUI"),
        ("WinUIShell.EntranceNavigationTransitionInfo, WinUIShell", "Microsoft.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo, Microsoft.WinUI"),
        ("WinUIShell.FontFamily, WinUIShell", "Microsoft.UI.Xaml.Media.FontFamily, Microsoft.WinUI"),
        ("WinUIShell.FontWeight, WinUIShell", "Windows.UI.Text.FontWeight, Microsoft.Windows.SDK.NET"),
        ("WinUIShell.Frame, WinUIShell", "Microsoft.UI.Xaml.Controls.Frame, Microsoft.WinUI"),
        ("WinUIShell.Grid, WinUIShell", "Microsoft.UI.Xaml.Controls.Grid, Microsoft.WinUI"),
        ("WinUIShell.GridLength, WinUIShell", "Microsoft.UI.Xaml.GridLength, Microsoft.WinUI"),
        ("WinUIShell.HyperlinkButton, WinUIShell", "Microsoft.UI.Xaml.Controls.HyperlinkButton, Microsoft.WinUI"),
        ("WinUIShell.MicaBackdrop, WinUIShell", "Microsoft.UI.Xaml.Media.MicaBackdrop, Microsoft.WinUI"),
        ("WinUIShell.NavigationView, WinUIShell", "Microsoft.UI.Xaml.Controls.NavigationView, Microsoft.WinUI"),
        ("WinUIShell.NavigationViewItem, WinUIShell", "Microsoft.UI.Xaml.Controls.NavigationViewItem, Microsoft.WinUI"),
        ("WinUIShell.NavigationViewItemHeader, WinUIShell", "Microsoft.UI.Xaml.Controls.NavigationViewItemHeader, Microsoft.WinUI"),
        ("WinUIShell.NavigationViewItemSeparator, WinUIShell", "Microsoft.UI.Xaml.Controls.NavigationViewItemSeparator, Microsoft.WinUI"),
        ("WinUIShell.PasswordBox, WinUIShell", "Microsoft.UI.Xaml.Controls.PasswordBox, Microsoft.WinUI"),
        ("WinUIShell.ProgressBar, WinUIShell", "Microsoft.UI.Xaml.Controls.ProgressBar, Microsoft.WinUI"),
        ("WinUIShell.ProgressRing, WinUIShell", "Microsoft.UI.Xaml.Controls.ProgressRing, Microsoft.WinUI"),
        ("WinUIShell.RowDefinition, WinUIShell", "Microsoft.UI.Xaml.Controls.RowDefinition, Microsoft.WinUI"),
        ("WinUIShell.ScrollView, WinUIShell", "Microsoft.UI.Xaml.Controls.ScrollView, Microsoft.WinUI"),
        ("WinUIShell.SlideNavigationTransitionInfo, WinUIShell", "Microsoft.UI.Xaml.Media.Animation.SlideNavigationTransitionInfo, Microsoft.WinUI"),
        ("WinUIShell.SolidColorBrush, WinUIShell", "Microsoft.UI.Xaml.Media.SolidColorBrush, Microsoft.WinUI"),
        ("WinUIShell.StackPanel, WinUIShell", "Microsoft.UI.Xaml.Controls.StackPanel, Microsoft.WinUI"),
        ("WinUIShell.SymbolIcon, WinUIShell", "Microsoft.UI.Xaml.Controls.SymbolIcon, Microsoft.WinUI"),
        ("WinUIShell.SymbolIconSource, WinUIShell", "Microsoft.UI.Xaml.Controls.SymbolIconSource, Microsoft.WinUI"),
        ("WinUIShell.TextBlock, WinUIShell", "Microsoft.UI.Xaml.Controls.TextBlock, Microsoft.WinUI"),
        ("WinUIShell.TextBox, WinUIShell", "Microsoft.UI.Xaml.Controls.TextBox, Microsoft.WinUI"),
        ("WinUIShell.Thickness, WinUIShell", "Microsoft.UI.Xaml.Thickness, Microsoft.WinUI"),
        ("WinUIShell.TitleBar, WinUIShell", "Microsoft.UI.Xaml.Controls.TitleBar, Microsoft.WinUI"),
        ("WinUIShell.ToggleSwitch, WinUIShell", "Microsoft.UI.Xaml.Controls.ToggleSwitch, Microsoft.WinUI"),
        ("WinUIShell.Uri, WinUIShell", "System.Uri, System.Private.Uri"),
        ("WinUIShell.Window, WinUIShell", "Microsoft.UI.Xaml.Window, Microsoft.WinUI"),
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
