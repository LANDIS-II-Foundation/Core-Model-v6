#ifndef PackageNameLong
  #define PackageNameLong PackageName
#endif

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

