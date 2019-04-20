' GCBASIC - A BASIC Compiler for microcontrollers
'  Preprocessor
' Copyright (C) 2006 - 2014 Hugh Considine
'
' This program is free software; you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation; either version 2 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License
' along with this program; if not, write to the Free Software
' Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
'
'If you have any questions about the source code, please email me: hconsidine at internode.on.net
'Any other questions, please email me or see the GCBASIC forums.

#include "file.bi"

Sub AddConstant(ConstName As String, ConstValue As String, ConstStartup As String = "", ReplaceExisting As Integer = -1)
	'Add a new constant to the list of constants

	Dim Meta As ConstMeta Pointer
	Meta = Callocate(SizeOf(ConstMeta))

	Meta->Value = ConstValue
	Meta->Startup = ConstStartup

	HashMapSet(Constants, ConstName, Meta, ReplaceExisting)
End Sub

Function CheckSysVarDef(ConditionIn As String) As String
	'Checks a condition from #ifdef or If in script
	'Returns result

	Dim As String Condition, Temp, Original
	Dim As Integer FV, FC, ConstFound

	Condition = ConditionIn

	'Test for SFR bit?
	Do While INSTR(Condition, "BIT(") <> 0
		'Get name of bit, and whether checking for presence or absence
		FV = 0: If INSTR(Condition, "NOBIT(") = INSTR(Condition, "BIT(") - 2 Then FV = 1
		Temp = Mid(Condition, INSTR(Condition, "BIT(") + 4)
		Temp = Left(Temp, INSTR(Temp, ")") - 1)
		If FV = 0 Then
			Original = "BIT(" + Temp + ")"
		Else
			Original = "NOBIT(" + Temp + ")"
		End If

		'Search for bit in list
		ConstFound = HasSFRBit(ReplaceConstantsLine(Trim(Temp), 0))

		'Replace result
		If FV = 1 Then
			'If ConstFound = 0 Then DelMode = 1 Else DelMode = 2
			If ConstFound = 0 Then
				ConstFound = 1
			Else
				ConstFound = 0
			End If
		Else
			If ConstFound <> 0 Then ConstFound = 1
		End If

		Replace Condition, Original, Str(ConstFound) + "=1"
	Loop

	'Test for SFR?
	Do While INSTR(Condition, "VAR(") <> 0
		'Get name of SFR, and checking mode
		FV = 0: IF INSTR(Condition, "NOVAR(") = INSTR(Condition, "VAR(") - 2 THEN FV = 1
		Temp = Mid(Condition, INSTR(Condition, "VAR(") + 4)
		Temp = Left(Temp, INSTR(Temp, ")") - 1)
		If FV = 0 Then
			Original = "VAR(" + Temp + ")"
		Else
			Original = "NOVAR(" + Temp + ")"
		End If

		ConstFound = HasSFR(ReplaceConstantsLine(Trim(Temp), 0))

		'Replace result
		If FV = 1 Then
			'If ConstFound = 0 Then DelMode = 1 Else DelMode = 2
			If ConstFound = 0 Then
				ConstFound = 1
			Else
				ConstFound = 0
			End If
		Else
			If ConstFound <> 0 Then ConstFound = 1
		End If

		Replace Condition, Original, Str(ConstFound) + "=1"
	Loop
	
	'Test for constant?
	Do While INSTR(Condition, "DEF(") <> 0
		'Get name of constant, and checking mode
		FV = 0: IF INSTR(Condition, "NODEF(") = INSTR(Condition, "DEF(") - 2 THEN FV = 1
		Temp = Mid(Condition, INSTR(Condition, "DEF(") + 4)
		Temp = Left(Temp, INSTR(Temp, ")") - 1)
		If FV = 0 Then
			Original = "DEF(" + Temp + ")"
		Else
			Original = "NODEF(" + Temp + ")"
		End If
		
		ConstFound = HashMapGet(Constants, Temp) <> 0

		'Replace result
		If FV = 1 Then
			'If ConstFound = 0 Then DelMode = 1 Else DelMode = 2
			If ConstFound = 0 Then
				ConstFound = 1
			Else
				ConstFound = 0
			End If
		Else
			If ConstFound <> 0 Then ConstFound = 1
		End If

		Replace Condition, Original, Str(ConstFound) + "=1"
	Loop

	Return Condition
End Function

Sub LoadTableFromFile(DataTable As DataTableType Pointer)
	'Read a data table from a file
	Dim TempData As String
	Dim DataIn As UByte
	Dim FileNo As Integer

	'Check that table is passed in and that it has a source file
	If DataTable = 0 Then Exit Sub
	If Dir(DataTable->SourceFile) = "" Then
		'Warning if file not found
		TempData = Message("WarningTableFileNotFound")
		Replace TempData, "%table%", DataTable->Name
		Replace TempData, "%file%", DataTable->SourceFile
		LogWarning(TempData, DataTable->Origin)
		Exit Sub
	End If

	'Open and read
	FileNo = FreeFile
	Open DataTable->SourceFile For Binary As FileNo
	Do While Not Eof(FileNo)
		Get #FileNo, , DataIn
		DataTable->CurrItem = LinkedListInsert(DataTable->CurrItem, Str(DataIn))
	Loop
	Close #FileNo

End Sub

Sub PrepareBuiltIn

	'Read built-in subs and constants
	Dim As String InnerLoop, OuterLoop
	Dim As Integer CD, T, T2
	Dim As Double L
	Dim As LinkedListElement Pointer CurrPos

	'Label Ending
	LabelEnd = ""
	If ModeAVR Then LabelEnd = ":"
	If ModeZ8 Then LabelEnd = ":"

	'Constants set by compiler
	'Set chip config defines for #IFDEF and #SCRIPT use
	AddConstant("CHIPNAME", ChipName)
	AddConstant("CHIP_" + ChipName, "")
	AddConstant("CHIPMHZ", Str(ChipMhz))
	AddConstant("CHIPOSC", OSCType)
	AddConstant("CHIPRESERVEHIGHPROG", Str(ReserveHighProg))

	If ModePIC Then AddConstant("PIC", "")
	If ModeAVR Then
		AddConstant("AVR", "")
		If HMult Then AddConstant("HARDWAREMULT", "")
	End If
	If ModeZ8 Then AddConstant("Z8", "")

	'Constant to give chip name as string
	SSC = SSC + 1
	StringStore(SSC).Value = ChipName
	StringStore(SSC).Used = 0
	AddConstant("CHIPNAMESTR", ";STRING" + Str(SSC) + ";")

	'Constants to provide information on RAM banks
	Dim As String TempData
	Dim As Integer Range, Min, Max
	For Range = 1 to MRC
		TempData = MemRanges(Range)
		Min = VAL("&h" + Left(TempData, INSTR(TempData, ":") - 1))
		Max = VAL("&h" + Mid(TempData, INSTR(TempData, ":") + 1))
		'Print "Bank " + Str(Min \ 128) + " starts 0x" + Hex(Min) + " ends 0x" + Hex(Max)
		AddConstant("CHIPBANK_" + Str(Min \ 128) + "_START", Str(Min))
		AddConstant("CHIPBANK_" + Str(Min \ 128) + "_END", Str(Max))
	Next

	'Delay subs
	'Time Intervals: us, 10us, ms, 10ms, sec, min, hour

	'Delay_US
	SBC += 1
	Subroutine(SBC) = NewSubroutine("Delay_US")
	CurrPos = Subroutine(SBC)->CodeStart

	If ModePIC Then
		'Each nop takes .2 us on 20 MHz chip
		L = ChipMhz / 4 - 3
		'Make sure L is positive integer
		'If not, us delays will be inaccurate
		If L < 0 Then
			L = 0
			USDelaysInaccurate = -1
		End If
		If L <> Int(L) Then
			USDelaysInaccurate = -1
		End If
		CurrPos = LinkedListInsert(CurrPos, " incf SysWaitTempUS_H, F")
		CurrPos = LinkedListInsert(CurrPos, " movf SysWaitTempUS, F")
		CurrPos = LinkedListInsert(CurrPos, " btfsc STATUS,Z")
		CurrPos = LinkedListInsert(CurrPos, " goto DUS_END")
		CurrPos = LinkedListInsert(CurrPos, "DUS_START")
		For CD = 1 TO L
			CurrPos = LinkedListInsert(CurrPos, " nop")
		Next
		CurrPos = LinkedListInsert(CurrPos, " decfsz SysWaitTempUS, F")
		CurrPos = LinkedListInsert(CurrPos, " goto DUS_START")
		CurrPos = LinkedListInsert(CurrPos, "DUS_END")
		CurrPos = LinkedListInsert(CurrPos, " decfsz SysWaitTempUS_H, F")
		CurrPos = LinkedListInsert(CurrPos, " goto DUS_START")
	ElseIf ModeAVR Then
		L = ChipMhz - 3
		If L < 0 Then
			L = 0
			USDelaysInaccurate = -1
		End If
		CurrPos = LinkedListInsert(CurrPos, " inc SysWaitTempUS_H")
		CurrPos = LinkedListInsert(CurrPos, " tst SysWaitTempUS")
		CurrPos = LinkedListInsert(CurrPos, " breq DUS_END")
		CurrPos = LinkedListInsert(CurrPos, "DUS_START:")
		CurrPos = LinkedListInsertList(CurrPos, GenerateExactDelay(L))
		CurrPos = LinkedListInsert(CurrPos, " dec SysWaitTempUS")
		CurrPos = LinkedListInsert(CurrPos, " brne DUS_START")
		CurrPos = LinkedListInsert(CurrPos, "DUS_END:")
		CurrPos = LinkedListInsert(CurrPos, " dec SysWaitTempUS_H")
		CurrPos = LinkedListInsert(CurrPos, " brne DUS_START")
	End If

	'Delay_10US
	SBC = SBC + 1
	Subroutine(SBC) = NewSubroutine("Delay_10US")
	CurrPos = Subroutine(SBC)->CodeStart
	If ModePIC Then

		CurrPos = LinkedListInsert(CurrPos, "D10US_START")
		'Need to delay for 10 us
		'L = number of cycles to waste
		'  = cycles in 10 us, minus cycles at start and end of loop
		'3 cycles at end of loop
		L = 10 * ChipMhz \ 4 - 3
		If L < 0 Then L = 0
		CurrPos = LinkedListInsertList(CurrPos, GenerateExactDelay(L))
		CurrPos = LinkedListInsert(CurrPos, " decfsz SysWaitTemp10US, F")
		CurrPos = LinkedListInsert(CurrPos, " goto D10US_START")

	ElseIf ModeAVR Then
		L = 10 * ChipMhz - 3
		If L < 0 Then L = 0
		CurrPos = LinkedListInsert(CurrPos, "D10US_START:")
		CurrPos = LinkedListInsertList(CurrPos, GenerateExactDelay(L))
		CurrPos = LinkedListInsert(CurrPos, " dec SysWaitTemp10US")
		CurrPos = LinkedListInsert(CurrPos, " brne D10US_START")
	End If

	'Delay_MS
	'Repeat 20(wait 50)
	SBC += 1
	Subroutine(SBC) = NewSubroutine("Delay_MS")
	CurrPos = Subroutine(SBC)->CodeStart

	Dim As Integer D1, D2, BestD1, BestD2, ThisTime, ReqCycles, DiffFromReq, BestDiff

	If ModePIC Then
		'Cycles for code below:
		'1 + (2 + (2 + Inner * 3 - 1) * (Outer * 3) - 1) + Time * 3

		'Calculate required number of wasted cycles
		ReqCycles = 1000 * ChipMhz / 4
		BestDiff = ReqCycles

		'Find best values for delay
		For D1 = 1 To 255
			For D2 = 1 To 255
				'Calc how long current D1, D2 values will give
				ThisTime = 5 + D2 * (3 * D1 + 4)
				'Check to see how close it is to the required delay
				If ThisTime < ReqCycles Then
					DiffFromReq = ReqCycles - ThisTime
				ElseIf ThisTime > ReqCycles Then
					DiffFromReq = ThisTime - ReqCycles
				End If
				'If it's the best, record
				If DiffFromReq < BestDiff Then
					BestD1 = D1
					BestD2 = D2
					BestDiff = DiffFromReq
				End If
			Next
		Next
		OuterLoop = Str(BestD2)
		InnerLoop = Str(BestD1)

		CurrPos = LinkedListInsert(CurrPos, " incf SysWaitTempMS_H, F")
		CurrPos = LinkedListInsert(CurrPos, "DMS_START")
		CurrPos = LinkedListInsert(CurrPos, "DELAYTEMP2 = " + OuterLoop)
		CurrPos = LinkedListInsert(CurrPos, "DMS_OUTER")
		CurrPos = LinkedListInsert(CurrPos, "DELAYTEMP = " + InnerLoop)
		CurrPos = LinkedListInsert(CurrPos, "DMS_INNER")
		CurrPos = LinkedListInsert(CurrPos, " decfsz DELAYTEMP, F")
		CurrPos = LinkedListInsert(CurrPos, " goto DMS_INNER")
		CurrPos = LinkedListInsert(CurrPos, " decfsz DELAYTEMP2, F")
		CurrPos = LinkedListInsert(CurrPos, " goto DMS_OUTER")
		CurrPos = LinkedListInsert(CurrPos, " decfsz SysWaitTempMS, F")
		CurrPos = LinkedListInsert(CurrPos, " goto DMS_START")
		CurrPos = LinkedListInsert(CurrPos, " decfsz SysWaitTempMS_H, F")
		CurrPos = LinkedListInsert(CurrPos, " goto DMS_START")
	ElseIf ModeAVR Then

		'Calculate required number of wasted cycles
		ReqCycles = 1000 * ChipMhz
		BestDiff = ReqCycles

		'Find best values for delay
		For D1 = 1 To 255
			For D2 = 1 To 255
				'Calc how long current D1, D2 values will give
				ThisTime = D2 * (3 + 3 * D1)
				'Check to see how close it is to the required delay
				If ThisTime < ReqCycles Then
					DiffFromReq = ReqCycles - ThisTime
				ElseIf ThisTime > ReqCycles Then
					DiffFromReq = ThisTime - ReqCycles
				End If
				'If it's the best, record
				If DiffFromReq < BestDiff Then
					BestD1 = D1
					BestD2 = D2
					BestDiff = DiffFromReq
				End If
			Next
		Next
		OuterLoop = Str(BestD2)
		InnerLoop = Str(BestD1)

		CurrPos = LinkedListInsert(CurrPos, " inc SysWaitTempMS_H")
		CurrPos = LinkedListInsert(CurrPos, "DMS_START:")

		CurrPos = LinkedListInsert(CurrPos, "DELAYTEMP2 = " + OuterLoop)
		CurrPos = LinkedListInsert(CurrPos, "DMS_OUTER:")
		CurrPos = LinkedListInsert(CurrPos, "DELAYTEMP = " + InnerLoop)
		CurrPos = LinkedListInsert(CurrPos, "DMS_INNER:")
		CurrPos = LinkedListInsert(CurrPos, " dec DELAYTEMP")
		CurrPos = LinkedListInsert(CurrPos, " brne DMS_INNER")
		CurrPos = LinkedListInsert(CurrPos, " dec DELAYTEMP2")
		CurrPos = LinkedListInsert(CurrPos, " brne DMS_OUTER")
		CurrPos = LinkedListInsert(CurrPos, " dec SysWaitTempMS")
		CurrPos = LinkedListInsert(CurrPos, " brne DMS_START")
		CurrPos = LinkedListInsert(CurrPos, " dec SysWaitTempMS_H")
		CurrPos = LinkedListInsert(CurrPos, " brne DMS_START")
	End If

	'Delay_10MS
	SBC += 1
	Subroutine(SBC) = NewSubroutine("Delay_10MS")
	CurrPos = Subroutine(SBC)->CodeStart
	If ModePIC Then
		CurrPos = LinkedListInsert(CurrPos, "D10MS_START")
		CurrPos = LinkedListInsert(CurrPos, "SysWaitTempMS = 10")
		CurrPos = LinkedListInsert(CurrPos, "Delay_MS")
		CurrPos = LinkedListInsert(CurrPos, " decfsz SysWaitTemp10MS, F")
		CurrPos = LinkedListInsert(CurrPos, " goto D10MS_START")
	ElseIf ModeAVR Then
		CurrPos = LinkedListInsert(CurrPos, "D10MS_START:")
		CurrPos = LinkedListInsert(CurrPos, "SysWaitTempMS = 10")
		CurrPos = LinkedListInsert(CurrPos, "Delay_MS")
		CurrPos = LinkedListInsert(CurrPos, " dec SysWaitTemp10MS")
		CurrPos = LinkedListInsert(CurrPos, " brne D10MS_START")
	End If

	'Delay_Sec
	SBC += 1
	Subroutine(SBC) = NewSubroutine("Delay_S")
	CurrPos = Subroutine(SBC)->CodeStart
	If ModePIC Then
		CurrPos = LinkedListInsert(CurrPos, "DS_START")
		CurrPos = LinkedListInsert(CurrPos, "SysWaitTempMS = 1000")
		CurrPos = LinkedListInsert(CurrPos, "Delay_MS")
		CurrPos = LinkedListInsert(CurrPos, " decfsz SysWaitTempS, F")
		CurrPos = LinkedListInsert(CurrPos, " goto DS_START")
	ElseIf ModeAVR Then
		CurrPos = LinkedListInsert(CurrPos, "DS_START:")
		CurrPos = LinkedListInsert(CurrPos, "SysWaitTempMS = 1000")
		CurrPos = LinkedListInsert(CurrPos, "Delay_MS")
		CurrPos = LinkedListInsert(CurrPos, " dec SysWaitTempS")
		CurrPos = LinkedListInsert(CurrPos, " brne DS_START")
	End If

	'Delay_Min
	SBC += 1
	Subroutine(SBC) = NewSubroutine("Delay_M")
	CurrPos = Subroutine(SBC)->CodeStart
	If ModePIC Then
		CurrPos = LinkedListInsert(CurrPos, "DMIN_START")
		CurrPos = LinkedListInsert(CurrPos, "SysWaitTempMS = 60000")
		CurrPos = LinkedListInsert(CurrPos, "Delay_MS")
		CurrPos = LinkedListInsert(CurrPos, " decfsz SysWaitTempM, F")
		CurrPos = LinkedListInsert(CurrPos, " goto DMIN_START")
	ElseIf ModeAVR Then
		CurrPos = LinkedListInsert(CurrPos, "DMIN_START:")
		CurrPos = LinkedListInsert(CurrPos, "SysWaitTempMS = 60000")
		CurrPos = LinkedListInsert(CurrPos, "Delay_MS")
		CurrPos = LinkedListInsert(CurrPos, " dec SysWaitTempM")
		CurrPos = LinkedListInsert(CurrPos, " brne DMIN_START")
	End If

	'Delay_Hour
	SBC += 1
	Subroutine(SBC) = NewSubroutine("Delay_H")
	CurrPos = Subroutine(SBC)->CodeStart
	If ModePIC Then
		CurrPos = LinkedListInsert(CurrPos, "DHOUR_START")
		CurrPos = LinkedListInsert(CurrPos, "SysWaitTempM = 60")
		CurrPos = LinkedListInsert(CurrPos, "Delay_M")
		CurrPos = LinkedListInsert(CurrPos, " decfsz SysWaitTempH, F")
		CurrPos = LinkedListInsert(CurrPos, " goto DHOUR_START")
	ElseIf ModeAVR Then
		CurrPos = LinkedListInsert(CurrPos, "DHOUR_START:")
		CurrPos = LinkedListInsert(CurrPos, "SysWaitTempM = 60")
		CurrPos = LinkedListInsert(CurrPos, "Delay_M")
		CurrPos = LinkedListInsert(CurrPos, " dec SysWaitTempH")
		CurrPos = LinkedListInsert(CurrPos, " brne DHOUR_START")
	End If

	'FSR alias on 18F
	If ChipFamily = 15 Or ChipFamily = 16 Then
		AddVar "AFSR0", "WORD", 1, 0, "ALIAS:FSR0H, FSR0L", ""
		AddVar "AFSR1", "WORD", 1, 0, "ALIAS:FSR1H, FSR1L", ""
		If ChipFamily = 16 Then AddVar "AFSR2", "WORD", 1, 0, "ALIAS:FSR2H, FSR2L", ""
	End If

	'Enable "GPIO" on chips that don't have it
	'This affects PIC 12F1822 and possibly others
	'Are we dealing with an 8 pin PIC?
	If ModePIC And ChipPins <= 8 Then
		'Is there a GPIO port?
		If Not HasSFR("GPIO") Then
			'If there's a PORTA, set GPIO to PORTA
			If HasSFR("PORTA") Then
				AddConstant "GPIO", "PORTA"

			'If there's a PORTB, set GPIO to PORTB
			ElseIf HasSFR("PORTB") Then
				AddConstant "GPIO", "PORTB"

			'If there's a PORTC, set GPIO to PORTC
			ElseIf HasSFR("PORTC") Then
				AddConstant "GPIO", "PORTC"

			'Let's hope none have PORTD as their only port!
			End If

		End If
	End If

END SUB

SUB PreProcessor

	Dim As String Origin, Temp, DataSource, PreserveIn, CurrentSub, StringTemp, SubName
	Dim As String Value, RTemp, LTemp, Ty, SubInType, ParamType, RestOfLine, VarName, FName, ConstName
	Dim As String TempFile, LastTableOrigin, NewFNType, CodeTemp, OtherChar, BinHexTemp
	Dim As String DataSourceOld, TranslatedFile, HFI, DataSourceRaw
	Dim As LinkedListElement Pointer CurrPos, MainCurrPos, SearchPos

	Dim As String LineToken(100)
	Dim As SourceFileType UnconvertedFile(100)
	Dim As OriginType Pointer LineOrigin

	Dim As Integer T, T2, ICCO, CE, PD, RF, S, LC, LCS, SID, CD, SL, NR
	Dim As Integer ForceMain, LineTokens, FoundFunction, FoundMacro, CurrChar
	Dim As Integer CurrCharPos, ReadType, ConvertAgain, UnconvertedFiles
	Dim As Single CurrPerc, PercAdd, PercOld
	Dim As Double LastCompTime, StartTime

	CurrentSub = ""
	UnconvertedFiles = 0
	
	'Find required files
	IF VBS = 1 THEN PRINT : PRINT SPC(5); Message("FindSource")
	SourceFiles = 1: SourceFile(1).FileName = ShortName(FI)
	T = 1
	
	'Get date on output (hex) file
	If FlashOnly Then
		HFI = ReplaceToolVariables("%filename%", "hex")
		If Dir(HFI) <> "" Then
			LastCompTime = FileDateTime(HFI)
		Else
			'If hex file doesn't exist, force compile
			FlashOnly = 0
			LastCompTime = 0
		End If
	End If
	
	'FindIncludeFiles:
	Do
		T2 = T
		ICCO = SourceFiles
		FOR T = T2 TO SourceFiles

			IF VBS = 1 THEN PRINT SPC(10); SourceFile(T).FileName;
			IF Dir(SourceFile(T).FileName) = "" THEN
				Temp = Message("NoFile")
				Replace Temp, FI, SourceFile(T).FileName
				IF VBS = 0 THEN
					PRINT Temp
				Else
					PRINT ": " + Message("NotFound")
				End If

				'Log warning
				LogWarning(Temp, SourceFile(T).IncludeOrigin)

			Else
				IF VBS = 1 THEN PRINT ": " + Message("found")
				
				'Is this file newer than output file? If so, cannot skip compile
				If FlashOnly And Not SkipHexCheck Then
					If LastCompTime < FileDateTime(SourceFile(T).FileName) Then
						FlashOnly = 0
					End If
				End If

				If Not SourceFile(T).RequiresConversion Then
					Open SourceFile(T).FileName For INPUT AS #1
					LC = 0

					DO WHILE NOT EOF(1)
						LINE INPUT #1, Temp
						LC += 1
						Temp = LTrim(Temp, Any Chr(9) + " ")
						IF UCase(Left(Temp, 8)) = "#INCLUDE" THEN
							IF INSTR(Temp, Chr(34)) <> 0 THEN
								Temp = Mid(Temp, INSTR(Temp, Chr(34)) + 1)
								Temp = Trim(Left(Temp, INSTR(Temp, Chr(34)) - 1))
								Temp = AddFullPath(Temp, ProgDir)
							ElseIF INSTR(Temp, "<") <> 0 THEN
								Temp = Mid(Temp, INSTR(Temp, "<") + 1)
								Temp = Left(Temp, INSTR(Temp, ">") - 1)
								Temp = AddFullPath(Temp, ID + "\include\")
							END IF
							Temp = ShortName(Temp)
							'Check to see if include file already in list
							CE = 0
							FOR PD = 1 TO SourceFiles
								IF UCase(SourceFile(PD).FileName) = UCase(Temp) THEN CE = 1: EXIT FOR
							Next

							'If not, add it
							IF CE = 0 Then

								'May need to convert
								TranslatedFile = TranslateFile(Temp)
								If TranslatedFile = "" Then
									UnconvertedFiles += 1
									UnconvertedFile(UnconvertedFiles).FileName = Temp
									UnconvertedFile(UnconvertedFiles).IncludeOrigin = ";?F" + Str(T) + "L" + Str(LC) + "S0?"
									UnconvertedFile(UnconvertedFiles).RequiresConversion = -1
								Else
									SourceFiles += 1
									SourceFile(SourceFiles).IncludeOrigin = ";?F" + Str(T) + "L" + Str(LC) + "S0?"
									SourceFile(SourceFiles).FileName = TranslatedFile
								End If

							End If
						END IF
					LOOP

					Close #1
				END If
			End If
		NEXT
	'IF ICCO < ICC THEN GOTO FindIncludeFiles
	Loop While ICCO < SourceFiles
	ICCO = SourceFiles
	
	'Add standard include files to list
	#IFDEF __FB_UNIX__
		OPEN ID + "/include/lowlevel.dat" FOR INPUT AS #1
	#ELSE
		OPEN ID + "\include\lowlevel.dat" FOR INPUT AS #1
	#EndIf

	DO WHILE NOT EOF(1)
		LINE INPUT #1, DataSource
		DataSource = Trim(DataSource)
		IF Left(DataSource, 1) <> "'" THEN
			#IFDEF __FB_UNIX__
				DataSource = ID + "/include/lowlevel/" + DataSource
			#ELSE
				DataSource = ID + "\INCLUDE\LOWLEVEL\" + DataSource
			#ENDIF
			CE = 0
			FOR PD = 1 TO SourceFiles
				IF UCase(SourceFile(PD).FileName) = UCase(DataSource) THEN CE = 1: EXIT FOR
			NEXT
			IF CE = 0 THEN
				SourceFiles += 1
				SourceFile(SourceFiles).FileName = DataSource
				Temp = Dir(DataSource)
				IF Temp <> "" Then
					Temp = ": " + Message("found")
					If FlashOnly Then
						If LastCompTime < FileDateTime(DataSource) Then
							FlashOnly = 0
						End If
					End If
				Else
					Temp = ": " + Message("NotFound")
				End If
				IF VBS = 1 THEN PRINT SPC(10); DataSource; Temp
				
			END IF
		END IF
	LOOP
	Close
	
	'Create Main subroutine
	Subroutine(0) = NewSubroutine("Main")
	MainCurrPos = Subroutine(0)->CodeStart
	Subroutine(0)->Required = -1 'Mark as required so that it gets compiled

	'Add any unconverted files to file list
	For RF = 1 To UnconvertedFiles
		SourceFiles += 1
		SourceFile(SourceFiles) = UnconvertedFile(RF)
	Next

	'Read all required files
	IF VBS = 1 THEN PRINT SPC(5); Message("LoadSource");
	PercOld = 0
	CurrPerc = 0.5
	PercAdd = 1 / SourceFiles * 100
	LineOrigin = 0
	StartTime = Timer
	FOR RF = 1 TO SourceFiles

		'Translate files if needed
		If SourceFile(RF).RequiresConversion Then
			SourceFile(RF).FileName = TranslateFile(SourceFile(RF).FileName)
			SourceFile(RF).RequiresConversion = 0
		EndIf

		If OPEN(SourceFile(RF).FileName FOR INPUT AS #1) <> 0 Then Goto LoadNextFile

		S = 0
		LC = 0
		LCS = 0

		DO WHILE NOT EOF(1)
		LoadFileData:
			LINE INPUT #1, DataSource
			LC = LC + 1
			LCS = LCS + 1

			'Origin of this line
			LineOrigin = Callocate(SizeOf(OriginType))
			LineOrigin->FileNo = RF
			LineOrigin->LineNo = LC

			'Save copy for Preserve mode
			'Only save stuff in main file or inside a subroutine
			'0 = nothing, 1 = comments (K:C), 2 = code (K:A), 3 = line numbers (K:L)
			PreserveIn = ""
			DataSourceRaw = DataSource
			If (PreserveMode = 1 Or PreserveMode = 2) And (RF = 1 OR S = 1) Then
				PreserveIn = DataSource
				Do While Left(PreserveIn, 1) = Chr(9): PreserveIn = Mid(PreserveIn, 2): Loop
				PreserveIn = TRIM(PreserveIn)

				'Preserve Comments
				If Left(PreserveIn, 1) = "'" OR Left(PreserveIn, 1) = ";" OR Left(PreserveIn, 4) = "REM " Then
					IF Left(PreserveIn, 4) = "REM " Then PreserveIn = "'" + Trim(Mid(PreserveIn, 4))
					PreserveIn = Trim(Mid(PreserveIn, 2))
					PCC += 1
					PreserveCode(PCC) = ";" + PreserveIn
					IF S = 0 THEN MainCurrPos = LinkedListInsert(MainCurrPos, "PRESERVE " + Str(PCC))
					IF S = 1 THEN CurrPos = LinkedListInsert(CurrPos, "PRESERVE " + Str(PCC))
					PreserveIn = ""
				End If
				If PreserveMode = 1 Then PreserveIn = ""
			End If
			If PreserveMode = 3 Then
				PreserveIn = "Source:F" + Str(RF) + "L" + Str(LC) + "S" + Str(SBC * S) + "I" + Str(LCS)
			End If

			'Extract strings, and remove comments with ; and '
			'Also process H' and B', 0x and 0b
			'Convert tabs to spaces

			'0 = standard, 1 = string, 2 = inside binary or hex (with '), 3 = comment, 4 = inside binary or hex (no ')
			ReadType = 0
			CodeTemp = ""
			StringTemp = ""
			BinHexTemp = ""
			CurrCharPos = 1
			Do While CurrCharPos <= Len(DataSource)
				CurrChar = Asc(DataSource, CurrCharPos)

				If CurrChar = Asc("""") Then
					'Start or end of string
					If ReadType = 0 Then
						ReadType = 1
						CurrChar = -1 'Don't append
						StringTemp = ""

					ElseIf ReadType = 1 Then
						'Is next character another double quote? If so, include it
						'Allows "" to represent a " in a string
						If Mid(DataSource, CurrCharPos + 1, 1) = Chr(34) Then
							CurrCharPos += 1 'Skip next char
						Else
							'End of string found
							ReadType = 0
							CurrChar = -1 'Don't append to code!

							'Store string
							'Check for duplicates
							SID = 0
							FOR CD = 1 to SSC
								If StringStore(CD).Value = StringTemp THEN SID = CD: EXIT FOR
							NEXT

							If SID = 0 Then
								SSC = SSC + 1
								StringStore(SSC).Value = StringTemp
								StringStore(SSC).Used = 0
								SID = SSC
							End If

							'Add reference to string to line
							CodeTemp += ";STRING" + Str(SID) + ";"
						End If
					End If

				ElseIf CurrChar = Asc("'") Then
					'Comment, or possibly part of binary/hex constant
					If ReadType = 0 Then
						OtherChar = LCase(Mid(DataSource, CurrCharPos - 1, 1))
						If OtherChar = "b" Or OtherChar = "h" Then
							ReadType = 2
							BinHexTemp = OtherChar + Chr(CurrChar)
							CurrChar = -1
							'b or h will have been appended to CodeTemp, remove
							CodeTemp = Left(CodeTemp, Len(CodeTemp) - 1)
						Else
							ReadType = 3
						End If

					ElseIf ReadType = 2 Then
						'Last part of binary/hex literal, b' or h' format
						BinHexTemp += Chr(CurrChar)
						If LCase(Left(BinHexTemp, 1)) = "h" Then
							BinHexTemp = "0x" + Mid(BinHexTemp, 3, Len(BinHexTemp) - 3)
						End If
						CodeTemp += Str(MakeDec(BinHexTemp))

						BinHexTemp = ""
						CurrChar = -1
						ReadType = 0
					End If

				ElseIf ReadType = 0 And CurrChar = Asc("0") Then
					OtherChar = LCase(Mid(DataSource, CurrCharPos + 1, 1))
					If (OtherChar = "b" Or OtherChar = "x") And IsDivider(Mid(DataSource, CurrCharPos - 1, 1)) Then
						ReadType = 4
						BinHexTemp = Chr(CurrChar)
						CurrChar = -1
					End If

				ElseIf ReadType = 4 Then
					OtherChar = LCase(Mid(DataSource, CurrCharPos + 1, 1))
					If IsDivider(OtherChar) Or OtherChar = "'" Or OtherChar = Chr(34) Or CurrCharPos = Len(DataSource) Then
						'Last part of binary/hex literal, 0x or 0b format
						BinHexTemp += Chr(CurrChar)
						If UCase(Left(BinHexTemp, 2)) = "0B" Then
							BinHexTemp = "b'" + Mid(BinHexTemp, 3) + "'"
						End If
						CodeTemp += Str(MakeDec(BinHexTemp))
						BinHexTemp = ""
						CurrChar = -1
						ReadType = 0
					End If

				ElseIf CurrChar = Asc(";") Then
					'If this occurs outside of a string, it means comment start
					If ReadType <> 1 Then ReadType = 3
				End If

				Select Case ReadType
					Case 0:
						'Append char to code line
						'Replace tabs with spaces
						If CurrChar = 9 Then CurrChar = Asc(" ")
						If CurrChar = 160 Then CurrChar = Asc(" ") 'non-breaking space, seems to end up in code sometimes and breaks compiler
						'Prevent multiple spaces
						If CurrChar <> Asc(" ") Or Right(CodeTemp, 1) <> " " Then
							If CurrChar <> -1 Then CodeTemp += Chr(CurrChar)
						End If

					Case 1:
						'Append char to string
						If CurrChar <> -1 Then StringTemp += Chr(CurrChar)

					Case 2, 4:
						'Store bin/hex character in temporary var
						If CurrChar <> -1 Then BinHexTemp += Chr(CurrChar)

					Case 3:
						'If in a comment, there's nothing else to read here
						Exit Do

				End Select

				CurrCharPos += 1
			Loop

			'If line ended while in a string, a string wasn't properly terminated
			If ReadType = 1 Then
				'Generate warning
				Temp = ";?F" + Str(RF) + "L" + Str(LC) + "S0" + "I" + Str(LCS) + "?"
				LogWarning(Message("NoClosingQuote"), Temp)

				'Store string
				'Check for duplicates
				SID = 0
				FOR CD = 1 to SSC
					If StringStore(CD).Value = StringTemp THEN SID = CD: EXIT FOR
				NEXT

				If SID = 0 Then
					SSC = SSC + 1
					StringStore(SSC).Value = StringTemp
					StringStore(SSC).Used = 0
					SID = SSC
				End If

				'Add reference to string to line
				CodeTemp += ";STRING" + Str(SID) + ";"

			End If

			'Put trimmed uppercase code line with comments and strings removed back into DataSource for code below
			DataSource = UCase(Trim(CodeTemp))

			'Add to count of loaded lines (main program only)
			If RF <= ICCO Then
				If DataSource <> "" Then
					MainProgramSize += 1
				End If
			End If

			'Only load line if it is valid
			T = 0
			IF DataSource = "" Then
				T = 1
			ElseIf Left(DataSource, 1) = "'" Then
				T = 1
			ElseIF Left(DataSource, 1) = ";" Then
				T = 1
			ElseIf Left(DataSource, 4) = "REM " Then
				T = 1
			ElseIF DataSource = "REM" Then
				T = 1
			
			ElseIf Left(DataSource, 8) = "#INCLUDE" Then
				T = 1
			
			'Process #asmraw directive, anything after this goes straight to asm with no processing
			ElseIf Left(DataSource, 8) = "#ASMRAW " Then
				PCC += 1: PreserveCode(PCC) = " " + Trim(Mid(DataSourceRaw, InStr(UCase(DataSourceRaw), "#ASMRAW ") + 8))
				IF S = 0 THEN MainCurrPos = LinkedListInsert(MainCurrPos, "PRESERVE " + Str(PCC))
				IF S = 1 THEN CurrPos = LinkedListInsert(CurrPos, "PRESERVE " + Str(PCC))
				
				T = 1
			End If

			'Load line
			IF T = 0 Then
				
				'Convert single-line IFs to multiple line
				IF INSTR(DataSource, "IF") <> 0 AND INSTR(DataSource, "THEN") <> 0 AND LEN(DataSource) > INSTR(DataSource, "THEN") + 3 THEN
					Do While InStr(DataSource, "THEN ") <> 0: Replace DataSource, "THEN ", "THEN: ": Loop
					Do While InStr(DataSource, " ELSE IF ") <> 0: Replace DataSource, " ELSE IF ", " :ELSE IF ": Loop
					If INSTR(DataSource, " ELSE ") <> 0 Then Replace DataSource, " ELSE ", " :ELSE: "
					DataSource = DataSource + ": END IF"
				END If

	MultiCommand:

				'Make adjustments to line if needed
				
				'Convert If( to If (
				If Left(DataSource, 3) = "IF(" Then
					DataSource = "IF (" + Mid(DataSource, 4)
				End If

				'Check that the IF has a THEN
				IF Left(DataSource, 3) = "IF " AND INSTR(DataSource, "THEN") = 0 THEN
					LogError Message("NoThen"), ";?F" + Str(RF) + "L" + Str(LC) + "?"
				END IF

				'Replace <> with ~ (not equal)
				Do While INSTR(DataSource, "<>") <> 0: Replace DataSource, "<>", "~": Loop
				'Replace => with } (equal or greater)
				Do While INSTR(DataSource, "=>") <> 0: Replace DataSource, "=>", "}": Loop
				Do While INSTR(DataSource, ">=") <> 0: Replace DataSource, ">=", "}": Loop
				'Replace =< with { (less or equal)
				Do While INSTR(DataSource, "=<") <> 0: Replace DataSource, "=<", "{": Loop
				Do While INSTR(DataSource, "<=") <> 0: Replace DataSource, "<=", "{": Loop

				'Convert DIM Value AS BIT to #define Value BitTemp.BVC
				IF Left(DataSource, 4) = "DIM " Then
					'DIM, bit variable
					If INSTR(DataSource, "AS BIT") <> 0 THEN
						Value = Mid(DataSource, INSTR(DataSource, " ") + 1)
						Value = Trim(Left(Value, INSTR(Value, "AS BIT") - 1))

						'DataSource = "#DEFINE " + Value + " SYSBITVAR" + Str(INT(BVC / 8)) + "." + Str(BVC-INT(BVC/8)*8)
						Do While Value <> ""
							ConstName = Value
							If Instr(ConstName, ",") <> 0 Then
								ConstName = Trim(Left(ConstName, Instr(ConstName, ",") - 1))
								Value = Trim(Mid(Value, Instr(Value, ",") + 1))
							Else
								Value = ""
							End If

							'If constant doesn't exist, add
							IF HashMapGet(Constants, ConstName) = 0 THEN
								AddConstant(ConstName, "SYSBITVAR" + Str(INT(BVC / 8)) + "." + Str(BVC MOD 8), Str(RF))
								CheckConstName ConstName, Origin
							END IF

							BVC = BVC + 1
						Loop

					END IF
				End If

				'Convert WORD FUNCTION x to FUNCTION x AS WORD
				IF Left(DataSource, 14) = "WORD FUNCTION " THEN
					FName = Trim(Mid(DataSource, 15))
					DataSource = "FUNCTION " + FName + " AS WORD"
				End If

				'PIC timer prescaler constant workarounds
				If InStr(DataSource, "PS") <> 0 Then
					DataSourceOld = DataSource
					ReplaceAll DataSource, "PS0_1/", "PS0_"
					ReplaceAll DataSource, "PS1_1/", "PS1_"
					ReplaceAll DataSource, "PS2_1/", "PS2_"
					ReplaceAll DataSource, "PS3_1/", "PS3_"
					ReplaceAll DataSource, "PS4_1/", "PS4_"
					ReplaceAll DataSource, "PS5_1/", "PS5_"
					ReplaceAll DataSource, "PS6_1/", "PS6_"
					ReplaceAll DataSource, "PS7_1/", "PS7_"
					ReplaceAll DataSource, "PS8_1/", "PS8_"
					ReplaceAll DataSource, "PS10_1/", "PS10_"
					ReplaceAll DataSource, "PS12_1/", "PS12_"
					
					If DataSourceOld <> DataSource Then
						'Code to show warning - enable later if any problems found with this workaround
						Temp = Message("WarningTimerConst")
						'LogWarning(Temp, ";?F" + Str(RF) + "L" + Str(LC) + "S" + Str(SBC) + "?")
					End If
				End If
				
				'Remove any tabs and double spaces (again)
				DO WHILE INSTR(DataSource, Chr(9)) <> 0: Replace DataSource, Chr(9), " ": Loop
				DO WHILE INSTR(DataSource, Chr(194)) <> 0: Replace DataSource, Chr(194), " ": Loop 'Odd character that sometimes shows up
				DO WHILE INSTR(DataSource, "  ") <> 0: Replace DataSource, "  ", " ": Loop
				DataSource = Trim(DataSource)

				'Decide if the line read is part of a sub or not
				IF Left(DataSource, 4) = "SUB " Or Left(DataSource, 9) = "FUNCTION " Or Left(DataSource, 6) = "MACRO " Then
					S = 1

					'0 = Sub, 1 = Function, 2 = Macro
					FoundFunction = 0
					FoundMacro = 0
					Do While Left(DataSource, 4) = "SUB " Or Left(DataSource, 9) = "FUNCTION " Or Left(DataSource, 6) = "MACRO "
						If Left(DataSource, 4) = "SUB " Then
							DataSource = Trim(Mid(DataSource, 4))
						ElseIf Left(DataSource, 9) = "FUNCTION " Then
							DataSource = Trim(Mid(DataSource, 9))
							FoundFunction = -1
						ElseIf Left(DataSource, 6) = "MACRO " Then
							DataSource = Trim(Mid(DataSource, 6))
							FoundMacro = -1
						End If
					Loop

					LCS = 0
					'Check for function type
					If FoundFunction Then
						'Also need to remove type def from line
						NewFNType = "BYTE"
						For T = Len(DataSource) To 1 Step -1
							'Found a ), any ASes before this will be for params
							If Mid(DataSource, T, 1) = ")" Then Exit For
							'Found an AS, get the type
							If Mid(DataSource, T, 4) = " AS " Then
								NewFNType = Trim(Mid(DataSource, T + 4))
								DataSource = Trim(Left(DataSource, T))
								Exit For
							End If
						Next
					End If
					'Get sub/function name
					CurrentSub = DataSource
					If INSTR(CurrentSub, "(") <> 0 Then CurrentSub = Trim(Left(CurrentSub, INSTR(CurrentSub, "(") - 1))

					NR = 0
					If INSTR(DataSource, "#NR") <> 0 Then
						NR = -1
						Replace DataSource, "#NR", ""
					End If
					If FoundFunction Then NR = -1

					SBC += 1
					Subroutine(SBC) = NewSubroutine(CurrentSub)
					CurrPos = Subroutine(SBC)->CodeStart
					With *Subroutine(SBC)
						.SourceFile = RF  'Source file
						.Origin = ";?F" + Str(RF) + "L" + Str(LC) + "S" + Str(SBC) + "?"

						'Function or macro?
						.IsMacro = FoundMacro
						.IsFunction = FoundFunction
						If FoundFunction Then
							.ReturnType = NewFNType
							'If length specified in type name, remove from function type
							'(Length is only needed when adding the variable)
							If InStr(.ReturnType, "*") <> 0 Then
								.ReturnType = Trim(Left(.ReturnType, InStr(.ReturnType, "*") - 1))
							End If
							AddVar .Name, NewFNType, 1, Subroutine(SBC), "REAL", .Origin, , -1
						End If

						'Is sub overloaded?
						.Overloaded = 0
						For T = 1 TO SBC - 1
							If Subroutine(T)->Name = .Name Then
								Subroutine(T)->Overloaded = -1
								.Overloaded = -1
								Exit For
							End If
						Next
						
						'Get parameters
						SubInType = ""
						Temp = Trim(Mid(DataSource, 4))
						IF INSTR(Temp, "(") <> 0 THEN
							Temp = Mid(Temp, INSTR(Temp, "(") + 1)
							FOR T = LEN(Temp) TO 1 STEP -1
								IF Mid(Temp, T, 1) = ")" THEN Temp = Left(Temp, T - 1): EXIT FOR
							Next
							Temp = Trim(Temp)

							Do While Instr(Temp, ",") <> 0
								.ParamCount += 1
								.Params(.ParamCount) = GetSubParam(Left(Temp, Instr(Temp, ",") - 1), NR)
								Temp = Mid(Temp, Instr(Temp, ",") + 1)
							Loop
							If Temp <> "" Then
								.ParamCount += 1
								.Params(.ParamCount) = GetSubParam(Temp, NR)
							End If
						End If

						'Find any large vars used as parameters
						For T = 1 To .ParamCount
							With .Params(T)
								If .Type = "STRING" Then
									AddVar .Name, .Type, 1, Subroutine(SBC), "POINTER", ";?F" + Str(RF) + "L" + Str(LC) + "S" + Str(SBC) + "?", , -1
								ElseIf Instr(.Type, "()") <> 0 Then
									Temp = .Type
									Replace Temp, "()", ""
									AddVar .Name, Temp, 2, Subroutine(SBC), "POINTER", ";?F" + Str(RF) + "L" + Str(LC) + "S" + Str(SBC) + "?", , -1
								Else
									AddVar .Name, .Type, 1, Subroutine(SBC), "REAL", ";?F" + Str(RF) + "L" + Str(LC) + "S" + Str(SBC) + "?", , -1
								End If
							End With
						Next

					End With

					GOTO LoadNextLine
				END IF

				If Left(DataSource, 7) = "END SUB" OR Left(DataSource, 9) = "END MACRO" Or Left(DataSource, 12) = "END FUNCTION" THEN
					S = 0
					CurrentSub = ""
					GOTO LoadNextLine
				END IF

				'Decide if the line read is part of a data table or not
				IF Left(DataSource, 6) = "TABLE " THEN
					'Open new data table
					S = 2

					'Get data from line
					GetTokens DataSource, LineToken(), LineTokens

					'Create table
					DataTables += 1
					With DataTable(DataTables)
						.Name = LineToken(2)
						.Origin = ";?F" + Str(RF) + "L" + Str(LC) + "S0" + "I" + Str(LCS) + "?"
						.StoreLoc = 0
						.Type = "BYTE"
						.RawItems = LinkedListCreate
						.CurrItem = .RawItems
						.Items = 0
						For T = 2 To LineTokens
							'Get store location
							If LineToken(T) = "STORE" Then
								If LineToken(T + 1) = "PROGRAM" Then
									.StoreLoc = 0
								ElseIf LineToken(T + 1) = "DATA" Then
									.StoreLoc = 1
								Else
									Temp = Message("BadTableLocation")
									Replace Temp, "%loc%", LineToken(T + 1)
									LogError Temp, .Origin
								End If
							'Get type
							ElseIf LineToken(T) = "AS" Then
								.Type = LineToken(T + 1)
							'Load table from file - get file and read
							ElseIf LineToken(T) = "FROM" Then
								.SourceFile = AddFullPath(GetString(";" + LineToken(T + 1), 0), ProgDir)
								LoadTableFromFile @DataTable(DataTables)
								S = 0
							End If
						Next
					End With

					GOTO LoadNextLine

				'End of table
				ElseIF Left(DataSource, 9) = "END TABLE" THEN
					S = 0
					GOTO LoadNextLine
				END IF

				'Does the command need to be inserted into the main routine, regardless of sub/not sub?
				ForceMain = 0
				Temp = ""

				If Left(DataSource, 1) = "#" Then
					'Automatic initialisation preparation
					IF Left(DataSource, 8) = "#STARTUP" Then
						With SourceFile(RF)
							.InitSub = Trim(Mid(DataSource, 9))
							.InitSubPriority = 100
							If InStr(.InitSub, ",") <> 0 Then
								.InitSubPriority = Val(Mid(.InitSub, InStr(.InitSub, ",") + 1))
								.InitSub = Trim(Left(.InitSub, InStr(.InitSub, ",") - 1))
							End If
						End With

						GOTO LoadNextLine

					ElseIF Left(DataSource, 7) = "#DEFINE" Then
						DataSource = DataSource + "':" + Str(RF)
						ForceMain = 1

					ElseIf Left(DataSource, 8) = "#OPTION " Then
						DataSource = Trim(Mid(DataSource, 8))
						If WholeINSTR(DataSource, "EXPLICIT") = 2 Then
							SourceFile(RF).OptionExplicit = -1
						End If
						IF gcOPTION <> "" THEN DataSource = "," + DataSource
						gcOPTION = gcOPTION + DataSource
						GoTo LoadNextLine

					ElseIF Left(DataSource, 5) = "#OSC " Then
						ForceMain = 1
					ElseIF Left(DataSource, 8) = "#CONFIG " Then
						ForceMain = 1
					ElseIf Left(DataSource, 9) = "#SAMEVAR " Then
						ForceMain = 1
					ElseIf Left(DataSource, 9) = "#SAMEBIT " Then
						ForceMain = 1
						
					ElseIf Left(DataSource, 5) = "#MEM " Then
						GoTo LoadNextLine 'Not used
					ElseIF Left(DataSource, 5) = "#RAM " Then
						GoTo LoadNextLine 'Not used
					ElseIF Left(DataSource, 5) = "#INT " Then
						GoTo LoadNextLine 'Not used

					ElseIf Left(DataSource, 6) = "#CHIP " THEN
						If ChipName = "" THEN
							ChipName = Trim(Mid(DataSource, 6))
							ChipMhz = 0
							If InStr(ChipName, ",") <> 0 Then
								ChipMhz = VAL(Mid(ChipName, INSTR(ChipName, ",") + 1))
								ChipName = Trim(Left(ChipName, INSTR(ChipName, ",") - 1))
							End If
							IF Left(UCase(ChipName), 3) = "PIC" THEN ChipName = Mid(ChipName, 4)
							IF Left(UCase(ChipName), 1) = "P" THEN ChipName = Mid(ChipName, 2)
							
						End If
						GoTo LoadNextLine
					End If

				End If

				'Split line at colons, unless line is a label or a #define
				RestOfLine = ""
				If InStr(DataSource, ":") <> 0 And Left(DataSource, 8) <> "#DEFINE " And _
					Not (Right(DataSource, 1) = ":" And IsValidName(Left(DataSource, Len(DataSource) - 1))) Then
					RestOfLine = LTrim(Mid(DataSource, INSTR(DataSource, ":") + 1))
					DataSource = RTrim(Left(DataSource, INSTR(DataSource, ":") - 1))

				END If

				DontSplitLoad:

				'Add tag to show origin of line, and make copy of line for preserve mode
				'Except for directives, ASM and labels
				IF (Left(DataSource, 1) <> "#" OR Left(DataSource, 8) = "#DEFINE ") AND Left(DataSource, 1) <> " " AND Right(DataSource, 1) <> ":" THEN
					'Add tag
					DataSource = DataSource + " ;?F" + Str(RF) + "L" + Str(LC) + "S" + Str(SBC * S) + "I" + Str(LCS) + "?"
					'Preserve
					If PreserveIn <> "" Then
						PCC += 1
						PreserveCode(PCC) = ";" + PreserveIn
						IF S = 0 OR ForceMain = 1 THEN MainCurrPos = LinkedListInsert(MainCurrPos, "PRESERVE " + Str(PCC))
						IF S = 1 AND ForceMain = 0 THEN CurrPos = LinkedListInsert(CurrPos, "PRESERVE " + Str(PCC))
						PreserveIn = ""
					End If
				END IF

				'If in sub and not forced to main, store line in sub
				IF S = 1 AND ForceMain = 0 Then
					CurrPos = LinkedListInsert(CurrPos, DataSource)
					Subroutine(SBC)->OriginalLOC += 1

				'We're in a data table, so add line to it
				ElseIf S = 2 THEN
					If INSTR(DataSource, ";") <> 0 Then DataSource = Trim(Left(DataSource, INSTR(DataSource, ";") - 1))
					With DataTable(DataTables)
						.CurrItem = LinkedListInsert(.CurrItem, DataSource)
						.CurrItem->MetaData = LineOrigin
					End With

				'Not in data table, not in sub or forced to main, so store in main
				Else
					MainCurrPos = LinkedListInsert(MainCurrPos, DataSource)
					'Only count lines in main routine of first file
					If RF = 1 Then Subroutine(0)->OriginalLOC += 1

				End If

				CommandAdded:

				IF RestOfLine <> "" THEN DataSource = RestOfLine: GOTO MultiCommand
			END IF
	LoadNextLine:
		LOOP

		IF S = 1 THEN
			S = 0
		END IF
		CLOSE
		If VBS = 1 And ShowProgressCounters Then
			CurrPerc += PercAdd
			If Int(CurrPerc) > Int(PercOld) Then
				PercOld = CurrPerc
				LOCATE , 60
				Print Int(CurrPerc);
				Print "%";
			End If
		End If

LoadNextFile:

	NEXT
	IF VBS = 1 THEN Print
	
	'Find compiler directives, except SCRIPT, ENDSCRIPT, IFDEF and ENDIF
	IF VBS = 1 THEN
		Print SPC(5); Message("CompDirs");
	END IF

	PercOld = 0
	CurrPerc = 0.5
	'PercAdd = 1 / APC * 100
	CurrPos = Subroutine(0)->CodeStart->Next
	Do While CurrPos <> 0

		Temp = CurrPos->Value

		IF Left(Temp, 1) = "#" AND INSTR(Temp, "IFDEF") = 0 AND INSTR(Temp, "IFNDEF") = 0 _
		                       And InStr(Temp, "ENDIF") = 0 And Left(Temp, 4) <> "#IF " _
		                       And Left(Temp, 9) <> "#SAMEVAR " And Left(Temp, 9) <> "#SAMEBIT " _
		                       AND InStr(Temp, "#SCRIPT") = 0 AND INSTR(Temp, "ENDSCRIPT") = 0 Then
		                       
			CurrPos = LinkedListDelete(CurrPos)

			IF Left(Temp, 7) = "#DEFINE" THEN
				Origin = ""
				IF INSTR(Temp, ";?F") <> 0 THEN
					Origin = Mid(Temp, INSTR(Temp, ";?F"))
					Temp = RTrim(Left(Temp, INSTR(Temp, ";?F") - 1))
				END IF

				ConstName = Trim(Mid(Temp, INSTR(Temp, " ") + 1))
				Temp = RTrim(Left(ConstName, INSTR(ConstName, "'") - 1))

				IF INSTR(Temp, " ") = 0 THEN
					ConstName = Temp
					TempFile = ""
					Value = ""
				Else
					Value = Mid(ConstName, INSTR(ConstName, " ") + 1)
					TempFile = Trim(Mid(Value, INSTR(Value, "':") + 2))
					Value = Trim(Left(Value, INSTR(Value, "':") - 1))
					ConstName = Left(ConstName, INSTR(ConstName, " ") - 1)
				END IF

				'Calculate define value if = present
				IF Left(Value, 1) = "=" THEN
					Value = Mid(Value, 2)
					Calculate Value
					Value = Trim(Value)
				END IF

				'Convert all non-decimal values to decimal
				If InStr(Value, "0X") <> 0 Or InStr(Value, "B'") <> 0 Then
					If IsConst(Value) Then
						Value = Str(MakeDec(Value))
					End If
				End If

				'Check to see if define exists
				IF HashMapGet(Constants, ConstName) = 0 THEN
					AddConstant(ConstName, Value, TempFile)
					CheckConstName ConstName, Origin
				END IF

			ElseIf Left(Temp, 4) = "#OSC" AND OSCType = "" THEN
				OSCType = Trim(Mid(Temp, 5))

			ElseIF Left(Temp, 7) = "#CONFIG" THEN
				Temp = Trim(Mid(Temp, 8))
				IF CONFIG <> "" THEN Temp = "," + Temp
				CONFIG = CONFIG + Temp
				
			End IF

		END IF

		CurrPos = CurrPos->Next
	Loop
	IF VBS = 1 THEN Print
	
	'Check that a chip is specified, and exit if it is not
	IF ChipName = "" THEN
		LogError Message("NoChip")
		WriteErrorLog
		END
	End IF

	'Get chip data
	IF VBS = 1 THEN PRINT SPC(5); Message("ReadChipData")
	ReadChipData
	
	'Correct clock speed
	CheckClockSpeed

	'Process #option
	ReadOptions(gcOPTION)

	'Prepare program memory page data
	PreparePageData

	'Initialise built-in data, and prepare built-in subs
	PrepareBuiltIn
	
	'Prepare programmer, need to know chip model and need to do this before checking config
	PrepareProgrammer
	
	'Force exit at this point if compilation is going to be skipped
	If FlashOnly Then
		'Do config settings in hex file need to be checked?
		If PrgTool <> 0 Then
			If PrgTool->ProgConfig <> "" Then
				'Check config settings in hex file
				'If invalid, need to force compilation
				FlashOnly = IsHexConfigValid(HFI, PrgTool->ProgConfig)
				GoTo HexConfigChecked
			End If
		End If
		
		'If no specific programmer settings, make sure LVP = Off and MCLR = Off (compiler default)
		Temp = ""
		If WholeInstr(CONFIG, "LVP") = 0 Then
			Temp = "LVP = OFF"
		End If
		If WholeInstr(CONFIG, "MCLR") = 0 Then
			If Temp <> "" Then Temp += ", "
			Temp += "MCLR = OFF"
		End If
		If Temp <> "" Then FlashOnly = IsHexConfigValid(HFI, Temp)
				
		HexConfigChecked:
	End If
	'If still skipping compilation, exit sub
	If FlashOnly Then Exit Sub

	'Display chip data
	IF VBS = 1 THEN
		PRINT
		PRINT SPC(10); Message("ChipS")
		PRINT SPC(15); Message("ChipM") + ChipName
		If ModePIC Then PRINT SPC(15); Message("ChipF") + "PIC (" + Trim(Str(ChipFamily)) + ")"
		If ModeAVR Then PRINT SPC(15); Message("ChipF") + "AVR"
		If ModeZ8 Then PRINT SPC(15); Message("ChipF") + "Z8"
		PRINT SPC(15); Message("ChipC") + Trim(Str(ChipMhz))
		PRINT SPC(15); Message("ChipR") + Trim(Str(ChipRam))
	END If

	'Find and run compiler scripts
	IF VBS = 1 THEN PRINT SPC(5); Message("RunScripts")
	RunScripts
	
	IF VBS = 1 THEN PRINT: PRINT SPC(5); Message("BuildMemoryMap")
	BuildMemoryMap
	
	'Process any #samevar and #samebit directives
	CurrPos = Subroutine(0)->CodeStart->Next
	Do While CurrPos <> 0
		Temp = Left(CurrPos->Value, 9)
		If Temp = "#SAMEVAR " Or Temp = "#SAMEBIT " Then
			ProcessSame CurrPos->Value
			CurrPos = LinkedListDelete(CurrPos)
		End If
		
		CurrPos = CurrPos->Next
	Loop

	'Remove any #IFDEFs that do not apply to the program
	IF VBS = 1 THEN PRINT : PRINT SPC(5); Message("RemIfDefs")
	RemIfDefs
	
	'Replace constants with their values
	If VBS = 1 THEN PRINT SPC(5); Message("RepDefs");
	ReplaceConstants
	IF VBS = 1 THEN PRINT
	
	'Replace constants and calculations in tables with actual values
	ReadTableValues
	
End SUB

Sub ProcessSame (DirectiveIn As String)
	'Process #samevar and #samebit directives
	'Need to find var (or bit) that exists from a list coming in, and create constants mapping all others to that
	Dim SearchBit As Integer
	Dim As String InputList, FoundName
	Dim As LinkedListElement Pointer SearchList, SearchListPos
	
	'Decide what to search for
	SearchBit = 0
	If Left(UCase(DirectiveIn), 8) = "#SAMEBIT" Then SearchBit = -1
	InputList = Trim(Mid(DirectiveIn, 9))
	SearchList = GetElements(InputList)
	
	'Find which item actually exists
	FoundName = ""
	SearchListPos = SearchList->Next
	Do While SearchListPos <> 0
		SearchListPos->NumVal = 0
		If (SearchBit AndAlso HasSFRBit(SearchListPos->Value)) OrElse ((Not SearchBit) AndAlso HasSFR(SearchListPos->Value)) Then
			SearchListPos->NumVal = -1
			If FoundName = "" Then FoundName = SearchListPos->Value
		End If
		
		SearchListPos = SearchListPos->Next
	Loop
	
	'Map all non-existing items to the first one that exists
	If FoundName <> "" Then
		SearchListPos = SearchList->Next
		Do While SearchListPos <> 0
			If SearchListPos->NumVal = 0 Then
				AddConstant(SearchListPos->Value, FoundName, "")
			End If
			
			SearchListPos = SearchListPos->Next
		Loop
	End If
	
	LinkedListDeleteList(SearchList, 0)
End Sub

Sub ReadTableValues

	Dim As Integer CurrTable, T
	Dim As String Temp, Value, OutMessage
	Dim As LinkedListElement Pointer CurrLine
	Dim As OriginType Pointer Origin

	'Search each table
	For CurrTable = 1 To DataTables
		With DataTable(CurrTable)

			'Parse raw values
			CurrLine = .RawItems->Next
			Do While CurrLine <> 0

				Origin = CurrLine->MetaData

				'Split line at commas
				Temp = CurrLine->Value

				T = -1
				Do While T
					If InStr(Temp, ",") <> 0 Then
						Value = RTrim(Left(Temp, InStr(Temp, ",") - 1))
						Temp = LTrim(Mid(Temp, InStr(Temp, ",") + 1))
					Else
						Value = Temp
						T = 0
					End If

					'is this an string a decimal ?
					if InStr(Value, ".") <> 0  then
						OutMessage = Message("TableItemInvalid")
						Replace OutMessage, "%item%", Value
						LogError(OutMessage, GetOriginString(Origin))
					end if

					'added to resolve empty tables in Linux build
					If Left(Value, 2) = "0X" Then
						'Convert to Hex string... so, this works on Linux builds
						Value = str(VAL("&H"+mid(Value,3)))
					end if


					'Is there a leading 0... for bytes...
					If Left(Value, 1) = "0" and Left(Value, 2) <> "0X" Then
						Value = str(VAL(Value))
					end if

					'Are constants or calculations present?
					If Not IsConst(Value) Then
						Value = ReplaceConstantsLine(Value, 0)
					End If
					If IsCalc(Value) Then
						Calculate(Value)
					End If

					If IsConst(Value) Then
						'Check that data can be stored in table, upgrade table if it can't
						' Print .Type, DataSource, TypeOfValue(DataSource, 0)
						If CastOrder(TypeOfValue(Value, 0)) > CastOrder(.Type) Then
							.Type = TypeOfValue(Value, 0)
						End If

						.Items += 1
						.Item(.Items) = MakeDec(Value)
					Else
						OutMessage = Message("TableItemInvalid")
						Replace OutMessage, "%item%", Value
						LogError(OutMessage, GetOriginString(Origin))
					End If

				Loop

				CurrLine = CurrLine -> Next
			Loop

		End With
	Next

End Sub

SUB RemIfDefs
	'Remove IFDEFs for which the condition is false
	Dim As String Temp, TVar, Value, Cmd, OldCmd
	Dim As Integer ForceMain, IL, DelMode, PMode, SV, FV, ConstFound, RecDetect
	Dim As Integer FC, DC, VF, SD, CheckValue, VC, TV, CD, EV, CurrSub
	Dim As ConstMeta Pointer FoundConst
	Dim As LinkedListElement Pointer CurrLine, StartDel, EndDel, CurrPos', SearchConstPos

	'Need to scan through main program and all subs
	For CurrSub = 0 To SBC
		CurrLine = Subroutine(CurrSub)->CodeStart->Next

		IL = 0
		Do While CurrLine <> 0

	RemIfDef:
				Temp = UCase(CurrLine->Value)
				DelMode = 0 '1 if condition tests false, 2 if true
				PMode = 0 '0 if IFDEF, 1 if IFNDEF, 2 if IF

				IF (Left(Temp, 7) = "#IFDEF " OR Left(Temp, 8) = "#IFNDEF " Or Left(Temp, 4) = "#IF ") AND IL = 0 THEN

					IF INSTR(Temp, ";") <> 0 THEN Temp = Left(Temp, INSTR(Temp, ";") - 1)
					StartDel = CurrLine

					'Read condition
					If Left(Temp, 4) = "#IF " Then
						Cmd = Trim(Mid(Temp, 5))
						PMode = 2
					ElseIf Left(Temp, 7) = "#IFDEF " Then
						Cmd = Trim(Mid(Temp, 8))
						PMode = 0
					Else
						Cmd = Trim(Mid(Temp, 9))
						PMode = 1
					End If

					'IF
					If PMode = 2 Then
						OldCmd = ""
						RecDetect = 0
						'Print "Start:", Cmd
						Do While OldCmd <> Cmd
							OldCmd = Cmd
							Cmd = ReplaceConstantsLine(CheckSysVarDef(Cmd), 0)
							'Print "Now:", Cmd
							RecDetect += 1
							'If have looped too many times, there is probably a recursive define
							If RecDetect > 100 Then Exit Do
						Loop
						Calculate Cmd
						'Print "Result:", Cmd
						If Val(Cmd) = 0 Then
							DelMode = 1
						Else
							DelMode = 2
						End If

					'IFDEF/IFNDEF
					Else
						'Test for SFR bit?
						IF INSTR(Cmd, "BIT(") <> 0 THEN
							'Get name of bit, and whether checking for presence or absence
							FV = 0: IF INSTR(Cmd, "NOBIT(") <> 0 THEN FV = 1
							Temp = Mid(Cmd, INSTR(Cmd, "(") + 1)
							Temp = Trim(Left(Temp, INSTR(Temp, ")") - 1))

							'Search for bit in list
							ConstFound = HasSFRBit(ReplaceConstantsLine(Temp, 0))

							'Set DelMode appropriately
							If FV = 0 Then
								If ConstFound = 0 Then DelMode = 1 Else DelMode = 2
							Else
								If ConstFound = 0 Then DelMode = 2 Else DelMode = 1
							End If
							GOTO IfDefProcessed
						END IF

						'Test for SFR?
						IF INSTR(Cmd, "VAR(") <> 0 THEN
							'Get name of SFR, and checking mode
							FV = 0: IF INSTR(Cmd, "NOVAR(") <> 0 THEN FV = 1
							Temp = Mid(Cmd, INSTR(Cmd, "(") + 1)
							Temp = Trim(Left(Temp, INSTR(Temp, ")") - 1))

							ConstFound = HasSFR(ReplaceConstantsLine(Temp, 0))

							'Set DelMode appropriately
							If FV = 0 Then
								If ConstFound = 0 Then DelMode = 1 Else DelMode = 2
							Else
								If ConstFound = 0 Then DelMode = 2 Else DelMode = 1
							End If
							GOTO IfDefProcessed
						END IF

						'Test to see if one or all of several constants have been defined
						IF INSTR(Cmd, "ONEOF(") <> 0 OR INSTR(Cmd, "ALLOF(") <> 0 THEN
							FV = 0: IF INSTR(Cmd, "ALLOF(") <> 0 THEN FV = 1

							'Get list of defines to search for
							Temp = Mid(Cmd, INSTR(Cmd, "(") + 1)
							Temp = UCase(Left(Temp, INSTR(Temp, ")") - 1))
							DC = 0
							DO WHILE INSTR(Temp, ",")
								DC = DC + 1
								TempData(DC) = Trim(Left(Temp, INSTR(Temp, ",") - 1))
								Temp = Mid(Temp, INSTR(Temp, ",") + 1)
							LOOP
							DC = DC + 1
							TempData(DC) = Trim(Temp)

							'Search
							VF = 0
							FOR SD = 1 to DC
								If HashMapGet(Constants, TempData(SD)) <> 0 Then
									VF = VF + 1
								End If
							NEXT

							'Decide outcome
							DelMode = 1 'Default, condition false
							IF FV = 0 AND VF > 0 THEN DelMode = 2 'Needed at least 1, 1 found
							IF FV = 1 AND VF = DC THEN DelMode = 2 'Needed all, found all
							GOTO IfDefProcessed
						END IF

						'All functions have been checked, so must now check plain constants
						'Does the value of the constant need to be checked?
						CheckValue = 0
						IF INSTR(Cmd, " ") <> 0 THEN
							CheckValue = 1
							Value = Mid(Cmd, INSTR(Cmd, " ") + 1)
							Cmd = Left(Cmd, INSTR(Cmd, " ") - 1)
						END IF

						'Don't check value, just see if constant exists
						IF CheckValue = 0 THEN
							DelMode = 1
							If HashMapGet(Constants, Cmd) <> 0 Then
								DelMode = 2
							End If
							GOTO IfDefProcessed
						END IF

						'Check to see if value matches desired value
						'Get list of values
						VC = 0
						DO WHILE INSTR(Value, ",") <> 0
							VC = VC + 1
							TempData(VC) = Trim(Left(Value, INSTR(Value, ",") - 1))
							Value = Mid(Value, INSTR(Value, ",") + 1)
						LOOP
						VC = VC + 1
						TempData(VC) = Trim(Value)

						'Replace names of test constants with values
						FOR SD = 1 TO VC
							FoundConst = HashMapGet(Constants, TempData(SD))
							IF FoundConst <> 0 Then
								TempData(SD) = UCase(FoundConst->Value)
							End If
						Next

						TVar = UCase(ReplaceConstantsLine(Cmd, 0))

						'Compare real and test values
						DelMode = 1
						FOR SD = 1 TO VC
							IF TempData(SD) = TVar THEN DelMode = 2: EXIT FOR
						Next
					End If

					'Remove whatever needs to be removed
					'DelMode = 1 > condition is false > remove all
					'DelMode = 2 > condition is true > remove just #ifdef and #endif
	IfDefProcessed:
					'Swap DelMode when mode is #IFNDEF (1)
					If PMode = 1 Then
						If DelMode = 1 Then
							DelMode = 2
						ElseIf DelMode = 2 Then
							DelMode = 1
						End If
					End If

					'Find End Location
					CurrPos = StartDel
					EndDel = StartDel
					IL = 0
					Do While CurrPos <> 0
						Temp = UCase(CurrPos->Value)
						IF Left(Temp, 7) = "#IFDEF " Or Left(Temp, 8) = "#IFNDEF " Or Left(Temp, 4) = "#IF " Then
							 IL = IL + 1
						End If
						IF Left(Temp, 6) = "#ENDIF" THEN IL = IL - 1
						If IL = 0 Then EndDel = CurrPos: Exit Do
						CurrPos = CurrPos->Next
					Loop

					'Remove everything up to the #ENDIF
					IF DelMode = 1 THEN
						'DelLines SV, EV
						'DS = DS - 1
						CurrLine = StartDel->Prev
						LinkedListDeleteList(StartDel, EndDel)
					END IF

					'Remove the IFDEF and corresponding #ENDIF
					IF DelMode = 2 THEN
						'DelLine EV
						'DelLine SV
						'DS = DS - 1
						CurrLine = StartDel->Prev
						LinkedListDelete(StartDel)
						LinkedListDelete(EndDel)
					END IF

				END IF

			If CurrLine <> 0 Then CurrLine = CurrLine->Next
		Loop
	Next

END SUB

SUB ReplaceConstants

	Dim As String Origin, SourceData, LeftSection
	Dim As Integer CurrSub
	Dim As LinkedListElement Pointer CurrLine
	Dim As Single CurrPerc, PercAdd, PercOld
	PercOld = 0
	CurrPerc = 0.5
	PercAdd = 1 / (SBC + 1) * 100

	'Replace constants with their values
	FOR CurrSub = 0 To SBC

		IF VBS = 1 And ShowProgressCounters Then
			CurrPerc += PercAdd
			If Int(CurrPerc) > Int(PercOld) Then
				PercOld = CurrPerc
				LOCATE , 60
				Print Int(CurrPerc);
				Print "%";
			End If
		End If

		CurrLine = Subroutine(CurrSub)->CodeStart->Next
		Do While CurrLine <> 0
			SourceData = CurrLine->Value

			LeftSection = ""
			IF Left(SourceData, 3) = "ON " THEN LeftSection = "ON ": SourceData = Mid(SourceData, 4)

			'Attempt to replace constants in line
			SourceData = ReplaceConstantsLine(SourceData, -1)

			CurrLine->Value = LeftSection + SourceData
			CurrLine = CurrLine->Next
		Loop
	Next

End SUB

Function ReplaceConstantsLine (ByRef DataSourceIn As String, IncludeStartup As Integer) As String

	Dim As String DataSource, Startup, Origin
	Dim As Integer LineChanged, RepCount
	Dim As LinkedListElement Pointer LineElements, CurrElement
	Dim As ConstMeta Pointer Meta

	RepCount = 0
	LineChanged = 0
	Startup = ""

	'Get all elements from line
	LineElements = GetElements(DataSourceIn, , -1)

	'Check each one to see if it is a constant
	CurrElement = LineElements->Next
	Do While CurrElement <> 0
		Meta = HashMapGet(Constants, UCase(CurrElement->Value))
		If Meta <> 0 Then
			'Found constant, replace

			RepCount += 1
			'Prevent recursion from crashing compiler
			If RepCount > 100 Then
				Origin = ""
				IF INSTR(DataSourceIn, ";?F") <> 0 THEN Origin = Mid(DataSourceIn, INSTR(DataSourceIn, ";?F"))
				If INSTR(Origin, ";STARTUP") <> 0 Then Origin = Left(Origin, INSTR(Origin, ";STARTUP") - 1)
				LogError Message("RecursiveDefine"), Origin
				CurrElement = LinkedListDelete(CurrElement)

			Else
				CurrElement = LinkedListDelete(CurrElement)
				LinkedListInsertList(CurrElement, GetElements(Meta->Value, , -1))
				LineChanged = -1

				If Meta->Startup <> "" AndAlso InStr(Startup, Meta->Startup) = 0 THEN
					If IncludeStartup Then
						Startup = Startup + ";STARTUP" + Meta->Startup
					End If
				End If
			End If

		End If
		CurrElement = CurrElement->Next
	Loop

	'Update line
	If LineChanged Then
		DataSource = ""
		CurrElement = LineElements->Next
		Do While CurrElement <> 0
			DataSource = DataSource + CurrElement->Value
			CurrElement = CurrElement->Next
		Loop
	Else
		DataSource = DataSourceIn
	End If

	LinkedListDeleteList(LineElements, 0)

	Return DataSource + Startup
End Function

SUB RunScripts

	Dim As String V1, Act, V2, Condition
	Dim As String CO, COCR, OCO, TempData, OtherData, MoreData
	Dim As String OutVar, Value, Origin
	Dim As Integer PD, ReadScript, CondFalse, TL, FC, CD
	Dim As Integer CurrSub, IsError
	Dim As LinkedListElement Pointer CurrLine, SearchConstPos

	Dim As LinkedListElement Pointer ScriptCode, ScriptCodePos

	'Read Scripts
	ScriptCode = LinkedListCreate
	ScriptCodePos = ScriptCode
	For CurrSub = 0 To SBC
		CurrLine = Subroutine(CurrSub)->CodeStart->Next
		Do While CurrLine <> 0

			IF CurrLine->Value = "#SCRIPT" THEN
				ReadScript = -1
				CurrLine = LinkedListDelete(CurrLine)
			ElseIf CurrLine->Value = "#ENDSCRIPT" Then
				ReadScript = 0
				CurrLine = LinkedListDelete(CurrLine)
			ElseIf ReadScript Then
				ScriptCodePos = LinkedListInsert(ScriptCodePos, CurrLine->Value)
				CurrLine = LinkedListDelete(CurrLine)
			End If

			If CurrLine <> 0 Then CurrLine = CurrLine->Next
		Loop
	Next

	'Execute Script
	ScriptCodePos = ScriptCode->Next
	Do While ScriptCodePos <> 0
		CO = ScriptCodePos->Value

		'Get origin
		Origin = ""
		IF InStr(CO, ";?F") <> 0 Then
			Origin = Trim(Mid(CO, InStr(CO, ";?F")))
			CO = Trim(Left(CO, INSTR(CO, ";?F") - 1))
		End If

		'Prepare a version of the command with constants replaced by values
		COCR = CO
		Do
			OCO = COCR
			COCR = ReplaceConstantsLine(COCR, 0)
		Loop While OCO <> COCR
		IF INSTR(COCR, ";?F") <> 0 THEN COCR = Left(COCR, INSTR(COCR, ";?F") - 1)

		'IF
		IF Left(CO, 3) = "IF " THEN
			Condition = Mid(COCR, 4)
			IF INSTR(Condition, "THEN") <> 0 THEN Condition = Left(Condition, INSTR(Condition, "THEN") - 1)
			Condition = Trim(Condition)
			CondFalse = -1
			Condition = CheckSysVarDef(Condition)
			If IsCalc(Condition) Then
				Calculate Condition
				If Val(Condition) <> 0 Then CondFalse = 0

			'If no action, check for constant defined
			Else
				Condition = Mid(CO, 4)
				IF INSTR(Condition, "THEN") <> 0 THEN Condition = Left(Condition, INSTR(Condition, "THEN") - 1)
				Condition = Trim(Condition)
				If HashMapGet(Constants, Condition) <> 0 Then
					CondFalse = 0
				End If
			End If

			If CondFalse Then
				TL = 1
				ScriptCodePos = ScriptCodePos->Next
				DO WHILE TL > 0 And ScriptCodePos <> 0
					TempData = ScriptCodePos->Value
					IF Left(TempData, 3) = "IF " THEN TL = TL + 1
					IF Left(TempData, 6) = "END IF" THEN TL = TL - 1
					ScriptCodePos = ScriptCodePos->Next
				LOOP
				'GCBASIC has come to end of script without closing end if, display error!
				IF TL > 0 Then
					TempData = Message("NoENDIF")
					LogError(TempData, Origin)
				ElseIf ScriptCodePos <> 0 Then
					ScriptCodePos = ScriptCodePos->Prev
				End If
			END IF
		END IF

		'ERROR
		IF Left(CO, 6) = "ERROR " Or Left(CO, 8) = "WARNING " Then
			IsError = Left(CO, 6) = "ERROR "
			If IsError Then
				TempData = Mid(COCR, 7)
			Else
				TempData = Mid(COCR, 9)
			End If

			Do While INSTR(TempData, ";STRING") <> 0
				OtherData = Mid(TempData, INSTR(TempData, ";") + 1)
				OtherData = ";" + Left(OtherData, INSTR(OtherData, ";"))
				Replace TempData, OtherData, StringStore(VAL(Mid(OtherData, 8))).Value
			Loop
			Do While INSTR(TempData, "MSG(") <> 0
				OtherData = Mid(TempData, INSTR(TempData, "MSG("))
				OtherData = Left(OtherData, INSTR(OtherData, ")"))
				MoreData = MID(OtherData, 5, LEN(OtherData) - 5)
				Replace TempData, OtherData, Message(MoreData)
			Loop

			If IsError Then
				LogError TempData
			Else
				LogWarning TempData
			EndIf
		END IF

		'CALCULATE
		IF INSTR(CO, "=") <> 0 AND Left(CO, 3) <> "IF " THEN
			'Get data and output name
			OutVar = Trim(Left(CO, INSTR(CO, "=") - 1))
			Value = Trim(Mid(COCR, INSTR(COCR, "=") + 1))

			'Check if the data is a sum, and calculate if it is
			DO WHILE INSTR(Value, "&") <> 0: Replace Value, "&", "AND": LOOP

			If IsCalc(Value) THEN
				Calculate Value
				Value = Trim(UCase(Value))
			End If

			'Retrieve string/s
			Do While INSTR(Value, ";STRING") <> 0
				TempData = Mid(Value, INSTR(Value, ";") + 1)
				TempData = ";" + Left(TempData, INSTR(TempData, ";"))
				Replace Value, TempData, StringStore(VAL(Mid(TempData, 8))).Value
			Loop

			'Write the data to the output
			AddConstant(OutVar, Trim(Value), , -1)
		End If

		'May have been forced to 0 by missing end if
		If ScriptCodePos <> 0 Then
			ScriptCodePos = ScriptCodePos->Next
		End If
	Loop

END Sub

Sub TidyInputSource (CompSub As SubType Pointer)
	Dim As String Value, Temp, Origin, InLine
	Dim As LinkedListElement Pointer CurrLine
	
	CurrLine = CompSub->CodeStart
	Do While CurrLine <> 0
		
		'Remove LET commands
		If Left(CurrLine->Value, 4) = "LET " Then
			CurrLine->Value = Trim(Mid(CurrLine->Value, 5))
		
		'Remove SET if line contains =
		ElseIF Left(CurrLine->Value, 4) = "SET " And INSTR(CurrLine->Value, "=") <> 0 Then
			CurrLine->Value = Trim(Mid(CurrLine->Value, 5))

		ElseIF Left(CurrLine->Value, 13) = "EXIT FUNCTION" Then
			CurrLine->Value = "EXIT SUB"

		'Convert WHILE and WEND to DO WHILE and LOOP
		ElseIF Left(CurrLine->Value, 6) = "WHILE " Then
			CurrLine->Value = "DO " + CurrLine->Value
		ElseIf Left(CurrLine->Value, 5) = "WEND;" Then
			CurrLine->Value = "LOOP"

		'Convert DO FOREVER To DO
		ElseIF Left(CurrLine->Value, 10) = "DO FOREVER" Then
			CurrLine->Value = "DO"

		'Convert LEFT and RIGHT in ROTATE command to LC or RC
		'Convert LEFT SIMPLE and RIGHT SIMPLE to L or R
		'(Prevents conflict with LEFT and RIGHT string functions
		ElseIf Left(CurrLine->Value, 7) = "ROTATE " Then
			Origin = ""
			InLine = CurrLine->Value
			IF INSTR(CurrLine->Value, ";?F") <> 0 THEN
				Origin = Mid(CurrLine->Value, INSTR(CurrLine->Value, ";?F"))
				InLine = RTrim(Left(CurrLine->Value, INSTR(CurrLine->Value, ";?F") - 1))
			END If
			
			If Right(InLine, 6) = " RIGHT" Then
				CurrLine->Value = Left(InLine, Len(InLine) - 6) + " RC" + Origin
			ElseIf Right(InLine, 5) = " LEFT" Then
				CurrLine->Value = Left(InLine, Len(InLine) - 5) + " LC" + Origin
			ElseIf Right(InLine, 13) = " RIGHT SIMPLE" Then
				CurrLine->Value = Left(InLine, Len(InLine) - 13) + " R" + Origin
			ElseIf Right(InLine, 12) = " LEFT SIMPLE" Then
				CurrLine->Value = Left(InLine, Len(InLine) - 12) + " L" + Origin
			End If
		End If

		'Replace ++, --, +=, -=
		If INSTR(CurrLine->Value, "++") <> 0 THEN
			Origin = ""
			IF INSTR(CurrLine->Value, ";?F") <> 0 Then Origin = Mid(CurrLine->Value, INSTR(CurrLine->Value, ";?F"))
			Value = Trim(Left(CurrLine->Value, INSTR(CurrLine->Value, "++") - 1))
			CurrLine->Value = Value + "=" + Value + "+1" + Origin
		ElseIf INSTR(CurrLine->Value, "--") <> 0 THEN
			Origin = ""
			IF INSTR(CurrLine->Value, ";?F") <> 0 Then Origin = Mid(CurrLine->Value, INSTR(CurrLine->Value, ";?F"))
			Value = Left(CurrLine->Value, INSTR(CurrLine->Value, "--") - 1)
			CurrLine->Value = Value + "=" + Value + "-1" + Origin
			
		ElseIf INSTR(CurrLine->Value, "+=") <> 0 THEN
			Value = Left(CurrLine->Value, INSTR(CurrLine->Value, "+=") - 1)
			Temp = Mid(CurrLine->Value, INSTR(CurrLine->Value, "+=") + 2)
			CurrLine->Value = Value + "=" + Value + "+" + Temp
		ElseIF INSTR(CurrLine->Value, "-=") <> 0 THEN
			Value = Left(CurrLine->Value, INSTR(CurrLine->Value, "-=") - 1)
			Temp = Mid(CurrLine->Value, INSTR(CurrLine->Value, "-=") + 2)
			CurrLine->Value = Value + "=" + Value + "-" + Temp
		END If
		
		CurrLine = CurrLine->Next
	Loop
End Sub
