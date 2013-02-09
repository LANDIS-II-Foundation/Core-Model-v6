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

; Functions for strings

;-----------------------------------------------------------------------------
; Does a string contain all whitespace characters?
; NOTE: Empty string returns False

#define IsWhitespace(str S) \
  Len(S) > 0 && Trim(S) == "" ? True : False

;-----------------------------------------------------------------------------
; Trim leading whitespace from a string.

#define LeftTrim(str S) \
  Len(S) == 0 \
    ? "" \
    : ( !IsWhitespace(Copy(S,1,1)) ? S : LeftTrim(Copy(S,2)) )

;-----------------------------------------------------------------------------
; Get the position of the first whitespace in a string.
; Returns 0 if the string has no whitespace.

#define Pos1stWhitespace(Str S) \
  Len(S) == 0 \
    ? 0 \
    : ( IsWhitespace(Copy(S,1,1)) \
          ? 1 \
          : ( Local[0]=Pos1stWhitespace(Copy(S,2)), (Local[0] > 0 ? Local[0] + 1 : 0) \
            ) \
      )

;-----------------------------------------------------------------------------
; Get first word from a string.
; A word is a sequence of one or more non-whitespace characters.  Whitespace
; before the first word is skipped.  If string doesn't contains a word, "" is
; returned.

#define FirstWord(str S) \
  S = LeftTrim(S), \
  Len(S) == 0 \
    ? "" \
    : ( Local[0]=Pos1stWhitespace(S), Local[0] > 0 ? Copy(S,1,Local[0]-1) : S ) 

;-----------------------------------------------------------------------------
; Trim leading and trailing double quote characters from a string.

#define TrimQuotes(str S) \
  Local[0] = Copy(S,1,1) == '"' ? 2 : 1, \
  Local[1] = Copy(S, Len(S)) == '"' ? Len(S)-1 : Len(S), \
  Copy(S, Local[0], Local[1] - Local[0] + 1)
  
;-----------------------------------------------------------------------------
; Is a character a digit '0' .. '9'?
; Note: If the argument has more than 1 character, only the first character is
; checked.

#define IsDigit(Str S) \
  Len(S) == 0 ? False : ( Pos(Copy(S,1,1), '0123456789') > 0 )

;-----------------------------------------------------------------------------
; Get the digits at the start of a string

#define GetDigits(Str S) \
  Len(S) == 0 \
    ? "" \
    : ( IsDigit(S) ? Copy(S,1,1) + GetDigits(Copy(S,2)) \
                   : "" \
      )

;-----------------------------------------------------------------------------
; Is a string an integer?

#define SignRequired   'required'
#define SignOptional   'optional'
#define SignNotAllowed 'not allowed'

#define IsInteger(Str S, Str SignSetting) \
  Len(S) == 0 \
    ? False \
    : ( Local[0] = Pos(Copy(S,1,1), "+-"), \
        Local[0] && Len(S) == 1 \
          ? False \
          : ( Local[0] && SignSetting == SignNotAllowed \
                ? False \
                : ( !Local[0] && SignSetting == SignRequired \
                      ? False \
                      : ( !Local[0] && !IsDigit(S) \
                            ? False \
                            : GetDigits(Copy(S,2)) == Copy(S,2) \
                        ) \
                  ) \
            ) \
      )

