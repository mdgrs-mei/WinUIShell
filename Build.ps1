param (
    [ValidateSet('Debug', 'Release')]
    [String]$Configuration = 'Debug',

    [Switch]$ExportApi
)

$originalProgressPreference = $ProgressPreference
$ProgressPreference = 'SilentlyContinue'

$netVersion = 'net8.0'
$serverTarget = 'net8.0-windows10.0.18362.0'
$copyExtensions = @('.dll', '.pdb')
$src = "$PSScriptRoot/src"
$apiSrc = "$src/WinUIShell.ApiExporter"
$coreSrc = "$src/WinUIShell"
$depSrc = "$src/WinUIShell.Common"
$serverSrc = "$src/WinUIShell.Server"

$apiPublish = [System.IO.Path]::GetFullPath("$apiSrc/bin/$Configuration/$serverTarget/publish/")
$corePublish = [System.IO.Path]::GetFullPath("$coreSrc/bin/$Configuration/$netVersion/publish/")
$depPublish = [System.IO.Path]::GetFullPath("$depSrc/bin/$Configuration/$netVersion/publish/")
$serverPublish = [System.IO.Path]::GetFullPath("$serverSrc/bin/$Configuration/$serverTarget/publish/")

$apiXml = "$apiSrc/Api.xml"
$apiExporter = "$apiPublish/WinUIShell.ApiExporter.exe"

$outDir = "$PSScriptRoot/module/WinUIShell/bin/$netVersion"
$outDeps = "$outDir/Dependencies"
$outServer = "$PSScriptRoot/module/WinUIShell/bin/$serverTarget"

function CopyFolderItems($FolderPath, $Destination) {
    if (Test-Path $Destination) {
        Copy-Item -Path "$FolderPath/*" -Destination $Destination -Recurse
    } else {
        Copy-Item -Path $FolderPath -Destination $Destination -Recurse
    }
}

Push-Location $src
$dotnetExeVersion = dotnet --version
Write-Host "dotnet.exe version: $dotnetExeVersion"
Pop-Location

Remove-Item -Path $outDir -Recurse -ErrorAction Ignore
Remove-Item -Path $outServer -Recurse -ErrorAction Ignore

if ($ExportApi) {
    Push-Location $apiSrc
    dotnet publish -c $Configuration -o $apiPublish
    Pop-Location

    Remove-Item -Path $apiXml -ErrorAction Ignore
    Start-Process -FilePath $apiExporter -ArgumentList $apiXml -Wait
}

Push-Location $depSrc
dotnet publish -c $Configuration -o $depPublish
Pop-Location

Push-Location $coreSrc
dotnet publish -c $Configuration -o $corePublish
Pop-Location

# Filter deps files.
Get-ChildItem -Path $depPublish -Recurse -File | Where-Object {
    $_.Extension -notin $copyExtensions
} | Remove-Item -Force

$deps = [System.Collections.Generic.List[string]]::new()
Get-ChildItem -Path $depPublish -Recurse -File | ForEach-Object {
    $deps.Add($_.FullName.Replace($depPublish, ''))
}

# Filter core dlls.
Get-ChildItem -Path $corePublish -Recurse -File | Where-Object {
    $path = $_.FullName.Replace($corePublish, '')
    ($_.Extension -notin $copyExtensions) -or ($deps.Contains($path))
} | Remove-Item -Force

# Remove empty folders of core dlls.
Get-ChildItem -Path $corePublish -Recurse -Directory | Where-Object {
    -not (Get-ChildItem -Path $_.FullName -Recurse -File)
} | Remove-Item -Force

# Output.
CopyFolderItems -FolderPath $corePublish -Destination $outDir
CopyFolderItems -FolderPath $depPublish -Destination $outDeps


# Build server.
Push-Location $serverSrc
dotnet publish -c $Configuration -o $serverPublish
Pop-Location

# Output.
CopyFolderItems -FolderPath $serverPublish -Destination $outServer

$ProgressPreference = $originalProgressPreference
