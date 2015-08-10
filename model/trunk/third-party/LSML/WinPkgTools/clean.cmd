@echo off

echo Cleaning %~dp0 ...
pushd %~dp0

rem  Unset environment variables associated with the toolkit
set WinPkgTools=
set DownloadTool=
set ChecksumTool=
set UnzipTool=

rem  Delete the files that were downloaded
call :deleteFile checksum.exe
call :deleteFile checksum-LICENSE.txt
call :deleteFile unzip.exe
call :deleteFile Info-ZIP-LICENSE.txt

popd
goto :eof

rem  -------------------------------------------------------------------------

:deleteFile

if exist %1 (
  del %1
  echo   Deleted %~nx1
)
goto :eof

rem  -------------------------------------------------------------------------
