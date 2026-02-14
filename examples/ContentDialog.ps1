using namespace WinUIShell
using namespace WinUIShell.Microsoft.UI.Xaml
using namespace WinUIShell.Microsoft.UI.Xaml.Controls
using namespace WinUIShell.Microsoft.UI.Xaml.Media

if (-not (Get-Module WinUIShell)) {
    Import-Module WinUIShell
}

$win = [Window]::new()
$win.Title = 'ContentDialog'
$win.SystemBackdrop = [DesktopAcrylicBackdrop]::new()
$win.AppWindow.ResizeClient(600, 400)

$button = [Button]::new()
$button.HorizontalAlignment = 'Center'
$button.VerticalAlignment = 'Center'
$button.Content = 'Open'
$button.AddClick({
        param ($argumentList, $s, $e)

        $senderButton = $s
        $dialog = [ContentDialog]::new()
        $dialog.XamlRoot = $senderButton.XamlRoot
        $dialog.Title = 'Save your work?'
        $dialog.PrimaryButtonText = 'Save'
        $dialog.SecondaryButtonText = 'Don''t Save'
        $dialog.CloseButtonText = 'Cancel'
        $dialog.DefaultButton = 'Primary'

        $contentCheck = [CheckBox]::new()
        $contentCheck.Content = 'Custom control in the content'
        $contentText = [TextBlock]::new()
        $contentText.Text = 'This is a content string.'

        $contentPanel = [StackPanel]::new()
        $contentPanel.Spacing = 4
        $contentPanel.Children.Add($contentText)
        $contentPanel.Children.Add($contentCheck)

        $dialog.Content = $contentPanel

        $script:openedDialog = $dialog
        $result = $dialog.ShowAsync().WaitForCompleted()
        $script:openedDialog = $null
        $status.Text = "You pressed [$result]. CheckBox is [$($contentCheck.IsChecked)]."
    })

$status = [TextBlock]::new()
$status.HorizontalAlignment = 'Center'

$panel = [StackPanel]::new()
$panel.VerticalAlignment = 'Center'
$panel.Spacing = 16
$panel.Children.Add($button)
$panel.Children.Add($status)

$win.Content = $panel

# ContentDialog does not close automatically when closing the window.
# Close the dialog here if it's open so that the button callback can get out of the WaitForCompleted() loop.
# Since UI elements cannot be accessed after closing the window, we use the Closing event and MainRunspaceSyncUI to make sure it's called before closing the window.
$openedDialog = $null
$closingCallback = [EventCallback]@{
    RunspaceMode = [EventCallbackRunspaceMode]::MainRunspaceSyncUI
    ScriptBlock = {
        if ($script:openedDialog) {
            $script:openedDialog.Hide()
        }
    }
}
$win.AppWindow.AddClosing($closingCallback)

$win.Activate()
$win.WaitForClosed()
