rem  Initialize the environment variables shared by multiple scripts

set BUILD_DIR=C:\Program Files\LANDIS-II\v6\bin\build
set SCRIPT_DIR=%~dp0
set SCRIPT_DIR=%SCRIPT_DIR:~,-1%
set STAGING_LIST=%SCRIPT_DIR%\file_list.txt
set STAGING_OUTPUT=%SCRIPT_DIR%\staging-output.txt
set TASK_NAME=LANDIS-II\StageFiles