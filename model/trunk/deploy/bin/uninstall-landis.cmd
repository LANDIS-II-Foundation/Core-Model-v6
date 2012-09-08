@echo off
setlocal

if "%~1" == "" goto :printHelp
for /f "delims=. tokens=1-2" %%X in ("%~1") do (
  set Major=%%X
  set Minor=%%Y
)
if "%Major" == "" (
  echo Error: Missing major version in "%~1"
  exit /b 1
)
if "%Minor" == "" (
  echo Error: Missing minor version in "%~1"
  exit /b 1
)
set MajorMinor=%Major%.%Minor%

rem  Run in the parent directory of this script
pushd %~dp0\..

set UninstallList=v%Major%\bin\%MajorMinor%\uninstall-list.txt
if not exist %UninstallList% (
  echo LANDIS-II %MajorMinor% is not installed
  goto :exitScript
)

echo Uninstalling LANDIS-II %MajorMinor%
for /f "delims=;" %%P in (%UninstallList%) do call :deletePath "%%P"
)

:exitScript

popd
exit /b 0

rem  --------------------------------------------------------------------------

:deletePath

set UnixPath=%~1
set WindowsPath=%UnixPath:/=\%

if not exist "%WindowsPath%" goto :eof
dir /ad "%WindowsPath%" >nul 2>&1
if errorlevel 1 (
  del "%WindowsPath%"
  echo Deleted %WindowsPath%
) else (
  rmdir /s /q "%WindowsPath%"
  echo Deleted %WindowsPath%\
)
goto :eof

rem  --------------------------------------------------------------------------

:printHelp

echo Usage: %~nx0 #.#
echo where #.# is the version of LANDIS-II to uninstall

:goto eof

rem  --------------------------------------------------------------------------
