using namespace WinUIShell

$modulePath = "$PSScriptRoot\..\module\WinUIShell"
Import-Module $modulePath

$win = [Window]::new()
$win.Title = 'Multi Threading'
$win.AppWindow.ResizeClient(420, 240)
$win.AddClosed({
        Write-Host ('Main window closed [{0}]' -f ([Environment]::CurrentManagedThreadId))
    })

$button = [Button]::new()
$button.Content = 'MainThread'
$clickCount = 0
Write-Host ('Main Thread ID [{0}]' -f ([Environment]::CurrentManagedThreadId))
$button.AddClick({
        $script:clickCount++
        $button.Content = 'MainThread[{0}][{1}]' -f ([Environment]::CurrentManagedThreadId), $script:clickCount
    })

$panel = [StackPanel]::new()
$panel.Margin = 32
$panel.Children.Add($button)

$win.Content = $panel
$win.Activate()

$threadJob = Start-ThreadJob -ScriptBlock {
    param ($modulePath)
    Write-Host ('Sub Thread ID [{0}]' -f ([Environment]::CurrentManagedThreadId))
    Import-Module $modulePath

    $win = [WinUIShell.Window]::new()
    $win.Title = 'Multi Threading'
    $win.AppWindow.ResizeClient(420, 240)
    $win.AddClosed({
            Write-Host ('Sub window closed [{0}]' -f ([Environment]::CurrentManagedThreadId))
        })

    $button = [WinUIShell.Button]::new()
    $button.Content = 'SubThread'
    $button.AddClick({
            $button.Content = 'SubThread[{0}] - Doing a long task' -f ([Environment]::CurrentManagedThreadId)
            # Doing a long task
            Start-Sleep -Seconds 5
            $button.Content = 'SubThread[{0}] - Done' -f ([Environment]::CurrentManagedThreadId)
        })

    $panel = [WinUIShell.StackPanel]::new()
    $panel.Margin = 32
    $panel.Children.Add($button)

    $win.Content = $panel
    $win.Activate()
    $win.WaitForClosed()
} -ArgumentList $modulePath -StreamingHost $host

$win.WaitForClosed()
Wait-Job $threadJob | Out-Null
