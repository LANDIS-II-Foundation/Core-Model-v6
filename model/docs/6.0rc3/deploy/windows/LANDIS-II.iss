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
#define VersionRelease       Version + ReleaseAbbr
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
ChangesEnvironment=yes
SetupLogging=yes
LicenseFile={#DocDir}\LANDIS-II_Binary_license.rtf

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


[Icons]
Name: {group}\Documentation;                         Filename: {app}\v{#Major}\docs
Name: {group}\Sample Input Files;                    Filename: {app}\v{#Major}\examples
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

function NextButtonClick(CurPageID: Integer): Boolean;
var
  LandisBinDir : String;
  MajorMinorScript : String;
begin
  if CurPageId = wpSelectDir then
    begin
    LandisBinDir := ExpandConstant('{app}\bin');
    MajorMinorScript := LandisBinDir + '\landis-' + MajorMinor + '.cmd';
    if FileExists(MajorMinorScript) then
      begin
      MsgBox('LANDIS-II ' + MajorMinor + ' is already installed in this directory.', mbError, MB_OK);
      Result := False;
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

