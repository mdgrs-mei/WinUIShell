
param(
    [bool]$UseTimerEvent = $true
)

$netVersion = 'net8.0'
$serverTarget = 'net9.0-desktop'
#$serverTarget = 'net9.0-windows10.0.26100'
$serverRid = 'win-x64'

$dll = "$PSScriptRoot/bin/$netVersion/WinUIShell.dll"
$server = "$PSScriptRoot/bin/$serverTarget/$serverRid/WinUIShell.Server.exe"

Import-Module $dll

$modulePath = $MyInvocation.MyCommand.Path
$useUno = $serverTarget.Contains('desktop')
[WinUIShell.Engine]::Get().InitRunspace($server, $host, $modulePath, $useUno, $UseTimerEvent)

$MyInvocation.MyCommand.ScriptBlock.Module.OnRemove = {
    [WinUIShell.Engine]::Get().TermRunspace()
}

$publicScripts = @(Get-ChildItem $PSScriptRoot/Public/*.ps1)
foreach ($private:script in $publicScripts) {
    . $script.FullName
}
