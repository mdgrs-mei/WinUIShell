<#
.SYNOPSIS
Sets options for the runspace pool that processes event callbacks whose RunspaceMode is RunspacePoolAsyncUI.

.DESCRIPTION
Sets options for the runspace pool that processes event callbacks whose RunspaceMode is RunspacePoolAsyncUI.
It's better to call this function at the beginning of a script as it recreates the runspace pool.

.PARAMETER RunspaceCount
The number of runspaces in the Runspace pool.

.PARAMETER InitializationScript
The ScriptBlock that is executed at the start of the runspace. This is typically used to initialize variables and functions in the runspace pool.
There's no need to manually load WinUIShell module as it is automatically loaded at runspace initialization.

.PARAMETER InitializationScriptArgumentList
The argument list that is passed to the InitializationScript.

.INPUTS
None.

.OUTPUTS
None.

.EXAMPLE
Set-WUIRunspacePoolOption -RunspaceCount 3 -InitializationScript {
    $globalVar = 'Hello'
    function Hello {
        Write-Host $globalVar
    }
}

.EXAMPLE
Set-WUIRunspacePoolOption -RunspaceCount 4
#>
function Set-WUIRunspacePoolOption {
    [CmdletBinding()]
    param (
        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [UInt32]$RunspaceCount,

        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [ScriptBlock]$InitializationScript,

        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [Object[]]$InitializationScriptArgumentList
    )

    process {
        [WinUIShell.Engine]::Get().SetCommandThreadPoolOption($RunspaceCount, $InitializationScript, $InitializationScriptArgumentList)
    }
}
