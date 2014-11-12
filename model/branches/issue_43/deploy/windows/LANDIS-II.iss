#define ScriptDir    ExtractFilePath(SourcePath)
#define DeployDir    ExtractFilePath(ScriptDir)
#define SolutionDir  ExtractFilePath(DeployDir)

#define DocDir           SolutionDir + "\docs"

#define BuildDir         SolutionDir + "\build"
#define ReleaseConfigDir BuildDir + "\Release"

#define StagingDir         BuildDir + "\install"
#define ReleaseStagingDir  StagingDir + "\Release"

;-----------------------------------------------------------------------------
; Fetch the version # from the core assembly
#define CoreName "Landis.Core.dll"
#define CorePath ReleaseConfigDir + "\" + CoreName
#pragma message 'Getting version from "' + CorePath + '" ...'
#if ! FileExists(CorePath)
  #error The Release configuration of the model has not been built
#endif
#define Major
#define Minor
#define Revision
#define Build
#define CoreVersion ParseVersion(CorePath, Major, Minor, Revision, Build)
#pragma message "Core version = " + CoreVersion

#define MajorMinor Str(Major) + "." + Str(Minor)
#define Version    MajorMinor

; Make sure that the Release configuration has been staged
#define StagedCorePath ReleaseStagingDir + "\v" + Str(Major) + "\bin\" + MajorMinor + "\" + CoreName
#pragma message "StagedCorePath = " + StagedCorePath
#if ! FileExists(StagedCorePath)
  #error The Release configuration of the model has not been staged
#endif
#define StagedCoreVersion GetFileVersion(StagedCorePath)
#if CoreVersion != StagedCoreVersion
  #error The staged copy of the model's Release configuration is out of date
#endif

; Read the release status and define variables with release information
#include "release-status.iss"

#if ReleaseAbbr != ""
  #define VersionReleaseName Version + " (" + ReleaseAbbr + ")"
#else
  #define VersionReleaseName Version
#endif
#if ReleaseType == "revision"
  #define VersionRelease     Version + "," + ReleaseAbbr
#else
  #define VersionRelease     Version + ReleaseAbbr
#endif
#define VersionReleaseFull   Version + " (" + Release + ")"

;-----------------------------------------------------------------------------

; Determine if the Inno Setup compiler (not the generated setup program) is
; running on a 32-bit or 64-bit system.
;
; NOTE: Inno Setup itself is a 32-bit program.
;
; This check is based on this blog post:
; http://blogs.msdn.com/b/david.wang/archive/2006/03/26/howto-detect-process-bitness.aspx

#if GetEnv("PROCESSOR_ARCHITECTURE") + "," + GetEnv("PROCESSOR_ARCHITEW6432") == "x86,"
  #define NBits "32"
#else
  #define NBits "64"
#endif
#pragma message "Script being compiled on " + NBits + "-bit Windows"

;-----------------------------------------------------------------------------

[Setup]
AppName=LANDIS-II {#VersionReleaseName}
AppVerName=LANDIS-II {#VersionReleaseFull}
AppPublisher=LANDIS-II Core Development Team
DefaultDirName={pf}\LANDIS-II
UsePreviousAppDir=no
DefaultGroupName=LANDIS-II\v{#Major}
UsePreviousGroup=no
SourceDir={#ReleaseStagingDir}
OutputDir={#ScriptDir}
OutputBaseFilename=LANDIS-II-{#VersionRelease}-setup{#NBits}
VersionInfoVersion={#Major}.{#Minor}.{#ReleaseAsInt}
ChangesEnvironment=yes
SetupLogging=yes
LicenseFile={#ScriptDir}\LANDIS-II_Binary_license.rtf
UninstallFilesDir={app}\config

; GDAL 64-bit binaries are built for x64
ArchitecturesInstallIn64BitMode=x64

; Allow the user to only run the setup program on the same N-bit system as it
; was built on.
#if NBits == "32"
  ArchitecturesAllowed=x86
#else
  ArchitecturesAllowed=x64
#endif


[Files]

; Use everything in staging directory except extensions database file and Unix shell scripts
Source: *; DestDir: {app}; Flags: recursesubdirs uninsneveruninstall; Excludes: "*.xml,*.sh"

; An interim version of the old plug-in admin tool for current extension
; installers (until they are updated to call the landis-v6-extensions.cmd
; script).
Source: {#ScriptDir}\plugin-admin\Landis.PlugIns.Admin.exe; DestDir: {app}\6.0\bin
Source: {#ScriptDir}\plugin-admin\Landis.PlugIns.Admin.cmd; DestDir: {app}\6.0\bin

; Documentation
; (Note: Documentation among minor versions can reside in the same folder
;        because all the files have version #s in their names.)
Source: {#DocDir}\LANDIS-II Model v6.0 Description.pdf; DestDir: {app}\v{#Major}\docs
Source: {#DocDir}\LANDIS-II Model v6.0 User Guide.pdf;  DestDir: {app}\v{#Major}\docs

; No example input files but a read me.
Source: {#DocDir}\READ ME.TXT; DestDir: {app}\v{#Major}\examples

; INI file with information about this release
Source: {#ScriptDir}\LANDIS-II X.Y.ini; DestDir: {app}\config; DestName: LANDIS-II {#MajorMinor}.ini


[Icons]
Name: {group}\Documentation;                         Filename: {app}\v{#Major}\docs
Name: {group}\Sample Input Files;                    Filename: {app}\v{#Major}\examples
Name: {group}\Licenses;                              Filename: {app}\v{#Major}\licenses; Flags: uninsneveruninstall
Name: {group}\Uninstall\LANDIS-II {#VersionRelease}; Filename: {uninstallexe}; Parameters: "/log";

[Registry]
; Add the LANDIS-II bin directory to the PATH environment variable
Root: HKLM; Subkey: "SYSTEM\CurrentControlSet\Control\Session Manager\Environment";   \
            ValueType: expandsz; ValueName: "Path"; ValueData: "{olddata};{app}\bin"; \
            Check: DirNotInPath(ExpandConstant('{app}\bin'))

[Run]
; Run the extension admin tool so if the extension dataset doesn't exist, it'll
; be created.  Need to do this during installation because running with elevated
; privileges on Vista and newer Windows.  A normal user can't run the tool and
; have it create the initial empty dataset because the user doesn't have write
; access to the "Program Files" folder.  Note: running the tool with no
; parameters simply has it display a brief list of installed extensions.
Filename: landis-v{#Major}-extensions.cmd; WorkingDir: {app}\bin; Flags: runhidden

[UninstallRun]
Filename: uninstall-extensions.cmd; Parameters: LANDIS-II {#MajorMinor} ; WorkingDir: {app}\bin;
Filename: uninstall-landis.cmd; Parameters: {#MajorMinor} ; WorkingDir: {app}\bin; Flags: runhidden

;-----------------------------------------------------------------------------

[Code]

const
  MajorVersion = '{#Major}';
  MajorMinor = '{#MajorMinor}';

var
  // Was the Inno Setup script for this installer found in the same directory
  // as the installer itself?  Used to determine whether to allow the user to
  // select the Destination Directory.
  InnoSetupScriptFound: Boolean;

function InitializeSetup(): Boolean;
var
  InstallerDir : String;
  InnoSetupScript : String;
begin
  InstallerDir := ExpandConstant('{src}');
  InnoSetupScript := AddBackslash(InstallerDir) + 'LANDIS-II.iss';
  InnoSetupScriptFound := FileExists(InnoSetupScript);
  Result := True;
end;

// ----------------------------------------------------------------------------

// Returns the path to the INI file for this particular LANDIS-II version

function PathToIniFile(): String;
var
  ConfigDir : String;
begin
  ConfigDir := ExpandConstant('{app}') + '\config';
  Result := ConfigDir + '\LANDIS-II ' + MajorMinor + '.ini';
end;

// ----------------------------------------------------------------------------

// Returns the path to the temporary command script used when uninstalling a
// release of the same LANDIS-II version.  See the procedure UninstallRelease
// below.

function PathToTemporaryScript(): String;
begin
  Result := ExpandConstant('{app}') + '\bin\landis-' + MajorVersion + '.Y.cmd';
end;

// ----------------------------------------------------------------------------

procedure UninstallRelease(CurrentRelease, IniFile: String);
var
  Uninstaller : String;
  ResultCode : Integer;
  TemporaryScript : String;
begin
  Uninstaller := GetIniString('LANDIS-II', 'uninstaller', '', IniFile);
  if Uninstaller = '' then
    begin
    Log('UninstallRelease: No uninstaller in "' + IniFile + '"');
    exit;
    end;

  if not FileExists(Uninstaller) then
    begin
    Log('UninstallRelease: uninstaller = "' + uninstaller + '" does not exist');
    exit;
    end;

  // Create a temporary dummy command script so if the current release is the
  // only release installed, then its uninstaller will not remove all the
  // LANDIS-II files including installed extensions.  In other words, keep the
  // installed extensions for the new release being installed.
  TemporaryScript := PathToTemporaryScript();
  if SaveStringToFile(TemporaryScript, 'REM temporary file' + #13#10, False) then
    Log('UninstallRelease: created temporary script "' + TemporaryScript + '"')
  else
    begin
    Log('UninstallRelease: unable to create temporary script "' + TemporaryScript + '"');
    exit;
    end;

  if Exec(Uninstaller, '/silent /log', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
    Log('UninstallRelease: uninstaller "' + Uninstaller + '" ran OK; exit code = ' + IntToStr(ResultCode))
  else
    Log('UninstallRelease: uninstaller "' + Uninstaller + '" failed; error code = ' + IntToStr(ResultCode));

  // The documentation for the Exec() function and ewWaitUntilTerminated says
  // the function returns after the process is terminated.  However, it's
  // returning while the uninstaller is still running (based on timestamps in
  // the logs for this installer and the uninstaller).  Perhaps, the uninstaller
  // spawns another process?  Anyhow, if we delete the temporary script here,
  // the uninstaller actually doesn't see it, and therefore, removes all of the
  // corresponding vX/ folder if there's only one minor version of vX installed.
  // Workaround = delete the temporary script in the post-install step (see
  // event function CurStepChanged below).
end;

// ----------------------------------------------------------------------------

// The installer for LANDIS-II 6.0 RC3 didn't create an INI file.  So check to
// see if it's installed.  If so, create an INI file for it.  Returns True if
// 6.0 RC3 is installed.

function CheckFor60RC3() : Boolean;
var
  UninstallKey : String;
  RootKey : Integer;
  UninstallCommand : String;
  IniFile : String;
  IniDir : String;
begin
  // Check if uninstall key exists for 6.0 RC3 in registry.
  UninstallKey := 'Software\Microsoft\Windows\CurrentVersion\Uninstall\LANDIS-II 6.0 (rc3)_is1';
  if RegKeyExists(HKLM, UninstallKey) then
    begin
    Log('RegKeyExists(HKLM, ' + UninstallKey + ')');
    RootKey := HKLM;
    end
  else if RegKeyExists(HKCU, UninstallKey) then
    begin
    Log('RegKeyExists(HKCU, ' + UninstallKey + ')');
    RootKey := HKCU;
    end
  else
    begin
    Log('LANDIS-II 6.0 (rc3) is not installed');
    Result := False;
    exit;
    end;

  // 6.0 RC3 is installed.
  Result := True;

  // Get the uninstall command for the key.
  if RegQueryStringValue(RootKey, UninstallKey, 'UninstallString', UninstallCommand) then
    begin
    Log('UninstallString for LANDIS-II 6.0 (rc3): ' + UninstallCommand);
    UninstallCommand := RemoveQuotes(UninstallCommand);
    end
  else
    begin
    Log('RegQueryString(RootKey, ' + UninstallKey + ', "UninstallString") failed');
    exit;
    end;

  // Create the .INI file for 6.0 RC3 with the uninstall command.
  IniFile := PathToIniFile();
  if FileExists(IniFile) then
    begin
    Log('INI file for 6.0 RC3 already exists: "' + IniFile + '"');
    exit;
    end;
  IniDir := ExtractFilePath(IniFile);
  if not DirExists(IniDir) then
    begin
    if ForceDirectories(IniDir) then
      Log('Created directory "' + IniDir + '"')
    else
      begin
      Log('Error: unable to create directory "' + IniDir + '"');
      exit;
      end;
    end;
  if not SaveStringToFile(IniFile, '[LANDIS-II]' + #13#10, False) then
    begin
    Log('Error: unable to create file "' + IniFile + '"');
    exit;
    end;
  SetIniString('LANDIS-II', 'version',     MajorMinor,            IniFile);
  SetIniString('LANDIS-II', 'release',     'release candidate 3', IniFile);
  SetIniString('LANDIS-II', 'uninstaller', UninstallCommand,      IniFile);
  Log('Created file "' + IniFile + '"');
end;

// ----------------------------------------------------------------------------

// 6.0 RC3 installed its GDAL native files under M.m.R/, but with 6.0 official
// GDAL now installed under M.m/.  So rename its GDAL folder.procedure CheckFor60RC3();

procedure RenameGDAL191Dir();
var
  GDAL191Dir : String;
  GDAL19Dir : String;
begin
  GDAL191Dir := ExpandConstant('{app}') + '\GDAL\1.9.1';
  GDAL19Dir  := ExpandConstant('{app}') + '\GDAL\1.9';
  if DirExists(GDAL191Dir) and not DirExists(GDAL19Dir) then
    begin
    if RenameFile(GDAL191Dir, GDAL19Dir) then
      Log('Renamed directory "' + GDAL191Dir + '" as "1.9"')
    else
      Log('Error while trying to rename directory "' + GDAL191Dir + '" as "1.9"');
    end;
end;

// ----------------------------------------------------------------------------

function NextButtonClick(CurPageID: Integer): Boolean;
var
  Is60RC3Installed : Boolean;
  IniFile : String;
  CurrentRelease : String;
  ReplaceResponse : Integer;
begin
  if CurPageId = wpSelectDir then
    begin
    // Special handling for L-II 6.0 RC3; can be removed in L-II 6.1
    Is60RC3Installed := CheckFor60RC3();

    IniFile := PathToIniFile();
    if FileExists(IniFile) then
      begin
      CurrentRelease := GetIniString('LANDIS-II', 'release', '(?)', IniFile);
      if CurrentRelease = '{#Release}' then
        begin
        MsgBox('LANDIS-II {#VersionReleaseFull} is already installed in this directory.', mbError, MB_OK);
        Result := False;
        end
      else
        begin
        ReplaceResponse := MsgBox('LANDIS-II ' + MajorMinor + ' (' + CurrentRelease + ') is already installed.  Replace it?', mbConfirmation, MB_OKCANCEL or MB_DEFBUTTON2);
        if ReplaceResponse = IDOK then
          begin
          if Is60RC3Installed then
            RenameGDAL191Dir();
          UninstallRelease(CurrentRelease, IniFile);
          Result := True;
          end
        else
          Result := False;
        end
      end
    else
      Result := True;
    end
  else // Current page is not Select Directory
    Result := True;
end;

// ----------------------------------------------------------------------------

// This function returns True if the current version number (i.e., major.minor)
// is in the extension admin tool's config file.  If present, then the tool is
// probing that particular version's subdirectory.

function VersionInToolConfig(): Boolean;
var
  LandisRootDir: String;
  MajorVerBinDir: String;
  ToolConfig: String;
  ToolConfigContents: String;
  PosOfVersion: Integer;
begin
  LandisRootDir := ExpandConstant('{app}');
  MajorVerBinDir := LandisRootDir + '\v' + MajorVersion + '\bin';
  ToolConfig := MajorVerBinDir + '\Landis.Extensions.exe.config';
  if not FileExists(ToolConfig) then
    begin
    Log('VersionInToolConfig: "' + ToolConfig + '" does not exist');
    Result := False;
    exit;
    end;
  if not LoadStringFromFile(ToolConfig, ToolConfigContents) then
    begin
    Log('VersionInToolConfig: Cannot read contents from "' + ToolConfig + '"');
    Result := False;
    exit;
    end;
  PosOfVersion := Pos(MajorMinor + ';', ToolConfigContents);
  Log('VersionInToolConfig: Position of "' + MajorMinor + '" in file = ' + IntToStr(PosOfVersion));
  Result := PosOfVersion <> 0;
end;

// ----------------------------------------------------------------------------

// This function and its corresponding use in the Check parameter in the
// Registry are described in this Stack Overflow answer:
// http://stackoverflow.com/a/3431379/1258514

function DirNotInPath(Directory: String): Boolean;
var
  OrigPath: String;
  PosInPath: Integer;
begin
  if not RegQueryStringValue(HKEY_LOCAL_MACHINE,
      'SYSTEM\CurrentControlSet\Control\Session Manager\Environment',
      'Path', OrigPath) then
    begin
    Result := True;
    exit;
    end;
  // look for the path with leading and trailing semicolon
  // Pos() returns 0 if not found
  PosInPath := Pos(';' + Directory + ';', ';' + OrigPath + ';');
  Result := PosInPath = 0;
  Log('DirNotInPath: Directory = "' + Directory + '"');
  Log('DirNotInPath: OrigPath = "' + OrigPath + '"');
  Log('DirNotInPath: PosInPath = ' + IntToStr(PosInPath));
end;

{-----------------------------------------------------------------------------}

function ShouldSkipPage(CurPage: Integer): Boolean;
begin
  { Skip the SelectProgramGroup page because program group fixed. }
  if CurPage = wpSelectProgramGroup then
    Result := True
  { Skip the Select Destination Directory page because that directory is fixed.
    Unless the Inno Setup script is alongside the installer, in which case, do
    NOT skip the page.  Enables the developer who's building the installer to
    select a different directory for test purposes. }
  else if CurPage = wpSelectDir then
    Result := not InnoSetupScriptFound
  else
    Result := False
end;

{-----------------------------------------------------------------------------}

function UpdateReadyMemo(Space, NewLine, MemoUserInfoInfo, MemoDirInfo,
                         MemoTypeInfo, MemoComponentsInfo, MemoGroupInfo,
                         MemoTasksInfo: String): String;
  { Return text for "Ready To Install" wizard page }
begin
  Result := MemoDirInfo + NewLine +
            NewLine +
            MemoGroupInfo + NewLine;
end;

// ----------------------------------------------------------------------------

// Store information about the installed release in the associated INI file.

procedure CurStepChanged(CurStep: TSetupStep);
var
  IniFile : String;
  Uninstaller : String;
  TemporaryScript : String;
begin
  if CurStep = ssPostInstall then
    begin
    IniFile := PathToIniFile();
    Uninstaller := ExpandConstant('{uninstallexe}');
    SetIniString('LANDIS-II', 'version',     MajorMinor,   IniFile);
    SetIniString('LANDIS-II', 'release',     '{#Release}', IniFile);
    SetIniString('LANDIS-II', 'uninstaller', Uninstaller,  IniFile);

    // Remove the temporary script (see UninstallRelease procedure above).
    TemporaryScript := PathToTemporaryScript();
    if DeleteFile(TemporaryScript) then
      Log('Deleted temporary script "' + TemporaryScript + '"')
    else
      Log('Error trying to delete temporary script "' + TemporaryScript + '"')
    end;
end;

// ----------------------------------------------------------------------------

// Count the number of LANDIS-II versions that are current installed.  Each
// version has its own "landis-X.Y.cmd" script, where X.Y is its version
// number (X = major version, Y = minor version).
//
// SiblingMinorVersions is assigned the number of minor versions found that are
// siblings of the version being uninstalled.  Siblings have the same major
// version number.

function CountVersions(var SiblingMinorVersions: Integer): Integer;
var
  BinDir: String;
  NumVersionsFound: Integer;
  FindRec: TFindRec;
  SiblingMarker: String;
  SiblingStatus: String;
begin
  SiblingMinorVersions := 0;
  BinDir := ExpandConstant('{app}\bin');
  if not DirExists(BinDir) then
    begin
    Log('CountVersions: directory does not exist: ' + BinDir);
    Result := 0;
    exit;
    end;

  // Look for "landis-X.Y.cmd" scripts
  NumVersionsFound := 0;
  SiblingMarker := '-' + MajorVersion + '.';  // The "-X." in script name
  if FindFirst(BinDir + '\landis-?.?.cmd', FindRec) then
    begin
    try
      repeat
        NumVersionsFound := NumVersionsFound + 1;
        if Pos(SiblingMarker, FindRec.Name) > 0 then
          begin
          SiblingStatus := ' (sibling)';
          SiblingMinorVersions := SiblingMinorVersions + 1;
          end
        else
          SiblingStatus := '';
        Log('CountVersions: (' + IntToStr(NumVersionsFound) + ') ' + BinDir + '\' + FindRec.Name + SiblingStatus);
      until not FindNext(FindRec);
    finally
      FindClose(FindRec);
    end;
    end;  // if
  Log('CountVersions: # found = ' + IntToStr(NumVersionsFound))
  Log('CountVersions: # of siblings = ' + IntToStr(SiblingMinorVersions))
  Result := NumVersionsFound;
end;

// ----------------------------------------------------------------------------

// Notify other top-level windows (applications) that there's been a change in
// the environment variables.  Based on this posting:
//
// http://news.jrsoftware.org/news/innosetup/msg25626.html

procedure BroadcastEnvironmentChange();
var
  Leaf: String;
  LeafPtr: Longint;
  ReturnVal: Longint;
begin
  Leaf := 'Environment';
  LeafPtr := CastStringToInteger(Leaf);
  ReturnVal := SendBroadcastMessage($1A {WM_SETTINGCHANGE}, 0, LeafPtr);
  Log('BroadcastEnvironmentChange: Result from SendBroadcastMessage = ' + IntToStr(ReturnVal));
end;

// ----------------------------------------------------------------------------

// Remove a specified directory from the system PATH environment variable.

procedure RemoveDirFromPath(Directory: String);
var
  CurrentPath: String;
  PosInPath: Integer;
  NewPath: String;
begin
  if not RegQueryStringValue(HKEY_LOCAL_MACHINE,
      'SYSTEM\CurrentControlSet\Control\Session Manager\Environment',
      'Path', CurrentPath) then
    begin
    // Nothing to do
    exit;
    end;

  // Look for the directory in the path (using the same trick with leading and
  // trailing semicolon as in the function DirNotInPath above).
  // Pos() returns 0 if not found
  PosInPath := Pos(';' + Directory + ';', ';' + CurrentPath + ';');
  Log('RemoveDirFromPath: Directory = "' + Directory + '"');
  Log('RemoveDirFromPath: CurrentPath = "' + CurrentPath + '"');
  Log('RemoveDirFromPath: PosInPath = ' + IntToStr(PosInPath));
  if PosInPath > 0 then
    begin
    NewPath := ';' + CurrentPath + ';';
    StringChangeEx(NewPath, ';' + Directory + ';', ';', True);

    // Strip leading and trailing ';'s
    NewPath := Copy(NewPath, 2, Length(NewPath)-2);
    Log('RemoveDirFromPath: NewPath = "' + NewPath + '"');

    if RegWriteExpandStringValue(HKEY_LOCAL_MACHINE,
        'SYSTEM\CurrentControlSet\Control\Session Manager\Environment',
        'Path', NewPath) then
      begin
      Log('RemoveDirFromPath: Updated system PATH environment variable');
      BroadcastEnvironmentChange();
      end
    else
      begin
      Log('RemoveDirFromPath: ERROR occurred when updating system PATH environment variable');
      end;
    end;
end;

// ----------------------------------------------------------------------------

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  SiblingVersionsLeft: Integer;
  LandisRootDir: String;
  MajorVersionDir: String;
  ExtToolScript: String;
begin
  if CurUninstallStep = usPostUninstall then
    begin
    if CountVersions(SiblingVersionsLeft) = 0 then
      begin
      LandisRootDir := ExpandConstant('{app}');
      Log('No LANDIS-II versions remain installed, so deleting ' + LandisRootDir + '\ ...');
      if not DelTree(LandisRootDir, True, True, True) then
        begin
        Log('Error: unable to delete everything in ' + LandisRootDir + '\');
        end;
      RemoveDirFromPath(LandisRootDir + '\bin');
      end
    else if SiblingVersionsLeft = 0 then
      begin
      LandisRootDir := ExpandConstant('{app}');
      MajorVersionDir := LandisRootDir + '\v' + MajorVersion;
      Log('No LANDIS-II ' + MajorVersion + '._ versions remain installed, so deleting ' + MajorVersionDir + '\ ...');
      if not DelTree(MajorVersionDir, True, True, True) then
        begin
        Log('Error: unable to delete everything in ' + MajorVersionDir + '\');
        end;

      ExtToolScript := LandisRootDir + '\bin\landis-v' + MajorVersion + '-extensions.cmd';
      if FileExists(ExtToolScript) then
        begin
        if DeleteFile(ExtToolScript) then
          Log('Deleted file: ' + ExtToolScript)
        else
          Log('Error: unable to delete file: ' + ExtToolScript);
        end;
      end; // if SiblingVersionsLeft = 0
    end; // if CurUninstallStep = usPostUninstall
end;

