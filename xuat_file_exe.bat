@echo off
setlocal
echo ================================================================
echo       DANG XUAT BAN GOI THUC THI (RELEASE .EXE) ERP
echo ================================================================

set "ROOT_DIR=%~dp0"
set "OUTPUT_DIR=%ROOT_DIR%Goi_Cai_Dat_ERP"
set "ZIP_FILE=%ROOT_DIR%HeThong_ERP_Release.zip"

echo [1/3] Bien dich toan bo giai phap o che do Release...
dotnet msbuild "%ROOT_DIR%ERP.sln" /t:Build /p:Configuration=Release /p:GenerateResourceMSBuildArchitecture=CurrentArchitecture /p:GenerateResourceMSBuildRuntime=CurrentRuntime /v:minimal

if %ERRORLEVEL% NEQ 0 (
    echo [LOI] Bien dich Release that bai!
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo [2/3] Sao chep cac file .exe va thu vien (.dll) can thiet...
if exist "%OUTPUT_DIR%" rd /s /q "%OUTPUT_DIR%"
mkdir "%OUTPUT_DIR%"

set "SRC_DIR=%ROOT_DIR%ERP_BanHang\ERP_Khach\bin\Release"

copy /y "%SRC_DIR%\*.exe" "%OUTPUT_DIR%\" >nul
copy /y "%SRC_DIR%\*.config" "%OUTPUT_DIR%\" >nul
copy /y "%SRC_DIR%\*.dll" "%OUTPUT_DIR%\" >nul

if exist "%SRC_DIR%\Resources" (
    xcopy /e /i /y "%SRC_DIR%\Resources" "%OUTPUT_DIR%\Resources" >nul
)

:: Tao file huong dan su dung
(
echo ================================================================
echo           HUONG DAN SU DUNG HE THONG ERP
echo ================================================================
echo 1. Nhap dup chuot vao file: ERP_Khach.exe de khoi dong he thong.
echo 2. Dang nhap va chon phan he lam viec:
echo    - Ban hang ^(ERP_BanHang^)
echo    - Quan ly Kho ^(ERPKho1^)
echo    - Nhan su ^(ERP_NhanSu^)
echo    - Logistics / Nha cung cap ^(ERP_Logistics^)
echo    - Ke toan tai chinh ^(KE_TOAN_TAI_CHINH^)
echo.
echo 3. Tai khoan dang nhap mau:
echo    - Quan tri vien ^(tat ca 5 phan he^): admin / admin
echo    - Ke toan tai chinh: ptha / 123
echo    - Ban hang: pmduc / 123
echo    - Kho: lmhoang / 123456
echo    - Logistics: tllan / 123
echo    - Nhan su: ntmai / 123
echo.
echo * Luu y: Khong xoa cac file .dll va .config di kem trong thu muc.
echo * Yeu cau: Windows 10 hoac Windows 11.
) > "%OUTPUT_DIR%\HUONG_DAN_SU_DUNG.txt"

echo.
echo [3/3] Dong goi thanh file nen HeThong_ERP_Release.zip...
if exist "%ZIP_FILE%" del /f /q "%ZIP_FILE%"
powershell -NoProfile -Command "Compress-Archive -Path '%OUTPUT_DIR%\*' -DestinationPath '%ZIP_FILE%' -Force"

echo.
echo ================================================================
echo  XUAT BAN THANH CONG!
echo ================================================================
echo - Thu muc chay truc tiep: %OUTPUT_DIR%
echo - File zip gui cho nguoi khac: %ZIP_FILE%
echo.
echo Ban chi can gui file HeThong_ERP_Release.zip cho bat ky ai de su dung!
pause
