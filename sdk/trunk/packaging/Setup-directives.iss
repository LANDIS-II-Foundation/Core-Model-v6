AppName=LANDIS-II {#ExtensionName} v{#Version}{#ReleaseForAppName}
AppVerName=LANDIS-II {#ExtensionName} Extension v{#Version}{#ReleaseForAppVerName}

AppPublisher=Portland State University
; DefaultDirName={pf}\LANDIS-II\{#CoreVersionRelease}
DefaultDirName={#LandisInstallDir}\{#CoreVersionRelease}
UsePreviousAppDir=no
DefaultGroupName=LANDIS-II\{#CoreVersionRelease}
UsePreviousGroup=no

OutputDir={#SourcePath}
OutputBaseFilename=LANDIS-II {#ExtensionName} {#VersionRelease}-setup
VersionInfoCompany=Portland State University
#if PatchLevel == ""
VersionInfoVersion={#MajorMinor}.0.{#ReleaseAsInt}
#else
VersionInfoVersion={#MajorMinor}.{#PatchLevel}.{#ReleaseAsInt}
#endif

LicenseFile={#LandisDeployDir}\..\licenses\LANDIS-II_Binary_license.rtf
