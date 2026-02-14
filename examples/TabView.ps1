using namespace WinUIShell
using namespace WinUIShell.Microsoft.UI.Xaml
using namespace WinUIShell.Microsoft.UI.Xaml.Controls

if (-not (Get-Module WinUIShell)) {
    Import-Module WinUIShell
}

$pageCount = 0
function Main() {
    $win = [Window]::new()
    $win.Title = 'TabView'
    $win.AppWindow.ResizeClient(800, 600)

    $tabView = [TabView]::new()
    $tabView.TabItems.Add((CreateTabViewItem))
    $tabView.TabItems.Add((CreateTabViewItem))

    $tabView.AddAddTabButtonClick({
            param ($argumentList, $tabView, $e)
            $tabView.TabItems.Add((CreateTabViewItem))
        })

    $tabView.AddTabCloseRequested({
            param ($argumentList, $tabView, $tabCloseRequestedEventArgs)
            $tabView.TabItems.Remove($tabCloseRequestedEventArgs.Tab)
        })

    $win.Content = $tabView
    $win.Activate()
    $win.WaitForClosed()
}

function CreateTabViewItem() {
    $script:pageCount++

    $pageName = "Page $pageCount"
    $item = [TabViewItem]::new()
    $item.Header = $pageName
    $item.IconSource = [SymbolIconSource]@{ Symbol = 'Document' }

    $editBox = [RichEditBox]::new()
    $editBox.Height = 540
    $editBox.Document.SetText([WinUIShell.Windows.UI.Text.TextSetOptions]::None, $pageName)

    $item.Content = $editBox
    $item
}

Main
