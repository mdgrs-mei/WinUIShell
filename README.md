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

## Event Callback

Event callbacks are script blocks that are invoked when UI events are fired. You can register them with `Add*` methods of UI elements. The typical example is the click event of a button:

```powershell
$button = [Button]::new()
$argumentList = 1, 2
$button.AddClick({
    param ($argumentList, $s, $e)
    Write-Host "ArgumentList: $argumentList"
    Write-Host "Sender: $s"
    Write-Host "EventArgs: $e"
}, $argumentList)
```

The same code can also be written using the `EventCallback` class. It allows you to customize the callback behavior:

```powershell
$button = [Button]::new()
$clickCallback = [EventCallback]::new()
$clickCallback.RunspaceMode = 'RunspacePoolAsyncUI'
$clickCallback.DisabledControlsWhileProcessing = $button
$clickCallback.ArgumentList = 1, 2
$clickCallback.ScriptBlock = {
    param ($argumentList, $s, $e)
    Write-Host "ArgumentList: $argumentList"
    Write-Host "Sender: $s"
    Write-Host "EventArgs: $e"
}
$button.AddClick($clickCallback)
```

The `RunspaceMode` property controls where and how the script block runs.

```powershell
$clickCallback.RunspaceMode = 'RunspacePoolAsyncUI'
```

There are three runspace modes, `MainRunspaceAsyncUI`, `MainRunspaceSyncUI`, and `RunspacePoolAsyncUI`.

### MainRunspaceAsyncUI (Default)

The default value of `RunspaceMode` is `MainRunspaceAsyncUI` which means that the script block runs in the runspace where the script block is created (Main runspace). The script block can be a bound script block and sees the global or script scope variables in the runspace.

`AsyncUI` means that the callbacks do not block the UI thread on the server side. Even if a callback takes long time to finish, the UI stays responsive. If a button is pressed while the previous callback is running, the new callback is queued and processed after the previous one completes. If this behavior is not desirable, you can specify controls that are disabled on the server side while the event callback is running:


```powershell
$clickCallback.DisabledControlsWhileProcessing = $button
```

It is a good practice to set the `DisabledControlsWhileProcessing` for long-running callbacks to avoid unintuitive queuing.

There is one callback queue per runspace where WinUIShell module is loaded, and callbacks in the queue are typically processed inside `$win.WaitForClosed()`. Please see [MultipleRunspaces.ps1](./examples/MultipleRunspaces.ps1) for an example of multi-runspace scenario.

### MainRunspaceSyncUI

Callbacks in `MainRunspaceSyncUI` mode run in the main runspace just like those with `MainRunspaceAsyncUI`, but they block the UI thread on the server side until they complete. Because they block the UI thread, it is guaranteed that no other events are triggered while the callback is running (No need to set `DisabledControlsWhileProcessing`).

Some callbacks need to run in `MainRunspaceSyncUI` mode to work properly. `BeforeTextChanging` event callback of the `TextBox` requires this mode for example. See [TextBoxValidation.ps1](./examples/TextBoxValidation.ps1) as a use case.

### RunspacePoolAsyncUI

Callbacks in `RunspacePoolAsyncUI` mode are handled in parallel by multiple runspaces in the runspace pool. This mode is ideal for long-running callbacks that must keep other callbacks responsive during execution. You can specify the number of runspaces in the pool and the script block that defines the global variables and functions in the runspaces:

```powershell
Set-WUIRunspacePoolOption -RunspaceCount 5 -InitializationScript {
    param ($ScriptRoot)
    $globalVar = 'Global variable in the runspace.'
    function GlobalFunction() {
        'Global function in the runspace.'
    }
} -InitializationScriptArgumentList $PSScriptRoot
```

Note that the WinUIShell module is automatically loaded in each runspace.

Since the callbacks are executed in parallel, you should pass variables via `ArgumentList` and handle thread safety just as you would with `Start-ThreadJob`. See [CancelLongRunningEventCallback.ps1](./examples/CancelLongRunningEventCallback.ps1) as a basic example, and [MultipleProgressBars.ps1](./examples/MultipleProgressBars.ps1) as an example of multiple concurrent tasks.

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
