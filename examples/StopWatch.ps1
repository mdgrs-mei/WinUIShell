using namespace WinUIShell.Microsoft.UI.Xaml
using namespace WinUIShell.Microsoft.UI.Xaml.Controls
using namespace WinUIShell.Microsoft.UI.Xaml.Media

if (-not (Get-Module WinUIShell)) {
    Import-Module WinUIShell
}

$stopWatch = [System.Diagnostics.StopWatch]::new()

$resources = [Application]::Current.Resources
$win = [Window]::new()
$win.Title = 'StopWatch'
$win.SystemBackdrop = [MicaBackdrop]::new()
$win.AppWindow.ResizeClient(400, 220)

$timerText = [TextBlock]::new()
$timerText.Text = $stopWatch.Elapsed.ToString('mm\:ss\.ff')
$timerText.HorizontalAlignment = 'Center'
$timerText.FontFamily = 'Courier New'
$timerText.FontSize = 64

$button = [Button]::new()
$button.Content = 'Start'
$button.HorizontalAlignment = 'Stretch'
$button.FontSize = 20
$button.Style = $resources['AccentButtonStyle']
$button.AddClick({
        if ($stopWatch.IsRunning) {
            $stopWatch.Stop()
            $button.Content = 'Start'
        } else {
            $stopWatch.Start()
            $button.Content = 'Stop'
        }
    })

$panel = [StackPanel]::new()
$panel.Margin = 32
$panel.Spacing = 24
$panel.Children.Add($timerText)
$panel.Children.Add($button)

$isClosed = $false
$win.Content = $panel
$win.AddClosed({
        $script:isClosed = $true
    })
$win.Activate()

while (-not $isClosed) {
    # Event callbacks can be handled in this loop using a timer event.
    $timerText.Text = $stopWatch.Elapsed.ToString('mm\:ss\.ff')
    Start-Sleep -Milliseconds 10
}
