@if not "%BATCH_ECHO%" == "on" echo off
setlocal EnableDelayedExpansion

rem  Copy files listed in a text file (LANDIS-II extension and libraries) into
rem  a directory where the LANDIS-II model can find them.

call %~dp0\initialize-env-vars.cmd

set LOG=%SCRIPT_DIR%\log.txt
call :log Working dir: %CD%

rem  Create temporary file where staging output will be written
set STAGING_OUTPUT_TEMP=%STAGING_OUTPUT:.txt=_tmp.txt%
type nul > "%STAGING_OUTPUT_TEMP%"
if exist "%STAGING_OUTPUT%" del "%STAGING_OUTPUT%"
for %%A in ("%STAGING_OUTPUT%") do set STAGING_OUTPUT_NAME=%%~nxA

rem  If the script directory and the LANDIS_SDK environment variable don't
rem  match, then the task was created with a different copy of the SDK.
if not "%SCRIPT_DIR%" == "%LANDIS_SDK%\staging" (
  call :recordError The task was created with a different LANDIS_SDK: %SCRIPT_DIR:~,-8%
  rem Write the output file to the SDK's staging directory because that's
  rem where its copy-to-build-dir.cmd script will be looking for it.
  copy /y "%STAGING_OUTPUT_TEMP%" "%LANDIS_SDK%\staging\%STAGING_OUTPUT_NAME%"
  goto exitScript
)

if not exist "%STAGING_LIST%" (
  call :recordError Missing file: %STAGING_LIST%
  goto :exitScript
)

rem  Initially, overwriting of files in the build directory is enabled.
set OVERWRITE=enabled

for /f "tokens=* delims=| usebackq" %%A in ("%STAGING_LIST%") do (
  call :log Line read from list: %%A
  call :stageFile %%A
)

:exitScript
rem  Move the temporary output file into place, so if this script is running
rem  as a task, then this renaming will signal that the task has finished its
rem  work.
move "%STAGING_OUTPUT_TEMP%" "%STAGING_OUTPUT%"

if "%EXIT_CODE%" == "" set EXIT_CODE 0
exit /b %EXIT_CODE%

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
rem  Record an error message in the log and the output file.

:recordError
call :log %*
echo ERROR: %* >> "%STAGING_OUTPUT_TEMP%"
set EXIT_CODE=1
exit /b

rem  ------------------------------------------------------------------------

:log
echo %DATE% %TIME% -- (%~n0) %* >> "%LOG%"
exit /b