@if not "%BATCH_ECHO%" == "on" echo off

rem Update the scheduled task that runs the stage-command script.  This script
rem is needed when the LANDIS_SDK environment variable has changed since the
rem task was created.  For example, a developer installed a newer release of
rem the SDK in a different directory, and updated the LANDIS_SDK variable to
rem point to the new directory.  This script must be "Run as administrator".

if not defined LANDIS_SDK (
   echo ERROR: The LANDIS_SDK environment variable must be set first
   goto error
)

call %~dp0\initialize-env-vars.cmd
set SCRIPT_NAME=%~n0

schtasks /query /tn %TASK_NAME% 1>"%SCRIPT_DIR%\%SCRIPT_NAME%_log.txt" 2>&1
if errorlevel 1 (
    echo ERROR: The task "%TASK_NAME%" has not been created.
	echo Please run the create-task.cmd script.
	goto error
)

set TASK_PROGRAM=%LANDIS_SDK%\staging\stage-files.vbs

echo Updating the "%TASK_NAME%" task in Task Scheduler...
schtasks /change /tn %TASK_NAME% /disable
if errorlevel 1 (
  echo This script must be "Run as administrator".
  goto error
)
schtasks /change /tn %TASK_NAME% /tr "%TASK_PROGRAM%" /enable
echo The task now runs this program: %TASK_PROGRAM%

call :pause
exit /b 0

rem  ------------------------------------------------------------------------

:error
call :pause
exit /b 1

rem  ------------------------------------------------------------------------
rem  Pause the script if it was launched by double-clicking in Windows
rem  Explorer.

:pause
call :runByDoubleClick %CMDCMDLINE%
if "%RUN_BY_DOUBLE_CLICK%" == "y" pause
exit /b

rem  ------------------------------------------------------------------------
rem  Was this script run by double-clicking it in Windows Explorer, or from
rem  within a command prompt?
rem
rem  Return: RUN_BY_DOUBLE_CLICK = y or n

:RunByDoubleClick
rem Based on this SO answer: http://stackoverflow.com/a/6797358/1258514
if /i "%2" == "/c" (
  set RUN_BY_DOUBLE_CLICK=y
) else (
  set RUN_BY_DOUBLE_CLICK=n
)
exit /b
