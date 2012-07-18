@echo off

set ScriptDir=%~dp0
rem Trim off trailing "\bin\"
set LandisRootDir=%ScriptDir:~0,-5%

set FileName=%~n0
set Version=%FileName:landis-=%
set Version=%Version:-extensions=%
for /f "delims=." %%X in ("%Version%") do set MajorVer=%%X

"%LandisRootDir%\v%MajorVer%\bin\Landis.Extensions-%Version%.exe" %*
