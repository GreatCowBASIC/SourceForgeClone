'
' Created by SharpDevelop.
' User: Administrator
' Date: 1/04/2007
' Time: 11:36 PM
'
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Collections.Generic

'Namespace Great_Cow_Graphical_BASIC
	
	Public Class PreferencesWindow
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
		
		Private Dim HeadingFont As Font
		Private Dim ItemFont As Font
		Private Dim pPreferences As Preferences
		Private Dim GCGBOptionsHidden as Boolean = False
		
		Public Sub New()
			MyBase.New
			'
			' The Me.InitializeComponent call is required for Windows Forms designer support.
			'
			Me.InitializeComponent
			
		End Sub
		
		Public Property Preferences As Preferences
			Get
				Return pPreferences
			End Get
			Set
				pPreferences = Value
				
				'Editor
				EditorWarnRecursion.Checked = False
				If Preferences.PrefIsYes(pPreferences.GetPref("GCGB", "WarnRecursion")) Then EditorWarnRecursion.Checked = True
				If pPreferences.GetPref("GCGB", "IndentSize") <> "" Then
					EditorIndentSize.Value = pPreferences.GetPref("GCGB", "IndentSize")
				End If
				
				'Compiler
				CompilerVerbose.Checked = Preferences.PrefIsYes(pPreferences.GetPref("GCBASIC", "Verbose"))
				CompilerShowBASIC.Checked = False
				If pPreferences.GetPref("GCBASIC", "Preserve") = "a" Then CompilerShowBASIC.Checked = True
				CompilerWarningsAsErrors.Checked = Preferences.PrefIsYes(pPreferences.GetPref("GCBASIC", "WarningsAsErrors"))
				CompilerPause.Checked = Preferences.PrefIsYes(pPreferences.GetPref("GCBASIC", "PauseAfterCompile"))
				
				'Programmer list
				'Restricted mode means that GCGB is in a school or other environment where the users may be up to no good
				'If restricted, don't allow them to define programmers.
				If Preferences.PrefIsYes(pPreferences.GetPref("GCGB", "Restricted")) Then
					Me.PrefsTabs.Controls.Remove(ProgrammerPrefs)
				Else
					'Find all tools
					Dim section As SettingSection
					Dim ProgName, ProgID As String
					'Get preferred programmers
					Dim PreferredProgrammers As New List(Of String)
					If pPreferences.GetPref("GCBASIC", "Programmer") <> "" Then
						PreferredProgrammers.AddRange(pPreferences.GetPref("GCBASIC", "Programmer").ToLower.Replace(" ", "").Split(","))
					End If
					
					'List those that are preferred
					ProgrammerList.Items.Add("<s>Programmers to use (in order)")
					For Each ProgID In PreferredProgrammers
						For Each section In pPreferences.PrefGroups
							If section.Name.ToLower.StartsWith("tool") Then
								If section.GetValue("type") = "programmer" Then
									ProgName = section.Name
									ProgName = ProgName.Substring(ProgName.IndexOf("=") + 1).Trim
									If ProgName.ToLower = ProgID Then
										ProgrammerList.Items.Add(section.GetValue("desc"))
									End If
								End If
							End If
						Next
					Next
					
					'List others
					ProgrammerList.Items.Add("<s>Other available programmers")
					For Each section In pPreferences.PrefGroups
						If section.Name.ToLower.StartsWith("tool") Then
							If section.GetValue("type") = "programmer" Then
								ProgName = section.Name
								ProgName = ProgName.Substring(ProgName.IndexOf("=") + 1).Trim
								If Not PreferredProgrammers.Contains(ProgName.ToLower) Then
									ProgrammerList.Items.Add(section.GetValue("desc"))
								End If
							End If
						End If
					Next
				End If
				
				'External tools list
				'Restricted mode means that GCGB is in a school or other environment where the users may be up to no good
				'If restricted, don't allow them to define programmers.
				If Preferences.PrefIsYes(pPreferences.GetPref("GCGB", "Restricted")) Then
					Me.PrefsTabs.Controls.Remove(ExtToolsPrefs)
				Else
					'Find all tools
					Dim section As SettingSection
					For Each section In pPreferences.PrefGroups
						If section.Name.ToLower.StartsWith("tool") Then
							If section.GetValue("type") = "external" Then
								toolList.Items.Add(section.GetValue("desc"))
							End If
						End If
					Next
				End If
			End Set
		End Property
		
		#Region " Windows Forms Designer generated code "
		' This method is required for Windows Forms designer support.
		' Do not change the method contents inside the source code editor. The Forms designer might
		' not be able to load this method if it was changed manually.
		Private Sub InitializeComponent()
			Me.ExtToolsPrefs = New System.Windows.Forms.TabPage
			Me.buttonDeleteTool = New System.Windows.Forms.Button
			Me.buttonEditTool = New System.Windows.Forms.Button
			Me.buttonAddTool = New System.Windows.Forms.Button
			Me.toolList = New System.Windows.Forms.ListBox
			Me.CompilerPrefs = New System.Windows.Forms.TabPage
			Me.CompilerPause = New System.Windows.Forms.CheckBox
			Me.CompilerShowBASIC = New System.Windows.Forms.CheckBox
			Me.CompilerVerbose = New System.Windows.Forms.CheckBox
			Me.PrefsTabs = New System.Windows.Forms.TabControl
			Me.EditorPrefs = New System.Windows.Forms.TabPage
			Me.EditorIndentSize = New System.Windows.Forms.NumericUpDown
			Me.EditorIndentLabel = New System.Windows.Forms.Label
			Me.EditorWarnRecursion = New System.Windows.Forms.CheckBox
			Me.ProgrammerPrefs = New System.Windows.Forms.TabPage
			Me.Button_DeleteProgrammer = New System.Windows.Forms.Button
			Me.Button_EditProgrammer = New System.Windows.Forms.Button
			Me.Button_AddProgrammer = New System.Windows.Forms.Button
			Me.ProgrammerList = New System.Windows.Forms.ListBox
			Me.Button_Cancel = New System.Windows.Forms.Button
			Me.PrefsHelp = New System.Windows.Forms.HelpProvider
			Me.Button_OK = New System.Windows.Forms.Button
			Me.CompilerWarningsAsErrors = New System.Windows.Forms.CheckBox
			Me.ExtToolsPrefs.SuspendLayout
			Me.CompilerPrefs.SuspendLayout
			Me.PrefsTabs.SuspendLayout
			Me.EditorPrefs.SuspendLayout
			CType(Me.EditorIndentSize,System.ComponentModel.ISupportInitialize).BeginInit
			Me.ProgrammerPrefs.SuspendLayout
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
			'buttonDeleteTool
			'
			Me.buttonDeleteTool.Enabled = false
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
			'buttonEditTool
			'
			Me.buttonEditTool.Enabled = false
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
			'toolList
			'
			Me.PrefsHelp.SetHelpString(Me.toolList, "Shows the external tools that have been set up to work with Great Cow Graphical B"& _
						"ASIC")
			Me.toolList.Location = New System.Drawing.Point(8, 8)
			Me.toolList.Name = "toolList"
			Me.PrefsHelp.SetShowHelp(Me.toolList, true)
			Me.toolList.Size = New System.Drawing.Size(264, 212)
			Me.toolList.TabIndex = 0
			AddHandler Me.toolList.SelectedIndexChanged, AddressOf Me.ToolListSelectedIndexChanged
			'
			'CompilerPrefs
			'
			Me.CompilerPrefs.Controls.Add(Me.CompilerWarningsAsErrors)
			Me.CompilerPrefs.Controls.Add(Me.CompilerPause)
			Me.CompilerPrefs.Controls.Add(Me.CompilerShowBASIC)
			Me.CompilerPrefs.Controls.Add(Me.CompilerVerbose)
			Me.CompilerPrefs.Location = New System.Drawing.Point(4, 22)
			Me.CompilerPrefs.Name = "CompilerPrefs"
			Me.CompilerPrefs.Size = New System.Drawing.Size(280, 262)
			Me.CompilerPrefs.TabIndex = 1
			Me.CompilerPrefs.Text = "Compiler"
			'
			'CompilerPause
			'
			Me.CompilerPause.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.PrefsHelp.SetHelpString(Me.CompilerPause, "Keeps the compiler window open until a key is pressed")
			Me.CompilerPause.Location = New System.Drawing.Point(16, 88)
			Me.CompilerPause.Name = "CompilerPause"
			Me.PrefsHelp.SetShowHelp(Me.CompilerPause, true)
			Me.CompilerPause.Size = New System.Drawing.Size(168, 16)
			Me.CompilerPause.TabIndex = 2
			Me.CompilerPause.Text = "Pause after compilation"
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
			'EditorPrefs
			'
			Me.EditorPrefs.Controls.Add(Me.EditorIndentSize)
			Me.EditorPrefs.Controls.Add(Me.EditorIndentLabel)
			Me.EditorPrefs.Controls.Add(Me.EditorWarnRecursion)
			Me.EditorPrefs.Location = New System.Drawing.Point(4, 22)
			Me.EditorPrefs.Name = "EditorPrefs"
			Me.EditorPrefs.Size = New System.Drawing.Size(280, 262)
			Me.EditorPrefs.TabIndex = 0
			Me.EditorPrefs.Text = "Editor"
			'
			'EditorIndentSize
			'
			Me.PrefsHelp.SetHelpString(Me.EditorIndentSize, "Amount to indent nested icons (like contents of If icon)")
			Me.EditorIndentSize.Location = New System.Drawing.Point(80, 48)
			Me.EditorIndentSize.Maximum = New Decimal(New Integer() {200, 0, 0, 0})
			Me.EditorIndentSize.Name = "EditorIndentSize"
			Me.PrefsHelp.SetShowHelp(Me.EditorIndentSize, true)
			Me.EditorIndentSize.Size = New System.Drawing.Size(48, 20)
			Me.EditorIndentSize.TabIndex = 2
			Me.EditorIndentSize.Value = New Decimal(New Integer() {20, 0, 0, 0})
			'
			'EditorIndentLabel
			'
			Me.PrefsHelp.SetHelpString(Me.EditorIndentLabel, "Amount to indent nested icons (like contents of If icon)")
			Me.EditorIndentLabel.Location = New System.Drawing.Point(8, 48)
			Me.EditorIndentLabel.Name = "EditorIndentLabel"
			Me.PrefsHelp.SetShowHelp(Me.EditorIndentLabel, true)
			Me.EditorIndentLabel.Size = New System.Drawing.Size(72, 16)
			Me.EditorIndentLabel.TabIndex = 1
			Me.EditorIndentLabel.Text = "Indent Size"
			Me.EditorIndentLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
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
			'Button_DeleteProgrammer
			'
			Me.Button_DeleteProgrammer.Enabled = false
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
			'Button_EditProgrammer
			'
			Me.Button_EditProgrammer.Enabled = false
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
			'ProgrammerList
			'
			Me.ProgrammerList.AllowDrop = true
			Me.ProgrammerList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
			Me.PrefsHelp.SetHelpString(Me.ProgrammerList, "Shows the programmers that have been set up to work with Great Cow Graphical BASI"& _
						"C")
			Me.ProgrammerList.Location = New System.Drawing.Point(8, 8)
			Me.ProgrammerList.Name = "ProgrammerList"
			Me.PrefsHelp.SetShowHelp(Me.ProgrammerList, true)
			Me.ProgrammerList.Size = New System.Drawing.Size(264, 212)
			Me.ProgrammerList.TabIndex = 0
			AddHandler Me.ProgrammerList.DrawItem, AddressOf Me.ProgrammerListDrawItem
			AddHandler Me.ProgrammerList.DragOver, AddressOf Me.ProgrammerListDragOver
			AddHandler Me.ProgrammerList.DragDrop, AddressOf Me.ProgrammerListDragDrop
			AddHandler Me.ProgrammerList.MouseDown, AddressOf Me.ProgrammerListMouseDown
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
			'CompilerWarningsAsErrors
			'
			Me.CompilerWarningsAsErrors.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.PrefsHelp.SetHelpString(Me.CompilerWarningsAsErrors, "Copies the original BASIC program into the assembly file produced by the compiler"& _
						". Useful for showing the link between icons and assembly commands.")
			Me.CompilerWarningsAsErrors.Location = New System.Drawing.Point(16, 64)
			Me.CompilerWarningsAsErrors.Name = "CompilerWarningsAsErrors"
			Me.PrefsHelp.SetShowHelp(Me.CompilerWarningsAsErrors, true)
			Me.CompilerWarningsAsErrors.Size = New System.Drawing.Size(224, 16)
			Me.CompilerWarningsAsErrors.TabIndex = 3
			Me.CompilerWarningsAsErrors.Text = "Treat warnings as errors"
			'
			'PreferencesWindow
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
			Me.Name = "PreferencesWindow"
			Me.PrefsHelp.SetShowHelp(Me, false)
			Me.ShowInTaskbar = false
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
			Me.Text = "Preferences"
			Me.ExtToolsPrefs.ResumeLayout(false)
			Me.CompilerPrefs.ResumeLayout(false)
			Me.PrefsTabs.ResumeLayout(false)
			Me.EditorPrefs.ResumeLayout(false)
			CType(Me.EditorIndentSize,System.ComponentModel.ISupportInitialize).EndInit
			Me.ProgrammerPrefs.ResumeLayout(false)
			Me.ResumeLayout(false)
		End Sub
		Private CompilerWarningsAsErrors As System.Windows.Forms.CheckBox
		Private EditorIndentLabel As System.Windows.Forms.Label
		Private EditorIndentSize As System.Windows.Forms.NumericUpDown
		#End Region
		
		Public Sub HideGCGBOptions
			Me.PrefsTabs.Controls.Remove(EditorPrefs)
			Me.PrefsTabs.Controls.Remove(ExtToolsPrefs)
			
			CompilerPause.Visible = False
			GCGBOptionsHidden = True
		End Sub
		
		Private Sub Button_OKClick(sender As System.Object, e As System.EventArgs)
			'Editor
			If Not GCGBOptionsHidden Then
				If Me.EditorWarnRecursion.Checked = False Then
					pPreferences.SetPref("GCGB", "WarnRecursion", "0")
				Else
					pPreferences.SetPref("GCGB", "WarnRecursion", "1")
				End If
				pPreferences.SetPref("GCGB", "IndentSize", Me.EditorIndentSize.Value.ToString)
			End If
			
			'Compiler
			If Me.CompilerVerbose.Checked = False Then
				pPreferences.SetPref("GCBASIC", "Verbose", "n")
			Else
				pPreferences.SetPref("GCBASIC", "Verbose", "y")
			End If
			If Me.CompilerShowBASIC.Checked = False Then
				pPreferences.SetPref("GCBASIC", "Preserve", "n")
			Else
				pPreferences.SetPref("GCBASIC", "Preserve", "a")
			End If
			If CompilerWarningsAsErrors.Checked Then
				pPreferences.SetPref("GCBASIC", "WarningsAsErrors", "y")
			Else
				pPreferences.SetPref("GCBASIC", "WarningsAsErrors", "n")
			End If
			If CompilerPause.Checked = False Or GCGBOptionsHidden Then
				pPreferences.SetPref("GCBASIC", "PauseAfterCompile", "n")
			Else
				pPreferences.SetPref("GCBASIC", "PauseAfterCompile", "y")
			End If
			
			'Programmer
			If Not Preferences.PrefIsYes(pPreferences.GetPref("GCGB", "Restricted"))
				Dim ProgrammerOutputList As String
				Dim FindProgrammer As String
				Dim ProgSectionNo As Integer = 0
				ProgrammerOutputList = ""
				For Each FindProgrammer In ProgrammerList.Items
					If FindProgrammer.StartsWith("<s>") Then
						ProgSectionNo += 1
					Else
						If ProgSectionNo = 1 Then
							Dim ProgrammerSection As SettingSection
							Dim ProgrammerID As String
							ProgrammerSection = pPreferences.GetToolSettings(FindProgrammer)
							ProgrammerID = ProgrammerSection.Name
							ProgrammerID = ProgrammerID.Substring(ProgrammerID.IndexOf("=") + 1).Trim()
							If ProgrammerOutputList = "" Then
								ProgrammerOutputList = ProgrammerID
							Else
								ProgrammerOutputList = ProgrammerOutputList + ", " + ProgrammerID
							End If
						End If
					End If
				Next
				pPreferences.SetPref("GCBASIC", "Programmer", ProgrammerOutputList)
				MainForm.MainFormInstance.UpdateProgrammerList
			End If
			
			'Save and Exit
			pPreferences.SavePreferences
			pPreferences.FireChanged
			Me.Close
			
		End Sub
		
		Private Sub Button_AddProgrammerClick(sender As System.Object, e As System.EventArgs)
			Dim progEdit As ProgrammerEditor = New ProgrammerEditor(pPreferences)
			
			progEdit.ShowDialog
			If ProgEdit.WasAccepted Then
				pPreferences.PrefGroups.Add(ProgEdit.Section)
				ProgrammerList.Items.Add(ProgEdit.ProgName)
			End If
			
		End Sub
		
		Private Sub Button_EditProgrammerClick(sender As System.Object, e As System.EventArgs)
			'Get Existing
			If ProgrammerList.SelectedIndex = -1 Then Exit Sub
			If ProgrammerList.SelectedItem.StartsWith("<s>") Then Exit Sub
			Dim section As SettingSection = pPreferences.GetToolSettings(ProgrammerList.SelectedItem.ToString)
			Dim ProgEdit As New ProgrammerEditor(pPreferences, section)
			
			progEdit.ShowDialog
			
			If ProgEdit.WasAccepted Then
				If ProgEdit.NameChanged Then
					Dim ListLoc As Integer
					For ListLoc = 0 To ProgrammerList.Items.Count
						If ProgrammerList.Items.Item(ListLoc) = section.GetValue("desc") Then
							ProgrammerList.Items.Item(ListLoc) = ProgEdit.ProgName
							Exit For
						End If
					Next
				End If
				
				section = ProgEdit.Section
			End If
			
		End Sub
		
		Private Sub Button_DeleteProgrammerClick(sender As System.Object, e As System.EventArgs)
			If ProgrammerList.SelectedIndex = -1 Then Exit Sub
			If ProgrammerList.SelectedItem.StartsWith("<s>") Then Exit Sub
			
			Dim result As DialogResult = MessageBox.Show("Are you sure that you want to delete the programmer?", "Delete " + programmerList.SelectedItem, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
			Select result
			Case DialogResult.OK
				'Get Existing
				Dim section As SettingSection = pPreferences.GetToolSettings(ProgrammerList.SelectedItem.ToString)
				
				'Delete from list
				Dim ListLoc As Integer
				For ListLoc = 0 To ProgrammerList.Items.Count
					If ProgrammerList.Items.Item(ListLoc) = section.GetValue("desc") Then
						ProgrammerList.Items.RemoveAt(ListLoc)
						Exit For
					End If
				Next
				
				'Delete tool from settings
				pPreferences.PrefGroups.Remove(section)
				
			Case DialogResult.Cancel
				
			End Select
			
		End Sub
		
		Private Sub ButtonAddToolClick(sender As System.Object, e As System.EventArgs)
			Dim progEdit As ProgrammerEditor = New ProgrammerEditor(pPreferences)
			progEdit.Mode = ProgrammerEditor.Modes.TOOL
			progEdit.ShowDialog
			
			If progEdit.WasAccepted Then
				pPreferences.PrefGroups.Add(progEdit.Section)
				toolList.Items.Add(ProgEdit.ProgName)
			End If
			
		End Sub
		
		Private Sub ButtonEditToolClick(sender As System.Object, e As System.EventArgs)
			'Get Existing
			If toolList.SelectedIndex = -1 Then Exit Sub
			Dim section As SettingSection = pPreferences.GetToolSettings(toolList.SelectedItem.ToString)
			Dim ProgEdit As New ProgrammerEditor(pPreferences, section)
			
			progEdit.Mode = ProgrammerEditor.Modes.TOOL
			progEdit.ShowDialog
			
			If ProgEdit.WasAccepted Then
				If ProgEdit.NameChanged Then
					Dim ListLoc As Integer
					For ListLoc = 0 To toolList.Items.Count
						If toolList.Items.Item(ListLoc) = section.GetValue("desc") Then
							toolList.Items.Item(ListLoc) = ProgEdit.ProgName
							Exit For
						End If
					Next
				End If
				
				section = ProgEdit.Section
			End If
			
		End Sub
		
		Private Sub ButtonDeleteToolClick(sender As System.Object, e As System.EventArgs)
			If toolList.SelectedIndex = -1 Then Exit Sub
			
			Dim result As DialogResult = MessageBox.Show("Are you sure that you want to delete the external tool?", "Delete " + toolList.SelectedItem, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
			Select result
			Case DialogResult.OK
				'Get Existing
				Dim section As SettingSection = pPreferences.GetToolSettings(toolList.SelectedItem.ToString)
				
				'Delete from list
				Dim ListLoc As Integer
				For ListLoc = 0 To toolList.Items.Count
					If toolList.Items.Item(ListLoc) = section.GetValue("desc") Then
						toolList.Items.RemoveAt(ListLoc)
						Exit For
					End If
				Next
				
				'Delete tool from settings
				pPreferences.PrefGroups.Remove(section)
				
			Case DialogResult.Cancel
				
			End Select
			
		End Sub
		
		Sub ProgrammerListDrawItem(sender As Object, e As DrawItemEventArgs)
			
			If ItemFont Is Nothing Then
				ItemFont = ProgrammerList.Font
			End If
			If HeadingFont Is Nothing Then
				HeadingFont = New Font(ProgrammerList.Font, FontStyle.Bold)
			End If
			
			Dim DisplayName As String
			Dim DisplayFont As Font
			Dim DisplayBrush As Brush
			DisplayName = ProgrammerList.Items(e.Index).ToString()
			If DisplayName.StartsWith("<s>") Then
				DisplayName = DisplayName.Substring(3)
				DisplayFont = HeadingFont
			Else
				DisplayFont = ItemFont
			End If
			DisplayBrush = New SolidBrush(e.ForeColor)
			
			e.DrawBackground
			e.Graphics.DrawString(DisplayName, DisplayFont, DisplayBrush, e.Bounds)
		End Sub
		
		Sub ProgrammerListMouseDown(sender As Object, e As MouseEventArgs)
			'Something must be selected
			If ProgrammerList.SelectedIndex = -1 Then
				Button_EditProgrammer.Enabled = False
				Button_DeleteProgrammer.Enabled = False
				Exit Sub
			End If
			
			'Do not allow dragging of sections
			If ProgrammerList.SelectedItem.ToString.StartsWith("<s>") Then
				Button_EditProgrammer.Enabled = False
				Button_DeleteProgrammer.Enabled = False
				Exit Sub
			End If
			
			Button_EditProgrammer.Enabled = True
			Button_DeleteProgrammer.Enabled = True
			ProgrammerList.DoDragDrop(ProgrammerList.SelectedItem, DragDropEffects.Move)
		End Sub
		
		Sub ProgrammerListDragDrop(sender As Object, e As DragEventArgs)
			'Get drop index
			Dim p As Point = ProgrammerList.PointToClient(New Point(e.X, e.Y))
			Dim NewIndex As Integer = ProgrammerList.IndexFromPoint(p)
			If NewIndex < 1 Then NewIndex = 1
			Dim MoveObject As Object = ProgrammerList.SelectedItem
			ProgrammerList.Items.Remove(MoveObject)
			ProgrammerList.Items.Insert(NewIndex, MoveObject)
			ProgrammerList.SelectedIndex = NewIndex
		End Sub
		
		Sub ProgrammerListDragOver(sender As Object, e As DragEventArgs)
			e.Effect = DragDropEffects.Move
		End Sub
		
		
		Sub ToolListSelectedIndexChanged(sender As Object, e As EventArgs)
			If ToolList.SelectedIndex = -1 Then
				buttonEditTool.Enabled = False
				buttonDeleteTool.Enabled = False
			Else
				buttonEditTool.Enabled = True
				buttonDeleteTool.Enabled = True
			End If
		End Sub
		
	End Class
'End Namespace
