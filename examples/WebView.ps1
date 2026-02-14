using namespace WinUIShell
using namespace WinUIShell.Microsoft.UI.Xaml
using namespace WinUIShell.Microsoft.UI.Xaml.Controls

if (-not (Get-Module WinUIShell)) {
    Import-Module WinUIShell
}

$defaultAddress = 'https://learn.microsoft.com/en-us/'

$resources = [Application]::Current.Resources
$win = [Window]::new()
$win.Title = 'WebView'

$addressBar = [TextBox]::new()
$addressBar.Text = $defaultAddress
[Grid]::SetRow($addressBar, 0)
[Grid]::SetColumn($addressBar, 0)

$button = [Button]::new()
$button.Content = 'Go'
$button.Style = $resources['AccentButtonStyle']
[Grid]::SetRow($button, 0)
[Grid]::SetColumn($button, 1)

$button.AddClick({
        $webView.Source = $addressBar.Text
    })

$webView = [WebView2]::new()
$webView.Source = $defaultAddress
[Grid]::SetRow($webView, 1)
[Grid]::SetColumnSpan($webView, 2)

$webView.AddNavigationStarting({
        param ($argumentList, $s, $navigationStartingEventArgs)
        $addressBar.Text = $navigationStartingEventArgs.Uri
    })

$row0 = [RowDefinition]::new()
$row0.Height = [GridLength]::Auto
$row1 = [RowDefinition]::new()
$row1.Height = [GridLength]::new(1, 'Star')
$col0 = [ColumnDefinition]::new()
$col0.Width = [GridLength]::new(1, 'Star')
$col1 = [ColumnDefinition]::new()
$col1.Width = [GridLength]::Auto

$grid = [Grid]::new()
$grid.Margin = 4
$grid.RowSpacing = 16
$grid.ColumnSpacing = 4
$grid.RowDefinitions.Add($row0)
$grid.RowDefinitions.Add($row1)
$grid.ColumnDefinitions.Add($col0)
$grid.ColumnDefinitions.Add($col1)
$grid.Children.Add($addressBar)
$grid.Children.Add($button)
$grid.Children.Add($webView)

$win.Content = $grid
$win.Activate()
$win.WaitForClosed()
