@ECHO OFF
:: Windows 7, 8, 8.1 only; FINDSTR validates Windows version
IF NOT "%OS%"=="Windows_NT" GOTO Syntax
VER | FINDSTR /R /E /C:" 6\.[0-3]\.[0-9\.]*\]" >NUL || GOTO Syntax

:: Check command line arguments
IF     "%~1"=="" GOTO Syntax
IF NOT "%~3"=="" GOTO Syntax
ECHO "%~1" | FINDSTR /R /C:"[/?]" >NUL && GOTO Syntax

SETLOCAL ENABLEDELAYEDEXPANSION
SET Count=0
SET "strExc=%~1 %~2"
FOR /F "tokens=*" %%A IN ('REG Query HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall /F "%~1" /D /S 2^>NUL ^| FINDSTR /R /B /C:"HKEY_"') DO (
	REG Query "%%~A" /F DisplayName /V /E | FINDSTR /R /I /C:" DisplayName .* .*%~1" >NUL 2>&1
	IF NOT ERRORLEVEL 1 (
		REG Query "%%~A" /F DisplayName /V /E | FINDSTR /R /I /C:" DisplayName .* .*%strExc%" >NUL 2>&1
		IF ERRORLEVEL 1 (
			SET /A Count += 1
			FOR /F "tokens=2*" %%B IN ('REG Query "%%~A" /F DisplayName    /V /E 2^>NUL ^| FIND /I " DisplayName "')     DO ECHO Program Name      = %%C
			FOR /F "tokens=2*" %%B IN ('REG Query "%%~A" /F QuietUninstallString /V /E 2^>NUL ^| FIND /I " QuietUninstallString "')  DO ECHO Quiet Uninstall String   = %%C
			ECHO.
		)
	)
)

WMIC.EXE Path Win32_Processor Get DataWidth 2>NUL | FIND "64" >NUL
IF ERRORLEVEL 1 (
	ECHO.
	ECHO %Count% programs found
) ELSE (
	SET Count32bit=0
	FOR /F "tokens=*" %%A IN ('REG Query HKLM\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall /F "%~1" /D /S 2^>NUL ^| FINDSTR /R /B /C:"HKEY_"') DO (
		REG Query "%%~A" /F DisplayName /V /E | FINDSTR /R /I /C:" DisplayName .* .*%~1" >NUL 2>&1
		IF NOT ERRORLEVEL 1 (
			REG Query "%%~A" /F DisplayName /V /E | FINDSTR /R /I /C:" DisplayName .* .*LANDIS-II 6.0" >NUL 2>&1
			IF ERRORLEVEL 1 (			
				SET /A Count32bit += 1
				FOR /F "tokens=2*" %%B IN ('REG Query "%%~A" /F DisplayName    /V /E 2^>NUL ^| FIND /I " DisplayName "')     DO ECHO Program Name      = %%C
				FOR /F "tokens=2*" %%B IN ('REG Query "%%~A" /F QuietUninstallString /V /E ^| FIND /I " QuietUninstallString "') DO (
					ECHO Quiet Uninstall String  = %%C
					%%C
				)
				ECHO.
			)
		)
	)
	ECHO.
	ECHO     %Count% 64-bit programs and !Count32bit! 32-bit programs uninstalled
)

ENDLOCAL
GOTO:EOF


:Syntax
ECHO.
ECHO GetUninstall.bat,  Version 2.00 for Windows Vista and later
ECHO List or search uninstall command lines
ECHO.
ECHO Usage:    GETUNINSTALL.BAT  "filter"
ECHO.
ECHO Where:    "filter"    narrows down the search result to programs whose
ECHO                       uninstall data contains the string "filter"
ECHO	       "version"   version indicating the LANDIS-II core that we do not want
ECHO                       to uninstall using this script (optional)
ECHO.
ECHO Example:  GETUNINSTALL.BAT "LANDIS-II" "6.0"
ECHO.
ECHO Written by Rob van der Woude
ECHO http://www.robvanderwoude.com
ECHO Adapted from GetUninstall.bat http://www.robvanderwoude.com/sourcecode.php?src=getuninstall_w7
ECHO Permission received from Rob van der Woude on 18=NOV-2014 to use script in LANDIS-II

:: Set return code for Windows NT 4 or later
IF "%OS%"=="Windows_NT" COLOR 00
