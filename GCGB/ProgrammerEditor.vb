'
' Created by SharpDevelop.
' User: Administrator
' Date: 22/01/2009
' Time: 2:54 PM
'
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.Windows.Forms

'Namespace Great_Cow_Graphical_BASIC
	
	Public Class ProgrammerEditor
		Inherits System.Windows.Forms.Form
		Private ProgrammerEditorHelp As System.Windows.Forms.HelpProvider
		Private ProgrammerName As System.Windows.Forms.TextBox
		Private ProgBrowse As System.Windows.Forms.Button
		Private fileLabel As System.Windows.Forms.Label
		Private ProgrammerFile As System.Windows.Forms.OpenFileDialog
		Private workingDirLabel As System.Windows.Forms.Label
		Private ProgrammerWorkingDir As System.Windows.Forms.TextBox
		Private ProgrammerParams As System.Windows.Forms.TextBox
		Private Button_OK As System.Windows.Forms.Button
		Private ProgrammerFileName As System.Windows.Forms.TextBox
		Private Button_Cancel As System.Windows.Forms.Button
		Private nameLabel As System.Windows.Forms.Label
		Private WorkingDirBrowser As System.Windows.Forms.FolderBrowserDialog
		Private browseWorkingDir As System.Windows.Forms.Button
		Private cmdLineLabel As System.Windows.Forms.Label
		
		Private Dim changeAccepted As Boolean
		Private Dim pSection As SettingSection = Nothing
		Private Dim pMode As Modes = Modes.PROGRAMMER
		Private Dim pNameChanged As Boolean = False
		
		Private Dim pSource As Preferences
		
		Public Enum Modes
			PROGRAMMER
			TOOL
		End Enum
		
		Public Sub New(Source As Preferences)
			MyBase.New
			'
			' The Me.InitializeComponent call is required for Windows Forms designer support.
			'
			Me.InitializeComponent
			
			changeAccepted = false
			pSource = Source
			
			'Fill port editor
			FillPortEditor
			
			ApplyTranslation
			UpdatePortEditor
		End Sub
		
		Public Sub New(Source As Preferences, Section As SettingSection)
			Me.New (Source)
			
			If Not Section Is Nothing Then
				pSection = Section
				ProgrammerName.Text = section.GetValue("desc")
				ProgrammerFileName.Text = section.GetValue("command")
				ProgrammerParams.Text = section.GetValue("params")
				ProgrammerWorkingDir.Text = section.GetValue("workingdir")
				portList.Text = section.GetValue("port")
			End If
			
			UpdatePortEditor
		End Sub
		
		#Region " Windows Forms Designer generated code "
		' This method is required for Windows Forms designer support.
		' Do not change the method contents inside the source code editor. The Forms designer might
		' not be able to load this method if it was changed manually.
		Private Sub InitializeComponent()
			Me.cmdLineLabel = New System.Windows.Forms.Label
			Me.browseWorkingDir = New System.Windows.Forms.Button
			Me.WorkingDirBrowser = New System.Windows.Forms.FolderBrowserDialog
			Me.nameLabel = New System.Windows.Forms.Label
			Me.Button_Cancel = New System.Windows.Forms.Button
			Me.ProgrammerFileName = New System.Windows.Forms.TextBox
			Me.Button_OK = New System.Windows.Forms.Button
			Me.ProgrammerParams = New System.Windows.Forms.TextBox
			Me.ProgrammerWorkingDir = New System.Windows.Forms.TextBox
			Me.workingDirLabel = New System.Windows.Forms.Label
			Me.ProgrammerFile = New System.Windows.Forms.OpenFileDialog
			Me.fileLabel = New System.Windows.Forms.Label
			Me.ProgBrowse = New System.Windows.Forms.Button
			Me.ProgrammerName = New System.Windows.Forms.TextBox
			Me.ProgrammerEditorHelp = New System.Windows.Forms.HelpProvider
			Me.portLabel = New System.Windows.Forms.Label
			Me.portList = New System.Windows.Forms.ComboBox
			Me.SuspendLayout
			'
			'cmdLineLabel
			'
			Me.cmdLineLabel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
									Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.cmdLineLabel.Location = New System.Drawing.Point(8, 120)
			Me.cmdLineLabel.Name = "cmdLineLabel"
			Me.cmdLineLabel.Size = New System.Drawing.Size(256, 16)
			Me.cmdLineLabel.TabIndex = 13
			Me.cmdLineLabel.Text = "Command Line Parameters:"
			'
			'browseWorkingDir
			'
			Me.browseWorkingDir.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.browseWorkingDir.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.ProgrammerEditorHelp.SetHelpString(Me.browseWorkingDir, "Browse for the program's working directory")
			Me.browseWorkingDir.Location = New System.Drawing.Point(192, 192)
			Me.browseWorkingDir.Name = "browseWorkingDir"
			Me.ProgrammerEditorHelp.SetShowHelp(Me.browseWorkingDir, true)
			Me.browseWorkingDir.Size = New System.Drawing.Size(72, 24)
			Me.browseWorkingDir.TabIndex = 8
			Me.browseWorkingDir.Text = "Browse..."
			AddHandler Me.browseWorkingDir.Click, AddressOf Me.BrowseWorkingDirClick
			'
			'WorkingDirBrowser
			'
			Me.WorkingDirBrowser.Description = "Which folder should be used as the working directory for the program?"
			'
			'nameLabel
			'
			Me.nameLabel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
									Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.nameLabel.Location = New System.Drawing.Point(8, 8)
			Me.nameLabel.Name = "nameLabel"
			Me.nameLabel.Size = New System.Drawing.Size(256, 16)
			Me.nameLabel.TabIndex = 11
			Me.nameLabel.Text = "Name:"
			'
			'Button_Cancel
			'
			Me.Button_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom
			Me.Button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me.Button_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.Button_Cancel.Location = New System.Drawing.Point(144, 273)
			Me.Button_Cancel.Name = "Button_Cancel"
			Me.Button_Cancel.Size = New System.Drawing.Size(80, 24)
			Me.Button_Cancel.TabIndex = 6
			Me.Button_Cancel.Text = "Cancel"
			'
			'ProgrammerFileName
			'
			Me.ProgrammerFileName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
									Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.ProgrammerEditorHelp.SetHelpString(Me.ProgrammerFileName, "Program file, which Great Cow Graphical BASIC will run.")
			Me.ProgrammerFileName.Location = New System.Drawing.Point(8, 80)
			Me.ProgrammerFileName.Name = "ProgrammerFileName"
			Me.ProgrammerEditorHelp.SetShowHelp(Me.ProgrammerFileName, true)
			Me.ProgrammerFileName.Size = New System.Drawing.Size(176, 20)
			Me.ProgrammerFileName.TabIndex = 2
			'
			'Button_OK
			'
			Me.Button_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom
			Me.Button_OK.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.Button_OK.Location = New System.Drawing.Point(48, 273)
			Me.Button_OK.Name = "Button_OK"
			Me.Button_OK.Size = New System.Drawing.Size(80, 24)
			Me.Button_OK.TabIndex = 5
			Me.Button_OK.Text = "OK"
			AddHandler Me.Button_OK.Click, AddressOf Me.Button_OKClick
			'
			'ProgrammerParams
			'
			Me.ProgrammerParams.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
									Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.ProgrammerEditorHelp.SetHelpString(Me.ProgrammerParams, "Command line parameters for the program. Please consult your programmer or extern"& _ 
						"al tool manual and the Configuration > Setting up a programmer section of the GC"& _ 
						"GB help for details.")
			Me.ProgrammerParams.Location = New System.Drawing.Point(8, 136)
			Me.ProgrammerParams.Name = "ProgrammerParams"
			Me.ProgrammerEditorHelp.SetShowHelp(Me.ProgrammerParams, true)
			Me.ProgrammerParams.Size = New System.Drawing.Size(256, 20)
			Me.ProgrammerParams.TabIndex = 3
			AddHandler Me.ProgrammerParams.TextChanged, AddressOf Me.ProgrammerParamsTextChanged
			'
			'ProgrammerWorkingDir
			'
			Me.ProgrammerWorkingDir.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
									Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.ProgrammerEditorHelp.SetHelpString(Me.ProgrammerWorkingDir, "Folder to use as the working directory for the program")
			Me.ProgrammerWorkingDir.Location = New System.Drawing.Point(8, 192)
			Me.ProgrammerWorkingDir.Name = "ProgrammerWorkingDir"
			Me.ProgrammerEditorHelp.SetShowHelp(Me.ProgrammerWorkingDir, true)
			Me.ProgrammerWorkingDir.Size = New System.Drawing.Size(176, 20)
			Me.ProgrammerWorkingDir.TabIndex = 4
			'
			'workingDirLabel
			'
			Me.workingDirLabel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
									Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.workingDirLabel.Location = New System.Drawing.Point(8, 176)
			Me.workingDirLabel.Name = "workingDirLabel"
			Me.workingDirLabel.Size = New System.Drawing.Size(256, 16)
			Me.workingDirLabel.TabIndex = 14
			Me.workingDirLabel.Text = "Working Directory:"
			'
			'ProgrammerFile
			'
			Me.ProgrammerFile.DefaultExt = "exe"
			Me.ProgrammerFile.Filter = "Programs|*.exe"
			Me.ProgrammerFile.RestoreDirectory = true
			Me.ProgrammerFile.Title = "Select Programmer Utility"
			AddHandler Me.ProgrammerFile.FileOk, AddressOf Me.ProgrammerFileFileOk
			'
			'fileLabel
			'
			Me.fileLabel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
									Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.fileLabel.Location = New System.Drawing.Point(8, 64)
			Me.fileLabel.Name = "fileLabel"
			Me.fileLabel.Size = New System.Drawing.Size(256, 16)
			Me.fileLabel.TabIndex = 12
			Me.fileLabel.Text = "File:"
			'
			'ProgBrowse
			'
			Me.ProgBrowse.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.ProgBrowse.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.ProgrammerEditorHelp.SetHelpString(Me.ProgBrowse, "Browse for the program file")
			Me.ProgBrowse.Location = New System.Drawing.Point(192, 80)
			Me.ProgBrowse.Name = "ProgBrowse"
			Me.ProgrammerEditorHelp.SetShowHelp(Me.ProgBrowse, true)
			Me.ProgBrowse.Size = New System.Drawing.Size(72, 24)
			Me.ProgBrowse.TabIndex = 7
			Me.ProgBrowse.Text = "Browse..."
			AddHandler Me.ProgBrowse.Click, AddressOf Me.ProgBrowseClick
			'
			'ProgrammerName
			'
			Me.ProgrammerName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
									Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.ProgrammerEditorHelp.SetHelpString(Me.ProgrammerName, "The name of the programmer or external tool. This will be shown in the list of pr"& _ 
						"ogrammers or tools")
			Me.ProgrammerName.Location = New System.Drawing.Point(8, 24)
			Me.ProgrammerName.Name = "ProgrammerName"
			Me.ProgrammerEditorHelp.SetShowHelp(Me.ProgrammerName, true)
			Me.ProgrammerName.Size = New System.Drawing.Size(256, 20)
			Me.ProgrammerName.TabIndex = 1
			'
			'portLabel
			'
			Me.portLabel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
									Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.portLabel.Location = New System.Drawing.Point(8, 224)
			Me.portLabel.Name = "portLabel"
			Me.portLabel.Size = New System.Drawing.Size(256, 16)
			Me.portLabel.TabIndex = 16
			Me.portLabel.Text = "Port:"
			Me.portLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
			'
			'portList
			'
			Me.portList.FormattingEnabled = true
			Me.ProgrammerEditorHelp.SetHelpString(Me.portList, "Serial port used by the programmer. If %port% is used in the Command Line Paramet"& _ 
						"ers, this box will display a list of serial ports. The selected port will be use"& _ 
						"d in place of %port% above.")
			Me.portList.Location = New System.Drawing.Point(8, 240)
			Me.portList.Name = "portList"
			Me.ProgrammerEditorHelp.SetShowHelp(Me.portList, true)
			Me.portList.Size = New System.Drawing.Size(120, 21)
			Me.portList.TabIndex = 17
			'
			'ProgrammerEditor
			'
			Me.AcceptButton = Me.Button_OK
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.CancelButton = Me.Button_Cancel
			Me.ClientSize = New System.Drawing.Size(272, 304)
			Me.Controls.Add(Me.portList)
			Me.Controls.Add(Me.portLabel)
			Me.Controls.Add(Me.browseWorkingDir)
			Me.Controls.Add(Me.ProgrammerWorkingDir)
			Me.Controls.Add(Me.workingDirLabel)
			Me.Controls.Add(Me.cmdLineLabel)
			Me.Controls.Add(Me.fileLabel)
			Me.Controls.Add(Me.nameLabel)
			Me.Controls.Add(Me.Button_Cancel)
			Me.Controls.Add(Me.Button_OK)
			Me.Controls.Add(Me.ProgrammerName)
			Me.Controls.Add(Me.ProgBrowse)
			Me.Controls.Add(Me.ProgrammerFileName)
			Me.Controls.Add(Me.ProgrammerParams)
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
			Me.HelpButton = true
			Me.MaximizeBox = false
			Me.MinimizeBox = false
			Me.Name = "ProgrammerEditor"
			Me.ShowInTaskbar = false
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
			Me.Text = "Programmer Editor"
			Me.ResumeLayout(false)
			Me.PerformLayout
		End Sub
		Private portList As System.Windows.Forms.ComboBox
		Private portLabel As System.Windows.Forms.Label
		#End Region
		
		Private Sub ApplyTranslation
			With MainForm.Translator
				If .CanTranslate Then
					
					'Get source for text, depending on current mode
					Dim TextSource As String
					If pMode = Modes.TOOL Then
						TextSource = "ToolEditor"
					Else
						TextSource = "ProgrammerEditor"
					End If
					
					'Set title
					.TryTranslate(TextSource, "Title", Me.Text)
					
					'Set labels and Browse button text
					.TryTranslate("ProgrammerEditor", "nameLabel", nameLabel.Text)
					.TryTranslate("ProgrammerEditor", "fileLabel", fileLabel.Text)
					.TryTranslate("ProgrammerEditor", "ProgBrowse", ProgBrowse.Text)
					.TryTranslate("ProgrammerEditor", "cmdLineLabel", cmdLineLabel.Text)
					
					.TryTranslate("ProgrammerEditor", "ProgBrowse", browseWorkingDir.Text)
					.TryTranslate("ProgrammerEditor", "workingDirLabel", workingDirLabel.Text)
					
					'Set tooltips
					Dim newTip As String
					newTip = .GetMessage("ProgrammerEditor", "NameTip")
					If newTip <> "" Then Me.ProgrammerEditorHelp.SetHelpString(Me.ProgrammerName, newTip)
					newTip = .GetMessage("ProgrammerEditor", "fileTip")
					If newTip <> "" Then Me.ProgrammerEditorHelp.SetHelpString(Me.ProgrammerFileName, newTip)
					newTip = .GetMessage("ProgrammerEditor", "cmdLineTip")
					If newTip <> "" Then Me.ProgrammerEditorHelp.SetHelpString(Me.ProgrammerParams, newTip)
					newTip = .GetMessage("ProgrammerEditor", "browseTip")
					If newTip <> "" Then Me.ProgrammerEditorHelp.SetHelpString(Me.ProgBrowse, newTip)
					
					newTip = .GetMessage("ProgrammerEditor", "dirBrowseTip")
					If newTip <> "" Then Me.ProgrammerEditorHelp.SetHelpString(Me.browseWorkingDir, newTip)
					newTip = .GetMessage("ProgrammerEditor", "workingDirTip")
					If newTip <> "" Then Me.ProgrammerEditorHelp.SetHelpString(Me.ProgrammerWorkingDir, newTip)
					
					'File dialog settings
					Me.ProgrammerFile.Filter = .GetMessage("ProgrammerEditor", "fileDialogFilter", "Programs") + "|*.exe"
					.TryTranslate("ProgrammerEditor", "FileDialogTitle", Me.ProgrammerFile.Title)
					
					'Working dir dialog settings
					.TryTranslate("ProgrammerEditor", "dirBrowserDesc", Me.WorkingDirBrowser.Description)
					
					'Set OK/Cancel button text
					.TryTranslate("Misc", "Cancel", Button_Cancel.Text)
					.TryTranslate("Misc", "OK", Button_OK.Text)
					
				End If
			End With
		End Sub
		
		Public Property Source As Preferences
			Get
				Return pSource
			End Get
			Set
				pSource = Value
			End Set
		End Property
		
		Public Property Mode As Modes
			Get
				Return pMode
			End Get
			Set
				pMode = Value
				If pMode = Modes.PROGRAMMER Then
					
					
				Else If pMode = Modes.TOOL Then
					Me.Text = "External Tool Editor"
				End If
				UpdatePortEditor
			End Set
		End Property
		
		Public ReadOnly Property Section As SettingSection
			Get
				If changeAccepted Then
					Dim ProgID As String
					ProgID = pSource.GetProgrammerID(Me.ProgrammerFileName.Text)
					'If no section exists, create one with suitable name
					If pSection Is Nothing Then
						pSection = New SettingSection("tool = " + ProgID)
					'Otherwise, make sure that name is correct
					Else
						'pSection.Name = "tool = " + ProgID
					End If
					
					'Put updated settings into settings section and return
					With pSection
						.AddSetting("desc", ProgrammerName.Text)
						If pMode = Modes.PROGRAMMER Then
							.AddSetting("type", "programmer")
						Else
							.AddSetting("type", "external")
						End If
						.AddSetting("command", Me.ProgrammerFileName.Text)
						.AddSetting("params", Me.ProgrammerParams.Text)
						.AddSetting("workingdir", Me.ProgrammerWorkingDir.Text)
						
						If Me.portList.Text <> "" And Me.portList.Enabled Then
							.AddSetting("port", Me.portList.Text)
						Else
							.RemoveSetting("port")
						End If
					End With
				End If
				
				Return pSection
			End Get
		End Property
		
		Public ReadOnly Property NameChanged As Boolean
			Get
				Return pNameChanged
			End Get
		End Property
		
		Public ReadOnly Property ProgName As String
			Get
				Return ProgrammerName.Text
			End Get
		End Property
		
		Private Sub Button_OKClick(sender As System.Object, e As System.EventArgs)
			
			'Validate input
			'Make sure that at a minimum, a programmer name and executable have been given
			If Me.ProgrammerFileName.Text = "" Or ProgrammerName.Text = "" Then
				If pMode = Modes.PROGRAMMER Then
					MessageBox.Show(Mainform.Translator.GetMessage("ProgrammerEditor", "MissingName", "A name and filename must be entered for the programmer."), "Great Cow Graphical BASIC", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
				Else
					MessageBox.Show(Mainform.Translator.GetMessage("ToolEditor", "MissingName", "A name and filename must be entered for the external tool."), "Great Cow Graphical BASIC", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
				End If
				Exit Sub
			End If
			
			'Make sure that name is not a duplicate
			pNameChanged = True
			If Not pSection Is Nothing Then
				pNameChanged = ProgrammerName.Text.ToLower.Trim <> pSection.GetValue("desc").Trim.ToLower
			End If
			'Proceed if tool not found because name changed, or name not changed
			If Not pSource.GetToolSettings(ProgrammerName.Text) Is Nothing And pNameChanged Then
				If pMode = Modes.PROGRAMMER Then
					MessageBox.Show(Mainform.Translator.GetMessage("ProgrammerEditor", "DuplicateName", "A different programmer already exists with the same name. Please choose a new name."), "Great Cow Graphical BASIC", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
				Else
					MessageBox.Show(Mainform.Translator.GetMessage("ToolEditor", "DuplicateName", "A different tool already exists with the same name. Please choose a new name."), "Great Cow Graphical BASIC", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
				End If
				Exit Sub
			End If
			
			'Save
			changeAccepted = True
			Me.Close
		End Sub
		
		Private Sub ProgBrowseClick(sender As System.Object, e As System.EventArgs)
			Me.ProgrammerFile.ShowDialog()
			
		End Sub
		
		Private Sub ProgrammerFileFileOk(sender As System.Object, e As System.ComponentModel.CancelEventArgs)
			me.ProgrammerFileName.Text = Me.ProgrammerFile.FileName
			
		End Sub
		
		Public Function WasAccepted As Boolean
			Return changeAccepted
		End Function
		
		Private Sub BrowseWorkingDirClick(sender As System.Object, e As System.EventArgs)
			If WorkingDirBrowser.ShowDialog = DialogResult.OK Then
				ProgrammerWorkingDir.Text = WorkingDirBrowser.SelectedPath
			End If
		End Sub
		
		Private Sub FillPortEditor
			'Fill port editor with a list of COM ports
			Dim CurrPort As String
			For Each CurrPort In My.Computer.Ports.SerialPortNames
				portList.Items.Add(CurrPort)
			Next
		End Sub
		
		Private Sub UpdatePortEditor
			'Update display of port editor
			
			'If in programmer mode, show
			If pMode = Modes.PROGRAMMER Then
				Me.portLabel.Visible = True
				Me.portList.Visible = True
				'If a %port% is found in command line parameters, enable
				If ProgrammerParams.Text.ToLower.IndexOf("%port%") <> -1 Then
					Me.portList.Enabled = True
				Else
					Me.portList.Enabled = False
				End If
			'If not in programmer mode, don't show port selector at all
			Else
				Me.portLabel.Visible = False
				Me.portList.Visible = False
			End If
			
		End Sub
		
		Sub ProgrammerParamsTextChanged(sender As Object, e As EventArgs)
			UpdatePortEditor
		End Sub
	End Class
'End Namespace
