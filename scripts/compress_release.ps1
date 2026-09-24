$src = "D:\html\erp\ERP_Repo\Goi_Cai_Dat_ERP"
$out = "D:\html\erp\ERP_Repo\HeThong_ERP_Release.zip"

if (Test-Path $out) {
    Remove-Item $out -Force
}

Write-Host "Compressing HeThong_ERP_Release.zip..."
Compress-Archive -Path (Get-ChildItem -Path $src).FullName -DestinationPath $out -Force
Write-Host "Done! File size: $((Get-Item $out).Length) bytes"
