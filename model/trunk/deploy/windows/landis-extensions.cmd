@echo off
setlocal

set SCRIPT_DIR=%~dp0
if "%SCRIPT_DIR:~-2%" == ":\" (
  rem In root directory, so don't strip trailing backslash
) else if "%SCRIPT_DIR:~-1%" == "\" (
  rem Strip trailing backslash
  set SCRIPT_DIR=%SCRIPT_DIR:~0,-1%
)

:: Has user specified a default version?
if "%LANDIS_VERSION%" == "" goto :getNewest

set LANDIS_EXT_TOOL=%SCRIPT_DIR%\landis-%LANDIS_VERSION%-extensions.cmd
if exist "%LANDIS_EXT_TOOL%" goto runTool
echo Error: The environment variable LANDIS_VERSION = %LANDIS_VERSION% but
echo        that version of LANDIS-II is not installed.
exit /b 1

::-----------------------------------------------------------------------------

:getNewest

rem Loop through installed landis-#.#-extensions.cmd scripts in numerical order
for %%I in ("%SCRIPT_DIR%\landis-?.?-extensions.cmd") do (
  set LANDIS_EXT_TOOL=%%I
)

:runTool
"%LANDIS_EXT_TOOL%" %*
