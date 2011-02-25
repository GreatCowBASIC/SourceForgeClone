'
' Created by SharpDevelop.
' User: Hugh
' Date: 21/04/2010
' Time: 6:25 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Collections

Namespace Great_Cow_Graphical_BASIC
	
	Public Class CustomHardwareSettings
		Inherits System.Windows.Forms.Form
		Private buttonAdd As System.Windows.Forms.Button
		Private buttonEdit As System.Windows.Forms.Button
		Private buttonOK As System.Windows.Forms.Button
		Private buttonDelete As System.Windows.Forms.Button
		Private labelSettings As System.Windows.Forms.Label
		Private buttonCancel As System.Windows.Forms.Button
		Private settingsList As System.Windows.Forms.ListBox
		
		Private InDevice As HardwareDevice
		Private Accepted As Boolean
		Private DeviceSettings As ArrayList
		
		Public Sub New()
			MyBase.New
			'
			' The Me.InitializeComponent call is required for Windows Forms designer support.
			'
			Me.InitializeComponent
			
			UpdateButtons
			Accepted = False
		End Sub
		
		Public Property Device As HardwareDevice
			Get
				If Accepted Then
					'If changes accepted, need to create new device and settings
					Dim OutDevice As New HardwareDevice
					If InDevice Is Nothing Then
						OutDevice.Name = ""
						OutDevice.CodeName = ""
					Else
						OutDevice.Name = InDevice.Name
						OutDevice.CodeName = InDevice.CodeName
					End If
					OutDevice.Settings = DeviceSettings
					Return OutDevice
					
				Else
					'If changes not accepted, return old
					Return InDevice
				End If
			End Get
			
			Set
				'Store device
				InDevice = Value
				
				'Clear any old settings
				DeviceSettings = New ArrayList
				SettingsList.Items.Clear
				Accepted = False
				
				'Load into display
				Dim CurrSetting As HardwareDeviceSetting
				If Not InDevice Is Nothing Then
					For Each CurrSetting In InDevice.Settings
						With CurrSetting
							
							'Clone setting
							Dim NewSetting As New HardwareDeviceSetting
							NewSetting.Name = .Name
							NewSetting.Description = .Description
							NewSetting.Constant = .Constant
							NewSetting.ParamType = .ParamType
							NewSetting.Condition = .Condition
							NewSetting.ControlsOther = .ControlsOther
							DeviceSettings.Add(NewSetting)
							
							'Display setting
							SettingsList.Items.Add(.Name)
							
						End With
					Next
				End If
				
			End Set
		End Property
		
		#Region " Windows Forms Designer generated code "
		' This method is required for Windows Forms designer support.
		' Do not change the method contents inside the source code editor. The Forms designer might
		' not be able to load this method if it was changed manually.
		Private Sub InitializeComponent()
			Me.settingsList = New System.Windows.Forms.ListBox
			Me.buttonCancel = New System.Windows.Forms.Button
			Me.labelSettings = New System.Windows.Forms.Label
			Me.buttonDelete = New System.Windows.Forms.Button
			Me.buttonOK = New System.Windows.Forms.Button
			Me.buttonEdit = New System.Windows.Forms.Button
			Me.buttonAdd = New System.Windows.Forms.Button
			Me.SuspendLayout
			'
			'settingsList
			'
			Me.settingsList.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.settingsList.IntegralHeight = false
			Me.settingsList.Location = New System.Drawing.Point(8, 24)
			Me.settingsList.Name = "settingsList"
			Me.settingsList.Size = New System.Drawing.Size(168, 224)
			Me.settingsList.TabIndex = 3
			AddHandler Me.settingsList.SelectedIndexChanged, AddressOf Me.SettingsListSelectedIndexChanged
			'
			'buttonCancel
			'
			Me.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom
			Me.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.buttonCancel.Location = New System.Drawing.Point(150, 260)
			Me.buttonCancel.Name = "buttonCancel"
			Me.buttonCancel.Size = New System.Drawing.Size(88, 24)
			Me.buttonCancel.TabIndex = 1
			Me.buttonCancel.Text = "Cancel"
			'
			'labelSettings
			'
			Me.labelSettings.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.labelSettings.Location = New System.Drawing.Point(8, 8)
			Me.labelSettings.Name = "labelSettings"
			Me.labelSettings.Size = New System.Drawing.Size(256, 16)
			Me.labelSettings.TabIndex = 2
			Me.labelSettings.Text = "Settings:"
			'
			'buttonDelete
			'
			Me.buttonDelete.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.buttonDelete.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.buttonDelete.Location = New System.Drawing.Point(184, 96)
			Me.buttonDelete.Name = "buttonDelete"
			Me.buttonDelete.Size = New System.Drawing.Size(80, 24)
			Me.buttonDelete.TabIndex = 6
			Me.buttonDelete.Text = "Delete ..."
			AddHandler Me.buttonDelete.Click, AddressOf Me.ButtonDeleteClick
			'
			'buttonOK
			'
			Me.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom
			Me.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.buttonOK.Location = New System.Drawing.Point(38, 260)
			Me.buttonOK.Name = "buttonOK"
			Me.buttonOK.Size = New System.Drawing.Size(88, 24)
			Me.buttonOK.TabIndex = 0
			Me.buttonOK.Text = "OK"
			AddHandler Me.buttonOK.Click, AddressOf Me.ButtonOKClick
			'
			'buttonEdit
			'
			Me.buttonEdit.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.buttonEdit.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.buttonEdit.Location = New System.Drawing.Point(184, 64)
			Me.buttonEdit.Name = "buttonEdit"
			Me.buttonEdit.Size = New System.Drawing.Size(80, 24)
			Me.buttonEdit.TabIndex = 5
			Me.buttonEdit.Text = "Edit ..."
			AddHandler Me.buttonEdit.Click, AddressOf Me.ButtonEditClick
			'
			'buttonAdd
			'
			Me.buttonAdd.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.buttonAdd.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.buttonAdd.Location = New System.Drawing.Point(184, 32)
			Me.buttonAdd.Name = "buttonAdd"
			Me.buttonAdd.Size = New System.Drawing.Size(80, 24)
			Me.buttonAdd.TabIndex = 4
			Me.buttonAdd.Text = "Add ..."
			AddHandler Me.buttonAdd.Click, AddressOf Me.ButtonAddClick
			'
			'CustomHardwareSettings
			'
			Me.AcceptButton = Me.buttonOK
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.CancelButton = Me.buttonCancel
			Me.ClientSize = New System.Drawing.Size(272, 294)
			Me.ControlBox = false
			Me.Controls.Add(Me.buttonDelete)
			Me.Controls.Add(Me.buttonEdit)
			Me.Controls.Add(Me.buttonAdd)
			Me.Controls.Add(Me.settingsList)
			Me.Controls.Add(Me.labelSettings)
			Me.Controls.Add(Me.buttonCancel)
			Me.Controls.Add(Me.buttonOK)
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
			Me.MaximizeBox = false
			Me.MinimizeBox = false
			Me.Name = "CustomHardwareSettings"
			Me.ShowInTaskbar = false
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
			Me.Text = "Custom Hardware Settings"
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
		Private Sub ButtonOKClick(sender As System.Object, e As System.EventArgs)
			Accepted = True
			Me.Close
		End Sub
		
		Private Sub SettingsListSelectedIndexChanged(sender As System.Object, e As System.EventArgs)
			UpdateButtons
			
		End Sub
		
		Private Sub UpdateButtons
			If SettingsList.SelectedIndex = -1 Then
				ButtonEdit.Enabled = False
				ButtonDelete.Enabled = False
			Else
				ButtonEdit.Enabled = True
				ButtonDelete.Enabled = True
			End If
		End Sub
		
		Private Sub ButtonAddClick(sender As System.Object, e As System.EventArgs)
			'Create settings editor
			Dim settingEditor As New HardwareSettingEditor
			
			'Generate list of other settings for editor
			Dim OtherSettings As New ArrayList
			Dim CurrSetting As HardwareDeviceSetting
			For Each CurrSetting In DeviceSettings
				OtherSettings.Add(CurrSetting.Constant)
			Next
			settingEditor.OtherSettings = OtherSettings
			
			'Show editor
			settingEditor.ShowDialog
			
			'Save new setting
			If Not settingEditor.Setting Is Nothing Then
				DeviceSettings.Add(settingEditor.Setting)
				SettingsList.Items.Add(settingEditor.Setting.Name)
			End If
			
		End Sub
		
		Private Sub ButtonEditClick(sender As System.Object, e As System.EventArgs)
			
			If SettingsList.SelectedIndex = -1 Then Exit Sub
			
			'Create settings editor
			Dim settingEditor As New HardwareSettingEditor
			
			'Generate list of other settings for editor
			Dim OtherSettings As New ArrayList
			Dim CurrSetting As HardwareDeviceSetting
			For Each CurrSetting In DeviceSettings
				If CurrSetting.Name <> SettingsList.SelectedItem Then
					OtherSettings.Add(CurrSetting.Constant)
				End If
			Next
			settingEditor.OtherSettings = OtherSettings
			
			'Load existing setting, show editor
			settingEditor.Setting = DeviceSettings(SettingsList.SelectedIndex)
			settingEditor.ShowDialog
			
			'Save new setting value
			If Not settingEditor.Setting Is Nothing Then
				DeviceSettings(SettingsList.SelectedIndex) = settingEditor.Setting
				SettingsList.Items(SettingsList.SelectedIndex) = settingEditor.Setting.Name
			End If
			
		End Sub
		
		Private Sub ButtonDeleteClick(sender As System.Object, e As System.EventArgs)
			If SettingsList.SelectedIndex <> -1 Then
				DeviceSettings.RemoveAt(SettingsList.SelectedIndex)
				SettingsList.Items.Remove(SettingsList.SelectedItem)
			End If
		End Sub
		
	End Class
End Namespace
