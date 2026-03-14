using namespace WinUIShell.Microsoft.UI
using namespace WinUIShell.Microsoft.UI.Xaml
using namespace WinUIShell.Microsoft.UI.Xaml.Controls
using namespace WinUIShell.Microsoft.UI.Xaml.Media

if (-not (Get-Module WinUIShell)) {
    Import-Module WinUIShell
}

$autoSuggestBox = [AutoSuggestBox]::new()
$autoSuggestBox.Width = 360
$autoSuggestBox.VerticalAlignment = 'Center'
$autoSuggestBox.PlaceholderText = 'Search..'
$autoSuggestBox.QueryIcon = [SymbolIcon]::new('Find')
$suggestions = [WinUIShell.System.Collections.Generic.List[string]]::new()
$suggestions.Add('Suggestion 1')
$suggestions.Add('Suggestion 2')
$suggestions.Add('Suggestion 3')
$autoSuggestBox.ItemsSource = $suggestions

$personPicture = [PersonPicture]::new()
$personPicture.Width = 30
$personPicture.Height = 30
$personPicture.Initials = 'MD'

$titleBar = [TitleBar]::new()
$titleBar.IconSource = [SymbolIconSource]@{
    Symbol = 'Home'
    Foreground = [SolidColorBrush]::new([Colors]::Aquamarine)
}
$titleBar.Title = 'WinUIShell'
$titleBar.Subtitle = 'Beta'
$titleBar.Content = $autoSuggestBox
$titleBar.RightHeader = $personPicture
[Grid]::SetRow($titleBar, 0)

$win = [Window]::new()
$win.SystemBackdrop = [MicaBackdrop]::new()
$win.AppWindow.ResizeClient(1000, 400)
$win.SetTitleBar($titleBar)
$win.ExtendsContentIntoTitleBar = $true
$win.AppWindow.TitleBar.PreferredHeightOption = 'Tall'

$text = [TextBlock]::new()
$text.Text = 'Hello from PowerShell!'
$text.FontSize = 24
$text.HorizontalAlignment = 'Center'
$text.VerticalAlignment = 'Center'
[Grid]::SetRow($text, 1)

$row0 = [RowDefinition]::new()
$row0.Height = [GridLength]::Auto
$row1 = [RowDefinition]::new()
$row1.Height = [GridLength]::new(1, 'Star')
$grid = [Grid]::new()
$grid.RowDefinitions.Add($row0)
$grid.RowDefinitions.Add($row1)
$grid.Children.Add($titleBar)
$grid.Children.Add($text)

$win.Content = $grid
$win.Activate()
$win.WaitForClosed()
