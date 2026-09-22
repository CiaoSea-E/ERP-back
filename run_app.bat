@echo off
set "DOTNET_ROOT=C:\Program Files\dotnet"
echo ================================================================
echo   DANG KHOI DONG HE THONG ERP TONG THE (.NET FRAMEWORK 4.8)
echo   Gom: ERP_Khach (Portal), ERP_BanHang, HR_Management (Nhan Su)
echo ================================================================

powershell -Command "Stop-Process -Name 'ERP_Khach','ERP_BanHang','HR_Management' -Force -ErrorAction SilentlyContinue"

echo [1/3] Kiem tra va khoi phuc thu vien NuGet...
if exist "%~dp0nuget.exe" (
    "%~dp0nuget.exe" restore "%~dp0ERP.sln" >nul 2>&1
)

echo [2/3] Dang bien dich toan bo giai phap ERP.sln...
dotnet msbuild "%~dp0ERP.sln" /t:Build /p:Configuration=Debug

if %ERRORLEVEL% NEQ 0 (
    echo [LOI] Bien dich that bai! Vui long kiem tra lai thong bao tren.
    pause
    exit /b %ERRORLEVEL%
)

echo [3/3] Khoi dong Cong He Thong ERP_Khach...
start "" "%~dp0ERP_BanHang\ERP_Khach\bin\Debug\ERP_Khach.exe"
echo Hoan tat khoi dong!
