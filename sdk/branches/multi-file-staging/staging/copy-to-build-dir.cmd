@if not "%BATCH_ECHO%" == "on" echo off
setlocal EnableDelayedExpansion

rem  Copy files listed in a text file (LANDIS-II extension and libraries) into
rem  a directory where the LANDIS-II model can find them.

set BUILD_DIR=C:\Program Files\LANDIS-II\v6\bin\build
set SCRIPT_DIR=%~dp0
set SCRIPT_DIR=%SCRIPT_DIR:~,-1%

call :processArgs %*
if errorlevel 1 exit /b %ERRORLEVEL%

echo Staging files to "%BUILD_DIR%\"...

rem  Read the list of file paths and write them as full paths into another
rem  file that the supporting stage-files.cmd script will read.
set LIST_WITH_FULL_PATHS=%SCRIPT_DIR%\file_list.txt
type nul > "%LIST_WITH_FULL_PATHS%"
for /f "tokens=* delims=| usebackq" %%A in ("%FILE_WITH_LIST%") do (
  call :writeFullPath "%%A" "%LIST_WITH_FULL_PATHS%"
)

call :checkWriteAccess
if errorlevel 1 exit /b %ERRORLEVEL%
if "%BATCH_DEBUG%" == "yes" echo WRITE_ACCESS: %WRITE_ACCESS%

rem  If we have write access to the build directory, then just run the staging
rem  script directly.  If no write access, then run the scheduler task that
rem  runs the script.
set STAGING_SCRIPT=%SCRIPT_DIR%\stage-files.cmd
set STAGING_OUTPUT=%STAGING_SCRIPT:.cmd=_out.txt%
if "%WRITE_ACCESS%" == "yes" (
    call "%STAGING_SCRIPT%"
  ) else (
    call :runTask
  )
)
type "%STAGING_OUTPUT%
goto :eof

rem  ------------------------------------------------------------------------
rem  Process the command line arguments.  Expected arguments are:
rem
rem    %1 : full path to the file with the list of files to copy (assigned to
rem         the FILE_WITH_LIST environment variable)
rem    %2 : relative path to the current configuration output directory from
rem         the project directory (e.g., "bin\Debug\"; assigned to the
rem         OUT_DIR_REL environment variable)
rem
rem  Additional environment variables set by this subroutine:
rem
rem    PROJECT_DIR : full path to the directory where the FILE_WITH_LIST is
rem    OUT_DIR_ABS : full path to current configuration output directory

:processArgs
set FILE_WITH_LIST=%1
if "%FILE_WITH_LIST%" == "" (
  call :printError "Missing path to the file with the list of files to copy"
  exit /b 1
)
if not exist "%FILE_WITH_LIST%" (
  call :printError "Missing file: %FILE_WITH_LIST%"
  exit /b 1
)

rem  Project directory is where the file list is located
set PROJECT_DIR=%~dp1
set PROJECT_DIR=%PROJECT_DIR:~,-1%

if "%~2" == "" (
  call :printError "Missing OutDir argument"
  exit /b 1
)
rem  OutDir is relative path from the project directory
set OUT_DIR_REL=%~2
set OUT_DIR_REL=%OUT_DIR_REL:~,-1%
set OUT_DIR_ABS=%PROJECT_DIR%\%OUT_DIR_REL%
if not exist "%OUT_DIR_ABS%" (
  call :printError "Missing directory: %OUT_DIR_ABS%"
  exit /b 1
)
exit /b 0

rem  ------------------------------------------------------------------------
rem  Determine the full path for a file, and write that path into a text file
rem  to be read by another script.

:writeFullPath
set FILE_PATH=%~1
set OUT_FILE=%~2
if "%FILE_PATH:~0,2%" == "--" (
  rem  It's a command line option, so write it as is
  echo %FILE_PATH% >> "%OUT_FILE%"
  exit /b
)
if "%FILE_PATH:~0,8%" == "{OutDir}" (
  echo %OUT_DIR_ABS%\%FILE_PATH:~9% >> "%OUT_FILE%"
  exit /b
)
if "%FILE_PATH:~0,1%" == "\" (
  rem  Absolute path on the drive with the project directory
  echo %PROJECT_DIR:~0,2%%FILE_PATH% >> "%OUT_FILE%"
  exit /b
)
if "%FILE_PATH:~1,1%" == ":" (
  rem  Absolute path on another drive
  echo %FILE_PATH% >> "%OUT_FILE%"
  exit /b
)
rem  Argument is the file's relative path from the project directory
echo %PROJECT_DIR%\%FILE_PATH% >> "%OUT_FILE%"
exit /b

rem  ------------------------------------------------------------------------
rem  Check if we have write access to the staging directory.
rem
rem  Return: ERRORLEVEL = 1 if build directory is not a directory
rem          ERRORLEVEL = 0 and WRITE_ACCESS = "yes" or "no"

:checkWriteAccess
call :ensureDirExists
if "%BATCH_DEBUG%" == "yes" echo DIR_STATUS: %DIR_STATUS%
if "%DIR_STATUS%" == "not dir" (
  echo ERROR: "%BUILD_DIR%" is not a directory
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
copy /y "%SCRIPT_DIR%\write-access.txt" "%BUILD_DIR%" 1>nul 2>&1
if errorlevel 1 (
  set WRITE_ACCESS=no
) else (
  set WRITE_ACCESS=yes
)
exit /b

rem  ------------------------------------------------------------------------
rem  Ensure the build directory exists.  If the directory does not exist,
rem  then it's created.
rem
rem  Return: DIR_STATUS = "exists"
rem                     = "not dir" if it exists but is not a directory
rem                     = "created" if it didn't exist but was created
rem                     = "not created" if it could not be created

:ensureDirExists
if exist "%BUILD_DIR%\" (
  set DIR_STATUS=exists
  exit /b
)
if exist "%BUILD_DIR%" (
  set DIR_STATUS=not dir
  exit /b
)
mkdir "%BUILD_DIR%" >nul 2>nul
if errorlevel 1 (
  set DIR_STATUS=not created
) else (
  set DIR_STATUS=created
)
exit /b

rem  ------------------------------------------------------------------------
rem  Run a scheduler task which executes this script with the highest
rem  privileges.

:runTask
echo Using a task with elevated privileges to stage the files...
set TASK_NAME=LANDIS-II\StageFiles
schtasks /query /tn %TASK_NAME% 1>nul 2>&1
if errorlevel 1 (
    echo ERROR: The task "%TASK_NAME%" has not been created.
	echo        To create the task, run this script as administrator:
	echo          "%SCRIPT_DIR%\create-task.cmd"
	exit /b 1
)

if exist "%STAGING_OUTPUT%" del "%STAGING_OUTPUT%"
schtasks /run /tn LANDIS-II\StageFiles
call :waitUntilFileExists "%STAGING_OUTPUT%"

exit /b

rem  -------------------------------------------------------------------------
rem  Wait until a file is created.  This is used since the staging task runs
rem  asynchronously, and uses a global file for argument-passing.

:waitUntilFileExists
if not exist %1 (
  rem  Wait a second (based on http://stackoverflow.com/a/735294/1258514)
  ping 127.0.0.1 -n 2 -w 1000 > nul
  goto :waitUntilFileExists
)
goto :eof

rem  ------------------------------------------------------------------------

rem  Print an error message

:printError
echo %~nx0 error: %~1
exit /b