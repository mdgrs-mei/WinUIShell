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

## Quick Start

This code creates a Window that has a clickable button:

```powershell
using namespace WinUIShell
Import-Module WinUIShell

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

Content             : Click Me
Background          : WinUIShell.Brush
BackgroundSizing    : InnerBorderEdge
BorderBrush         : WinUIShell.Brush
BorderThickness     : 1,1,1,1
CharacterSpacing    : 0
:
```

Since the API of WinUIShell tries following the WinUI 3's API, you can read the Windows App SDK documentation to see what should be available. The documentation of the `Button` class is [here](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.button?view=windows-app-sdk-1.7) for example.

You can also look at the [examples](./examples/) folder for script examples.

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

## XamlReader

Instead of creating UI elements by code, you can also create them by loading XAML similar to WPF in PowerShell. You can search for an UI element by the `FindName` method of `FrameworkElement` and add event handlers from PowerShell. Note that you can't use `x:Class` attributes or code-behind binding in XAML as mentioned in this [documentation](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.markup.xamlreader).

```powershell
$xamlString = @'
<Window
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

    <StackPanel VerticalAlignment="Center" HorizontalAlignment="Center">
        <Button x:Name="button" Content="Click Me"/>
    </StackPanel>
</Window>
'@

$win = [XamlReader]::Load($xamlString)
$win.AppWindow.ResizeClient(400, 200)
# FrameworkElement has FindName method (Window doesn't have it).
$button = $win.Content.FindName('button')
$button.AddClick({
    $button.Content = 'Clicked!'
})

$win.Activate()
$win.WaitForClosed()
```

You can use any UI element that is available in Windows App SDK but it has to be a type that is supported in WinUIShell to get the object or access properties in PowerShell. It throws an error when calling `FindName` if the type is not supported by WinUIShell.

## Major Limitations

- Not all UI elements are supported yet

## Contributing

### Code of Conduct

Please read our [Code of Conduct](./CODE_OF_CONDUCT.md) to foster a welcoming environment. By participating in this project, you are expected to uphold this code.

### Have a question or want to showcase something?

Please come to our [Discussions](https://github.com/mdgrs-mei/WinUIShell/discussions) page and avoid filing an issue to ask a question.

### Want to file an issue or make a PR?

Please see our [Contribution Guidelines](./CONTRIBUTING.md).

## Credits

*WinUIShell* uses:

- WindowsAppSDK<br>https://github.com/microsoft/WindowsAppSDK
- vs-StreamJsonRpc<br>https://github.com/microsoft/vs-streamjsonrpc
