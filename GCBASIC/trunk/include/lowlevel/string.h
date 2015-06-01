'    String routines for the GCBASIC compiler
'    Copyright (C) 2006 - 2013 Hugh Considine

'    This library is free software; you can redistribute it and/or
'    modify it under the terms of the GNU Lesser General Public
'    License as published by the Free Software Foundation; either
'    version 2.1 of the License, or (at your option) any later version.

'    This library is distributed in the hope that it will be useful,
'    but WITHOUT ANY WARRANTY; without even the implied warranty of
'    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
'    Lesser General Public License for more details.

'    You should have received a copy of the GNU Lesser General Public
'    License along with this library; if not, write to the Free Software
'    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

'********************************************************************************
'IMPORTANT:
'THIS FILE IS ESSENTIAL FOR SOME OF THE COMMANDS IN GCBASIC. DO NOT ALTER THIS FILE
'UNLESS YOU KNOW WHAT YOU ARE DOING. CHANGING THIS FILE COULD RENDER SOME GCBASIC
'COMMANDS UNUSABLE!
'********************************************************************************

'Changes:
' 15/2/2011: First working versions of LCase, UCase, Str
' 18/2/2011: Added Val function
' 13/2/2013: Added Left, Right, Mid functions
' New functions  added Jan 2014 by 2014 Evan R Venn
' a built-in function, asc(), which takes a character and returns the numeric value for that character in the machine's character set. If the string passed to asc() has more than one character, only the first one is used.
' The inverse of this function is chr() (from the function of the same name in Pascal), which takes a number and returns the corresponding character. Both functions are written very nicely in GCB.
' 22/12/2014: Add PAD function
' 1/6/2016: Add Fill function

'Length/position
Function Len (LenTemp())
  Len = LENTemp(0)
End Function

Function Instr (SysInString(), SysFindString())
  Instr = 0

  'Check length of search and find strings
  'Exit if the find string cannot fit into the search string
  SysStrLen = SysInString(0)
  If SysStrLen = 0 Then Exit Function
  SysFindLen = SysFindString(0)
  If SysFindLen = 0 Then Exit Function
  If SysFindLen > SysStrLen Then Exit Function

  'Start searching
  SysSearchEnd = SysStrLen - SysFindLen
  For SysStringTemp = 0 to SysSearchEnd
    For SysStrData = 1 To SysFindLen
      'If a mismatch is found, search next position
      If SysInString(SysStringTemp + SysStrData) <> SysFindString(SysStrData) Then Goto SysInstrNextPos
    Next
    'If no mismatches were found, return current start position
    Instr = SysStringTemp + 1
    Exit Function
    SysInstrNextPos:
  Next

  'Have reached end of search string without finding anything, return 0
  Instr = 0
End Function

'String/number conversion
'Word > String
'(Max output will be 5 characters)
Function Str(SysValTemp As Word) As String * 5

  SysCharCount = 0
  Dim SysCalcTempX As Word

  'Ten Thousands
  IF SysValTemp >= 10000 then
    SysStrData = SysValTemp / 10000
    SysValTemp = SysCalcTempX
    SysCharCount += 1
    Str(SysCharCount) = SysStrData + 48
    Goto SysValThousands
  End If

  'Thousands
  IF SysValTemp >= 1000 then
    SysValThousands:
    SysStrData = SysValTemp / 1000
    SysValTemp = SysCalcTempX
    SysCharCount += 1
    Str(SysCharCount) = SysStrData + 48
    Goto SysValHundreds
  End If

  'Hundreds
  IF SysValTemp >= 100 then
    SysValHundreds:
    SysStrData = SysValTemp / 100
    SysValTemp = SysCalcTempX
    SysCharCount += 1
    Str(SysCharCount) = SysStrData + 48
    Goto SysValTens
  End If
  'Tens
  IF SysValTemp >= 10 Then
    SysValTens:
    SysStrData = SysValTemp / 10
    SysValTemp = SysCalcTempX
    SysCharCount += 1
    Str(SysCharCount) = SysStrData + 48
  End If

  'Ones
  SysCharCount += 1
  Str(SysCharCount) = SysValTemp + 48
  SysValTemp = SysCalcTempX

  Str(0) = SysCharCount

End Function

'String > Word
Function Val(SysInString As String) As Word
  'Parse SysInString, convert to word
  'Stop parsing and exit on any non-number character

  'Clear output value
  Val = 0

  'Get input length
  SysCharCount = SysInString(0)
  If SysCharCount = 0 Then Exit Function

  'Parse
  For SysStringTemp = 1 to SysCharCount
    SysStrData = SysInString(SysStringTemp)
    'Exit if non-digit encountered
    If SysStrData < 48 Then Exit Sub
    If SysStrData > 57 Then Exit Sub
    'Add to output value
    Val = Val * 10 + SysStrData - 48
  Next

End Function

'Decimal > Hex
Function Hex(In SysValTemp) As String * 3
  Hex(0) = 2

  'Low nibble
  SysStringTemp = SysValTemp And 0x0F
  If SysStringTemp > 9 Then SysStringTemp = SysStringTemp + 7
  Hex(2) = SysStringTemp + 48

  'Get high nibble
  For SysStringTemp = 1 to 4
    Rotate SysValTemp Right
  Next
  SysStringTemp = SysValTemp And 0x0F
  If SysStringTemp > 9 Then SysStringTemp = SysStringTemp + 7
  Hex(1) = SysStringTemp + 48

End Function

'Truncation
Function Left (SysInString(), SysCharCount) As String
  'Empty input?
  If SysInString(0) = 0 Then
    Left(0) = 0
    Exit Function
  End If

  'Input length too high?
  If SysInString(0) < SysCharCount Then
    SysCharCount = SysInString(0)
  End If

  'Copy leftmost characters
  For SysStringTemp = 1 To SysCharCount
    Left(SysStringTemp) = SysInString(SysStringTemp)
  Next
  Left(0) = SysCharCount
End Function

Function Right (SysInString(), SysCharCount) As String
  'Empty input?
  If SysInString(0) = 0 Then
    Right(0) = 0
    Exit Function
  End If

  'Input length too high?
  If SysInString(0) < SysCharCount Then
    SysCharCount = SysInString(0)
  End If

  'Copy rightmost characters
  SysCharStart = SysInString(0) - SysCharCount
  For SysStringTemp = 1 To SysCharCount
    Right(SysStringTemp) = SysInString(SysCharStart + SysStringTemp)
  Next
  Right(0) = SysCharCount
End Function

Function Mid (SysInString(), SysCharStart, Optional SysCharCount = 255) As String
  'Empty input?
  If SysInString(0) = 0 Then
    Mid(0) = 0
    Exit Function
  End If

  'Starting position too low?
  If SysCharStart < 1 Then SysCharStart = 1

  'Starting position too high?
  If SysCharStart > SysInString(0) Then
    Mid(0) = 0
    Exit Function
  End If

  'Input length too high?
  SysCharStart -= 1
  SysStringTemp = SysInString(0) - SysCharStart 'Max number of characters
  If SysCharCount > SysStringTemp Then
    SysCharCount = SysStringTemp
  End If

  'Copy characters
  For SysStringTemp = 1 To SysCharCount
    Mid(SysStringTemp) = SysInString(SysCharStart + SysStringTemp)
  Next
  Mid(0) = SysCharCount
End Function

'Trimming

Function RTrim (SysInString3 as string) As String
  'Get length of string, return empty string.  If you dont do this you will return an uninitialize value... ooops
  RTrim = ""
  SysCharRTCount = SysInString3(0)
  If SysCharRTCount = 0 Then Exit Function

  'find first non white space in the string from the Right
  SysCharRT = 0
  For SysTempRT = SysCharRTCount to 1 step -1

      ' was Asc(Mid (SysInString3, SysTempRT , 1))
      if SysInString3(SysTempRT) <> 32 then
         SysCharRT = SysTempRT
         goto FoundNonSpaceChar2
      end if
  Next

FoundNonSpaceChar2:
  if SysCharRT <> 0 then
     Rtrim = Mid (SysInString3, 1, SysCharRT )
     Exit Function
  end if


End Function


function Trim (SysInString()) As String
  trim = rtrim(ltrim(SysInString))
end function

function LTrim (SysInString2 as string) As String

  'Get length of string, return empty string.  If you dont do this you will return an uninitialize value... ooops
  Ltrim = ""
  SysCharLTCount = SysInString2(0)
  If SysCharLTCount = 0 Then Exit Function


  'find first non white space in the string
  SysCharLT = 0
  For SysTempLT = 1 to SysCharLTCount

      if Asc(Mid (SysInString2, SysTempLT , 1)) <> 32 then
         SysCharLT = SysTempLT
         goto FoundNonSpaceChar1
      end if
  Next

  FoundNonSpaceChar1:
  if SysCharLT <> 0 then
     Ltrim = Mid (SysInString2, SysCharLT )
     Exit Function
  end if

end function





'Case conversion
Function UCase (SysInString()) As String

  'Get length of string, don't try copying if it's empty
  SysCharCount = SysInString(0)
  UCase(0) = SysCharCount
  If SysCharCount = 0 Then Exit Function

  'Search string for lowercase letters, change to uppercase
  For SysStringTemp = 1 to SysCharCount
    SysStrData = SysInString(SysStringTemp)
    If SysStrData >= 97 Then
      If SysStrData <= 122 Then SysStrData -= 32
    End If
    UCase(SysStringTemp) = SysStrData
  Next
End Function

Function LCase (SysInString()) As String

  'Get length of string, don't try copying if it's empty
  SysCharCount = SysInString(0)
  LCase(0) = SysCharCount
  If SysCharCount = 0 Then Exit Function

  'Search string for uppercase letters, drop to lowercase
  For SysStringTemp = 1 to SysCharCount
    SysStrData = SysInString(SysStringTemp)
    If SysStrData >= 65 Then
      If SysStrData <= 90 Then SysStrData += 32
    End If
    LCase(SysStringTemp) = SysStrData
  Next
End Function



Function Asc (in SysInStringA(), optional Syschar = 1) As Byte
  Asc = 0
  'Get length of string, don't try _asc ing if it's empty
  SysCharCount = SysInStringA(0)
  If SysCharCount = 0 Then Exit Function

  if Syschar > SysCharCount Then Exit Function

  'Get first char of string, only first character is of interest, Syschar if provided
  Asc = SysInStringA( Syschar )

End Function

Function Chr (SysChar ) As String
  'Empty input?
  If SysChar < 0 Then
    Chr(0) = 0
    Exit Function
  End If

  Chr(0) = 1
  'Copy characters
  Chr(1) = SysChar

End Function

' Return a binary number

Function ByteToBin (In ByteNum as Byte ) as String * 8
  ByteToBin = ""
  Repeat 8
      If ByteNum.7 = 1 Then
         ByteToBin = ByteToBin +"1"
      Else
         ByteToBin = ByteToBin +"0"
      End If
      Rotate ByteNum Left
  End Repeat
End Function

Function WordToBin (In WordNum as Word ) as String * 16
  WordToBin = ""
  Repeat 16
      If WordNum.15 = 1 Then
         WordToBin = WordToBin +"1"
      Else
         WordToBin = WordToBin +"0"
      End If
      Rotate WordNum Left
  End Repeat
End Function


'PAD(str,len,padchr )
'Description  - The PAD() function pads a specified string
'Parameters
'str - String to be padded.
'len - Length of str.
'padchr - Pad character, which can be any string. The first character is used to pad new space in the output string.
Function Pad ( in SysInString as string, SysStrLen, optional SysInString3 as string = " " ) As String
  'Check length of search and find strings
  'Exit if the find string cannot fit into the search string
  ' If SysInString(0) = 0 Then Exit Function
  'Check it is not already at the correct length
    'Input length too high?
  If SysInString(0) <= SysStrLen Then
    SysCharCount = SysInString(0)
      'Copy leftmost characters
      For SysStringTemp = 1 To SysCharCount
        Pad(SysStringTemp) = SysInString(SysStringTemp)
      Next
      For SysStringTemp = SysCharCount+1 to SysStrLen
        Pad(SysStringTemp) = SysInString3(1)
      Next
  else
      'Copy leftmost characters
      For SysStringTemp = 1 To SysStrLen
        Pad(SysStringTemp) = SysInString(SysStringTemp)
      Next
  End If

  Pad(0) = SysStrLen

End Function


'Fill(len,padchr )
'Description  - The Fill() function returns a string
'Parameters
'len - Length of str.
'Fill - Pad characters, the second is used to pad new space in the output string.
Function Fill ( SysStrLen, optional SysInString3 as string = " " ) As String

      'Copy leftmost characters
      For SysStringTemp = 1 To SysStrLen
        Fill(SysStringTemp) = SysInString3(1)
      Next

  Fill(0) = SysStrLen

End Function

