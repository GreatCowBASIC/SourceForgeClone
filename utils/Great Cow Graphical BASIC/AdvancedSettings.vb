'
' Created by SharpDevelop.
' User: Hugh
' Date: 26/01/2010
' Time: 1:39 AM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.Windows.Forms

''Namespace Great_Cow_Graphical_BASIC
	
	Public Class AdvancedSettings
		Inherits System.Windows.Forms.Form
		Private startupLabel As System.Windows.Forms.Label
		Private buttonHardwareSettings As System.Windows.Forms.Button
		Private startLoc As System.Windows.Forms.TextBox
		Private bootloaderGroup As System.Windows.Forms.GroupBox
		Private locPromptLabel As System.Windows.Forms.Label
		Private startupSubSelect As System.Windows.Forms.ComboBox
		Private buttonOK As System.Windows.Forms.Button
		Private useBootloader As System.Windows.Forms.CheckBox
		Private buttonCancel As System.Windows.Forms.Button
		Private contextHelpProvider As System.Windows.Forms.HelpProvider
		
		Private Dim CurrProgram As GCBProgram
		
		Private Dim Device As HardwareDevice
		
		Public Sub New(CurrProgram As GCBProgram)
			MyBase.New
			'
			' The Me.InitializeComponent call is required for Windows Forms designer support.
			'
			Me.InitializeComponent
			
			'Set program
			Me.CurrProgram = CurrProgram
			
			'Fill sub list
			'Add all subroutines in the program to the list
			Dim NoStartupSub As String = "No startup subroutine"
			If Not MainForm.Translator Is Nothing Then
				MainForm.Translator.TryTranslate("AdvancedSettings", "NoStartupSub", NoStartupSub)
			End If
			startupSubSelect.Items.Add(NoStartupSub)
			startupSubSelect.SelectedIndex = 0
			Dim Subroutine As GCBSubroutine
			For Each Subroutine In CurrProgram.Subroutines
				If Not CurrProgram.IsMainSub(Subroutine) And Subroutine.IsSub Then
					If Subroutine.Name <> "" And Subroutine.Parameters.Count = 0 Then
						 startupSubSelect.Items.Add(Subroutine.Name)
					End If
				End If
			Next
			
			'Show current settings
			useBootloader.Checked = CurrProgram.UseBootloader
			startLoc.Text = CurrProgram.BootloaderPos
			OtherOptions.Text = CurrProgram.OtherOptions
			'Select appropriate startup sub option
			Dim checkItem As Integer
			For checkItem = 0 To startupSubSelect.Items.Count - 1
				If startupSubSelect.Items(checkItem).ToLower.Trim = CurrProgram.StartupSub.ToLower.Trim Then
					startupSubSelect.SelectedIndex = checkItem
					Exit For
				End If
			Next
			
			Device = CurrProgram.Device
			
			ApplyTranslation
			
		End Sub
		
		#Region " Windows Forms Designer generated code "
		' This method is required for Windows Forms designer support.
		' Do not change the method contents inside the source code editor. The Forms designer might
		' not be able to load this method if it was changed manually.
		Private Sub InitializeComponent()
			Me.contextHelpProvider = New System.Windows.Forms.HelpProvider
			Me.useBootloader = New System.Windows.Forms.CheckBox
			Me.startupSubSelect = New System.Windows.Forms.ComboBox
			Me.startLoc = New System.Windows.Forms.TextBox
			Me.OtherOptions = New System.Windows.Forms.TextBox
			Me.buttonCancel = New System.Windows.Forms.Button
			Me.buttonOK = New System.Windows.Forms.Button
			Me.locPromptLabel = New System.Windows.Forms.Label
			Me.bootloaderGroup = New System.Windows.Forms.GroupBox
			Me.buttonHardwareSettings = New System.Windows.Forms.Button
			Me.startupLabel = New System.Windows.Forms.Label
			Me.optionsLabel = New System.Windows.Forms.Label
			Me.bootloaderGroup.SuspendLayout
			Me.SuspendLayout
			'
			'useBootloader
			'
			Me.contextHelpProvider.SetHelpString(Me.useBootloader, "Move the reset vector.")
			Me.useBootloader.Location = New System.Drawing.Point(8, 24)
			Me.useBootloader.Name = "useBootloader"
			Me.contextHelpProvider.SetShowHelp(Me.useBootloader, true)
			Me.useBootloader.Size = New System.Drawing.Size(200, 24)
			Me.useBootloader.TabIndex = 0
			Me.useBootloader.Text = "Move code start location"
			AddHandler Me.useBootloader.CheckedChanged, AddressOf Me.UseBootloaderCheckedChanged
			'
			'startupSubSelect
			'
			Me.startupSubSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me.contextHelpProvider.SetHelpString(Me.startupSubSelect, "Use in libraries. If any subroutines, functions or constants from this file are u"& _ 
						"sed, then this subroutine will be run at the start of the program.")
			Me.startupSubSelect.Location = New System.Drawing.Point(8, 24)
			Me.startupSubSelect.Name = "startupSubSelect"
			Me.contextHelpProvider.SetShowHelp(Me.startupSubSelect, true)
			Me.startupSubSelect.Size = New System.Drawing.Size(216, 21)
			Me.startupSubSelect.TabIndex = 1
			'
			'startLoc
			'
			Me.startLoc.Enabled = false
			Me.contextHelpProvider.SetHelpString(Me.startLoc, "New location for the reset vector. The interrupt vector will follow, then the pro"& _ 
						"gram code.")
			Me.startLoc.Location = New System.Drawing.Point(136, 56)
			Me.startLoc.Name = "startLoc"
			Me.contextHelpProvider.SetShowHelp(Me.startLoc, true)
			Me.startLoc.Size = New System.Drawing.Size(72, 20)
			Me.startLoc.TabIndex = 2
			'
			'OtherOptions
			'
			Me.contextHelpProvider.SetHelpString(Me.OtherOptions, "Other options. Options are passed to GCBASIC #option directive. Use nolatch to di"& _ 
						"sable port latch use, or explicit to force variable declaration.")
			Me.OtherOptions.Location = New System.Drawing.Point(8, 168)
			Me.OtherOptions.Name = "OtherOptions"
			Me.contextHelpProvider.SetShowHelp(Me.OtherOptions, true)
			Me.OtherOptions.Size = New System.Drawing.Size(216, 20)
			Me.OtherOptions.TabIndex = 3
			'
			'buttonCancel
			'
			Me.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom
			Me.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.buttonCancel.Location = New System.Drawing.Point(120, 233)
			Me.buttonCancel.Name = "buttonCancel"
			Me.buttonCancel.Size = New System.Drawing.Size(96, 24)
			Me.buttonCancel.TabIndex = 4
			Me.buttonCancel.Text = "Cancel"
			'
			'buttonOK
			'
			Me.buttonOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
			Me.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.buttonOK.Location = New System.Drawing.Point(16, 233)
			Me.buttonOK.Name = "buttonOK"
			Me.buttonOK.Size = New System.Drawing.Size(96, 24)
			Me.buttonOK.TabIndex = 3
			Me.buttonOK.Text = "OK"
			AddHandler Me.buttonOK.Click, AddressOf Me.ButtonOKClick
			'
			'locPromptLabel
			'
			Me.locPromptLabel.Location = New System.Drawing.Point(8, 56)
			Me.locPromptLabel.Name = "locPromptLabel"
			Me.locPromptLabel.Size = New System.Drawing.Size(112, 24)
			Me.locPromptLabel.TabIndex = 1
			Me.locPromptLabel.Text = "New start address:"
			Me.locPromptLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
			'
			'bootloaderGroup
			'
			Me.bootloaderGroup.Controls.Add(Me.startLoc)
			Me.bootloaderGroup.Controls.Add(Me.locPromptLabel)
			Me.bootloaderGroup.Controls.Add(Me.useBootloader)
			Me.bootloaderGroup.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.bootloaderGroup.Location = New System.Drawing.Point(8, 56)
			Me.bootloaderGroup.Name = "bootloaderGroup"
			Me.bootloaderGroup.Size = New System.Drawing.Size(216, 88)
			Me.bootloaderGroup.TabIndex = 2
			Me.bootloaderGroup.TabStop = false
			Me.bootloaderGroup.Text = "Bootloader"
			'
			'buttonHardwareSettings
			'
			Me.buttonHardwareSettings.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.buttonHardwareSettings.Location = New System.Drawing.Point(8, 200)
			Me.buttonHardwareSettings.Name = "buttonHardwareSettings"
			Me.buttonHardwareSettings.Size = New System.Drawing.Size(216, 24)
			Me.buttonHardwareSettings.TabIndex = 5
			Me.buttonHardwareSettings.Text = "Edit custom Hardware Settings ..."
			AddHandler Me.buttonHardwareSettings.Click, AddressOf Me.ButtonHardwareSettingsClick
			'
			'startupLabel
			'
			Me.startupLabel.Location = New System.Drawing.Point(8, 8)
			Me.startupLabel.Name = "startupLabel"
			Me.startupLabel.Size = New System.Drawing.Size(208, 16)
			Me.startupLabel.TabIndex = 0
			Me.startupLabel.Text = "Startup subroutine:"
			'
			'optionsLabel
			'
			Me.optionsLabel.Location = New System.Drawing.Point(8, 152)
			Me.optionsLabel.Name = "optionsLabel"
			Me.optionsLabel.Size = New System.Drawing.Size(216, 16)
			Me.optionsLabel.TabIndex = 6
			Me.optionsLabel.Text = "Options"
			'
			'AdvancedSettings
			'
			Me.AcceptButton = Me.buttonOK
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.CancelButton = Me.buttonCancel
			Me.ClientSize = New System.Drawing.Size(234, 265)
			Me.Controls.Add(Me.OtherOptions)
			Me.Controls.Add(Me.optionsLabel)
			Me.Controls.Add(Me.buttonHardwareSettings)
			Me.Controls.Add(Me.buttonCancel)
			Me.Controls.Add(Me.buttonOK)
			Me.Controls.Add(Me.bootloaderGroup)
			Me.Controls.Add(Me.startupSubSelect)
			Me.Controls.Add(Me.startupLabel)
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
			Me.HelpButton = true
			Me.MaximizeBox = false
			Me.MinimizeBox = false
			Me.Name = "AdvancedSettings"
			Me.ShowInTaskbar = false
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
			Me.Text = "Advanced Settings"
			Me.bootloaderGroup.ResumeLayout(false)
			Me.bootloaderGroup.PerformLayout
			Me.ResumeLayout(false)
			Me.PerformLayout
		End Sub
		Private OtherOptions As System.Windows.Forms.TextBox
		Private optionsLabel As System.Windows.Forms.Label
		#End Region
		
		Private Sub ApplyTranslation
			If MainForm.Translator Is Nothing Then Exit Sub
			
			With MainForm.Translator
				.TryTranslate("AdvancedSettings", "Title", Me.Text)
				.TryTranslate("AdvancedSettings", "startupLabel", Me.startupLabel.Text)
				.TryTranslate("AdvancedSettings", "bootloaderGroup", Me.bootloaderGroup.Text)
				.TryTranslate("AdvancedSettings", "useBootloader", Me.useBootloader.Text)
				.TryTranslate("AdvancedSettings", "locPromptLabel", Me.locPromptLabel.Text)
				.TryTranslate("AdvancedSettings", "optionsLabel", Me.optionsLabel.Text)
				
				'Set OK/Cancel button text
				.TryTranslate("Misc", "Cancel", ButtonCancel.Text)
				.TryTranslate("Misc", "OK", ButtonOK.Text)
			End With
			
		End Sub
		
		Private Sub UseBootloaderCheckedChanged(sender As System.Object, e As System.EventArgs)
			
			Me.startLoc.Enabled = UseBootloader.Checked
			
		End Sub
		
		Private Sub ButtonOKClick(sender As System.Object, e As System.EventArgs)
			
			With CurrProgram
				'Apply changes
				If startupSubSelect.SelectedIndex > 0 Then
					.StartupSub = startupSubSelect.SelectedItem
				Else
					.StartupSub = ""
				End If
				
				.UseBootloader = useBootloader.Checked
				If .UseBootloader Then
					.BootloaderPos = startLoc.Text
				Else
					.BootloaderPos = ""
				End If
				
				.OtherOptions = OtherOptions.Text
				
				.Device = Device
				
				'Set changed flag
				.HasChanged = True
			End With	
			
			'Exit
			Me.Close
			
		End Sub
		
		Private Sub ButtonHardwareSettingsClick(sender As System.Object, e As System.EventArgs)
			Dim hardwareEditor As New CustomHardwareSettings
			hardwareEditor.Device = Device
			hardwareEditor.ShowDialog
			
			Device = hardwareEditor.Device
		End Sub
		
	End Class
''End Namespace
