; Define variables related to LANDIS-II.
;
; Inputs:  CoreVersion = required LANDIS-II version formatted as "#.#"

#define CoreMajorVersion GetDigits(CoreVersion)

#define LandisInstallDir   "C:\Program Files\LANDIS-II"
#define LandisBinDir       LandisInstallDir + "\bin"

#define LandisMajorVerDir  LandisInstallDir + "\v" + CoreMajorVersion
#define LandisMajorBinDir  LandisMajorVerDir + "\bin"
#define LandisExtInfoDir   LandisMajorVerDir + "\ext-info"

#define LandisExtDir       LandisMajorBinDir + "\extensions"
#define ExtAdminTool       LandisMajorBinDir + "\Landis.Extensions.exe"

; The directory where LANDIS-II code is built during development
#define LandisBuildDir     LandisMajorBinDir + "\build"

#define LandisCoreDir      LandisMajorBinDir + "\" + CoreVersion
