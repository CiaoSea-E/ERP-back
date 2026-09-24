@echo off
setlocal
title Khoi Dong He Thong ERP
chcp 65001 >nul

echo ================================================================
echo       HE THONG QUAN TRI DOANH NGHIEP TOAN DIEN - ACECOOK ERP
echo ================================================================
echo.

set "ROOT_DIR=%~dp0"
if exist "%ROOT_DIR%ERP_Code\ERP.sln" (
    set "CODE_DIR=%ROOT_DIR%ERP_Code\"
) else (
    set "CODE_DIR=%ROOT_DIR%"
)

set "EXE_PATH=%CODE_DIR%ERP_BanHang\ERP_Khach\bin\Debug\ERP_Khach.exe"

:: Dong cac tien trinh ERP cu dang chay ngam neu co
powershell -Command "Stop-Process -Name 'ERP_Khach','ERP_BanHang','ERP_NhanSu','ERPKho','ERP_Logistics','KE_TOAN_TAI_CHINH' -Force -ErrorAction SilentlyContinue"

:: Neu chua co file .exe thi tien hanh bien dich
if not exist "%EXE_PATH%" (
    echo [1/2] Dang khoi phuc thu vien NuGet va bien dich giai phap...
    if exist "%CODE_DIR%nuget.exe" (
        "%CODE_DIR%nuget.exe" restore "%CODE_DIR%ERP.sln" >nul 2>&1
    )
    dotnet msbuild "%CODE_DIR%ERP.sln" /t:Build /p:Configuration=Debug /p:GenerateResourceMSBuildArchitecture=CurrentArchitecture /p:GenerateResourceMSBuildRuntime=CurrentRuntime /v:minimal
    if %ERRORLEVEL% NEQ 0 (
        echo.
        echo [LOI] Bien dich that bai! Vui long kiem tra loi phia tren.
        pause
        exit /b %ERRORLEVEL%
    )
)

echo [2/2] Dang khoi chay Master Portal (ERP_Khach)...
echo.
echo ----------------------------------------------------------------
echo  Tai khoan dang nhap mau:
echo   - Quan tri vien (Tat ca 5 phan he): admin / admin
echo   - Ke toan tai chinh: ptha / 123
echo   - Ban hang: pmduc / 123
echo   - Kho: lmhoang / 123456
echo   - Logistics: tllan / 123
echo   - Nhan su: ntmai / 123
echo ----------------------------------------------------------------
echo.

start "" "%EXE_PATH%"
echo Da khoi dong ung dung thanh cong!
ping 127.0.0.1 -n 3 >nul
exit /b 0
