@echo off
dir .learningtransport >nul 2>&1
if not errorlevel 1 rmdir /s /q .learningtransport
