'
' Created by SharpDevelop.
' User: Administrator
' Date: 8/01/2007
' Time: 1:50 AM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.Windows.Forms

Namespace Great_Cow_Graphical_BASIC
	
	Public Class SubNameDialog
		Inherits System.Windows.Forms.Form
		Private Button_Ok As System.Windows.Forms.Button
		Private nameLabel As System.Windows.Forms.Label
		Private AddSubHelp As System.Windows.Forms.HelpProvider
		Private Button_Cancel As System.Windows.Forms.Button
		Private fnTypeWord As System.Windows.Forms.RadioButton
		Private ModeFunction As System.Windows.Forms.RadioButton
		Private SubDescInput As System.Windows.Forms.TextBox
		Private SubNameInput As System.Windows.Forms.TextBox
		Private fnTypeString As System.Windows.Forms.RadioButton
		Private fnTypeByte As System.Windows.Forms.RadioButton
		Private typeLabel As System.Windows.Forms.Label
		Private fnTypeInteger As System.Windows.Forms.RadioButton
		Private hideSub As System.Windows.Forms.CheckBox
		Private descLabel As System.Windows.Forms.Label
		Private ModeSub As System.Windows.Forms.RadioButton
		Private groupFnType As System.Windows.Forms.GroupBox
		
		Public Dim SubType As String
		Private Dim OldSubName As String
		Private Dim Accepted As Boolean = False
		
		Private Dim CurrentFile As GCBProgram
		
		Public Sub New(CurrentFile As GCBProgram)
			MyBase.New
			'
			' The Me.InitializeComponent call is required for Windows Forms designer support.
			'
			Me.InitializeComponent
			
			OldSubName = ""
			SubType = ""
			Me.CurrentFile = CurrentFile
			
			Me.SubNameInput.Select
			
			ApplyTranslation
			
		End Sub
		
		Private Sub ApplyTranslation
			With MainForm.Translator
				If .CanTranslate Then
					
					'Set title
					.TryTranslate("SubNameDialog", "Title", Me.Text)
					
					'Set labels
					.TryTranslate("SubNameDialog", "NameLabel", nameLabel.Text)
					.TryTranslate("SubNameDialog", "TypeLabel", typeLabel.Text)
					.TryTranslate("SubNameDialog", "DescLabel", descLabel.Text)
					
					.TryTranslate("SubNameDialog", "FnReturnType", groupFnType.Text)
					.TryTranslate("SubNameDialog", "HideSub", HideSub.Text)
					
					'Set tooltips
					Dim newTip As String
					newTip = .GetMessage("SubNameDialog", "NameTip")
					If newTip <> "" Then Me.AddSubHelp.SetHelpString(Me.SubNameInput, newTip)
					newTip = .GetMessage("SubNameDialog", "SubTip")
					If newTip <> "" Then Me.AddSubHelp.SetHelpString(Me.ModeSub, newTip)
					newTip = .GetMessage("SubNameDialog", "FunctionTip")
					If newTip <> "" Then Me.AddSubHelp.SetHelpString(Me.ModeFunction, newTip)
					
					newTip = .GetMessage("SubNameDialog", "DescTip")
					If newTip <> "" Then Me.AddSubHelp.SetHelpString(Me.SubDescInput, newTip)
					newTip = .GetMessage("SubNameDialog", "HideSubTip")
					If newTip <> "" Then Me.AddSubHelp.SetHelpString(Me.HideSub, newTip)
					
					'Set OK/Cancel button text
					.TryTranslate("Misc", "Cancel", Button_Cancel.Text)
					.TryTranslate("Misc", "OK", Button_OK.Text)
					
				End If
			End With
		End Sub
		
		Public Sub SetTitle(NewTitle As String)
			Me.Text = NewTitle
		End Sub
		
		Public Sub Init(SubName As String, SubType As String, SubDesc As String, SubHidden As Boolean)
			Me.OldSubName = SubName
			Me.SubNameInput.Text = SubName
			
			'Set type radio buttons
			Select Case SubType.ToLower
				Case "":
					Me.ModeSub.Checked = True
				Case "byte":
					Me.ModeFunction.Checked = True
					Me.fnTypeByte.Checked = True
				Case "word":
					Me.ModeFunction.Checked = True
					Me.fnTypeWord.Checked = True
				Case "integer":
					Me.ModeFunction.Checked = True
					Me.fnTypeInteger.Checked = True
				Case "string":
					Me.ModeFunction.Checked = True
					Me.fnTypeString.Checked = True
			End Select
			
			'Set description and hidden flag
			SubDescInput.Text = SubDesc
			hideSub.Checked = SubHidden
			
		End Sub
		
		Public Function IsAccepted As Boolean
			Return Accepted
		End Function
		
		#Region " Windows Forms Designer generated code "
		' This method is required for Windows Forms designer support.
		' Do not change the method contents inside the source code editor. The Forms designer might
		' not be able to load this method if it was changed manually.
		Private Sub InitializeComponent()
			Me.groupFnType = New System.Windows.Forms.GroupBox
			Me.ModeSub = New System.Windows.Forms.RadioButton
			Me.descLabel = New System.Windows.Forms.Label
			Me.hideSub = New System.Windows.Forms.CheckBox
			Me.fnTypeInteger = New System.Windows.Forms.RadioButton
			Me.typeLabel = New System.Windows.Forms.Label
			Me.fnTypeByte = New System.Windows.Forms.RadioButton
			Me.fnTypeString = New System.Windows.Forms.RadioButton
			Me.SubNameInput = New System.Windows.Forms.TextBox
			Me.SubDescInput = New System.Windows.Forms.TextBox
			Me.ModeFunction = New System.Windows.Forms.RadioButton
			Me.fnTypeWord = New System.Windows.Forms.RadioButton
			Me.Button_Cancel = New System.Windows.Forms.Button
			Me.AddSubHelp = New System.Windows.Forms.HelpProvider
			Me.nameLabel = New System.Windows.Forms.Label
			Me.Button_Ok = New System.Windows.Forms.Button
			Me.groupFnType.SuspendLayout
			Me.SuspendLayout
			'
			'groupFnType
			'
			Me.groupFnType.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.groupFnType.Controls.Add(Me.fnTypeString)
			Me.groupFnType.Controls.Add(Me.fnTypeInteger)
			Me.groupFnType.Controls.Add(Me.fnTypeWord)
			Me.groupFnType.Controls.Add(Me.fnTypeByte)
			Me.groupFnType.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.groupFnType.Location = New System.Drawing.Point(192, 8)
			Me.groupFnType.Name = "groupFnType"
			Me.groupFnType.Size = New System.Drawing.Size(176, 72)
			Me.groupFnType.TabIndex = 9
			Me.groupFnType.TabStop = false
			Me.groupFnType.Text = "Function Type"
			'
			'ModeSub
			'
			Me.ModeSub.Checked = true
			Me.ModeSub.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.AddSubHelp.SetHelpString(Me.ModeSub, "Create a subroutine. Subroutines are essentially miniature programs that can be c"& _ 
"ombined to form larger programs.")
			Me.ModeSub.Location = New System.Drawing.Point(24, 80)
			Me.ModeSub.Name = "ModeSub"
			Me.AddSubHelp.SetShowHelp(Me.ModeSub, true)
			Me.ModeSub.Size = New System.Drawing.Size(80, 24)
			Me.ModeSub.TabIndex = 2
			Me.ModeSub.TabStop = true
			Me.ModeSub.Text = "Subroutine"
			'
			'descLabel
			'
			Me.descLabel.Location = New System.Drawing.Point(8, 120)
			Me.descLabel.Name = "descLabel"
			Me.descLabel.Size = New System.Drawing.Size(168, 16)
			Me.descLabel.TabIndex = 10
			Me.descLabel.Text = "Description (optional):"
			'
			'hideSub
			'
			Me.hideSub.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.AddSubHelp.SetHelpString(Me.hideSub, "If this box is checked, then the subroutine or function will not be shown in any "& _ 
"lists that would allow it to be added to the program. This is useful for any sub"& _ 
"routines used internally by a library.")
			Me.hideSub.Location = New System.Drawing.Point(200, 96)
			Me.hideSub.Name = "hideSub"
			Me.AddSubHelp.SetShowHelp(Me.hideSub, true)
			Me.hideSub.Size = New System.Drawing.Size(168, 40)
			Me.hideSub.TabIndex = 12
			Me.hideSub.Text = "Don't show in lists of subroutines or functions to add"
			'
			'fnTypeInteger
			'
			Me.fnTypeInteger.Enabled = false
			Me.fnTypeInteger.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.fnTypeInteger.Location = New System.Drawing.Point(104, 24)
			Me.fnTypeInteger.Name = "fnTypeInteger"
			Me.fnTypeInteger.Size = New System.Drawing.Size(64, 16)
			Me.fnTypeInteger.TabIndex = 2
			Me.fnTypeInteger.Text = "Integer"
			'
			'typeLabel
			'
			Me.typeLabel.Location = New System.Drawing.Point(8, 64)
			Me.typeLabel.Name = "typeLabel"
			Me.typeLabel.Size = New System.Drawing.Size(176, 16)
			Me.typeLabel.TabIndex = 8
			Me.typeLabel.Text = "Type:"
			'
			'fnTypeByte
			'
			Me.fnTypeByte.Checked = true
			Me.fnTypeByte.Enabled = false
			Me.fnTypeByte.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.fnTypeByte.Location = New System.Drawing.Point(16, 24)
			Me.fnTypeByte.Name = "fnTypeByte"
			Me.fnTypeByte.Size = New System.Drawing.Size(64, 16)
			Me.fnTypeByte.TabIndex = 0
			Me.fnTypeByte.TabStop = true
			Me.fnTypeByte.Text = "Byte"
			'
			'fnTypeString
			'
			Me.fnTypeString.Enabled = false
			Me.fnTypeString.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.fnTypeString.Location = New System.Drawing.Point(104, 48)
			Me.fnTypeString.Name = "fnTypeString"
			Me.fnTypeString.Size = New System.Drawing.Size(64, 16)
			Me.fnTypeString.TabIndex = 3
			Me.fnTypeString.Text = "String"
			'
			'SubNameInput
			'
			Me.AddSubHelp.SetHelpString(Me.SubNameInput, "The name for the new subroutine or function")
			Me.SubNameInput.Location = New System.Drawing.Point(8, 24)
			Me.SubNameInput.Name = "SubNameInput"
			Me.AddSubHelp.SetShowHelp(Me.SubNameInput, true)
			Me.SubNameInput.Size = New System.Drawing.Size(176, 20)
			Me.SubNameInput.TabIndex = 0
			Me.SubNameInput.Text = ""
			'
			'SubDescInput
			'
			Me.AddSubHelp.SetHelpString(Me.SubDescInput, "If a description is typed here, it will show up when the mouse hovers over the ic"& _ 
"on in the list of available subroutines, or under the function in the list of av"& _ 
"ailable functions.")
			Me.SubDescInput.Location = New System.Drawing.Point(8, 136)
			Me.SubDescInput.Name = "SubDescInput"
			Me.AddSubHelp.SetShowHelp(Me.SubDescInput, true)
			Me.SubDescInput.Size = New System.Drawing.Size(176, 20)
			Me.SubDescInput.TabIndex = 11
			Me.SubDescInput.Text = ""
			'
			'ModeFunction
			'
			Me.ModeFunction.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.AddSubHelp.SetHelpString(Me.ModeFunction, "Create a function. Functions are similar to subroutines, but they can be treated "& _ 
"as variables in calculations. This is useful for reading sensors.")
			Me.ModeFunction.Location = New System.Drawing.Point(112, 80)
			Me.ModeFunction.Name = "ModeFunction"
			Me.AddSubHelp.SetShowHelp(Me.ModeFunction, true)
			Me.ModeFunction.Size = New System.Drawing.Size(72, 24)
			Me.ModeFunction.TabIndex = 3
			Me.ModeFunction.TabStop = true
			Me.ModeFunction.Text = "Function"
			AddHandler Me.ModeFunction.CheckedChanged, AddressOf Me.ModeFunctionCheckedChanged
			'
			'fnTypeWord
			'
			Me.fnTypeWord.Enabled = false
			Me.fnTypeWord.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.fnTypeWord.Location = New System.Drawing.Point(16, 48)
			Me.fnTypeWord.Name = "fnTypeWord"
			Me.fnTypeWord.Size = New System.Drawing.Size(64, 16)
			Me.fnTypeWord.TabIndex = 1
			Me.fnTypeWord.Text = "Word"
			'
			'Button_Cancel
			'
			Me.Button_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom
			Me.Button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me.Button_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.Button_Cancel.Location = New System.Drawing.Point(196, 168)
			Me.Button_Cancel.Name = "Button_Cancel"
			Me.Button_Cancel.Size = New System.Drawing.Size(88, 24)
			Me.Button_Cancel.TabIndex = 6
			Me.Button_Cancel.Text = "Cancel"
			'
			'nameLabel
			'
			Me.nameLabel.Location = New System.Drawing.Point(8, 8)
			Me.nameLabel.Name = "nameLabel"
			Me.nameLabel.Size = New System.Drawing.Size(176, 16)
			Me.nameLabel.TabIndex = 7
			Me.nameLabel.Text = "Name:"
			'
			'Button_Ok
			'
			Me.Button_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom
			Me.Button_Ok.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.Button_Ok.Location = New System.Drawing.Point(92, 168)
			Me.Button_Ok.Name = "Button_Ok"
			Me.Button_Ok.Size = New System.Drawing.Size(88, 24)
			Me.Button_Ok.TabIndex = 5
			Me.Button_Ok.Text = "OK"
			AddHandler Me.Button_Ok.Click, AddressOf Me.Button_OkClick
			'
			'SubNameDialog
			'
			Me.AcceptButton = Me.Button_Ok
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.CancelButton = Me.Button_Cancel
			Me.ClientSize = New System.Drawing.Size(378, 200)
			Me.Controls.Add(Me.hideSub)
			Me.Controls.Add(Me.SubDescInput)
			Me.Controls.Add(Me.descLabel)
			Me.Controls.Add(Me.groupFnType)
			Me.Controls.Add(Me.typeLabel)
			Me.Controls.Add(Me.nameLabel)
			Me.Controls.Add(Me.Button_Cancel)
			Me.Controls.Add(Me.Button_Ok)
			Me.Controls.Add(Me.SubNameInput)
			Me.Controls.Add(Me.ModeSub)
			Me.Controls.Add(Me.ModeFunction)
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
			Me.HelpButton = true
			Me.MaximizeBox = false
			Me.MinimizeBox = false
			Me.Name = "SubNameDialog"
			Me.ShowInTaskbar = false
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
			Me.Text = "Add Subroutine"
			Me.groupFnType.ResumeLayout(false)
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
		Public ReadOnly Property SubName As String
			Get
				If Accepted Then
					Return SubNameInput.Text
				Else
					Return ""
				End If
			End Get
		End Property
		Public ReadOnly Property SubDesc As String
			Get
				Return SubDescInput.Text
			End Get
		End Property
		Public ReadOnly Property SubHidden As Boolean
			Get
				Return hideSub.Checked
			End Get
		End Property
		
		Private Sub Button_OkClick(sender As System.Object, e As System.EventArgs)
			Dim NameCheck As String
			Dim NewNameIn As String
			'SubName = ""
			Accepted = False
			
			'Get translated messages
			Dim InvalidName, DupName, BadChar, BadChars As String
			With MainForm.Translator
				InvalidName = .GetMessage("SubNameDialog", "InvalidName", "Invalid Name")
				DupName = .GetMessage("SubNameDialog", "DupName", "Duplicate subroutine/function name")
				BadChar = .GetMessage("SubNameDialog", "BadChar", "The subroutine name contains an invalid symbol")
				BadChars = .GetMessage("SubNameDialog", "BadChars", "The subroutine name contains invalid symbols")
			End With
			
			If CurrentFile.IsSub(SubNameInput.Text) And SubNameInput.Text.ToLower <> OldSubName.ToLower Then
				MessageBox.Show(DupName, InvalidName, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
				Exit Sub
			End If
			
			NameCheck = LowLevel.CheckName(SubNameInput.Text)
			If NameCheck <> "" Then
				If NameCheck.Length = 1 Then MessageBox.Show(BadChar + " (" + NameCheck + ")", InvalidName, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
				If NameCheck.Length > 1 Then MessageBox.Show(BadChars + " (" + NameCheck + ")", InvalidName, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
				Exit Sub
			End If
			
			'SubName = SubNameInput.Text
			SubType = ""
			If ModeFunction.Checked Then
				If fnTypeByte.Checked Then SubType = "byte"
				If fnTypeWord.Checked Then SubType = "word"
				If fnTypeInteger.Checked Then SubType = "integer"
				If fnTypeString.Checked Then SubType = "string"
			End If
			
			Accepted = True
			Me.Close()
			
		End Sub
		
		Private Sub ModeFunctionCheckedChanged(sender As System.Object, e As System.EventArgs)
			'If Function selected/deselected, enable/disable return type entry
			Dim currControl As Control
			For Each currControl In groupFnType.Controls
				currControl.Enabled = ModeFunction.Checked
			Next
		End Sub
		
	End Class
End Namespace
