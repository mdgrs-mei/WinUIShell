using namespace WinUIShell
using namespace WinUIShell.Microsoft.UI.Xaml
using namespace WinUIShell.Microsoft.UI.Xaml.Controls
using namespace WinUIShell.Microsoft.UI.Xaml.Media
using namespace WinUIShell.Microsoft.UI.Xaml.Media.Imaging

if (-not (Get-Module WinUIShell)) {
    Import-Module WinUIShell
}

$resources = [Application]::Current.Resources

function Main() {
    $win = [Window]::new()
    $win.SystemBackdrop = [MicaBackdrop]::new()
    $win.AppWindow.ResizeClient(800, 600)

    $gridView = [GridView]::new()
    $gridView.Margin = 16
    $gridView.BorderThickness = 1
    $gridView.BorderBrush = $resources['SystemControlForegroundBaseMediumLowBrush']
    $gridView.CanReorderItems = $true
    $gridView.AllowDrop = $true
    AddItems $gridView
    [Grid]::SetRow($gridView, 0)

    $multiSelection = [ToggleSwitch]::new()
    $multiSelection.Margin = 16
    $multiSelection.HorizontalAlignment = 'Left'
    $multiSelection.IsOn = $false
    $multiSelection.Header = 'Multi Selection'
    $multiSelection.AddToggled({
            param ($GridView, $SenderToggle)
            if ($SenderToggle.IsOn) {
                $GridView.SelectionMode = 'Multiple'
            } else {
                $GridView.SelectionMode = 'Single'
            }
        }, $gridView)
    [Grid]::SetRow($multiSelection, 1)

    $removeItemButton = [Button]::new()
    $removeItemButton.Margin = [Thickness]::new(16, 0, 16, 16)
    $removeItemButton.HorizontalAlignment = 'Stretch'
    $removeItemButton.Content = 'Remove Selection'
    $removeItemButton.AddClick({
            param ($GridView)
            $removedItems = @()
            $GridView.SelectedItems | ForEach-Object { $removedItems += $_ }
            $removedItems | ForEach-Object { $GridView.Items.Remove($_) }
        }, $gridView)
    [Grid]::SetRow($removeItemButton, 2)

    $row0 = [RowDefinition]::new()
    $row0.Height = [GridLength]::new(1, 'Star')
    $row1 = [RowDefinition]::new()
    $row1.Height = [GridLength]::Auto
    $row2 = [RowDefinition]::new()
    $row2.Height = [GridLength]::Auto

    $grid = [Grid]::new()
    $grid.RowDefinitions.Add($row0)
    $grid.RowDefinitions.Add($row1)
    $grid.RowDefinitions.Add($row2)
    $grid.Children.Add($gridView)
    $grid.Children.Add($multiSelection)
    $grid.Children.Add($removeItemButton)

    $win.Content = $grid
    $win.Activate()
    $win.WaitForClosed()
}

function AddItems($View) {
    $itemNames = @(
        'Apple'
        'Banana'
        'Mango'
        'Orange'
        'Pineapple'
        'Strawberry'
        'Blueberry'
        'Raspberry'
        'Kiwi'
        'Grapes'
        'Peach'
        'Pear'
        'Watermelon'
        'Cherry'
    )

    $viewItems = $View.Items
    $nameStyle = $resources['SubtitleTextBlockStyle']
    $descStyle = $resources['CaptionTextBlockStyle']
    $bitmap = [BitmapImage]::new("$PSScriptRoot/resources/Circle.png")

    foreach ($itemName in $itemNames) {
        $icon = [ImageIcon]::new()
        $icon.Width = 64
        $icon.Source = $bitmap

        $name = [TextBlock]::new()
        $name.Width = 128
        $name.Text = $itemName
        $name.Style = $nameStyle

        $desc = [TextBlock]::new()
        $desc.Text = $itemName
        $desc.Style = $descStyle

        $textPanel = [StackPanel]::new()
        $textPanel.Spacing = 4
        $textPanelChildren = $textPanel.Children
        $textPanelChildren.Add($name)
        $textPanelChildren.Add($desc)

        $rootPanel = [StackPanel]::new()
        $rootPanel.Margin = 4
        $rootPanel.Spacing = 8
        $rootPanel.Orientation = 'Horizontal'
        $rootPanelChildren = $rootPanel.Children
        $rootPanelChildren.Add($icon)
        $rootPanelChildren.Add($textPanel)

        $viewItems.Add($rootPanel)
    }
}

Main
