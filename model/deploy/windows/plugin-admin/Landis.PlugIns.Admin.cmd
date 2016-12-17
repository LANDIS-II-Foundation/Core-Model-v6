@if "%DEBUG_BATCH%" == "" echo off
setlocal

set ScriptDir=%~dp0
set ScriptDir=%ScriptDir:~0,-1%
set ScriptName=%~n0
set Log=%ScriptDir%\%ScriptName%_Log.txt

echo =============================================================== >> "%Log%"
for /f "delims=;" %%D in ('date /t') do set CurrentDate=%%D
for /f "delims=;" %%T in ('time /t') do set CurrentTime=%%T
echo %CurrentDate%, %CurrentTime% >> "%Log%"
echo. >> "%Log%"
echo Script arguments: %* >> "%Log%"
echo. >> "%Log%"

rem This script located in LANDIS-II\6.0\bin (known as CoreBinDir in extension
rem installers).
set LandisII60BinDir=%ScriptDir%
set LandisII60Dir=%ScriptDir:\bin=%
set LandisIIDir=%LandisII60Dir:\6.0=%

set LandisIIv6Dir=%LandisIIDir%\v6
set LandisIIv6BinDir=%LandisIIv6Dir%\bin
set LandisIIv6ExtDir=%LandisIIv6BinDir%\extensions

set ExtAdminTool=%LandisIIv6BinDir%\Landis.Extensions.exe
echo Running: "%ExtAdminTool%" %* >> "%Log%"
"%ExtAdminTool%" %* >> "%Log%"
if errorlevel 1 (
  echo ERRORLEVEL = %ERRORLEVEL% >> "%Log%"
  exit /b 1
)

if not "%1" == "add" goto :eof

echo. >> "%Log%"
echo Moving assemblies from 6.0\bin to v6\bin\extensions ... >> "%Log%"
for %%A in ("%LandisII60BinDir%\*.dll") do call :moveItem "%%A" "%LandisIIv6ExtDir%"

echo. >> "%Log%"
echo Moving documentation from 6.0\doc to v6\doc ... >> "%Log%"
set LandisII60Docs=%LandisII60Dir%\docs
set LandisIIv6Docs=%LandisIIv6Dir%\docs
for %%F in ("%LandisII60Docs%\*") do call :moveItem "%%F" "%LandisIIv6Docs%"

echo. >> "%Log%"
echo Moving examples from 6.0\examples to v6\examples ... >> "%Log%"
set LandisII60Examples=%LandisII60Dir%\examples
set LandisIIv6Examples=%LandisIIv6Dir%\examples
for %%F in ("%LandisII60Examples%\*") do call :moveItem "%%F" "%LandisIIv6Examples%"
for /d %%D in ("%LandisII60Examples%\*") do call :moveItem "%%D" "%LandisIIv6Examples%"

goto :eof

rem --------------------------------------------------------------------------

:moveItem

move /y %1 %2
echo Moved %1 >> "%Log%"
goto :eof
