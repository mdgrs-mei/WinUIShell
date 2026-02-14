using namespace WinUIShell
using namespace WinUIShell.Microsoft.UI.Xaml
using namespace WinUIShell.Microsoft.UI.Xaml.Controls
using namespace WinUIShell.Microsoft.UI.Xaml.Media

if (-not (Get-Module WinUIShell)) {
    Import-Module WinUIShell
}

$win = [Window]::new()
$win.Title = 'Cancel Long-Running EventCallback'
$win.SystemBackdrop = [DesktopAcrylicBackdrop]::new()
$win.AppWindow.TitleBar.PreferredTheme = 'UseDefaultAppMode'
$win.AppWindow.ResizeClient(420, 160)

# Use a synchronized hashtable to store objects that are accessed from multiple runspaces.
$syncHash = [Hashtable]::Synchronized(@{})

$progressRing = [ProgressRing]::new()
$progressRing.IsIndeterminate = $false
$progressRing.Value = 0
$syncHash.progressRing = $progressRing

$cancelButton = [Button]::new()
$cancelButton.HorizontalAlignment = 'Stretch'
$cancelButton.Content = 'Cancel'
$cancelButton.IsEnabled = $false
$syncHash.isCancel = $false
$syncHash.cancelButton = $cancelButton

# This cancel button's callback runs in the main runspace as [WinUIShell.EventCallbackRunspaceMode]::MainRunspaceAsyncUI is the default mode.
$cancelButton.AddClick({
        $syncHash.isCancel = $true
    })

$startButton = [Button]::new()
$startButton.HorizontalAlignment = 'Stretch'
$startButton.Content = 'Start'

# Create a custom callback using the EventCallback class to control the runspace mode.
$longRunningCallback = [EventCallback]::new()

# RunspacePoolAsyncUI runs the callback on a background thread, allowing the cancel button's callback to run in parallel.
$longRunningCallback.RunspaceMode = 'RunspacePoolAsyncUI'

# Since the runspace pool callbacks can run in parallel, disable the button to avoid being clicked multiple times.
$longRunningCallback.DisabledControlsWhileProcessing = $startButton

# Pass objects via ArgumentList as runspace pool callbacks run in separate runspaces like ThreadJobs.
$longRunningCallback.ArgumentList = $syncHash
$longRunningCallback.ScriptBlock = {
    param ($syncHash)
    $syncHash.isCancel = $false
    $syncHash.cancelButton.IsEnabled = $true

    1..10 | ForEach-Object {
        if ($syncHash.isCancel) {
            $syncHash.progressRing.Value = 0
            return
        }

        # Properties of WinUIShell objects are thread-safe and can be updated from any thread.
        $syncHash.progressRing.Value = $_ * 10

        # Do some long-running work.
        Start-Sleep -Milliseconds 200
    }

    $syncHash.cancelButton.IsEnabled = $false
}
$startButton.AddClick($longRunningCallback)

$panel = [StackPanel]::new()
$panel.Margin = 16
$panel.Spacing = 16
$panel.Orientation = 'Vertical'
$panel.Children.Add($progressRing)
$panel.Children.Add($startButton)
$panel.Children.Add($cancelButton)

$win.Content = $panel
$win.Activate()
$win.WaitForClosed()
