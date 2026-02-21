$examples = Get-ChildItem $PSScriptRoot/*.ps1 -Exclude $MyInvocation.MyCommand.Name
foreach ($example in $examples) {
    & $example.FullName
}
