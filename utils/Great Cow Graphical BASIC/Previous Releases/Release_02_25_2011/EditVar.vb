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

Namespace Great_Cow_Graphical_BASIC
	
	Public Class EditVar
		Inherits System.Windows.Forms.Form
		Private Type_Bit As System.Windows.Forms.RadioButton
		Private Type_String As System.Windows.Forms.RadioButton
		Private groupBox2 As System.Windows.Forms.GroupBox
		Private Type_Word As System.Windows.Forms.RadioButton
		Private groupBox1 As System.Windows.Forms.GroupBox
		Private Button_OK As System.Windows.Forms.Button
		Private Button_Cancel As System.Windows.Forms.Button
		Private EditVarHelp As System.Windows.Forms.HelpProvider
		Private label1 As System.Windows.Forms.Label
		Private VarName As System.Windows.Forms.TextBox
		Private ArraySize As System.Windows.Forms.NumericUpDown
		Private Type_Array As System.Windows.Forms.RadioButton
		Private Type_Byte As System.Windows.Forms.RadioButton
		Private Type_Integer As System.Windows.Forms.RadioButton
		
		Public Dim NewVarName As String = ""
		Public Dim NewVarType As String = ""
		Public Dim NewArraySize As Integer = 1
		
		Private Dim NoArrays, NoArraySize, ArrayOnly As Boolean
		
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
			Me.Type_Integer = New System.Windows.Forms.RadioButton
			Me.Type_Byte = New System.Windows.Forms.RadioButton
			Me.Type_Array = New System.Windows.Forms.RadioButton
			Me.ArraySize = New System.Windows.Forms.NumericUpDown
			Me.VarName = New System.Windows.Forms.TextBox
			Me.label1 = New System.Windows.Forms.Label
			Me.EditVarHelp = New System.Windows.Forms.HelpProvider
			Me.Button_Cancel = New System.Windows.Forms.Button
			Me.Button_OK = New System.Windows.Forms.Button
			Me.groupBox1 = New System.Windows.Forms.GroupBox
			Me.Type_Word = New System.Windows.Forms.RadioButton
			Me.groupBox2 = New System.Windows.Forms.GroupBox
			Me.Type_String = New System.Windows.Forms.RadioButton
			Me.Type_Bit = New System.Windows.Forms.RadioButton
			CType(Me.ArraySize,System.ComponentModel.ISupportInitialize).BeginInit
			Me.groupBox1.SuspendLayout
			Me.groupBox2.SuspendLayout
			Me.SuspendLayout
			'
			'Type_Integer
			'
			Me.Type_Integer.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.EditVarHelp.SetHelpString(Me.Type_Integer, "Integer variables use 2 bytes of RAM and can store values between -32768 and 3276"& _ 
"7.")
			Me.Type_Integer.Location = New System.Drawing.Point(8, 76)
			Me.Type_Integer.Name = "Type_Integer"
			Me.EditVarHelp.SetShowHelp(Me.Type_Integer, true)
			Me.Type_Integer.Size = New System.Drawing.Size(144, 16)
			Me.Type_Integer.TabIndex = 5
			Me.Type_Integer.Text = "Integer"
			'
			'Type_Byte
			'
			Me.Type_Byte.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.EditVarHelp.SetHelpString(Me.Type_Byte, "Byte variables use 1 byte of RAM and can store values between 0 and 255.")
			Me.Type_Byte.Location = New System.Drawing.Point(8, 36)
			Me.Type_Byte.Name = "Type_Byte"
			Me.EditVarHelp.SetShowHelp(Me.Type_Byte, true)
			Me.Type_Byte.Size = New System.Drawing.Size(144, 16)
			Me.Type_Byte.TabIndex = 3
			Me.Type_Byte.Text = "Byte"
			'
			'Type_Array
			'
			Me.Type_Array.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.EditVarHelp.SetHelpString(Me.Type_Array, "Array variables can store multiple values (elements) simultaneously, each of whic"& _ 
"h can be accessed individually. Each value can be between 0 and 255.")
			Me.Type_Array.Location = New System.Drawing.Point(8, 116)
			Me.Type_Array.Name = "Type_Array"
			Me.EditVarHelp.SetShowHelp(Me.Type_Array, true)
			Me.Type_Array.Size = New System.Drawing.Size(56, 16)
			Me.Type_Array.TabIndex = 7
			Me.Type_Array.Text = "Array"
			AddHandler Me.Type_Array.CheckedChanged, AddressOf Me.Type_ArrayCheckedChanged
			'
			'ArraySize
			'
			Me.ArraySize.Enabled = false
			Me.EditVarHelp.SetHelpString(Me.ArraySize, "The number of elements in the array. Each element uses 1 byte of RAM.")
			Me.ArraySize.Location = New System.Drawing.Point(64, 132)
			Me.ArraySize.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
			Me.ArraySize.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
			Me.ArraySize.Name = "ArraySize"
			Me.EditVarHelp.SetShowHelp(Me.ArraySize, true)
			Me.ArraySize.Size = New System.Drawing.Size(56, 20)
			Me.ArraySize.TabIndex = 8
			Me.ArraySize.Value = New Decimal(New Integer() {1, 0, 0, 0})
			'
			'VarName
			'
			Me.EditVarHelp.SetHelpString(Me.VarName, "The name for the variable.")
			Me.VarName.Location = New System.Drawing.Point(8, 16)
			Me.VarName.Name = "VarName"
			Me.EditVarHelp.SetShowHelp(Me.VarName, true)
			Me.VarName.Size = New System.Drawing.Size(168, 20)
			Me.VarName.TabIndex = 1
			Me.VarName.Text = ""
			'
			'label1
			'
			Me.label1.Location = New System.Drawing.Point(32, 136)
			Me.label1.Name = "label1"
			Me.label1.Size = New System.Drawing.Size(32, 16)
			Me.label1.TabIndex = 3
			Me.label1.Text = "Size:"
			'
			'Button_Cancel
			'
			Me.Button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me.Button_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.Button_Cancel.Location = New System.Drawing.Point(104, 232)
			Me.Button_Cancel.Name = "Button_Cancel"
			Me.Button_Cancel.Size = New System.Drawing.Size(88, 24)
			Me.Button_Cancel.TabIndex = 10
			Me.Button_Cancel.Text = "Cancel"
			'
			'Button_OK
			'
			Me.Button_OK.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.Button_OK.Location = New System.Drawing.Point(8, 232)
			Me.Button_OK.Name = "Button_OK"
			Me.Button_OK.Size = New System.Drawing.Size(88, 24)
			Me.Button_OK.TabIndex = 9
			Me.Button_OK.Text = "OK"
			AddHandler Me.Button_OK.Click, AddressOf Me.Button_OKClick
			'
			'groupBox1
			'
			Me.groupBox1.Controls.Add(Me.VarName)
			Me.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.groupBox1.Location = New System.Drawing.Point(8, 8)
			Me.groupBox1.Name = "groupBox1"
			Me.groupBox1.Size = New System.Drawing.Size(184, 48)
			Me.groupBox1.TabIndex = 20
			Me.groupBox1.TabStop = false
			Me.groupBox1.Text = "Name"
			'
			'Type_Word
			'
			Me.Type_Word.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.EditVarHelp.SetHelpString(Me.Type_Word, "Word variables use 2 bytes of RAM and can store values between 0 and 65535.")
			Me.Type_Word.Location = New System.Drawing.Point(8, 56)
			Me.Type_Word.Name = "Type_Word"
			Me.EditVarHelp.SetShowHelp(Me.Type_Word, true)
			Me.Type_Word.Size = New System.Drawing.Size(144, 16)
			Me.Type_Word.TabIndex = 4
			Me.Type_Word.Text = "Word"
			'
			'groupBox2
			'
			Me.groupBox2.Controls.Add(Me.Type_Integer)
			Me.groupBox2.Controls.Add(Me.Type_String)
			Me.groupBox2.Controls.Add(Me.Type_Bit)
			Me.groupBox2.Controls.Add(Me.ArraySize)
			Me.groupBox2.Controls.Add(Me.label1)
			Me.groupBox2.Controls.Add(Me.Type_Array)
			Me.groupBox2.Controls.Add(Me.Type_Word)
			Me.groupBox2.Controls.Add(Me.Type_Byte)
			Me.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.groupBox2.Location = New System.Drawing.Point(8, 64)
			Me.groupBox2.Name = "groupBox2"
			Me.groupBox2.Size = New System.Drawing.Size(184, 160)
			Me.groupBox2.TabIndex = 21
			Me.groupBox2.TabStop = false
			Me.groupBox2.Text = "Type"
			'
			'Type_String
			'
			Me.Type_String.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.EditVarHelp.SetHelpString(Me.Type_String, "String variables use 41 bytes of RAM, and can store up to 40 letters, numbers or "& _ 
"symbols")
			Me.Type_String.Location = New System.Drawing.Point(8, 96)
			Me.Type_String.Name = "Type_String"
			Me.EditVarHelp.SetShowHelp(Me.Type_String, true)
			Me.Type_String.Size = New System.Drawing.Size(144, 16)
			Me.Type_String.TabIndex = 6
			Me.Type_String.Text = "String"
			'
			'Type_Bit
			'
			Me.Type_Bit.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.EditVarHelp.SetHelpString(Me.Type_Bit, "Bit variables use 1 bit of RAM, and can store values 0 and 1")
			Me.Type_Bit.Location = New System.Drawing.Point(8, 16)
			Me.Type_Bit.Name = "Type_Bit"
			Me.EditVarHelp.SetShowHelp(Me.Type_Bit, true)
			Me.Type_Bit.Size = New System.Drawing.Size(144, 16)
			Me.Type_Bit.TabIndex = 2
			Me.Type_Bit.Text = "Bit"
			'
			'EditVar
			'
			Me.AcceptButton = Me.Button_OK
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.CancelButton = Me.Button_Cancel
			Me.ClientSize = New System.Drawing.Size(202, 263)
			Me.Controls.Add(Me.Button_Cancel)
			Me.Controls.Add(Me.Button_OK)
			Me.Controls.Add(Me.groupBox2)
			Me.Controls.Add(Me.groupBox1)
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
			Me.HelpButton = true
			Me.MaximizeBox = false
			Me.MinimizeBox = false
			Me.Name = "EditVar"
			Me.ShowInTaskbar = false
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
			Me.Text = "Edit Variable"
			CType(Me.ArraySize,System.ComponentModel.ISupportInitialize).EndInit
			Me.groupBox1.ResumeLayout(false)
			Me.groupBox2.ResumeLayout(false)
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
		Public Sub InitEdit(ByVal NewName As String, ByVal NewType As String, ByVal NewSize As Integer)
			
			NoArrays = False
			NoArraySize = False
			ArrayOnly = False
			If NewSize < 0 Then
				
				If NewSize = -1 Then
					NoArrays = True
					Me.Type_Array.Enabled = False
					Me.ArraySize.Enabled = False
				ElseIf NewSize = -2 Then
					NoArraySize = True
					Me.ArraySize.Enabled = False
					NewSize = -1
				ElseIf NewSize = -3 Then
					ArrayOnly = True
					Me.Type_Bit.Enabled = False
					Me.Type_Byte.Enabled = False
					Me.Type_Word.Enabled = False
					Me.Type_Integer.Enabled = False
					NewType = "array"
					NewSize = -10
				End If
				NewSize *= -1
			End If
			
			If NewSize = 0 Then NewSize = 1
			
			Me.VarName.Text = NewName
			Me.ArraySize.Value = NewSize
			
			Select Case NewType.ToLower
				Case "bit"
					Me.Type_Bit.Select
					Me.ArraySize.Enabled = False
				Case "word"
					Me.Type_Word.Select
					Me.ArraySize.Enabled = False
				Case "integer"
					Me.Type_Integer.Select
					Me.ArraySize.Enabled = False
				Case "string"
					Me.Type_String.Select
					Me.ArraySize.Enabled = False
				Case "array"
					Me.Type_Array.Select
					If Not NoArraySize Then Me.ArraySize.Enabled = True
				Case Else
					Me.Type_Byte.Select
					Me.ArraySize.Enabled = False
			End Select
			
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
			If Me.Type_Bit.Checked Then NewVarType = "bit"
			If Me.Type_Byte.Checked Then NewVarType = "byte"
			If Me.Type_Word.Checked Then NewVarType = "word"
			If me.Type_Integer.Checked Then NewVarType = "integer"
			If Me.Type_String.Checked Then NewVarType = "string"
			If Me.Type_Array.Checked Then
				NewArraySize = Me.ArraySize.Value
				NewVarType = "array"
			End If
			
			Me.Close()
		End Sub
		
		Private Sub Type_ArrayCheckedChanged(sender As System.Object, e As System.EventArgs)
			If Not NoArraySize Then
				If Me.Type_Array.Checked Then Me.ArraySize.Enabled = True
				If Not Me.Type_Array.Checked Then Me.ArraySize.Enabled = False
			End If
		End Sub
		
	End Class
End Namespace
