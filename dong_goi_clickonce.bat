@echo off
echo Dang dong goi ClickOnce cho tat ca cac phan he...
powershell -ExecutionPolicy Bypass -File "%~dp0build_all_clickonce.ps1"
pause
