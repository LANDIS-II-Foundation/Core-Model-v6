#define WindowsDeployDir ExtractFilePath(SourcePath)
#define DeployDir        ExtractFilePath(WindowsDeployDir)
#define SolutionDir      ExtractFilePath(DeployDir)

#define BuildDir         SolutionDir + "\build"
#define ReleaseConfigDir BuildDir + "\Release"

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


#define PkgWindowsFiles    SourcePath
#define PkgHomeDir		   "J:\Scheller\LANDIS-II\GoogleCodeExtensions\core-install-library\trunk\"
#define PkgDocDir          PkgHomeDir + "docs"
#define PkgSoftwareDir     PkgHomeDir + "software"

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
OutputDir={#PkgWindowsFiles}

;OutputBaseFilename=LANDIS-II-{#VersionRelease}-setup
OutputBaseFilename=LANDIS-II-6.0-setup
LicenseFile={#PkgDocDir}\LANDIS-II_Binary_license.rtf


[Files]

; Core framework
Source: Landis.Core.dll; DestDir: {app}\bin; Flags: replacesameversion
Source: Landis.Core.Implementation.dll; DestDir: {app}\bin; Flags: replacesameversion
Source: Landis.SpatialModeling.dll; DestDir: {app}\bin; Flags: replacesameversion
Source: Landis.SpatialModeling.CoreServices.dll; DestDir: {app}\bin; Flags: replacesameversion

; Libraries
Source: Landis.Library.AgeOnlyCohorts.dll; DestDir: {app}\bin; Flags: replacesameversion
Source: Landis.Library.Cohorts.dll; DestDir: {app}\bin; Flags: replacesameversion
Source: Landis.Library.Succession.dll; DestDir: {app}\bin; Flags: replacesameversion
Source: log4net.dll; DestDir: {app}\bin; Flags: replacesameversion
Source: Troschuetz.Random.dll; DestDir: {app}\bin; Flags: replacesameversion
Source: Edu.Wisc.Forest.Flel.Util.dll; DestDir: {app}\bin; Flags: replacesameversion
Source: gdal_csharp.dll; DestDir: {app}\bin; Flags: replacesameversion
Source: gdal_wrap.dll; DestDir: {app}\bin; Flags: replacesameversion

; Console interface
Source: Landis.Console.exe; DestDir: {app}\bin; Flags: replacesameversion
Source: Landis.Console.exe.config; DestDir: {app}\bin; Flags: replacesameversion

; Command scripts that call console interface
Source: {#PkgWindowsFiles}\landis-{#VersionRelease}.cmd; DestDir: {#LandisBinDir}
Source: {#PkgWindowsFiles}\landis-ii.cmd; DestDir: {#LandisBinDir}; Flags: uninsneveruninstall
Source: {#PkgWindowsFiles}\landis.cmd; DestDir: {#LandisBinDir}; Flags: uninsneveruninstall

; Auxiliary tool for identifying versions of LANDIS-II installed
Source: {#PkgSoftwareDir}\Landis.Versions.exe; DestDir: {#LandisBinDir}; Flags: uninsneveruninstall
Source: Edu.Wisc.Forest.Flel.Util.dll; DestDir: {#LandisBinDir}; Flags: uninsneveruninstall

; Auxiliary tool for administering plug-ins
Source: Landis.PlugIns.Admin.exe; DestDir: {app}\bin; Flags: replacesameversion
Source: Landis.PlugIns.Admin.exe.config; DestDir: {app}\bin; Flags: replacesameversion
Source: {#PkgWindowsFiles}\landis-{#VersionRelease}-extensions.cmd; DestDir: {#LandisBinDir}
Source: {#PkgWindowsFiles}\landis-extensions.cmd; DestDir: {#LandisBinDir}; Flags: uninsneveruninstall

; Documentation
Source: {#PkgDocDir}\LANDIS-II Model v6.0 Description.pdf; DestDir: {app}\docs
Source: {#PkgDocDir}\LANDIS-II Model v6.0 User Guide.pdf; DestDir: {app}\docs

; No example input files but a read me.
Source: {#PkgDocDir}\READ ME.TXT; DestDir: {app}\examples

; Auxillary 3-rd party files.
Source: {#PkgWindowsFiles}\3rd-party\*; DestDir: {#LandisInstallDir}\bin
Source: {#PkgWindowsFiles}\gdal-files\*; DestDir: {app}\bin

; Script for uninstalling a LANDIS-II release
#define UninstallReleaseScript "uninstall-landis-release.cmd"
Source: {#PkgWindowsFiles}\{#UninstallReleaseScript}; DestDir: {#LandisBinDir}; Flags: uninsneveruninstall


[Icons]
Name: {group}\Documentation; Filename: {app}\docs
Name: {group}\Sample Input Files; Filename: {app}\examples
Name: {group}\Uninstall; Filename: {uninstallexe}

[Run]
; Add the LANDIS-II bin directory to the PATH environment variable
Filename: {#LandisBinDir}\envinst.exe; Parameters: "-silent -broadcast -addval -name=PATH -value=""{#LandisInstallDir}\bin"" -append"

; Run tool to determine newest version of LANDIS-II installed
Filename: {#LandisBinDir}\Landis.Versions.exe; Parameters: store-newest

[UninstallRun]
Filename: {#LandisBinDir}\{#UninstallReleaseScript}; Parameters: {#VersionRelease}

; Remove the LANDIS-II bin directory to the PATH environment variable
;DISABLED THE LINE BELOW BECAUSE IT SHOULD ONLY REMOVE THE BIN DIR FROM PATH IF THERE ARE NO VERSIONS INSTALLED
;Filename: {pf}\LANDIS-II\bin\envinst.exe; Parameters: "-silent -broadcast -eraseval -name=PATH -value=""{pf}\LANDIS-II\bin"""

;; EVENTUALLY TO-DO:
;; Run the versions tool after this version's command script removed.
;; If no versions installed, i.e. landis-newest.txt doesn't exist, then
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
