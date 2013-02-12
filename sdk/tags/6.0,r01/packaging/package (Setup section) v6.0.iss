#ifndef PackageNameLong
  #define PackageNameLong PackageName
#endif

#ifdef Version
  #define Pos1stDot Pos(".", Version)
  #if Pos1stDot == 0
    #error Version doesn't have proper format: "{major}.{minor}" or "{major}.{minor}.{patch}"
  #endif
  #define MajorVersion Copy(Version, 1, Pos1stDot-1)
  #if MajorVersion == ""
    #error Missing major version number in Version variable
  #endif
  #define TextAfter1stDot Copy(Version, Pos1stDot+1)
  #if TextAfter1stDot == ""
    #error Missing minor version number in Version variable
  #endif
  #define Pos2ndDot Pos(".", TextAfter1stDot)
  #if Pos2ndDot == 0
    #define MinorVersion TextAfter1stDot
    #define PatchLevel ""
  #else
    #define MinorVersion Copy(TextAfter1stDot, 1, Pos2ndDot-1)
    #if MinorVersion == ""
      #error Missing minor version number in Version variable
    #endif
    #define PatchLevel Copy(TextAfter1stDot, Pos2ndDot+1)
    #if PatchLevel == ""
      #error Missing patch level after 2nd period in Version variable
    #endif
  #endif
  #define MajorMinor MajorVersion + "." + MinorVersion
#else
  #ifndef MajorMinor
    #error The pre-processor variable "MajorMinor" is not defined
  #endif
  #ifdef PatchLevel
    #define Version MajorMinor + "." + PatchLevel
  #else
    #define Version MajorMinor
  #endif
#endif

#if ReleaseType == "official"
  #define ReleaseAbbr ""
  #define ReleaseSuffix ""
  #define ReleaseForAppName ""
  #define ReleaseForAppVerName ""
  #define ReleaseAsInt 400
#else
  #if ReleaseType == "alpha"
    #define ReleaseTypeAbbr "a"
    #define ReleaseForAppVerName " (alpha " + Str(ReleaseNumber) + " release)"
    #define ReleaseOffset 100
  #elif ReleaseType == "beta"
    #define ReleaseTypeAbbr "b"
    #define ReleaseForAppVerName " (beta " + Str(ReleaseNumber) + " release)"
    #define ReleaseOffset 200
  #elif ReleaseType == "candidate"
    #define ReleaseTypeAbbr "rc"
    #define ReleaseForAppVerName " (release candidate " + Str(ReleaseNumber) + ")"
    #define ReleaseOffset 300
  #else
    #error Invalid ReleaseType
  #endif

  #define ReleaseAbbr ReleaseTypeAbbr + Str(ReleaseNumber)
  #define ReleaseSuffix "-" + ReleaseAbbr
  #define ReleaseForAppName " (" + ReleaseAbbr + ")"
  #define ReleaseAsInt ReleaseOffset + Int(ReleaseNumber)
#endif

#define VersionRelease     Version + ReleaseSuffix

#define LandisDeployDir    GetEnv("LANDIS_DEPLOY")
#define LandisInstallDir   "C:\Program Files\LANDIS-II"
#define LandisBinDir       LandisInstallDir + "\bin"
#define LandisPlugInDir    LandisInstallDir + "\plug-ins"

; The directory where LANDIS-II code is built for deployment
#define LandisBuildDir     "C:\Program Files\LANDIS-II\6.0\bin"


#if CoreReleaseAbbr == ""
  #define CoreReleaseSuffix ""
#else
  #define CoreReleaseSuffix "-" + CoreReleaseAbbr
#endif
#define CoreVersionRelease CoreVersion + CoreReleaseSuffix
#define CoreDir            LandisInstallDir + "\" + CoreVersionRelease
#define CoreBinDir         CoreDir + "\bin"

[Setup]
AppName=LANDIS-II {#PackageName} v{#Version}{#ReleaseForAppName}
AppVerName=LANDIS-II {#PackageNameLong} v{#Version}{#ReleaseForAppVerName}
AppPublisher=Portland State University
; DefaultDirName={pf}\LANDIS-II\{#CoreVersionRelease}
DefaultDirName={#LandisInstallDir}\{#CoreVersionRelease}
UsePreviousAppDir=no
DefaultGroupName=LANDIS-II\{#CoreVersionRelease}
UsePreviousGroup=no

OutputDir={#SourcePath}
OutputBaseFilename=LANDIS-II {#PackageName} {#VersionRelease}-setup
VersionInfoCompany=Portland State University
#if PatchLevel == ""
VersionInfoVersion={#MajorMinor}.0.{#ReleaseAsInt}
#else
VersionInfoVersion={#MajorMinor}.{#PatchLevel}.{#ReleaseAsInt}
#endif

LicenseFile={#LandisDeployDir}\..\licenses\LANDIS-II_Binary_license.rtf
