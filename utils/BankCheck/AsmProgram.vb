'	BankCheck, a program to check bank and page selection in PIC assembly
'	Class to represent an assembly program
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
' Date: 26/04/2014
' Time: 2:44 AM

Imports System.IO

Public Class AsmProgram
	Public Dim Lines As List(Of AsmLine)
	Public Dim IncPath As String
	
	Public Dim SymbolTable As Dictionary(Of String, Integer)
	
	Private Dim ResetVector, IntVector As AsmLine
	
	Private Dim pChipModel As String
	Private Dim pEnhanced As Boolean
	
	Private Dim pLoaded As Boolean = False 'Set once program is loaded
	
	Public Sub New(IncPath As String, SourceFile As String)
		Me.IncPath = IncPath
		Me.pChipModel = ""
		Try
			Lines = LoadFile(SourceFile)
		Catch Ex As Exception
			Console.WriteLine("Error loading assembly file:")
			Console.WriteLine(Ex.Message)
			Console.WriteLine(Ex.StackTrace)
			Exit Sub
		End Try
		
		Dim PrevLine, CurrLine As AsmLine
		
		'Identify chip model
		For Each CurrLine In Lines
			If CurrLine.LineType = AsmLine.LineTypes.DIRECTIVE Then
				If CurrLine.Tidied.StartsWith("list ") Then
					Dim list As String = CurrLine.Tidied.Replace(" ", "")
					If list.IndexOf("p=") <> -1 Then
						pChipModel = list.Substring(list.IndexOf("p=") + 2).ToLower
						If pChipModel.IndexOf(",") <> -1 Then
							pChipModel = pChipModel.Substring(0, pChipModel.IndexOf(","))
							Exit For
						End If
					End If
				End If
			End If
		Next
		
		'Error if could not find chip model
		If pChipModel = "" Then
			Console.WriteLine("Could not identify chip model. Ensure LIST P= line is present")
			Exit Sub
		Else If (pChipModel.StartsWith("12") Or pChipModel.StartsWith("16")) And Not pChipModel.Contains("f5") Then
			pEnhanced = False
			If pChipModel.Contains("f1") Then
				pEnhanced = True
			End If
		Else
			Console.WriteLine("Chip model " + pChipModel + " not supported by this program")
			Exit Sub
		End If
		
		'Calculate destination prog memory locations for each line
		PrevLine = Nothing
		For Each CurrLine In Lines
			If Not PrevLine Is Nothing Then
				CurrLine.Location = PrevLine.Location + PrevLine.Size
			End If
			PrevLine = CurrLine
		Next
		
		'Generate symbol table
		BuildSymbolTable
		
		'Generate flow graph
		BuildFlowGraph
		
		'Find bank and page selects
		FindBankAndPageSelects
		
		pLoaded = True
		
	End Sub
	
	Private Sub BuildSymbolTable
		Dim CurrLine As AsmLine
		SymbolTable = New Dictionary(Of String, Integer)
		
		Dim NameTemp, ValTemp As String
		For Each CurrLine In Lines
			If CurrLine.LineType = AsmLine.LineTypes.DIRECTIVE Then
				'Label
				If Not CurrLine.Tidied.Contains(" ") Then
					AddSymbol(CurrLine.Tidied, CurrLine.Location)
					
				'equ directive
				Else If CurrLine.Tidied.Contains(" equ ") Then
					NameTemp = CurrLine.Tidied.Substring(0, CurrLine.Tidied.IndexOf(" equ ")).Trim
					ValTemp = CurrLine.Tidied.Substring(CurrLine.Tidied.IndexOf(" equ ") + 5).Trim
					
					AddSymbol(NameTemp, MakeDec(ValTemp))
				End If
			End If
		Next
	End Sub
	
	Private Sub AddSymbol(Name As String, Val As Integer)
		Try
			SymbolTable.Add(Name, Val)
		Catch Ex As ArgumentException
			If Val <> SymbolTable.Item(Name) Then
				Console.WriteLine("Conflicting definitions for symbol " + Name + " = " + Val.ToString)
			End If
		End Try
	End Sub
	
	Public Function GetSymbolValue(Name As String) As Integer
		Try
			Return SymbolTable.Item(Name)
		Catch Ex As KeyNotFoundException
			Console.WriteLine("Symbol " + Name + " not found")
			Return 0
		End Try
	End Function
	
	Private Sub BuildFlowGraph
		Dim CurrLine, NextLine As AsmLine
		Dim CurrPos As Integer
		For CurrPos = 0 To Lines.Count - 1
			'Decide what next command will be
			CurrLine = Lines.Item(CurrPos)
			
			If CurrLine.LineType = AsmLine.LineTypes.INST Then
				
				'Vector?
				If CurrLine.Location = 0 Then
					ResetVector = CurrLine
				Else If CurrLine.Location = 4 Then
					IntVector = CurrLine
				End If
				
				'Could have:
				' - normal instruction (run next one)
				' - skip (run instruction after next)
				' - call (run another then return back)
				' - goto (jump to another)
				If CurrLine.IsSimple Then
					CurrLine.NextLines.Add(FindLineAt(CurrLine.Location + 1))
				End If
				If CurrLine.IsSkip Then
					CurrLine.NextLines.Add(FindLineAt(CurrLine.Location + 2))
				End If
				If CurrLine.IsCall Or CurrLine.IsGoto Then
					CurrLine.NextLines.Add(FindLineAt(GetSymbolValue(CurrLine.GetDest)))
				End If
				
			End If
			
		Next
		
		'Add links for call/return
		For Each CurrLine In Lines
			If CurrLine.IsCall And CurrLine.NextLines.Count = 1 Then
				'Found call, search through next lines until a return is found
				'Add this as next line to the return
				'(Do not go into other calls)
				Dim ReturnLines As List(Of AsmLine) = FindReturns(CurrLine.NextLines.Item(0))
				Dim ReturnLine As AsmLine
				Dim LineAfterCall As AsmLine = FindLineAt(CurrLine.Location + 1)
				For Each ReturnLine In ReturnLines
					ReturnLine.NextLines.Add(LineAfterCall)
				Next
				
			End If
		Next
		
		'Add previous lines
		For Each CurrLine In Lines
			If CurrLine.LineType = AsmLine.LineTypes.INST Then
				For Each NextLine In CurrLine.NextLines
					NextLine.PrevLines.Add(CurrLine)
				Next
			End If
		Next
		
	End Sub
	
	Private Sub FindBankAndPageSelects
		Dim CurrLine As AsmLine
		Dim Dest As String
		'Find every bank and page select in program
		For Each CurrLine In Lines
			If CurrLine.IsBanksel Then
				CurrLine.BankOrPage = GetSymbolValue(CurrLine.GetElement(1)) \ 128
			Else If CurrLine.IsPagesel Then
				Dest = CurrLine.GetElement(1)
				If Dest = "$" Then
					CurrLine.BankOrPage = CurrLine.Location \ 2048
				Else
					CurrLine.BankOrPage = GetSymbolValue(Dest) \ 2048
				End If
			End If
		Next
		
		'Find selected bank and page for every line
		For Each CurrLine In Lines
			If CurrLine.IsBanksel Then
				ApplyBank(CurrLine, CurrLine.BankOrPage)
			End If
			If CurrLine.IsPagesel Then
				ApplyPage(CurrLine, CurrLine.BankOrPage)
			End If
		Next
	End Sub
	
	Private Sub ApplyBank(StartLine As AsmLine, Bank As Integer, ByRef Optional VisitedLines As List(Of AsmLine) = Nothing)
		Dim CurrLine As AsmLine
		
		If VisitedLines Is Nothing Then
			VisitedLines = New List(Of AsmLine)
		End If
		VisitedLines.Add(StartLine)
		If Not StartLine.PossibleBanks.Contains(Bank) Then
			StartLine.PossibleBanks.Add(Bank)
		End If
		
		For Each CurrLine In StartLine.NextLines
			If Not CurrLine.IsBanksel And Not VisitedLines.Contains(CurrLine) Then
				ApplyBank(CurrLine, Bank, VisitedLines)
			End If
		Next
	End Sub
	
	Private Sub ApplyPage(StartLine As AsmLine, Page As Integer, ByRef Optional VisitedLines As List(Of AsmLine) = Nothing)
		Dim CurrLine As AsmLine
		
		If VisitedLines Is Nothing Then
			VisitedLines = New List(Of AsmLine)
		End If
		VisitedLines.Add(StartLine)
		If Not StartLine.PossiblePages.Contains(Page) Then
			StartLine.PossiblePages.Add(Page)
		End If
		
		For Each CurrLine In StartLine.NextLines
			If Not CurrLine.IsPagesel And Not VisitedLines.Contains(CurrLine) Then
				ApplyPage(CurrLine, Page, VisitedLines)
			End If
		Next
	End Sub
	
	'Find line at a given memory location, or next closest
	Private Function FindLineAt(Location As Integer) As AsmLine
		Dim BestLine, CurrLine As AsmLine
		Dim BestDiff, CurrDiff As Integer
		
		BestDiff = Integer.MaxValue
		BestLine = Nothing
		For Each CurrLine In Lines
			If CurrLine.LineType = AsmLine.LineTypes.INST Then
				If CurrLine.Location = Location Then
					Return CurrLine
				Else If CurrLine.Location > Location Then
					CurrDiff = CurrLine.Location - Location
					If CurrDiff < BestDiff Then
						BestDiff = CurrDiff
						BestLine = CurrLine
					End If
				End If
			End If
		Next
		
		If BestLine Is Nothing Then
			Console.WriteLine("Error: Program ends unexpectedly at location " + Location.ToString("X"))
			Return ResetVector
		End If
		
		Return BestLine
	End Function
	
	'When passed the first line of a subroutine (ie, the destination for a call),
	'this will return a list of all return statements from that sub.
	Private Function FindReturns(StartLine As AsmLine, ByRef Optional ExploredList As List(Of AsmLine) = Nothing) As List(Of AsmLine)
		Dim OutList As New List(Of AsmLine)
		Dim NextLine As AsmLine
		
		If ExploredList Is Nothing Then
			ExploredList = New List(Of AsmLine)
		End If
		
		'This line is a return, return it and no others (empty sub)
		If StartLine.IsReturn Then
			OutList.Add(StartLine)
		
		'This line is a call, find the next location
		ElseIf StartLine.IsCall Then
			OutList.AddRange(FindReturns(FindLineAt(StartLine.Location + 1), ExploredList))
		
		'Search
		Else
			For Each NextLine In StartLine.NextLines
				If Not ExploredList.Contains(NextLine) Then
					ExploredList.Add(NextLine)
					OutList.AddRange(FindReturns(NextLine, ExploredList))
				End If
			Next
		End If
		
		Return OutList
	End Function
	
	Private Function LoadFile(SourceFile As String) As List(Of AsmLine)
		Dim OutProgLines As New List(Of AsmLine)
		Dim incTrimChars() As Char = {"<"c, ">"c, " "c, """"c}
		
		'Load assembly file
		Try
			Dim readFile As New StreamReader(SourceFile)
			Dim inLine As String
			Dim currLineNo As Integer = 0
			Dim newLine As AsmLine
			Do While Not readFile.EndOfStream
				inLine = readFile.ReadLine
				currLineNo += 1
				
				newLine = New AsmLine(inLine, SourceFile, currLineNo, Me)
				If newLine.LineType = AsmLine.LineTypes.DIRECTIVE And newLine.Tidied.StartsWith("#include") Then
					Dim incFile As String = newLine.Tidied.Substring(9).Trim(incTrimChars)
					If incFile.IndexOf("\") = -1 Then
						incFile = IncPath + incFile
					End If
					OutProgLines.AddRange(LoadFile(incFile))
				Else
					OutProgLines.Add(newLine)
				End If
			Loop
		
		Catch ex As FileNotFoundException
			Console.WriteLine("File " + SourceFile + " was not found")
			
		Catch Ex As Exception
			Console.WriteLine("Unknown error occurred loading file " + SourceFile + ":")
			Console.WriteLine(Ex.Message)
			Console.WriteLine(Ex.StackTrace)
			
		End Try
		
		Return outProgLines
	End Function
	
	Public ReadOnly Property Loaded As Boolean
		Get
			Return pLoaded
		End Get
	End Property
	
	Public ReadOnly Property ChipModel As String
		Get
			Return pChipModel
		End Get
	End Property
	
	Public Function EnhancedChip As Boolean
		Return pEnhanced
	End Function
	
	Public Shared Function MakeDec(ValTemp As String) As Integer
		ValTemp = ValTemp.Trim.ToLower
		If ValTemp.StartsWith("h'") Then
			ValTemp = ValTemp.Substring(2)
			If ValTemp.Contains("'") Then
				ValTemp = ValTemp.Substring(0, ValTemp.IndexOf("'"))
			End If
			Return Convert.ToUInt32(ValTemp, 16)
		Else If ValTemp.StartsWith("0x") Then
			ValTemp = ValTemp.Substring(2)
			Return Convert.ToUInt32(ValTemp, 16)
		Else
			Return ValTemp
		End If
	End Function
End Class
