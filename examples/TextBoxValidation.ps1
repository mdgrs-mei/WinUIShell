using namespace WinUIShell
using namespace WinUIShell.Microsoft.UI.Xaml
using namespace WinUIShell.Microsoft.UI.Xaml.Controls
using namespace WinUIShell.Microsoft.UI.Xaml.Media

if (-not (Get-Module WinUIShell)) {
    Import-Module WinUIShell
}

$win = [Window]::new()
$win.Title = 'TextBox Validation'
$win.SystemBackdrop = [DesktopAcrylicBackdrop]::new()
$win.AppWindow.ResizeClient(420, 160)

$textBox = [TextBox]::new()
$textBox.Header = 'Account Name'
$textBox.PlaceHolderText = 'Your account name'
$textBox.Margin = [Thickness]::new(0, 0, 0, 24)

$beforeTextChangingCallback = [EventCallback]::new()
# BeforeTextChanging event handler must use MainRunspaceSyncUI runspace mode to make the Cancel property work.
$beforeTextChangingCallback.RunspaceMode = [EventCallbackRunspaceMode]::MainRunspaceSyncUI
$beforeTextChangingCallback.ScriptBlock = {
    param ($argumentList, $s, $e)
    if ($e.NewText.Length -gt 3) {
        $e.Cancel = $true
        $textBox.Description = 'Account Name must be less than 4 characters'
    } else {
        $textBox.Description = ''
    }
}
$textBox.AddBeforeTextChanging($beforeTextChangingCallback)

$panel = [StackPanel]::new()
$panel.Margin = 32
$panel.Children.Add($textBox)

$win.Content = $panel
$win.Activate()
$win.WaitForClosed()
