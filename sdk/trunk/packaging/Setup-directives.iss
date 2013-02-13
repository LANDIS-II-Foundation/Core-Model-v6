; Directives for the [Setup] section of the Inno Setup script for the setup
; program (installer) for a LANDIS-II extension.

AppName=LANDIS-II {#ExtensionName} v{#Version}{#ReleaseForAppName}
AppVerName=LANDIS-II {#ExtensionName} Extension v{#Version}{#ReleaseForAppVerName}

#ifdef Organization
AppPublisher={#Organization}
#endif

DefaultDirName={#LandisInstallDir}\v{#CoreMajorVersion}
UsePreviousAppDir=no
DefaultGroupName=LANDIS-II\{#CoreVersion}
UsePreviousGroup=no

OutputDir={#SourcePath}
OutputBaseFilename=LANDIS-II {#ExtensionName} {#VersionRelease}-setup

#if PatchLevel == ""
VersionInfoVersion={#MajorMinor}.0.{#ReleaseAsInt}
#else
VersionInfoVersion={#MajorMinor}.{#PatchLevel}.{#ReleaseAsInt}
#endif

LicenseFile={#LandisDeployDir}\..\licenses\LANDIS-II_Binary_license.rtf
