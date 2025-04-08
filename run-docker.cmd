@echo off

powershell -ExecutionPolicy ByPass -NoProfile -file ./%~n0.ps1
exit /b %ERRORLEVEL%