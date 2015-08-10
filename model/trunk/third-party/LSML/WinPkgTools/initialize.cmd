@echo off

rem  Set environment variables for calling script to use
set WinPkgTools=%~dp0

rem  Remove trailing backslash
set WinPkgTools=%WinPkgTools:~0,-1%

set DownloadTool=%WinPkgTools%\Landis.Tools.DownloadFile.exe
set ChecksumTool=%WinPkgTools%\checksum.exe
set UnzipTool=%WinPkgTools%\unzip.exe

rem  If "vars" argument specified, then don't download missing tools
if "%~1" == "vars" goto :eof

setlocal

rem  The download tool is already present; check if the other 2 tools have
rem  already been downloaded.
set MissingTool=no
if not exist "%ChecksumTool%" set MissingTool=yes
if not exist "%UnzipTool%"    set MissingTool=yes

if "%MissingTool%" == "no" goto :eof


echo Initializing toolkit in %WinPkgTools%\ ...

set ScriptURL=$HeadURL: https://landis-spatial.googlecode.com/svn/tags/tools/WinPkg/1.6/clients/initialize.cmd $
rem  Remove leading and trailing delimiters of svn keyword from the URL
set ScriptURL=%ScriptURL:$HeadURL: =%
set ScriptURL=%ScriptURL: $=%

set ToolkitRootURL=%ScriptURL:/clients/initialize.cmd=%
set DownloadURL=%ToolkitRootURL%/download

call :getFile checksum.exe
call :getFile checksum-LICENSE.txt
call :getFile unzip.exe
call :getFile Info-ZIP-LICENSE.txt

echo Toolkit initialized
goto :eof

rem  -------------------------------------------------------------------------

:getFile

set FileName=%1
if exist %FileName% (
  echo %FileName% already downloaded
  goto :eof
)
echo Downloading %FileName% ...
"%DownloadTool%" %DownloadURL%/%FileName% "%WinPkgTools%\%FileName%"
goto :eof

rem  -------------------------------------------------------------------------
