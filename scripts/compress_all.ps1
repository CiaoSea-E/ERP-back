$rar = "C:\Program Files\WinRAR\rar.exe"
$allDir = Join-Path $PSScriptRoot "..\All"
$zipOut = Join-Path $allDir "All.zip"
$rarOut = Join-Path $allDir "All.rar"

if (Test-Path $zipOut) { Remove-Item $zipOut -Force }
if (Test-Path $rarOut) { Remove-Item $rarOut -Force }

Write-Host "Compressing All.zip..."
$items = (Get-ChildItem -Path $allDir -Exclude 'All.zip','All.rar').FullName
Compress-Archive -Path $items -DestinationPath $zipOut -Force
Write-Host "Zip done, size: $((Get-Item $zipOut).Length) bytes"

if (Test-Path $rar) {
    Write-Host "Compressing All.rar..."
    & $rar a -r -ep1 -x"*.zip" -x"*.rar" $rarOut (Join-Path $allDir "*")
    Write-Host "RAR done, size: $((Get-Item $rarOut).Length) bytes"
}
