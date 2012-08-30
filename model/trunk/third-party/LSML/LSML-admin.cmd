@echo off

rem  Run this script in the directory where it's located
pushd %~dp0
setlocal

set Script=%~nx0
call :processArgs %*
if "%Action%" == "error" goto :exitScript
if "%Action%" == "clean" (
  call :clean
  goto :exitScript
)
if "%Action%" == "help" (
  call :usage
  goto :exitScript
)

rem  Read LSML version # and the SHA1 checksum for that version
for /f "tokens=1,2" %%i in (version.txt) do (
  set LibraryVer=%%i
  set LibrarySHA1=%%j
)

rem  Set environment variables about specific library version
set LibraryFileName=LSML-%LibraryVer%.zip
set LibraryURL=http://landis-spatial.googlecode.com/files/%LibraryFileName%
set DownloadDir=download
set LibraryPackage=%DownloadDir%\%LibraryFileName%

if "%Action%" == "distclean" (
  call :distclean
  goto :exitScript
)

rem  Do "get" action -- download the specific library version

set FileInPkg=Landis.SpatialModeling.dll

call WinPkgTools\getPackage.cmd %LibraryUrl% %LibraryPackage% %LibrarySHA1% %FileInPkg%

rem  Get the GDAL libraries and C# bindings
call :GDALadmin get


:exitScript

popd

set ExitCode=0
if "%Action%" == "error" (
  set ExitCode=1
) else (
  set ExitCode=0
)
exit /b %ExitCode%

rem  ------------------------------------------------------------------------

:processArgs

set Action=
if "%~1" == "get"       set Action=get
if "%~1" == "clean"     set Action=clean
if "%~1" == "distclean" set Action=distclean
if "%~1" == "help"      set Action=help
if "%~1" == ""          set Action=help

if "%Action%" == "" (
  call :error unknown action "%~1"
  goto :eof
)
if not "%~3" == "" (
  call :error extra arguments after "%~1" action: %2 ...
  goto :eof
)
if not "%~2" == "" (
  call :error extra argument after "%~1" action: %2
  goto :eof
)

goto :eof

rem  ------------------------------------------------------------------------

:error

echo Error: %*
call :usage
set Action=error
goto :eof

rem  ------------------------------------------------------------------------

:usage

echo Usage: %Script% [ACTION]
echo where ACTION is:
echo   get       -- download and unpack LSML and GDAL
echo   clean     -- remove all unpacked files
echo   distclean -- Same as "clean" action, plus remove all downloaded files
echo   help      -- display this message (default)

goto :eof

rem  ------------------------------------------------------------------------

:clean

rem  Delete all the unpacked files

call :GDALadmin clean

for %%F in ("*.dll") do call :deleteFile "%%F"
call :deleteFile README.txt

goto :eof

rem  ------------------------------------------------------------------------

:distclean

call :GDALadmin distclean
call :deleteFile GDAL-version.txt
call :deleteFile GDAL-admin.cmd
call :deleteFile GDAL-admin.sh

call :clean
call :deleteFile %LibraryPackage%
if exist %DownloadDir% (
  rmdir /s /q %DownloadDir%
  echo Deleted %DownloadDir%\
)

rem  Delete the files downloaded for WinPkgTools
call WinPkgTools\clean.cmd

goto :eof

rem  ------------------------------------------------------------------------

:deleteFile

if exist "%~1" (
  del "%~1"
  echo Deleted %~1
)
goto :eof

rem  ------------------------------------------------------------------------

:GDALadmin

set GdalAdmin_VersionFile=GDAL-version.txt
set GdalAdmin_InstallDir=GDAL
call WinPkgTools\initialize.cmd

if exist GDAL-admin.cmd call GDAL-admin.cmd %1

goto :eof

rem  ------------------------------------------------------------------------
