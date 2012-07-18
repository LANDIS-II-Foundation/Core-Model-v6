#define ScriptDir    ExtractFilePath(SourcePath)
#define DeployDir    ExtractFilePath(ScriptDir)
#define SolutionDir  ExtractFilePath(DeployDir)

#define ConsoleDir       SolutionDir + "\console"
#define ThirdPartyDir    SolutionDir + "\third-party"
#define LandisSpatialDir ThirdPartyDir + "\LSML"
#define GdalDir          LandisSpatialDir + "\GDAL"

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
  #define VersionRelease   Version + " (" + ReleaseAbbr + ")"
#else
  #define VersionRelease   Version
#endif
#define VersionReleaseFull Version + " (" + Release + ")"

;-----------------------------------------------------------------------------

#define DocDir             SolutionDir + "\docs"

#define LandisInstallDir   "C:\Program Files\LANDIS-II"
#define LandisBinDir       LandisInstallDir + "\bin"
#define LandisPlugInDir    LandisInstallDir + "\plug-ins"

[Setup]
AppName=LANDIS-II {#VersionRelease}
AppVerName=LANDIS-II {#VersionReleaseFull}
AppPublisher=Portland State University
DefaultDirName={#LandisInstallDir}\{#MajorMinor}
UsePreviousAppDir=no
DefaultGroupName=LANDIS-II\{#MajorMinor}
UsePreviousGroup=no
SourceDir={#ReleaseConfigDir}
OutputDir={#ScriptDir}

OutputBaseFilename=LANDIS-II-{#VersionRelease}-setup
LicenseFile={#DocDir}\LANDIS-II_Binary_license.rtf


[Files]

; Core framework
Source: Landis.Core.dll; DestDir: {app}\bin;
Source: Landis.Core.Implementation.dll; DestDir: {app}\bin;

; Libraries
Source: log4net.dll; DestDir: {app}\bin;
Source: Troschuetz.Random.dll; DestDir: {app}\bin;
Source: Edu.Wisc.Forest.Flel.Util.dll; DestDir: {app}\bin;

; LSML and GDAL
Source: Landis.SpatialModeling.dll; DestDir: {app}\bin;
Source: Landis.Landscapes.dll; DestDir: {app}\bin;
Source: Landis.RasterIO.dll; DestDir: {app}\bin;
Source: Landis.RasterIO.Gdal.dll; DestDir: {app}\bin;
Source: {#GdalDir}\gdal_csharp.dll; DestDir: {app}\bin;
Source: {#GdalDir}\native\*; DestDir: {app}\bin

; Console interface
Source: Landis.Console.exe; DestDir: {app}\bin;
Source: {#ConsoleDir}\Landis.App.config; DestDir: {app}\bin;

; Command scripts that call console interface
Source: {#ScriptDir}\landis-X.Y.cmd; DestDir: {#LandisBinDir}; DestName: landis-{#MajorMinor}.cmd
Source: {#ScriptDir}\landis-ii.cmd; DestDir: {#LandisBinDir}; Flags: uninsneveruninstall
Source: {#ScriptDir}\landis.cmd; DestDir: {#LandisBinDir}; Flags: uninsneveruninstall

; Auxiliary tool for administering extensions
Source: Landis.Extensions.Admin.exe; DestDir: {app}\bin;
Source: {#ScriptDir}\landis-X.Y-extensions.cmd; DestDir: {#LandisBinDir}; DestName: landis-{#MajorMinor}-extensions.cmd
Source: {#ScriptDir}\landis-extensions.cmd; DestDir: {#LandisBinDir}; Flags: uninsneveruninstall

; Documentation
Source: {#DocDir}\LANDIS-II Model v6.0 Description.pdf; DestDir: {app}\docs
Source: {#DocDir}\LANDIS-II Model v6.0 User Guide.pdf; DestDir: {app}\docs

; No example input files but a read me.
Source: {#DocDir}\READ ME.TXT; DestDir: {app}\examples

; Auxillary 3-rd party files.
Source: {#ScriptDir}\3rd-party\*; DestDir: {#LandisInstallDir}\bin

; Script for uninstalling a LANDIS-II release
#define UninstallReleaseScript "uninstall-landis-release.cmd"
Source: {#ScriptDir}\{#UninstallReleaseScript}; DestDir: {#LandisBinDir}; Flags: uninsneveruninstall


[Icons]
Name: {group}\Documentation; Filename: {app}\docs
Name: {group}\Sample Input Files; Filename: {app}\examples
Name: {group}\Uninstall; Filename: {uninstallexe}

[Run]
; Add the LANDIS-II bin directory to the PATH environment variable
Filename: {#LandisBinDir}\envinst.exe; Parameters: "-silent -broadcast -addval -name=PATH -value=""{#LandisInstallDir}\bin"" -append"

[UninstallRun]
Filename: {#LandisBinDir}\{#UninstallReleaseScript}; Parameters: {#VersionRelease}

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


[Code]

{-----------------------------------------------------------------------------}

function ShouldSkipPage(CurPage: Integer): Boolean;
begin
  { Skip the SelectProgramGroup page because program group fixed. }
  if CurPage = wpSelectProgramGroup then
    Result := True
  { Skip the SelectDestination Directory page because that directory is fixed. }
  else if CurPage = wpSelectDir then
    Result := True
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
