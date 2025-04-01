
$netVersion = 'net8.0'
$serverTarget = 'net8.0-windows10.0.18362.0'

$dll = "$PSScriptRoot/bin/$netVersion/WinUIShell.dll"
$server = "$PSScriptRoot/bin/$serverTarget/WinUIShell.Server.exe"

Import-Module $dll

[WinUIShell.Engine]::Start($server)

$MyInvocation.MyCommand.ScriptBlock.Module.OnRemove = {
    [WinUIShell.Engine]::Stop()
}
