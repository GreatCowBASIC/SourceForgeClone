'
' Created by SharpDevelop.
' User: Administrator
' Date: 2/04/2007
' Time: 4:45 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Collections

Namespace Great_Cow_Graphical_BASIC
	
	Public Class Variables
		Inherits System.Windows.Forms.Form
		Private Button_OK As System.Windows.Forms.Button
		Private Button_Cancel As System.Windows.Forms.Button
		Private VarList As System.Windows.Forms.ListBox
		Private labelVars As System.Windows.Forms.Label
		Private Button_Sort As System.Windows.Forms.Button
		Private Button_Delete As System.Windows.Forms.Button
		Private Button_Find As System.Windows.Forms.Button
		Private VarsHelp As System.Windows.Forms.HelpProvider
		Private labelRam As System.Windows.Forms.Label
		Private Button_Edit As System.Windows.Forms.Button
		Private VarSpace As System.Windows.Forms.Label
		Private Button_Add As System.Windows.Forms.Button
		
		Dim RamUsed As Integer = 0
		Private Dim VariableList As New ArrayList
		
		Dim CurrentFile As GCBProgram
				
		Public Sub New(CurrentFile As GCBProgram)
			MyBase.New
			'
			' The Me.InitializeComponent call is required for Windows Forms designer support.
			'
			Me.InitializeComponent
			Me.CurrentFile = CurrentFile
			
			Dim Temp As Integer = 0
			
			'Get var list from MainForm
			With CurrentFile
				For Temp = 0 To .Variables.Count - 1
					VariableList.Add(.Variables.GetItem(Temp))
				Next
			End With
			
			'Draw var list
			UpdateVarList
			
		End Sub
		
		#Region " Windows Forms Designer generated code "
		' This method is required for Windows Forms designer support.
		' Do not change the method contents inside the source code editor. The Forms designer might
		' not be able to load this method if it was changed manually.
		Private Sub InitializeComponent()
			Me.Button_Add = New System.Windows.Forms.Button
			Me.VarSpace = New System.Windows.Forms.Label
			Me.Button_Edit = New System.Windows.Forms.Button
			Me.labelRam = New System.Windows.Forms.Label
			Me.VarsHelp = New System.Windows.Forms.HelpProvider
			Me.Button_Find = New System.Windows.Forms.Button
			Me.Button_Delete = New System.Windows.Forms.Button
			Me.Button_Sort = New System.Windows.Forms.Button
			Me.labelVars = New System.Windows.Forms.Label
			Me.VarList = New System.Windows.Forms.ListBox
			Me.Button_Cancel = New System.Windows.Forms.Button
			Me.Button_OK = New System.Windows.Forms.Button
			Me.SuspendLayout
			'
			'Button_Add
			'
			Me.Button_Add.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.Button_Add.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.VarsHelp.SetHelpString(Me.Button_Add, "Add a new variable.")
			Me.Button_Add.Location = New System.Drawing.Point(184, 72)
			Me.Button_Add.Name = "Button_Add"
			Me.VarsHelp.SetShowHelp(Me.Button_Add, true)
			Me.Button_Add.Size = New System.Drawing.Size(88, 24)
			Me.Button_Add.TabIndex = 6
			Me.Button_Add.Text = "Add ..."
			AddHandler Me.Button_Add.Click, AddressOf Me.Button_AddClick
			'
			'VarSpace
			'
			Me.VarSpace.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.VarSpace.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
			Me.VarSpace.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.VarsHelp.SetHelpString(Me.VarSpace, "The number of bytes of RAM that will be used in the microcontroller. Note that th"& _ 
"e actual figure is a little higher, as the compiler uses some RAM for internal v"& _ 
"ariables.")
			Me.VarSpace.Location = New System.Drawing.Point(192, 24)
			Me.VarSpace.Name = "VarSpace"
			Me.VarsHelp.SetShowHelp(Me.VarSpace, true)
			Me.VarSpace.Size = New System.Drawing.Size(72, 24)
			Me.VarSpace.TabIndex = 0
			Me.VarSpace.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
			'
			'Button_Edit
			'
			Me.Button_Edit.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.Button_Edit.Enabled = false
			Me.Button_Edit.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.VarsHelp.SetHelpString(Me.Button_Edit, "Change the name or type of a current variable.")
			Me.Button_Edit.Location = New System.Drawing.Point(184, 104)
			Me.Button_Edit.Name = "Button_Edit"
			Me.VarsHelp.SetShowHelp(Me.Button_Edit, true)
			Me.Button_Edit.Size = New System.Drawing.Size(88, 24)
			Me.Button_Edit.TabIndex = 7
			Me.Button_Edit.Text = "Edit ..."
			AddHandler Me.Button_Edit.Click, AddressOf Me.Button_EditClick
			'
			'labelRam
			'
			Me.labelRam.Location = New System.Drawing.Point(184, 8)
			Me.labelRam.Name = "labelRam"
			Me.labelRam.Size = New System.Drawing.Size(80, 16)
			Me.labelRam.TabIndex = 13
			Me.labelRam.Text = "RAM Used:"
			'
			'Button_Find
			'
			Me.Button_Find.Anchor = System.Windows.Forms.AnchorStyles.Right
			Me.Button_Find.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.VarsHelp.SetHelpString(Me.Button_Find, "Searches the program for any variables that are not shown in the list. This is us"& _ 
"eful for importing GCBASIC programs.")
			Me.Button_Find.Location = New System.Drawing.Point(184, 200)
			Me.Button_Find.Name = "Button_Find"
			Me.VarsHelp.SetShowHelp(Me.Button_Find, true)
			Me.Button_Find.Size = New System.Drawing.Size(88, 24)
			Me.Button_Find.TabIndex = 4
			Me.Button_Find.Text = "Find ..."
			AddHandler Me.Button_Find.Click, AddressOf Me.Button_FindClick
			'
			'Button_Delete
			'
			Me.Button_Delete.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.Button_Delete.Enabled = false
			Me.Button_Delete.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.VarsHelp.SetHelpString(Me.Button_Delete, "Deletes the selected variable.")
			Me.Button_Delete.Location = New System.Drawing.Point(184, 136)
			Me.Button_Delete.Name = "Button_Delete"
			Me.VarsHelp.SetShowHelp(Me.Button_Delete, true)
			Me.Button_Delete.Size = New System.Drawing.Size(88, 24)
			Me.Button_Delete.TabIndex = 8
			Me.Button_Delete.Text = "Delete"
			AddHandler Me.Button_Delete.Click, AddressOf Me.Button_DeleteClick
			'
			'Button_Sort
			'
			Me.Button_Sort.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.Button_Sort.Enabled = false
			Me.Button_Sort.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.VarsHelp.SetHelpString(Me.Button_Sort, "Sorts the variable list alphabetically.")
			Me.Button_Sort.Location = New System.Drawing.Point(184, 264)
			Me.Button_Sort.Name = "Button_Sort"
			Me.VarsHelp.SetShowHelp(Me.Button_Sort, true)
			Me.Button_Sort.Size = New System.Drawing.Size(88, 24)
			Me.Button_Sort.TabIndex = 11
			Me.Button_Sort.Text = "Sort List (A-Z)"
			AddHandler Me.Button_Sort.Click, AddressOf Me.Button_SortClick
			'
			'labelVars
			'
			Me.labelVars.Location = New System.Drawing.Point(8, 8)
			Me.labelVars.Name = "labelVars"
			Me.labelVars.Size = New System.Drawing.Size(160, 16)
			Me.labelVars.TabIndex = 12
			Me.labelVars.Text = "Variables:"
			'
			'VarList
			'
			Me.VarList.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.VarsHelp.SetHelpString(Me.VarList, "List of available variables.")
			Me.VarList.IntegralHeight = false
			Me.VarList.Location = New System.Drawing.Point(8, 24)
			Me.VarList.Name = "VarList"
			Me.VarsHelp.SetShowHelp(Me.VarList, true)
			Me.VarList.Size = New System.Drawing.Size(168, 264)
			Me.VarList.TabIndex = 0
			AddHandler Me.VarList.SelectedIndexChanged, AddressOf Me.VarListSelectedIndexChanged
			'
			'Button_Cancel
			'
			Me.Button_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom
			Me.Button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me.Button_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.Button_Cancel.Location = New System.Drawing.Point(152, 304)
			Me.Button_Cancel.Name = "Button_Cancel"
			Me.Button_Cancel.Size = New System.Drawing.Size(88, 24)
			Me.Button_Cancel.TabIndex = 1
			Me.Button_Cancel.Text = "Cancel"
			'
			'Button_OK
			'
			Me.Button_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom
			Me.Button_OK.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.Button_OK.Location = New System.Drawing.Point(40, 304)
			Me.Button_OK.Name = "Button_OK"
			Me.Button_OK.Size = New System.Drawing.Size(88, 24)
			Me.Button_OK.TabIndex = 0
			Me.Button_OK.Text = "OK"
			AddHandler Me.Button_OK.Click, AddressOf Me.Button_OKClick
			'
			'Variables
			'
			Me.AcceptButton = Me.Button_OK
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.CancelButton = Me.Button_Cancel
			Me.ClientSize = New System.Drawing.Size(282, 335)
			Me.Controls.Add(Me.labelRam)
			Me.Controls.Add(Me.labelVars)
			Me.Controls.Add(Me.Button_Sort)
			Me.Controls.Add(Me.Button_Delete)
			Me.Controls.Add(Me.Button_Edit)
			Me.Controls.Add(Me.Button_Add)
			Me.Controls.Add(Me.Button_Find)
			Me.Controls.Add(Me.Button_Cancel)
			Me.Controls.Add(Me.Button_OK)
			Me.Controls.Add(Me.VarList)
			Me.Controls.Add(Me.VarSpace)
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
			Me.HelpButton = true
			Me.MaximizeBox = false
			Me.MinimizeBox = false
			Me.Name = "Variables"
			Me.ShowInTaskbar = false
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
			Me.Text = "Variables"
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
		Private Sub UpdateVarList
			Dim Temp As Integer = 0
			Dim VarSize As Integer = 0
			Dim TempData As String
			Dim OldSelected As Integer = Me.VarList.SelectedIndex
			Dim ThisVarType As String
			
			'Remove old items
			Me.VarList.Items.Clear()
			RamUsed = 0
			
			'Redraw list
			For Temp = 0 To VariableList.Count - 1
				With VariableList(Temp)
					
					'Get type to display
					ThisVarType = " (" + .Type
					If .IsArray Then
						ThisVarType += " array"
					End If
					ThisVarType += ")"
					
					'Draw
					Me.VarList.Items.Add(.Name + ThisVarType)
					
					'Get new size
					VarSize = 1
					If .Type.Trim.ToLower = "word" Then
						VarSize = 2
					ElseIf .Type.Trim.ToLower = "integer" Then
						VarSize = 2
					ElseIf .Type.Trim.ToLower = "string" Then
						VarSize = 20
					End If
					If .IsArray Then
						VarSize *= .Length
					End If
					RamUsed += VarSize
				End With
			Next
			
			'Select
			Try
				Me.VarList.SelectedIndex = OldSelected
			Catch
				If OldSelected <> -1 Then Me.VarList.SelectedIndex = OldSelected - 1
			End Try
			
			'Display RAM used
			Me.VarSpace.Text = RamUsed.ToString
			
			'Update buttons
			UpdateButtonStatus
		End Sub
		
		Private Function ExtractVars(ByVal ScanLine As String) As String
			Dim Temp As Integer = 0
			Dim WordList(100) As String
			Dim WordCount As Integer = 0
			Dim DelStart, DelEnd As Integer
			Dim TempData As String
			Dim IsNotVar As Boolean
			Dim AllDigits(10) As Char
			Dim FindConst As Integer
			Dim OutputBuffer As String = ""
			Dim CurrentLib, LibLine As Integer
			
			'Set up AllDigits array
			AllDigits(0) = "0"
			AllDigits(1) = "1"
			AllDigits(2) = "2"
			AllDigits(3) = "3"
			AllDigits(4) = "4"
			AllDigits(5) = "5"
			AllDigits(6) = "6"
			AllDigits(7) = "7"
			AllDigits(8) = "8"
			AllDigits(9) = "9"
			AllDigits(10) = "0"
			
			'MessageBox.Show(ScanLine, "Line", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1)
			
			'Don't process labels or comments
			If ScanLine.Trim.EndsWith(":") Then Return ""
			If ScanLine.Trim.StartsWith("'") Then Return ""
			
			'Don't process some commands
			If ScanLine.ToLower.Trim.StartsWith("goto ") Then Return ""
			If ScanLine.ToLower.Trim.StartsWith("end ") Then Return ""
			If ScanLine.ToLower.Trim.StartsWith("exit ") Then Return ""
			If ScanLine.ToLower.Trim = "loop" Then Return ""
			If ScanLine.ToLower.Trim = "next" Then Return ""
			If ScanLine.ToLower.Trim.StartsWith("dir ") Then ScanLine = ScanLine.Substring(3).Trim
			
			'MessageBox.Show(ScanLine, "Line", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1)
			
			'Remove string literals from ScanLine
			Do While ScanLine.IndexOf("""") <> -1
				DelStart = ScanLine.IndexOf("""")
				DelEnd = ScanLine.IndexOf("""", DelStart + 1)
				If DelEnd = -1 Then ScanLine = ScanLine.Replace("""", ""): Exit Do
				ScanLine = ScanLine.Substring(0, DelStart) + ScanLine.Substring(DelEnd + 1)
				'MessageBox.Show(ScanLine + ":" + DelStart.ToString + ":" + DelEnd.ToString, "Line", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1)
			Loop
			
			'Add dividers to ScanLine
			ScanLine = ScanLine.Replace(" ", ":").Replace(",", ":").Replace("=", ":").Replace(".", ":")
			ScanLine = ScanLine.Replace("<", ":").Replace(">", ":").Replace("!", ":")
			ScanLine = ScanLine.Replace("<>", ":").Replace("<=", ":").Replace(">=", ":").Replace("=<", ":").Replace("=>", ":")
			ScanLine = ScanLine.Replace("*", ":").Replace("/", ":").Replace("%", ":").Replace("+", ":").Replace("-",":")
			ScanLine = ScanLine.Replace("&", ":").Replace("|", ":").Replace("#", ":").Replace("(", ":").Replace(")", ":") + ":"
			Do While ScanLine.IndexOf("::") <> -1
				ScanLine = ScanLine.Replace("::", ":")
			Loop
			
			'Remove first section, it should be a command
			ScanLine = ScanLine.Substring(ScanLine.IndexOf(":") + 1) + " "
			
			'Break up ScanLine
			Do While ScanLine.IndexOf(":") <> -1
				WordCount += 1
				WordList(WordCount) = ScanLine.Substring(0, ScanLine.IndexOf(":"))
				ScanLine = ScanLine.Substring(ScanLine.IndexOf(":") + 1)
			Loop
			
			'Remove anything that does not belong
			For Temp = 1 To WordCount
				TempData = WordList(Temp).Trim.ToLower
				IsNotVar = False
				
				'Literals
				If TempData.IndexOfAny(AllDigits) = 0 Then IsNotVar = True
				
				'Keywords
				If TempData = "to" Then IsNotVar = True
				If TempData = "then" Then IsNotVar = True
				If TempData = "until" Or TempData = "while" Then IsNotVar = True
				If TempData = "us" Or TempData = "ms" Or TempData = "s" Or TempData = "m" Or TempData = "h" Then IsNotVar = True
				If TempData = "usec" Or TempData = "msec" Or TempData = "sec" Or TempData = "min" Then IsNotVar = True
				If TempData = "true" Or TempData = "false" Then IsNotVar = True
				If TempData = "on" Or TempData = "off" Then IsNotVar = True
				If TempData = "in" Or TempData = "out" Or TempData = "input" Or TempData = "output" Then IsNotVar = True
				
				'Program constants
				Dim ConstTemp As Setting
				If Not IsNotVar Then
					With CurrentFile
						For Each ConstTemp In .Constants
							If ConstTemp.Name.Trim.ToLower = TempData Then IsNotVar = True: Exit For
						Next
					End With
				End If
				
				'Library constants
				'File, Include File, Element, Data
				'Element: 0 = sub, 1 = constants, 2 = chip settings
				'Public Dim Shared LibraryData(MaxFilesOpen, MaxIncludes, 2, 100) As String
				'Public Dim Shared LibraryCount(MaxFilesOpen, MaxIncludes, 2) As Integer
				Dim LibTemp As LibraryType
				With CurrentFile
					For Each LibTemp in .Libraries
						For Each ConstTemp In LibTemp.Constants
							If ConstTemp.Name.Trim.ToLower = TempData Then IsNotVar = True: Goto FoundInLib
						Next
					Next
				End With
				FoundInLib:
				
				'Constant group constants
				
				'Function names
				
				
				If IsNotVar Then WordList(Temp) = ""								
			Next
			
			'Create list of variables
			OutputBuffer = ""
			For Temp = 1 To WordCount
				If WordList(Temp) <> "" Then
					'MessageBox.Show(WordList(Temp), "Message", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1)
					If OutputBuffer <> "" Then OutputBuffer = OutputBuffer + "," + WordList(Temp)
					If OutputBuffer = "" Then OutputBuffer = WordList(Temp)
				End If
			Next
			
			Return OutputBuffer
			
		End Function

		Private Sub Button_FindClick(sender As System.Object, e As System.EventArgs)
			
			'Confirm
			Dim result As DialogResult = MessageBox.Show("This will search the program for unlisted variables, and add them to the list. Some false detections may occur, and these will need to be manually removed.", "Great Cow Graphical BASIC", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
			If result = DialogResult.Cancel Then Exit Sub
			
			'Initialise Vars
			Dim Temp As Integer = 0
			Dim SearchList As Integer = 0
			'Dim SearchSub, SearchLine As Integer
			Dim SearchSub As GCBSubroutine
			Dim SearchLine As String
			Dim TempVarList(200) As String
			Dim TempVarCount As Integer = 0
			Dim TempData As String
			Dim VarExists As Boolean
			
			'Scan program for vars
			With CurrentFile
				For Each SearchSub In .Subroutines
					For Each SearchLine in SearchSub.Lines
						TempData = ExtractVars(SearchLine)
						If TempData <> "" Then
							'Get list of new vars
							Do While TempData.IndexOf(",") <> -1
								TempVarCount += 1
								TempVarList(TempVarCount) = TempData.Substring(0, TempData.IndexOf(","))
								TempData = TempData.Substring(TempData.IndexOf(",") + 1)
							Loop
							TempVarCount += 1
							TempVarList(TempVarCount) = TempData
							
							'Combine with existing list
							For Temp = 1 To TempVarCount
								VarExists = False
								For SearchList = 0 To VariableList.Count - 1
									If VariableList(SearchList).Name.Trim.ToLower = TempVarList(Temp).Trim.ToLower Then
										VarExists = True
										Exit For
									End If
								Next
								If Not VarExists Then
									VariableList.Add(New VariableListItem(TempVarList(Temp).Trim, "byte"))
								End If
							Next
						End If					
					Next
				Next
			End With
			
			'Update display
			UpdateVarList
			
		End Sub
		
		Private Sub Button_SortClick(sender As System.Object, e As System.EventArgs)
			Dim Temp As Integer = 0
			Dim SortMore As Boolean
			Dim SwapTemp As VariableListItem
			
			If VariableList.Count < 2 Then Exit Sub
			
			'Sort List
			SortVarListMore:
			SortMore = False
			For Temp = 0 To VariableList.Count - 2
				If VariableList(Temp).Name > VariableList(Temp + 1).Name Then
					SwapTemp = VariableList(Temp)
					VariableList(Temp) = VariableList(Temp + 1)
					VariableList(Temp + 1) = SwapTemp
					SortMore = True
				End If
			Next
			If SortMore Then Goto SortVarListMore
			
			'Update
			UpdateVarList
			
		End Sub
		
		Private Sub Button_OKClick(sender As System.Object, e As System.EventArgs)
			Dim Temp As Integer
			
			With CurrentFile
				'Clear old values
				.Variables.Clear
				
				'Copy over new values
				For Temp = 0 To VariableList.Count - 1
					.Variables.AddVar(VariableList(Temp))
				Next
				
				'Exit
				.HasChanged = True
			End With
			Me.Close()
			
		End Sub
		
		Private Sub Button_DeleteClick(sender As System.Object, e As System.EventArgs)
			Dim Temp As Integer = 0
			Dim DeleteIndex As Integer = Me.VarList.SelectedIndex
			If DeleteIndex = -1 Then Exit Sub
			
			'Delete
			VariableList.RemoveAt(DeleteIndex)
			
			'Redraw
			UpdateVarList
			
		End Sub
		
		Private Sub Button_AddClick(sender As System.Object, e As System.EventArgs)
			Dim SearchList As Integer = 0
			Dim VarExists As Boolean = False
			Dim FoundVar As VariableListItem = Nothing
			Dim TempData As String = ""
			
			'Display dialog
			Dim fEditVar As New EditVar
			fEditVar.Text = "Add Variable"
			fEditVar.InitEdit("", "byte", 1)
			
			GetNewVarName:
			
			fEditVar.ShowDialog()
			
			'Apply
			If fEditVar.NewVarName <> "" Then
				VarExists = False
				For SearchList = 0 To VariableList.Count - 1
					If VariableList(SearchList).Name.Trim.ToLower = fEditVar.NewVarName.Trim.ToLower Then
						VarExists = True
						FoundVar = VariableList(SearchList)
						Exit For
					End If
				Next
				
				If Not VarExists Then
					TempData = fEditVar.NewVarType
					If TempData = "array" Then
						VariableList.Add(New VariableListItem(fEditVar.NewVarName.Trim, "byte", True, fEditVar.NewArraySize))
					Else
						VariableList.Add(New VariableListItem(fEditVar.NewVarName.Trim, TempData))
					End If
					
					UpdateVarList
					
				Else
					'Variable with the same name as the new one already exists
					Dim DecideAction As New VariableConflict
					DecideAction.VarName = fEditVar.NewVarName
					DecideAction.ShowDialog
					
					Select Case DecideAction.Result
						Case VariableConflict.Resolution.Rename
							Goto GetNewVarName
							
						Case VariableConflict.Resolution.Replace
							'Remove old
							VariableList.Remove(FoundVar)
							
							'Add new
							TempData = fEditVar.NewVarType
							If TempData = "array" Then
								VariableList.Add(New VariableListItem(fEditVar.NewVarName.Trim, "byte", True, fEditVar.NewArraySize))
							Else
								VariableList.Add(New VariableListItem(fEditVar.NewVarName.Trim, TempData))
							End If
							
							UpdateVarList
							
						Case VariableConflict.Resolution.Cancel
							Exit Sub
							
					End Select
					
				End If
			End If
			
		End Sub
		
		Private Sub Button_EditClick(sender As System.Object, e As System.EventArgs)
			Dim NewType As String = ""
			Dim NewSize As Integer = 1
			Dim SearchList As Integer = 0
			Dim VarExists As Boolean = False
			Dim TempData As String = ""
			
			'Get location and existing values
			Dim EditIndex As Integer = Me.VarList.SelectedIndex
			If EditIndex = -1 Then Exit Sub
			
			NewType = VariableList(EditIndex).Type.Trim
			If VariableList(EditIndex).IsArray Then
				NewType = "array"
				NewSize = VariableList(EditIndex).Length		
			End If
			
			'Display dialog
			Dim fEditVar As New EditVar
			fEditVar.Text = "Edit Variable"
			fEditVar.InitEdit(VariableList(EditIndex).Name, NewType, NewSize)
			fEditVar.ShowDialog()
						
			'Apply
			If fEditVar.NewVarName <> "" Then
				TempData = fEditVar.NewVarType
				If TempData = "array" Then
					VariableList(EditIndex) = New VariableListItem(fEditVar.NewVarName.Trim, "byte", True, fEditVar.NewArraySize)
				Else
					VariableList(EditIndex) = New VariableListItem(fEditVar.NewVarName.Trim, TempData)
				End If
				
				UpdateVarList
			End If
			
		End Sub
		
		Private Sub UpdateButtonStatus
			'Enable/disable buttons as appropriate
			If VarList.SelectedIndex = -1 Then
				Button_Edit.Enabled = False
				Button_Delete.Enabled = False
			Else
				Button_Edit.Enabled = True
				Button_Delete.Enabled = True
			End If
			
			If VarList.Items.Count < 2 Then
				Button_Sort.Enabled = False
			Else
				Button_Sort.Enabled = True
			End If
			
		End Sub
		
		Private Sub VarListSelectedIndexChanged(sender As System.Object, e As System.EventArgs)
			UpdateButtonStatus
			
		End Sub
		
	End Class
End Namespace
