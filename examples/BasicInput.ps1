using namespace WinUIShell
using namespace WinUIShell.Microsoft.UI.Xaml
using namespace WinUIShell.Microsoft.UI.Xaml.Controls
using namespace WinUIShell.Microsoft.UI.Xaml.Controls.Primitives
using namespace WinUIShell.Microsoft.UI.Xaml.Media

if (-not (Get-Module WinUIShell)) {
    Import-Module WinUIShell
}

$win = [Window]::new()
$win.SystemBackdrop = [DesktopAcrylicBackdrop]::new()
$win.AppWindow.ResizeClient(800, 700)

$toggleSwitch = [ToggleSwitch]::new()
$toggleSwitch.Header = 'ToggleSwitch'
$toggleSwitch.AddToggled({
        Write-Host "Toggled [$($toggleSwitch.IsOn)]"
    })

$toggleButton = [ToggleButton]::new()
$toggleButton.Content = 'ToggleButton'
$toggleButton.AddClick({
        Write-Host "Toggled [$($toggleButton.IsChecked)]"
    })

$checkBox = [CheckBox]::new()
$checkBox.Content = 'CheckBox'
$checkBox.AddChecked({
        Write-Host 'CheckBox is checked'
    })
$checkBox.AddUnchecked({
        Write-Host 'CheckBox is unchecked'
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

$leftPanel = [StackPanel]::new()
$leftPanel.Spacing = 16
$leftPanel.Children.Add($toggleButton)
$leftPanel.Children.Add($toggleSwitch)
$leftPanel.Children.Add($checkBox)
$leftPanel.Children.Add($comboBox)
$leftPanel.Children.Add($radioButtons)

$colorPicker = [ColorPicker]::new()
$colorPicker.IsMoreButtonVisible = $true
$colorPicker.IsAlphaEnabled = $true
$colorPicker.IsAlphaSliderVisible = $true
$colorPicker.AddColorChanged({
        Write-Host "Color changed to [$($colorPicker.Color)]"
    })

$horizontalSlider = [Slider]::new()
$horizontalSlider.Width = 200
$horizontalSlider.AddValueChanged({
        Write-Host "Slider value changed to [$($horizontalSlider.Value)]"
    })

$verticalSlider = [Slider]::new()
$verticalSlider.Orientation = 'Vertical'
$verticalSlider.Height = 200
$verticalSlider.TickPlacement = 'Outside'
$verticalSlider.TickFrequency = 20
$verticalSlider.AddValueChanged({
        Write-Host "Slider value changed to [$($verticalSlider.Value)]"
    })

$ratingControl = [RatingControl]::new()
$ratingControl.Caption = 'RatingControl'
$ratingControl.AddValueChanged({
        Write-Host "Rating changed to [$($ratingControl.Value)]"
    })

$rightPanel = [StackPanel]::new()
$rightPanel.Spacing = 16
$rightPanel.Children.Add($horizontalSlider)
$rightPanel.Children.Add($verticalSlider)
$rightPanel.Children.Add($ratingControl)

$rootPanel = [StackPanel]::new()
$rootPanel.Orientation = 'Horizontal'
$rootPanel.Margin = 32
$rootPanel.Spacing = 32
$rootPanel.Children.Add($leftPanel)
$rootPanel.Children.Add($colorPicker)
$rootPanel.Children.Add($rightPanel)

$win.Content = $rootPanel
$win.Activate()
$win.WaitForClosed()
