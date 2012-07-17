@echo off
setlocal

:: Usage: {this script} {version-release}
:: 
:: where {version-release} is the version and release in short form, for
:: example, "5.1" or "5.1-rc1"

if "%1" == "" goto :noRelease
if "%1" == "5.0" (
    set LANDIS_SCRIPT=Landis-II-5.0.cmd
) else (
    set LANDIS_SCRIPT=landis-%1.cmd
)
if exist "%~dp0\%LANDIS_SCRIPT%" del "%~dp0\%LANDIS_SCRIPT%"

: Now run the version tool to determine the newest release still installed
"%~dp0\Landis.Versions.exe" store-newest

goto :EOF

::-----------------------------------------------------------------------------

:noRelease

echo Error: No version and release specified
echo Usage: %~n0 {release}
exit /b 1
