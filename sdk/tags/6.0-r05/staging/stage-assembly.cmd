@if not "%BATCH_ECHO%" == "on" echo off
setlocal EnableDelayedExpansion

echo NOTE: The %~nx0 script is deprecated; please use the copy-to-build-dir.cmd script instead.

rem Stage an assembly (LANDIS-II extension or library) in a directory where
rem the LANDIS-II model can find it.

set SCRIPT_DIR=%~dp0
rem Strip trailing path separator
set SCRIPT_DIR=%SCRIPT_DIR:~0,-1%

set LOG=%SCRIPT_DIR%\log.txt
call :log Arguments: %*
call :log Working dir: %CD%

set TARGET_PATH=%~1

rem Write the TargetPath into a temporary file along side the target in its
rem output directory.  We use this directory because it shouldn't be under
rem version control so we don't pollute the project with a spurious file.
set TARGET_DIR=%~dp1
set FILE_WITH_LIST=%TARGET_DIR%staging-list.txt
echo %TARGET_PATH% > "%FILE_WITH_LIST%"

rem Call the copy-to-build-dir.cmd script with this temporary file, and a
rem "dummy" path to the OutputDir.  The dummy is just ".", so it'll pass
rem the directory tests in the script.  That script will compute the wrong
rem project directory, but that doesn't matter since it won't be used
rem because the temporary file only has one absolute path (and no relative
rem paths.
call "%SCRIPT_DIR%\copy-to-build-dir.cmd" "%FILE_WITH_LIST%" "."

goto :eof

rem  ------------------------------------------------------------------------

:log
echo %DATE% %TIME% -- (%~n0) %* >> "%LOG%"
exit /b