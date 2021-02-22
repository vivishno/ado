@echo off
REM This is a cmd wrapper for a PowerShell script since CDPX only runs cmd files
powershell -NoProfile -ExecutionPolicy Unrestricted -File "%~dp0%~n0.ps1"
if errorlevel 1 (
    exit /b %errorlevel%
)