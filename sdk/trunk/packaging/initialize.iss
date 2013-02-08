; Set the SDK directory based on the location of this file, which in the
; packaging subfolder (i.e., SDK_DIR\packaging\THIS_FILE) 
#define SDKpackaging  ExtractFilePath(__PATHFILENAME__)
#define LandisSDK     ExtractFilePath(SDKpackaging)

#include SDKpackaging + '\string-funcs.iss'
