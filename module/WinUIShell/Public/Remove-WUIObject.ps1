<#
.SYNOPSIS
Releases a WinUIShell object or all WinUIShell objects.

.DESCRIPTION
Due to the RPC architecture, objects that are managed by WinUIShell module are not freed automatically even if they are no longer used.
This function explicitly releases those WinUIShell objects. Use this when repeatedly creating and removing objects to prevent memory leaks.

.PARAMETER InputObject
The WinUIShell object to release. This parameter accepts objects from the pipeline.

.PARAMETER All
Releases all WinUIShell objects.

.INPUTS
WinUIShell.IWinUIShellObject

.OUTPUTS
None.

.EXAMPLE
$text = [TextBlock]::new()
Remove-WUIObject $text

.EXAMPLE
$text1 = [TextBlock]::new()
$text2 = [TextBlock]::new()
$text1, $text2 | Remove-WUIObject

.EXAMPLE
Remove-WUIObject -All
#>
function Remove-WUIObject {
    [CmdletBinding(DefaultParameterSetName = 'Object')]
    param (
        [Parameter(ParameterSetName = 'Object', Mandatory = $true, Position = 0, ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [WinUIShell.IWinUIShellObject]$InputObject,

        [Parameter(ParameterSetName = 'All', Mandatory = $true, ValueFromPipelineByPropertyName = $true)]
        [Switch]$All
    )

    process {
        if ($All) {
            [WinUIShell.Engine]::Get().RemoveAllWinUIShellObjects()
        } else {
            [WinUIShell.Engine]::Get().RemoveWinUIShellObject($InputObject)
        }
    }
}
