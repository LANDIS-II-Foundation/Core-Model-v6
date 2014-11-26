@if not "%BATCH_ECHO%" == "on" echo off
setlocal EnableDelayedExpansion

rem Stage an assembly (LANDIS-II extension or library) in a directory where
rem the LANDIS-II model can find it.

call %~dp0\initialize-env-vars.cmd

set ARGUMENTS_FILE=%SCRIPT_DIR%\%~n0_args.txt
set LOG=%SCRIPT_DIR%\log.txt
call :log Arguments: %*
call :log Working dir: %CD%

if "%~1" == "" (
  rem The script was started by a scheduled task with elevated privileges.
  rem The script's arguments were written to a file before the task was run
  rem (see below).
  for /f "tokens=* delims=| usebackq" %%A in ("%ARGUMENTS_FILE%") do (
    call :log Arguments read in: %%A
    call :stageAssembly %%A
  )
) else (
  set ASSEMBLY_NAME=%~nx1
  echo Staging !ASSEMBLY_NAME! to "%STAGING_DIR%\"...
    call :stageAssembly %*
)
goto :eof

rem  ------------------------------------------------------------------------
rem  Stage the assembly by copying it to the staging directory.

:stageAssembly
xcopy /q /y /d "%~1" "%STAGING_DIR%"
if errorlevel 1 call :log Errorlevel = %ERRORLEVEL% after xcopy with "%~1"
exit /b

rem  ------------------------------------------------------------------------

:log
echo %DATE% %TIME% -- %* >> "%LOG%"
exit /b