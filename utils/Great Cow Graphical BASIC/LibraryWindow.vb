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

''Namespace Great_Cow_Graphical_BASIC
	
	Public Class LibraryWindow
		Inherits System.Windows.Forms.Form
		Private AddGCBLib As System.Windows.Forms.Button
		Private groupBox2 As System.Windows.Forms.GroupBox
		Private groupBox1 As System.Windows.Forms.GroupBox
		Private Cancel_Button As System.Windows.Forms.Button
		Private LibraryChooser As System.Windows.Forms.OpenFileDialog
		Private GCBLibs As System.Windows.Forms.ListBox
		Private AddFileLib As System.Windows.Forms.Button
		Private DeleteLib As System.Windows.Forms.Button
		Private OK_Button As System.Windows.Forms.Button
		Private helpLink As System.Windows.Forms.LinkLabel
		Private LibraryList As System.Windows.Forms.ListBox
		Private LibHelp As System.Windows.Forms.HelpProvider
		
		Private Dim CurrentFile As GCBProgram
		
		Public Sub New(CurrentFile As GCBProgram)
			MyBase.New
			
			'
			' The Me.InitializeComponent call is required for Windows Forms designer support.
			'
			Me.InitializeComponent
			
			Dim Temp As Integer
			
			'Get current program
			Me.CurrentFile = CurrentFile
			
			With CurrentFile
				
				'Draw current include list
				Dim LibTemp As GCBLibrary
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
							Me.GCBLibs.Items.Add("""" + FileName + """")
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
			Me.LibHelp = New System.Windows.Forms.HelpProvider
			Me.LibraryList = New System.Windows.Forms.ListBox
			Me.helpLink = New System.Windows.Forms.LinkLabel
			Me.OK_Button = New System.Windows.Forms.Button
			Me.DeleteLib = New System.Windows.Forms.Button
			Me.AddFileLib = New System.Windows.Forms.Button
			Me.GCBLibs = New System.Windows.Forms.ListBox
			Me.LibraryChooser = New System.Windows.Forms.OpenFileDialog
			Me.Cancel_Button = New System.Windows.Forms.Button
			Me.groupBox1 = New System.Windows.Forms.GroupBox
			Me.groupBox2 = New System.Windows.Forms.GroupBox
			Me.AddGCBLib = New System.Windows.Forms.Button
			Me.groupBox1.SuspendLayout
			Me.groupBox2.SuspendLayout
			Me.SuspendLayout
			'
			'LibraryList
			'
			Me.LibraryList.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.LibHelp.SetHelpString(Me.LibraryList, "Displays a list of libraries that are currently used by the program")
			Me.LibraryList.IntegralHeight = false
			Me.LibraryList.Location = New System.Drawing.Point(8, 19)
			Me.LibraryList.Name = "LibraryList"
			Me.LibHelp.SetShowHelp(Me.LibraryList, true)
			Me.LibraryList.Size = New System.Drawing.Size(192, 293)
			Me.LibraryList.TabIndex = 0
			AddHandler Me.LibraryList.DoubleClick, AddressOf Me.LibraryListDoubleClick
			AddHandler Me.LibraryList.SelectedIndexChanged, AddressOf Me.LibraryListSelectedIndexChanged
			'
			'helpLink
			'
			Me.helpLink.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.helpLink.Location = New System.Drawing.Point(312, 368)
			Me.helpLink.Name = "helpLink"
			Me.helpLink.Size = New System.Drawing.Size(128, 32)
			Me.helpLink.TabIndex = 15
			Me.helpLink.TabStop = true
			Me.helpLink.Text = "What is a library?"
			Me.helpLink.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
			AddHandler Me.helpLink.LinkClicked, AddressOf Me.LinkLabel1LinkClicked
			'
			'OK_Button
			'
			Me.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.Bottom
			Me.OK_Button.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.OK_Button.Location = New System.Drawing.Point(144, 368)
			Me.OK_Button.Name = "OK_Button"
			Me.OK_Button.Size = New System.Drawing.Size(72, 24)
			Me.OK_Button.TabIndex = 1
			Me.OK_Button.Text = "OK"
			AddHandler Me.OK_Button.Click, AddressOf Me.OK_ButtonClick
			'
			'DeleteLib
			'
			Me.DeleteLib.Anchor = System.Windows.Forms.AnchorStyles.Bottom
			Me.DeleteLib.Enabled = false
			Me.DeleteLib.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.LibHelp.SetHelpString(Me.DeleteLib, "Deletes a library from the program.")
			Me.DeleteLib.Location = New System.Drawing.Point(64, 320)
			Me.DeleteLib.Name = "DeleteLib"
			Me.LibHelp.SetShowHelp(Me.DeleteLib, true)
			Me.DeleteLib.Size = New System.Drawing.Size(72, 24)
			Me.DeleteLib.TabIndex = 10
			Me.DeleteLib.Text = "Delete"
			AddHandler Me.DeleteLib.Click, AddressOf Me.DeleteLibClick
			'
			'AddFileLib
			'
			Me.AddFileLib.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
			Me.AddFileLib.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.LibHelp.SetHelpString(Me.AddFileLib, "Allows a library to be added that isn't shown on the list")
			Me.AddFileLib.Location = New System.Drawing.Point(40, 328)
			Me.AddFileLib.Name = "AddFileLib"
			Me.LibHelp.SetShowHelp(Me.AddFileLib, true)
			Me.AddFileLib.Size = New System.Drawing.Size(144, 24)
			Me.AddFileLib.TabIndex = 14
			Me.AddFileLib.Text = "Add another library ..."
			AddHandler Me.AddFileLib.Click, AddressOf Me.AddFileLibClick
			'
			'GCBLibs
			'
			Me.GCBLibs.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.LibHelp.SetHelpString(Me.GCBLibs, "Libraries that can be added to the program. Libraries with < and > around them ar"& _ 
"e included with Great Cow Graphical BASIC. Libraries with "" and "" are ones saved"& _ 
" in the same place as the program. To add one to the program, click on it and th"& _ 
"en click the ""Add >"" button below")
			Me.GCBLibs.IntegralHeight = false
			Me.GCBLibs.Location = New System.Drawing.Point(8, 19)
			Me.GCBLibs.Name = "GCBLibs"
			Me.LibHelp.SetShowHelp(Me.GCBLibs, true)
			Me.GCBLibs.Size = New System.Drawing.Size(192, 253)
			Me.GCBLibs.TabIndex = 4
			AddHandler Me.GCBLibs.DoubleClick, AddressOf Me.GCBLibsDoubleClick
			AddHandler Me.GCBLibs.SelectedIndexChanged, AddressOf Me.GCBLibsSelectedIndexChanged
			'
			'LibraryChooser
			'
			Me.LibraryChooser.DefaultExt = "h"
			Me.LibraryChooser.Filter = "GCBASIC Libraries|*.h|All files|*.*"
			Me.LibraryChooser.Title = "Add another library"
			AddHandler Me.LibraryChooser.FileOk, AddressOf Me.LibraryChooserFileOk
			'
			'Cancel_Button
			'
			Me.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.Bottom
			Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me.Cancel_Button.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.Cancel_Button.Location = New System.Drawing.Point(224, 368)
			Me.Cancel_Button.Name = "Cancel_Button"
			Me.Cancel_Button.Size = New System.Drawing.Size(72, 24)
			Me.Cancel_Button.TabIndex = 2
			Me.Cancel_Button.Text = "Cancel"
			'
			'groupBox1
			'
			Me.groupBox1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.groupBox1.Controls.Add(Me.LibraryList)
			Me.groupBox1.Controls.Add(Me.DeleteLib)
			Me.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.groupBox1.Location = New System.Drawing.Point(224, 8)
			Me.groupBox1.Name = "groupBox1"
			Me.groupBox1.Size = New System.Drawing.Size(208, 352)
			Me.groupBox1.TabIndex = 11
			Me.groupBox1.TabStop = false
			Me.groupBox1.Text = "Libraries already added"
			'
			'groupBox2
			'
			Me.groupBox2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
			Me.groupBox2.Controls.Add(Me.GCBLibs)
			Me.groupBox2.Controls.Add(Me.AddGCBLib)
			Me.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.groupBox2.Location = New System.Drawing.Point(8, 8)
			Me.groupBox2.Name = "groupBox2"
			Me.groupBox2.Size = New System.Drawing.Size(208, 312)
			Me.groupBox2.TabIndex = 12
			Me.groupBox2.TabStop = false
			Me.groupBox2.Text = "Available Libraries"
			'
			'AddGCBLib
			'
			Me.AddGCBLib.Anchor = System.Windows.Forms.AnchorStyles.Bottom
			Me.AddGCBLib.Enabled = false
			Me.AddGCBLib.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.LibHelp.SetHelpString(Me.AddGCBLib, "Adds the library selected above")
			Me.AddGCBLib.Location = New System.Drawing.Point(64, 280)
			Me.AddGCBLib.Name = "AddGCBLib"
			Me.LibHelp.SetShowHelp(Me.AddGCBLib, true)
			Me.AddGCBLib.Size = New System.Drawing.Size(72, 24)
			Me.AddGCBLib.TabIndex = 8
			Me.AddGCBLib.Text = "Add >"
			AddHandler Me.AddGCBLib.Click, AddressOf Me.AddGCBLibClick
			'
			'LibraryWindow
			'
			Me.AcceptButton = Me.OK_Button
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.CancelButton = Me.Cancel_Button
			Me.ClientSize = New System.Drawing.Size(442, 404)
			Me.Controls.Add(Me.helpLink)
			Me.Controls.Add(Me.groupBox2)
			Me.Controls.Add(Me.groupBox1)
			Me.Controls.Add(Me.Cancel_Button)
			Me.Controls.Add(Me.OK_Button)
			Me.Controls.Add(Me.AddFileLib)
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
						.Libraries.Add(New GCBLibrary(TempData))
					End If
				Next
				
				'Update library data
				CurrentFile.LoadIncludes
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
		
		Private Sub GCBLibsDoubleClick(sender As System.Object, e As System.EventArgs)
			If Me.GCBLibs.SelectedIndex = -1 Then Exit Sub
			If Me.LibraryList.Items.Contains(Me.GCBLibs.SelectedItem) Then Exit Sub
			
			Me.LibraryList.Items.Add(Me.GCBLibs.SelectedItem)
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
		
		Private Sub LibraryListSelectedIndexChanged(sender As System.Object, e As System.EventArgs)
			If LibraryList.SelectedIndex = -1 Then
				DeleteLib.Enabled = False
			Else
				DeleteLib.Enabled = True
			End If
			
			
		End Sub
		
		Private Sub LinkLabel1LinkClicked(sender As System.Object, e As System.Windows.Forms.LinkLabelLinkClickedEventArgs)
			Mainform.MainformInstance.ShowHelpArticle("include.htm")
			
		End Sub
		
	End Class
''End Namespace
