$rar = "C:\Program Files\WinRAR\rar.exe"
$allDir = "d:\html\erp\All\All"
$zipOut = "d:\html\erp\All\All.zip"
$rarOut = "d:\html\erp\All\All.rar"

if (Test-Path $zipOut) { Remove-Item $zipOut -Force }
Write-Host "Compressing All.zip..."
Compress-Archive -Path (Get-ChildItem -Path $allDir).FullName -DestinationPath $zipOut -Force
Write-Host "Zip done, size: $((Get-Item $zipOut).Length) bytes"

if (Test-Path $rarOut) { Remove-Item $rarOut -Force }
Write-Host "Compressing All.rar..."
& $rar a -r -ep1 $rarOut (Join-Path $allDir "*")
Write-Host "RAR done, size: $((Get-Item $rarOut).Length) bytes"
