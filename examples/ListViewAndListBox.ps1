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

    $listView = [ListView]::new()
    $listView.Margin = 16
    $listView.BorderThickness = 1
    $listView.BorderBrush = $resources['SystemControlForegroundBaseMediumLowBrush']
    $listView.CanReorderItems = $true
    $listView.AllowDrop = $true
    AddListItems $listView
    [Grid]::SetRow($listView, 0)
    [Grid]::SetColumn($listView, 0)

    $listViewMultiSelection = [ToggleSwitch]::new()
    $listViewMultiSelection.Margin = 16
    $listViewMultiSelection.HorizontalAlignment = 'Left'
    $listViewMultiSelection.IsOn = $false
    $listViewMultiSelection.Header = 'Multi Selection'
    $listViewMultiSelection.AddToggled({
            param ($ListView, $SenderToggle)
            if ($SenderToggle.IsOn) {
                $ListView.SelectionMode = 'Multiple'
            } else {
                $ListView.SelectionMode = 'Single'
            }
        }, $listView)
    [Grid]::SetRow($listViewMultiSelection, 1)
    [Grid]::SetColumn($listViewMultiSelection, 0)

    $listViewRemoveItemButton = [Button]::new()
    $listViewRemoveItemButton.Margin = [Thickness]::new(16, 0, 16, 16)
    $listViewRemoveItemButton.HorizontalAlignment = 'Stretch'
    $listViewRemoveItemButton.Content = 'Remove Selection'
    $listViewRemoveItemButton.AddClick({
            param ($ListView)
            $removedItems = @()
            $ListView.SelectedItems | ForEach-Object { $removedItems += $_ }
            $removedItems | ForEach-Object { $ListView.Items.Remove($_) }
        }, $listView)
    [Grid]::SetRow($listViewRemoveItemButton, 2)
    [Grid]::SetColumn($listViewRemoveItemButton, 0)

    $listBox = [ListBox]::new()
    $listBox.Margin = 16
    $listBox.BorderThickness = 1
    $listBox.BorderBrush = $resources['SystemControlForegroundBaseMediumLowBrush']
    AddListItems $listBox
    [Grid]::SetRow($listBox, 0)
    [Grid]::SetColumn($listBox, 1)

    $listBoxMultiSelection = [ToggleSwitch]::new()
    $listBoxMultiSelection.Margin = 16
    $listBoxMultiSelection.HorizontalAlignment = 'Left'
    $listBoxMultiSelection.IsOn = $false
    $listBoxMultiSelection.Header = 'Multi Selection'
    $listBoxMultiSelection.AddToggled({
            param ($ListBox, $SenderToggle)
            if ($SenderToggle.IsOn) {
                $ListBox.SelectionMode = 'Multiple'
            } else {
                $ListBox.SelectionMode = 'Single'
            }
        }, $listBox)
    [Grid]::SetRow($listBoxMultiSelection, 1)
    [Grid]::SetColumn($listBoxMultiSelection, 1)

    $listBoxRemoveItemButton = [Button]::new()
    $listBoxRemoveItemButton.Margin = [Thickness]::new(16, 0, 16, 16)
    $listBoxRemoveItemButton.HorizontalAlignment = 'Stretch'
    $listBoxRemoveItemButton.Content = 'Remove Selection'
    $listBoxRemoveItemButton.AddClick({
            param ($ListBox)
            $removedItems = @()
            $ListBox.SelectedItems | ForEach-Object { $removedItems += $_ }
            $removedItems | ForEach-Object { $ListBox.Items.Remove($_) }
        }, $listBox)
    [Grid]::SetRow($listBoxRemoveItemButton, 2)
    [Grid]::SetColumn($listBoxRemoveItemButton, 1)

    $row0 = [RowDefinition]::new()
    $row0.Height = [GridLength]::new(1, 'Star')
    $row1 = [RowDefinition]::new()
    $row1.Height = [GridLength]::Auto
    $row2 = [RowDefinition]::new()
    $row2.Height = [GridLength]::Auto
    $column0 = [ColumnDefinition]::new()
    $column0.Width = [GridLength]::new(1, 'Star')
    $column1 = [ColumnDefinition]::new()
    $column1.Width = [GridLength]::new(1, 'Star')

    $grid = [Grid]::new()
    $grid.RowDefinitions.Add($row0)
    $grid.RowDefinitions.Add($row1)
    $grid.RowDefinitions.Add($row2)
    $grid.ColumnDefinitions.Add($column0)
    $grid.ColumnDefinitions.Add($column1)
    $grid.Children.Add($listView)
    $grid.Children.Add($listViewMultiSelection)
    $grid.Children.Add($listViewRemoveItemButton)
    $grid.Children.Add($listBox)
    $grid.Children.Add($listBoxMultiSelection)
    $grid.Children.Add($listBoxRemoveItemButton)

    $win.Content = $grid
    $win.Activate()
    $win.WaitForClosed()
}

function AddListItems($List) {
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

    $listItems = $List.Items
    $nameStyle = $resources['BodyStrongTextBlockStyle']
    $descStyle = $resources['CaptionTextBlockStyle']
    $bitmap = [BitmapImage]::new("$PSScriptRoot/resources/Circle.png")

    foreach ($itemName in $itemNames) {
        $icon = [ImageIcon]::new()
        $icon.Width = 32
        $icon.Source = $bitmap

        $name = [TextBlock]::new()
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

        $listItems.Add($rootPanel)
    }
}

Main
