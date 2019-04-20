'	BankCheck, a program to check bank and page selection in PIC assembly
'	AsmLine class, represents a single line in a program
'	Copyright (C) 2014 Hugh Considine
'
'	This program is free software; you can redistribute it and/or modify
'	it under the terms of the GNU General Public License as published by
'	the Free Software Foundation; either version 2 of the License, or
'	(at your option) any later version.
'
'	This program is distributed in the hope that it will be useful,
'	but WITHOUT ANY WARRANTY; without even the implied warranty of
'	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'	GNU General Public License for more details.
'
'	You should have received a copy of the GNU General Public License
'	along with this program; if not, write to the Free Software
'	Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
'
' Created by SharpDevelop.
' User: Hugh
' Date: 24/04/2014
' Time: 8:22 PM

Public Class AsmLine
	
	Public Enum LineTypes
		COMMENT
		DIRECTIVE
		INST
		LABEL
	End Enum
	
	Public ReadOnly Shared Dim Instructions() As String = { _
		"addwf", "addwfc", "andwf", "asrf", "lslf", "lsrf", "clrf", "clrw", "comf", "decf", "incf", "iorwf", "movf", "movwf", "rlf", "rrf", "subwf", "subwfb", "swapf", "xorwf", "decfsz", "incfsz", _
		"bcf", "bsf", "btfsc", "btfss", _
		"addlw", "andlw", "iorlw", "movlb", "movlp", "movlw", "sublw", "xorlw", _
		"bra", "brw", "call", "callw", "goto", "retfie", "retlw", "return", _
		"clrwdt", "nop", "option", "reset", "sleep", "tris", "addfsr", "moviw", "movwi", _
		"banksel", "bankisel", "pagesel" _
	}
	
	'Things passed in to constructor - original line, source file and line, parent program
	Private Dim pOriginal As String
	Private Dim pSourceFile As String
	Private Dim pFileLine As Integer
	Private Dim Program As AsmProgram
	
	'Values calculated for line
	Private Dim pLocation As Integer 'Location in memory
	Public Dim PrevLines As List(Of AsmLine) 'Line/s executed before this one
	Public Dim NextLines As List(Of AsmLine) 'Line/s executed after this one
	Private Dim pType As LineTypes 'Type of line (operation, directive, etc)
	Private Dim pTidied As String 'Original line, in lower case, trimmed, tabs as spaces
	Private Dim banksel, pagesel As Boolean
	Private Dim pBankOrPage As Integer
	
	Private Dim pRequiredBank, pRequiredPage As Integer
	Public Dim PossiblePages, PossibleBanks As List(Of Integer)
	
	'Store split tidied line
	Private Dim Elements() As String
	
	Public Sub New(InLine As String, sourceFile As String, fileLine As Integer, Program As AsmProgram)
		Dim instSplitChars() As Char = {" "c, ","c}
		
		pFileLine = fileLine
		pSourceFile = sourceFile
		Me.Program = Program
		
		pOriginal = InLine
		pTidied = pOriginal.ToLower.Replace(vbTab, " ").Trim
		PrevLines = New List(Of AsmLine)
		NextLines = New List(Of AsmLine)
		
		banksel = False
		pagesel = False
		pBankOrPage = -1
		
		pRequiredBank = -2
		pRequiredPage = -2
		PossibleBanks = New List(Of Integer)
		PossiblePages = New List(Of Integer)
		
		'Comment?
		If pTidied = "" Or pTidied.StartsWith(";") Then
			pType = LineTypes.COMMENT
			
		'Include?
		Else If pTidied.StartsWith("#") Then
			pType = LineTypes.DIRECTIVE
		
		Else
			'Instruction?
			Dim inst As String
			For Each inst In Instructions
				If pTidied = inst Or pTidied.StartsWith(inst + " ") Then
					'Remove comment from tidied
					If pTidied.Contains(";") Then
						pTidied = pTidied.Substring(0, pTidied.IndexOf(";"))
					End If
					Elements = pTidied.Split(instSplitChars, StringSplitOptions.RemoveEmptyEntries)
					
					If inst = "pagesel" Then
						pagesel = True
					Else If inst = "banksel" Then
						banksel = True
					End If
					
					pType = LineTypes.INST
					Goto LineTypeFound
				End If
			Next
			
			'Directive or label
			pType = LineTypes.DIRECTIVE
			
		End If
		LineTypeFound:
		
	End Sub
	
	'Get code size of this line
	Public ReadOnly Property Size As Integer
		Get
			
			If pTidied.StartsWith("banksel ") Or pTidied.StartsWith("pagesel ") Then
				If Program.EnhancedChip Then
					Return 1
				Else
					Return 2
				End If
			Else If pTidied.StartsWith("bankisel ") Then
				Return 1
				
			Else If pType = LineTypes.INST Then
				Return 1
			Else
				Return 0
			End If
			
		End Get
	End Property
	
	Public ReadOnly Property LineType As LineTypes
		Get
			Return pType
		End Get
	End Property
	
	Public ReadOnly Property Tidied As String
		Get
			Return pTidied
		End Get
	End Property
	
	Public ReadOnly Property SourceFile As String
		Get
			Return pSourceFile
		End Get
	End Property
	
	Public ReadOnly Property FileLine As Integer
		Get
			Return pFileLine
		End Get
	End Property
	
	Public ReadOnly Property Original As String
		Get
			Return pOriginal
		End Get
	End Property
	
	Public Function GetElement(ElementNo As Integer) As String
		If ElementNo >= Elements.Length Then
			Return ""
		Else
			Return Elements(ElementNo)
		End If
	End Function
	
	Public Property Location As Integer
		Get
			'If ORG directive, return that location
			If pType = LineTypes.DIRECTIVE And pTidied.StartsWith("org ") Then
				Return AsmProgram.MakeDec(pTidied.Substring(4))
			Else
				Return pLocation
			End If
		End Get
		Set
			pLocation = Value
			'Location 0 gets implicit bank and page selection
			If pLocation = 0 And pType = LineTypes.INST Then
				banksel = True
				pagesel = True
				pBankOrPage = 0
			End If
		End Set
	End Property
	
	Public Overrides Function ToString As String
		Return pSourceFile + "(" + pFileLine.ToString + "):" + pOriginal
	End Function
	
	Public Function IsBanksel As Boolean
		Return banksel
	End Function
	
	Public Function IsPagesel As Boolean
		Return pagesel
	End Function
	
	Public Property BankOrPage As Integer
		Get
			Return pBankOrPage
		End Get
		Set
			If pBankOrPage = -1 Then
				pBankOrPage = Value
			End If
		End Set
	End Property
	
	'True if this line will or may skip next line
	Public Function IsSkip As Boolean
		If pType <> LineTypes.INST Then
			Return False
		End If
		
		Select Case Elements(0)
			Case "btfsc", "btfss", "decfsz", "incfsz": Return True
			Case Else: Return False
		End Select
	End Function
	
	'True if this line returns
	Public Function IsReturn As Boolean
		If pType <> LineTypes.INST Then
			Return False
		End If
		
		Select Case Elements(0)
			Case "return", "retfie", "retlw": Return True
			Case Else: Return False
		End Select
	End Function
	
	'True if this line branches
	Public Function IsGoto As Boolean
		If pType <> LineTypes.INST Then
			Return False
		End If
		
		Select Case Elements(0)
			Case "brw": Return True
			Case "bra", "goto"
				If pTidied.Replace(" ", "").Contains("$+1") Then
					Return False
				Else If pTidied.Contains("$") Then
					Return False
				Else
					Return True
				End If
			Case Else: Return False
		End Select
	End Function
	
	'True if this line calls, and a return is expected
	Public Function IsCall As Boolean
		If pType <> LineTypes.INST Then
			Return False
		End If
		
		Select Case Elements(0)
			Case "call", "callw": Return True
			Case Else: Return False
		End Select
	End Function
	
	'True if this line will or might run next line
	Public Function IsSimple As Boolean
		If pType <> LineTypes.INST Then
			Return False
		End If
		
		Select Case Elements(0)
			Case "brw", "return", "retfie", "retlw", "reset": Return False
			Case "call", "callw": Return False
			Case "bra", "goto":
				If pTidied.Replace(" ", "").Contains("$+1") Then
					Return True
				Else
					Return False
				End If
			Case Else: Return True
		End Select
	End Function
	
	'Return destination for goto or call
	Public Function GetDest As String
		If IsCall Or IsGoto Then
			Dim Dest As String = ""
			If Elements.Length >= 2 Then
				Dest = Elements(1)
			End If
			Return Dest
		Else
			Return ""
		End If
		
	End Function
	
	Public Function RequiredBank As Integer
		If pType <> LineTypes.INST Then
			Return -1
		End If
		
		'Is bank known?
		Dim UsedLoc, BankLoc As Integer
		If pRequiredBank = -2 Then
			Select Case Elements(0)
				Case "addwf", "addwfc", "andwf", "asrf", "lslf", "lsrf", "clrf", _
					 "comf", "decf", "incf", "iorwf", "movf", "movwf", "rlf", "rrf", _
					 "subwf", "subwfb", "swapf", "xorwf", "decfsz", "incfsz", _
					 "bcf", "bsf", "btfsc", "btfss"
					UsedLoc = Program.GetSymbolValue(Elements(1))
					'Shared bank?
					BankLoc = UsedLoc And 127
					If IsCommonLoc(BankLoc) Then
						pRequiredBank = -1
					Else
						pRequiredBank = UsedLoc \ 128
					End If
				Case Else
					pRequiredBank = -1
			End Select
		End If
		
		Return pRequiredBank
	End Function
	
	Private Function IsCommonLoc(Location As Integer) As Boolean
		'RAM
		Select Case Program.ChipModel
			Case "12f629", "12f675", "16f630"
				If Location >= &h20 And Location <= &h5F Then Return True
			Case Else
				If Location >= 112 Then Return True
		End Select
		
		'Common registers
		If Program.EnhancedChip Then
			If Location < 12 Then Return True
		Else
			Select Case Location
				Case 0, 2, 3, 4, 10, 11: Return True
				Case Else: Return False
			End Select
		End If
		
		Return False
	End Function
	
	Public Function RequiredPage As Integer
		If pType <> LineTypes.INST Then
			Return -1
		End If
		
		'Is page known?
		Dim UsedLoc As Integer
		If pRequiredPage = -2 Then
			If Elements(0) = "call" Or (Elements(0) = "goto" And IsGoto) Then
				UsedLoc = Program.GetSymbolValue(Elements(1))
				pRequiredPage = UsedLoc \ 2048
			Else
				pRequiredPage = -1
			End If
		End If
		
		Return pRequiredPage
	End Function
	
End Class
