function Remove-WUIObject {
    [CmdletBinding(DefaultParameterSetName = 'Object')]
    param (
        [Parameter(ParameterSetName = 'Object', Mandatory = $true, ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
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
