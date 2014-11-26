@if not "%BATCH_ECHO%" == "on" echo off
setlocal EnableDelayedExpansion

rem Stage an assembly (LANDIS-II extension or library) in a directory where
rem the LANDIS-II model can find it.

set STAGING_DIR=C:\Program Files\LANDIS-II\v6\bin\build
set SCRIPT_DIR=%~dp0
rem Strip trailing path separator
set SCRIPT_DIR=%SCRIPT_DIR:~0,-1%
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
  call :checkWriteAccess
  if errorlevel 1 exit /b %ERRORLEVEL%
  if "%BATCH_DEBUG%" == "yes" echo WRITE_ACCESS: %WRITE_ACCESS%
  if "%WRITE_ACCESS%" == "yes" (
    call :stageAssembly %*
  ) else (
    call :runTask %*
  )
)
goto :eof

rem  ------------------------------------------------------------------------
rem  Check if we have write access to the staging directory.
rem
rem  Return: ERRORLEVEL = 1 if staging directory is not a directory
rem          ERRORLEVEL = 0 and WRITE_ACCESS = "yes" or "no"

:checkWriteAccess
call :ensureDirExists
if "%BATCH_DEBUG%" == "yes" echo DIR_STATUS: %DIR_STATUS%
if "%DIR_STATUS%" == "not dir" (
  echo ERROR: "%STAGING_DIR%" is not a directory
  exit /b 1
)
if "%DIR_STATUS%" == "created" (
  set WRITE_ACCESS=yes
  exit /b
)
if "%DIR_STATUS%" == "not created" (
  set WRITE_ACCESS=no
  exit /b
)
rem The directory exists, so check if we can write to it
copy /y "%SCRIPT_DIR%\write-access.txt" "%STAGING_DIR%" 1>nul 2>&1
if errorlevel 1 (
  set WRITE_ACCESS=no
) else (
  set WRITE_ACCESS=yes
)
exit /b

rem  ------------------------------------------------------------------------
rem  Ensure the staging directory exists.  If the directory does not exist,
rem  then it's created.
rem
rem  Return: DIR_STATUS = "exists"
rem                     = "not dir" if it exists but is not a directory
rem                     = "created" if it didn't exist but was created
rem                     = "not created" if it could not be created

:ensureDirExists
if exist "%STAGING_DIR%\" (
  set DIR_STATUS=exists
  exit /b
)
if exist "%STAGING_DIR%" (
  set DIR_STATUS=not dir
  exit /b
)
mkdir "%STAGING_DIR%" >nul 2>nul
if errorlevel 1 (
  set DIR_STATUS=not created
) else (
  set DIR_STATUS=created
)
exit /b

rem  ------------------------------------------------------------------------
rem  Stage the assembly by copying it to the staging directory.

:stageAssembly
xcopy /q /y /d "%~1" "%STAGING_DIR%"
if errorlevel 1 call :log Errorlevel = %ERRORLEVEL% after xcopy with "%~1"
exit /b

rem  ------------------------------------------------------------------------
rem  Run a scheduler task which executes this script with the highest
rem  privileges.

:runTask
echo Using a task with elevated privileges to stage the assembly...
set TASK_NAME=LANDIS-II\StageAssembly
schtasks /query /tn %TASK_NAME% 1>nul 2>&1
if errorlevel 1 (
    echo ERROR: The task "%TASK_NAME%" has not been created.
	echo        To create the task, run this script as administrator:
	echo          "%SCRIPT_DIR%\create-task.cmd"
	exit /b 1
)
rem Write the full path to the assembly
echo "%~dpnx1" > "%ARGUMENTS_FILE%"
schtasks /run /tn LANDIS-II\StageAssembly
exit /b

rem  ------------------------------------------------------------------------

:log
echo %DATE% %TIME% -- %* >> "%LOG%"
exit /b