; Set the SDK directory based on the location of this file, which in the
; packaging subfolder (i.e., SDK_DIR\packaging\THIS_FILE) 
#define SDKpackaging  ExtractFilePath(__PATHFILENAME__)
#define LandisSDK     ExtractFilePath(SDKpackaging)

#include SDKpackaging + '\string-funcs.iss'


;-----------------------------------------------------------------------------
; Read SDK version

#define SdkVersionPath LandisSDK + "\version.txt"

#define SdkVerFileError(Str Message, Int Line=0) \
  Message + (Line ? ' from line ' + Str(Line) + ' in': '') + ' the file "' + SdkVersionpath + '"'

#define SdkVersionFile FileOpen(SdkVersionPath)
#if !SdkVersionFile
  #pragma error SdkVerFileError('Unable to open')
#endif

#define LandisSDKversion Trim(StringChange(FileRead(SdkVersionFile), 'version', ''))
#if LandisSDKversion == ''
  #pragma error SdkVerFileError('Unable to read version', 1)
#endif

#define LandisSDKrelease Trim(StringChange(FileRead(SdkVersionFile), 'release', ''))
#if LandisSDKrelease == ''
  #pragma error SdkVerFileError('Unable to read release info', 2)
#endif

#define SdkVerFileURL Trim(StringChange(FileRead(SdkVersionFile), '$', ''))
#if SdkVerFileURL == ''
  #pragma error SdkVerFileError('Unable to read Subversion URL', 3)
#endif
#define SdkVerFileSvnPath Copy(SdkVerFileURL, Pos('/sdk/', SdkVerFileURL))
#define SdkSvnPath        StringChange(SdkVerFileSvnPath, '/version.txt', '')

#pragma message 'LANDIS-II SDK version ' + LandisSDKversion + ', release ' + LandisSDKrelease + ' (svn path: ' + SdkSvnPath + ')'
