'
' Created by SharpDevelop.
' User: Administrator
' Date: 31/05/2007
' Time: 9:11 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.Windows.Forms

Namespace Great_Cow_Graphical_BASIC
	
	Public Class TableNameInput
		Inherits System.Windows.Forms.Form
		Private NewTableHelp As System.Windows.Forms.HelpProvider
		Private Button_OK As System.Windows.Forms.Button
		Private Button_Cancel As System.Windows.Forms.Button
		Private NewName As System.Windows.Forms.TextBox
		Private label1 As System.Windows.Forms.Label
		
		Private Dim Mode As String = ""
		Public Dim TableName As String = ""
		
		Public Sub New(Mode As String, Optional TableName As String = "")
			MyBase.New
			'
			' The Me.InitializeComponent call is required for Windows Forms designer support.
			'
			Me.InitializeComponent
			
			Me.Mode = Mode
			Me.Text = Mode + " " + Me.Text
			
			NewName.Text = TableName
			
		End Sub
		
		#Region " Windows Forms Designer generated code "
		' This method is required for Windows Forms designer support.
		' Do not change the method contents inside the source code editor. The Forms designer might
		' not be able to load this method if it was changed manually.
		Private Sub InitializeComponent()
			Me.label1 = New System.Windows.Forms.Label
			Me.NewName = New System.Windows.Forms.TextBox
			Me.Button_Cancel = New System.Windows.Forms.Button
			Me.Button_OK = New System.Windows.Forms.Button
			Me.NewTableHelp = New System.Windows.Forms.HelpProvider
			Me.SuspendLayout
			'
			'label1
			'
			Me.label1.Location = New System.Drawing.Point(8, 8)
			Me.label1.Name = "label1"
			Me.label1.Size = New System.Drawing.Size(112, 16)
			Me.label1.TabIndex = 2
			Me.label1.Text = "Data Table Name:"
			'
			'NewName
			'
			Me.NewTableHelp.SetHelpString(Me.NewName, "Name of the new data table.")
			Me.NewName.Location = New System.Drawing.Point(8, 24)
			Me.NewName.Name = "NewName"
			Me.NewTableHelp.SetShowHelp(Me.NewName, true)
			Me.NewName.Size = New System.Drawing.Size(216, 21)
			Me.NewName.TabIndex = 0
			Me.NewName.Text = ""
			'
			'Button_Cancel
			'
			Me.Button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me.Button_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.Button_Cancel.Location = New System.Drawing.Point(120, 56)
			Me.Button_Cancel.Name = "Button_Cancel"
			Me.Button_Cancel.Size = New System.Drawing.Size(96, 24)
			Me.Button_Cancel.TabIndex = 2
			Me.Button_Cancel.Text = "Cancel"
			'
			'Button_OK
			'
			Me.Button_OK.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.Button_OK.Location = New System.Drawing.Point(16, 56)
			Me.Button_OK.Name = "Button_OK"
			Me.Button_OK.Size = New System.Drawing.Size(96, 24)
			Me.Button_OK.TabIndex = 1
			Me.Button_OK.Text = "OK"
			AddHandler Me.Button_OK.Click, AddressOf Me.Button_OKClick
			'
			'TableNameInput
			'
			Me.AcceptButton = Me.Button_OK
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 14)
			Me.CancelButton = Me.Button_Cancel
			Me.ClientSize = New System.Drawing.Size(234, 87)
			Me.Controls.Add(Me.NewName)
			Me.Controls.Add(Me.label1)
			Me.Controls.Add(Me.Button_Cancel)
			Me.Controls.Add(Me.Button_OK)
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
			Me.HelpButton = true
			Me.MaximizeBox = false
			Me.MinimizeBox = false
			Me.Name = "TableNameInput"
			Me.ShowInTaskbar = false
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
			Me.Text = "Data Table"
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
		Private Sub Button_OKClick(sender As System.Object, e As System.EventArgs)
			Dim NameCheck As String = LowLevel.CheckName(NewName.Text)
			If NameCheck <> "" Then
				If NameCheck.Length = 1 Then MessageBox.Show("The table name contains an invalid symbol (" + NameCheck + ")", "Invalid Name", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
				If NameCheck.Length > 1 Then MessageBox.Show("The table name contains invalid symbols (" + NameCheck + ")", "Invalid Name", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
				Exit Sub
			End If
			
			TableName = NewName.Text
			Me.Close
		End Sub
		
	End Class
End Namespace
