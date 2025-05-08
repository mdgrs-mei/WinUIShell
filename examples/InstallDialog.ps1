using namespace WinUIShell
if (-not (Get-Module WinUIShell)) {
    Import-Module WinUIShell
}

$resources = [Application]::Current.Resources
$win = [Window]::new()
$win.SystemBackdrop = [MicaBackdrop]::new()
$win.ExtendsContentIntoTitleBar = $true

$presenter = [CompactOverlayPresenter]::new()
$win.AppWindow.SetPresenter($presenter)
$win.AppWindow.ResizeClient(420, 260)

$icon = [SymbolIcon]::new('Sync')

$title = [TextBlock]::new()
$title.Text = 'Upgrade PowerShell'
$title.Style = $resources['TitleTextBlockStyle']

$titlePanel = [StackPanel]::new()
$titlePanel.Orientation = 'Horizontal'
$titlePanel.Spacing = 16
$titlePanel.Margin = [Thickness]::new(0, 12, 0, 24)
$titlePanel.Children.Add($icon)
$titlePanel.Children.Add($title)

$bodyText = [TextBlock]::new()
$bodyText.Text = 'pwsh.exe 7.5.0 - Microsoft Windows 10.0.26100'
$bodyText.Style = $resources['BodyStrongTextBlockStyle']
$bodyText.Margin = [Thickness]::new(0, 0, 0, 12)

$status = [TextBlock]::new()
$status.Text = 'Ready to upgrade'
$status.Margin = [Thickness]::new(0, 0, 0, 24)

$pb = [ProgressBar]::new()
$pb.Margin = [Thickness]::new(0, 0, 0, 24)

$pressToClose = $false
$button = [Button]::new()
$button.HorizontalAlignment = 'Right'
$button.Content = 'Upgrade'
$button.Style = $resources['AccentButtonStyle']
$button.AddClick({
        if ($script:pressToClose) {
            $win.Close()
            return
        }
        $button.IsEnabled = $false
        $status.Text = 'Downloading...'
        1..20 | ForEach-Object {
            $pb.Value = $_
            Start-Sleep -Milliseconds 100
        }

        $status.Text = 'Installing...'
        21..40 | ForEach-Object {
            $pb.Value = $_
            Start-Sleep -Milliseconds 100
        }

        $status.Text = 'Error!'
        $pb.ShowError = $true
        Start-Sleep -Milliseconds 2000

        $status.Text = 'Restoring...'
        $pb.IsIndeterminate = $true
        $pb.ShowError = $false
        Start-Sleep -Milliseconds 2000

        $status.Text = 'Installing...'
        $pb.IsIndeterminate = $false
        41..100 | ForEach-Object {
            $pb.Value = $_
            Start-Sleep -Milliseconds 50
        }

        $status.Text = '🎉Done!'

        $button.Content = 'Close'
        $script:pressToClose = $true
        $button.IsEnabled = $true
    })

$panel = [StackPanel]::new()
$panel.Margin = 32

$panel.Children.Add($titlePanel)
$panel.Children.Add($bodyText)
$panel.Children.Add($status)
$panel.Children.Add($pb)
$panel.Children.Add($button)

$win.Content = $panel
$win.Activate()
$win.WaitForClosed()
