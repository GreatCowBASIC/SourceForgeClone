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
		Private Dim ExternalToolVarsPage As TabPage
		
		Private Dim ClickedToolVarRow As Integer
		
		'Variables for auto scroll on drag and drop
		Private Dim ListToScroll As ListBox
		Private Dim ScrollDirection As Integer = 0
		Private Dim ScrollDistance As Integer = 0
		
		Public Sub New()
			MyBase.New
			'
			' The Me.InitializeComponent call is required for Windows Forms designer support.
			'
			Me.InitializeComponent
			
			'Hide external tool vars page by default
			ExternalToolVarsPage = ToolVarsPrefs
			Me.PrefsTabs.TabPages.Remove(ToolVarsPrefs)
			
			ClickedToolVarRow = -1
			
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
				If pPreferences.GetPref("GCGB", "MDI").ToLower = "tabs" Then
					MDITabs.Checked = True
				Else
					MDIWindows.Checked = True
				End If
				
				'Compiler
				CompilerVerbose.Checked = Preferences.PrefIsYes(pPreferences.GetPref("GCBASIC", "Verbose"))
				CompilerShowBASIC.Checked = False
				If pPreferences.GetPref("GCBASIC", "Preserve") = "a" Then CompilerShowBASIC.Checked = True
				CompilerWarningsAsErrors.Checked = Preferences.PrefIsYes(pPreferences.GetPref("GCBASIC", "WarningsAsErrors"))
				CompilerNoRecompile.Checked = Preferences.PrefIsYes(pPreferences.GetPref("GCBASIC", "FlashOnly"))
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
				
				'Tool variables list
				'Restricted mode means that GCGB is in a school or other environment where the users may be up to no good
				'If restricted, don't allow them to define programmers.
				If Preferences.PrefIsYes(pPreferences.GetPref("GCGB", "Restricted")) Then
					Me.PrefsTabs.Controls.Remove(ToolVarsPrefs)
				Else
					'Find all tools
					Me.ToolVarDataGrid.Rows.Clear
					Dim section As SettingSection = pPreferences.GetSection("toolvariables")
					If Not section is Nothing Then
						For Each currSetting As Setting In section.Settings
							Me.ToolVarDataGrid.Rows.Add(New Object(){currSetting.Name, currSetting.Value})
						Next
					End If
				End If
			End Set
		End Property
		
		#Region " Windows Forms Designer generated code "
		' This method is required for Windows Forms designer support.
		' Do not change the method contents inside the source code editor. The Forms designer might
		' not be able to load this method if it was changed manually.
		Private Sub InitializeComponent()
			Me.components = New System.ComponentModel.Container
			Me.ExtToolsPrefs = New System.Windows.Forms.TabPage
			Me.buttonDeleteTool = New System.Windows.Forms.Button
			Me.buttonEditTool = New System.Windows.Forms.Button
			Me.buttonAddTool = New System.Windows.Forms.Button
			Me.toolList = New System.Windows.Forms.ListBox
			Me.CompilerPrefs = New System.Windows.Forms.TabPage
			Me.CompilerNoRecompile = New System.Windows.Forms.CheckBox
			Me.CompilerWarningsAsErrors = New System.Windows.Forms.CheckBox
			Me.CompilerPause = New System.Windows.Forms.CheckBox
			Me.CompilerShowBASIC = New System.Windows.Forms.CheckBox
			Me.CompilerVerbose = New System.Windows.Forms.CheckBox
			Me.PrefsTabs = New System.Windows.Forms.TabControl
			Me.ProgrammerPrefs = New System.Windows.Forms.TabPage
			Me.Button_DeleteProgrammer = New System.Windows.Forms.Button
			Me.Button_EditProgrammer = New System.Windows.Forms.Button
			Me.Button_AddProgrammer = New System.Windows.Forms.Button
			Me.ProgrammerList = New System.Windows.Forms.ListBox
			Me.EditorPrefs = New System.Windows.Forms.TabPage
			Me.MDILabel = New System.Windows.Forms.GroupBox
			Me.MDITabs = New System.Windows.Forms.RadioButton
			Me.MDIWindows = New System.Windows.Forms.RadioButton
			Me.EditorIndentSize = New System.Windows.Forms.NumericUpDown
			Me.EditorIndentLabel = New System.Windows.Forms.Label
			Me.EditorWarnRecursion = New System.Windows.Forms.CheckBox
			Me.ToolVarsPrefs = New System.Windows.Forms.TabPage
			Me.ToolVarDataGrid = New System.Windows.Forms.DataGridView
			Me.Variable = New System.Windows.Forms.DataGridViewTextBoxColumn
			Me.Value = New System.Windows.Forms.DataGridViewTextBoxColumn
			Me.BrowseColumn = New System.Windows.Forms.DataGridViewButtonColumn
			Me.toolVariableEditMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
			Me.browseForFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
			Me.browseForFolderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
			Me.Button_Cancel = New System.Windows.Forms.Button
			Me.PrefsHelp = New System.Windows.Forms.HelpProvider
			Me.Button_OK = New System.Windows.Forms.Button
			Me.prefsToolTip = New System.Windows.Forms.ToolTip(Me.components)
			Me.toolVarFolderBrowserDialog = New System.Windows.Forms.FolderBrowserDialog
			Me.toolVarOpenFileDialog = New System.Windows.Forms.OpenFileDialog
			Me.ScrollTimer = New System.Windows.Forms.Timer(Me.components)
			Me.ExtToolsPrefs.SuspendLayout
			Me.CompilerPrefs.SuspendLayout
			Me.PrefsTabs.SuspendLayout
			Me.ProgrammerPrefs.SuspendLayout
			Me.EditorPrefs.SuspendLayout
			Me.MDILabel.SuspendLayout
			CType(Me.EditorIndentSize,System.ComponentModel.ISupportInitialize).BeginInit
			Me.ToolVarsPrefs.SuspendLayout
			CType(Me.ToolVarDataGrid,System.ComponentModel.ISupportInitialize).BeginInit
			Me.toolVariableEditMenu.SuspendLayout
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
			Me.ExtToolsPrefs.Size = New System.Drawing.Size(341, 262)
			Me.ExtToolsPrefs.TabIndex = 3
			Me.ExtToolsPrefs.Text = "External Tools"
			Me.ExtToolsPrefs.UseVisualStyleBackColor = true
			'
			'buttonDeleteTool
			'
			Me.buttonDeleteTool.Anchor = System.Windows.Forms.AnchorStyles.Bottom
			Me.buttonDeleteTool.Enabled = false
			Me.buttonDeleteTool.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.PrefsHelp.SetHelpString(Me.buttonDeleteTool, "Delete an external tool")
			Me.buttonDeleteTool.Location = New System.Drawing.Point(214, 232)
			Me.buttonDeleteTool.Name = "buttonDeleteTool"
			Me.PrefsHelp.SetShowHelp(Me.buttonDeleteTool, true)
			Me.buttonDeleteTool.Size = New System.Drawing.Size(88, 24)
			Me.buttonDeleteTool.TabIndex = 3
			Me.buttonDeleteTool.Text = "Delete"
			Me.prefsToolTip.SetToolTip(Me.buttonDeleteTool, "Delete the selected external tool")
			AddHandler Me.buttonDeleteTool.Click, AddressOf Me.ButtonDeleteToolClick
			'
			'buttonEditTool
			'
			Me.buttonEditTool.Anchor = System.Windows.Forms.AnchorStyles.Bottom
			Me.buttonEditTool.Enabled = false
			Me.buttonEditTool.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.PrefsHelp.SetHelpString(Me.buttonEditTool, "Edit the settings of an existing external tool")
			Me.buttonEditTool.Location = New System.Drawing.Point(126, 232)
			Me.buttonEditTool.Name = "buttonEditTool"
			Me.PrefsHelp.SetShowHelp(Me.buttonEditTool, true)
			Me.buttonEditTool.Size = New System.Drawing.Size(88, 24)
			Me.buttonEditTool.TabIndex = 2
			Me.buttonEditTool.Text = "Edit..."
			Me.prefsToolTip.SetToolTip(Me.buttonEditTool, "Edit the selected external tool")
			AddHandler Me.buttonEditTool.Click, AddressOf Me.ButtonEditToolClick
			'
			'buttonAddTool
			'
			Me.buttonAddTool.Anchor = System.Windows.Forms.AnchorStyles.Bottom
			Me.buttonAddTool.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.PrefsHelp.SetHelpString(Me.buttonAddTool, "Add a new external tool")
			Me.buttonAddTool.Location = New System.Drawing.Point(38, 232)
			Me.buttonAddTool.Name = "buttonAddTool"
			Me.PrefsHelp.SetShowHelp(Me.buttonAddTool, true)
			Me.buttonAddTool.Size = New System.Drawing.Size(88, 24)
			Me.buttonAddTool.TabIndex = 1
			Me.buttonAddTool.Text = "Add..."
			Me.prefsToolTip.SetToolTip(Me.buttonAddTool, "Add a new external tool")
			AddHandler Me.buttonAddTool.Click, AddressOf Me.ButtonAddToolClick
			'
			'toolList
			'
			Me.toolList.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
									Or System.Windows.Forms.AnchorStyles.Left)  _
									Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.PrefsHelp.SetHelpString(Me.toolList, "Shows the external tools that have been set up to work with Great Cow Graphical B"& _ 
						"ASIC")
			Me.toolList.IntegralHeight = false
			Me.toolList.Location = New System.Drawing.Point(8, 8)
			Me.toolList.Name = "toolList"
			Me.PrefsHelp.SetShowHelp(Me.toolList, true)
			Me.toolList.Size = New System.Drawing.Size(325, 212)
			Me.toolList.TabIndex = 0
			Me.prefsToolTip.SetToolTip(Me.toolList, "List of all currently defined external tools")
			AddHandler Me.toolList.SelectedIndexChanged, AddressOf Me.ToolListSelectedIndexChanged
			'
			'CompilerPrefs
			'
			Me.CompilerPrefs.Controls.Add(Me.CompilerNoRecompile)
			Me.CompilerPrefs.Controls.Add(Me.CompilerWarningsAsErrors)
			Me.CompilerPrefs.Controls.Add(Me.CompilerPause)
			Me.CompilerPrefs.Controls.Add(Me.CompilerShowBASIC)
			Me.CompilerPrefs.Controls.Add(Me.CompilerVerbose)
			Me.CompilerPrefs.Location = New System.Drawing.Point(4, 22)
			Me.CompilerPrefs.Name = "CompilerPrefs"
			Me.CompilerPrefs.Size = New System.Drawing.Size(341, 262)
			Me.CompilerPrefs.TabIndex = 1
			Me.CompilerPrefs.Text = "Compiler"
			Me.CompilerPrefs.UseVisualStyleBackColor = true
			'
			'CompilerNoRecompile
			'
			Me.CompilerNoRecompile.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.PrefsHelp.SetHelpString(Me.CompilerNoRecompile, "Prevent the compiler from taking time to recompile a program if the program has n"& _ 
						"ot changed since it was last compiled.")
			Me.CompilerNoRecompile.Location = New System.Drawing.Point(16, 88)
			Me.CompilerNoRecompile.Name = "CompilerNoRecompile"
			Me.PrefsHelp.SetShowHelp(Me.CompilerNoRecompile, true)
			Me.CompilerNoRecompile.Size = New System.Drawing.Size(224, 16)
			Me.CompilerNoRecompile.TabIndex = 4
			Me.CompilerNoRecompile.Text = "Prevent recompile if code is unchanged"
			Me.prefsToolTip.SetToolTip(Me.CompilerNoRecompile, "Prevent the compiler from taking time to recompile a program if the program has n"& _ 
						"ot changed since it was last compiled.")
			'
			'CompilerWarningsAsErrors
			'
			Me.CompilerWarningsAsErrors.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.PrefsHelp.SetHelpString(Me.CompilerWarningsAsErrors, "Stops the program from being downloaded to the chip if any warnings are generated"& _ 
						" while compiling it")
			Me.CompilerWarningsAsErrors.Location = New System.Drawing.Point(16, 64)
			Me.CompilerWarningsAsErrors.Name = "CompilerWarningsAsErrors"
			Me.PrefsHelp.SetShowHelp(Me.CompilerWarningsAsErrors, true)
			Me.CompilerWarningsAsErrors.Size = New System.Drawing.Size(224, 16)
			Me.CompilerWarningsAsErrors.TabIndex = 3
			Me.CompilerWarningsAsErrors.Text = "Treat warnings as errors"
			Me.prefsToolTip.SetToolTip(Me.CompilerWarningsAsErrors, "Stops the program from being downloaded to the chip if any warnings are generated"& _ 
						" while compiling it")
			'
			'CompilerPause
			'
			Me.CompilerPause.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.PrefsHelp.SetHelpString(Me.CompilerPause, "Keeps the compiler window open until a key is pressed")
			Me.CompilerPause.Location = New System.Drawing.Point(16, 112)
			Me.CompilerPause.Name = "CompilerPause"
			Me.PrefsHelp.SetShowHelp(Me.CompilerPause, true)
			Me.CompilerPause.Size = New System.Drawing.Size(168, 16)
			Me.CompilerPause.TabIndex = 2
			Me.CompilerPause.Text = "Pause after compilation"
			Me.prefsToolTip.SetToolTip(Me.CompilerPause, "Keeps the compiler window open until a key is pressed")
			'
			'CompilerShowBASIC
			'
			Me.CompilerShowBASIC.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.PrefsHelp.SetHelpString(Me.CompilerShowBASIC, "Copies the original BASIC program into the assembly file produced by the compiler"& _ 
						"")
			Me.CompilerShowBASIC.Location = New System.Drawing.Point(16, 40)
			Me.CompilerShowBASIC.Name = "CompilerShowBASIC"
			Me.PrefsHelp.SetShowHelp(Me.CompilerShowBASIC, true)
			Me.CompilerShowBASIC.Size = New System.Drawing.Size(256, 16)
			Me.CompilerShowBASIC.TabIndex = 1
			Me.CompilerShowBASIC.Text = "Show BASIC code as comments in assembly"
			Me.prefsToolTip.SetToolTip(Me.CompilerShowBASIC, "Copies the original BASIC program into the assembly file produced by the compiler"& _ 
						"")
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
			Me.prefsToolTip.SetToolTip(Me.CompilerVerbose, "Makes compiler display detailed information about the compilation process.")
			'
			'PrefsTabs
			'
			Me.PrefsTabs.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
									Or System.Windows.Forms.AnchorStyles.Left)  _
									Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.PrefsTabs.Controls.Add(Me.ProgrammerPrefs)
			Me.PrefsTabs.Controls.Add(Me.CompilerPrefs)
			Me.PrefsTabs.Controls.Add(Me.EditorPrefs)
			Me.PrefsTabs.Controls.Add(Me.ExtToolsPrefs)
			Me.PrefsTabs.Controls.Add(Me.ToolVarsPrefs)
			Me.PrefsTabs.Location = New System.Drawing.Point(8, 8)
			Me.PrefsTabs.Name = "PrefsTabs"
			Me.PrefsTabs.SelectedIndex = 0
			Me.PrefsTabs.Size = New System.Drawing.Size(349, 288)
			Me.PrefsTabs.TabIndex = 0
			'
			'ProgrammerPrefs
			'
			Me.ProgrammerPrefs.Controls.Add(Me.Button_DeleteProgrammer)
			Me.ProgrammerPrefs.Controls.Add(Me.Button_EditProgrammer)
			Me.ProgrammerPrefs.Controls.Add(Me.Button_AddProgrammer)
			Me.ProgrammerPrefs.Controls.Add(Me.ProgrammerList)
			Me.ProgrammerPrefs.Location = New System.Drawing.Point(4, 22)
			Me.ProgrammerPrefs.Name = "ProgrammerPrefs"
			Me.ProgrammerPrefs.Size = New System.Drawing.Size(341, 262)
			Me.ProgrammerPrefs.TabIndex = 2
			Me.ProgrammerPrefs.Text = "Programmers"
			Me.ProgrammerPrefs.UseVisualStyleBackColor = true
			'
			'Button_DeleteProgrammer
			'
			Me.Button_DeleteProgrammer.Anchor = System.Windows.Forms.AnchorStyles.Bottom
			Me.Button_DeleteProgrammer.Enabled = false
			Me.Button_DeleteProgrammer.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.PrefsHelp.SetHelpString(Me.Button_DeleteProgrammer, "Delete a programmer")
			Me.Button_DeleteProgrammer.Location = New System.Drawing.Point(214, 232)
			Me.Button_DeleteProgrammer.Name = "Button_DeleteProgrammer"
			Me.PrefsHelp.SetShowHelp(Me.Button_DeleteProgrammer, true)
			Me.Button_DeleteProgrammer.Size = New System.Drawing.Size(88, 24)
			Me.Button_DeleteProgrammer.TabIndex = 3
			Me.Button_DeleteProgrammer.Text = "Delete"
			Me.prefsToolTip.SetToolTip(Me.Button_DeleteProgrammer, "Delete the selected programmer")
			AddHandler Me.Button_DeleteProgrammer.Click, AddressOf Me.Button_DeleteProgrammerClick
			'
			'Button_EditProgrammer
			'
			Me.Button_EditProgrammer.Anchor = System.Windows.Forms.AnchorStyles.Bottom
			Me.Button_EditProgrammer.Enabled = false
			Me.Button_EditProgrammer.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.PrefsHelp.SetHelpString(Me.Button_EditProgrammer, "Edit the settings of an existing programmer")
			Me.Button_EditProgrammer.Location = New System.Drawing.Point(126, 232)
			Me.Button_EditProgrammer.Name = "Button_EditProgrammer"
			Me.PrefsHelp.SetShowHelp(Me.Button_EditProgrammer, true)
			Me.Button_EditProgrammer.Size = New System.Drawing.Size(88, 24)
			Me.Button_EditProgrammer.TabIndex = 2
			Me.Button_EditProgrammer.Text = "Edit..."
			Me.prefsToolTip.SetToolTip(Me.Button_EditProgrammer, "Edit the selected programmer")
			AddHandler Me.Button_EditProgrammer.Click, AddressOf Me.Button_EditProgrammerClick
			'
			'Button_AddProgrammer
			'
			Me.Button_AddProgrammer.Anchor = System.Windows.Forms.AnchorStyles.Bottom
			Me.Button_AddProgrammer.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.PrefsHelp.SetHelpString(Me.Button_AddProgrammer, "Add a new programmer")
			Me.Button_AddProgrammer.Location = New System.Drawing.Point(38, 232)
			Me.Button_AddProgrammer.Name = "Button_AddProgrammer"
			Me.PrefsHelp.SetShowHelp(Me.Button_AddProgrammer, true)
			Me.Button_AddProgrammer.Size = New System.Drawing.Size(88, 24)
			Me.Button_AddProgrammer.TabIndex = 1
			Me.Button_AddProgrammer.Text = "Add..."
			Me.prefsToolTip.SetToolTip(Me.Button_AddProgrammer, "Add a new programmer")
			AddHandler Me.Button_AddProgrammer.Click, AddressOf Me.Button_AddProgrammerClick
			'
			'ProgrammerList
			'
			Me.ProgrammerList.AllowDrop = true
			Me.ProgrammerList.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
									Or System.Windows.Forms.AnchorStyles.Left)  _
									Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.ProgrammerList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
			Me.PrefsHelp.SetHelpString(Me.ProgrammerList, "Shows the programmers that have been set up to work with Great Cow Graphical BASI"& _ 
						"C. Drag and drop to reorder.")
			Me.ProgrammerList.IntegralHeight = false
			Me.ProgrammerList.Location = New System.Drawing.Point(8, 8)
			Me.ProgrammerList.Name = "ProgrammerList"
			Me.PrefsHelp.SetShowHelp(Me.ProgrammerList, true)
			Me.ProgrammerList.Size = New System.Drawing.Size(325, 212)
			Me.ProgrammerList.TabIndex = 0
			Me.prefsToolTip.SetToolTip(Me.ProgrammerList, "Shows the programmers that have been set up to work with Great Cow Graphical BASI"& _ 
						"C. Drag and drop to reorder.")
			AddHandler Me.ProgrammerList.DrawItem, AddressOf Me.ProgrammerListDrawItem
			AddHandler Me.ProgrammerList.DragOver, AddressOf Me.ProgrammerListDragOver
			AddHandler Me.ProgrammerList.DragDrop, AddressOf Me.ProgrammerListDragDrop
			AddHandler Me.ProgrammerList.MouseDown, AddressOf Me.ProgrammerListMouseDown
			'
			'EditorPrefs
			'
			Me.EditorPrefs.Controls.Add(Me.MDILabel)
			Me.EditorPrefs.Controls.Add(Me.EditorIndentSize)
			Me.EditorPrefs.Controls.Add(Me.EditorIndentLabel)
			Me.EditorPrefs.Controls.Add(Me.EditorWarnRecursion)
			Me.EditorPrefs.Location = New System.Drawing.Point(4, 22)
			Me.EditorPrefs.Name = "EditorPrefs"
			Me.EditorPrefs.Size = New System.Drawing.Size(341, 262)
			Me.EditorPrefs.TabIndex = 0
			Me.EditorPrefs.Text = "Editor"
			Me.EditorPrefs.UseVisualStyleBackColor = true
			'
			'MDILabel
			'
			Me.MDILabel.Controls.Add(Me.MDITabs)
			Me.MDILabel.Controls.Add(Me.MDIWindows)
			Me.MDILabel.Location = New System.Drawing.Point(8, 80)
			Me.MDILabel.Name = "MDILabel"
			Me.MDILabel.Size = New System.Drawing.Size(264, 48)
			Me.MDILabel.TabIndex = 3
			Me.MDILabel.TabStop = false
			Me.MDILabel.Text = "To display multiple programs, use"
			'
			'MDITabs
			'
			Me.PrefsHelp.SetHelpString(Me.MDITabs, "Show multiple programs in tabs. Easier to change between programs, but offers les"& _ 
						"s freedom in moving things around.")
			Me.MDITabs.Location = New System.Drawing.Point(128, 16)
			Me.MDITabs.Name = "MDITabs"
			Me.PrefsHelp.SetShowHelp(Me.MDITabs, true)
			Me.MDITabs.Size = New System.Drawing.Size(88, 24)
			Me.MDITabs.TabIndex = 1
			Me.MDITabs.TabStop = true
			Me.MDITabs.Text = "Tabs"
			Me.prefsToolTip.SetToolTip(Me.MDITabs, "Show multiple programs in tabs. Easier to change between programs, but offers les"& _ 
						"s freedom in moving things around.")
			Me.MDITabs.UseVisualStyleBackColor = true
			'
			'MDIWindows
			'
			Me.PrefsHelp.SetHelpString(Me.MDIWindows, "Show multiple programs in windows inside the main GCGB window. Allows programs to"& _ 
						" be placed side by side easily, but can also make it harder to find a particular"& _ 
						" program.")
			Me.MDIWindows.Location = New System.Drawing.Point(8, 16)
			Me.MDIWindows.Name = "MDIWindows"
			Me.PrefsHelp.SetShowHelp(Me.MDIWindows, true)
			Me.MDIWindows.Size = New System.Drawing.Size(88, 24)
			Me.MDIWindows.TabIndex = 0
			Me.MDIWindows.TabStop = true
			Me.MDIWindows.Text = "Windows"
			Me.prefsToolTip.SetToolTip(Me.MDIWindows, "Show multiple programs in windows inside the main GCGB window. Allows programs to"& _ 
						" be placed side by side easily, but can also make it harder to find a particular"& _ 
						" program.")
			Me.MDIWindows.UseVisualStyleBackColor = true
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
			Me.prefsToolTip.SetToolTip(Me.EditorIndentSize, "Amount to indent nested icons (like contents of If icon)")
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
			Me.prefsToolTip.SetToolTip(Me.EditorWarnRecursion, "Warn whenever a subroutine is called from itself.")
			'
			'ToolVarsPrefs
			'
			Me.ToolVarsPrefs.Controls.Add(Me.ToolVarDataGrid)
			Me.ToolVarsPrefs.Location = New System.Drawing.Point(4, 22)
			Me.ToolVarsPrefs.Name = "ToolVarsPrefs"
			Me.ToolVarsPrefs.Padding = New System.Windows.Forms.Padding(3)
			Me.ToolVarsPrefs.Size = New System.Drawing.Size(341, 262)
			Me.ToolVarsPrefs.TabIndex = 4
			Me.ToolVarsPrefs.Text = "Tool Variables"
			Me.ToolVarsPrefs.UseVisualStyleBackColor = true
			'
			'ToolVarDataGrid
			'
			Me.ToolVarDataGrid.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
									Or System.Windows.Forms.AnchorStyles.Left)  _
									Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.ToolVarDataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
			Me.ToolVarDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
			Me.ToolVarDataGrid.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Variable, Me.Value, Me.BrowseColumn})
			Me.ToolVarDataGrid.Location = New System.Drawing.Point(8, 16)
			Me.ToolVarDataGrid.Name = "ToolVarDataGrid"
			Me.ToolVarDataGrid.Size = New System.Drawing.Size(325, 240)
			Me.ToolVarDataGrid.TabIndex = 1
			Me.prefsToolTip.SetToolTip(Me.ToolVarDataGrid, "Set up extra variables that can be used to configure all programmers")
			AddHandler Me.ToolVarDataGrid.CellClick, AddressOf Me.ToolVarDataGridCellClick
			'
			'Variable
			'
			Me.Variable.FillWeight = 40!
			Me.Variable.HeaderText = "Variable"
			Me.Variable.Name = "Variable"
			'
			'Value
			'
			Me.Value.HeaderText = "Value"
			Me.Value.Name = "Value"
			'
			'BrowseColumn
			'
			Me.BrowseColumn.FillWeight = 20!
			Me.BrowseColumn.HeaderText = ""
			Me.BrowseColumn.Name = "BrowseColumn"
			Me.BrowseColumn.Text = "..."
			Me.BrowseColumn.UseColumnTextForButtonValue = true
			'
			'toolVariableEditMenu
			'
			Me.toolVariableEditMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.browseForFileToolStripMenuItem, Me.browseForFolderToolStripMenuItem})
			Me.toolVariableEditMenu.Name = "contextMenuStrip1"
			Me.toolVariableEditMenu.Size = New System.Drawing.Size(177, 48)
			'
			'browseForFileToolStripMenuItem
			'
			Me.browseForFileToolStripMenuItem.Name = "browseForFileToolStripMenuItem"
			Me.browseForFileToolStripMenuItem.Size = New System.Drawing.Size(176, 22)
			Me.browseForFileToolStripMenuItem.Text = "Browse for file ..."
			AddHandler Me.browseForFileToolStripMenuItem.Click, AddressOf Me.BrowseForFileToolStripMenuItemClick
			'
			'browseForFolderToolStripMenuItem
			'
			Me.browseForFolderToolStripMenuItem.Name = "browseForFolderToolStripMenuItem"
			Me.browseForFolderToolStripMenuItem.Size = New System.Drawing.Size(176, 22)
			Me.browseForFolderToolStripMenuItem.Text = "Browse for folder ..."
			AddHandler Me.browseForFolderToolStripMenuItem.Click, AddressOf Me.BrowseForFolderToolStripMenuItemClick
			'
			'Button_Cancel
			'
			Me.Button_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom
			Me.Button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me.Button_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.Button_Cancel.Location = New System.Drawing.Point(190, 304)
			Me.Button_Cancel.Name = "Button_Cancel"
			Me.Button_Cancel.Size = New System.Drawing.Size(80, 24)
			Me.Button_Cancel.TabIndex = 2
			Me.Button_Cancel.Text = "Cancel"
			'
			'Button_OK
			'
			Me.Button_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom
			Me.Button_OK.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.Button_OK.Location = New System.Drawing.Point(102, 304)
			Me.Button_OK.Name = "Button_OK"
			Me.Button_OK.Size = New System.Drawing.Size(80, 24)
			Me.Button_OK.TabIndex = 1
			Me.Button_OK.Text = "OK"
			AddHandler Me.Button_OK.Click, AddressOf Me.Button_OKClick
			'
			'prefsToolTip
			'
			AddHandler Me.prefsToolTip.Popup, AddressOf Me.PrefsToolTipPopup
			'
			'toolVarOpenFileDialog
			'
			AddHandler Me.toolVarOpenFileDialog.FileOk, AddressOf Me.ToolVarOpenFileDialogFileOk
			'
			'ScrollTimer
			'
			AddHandler Me.ScrollTimer.Tick, AddressOf Me.ScrollTimerTick
			'
			'PreferencesWindow
			'
			Me.AcceptButton = Me.Button_OK
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.CancelButton = Me.Button_Cancel
			Me.ClientSize = New System.Drawing.Size(367, 335)
			Me.Controls.Add(Me.Button_Cancel)
			Me.Controls.Add(Me.Button_OK)
			Me.Controls.Add(Me.PrefsTabs)
			Me.MaximizeBox = false
			Me.MinimizeBox = false
			Me.MinimumSize = New System.Drawing.Size(322, 373)
			Me.Name = "PreferencesWindow"
			Me.PrefsHelp.SetShowHelp(Me, false)
			Me.ShowInTaskbar = false
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
			Me.Text = "Preferences"
			Me.ExtToolsPrefs.ResumeLayout(false)
			Me.CompilerPrefs.ResumeLayout(false)
			Me.PrefsTabs.ResumeLayout(false)
			Me.ProgrammerPrefs.ResumeLayout(false)
			Me.EditorPrefs.ResumeLayout(false)
			Me.MDILabel.ResumeLayout(false)
			CType(Me.EditorIndentSize,System.ComponentModel.ISupportInitialize).EndInit
			Me.ToolVarsPrefs.ResumeLayout(false)
			CType(Me.ToolVarDataGrid,System.ComponentModel.ISupportInitialize).EndInit
			Me.toolVariableEditMenu.ResumeLayout(false)
			Me.ResumeLayout(false)
		End Sub
		Private ScrollTimer As System.Windows.Forms.Timer
		Private toolVarOpenFileDialog As System.Windows.Forms.OpenFileDialog
		Private toolVarFolderBrowserDialog As System.Windows.Forms.FolderBrowserDialog
		Private BrowseColumn As System.Windows.Forms.DataGridViewButtonColumn
		Private toolVariableEditMenu As System.Windows.Forms.ContextMenuStrip
		Private browseForFolderToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
		Private browseForFileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
		Private components As System.ComponentModel.IContainer
		Private prefsToolTip As System.Windows.Forms.ToolTip
		Private ToolVarDataGrid As System.Windows.Forms.DataGridView
		Private Value As System.Windows.Forms.DataGridViewTextBoxColumn
		Private Variable As System.Windows.Forms.DataGridViewTextBoxColumn
		Private ToolVarsPrefs As System.Windows.Forms.TabPage
		Private CompilerNoRecompile As System.Windows.Forms.CheckBox
		Private MDIWindows As System.Windows.Forms.RadioButton
		Private MDITabs As System.Windows.Forms.RadioButton
		Private MDILabel As System.Windows.Forms.GroupBox
		Private CompilerWarningsAsErrors As System.Windows.Forms.CheckBox
		Private EditorIndentLabel As System.Windows.Forms.Label
		Private EditorIndentSize As System.Windows.Forms.NumericUpDown
		#End Region
		
		Public Sub HideGCGBOptions
			'Remove tabs that don't get shown outside of GCGB
			Me.PrefsTabs.Controls.Remove(EditorPrefs)
			Me.PrefsTabs.Controls.Remove(ExtToolsPrefs)
			
			'Add tab that doesn't get shown in GCGB
			Me.PrefsTabs.Controls.Add(ExternalToolVarsPage)
			
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
				If MDIWindows.Checked Then
					pPreferences.SetPref("GCGB", "MDI", "windows")
				Else If MDITabs.Checked Then
					pPreferences.SetPref("GCGB", "MDI", "tabs")
				End If
			End If
			
			'Compiler
			pPreferences.SetPref("GCBASIC", "Verbose", CompilerVerbose.Checked)
			If Me.CompilerShowBASIC.Checked = False Then
				pPreferences.SetPref("GCBASIC", "Preserve", "n")
			Else
				pPreferences.SetPref("GCBASIC", "Preserve", "a")
			End If
			pPreferences.SetPref("GCBASIC", "WarningsAsErrors", CompilerWarningsAsErrors.Checked)
			pPreferences.SetPref("GCBASIC", "FlashOnly", CompilerNoRecompile.Checked)
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
			
			'Tool variables
			'Clear out old list
			If GCGBOptionsHidden Then
				Dim ToolVarsSection As SettingSection = pPreferences.GetSection("toolvariables")
				If Not ToolVarsSection Is Nothing Then
					pPreferences.PrefGroups.Remove(ToolVarsSection)
				End If
				'Create new list from table
				Dim RowName, RowValue As String
				For Each Row As DataGridViewRow In ToolVarDataGrid.Rows
					RowName = Row.Cells.Item(0).FormattedValue
					RowValue = Row.Cells.Item(1).FormattedValue
					If RowName <> "" Then
						pPreferences.SetPref("toolvariables", RowName, RowValue)
					End If
				Next
			End If
			
			'Save and Exit
			pPreferences.SavePreferences
			pPreferences.FireChanged
			Me.Close
			
		End Sub
		
		Private Sub Button_AddProgrammerClick(sender As System.Object, e As System.EventArgs)
			'Hidden option to display version number
			If My.Computer.Keyboard.AltKeyDown Then
				Me.Text = "Preferences (editor revision 13)"
				Exit Sub
			End If
			
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
			
			ScrollDistance = 16
			ScrollDirection = 0
			Dim MouseLoc As Point = ProgrammerList.PointToClient(New Point(e.X, e.Y))
			Dim PanelBottom As Integer = ProgrammerList.Size.Height - ScrollDistance
			ListToScroll = ProgrammerList
			
			If MouseLoc.Y < ScrollDistance Then
				ScrollDirection = -1
				ScrollTimer.Start
			ElseIf MouseLoc.Y > PanelBottom Then
				ScrollDirection = 1
				ScrollTimer.Start
			Else
				ScrollTimer.Stop
			End If
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
		
		Sub BrowseForFileToolStripMenuItemClick(sender As Object, e As EventArgs)
			If ClickedToolVarRow <> -1 Then
				toolVarOpenFileDialog.FileName = ToolVarDataGrid.Rows(ClickedToolVarRow).Cells(1).Value
				toolVarOpenFileDialog.ShowDialog
			End If
		End Sub
		
		Sub BrowseForFolderToolStripMenuItemClick(sender As Object, e As EventArgs)
			If ClickedToolVarRow <> -1 Then
				'Set folder browser to current path, show
				toolVarFolderBrowserDialog.SelectedPath = ToolVarDataGrid.Rows(ClickedToolVarRow).Cells(1).Value
				toolVarFolderBrowserDialog.ShowDialog
				'If new path was set, update cell
				If toolVarFolderBrowserDialog.SelectedPath <> "" Then
					ToolVarDataGrid.Rows(ClickedToolVarRow).Cells(1).Value = toolVarFolderBrowserDialog.SelectedPath
				End If
				ClickedToolVarRow = -1
			End If
		End Sub
		
		Sub ToolVarDataGridCellClick(sender As Object, e As DataGridViewCellEventArgs)
			Dim table As DataGridView = sender
			
			'Button cell?
			If e.ColumnIndex = 2 Then
				If e.RowIndex >= 0 And e.RowIndex < table.Rows.Count - 1 Then
					ClickedToolVarRow = e.RowIndex
					
					Dim buttonLocation As Point = table.PointToScreen(table.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, False).Location)
					toolVariableEditMenu.Show(Point.Add(buttonLocation, New Size(0, table.Rows(e.RowIndex).Height)))
					
				End If
			End If
		End Sub
		
		Sub ToolVarOpenFileDialogFileOk(sender As Object, e As System.ComponentModel.CancelEventArgs)
			If ClickedToolVarRow <> -1 Then
				ToolVarDataGrid.Rows(ClickedToolVarRow).Cells(1).Value = toolVarOpenFileDialog.FileName
				ClickedToolVarRow = -1
			End If
		End Sub
		
		Sub ScrollTimerTick(sender As Object, e As EventArgs)
			'When timer ticks, scroll panel up or down
			If ListToScroll Is Nothing Then Exit Sub
			If ScrollDirection = 0 Then Exit Sub
			
			ListToScroll.TopIndex = ListToScroll.TopIndex + ScrollDirection
			
		End Sub
		
		
		Sub PrefsToolTipPopup(sender As Object, e As PopupEventArgs)
			Dim TipChanged As Boolean = False
			If GCGBOptionsHidden Then
				Dim TipText As String = prefsToolTip.GetToolTip(e.AssociatedControl)
				If TipText.Contains("Great Cow Graphical BASIC") Then
					TipText = TipText.Replace("Great Cow Graphical BASIC", "Great Cow BASIC")
					TipChanged = True
				End If
				If TipText.Contains("GCGB") Then
					TipText = TipText.Replace("GCGB", "GCBASIC")
					TipChanged = True
				End If
				If TipChanged Then
					prefsToolTip.SetToolTip(e.AssociatedControl, TipText)
				End If
			End If
			
		End Sub
	End Class
'End Namespace
