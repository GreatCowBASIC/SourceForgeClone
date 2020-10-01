'	BankCheck, a program to check bank and page selection in PIC assembly
'	Bank Checking class
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
' Time: 8:39 PM

Imports System.IO

Public Class BankChecker
	
	Public Dim Program As AsmProgram
	
	Public Sub New(IncPath As String, SourceFile As String)
        LogError("")
        LogError("Bank and Page Checkter 1.02")
        LogError("")
        Program = New AsmProgram(IncPath, SourceFile)

        If Program.Loaded Then


            'Check bank selection
            CheckBanks()

            'Check page selection
            CheckPages()
        End If

    End Sub
	
	Private Sub CheckBanks
		Dim CurrLine As AsmLine
        Dim PossBank As Integer
        Dim ModCurrline As String
        LogError("Bank Analysis")
        For Each CurrLine In Program.Lines
            If CurrLine.RequiredBank <> -1 Then
                'Line requires particular bank, which one is currently selected?
                For Each PossBank In CurrLine.PossibleBanks
                    ModCurrline = Left(Replace(CurrLine.Original, vbTab, "|") + Space(32), 32)
                    ModCurrline = Right(Space(6) + CurrLine.FileLine.ToString, 6) + Replace(ModCurrline, "|", " ")
                    If PossBank <> CurrLine.RequiredBank Then
                        'LogError(CurrLine.SourceFile + "(" + CurrLine.FileLine.ToString + "): Bad bank, requires " + CurrLine.RequiredBank.ToString + " but could be in " + PossBank.ToString)
                        LogError(ModCurrline + ": Bad bank, requires " + CurrLine.RequiredBank.ToString + " but could be in " + PossBank.ToString)
                    Else
                        LogError(ModCurrline + ": Bank map " + CurrLine.RequiredBank.ToString + " - " + PossBank.ToString)
                    End If
                Next
            End If
        Next
        LogError("")

    End Sub
	
	Private Sub CheckPages
		Dim CurrLine As AsmLine
        Dim PossPage As Integer
        Dim ModCurrline As String
        LogError("")
        LogError("Page Analysis")
        For Each CurrLine In Program.Lines
            If CurrLine.RequiredPage <> -1 Then
                'Line requires particular page, which one is currently selected?
                For Each PossPage In CurrLine.PossiblePages
                    If PossPage <> CurrLine.RequiredPage Then
                        ModCurrline = Left(Replace(CurrLine.Original, vbTab, "|") + Space(32), 32)
                        ModCurrline = Right(Space(6) + CurrLine.FileLine.ToString, 6) + Replace(ModCurrline, "|", " ")
                        LogError(ModCurrline + ": Bad page, requires " + CurrLine.RequiredPage.ToString + " but could be in " + PossPage.ToString)
                    End If
                Next
            End If
        Next
        LogError("")
    End Sub
	
	Private Sub DumpProgram
		Dim log As New StreamWriter("D:\Temp\bankchecker.txt")
		
		Dim PrevLoc, NextLoc, Selects, PossBank, PossPage As String
		Dim CurrLine, FindLine As AsmLine
		Dim CurrBank, CurrPage As Integer
		For Each CurrLine In Program.Lines
			If CurrLine.SourceFile.Contains(".asm") Then
				NextLoc = ""
				PrevLoc = ""
				Selects = ""
				log.WriteLine(CurrLine.Location.ToString("X") + vbTab + CurrLine.ToString)
				For Each FindLine In CurrLine.PrevLines
					PrevLoc += " " + FindLine.Location.ToString("X")
				Next
				For Each FindLine In CurrLine.NextLines
					NextLoc += " " + FindLine.Location.ToString("X")
				Next
				If CurrLine.IsBanksel Then
					Selects = ", Selects bank " + CurrLine.BankOrPage.ToString
				End If
				If CurrLine.IsPagesel Then
					Selects = ", Selects page " + CurrLine.BankOrPage.ToString
				End If
				PossBank = ""
				If CurrLine.PossibleBanks.Count > 0 Then
					PossBank = ", Possible bank/s:"
					For Each CurrBank In CurrLine.PossibleBanks
						PossBank += " " + CurrBank.ToString
					Next
				End If
				PossPage = ""
				If CurrLine.PossiblePages.Count > 0 Then
					PossPage = ", Possible page/s:"
					For Each CurrPage In CurrLine.PossiblePages
						PossPage += " " + CurrPage.ToString
					Next
				End If
				log.WriteLine(vbTab + "Prev:" + PrevLoc + ", Next:" + NextLoc + Selects + PossBank + PossPage)
			End If
		Next
		
		log.Close
	End Sub
	
	Private Sub LogError(Message As String)
		Console.WriteLine(Message)
	End Sub
	
End Class
