﻿'
' Created by SharpDevelop.
' User: Hugh
' Date: 8/02/2015
' Time: 11:59 PM
'
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.IO
Imports System.Windows.Forms
Imports System.Collections.Generic
Imports System.Text

Public Partial Class ExampleChooser
	
	Private Dim ExampleDir As String
	Private Dim Program As GCBProgram
	Private Dim SelectedFile As FileInfo
	
	Public Sub New(ExampleDir As String)
		' The Me.InitializeComponent call is required for Windows Forms designer support.
		Me.InitializeComponent()
		
		Me.ExampleDir = ExampleDir
		Program = Nothing
		
		FillExampleList
		
		FillChipList
		
		UpdateDisplay
	End Sub
	
	Private Sub FillExampleList
		AddExamplesFromDir(ExampleDir, Nothing)
		
	End Sub
	
	Private Sub FillChipList
		'Get list of supported chips
		Dim ChipDataDir As New IO.DirectoryInfo(MainForm.InstallDir + Mainform.FilenameChipDir)
		Dim ChipNameList As IO.FileInfo() = ChipDataDir.GetFiles()
		Dim ChipFileName As IO.FileInfo			
		Dim ChipList As New List(Of String)
		Dim ChipName As String
		
		'Copy into list, then sort
		For Each ChipFileName In ChipNameList
			If ChipFileName.FullName.ToUpper.IndexOf("CORE") = -1 Then
				If ChipFileName.Name.StartsWith("1") Then
					ChipName = ChipFileName.Name.ToUpper
				Else
					ChipName = ChipFileName.Name.ToLower
				End If
				ChipList.Add(ChipName.Substring(0, ChipName.IndexOf(".")))
			End If
		Next
		
		newChipModel.Items.Clear
		ChipList.Sort
		
		Dim ChipNameTemp As String
		For Each ChipNameTemp In ChipList
			newChipModel.Items.Add(ChipNameTemp)
		Next
	End Sub
	
	Private Sub AddExamplesFromDir(CurrDir As String, ParentNode As TreeNode)
		Dim NewItem As TreeNode
		
		'Get list of folders
		Dim ExampleDir As New IO.DirectoryInfo(CurrDir)
		Dim FolderList As IO.DirectoryInfo() = ExampleDir.GetDirectories
		Dim ExampleDirName As IO.DirectoryInfo
		For Each ExampleDirName In FolderList
			Dim NewNode As New TreeNode(ExampleDirName.Name)
			AddExamplesFromDir(ExampleDirName.FullName, NewNode)
			If ParentNode Is Nothing Then
				Me.exampleTree.Nodes.Add(NewNode)
			Else
				ParentNode.Nodes.Add(NewNode)
			End If
		Next
		
		'Get list of .gcb files
		Dim ExampleList As IO.FileInfo() = ExampleDir.GetFiles("*.gcb")
		Dim ExampleFileName As IO.FileInfo
		For Each ExampleFileName In ExampleList
			NewItem = New TreeNode(ExampleFileName.Name.Substring(0, ExampleFileName.Name.Length - 4))
			NewItem.Tag = ExampleFileName
			
			If ParentNode Is Nothing Then
				Me.exampleTree.Nodes.Add(NewItem)
			Else
				ParentNode.Nodes.Add(NewItem)
			End If
		Next
		
	End Sub
	
	Sub ExampleTreeAfterSelect(sender As Object, e As TreeViewEventArgs)
		If Not e.Node.Tag Is Nothing Then
			If TypeOf e.Node.Tag Is IO.FileInfo Then
				SelectedFile = e.Node.Tag
				
				Program = New GCBProgram(Nothing)
				Program.LoadFile(SelectedFile.FullName)
				UpdateDisplay
				
				Dim exampleName As String = SelectedFile.Name
				If exampleName.ToLower.EndsWith(".gcb") Then
					exampleName = exampleName.Substring(0, ExampleName.Length - 4)
				End If
				labelExampleName.Text = exampleName
				Exit Sub
			End If
		End If
		
		'Nothing selected
		Program = Nothing
		SelectedFile = Nothing
		UpdateDisplay
	End Sub
	
	Sub LicenceLinkLinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs)
		'If GPL or LGPL, display
		Dim LicenceFile As String = ""
		
		Select Case LicenceLink.Text.ToUpper
			Case "GPL": LicenceFile = "license.txt"
			Case "LGPL": LicenceFile = "lesser.txt"
		End Select
		
		If LicenceFile <> "" Then
			System.Diagnostics.Process.Start(Mainform.InstallDir + "/docs/" + LicenceFile)
		End If
		
	End Sub
	
	Private Sub UpdateDisplay
		'If no demo selected, hide all
		If Program Is Nothing Then
			Dim CurrControl As Control
			For Each CurrControl In Me.dialogSplit.Panel2.Controls
				If Not CurrControl Is labelExampleName Then
					CurrControl.Visible = False
				End If
			Next
			
			buttonOpenExample.Enabled = False
			labelExampleName.Text = "Select an example"
		Else
			'Version, Author and Licence
			labelVersion.Text = ""
			If Program.About Is Nothing Then
				If Not SelectedFile Is Nothing Then
					labelVersion.Text = "Version " + SelectedFile.LastWriteTime.ToString("d")
				End If
			Else If Program.About.Version <> "" Then
				labelVersion.Text = "Version " + Program.About.Version
			End If
			
			labelAuthor.Text = ""
			If Not Program.About Is Nothing Then
				If Program.About.Author <> "" Then
					labelAuthor.Text = "Author " + Program.About.Author
				End If
			End If
			
			LicenceLink.Text = "Unknown"
			If Not Program.About Is Nothing Then
				If Program.About.Licence <> "" Then
					LicenceLink.Text = Program.About.Licence
				End If
			End If
			If LicenceLink.Text.ToUpper = "GPL" Or LicenceLink.Text.ToUpper = "LGPL" Then
				LicenceLink.Enabled = True
			Else
				LicenceLink.Enabled = False
			End If
			
			'Summary display
			If Not Program.About Is Nothing Then
				summaryDisplay.Text = Program.About.Description
			Else
				summaryDisplay.Text = ""
			End If
			
			'Chip model display
			writtenFor.Text = "Written for " + Program.ChipName
			newChipModel.Text = Program.ChipName
			
			'Show connections
			DrawConnections
			
			'Demo is selected, show all controls
			Dim CurrControl As Control
			For Each CurrControl In Me.dialogSplit.Panel2.Controls
				CurrControl.Visible = True
			Next
			buttonOpenExample.Enabled = True
			
		End If
	End Sub
	
	Sub DrawConnections
		'Show connections between chip and devices
		Dim ChipPin, ChipPinPort, DevPin As String
		
		Dim OutBuffer As New StringBuilder
		
		'Add constants that refer to IO pins
		'This program
		Dim Constant As Setting
		For Each Constant In Program.Constants
			'Check if constant refers to a pin
			If LowLevel.IsBitConst(Constant.Value) Then
				ChipPin = Constant.Value
				ChipPinPort = ChipPin.Substring(0, ChipPin.IndexOf(".")).ToUpper
				If ChipPinPort = "GPIO" Or (ChipPinPort.Length = 5 And ChipPinPort.StartsWith("PORT")) Then
					DevPin = Constant.Name
					OutBuffer.AppendLine(ChipPin + " to " + DevPin)
				End If
			End If
		Next
		'Libraries
		Dim CheckLib As LibraryType
		For Each CheckLib In Program.Libraries
			For Each Constant In CheckLib.Constants
				If LowLevel.IsBitConst(Constant.Value) Then
					ChipPin = Constant.Value
					ChipPinPort = ChipPin.Substring(0, ChipPin.IndexOf(".")).ToUpper
					If ChipPinPort = "GPIO" Or (ChipPinPort.Length = 5 And ChipPinPort.StartsWith("PORT")) Then
						DevPin = Constant.Name
						OutBuffer.AppendLine(ChipPin + " to " + DevPin)
					End If
				End If
			Next
		Next
		
		connectionsDisplay.Text = OutBuffer.ToString
		
	End Sub
	
	Sub ButtonOpenExampleClick(sender As Object, e As EventArgs)
		'Send example to GCGB editor
		Program.FileName = ""
		Program.ChipName = newChipModel.Text
		Mainform.MainformInstance.CreateProgramContainer(Program)
		Me.Close
	End Sub
	
	Sub NewChipModelSelectedValueChanged(sender As Object, e As EventArgs)
		
	End Sub
End Class
