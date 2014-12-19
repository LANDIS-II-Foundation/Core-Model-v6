rem Run a companion script file without a command prompt window flashing
rem on the screen.
batchFile = Replace(WScript.ScriptFullName, ".vbs", ".cmd")
windowStyle = 0  rem Hide the window
Dim WinScriptHost
Set WinScriptHost = CreateObject("WScript.Shell")
WinScriptHost.Run Chr(34) & batchFile & Chr(34), windowStyle
Set WinScriptHost = Nothing