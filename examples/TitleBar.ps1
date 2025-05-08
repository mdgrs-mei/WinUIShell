using namespace WinUIShell
if (-not (Get-Module WinUIShell)) {
    Import-Module WinUIShell
}

$titleBar = [TitleBar]::new()
$titleBar.IconSource = [SymbolIconSource]@{
    Symbol = 'Home'
    Foreground = [SolidColorBrush]::new([Colors]::Aquamarine)
}
$titleBar.Title = 'WinUIShell'
$titleBar.Subtitle = 'Beta'
[Grid]::SetRow($titleBar, 0)

$win = [Window]::new()
$win.AppWindow.ResizeClient(400, 200)
$win.ExtendsContentIntoTitleBar = $true
$win.SetTitleBar($titleBar)

$text = [TextBlock]::new()
$text.Text = 'Hello from PowerShell!'
$text.HorizontalAlignment = 'Center'
$text.VerticalAlignment = 'Center'
[Grid]::SetRow($text, 1)

$row1 = [RowDefinition]::new()
$row1.Height = [GridLength]::Auto
$row2 = [RowDefinition]::new()
$row2.Height = [GridLength]::new(1, 'Star')
$grid = [Grid]::new()
$grid.RowDefinitions.Add($row1)
$grid.RowDefinitions.Add($row2)
$grid.Children.Add($titleBar)
$grid.Children.Add($text)

$win.Content = $grid
$win.Activate()
$win.WaitForClosed()
