; #include "build\settings.iss"


#define Version    "5.1"
#define MajorMinor "5.1"

#define Release       "official"
#define ReleaseType   "official"
#define ReleaseNumber "2"
#define ReleaseAbbr   ""
#define ReleaseFull   "official release"

#define VersionRelease     Version + ReleaseAbbr
#define VersionReleaseFull Version + " (" + ReleaseFull + ")"

#define PkgWindowsFiles    SourcePath
#define PkgCommonFiles     ExtractFilePath(PkgWindowsFiles)
#define PkgHomeDir         ExtractFilePath(PkgCommonFiles)
#define PkgDocDir          PkgHomeDir + "\doc"

#define LandisSDK          "J:\Landis-II\SDK"
; #define LandisSDK		   GetEnv("LANDIS_SDK")
; #define MyDocs             GetEnv("USERPROFILE") + "\My Documents"

#define LandisInstallDir   "C:\Program Files\LANDIS-II"
#define LandisBinDir       LandisInstallDir + "\bin"
#define LandisPlugInDir    LandisInstallDir + "\plug-ins"

[Setup]
AppName=LANDIS-II {#VersionRelease}
AppVerName=LANDIS-II {#VersionReleaseFull}
AppPublisher=University of Wisconsin-Madison
DefaultDirName={pf}\LANDIS-II\{#MajorMinor}
UsePreviousAppDir=no
DefaultGroupName=LANDIS-II\{#MajorMinor}
UsePreviousGroup=no
; SourceDir={#MyDocs}\software\release
SourceDir=J:\Landis-II\CoreInstall\software\release
OutputDir={#PkgWindowsFiles}\build
; OutputBaseFilename=LANDIS-II-{#VersionRelease}-setup
OutputBaseFilename=LANDIS-II-5.1.2-setup
LicenseFile={#LandisSDK}\src\deployment\LANDIS-II_Binary_license.rtf


[Files]

; Core framework
Source: Landis.Core.dll; DestDir: {app}\bin
Source: Landis.Cohorts.netmodule; DestDir: {app}\bin
Source: Landis.Ecoregions.netmodule; DestDir: {app}\bin
Source: Landis.Landscape.netmodule; DestDir: {app}\bin
Source: Landis.Main.netmodule; DestDir: {app}\bin
Source: Landis.PlugIns.netmodule; DestDir: {app}\bin
Source: Landis.RasterIO.netmodule; DestDir: {app}\bin
Source: Landis.Species.netmodule; DestDir: {app}\bin
Source: Landis.Util.netmodule; DestDir: {app}\bin

; Raster drivers
Source: {#PkgCommonFiles}\raster-drivers.xml; DestDir: {app}\bin
Source: Landis.RasterIO.Drivers.Erdas74.dll; DestDir: {app}\bin

; Libraries
Source: Landis.AgeCohort.dll; DestDir: {app}\bin
Source: Landis.Succession.dll; DestDir: {app}\bin
Source: Edu.Wisc.Forest.Flel.Grids.dll; DestDir: {app}\bin
Source: Edu.Wisc.Forest.Flel.Util.dll; DestDir: {app}\bin
Source: log4net.dll; DestDir: {app}\bin

; Console interface
Source: Landis.Console.exe; DestDir: {app}\bin
Source: {#PkgCommonFiles}\Landis.Console.exe.config; DestDir: {app}\bin

; Command scripts that call console interface
Source: {#PkgWindowsFiles}\landis-{#VersionRelease}.cmd; DestDir: {#LandisBinDir}
Source: {#PkgWindowsFiles}\landis-ii.cmd; DestDir: {#LandisBinDir}; Flags: uninsneveruninstall
Source: {#PkgWindowsFiles}\landis.cmd; DestDir: {#LandisBinDir}; Flags: uninsneveruninstall

; Auxiliary tool for identifying versions of LANDIS-II installed
Source: Landis.Versions.exe; DestDir: {#LandisBinDir}; Flags: uninsneveruninstall
Source: Edu.Wisc.Forest.Flel.Util.dll; DestDir: {#LandisBinDir}; Flags: uninsneveruninstall

; Auxiliary tool for administering plug-ins
Source: Landis.PlugIns.Admin.exe; DestDir: {app}\bin
Source: {#PkgCommonFiles}\Landis.PlugIns.Admin.exe.config; DestDir: {app}\bin
Source: {#PkgWindowsFiles}\landis-{#VersionRelease}-extensions.cmd; DestDir: {#LandisBinDir}
Source: {#PkgWindowsFiles}\landis-extensions.cmd; DestDir: {#LandisBinDir}; Flags: uninsneveruninstall

; Auxiliary command script that runs at the end of the installation.
; It adds entries to the plug-ins database for the base plug-ins;
; #define FinishSetupScript "finish-setup.cmd"
; Source: {#PkgWindowsFiles}\{#FinishSetupScript}; DestDir: {app}

; Example input files
Source: {#PkgCommonFiles}\examples\*; DestDir: {app}\examples

; Documentation
Source: {#PkgDocDir}\build\*; DestDir: {app}\docs

; 3rd-party utility for setting environment variables
Source: {#PkgWindowsFiles}\3rd-party\envinst.exe; DestDir: {pf}\LANDIS-II\bin

; Script for uninstalling a LANDIS-II release
#define UninstallReleaseScript "uninstall-landis-release.cmd"
Source: {#PkgWindowsFiles}\{#UninstallReleaseScript}; DestDir: {#LandisBinDir}; Flags: uninsneveruninstall


[Icons]
Name: {group}\Documentation; Filename: {app}\docs\index.html
Name: {group}\Sample Input Files; Filename: {app}\examples
Name: {group}\Uninstall; Filename: {uninstallexe}

[Run]
; Add the LANDIS-II bin directory to the PATH environment variable
Filename: {#LandisBinDir}\envinst.exe; Parameters: "-silent -broadcast -addval -name=PATH -value=""{pf}\LANDIS-II\bin"" -append"

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
