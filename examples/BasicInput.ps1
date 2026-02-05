using namespace WinUIShell
using namespace WinUIShell.Microsoft.UI.Xaml
using namespace WinUIShell.Microsoft.UI.Xaml.Controls
using namespace WinUIShell.Microsoft.UI.Xaml.Media

if (-not (Get-Module WinUIShell)) {
    Import-Module WinUIShell
}

$win = [Window]::new()
$win.SystemBackdrop = [DesktopAcrylicBackdrop]::new()
$win.AppWindow.ResizeClient(420, 420)

$toggleSwitch = [ToggleSwitch]::new()
$toggleSwitch.Header = 'ToggleSwitch'
$toggleSwitch.AddToggled({
        Write-Host "Toggled [$($toggleSwitch.IsOn)]"
    })

$comboBox = [ComboBox]::new()
$comboBox.Header = 'ComboBox'
$comboBox.Description = 'Description'
$comboBox.PlaceholderText = 'Placeholder'
$comboBox.Items.Add('Apple')
$comboBox.Items.Add('Banana')
$comboBox.Items.Add('Orange')
$comboBox.AddSelectionChanged({
        Write-Host "Selection changed to [$($comboBox.SelectedItem)]"
    })

$panel = [StackPanel]::new()
$panel.Margin = 32
$panel.Spacing = 12

$panel.Children.Add($toggleSwitch)
$panel.Children.Add($comboBox)

$win.Content = $panel
$win.Activate()
$win.WaitForClosed()
