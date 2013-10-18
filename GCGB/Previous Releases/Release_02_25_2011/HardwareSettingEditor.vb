'
' Created by SharpDevelop.
' User: Hugh
' Date: 21/04/2010
' Time: 9:41 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Collections

Namespace Great_Cow_Graphical_BASIC
	
	Public Class HardwareSettingEditor
		Inherits System.Windows.Forms.Form
		Private allowAny As System.Windows.Forms.RadioButton
		Private typesEditor As System.Windows.Forms.TextBox
		Private otherSettingsList As System.Windows.Forms.ComboBox
		Private labelConst As System.Windows.Forms.Label
		Private typesHelp As System.Windows.Forms.TextBox
		Private conditionValues As System.Windows.Forms.TextBox
		Private labelName As System.Windows.Forms.Label
		Private showCondition As System.Windows.Forms.RadioButton
		Private nameEditor As System.Windows.Forms.TextBox
		Private allowCustom As System.Windows.Forms.RadioButton
		Private rangeMax As System.Windows.Forms.NumericUpDown
		Private constEditor As System.Windows.Forms.TextBox
		Private groupShow As System.Windows.Forms.GroupBox
		Private labelTo As System.Windows.Forms.Label
		Private labelEquals As System.Windows.Forms.Label
		Private allowAnyNum As System.Windows.Forms.RadioButton
		Private allowRange As System.Windows.Forms.RadioButton
		Private groupTypes As System.Windows.Forms.GroupBox
		Private buttonOK As System.Windows.Forms.Button
		Private allowAnyVar As System.Windows.Forms.RadioButton
		Private buttonCancel As System.Windows.Forms.Button
		Private showAlways As System.Windows.Forms.RadioButton
		Private allowIO As System.Windows.Forms.RadioButton
		Private rangeMin As System.Windows.Forms.NumericUpDown
		
		Private InSetting As HardwareDeviceSetting
		Private Accepted As Boolean
		
		Public Sub New()
			MyBase.New
			'
			' The Me.InitializeComponent call is required for Windows Forms designer support.
			'
			Me.InitializeComponent
			
			InSetting = Nothing
			
		End Sub
		
		#Region " Windows Forms Designer generated code "
		' This method is required for Windows Forms designer support.
		' Do not change the method contents inside the source code editor. The Forms designer might
		' not be able to load this method if it was changed manually.
		Private Sub InitializeComponent()
			Me.rangeMin = New System.Windows.Forms.NumericUpDown
			Me.allowIO = New System.Windows.Forms.RadioButton
			Me.showAlways = New System.Windows.Forms.RadioButton
			Me.buttonCancel = New System.Windows.Forms.Button
			Me.allowAnyVar = New System.Windows.Forms.RadioButton
			Me.buttonOK = New System.Windows.Forms.Button
			Me.groupTypes = New System.Windows.Forms.GroupBox
			Me.allowRange = New System.Windows.Forms.RadioButton
			Me.allowAnyNum = New System.Windows.Forms.RadioButton
			Me.labelEquals = New System.Windows.Forms.Label
			Me.labelTo = New System.Windows.Forms.Label
			Me.groupShow = New System.Windows.Forms.GroupBox
			Me.constEditor = New System.Windows.Forms.TextBox
			Me.rangeMax = New System.Windows.Forms.NumericUpDown
			Me.allowCustom = New System.Windows.Forms.RadioButton
			Me.nameEditor = New System.Windows.Forms.TextBox
			Me.showCondition = New System.Windows.Forms.RadioButton
			Me.labelName = New System.Windows.Forms.Label
			Me.conditionValues = New System.Windows.Forms.TextBox
			Me.typesHelp = New System.Windows.Forms.TextBox
			Me.labelConst = New System.Windows.Forms.Label
			Me.otherSettingsList = New System.Windows.Forms.ComboBox
			Me.typesEditor = New System.Windows.Forms.TextBox
			Me.allowAny = New System.Windows.Forms.RadioButton
			CType(Me.rangeMin,System.ComponentModel.ISupportInitialize).BeginInit
			Me.groupTypes.SuspendLayout
			Me.groupShow.SuspendLayout
			CType(Me.rangeMax,System.ComponentModel.ISupportInitialize).BeginInit
			Me.SuspendLayout
			'
			'rangeMin
			'
			Me.rangeMin.Enabled = false
			Me.rangeMin.InterceptArrowKeys = false
			Me.rangeMin.Location = New System.Drawing.Point(112, 72)
			Me.rangeMin.Maximum = New Decimal(New Integer() {65535, 0, 0, 0})
			Me.rangeMin.Name = "rangeMin"
			Me.rangeMin.Size = New System.Drawing.Size(48, 20)
			Me.rangeMin.TabIndex = 6
			'
			'allowIO
			'
			Me.allowIO.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.allowIO.Location = New System.Drawing.Point(128, 24)
			Me.allowIO.Name = "allowIO"
			Me.allowIO.Size = New System.Drawing.Size(96, 16)
			Me.allowIO.TabIndex = 0
			Me.allowIO.Text = "IO Pin"
			'
			'showAlways
			'
			Me.showAlways.Checked = true
			Me.showAlways.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.showAlways.Location = New System.Drawing.Point(16, 16)
			Me.showAlways.Name = "showAlways"
			Me.showAlways.Size = New System.Drawing.Size(144, 24)
			Me.showAlways.TabIndex = 0
			Me.showAlways.TabStop = true
			Me.showAlways.Text = "Always"
			'
			'buttonCancel
			'
			Me.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.buttonCancel.Location = New System.Drawing.Point(232, 288)
			Me.buttonCancel.Name = "buttonCancel"
			Me.buttonCancel.Size = New System.Drawing.Size(96, 24)
			Me.buttonCancel.TabIndex = 6
			Me.buttonCancel.Text = "Cancel"
			'
			'allowAnyVar
			'
			Me.allowAnyVar.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.allowAnyVar.Location = New System.Drawing.Point(128, 48)
			Me.allowAnyVar.Name = "allowAnyVar"
			Me.allowAnyVar.Size = New System.Drawing.Size(80, 16)
			Me.allowAnyVar.TabIndex = 3
			Me.allowAnyVar.Text = "Any variable"
			'
			'buttonOK
			'
			Me.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.buttonOK.Location = New System.Drawing.Point(120, 288)
			Me.buttonOK.Name = "buttonOK"
			Me.buttonOK.Size = New System.Drawing.Size(96, 24)
			Me.buttonOK.TabIndex = 5
			Me.buttonOK.Text = "OK"
			AddHandler Me.buttonOK.Click, AddressOf Me.ButtonOKClick
			'
			'groupTypes
			'
			Me.groupTypes.Controls.Add(Me.typesHelp)
			Me.groupTypes.Controls.Add(Me.typesEditor)
			Me.groupTypes.Controls.Add(Me.rangeMax)
			Me.groupTypes.Controls.Add(Me.labelTo)
			Me.groupTypes.Controls.Add(Me.rangeMin)
			Me.groupTypes.Controls.Add(Me.allowCustom)
			Me.groupTypes.Controls.Add(Me.allowRange)
			Me.groupTypes.Controls.Add(Me.allowAnyVar)
			Me.groupTypes.Controls.Add(Me.allowAnyNum)
			Me.groupTypes.Controls.Add(Me.allowAny)
			Me.groupTypes.Controls.Add(Me.allowIO)
			Me.groupTypes.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.groupTypes.Location = New System.Drawing.Point(200, 8)
			Me.groupTypes.Name = "groupTypes"
			Me.groupTypes.Size = New System.Drawing.Size(248, 272)
			Me.groupTypes.TabIndex = 2
			Me.groupTypes.TabStop = false
			Me.groupTypes.Text = "Allowed Value Types"
			'
			'allowRange
			'
			Me.allowRange.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.allowRange.Location = New System.Drawing.Point(8, 72)
			Me.allowRange.Name = "allowRange"
			Me.allowRange.Size = New System.Drawing.Size(96, 16)
			Me.allowRange.TabIndex = 4
			Me.allowRange.Text = "Number from"
			AddHandler Me.allowRange.CheckedChanged, AddressOf Me.AllowRangeCheckedChanged
			'
			'allowAnyNum
			'
			Me.allowAnyNum.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.allowAnyNum.Location = New System.Drawing.Point(8, 48)
			Me.allowAnyNum.Name = "allowAnyNum"
			Me.allowAnyNum.Size = New System.Drawing.Size(104, 16)
			Me.allowAnyNum.TabIndex = 2
			Me.allowAnyNum.Text = "Any number"
			'
			'labelEquals
			'
			Me.labelEquals.Location = New System.Drawing.Point(16, 96)
			Me.labelEquals.Name = "labelEquals"
			Me.labelEquals.Size = New System.Drawing.Size(144, 24)
			Me.labelEquals.TabIndex = 4
			Me.labelEquals.Text = "equals"
			Me.labelEquals.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
			'
			'labelTo
			'
			Me.labelTo.Location = New System.Drawing.Point(160, 72)
			Me.labelTo.Name = "labelTo"
			Me.labelTo.Size = New System.Drawing.Size(32, 16)
			Me.labelTo.TabIndex = 7
			Me.labelTo.Text = "to"
			Me.labelTo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
			'
			'groupShow
			'
			Me.groupShow.Controls.Add(Me.labelEquals)
			Me.groupShow.Controls.Add(Me.otherSettingsList)
			Me.groupShow.Controls.Add(Me.conditionValues)
			Me.groupShow.Controls.Add(Me.showCondition)
			Me.groupShow.Controls.Add(Me.showAlways)
			Me.groupShow.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.groupShow.Location = New System.Drawing.Point(8, 104)
			Me.groupShow.Name = "groupShow"
			Me.groupShow.Size = New System.Drawing.Size(176, 152)
			Me.groupShow.TabIndex = 7
			Me.groupShow.TabStop = false
			Me.groupShow.Text = "Show Setting"
			'
			'constEditor
			'
			Me.constEditor.Location = New System.Drawing.Point(8, 72)
			Me.constEditor.Name = "constEditor"
			Me.constEditor.Size = New System.Drawing.Size(176, 20)
			Me.constEditor.TabIndex = 4
			Me.constEditor.Text = ""
			'
			'rangeMax
			'
			Me.rangeMax.Enabled = false
			Me.rangeMax.InterceptArrowKeys = false
			Me.rangeMax.Location = New System.Drawing.Point(192, 72)
			Me.rangeMax.Maximum = New Decimal(New Integer() {65535, 0, 0, 0})
			Me.rangeMax.Name = "rangeMax"
			Me.rangeMax.Size = New System.Drawing.Size(48, 20)
			Me.rangeMax.TabIndex = 8
			'
			'allowCustom
			'
			Me.allowCustom.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.allowCustom.Location = New System.Drawing.Point(8, 96)
			Me.allowCustom.Name = "allowCustom"
			Me.allowCustom.Size = New System.Drawing.Size(184, 16)
			Me.allowCustom.TabIndex = 5
			Me.allowCustom.Text = "Custom list of types:"
			AddHandler Me.allowCustom.CheckedChanged, AddressOf Me.AllowCustomCheckedChanged
			'
			'nameEditor
			'
			Me.nameEditor.Location = New System.Drawing.Point(8, 24)
			Me.nameEditor.Name = "nameEditor"
			Me.nameEditor.Size = New System.Drawing.Size(176, 20)
			Me.nameEditor.TabIndex = 3
			Me.nameEditor.Text = ""
			'
			'showCondition
			'
			Me.showCondition.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.showCondition.Location = New System.Drawing.Point(16, 40)
			Me.showCondition.Name = "showCondition"
			Me.showCondition.Size = New System.Drawing.Size(144, 24)
			Me.showCondition.TabIndex = 1
			Me.showCondition.Text = "Only when"
			AddHandler Me.showCondition.CheckedChanged, AddressOf Me.ShowConditionCheckedChanged
			'
			'labelName
			'
			Me.labelName.Location = New System.Drawing.Point(8, 8)
			Me.labelName.Name = "labelName"
			Me.labelName.Size = New System.Drawing.Size(176, 16)
			Me.labelName.TabIndex = 0
			Me.labelName.Text = "Name:"
			'
			'conditionValues
			'
			Me.conditionValues.Enabled = false
			Me.conditionValues.Location = New System.Drawing.Point(16, 120)
			Me.conditionValues.Name = "conditionValues"
			Me.conditionValues.Size = New System.Drawing.Size(144, 20)
			Me.conditionValues.TabIndex = 2
			Me.conditionValues.Text = ""
			'
			'typesHelp
			'
			Me.typesHelp.Location = New System.Drawing.Point(16, 152)
			Me.typesHelp.Multiline = true
			Me.typesHelp.Name = "typesHelp"
			Me.typesHelp.ReadOnly = true
			Me.typesHelp.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
			Me.typesHelp.Size = New System.Drawing.Size(224, 112)
			Me.typesHelp.TabIndex = 10
			Me.typesHelp.Text = "To allow for multiple value types, enter a list of types with a | between them."&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)& _ 
""&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&"Allowed types are:"&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&"free - Allows anything"&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&"label - Allows any label"&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&"conditio"& _ 
"n - Allows a condition"&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&"table - Allows a data table"&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&"bit - Allows any bit from v"& _ 
"ariable, or bit variable"&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&"io_pin - Allows an IO pin"&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&"io_port - Allows an entire "& _ 
"IO port"&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&"number - Allows any fixed number or variable"&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&"rangex:y - Allows any n"& _ 
"umber between x and y"&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&"var - Allows any variable"&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&"var_byte - Allows any byte v"& _ 
"ariable"&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&"var_word - Allows any word variable"&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&"var_integer - Allows any integer v"& _ 
"ariable"&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&"var_string - Allows any string variable"&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&"const - Allows any fixed num"& _ 
"ber"&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&"const_byte - Allows any byte sized fixed number"&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&"const_word - Allows any wo"& _ 
"rd sized fixed number"&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&"const_integer - Allows any integer sized fixed number"&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&"co"& _ 
"nst_string - Allows any fixed string"&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&"byte - Allows any byte (fixed number or "& _ 
"variable)"&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&"word - Allows any word"&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&"integer - Allows any integer"&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&"string - Allows"& _ 
" any string"&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&"array - Allows any array"
			Me.typesHelp.Visible = false
			'
			'labelConst
			'
			Me.labelConst.Location = New System.Drawing.Point(8, 56)
			Me.labelConst.Name = "labelConst"
			Me.labelConst.Size = New System.Drawing.Size(168, 16)
			Me.labelConst.TabIndex = 1
			Me.labelConst.Text = "Constant Name:"
			'
			'otherSettingsList
			'
			Me.otherSettingsList.Enabled = false
			Me.otherSettingsList.Location = New System.Drawing.Point(16, 72)
			Me.otherSettingsList.Name = "otherSettingsList"
			Me.otherSettingsList.Size = New System.Drawing.Size(144, 21)
			Me.otherSettingsList.TabIndex = 3
			'
			'typesEditor
			'
			Me.typesEditor.Enabled = false
			Me.typesEditor.Location = New System.Drawing.Point(16, 120)
			Me.typesEditor.Name = "typesEditor"
			Me.typesEditor.Size = New System.Drawing.Size(224, 20)
			Me.typesEditor.TabIndex = 9
			Me.typesEditor.Text = ""
			'
			'allowAny
			'
			Me.allowAny.Checked = true
			Me.allowAny.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.allowAny.Location = New System.Drawing.Point(8, 24)
			Me.allowAny.Name = "allowAny"
			Me.allowAny.Size = New System.Drawing.Size(96, 16)
			Me.allowAny.TabIndex = 1
			Me.allowAny.TabStop = true
			Me.allowAny.Text = "Any"
			'
			'HardwareSettingEditor
			'
			Me.AcceptButton = Me.buttonOK
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.CancelButton = Me.buttonCancel
			Me.ClientSize = New System.Drawing.Size(458, 320)
			Me.ControlBox = false
			Me.Controls.Add(Me.groupShow)
			Me.Controls.Add(Me.buttonCancel)
			Me.Controls.Add(Me.buttonOK)
			Me.Controls.Add(Me.constEditor)
			Me.Controls.Add(Me.nameEditor)
			Me.Controls.Add(Me.labelConst)
			Me.Controls.Add(Me.labelName)
			Me.Controls.Add(Me.groupTypes)
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
			Me.MaximizeBox = false
			Me.MinimizeBox = false
			Me.Name = "HardwareSettingEditor"
			Me.ShowInTaskbar = false
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
			Me.Text = "Hardware Setting Editor"
			CType(Me.rangeMin,System.ComponentModel.ISupportInitialize).EndInit
			Me.groupTypes.ResumeLayout(false)
			Me.groupShow.ResumeLayout(false)
			CType(Me.rangeMax,System.ComponentModel.ISupportInitialize).EndInit
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
		Public WriteOnly Property OtherSettings As ArrayList
			Set
				'Clear other settings list
				otherSettingsList.Items.Clear
				
				'Fill other settings list with new contents
				If Not Value Is Nothing Then
					Dim CurrSetting As String
					For Each CurrSetting In Value
						otherSettingsList.Items.Add(CurrSetting)
					Next
				End If
				
			End Set
		End Property
		
		Public Property Setting
			Get
				If Accepted Then
					Dim OutSetting As New HardwareDeviceSetting
					With OutSetting
						'Store name and constant
						.Name = nameEditor.Text
						.Constant = constEditor.Text
						
						'Store condition
						If showCondition.Checked Then
							.Condition = otherSettingsList.Text.Trim + " " + conditionValues.Text.Trim
						Else
							.Condition = "All"
						End If
						
						'Store accepted types
						If allowAny.Checked Then
							.ParamType = "free"
						Else If allowIO.Checked Then
							.ParamType = "io_pin"
						Else If allowAnyVar.Checked Then
							.ParamType = "var"
						Else If allowAnyNum.Checked Then
							.ParamType = "number"
						Else If allowRange.Checked Then
							.ParamType = "range" + rangeMin.Value.ToString + ":" + rangeMax.Value.ToString
						Else If allowCustom.Checked Then
							.ParamType = typesEditor.Text.Trim
						End If
						
					End With
					
					Return OutSetting
					
				Else
					Return InSetting
				End If
			End Get
			
			Set
				InSetting = Value
				Accepted = False
				
				If Not InSetting Is Nothing Then
					With InSetting
						nameEditor.Text = .Name
						constEditor.Text = .Constant
						
						'Load condition
						If .Condition.ToLower = "all" Then
							showAlways.Checked = True
						Else
							showCondition.Checked = True
							otherSettingsList.Text = .Condition.Substring(0, .Condition.IndexOf(" ")).Trim
							conditionValues.Text = .Condition.Substring(.Condition.IndexOf(" ") + 1).Trim
						End If
						
						'Store accepted types
						If .ParamType.ToLower = "free" Then
							allowAny.Checked = True
						Else If .ParamType.ToLower = "io_pin" Then
							allowIO.Checked = True
						Else If .ParamType.ToLower = "var" Then
							allowAnyVar.Checked = True
						Else If .ParamType.ToLower = "number" Then
							allowAnyNum.Checked = True
						Else If .ParamType.ToLower.StartsWith("range") Then
							allowRange.Checked = True
							Dim RangeTemp As String = .ParamType.Substring(5)
							rangeMin.Value = RangeTemp.Substring(0, RangeTemp.IndexOf(":"))
							rangeMax.Value = RangeTemp.Substring(RangeTemp.IndexOf(":") + 1)
						Else
							allowCustom.Checked = True
							typesEditor.Text = .ParamType
						End If
						
					End With
				End If
			End Set
		End Property
		
		Private Sub ShowConditionCheckedChanged(sender As System.Object, e As System.EventArgs)
			If showCondition.Checked Then
				otherSettingsList.Enabled = True
				conditionValues.Enabled = True
			Else
				otherSettingsList.Enabled = False
				conditionValues.Enabled = False
			End If
			
		End Sub
		
		Private Sub AllowCustomCheckedChanged(sender As System.Object, e As System.EventArgs)
			If allowCustom.Checked Then
				typesEditor.Enabled = True
				typesHelp.Visible = True
			Else
				typesEditor.Enabled = False
				typesHelp.Visible = False
			End If
			
		End Sub
		
		Private Sub AllowRangeCheckedChanged(sender As System.Object, e As System.EventArgs)
			If allowRange.Checked Then
				rangeMin.Enabled = True
				rangeMax.Enabled = True
			Else
				rangeMin.Enabled = False
				rangeMax.Enabled = False
			End If
			
		End Sub
		
		Private Sub ButtonOKClick(sender As System.Object, e As System.EventArgs)
			
			'Check data
			If nameEditor.Text = "" Then
				MessageBox.Show("Please type a name for the hardware setting!", "Great Cow Graphical BASIC", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
				Exit Sub
			End If
			If constEditor.Text = "" Then
				MessageBox.Show("Please type a constant for the hardware setting!", "Great Cow Graphical BASIC", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
				Exit Sub
			End If
			If LowLevel.CheckName(constEditor.Text) <> "" Then
				MessageBox.Show("Please check the name of the constant. It should start with a letter, and only contain letters, numbers, and underscores.", "Great Cow Graphical BASIC", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
				Exit Sub
			End If
			
			'Accept and exit
			Accepted = True
			Me.Close
		End Sub
		
	End Class
End Namespace
