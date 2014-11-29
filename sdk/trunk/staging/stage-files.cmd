@if not "%BATCH_ECHO%" == "on" echo off
setlocal EnableDelayedExpansion

rem  Copy files listed in a text file (LANDIS-II extension and libraries) into
rem  a directory where the LANDIS-II model can find them.

call %~dp0\initialize-env-vars.cmd

set LOG=%SCRIPT_DIR%\log.txt
call :log Working dir: %CD%

if not exist "%STAGING_LIST%" (
  call :log Missing file: %STAGING_LIST%
  exit /b 1
)

rem  Create temporary file where staging output will be written
set STAGING_OUTPUT_TEMP=%STAGING_OUTPUT:.txt=_tmp.txt%
type nul > "%STAGING_OUTPUT_TEMP%"
if exist "%STAGING_OUTPUT%" del "%STAGING_OUTPUT%"

rem  Initially, overwriting of files in the build directory is enabled.
set OVERWRITE=enabled

for /f "tokens=* delims=| usebackq" %%A in ("%STAGING_LIST%") do (
  call :log Line read from list: %%A
  call :stageFile %%A
)

rem  Move the temporary output file into place, so if this script is running
rem  as a task, then this renaming will signal that the task has finished its
rem  work.
move "%STAGING_OUTPUT_TEMP%" "%STAGING_OUTPUT%"

goto :eof

rem  ------------------------------------------------------------------------
rem  Stage a file by copying it to the build directory.

:stageFile
set SOURCE_PATH=%~1
if "%SOURCE_PATH:~0,2%" == "--" (
  call :processOption %1
  exit /b
)
if not exist "%SOURCE_PATH%" (
  echo MISSING: %SOURCE_PATH% >> "%STAGING_OUTPUT_TEMP%"
  exit /b
)
set TARGET_PATH=%BUILD_DIR%\%~nx1
if "%OVERWRITE%" == "disabled" (
  if exist "%TARGET_PATH%" (
    echo Skipped: %SOURCE_PATH% >> "%STAGING_OUTPUT_TEMP%"
    exit /b
  )
)
if exist "%TARGET_PATH%" (
  set ACTION=Updated
) else (
  set ACTION=Created
)
xcopy /q /y /d "%SOURCE_PATH%" "%TARGET_PATH%"
if errorlevel 1 call :log Errorlevel = %ERRORLEVEL% after xcopy with "%~1"
echo %ACTION%: %TARGET_PATH% >> "%STAGING_OUTPUT_TEMP%"
exit /b

rem  ------------------------------------------------------------------------
rem  Process a command line option.

:processOption
if "%~1" == "--no-overwrite" (
  set OVERWRITE=disabled
) else (
  call :log Unknown option: %1
)
exit /b

rem  ------------------------------------------------------------------------

:log
echo %DATE% %TIME% -- (%~n0) %* >> "%LOG%"
exit /b