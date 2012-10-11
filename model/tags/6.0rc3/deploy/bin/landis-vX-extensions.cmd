@echo off

set ScriptDir=%~dp0
rem Trim off trailing "\bin\"
set LandisRootDir=%ScriptDir:~0,-5%

set FileName=%~n0
set MajorVer=%FileName:landis-v=%
set MajorVer=%MajorVer:-extensions=%

"%LandisRootDir%\v%MajorVer%\bin\Landis.Extensions.exe" %*
