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

set SCRIPT_DIR=%~dp0
rem Strip trailing path separator
set SCRIPT_DIR=%SCRIPT_DIR:~0,-1%
set SCRIPT_NAME=%~n0

set TASK_NAME=LANDIS-II\StageFiles
schtasks /query /tn %TASK_NAME% 1>"%SCRIPT_DIR%\%SCRIPT_NAME%_log.txt" 2>&1
if %ERRORLEVEL% == 0 (
    echo ERROR: The task "%TASK_NAME%" has already been created.
	echo If you want to replace it, first delete it in the Task Scheduler,
	echo and then run this script again.
	goto error
)

set TASK_TEMPLATE=%SCRIPT_DIR%\task-template.xml
set TASK_FILENAME=stage-files.xml
set TASK_XML=%SCRIPT_DIR%\%TASK_FILENAME%
echo Creating task definition in "%SCRIPT_DIR%\" ...
copy /y "%TASK_TEMPLATE%" "%TASK_XML%"
call :updateTaskXML "{USER_ID}" "%USERDOMAIN%\%USERNAME%"
call :updateTaskXML "{LANDIS_SDK}" "%LANDIS_SDK%"
echo Task definition ready in "%TASK_FILENAME%"

echo Creating task with Task Scheduler...
schtasks /create /tn %TASK_NAME% /xml "%TASK_XML%"
if errorlevel 1 (
  echo This script must be "Run as administrator".
  goto error
)
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
rem  Replace a placeholder string in the task's XML file with actual value.
rem
rem  Parameters: %1 = placeholder string
rem              %2 = text to replace placeholder with

:updateTaskXML
"%SCRIPT_DIR%\Find_And_Replace" "%TASK_XML%" "%~1" "%~2"
echo Updated definition: "%~1" --^>^ "%~2"
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
