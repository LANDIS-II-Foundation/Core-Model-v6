[Code]
// The script that includes this file can specify one or more plug-in names
// that should be removed from the database of installed plug-ins:
//
// 1) set PlugInNamesToRemove to a list of names separated by "<|>",
//    e.g., "Foo Succession<|>Bar Output"
//
// 2) set PlugInNameToRemove to a single plug-in name
//
// 3) if neither of the two variables above are set, then the default is
//    to use the package's name as the plug-in name to remove.

#ifndef PlugInNamesToRemove
  #ifdef PlugInNameToRemove
    #define PlugInNamesToRemove PlugInNameToRemove
  #else
    #define PlugInNamesToRemove PackageName
  #endif
#endif

//-----------------------------------------------------------------------------

// Uninstall the previous version of the package.

procedure UninstallPreviousVersion();
  	var appId: String;
  	    uninstallKey: String;
  	    uninstaller: String;
  	    resultCode: Integer;

  	    namesToRemove: String;
  	    delimiter: String;
  	    delimiterLen: Integer;
  	    delimiterPos: Integer;
  	    name: String;
begin
  // See if there's uninstaller for previous version; if so, run it.
  appId := 'LANDIS-II {#PackageName} v{#UninstallPrevVersion}';
  uninstallKey := 'Software\Microsoft\Windows\CurrentVersion\Uninstall\' + appId + '_is1';
  uninstaller := '';
  if not RegQueryStringValue(HKLM, uninstallKey, 'UninstallString', uninstaller) then begin
    RegQueryStringValue(HKCU, uninstallKey, 'UninstallString', uninstaller);
  end;

  if uninstaller <> '' then begin
    uninstaller := RemoveQuotes(uninstaller);
    //msgBox('uninstaller = "' + uninstaller + '"', mbInformation, MB_OK);
    Exec(uninstaller, '/VERYSILENT', ExtractFilePath(uninstaller),
         SW_HIDE, ewWaitUntilTerminated, resultCode);

  #ifdef PlugInNamesToRemove
    // remove extensions from plug-ins database
    // names are stored in a string separated by delimiter string
    namesToRemove := '{#PlugInNamesToRemove}';
    delimiter := '<|>';
    delimiterLen := Length(delimiter);
    while Length(namesToRemove) > 0 do begin
		// extract the left-most name in the list of names
		delimiterPos := Pos(delimiter, namesToRemove);
		if delimiterPos > 0 then begin
			name := Copy(namesToRemove, 1, delimiterPos-1);
			namesToRemove := Copy(namesToRemove, delimiterPos+delimiterLen, Length(namesToRemove));
		end
		else begin
			name := namesToRemove;
			namesToRemove := '';
		end;
		if Length(name) > 0 then begin
			Exec('{#PlugInAdminTool}', 'remove "' + name + '"', ExtractFilePath('{#PlugInAdminTool}'),
				 SW_HIDE, ewWaitUntilTerminated, resultCode);
			//MsgBox('Name to remove = "' + name + '"', mbInformation, MB_OK);
		end;
	end;
  #endif

  end;
end;

