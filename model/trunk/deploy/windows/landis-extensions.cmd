@echo off
setlocal

:: Has user specified a default version?
if "%LANDIS_VERSION%"=="" goto useNewest
"%~dp0\Landis.Versions.exe" check-env-var LANDIS_VERSION
if errorlevel 1 exit /b %errorlevel%
goto runTool

::-----------------------------------------------------------------------------

:useNewest
for /f "usebackq" %%v in ("%~dp0\landis-newest.txt") do set LANDIS_VERSION=%%v

:runTool
if "%LANDIS_VERSION%" == "5.0" goto notWork
set LANDIS_TOOL=landis-%LANDIS_VERSION%-extensions.cmd
"%~dp0\%LANDIS_TOOL%" %*
goto :EOF

::-----------------------------------------------------------------------------

:notWork
echo The extension administration tool doesn't work with LANDIS-II 5.0
