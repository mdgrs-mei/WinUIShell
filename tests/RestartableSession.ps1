#Requires -Modules RestartableSession

$root = Split-Path $PSScriptRoot -Parent
Enter-RSSession -OnStart {
    $root = $args[0]
    $build = "$root/Build.ps1"

    $server = Get-Process -Name WinUIShell.Server -ErrorAction SilentlyContinue
    if ($server) {
        Write-Host 'Waiting for server close...'
        $server.WaitForExit()
    }

    & $build Debug
    Import-Module "$root/module/WinUIShell"

    function Restart {
        Restart-RSSession
    }
    function Pester {
        & "$root/tests/RunPesterTests.ps1"
    }
} -OnStartArgumentList $root -ShowProcessId
