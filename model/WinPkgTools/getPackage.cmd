@echo off
setlocal

call :processArgs %*
if not "%ArgError%" == "" exit /b 1 

rem  Set environment variables
set WinPkgTools=%~dp0

rem  Remove trailing backslash
set WinPkgTools=%WinPkgTools:~0,-1%

set DownloadTool=%WinPkgTools%\Landis.Tools.DownloadFile.exe
set UnzipTool=%WinPkgTools%\unzip.exe

rem  If the directory where the package will be downloaded to doesn't exist,
rem  make it.
call :ensureDirExists "%PackageDir%"

rem  Download the package if it doesn't exist on the local filesystem.
if exist "%PackagePath%" (
  echo %PackagePath% already downloaded.
) else (
  echo Downloading %PackagePath% ...
  "%DownloadTool%" %PackageUrl% "%PackagePath%"
  if errorlevel 1 goto :eof
)

rem  Unzip the package file if the sentinel file or directory doesn't exist.
if exist "%UnpackedItem%" (
  echo %PackagePath% has already been unpacked.
) else (
  call :ensureDirExists "%DirForUnpacking%"
  echo Unpacking %PackagePath% ...
  rem  The closing quote is deliberately left off the argument for the -d
  rem  option below because of a bug in the unzip tool.  The bug includes
  rem  the closing quote in the paths of files being extracted.
  "%UnZipTool%" -u "%PackagePath%" -d "%DirForUnpacking%
)
goto :eof

rem -------------------------------------------------------------------------

:processArgs

set ArgError=

if "%~1" == "" (
  set ArgError=Missing PackageURL
  goto usageError
)
set PackageURL=%~1

if "%~2" == "" (
  set ArgError=Missing PackagePath
  goto usageError
)
set PackagePath=%~2
set PackageDir=%~dp2
set PackageName=%~nx2

if "%~3" == "" (
  set ArgError=Missing UnpackedItem
  goto usageError
)
set UnpackedItem=%~3

set DirForUnpacking=libs

goto :eof

rem -------------------------------------------------------------------------

:usageError

echo Error: %ArgError%
echo Usage: %~nx0 PackageURL PackagePath UnpackedItem

goto :eof

rem -------------------------------------------------------------------------

:ensureDirExists

if not exist "%~1" (
  echo Making directory %~1 ...
  mkdir "%~1"
)

goto :eof

rem -------------------------------------------------------------------------
