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

set LANDIS_SCRIPT=%SCRIPT_DIR%\landis-%LANDIS_VERSION%.cmd
if exist "%LANDIS_SCRIPT%" goto runModel
echo Error: The environment variable LANDIS_VERSION = %LANDIS_VERSION% but
echo        that version of LANDIS-II is not installed.
exit /b 1

::-----------------------------------------------------------------------------

:getNewest

rem Loop through installed landis-#.#.cmd scripts in numerical order
for %%I in ("%SCRIPT_DIR%\landis-?.?.cmd") do (
  set LANDIS_SCRIPT=%%I
)

:runModel
"%LANDIS_SCRIPT%" %*
