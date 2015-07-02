'	ChipDiff, a program to compare GCBASIC chip data files
'	Main File
'	Copyright (C) 2015 Hugh Considine
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
' Date: 2/07/2015
' Time: 10:14 PM
'
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Imports System.IO
Imports System.Linq

Module Program
	Sub Main()
		
		Dim SourceDir1, SourceDir2 As String
		
		SourceDir1 = "D:\GCBASIC\GCBASIC\trunk\chipdata\"
		SourceDir2 = "C:\Final\New Chip Data\chipdata\"
		
		'Get command line arguments
		If My.Application.CommandLineArgs.Count <> 2 Then
			Console.WriteLine("ChipDiff - compares two chip data file folders")
			Console.WriteLine()
			Console.WriteLine("Syntax:")
			Console.WriteLine("       ChipDiff folder1 folder2")
			Console.WriteLine()
			Console.WriteLine("where folder1 and folder2 are paths to folders containing the .dat files")
			Exit Sub
		Else
			SourceDir1 = My.Application.CommandLineArgs(0)
			SourceDir2 = My.Application.CommandLineArgs(1)
		End If
		
		'Create lists
		Dim Files1, Files2 As Dictionary(Of String, String)
		Dim AllChips As New List(Of String)
		
		'Get list of files to compare
		Try
			Files1 = FindChipFiles(SourceDir1)
		Catch
			Console.WriteLine("Error: Check that " + SourceDir1 + " is a valid folder")
			Exit Sub
		End Try
		Try
			Files2 = FindChipFiles(SourceDir2)
		Catch
			Console.WriteLine("Error: Check that " + SourceDir2 + " is a valid folder")
			Exit Sub
		End Try
		
		AllChips.AddRange(Files1.Keys.Union(Files2.Keys))
		
		'Compare each file, show issues
		For Each CurrChip As String In AllChips
			'Need to find chip in both lists, warn if missing from one
			If Not Files1.ContainsKey(CurrChip) Then
				Console.WriteLine("Only in 2:" + CurrChip)
			Else If Not Files2.ContainsKey(CurrChip) Then
				Console.WriteLine("Only in 1:" + CurrChip)
			Else
				'Chip in both lists, compare
				Dim Chip1, Chip2 As ChipInfo
				Chip1 = New ChipInfo(SourceDir1, CurrChip)
				Chip2 = New ChipInfo(SourceDir2, CurrChip)
				CompareChips(Chip1, Chip2)
				
			End If
			
		Next
		
		'Console.ReadKey(True)
	End Sub
	
	Sub CompareChips(Chip1 As ChipInfo, Chip2 As ChipInfo)
		'Check basic info
		If Chip1.ChipProgMem <> Chip2.ChipProgMem Then
			Console.WriteLine(Chip1.ChipName + ":Prog memory differs " + Chip1.ChipProgMem.ToString + ", " + Chip2.ChipProgMem.ToString)
		End If
		If Chip1.ChipEEPROM <> Chip2.ChipEEPROM Then
			Console.WriteLine(Chip1.ChipName + ":EEPROM differs " + Chip1.ChipEEPROM.ToString + ", " + Chip2.ChipEEPROM.ToString)
		End If
		If Chip1.ChipRAM <> Chip2.ChipRAM Then
			Console.WriteLine(Chip1.ChipName + ":RAM differs " + Chip1.ChipRAM.ToString + ", " + Chip2.ChipRAM.ToString)
		End If
		If Chip1.IO <> Chip2.IO Then
			Console.WriteLine(Chip1.ChipName + ":IO differs " + Chip1.IO.ToString + ", " + Chip2.IO.ToString)
		End If
		If Chip1.ADCInputs <> Chip2.ADCInputs Then
			Console.WriteLine(Chip1.ChipName + ":ADC Inputs differs " + Chip1.ADCInputs.ToString + ", " + Chip2.ADCInputs.ToString)
		End If
		If Chip1.MaxMhz <> Chip2.MaxMhz Then
			Console.WriteLine(Chip1.ChipName + ":Max Mhz differs " + Chip1.MaxMhz.ToString + ", " + Chip2.MaxMhz.ToString)
		End If
		If Chip1.IntOscMHz <> Chip2.IntOscMHz Then
			Console.WriteLine(Chip1.ChipName + ":Int Osc MHz differs " + Chip1.IntOscMHz.ToString + ", " + Chip2.IntOscMHz.ToString)
		End If
		If Chip1.ChipFamily <> Chip2.ChipFamily Then
			Console.WriteLine(Chip1.ChipName + ":Chip family differs " + Chip1.ChipFamily.ToString + ", " + Chip2.ChipFamily.ToString)
		End If
		If Chip1.PSP <> Chip2.PSP Then
			Console.WriteLine(Chip1.ChipName + ":PSP differs " + Chip1.PSP.ToString + ", " + Chip2.PSP.ToString)
		End If
		If Chip1.MaxAddress <> Chip2.MaxAddress Then
			Console.WriteLine(Chip1.ChipName + ":Max Address differs " + Chip1.MaxAddress.ToString + ", " + Chip2.MaxAddress.ToString)
		End If
		
		If Chip1.ConfigWords <> Chip2.ConfigWords Then
			Console.WriteLine(Chip1.ChipName + ":Config Words differs " + Chip1.ConfigWords.ToString + ", " + Chip2.ConfigWords.ToString)
		End If
		If Chip1.GPR <> Chip2.GPR Then
			Console.WriteLine(Chip1.ChipName + ":GPR differs " + Chip1.GPR.ToString + ", " + Chip2.GPR.ToString)
		End If
		If Chip1.HardwareMult <> Chip2.HardwareMult Then
			Console.WriteLine(Chip1.ChipName + ":Hardware Mult differs " + Chip1.HardwareMult.ToString + ", " + Chip2.HardwareMult.ToString)
		End If
		
		'Check interrupts
		Dim AllInterrupts As New List(Of String)
		Dim Int1, Int2 As ChipInterrupt
		AllInterrupts.AddRange(Chip1.InterruptList.Keys.Union(Chip2.InterruptList.Keys))
		'Console.WriteLine(Chip1.ChipName + ":" + Chip1.InterruptList.Count.ToString + "," + Chip2.InterruptList.Count.ToString + "," + AllInterrupts.Count.ToString)
		For Each CurrInterrupt As String In AllInterrupts
			If Not Chip1.InterruptList.ContainsKey(CurrInterrupt) Then
				Console.WriteLine(Chip1.ChipName + ": Interrupt " + CurrInterrupt + " only in 2")
			Else If Not Chip2.InterruptList.ContainsKey(CurrInterrupt) Then
				Console.WriteLine(Chip1.ChipName + ": Interrupt " + CurrInterrupt + " only in 1")
			Else
				'Compare interrupts
				Int1 = Chip1.InterruptList(CurrInterrupt)
				Int2 = Chip2.InterruptList(CurrInterrupt)
				If Int1.Flag <> Int2.Flag Then
					Console.WriteLine(Chip1.ChipName + ":" + CurrInterrupt + ": Flag different " + Int1.Flag + "," + Int2.Flag)
				End If
				If Int1.Enable <> Int2.Enable Then
					Console.WriteLine(Chip1.ChipName + ":" + CurrInterrupt + ": Enable different " + Int1.Enable + "," + Int2.Enable)
				End If
				If Int1.VectorLoc <> Int2.VectorLoc Then
					Console.WriteLine(Chip1.ChipName + ":" + CurrInterrupt + ": Vector loc different " + Int1.VectorLoc.ToString + "," + Int2.VectorLoc.ToString)
				End If
			End If
		Next
		
		'Check registers
		Dim AllRegisters As New List(Of String)
		Dim Var1, Var2 As SFRVar
		AllRegisters.AddRange(Chip1.SFRVars.Keys.Union(Chip2.SFRVars.Keys))
		For Each CurrRegister As String In AllRegisters
			If Not Chip1.SFRVars.ContainsKey(CurrRegister) Then
				Console.WriteLine(Chip1.ChipName + ": Register " + CurrRegister + " only in 2")
			Else If Not Chip2.SFRVars.ContainsKey(CurrRegister) Then
				Console.WriteLine(Chip1.ChipName + ": Register " + CurrRegister + " only in 1")
			Else
				'Compare var
				Var1 = Chip1.SFRVars(CurrRegister)
				Var2 = Chip2.SFRVars(CurrRegister)
				If Var1.Location <> Var2.Location Then
					Console.WriteLine(Chip1.ChipName + ":" + CurrRegister + ": Register loc different " + Var1.Location.ToString + "," + Var2.Location.ToString)
				End If
			End If
		Next
		
		'Check register bits
		Dim AllBits As New List(Of String)
		Dim Bit1, Bit2 As SFRVarBit
		AllBits.AddRange(Chip1.SFRVarBits.Keys.Union(Chip2.SFRVarBits.Keys))
		For Each CurrBit As String In AllBits
			If Not Chip1.SFRVarBits.ContainsKey(CurrBit) Then
				Console.WriteLine(Chip1.ChipName + ": Bit " + CurrBit + " only in 2")
			Else If Not Chip2.SFRVarBits.ContainsKey(CurrBit) Then
				Console.WriteLine(Chip1.ChipName + ": Bit " + CurrBit + " only in 1")
			Else
				'Compare var bit
				Bit1 = Chip1.SFRVarBits(CurrBit)
				Bit2 = Chip2.SFRVarBits(CurrBit)
				If Bit1.Var <> Bit2.Var Then
					Console.WriteLine(Chip1.ChipName + ":" + CurrBit + ": Bit var different " + Bit1.Var + "," + Bit2.Var)
				End If
				If Bit1.Bit <> Bit2.Bit Then
					Console.WriteLine(Chip1.ChipName + ":" + CurrBit + ": Bit loc different " + Bit1.Bit.ToString + "," + Bit2.Bit.ToString)
				End If
			End If
		Next
		
		'Check free ram
		
		
	End Sub
	
	Function FindChipFiles(SearchDir As String) As Dictionary(Of String, String)
		Dim OutList As New Dictionary(Of String, String)(StringComparer.CurrentCultureIgnoreCase)
		
		Dim Dir As New DirectoryInfo(SearchDir)
		For Each File As FileInfo In Dir.GetFiles("*.dat")
			If Not File.Name.Contains("core") Then
				OutList.Add(File.Name.Substring(0, File.Name.Length - 4), File.FullName)
			End If
		Next
		
		Return OutList
	End Function
	
End Module
