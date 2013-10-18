'
' Created by SharpDevelop.
' User: Administrator
' Date: 12/09/2006
' Time: 8:52 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports System.IO
Imports System.Collections

Namespace Great_Cow_Graphical_BASIC
	
	Public Class ChipSettings
		Inherits System.Windows.Forms.Form
		Private ChipConfOK As System.Windows.Forms.Button
		Private EditChipSpeed As System.Windows.Forms.NumericUpDown
		Private label1 As System.Windows.Forms.Label
		Private AlterChipConfig As System.Windows.Forms.GroupBox
		Private EditChipModel As System.Windows.Forms.ComboBox
		Private DeviceSelect As System.Windows.Forms.ComboBox
		Private SelectChipModel As System.Windows.Forms.GroupBox
		Private DeviceSettingsGroup As System.Windows.Forms.GroupBox
		Private ChipSettingsPanel As System.Windows.Forms.Panel
		Private HardwareHelp As System.Windows.Forms.HelpProvider
		Private label2 As System.Windows.Forms.Label
		Private ChipConfCancel As System.Windows.Forms.Button
		Private HardwarePanel As System.Windows.Forms.Panel
		
		Const MaxConfig = 75
		
		Private ConfParamName(MaxConfig) As System.Windows.Forms.Label
		Private ConfParamValue(MaxConfig) As System.Windows.Forms.ComboBox
		Private ConfParamError As System.Windows.Forms.Label
		
		Private Dim HardwareSetting(MaxConfig) As ParamEditBox
		
		'Store list of all original constants
		'Change constants as settings are changed, but revert if cancel clicked
		Private Dim OldConstants As ArrayList
		
		Private Dim CurrentConfig(MaxConfig, 3) As String
		Private Dim ConfigCount As Integer = 0
		Private Dim ChipData As ChipInfo
		
		Private Dim ChipList As New ArrayList
		Private Dim ChipCount As Integer = 0
		
		Private Dim CurrentProgram As GCBProgram
		
		Public Sub New(ByRef CurrentProgram As GCBProgram)
			MyBase.New
			'
			' The Me.InitializeComponent call is required for Windows Forms designer support.
			'
			Me.InitializeComponent
			
			Me.CurrentProgram = CurrentProgram
			
			Dim Temp As Integer
			Dim ChipName As String
			Dim ClearData As Integer
			Dim DataTemp As String = ""
			Dim ConfigTemp As String = ""
			Dim ChipListTemp As New ArrayList
			Dim CurrentDevice As HardwareDevice
			
			'Get list of supported chips
			Dim ChipDataDir As New IO.DirectoryInfo(MainForm.InstallDir + Mainform.FilenameChipDir)
			Dim ChipNameList As IO.FileInfo() = ChipDataDir.GetFiles()
			Dim ChipFileName As IO.FileInfo			
			
			'Copy into list, then sort
			For Each ChipFileName In ChipNameList
				If ChipFileName.FullName.ToUpper.IndexOf("CORE") = -1 Then
					ChipCount += 1
					ChipList.Add(ChipFileName.FullName)
					If ChipFileName.Name.StartsWith("1") Then
						ChipName = ChipFileName.Name.ToUpper
					Else
						ChipName = ChipFileName.Name.ToLower
					End If
					ChipListTemp.Add(ChipName.Substring(0, ChipName.IndexOf(".")))
				End If
			Next
			
			ChipList.Sort
			ChipListTemp.Sort
			
			Dim ChipNameTemp As String
			For Each ChipNameTemp In ChipListTemp
				EditChipModel.Items.Add(ChipNameTemp)
			Next
			
			'Split up config line
			For ClearData = 0 To MaxConfig
				CurrentConfig(ClearData, 1) = ""
				CurrentConfig(ClearData, 2) = ""
			Next
			ConfigTemp = CurrentProgram.ChipConfig
			If ConfigTemp <> "" Then
				Do While ConfigTemp.IndexOf(",") <> -1
					ConfigCount += 1
					CurrentConfig(ConfigCount, 0) = ConfigTemp.Substring(0, ConfigTemp.IndexOf(",")).Trim.ToUpper
					ConfigTemp = ConfigTemp.Substring(ConfigTemp.IndexOf(",") + 1).Trim
				Loop
				ConfigCount += 1
				CurrentConfig(ConfigCount, 0) = ConfigTemp.Trim.ToUpper
			End If

			'Process config data
			For Temp = 1 To ConfigCount
				
				ConfigTemp = CurrentConfig(Temp, 0)
				
				If ConfigTemp.IndexOf("=") <> -1 Then
					CurrentConfig(Temp, 1) = ConfigTemp.Substring(0, ConfigTemp.IndexOf("=")).Trim
					CurrentConfig(Temp, 2) = ConfigTemp.Substring(ConfigTemp.IndexOf("=")+1).Trim
				End If
				
				If ConfigTemp.IndexOf("=") = -1 Then
					
			        CurrentConfig(Temp, 1) = ConfigTemp.Substring(0, ConfigTemp.IndexOf("_")).Trim
					CurrentConfig(Temp, 2) = ConfigTemp.Substring(ConfigTemp.IndexOf("_")+1).Trim
					
        			IF ConfigTemp.IndexOf("INTRC") <> -1 OR ConfigTemp.IndexOf("EXTRC") <> -1 OR ConfigTemp = "EXTCLK" THEN
            			CurrentConfig(Temp, 1) = "OSC"
            			CurrentConfig(Temp, 2) = ConfigTemp
            			ConfigTemp = ""
        			END IF
        			
        			IF CurrentConfig(Temp, 2) = "OSC" THEN
            			ConfigTemp = CurrentConfig(Temp, 2)
            			CurrentConfig(Temp, 2) = CurrentConfig(Temp, 1)
            			CurrentConfig(Temp, 1) = ConfigTemp
            			ConfigTemp = ""
        			END IF
        
        			IF ConfigTemp.IndexOf("OSC") <> -1 THEN
            			CurrentConfig(Temp, 1) = "OSC"
            			CurrentConfig(Temp, 2) = ConfigTemp
            			ConfigTemp = ""
        			END IF
        
        			IF CurrentConfig(Temp, 1) = "" AND CurrentConfig(Temp, 2).IndexOf("BOR") <> -1 THEN
            			CurrentConfig(Temp, 1) = "BORV"
            			CurrentConfig(Temp, 2).Replace("BOR", "s")
            			CurrentConfig(Temp, 2).Replace("V", "")
        			END IF
					
				End If								
			Next
			
			'Add existing data
			Me.EditChipModel.Text = CurrentProgram.GetCurrentChip
			Me.EditChipSpeed.Value = CurrentProgram.GetCurrentChipSpeed
			
			RedrawChipConfig
			
			'Copy old constants into backup list
			Dim currConst As Setting
			oldConstants = New ArrayList
			For Each currConst In CurrentProgram.Constants
				oldConstants.Add(New Setting(currConst.Name, currConst.Value))
			Next
			
		    'Add hardware config
		    Me.DeviceSelect.Items.Add("Select a device to edit ...")
		    For Each CurrentDevice In MainForm.HardwareConf
		    	Me.DeviceSelect.Items.Add(CurrentDevice.Name)
		    Next
		    Dim CurrLib As LibraryType
		    For Each CurrLib In CurrentProgram.Libraries
		    	If Not CurrLib.Device Is Nothing Then
		    		Me.DeviceSelect.Items.Add(CurrLib.Device.Name)
		    	End If
		    Next
		    Me.DeviceSelect.SelectedIndex = 0
			
			RedrawHardwareConfig
			
		End Sub
		
		#Region " Windows Forms Designer generated code "
		' This method is required for Windows Forms designer support.
		' Do not change the method contents inside the source code editor. The Forms designer might
		' not be able to load this method if it was changed manually.
		Private Sub InitializeComponent()
			Me.HardwarePanel = New System.Windows.Forms.Panel
			Me.ChipConfCancel = New System.Windows.Forms.Button
			Me.label2 = New System.Windows.Forms.Label
			Me.HardwareHelp = New System.Windows.Forms.HelpProvider
			Me.ChipSettingsPanel = New System.Windows.Forms.Panel
			Me.DeviceSettingsGroup = New System.Windows.Forms.GroupBox
			Me.SelectChipModel = New System.Windows.Forms.GroupBox
			Me.DeviceSelect = New System.Windows.Forms.ComboBox
			Me.EditChipModel = New System.Windows.Forms.ComboBox
			Me.AlterChipConfig = New System.Windows.Forms.GroupBox
			Me.label1 = New System.Windows.Forms.Label
			Me.EditChipSpeed = New System.Windows.Forms.NumericUpDown
			Me.ChipConfOK = New System.Windows.Forms.Button
			Me.DeviceSettingsGroup.SuspendLayout
			Me.SelectChipModel.SuspendLayout
			Me.AlterChipConfig.SuspendLayout
			CType(Me.EditChipSpeed,System.ComponentModel.ISupportInitialize).BeginInit
			Me.SuspendLayout
			'
			'HardwarePanel
			'
			Me.HardwarePanel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.HardwarePanel.AutoScroll = true
			Me.HardwarePanel.BackColor = System.Drawing.SystemColors.Window
			Me.HardwarePanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
			Me.HardwareHelp.SetHelpString(Me.HardwarePanel, "Edit these settings so that they correspond to the actual circuit. If a device is"& _ 
" not used, it does not have to be set up.")
			Me.HardwarePanel.Location = New System.Drawing.Point(8, 55)
			Me.HardwarePanel.Name = "HardwarePanel"
			Me.HardwareHelp.SetShowHelp(Me.HardwarePanel, true)
			Me.HardwarePanel.Size = New System.Drawing.Size(184, 257)
			Me.HardwarePanel.TabIndex = 1
			AddHandler Me.HardwarePanel.Click, AddressOf Me.HardwarePanelClick
			'
			'ChipConfCancel
			'
			Me.ChipConfCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
			Me.ChipConfCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me.ChipConfCancel.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.ChipConfCancel.Location = New System.Drawing.Point(112, 313)
			Me.ChipConfCancel.Name = "ChipConfCancel"
			Me.ChipConfCancel.Size = New System.Drawing.Size(88, 24)
			Me.ChipConfCancel.TabIndex = 4
			Me.ChipConfCancel.Text = "Cancel"
			'
			'label2
			'
			Me.label2.Location = New System.Drawing.Point(20, 48)
			Me.label2.Name = "label2"
			Me.label2.Size = New System.Drawing.Size(76, 16)
			Me.label2.TabIndex = 13
			Me.label2.Text = "Speed (MHz):"
			'
			'ChipSettingsPanel
			'
			Me.ChipSettingsPanel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
			Me.ChipSettingsPanel.AutoScroll = true
			Me.ChipSettingsPanel.BackColor = System.Drawing.SystemColors.Window
			Me.ChipSettingsPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
			Me.HardwareHelp.SetHelpString(Me.ChipSettingsPanel, "Internal settings in the microcontroller.")
			Me.ChipSettingsPanel.Location = New System.Drawing.Point(8, 17)
			Me.ChipSettingsPanel.Name = "ChipSettingsPanel"
			Me.HardwareHelp.SetShowHelp(Me.ChipSettingsPanel, true)
			Me.ChipSettingsPanel.Size = New System.Drawing.Size(184, 192)
			Me.ChipSettingsPanel.TabIndex = 0
			AddHandler Me.ChipSettingsPanel.MouseDown, AddressOf Me.ChipSettingsPanelMouseDown
			'
			'DeviceSettingsGroup
			'
			Me.DeviceSettingsGroup.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.DeviceSettingsGroup.Controls.Add(Me.HardwarePanel)
			Me.DeviceSettingsGroup.Controls.Add(Me.DeviceSelect)
			Me.DeviceSettingsGroup.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.DeviceSettingsGroup.Location = New System.Drawing.Point(224, 8)
			Me.DeviceSettingsGroup.Name = "DeviceSettingsGroup"
			Me.DeviceSettingsGroup.Size = New System.Drawing.Size(200, 321)
			Me.DeviceSettingsGroup.TabIndex = 13
			Me.DeviceSettingsGroup.TabStop = false
			Me.DeviceSettingsGroup.Text = "Device Settings"
			'
			'SelectChipModel
			'
			Me.SelectChipModel.Controls.Add(Me.EditChipModel)
			Me.SelectChipModel.Controls.Add(Me.EditChipSpeed)
			Me.SelectChipModel.Controls.Add(Me.label1)
			Me.SelectChipModel.Controls.Add(Me.label2)
			Me.SelectChipModel.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.SelectChipModel.Location = New System.Drawing.Point(8, 8)
			Me.SelectChipModel.Name = "SelectChipModel"
			Me.SelectChipModel.Size = New System.Drawing.Size(200, 72)
			Me.SelectChipModel.TabIndex = 11
			Me.SelectChipModel.TabStop = false
			Me.SelectChipModel.Text = "Chip Selection"
			'
			'DeviceSelect
			'
			Me.DeviceSelect.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.DeviceSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me.HardwareHelp.SetHelpString(Me.DeviceSelect, "The hardware item to set up")
			Me.DeviceSelect.Location = New System.Drawing.Point(8, 23)
			Me.DeviceSelect.MaxDropDownItems = 15
			Me.DeviceSelect.Name = "DeviceSelect"
			Me.HardwareHelp.SetShowHelp(Me.DeviceSelect, true)
			Me.DeviceSelect.Size = New System.Drawing.Size(184, 21)
			Me.DeviceSelect.TabIndex = 0
			AddHandler Me.DeviceSelect.SelectedIndexChanged, AddressOf Me.DeviceSelectSelectedIndexChanged
			'
			'EditChipModel
			'
			Me.HardwareHelp.SetHelpString(Me.EditChipModel, "The model of microcontroller that is to be programmed. This must be set before a "& _ 
"program is compiled.")
			Me.EditChipModel.Location = New System.Drawing.Point(96, 16)
			Me.EditChipModel.MaxDropDownItems = 18
			Me.EditChipModel.Name = "EditChipModel"
			Me.HardwareHelp.SetShowHelp(Me.EditChipModel, true)
			Me.EditChipModel.Size = New System.Drawing.Size(96, 21)
			Me.EditChipModel.TabIndex = 16
			Me.EditChipModel.Text = "comboBox1"
			AddHandler Me.EditChipModel.TextChanged, AddressOf Me.EditChipModelTextChanged
			AddHandler Me.EditChipModel.SelectedIndexChanged, AddressOf Me.EditChipModelSelectedIndexChanged
			'
			'AlterChipConfig
			'
			Me.AlterChipConfig.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
			Me.AlterChipConfig.Controls.Add(Me.ChipSettingsPanel)
			Me.AlterChipConfig.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.AlterChipConfig.Location = New System.Drawing.Point(8, 88)
			Me.AlterChipConfig.Name = "AlterChipConfig"
			Me.AlterChipConfig.Size = New System.Drawing.Size(200, 217)
			Me.AlterChipConfig.TabIndex = 12
			Me.AlterChipConfig.TabStop = false
			Me.AlterChipConfig.Text = "Advanced Chip Settings"
			'
			'label1
			'
			Me.label1.Location = New System.Drawing.Point(20, 20)
			Me.label1.Name = "label1"
			Me.label1.Size = New System.Drawing.Size(72, 16)
			Me.label1.TabIndex = 15
			Me.label1.Text = "Chip Model:"
			'
			'EditChipSpeed
			'
			Me.EditChipSpeed.DecimalPlaces = 3
			Me.HardwareHelp.SetHelpString(Me.EditChipSpeed, "Speed of the microcontroller.")
			Me.EditChipSpeed.Location = New System.Drawing.Point(96, 44)
			Me.EditChipSpeed.Maximum = New Decimal(New Integer() {64, 0, 0, 0})
			Me.EditChipSpeed.Name = "EditChipSpeed"
			Me.HardwareHelp.SetShowHelp(Me.EditChipSpeed, true)
			Me.EditChipSpeed.Size = New System.Drawing.Size(96, 20)
			Me.EditChipSpeed.TabIndex = 1
			'
			'ChipConfOK
			'
			Me.ChipConfOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
			Me.ChipConfOK.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.ChipConfOK.Location = New System.Drawing.Point(16, 313)
			Me.ChipConfOK.Name = "ChipConfOK"
			Me.ChipConfOK.Size = New System.Drawing.Size(88, 24)
			Me.ChipConfOK.TabIndex = 3
			Me.ChipConfOK.Text = "OK"
			AddHandler Me.ChipConfOK.Click, AddressOf Me.ChipConfOKClick
			'
			'ChipSettings
			'
			Me.AcceptButton = Me.ChipConfOK
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.CancelButton = Me.ChipConfCancel
			Me.ClientSize = New System.Drawing.Size(432, 342)
			Me.Controls.Add(Me.DeviceSettingsGroup)
			Me.Controls.Add(Me.ChipConfCancel)
			Me.Controls.Add(Me.ChipConfOK)
			Me.Controls.Add(Me.AlterChipConfig)
			Me.Controls.Add(Me.SelectChipModel)
			Me.HelpButton = true
			Me.MaximizeBox = false
			Me.MinimizeBox = false
			Me.Name = "ChipSettings"
			Me.ShowInTaskbar = false
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
			Me.Text = "Hardware Settings"
			AddHandler Closed, AddressOf Me.ChipSettingsClosed
			Me.DeviceSettingsGroup.ResumeLayout(false)
			Me.SelectChipModel.ResumeLayout(false)
			Me.AlterChipConfig.ResumeLayout(false)
			CType(Me.EditChipSpeed,System.ComponentModel.ISupportInitialize).EndInit
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
		Private Sub RedrawChipConfig
			Dim ClearData, Temp, AddData, SelectedConf, FindSub As Integer
			Dim NameTemp As String
			Dim SettingMatch As Boolean
			
			if Me.EditChipModel.Text = "" Then Exit Sub
			
			'Read list of config options for current chip if chip model changed
			If ChipData Is Nothing Then
				ChipData = New ChipInfo(EditChipModel.Text)
			Else
				If ChipData.ChipName <> EditChipModel.Text Then
					ChipData = New ChipInfo(EditChipModel.Text)
				End If
			End If
			
			'Remove old settings list
			Try
				Me.ChipSettingsPanel.Controls.Remove(Me.ConfParamError)
			Catch
					
			End Try
			For ClearData = 0 To MaxConfig
				Try
					RemoveHandler Me.ConfParamValue(ClearData).SelectedIndexChanged, AddressOf Me.ChipConfigChanged
					Me.ChipSettingsPanel.Controls.Remove(Me.ConfParamName(ClearData))
					Me.ChipSettingsPanel.Controls.Remove(Me.ConfParamValue(ClearData))
				Catch
				
				End Try				
			Next
			
			'If no config for this chip, show an error
			If ChipData.ConfigSettings.Count = 0 Then
				Me.ConfParamError = New System.Windows.Forms.Label
				Me.ChipSettingsPanel.Controls.Add(Me.ConfParamError)
				Me.ConfParamError.BackColor = System.Drawing.Color.White
				Me.ConfParamError.Font = New System.Drawing.Font("Microsoft Sans Serif", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
				Me.ConfParamError.ForeColor = System.Drawing.Color.Black
				Me.ConfParamError.Location = New System.Drawing.Point(4, 4)
				'Me.ConfParamError.Name = IconHandler
				Me.ConfParamError.Size = New System.Drawing.Size(170, 130)
				Me.ConfParamError.TabIndex = 0
				Me.ConfParamError.TextAlign = System.Drawing.ContentAlignment.TopLeft
				
				If ChipData.NoConfig = 0 Then Me.ConfParamError.Text = "Great Cow Graphical BASIC could not find any config settings for this chip."
				If ChipData.NoConfig = 1 Then Me.ConfParamError.Text = "Great Cow Graphical BASIC cannot alter the config settings on this chip."
				If ChipData.NoConfig = 2 Then Me.ConfParamError.Text = "Great Cow Graphical BASIC cannot alter the config settings on this chip. Please use an external programmer utility to edit this chip's configuration."
			End If
			
			'Draw new settings
			Dim ConfigSetting As ChipConfigOption
			For Temp = 1 To ChipData.ConfigSettings.Count
				
				ConfigSetting = ChipData.ConfigSettings.Item(Temp - 1)
				
				'Parameter Name
				Me.ConfParamName(Temp) = New System.Windows.Forms.Label
				Me.ChipSettingsPanel.Controls.Add(Me.ConfParamName(Temp))
				With Me.ConfParamName(Temp)
					.BackColor = System.Drawing.Color.White
					.Font = New System.Drawing.Font("Microsoft Sans Serif", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
					.ForeColor = System.Drawing.Color.Black
					.Location = New System.Drawing.Point(4, 4+(Temp-1)*30)
					'.Name = IconHandler
					.Size = New System.Drawing.Size(55, 20)
					.TabIndex = 0
					.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
					.Text = ConfigSetting.Name
				End With
				
				'Parameter value
				Me.ConfParamValue(Temp) = New System.Windows.Forms.ComboBox
				Me.ChipSettingsPanel.Controls.Add(Me.ConfParamValue(Temp))
				With Me.ConfParamValue(Temp)
					.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
					.Location = New System.Drawing.Point(60, 4 + (Temp - 1) * 30)
					.MaxDropDownItems = 10
					.Items.Clear
					.Size = New System.Drawing.Size(100, 20)
					.TabIndex = 0
					.Name = Temp.ToString + "," + ConfigSetting.Name
					.Items.Add("Default")
					For AddData = 1 To ConfigSetting.Options.Length
						If ConfigSetting.Options(AddData - 1) <> "" Then .Items.Add(ConfigSetting.Options(AddData - 1))
					Next
				End With
				
				SelectedConf = 0
				For AddData = 1 To MaxConfig
					NameTemp = CurrentConfig(AddData, 1).ToUpper
					SettingMatch = False
					
					If NameTemp <> "" Then
						If NameTemp = ConfigSetting.Name.ToUpper Then SettingMatch = True
						If NameTemp = "MCLR" And ConfigSetting.Name.ToUpper = "MCLRE" Then SettingMatch = True
						If NameTemp = "MCLRE" AND ConfigSetting.Name.ToUpper = "MCLR" Then SettingMatch = True
						If NameTemp = "BOREN" And ConfigSetting.Name.ToUpper = "BODEN" Then SettingMatch = True
						If NameTemp = "BODEN" And ConfigSetting.Name.ToUpper = "BOREN" Then SettingMatch = True
					End If
					
					If SettingMatch = True Then
						Me.ConfParamValue(Temp).Name += ":" + NameTemp
						For FindSub = 1 To ConfigSetting.Options.Length
							If CurrentConfig(AddData, 2).Trim.ToUpper = ConfigSetting.Options(FindSub - 1) Then SelectedConf = FindSub: Exit For
						Next
						
						Exit For
					End If
				Next
				
				Me.ConfParamValue(Temp).SelectedIndex = SelectedConf
				
				AddHandler Me.ConfParamValue(Temp).SelectedIndexChanged, AddressOf Me.ChipConfigChanged
				
			Next
			
			'If chip model has changed, will need to redraw hardware config also
			StoreHardwareShadow
			RedrawHardwareConfig
			
		End Sub
		
		Private Sub RedrawHardwareConfig
			Dim Temp, CurrentHardware, ValueCount, SearchConst As Integer
			Dim ConfCount As Integer = 0
			Dim DrawThisItem As Boolean
			Dim CompareConst, TempData, ItemValue, OriginalChip As String
			Dim ValidValues(20) As String
			Dim FoundConst As Boolean = False
			Dim CurrentDevice As HardwareDevice
			Dim CurrentDeviceSetting As HardwareDeviceSetting
			
			'Store old settings in shadow array
			StoreHardwareShadow
			
			'Remove old config
			For Temp = 1 To MaxConfig
				Try
					RemoveHandler Me.HardwareSetting(Temp).ParamChanged, AddressOf HardwareSettingParamChanged
					RemoveHandler Me.HardwareSetting(Temp).ParamChanged, AddressOf HardwareSettingParamChangedRedraw
					Me.HardwarePanel.Controls.Remove(Me.HardwareSetting(Temp))
					
				Catch
				End Try
			Next
			Me.HardwarePanel.Controls.Clear
			
			'Load data for currently selected chip
			With CurrentProgram
				OriginalChip = .ChipName
				.ChipName = EditChipModel.Text
				.CheckChipData
			End With
			
		    'Draw new config
		    If Me.DeviceSelect.SelectedIndex <> -1 Then
			    
			    'CurrentDevice = MainForm.HardwareConf(Me.DeviceSelect.SelectedIndex)
			    'Get current device
			    CurrentDevice = Nothing
			    If Me.DeviceSelect.SelectedIndex > MainForm.HardwareConf.Count Then
			    	
			    	Dim CurrLib As LibraryType
			    	For Each CurrLib In CurrentProgram.Libraries
			    		If Not CurrLib.Device Is Nothing Then
			    			If CurrLib.Device.Name = Me.DeviceSelect.SelectedItem Then
			    				CurrentDevice = CurrLib.Device
			    				Exit For
			    			End If
			    		End If
			    	Next
			    Else If Me.DeviceSelect.SelectedIndex <> 0 Then
			    	CurrentDevice = MainForm.HardwareConf(Me.DeviceSelect.SelectedIndex - 1)
			    End If
			    
			    'Draw settings for device
				If Not CurrentDevice Is Nothing Then
					For Each CurrentDeviceSetting In CurrentDevice.Settings
						
						'Should setting be shown?
						DrawThisItem = False
						ValueCount = 0
						
						TempData = CurrentDeviceSetting.Condition.ToLower
						If TempData = "all" Then DrawThisItem = True
						If Not DrawThisItem Then
							'Get const and possible values
							CompareConst = TempData.Substring(0, TempData.IndexOf(" "))
							TempData = TempData.Substring(TempData.IndexOf(" "))
							Do While TempData.IndexOf(",") <> -1
								ValueCount += 1
								ValidValues(ValueCount) = TempData.Substring(0, TempData.IndexOf(",")).Trim
								TempData = TempData.Substring(TempData.IndexOf(",") + 1)
							Loop
							ValueCount += 1
							ValidValues(ValueCount) = TempData.Trim
							FoundConst = False
							
							'Get const value
							Dim currConst As Setting
							For Each currConst In CurrentProgram.Constants
								If CompareConst = currConst.Name.Trim.ToLower Then
									CompareConst = currConst.Value.Trim.ToLower
									Exit For
								End If
							Next
							
							'Compare constant
							For SearchConst = 1 To ValueCount
								If CompareConst = ValidValues(SearchConst).ToLower Then DrawThisItem = True: Exit For
							Next
						End If
						
						If DrawThisItem Then
							ConfCount += 1
							
							'Get value
							ItemValue = ""
							CompareConst = CurrentDeviceSetting.Constant.Trim.ToLower
							Dim currConst As Setting
							For Each currConst In CurrentProgram.Constants
								If CompareConst = currConst.Name.Trim.ToLower Then
									ItemValue = currConst.Value.Trim
									Exit For
								End If
							Next
							
							'Parameter
							Me.HardwareSetting(ConfCount) = New ParamEditBox
							Me.HardwareSetting(ConfCount).Init(CurrentProgram, Nothing, CurrentDeviceSetting.Name, ItemValue, CurrentDeviceSetting.ParamType, "Add description", True, CurrentDeviceSetting.Constant)
							Me.HardwareSetting(ConfCount).HasDescription = False
							Me.HardwarePanel.Controls.Add(Me.HardwareSetting(ConfCount))
							With Me.HardwareSetting(ConfCount)
								.Name = CurrentDeviceSetting.Constant
								.Location = New System.Drawing.Point(4, 4 + (ConfCount - 1) * 60) 'Was * 100 with description
							End With
							
							If CurrentDeviceSetting.ControlsOther Then
								AddHandler Me.HardwareSetting(ConfCount).ParamChanged, AddressOf HardwareSettingParamChangedRedraw
							Else
								AddHandler Me.HardwareSetting(ConfCount).ParamChanged, AddressOf HardwareSettingParamChanged
							End If
							
						End If
						
					Next
				End If
				
			End If
			
			'Revert to original chip
			CurrentProgram.ChipName = OriginalChip
			
		End Sub
		
		Private Sub HardwareSettingParamChangedRedraw
			StoreHardwareShadow
			RedrawHardwareConfig
		End Sub
		
		Private Sub HardwareSettingParamChanged
			StoreHardwareShadow
		End Sub
		
		Private Sub StoreHardwareShadow
			'Copy settings out of window and into settings shadow area
			'Dim Temp, SearchList, FoundLoc As Integer
			Dim Temp As Integer
			Dim NameTemp, ValueTemp As String
			Dim searchConst, thisConst As Setting
			
			For Temp = 1 To MaxConfig
				Try
					NameTemp = Me.HardwareSetting(Temp).Name
					ValueTemp = Me.HardwareSetting(Temp).GetValue
					
					thisConst = Nothing
					For Each searchConst In CurrentProgram.Constants
						If NameTemp.ToLower = searchConst.Name.Trim.ToLower Then
							thisConst = searchConst
							Exit For
						End If
					Next
					If thisConst Is Nothing Then
						If valueTemp <> "" Then
							thisConst = New Setting(NameTemp, ValueTemp)
							CurrentProgram.Constants.Add(thisConst)
						End If
					Else
						If valueTemp <> "" Then
							thisConst.Name = NameTemp
							thisConst.Value = ValueTemp
						Else
							CurrentProgram.Constants.Remove(thisConst)
						End If
					End If
					
				Catch
				End Try
			Next
		End Sub
		
		Private Sub ChipConfOKClick(sender As System.Object, e As System.EventArgs)
			Dim NewConfig As String = ""
			Dim Temp, CheckValid, FoundLoc, SearchConst As Integer
			Dim NameTemp, ValueTemp As String
			Dim IsValid As Boolean = False
			
			With CurrentProgram
				
				'Replace stored constants with new ones
				oldConstants = .Constants
				
				'Chip model and speed
				.ChipName = ""
				.ChipSpeed = 0
				If CurrentProgram.GetCurrentChip.ToUpper <> EditChipModel.Text.Trim.ToUpper Or EditChipSpeed.Value <> CurrentProgram.GetCurrentChipSpeed Then
					.ChipName = EditChipModel.Text
					.ChipSpeed = EditChipSpeed.Value
				End If
				
				'If chip is invalid, ChipData will exist with error flag set
				If Not .ChipData Is Nothing Then
					'Clear name to force reload
					.ChipData.ChipName = ""
					
					'Clear flag to re-enable error
					.ChipData.ChipNotValid = False
				End If
				
				'Verify that loaded chip data file matches current chip
				.CheckChipData
				
				'If chip is invalid, ChipData will exist with error flag set
				If Not .ChipData Is Nothing Then
					If .ChipData.ChipNotValid Then
						'Cancel close
						Exit Sub
					End If
				End If
				
				'Chip config
				If Not ChipData Is Nothing Then
					For Temp = 1 To ChipData.ConfigSettings.Count
						If ConfParamName(Temp).Text <> "" And ConfParamValue(Temp).Text <> "" And ConfParamValue(Temp).Text.ToLower <> "default" Then
							NewConfig = NewConfig + ", " + ConfParamName(Temp).Text + "=" + ConfParamValue(Temp).Text
						End If
					Next
				End If
				
				If NewConfig <> "" Then NewConfig = NewConfig.Substring(2).Trim
				.ChipConfig = NewConfig
				
				'Device settings
				StoreHardwareShadow
				
				'Set changed flag and exit
				.HasChanged = True
			End With
			
			Me.Close
		End Sub
		
		Private Sub EditChipModelSelectedIndexChanged(sender As System.Object, e As System.EventArgs)
			RedrawChipConfig
			
		End Sub
		
		Private Sub ChipSettingsPanelMouseEnter(sender As System.Object, e As System.EventArgs)
			Me.ChipSettingsPanel.Select
		End Sub
		
		Private Sub ChipConfigChanged(sender As System.Object, e As System.EventArgs)
			'MessageBox.Show(sender.Name, "Message", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1)
			Dim OldName As String = ""
			Dim NewName As String = ""
			Dim TempData As String = ""
			Dim DataLocation As Integer = 0
			Dim Temp As Integer = 0
			
			NewName = Sender.Name
			DataLocation = NewName.Substring(0, NewName.IndexOf(",")).Trim
			NewName = NewName.Substring(NewName.IndexOf(",") + 1).Trim
			
			If NewName.IndexOf(":") <> -1 Then
				OldName = NewName.Substring(NewName.IndexOf(":") + 1).Trim
				NewName = NewName.Substring(0, NewName.IndexOf(":")).Trim
			End If
			
			If OldName = "" Then
				ConfigCount += 1
				CurrentConfig(ConfigCount, 1) = NewName
				CurrentConfig(ConfigCount, 2) = Me.ConfParamValue(DataLocation).Text
			End If
			If OldName <> "" Then
				For Temp = 1 To ConfigCount
					If OldName.ToUpper = CurrentConfig(Temp, 1).ToUpper.Trim Then
						CurrentConfig(Temp, 1) = NewName
						CurrentConfig(Temp, 2) = Me.ConfParamValue(DataLocation).Text
						'MessageBox.Show(Me.ConfParamValue(DataLocation).Text, "Message", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1)
						Exit For
					End If
				Next
			End If
			
		End Sub
		
		Private Sub DeviceSelectSelectedIndexChanged(sender As System.Object, e As System.EventArgs)
			RedrawHardwareConfig
			Me.HardwarePanel.Select
		End Sub
		
		Private Sub HardwarePanelClick(sender As System.Object, e As System.EventArgs)
			Me.HardwarePanel.Select()
		End Sub
		
		Private Sub ChipSettingsPanelMouseDown(sender As System.Object, e As System.Windows.Forms.MouseEventArgs)
			Me.ChipSettingsPanel.Select
			
		End Sub
		
		Private Sub EditChipModelTextChanged(sender As System.Object, e As System.EventArgs)
			'If there is a complete chip model entered, show its settings
			Dim ItemFound As Boolean
			Dim SearchChip As String
			
			'Search in list of chips
			ItemFound = False
			For Each SearchChip In EditChipModel.Items
				If SearchChip.ToUpper = EditChipModel.Text.ToUpper Then
					ItemFound = True
					Exit For
				End If
			Next
			
			'If found, redraw
			If ItemFound Then
				RedrawChipConfig
			End If
			
		End Sub
		
		Private Sub ChipSettingsClosed(sender As System.Object, e As System.EventArgs)
			'Revert to saved constants
			CurrentProgram.Constants = oldConstants
		End Sub
		
	End Class
End Namespace
