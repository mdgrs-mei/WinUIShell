using namespace WinUIShell.Microsoft.UI.Xaml
using namespace WinUIShell.Microsoft.UI.Xaml.Controls
using namespace WinUIShell.Microsoft.UI.Xaml.Media

if (-not (Get-Module WinUIShell)) {
    Import-Module WinUIShell
}

$resources = [Application]::Current.Resources
$win = [Window]::new()
$win.Title = 'Remove Object'
$win.SystemBackdrop = [MicaBackdrop]::new()
$win.AppWindow.ResizeClient(400, 220)

$listView = [ListView]::new()
$listView.Margin = 16
$listView.BorderThickness = 1
$listView.BorderBrush = $resources['SystemControlForegroundBaseMediumLowBrush']

$isClosed = $false
$win.Content = $listView
$win.AddClosed({
        $script:isClosed = $true
    })
$win.Activate()

$addCount = 0
while (-not $isClosed) {
    try {
        $items = $listView.Items
        if ($items.Count -gt 3) {
            $item = $items[0]
            $items.Remove($item) | Out-Null

            # Due to the RPC architecture, objects are not freed automatically.
            # This is usually not a problem, but if you repeatedly create and destroy objects, you should remove them explicitly to prevent memory leaks.
            Remove-WUIObject $item
        }

        $text = [TextBlock]::new()
        $text.Text = "Item $addCount"
        $items.Add($text)
        $addCount++
    } catch {
        # If the Window is closed, properties return null. Protect the null access with try catch.
        Write-Host 'Window is already closed.'
    }
    Start-Sleep -Milliseconds 200
}

# Explicitly frees all WinUIShell objects.
# This is not necessary if the process terminates here, but useful to free up resources if your script continues running in the background.
Remove-WUIObject -All
