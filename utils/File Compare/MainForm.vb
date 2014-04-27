'
' Created by SharpDevelop.
' User: Hugh
' Date: 17/07/2011
' Time: 1:46 AM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Imports System
Imports System.Drawing
Imports System.Windows.Forms

Namespace File_Compare
	
	Public Class MainForm
		Inherits System.Windows.Forms.Form
		Private FileContents As System.Windows.Forms.ListBox
		Private buttonLoadA As System.Windows.Forms.Button
		Private buttonLoadB As System.Windows.Forms.Button
		Private labelB As System.Windows.Forms.Label
		Private labelA As System.Windows.Forms.Label
		Private openFileDialogB As System.Windows.Forms.OpenFileDialog
		Private openFileDialogA As System.Windows.Forms.OpenFileDialog
		
		Private FileA As IntelHexFile
		Private FileB As IntelHexFile
		
		Public Const InstallDir As String = "C:\Final\utils\File Compare"
		
		Public Shared Sub Main
			Dim fMainForm As New MainForm
			fMainForm.ShowDialog()
		End Sub
		
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
			Me.openFileDialogA = New System.Windows.Forms.OpenFileDialog
			Me.openFileDialogB = New System.Windows.Forms.OpenFileDialog
			Me.labelA = New System.Windows.Forms.Label
			Me.labelB = New System.Windows.Forms.Label
			Me.buttonLoadB = New System.Windows.Forms.Button
			Me.buttonLoadA = New System.Windows.Forms.Button
			Me.FileContents = New System.Windows.Forms.ListBox
			Me.SuspendLayout
			'
			'openFileDialogA
			'
			Me.openFileDialogA.Filter = "Hex files|*.hex"
			Me.openFileDialogA.Title = "Open File A"
			AddHandler Me.openFileDialogA.FileOk, AddressOf Me.OpenFileDialogAFileOk
			'
			'openFileDialogB
			'
			Me.openFileDialogB.Filter = "Hex files|*.hex"
			Me.openFileDialogB.Title = "Open File B"
			AddHandler Me.openFileDialogB.FileOk, AddressOf Me.OpenFileDialogBFileOk
			'
			'labelA
			'
			Me.labelA.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.labelA.Location = New System.Drawing.Point(256, 48)
			Me.labelA.Name = "labelA"
			Me.labelA.Size = New System.Drawing.Size(160, 64)
			Me.labelA.TabIndex = 3
			Me.labelA.Text = "No File"
			'
			'labelB
			'
			Me.labelB.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.labelB.Location = New System.Drawing.Point(256, 176)
			Me.labelB.Name = "labelB"
			Me.labelB.Size = New System.Drawing.Size(160, 64)
			Me.labelB.TabIndex = 4
			Me.labelB.Text = "No File"
			'
			'buttonLoadB
			'
			Me.buttonLoadB.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.buttonLoadB.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.buttonLoadB.Location = New System.Drawing.Point(256, 136)
			Me.buttonLoadB.Name = "buttonLoadB"
			Me.buttonLoadB.Size = New System.Drawing.Size(160, 32)
			Me.buttonLoadB.TabIndex = 1
			Me.buttonLoadB.Text = "Load File B ..."
			AddHandler Me.buttonLoadB.Click, AddressOf Me.ButtonLoadBClick
			'
			'buttonLoadA
			'
			Me.buttonLoadA.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.buttonLoadA.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.buttonLoadA.Location = New System.Drawing.Point(256, 8)
			Me.buttonLoadA.Name = "buttonLoadA"
			Me.buttonLoadA.Size = New System.Drawing.Size(160, 32)
			Me.buttonLoadA.TabIndex = 0
			Me.buttonLoadA.Text = "Load File A ..."
			AddHandler Me.buttonLoadA.Click, AddressOf Me.ButtonLoadAClick
			'
			'FileContents
			'
			Me.FileContents.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.FileContents.IntegralHeight = false
			Me.FileContents.Location = New System.Drawing.Point(8, 8)
			Me.FileContents.Name = "FileContents"
			Me.FileContents.Size = New System.Drawing.Size(240, 407)
			Me.FileContents.TabIndex = 2
			'
			'MainForm
			'
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.ClientSize = New System.Drawing.Size(424, 426)
			Me.Controls.Add(Me.labelB)
			Me.Controls.Add(Me.labelA)
			Me.Controls.Add(Me.FileContents)
			Me.Controls.Add(Me.buttonLoadB)
			Me.Controls.Add(Me.buttonLoadA)
			Me.Name = "MainForm"
			Me.Text = "Compare Intel Hex Files"
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
		Private Sub ButtonLoadAClick(sender As System.Object, e As System.EventArgs)
			OpenFileDialogA.ShowDialog
		End Sub
		
		Private Sub ButtonLoadBClick(sender As System.Object, e As System.EventArgs)
			OpenFileDialogB.ShowDialog
		End Sub
		
		Private Sub OpenFileDialogBFileOk(sender As System.Object, e As System.ComponentModel.CancelEventArgs)
			FileB = New IntelHexFile(OpenFileDialogB.FileName)
			labelB.Text = OpenFileDialogB.FileName
			UpdateDisplay
		End Sub
		
		Private Sub OpenFileDialogAFileOk(sender As System.Object, e As System.ComponentModel.CancelEventArgs)
			FileA = New IntelHexFile(OpenFileDialogA.FileName)
			labelA.Text = OpenFileDialogA.FileName
			UpdateDisplay
		End Sub
		
		Private Sub UpdateDisplay
			If Not FileA Is Nothing And Not FileB Is Nothing Then
				
				FileContents.Items.Clear
				Dim CurrLoc As Integer
				Dim ValLoc, ValA, ValB, DiffText As String
				For CurrLoc = 0 To &H3FFF
					ValLoc = Utils.Dec2Other(CurrLoc * 2, 16)
					ValA = Utils.Dec2Other(FileA.GetByte(CurrLoc * 2 + 1, 255) * 256 + FileA.GetByte(CurrLoc * 2, 255), 16)
					ValB = Utils.Dec2Other(FileB.GetByte(CurrLoc * 2 + 1, 255) * 256 + FileB.GetByte(CurrLoc * 2, 255), 16)
					
					Do While ValLoc.Length < 4
						ValLoc = "0" + ValLoc
					Loop
					Do While ValA.Length < 4
						ValA = "0" + ValA
					Loop
					Do While ValB.Length < 4
						ValB = "0" + ValB
					Loop
					
					DiffText = ""
					If ValA <> ValB Then
						DiffText = "    Different!"
					End If
					FileContents.Items.Add(ValLoc + ": " + ValA + ", " + ValB + DiffText)
				Next
				
			End If
			
		End Sub
		
	End Class
End Namespace
