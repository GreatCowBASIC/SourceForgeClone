'
' Created by SharpDevelop.
' User: Administrator
' Date: 4/02/2007
' Time: 9:22 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.Windows.Forms

Namespace Great_Cow_Graphical_BASIC
	
	Public Class LibraryWindow
		Inherits System.Windows.Forms.Form
		Private AddFileLib As System.Windows.Forms.Button
		Private groupBox3 As System.Windows.Forms.GroupBox
		Private groupBox2 As System.Windows.Forms.GroupBox
		Private groupBox1 As System.Windows.Forms.GroupBox
		Private Cancel_Button As System.Windows.Forms.Button
		Private AddGCBLib As System.Windows.Forms.Button
		Private LibraryChooser As System.Windows.Forms.OpenFileDialog
		Private LibHelp As System.Windows.Forms.HelpProvider
		Private GCBLibs As System.Windows.Forms.ListBox
		Private DeleteLib As System.Windows.Forms.Button
		Private OK_Button As System.Windows.Forms.Button
		Private LibraryList As System.Windows.Forms.ListBox
		Private AddUserLib As System.Windows.Forms.Button
		Private UserLibs As System.Windows.Forms.ListBox
		
		Private Dim CurrentFile As GCBProgram
		
		Public Sub New(CurrentFile As GCBProgram)
			MyBase.New
			
			Dim Temp As Integer
			Dim TempData As String
			'
			' The Me.InitializeComponent call is required for Windows Forms designer support.
			'
			Me.InitializeComponent
			
			'Get current program
			Me.CurrentFile = CurrentFile
			
			With CurrentFile
				
				'Draw current include list
				Dim LibTemp As LibraryType
				For Each LibTemp In CurrentFile.Libraries
					Me.LibraryList.Items.Add(LibTemp.FileName)
				Next
				
				'Draw list of GCBASIC standard includes
				Dim LibDir As New IO.DirectoryInfo(MainForm.InstallDir + Mainform.FilenameIncludeDir)
				Dim LibList As IO.FileInfo() = LibDir.GetFiles("*.h")
				Dim LibFileName As IO.FileInfo
				For Each LibFileName In LibList
					Me.GCBLibs.Items.Add("<" + LibFileName.Name + ">")
				Next
				
				'Draw list of libraries in program directory
				If .ProgramDir <> "" Then
					Dim IsValidType As Boolean
					Dim FileName, FileType As String
					
					Dim UserLibDir As New IO.DirectoryInfo(.ProgramDir)
					Dim UserLibList As IO.FileInfo() = UserLibDir.GetFiles("*.*")
					Dim UserLibFileName As IO.FileInfo
					For Each UserLibFileName In UserLibList
						
						'Get type
						FileName = UserLibFileName.Name
						FileType = ""
						For Temp = FileName.Length - 1 To 0 Step -1
							If FileName.Substring(Temp, 1) = "." Then
								FileType = FileName.Substring(Temp + 1).ToLower
								Exit For
							End If
						Next
						
						'Check type
						IsValidType = False
						If FileType = "h" Then
							IsValidType = True
						Else
							If Not MainForm.GetFileConverter(FileName) Is Nothing Then
								IsValidType = True
							End If
						End If
						
						'If type valid, add library to list
						If IsValidType Then
							Me.UserLibs.Items.Add("""" + FileName + """")
						End If
					Next
				End If
			End With
			
			'Update list of library types
			UpdateFileLoadTypeList
			
		End Sub
		
		#Region " Windows Forms Designer generated code "
		' This method is required for Windows Forms designer support.
		' Do not change the method contents inside the source code editor. The Forms designer might
		' not be able to load this method if it was changed manually.
		Private Sub InitializeComponent()
			Me.UserLibs = New System.Windows.Forms.ListBox
			Me.AddUserLib = New System.Windows.Forms.Button
			Me.LibraryList = New System.Windows.Forms.ListBox
			Me.OK_Button = New System.Windows.Forms.Button
			Me.DeleteLib = New System.Windows.Forms.Button
			Me.GCBLibs = New System.Windows.Forms.ListBox
			Me.LibHelp = New System.Windows.Forms.HelpProvider
			Me.LibraryChooser = New System.Windows.Forms.OpenFileDialog
			Me.AddGCBLib = New System.Windows.Forms.Button
			Me.Cancel_Button = New System.Windows.Forms.Button
			Me.groupBox1 = New System.Windows.Forms.GroupBox
			Me.groupBox2 = New System.Windows.Forms.GroupBox
			Me.groupBox3 = New System.Windows.Forms.GroupBox
			Me.AddFileLib = New System.Windows.Forms.Button
			Me.groupBox1.SuspendLayout
			Me.groupBox2.SuspendLayout
			Me.groupBox3.SuspendLayout
			Me.SuspendLayout
			'
			'UserLibs
			'
			Me.LibHelp.SetHelpString(Me.UserLibs, "Libraries in the same folder as the program. To add one to the program, click on "& _ 
"it and then click the ""Add >"" button to the right.")
			Me.UserLibs.IntegralHeight = false
			Me.UserLibs.Location = New System.Drawing.Point(8, 16)
			Me.UserLibs.Name = "UserLibs"
			Me.LibHelp.SetShowHelp(Me.UserLibs, true)
			Me.UserLibs.Size = New System.Drawing.Size(120, 112)
			Me.UserLibs.TabIndex = 5
			AddHandler Me.UserLibs.DoubleClick, AddressOf Me.UserLibsDoubleClick
			AddHandler Me.UserLibs.SelectedIndexChanged, AddressOf Me.UserLibsSelectedIndexChanged
			'
			'AddUserLib
			'
			Me.AddUserLib.Enabled = false
			Me.AddUserLib.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.LibHelp.SetHelpString(Me.AddUserLib, "Adds a custom library")
			Me.AddUserLib.Location = New System.Drawing.Point(136, 56)
			Me.AddUserLib.Name = "AddUserLib"
			Me.LibHelp.SetShowHelp(Me.AddUserLib, true)
			Me.AddUserLib.Size = New System.Drawing.Size(64, 24)
			Me.AddUserLib.TabIndex = 9
			Me.AddUserLib.Text = "Add >"
			AddHandler Me.AddUserLib.Click, AddressOf Me.AddUserLibClick
			'
			'LibraryList
			'
			Me.LibHelp.SetHelpString(Me.LibraryList, "Displays a list of libraries that are currently used by the program")
			Me.LibraryList.Location = New System.Drawing.Point(8, 16)
			Me.LibraryList.Name = "LibraryList"
			Me.LibHelp.SetShowHelp(Me.LibraryList, true)
			Me.LibraryList.Size = New System.Drawing.Size(192, 251)
			Me.LibraryList.TabIndex = 0
			AddHandler Me.LibraryList.DoubleClick, AddressOf Me.LibraryListDoubleClick
			AddHandler Me.LibraryList.SelectedIndexChanged, AddressOf Me.LibraryListSelectedIndexChanged
			'
			'OK_Button
			'
			Me.OK_Button.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.OK_Button.Location = New System.Drawing.Point(120, 328)
			Me.OK_Button.Name = "OK_Button"
			Me.OK_Button.Size = New System.Drawing.Size(88, 24)
			Me.OK_Button.TabIndex = 1
			Me.OK_Button.Text = "OK"
			AddHandler Me.OK_Button.Click, AddressOf Me.OK_ButtonClick
			'
			'DeleteLib
			'
			Me.DeleteLib.Enabled = false
			Me.DeleteLib.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.LibHelp.SetHelpString(Me.DeleteLib, "Deletes a library from the program.")
			Me.DeleteLib.Location = New System.Drawing.Point(64, 280)
			Me.DeleteLib.Name = "DeleteLib"
			Me.LibHelp.SetShowHelp(Me.DeleteLib, true)
			Me.DeleteLib.Size = New System.Drawing.Size(72, 24)
			Me.DeleteLib.TabIndex = 10
			Me.DeleteLib.Text = "Delete"
			AddHandler Me.DeleteLib.Click, AddressOf Me.DeleteLibClick
			'
			'GCBLibs
			'
			Me.LibHelp.SetHelpString(Me.GCBLibs, "Libraries included with Great Cow Graphical BASIC. To add one to the program, cli"& _ 
"ck on it and then click the ""Add >"" button to the right.")
			Me.GCBLibs.IntegralHeight = false
			Me.GCBLibs.Location = New System.Drawing.Point(8, 16)
			Me.GCBLibs.Name = "GCBLibs"
			Me.LibHelp.SetShowHelp(Me.GCBLibs, true)
			Me.GCBLibs.Size = New System.Drawing.Size(120, 112)
			Me.GCBLibs.TabIndex = 4
			AddHandler Me.GCBLibs.DoubleClick, AddressOf Me.GCBLibsDoubleClick
			AddHandler Me.GCBLibs.SelectedIndexChanged, AddressOf Me.GCBLibsSelectedIndexChanged
			'
			'LibraryChooser
			'
			Me.LibraryChooser.DefaultExt = "h"
			Me.LibraryChooser.Filter = "GCBASIC Libraries|*.h|All files|*.*"
			Me.LibraryChooser.Title = "Add Library From File"
			AddHandler Me.LibraryChooser.FileOk, AddressOf Me.LibraryChooserFileOk
			'
			'AddGCBLib
			'
			Me.AddGCBLib.Enabled = false
			Me.AddGCBLib.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.LibHelp.SetHelpString(Me.AddGCBLib, "Adds an included library")
			Me.AddGCBLib.Location = New System.Drawing.Point(136, 56)
			Me.AddGCBLib.Name = "AddGCBLib"
			Me.LibHelp.SetShowHelp(Me.AddGCBLib, true)
			Me.AddGCBLib.Size = New System.Drawing.Size(64, 24)
			Me.AddGCBLib.TabIndex = 8
			Me.AddGCBLib.Text = "Add >"
			AddHandler Me.AddGCBLib.Click, AddressOf Me.AddGCBLibClick
			'
			'Cancel_Button
			'
			Me.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.Bottom
			Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me.Cancel_Button.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.Cancel_Button.Location = New System.Drawing.Point(232, 329)
			Me.Cancel_Button.Name = "Cancel_Button"
			Me.Cancel_Button.Size = New System.Drawing.Size(88, 24)
			Me.Cancel_Button.TabIndex = 2
			Me.Cancel_Button.Text = "Cancel"
			'
			'groupBox1
			'
			Me.groupBox1.Controls.Add(Me.LibraryList)
			Me.groupBox1.Controls.Add(Me.DeleteLib)
			Me.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.groupBox1.Location = New System.Drawing.Point(224, 8)
			Me.groupBox1.Name = "groupBox1"
			Me.groupBox1.Size = New System.Drawing.Size(208, 312)
			Me.groupBox1.TabIndex = 11
			Me.groupBox1.TabStop = false
			Me.groupBox1.Text = "Current Libraries"
			'
			'groupBox2
			'
			Me.groupBox2.Controls.Add(Me.GCBLibs)
			Me.groupBox2.Controls.Add(Me.AddGCBLib)
			Me.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.groupBox2.Location = New System.Drawing.Point(8, 8)
			Me.groupBox2.Name = "groupBox2"
			Me.groupBox2.Size = New System.Drawing.Size(208, 136)
			Me.groupBox2.TabIndex = 12
			Me.groupBox2.TabStop = false
			Me.groupBox2.Text = "Included Libraries"
			'
			'groupBox3
			'
			Me.groupBox3.Controls.Add(Me.UserLibs)
			Me.groupBox3.Controls.Add(Me.AddUserLib)
			Me.groupBox3.Controls.Add(Me.AddFileLib)
			Me.groupBox3.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.groupBox3.Location = New System.Drawing.Point(8, 152)
			Me.groupBox3.Name = "groupBox3"
			Me.groupBox3.Size = New System.Drawing.Size(208, 168)
			Me.groupBox3.TabIndex = 13
			Me.groupBox3.TabStop = false
			Me.groupBox3.Text = "Custom Libraries"
			'
			'AddFileLib
			'
			Me.AddFileLib.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.LibHelp.SetHelpString(Me.AddFileLib, "Allows a library to be added that isn't shown on either of the lists")
			Me.AddFileLib.Location = New System.Drawing.Point(24, 136)
			Me.AddFileLib.Name = "AddFileLib"
			Me.LibHelp.SetShowHelp(Me.AddFileLib, true)
			Me.AddFileLib.Size = New System.Drawing.Size(160, 23)
			Me.AddFileLib.TabIndex = 14
			Me.AddFileLib.Text = "Add From File ..."
			AddHandler Me.AddFileLib.Click, AddressOf Me.AddFileLibClick
			'
			'LibraryWindow
			'
			Me.AcceptButton = Me.OK_Button
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 14)
			Me.CancelButton = Me.Cancel_Button
			Me.ClientSize = New System.Drawing.Size(442, 360)
			Me.Controls.Add(Me.groupBox3)
			Me.Controls.Add(Me.groupBox2)
			Me.Controls.Add(Me.groupBox1)
			Me.Controls.Add(Me.Cancel_Button)
			Me.Controls.Add(Me.OK_Button)
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
			Me.HelpButton = true
			Me.MaximizeBox = false
			Me.MinimizeBox = false
			Me.Name = "LibraryWindow"
			Me.ShowInTaskbar = false
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
			Me.Text = "Libraries"
			Me.groupBox1.ResumeLayout(false)
			Me.groupBox2.ResumeLayout(false)
			Me.groupBox3.ResumeLayout(false)
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
		Private Sub LibraryWindowHelpRequested(sender As System.Object, hlpevent As System.Windows.Forms.HelpEventArgs)
			'MessageBox.Show("No help for you! (Not yet, anyway)", "Help Nazi", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1)
			
		End Sub
		
		Private Sub UpdateFileLoadTypeList
			
			'Remove "All Files"
			Dim FileFilter, AllFiles As String
			FileFilter = LibraryChooser.Filter
			AllFiles = FileFilter.Substring(FileFilter.ToLower.IndexOf("|all files"))
			FileFilter = FileFilter.Substring(0, FileFilter.ToLower.IndexOf("|all files"))
			
			'Find all importers, add names and types to File Open dialog
			Dim ThisConverter As FileConverter
			For Each ThisConverter In MainForm.FileConverters
				With ThisConverter
					FileFilter += ("|" + .GetDescription + "|" + "*." + .GetInType)
				End With
			Next
			
			'Re-add "All Files"
			LibraryChooser.Filter = FileFilter + AllFiles
			
		End Sub
		
		Private Sub OK_ButtonClick(sender As System.Object, e As System.EventArgs)
			Dim Temp As Integer
			Dim TempData As String
			
			With CurrentFile
				'Create new library list
				.Libraries.Clear
				For Temp = 0 To Me.LibraryList.Items.Count - 1
					Me.LibraryList.SelectedIndex = Temp
					TempData = Me.LibraryList.SelectedItem
					If TempData <> "" Then
						.Libraries.Add(New LibraryType(TempData))
					End If
				Next
				
				'Update library data
				CurrentFile.LoadIncludes
				'RaiseEvent LibraryListChanged()
				.FireLibraryListChanged
				
				
				'Set unsaved changes flag
				.HasChanged = True
			End With
			
			'Exit
			Me.Close()
			
		End Sub
		
		Private Sub DeleteLibClick(sender As System.Object, e As System.EventArgs)
			If Me.LibraryList.SelectedIndex = -1 Then Exit Sub
			
			Me.LibraryList.Items.Remove(Me.LibraryList.SelectedItem)
		End Sub
		
		Private Sub AddGCBLibClick(sender As System.Object, e As System.EventArgs)
			If Me.GCBLibs.SelectedIndex = -1 Then Exit Sub
			If Me.LibraryList.Items.Contains(Me.GCBLibs.SelectedItem) Then Exit Sub
			
			Me.LibraryList.Items.Add(Me.GCBLibs.SelectedItem)
		End Sub
		
		Private Sub AddUserLibClick(sender As System.Object, e As System.EventArgs)
			If Me.UserLibs.SelectedIndex = -1 Then Exit Sub
			If Me.LibraryList.Items.Contains(Me.UserLibs.SelectedItem) Then Exit Sub
			
			Me.LibraryList.Items.Add(Me.UserLibs.SelectedItem)
		End Sub
		
		Private Sub GCBLibsDoubleClick(sender As System.Object, e As System.EventArgs)
			If Me.GCBLibs.SelectedIndex = -1 Then Exit Sub
			If Me.LibraryList.Items.Contains(Me.GCBLibs.SelectedItem) Then Exit Sub
			
			Me.LibraryList.Items.Add(Me.GCBLibs.SelectedItem)
		End Sub
		
		Private Sub UserLibsDoubleClick(sender As System.Object, e As System.EventArgs)
			If Me.UserLibs.SelectedIndex = -1 Then Exit Sub
			If Me.LibraryList.Items.Contains(Me.UserLibs.SelectedItem) Then Exit Sub
			
			Me.LibraryList.Items.Add(Me.UserLibs.SelectedItem)
		End Sub
		
		Private Sub AddFileLibClick(sender As System.Object, e As System.EventArgs)
			LibraryChooser.ShowDialog
			
		End Sub
		
		Private Sub LibraryChooserFileOk(sender As System.Object, e As System.ComponentModel.CancelEventArgs)
			Dim NewLibFile As String = """" + LibraryChooser.FileName + """"
			
			'Don't add library that's already in the list
			If Me.LibraryList.Items.Contains(NewLibFile) Then Exit Sub
			Me.LibraryList.Items.Add(NewLibFile)
		End Sub
		
		Private Sub LibraryListDoubleClick(sender As System.Object, e As System.EventArgs)
			'Remove item from list when double clicked
			If Me.LibraryList.SelectedIndex = -1 Then
				Exit Sub
			Else
				Me.LibraryList.Items.RemoveAt(Me.LibraryList.SelectedIndex)
			End If
			
		End Sub
		
		Private Sub GCBLibsSelectedIndexChanged(sender As System.Object, e As System.EventArgs)
			If GCBLibs.SelectedIndex = -1 Then
				AddGCBLib.Enabled = False
			Else
				AddGCBLib.Enabled = True
			End If
			
		End Sub
		
		Private Sub UserLibsSelectedIndexChanged(sender As System.Object, e As System.EventArgs)
			If UserLibs.SelectedIndex = -1 Then
				AddUserLib.Enabled = False
			Else
				AddUserLib.Enabled = True
			End If
			
			
		End Sub
		
		Private Sub LibraryListSelectedIndexChanged(sender As System.Object, e As System.EventArgs)
			If LibraryList.SelectedIndex = -1 Then
				DeleteLib.Enabled = False
			Else
				DeleteLib.Enabled = True
			End If
			
			
		End Sub
		
	End Class
End Namespace
