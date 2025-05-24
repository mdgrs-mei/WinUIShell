namespace WinUIShell.Common;

internal sealed class EnumTypeMapping : Singleton<EnumTypeMapping>
{
    private readonly Dictionary<string, string> _map = [];

    private readonly List<(string, string)> _list = [
        ("WinUIShell.HorizontalAlignment, WinUIShell", "Microsoft.UI.Xaml.HorizontalAlignment, Microsoft.WinUI"),
        ("WinUIShell.VerticalAlignment, WinUIShell", "Microsoft.UI.Xaml.VerticalAlignment, Microsoft.WinUI"),
        ("WinUIShell.TextAlignment, WinUIShell", "Microsoft.UI.Xaml.TextAlignment, Microsoft.WinUI"),
        ("WinUIShell.CompactOverlaySize, WinUIShell", "Microsoft.UI.Windowing.CompactOverlaySize, Microsoft.InteractiveExperiences.Projection"),
        ("WinUIShell.Symbol, WinUIShell", "Microsoft.UI.Xaml.Controls.Symbol, Microsoft.WinUI"),
        ("WinUIShell.Orientation, WinUIShell", "Microsoft.UI.Xaml.Controls.Orientation, Microsoft.WinUI"),
        ("WinUIShell.TitleBarTheme, WinUIShell", "Microsoft.UI.Windowing.TitleBarTheme, Microsoft.InteractiveExperiences.Projection"),
        ("WinUIShell.GridUnitType, WinUIShell", "Microsoft.UI.Xaml.GridUnitType, Microsoft.WinUI"),
        ("WinUIShell.BackgroundSizing, WinUIShell", "Microsoft.UI.Xaml.Controls.BackgroundSizing, Microsoft.WinUI"),
        ("WinUIShell.MicaKind, WinUIShell", "Microsoft.UI.Composition.SystemBackdrops.MicaKind, Microsoft.InteractiveExperiences.Projection"),
        ("WinUIShell.LineStackingStrategy, WinUIShell", "Microsoft.UI.Xaml.LineStackingStrategy, Microsoft.WinUI"),
        ("WinUIShell.OpticalMarginAlignment, WinUIShell", "Microsoft.UI.Xaml.OpticalMarginAlignment, Microsoft.WinUI"),
        ("WinUIShell.TextDecorations, WinUIShell", "Windows.UI.Text.TextDecorations, Microsoft.Windows.SDK.NET"),
        ("WinUIShell.TextLineBounds, WinUIShell", "Microsoft.UI.Xaml.TextLineBounds, Microsoft.WinUI"),
        ("WinUIShell.TextReadingOrder, WinUIShell", "Microsoft.UI.Xaml.TextReadingOrder, Microsoft.WinUI"),
        ("WinUIShell.TextTrimming, WinUIShell", "Microsoft.UI.Xaml.TextTrimming, Microsoft.WinUI"),
        ("WinUIShell.TextWrapping, WinUIShell", "Microsoft.UI.Xaml.TextWrapping, Microsoft.WinUI"),
        ("WinUIShell.FontStretch, WinUIShell", "Windows.UI.Text.FontStretch, Microsoft.Windows.SDK.NET"),
        ("WinUIShell.FontStyle, WinUIShell", "Windows.UI.Text.FontStyle, Microsoft.Windows.SDK.NET"),
        ("WinUIShell.CharacterCasing, WinUIShell", "Microsoft.UI.Xaml.Controls.CharacterCasing, Microsoft.WinUI"),
        ("WinUIShell.CandidateWindowAlignment, WinUIShell", "Microsoft.UI.Xaml.Controls.CandidateWindowAlignment, Microsoft.WinUI"),
        ("WinUIShell.ScrollingContentOrientation, WinUIShell", "Microsoft.UI.Xaml.Controls.ScrollingContentOrientation, Microsoft.WinUI"),
        ("WinUIShell.ScrollingScrollBarVisibility, WinUIShell", "Microsoft.UI.Xaml.Controls.ScrollingScrollBarVisibility, Microsoft.WinUI"),
        ("WinUIShell.ScrollingChainMode, WinUIShell", "Microsoft.UI.Xaml.Controls.ScrollingChainMode, Microsoft.WinUI"),
        ("WinUIShell.ScrollingScrollMode, WinUIShell", "Microsoft.UI.Xaml.Controls.ScrollingScrollMode, Microsoft.WinUI"),
        ("WinUIShell.ScrollingRailMode, WinUIShell", "Microsoft.UI.Xaml.Controls.ScrollingRailMode, Microsoft.WinUI"),
        ("WinUIShell.NavigationCacheMode, WinUIShell", "Microsoft.UI.Xaml.Navigation.NavigationCacheMode, Microsoft.WinUI"),
    ];

    public EnumTypeMapping()
    {
        foreach (var map in _list)
        {
            _map.Add(map.Item1, map.Item2);
            _map.Add(map.Item2, map.Item1);
        }
    }

    public bool TryGetValue(string sourceEnumType, out string? targetEnumType)
    {
        return _map.TryGetValue(sourceEnumType, out targetEnumType);
    }
}
