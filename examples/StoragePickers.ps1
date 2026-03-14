using namespace WinUIShell
using namespace WinUIShell.Microsoft.UI.Xaml
using namespace WinUIShell.Microsoft.UI.Xaml.Controls
using namespace WinUIShell.Microsoft.UI.Xaml.Media
using namespace WinUIShell.Microsoft.Windows.Storage.Pickers

if (-not (Get-Module WinUIShell)) {
    Import-Module WinUIShell
}

$win = [Window]::new()
$win.Title = 'Storage Pickers'
$win.SystemBackdrop = [DesktopAcrylicBackdrop]::new()
$win.AppWindow.ResizeClient(600, 240)

# FileOpenPicker
$textBox0 = [TextBox]::new()
$textBox0.IsReadonly = $true
$button0 = [Button]::new()
$button0.Content = 'Pick Single File'
$button0.AddClick({
        param ($TextBox, $s, $e)

        $senderButton = $s
        $picker = [FileOpenPicker]::new($senderButton.XamlRoot.ContentIslandEnvironment.AppWindowId)
        $picker.CommitButtonText = 'Pick Single File'
        $picker.FileTypeFilter.Add('.ps1')
        $picker.FileTypeFilter.Add('.cs')
        $result = $picker.PickSingleFileAsync().WaitForCompleted()
        $TextBox.Text = $result.Path
    }, $textBox0)
[Grid]::SetRow($textBox0, 0)
[Grid]::SetColumn($textBox0, 0)
[Grid]::SetRow($button0, 0)
[Grid]::SetColumn($button0, 1)

$textBox1 = [TextBox]::new()
$textBox1.IsReadonly = $true
$button1 = [Button]::new()
$button1.Content = 'Pick Multiple Files'
$button1.AddClick({
        param ($TextBox, $s, $e)

        $senderButton = $s
        $picker = [FileOpenPicker]::new($senderButton.XamlRoot.ContentIslandEnvironment.AppWindowId)
        $picker.CommitButtonText = 'Pick Multiple Files'
        $results = $picker.PickMultipleFilesAsync().WaitForCompleted()
        $TextBox.Text = $results.Path -join ';'
    }, $textBox1)
[Grid]::SetRow($textBox1, 1)
[Grid]::SetColumn($textBox1, 0)
[Grid]::SetRow($button1, 1)
[Grid]::SetColumn($button1, 1)

# FileSavePicker
$textBox2 = [TextBox]::new()
$textBox2.IsReadonly = $true
$button2 = [Button]::new()
$button2.Content = 'Save File'
$button2.AddClick({
        param ($TextBox, $s, $e)

        $senderButton = $s
        $picker = [FileSavePicker]::new($senderButton.XamlRoot.ContentIslandEnvironment.AppWindowId)
        $picker.CommitButtonText = 'Save File'
        $picker.SuggestedFileName = 'DefaultFileName'
        $picker.DefaultFileExtension = '.ps1'

        $fileTypes = [WinUIShell.System.Collections.Generic.List[string]]::new()
        $fileTypes.Add('.ps1')
        $fileTypes.Add('.psd1')
        $fileTypes.Add('.psm1')
        $picker.FileTypeChoices.Add('PowerShell files', $fileTypes)

        $result = $picker.PickSaveFileAsync().WaitForCompleted()
        $TextBox.Text = $result.Path
    }, $textBox2)
[Grid]::SetRow($textBox2, 2)
[Grid]::SetColumn($textBox2, 0)
[Grid]::SetRow($button2, 2)
[Grid]::SetColumn($button2, 1)

# FolderPicker
$textBox3 = [TextBox]::new()
$textBox3.IsReadonly = $true
$button3 = [Button]::new()
$button3.Content = 'Pick Folder'
$button3.AddClick({
        param ($TextBox, $s, $e)

        $senderButton = $s
        $picker = [FolderPicker]::new($senderButton.XamlRoot.ContentIslandEnvironment.AppWindowId)
        $picker.CommitButtonText = 'Pick Folder'
        $result = $picker.PickSingleFolderAsync().WaitForCompleted()
        $TextBox.Text = $result.Path
    }, $textBox3)
[Grid]::SetRow($textBox3, 3)
[Grid]::SetColumn($textBox3, 0)
[Grid]::SetRow($button3, 3)
[Grid]::SetColumn($button3, 1)

$row0 = [RowDefinition]::new()
$row0.Height = [GridLength]::Auto
$row1 = [RowDefinition]::new()
$row1.Height = [GridLength]::Auto
$row2 = [RowDefinition]::new()
$row2.Height = [GridLength]::Auto
$row3 = [RowDefinition]::new()
$row3.Height = [GridLength]::Auto

$col0 = [ColumnDefinition]::new()
$col0.Width = [GridLength]::new(1, 'Star')
$col1 = [ColumnDefinition]::new()
$col1.Width = [GridLength]::Auto

$grid = [Grid]::new()
$grid.Margin = 16
$grid.RowSpacing = 16
$grid.ColumnSpacing = 4
$grid.RowDefinitions.Add($row0)
$grid.RowDefinitions.Add($row1)
$grid.RowDefinitions.Add($row2)
$grid.RowDefinitions.Add($row3)
$grid.ColumnDefinitions.Add($col0)
$grid.ColumnDefinitions.Add($col1)

$grid.Children.Add($textBox0)
$grid.Children.Add($button0)
$grid.Children.Add($textBox1)
$grid.Children.Add($button1)
$grid.Children.Add($textBox2)
$grid.Children.Add($button2)
$grid.Children.Add($textBox3)
$grid.Children.Add($button3)

$win.Content = $grid
$win.Activate()
$win.WaitForClosed()
