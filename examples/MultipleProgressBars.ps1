# See CancelLongRunningEventCallback.ps1 first for basic usage of runspace pool event callbacks.

using namespace WinUIShell
using namespace WinUIShell.Microsoft.UI.Xaml
using namespace WinUIShell.Microsoft.UI.Xaml.Controls
using namespace WinUIShell.Microsoft.UI.Xaml.Media

$resources = [Application]::Current.Resources
$syncHashList = @()

function Main() {
    if (-not (Get-Module WinUIShell)) {
        Import-Module WinUIShell
    }

    # This script runs in the global scope of each runspace in the runspace pool.
    $runspaceInitializationScript = {
        param ($ScriptRoot)
        "ScriptRoot: $ScriptRoot Runspace:$([Runspace]::DefaultRunspace.Id)" | Write-Host

        # Functions and variables defined here become available in each runspace.
        function SetPause($SyncHash, $IsPaused) {
            $SyncHash.isPaused = $IsPaused
            if ($IsPaused) {
                $SyncHash.pauseButton.Content = $SyncHash.playIcon
            } else {
                $SyncHash.pauseButton.Content = $SyncHash.pauseIcon
            }
        }
    }

    # RunspaceCount defines the number of event callbacks that can run in parallel at a time.
    Set-WUIRunspacePoolOption -RunspaceCount 3 -InitializationScript $runspaceInitializationScript -InitializationScriptArgumentList $PSScriptRoot

    # Dot source the initialization script here to make the functions available in the default runspace too.
    . $runspaceInitializationScript $PSScriptRoot

    $win = [Window]::new()
    $win.Title = 'Multiple ProgressBars'
    $win.AppWindow.TitleBar.PreferredTheme = 'UseDefaultAppMode'
    $win.AppWindow.ResizeClient(420, 200)

    $win.AddClosed({
            # $win.WaitForClosed() waits until all the event callbacks associated with the window finish so
            # clear the pause state to resume the event callbacks.
            foreach ($syncHash in $script:syncHashList) {
                SetPause $syncHash $false
            }
        })


    $panel = [StackPanel]::new()
    $panel.Margin = 16
    $panel.Spacing = 16
    $panel.Orientation = 'Vertical'
    $panel.Children.Add((CreateProgressBarSet))
    $panel.Children.Add((CreateProgressBarSet))
    $panel.Children.Add((CreateProgressBarSet))

    $win.Content = $panel
    $win.Activate()
    $win.WaitForClosed()
}

function CreateProgressBarSet() {
    $syncHash = [Hashtable]::Synchronized(@{})
    $script:syncHashList += $syncHash

    $progressBar = [ProgressBar]::new()
    $progressBar.Width = 200
    $progressBar.Value = 0
    $syncHash.progressBar = $progressBar

    $syncHash.pauseIcon = [SymbolIcon]::new('Pause')
    $syncHash.playIcon = [SymbolIcon]::new('Play')

    $pauseButton = [Button]::new()
    $pauseButton.Content = $syncHash.pauseIcon
    $pauseButton.IsEnabled = $false
    $pauseCallback = {
        # This runs in the default runspace.
        param ($SyncHash)
        SetPause $SyncHash (-not $SyncHash.isPaused)
    }
    $pauseButton.AddClick($pauseCallback, $syncHash)
    $syncHash.pauseButton = $pauseButton
    $syncHash.isPaused = $false

    $startButton = [Button]::new()
    $startButton.Content = 'Start'
    $startButton.Style = $script:resources['AccentButtonStyle']

    $startCallback = [EventCallback]::new()
    $startCallback.DisabledControlsWhileProcessing = $startButton
    $startCallback.RunspaceMode = 'RunspacePoolAsyncUI'
    $startCallback.ArgumentList = $syncHash
    $startCallback.ScriptBlock = {
        # This runs in the runspace pool.
        param ($SyncHash)
        SetPause $SyncHash $false
        $SyncHash.pauseButton.IsEnabled = $true

        1..100 | ForEach-Object {
            while ($SyncHash.isPaused) {
                Start-Sleep -Milliseconds 20
            }
            $SyncHash.progressBar.Value = $_
            Start-Sleep -Milliseconds 20
        }

        $SyncHash.pauseButton.IsEnabled = $false
    }
    $startButton.AddClick($startCallback)

    $panel = [StackPanel]::new()
    $panel.Spacing = 16
    $panel.Orientation = 'Horizontal'
    $panel.Children.Add($progressBar)
    $panel.Children.Add($startButton)
    $panel.Children.Add($pauseButton)
    $panel
}

Main
