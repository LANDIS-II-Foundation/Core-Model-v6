#define ScriptDir    ExtractFilePath(SourcePath)
#define DeployDir    ExtractFilePath(ScriptDir)
#define SolutionDir  ExtractFilePath(DeployDir)

#define ConsoleDir       SolutionDir + "\console"
#define ThirdPartyDir    SolutionDir + "\third-party"
#define LandisSpatialDir ThirdPartyDir + "\LSML"
#define GdalDir          LandisSpatialDir + "\GDAL"
#define ExtToolDir       SolutionDir + "\ext-admin"
#define DocDir           SolutionDir + "\docs"

#define BuildDir         SolutionDir + "\build"
#define ReleaseConfigDir BuildDir + "\Release"

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

[Setup]
AppName=LANDIS-II {#VersionReleaseName}
AppVerName=LANDIS-II {#VersionReleaseFull}
AppPublisher=Portland State University
DefaultDirName={pf}\LANDIS-II
UsePreviousAppDir=no
DefaultGroupName=LANDIS-II\v{#Major}
UsePreviousGroup=no
SourceDir={#ReleaseConfigDir}
OutputDir={#ScriptDir}

OutputBaseFilename=LANDIS-II-{#VersionRelease}-setup
LicenseFile={#DocDir}\LANDIS-II_Binary_license.rtf


[Files]

; Core framework
Source: Landis.Core.dll;                DestDir: {app}\v{#Major}\bin\{#MajorMinor};
Source: Landis.Core.Implementation.dll; DestDir: {app}\v{#Major}\bin\{#MajorMinor};

; Libraries
Source: log4net.dll;                   DestDir: {app}\v{#Major}\bin\{#MajorMinor};
Source: Troschuetz.Random.dll;         DestDir: {app}\v{#Major}\bin\{#MajorMinor};
Source: Edu.Wisc.Forest.Flel.Util.dll; DestDir: {app}\v{#Major}\bin\{#MajorMinor};

; LSML and GDAL
Source: Landis.SpatialModeling.dll; DestDir: {app}\v{#Major}\bin\{#MajorMinor};
Source: Landis.Landscapes.dll;      DestDir: {app}\v{#Major}\bin\{#MajorMinor};
Source: Landis.RasterIO.dll;        DestDir: {app}\v{#Major}\bin\{#MajorMinor};
Source: Landis.RasterIO.Gdal.dll;   DestDir: {app}\v{#Major}\bin\{#MajorMinor};
Source: {#GdalDir}\gdal_csharp.dll; DestDir: {app}\v{#Major}\bin\{#MajorMinor};
Source: {#GdalDir}\native\*;        DestDir: {app}\v{#Major}\bin\GDAL

; Console interface
Source: Landis.Console.exe;              DestDir: {app}\v{#Major}\bin; DestName: Landis.Console-{#MajorMinor}.exe
Source: {#ConsoleDir}\Landis.App.config; DestDir: {app}\v{#Major}\bin; DestName: Landis.Console-{#MajorMinor}.exe.config

; Command scripts that call console interface
Source: {#ScriptDir}\landis-X.Y.cmd; DestDir: {app}\bin; DestName: landis-{#MajorMinor}.cmd
Source: {#ScriptDir}\landis-ii.cmd;  DestDir: {app}\bin; Flags: uninsneveruninstall
Source: {#ScriptDir}\landis.cmd;     DestDir: {app}\bin; Flags: uninsneveruninstall

; Auxiliary tool for administering extensions
Source: Landis.Extensions.exe;                  DestDir: {app}\v{#Major}\bin; DestName: Landis.Extensions-{#MajorMinor}.exe
Source: {#ExtToolDir}\App.config;               DestDir: {app}\v{#Major}\bin; DestName: Landis.Extensions-{#MajorMinor}.exe.config
Source: {#ScriptDir}\landis-X.Y-extensions.cmd; DestDir: {app}\bin;           DestName: landis-{#MajorMinor}-extensions.cmd
Source: {#ScriptDir}\landis-extensions.cmd;     DestDir: {app}\bin;           Flags: uninsneveruninstall

; The library for extension dataset is stored where the extensions are installed
Source: Landis.Extensions.Dataset.dll; DestDir: {app}\v{#Major}\bin\extensions; Flags: uninsneveruninstall

; Documentation
; (Note: Documentation among minor versions can reside in the same folder
;        because all the files have version #s in their names.)
Source: {#DocDir}\LANDIS-II Model v6.0 Description.pdf; DestDir: {app}\v{#Major}\docs
Source: {#DocDir}\LANDIS-II Model v6.0 User Guide.pdf;  DestDir: {app}\v{#Major}\docs

; No example input files but a read me.
Source: {#DocDir}\READ ME.TXT; DestDir: {app}\v{#Major}\examples

; Auxillary 3-rd party files.
Source: {#ScriptDir}\3rd-party\*; DestDir: {app}\bin

; Script for uninstalling a LANDIS-II release
#define UninstallReleaseScript "uninstall-landis-release.cmd"
Source: {#ScriptDir}\{#UninstallReleaseScript}; DestDir: {app}\bin; Flags: uninsneveruninstall


[Icons]
Name: {group}\Documentation;           Filename: {app}\v{#Major}\docs
Name: {group}\Sample Input Files;      Filename: {app}\v{#Major}\examples
Name: {group}\Uninstall {#MajorMinor}; Filename: {uninstallexe}

[Run]
; Add the LANDIS-II bin directory to the PATH environment variable
Filename: {app}\bin\envinst.exe; Parameters: "-silent -broadcast -addval -name=PATH -value=""{app}\bin"" -append"

[UninstallRun]
Filename: {app}\bin\{#UninstallReleaseScript}; Parameters: {#VersionRelease}

; Remove the LANDIS-II bin directory to the PATH environment variable
;DISABLED THE LINE BELOW BECAUSE IT SHOULD ONLY REMOVE THE BIN DIR FROM PATH IF THERE ARE NO VERSIONS INSTALLED
;Filename: {pf}\LANDIS-II\bin\envinst.exe; Parameters: "-silent -broadcast -eraseval -name=PATH -value=""{pf}\LANDIS-II\bin"""

;; EVENTUALLY TO-DO:
;; If no versions installed, i.e. no landis-#.#.cmd scripts exist, then
;; remove version-independent cmd scripts (landis.cmd, landis-ii.cmd, admin tools?)
;; if bin now empty, remove it
;; if parent dir ({pf}\LANDIS-II) empty, remove it

[UninstallDelete]
Name: {app}; Type: filesandordirs


;-----------------------------------------------------------------------------

[Code]

var
  { Was the Inno Setup script for this installer found in the same directory
    as the installer itself?  Used to determine whether to allow the user to
    select the Destination Directory. }
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
