using namespace WinUIShell
if (-not (Get-Module WinUIShell)) {
    Import-Module WinUIShell
}

$win = [Window]::new()
$win.Title = 'Hello from PowerShell!'
$win.AppWindow.ResizeClient(400, 200)

$button = [Button]::new()
$button.Content = 'Click Me'
$button.HorizontalAlignment = 'Center'
$button.VerticalAlignment = 'Center'
$button.AddClick({
        $button.Content = 'Clicked!'
    })

$win.Content = $button
# Activate() shows the window but does not block the script.
$win.Activate()
$win.WaitForClosed()
