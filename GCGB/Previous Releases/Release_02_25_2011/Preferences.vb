'
' Created by SharpDevelop.
' User: Administrator
' Date: 1/04/2007
' Time: 11:36 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.IO
Imports System.Drawing
Imports System.Windows.Forms

Namespace Great_Cow_Graphical_BASIC
	
	Public Class Preferences
		Inherits System.Windows.Forms.Form
		Private Button_DeleteProgrammer As System.Windows.Forms.Button
		Private ProgrammerList As System.Windows.Forms.ListBox
		Private CompilerVerbose As System.Windows.Forms.CheckBox
		Private EditorWarnRecursion As System.Windows.Forms.CheckBox
		Private EditorPrefs As System.Windows.Forms.TabPage
		Private buttonDeleteTool As System.Windows.Forms.Button
		Private Button_OK As System.Windows.Forms.Button
		Private Button_EditProgrammer As System.Windows.Forms.Button
		Private CompilerPause As System.Windows.Forms.CheckBox
		Private buttonAddTool As System.Windows.Forms.Button
		Private PrefsHelp As System.Windows.Forms.HelpProvider
		Private Button_AddProgrammer As System.Windows.Forms.Button
		Private Button_Cancel As System.Windows.Forms.Button
		Private ProgrammerPrefs As System.Windows.Forms.TabPage
		Private toolList As System.Windows.Forms.ListBox
		Private PrefsTabs As System.Windows.Forms.TabControl
		Private CompilerShowBASIC As System.Windows.Forms.CheckBox
		Private buttonEditTool As System.Windows.Forms.Button
		Private CompilerPrefs As System.Windows.Forms.TabPage
		Private ExtToolsPrefs As System.Windows.Forms.TabPage
		
		Public Sub New()
			MyBase.New
			'
			' The Me.InitializeComponent call is required for Windows Forms designer support.
			'
			Me.InitializeComponent
			
			'Editor
			EditorWarnRecursion.Checked = False
			If PrefIsYes(GetPref("GCGB", "WarnRecursion")) Then EditorWarnRecursion.Checked = True
						
			'Compiler
			CompilerVerbose.Checked = False
			If PrefIsYes(GetPref("GCBASIC", "Verbose")) Then CompilerVerbose.Checked = True
			CompilerShowBASIC.Checked = False
			If GetPref("GCBASIC", "Preserve") = "a" Then CompilerShowBASIC.Checked = True
			CompilerPause.Checked = False
			If PrefIsYes(GetPref("GCBASIC", "PauseAfterCompile")) Then CompilerPause.Checked = True
			
			'Programmer list
			'Restricted mode means that GCGB is in a school or other environment where the users may be up to no good
			'If restricted, don't allow them to define programmers.
			If PrefIsYes(GetPref("GCGB", "Restricted")) Then
				Me.PrefsTabs.Controls.Remove(ProgrammerPrefs)
			Else
				'Find all tools
				Dim section As SettingSection
				Dim ProgName As String
				For Each section In MainForm.PrefGroups
					If section.Name.ToLower.StartsWith("tool") Then
						If section.GetValue("type") = "programmer" Then
							ProgrammerList.Items.Add(section.GetValue("desc"))
							ProgName = section.Name
							ProgName = ProgName.Substring(ProgName.IndexOf("=") + 1).Trim
							If ProgName = GetPref("GCBASIC", "Programmer") Then
								ProgrammerList.SelectedIndex = ProgrammerList.Items.Count - 1
							End If
						End If
					End If
				Next
			End If
			
			'External tools list
			'Restricted mode means that GCGB is in a school or other environment where the users may be up to no good
			'If restricted, don't allow them to define programmers.
			If PrefIsYes(GetPref("GCGB", "Restricted")) Then
				Me.PrefsTabs.Controls.Remove(ExtToolsPrefs)
			Else
				'Find all tools
				Dim section As SettingSection
				Dim ProgName As String
				For Each section In MainForm.PrefGroups
					If section.Name.ToLower.StartsWith("tool") Then
						If section.GetValue("type") = "external" Then
							toolList.Items.Add(section.GetValue("desc"))
						End If
					End If
				Next
			End If
			
		End Sub
		
		#Region " Windows Forms Designer generated code "
		' This method is required for Windows Forms designer support.
		' Do not change the method contents inside the source code editor. The Forms designer might
		' not be able to load this method if it was changed manually.
		Private Sub InitializeComponent()
			Me.ExtToolsPrefs = New System.Windows.Forms.TabPage
			Me.CompilerPrefs = New System.Windows.Forms.TabPage
			Me.buttonEditTool = New System.Windows.Forms.Button
			Me.CompilerShowBASIC = New System.Windows.Forms.CheckBox
			Me.PrefsTabs = New System.Windows.Forms.TabControl
			Me.toolList = New System.Windows.Forms.ListBox
			Me.ProgrammerPrefs = New System.Windows.Forms.TabPage
			Me.Button_Cancel = New System.Windows.Forms.Button
			Me.Button_AddProgrammer = New System.Windows.Forms.Button
			Me.PrefsHelp = New System.Windows.Forms.HelpProvider
			Me.buttonAddTool = New System.Windows.Forms.Button
			Me.CompilerPause = New System.Windows.Forms.CheckBox
			Me.Button_EditProgrammer = New System.Windows.Forms.Button
			Me.Button_OK = New System.Windows.Forms.Button
			Me.buttonDeleteTool = New System.Windows.Forms.Button
			Me.EditorPrefs = New System.Windows.Forms.TabPage
			Me.EditorWarnRecursion = New System.Windows.Forms.CheckBox
			Me.CompilerVerbose = New System.Windows.Forms.CheckBox
			Me.ProgrammerList = New System.Windows.Forms.ListBox
			Me.Button_DeleteProgrammer = New System.Windows.Forms.Button
			Me.ExtToolsPrefs.SuspendLayout
			Me.CompilerPrefs.SuspendLayout
			Me.PrefsTabs.SuspendLayout
			Me.ProgrammerPrefs.SuspendLayout
			Me.EditorPrefs.SuspendLayout
			Me.SuspendLayout
			'
			'ExtToolsPrefs
			'
			Me.ExtToolsPrefs.Controls.Add(Me.buttonDeleteTool)
			Me.ExtToolsPrefs.Controls.Add(Me.buttonEditTool)
			Me.ExtToolsPrefs.Controls.Add(Me.buttonAddTool)
			Me.ExtToolsPrefs.Controls.Add(Me.toolList)
			Me.ExtToolsPrefs.Location = New System.Drawing.Point(4, 22)
			Me.ExtToolsPrefs.Name = "ExtToolsPrefs"
			Me.ExtToolsPrefs.Size = New System.Drawing.Size(280, 262)
			Me.ExtToolsPrefs.TabIndex = 3
			Me.ExtToolsPrefs.Text = "External Tools"
			'
			'CompilerPrefs
			'
			Me.CompilerPrefs.Controls.Add(Me.CompilerPause)
			Me.CompilerPrefs.Controls.Add(Me.CompilerShowBASIC)
			Me.CompilerPrefs.Controls.Add(Me.CompilerVerbose)
			Me.CompilerPrefs.Location = New System.Drawing.Point(4, 22)
			Me.CompilerPrefs.Name = "CompilerPrefs"
			Me.CompilerPrefs.Size = New System.Drawing.Size(280, 262)
			Me.CompilerPrefs.TabIndex = 1
			Me.CompilerPrefs.Text = "Compiler"
			'
			'buttonEditTool
			'
			Me.buttonEditTool.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.PrefsHelp.SetHelpString(Me.buttonEditTool, "Edit the settings of an existing external tool")
			Me.buttonEditTool.Location = New System.Drawing.Point(96, 232)
			Me.buttonEditTool.Name = "buttonEditTool"
			Me.PrefsHelp.SetShowHelp(Me.buttonEditTool, true)
			Me.buttonEditTool.Size = New System.Drawing.Size(88, 24)
			Me.buttonEditTool.TabIndex = 2
			Me.buttonEditTool.Text = "Edit..."
			AddHandler Me.buttonEditTool.Click, AddressOf Me.ButtonEditToolClick
			'
			'CompilerShowBASIC
			'
			Me.CompilerShowBASIC.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.PrefsHelp.SetHelpString(Me.CompilerShowBASIC, "Copies the original BASIC program into the assembly file produced by the compiler"& _ 
". Useful for showing the link between icons and assembly commands.")
			Me.CompilerShowBASIC.Location = New System.Drawing.Point(16, 40)
			Me.CompilerShowBASIC.Name = "CompilerShowBASIC"
			Me.PrefsHelp.SetShowHelp(Me.CompilerShowBASIC, true)
			Me.CompilerShowBASIC.Size = New System.Drawing.Size(224, 16)
			Me.CompilerShowBASIC.TabIndex = 1
			Me.CompilerShowBASIC.Text = "Show BASIC in assembly listing"
			'
			'PrefsTabs
			'
			Me.PrefsTabs.Controls.Add(Me.EditorPrefs)
			Me.PrefsTabs.Controls.Add(Me.CompilerPrefs)
			Me.PrefsTabs.Controls.Add(Me.ProgrammerPrefs)
			Me.PrefsTabs.Controls.Add(Me.ExtToolsPrefs)
			Me.PrefsTabs.Location = New System.Drawing.Point(8, 8)
			Me.PrefsTabs.Name = "PrefsTabs"
			Me.PrefsTabs.SelectedIndex = 0
			Me.PrefsTabs.Size = New System.Drawing.Size(288, 288)
			Me.PrefsTabs.TabIndex = 0
			'
			'toolList
			'
			Me.PrefsHelp.SetHelpString(Me.toolList, "Shows the external tools that have been set up to work with Great Cow Graphical B"& _ 
"ASIC")
			Me.toolList.Location = New System.Drawing.Point(8, 8)
			Me.toolList.Name = "toolList"
			Me.PrefsHelp.SetShowHelp(Me.toolList, true)
			Me.toolList.Size = New System.Drawing.Size(264, 212)
			Me.toolList.TabIndex = 0
			'
			'ProgrammerPrefs
			'
			Me.ProgrammerPrefs.Controls.Add(Me.Button_DeleteProgrammer)
			Me.ProgrammerPrefs.Controls.Add(Me.Button_EditProgrammer)
			Me.ProgrammerPrefs.Controls.Add(Me.Button_AddProgrammer)
			Me.ProgrammerPrefs.Controls.Add(Me.ProgrammerList)
			Me.ProgrammerPrefs.Location = New System.Drawing.Point(4, 22)
			Me.ProgrammerPrefs.Name = "ProgrammerPrefs"
			Me.ProgrammerPrefs.Size = New System.Drawing.Size(280, 262)
			Me.ProgrammerPrefs.TabIndex = 2
			Me.ProgrammerPrefs.Text = "Programmers"
			'
			'Button_Cancel
			'
			Me.Button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me.Button_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.Button_Cancel.Location = New System.Drawing.Point(160, 304)
			Me.Button_Cancel.Name = "Button_Cancel"
			Me.Button_Cancel.Size = New System.Drawing.Size(80, 24)
			Me.Button_Cancel.TabIndex = 2
			Me.Button_Cancel.Text = "Cancel"
			'
			'Button_AddProgrammer
			'
			Me.Button_AddProgrammer.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.PrefsHelp.SetHelpString(Me.Button_AddProgrammer, "Add a new programmer")
			Me.Button_AddProgrammer.Location = New System.Drawing.Point(8, 232)
			Me.Button_AddProgrammer.Name = "Button_AddProgrammer"
			Me.PrefsHelp.SetShowHelp(Me.Button_AddProgrammer, true)
			Me.Button_AddProgrammer.Size = New System.Drawing.Size(88, 24)
			Me.Button_AddProgrammer.TabIndex = 1
			Me.Button_AddProgrammer.Text = "Add..."
			AddHandler Me.Button_AddProgrammer.Click, AddressOf Me.Button_AddProgrammerClick
			'
			'buttonAddTool
			'
			Me.buttonAddTool.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.PrefsHelp.SetHelpString(Me.buttonAddTool, "Add a new external tool")
			Me.buttonAddTool.Location = New System.Drawing.Point(8, 232)
			Me.buttonAddTool.Name = "buttonAddTool"
			Me.PrefsHelp.SetShowHelp(Me.buttonAddTool, true)
			Me.buttonAddTool.Size = New System.Drawing.Size(88, 24)
			Me.buttonAddTool.TabIndex = 1
			Me.buttonAddTool.Text = "Add..."
			AddHandler Me.buttonAddTool.Click, AddressOf Me.ButtonAddToolClick
			'
			'CompilerPause
			'
			Me.CompilerPause.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.PrefsHelp.SetHelpString(Me.CompilerPause, "Keeps the compiler window open until a key is pressed")
			Me.CompilerPause.Location = New System.Drawing.Point(16, 64)
			Me.CompilerPause.Name = "CompilerPause"
			Me.PrefsHelp.SetShowHelp(Me.CompilerPause, true)
			Me.CompilerPause.Size = New System.Drawing.Size(168, 16)
			Me.CompilerPause.TabIndex = 2
			Me.CompilerPause.Text = "Pause after compilation"
			'
			'Button_EditProgrammer
			'
			Me.Button_EditProgrammer.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.PrefsHelp.SetHelpString(Me.Button_EditProgrammer, "Edit the settings of an existing programmer")
			Me.Button_EditProgrammer.Location = New System.Drawing.Point(96, 232)
			Me.Button_EditProgrammer.Name = "Button_EditProgrammer"
			Me.PrefsHelp.SetShowHelp(Me.Button_EditProgrammer, true)
			Me.Button_EditProgrammer.Size = New System.Drawing.Size(88, 24)
			Me.Button_EditProgrammer.TabIndex = 2
			Me.Button_EditProgrammer.Text = "Edit..."
			AddHandler Me.Button_EditProgrammer.Click, AddressOf Me.Button_EditProgrammerClick
			'
			'Button_OK
			'
			Me.Button_OK.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.Button_OK.Location = New System.Drawing.Point(72, 304)
			Me.Button_OK.Name = "Button_OK"
			Me.Button_OK.Size = New System.Drawing.Size(80, 24)
			Me.Button_OK.TabIndex = 1
			Me.Button_OK.Text = "OK"
			AddHandler Me.Button_OK.Click, AddressOf Me.Button_OKClick
			'
			'buttonDeleteTool
			'
			Me.buttonDeleteTool.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.PrefsHelp.SetHelpString(Me.buttonDeleteTool, "Delete an external tool")
			Me.buttonDeleteTool.Location = New System.Drawing.Point(184, 232)
			Me.buttonDeleteTool.Name = "buttonDeleteTool"
			Me.PrefsHelp.SetShowHelp(Me.buttonDeleteTool, true)
			Me.buttonDeleteTool.Size = New System.Drawing.Size(88, 24)
			Me.buttonDeleteTool.TabIndex = 3
			Me.buttonDeleteTool.Text = "Delete"
			AddHandler Me.buttonDeleteTool.Click, AddressOf Me.ButtonDeleteToolClick
			'
			'EditorPrefs
			'
			Me.EditorPrefs.Controls.Add(Me.EditorWarnRecursion)
			Me.EditorPrefs.Location = New System.Drawing.Point(4, 22)
			Me.EditorPrefs.Name = "EditorPrefs"
			Me.EditorPrefs.Size = New System.Drawing.Size(280, 262)
			Me.EditorPrefs.TabIndex = 0
			Me.EditorPrefs.Text = "Editor"
			'
			'EditorWarnRecursion
			'
			Me.EditorWarnRecursion.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.PrefsHelp.SetHelpString(Me.EditorWarnRecursion, "Warn whenever a subroutine is called from itself.")
			Me.EditorWarnRecursion.Location = New System.Drawing.Point(16, 16)
			Me.EditorWarnRecursion.Name = "EditorWarnRecursion"
			Me.PrefsHelp.SetShowHelp(Me.EditorWarnRecursion, true)
			Me.EditorWarnRecursion.Size = New System.Drawing.Size(192, 16)
			Me.EditorWarnRecursion.TabIndex = 0
			Me.EditorWarnRecursion.Text = "Subroutine Recursion Warning"
			'
			'CompilerVerbose
			'
			Me.CompilerVerbose.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.PrefsHelp.SetHelpString(Me.CompilerVerbose, "Makes compiler display detailed information about the compilation process.")
			Me.CompilerVerbose.Location = New System.Drawing.Point(16, 16)
			Me.CompilerVerbose.Name = "CompilerVerbose"
			Me.PrefsHelp.SetShowHelp(Me.CompilerVerbose, true)
			Me.CompilerVerbose.Size = New System.Drawing.Size(160, 16)
			Me.CompilerVerbose.TabIndex = 0
			Me.CompilerVerbose.Text = "Verbose Mode"
			'
			'ProgrammerList
			'
			Me.PrefsHelp.SetHelpString(Me.ProgrammerList, "Shows the programmers that have been set up to work with Great Cow Graphical BASI"& _ 
"C")
			Me.ProgrammerList.Location = New System.Drawing.Point(8, 8)
			Me.ProgrammerList.Name = "ProgrammerList"
			Me.PrefsHelp.SetShowHelp(Me.ProgrammerList, true)
			Me.ProgrammerList.Size = New System.Drawing.Size(264, 212)
			Me.ProgrammerList.TabIndex = 0
			'
			'Button_DeleteProgrammer
			'
			Me.Button_DeleteProgrammer.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.PrefsHelp.SetHelpString(Me.Button_DeleteProgrammer, "Delete a programmer")
			Me.Button_DeleteProgrammer.Location = New System.Drawing.Point(184, 232)
			Me.Button_DeleteProgrammer.Name = "Button_DeleteProgrammer"
			Me.PrefsHelp.SetShowHelp(Me.Button_DeleteProgrammer, true)
			Me.Button_DeleteProgrammer.Size = New System.Drawing.Size(88, 24)
			Me.Button_DeleteProgrammer.TabIndex = 3
			Me.Button_DeleteProgrammer.Text = "Delete"
			AddHandler Me.Button_DeleteProgrammer.Click, AddressOf Me.Button_DeleteProgrammerClick
			'
			'Preferences
			'
			Me.AcceptButton = Me.Button_OK
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.CancelButton = Me.Button_Cancel
			Me.ClientSize = New System.Drawing.Size(306, 335)
			Me.Controls.Add(Me.Button_Cancel)
			Me.Controls.Add(Me.Button_OK)
			Me.Controls.Add(Me.PrefsTabs)
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
			Me.HelpButton = true
			Me.MaximizeBox = false
			Me.MinimizeBox = false
			Me.Name = "Preferences"
			Me.PrefsHelp.SetShowHelp(Me, false)
			Me.ShowInTaskbar = false
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
			Me.Text = "Preferences"
			Me.ExtToolsPrefs.ResumeLayout(false)
			Me.CompilerPrefs.ResumeLayout(false)
			Me.PrefsTabs.ResumeLayout(false)
			Me.ProgrammerPrefs.ResumeLayout(false)
			Me.EditorPrefs.ResumeLayout(false)
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
		Public Shared Function PrefIsYes(PrefValue As String) As Boolean
			PrefValue = PrefValue.ToLower
			If PrefValue.StartsWith("y") Or PrefValue.StartsWith("t") Or PrefValue = "1" Then
				Return True
			Else
				Return False
			End If
		End Function
		
		Shared Public Sub LoadPreferences
	   		Dim TempData As String = ""
	   		Dim CurrentTag, MsgName, MsgValue As String
	   		Dim CurrentSection As SettingSection
	   		
	   		Dim PrefFile(20) As String
	   		Dim PrefFiles As Integer
	   		Dim CurrFile, CheckFile As Integer
	   		Dim FileNotFound As Boolean
	   		
	   		MainForm.PrefFile = MainForm.InstallDir + Mainform.FilenamePrefs
	   		PrefFile(1) = MainForm.PrefFile
	   		PrefFiles = 1
	   		
	   		CurrFile = 0
	   		Do
	   			CurrFile += 1
		   		Try
		   			Dim ReadFile As New StreamReader(PrefFile(CurrFile))
		   			
		   			Do While ReadFile.Peek() <> -1
			     		TempData = ReadFile.ReadLine().Replace(Microsoft.VisualBasic.Chr(9), " ").Trim
			     		
			     		If TempData.Length > 0 And TempData.IndexOf("'") <> 0 Then
			     			
			     			If TempData.StartsWith("[") And TempData.EndsWith("]") Then
			     				CurrentTag = TempData.Substring(1, TempData.Length - 2).Trim.ToLower
			     				'Check if section exists
			     				Dim FoundSection As Boolean = False
			     				For Each CurrentSection In MainForm.PrefGroups
			     					If CurrentSection.Name = CurrentTag Then
			     						FoundSection = True
			     						Exit For
			     					End If
			     				Next
			     				'If not found, add it
			     				If Not FoundSection Then
			     					CurrentSection = New SettingSection(CurrentTag)
			     					MainForm.PrefGroups.Add(CurrentSection)
			     				End If
			     				
			     			Else If TempData.IndexOf("=") <> -1 Then
			     				MsgName = TempData.Substring(0, TempData.IndexOf("=")).Trim.ToLower
			     				MsgValue = TempData.Substring(TempData.IndexOf("=") + 1).Trim
			     				
			     				CurrentSection.AddSetting(MsgName, MsgValue)
			     				
			     			Else If TempData.ToLower.StartsWith("include ") Then
			     				MsgValue = LowLevel.ReplaceToolVariables(TempData.Substring(TempData.IndexOf(" ") + 1).Trim)
			     				FileNotFound = True
			     				For CheckFile = 1 To PrefFiles
			     					If MsgValue.ToLower = PrefFile(CheckFile).ToLower Then
			     						FileNotFound = False
			     						Exit For
			     					End If
			     				Next
			     				
			     				If FileNotFound And File.Exists(MsgValue) Then
			     					PrefFiles += 1
			     					PrefFile(PrefFiles) = MsgValue
			     				End If
			     			End If
			     			
		     			End If	     		
		     		Loop
		     		
		     		ReadFile.Close()
		     		
		     	Catch
		   		End Try
		    Loop While CurrFile < PrefFiles
		    
	    End Sub
		
		Public Shared Sub SavePreferences
			Dim Temp As Integer = 0
			Dim ProgFilesIni, UserIni As StreamWriter
			Dim MultipleFiles As Boolean
			
			'Save in Application Data?
			If PrefIsYes(GetPref("GCGB", "UseAppdata")) Then
				'Open Files
				'User/Application Data
				UserIni = New StreamWriter(LowLevel.ReplaceToolVariables(MainForm.FileNameUserPrefs))
				'Write header
	    		UserIni.WriteLine("'Preferences file for Great Cow Graphical BASIC")
	    		UserIni.WriteLine("'Location: [user]/Application Data")
	    		UserIni.WriteLine("")
				
			Else
				'Open File
	    		UserIni = New StreamWriter(MainForm.PrefFile)
	    		'Write header
	    		UserIni.WriteLine("'Preferences file for Great Cow Graphical BASIC")
	    		UserIni.WriteLine("'Location: GCGB install dir")
	    		UserIni.WriteLine("")
	    	End If
	    	
	    	'Data
	    	Dim section As SettingSection
			For Each section In MainForm.PrefGroups
				UserIni.WriteLine("[" + section.Name + "]")
					
				Dim s As Setting
				For Each s In section.Settings
					UserIni.WriteLine(s.Name + " = " + s.Value)
				Next
					
				UserIni.WriteLine()
			Next
	    	
	    	'Close
	    	UserIni.Close()
	    	
		End Sub
				
		Public Shared Function GetPref(ByVal SectionName As String, ByVal PrefName As String) As String
			Dim Temp As Integer = 0
			Dim PrefLoc As Integer = 0
			
			SectionName = SectionName.Trim.ToLower
			PrefName = PrefName.Trim.ToLower
			
			If MainForm.PrefGroups Is Nothing Then
				Return ""
			End If
			
			Dim section As SettingSection
			For Each section In MainForm.PrefGroups
				If section.Name.ToLower = SectionName Then
					
					Dim s As Setting
					For Each s In section.Settings
						If s.Name = PrefName Then
							Return s.Value
						End If
					Next
					
				End If
			Next
			
			Return ""
			
		End Function
		
		Public Shared Sub SetPref(ByVal SectionName As String, ByVal PrefName As String, ByVal PrefValue As String)
			Dim Temp As Integer = 0
			Dim PrefLoc As Integer = 0
			
			Dim section As SettingSection
			For Each section In MainForm.PrefGroups
				If section.Name.ToLower = SectionName.ToLower Then
					
					Dim s As Setting
					For Each s In section.Settings
						If s.Name.ToLower = PrefName.ToLower Then
							s.Value = PrefValue
							Return
						End If
					Next
					
					section.AddSetting(PrefName, PrefValue)
					Return
				End If
			Next
			
			section = New SettingSection(SectionName)
			section.AddSetting(PrefName, PrefValue)
			MainForm.PrefGroups.Add(section)
		End Sub
		
		Private Sub Button_OKClick(sender As System.Object, e As System.EventArgs)
			'Editor
			If Me.EditorWarnRecursion.Checked = False Then
				SetPref("GCGB", "WarnRecursion", "0")
			Else
				SetPref("GCGB", "WarnRecursion", "1")
			End If
			
			'Compiler
			If Me.CompilerVerbose.Checked = False Then
				SetPref("GCBASIC", "Verbose", "n")
			Else
				SetPref("GCBASIC", "Verbose", "y")
			End If
			If Me.CompilerShowBASIC.Checked = False Then
				SetPref("GCBASIC", "Preserve", "n")
			Else
				SetPref("GCBASIC", "Preserve", "a")
			End If
			If CompilerPause.Checked = False Then
				SetPref("GCBASIC", "PauseAfterCompile", "n")
			Else
				SetPref("GCBASIC", "PauseAfterCompile", "y")
			End If
			
			'Programmer
			If ProgrammerList.SelectedIndex <> -1 Then
				Dim ProgrammerID As String
				Dim ProgrammerSection As SettingSection
				ProgrammerSection = GetToolSettings(ProgrammerList.SelectedItem)
				ProgrammerID = ProgrammerSection.Name
				ProgrammerID = ProgrammerID.Substring(ProgrammerID.IndexOf("=") + 1).Trim()
				SetPref("GCBASIC", "Programmer", ProgrammerID)
			End If
			MainForm.MainFormInstance.UpdateProgrammerList
			
			'Save and Exit
			SavePreferences
			Me.Close
			
		End Sub
		
		Private Function GetProgrammerID(ByVal BaseName As String) As String
			Dim ToolName, OutName As String
			Dim Temp, Pos As Integer
			
			'Get a possible name from the filename, stripping off the path and extension
			For Pos = BaseName.Length - 1 To 0 Step -1
				If "\/".IndexOf(BaseName.Substring(Pos, 1)) <> -1 Then
					BaseName = BaseName.Substring(Pos + 1)
					Exit For
				Else If BaseName.Substring(Pos, 1) = "." Then
					BaseName = BaseName.Substring(0, Pos)
				End If
			Next
			BaseName = BaseName.Trim.ToLower.Replace(" ", "_")
			
			'Check for existing programmer with name
			Dim UniqueID As Integer = 0
			OutName = BaseName
			Dim section As SettingSection
			StartToolSearchAgain:
			For Each section In MainForm.PrefGroups
				If Section.name.ToLower.StartsWith("tool") And Section.Name.IndexOf("=") <> -1 Then
					ToolName = Section.Name.Substring(Section.Name.IndexOf("=") + 1).Trim()
					If ToolName = OutName Then
						OutName = BaseName + UniqueID
						UniqueID += 1
						Goto StartToolSearchAgain
					End If
				End If
			Next
			
			Return OutName
		End Function
		
		'Accepts the name (aka description, not ID) of a toop and finds the setting section for it
		Public Shared Function GetToolSettings(ToolName As String) As SettingSection
			Dim section As SettingSection
			
			For Each section In MainForm.PrefGroups
				If Section.name.ToLower.StartsWith("tool") And Section.Name.IndexOf("=") <> -1 Then
					if section.GetValue("desc").ToLower = ToolName.ToLower Then Return section
				End If
			Next
			
			Return Nothing
		End Function
		
		Private Sub Button_AddProgrammerClick(sender As System.Object, e As System.EventArgs)
			Dim progEdit As ProgrammerEditor = New ProgrammerEditor
			
			progEdit.ShowDialog
			Do While ProgEdit.WasAccepted
				'Create new settings group
				If GetToolSettings(ProgEdit.GetProgName) is Nothing Then
					
					If ProgEdit.GetProgFile = "" Or ProgEdit.GetProgName = "" Then
						MessageBox.Show("A name and filename must be entered for the programmer.", "Great Cow Graphical BASIC", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
						ProgEdit.Redisplay
					Else
						Dim ProgID As String
						ProgID = GetProgrammerID(progEdit.GetProgFile)
						
						Dim section As SettingSection = New SettingSection("tool = " + ProgID)
						With section
							.AddSetting("desc", ProgEdit.GetProgName)
							.AddSetting("type", "programmer")
							.AddSetting("command", ProgEdit.GetProgFile)
							.AddSetting("params", ProgEdit.GetProgParams)
							.AddSetting("workingdir", ProgEdit.GetProgDir)
						End With
						MainForm.PrefGroups.Add(section)
						ProgrammerList.Items.Add(ProgEdit.GetProgName)
						
						Exit Sub
					End If
				Else
					MessageBox.Show("A tool already exists with the same name. Please choose a new name.", "Great Cow Graphical BASIC", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
					ProgEdit.Redisplay
				End If
				
			Loop
			
		End Sub
		
		Private Sub Button_EditProgrammerClick(sender As System.Object, e As System.EventArgs)
			'Get Existing
			If ProgrammerList.SelectedIndex = -1 Then Exit Sub
			Dim section As SettingSection = GetToolSettings(ProgrammerList.SelectedItem.ToString)
			Dim ProgEdit As ProgrammerEditor
			ProgEdit = New ProgrammerEditor(section.GetValue("desc"), _
			                                section.GetValue("command"), _
			                                section.GetValue("params"), _
			                                section.GetValue("workingdir"))
			
			progEdit.ShowDialog
			
			Dim NameChanged As Boolean
			
			Do While ProgEdit.WasAccepted
				'Check if the name has been changed
				NameChanged = ProgEdit.GetProgName.ToLower.Trim <> section.GetValue("desc").Trim.ToLower
				'Proceed if tool not found because name changed, or name not changed
				If GetToolSettings(ProgEdit.GetProgName) is Nothing Or Not NameChanged Then
					
					If ProgEdit.GetProgFile = "" Or ProgEdit.GetProgName = "" Then
						MessageBox.Show("A name and filename must be entered for the programmer.", "Great Cow Graphical BASIC", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
						ProgEdit.Redisplay
					Else
						If NameChanged Then
							Dim ListLoc As Integer
							For ListLoc = 0 To ProgrammerList.Items.Count
								If ProgrammerList.Items.Item(ListLoc) = section.GetValue("desc") Then
									ProgrammerList.Items.Item(ListLoc) = ProgEdit.GetProgName
									Exit For
								End If
							Next
						End If
						With section
							.AddSetting("desc", ProgEdit.GetProgName)
							.AddSetting("command", ProgEdit.GetProgFile)
							.AddSetting("params", ProgEdit.GetProgParams)
							.AddSetting("workingdir", ProgEdit.GetProgDir)
						End With
						
						Exit Sub
					End If
				Else
					MessageBox.Show("A different tool already exists with the same name. Please choose a new name.", "Great Cow Graphical BASIC", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
					ProgEdit.Redisplay
				End If
				
			Loop
			
		End Sub
		
		Private Sub Button_DeleteProgrammerClick(sender As System.Object, e As System.EventArgs)
			If ProgrammerList.SelectedIndex = -1 Then Exit Sub
			
			Dim result As DialogResult = MessageBox.Show("Are you sure that you want to delete the programmer?", "Delete " + programmerList.SelectedItem, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
			Select result
			Case DialogResult.OK
				'Get Existing
				Dim section As SettingSection = GetToolSettings(ProgrammerList.SelectedItem.ToString)
				
				'Delete from list
				Dim ListLoc As Integer
				For ListLoc = 0 To ProgrammerList.Items.Count
					If ProgrammerList.Items.Item(ListLoc) = section.GetValue("desc") Then
						ProgrammerList.Items.RemoveAt(ListLoc)
						Exit For
					End If
				Next
				
				'Delete tool from settings
				MainForm.PrefGroups.Remove(section)
				
			Case DialogResult.Cancel
				
			End Select
			
		End Sub
		
		Private Sub ButtonAddToolClick(sender As System.Object, e As System.EventArgs)
			Dim progEdit As ProgrammerEditor = New ProgrammerEditor
			progEdit.Text = "External Tool Editor"
			
			progEdit.ShowDialog
			Do While ProgEdit.WasAccepted
				'Create new settings group
				If GetToolSettings(ProgEdit.GetProgName) is Nothing Then
					
					If ProgEdit.GetProgFile = "" Or ProgEdit.GetProgName = "" Then
						MessageBox.Show("A name and filename must be entered for the external tool.", "Great Cow Graphical BASIC", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
						ProgEdit.Redisplay
					Else
						Dim ProgID As String
						ProgID = GetProgrammerID(progEdit.GetProgFile)
						
						Dim section As SettingSection = New SettingSection("tool = " + ProgID)
						With section
							.AddSetting("desc", ProgEdit.GetProgName)
							.AddSetting("type", "external")
							.AddSetting("command", ProgEdit.GetProgFile)
							.AddSetting("params", ProgEdit.GetProgParams)
							.AddSetting("workingdir", ProgEdit.GetProgDir)
						End With
						MainForm.PrefGroups.Add(section)
						toolList.Items.Add(ProgEdit.GetProgName)
						
						Exit Sub
					End If
				Else
					MessageBox.Show("A tool already exists with the same name. Please choose a new name.", "Great Cow Graphical BASIC", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
					ProgEdit.Redisplay
				End If
				
			Loop
			
		End Sub
		
		Private Sub ButtonEditToolClick(sender As System.Object, e As System.EventArgs)
			'Get Existing
			If toolList.SelectedIndex = -1 Then Exit Sub
			Dim section As SettingSection = GetToolSettings(toolList.SelectedItem.ToString)
			Dim ProgEdit As ProgrammerEditor
			ProgEdit = New ProgrammerEditor(section.GetValue("desc"), _
			                                section.GetValue("command"), _
			                                section.GetValue("params"), _
			                                section.GetValue("workingdir"))
			
			progEdit.Text = "External Tool Editor"
			progEdit.ShowDialog
			
			Dim NameChanged As Boolean
			
			Do While ProgEdit.WasAccepted
				'Check if the name has been changed
				NameChanged = ProgEdit.GetProgName.ToLower.Trim <> section.GetValue("desc").Trim.ToLower
				'Proceed if tool not found because name changed, or name not changed
				If GetToolSettings(ProgEdit.GetProgName) is Nothing Or Not NameChanged Then
					
					If ProgEdit.GetProgFile = "" Or ProgEdit.GetProgName = "" Then
						MessageBox.Show("A name and filename must be entered for the external tool.", "Great Cow Graphical BASIC", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
						ProgEdit.Redisplay
					Else
						If NameChanged Then
							Dim ListLoc As Integer
							For ListLoc = 0 To toolList.Items.Count
								If toolList.Items.Item(ListLoc) = section.GetValue("desc") Then
									toolList.Items.Item(ListLoc) = ProgEdit.GetProgName
									Exit For
								End If
							Next
						End If
						With section
							.AddSetting("desc", ProgEdit.GetProgName)
							.AddSetting("command", ProgEdit.GetProgFile)
							.AddSetting("params", ProgEdit.GetProgParams)
							.AddSetting("workingdir", ProgEdit.GetProgDir)
						End With
						
						Exit Sub
					End If
				Else
					MessageBox.Show("A different tool already exists with the same name. Please choose a new name.", "Great Cow Graphical BASIC", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
					ProgEdit.Redisplay
				End If
				
			Loop
			
		End Sub
		
		Private Sub ButtonDeleteToolClick(sender As System.Object, e As System.EventArgs)
			If toolList.SelectedIndex = -1 Then Exit Sub
			
			Dim result As DialogResult = MessageBox.Show("Are you sure that you want to delete the external tool?", "Delete " + toolList.SelectedItem, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
			Select result
			Case DialogResult.OK
				'Get Existing
				Dim section As SettingSection = GetToolSettings(toolList.SelectedItem.ToString)
				
				'Delete from list
				Dim ListLoc As Integer
				For ListLoc = 0 To toolList.Items.Count
					If toolList.Items.Item(ListLoc) = section.GetValue("desc") Then
						toolList.Items.RemoveAt(ListLoc)
						Exit For
					End If
				Next
				
				'Delete tool from settings
				MainForm.PrefGroups.Remove(section)
				
			Case DialogResult.Cancel
				
			End Select
			
		End Sub
		
	End Class
End Namespace
