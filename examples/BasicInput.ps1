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

$radioButtons = [RadioButtons]::new()
$radioButtons.Header = 'Radio Buttons'
$radioButton1 = [RadioButton]::new()
$radioButton2 = [RadioButton]::new()
$radioButton3 = [RadioButton]::new()
$radioButton1.Content = 'Option1'
$radioButton2.Content = 'Option2'
$radioButton3.Content = 'Option3'
$radioButtons.Items.Add($radioButton1)
$radioButtons.Items.Add($radioButton2)
$radioButtons.Items.Add($radioButton3)
$radioButtons.AddSelectionChanged({
        param ($argumentList, $s, $selectionChangedEventArgs)
        # This event seems to be raised twice for checked and unchecked. Only print for the checked event.
        if ($selectionChangedEventArgs.AddedItems[0]) {
            Write-Host "Selection changed to [$($radioButtons.SelectedItem.Content)]"
        }
    })

$panel = [StackPanel]::new()
$panel.Margin = 32
$panel.Spacing = 12

$panel.Children.Add($toggleSwitch)
$panel.Children.Add($comboBox)
$panel.Children.Add($radioButtons)

$win.Content = $panel
$win.Activate()
$win.WaitForClosed()
