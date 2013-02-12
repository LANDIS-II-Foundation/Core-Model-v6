// To these subroutines display debugging information in message boxes,
// either uncomment the pre-processor directive below, or define the
// Debug variable before including this file.  Otherwise, these
// subroutines do not report anything to the user.
//#define Debug

//-----------------------------------------------------------------------------

// Determines if a directory contains a sub-directory called "LANDIS-II"
// It ignores case, so it'll find a sub-directory called "landis-ii" or
// "Landis-II".
// Returns the name of the LANDIS-II sub-directory, or '' if there is no
// sub-directory with that name.
function GetLandisIISubDir(dir: String): String;
var
  foundSubDir: Boolean;
  findRec: TFindRec;
begin
  foundSubDir := False;
  if FindFirst(AddBackslash(dir)+'L*', findRec) then begin
    try
      repeat
        // Skip if not a directory
        if findRec.Attributes and FILE_ATTRIBUTE_DIRECTORY = 0 then
          continue;
        if UpperCase(findRec.Name) = 'LANDIS-II' then
          foundSubDir := True;
      until foundSubDir or not FindNext(findRec);
    finally
      FindClose(findRec);
    end;
  end;
  if foundSubDir then
    Result := findRec.Name
  else
    Result := '';
end;

//-----------------------------------------------------------------------------

// Renames a sub-directory to all uppercase.
// Parameters:
//   path = full path to the sub-directory
procedure RenameSubDirAsUppercase(path: String);
var
  dirName: String;
  dirNameUC: String;
  newPath: String;
begin
  if not DirExists(path) then
    MsgBox('Error: The directory "' + path + '" does not exist.',
           mbError, MB_OK)
  else
    begin
    #ifdef Debug
      MsgBox('RenameSubDirAsUppercase called; path = "' + path + '"',
             mbInformation, MB_OK);
    #endif
    dirName := ExtractFileName(path);
    dirNameUC := Uppercase(dirName);
    if dirName <> dirNameUC then
      begin
      newPath := ExtractFilePath(path) + dirNameUC;
      #ifdef Debug
        MsgBox('Renaming "' + path + '" to "' + newPath + '"',
               mbInformation, MB_OK);
      #endif
      if not RenameFile(path, newPath) then
        begin
        #ifdef Debug
          MsgBox('Error: rename operation failed', mbError, MB_OK);
        #endif
        end;
      end;
    end;
end;

//-----------------------------------------------------------------------------

// Ensures various LANDIS-II sub-folders are uppercase.
procedure EnsureLandisIIDirsUC;
var
  dirs: TStringList;
  i: Integer;
  dir: String;
  landisIIDirName: String;
  landisIIDirPath: String;
begin
  dirs := TStringList.Create();
  dirs.Add(ExpandConstant('{pf}'));
  dirs.Add(ExpandConstant('{commonprograms}'));
  dirs.Add(ExpandConstant('{userprograms}'));

  for i := 0 to dirs.Count-1 do
    begin
    dir := dirs[i];
    #ifdef Debug
      MsgBox(dirs[i], mbInformation, MB_OK);
    #endif
    landisIIDirName := GetLandisIISubDir(dir);
    if landisIIDirName = '' then
      begin
      #ifdef Debug
        MsgBox('No Landis-II folder in ' + dir, mbInformation, MB_OK);
      #endif
      end
    else
      begin
      landisIIDirPath := AddBackslash(dir) + landisIIDirName;
      #ifdef Debug
        MsgBox('LANDIS-II folder = ' + landisIIDirPath,
               mbInformation, MB_OK);
      #endif
      RenameSubDirAsUppercase(landisIIDirPath);
      end;
    end;
end;
