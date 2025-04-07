using namespace WinUIShell
Import-Module "$PSScriptRoot\..\module\WinUIShell"

$win = [Window]::new()
$win.Title = 'Cancel Progress Ring'
$win.SystemBackdrop = [DesktopAcrylicBackdrop]::new()
$win.AppWindow.TitleBar.PreferredTheme = 'UseDefaultAppMode'
$win.AppWindow.ResizeClient(300, 200)

$progressRing = [ProgressRing]::new()
$progressRing.IsIndeterminate = $false
$progressRing.Value = 0
$toggle = [ToggleSwitch]::new()
$toggle.IsOn = $false
$toggle.AddToggled({
        if ($toggle.IsOn) {
            1..10 | ForEach-Object {
                # Toggled event is fired even in this loop by an idle timer event.
                # You can cancel the progress ring by checking the toggle state.
                if (-not $toggle.IsOn) {
                    $progressRing.Value = 0
                    return
                }

                $progressRing.Value = $_ * 10

                # The idle timer event could be fired here too.
                if (-not $toggle.IsOn) {
                    $progressRing.Value = 0
                    return
                }

                Start-Sleep -Milliseconds 100
            }
            $progressRing.Value = 100
        } else {
            $progressRing.Value = 0
        }
    })

$panel = [StackPanel]::new()
$panel.Margin = 48
$panel.Orientation = 'Horizontal'
$panel.Children.Add($toggle)
$panel.Children.Add($progressRing)

$win.Content = $panel
$win.Activate()
$win.WaitForClosed()
