'
' Created by SharpDevelop.
' User: Administrator
' Date: 6/01/2007
' Time: 12:51 AM
'
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Windows.Forms
Imports Microsoft.VisualBasic
Imports System.Collections.Generic

''Namespace Great_Cow_Graphical_BASIC

	Public Class LowLevel
	
		Public Sub New()
		End Sub
		
		Public Shared Function GetIndent(ByVal DataTemp As String) As Integer
			
			If DataTemp Is Nothing Then Return 0
			
			'If line is a comment, don't change indent
			If DataTemp.StartsWith("'") Then Return 0
			
			Dim CommandName As String = DataTemp.ToLower.Trim + " "
			Dim IsMatch As Integer = 0
			Dim IndexTemp As Integer
			Dim SyntaxTemp As String
			
			'Get Command Pointer
			Dim CommandPointer As Integer = -1
			For IndexTemp = 0 To MainForm.CommandList.Count - 1
				With MainForm.CommandList(IndexTemp)
					SyntaxTemp = .Syntax
					If SyntaxTemp <> "" Then
						SyntaxTemp = SyntaxTemp.ToLower
						If SyntaxTemp.IndexOf("%") <> -1 Then SyntaxTemp = SyntaxTemp.ToLower.Substring(0, SyntaxTemp.IndexOf("%")).Trim + " "
						'Command matches searched command
						If CommandName.Length >= SyntaxTemp.Length Then
							If CommandName.Substring(0, SyntaxTemp.Length).ToLower = SyntaxTemp Then
								'Command is only allowed in a block, drop it back a level
								If .OnlyIn <> "" And .OnlyIn.Trim.ToLower <> "sub" Then
									Return -2
								End If
								CommandPointer = IndexTemp
								Exit For
							End If
						End If
						'Match matches searched command
						If .MatchSyntax <> "" Then
							If CommandName.IndexOf(.MatchSyntax.ToLower) <> -1 Then IsMatch = -1: CommandPointer = IndexTemp: Exit For
						End If
					End If
				End With
			Next
				
			'Get Indent
			If CommandPointer <> -1 And IsMatch <> -1 Then
				If MainForm.CommandList(CommandPointer).MatchSyntax <> "" Then IsMatch = 1
			End If
			
			Return IsMatch
			
		End Function
		
		Public Shared Function GetIndentSpaces(Level As Integer) As String
			Dim OutVal As String = ""
			Dim Temp As Integer
			
			For Temp = 1 To Level
				OutVal += Chr(9)
			Next
			
			Return OutVal
		End Function
		
		Public Shared Function GetSubName(SubNameIn As String) As String
			'Returns sub name if it is a sub, or a blank string otherwise
			Dim SubNameOut As String
			
			'If it's not a sub, return nothing
			If Not SubNameIn.ToLower.StartsWith("sub ") Then
				Return ""
			End If
			
			'If it is a sub, get name with no params
			SubNameOut = SubNameIn.Substring(4).Trim
			If SubNameOut.IndexOf(" ") <> -1 Then
				SubNameOut = SubNameOut.Substring(0, SubNameOut.IndexOf(" ")).Trim
			End If
			Return SubNameOut
			
		End Function
		
		Public Shared Function IsLet(ByVal InLine As String) As Boolean
			Dim ProcessTemp As String
			Dim EqLoc, SpaceCount, Temp As Integer
			
			If InLine.StartsWith("'") Or InLine.StartsWith(";") Then Return False
			
			If InLine.IndexOf("=") <> -1 Then
	     		ProcessTemp = InLine.Trim.Replace("=", "~!~").Replace("~!~", " = ").Replace("  ", " ")
	     		EqLoc = ProcessTemp.IndexOf("=") - 1
	     		SpaceCount = 0
	     		For Temp = EqLoc To 0 Step -1
	     			If ProcessTemp.Substring(Temp, 1) = " " Then SpaceCount += 1
	     		Next
	     		If SpaceCount = 1 Then Return True
	     	End If
	     	
	     	Return False
		End Function
		
		Public Shared Function IsIOPinName(PinName As String) As Boolean
			'Checks if PinName is that of an IO pin.
			'Format: [RP][A-Z][0-7] Or GP[0-7]
			If PinName.Length <> 3 Then Return False
			PinName = PinName.ToUpper
			
			If "RP".IndexOf(PinName.Substring(0, 1)) <> -1 Then
				If PinName.Substring(1, 1) < "A" Or PinName.Substring(1, 1) > "Z" Then Return False
				If "01234567".IndexOf(PinName.Substring(2, 1)) = -1 Then Return False
			Else
				If Not PinName.StartsWith("GP") Then Return False
				If "01234567".IndexOf(PinName.Substring(2, 1)) = -1 Then Return False
			End If
			
			Return True
		End Function
		
		'Adjust input lines to make them easier to deal with
		Public Shared Function FixLine(ByVal TempData As String) As String
			
	     	'Add "LET"
	     	If LowLevel.IsLet(TempData) Then TempData = "Let " + TempData.Trim
	     	
	     	'Change Wait to WaitFor
	     	If TempData.ToLower.StartsWith("wait until ") Or TempData.ToLower.StartsWith("wait while ")
	     		TempData = "WaitFor " + TempData.Substring(4).Trim
	     	End If
	     	
	     	'Change Do to Do Forever
	     	If TempData.ToLower = "do" Then
	     		TempData = "Do Forever"
	     	End If
	     	
	     	'Change +=, -= to long form
	     	Dim CCVar As String
	     	Dim CCParam As String
	     	If TempData.IndexOf("+=") <> -1 Then
	     		CCVar = TempData.Substring(0, TempData.IndexOf("+=")).Trim
	     		CCParam = TempData.Substring(TempData.IndexOf("+=") + 2).Trim
	     		TempData = "LET " + CCVar + " = " + CCVar + " + " + CCParam
	     	End If
	     	If TempData.IndexOf("-=") <> -1 Then
	     		CCVar = TempData.Substring(0, TempData.IndexOf("-=")).Trim
	     		CCParam = TempData.Substring(TempData.IndexOf("-=") + 2).Trim
	     		TempData = "LET " + CCVar + " = " + CCVar + " - " + CCParam
	     	End If
	     	If TempData.IndexOf("*=") <> -1 Then
	     		CCVar = TempData.Substring(0, TempData.IndexOf("*=")).Trim
	     		CCParam = TempData.Substring(TempData.IndexOf("*=") + 2).Trim
	     		TempData = "LET " + CCVar + " = " + CCVar + " * " + CCParam
	     	End If
	     	If TempData.IndexOf("/=") <> -1 Then
	     		CCVar = TempData.Substring(0, TempData.IndexOf("/=")).Trim
	     		CCParam = TempData.Substring(TempData.IndexOf("/=") + 2).Trim
	     		TempData = "LET " + CCVar + " = " + CCVar + " / " + CCParam
	     	End If
	     	
			Return TempData
		End Function
		
		'Split up an input line
		Public Shared Function GetCommands(InLine As String) As List(Of String)
			Dim OutList As New List(Of String)
			Dim CurrCharNo As Integer
			Dim InQuote, InBinConst As Boolean
			Dim TempBuffer, CurrChar As String
			Dim AfterThen As String
			
			'Change tabs to spaces
			InLine = InLine.Replace(Convert.ToChar(9), " ").Trim
				     	
			'Split single line IFs
     		If Not InLine.StartsWith("'") and InLine.ToLower.IndexOf(" then ") <> -1 Then
     			AfterThen = InLine.Substring(InLine.ToLower.IndexOf(" then ") + 6).Trim
     			If Not AfterThen.StartsWith("'") Then
     				Replace(InLine, " then ", " then: ")
     				If InLine.IndexOf("'") <> -1 Then InLine = InLine.Substring(0, InLine.IndexOf("'"))
     				InLine = InLine + ": end if"
     			End If
     			
     		End If
	     	
     		'Replace equivalents
     		If InLine.ToUpper.StartsWith("REM ") Then InLine = "'" + InLine.Substring(4)
			
			'Need to split at newline, and when not in quote : and '
			TempBuffer = ""
			InQuote = False
			InBinConst = False
			For CurrCharNo = 0 To InLine.Length - 1
				CurrChar = InLine.Substring(CurrCharNo, 1)
				'CR or LF
				If CurrChar = Convert.ToChar(10) Or CurrChar = Convert.ToChar(13) Then
					InQuote = False
					If TempBuffer <> "" Then
						OutList.Add(FixLine(TempBuffer))
						TempBuffer = ""
					End If
				
				'"
				ElseIf CurrChar = """" Then
					InQuote = Not InQuote
					TempBuffer += CurrChar
					
				'In quotes, anything goes
				ElseIf InQuote Then
					TempBuffer += CurrChar
					
				'binary constant
				ElseIf CurrChar = "'" And TempBuffer.ToLower.EndsWith("b") And InLine.IndexOf("'", CurrCharNo + 1) <> -1 Then
					InBinConst = True
					TempBuffer += CurrChar
					
				ElseIf InBinConst And CurrChar = "'" Then
					InBinConst = False
					TempBuffer += CurrChar
					
				'Not in quotes, so split at : and '
				Else
					If CurrChar = ":" And CurrCharNo <> InLine.Length - 1 Then
						If TempBuffer <> "" Then
							OutList.Add(FixLine(TempBuffer))
							TempBuffer = ""
						End If
					ElseIf CurrChar = "'" Then
						If TempBuffer <> "" Then
							OutList.Add(FixLine(TempBuffer))
						End If
						TempBuffer = InLine.Substring(CurrCharNo)
						Exit For
						
					'Other char
					Else
						'Don't add double spaces
						If CurrChar <> " " Or Not TempBuffer.EndsWith(" ") Then
							TempBuffer += CurrChar
						End If
					End If
				End If
			Next
			
			'Add anything left in buffer
			If TempBuffer <> "" Then
				OutList.Add(FixLine(TempBuffer))
			End If
			
			Return OutList
		End Function
		
		Public Shared Function IsBitConst(ByVal InValue As String) As Boolean
			Dim Temp As Integer
			
			If InValue = "" Then Return False
			If InValue.IndexOf(".") = -1 Then Return False
			InValue = InValue.Substring(0, InValue.IndexOf(".")).Trim
			
			Try
				Temp = InValue
			Catch
				Return True
			End Try
			
			Return False
		End Function
		
		Public Shared Function GetDecValue(InChar As Char) As Integer
			If InChar >= "0" And InChar <= "9" Then Return Asc(InChar) - Asc("0")
			If InChar >= "A" And InChar <= "Z" Then Return Asc(InChar) - Asc("A") + 10
			If InChar >= "a" And InChar <= "z" Then Return Asc(InChar) - Asc("a") + 10
			Return -1
		End Function
		
		Public Shared Function GetDigitValue(InChar As Integer) As Char
			If InChar >= 0 And InChar <= 9 Then Return Chr(InChar + Asc("0"))
			If InChar >= 10 And InChar <= 36 Then Return Chr(InChar + Asc("A") - 10)
			Return ""
		End Function
		
		Public Shared Function CanEditParams(ByVal InParams As String) As Boolean
			
			If InParams.Trim = "" Then Return False
			Dim CurrentParam As String
			For Each CurrentParam In InParams.Split("|")
				CurrentParam = CurrentParam.Trim.ToLower
				If CurrentParam.StartsWith("cgr_") Then Return False
				If CurrentParam.StartsWith("table") Then Return False
				If CurrentParam.StartsWith("label") Then Return False
				If CurrentParam.StartsWith("free") Then Return False
				
			Next
			
			Return True
		End Function
		
		Public Shared Function CanTypeParams(ByVal InParams As String) As Boolean
			If InParams.Trim = "" Then Return True
			Dim CurrentParam As String
			For Each CurrentParam In InParams.Split("|")
				CurrentParam = CurrentParam.Trim.ToLower
				If CurrentParam.StartsWith("cgr_") Then Return False
				If CurrentParam.StartsWith("table") Then Return False
				If CurrentParam.StartsWith("label") Then Return False
				If CurrentParam.StartsWith("free") Then Return True
			Next
			
			Return True
		End Function
		
		Public Shared Function CastOrder (InType As String) As Integer
			Select Case InType.Trim.ToUpper
				Case "BIT": Return 0
				Case "BYTE": Return 1
				Case "WORD": Return 2
				Case "INTEGER": Return 3
				Case "LONG": Return 4
				Case "FLOAT": Return 5
				Case "STRING": Return 6
				
				Case Else: Return -1
			End Select
		End Function
		
		Public Shared Function CanCopy (SourceIn As String, DestIn As String) As Boolean
			'Return true if value of type Source can be copied to destination of type Dest
			
			'Dest type may contain |
			'If so, split at |, and use recursion
			'If any of the dest types is suitable, copy is possible
			If DestIn.IndexOf("|") <> -1 Then
				Dim DestTypes() As String = DestIn.Split("|")
				Dim ThisType As String
				For Each ThisType In DestTypes
					If CanCopy(SourceIn, ThisType) Then Return True
				Next
				Return False
			End If
			
			Dim Source, Dest As String
			Source = SourceIn.Trim.ToLower
			Dest = DestIn.Trim.ToLower
			
			If Dest.StartsWith("const") Then Return False
			
			If Source.EndsWith("byte") Then Source = "byte"
			If Source.EndsWith("word") Then Source = "word"
			If Source = "io_pin" Then Source = "bit"
			If Source = "io_port" Then Source = "byte"
			If Dest = "io_pin" Then Dest = "bit"
			If Dest = "io_port" Then Dest = "byte"
			
			If CastOrder(Source) <> -1 And CastOrder(Dest) <> -1 Then
				If CastOrder(Source) <= CastOrder(Dest) Then
					Return True
				Else
					Return False
				End If
			End If
			
			Select Case Dest
				Case "var", "number", "free":
					Return True
				Case "condition":
					Select Case Source
						Case "bit", "byte", "word", "const": Return True
						Case Else: Return False
					End Select
					
				Case Else:
					Return False
			End Select
			
		End Function
		
		Public Shared Function Dec2Other(ByVal InVal As Integer, ByVal InBase As Integer) As String
			Dim OutVal As String
			Dim Temp, Place, CurVal, CurPlace As Integer
			OutVal = ""
			
			If InBase = 1 Then
				For Temp = 1 To InVal
					OutVal += "1"
				Next
				
			Else
				Place = 0
				Do While InBase ^ Place <= InVal
					Place += 1
				Loop
				Place -= 1
				
				For CurPlace = Place To 0 Step -1
					CurVal = InVal \ InBase ^ CurPlace
					InVal = InVal Mod InBase ^ CurPlace
					OutVal = OutVal + GetDigitValue(CurVal)
				Next
			End If
			Return OutVal
			
		End Function
		
		Public Shared Function Other2Dec(ByVal InVal As String, ByVal InBase As Integer, Optional StopOnInvalid As Boolean = False) As Integer
			Dim OutVal, Temp, AddVal As Integer
			
			OutVal = 0
			For Temp = 0 To InVal.Length - 1
				AddVal = GetDecValue(InVal.Substring(Temp, 1))
				'Is character invalid?
				If AddVal = -1 And StopOnInvalid Then
					Return OutVal
				End If
				'Check if a character is too big for the current base
				If (AddVal >= InBase And InBase > 1) Or (AddVal <> 1 And InBase = 1) Or AddVal = -1 Then
					MessageBox.Show(InVal.Substring(Temp, 1) + " is not a valid digit for base-" + InBase.ToString + " numbers", "Great Cow Graphical BASIC", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
					Return -99999
				End If
				
				OutVal = OutVal * InBase + AddVal
			Next
			
			Return OutVal
		End Function
		
		Public Shared Function CheckName(ByVal InName As String) As String
			'Check if InName is valid
			'If it contains invalid characters, return them
			Dim BadChars As String = ""
			
			'Must start with a letter
			Dim Temp As Integer
			Try
				Temp = InName.Substring(0, 1)
				'Shouldn't get to this line:
				BadChars += InName.Substring(0, 1)
			Catch
			End Try
			
			'Can't contain any of these symbols
			If InName.IndexOf(" ") <> -1 Then BadChars += " "
			If InName.IndexOf("(") <> -1 Then BadChars += "("
			If InName.IndexOf(")") <> -1 Then BadChars += ")"
			If InName.IndexOf("*") <> -1 Then BadChars += "*"
			If InName.IndexOf("/") <> -1 Then BadChars += "/"
			If InName.IndexOf("%") <> -1 Then BadChars += "%"
			If InName.IndexOf("+") <> -1 Then BadChars += "+"
			If InName.IndexOf("-") <> -1 Then BadChars += "-"
			If InName.IndexOf("&") <> -1 Then BadChars += "&"
			If InName.IndexOf("|") <> -1 Then BadChars += "|"
			If InName.IndexOf("#") <> -1 Then BadChars += "#"
			If InName.IndexOf("!") <> -1 Then BadChars += "!"
			If InName.IndexOf("=") <> -1 Then BadChars += "="
			If InName.IndexOf("<") <> -1 Then BadChars += "<"
			If InName.IndexOf(">") <> -1 Then BadChars += ">"
			If InName.IndexOf(":") <> -1 Then BadChars += ":"
			If InName.IndexOf(";") <> -1 Then BadChars += ";"
			If InName.IndexOf(".") <> -1 Then BadChars += "."
			If InName.IndexOf(",") <> -1 Then BadChars += ","
			If InName.IndexOf("$") <> -1 Then BadChars += "$"
			If InName.IndexOf("@") <> -1 Then BadChars += "@"
			If InName.IndexOf("'") <> -1 Then BadChars += "'"
			If InName.IndexOf(" ") <> -1 Then BadChars += " "
			
			Return BadChars
		End Function
		
		Public Shared Function IsValue(ByVal InData As String) As Boolean
			'Check if a string is a number by attempting to convert to integer
			Dim Temp As Integer
			Try
				Temp = InData.Trim
			Catch
				'If it can't be converted from decimal, check if it's hex or binary
				If InData.Length > 1 Then
					If InData.Substring(0, 2).ToLower = "b'" Then Return True
					If InData.Substring(0, 2).ToLower = "0x" Then Return True
				End If
				
				Return False
			End Try
			
			Return True
		End Function
		
		Public Shared Function MakeDec (Temp As String) As Integer
	
			Dim DataSource As String
			Dim T As Integer
			
			DataSource = Temp.Trim.ToUpper
			
			If DataSource.IndexOf("[BYTE]") <> -1 Then Replace(DataSource, "[BYTE]", "")
			If DataSource.IndexOf("[WORD]") <> -1 Then Replace(DataSource, "[WORD]", "")
			If DataSource.IndexOf("[INTEGER]") <> -1 Then Replace(DataSource, "[INTEGER]", "")
			If DataSource.IndexOf("[SINGLE]") <> -1 Then Replace(DataSource, "[SINGLE]", "")
			
			'Hex
			IF DataSource.IndexOf("0X") <> -1 THEN
				DataSource = DataSource.Substring(DataSource.IndexOf("0X") + 2)
				Return Other2Dec(DataSource, 16, True)
			END IF
			
			'Decimal already
			Try
				T = DataSource.Trim
				Return T
			Catch
			End Try
			
			IF DataSource.Length = 1 Then Return Convert.ToInt32(DataSource)
			
			IF DataSource.IndexOf("B'") <> -1 THEN
				DataSource = DataSource.Substring(DataSource.IndexOf("B'") + 2)
				If DataSource.IndexOf("'") <> -1 Then DataSource = DataSource.Substring(0, DataSource.IndexOf("'"))
				Return Other2Dec(DataSource, 2, True)
			END If
			
		End Function
		
		Public Shared Sub Replace(ByRef InString As String, ByVal SearchString As String, ByVal ReplaceString As String)
			Dim StartPos, EndPos As Integer
			Dim SaveLeft, SaveRight As String
			SearchString = SearchString.ToLower
			
			ReplaceAgain:
			
			if InString.ToLower.IndexOf(SearchString) = -1 Then Exit Sub
			
			StartPos = InString.ToLower.IndexOf(SearchString)
			EndPos = SearchString.Length
			
			SaveLeft = InString.Substring(0, StartPos)
			SaveRight = InString.Substring(StartPos + EndPos)
			
			InString = SaveLeft + ReplaceString + SaveRight
			
			Goto ReplaceAgain
			
		End Sub
		
		Public Shared Function IsDivider (Temp As String) As Boolean
			
			Select Case Temp
			Case " ", "(", ")", ",", ".", ":", ";", "+", "-", "*", "/", "%": Return True
			Case "=", "!", "<", ">", "{", "}", "~", "&", "|", "#": Return True
			Case Else: Return False
			End Select
			
		End Function
		
		Public Shared Sub WholeReplace (ByRef DataVar As String, Find As String, Rep As String)
			
			Dim VarTemp, FindTemp, Temp, NewData As String
			Dim T, RepAll As Integer
			
			RepAll = -1
			If Left(Find, 3) = "[1]" Then
				RepAll = 0
				Find = Mid(Find, 4)
			End If
			
			VARTemp = DataVar.ToUpper: FINDTemp = Find.ToUpper
			IF VARTemp.IndexOf(FINDTemp) = -1 THEN EXIT SUB
			IF VARTemp = FINDTemp THEN DataVar = Rep: EXIT SUB
			If FindTemp = Rep.ToUpper Then Exit Sub
			
			Do
				IF DataVar.ToUpper.IndexOf(FINDTemp) = -1 THEN Exit Do
				
				'Check for occurance of whole string
				'T: 0 not found, 1 left matches, 2 right matches (all found), 3 = found but not whole
				T = 0
				'Check start
				IF INSTR(DataVar.ToUpper, FINDTemp) = 1 THEN T = 1
				IF T = 0 THEN
					Temp = Mid(DataVar, INSTR(DataVar.ToUpper, FINDTemp) - 1, 1)
					IF IsDivider(Temp) THEN T = 1
				END IF
				'Check end
				IF INSTR(DataVar.ToUpper, FINDTemp) + LEN(FINDTemp) - 1 = LEN(DataVar) AND T = 1 THEN T = 2
				IF T = 1 THEN
					Temp = Mid(DataVar, INSTR(DataVar.ToUpper, FINDTemp) + LEN(FINDTemp), 1)
					IF IsDivider(Temp) THEN T = 2
				END IF
				'If search string present but part of another word, T = 3
				IF T = 0 AND INSTR(DataVar.ToUpper, FINDTemp) <> 0 THEN T = 3
				
				'Search string not found at all, clean up and exit
				IF T = 0 Then Exit Do
				
				'If search string found but part of another word, convert to Chr(26)
				If T = 1 OR T = 3 THEN
					Replace(DataVar, FINDTemp, Chr(26))
				
				'If whole string found, replace
				ElseIf T = 2 THEN
					NewData = Left(DataVar, INSTR(DataVar.ToUpper, FINDTemp) - 1)
					NewData = NewData + Rep
					DataVar = NewData + Mid(DataVar, INSTR(DataVar.ToUpper, FINDTemp) + LEN(Find))
					'If only replacing single occurance, exit
					If Not RepAll Then Exit Do
				END IF
				
			Loop
			
			'Tidy and exit
			Do While InStr(DataVar, Chr(26)) <> 0
				Replace(DataVar, Chr(26), Find)
			Loop
			
			Exit Sub
			
		End Sub
		
		Public Shared Function AddFullPath(CurrPath As String, FullPath As String) As String
			'Will return a fully qualified path from CurrPath.
			'FullPathIn is that path that should be added to CurrPath if not FQ
			
			Dim OutPath As String
			OutPath = CurrPath
			
			If FullPath = "" Then
				Return OutPath
			End If
			
			'Remove trailing slash from full path
			If FullPath.EndsWith("/") Or FullPath.EndsWith("\") Then FullPath = FullPath.Substring(0, FullPath.Length - 1)
			'If CurrPath not FQ, add full path
			If OutPath.Substring(1, 1) <> ":" And Not OutPath.StartsWith("\\") Then
				OutPath = FullPath + Mainform.FilePathSplitter + CurrPath
			End If
			
			Return OutPath
		End Function
		
		Declare Unicode Function GetShortPathName Lib "kernel32.dll" Alias "GetShortPathNameW" (ByVal lpszLongPath As String, ByVal lpszShortPath As String, ByVal cchBuffer As Integer) As Integer
		Public Shared Function ShortFileName(LongName As String) As String
			Dim OutputName As String = Space(100)
			Dim TempData As Integer = GetShortPathName(LongName, OutputName, 100)
			Return OutputName.Substring(0, TempData)
		End Function
		
		Public Shared Function SplitToFit(CurrLine As String, labelFont As System.Drawing.Font, g As System.Drawing.Graphics, width As Integer) As String
			'Splits a string so that it will fit into a given width, using the specified font.
			
			'Split CurrLine into lines
			Dim IconLines As New List(Of String)(CurrLine.Split(Environment.NewLine))
			Dim IconLine As String
			Dim OutLine As String
			Dim splitPos, firstSplitPos, currIconLine As Integer
			Dim CanSplit As Boolean
			
			'Don't even try with really short spaces
			If width < 10 Then Return ""
			
			'Split lines that are too long
			currIconLine = 0
			Do While currIconLine < IconLines.Count
				IconLine = IconLines(currIconLine)
				CanSplit = True
				
				'Is line too long?
				If g.MeasureString(IconLine, labelFont).Width > width - 6 Then
					firstSplitPos = IconLine.Length
					'Scan back until line is short enough
					Do While g.MeasureString(IconLine.Substring(0, firstSplitPos), labelFont).Width > width - 6
						firstSplitPos -= 1
						If firstSplitPos < 0 Then
							firstSplitPos = 2
							Exit Do
						End If
					Loop
					'Scan back to dividing character
					splitPos = firstSplitPos - 1
					Do While splitPos > 0
						If LowLevel.IsDivider(IconLine.Substring(splitPos, 1)) Then
							Exit Do
						End If
						splitPos -= 1
					Loop
					If splitPos = 0 Then
						splitPos = firstSplitPos - 1
					End If
					
					If splitPos < 1 Then
						CanSplit = False
					End If
					
					'Have found place to split, split line
					If CanSplit Then
						iconLines.RemoveAt(currIconLine)
						iconLines.Insert(currIconLine, iconLine.substring(0, splitPos).Trim)
						iconLines.Insert(currIconLine + 1, iconLine.substring(splitPos).Trim)
					End If
				End If
				
				currIconLine += 1
			Loop
											
			'Rejoin CurrLine
			OutLine = ""
			For Each IconLine In IconLines
				If OutLine = "" Then
					OutLine = IconLine
				Else
					OutLine = OutLine + Environment.NewLine + IconLine
				End If
			Next
			
			Return OutLine
		End Function
		
		Public Shared Function ReplaceToolVariables(InData As String, Optional FNExtension As String = "", Optional FileNameIn As String = "", Optional CurrentProgram As GCBProgram = Nothing) As String
			
			Dim FileName, FileNameNoExt, OutData As String
			Dim PD As Integer
			OutData = InData
			
			FileName = FileNameIn
			
			'Get filename without extension
			FileNameNoExt = ""
			For PD = FileName.Length To 1 Step -1
				If FileName.Substring(PD - 1, 1) = "." Then
					FileNameNoExt = FileName.Substring(0, PD - 1)
					Exit For
				End If
			Next
			'If requested, change filename extension
			If FNExtension <> "" Then
				FileName = FileNameNoExt + "." + FNExtension
			End If
			
			'Items typically found in parameters
			Do While OutData.ToUpper.IndexOf("%FILENAME%") <> -1: Replace (OutData, "%FILENAME%", FileName): Loop
			Do While OutData.ToUpper.IndexOf("%SHORTNAME%") <> -1: Replace (OutData, "%SHORTNAME%", ShortFileName(FileName)): Loop
			Do While OutData.ToUpper.IndexOf("%FN_NOEXT%") <> -1: Replace (OutData, "%FN_NOEXT%", FileNameNoExt): Loop
			If Not CurrentProgram Is Nothing Then
				Do While OutData.ToUpper.IndexOf("%CHIPMODEL%") <> -1: Replace (OutData, "%CHIPMODEL%", CurrentProgram.ChipName): Loop
			End If
			
			'Items typically found in paths
			Do While OutData.ToUpper.IndexOf("%APPDATA%") <> -1: Replace (OutData, "%appdata%", Environ("APPDATA")): Loop
			Do While OutData.ToUpper.IndexOf("%ALLUSERSAPPDATA%") <> -1: Replace (OutData, "%allusersappdata%", Environ("ALLUSERSPROFILE") + "\Application Data"): Loop
			Do While OutData.ToUpper.IndexOf("%TEMP%") <> -1: Replace (OutData, "%temp%", Environ("TEMP")): Loop
			Do While OutData.ToUpper.IndexOf("%INSTDIR%") <> -1: Replace (OutData, "%instdir%", MainForm.InstallDir): Loop
			
			Return OutData
		End Function
		
	End Class
''End Namespace
