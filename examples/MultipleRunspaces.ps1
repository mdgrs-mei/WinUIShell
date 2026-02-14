using namespace WinUIShell
using namespace WinUIShell.Microsoft.UI.Xaml
using namespace WinUIShell.Microsoft.UI.Xaml.Controls
using namespace WinUIShell.Microsoft.UI.Xaml.Media

if (-not (Get-Module WinUIShell)) {
    Import-Module WinUIShell
}

$modulePath = (Get-Module WinUIShell).Path

"Main Runspace ID [$([Runspace]::DefaultRunspace.Id)]" | Write-Host
$win = [Window]::new()
$win.Title = 'Multiple Runspaces'
$win.AppWindow.ResizeClient(420, 240)
$win.AddClosed({
        # This callback is processed in the runspace where it's added, the main runspace.
        "Main window closed [$([Runspace]::DefaultRunspace.Id)]" | Write-Host
    })

$button = [Button]::new()
$button.Content = 'Main Runspace'
$clickCount = 0
$button.AddClick({
        # The callback here sees the global or script scoped variables in the main runspace.
        $script:clickCount++
        $button.Content = "Main Runspace[$([Runspace]::DefaultRunspace.Id)][$($script:clickCount)]"
    })

$panel = [StackPanel]::new()
$panel.Margin = 32
$panel.Children.Add($button)

$win.Content = $panel
$win.Activate()

$threadJob = Start-ThreadJob -ScriptBlock {
    param ($ModulePath)
    # By importing the WinUIShell module, the event callback processor for this runspace is also created.
    Import-Module $ModulePath

    "Sub Runspace ID [$([Runspace]::DefaultRunspace.Id)]" | Write-Host
    $win = [WinUIShell.Microsoft.UI.Xaml.Window]::new()
    $win.Title = 'Multiple Runspaces'
    $win.AppWindow.ResizeClient(420, 240)
    $win.AddClosed({
            # This callback is processed in the runspace where it's added, the sub runspace.
            "Sub window closed [$([Runspace]::DefaultRunspace.Id)]" | Write-Host
        })

    $button = [WinUIShell.Microsoft.UI.Xaml.Controls.Button]::new()
    $button.Content = 'Sub Runspace'
    $button.AddClick({
            # Long-running tasks here do not block the button click on the main window as this callback is processed independent of the main runspace.
            $button.Content = "Sub Runspace[$([Runspace]::DefaultRunspace.Id)] - Doing a long task"
            Start-Sleep -Seconds 5
            $button.Content = "Sub Runspace[$([Runspace]::DefaultRunspace.Id)] - Done"
        })

    $panel = [WinUIShell.Microsoft.UI.Xaml.Controls.StackPanel]::new()
    $panel.Margin = 32
    $panel.Children.Add($button)

    $win.Content = $panel
    $win.Activate()

    # Callbacks in the sub runspace are processed here.
    $win.WaitForClosed()
} -ArgumentList $modulePath -StreamingHost $host

# Callbacks in the main runspace are processed here.
$win.WaitForClosed()
Wait-Job $threadJob | Out-Null
