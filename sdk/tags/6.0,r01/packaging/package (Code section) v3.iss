[Code]
// This Code section uses a number of preprocessor variables defined in the
// the corresponding Setup section (in the file "package (Setup section).iss".
//
// This Code section contains a forward declaration for a boolean function
// called InitializeSetup_FirstPhase.  That function represents the first
// phase of the initialization, so the first thing that InitializeSetup does
// is call InitializeSetup_FirstPhase.  The script that includes this file
// must define the InitializeSetup_FirstPhase function after the include
// directive.

//-----------------------------------------------------------------------------

type
  // Information about an installed version of this package.
  TInstalledVersion = record
    Version: String;         // version and release in short form, e.g., "1.2 (b3)"
    Name: String;            // includes package name and version, e.g., "LANDIS-II Foo Extension v1.2 (b3)"
    UninstallerKey: String;  // registry key name for the version's uninstaller
  end;

  TBooleanFunction = function(): Boolean;

  // A function that does additional steps after running the current version's
  // uninstaller.
  // Returns:
  //   0 if the post-uninstallation steps are done successfully.
  //   <> 0 if an error occurred.
  TPostUninstall = function(versionRemoved: TInstalledVersion): Integer;

const
  UninstallersKey = 'Software\Microsoft\Windows\CurrentVersion\Uninstall';
  InstallerKeySuffix = '_is1';

var
  // Optional final phase of InitializeSetup.
  InitializeSetup_FinalPhase: TBooleanFunction;

  // Current version of the package that's installed.
  CurrentVersion: TInstalledVersion;

  // Custom page to ask about removing the current version.
  CurrVersPage: TWizardPage;
  CurrVers_RemoveRadioButton: TRadioButton;
  CurrVers_LeaveRadioButton: TRadioButton;

  // Optional function that's called after the current version has been
  // uninstalled.
  CurrVers_PostUninstall: TPostUninstall;

//-----------------------------------------------------------------------------

// A function to determine if a string starts with a particular prefix.
function StartsWith(const S, Prefix: String): Boolean;
begin
  Result := Copy(S, 0, Length(Prefix)) = Prefix;
end;

//-----------------------------------------------------------------------------

// Extracts information about an installed version from an uninstaller key
// name.

function GetInstalledVersion(const UninstallerKey: String): TInstalledVersion;
var
  suffixPos: Integer;
  versionPos: Integer;
begin
  Result.UninstallerKey := UninstallerKey;

  //  Full name of the version is the package name plus "v{version}"
  suffixPos := Pos(InstallerKeySuffix, UninstallerKey);
  if suffixPos > 0 then
    Result.Name := Copy(UninstallerKey, 1, suffixPos-1)
  else
    Result.Name := UninstallerKey;

  //  Extract the version and release from the full name
  versionPos := Pos(' v', Result.Name);
  if versionPos > 0 then
    Result.Version := Copy(Result.Name, versionPos + 2, Length(Result.Name))
  else
    Result.Version := '';
end;

//-----------------------------------------------------------------------------

// Gets the key names for all the software uninstallers on the machine.
// Returns True if the keys were successfully read from registry; returns
// False if keys could not be read.

function GetUninstallerKeys(var names: TArrayOfString): Boolean;
begin
  Result := RegGetSubkeyNames(HKLM, UninstallersKey, names);
end;

//-----------------------------------------------------------------------------

// Gets the key names for software uninstallers that start with a particular
// prefix.  Returns True if the keys were successfully read from registry;
// returns False if keys could not be read.

function GetUninstallerKeysStartingWith(const prefix: String; var names: TStringList): Boolean;
var
  uninstallerKeys: TArrayOfString;
  i: Integer;
begin
  if GetUninstallerKeys(uninstallerKeys) then
    begin
      names := TStringList.Create;
      for i := 0 to GetArrayLength(uninstallerKeys)-1 do
        begin
		  if StartsWith(uninstallerKeys[i], prefix) then
		    names.Add(uninstallerKeys[i]);
        end;
      Result := True;
	end
  else
    Result := False;
end;

//-----------------------------------------------------------------------------

// Gets the uninstall command-line associated with a particular uninstall key.

function GetUninstallCmd(uninstallKey: String): String;
var
  uninstallKey_FullPath: String;
begin
  Result := '';
  uninstallKey_FullPath := UninstallersKey + '\' + uninstallKey;
  if not RegQueryStringValue(HKLM, uninstallKey_FullPath, 'UninstallString', Result) then begin
    RegQueryStringValue(HKCU, uninstallKey_FullPath, 'UninstallString', result);
  end;
end;

//-----------------------------------------------------------------------------

// Runs an uninstall command-line.
// Returns the result code from executing the uninstall command.

function RunUninstallCmd(uninstallCmd: String): Integer;
begin
  uninstallCmd := RemoveQuotes(uninstallCmd);
  Exec(uninstallCmd, '/VERYSILENT', ExtractFilePath(uninstallCmd),
       SW_HIDE, ewWaitUntilTerminated, Result);
end;

//-----------------------------------------------------------------------------

// Gets the installed versions of a package.
// Parameters:
//   prefix : prefix that starts the package's uninstallers;
//            format = "LANDIS-II {package name} v"
//   installedVersions : array of information about the package's installed
//                       versions
// Returns:
//   True if any information was successfully read from the registry (Note:
//        this value is returned if there are no installed versions)
//   False if there was an error reading information from the registry

function GetInstalledVersions(const prefix: String;
                              var   installedVersions:  Array of TInstalledVersion)
         : Boolean;
var
  uninstallerKeys: TStringList;
  i: Integer;
begin
  if GetUninstallerKeysStartingWith(prefix, uninstallerKeys) then
    begin
    SetArrayLength(installedVersions, uninstallerKeys.Count);
    for i := 0 to uninstallerKeys.Count-1 do
      begin
      installedVersions[i] := GetInstalledVersion(uninstallerKeys[i]);
      end;
    Result := True;
    end
  else
    begin
    MsgBox('Error:  Cannot read information about installed software from the system registry.',
           mbError, MB_OK);
    Result := False;
    end;
end;

//-----------------------------------------------------------------------------

// See comment at top of file.
function InitializeSetup_FirstPhase(): Boolean;
forward;

//-----------------------------------------------------------------------------

#include AddBackslash(GetEnv("LANDIS_DEPLOY")) + "EnsureLandisIIDirsUC.iss"

//-----------------------------------------------------------------------------

function InitializeSetup(): Boolean;
var
  installedVersions: Array of TInstalledVersion;
  i: Integer;
  mesg: String;
begin
  EnsureLandisIIDirsUC();

  // Not really necessary to initialize these variables, but doing so
  // avoids warnings from compiler about unused variables.
  InitializeSetup_FinalPhase := nil;
  CurrVers_PostUninstall := nil;

  if not InitializeSetup_FirstPhase then
    begin
    Result := False
    exit
    end;

  if not DirExists('{#CoreBinDir}') then
    begin
    MsgBox('This package requires LANDIS-II {#CoreVersionRelease} which is not installed.',
           mbError, MB_OK)
    Result := False
    exit
    end;

  // Determine if there is already a version of this package installed
  if GetInstalledVersions('LANDIS-II {#PackageName} v', installedVersions) then
    case GetArrayLength(installedVersions) of
      0:
        CurrentVersion.Version := '';
      1:
        begin
        CurrentVersion := installedVersions[0];
        if CurrentVersion.Version = '{#Version}{#ReleaseForAppName}' then
          begin
          MsgBox('LANDIS-II {#PackageName} v{#Version}{#ReleaseForAppName} is already installed.',
                  mbInformation, MB_OK)
          Result := True
          exit
          end
        end
    else
      begin
      mesg := 'Warning:  Multiple versions of this package are already installed:'#13#10;
      for i := 0 to GetArrayLength(installedVersions)-1 do
        mesg := mesg + #13#10 + '    ' + installedVersions[i].Name;
      MsgBox(mesg, mbError, MB_OK);
      Result := True
      exit
      end;
    end;  // case

  if InitializeSetup_FinalPhase <> nil then
    begin
    Result := InitializeSetup_FinalPhase()
    exit
    end;

  Result := True
end;

//-----------------------------------------------------------------------------

// Compute the bottom position of a control.

function Bottom(control: TControl): Integer;
begin
  Result := control.Top + control.Height;
end;

//-----------------------------------------------------------------------------

procedure OnRadioButtonClick(sender: TObject);
begin
  WizardForm.NextButton.Enabled := CurrVers_RemoveRadioButton.Checked;
end;

//-----------------------------------------------------------------------------

procedure InitializeWizard;
var
  //staticText: TNewStaticText;
  uninstallCmd: String;
  resultCode: Integer;
  resultCodeFrom: String;
begin
//  if CurrentVersion.Version <> '' then
//    begin
//    CurrVersPage := nil
//    end;

//  else
//  begin
  if CurrVersPage <> nil then
    begin
      uninstallCmd := GetUninstallCmd(CurrentVersion.UninstallerKey);
      if uninstallCmd = '' then
        MsgBox('Error: ' + CurrentVersion.Name + ' has no uninstaller.',
             mbError, MB_OK)
      else
        begin
        resultCode := RunUninstallCmd(uninstallCmd);
        if resultCode <> 0 then
          resultCodeFrom := 'uninstaller'
        else if CurrVers_PostUninstall <> nil then
          begin
            resultCode := CurrVers_PostUninstall(CurrentVersion);
            if resultCode <> 0 then
              resultCodeFrom := 'post-uninstallation';
          end;
        if resultCode = 0 then
          MsgBox(CurrentVersion.Name + ' was successfully uninstalled.',
                 mbInformation, MB_OK)
        else
          MsgBox('Error occurred while uninstalling ' + CurrentVersion.Name + ':'#13#10
               + #13#10
               + '(exit code from ' + resultCodeFrom + ' = ' + IntToStr(resultCode) + ')',
               mbError, MB_OK);
        end;
      CurrentVersion.Name := '';
      CurrentVersion.Version := '';
      CurrentVersion.UninstallerKey := '';
    end;
  //Result := True;
//    end;
end;

//-----------------------------------------------------------------------------

function ShouldSkipPage(pageID: Integer): Boolean;
begin
  // Skip the SelectProgramGroup page because program group fixed.
  if pageID = wpSelectProgramGroup then
    Result := True

  // Skip the SelectDestination Directory page because that directory is fixed.
  else if pageID = wpSelectDir then
    Result := True

  else if (CurrVersPage <> nil) and (pageID = CurrVersPage.ID) then
      //  Skip the Current Version page if there is no current version
      if CurrentVersion.Version = '' then
        Result := True
      else
        Result := False

  else
    Result := False;
end;

//-----------------------------------------------------------------------------

procedure CurPageChanged(curPageID: Integer);
begin
  if (CurrVersPage <> nil) and (curPageID = CurrVersPage.ID) then
    begin
    CurrVers_RemoveRadioButton.Checked := False;
    CurrVers_LeaveRadioButton.Checked := True;
    WizardForm.NextButton.Enabled := False;
    end;
end;

//-----------------------------------------------------------------------------

function NextButtonClick(curPageID: Integer): Boolean;
var
  uninstallCmd: String;
  resultCode: Integer;
  resultCodeFrom: String;
begin
  if (CurrVersPage <> nil) and (curPageID = CurrVersPage.ID) then
    begin
    uninstallCmd := GetUninstallCmd(CurrentVersion.UninstallerKey);
    if uninstallCmd = '' then
      MsgBox('Error: ' + CurrentVersion.Name + ' has no uninstaller.',
             mbError, MB_OK)
    else
      begin
      resultCode := RunUninstallCmd(uninstallCmd);
      if resultCode <> 0 then
        resultCodeFrom := 'uninstaller'
      else if CurrVers_PostUninstall <> nil then
        begin
          resultCode := CurrVers_PostUninstall(CurrentVersion);
          if resultCode <> 0 then
            resultCodeFrom := 'post-uninstallation';
        end;
      if resultCode = 0 then
        MsgBox(CurrentVersion.Name + ' was successfully uninstalled.',
               mbInformation, MB_OK)
      else
        MsgBox('Error occurred while uninstalling ' + CurrentVersion.Name + ':'#13#10
               + #13#10
               + '(exit code from ' + resultCodeFrom + ' = ' + IntToStr(resultCode) + ')',
               mbError, MB_OK);
      end;
    CurrentVersion.Name := '';
    CurrentVersion.Version := '';
    CurrentVersion.UninstallerKey := '';
    end;
  Result := True;
end;

//-----------------------------------------------------------------------------

// Returns the text for the "Ready To Install" wizard page

function UpdateReadyMemo(space, newLine, memoUserInfoInfo, memoDirInfo,
                         memoTypeInfo, memoComponentsInfo, memoGroupInfo,
                         memoTasksInfo: String): String;
begin
  Result := memoDirInfo + newLine +
            newLine +
            memoGroupInfo + newLine
end;
