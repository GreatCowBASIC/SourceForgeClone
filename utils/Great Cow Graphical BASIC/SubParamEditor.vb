'
' Created by SharpDevelop.
' User: Administrator
' Date: 10/03/2008
' Time: 12:46 AM
'
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.Windows.Forms

'Namespace Great_Cow_Graphical_BASIC
	
	Public Class SubParamEditor
		Inherits System.Windows.Forms.Form
		Private ParamDesc As System.Windows.Forms.TextBox
		Private ButtonOK As System.Windows.Forms.Button
		Private DataDirBoth As System.Windows.Forms.RadioButton
		Private SubParamEditHelp As System.Windows.Forms.HelpProvider
		Private defLabel As System.Windows.Forms.Label
		Private DefValue As System.Windows.Forms.TextBox
		Private DataDirIn As System.Windows.Forms.RadioButton
		Private ButtonCancel As System.Windows.Forms.Button
		Private nameLabel As System.Windows.Forms.Label
		Private ParamName As System.Windows.Forms.TextBox
		Private dirGroup As System.Windows.Forms.GroupBox
		Private descLabel As System.Windows.Forms.Label
		Private DataDirOut As System.Windows.Forms.RadioButton
		
		Private Dim OldData As SubParam
		Private Dim NewAccepted As Boolean
		Private Dim CurrSub As GCBSubroutine
		
		Public Sub New(CurrSub As GCBSubroutine, Optional InData As SubParam = Nothing)
			MyBase.New
			'
			' The Me.InitializeComponent call is required for Windows Forms designer support.
			'
			Me.InitializeComponent
			
			NewAccepted = False
			Me.CurrSub = CurrSub
			
			'Possible parameter types
			ParamType.Add(New CaptionedListItem("BIT", "Bit", "Stores a 0 or a 1"))
			ParamType.Add(New CaptionedListItem("BYTE", "Byte", "Stores a whole number between 0 and 255"))
			ParamType.Add(New CaptionedListItem("WORD", "Word", "Stores a whole number between 0 and 65535"))
			ParamType.Add(New CaptionedListItem("INTEGER", "Integer", "Stores a whole number between -32768 and 32767"))
			ParamType.Add(New CaptionedListItem("LONG", "Long", "Stores a whole number between 0 and 4.2 billion"))
			ParamType.Add(New CaptionedListItem("STRING", "String", "Stores text (any letters, numbers and punctuation)"))
			ParamType.Add(New CaptionedListItem("ARRAY", "Array", "Stores a list of numbers between 0 and 255"))
			
			'Defaults
			ParamType.SelectedID = "byte"
			Me.DataDirBoth.Checked = True
			
			'Translate
			ApplyTranslation
			
			'Functions can only have input parameters
			If CurrSub.IsFunction Then
				Me.DataDirIn.Checked = True
				Me.DataDirIn.Enabled = False
				Me.DataDirOut.Enabled = False
				Me.DataDirBoth.Enabled = False
			End If
			
			If Not InData Is Nothing Then
				OldData = InData
				With InData
					Me.ParamName.Text = .Name
					Me.ParamDesc.Text = .Description
					
		 			If .DataArray Then
		 				Me.ParamType.SelectedID = "array"
		 			Else
		 				Me.ParamType.SelectedID = .DataType
		 			End If
		 			
		 			If .DataDir = "I" Then
		 				Me.DataDirIn.Checked = True
		 			ElseIf .DataDir = "O" Then
		 				Me.DataDirOut.Checked = True
		 			Else
		 				Me.DataDirBoth.Checked = True
		 			End If
		 			
		 			Me.DefValue.Text = .DefValue
				End With
			End If
			
			Me.ParamName.Select
			
		End Sub
		
		#Region " Windows Forms Designer generated code "
		' This method is required for Windows Forms designer support.
		' Do not change the method contents inside the source code editor. The Forms designer might
		' not be able to load this method if it was changed manually.
		Private Sub InitializeComponent()
			Me.DataDirOut = New System.Windows.Forms.RadioButton
			Me.descLabel = New System.Windows.Forms.Label
			Me.dirGroup = New System.Windows.Forms.GroupBox
			Me.DataDirIn = New System.Windows.Forms.RadioButton
			Me.DataDirBoth = New System.Windows.Forms.RadioButton
			Me.ParamName = New System.Windows.Forms.TextBox
			Me.ParamType = New Great_Cow_Graphical_BASIC.CaptionedComboBox
			Me.nameLabel = New System.Windows.Forms.Label
			Me.ButtonCancel = New System.Windows.Forms.Button
			Me.DefValue = New System.Windows.Forms.TextBox
			Me.defLabel = New System.Windows.Forms.Label
			Me.SubParamEditHelp = New System.Windows.Forms.HelpProvider
			Me.ButtonOK = New System.Windows.Forms.Button
			Me.ParamDesc = New System.Windows.Forms.TextBox
			Me.dataTypeLabel = New System.Windows.Forms.Label
			Me.dirGroup.SuspendLayout
			Me.SuspendLayout
			'
			'DataDirOut
			'
			Me.DataDirOut.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.SubParamEditHelp.SetHelpString(Me.DataDirOut, "Parameter is used to get data from subroutine, but does not put any data in.")
			Me.DataDirOut.Location = New System.Drawing.Point(16, 64)
			Me.DataDirOut.Name = "DataDirOut"
			Me.SubParamEditHelp.SetShowHelp(Me.DataDirOut, true)
			Me.DataDirOut.Size = New System.Drawing.Size(120, 16)
			Me.DataDirOut.TabIndex = 2
			Me.DataDirOut.Text = "Out of sub only"
			'
			'descLabel
			'
			Me.descLabel.Location = New System.Drawing.Point(192, 8)
			Me.descLabel.Name = "descLabel"
			Me.descLabel.Size = New System.Drawing.Size(152, 16)
			Me.descLabel.TabIndex = 8
			Me.descLabel.Text = "Description"
			'
			'dirGroup
			'
			Me.dirGroup.Controls.Add(Me.DataDirOut)
			Me.dirGroup.Controls.Add(Me.DataDirIn)
			Me.dirGroup.Controls.Add(Me.DataDirBoth)
			Me.dirGroup.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.SubParamEditHelp.SetHelpString(Me.dirGroup, "Direction that the parameter moves data in")
			Me.dirGroup.Location = New System.Drawing.Point(192, 64)
			Me.dirGroup.Name = "dirGroup"
			Me.SubParamEditHelp.SetShowHelp(Me.dirGroup, true)
			Me.dirGroup.Size = New System.Drawing.Size(176, 88)
			Me.dirGroup.TabIndex = 4
			Me.dirGroup.TabStop = false
			Me.dirGroup.Text = "Data direction"
			'
			'DataDirIn
			'
			Me.DataDirIn.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.SubParamEditHelp.SetHelpString(Me.DataDirIn, "Parameter is used to copy data into the subroutine, but not back out again")
			Me.DataDirIn.Location = New System.Drawing.Point(16, 40)
			Me.DataDirIn.Name = "DataDirIn"
			Me.SubParamEditHelp.SetShowHelp(Me.DataDirIn, true)
			Me.DataDirIn.Size = New System.Drawing.Size(120, 24)
			Me.DataDirIn.TabIndex = 1
			Me.DataDirIn.Text = "In to sub only"
			'
			'DataDirBoth
			'
			Me.DataDirBoth.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.SubParamEditHelp.SetHelpString(Me.DataDirBoth, "Parameter is used to copy data into and out of the subroutine")
			Me.DataDirBoth.Location = New System.Drawing.Point(16, 24)
			Me.DataDirBoth.Name = "DataDirBoth"
			Me.SubParamEditHelp.SetShowHelp(Me.DataDirBoth, true)
			Me.DataDirBoth.Size = New System.Drawing.Size(120, 16)
			Me.DataDirBoth.TabIndex = 0
			Me.DataDirBoth.Text = "In and out of sub"
			'
			'ParamName
			'
			Me.SubParamEditHelp.SetHelpString(Me.ParamName, "The name of the parameter")
			Me.ParamName.Location = New System.Drawing.Point(8, 24)
			Me.ParamName.Name = "ParamName"
			Me.SubParamEditHelp.SetShowHelp(Me.ParamName, true)
			Me.ParamName.Size = New System.Drawing.Size(176, 20)
			Me.ParamName.TabIndex = 0
			'
			'ParamType
			'
			Me.ParamType.ListFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
			Me.ParamType.Location = New System.Drawing.Point(8, 72)
			Me.ParamType.Name = "ParamType"
			Me.ParamType.SelectedID = ""
			Me.ParamType.SelectedIndex = -1
			Me.ParamType.SelectedObject = Nothing
			Me.ParamType.Size = New System.Drawing.Size(176, 72)
			Me.ParamType.TabIndex = 0
			'
			'nameLabel
			'
			Me.nameLabel.Location = New System.Drawing.Point(8, 8)
			Me.nameLabel.Name = "nameLabel"
			Me.nameLabel.Size = New System.Drawing.Size(144, 16)
			Me.nameLabel.TabIndex = 6
			Me.nameLabel.Text = "Name"
			'
			'ButtonCancel
			'
			Me.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me.ButtonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.ButtonCancel.Location = New System.Drawing.Point(200, 216)
			Me.ButtonCancel.Name = "ButtonCancel"
			Me.ButtonCancel.Size = New System.Drawing.Size(72, 24)
			Me.ButtonCancel.TabIndex = 1
			Me.ButtonCancel.Text = "Cancel"
			'
			'DefValue
			'
			Me.SubParamEditHelp.SetHelpString(Me.DefValue, "Default value, for when the subroutine is called but no value is given for this p"& _
						"arameter. Optional.")
			Me.DefValue.Location = New System.Drawing.Point(192, 184)
			Me.DefValue.Name = "DefValue"
			Me.SubParamEditHelp.SetShowHelp(Me.DefValue, true)
			Me.DefValue.Size = New System.Drawing.Size(176, 20)
			Me.DefValue.TabIndex = 0
			'
			'defLabel
			'
			Me.defLabel.Location = New System.Drawing.Point(192, 168)
			Me.defLabel.Name = "defLabel"
			Me.defLabel.Size = New System.Drawing.Size(160, 16)
			Me.defLabel.TabIndex = 7
			Me.defLabel.Text = "Default value (optional)"
			'
			'ButtonOK
			'
			Me.ButtonOK.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.SubParamEditHelp.SetHelpString(Me.ButtonOK, "It's pretty obvious that this is the OK button.")
			Me.ButtonOK.Location = New System.Drawing.Point(104, 216)
			Me.ButtonOK.Name = "ButtonOK"
			Me.SubParamEditHelp.SetShowHelp(Me.ButtonOK, true)
			Me.ButtonOK.Size = New System.Drawing.Size(72, 24)
			Me.ButtonOK.TabIndex = 0
			Me.ButtonOK.Text = "OK"
			AddHandler Me.ButtonOK.Click, AddressOf Me.OKButtonClick
			'
			'ParamDesc
			'
			Me.SubParamEditHelp.SetHelpString(Me.ParamDesc, "A description for the parameter that will be shown every time it needs to be give"& _
						"n a value")
			Me.ParamDesc.Location = New System.Drawing.Point(192, 24)
			Me.ParamDesc.Name = "ParamDesc"
			Me.SubParamEditHelp.SetShowHelp(Me.ParamDesc, true)
			Me.ParamDesc.Size = New System.Drawing.Size(176, 20)
			Me.ParamDesc.TabIndex = 9
			'
			'dataTypeLabel
			'
			Me.dataTypeLabel.Location = New System.Drawing.Point(8, 56)
			Me.dataTypeLabel.Name = "dataTypeLabel"
			Me.dataTypeLabel.Size = New System.Drawing.Size(144, 16)
			Me.dataTypeLabel.TabIndex = 10
			Me.dataTypeLabel.Text = "Type"
			'
			'SubParamEditor
			'
			Me.AcceptButton = Me.ButtonOK
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.CancelButton = Me.ButtonCancel
			Me.ClientSize = New System.Drawing.Size(378, 248)
			Me.Controls.Add(Me.dataTypeLabel)
			Me.Controls.Add(Me.ParamType)
			Me.Controls.Add(Me.ParamDesc)
			Me.Controls.Add(Me.descLabel)
			Me.Controls.Add(Me.defLabel)
			Me.Controls.Add(Me.nameLabel)
			Me.Controls.Add(Me.dirGroup)
			Me.Controls.Add(Me.ButtonCancel)
			Me.Controls.Add(Me.ButtonOK)
			Me.Controls.Add(Me.ParamName)
			Me.Controls.Add(Me.DefValue)
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
			Me.HelpButton = true
			Me.MaximizeBox = false
			Me.MinimizeBox = false
			Me.Name = "SubParamEditor"
			Me.ShowInTaskbar = false
			Me.Text = "Subroutine Parameter Editor"
			Me.dirGroup.ResumeLayout(false)
			Me.ResumeLayout(false)
			Me.PerformLayout
		End Sub
		Private dataTypeLabel As System.Windows.Forms.Label
		Private ParamType As Great_Cow_Graphical_BASIC.CaptionedComboBox
		#End Region
		
		Private Sub ApplyTranslation
			
			If MainForm.Translator Is Nothing Then Exit Sub
			
			With MainForm.Translator
				If .CanTranslate Then
					
					'Title
					.TryTranslate("SubParamEditor", "Title", Me.Text)
					
					'Labels
					.TryTranslate("SubParamEditor", "NameLabel", Me.nameLabel.Text)
					.TryTranslate("SubParamEditor", "DescLabel", Me.descLabel.Text)
					.TryTranslate("SubParamEditor", "TypeLabel", Me.dataTypeLabel.Text)
					.TryTranslate("SubParamEditor", "DataDirLabel", Me.dirGroup.Text)
					.TryTranslate("SubParamEditor", "DefaultLabel", Me.defLabel.Text)
					
					'Direction labels
	 				.TryTranslate("SubParamEditor", "DirIn", Me.DataDirIn.Text)
					.TryTranslate("SubParamEditor", "DirOut", Me.DataDirOut.Text)
					.TryTranslate("SubParamEditor", "DirInOut", Me.DataDirBoth.Text)
					
					'Tooltips
					Dim newTip As String
					newTip = .GetMessage("SubParamEditor", "NameTip")
					If newTip <> "" Then Me.SubParamEditHelp.SetHelpString(Me.ParamName, newTip)
					newTip = .GetMessage("SubParamEditor", "DescTip")
					If newTip <> "" Then Me.SubParamEditHelp.SetHelpString(Me.ParamDesc, newTip)
					
					newTip = .GetMessage("SubParamEditor", "TypeTip")
					If newTip <> "" Then Me.SubParamEditHelp.SetHelpString(Me.ParamType, newTip)
					
					newTip = .GetMessage("SubParamEditor", "DefValueTip")
					If newTip <> "" Then
						Me.SubParamEditHelp.SetHelpString(Me.DefValue, newTip)
						Me.SubParamEditHelp.SetHelpString(Me.defLabel, newTip)
					End If
					
					newTip = .GetMessage("SubParamEditor", "DirTip")
					If newTip <> "" Then Me.SubParamEditHelp.SetHelpString(Me.dirGroup, newTip)
					newTip = .GetMessage("SubParamEditor", "DirInTip")
					If newTip <> "" Then Me.SubParamEditHelp.SetHelpString(Me.DataDirIn, newTip)
					newTip = .GetMessage("SubParamEditor", "DirOutTip")
					If newTip <> "" Then Me.SubParamEditHelp.SetHelpString(Me.DataDirOut, newTip)
					newTip = .GetMessage("SubParamEditor", "DirInOutTip")
					If newTip <> "" Then Me.SubParamEditHelp.SetHelpString(Me.DataDirBoth, newTip)
					
					'Set OK/Cancel button text
					.TryTranslate("Misc", "Cancel", ButtonCancel.Text)
					.TryTranslate("Misc", "OK", ButtonOK.Text)
					
				End If
			End With
		End Sub
		
		Private Sub OKButtonClick(sender As System.Object, e As System.EventArgs)
			NewAccepted = True
			Me.Close
		End Sub
		
		Public Function GetOutput As SubParam
			
			If NewAccepted Then
				Dim OutVar As SubParam = New SubParam
				With OutVar
					.Name = Me.ParamName.Text
		 			.Description = Me.ParamDesc.Text
		 			
		 			If ParamType.SelectedID.ToLower = "array" Then
		 				.DataType = "byte"
		 				.DataArray = True
		 			Else
		 				.DataType = ParamType.SelectedID.ToLower
		 				.DataArray = False
		 			End If
		 			
		 			.DataDir = "IO"
		 			If Me.DataDirIn.Checked Then
		 				.DataDir = "I"
		 			ElseIf Me.DataDirOut.Checked Then
		 				.DataDir = "O"
		 			End If
		 	
		 			.DefValue = Me.DefValue.Text
		 		End With
		 		
		 		Return OutVar
		 	
		 	Else
		 		Return OldData
		 	End If
		 	
		End Function
		
	End Class
'End Namespace
