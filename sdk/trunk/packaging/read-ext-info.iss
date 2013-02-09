; Copyright 2013 Green Code LLC
;
; Licensed under the Apache License, Version 2.0 (the "License");
; you may not use this file except in compliance with the License.
; You may obtain a copy of the License at
;
;     http://www.apache.org/licenses/LICENSE-2.0
;
; Unless required by applicable law or agreed to in writing, software
; distributed under the License is distributed on an "AS IS" BASIS,
; WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
; See the License for the specific language governing permissions and
; limitations under the License.
;
; Contributors:
;   James Domingo, Green Code
;-----------------------------------------------------------------------------

; Read a LANDIS-II extension info file specified by ExtInfoFile variable

#ifndef ExtInfoFile
  #error  The variable ExtInfoFile is not defined
#endif

#if ! FileExists(ExtInfoFile)
  #pragma error 'The variable ExtInfoFile refers to a non-existant file "' + Str(ExtInfoFile) + '"'
#endif

#pragma message 'Reading extension info from "' + ExtInfoFile + '" ..'

#define FileHandle FileOpen(ExtInfoFile)
#if ! FileHandle
  #error  Cannot open the extension info file
#endif

#define FileLine
#define LineNumber 0

#sub ReadFileLine
  #if FileEof(FileHandle)
    #undef public FileLine
  #else
    #define public FileLine FileRead(FileHandle)
    #expr LineNumber++
  #endif
#endsub

; Determine if FileLine is a data line (non-blank).  If so, set DataLine.
#sub CheckIfDataLine
  #if Trim(FileLine) != ""
    #define public DataLine FileLine
  #endif
#endsub

; Get next data line from the extension info file
#sub ReadDataLine
  #undef public DataLine
  #for { ReadFileLine; Defined(FileLine) && !Defined(DataLine); !Defined(DataLine) ? ReadFileLine : ""} CheckIfDataLine
#endsub

; Read the next input variable from the extension info file.
; Each input variable is on its own line, has the format:
;
;   variable-name  value
#sub ReadInputVariable
  #expr ReadDataLine
  #if Defined(DataLine)
    #define public InputVariableName FirstWord(DataLine)
    #define        RestOfLine Copy(Trim(DataLine), Len(InputVariableName)+1)
    #define public InputVariableValue TrimQuotes(Trim(RestOfLine))
  #else
    #undef public InputVariableName
    #undef public InputVariableValue
  #endif
#endsub

; Construct an error message for a particular location in the extension info
; file.  If the location is not specified, then the current line number is used.
#define InfoFileMessage(Str Text, Str Location = "") \
  Local[0] = Location == "" ? "line " + Str(LineNumber) : Location , \
  "At " + Local[0] + ' of the file "' + ExtInfoFile + '", ' + Text

#sub ReadInputVarIfNotDefined
  #ifndef InputVariableName
    #expr ReadInputVariable
  #endif
#endsub

#define public ExpectedName
#define InputVariableValue

#sub MissingName
  #pragma error InfoFileMessage('did not find the variable ' + ExpectedName, 'the end')
#endsub

#sub ClearVariableName
  #undef public InputVariableName
#endsub

#sub WrongName
  #pragma error InfoFileMessage('found the ' + InputVariableName + ' variable instead of the ' + ExpectedName + ' variable')
#endsub

; Read a required input variable from the extension info file.
#define ReadRequiredVar(str VarName) \
  ReadInputVarIfNotDefined, ExpectedName=VarName, \
  !Defined(InputVariableName) \
    ? (MissingName, "") \
    : (InputVariableName == VarName ? (ClearVariableName, InputVariableValue) \
                                    : (WrongName, "") )

#define LandisData ReadRequiredVar("LandisData")
#if LandisData != "Extension"
  #pragma error InfoFileMessage('LandisData is "' + LandisData + '", but should be "Extension"')
#endif

#define public ExtensionName ReadRequiredVar("Name")
#pragma message 'ExtensionName = "' + ExtensionName + '"'

; Read an optional variable from the extension info file.
#define ReadOptionalVar(str VarName) \
  ReadInputVarIfNotDefined, \
  !Defined(InputVariableName) \
    ? False \
    : (InputVariableName == VarName ? (ClearVariableName, True) \
                                    : False )

#if ReadOptionalVar("Version")
  #define public ExtensionVersion InputVariableValue
  #pragma message 'ExtensionVersion = "' + ExtensionVersion + '"'
#endif

#if ReadOptionalVar("Type")
  #define public ExtensionType InputVariableValue
  #pragma message 'ExtensionType = "' + ExtensionType + '"'
#endif

#if ReadOptionalVar("Assembly")
  #define public ExtensionAssembly InputVariableValue
  #pragma message 'ExtensionAssembly = "' + ExtensionAssembly + '"'
#endif

#if ReadOptionalVar("Class")
  #define public ExtensionMainClass InputVariableValue
  #pragma message 'ExtensionMainClass = "' + ExtensionMainClass + '"'
#endif

#if ReadOptionalVar("Description")
  #define public ExtensionDescription InputVariableValue
  #pragma message 'ExtensionDescription = "' + ExtensionDescription + '"'
#endif

#if ReadOptionalVar("UserGuide")
  #define public UserGuide InputVariableValue
  #pragma message 'UserGuide = "' + UserGuide + '"'
#endif

#if ReadOptionalVar("CoreVersion")
  #define public CoreVersion InputVariableValue
  #pragma message 'CoreVersion = "' + CoreVersion + '"'
#endif

#if defined(InputVariableName)
  #pragma warning InfoFileMessage('Unknown variable name: ' + InputVariableName)
#endif

#expr FileClose(FileHandle)
