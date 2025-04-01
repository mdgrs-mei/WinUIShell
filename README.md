<div align="center">

# WinUIShell

[![GitHub license](https://img.shields.io/github/license/mdgrs-mei/WinUIShell)](https://github.com/mdgrs-mei/WinUIShell/blob/main/LICENSE)
[![PowerShell Gallery](https://img.shields.io/powershellgallery/p/WinUIShell)](https://www.powershellgallery.com/packages/WinUIShell)
[![PowerShell Gallery](https://img.shields.io/powershellgallery/dt/WinUIShell)](https://www.powershellgallery.com/packages/WinUIShell)

Scripting WinUI 3 with PowerShell.

![Demo](https://github.com/user-attachments/assets/0ca145dd-bf42-4bf1-bbed-b06ef74ed101)

*WinUIShell* is a PowerShell module that allows you to create WinUI 3 applications in PowerShell.

</div>

> [!NOTE]
> This module is in its prototyping phase. Frequent breaking changes are expected until this notice is removed.

## Installation

```powershell
Install-PSResource -Name WinUIShell
```

## Requirements

- PowerShell 7.4 or newer
- Windows 10/11 build 10.0.17763.0 or newer
- Windows App Runtime 1.7.0 or newer
- .NET Desktop Runtime 8.0 or newer

## Quick Start

This code creates a Window that has a clickable button:

```powershell
Import-Module WinUIShell
using namespace WinUIShell

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
```

![QuickStart](https://github.com/user-attachments/assets/45b36c3c-1380-4384-bff2-18fe114c2dc1)

If you dot-source the script and comment out `$win.WaitForClosed()`, you can inspect UI objects or even modify them on the terminal:

```powershell
PS> $button

Content                    : Click Me
Background                 : WinUIShell.Brush
BackgroundSizing           : InnerBorderEdge
BorderBrush                : WinUIShell.Brush
BorderThickness            : 1,1,1,1
CharacterSpacing           : 0
:
```

## How It Works

*WinUIShell* launches a server process `WinUIShell.Server.exe` that provides all the UI functionalities. The WinUIShell module communicates with the server through IPC (Inter-Process Communication) to create UI elements and handle events. No WinUI 3 dlls are loaded in PowerShell.

<img src=https://github.com/user-attachments/assets/9de693bd-0071-4dd3-9826-5280d7a56d11 width="600">

This model simplifies the script structure. You can write long-running code in event handlers without blocking GUI. It's also allowed to access properties of UI elements directly on any thread without using Dispatchers.

This works:

```powershell
$status = [TextBlock]::new()
$progressBar = [ProgressBar]::new()
$button.AddClick({
    $button.IsEnabled = $false
    $status.Text = 'Downloading...'
    1..50 | ForEach-Object {
        $progressBar.Value = $_
        Start-Sleep -Milliseconds 50
    }

    $status.Text = 'Installing...'
    51..100 | ForEach-Object {
        $progressBar.Value = $_
        Start-Sleep -Milliseconds 50
    }

    $status.Text = '🎉Done!'
    $button.IsEnabled = $true
})
```

![ProgressBar](https://github.com/user-attachments/assets/1dc6bf2e-6529-4036-84b1-c20e8bcf9940)

## Major Limitations

- Not all UI elements are supported
- No XAML support

## Credits

*WinUIShell* uses:

- WindowsAppSDK<br>https://github.com/microsoft/WindowsAppSDK
- vs-StreamJsonRpc<br>https://github.com/microsoft/vs-streamjsonrpc
