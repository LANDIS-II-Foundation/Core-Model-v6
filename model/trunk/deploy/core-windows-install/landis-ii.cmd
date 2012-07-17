@echo off
setlocal

:: Has user specified a default version?
if "%LANDIS_VERSION%"=="" goto useNewest
"%~dp0\Landis.Versions.exe" check-env-var LANDIS_VERSION
if errorlevel 1 exit /b %errorlevel%
goto runModel

::-----------------------------------------------------------------------------

:useNewest
for /f "usebackq" %%v in ("%~dp0\landis-newest.txt") do set LANDIS_VERSION=%%v

:runModel
if "%LANDIS_VERSION%" == "5.0" (
    set LANDIS_SCRIPT=Landis-II-5.0.cmd
) else (
    set LANDIS_SCRIPT=landis-%LANDIS_VERSION%.cmd
)
"%~dp0\%LANDIS_SCRIPT%" %*
