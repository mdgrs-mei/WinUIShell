
param(
    [bool]$IsForegroundRunspace = $true,
    [String]$ServerModuleRoot
)

$publicScripts = @(Get-ChildItem $PSScriptRoot/Public/*.ps1)
foreach ($private:script in $publicScripts) {
    . $script.FullName
}

$netVersion = 'net8.0'
$clientDll = "$PSScriptRoot/bin/$netVersion/WinUIShell.dll"
Import-Module $clientDll

$script:isMainRunspace = $false
if ($IsForegroundRunspace) {
    $script:isMainRunspace = [WinUIShell.Engine]::Get().AcquireMainRunspace()
}

$MyInvocation.MyCommand.ScriptBlock.Module.OnRemove = {
    [WinUIShell.Engine]::Get().TermRunspace()
    if ($script:isMainRunspace) {
        [WinUIShell.Engine]::Get().ReleaseMainRunspace()
    }
}

if ($script:isMainRunspace) {
    $serverTarget = 'net8.0-windows10.0.18362.0'
    $serverPath = "$PSScriptRoot/bin/$serverTarget/WinUIShell.Server.exe"
}

$modulePath = $MyInvocation.MyCommand.Path
$useTimerEvent = $IsForegroundRunspace
if ($script:isMainRunspace) {
    [WinUIShell.Engine]::Get().InitMainRunspace($serverPath, $host, $modulePath, $useTimerEvent)
} else {
    [WinUIShell.Engine]::Get().InitSubRunspace($useTimerEvent)
}
