; Parse a version number in the variable Version

#sub ParseVersion
  #define Pos1stDot Pos(".", Version)
  #if Pos1stDot == 0
    #error Version doesn't have proper format: "{major}.{minor}" or "{major}.{minor}.{patch}"
  #endif
  #define public MajorVersion Copy(Version, 1, Pos1stDot-1)
  #if MajorVersion == ""
    #error Missing major version number in Version variable
  #endif
  #define TextAfter1stDot Copy(Version, Pos1stDot+1)
  #if TextAfter1stDot == ""
    #error Missing minor version number in Version variable
  #endif
  #define Pos2ndDot Pos(".", TextAfter1stDot)
  #if Pos2ndDot == 0
    #define public MinorVersion TextAfter1stDot
    #define public PatchLevel ""
  #else
    #define public MinorVersion Copy(TextAfter1stDot, 1, Pos2ndDot-1)
    #if MinorVersion == ""
      #error Missing minor version number in Version variable
    #endif
    #define public PatchLevel Copy(TextAfter1stDot, Pos2ndDot+1)
    #if PatchLevel == ""
      #error Missing patch level after 2nd period in Version variable
    #endif
  #endif
  #define public MajorMinor MajorVersion + "." + MinorVersion
#endsub

;-----------------------------------------------------------------------------
; Parse release info in the variable ReleaseInfo
;
; Valid formats: "alpha release #" or "alpha # release"
;                "beta release #"  or "beta # release"
;                "release candidate #"
;                "official release"
;
; Outputs:  ReleaseType = "alpha", "beta", "candidate" or "official"
;           ReleaseNumber = defined as integer if ReleaseType != "official"

#sub ParseReleaseInfo
  ; Remove "release" keyword so release info reduced to "alpha #", "beta #",
  ; "candidate #", or "official"
  #define ReleaseInfo Trim( StringChange(ReleaseInfo, 'release', '') )
  #if ReleaseInfo == ""
    #pragma error "Missing release type (alpha, beta, candidate, official)"
  #endif
  #define public ReleaseType FirstWord(ReleaseInfo)
  #if Pos("|"+ReleaseType+"|", "|alpha|beta|candidate|official|") == 0
    #pragma error 'Unknown release type: "' + ReleaseType + '"'
  #endif
  #pragma message 'ReleaseType = "' + ReleaseType + '"'

  ; Get release number for non-official releases
  #if ReleaseType != "official"
    #define ReleaseNumber Trim( StringChange(ReleaseInfo, ReleaseType, '') )
    #if ReleaseNumber == ""
      #pragma error "Missing release number"
    #endif
    #if IsInteger(ReleaseNumber, SignNotAllowed)
      #define public ReleaseNumber Int(ReleaseNumber)
    #else
      #pragma error 'Invalid release number: "' + '"'
    #endif
    #if ReleaseNumber == 0
      #pragma error 'Release number is 0'
    #endif
    #pragma message 'ReleaseNumber = ' + Str(ReleaseNumber)
  #endif
#endsub

;-----------------------------------------------------------------------------
; Set preprocessor variables with release info
;
; Inputs:  ReleaseType
;          ReleaseNumber (only if ReleaseType != "official")
;
; Outputs:  ReleaseAbbr
;           ReleaseSuffix
;           ReleaseForAppName
;           ReleaseForAppVerName
;           ReleaseAsInt

#sub SetReleaseVars
#if ReleaseType == "official"
  #define public ReleaseAbbr ""
  #define public ReleaseSuffix ""
  #define public ReleaseForAppName ""
  #define public ReleaseForAppVerName ""
  #define public ReleaseAsInt 400
#else
  #if ReleaseType == "alpha"
    #define public ReleaseTypeAbbr "a"
    #define public ReleaseForAppVerName " (alpha " + Str(ReleaseNumber) + " release)"
    #define ReleaseOffset 100
  #elif ReleaseType == "beta"
    #define public ReleaseTypeAbbr "b"
    #define public ReleaseForAppVerName " (beta " + Str(ReleaseNumber) + " release)"
    #define ReleaseOffset 200
  #elif ReleaseType == "candidate"
    #define public ReleaseTypeAbbr "rc"
    #define public ReleaseForAppVerName " (release candidate " + Str(ReleaseNumber) + ")"
    #define ReleaseOffset 300
  #else
    #error Invalid ReleaseType
  #endif

  #define public ReleaseAbbr ReleaseTypeAbbr + Str(ReleaseNumber)
  #define public ReleaseSuffix "-" + ReleaseAbbr
  #define public ReleaseForAppName " (" + ReleaseAbbr + ")"
  #define public ReleaseAsInt ReleaseOffset + Int(ReleaseNumber)
#endif
#endsub