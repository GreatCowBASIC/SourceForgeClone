'
' Created by SharpDevelop.
' User: Hugh
' Date: 3/02/2009
' Time: 10:49 PM
'
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.IO
Imports System.Windows.Forms
'Imports System.Collections
Imports System.Collections.Generic
Imports System.Text
Imports System.Text.RegularExpressions

''Namespace Great_Cow_Graphical_BASIC

Public Class GCBProgram
	
	'Program storage arrays
	Public Dim Subroutines As List(Of GCBSubroutine)
	
	Public Dim Constants As List(Of Setting)
	Public Dim ChipName, ChipConfig As String
	Public Dim ChipSpeed As Double
	'Public Dim ProgVariables(MainForm.MaxVars, 2) As String
	Public Dim Variables As VariableList
	Public Dim ProgTables(MainForm.MaxTables, MainForm.MaxTableElements) As String
	Public Dim ProgInterrupts As List(Of InterruptHandler)
	Public Dim ProgramDir As String
	
	'Documentation for program
	Public Dim About As DocSection
	
	'Local constant groups
	'Used for A/D channels, and anything else that needs to be generated for a particular chip
	Private Dim LocalConstGroups As List(Of ConstGroup)
	
	'Program size counters
	'Public Dim VarCount As Integer
	Public Dim TableCount As Integer
	Public Dim TableSize(MainForm.MaxTables) As Integer
	
	'Misc Settings
	Public Dim StartupSub As String = ""
	Public Dim UseBootloader As Boolean = False
	Public Dim OtherOptions As String = ""
	Public Dim BootloaderPos As String = ""
	Public Dim Device As HardwareDevice
	
	'Used to display error locations in Main
	Public Dim FirstCodeLine As Integer = 0
	
	'Program changed detection
	Public Dim HasChanged As Boolean
	
	'Filename
	Public Dim FileName As String
	Private Dim MainSub As GCBSubroutine
	
	'Flag to store whether program can be written to
	'Set if file is read only or on drive GCGB cannot write to
	Public Dim FileLocked As Boolean
	
	'Undo/Redo History
	Public Dim UndoRedoHistory(MainForm.MaxUndo) As List(Of UndoHistoryItem)
	Public Dim UndoRedoCount As Integer
	Public Dim UndoRedoLocation As Integer
	
	'Library data storage arrays
	Public Dim Libraries As List(Of LibraryType)
	Private Dim RequestLibraryReload As Boolean
	
	Public Dim CurrentCat As Integer
	Public Dim CurrentListScroll As Integer
	
	'Reference to whatever is editing this object
	Public Dim Editor As EditorPanel
	
	'Chip data file for current chip
	Public Dim ChipData As ChipInfo
	
	'Event for when lib added
	Public Event LibraryListChanged
	Public Sub FireLibraryListChanged
		Try
			RaiseEvent LibraryListChanged
		Catch
			
		End Try
	End Sub
	
	Public Sub New(Editor As EditorPanel)
		'Create ArrayLists
		Subroutines = New List(Of GCBSubroutine)
		Libraries = New List(Of LibraryType)
		Variables = New VariableList
		ProgInterrupts = New List(Of InterruptHandler)
		Constants = New List(Of Setting)
		LocalConstGroups = New List(Of ConstGroup)
		
		'Create main sub
		MainSub = New GCBSubroutine("Main")
		Subroutines.Add(MainSub)
		
		'Set reference to editor of this file
		Me.Editor = Editor
		
	End Sub
	
	Public Function GetTextProgram As String
		Dim TempData As String = ""
		Dim LineTemp As String
		Dim Temp, InnerTemp, CurrentIndent, NewIndent, CurrLineNo As Integer
		Dim HeaderWritten As Boolean
		'Dim IndentSpace As String = "                                                   "
		Dim OutCode As New StringBuilder
		
		Dim IsLibrary As Boolean = False
		If FileName.ToLower.EndsWith(".h") Then IsLibrary = True
		
		'Header
		CurrLineNo = 0
		If Not About Is Nothing Then
			Dim DescLines() As String = About.Description.Split(Environment.NewLine)
			Dim MarkHeaderEnd As Boolean = False
			For Each LineTemp In DescLines
				OutCode.AppendLine("'''" + LineTemp.Trim)
				MarkHeaderEnd = True
			Next
			If About.Author <> "" Then
				OutCode.AppendLine("'''@author " + About.Author)
				MarkHeaderEnd = True
			End If
			If About.Licence <> "" Then
				OutCode.AppendLine("'''@licence " + About.Licence)
				MarkHeaderEnd = True
			End If
			If About.Version <> "" Then
				OutCode.AppendLine("'''@version " + About.Version)
				MarkHeaderEnd = True
			End If
			If MarkHeaderEnd Then
				OutCode.AppendLine("'''***************************************************************************")
				OutCode.AppendLine
			End If
		End If
		
		'Chip Settings
		CurrLineNo += 2
		HeaderWritten = False
		If ChipName <> "" Or ChipSpeed <> 0 Then
			OutCode.Append(";Chip Settings" + Environment.NewLine)
			HeaderWritten = True
			If GetCurrentChipSpeed = 0 Then
				OutCode.Append("#chip " + GetCurrentChip + Environment.NewLine): CurrLineNo += 1
			Else
				OutCode.Append("#chip " + GetCurrentChip + "," + GetCurrentChipSpeed.ToString + Environment.NewLine): CurrLineNo += 1
			End If
		End If
		If ChipConfig <> "" Then
			If Not HeaderWritten Then
				OutCode.Append(";Chip Settings" + Environment.NewLine)
				HeaderWritten = True
			End If
			OutCode.Append("#config " + ChipConfig + Environment.NewLine): CurrLineNo += 1
		End If
		If UseBootloader Or OtherOptions <> "" Then
			If Not HeaderWritten Then
				OutCode.Append(";Chip Settings" + Environment.NewLine)
				HeaderWritten = True
			End If
			If UseBootloader And OtherOptions <> "" Then
				OutCode.Append("#option bootloader " + BootloaderPos + " " + OtherOptions + Environment.NewLine): CurrLineNo += 1
			Else If UseBootloader Then
				OutCode.Append("#option bootloader " + BootloaderPos + Environment.NewLine): CurrLineNo += 1
			Else
				OutCode.Append("#option " + OtherOptions + Environment.NewLine): CurrLineNo += 1
			End If
		End If
		If HeaderWritten Then OutCode.Append(Environment.NewLine)
		
		'If there are interrupt handlers and this is a library, make sure there is an init routine
		If StartupSub = "" And IsLibrary And ProgInterrupts.Count > 0 Then
			'Generate sub name from filename
			StartupSub = FileName
			If StartupSub.IndexOf(".") <> -1 Then
				StartupSub = StartupSub.Substring(0, StartupSub.IndexOf("."))
			End If
			If StartupSub.IndexOfAny("/\") <> -1 Then
				StartupSub = StartupSub.Substring(StartupSub.LastIndexOfAny("/\") + 1)
			End If
			StartupSub = "Init" + StartupSub.Substring(0, 1).ToUpper + StartupSub.Substring(1)
			
			'Add routine
			Dim InitRoutine As New GCBSubroutine(StartupSub)
			InitRoutine.HideSub = True
			Subroutines.Add(InitRoutine)
			
		End If
		
		'Initialisation routine
		If StartupSub <> "" Then
			OutCode.Append(";Startup routine" + Environment.NewLine)
			OutCode.Append("#startup " + StartupSub + Environment.NewLine): CurrLineNo += 1
			OutCode.Append(Environment.NewLine)
		End If
		
		'Includes
		If Libraries.Count > 0 Then
			OutCode.Append(";Include files (Libraries)" + Environment.NewLine): CurrLineNo += 1
			Dim SaveLib As LibraryType
			For Each SaveLib In Libraries
				If SaveLib.FileName <> "" Then
					OutCode.Append("#include " + SaveLib.FileName + Environment.NewLine): CurrLineNo += 1
				End If
			Next
			OutCode.Append(Environment.NewLine): CurrLineNo += 1
		End If
		
		'Hardware settings
		If Not Device Is Nothing Then
			Dim CurrSetting As HardwareDeviceSetting
			For Each CurrSetting In Device.Settings
				With CurrSetting
					OutCode.Append("'''@hardware " + .Condition + "; " + .Name + "; " + .Constant + "; " + .ParamType + Environment.NewLine): CurrLineNo += 1
				End With
			Next
			OutCode.Append(Environment.NewLine): CurrLineNo += 1
		End If
		
		'Defines
		If Constants.Count > 0 Then
			OutCode.Append(";Defines (Constants)" + Environment.NewLine): CurrLineNo += 1
			Dim currConst As Setting
			For Each currConst In Constants
				TempData = currConst.Name.Trim
				If TempData <> "" Then
					OutCode.Append("#define " + TempData + " " + currConst.Value + Environment.NewLine): CurrLineNo += 1
				End If
			Next
			OutCode.Append(Environment.NewLine): CurrLineNo += 1
		End If
		
		'Vars
		If Variables.Count > 0 Then
			OutCode.Append(";Variables" + Environment.NewLine): CurrLineNo += 1
			Dim currVar As Integer
			For currVar = 0 To Variables.Count - 1
				With Variables.GetItem(currVar)
					If .Hidden Then
						OutCode.Append("'''@hide" + Environment.NewLine): CurrLineNo += 1
					End If
					OutCode.Append(.Code + Environment.NewLine): CurrLineNo += 1
				End With
			Next
			OutCode.Append(Environment.NewLine): CurrLineNo += 1
		End If
		
		'Interrupt handlers
		If Not IsLibrary Then
			Dim CurrHandler As InterruptHandler
			If ProgInterrupts.Count > 0 Then
				OutCode.Append(";Interrupt Handlers" + Environment.NewLine): CurrLineNo += 1
				For Each CurrHandler In ProgInterrupts
					OutCode.Append("On Interrupt " + CurrHandler.IntEvent + " Call " + CurrHandler.HandlerSub + Environment.NewLine)
					CurrLineNo += 1
				Next
				OutCode.Append(Environment.NewLine): CurrLineNo += 1
			End If
		End If
		
		'Data tables
		If TableCount > 0 Then
			OutCode.Append(";Data tables" + Environment.NewLine): CurrLineNo += 1
			For Temp = 1 To TableCount
				OutCode.Append("Table " + ProgTables(Temp, 0) + Environment.NewLine): CurrLineNo += 1
				For InnerTemp = 1 To TableSize(Temp)
					OutCode.Append(" " + ProgTables(Temp, InnerTemp).Trim + Environment.NewLine): CurrLineNo += 1
				Next
				OutCode.Append("End Table" + Environment.NewLine): CurrLineNo += 1
				OutCode.Append(Environment.NewLine): CurrLineNo += 1
			Next
		End If
		
		'Subroutines
		'FirstCodeLine = CurrLineNo + 1
		Dim CurrCheckChar As Integer
		FirstCodeLine = 1
		Dim TempOutVal As String = OutCode.ToString
		For CurrCheckChar = 0 To TempOutVal.Length - 1
			If TempOutVal.Substring(CurrCheckChar).StartsWith(Environment.NewLine) Then
				FirstCodeLine += 1
			End If
		Next
		
		CurrentIndent = 0
		Dim CurrentSub As GCBSubroutine
		Dim subPass As Integer
		For subPass = 1 To 2
			'On pass 1, write Main
			'On pass 2, other subs
			For Each CurrentSub In Subroutines
				
				If (subPass = 1 And CurrentSub Is MainSub) Or (subPass = 2 And Not CurrentSub Is MainSub) Then
					
					If subPass = 1 Then
						'Main sub requires nothing at the start
						CurrentIndent = 0
					Else
						
						'Write description
						With CurrentSub
							If .Description <> "" Then
								OutCode.Append("'''" + .Description + Environment.NewLine)
								CurrLineNo += 1
							End If
							If .HideSub Then
								OutCode.Append("'''@hide" + Environment.NewLine)
								CurrLineNo += 1
							End If
							Dim currParam As SubParam
							For Each currParam In .Parameters
								With currParam
									If .Description <> "" Then
										OutCode.Append("'''@param " + .Name + " " + .Description + Environment.NewLine)
										CurrLineNo += 1
									End If
								End With
							Next
						End With
						
						'Write sub start line
						CurrentIndent = 1
						OutCode.Append(CurrentSub.StartCode + Environment.NewLine)
						CurrLineNo += 1
					End If
					
					'Write sub code
					For Temp = 0 To CurrentSub.Lines.Count - 1
						If Not CurrentSub.Lines(Temp) Is Nothing Then
							NewIndent = LowLevel.GetIndent(CurrentSub.Lines(Temp))
							If NewIndent = -1 Then CurrentIndent -= 1
							If NewIndent = -2 Then CurrentIndent -= 1
							If CurrentIndent < 0 Then CurrentIndent = 0
							If Temp > 1 Then
								If CurrentSub.Lines(Temp).Trim.StartsWith("'") And Not CurrentSub.Lines(Temp - 1).Trim.StartsWith("'") Then
									OutCode.Append(Environment.NewLine): CurrLineNo += 1
								End If
							End If
							LineTemp = CurrentSub.Lines(Temp)
							If LineTemp.ToUpper.StartsWith("LET ") Then LineTemp = LineTemp.Substring(4).Trim
							If LineTemp.ToLower.StartsWith("waitfor ") Then LineTemp = "Wait " + LineTemp.Substring(7).Trim
							
							OutCode.Append(LowLevel.GetIndentSpaces(CurrentIndent) + LineTemp + Environment.NewLine): CurrLineNo += 1
							If NewIndent = 1 Then CurrentIndent += 1
							If NewIndent = -2 Then CurrentIndent += 1
						End If
					Next
					
					If subPass = 2 Then
						
						'Interrupt handlers
						If IsLibrary Then
							If CurrentSub.Name.ToLower = StartupSub.ToLower THen
								
								Dim CurrHandler As InterruptHandler
								If ProgInterrupts.Count > 0 Then
									OutCode.Append(LowLevel.GetIndentSpaces(CurrentIndent) + Environment.NewLine): CurrLineNo += 1
									OutCode.Append(LowLevel.GetIndentSpaces(CurrentIndent) + ";Interrupt Handlers" + Environment.NewLine): CurrLineNo += 1
									For Each CurrHandler In ProgInterrupts
										OutCode.Append(LowLevel.GetIndentSpaces(CurrentIndent) + "On Interrupt " + CurrHandler.IntEvent + " Call " + CurrHandler.HandlerSub + Environment.NewLine)
										CurrLineNo += 1
									Next
									OutCode.Append(LowLevel.GetIndentSpaces(CurrentIndent) + Environment.NewLine): CurrLineNo += 1
								End If
							End If
						End If
						
						OutCode.Append(CurrentSub.EndCode + Environment.NewLine): CurrLineNo += 1
					End If
					OutCode.Append(Environment.NewLine): CurrLineNo += 1
					
				End If
			Next
		Next
		
		Return OutCode.ToString
		
	End Function
	
	Public Sub ReadTextProgram(InData As String, Optional SourceFile As String = "")
		
		Dim SourceLine, SourceCommand As String
		Dim CurrDoc As DocSection = Nothing
		
		Dim CurrentSub As GCBSubroutine
		Dim CurrLineNo As Integer
		Dim IsFirstCodeLine As Boolean = True
		Dim FoundNonComment As Boolean = False
		
		Dim InTable As Boolean = False
		
		Dim LineRecognised As Integer
		
		Dim LabelTemp As String = ""
		Dim CodeFromLine As List(Of String)
		
		'Get file path
		If SourceFile <> "" Then
			ProgramDir = Path.GetDirectoryName(SourceFile)
			FileName = SourceFile
		End If
		'MessageBox.Show(ProgramDir, "Message", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1)
		
		'Set up values
		ChipName = ""
		ChipSpeed = 0
		ChipConfig = ""
		OtherOptions = ""
		UseBootloader = False
		
		TableCount = 0
		Constants = New List(Of Setting)
		Variables = New VariableList
		Libraries = New List(Of LibraryType)
		
		Device = Nothing
		About = Nothing
		
		'Create main subroutine
		Subroutines.Clear
		mainSub = New GCBSubroutine("Main")
		CurrentSub = mainSub
		Subroutines.Add(mainSub)
		
		'Clear handlers
		ProgInterrupts = New List(Of InterruptHandler)
		
		'Read file
		Dim SourceLines As New StringReader(InData)
		CurrLineNo = 0
		Do While SourceLines.Peek() <> -1
			SourceLine = SourceLines.ReadLine.Trim
			
			CurrLineNo += 1
			SourceLine = SourceLine.Trim
			If SourceLine <> "" Then
				
				'Get source code documentation
				If SourceLine.StartsWith("'''") Then
					If CurrDoc Is Nothing Then
						CurrDoc = New DocSection
					End If
					
					'If comment has 3 or more stars, it marks end of a section
					If SourceLine.Replace(" ", "").StartsWith("'''***") Then
						'If nothing stored for About, store current buffer
						If Not FoundNonComment And About Is Nothing Then
							About = CurrDoc
							CurrDoc = Nothing
						End If
						
					Else
						CurrDoc.ParseLine(SourceLine)
						
						'Hardware setting?
						If CurrDoc.IsHardwareSetting Then
							
							'Has a device been made for the library?
							If Device Is Nothing Then
								Device = New HardwareDevice
								Device.Name = FileName
							End If
							
							'Create new device setting
							Dim SettingsLine() As String = CurrDoc.HardwareSetting.Split(";")
							CurrDoc = Nothing
							Dim CurrentDeviceSetting As New HardwareDeviceSetting
							Device.Settings.Add(CurrentDeviceSetting)
							
							Try
								'Condition
								CurrentDeviceSetting.Condition = SettingsLine(0).Trim
								'Name
								CurrentDeviceSetting.Name = SettingsLine(1).Trim
								'Constant
								CurrentDeviceSetting.Constant = SettingsLine(2).Trim
								'Type
								CurrentDeviceSetting.ParamType = SettingsLine(3).Trim
							Catch
								MessageBox.Show("Error in custom hardware settings", "Great Cow Graphical BASIC", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
							End Try
							
							CurrDoc = Nothing
						End If
					End If
					
					'Remove comments starting with ; as these are inserted automatically by the save routine
				Else If SourceLine.StartsWith(";") Then
					'Ignore
					
					'Read directives
				Else If ReadDirective(SourceLine, CurrDoc) Then
					'Do nothing
					CurrDoc = Nothing
					FoundNonComment = True
					
				Else If SourceLine.ToLower.StartsWith("sub ") Or SourceLine.ToLower.StartsWith("function ") Then
					InTable = False
					LineRecognised = 1
					
					CurrentSub = GCBSubroutine.FromCode(SourceLine, CurrDoc)
					Subroutines.Add(CurrentSub)
					CurrDoc = Nothing
					FoundNonComment = True
					
				Else If SourceLine.ToLower.StartsWith("end sub") or SourceLine.ToLower.StartsWith("end function") Then
					CurrentSub = MainSub
					CurrDoc = Nothing
					FoundNonComment = True
					
				Else If SourceLine.ToLower.StartsWith("table ") Then
					TableCount += 1
					TableSize(TableCount) = 0
					ProgTables(TableCount, 0) = SourceLine.Substring(5).Trim
					InTable = True
					CurrDoc = Nothing
					FoundNonComment = True
					
				Else If SourceLine.ToLower.StartsWith("end table") Then
					InTable = False
					CurrDoc = Nothing
					FoundNonComment = True
					
				Else If InTable Then
					TableSize(TableCount) += 1
					ProgTables(TableCount, TableSize(TableCount)) = SourceLine
					CurrDoc = Nothing
					FoundNonComment = True
					
				Else
					'Line is plain GCBASIC code, so load into buffer
					CodeFromLine = LowLevel.GetCommands(SourceLine)
					For Each SourceCommand In CodeFromLine
						If Not SourceCommand Is Nothing Then
							If IsFirstCodeLine Then
								IsFirstCodeLine = False
								FirstCodeLine = CurrLineNo
							End If
							CurrentSub.Lines.Add(SourceCommand)
						End If
					Next
					CurrDoc = Nothing
					FoundNonComment = True
				End If
			End If
		Loop
		
		'Load libraries
		If RequestLibraryReload Then
			ReloadLibraries
		End If
		
	End Sub
	
	Public Sub SaveFile (ByVal OutFile As String)
		'Open File
		Dim WriteFile As StreamWriter
		'Try to open
		Try
			WriteFile = New StreamWriter(OutFile)
			
			'Set locked flag if opening fails
		Catch e As UnauthorizedAccessException
			Me.FileLocked = True
			Throw e
		End Try
		
		'Make sure program contains editor contents
		Editor.SyncEditorToProgram
		
		'Write
		WriteFile.Write(GetTextProgram)
		
		'Has file been saved to original location?
		If FileName.ToLower = OutFile.ToLower Then
			HasChanged = False
			
			'Update locked flag
			Me.FileLocked = False
		End If
		
		'Close File
		WriteFile.Close()
		
	End Sub
	
	Public Sub LoadFile (ByVal InFile As String)
		'Read file
		Dim ReadFile As New StreamReader(InFile)
		ReadTextProgram(ReadFile.ReadToEnd, InFile)
		ReadFile.Close()
		
		LoadIncludes
		
		'Update editor from loaded program
		If Not Editor Is Nothing Then
			Editor.SyncProgramToEditor
		End If
	End Sub
	
	Public Function GetActualLine(SubNo As Integer, LineNo As Integer) As Integer
		
		'Take a line number given by GCBASIC, translate into icon number
		'Consider:
		'	- Code before first Main icon
		'	- Blank lines inserted before each comment or group of comments
		
		'Extra code in Main
		If SubNo = 0 Then
			Dim LineNoOld As Integer = LineNo
			LineNo = LineNo - FirstCodeLine + 1
			
		End If
		
		'Something odd going on, line is not code line
		If LineNo = 0 Then Return -1
		
		'Count number of blank lines that would be added
		Dim CurrLine, ActualLine As Integer
		Dim LastLineWasComment As Boolean
		CurrLine = 1
		ActualLine = 1
		LastLineWasComment = False
		Do While CurrLine < LineNo
			'If ProgCode(SubNo, ActualLine).StartsWith("'") Then
			If Subroutines(SubNo).Lines(ActualLine).StartsWith("'") Then
				If Not LastLineWasComment Then
					CurrLine += 1
				End If
				CurrLine += 1
				ActualLine += 1
				LastLineWasComment = True
			Else
				CurrLine += 1
				ActualLine += 1
				LastLineWasComment = False
			End If
		Loop
		
		Return ActualLine
	End Function
	
	Public Sub InsertLine(ByVal CurrentSub As GCBSubroutine, ByVal NewProgLine As String, AddIndex As Integer)
		Dim LabelTemp As String = ""
		
		'Check for recursion, show warning
		If GetCalledSub(NewProgLine) Is CurrentSub And MainForm.GetPreferences.GetPref("GCGB", "WarnRecursion") = "1" Then
			Dim result As DialogResult = MessageBox.Show("Calling a subroutine from itself can cause infinite loops. Are you sure you want to continue?", "Great Cow Graphical BASIC", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
			If result = DialogResult.No Then Exit Sub
		End If
		
		'Check AddIndex is valid, fix it if it isn't
		With CurrentSub
			If AddIndex > .Lines.Count Then AddIndex = .Lines.Count
			
			'Add line
			.Lines.Insert(AddIndex, NewProgLine)
			
		End With
		
		HasChanged = True
		
	End Sub
	
	Public Sub DeleteLine(ByVal CurrentSub As GCBSubroutine, ByVal DeleteIndex As Integer)
		Dim TempData As String = ""
		
		If DeleteIndex < 0 Then Exit Sub
		
		CurrentSub.Lines.RemoveAt(DeleteIndex)
		HasChanged = True
		
	End Sub
	
	Public Function GetCommandData(ByVal InputLine As String) As String
		Dim DataTemp, CommandName, SyntaxTemp As String
		Dim CommandPointer, IsMatch, IndexTemp As Integer
		
		IsMatch = 0
		
		'Special cases
		'Comment
		If InputLine.StartsWith("'") Then
			DataTemp = InputLine.Replace("'", "")
			Return "Comment:" + DataTemp
		End If
		'Label
		If InputLine.EndsWith(":") Then
			DataTemp = InputLine.Substring(0, InputLine.Length - 1)
			If DataTemp.IndexOf("SysBlankLabel") <> -1 Then DataTemp = ""
			Return "Label:" + DataTemp
		End If
		'Subroutine
		If IsSub(InputLine) Then
			Return "Subroutine:" + EditSubIndex(InputLine).ToString
		End If
		
		'Get Command
		CommandName = InputLine.ToLower.Trim + " "
		
		'Get Command Pointer
		CommandPointer = -1
		For IndexTemp = 0 To MainForm.CommandList.Count - 1
			With MainForm.CommandList(IndexTemp)
				SyntaxTemp = .Syntax
				If SyntaxTemp <> "" Then
					If .Name.ToLower <> "label" Then
						SyntaxTemp = SyntaxTemp.ToLower
						'If SyntaxTemp.IndexOf(" ") <> -1 Then SyntaxTemp = SyntaxTemp.ToLower.Substring(0, SyntaxTemp.IndexOf(" "))
						'If CommandName = SyntaxTemp Then CommandPointer = IndexTemp: Exit For
						If SyntaxTemp.IndexOf("%") <> -1 Then SyntaxTemp = SyntaxTemp.ToLower.Substring(0, SyntaxTemp.IndexOf("%")).Trim + " "
						If CommandName.Length >= SyntaxTemp.Length Then
							If CommandName.Substring(0, SyntaxTemp.Length).ToLower = SyntaxTemp Then CommandPointer = IndexTemp: Exit For
						End If
						If .MatchSyntax <> "" Then
							'If CommandName.IndexOf(MainForm.CommandList(IndexTemp, 1, 4).ToLower) <> -1 Then IsMatch = -1: CommandPointer = IndexTemp: Exit For
							If CommandName.Trim = .MatchSyntax.ToLower.Trim Then IsMatch = -1: CommandPointer = IndexTemp: Exit For
						End If
					End If
				End If
			End With
		Next
		
		'Search again, but replace ( with space
		If CommandPointer = -1 Then
			CommandName = CommandName.Replace("(", " (")
			For IndexTemp = 0 To MainForm.CommandList.Count - 1
				With MainForm.CommandList(IndexTemp)
					SyntaxTemp = .Syntax
					If SyntaxTemp <> "" Then
						If .Name.ToLower <> "label" Then
							SyntaxTemp = SyntaxTemp.ToLower
							If SyntaxTemp.IndexOf("%") <> -1 Then SyntaxTemp = SyntaxTemp.ToLower.Substring(0, SyntaxTemp.IndexOf("%")).Trim + " "
							If CommandName.Length >= SyntaxTemp.Length Then
								If CommandName.Substring(0, SyntaxTemp.Length).ToLower = SyntaxTemp Then CommandPointer = IndexTemp: Exit For
							End If
							If .MatchSyntax <> "" Then
								'Triggers error when select case loopcounter used
								'If CommandName.IndexOf(.MatchSyntax.ToLower) <> -1 Then IsMatch = -1: CommandPointer = IndexTemp: Exit For
								'Fix:
								If CommandName.StartsWith(.MatchSyntax.ToLower) Then IsMatch = -1: CommandPointer = IndexTemp: Exit For
							End If
						End If
					End If
				End With
			Next
		End If
		
		If CommandPointer = -1 Then Return "Unknown"
		
		If IsMatch = -1 Then Return "Match:" + CommandPointer.ToString
		Return "Command:" + CommandPointer.ToString
		
	End Function
	
	Public Function GetLabels(CurrSub As GCBSubroutine) As List(Of String)
		'Returns a list of all labels in the requested sub
		Dim LineTemp As String
		Dim OutList As New List(Of String)
		
		For Each LineTemp In CurrSub.Lines
			If LineTemp.EndsWith(":") And LineTemp <> "SysBlankLabel:" Then
				OutList.Add(LineTemp.Substring(0, LineTemp.Length - 1).Trim)
			End If
		Next
		
		Return OutList
	End Function
	
	Public Function GetAllConstGroups As List(Of ConstGroup)
		'Get all constant groups applicable to this program
		'Return a list of local and global groups
		Dim OutList As New List(Of ConstGroup)
		OutList.AddRange(LocalConstGroups)
		OutList.AddRange(MainForm.ConstantGroups)
		Return OutList
	End Function
	
	Public Function GetConstGroup(GroupName As String) As ConstGroup
		'Get the requested constant group list
		Dim searchGroup As ConstGroup
		
		'Search program first for any local groups (AD channels)
		For Each searchGroup In LocalConstGroups
			If searchGroup.Name.ToUpper = GroupName.ToUpper Then
				Return searchGroup
			End If
		Next
		
		'Then search MainForm for global groups (param values)
		For Each searchGroup In MainForm.ConstantGroups
			If searchGroup.Name.ToUpper = GroupName.ToUpper Then
				Return searchGroup
			End If
		Next
		
		'If nothing found, return nothing
		Return Nothing
		
	End Function
	
	Public Function GetCurrentChip As String
		
		'Check main file
		If ChipName.Trim <> "" Then Return ChipName.Trim
		
		'Check libraries
		Dim CheckLib As LibraryType
		For Each CheckLib In Libraries
			Try
				If CheckLib.ChipName <> "" Then Return CheckLib.ChipName.Trim
			Catch
			End Try
		Next
		
		Return ""
	End Function
	
	Public Function GetCurrentChipSpeed As Double
		
		'Check main file
		If ChipSpeed > 0 Then Return ChipSpeed
		
		'Check libraries
		Dim CheckLib As LibraryType
		For Each CheckLib In Libraries
			Try
				If CheckLib.ChipSpeed > 0 Then Return CheckLib.ChipSpeed
			Catch
			End Try
		Next
		
		Return 0
	End Function
	
	Public Sub CheckChipData
		'Check that currently loaded chip data file matches current chip
		'If it doesn't, load a new file.
		
		Dim CurrChip As String
		CurrChip = GetCurrentChip
		Dim LoadNewFile As Boolean
		LoadNewFile = False
		
		'If current chip isn't set, can't do anything anyway
		If CurrChip = "" Then
			'Clear A/D channels list
			Dim ADChannelList As ConstGroup = GetConstGroup("ADChannels")
			If Not ADChannelList Is Nothing Then
				ADChannelList.Items.Clear
			End If
			
			Exit Sub
		End If
		
		'If no file loaded, load
		If ChipData Is Nothing Then
			LoadNewFile = True
		Else
			'If loaded file doesn't match chip, load
			If CurrChip <> ChipData.ChipName Then
				LoadNewFile = True
			End If
		End If
		
		If LoadNewFile Then
			'Load new data
			ChipData = New ChipInfo(CurrChip)
			
			'Create A/D channel list
			Dim ADChannelList As ConstGroup = GetConstGroup("ADChannels")
			If ADChannelList Is Nothing Then
				ADChannelList = New ConstGroup("ADChannels")
				LocalConstGroups.Add(ADChannelList)
			End If
			With ADChannelList
				'Clear existing list
				.Items.Clear
				'Fill with A/D channel names
				Dim currADPin As Integer
				For currADPin = 1 To ChipData.ADCInputs
					.Items.Add("AN" + (currADPin - 1).ToString)
				Next
			End With
			
		End If
		
	End Sub
	
	Public Function IsSub(ByVal SubName As String) As Boolean
		
		If GetCalledSub(SubName) Is Nothing Then
			Return False
		Else
			Return True
		End If
		
	End Function
	
	Public Sub LoadIncludes
		
		'Clear old data
		Dim ClearLib As LibraryType
		For Each ClearLib In Libraries
			ClearLib.ClearData
		Next
		
		'Read include files, and extract data
		Dim LoadLib As LibraryType
		For Each LoadLib In Libraries
			LoadLibraryFromFile(LoadLib, LoadLib.FileName)
			
			If Not LoadLib.Device Is Nothing Then
				LoadLib.Device.FindControllers
			End If
		Next
		
	End Sub
	
	Private Sub LoadLibraryFromFile(LoadLib As LibraryType, FileNameTemp As String, Optional FirstFile As String = "")
		'Read contents of library file, and append to library
		'Can be used to handle recursive includes if FirstFile is set to
		'the name of the library that the user originally added
		
		Dim TempData, OriginalFile As String
		Dim CurrDoc As DocSection
		Dim DefineName, DefineValue, NewInclude, ChipModel As String
		Dim InSub As Boolean
		
		Dim NewIncludes As New List(Of String)
		
		OriginalFile = FileNameTemp
		If FileNameTemp.IndexOf("<") <> -1 Then
			FileNameTemp = FileNameTemp.Replace("<", "").Replace(">", "").Trim
			FileNameTemp = MainForm.InstallDir + Mainform.FilenameIncludeDir + FileNameTemp
		Else If FileNameTemp.IndexOf("""") <> -1 Then
			FileNameTemp = FileNameTemp.Replace("""", "").Trim
			FileNameTemp = Lowlevel.AddFullPath(FileNameTemp, ProgramDir)
			
		End If
		
		'Check to see if file requires conversion to load
		Dim Converter As FileConverter
		Converter = MainForm.GetFileConverter(FileNameTemp)
		'File needs to be converted, so convert it
		If Not Converter Is Nothing Then
			FileNameTemp = Converter.Convert(FileNameTemp)
		End If
		
		'Check filename, and load if possible
		Try
			Dim FileCheck As New IO.FileInfo(FileNameTemp)
			If FileCheck.Exists Then
				
				'Open File
				Dim ReadInclude As New StreamReader(FileNameTemp)
				InSub = False
				CurrDoc = Nothing
				
				'Read File
				Do While ReadInclude.Peek() <> -1
					TempData = ReadInclude.ReadLine().Replace(Convert.ToChar(9), " ").Trim
					
					'Check for documentation
					If TempData.StartsWith("'''") Then
						If CurrDoc Is Nothing Then
							CurrDoc = New DocSection
						End If
						CurrDoc.ParseLine(TempData)
						
						'Hardware setting?
						If CurrDoc.IsHardwareSetting Then
							
							'Has a device been made for the library?
							If LoadLib.Device Is Nothing Then
								LoadLib.Device = New HardwareDevice
								LoadLib.Device.Name = LoadLib.FileName
							End If
							
							'Create new device setting
							Dim SettingsLine() As String = CurrDoc.HardwareSetting.Split(";")
							CurrDoc = Nothing
							Dim CurrentDeviceSetting As New HardwareDeviceSetting
							LoadLib.Device.Settings.Add(CurrentDeviceSetting)
							
							Try
								'Condition
								CurrentDeviceSetting.Condition = SettingsLine(0).Trim
								'Name
								CurrentDeviceSetting.Name = SettingsLine(1).Trim
								'Constant
								CurrentDeviceSetting.Constant = SettingsLine(2).Trim
								'Type
								CurrentDeviceSetting.ParamType = SettingsLine(3).Trim
							Catch
								MessageBox.Show("Error in custom hardware settings for library " + LoadLib.FileName, "Great Cow Graphical BASIC", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
							End Try
							
							CurrDoc = Nothing
						End If
						
						'Check for subroutine/function
					Else If TempData.ToLower.StartsWith("sub ") Or TempData.ToLower.StartsWith("function ") Or TempData.ToLower.StartsWith("word function ") Then
						If FirstFile <> "" Then
							If CurrDoc Is Nothing Then
								CurrDoc = New DocSection
							End If
							CurrDoc.IncludedFrom = OriginalFile
						End If
						LoadLib.Subroutines.Add(GCBSubroutine.FromCode(TempData, CurrDoc))
						CurrDoc = Nothing
						InSub = True
						
						'Check for end of sub/function
					Else If TempData.ToLower.StartsWith("end sub") Or TempData.ToLower.StartsWith("end function") Then
						InSub = False
						
						'Check for constant
					Else If TempData.ToLower.Trim.StartsWith("#define ") Then
						DefineName = TempData.Trim.Substring(7).Trim
						DefineValue = ""
						If DefineName.IndexOf(" ") <> -1 Then
							DefineValue = DefineName.Substring(DefineName.IndexOf(" ") + 1).Trim
							DefineName = DefineName.Substring(0, DefineName.IndexOf(" ")).Trim
						End If
						LoadLib.Constants.Add(New Setting(DefineName, DefineValue))
						CurrDoc = Nothing
						
						'Check for chip model
					Else If TempData.ToLower.StartsWith("#chip ") And LoadLib.ChipName = "" Then
						ChipModel = TempData.Substring(5).Trim
						If ChipModel.IndexOf(",") <> -1 Then
							LoadLib.ChipSpeed = ChipModel.Substring(ChipModel.IndexOf(",") + 1).Trim
							LoadLib.ChipName = ChipModel.Substring(0,ChipModel.IndexOf(",")).Trim
						Else
							LoadLib.ChipSpeed = 0
							LoadLib.ChipName = ChipModel
						End If
						CurrDoc = Nothing
						
						'Check for another library
					Else If TempData.ToLower.StartsWith("#include ") Then
						NewIncludes.Add(TempData.Substring(9).Trim)
						
						'Check for data table
					Else If TempData.ToLower.StartsWith("table ") Then
						LoadLib.DataTables.Add(TempData.Substring(5).Trim)
						CurrDoc = Nothing
						
						'Check for variable
					Else If TempData.ToLower.StartsWith("dim ") And Not InSub Then
						Dim NewVar As VariableListItem = VariableListItem.FromCode(TempData)
						
						'Hide variable?
						NewVar.Hidden = False
						If Not CurrDoc Is Nothing Then
							If CurrDoc.HideItem Then
								NewVar.Hidden = True
								CurrDoc = Nothing
							End If
						End If
						LoadLib.Variables.Add(NewVar)
						
					End If
					
				Loop
				
				ReadInclude.Close()
				
				'Load recursively added libraries
				For Each NewInclude In NewIncludes
					If FirstFile = "" Then
						LoadLibraryFromFile(LoadLib, NewInclude, OriginalFile)
					Else
						LoadLibraryFromFile(LoadLib, NewInclude, FirstFile)
					End If
				Next
				
			End If
		Catch ex As Exception
			MessageBox.Show(ex.ToString, "Error loading library")
		End Try
	End Sub
	
	Public Sub ReloadLibraries (Optional CheckRequest As Boolean = False)
		
		'If CheckRequest is true, only reload if libraries changed
		If CheckRequest And Not RequestLibraryReload Then Exit Sub
		
		'Reload libraries, notify other objects
		LoadIncludes
		RaiseEvent LibraryListChanged
		
		RequestLibraryReload = False
	End Sub
	
	Public Function ReadDirective(ByVal TempData As String, Optional ByRef CurrDoc As DocSection = Nothing) As Boolean
		'Will check line for directive
		'If directive found, will process and return true
		
		Dim ChipModel, IncludeTemp, DefineTemp As String
		Dim DefineName, DefineValue As String
		Dim FoundIndex As Integer
		
		With Me
			
			'Chip settings
			If TempData.ToLower.StartsWith("#chip") Then
				ChipModel = TempData.Substring(5).Trim
				If ChipModel.IndexOf(",") <> -1 Then
					.ChipSpeed = ChipModel.Substring(ChipModel.IndexOf(",") + 1).Trim
					.ChipName = ChipModel.Substring(0, ChipModel.IndexOf(",")).Trim
				Else
					.ChipSpeed = 0
					.ChipName = ChipModel.Trim
				End If
				'MessageBox.Show(.ProgConfig(1), "Test", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1)
				Return True
			End If
			If TempData.ToLower.StartsWith("#config") Then
				.ChipConfig = TempData.Substring(7).ToUpper.Trim
				Return True
			End If
			
			'Library
			If TempData.ToLower.IndexOf("#include") = 0 Then
				IncludeTemp = TempData.Substring(8).Trim
				FoundIndex = 0
				Dim CheckLib As LibraryType
				For Each CheckLib In Libraries
					If IncludeTemp = CheckLib.FileName Then FoundIndex = 1: Exit For
				Next
				If FoundIndex = 0 Then
					Libraries.Add(New LibraryType(IncludeTemp))
					RequestLibraryReload = True
				End If
				Return True
			End If
			
			'Constant
			If TempData.ToLower.StartsWith("#define") Then
				DefineTemp = TempData.Substring(7).Trim
				If DefineTemp.IndexOf(" ") = -1 Then
					DefineName = DefineTemp
					DefineValue = ""
				Else
					DefineName = DefineTemp.Substring(0, DefineTemp.IndexOf(" ")).Trim
					DefineValue = DefineTemp.Substring(DefineTemp.IndexOf(" ") + 1).Trim
				End If
				
				Dim currConst As Setting
				For Each currConst In Constants
					If DefineName.ToLower = currConst.Name.ToLower Then Return True
				Next
				
				Constants.Add(New Setting(DefineName, DefineValue))
				Return True
			End If
			
			'#mem (remove only)
			If TempData.ToLower.StartsWith("#mem") Then Return True
			
			'#startup
			If TempData.ToLower.StartsWith("#startup") Then
				.StartupSub = TempData.Substring(8).Trim
				Return True
			End If
			
			'#option
			If TempData.ToLower.StartsWith("#option") Then
				'.StartupSub = TempData.Substring(7).ToUpper.Trim
				Dim Options() As String = TempData.Trim.Split(" ")
				Dim CurrOpt As Integer = 1
				Do While CurrOpt < Options.Length
					
					'Read bootloader option
					If Options(CurrOpt).ToLower = "bootloader" Then
						UseBootloader = True
						BootloaderPos = ""
						If CurrOpt + 1 < Options.Length Then
							CurrOpt += 1
							BootloaderPos = Options(CurrOpt)
						End If
						
						'Read other option
					Else
						If OtherOptions = "" Then
							OtherOptions = Options(CurrOpt)
						Else
							OtherOptions = OtherOptions + " " + Options(CurrOpt)
						End If
					End If
					
					CurrOpt += 1
				Loop
				
				Return True
			End If
			
			'Variable declaration
			If TempData.ToLower.StartsWith("dim ") Then
				Dim NewVar As VariableListItem = VariableListItem.FromCode(TempData)
				
				'Hide variable?
				NewVar.Hidden = False
				If Not CurrDoc Is Nothing Then
					If CurrDoc.HideItem Then
						NewVar.Hidden = True
						CurrDoc = Nothing
					End If
				End If
				
				Variables.AddVar(NewVar)
				Return True
			End If
			
			'Interrupt handler declaration
			If TempData.ToLower.StartsWith("on interrupt ") Then
				Dim Tokens() As String
				Tokens = TempData.Split
				Dim NewHandler As New InterruptHandler
				NewHandler.IntEvent = Tokens(2)
				NewHandler.HandlerSub = Tokens(4)
				ProgInterrupts.Add(NewHandler)
				
				Return True
			End If
			
			'Semicolon (remove)
			If TempData.StartsWith(";") Then
				Return True
			End If
			
		End With
		
		Return False
	End Function
	
	Public Sub ReplaceInProgram(OldToken As String, NewToken As String)
		
		'Search program for occurances of OldToken, and replace them with NewToken
		Dim CurrSub As GCBSubroutine
		Dim CurrLine As Integer
		
		'Search each sub
		For Each CurrSub In Subroutines
			'Search every line
			For CurrLine = 0 To CurrSub.Lines.Count - 1
				'Replace
				Lowlevel.WholeReplace(CurrSub.Lines(CurrLine), OldToken, NewToken)
			Next
		Next
		
	End Sub
	
	Public Function CanInsertCommand(ByVal NewCommand As String, CurrSub As GCBSubroutine, CurrLine As Integer) As Boolean
		'Will check OnlyIn value for NewCommand
		'If present, will check if it can be inserted at CurrLine
		
		Dim TempData, TempLine, SyntaxTemp As String
		Dim NewCommandIndex, OnlyIn, BlockLevel, SearchLine, ThisCmd, IndexTemp As Integer
		Dim IsMatch As Boolean
		
		TempData = GetCommandData(NewCommand)
		NewCommandIndex = 0
		If TempData.StartsWith("Command:") Then
			NewCommandIndex = TempData.Substring(8).Trim
		End If
		'Don't restrict unknown commands
		If NewCommandIndex = 0 Then Return True
		
		With MainForm.CommandList(NewCommandIndex)
			
			'Check if command needs to be restricted
			If .OnlyIn = "" Then
				Return True
			End If
			
			'Check if icon can only be placed in sub
			If .OnlyIn = "sub" Then
				If CurrSub Is MainSub Then
					Return False
				Else
					Return True
				End If
			End If
			
			'Get the block that this icon must go in
			TempData = GetCommandData(.OnlyIn)
			If TempData.StartsWith("Command:") Then
				OnlyIn = TempData.Substring(8).Trim
			Else
				'Don't restrict unknown commands
				Return True
			End If
			
			'Get level of CurrLine
			BlockLevel = 0
			For SearchLine = 0 To CurrLine
				TempLine = CurrSub.Lines(SearchLine)
				
				'Get icon on SearchLine
				ThisCmd = 0
				For IndexTemp = 0 To MainForm.CommandList.Count - 1
					With MainForm.CommandList(IndexTemp)
						SyntaxTemp = .Syntax
						If SyntaxTemp <> "" Then
							SyntaxTemp = SyntaxTemp.ToLower
							If SyntaxTemp.IndexOf("%") <> -1 Then SyntaxTemp = SyntaxTemp.ToLower.Substring(0, SyntaxTemp.IndexOf("%")).Trim
							If TempLine.Length >= SyntaxTemp.Length Then
								If TempLine.Substring(0, SyntaxTemp.Length).ToLower = SyntaxTemp Then
									IsMatch = False
									ThisCmd = IndexTemp
									Exit For
								End If
							End If
							If .MatchSyntax <> "" Then
								If TempLine.ToLower.IndexOf(.MatchSyntax.ToLower) <> -1 Then
									IsMatch = True
									ThisCmd = IndexTemp
									Exit For
								End If
							End If
						End If
					End With
				Next
				
				'Is it the one we're interested in?
				If ThisCmd = OnlyIn Then
					If IsMatch Then
						BlockLevel -= 1
					Else
						BlockLevel += 1
					End If
				End If
				
			Next
			
			'If CurrLine is in a block, BlockLevel will be greater than 0
			If BlockLevel > 0 Then
				Return True
			Else
				Return False
			End If
			
		End With
		
	End Function
	
	Public Function EditSubIndex(ByVal InputLine As String) As Integer
		'Return the subroutine called in the line
		
		Dim LineTrim As String
		Dim searchSub As GCBSubroutine
		Dim currSubNo As Integer
		
		LineTrim = InputLine.Trim.ToLower.Replace("(", " ")
		If LineTrim.IndexOf("*") <> -1 Then
			LineTrim = LineTrim.Replace("*","")
		End If
		If LineTrim.IndexOf(" ") <> -1 Then LineTrim = LineTrim.Substring(0, LineTrim.IndexOf(" "))
		
		'Search main file
		currSubNo = 0
		For Each searchSub In Subroutines
			If searchSub.Name.ToLower = LineTrim Then Return currSubNo
			currSubNo += 1
		Next
		
		'Search libraries
		Dim CheckLib As LibraryType
		currSubNo = 0
		For Each CheckLib In Libraries
			For Each searchSub In CheckLib.Subroutines
				currSubNo -= 1
				If searchSub.Name.ToLower = LineTrim Then Return currSubNo
			Next
		Next
		
		Return 0
	End Function
	
	Public Function GetCalledSub(InputLine As String) As GCBSubroutine
		'Return the subroutine called in the line
		
		Dim LineTrim As String
		Dim searchSub As GCBSubroutine
		
		LineTrim = InputLine.Trim.ToLower.Replace("(", " ")
		If LineTrim.IndexOf("*") <> -1 Then
			LineTrim = LineTrim.Replace("*","")
		End If
		If LineTrim.IndexOf(" ") <> -1 Then LineTrim = LineTrim.Substring(0, LineTrim.IndexOf(" "))
		
		'Search main file
		For Each searchSub In Subroutines
			If searchSub.Name.ToLower = LineTrim Then Return searchSub
		Next
		
		'Search libraries
		Dim CheckLib As LibraryType
		For Each CheckLib In Libraries
			For Each searchSub In CheckLib.Subroutines
				If searchSub.Name.ToLower = LineTrim Then Return searchSub
			Next
		Next
		
		Return Nothing
	End Function
	
	Public Function IsMainSub(TestSub As GCBSubroutine) As Boolean
		Return TestSub Is MainSub
	End Function
	
	Public Function SubSourceLib(ByVal SubName As String) As Integer
		Dim LineTrim, SubTrim As String
		Dim LibNo As Integer
		Dim MatchParams As Boolean = True
		
		LineTrim = SubName.Trim.ToLower.Replace("(", " ")
		If LineTrim.IndexOf("*") <> -1 Then LineTrim = LineTrim.Replace("*",""): MatchParams = False
		If LineTrim.IndexOf(" ") <> -1 Then LineTrim = LineTrim.Substring(0, LineTrim.IndexOf(" "))
		
		'Search libraries
		Dim CheckLib As LibraryType
		Dim searchSub As GCBSubroutine
		LibNo = 0
		For Each CheckLib In Libraries
			LibNo += 1
			For Each searchSub In CheckLib.Subroutines
				SubTrim = searchSub.Name.ToLower.Trim.Replace("("," ")
				If SubTrim.IndexOf(" ") <> -1 Then SubTrim = SubTrim.Substring(SubTrim.IndexOf(" ")).Trim
				If SubTrim.IndexOf(" ") <> -1 Then SubTrim = SubTrim.Substring(0, SubTrim.indexOf(" "))
				If SubTrim = LineTrim Then Return LibNo
			Next
		Next
		
		Return 0
	End Function
	
	Public Function ExtractVars(ByVal ScanLine As String) As List(Of String)
		Dim Temp As Integer = 0
		Dim WordList(100) As String
		Dim WordCount As Integer = 0
		Dim DelStart, DelEnd As Integer
		Dim TempData As String
		Dim IsNotVar As Boolean
		
		Dim OutList As New List(Of String)
		
		'Don't process labels or comments
		If ScanLine.Trim.EndsWith(":") Then Return OutList
		If ScanLine.Trim.StartsWith("'") Then Return OutList
		
		'Don't process some commands
		If ScanLine.ToLower.Trim.StartsWith("goto ") Then Return OutList
		If ScanLine.ToLower.Trim.StartsWith("end ") Then Return OutList
		If ScanLine.ToLower.Trim.StartsWith("exit ") Then Return OutList
		If ScanLine.ToLower.Trim = "loop" Then Return OutList
		If ScanLine.ToLower.Trim = "next" Then Return OutList
		If ScanLine.ToLower.Trim.StartsWith("dir ") Then ScanLine = ScanLine.Substring(3).Trim
		
		'Remove string literals from ScanLine
		Do While ScanLine.IndexOf("""") <> -1
			DelStart = ScanLine.IndexOf("""")
			DelEnd = ScanLine.IndexOf("""", DelStart + 1)
			If DelEnd = -1 Then ScanLine = ScanLine.Replace("""", ""): Exit Do
			ScanLine = ScanLine.Substring(0, DelStart) + ScanLine.Substring(DelEnd + 1)
			'MessageBox.Show(ScanLine + ":" + DelStart.ToString + ":" + DelEnd.ToString, "Line", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1)
		Loop
		
		Dim Dividers() As Char = {" ", ",", "=", ",", "<", ">", "!", "*", "/", "%", "+", "-", "&", "|", "#", "(", ")"}
		
		'Remove first section, it should be a command
		ScanLine = ScanLine.Substring(ScanLine.IndexOfAny(Dividers) + 1) + " "
		
		'Break up ScanLine
		Do While ScanLine.IndexOfAny(Dividers) <> -1
			WordCount += 1
			WordList(WordCount) = ScanLine.Substring(0, ScanLine.IndexOfAny(Dividers))
			If WordList(WordCount).Length = 0 Then
				WordCount -= 1
			End If
			ScanLine = ScanLine.Substring(ScanLine.IndexOfAny(Dividers) + 1)
		Loop
		
		'Remove anything that does not belong
		For Temp = 1 To WordCount
			TempData = WordList(Temp).Trim.ToLower
			IsNotVar = False
			
			'Literals (check if first char is number)
			If Char.IsDigit(TempData, 0) Then IsNotVar = True
			
			'Keywords
			If TempData = "to" Then IsNotVar = True
			If TempData = "then" Then IsNotVar = True
			If TempData = "until" Or TempData = "while" Or TempData = "forever" Then IsNotVar = True
			If TempData = "us" Or TempData = "ms" Or TempData = "s" Or TempData = "m" Or TempData = "h" Then IsNotVar = True
			If TempData = "usec" Or TempData = "msec" Or TempData = "sec" Or TempData = "min" Then IsNotVar = True
			If TempData = "true" Or TempData = "false" Then IsNotVar = True
			If TempData = "on" Or TempData = "off" Then IsNotVar = True
			If TempData = "in" Or TempData = "out" Or TempData = "input" Or TempData = "output" Then IsNotVar = True
			
			'Program constants
			Dim ConstTemp As Setting
			If Not IsNotVar Then
				For Each ConstTemp In Constants
					If ConstTemp.Name.Trim.ToLower = TempData Then IsNotVar = True: Exit For
				Next
			End If
			
			'Library constants
			Dim LibTemp As LibraryType
			For Each LibTemp in Libraries
				For Each ConstTemp In LibTemp.Constants
					If ConstTemp.Name.Trim.ToLower = TempData Then IsNotVar = True: Goto FoundInLib
				Next
			Next
			FoundInLib:
			
			'Constant group constants
			
			'Function names
			
			
			If Not IsNotVar Then
				OutList.Add(WordList(Temp))
			End If
		Next
		
		Return OutList
		
	End Function
	
	Public Sub FillParamList(ByVal CurrentSub As GCBSubroutine, ByRef ParamListBox As System.Windows.Forms.ComboBox, ByVal ParamList As String, ByVal CurrentValue As String, Optional ParamName As String = "")
		Dim Temp, AddListItem, OptionCount As Integer
		Dim OptionList(200) As String
		Dim OptionListOriginal(200) As String
		Dim CGR_Name As String
		
		If ParamList.Trim = "" Then Exit Sub
		
		OptionCount = 0
		Do While ParamList.IndexOf("|") <> -1
			OptionCount += 1
			OptionListOriginal(OptionCount) = ParamList.Substring(0, ParamList.IndexOf("|")).Trim
			OptionList(OptionCount) = OptionListOriginal(OptionCount).ToLower
			ParamList = ParamList.Substring(ParamList.IndexOf("|") + 1).Trim
		Loop
		OptionCount += 1
		OptionListOriginal(OptionCount) = ParamList.Trim
		OptionList(OptionCount) = ParamList.Trim.ToLower
		
		'Values for literal constants
		Dim MinVal, MaxVal As Long
		Dim ConstRangeSet As Boolean = False
		
		For AddListItem = 1 To OptionCount
			
			'Constant value
			If OptionList(AddListItem).StartsWith("""") And OptionList(AddListItem).EndsWith("""") Then
				Dim NewValue As String = OptionListOriginal(AddListItem).Trim("""")
				NewValue = NewValue.Trim
				ParamListBox.Items.Add(NewValue)
			End If
			
			'Constant groups
			If OptionList(AddListItem).IndexOf("cgr_") = 0 Then
				CGR_Name = OptionList(AddListItem).Substring(4).Trim
				'MessageBox.Show(CGR_Name, "Message", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1)
				
				'Find constant group
				Dim thisConstGroup As ConstGroup = GetConstGroup(CGR_Name)
				If Not thisConstGroup Is Nothing Then
					
					'Add constants that refer to members of the constant group
					'This program
					Dim Constant As Setting
					Dim groupItem As String
					For Each Constant In Constants
						'For AddParams = 1 To MainForm.MaxCGRItems
						For Each groupItem in thisConstGroup.Items
							If Constant.Value.Trim.ToUpper = groupItem.ToUpper And ParamName <> Constant.Name Then
								ParamListBox.Items.Add(Constant.Name.Trim)
							End If
						Next
					Next
					'Libraries
					Dim CheckLib As LibraryType
					For Each CheckLib In Libraries
						For Each Constant In CheckLib.Constants
							'For AddParams = 1 To MainForm.MaxCGRItems
							For Each groupItem In thisConstGroup.Items
								If Constant.Value.Trim.ToUpper = groupItem.ToUpper And ParamName <> Constant.Name Then
									ParamListBox.Items.Add(Constant.Name.Trim)
								End If
							Next
						Next
					Next
					'Add all items from constant group
					'Don't sort constant group items
					ParamListBox.Sorted = False
					For Each groupItem In thisConstGroup.Items
						ParamListBox.Items.Add(groupItem.Trim)
					Next
				End If
			End If
			
			'IO pins
			If OptionList(AddListItem) = "io_pin" Or OptionList(AddListItem) = "io" Then
				CheckChipData
				If Not ChipData Is Nothing Then
					Dim CurrPin As ChipPinInfo
					Dim PinName As String
					
					'Add constants that refer to IO pins
					'This program
					Dim Constant As Setting
					For Each Constant In Constants
						If ChipData.IsIOPin(Constant.Value) And ParamName <> Constant.Name Then
							'Add pin and select it
							ParamListBox.Items.Add(Constant.Name.Trim)
						End If
					Next
					'Libraries
					Dim CheckLib As LibraryType
					For Each CheckLib In Libraries
						For Each Constant In CheckLib.Constants
							If ChipData.IsIOPin(Constant.Value) And ParamName <> Constant.Name Then
								'Add pin and select it
								ParamListBox.Items.Add(Constant.Name.Trim)
							End If
						Next
					Next
					
					'Add actual IO pins
					For Each CurrPin In ChipData.PinList
						If CurrPin.PortBit <> -1 Then
							PinName = CurrPin.PortName + "." + CurrPin.PortBit.ToString
							ParamListBox.Items.Add(PinName)
						End If
					Next
				End If
			End If
			
			'IO ports
			If OptionList(AddListItem) = "io_port" Or OptionList(AddListItem) = "io" Then
				
			End If
			
			'Labels
			If OptionList(AddListItem) = "label" And Not CurrentSub Is Nothing Then
				
				Dim LabelList As List(Of String)
				LabelList = GetLabels(CurrentSub)
				Dim Label As String
				
				For Each Label In LabelList
					ParamListBox.Items.Add(Label)
				Next
			End If
			
			'Data tables
			If OptionList(AddListItem) = "table" Then
				For Temp = 1 To TableCount
					If ProgTables(Temp, 0) <> "" Then
						ParamListBox.Items.Add(ProgTables(Temp, 0).Trim)
					End If
				Next
				Dim CheckLib As LibraryType
				Dim TableName As String
				For Each CheckLib In Libraries
					For Each TableName In CheckLib.DataTables
						If TableName <> "" Then
							ParamListBox.Items.Add(TableName.Trim)
						End If
					Next
				Next
			End If
			
			Dim VarTypes() As String = {"byte", "word", "integer", "long", "string"}
			For Each varType As String In VarTypes
				If OptionList(AddListItem) = "number" Or OptionList(AddListItem) = "var" Or OptionList(AddListItem) = varType Or OptionList(AddListItem) = "var_" + varType Then
					For Temp = 0 To Variables.Count - 1
						With Variables.GetItem(Temp)
							If .Type = varType And Not .IsArray Then
								ParamListBox.Items.Add(.Name.Trim)
							End If
						End With
					Next
					
					If Not CurrentSub Is Nothing Then
						Dim currParam As SubParam
						For Each currParam In CurrentSub.Parameters
							If currParam.DataType.ToLower = varType Then
								ParamListBox.Items.Add(currParam.Name)
							End If
						Next
						
						If CurrentSub.IsFunction And CurrentSub.ReturnType = varType Then
							ParamListBox.Items.Add(CurrentSub.Name)
						End If
					End If
					
					'Get variables from library
					Dim CheckLib As LibraryType
					For Each CheckLib In Libraries
						Dim currVar As VariableListItem
						For Each currVar In CheckLib.Variables
							If Not currVar.Hidden And currVar.Type.ToLower = varType And Not currVar.IsArray Then
								ParamListBox.Items.Add(CurrVar.Name)
							End If
						Next
					Next
					
				End If
			Next
			
			'Arrays
			If OptionList(AddListItem) = "array" Then
				For Temp = 0 To Variables.Count - 1
					With Variables.GetItem(Temp)
						If .IsArray Then
							ParamListBox.Items.Add(.Name.Trim)
						End If
					End With
				Next
				
				If Not CurrentSub Is Nothing Then
					Dim currParam As SubParam
					For Each currParam In CurrentSub.Parameters
						If currParam.DataType.ToLower = "array" Then
							ParamListBox.Items.Add(currParam.Name)
						End If
					Next
				End If
				
			End If
			
			'Bit vars
			If OptionList(AddListItem) = "bit" Then
				Dim Constant As Setting
				For Each Constant In Constants
					If LowLevel.IsBitConst(Constant.Value) Then
						ParamListBox.Items.Add(Constant.Name.Trim)
					End If
				Next
				Dim CheckLib As LibraryType
				For Each CheckLib In Libraries
					For Each Constant In CheckLib.Constants
						If LowLevel.IsBitConst(Constant.Value) Then
							ParamListBox.Items.Add(Constant.Name.Trim)
						End If
					Next
				Next
			End If
			
			'Constants with literal values
			If OptionList(AddListItem).StartsWith("range") Or OptionList(AddListItem).StartsWith("const") Or _
				OptionList(AddListItem) = "byte" Or OptionList(AddListItem) = "word" Or OptionList(AddListItem) = "integer" Or _
				OptionList(AddListItem) = "long" Or OptionList(AddListItem) = "number" Then
				'Set min and max
				Dim ThisType As String = OptionList(AddListItem).Trim.ToLower
				If ConstRangeSet Then
					If ThisType.EndsWith("byte") Then
						If MaxVal < 255 Then MaxVal = 255
						If MinVal > 0 Then MinVal = 0
					ElseIf ThisType.EndsWith("word") Then
						If MaxVal < 65535 Then MaxVal = 65535
						If MinVal > 0 Then MinVal = 0
					ElseIf ThisType.EndsWith("integer") Then
						If MaxVal < 32767 Then MaxVal = 32767
						If MinVal > -32768 Then MinVal = -32768
					ElseIf ThisType.EndsWith("word") Then
						If MaxVal < 2 ^ 32 - 1 Then MaxVal = 2 ^ 32 - 1
						If MinVal > 0 Then MinVal = 0
					ElseIf ThisType.StartsWith("range") Then
						Dim TempStr As String = ThisType.Substring(5)
						If TempStr.IndexOf(":") = -1 Then TempStr = TempStr.Replace("-", ":")
						MinVal = TempStr.Substring(0, TempStr.IndexOf(":"))
						MaxVal = TempStr.Substring(TempStr.IndexOf(":") + 1)
					Else
						If MaxVal < 2 ^ 32 - 1 Then MaxVal = 2 ^ 32 - 1
						If MinVal > -32768 Then MinVal = -32768
					End If
				Else
					ConstRangeSet = True
					If ThisType.EndsWith("byte") Then
						MaxVal = 255
						MinVal = 0
					ElseIf ThisType.EndsWith("word") Then
						MaxVal = 65535
						MinVal = 0
					ElseIf ThisType.EndsWith("integer") Then
						MaxVal = 32767
						MinVal = -32768
					ElseIf ThisType.EndsWith("long") Then
						MaxVal = 2 ^ 32 - 1
						MinVal = 0
					ElseIf ThisType.StartsWith("range") Then
						Dim TempStr As String = ThisType.Substring(5)
						If TempStr.IndexOf(":") = -1 Then TempStr = TempStr.Replace("-", ":")
						MinVal = TempStr.Substring(0, TempStr.IndexOf(":"))
						MaxVal = TempStr.Substring(TempStr.IndexOf(":") + 1)
					Else
						MaxVal = 2 ^ 32 - 1
						MinVal = -32768
					End If
				End If
				
			End If
			
			'Subroutines
			If OptionList(AddListItem) = "subroutine" Then
				'Dim AddSubNo As Integer
				
			End If
		Next
		
		'Having checked types, now find constants with correct literal values
		If ConstRangeSet Then
			'MessageBox.Show("Min:" + minVal.ToString + ", Max:" + maxVal.ToString, "Values")
			'This program
			Dim Constant As Setting
			Dim ThisValue As Long
			For Each Constant In Constants
				If LowLevel.IsValue(Constant.Value) Then
					ThisValue = LowLevel.MakeDec(Constant.Value)
					If ThisValue >= MinVal And ThisValue <= MaxVal Then
						'Add constant
						ParamListBox.Items.Add(Constant.Name.Trim)
					End If
				End If
			Next
			'Libraries
			Dim CheckLib As LibraryType
			For Each CheckLib In Libraries
				For Each Constant In CheckLib.Constants
					If LowLevel.IsValue(Constant.Value) Then
						ThisValue = LowLevel.MakeDec(Constant.Value)
						If ThisValue >= MinVal And ThisValue <= MaxVal Then
							'Add constant
							ParamListBox.Items.Add(Constant.Name.Trim)
						End If
					End If
				Next
			Next
		End If
		
		'Find and remove duplicates
		Dim Look1, Look2 As Integer
		If ParamListBox.Items.Count > 2 Then
			Look1 = 1
			Do
				For Look2 = 0 To Look1 - 1
					If ParamListBox.Items(Look1) = ParamListBox.Items(Look2) Then
						ParamListBox.Items.RemoveAt(Look1)
						Look1 -= 1
						Exit For
					End If
				Next
				Look1 += 1
			Loop While Look1 < ParamListBox.Items.Count
		End If
		
		'Select appropriate option
		Dim checkItem As Integer
		For checkItem = 0 To ParamListBox.Items.Count - 1
			If ParamListBox.Items(checkItem).ToLower.Trim = CurrentValue.ToLower.Trim Then
				ParamListBox.SelectedIndex = checkItem
				Exit For
			End If
		Next
		
	End Sub
	
	Public Function FindGeneratedFile(ByVal FileExt As String) As String
		'Finds the name of a generated file
		Dim Temp As Integer
		
		'Make sure FileExt has a . before it
		If Not FileExt.StartsWith(".") Then
			FileExt = "." + FileExt
		End If
		
		'Find name that original file has
		Dim GeneratedFile As String
		If Filename.ToLower.Substring(0,8) = "untitled" Or FileLocked Then
			GeneratedFile = System.Environment.GetEnvironmentVariable("Temp") + "\GCGB.tmp"
		Else
			GeneratedFile = Filename
		End If
		
		'Replace extension
		For Temp = GeneratedFile.Length - 1 To 0 Step -1
			If GeneratedFile.Substring(Temp, 1) = "." Then
				GeneratedFile = GeneratedFile.Substring(0, Temp) + FileExt
				Exit For
			End If
		Next
		
		Return GeneratedFile
	End Function
	
	Public Function FindSFRNames As List(Of String)
		'Find all special function registers and bits used by program
		Dim SFRList As New List(Of String)
		
		'If no chipdata, can't possible find any SFRs
		CheckChipData
		If ChipData Is Nothing Then
			Return SFRList
		End If
		
		Dim SearchSub As GCBSubroutine
		Dim SearchLine As String
		Dim LineElements As List(Of String)
		Dim CurrElement As String
		
		For Each SearchSub In Subroutines
			For Each SearchLine in SearchSub.Lines
				LineElements = ExtractVars(SearchLine)
				For Each CurrElement In LineElements
					If ChipData.SFRVars.ContainsKey(CurrElement) Then
						SFRList.Add(CurrElement)
					Else If ChipData.SFRVarBits.ContainsKey(CurrElement) Then
						SFRList.Add(CurrElement)
					End If
				Next
			Next
		Next
		
		Return SFRList
	End Function
	
	Public Function GetParams(ByVal InputLine As String, CmdRegex As Regex) As List(Of String)
		'Extract parameters from InputLine using CmdRegex
		'Regex must extract values for p1, p2, etc
		Dim CurrParamNo As Integer
		Dim OutList As New List(Of String)
		
		'Try to match line to regex
		Dim m As Match = CmdRegex.Match(InputLine)
		If m.Success Then
			For CurrParamNo = 1 To MainForm.MaxIconParams
				OutList.Add(m.Result("${p" + CurrParamNo.ToString + "}"))
			Next
		End If
		
		Return OutList
	End Function
	
	Public Function GetParamString(ByVal InputLine As String, ByVal SyntaxTemp As String) As List(Of String)
		Dim SyntaxPointer As String = ""
		Dim SyntaxSource As Integer = 0
		'Dim IconParameters As String = ""
		Dim OutList As New List(Of String)
		
		Dim CommandTemp, CommandRemainder As String
		Dim ParamStart, ParamData, ParamEnd As String
		Dim ParamNo As Integer
		
		'Get syntax (if necessary)
		If SyntaxTemp <> "" Then SyntaxSource = 1
		If SyntaxTemp = "" Then
			SyntaxPointer = GetCommandData(InputLine)
			If SyntaxPointer.ToLower.IndexOf("command:") <> -1 Then
				SyntaxPointer = SyntaxPointer.Substring(SyntaxPointer.IndexOf(":") + 1)
				SyntaxTemp = MainForm.CommandList(SyntaxPointer).Syntax
				SyntaxSource = 1
			End If
		End If
		
		If SyntaxSource = 0 Then Return Nothing
		
		CommandTemp = InputLine.Trim + " EOF"
		SyntaxTemp = SyntaxTemp.ToLower.Trim + " EOF"
		'MessageBox.Show(CommandTemp + "," + SyntaxTemp, "Message", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1)
		Do While SyntaxTemp.IndexOf("%p") <> -1
			'Find code before and after
			ParamStart = SyntaxTemp.Substring(0, SyntaxTemp.IndexOf("%p"))
			ParamNo = Microsoft.VisualBasic.Conversion.Val(SyntaxTemp.Substring(SyntaxTemp.IndexOf("%p")+2))
			ParamData = CommandTemp.Substring(CommandTemp.ToLower.IndexOf(ParamStart.ToLower)+ParamStart.Length)
			
			SyntaxTemp = SyntaxTemp.Substring(SyntaxTemp.ToLower.IndexOf("%p")+2)
			SyntaxTemp = SyntaxTemp.Substring(SyntaxTemp.IndexOf("%")+1)
			ParamEnd = SyntaxTemp
			If ParamEnd.IndexOf("%") <> -1 Then ParamEnd = SyntaxTemp.Substring(0, SyntaxTemp.IndexOf("%"))
			If ParamEnd.Trim <> "" Then ParamEnd = ParamEnd.Trim
			If ParamData.ToLower.IndexOf(ParamEnd.ToLower) <> -1 Then ParamData = ParamData.Substring(0, ParamData.ToLower.IndexOf(ParamEnd.ToLower))
			
			'MessageBox.Show(ParamStart + "," + ParamEnd + "," + ParamData + "," + CommandTemp, "Message", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1)
			
			If ParamData.ToUpper.Trim = "EOF" Then Exit Do
			
			'CommandTemp = CommandTemp.Substring(CommandTemp.ToLower.IndexOf(ParamEnd.ToLower))'+ParamEnd.Length)
			CommandTemp = CommandTemp.Substring(CommandTemp.ToLower.IndexOf(ParamData.ToLower)+ParamData.Length)
			'IconParameters = IconParameters + ParamNo.ToString + "[" + ParamData.Trim + "]"
			OutList.Add(ParamData.Trim)
			
			CommandRemainder = CommandTemp.ToUpper.Replace("EOF", "").Trim
			'MessageBox.Show(CommandTemp + "," + CommandRemainder, "Message", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1)
			if CommandRemainder = "" Then Exit Do
			
		Loop
		
		'Return IconParameters
		Return OutList
		
	End Function
	
End Class
