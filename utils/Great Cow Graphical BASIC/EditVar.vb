'
' Created by SharpDevelop.
' User: Administrator
' Date: 3/04/2007
' Time: 11:20 PM
'
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.Windows.Forms

''Namespace Great_Cow_Graphical_BASIC
	
	Public Class EditVar
		Inherits System.Windows.Forms.Form
		Private Button_OK As System.Windows.Forms.Button
		Private Button_Cancel As System.Windows.Forms.Button
		Private EditVarHelp As System.Windows.Forms.HelpProvider
		Private VarName As System.Windows.Forms.TextBox
		Private ArraySize As System.Windows.Forms.DomainUpDown
		
		Public Dim NewVarName As String = ""
		Public Dim NewVarType As String = ""
		Public Dim NewArraySize As String = ""
		
		Public Enum EditOptions
			NO_ARRAY
			ARRAY_ONLY
			SIZELESS_ARRAY
			ANY
		End Enum
		
		Private Dim NoArraySize, ArrayOnly As Boolean
		
		Public Sub New()
			MyBase.New
			'
			' The Me.InitializeComponent call is required for Windows Forms designer support.
			'
			Me.InitializeComponent
			
		End Sub
		
		#Region " Windows Forms Designer generated code "
		' This method is required for Windows Forms designer support.
		' Do not change the method contents inside the source code editor. The Forms designer might
		' not be able to load this method if it was changed manually.
		Private Sub InitializeComponent()
			Me.VarName = New System.Windows.Forms.TextBox
			Me.ArraySizeLabel = New System.Windows.Forms.Label
			Me.EditVarHelp = New System.Windows.Forms.HelpProvider
			Me.VarType = New Great_Cow_Graphical_BASIC.CaptionedComboBox
			Me.Button_Cancel = New System.Windows.Forms.Button
			Me.Button_OK = New System.Windows.Forms.Button
			Me.label2 = New System.Windows.Forms.Label
			Me.label3 = New System.Windows.Forms.Label
			Me.ArraySize = New System.Windows.Forms.DomainUpDown
			Me.SuspendLayout
			'
			'VarName
			'
			Me.EditVarHelp.SetHelpString(Me.VarName, "The name for the variable.")
			Me.VarName.Location = New System.Drawing.Point(8, 24)
			Me.VarName.Name = "VarName"
			Me.EditVarHelp.SetShowHelp(Me.VarName, true)
			Me.VarName.Size = New System.Drawing.Size(232, 20)
			Me.VarName.TabIndex = 1
			'
			'ArraySizeLabel
			'
			Me.ArraySizeLabel.Location = New System.Drawing.Point(24, 136)
			Me.ArraySizeLabel.Name = "ArraySizeLabel"
			Me.ArraySizeLabel.Size = New System.Drawing.Size(96, 16)
			Me.ArraySizeLabel.TabIndex = 3
			Me.ArraySizeLabel.Text = "Items in array"
			Me.ArraySizeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
			'
			'VarType
			'
			Me.EditVarHelp.SetHelpString(Me.VarType, "Type of data that will be stored in this variable.")
			Me.VarType.ListFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
			Me.VarType.Location = New System.Drawing.Point(8, 72)
			Me.VarType.Name = "VarType"
			Me.VarType.SelectedID = ""
			Me.VarType.SelectedIndex = -1
			Me.VarType.SelectedObject = Nothing
			Me.EditVarHelp.SetShowHelp(Me.VarType, true)
			Me.VarType.Size = New System.Drawing.Size(232, 56)
			Me.VarType.TabIndex = 2
			AddHandler Me.VarType.SelectedIndexChanged, AddressOf Me.VarTypeSelectedIndexChanged
			'
			'Button_Cancel
			'
			Me.Button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me.Button_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.Button_Cancel.Location = New System.Drawing.Point(136, 176)
			Me.Button_Cancel.Name = "Button_Cancel"
			Me.Button_Cancel.Size = New System.Drawing.Size(88, 24)
			Me.Button_Cancel.TabIndex = 5
			Me.Button_Cancel.Text = "Cancel"
			'
			'Button_OK
			'
			Me.Button_OK.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.Button_OK.Location = New System.Drawing.Point(24, 176)
			Me.Button_OK.Name = "Button_OK"
			Me.Button_OK.Size = New System.Drawing.Size(88, 24)
			Me.Button_OK.TabIndex = 4
			Me.Button_OK.Text = "OK"
			AddHandler Me.Button_OK.Click, AddressOf Me.Button_OKClick
			'
			'label2
			'
			Me.label2.Location = New System.Drawing.Point(8, 8)
			Me.label2.Name = "label2"
			Me.label2.Size = New System.Drawing.Size(88, 16)
			Me.label2.TabIndex = 22
			Me.label2.Text = "Name"
			'
			'label3
			'
			Me.label3.Location = New System.Drawing.Point(8, 56)
			Me.label3.Name = "label3"
			Me.label3.Size = New System.Drawing.Size(104, 16)
			Me.label3.TabIndex = 23
			Me.label3.Text = "Data type stored"
			'
			'ArraySize
			'
			Me.ArraySize.Location = New System.Drawing.Point(128, 136)
			Me.ArraySize.Name = "ArraySize"
			Me.ArraySize.Size = New System.Drawing.Size(112, 20)
			Me.ArraySize.TabIndex = 3
			Me.ArraySize.Text = "1"
			'
			'EditVar
			'
			Me.AcceptButton = Me.Button_OK
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.CancelButton = Me.Button_Cancel
			Me.ClientSize = New System.Drawing.Size(251, 208)
			Me.Controls.Add(Me.ArraySize)
			Me.Controls.Add(Me.VarType)
			Me.Controls.Add(Me.label3)
			Me.Controls.Add(Me.ArraySizeLabel)
			Me.Controls.Add(Me.label2)
			Me.Controls.Add(Me.VarName)
			Me.Controls.Add(Me.Button_Cancel)
			Me.Controls.Add(Me.Button_OK)
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
			Me.HelpButton = true
			Me.MaximizeBox = false
			Me.MinimizeBox = false
			Me.Name = "EditVar"
			Me.ShowInTaskbar = false
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
			Me.Text = "Edit Variable"
			Me.ResumeLayout(false)
			Me.PerformLayout
		End Sub
		Private ArraySizeLabel As System.Windows.Forms.Label
		Private VarType As Great_Cow_Graphical_BASIC.CaptionedComboBox
		Private label3 As System.Windows.Forms.Label
		Private label2 As System.Windows.Forms.Label
		#End Region
		
		Public Sub InitEdit(ByVal NewName As String, ByVal NewType As String, ByVal NewSize As String, ByVal Options As EditOptions)
			
			Dim AddArray As Boolean = True
			Dim AddScalars As Boolean = True
			
			'Decide what needs to be shown on the dialog
			AddArray = True
			NoArraySize = False
			ArrayOnly = False
				
			'Don't show array or array size (was -1)
			If Options = EditOptions.NO_ARRAY Then
				AddArray = False
				Me.ArraySize.Enabled = False
			
			'Show array option but don't allow size to be entered (was -2)
			ElseIf Options = EditOptions.SIZELESS_ARRAY Then
				NoArraySize = True
				NewSize = -1
				
			'Allow only array, no scalar variables (was -3)
			ElseIf Options = EditOptions.ARRAY_ONLY Then
				ArrayOnly = True
				AddScalars = False
				NewType = "array"
				NewSize = "10"
			End If
			
			'Fill type box
			If AddScalars Then
				VarType.Add(New CaptionedListItem("BIT", "Bit", "Stores a 0 or a 1"))
				VarType.Add(New CaptionedListItem("BYTE", "Byte", "Stores a whole number between 0 and 255"))
				VarType.Add(New CaptionedListItem("WORD", "Word", "Stores a whole number between 0 and 65535"))
				VarType.Add(New CaptionedListItem("INTEGER", "Integer", "Stores a whole number between -32768 and 32767"))
				VarType.Add(New CaptionedListItem("LONG", "Long", "Stores a whole number between 0 and 4.2 billion"))
				VarType.Add(New CaptionedListItem("STRING", "String", "Stores text (any letters, numbers and punctuation)"))
			End If
			If AddArray Then
				VarType.Add(New CaptionedListItem("ARRAY", "Array", "Stores a list of numbers between 0 and 255"))
			End If
			
			'Fill domain for array size
			Dim PossibleSize As Integer
			For PossibleSize = 1 to 1000
				ArraySize.Items.Add(PossibleSize.ToString)
			Next
			
			'Load data from parameters into form
			'Variable name
			Me.VarName.Text = NewName
			'Variable type
			VarType.SelectedID = NewType
			'Hide array size entry unless we have an array
			Me.ArraySize.Text = NewSize
			Me.ArraySizeLabel.Visible = False
			Me.ArraySize.Visible = False
			
			If NewType.ToLower = "array" And Not NoArraySize Then
				Me.ArraySize.Visible = True
				Me.ArraySizeLabel.Visible = True
			End If
			
			VarName.Select
		End Sub
		
		Private Sub Button_OKClick(sender As System.Object, e As System.EventArgs)
			'Check name
			Dim NameCheck As String = LowLevel.CheckName(VarName.Text)
			If NameCheck <> "" Then
				If NameCheck.Length = 1 Then MessageBox.Show("The variable name contains an invalid symbol (" + NameCheck + ")", "Invalid Name", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
				If NameCheck.Length > 1 Then MessageBox.Show("The variable name contains invalid symbols (" + NameCheck + ")", "Invalid Name", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
				Exit Sub
			End If
			
			'If name ok, pass back to caller
			NewVarName = Me.VarName.Text.Trim
			NewVarType = Me.VarType.SelectedID.ToLower
			If NewVarType = "array" Then
				NewArraySize = Me.ArraySize.Text
			End If
			
'			If Me.Type_Bit.Checked Then NewVarType = "bit"
'			If Me.Type_Byte.Checked Then NewVarType = "byte"
'			If Me.Type_Word.Checked Then NewVarType = "word"
'			If me.Type_Integer.Checked Then NewVarType = "integer"
'			If Me.Type_String.Checked Then NewVarType = "string"
'			If Me.Type_Array.Checked Then
'				NewArraySize = Me.ArraySize.Value
'				NewVarType = "array"
'			End If
			
			Me.Close()
		End Sub
		
		Sub VarTypeSelectedIndexChanged(sender As Object, e As EventArgs)
			If Not NoArraySize Then
				If VarType.SelectedID.ToLower = "array" Then
					Me.ArraySize.Visible = True
					Me.ArraySizeLabel.Visible = True
				Else
					Me.ArraySize.Visible = False
					Me.ArraySizeLabel.Visible = False
				End If
			End If
		End Sub
		
	End Class
''End Namespace
