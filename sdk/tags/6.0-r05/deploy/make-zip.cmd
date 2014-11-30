@echo off
setlocal

rem  Is MSBuild on the PATH?
MSBuild.exe /version > nul 2>&1
if not errorlevel 1 (
  set MSBUILD_EXE=MSBuild.exe
  goto :foundMSBuild
)

rem  Check if NET 3.5 MSBuild available
set MSBUILD_35=%WINDIR%\Microsoft.NET\Framework\v3.5\MSBuild.exe
if exist "%MSBUILD_35%" (
  set MSBUILD_EXE=%MSBUILD_35%
  goto :foundMSBuild
)

echo Error: Cannot locate MSBuild.exe on PATH or in NET 3.5 Framework
exit /b 1

rem --------------------------------------------------------------------------

:foundMSBuild

"%MSBUILD_EXE%" package.proj /t:zip-file