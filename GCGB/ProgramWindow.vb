'
' Created by SharpDevelop.
' User: Administrator
' Date: 7/08/2006
' Time: 3:57 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.Windows.Forms

'Namespace Great_Cow_Graphical_BASIC
	
	Public Class ProgramWindow
		Inherits System.Windows.Forms.Form
		Implements ProgramContainer
		Private guiProgramEditor As Great_Cow_Graphical_BASIC.GuiEditorPanel
		
		Private cancelClose As Boolean
		Private programEditor As EditorPanel
		Private textProgramEditor As TextEditorPanel
		
		Public Sub New()
			Me.New("")
			
			'
			' The Me.InitializeComponent call is required for Windows Forms designer support.
			'
			'Me.InitializeComponent
			
		End Sub
		
		Public Sub New(InFile As String)
			MyBase.New
			
			'
			' The Me.InitializeComponent call is required for Windows Forms designer support.
			'
			Me.InitializeComponent
			
			programEditor = guiProgramEditor
			programEditor.Init(InFile)
			
			Me.Text = programEditor.Text
			
		End Sub
		
		#Region " Windows Forms Designer generated code "
		' This method is required for Windows Forms designer support.
		' Do not change the method contents inside the source code editor. The Forms designer might
		' not be able to load this method if it was changed manually.
		Private Sub InitializeComponent()
			Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(ProgramWindow))
			Me.guiProgramEditor = New Great_Cow_Graphical_BASIC.GuiEditorPanel
			Me.SuspendLayout
			'
			'guiProgramEditor
			'
			Me.guiProgramEditor.Dock = System.Windows.Forms.DockStyle.Fill
			Me.guiProgramEditor.Location = New System.Drawing.Point(0, 0)
			Me.guiProgramEditor.Name = "guiProgramEditor"
			Me.guiProgramEditor.Size = New System.Drawing.Size(492, 453)
			Me.guiProgramEditor.TabIndex = 0
			AddHandler Me.guiProgramEditor.RefreshSubList, AddressOf Me.ProgramEditorRefreshSubList
			AddHandler Me.guiProgramEditor.RefreshUndoRedo, AddressOf Me.ProgramEditorRefreshUndoRedo
			AddHandler Me.guiProgramEditor.SubAdded, AddressOf Me.ProgramEditorSubAdded
			'
			'ProgramWindow
			'
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.BackColor = System.Drawing.SystemColors.Control
			Me.ClientSize = New System.Drawing.Size(492, 453)
			Me.Controls.Add(Me.guiProgramEditor)
			Me.Icon = CType(resources.GetObject("$this.Icon"),System.Drawing.Icon)
			Me.Location = New System.Drawing.Point(181, 0)
			Me.MinimumSize = New System.Drawing.Size(320, 240)
			Me.Name = "ProgramWindow"
			AddHandler Closing, AddressOf Me.ProgramClosed
			AddHandler KeyUp, AddressOf Me.ProgramWindowKeyUp
			AddHandler Activated, AddressOf Me.ProgramSelected
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
		Public Property Program As GCBProgram Implements ProgramContainer.Program
			Get
				Return programEditor.Program
			End Get
			Set
				programEditor.Program = Value
				Program.Editor = programEditor
			End Set
		End Property
		
		Public Property ProgramName As String Implements ProgramContainer.ProgramName
			Get
				Return Me.Text
			End Get
			Set
				Me.Text = Value
			End Set
		End Property
		
		Public Event RefreshUndoRedo Implements ProgramContainer.RefreshUndoRedo
		Public Event SubAdded Implements ProgramContainer.SubAdded
		Public Event RefreshSubList Implements ProgramContainer.RefreshSubList
		Public Event ThisWindowSelected Implements ProgramContainer.ThisWindowSelected
		Public Event ViewModeChanged Implements ProgramContainer.ViewModeChanged
		
		Private Sub ProgramClosed(sender As System.Object, e As System.ComponentModel.CancelEventArgs)
			
			If Program.HasChanged Then 
				Dim UnsavedPrompt As String = "There are unsaved changes to this file. Save them?"
				MainForm.Translator.TryTranslate("Misc", "UnsavedChanges", UnsavedPrompt)
				
				Dim SaveChangesResult As DialogResult = MessageBox.Show(UnsavedPrompt, Program.Filename, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
				
				If SaveChangesResult = DialogResult.Cancel Then
					 e.Cancel = True
					 cancelClose = True
					 Exit Sub
				End If
				If SaveChangesResult = DialogResult.Yes Then
					If Program.Filename.ToLower.Substring(0, 8) = "untitled" Then
						Mainform.MainFormInstance.SaveFileSelect.FileName = ""
						Mainform.MainFormInstance.SaveFileSelect.ShowDialog
						Exit Sub
					End If
					'Try to save file
					Try
				 		Program.SaveFile(Program.Filename)
				 	Catch ex as UnauthorizedAccessException
				 		'Could not save file, offer to let user choose new location
				 		Dim result As DialogResult = MessageBox.Show("GCGB cannot save your file in the chosen location. Would you like to choose a new name or folder?", "Great Cow Graphical BASIC", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
						If result = DialogResult.Yes Then
							MainForm.MainFormInstance.SaveFileSelect.FileName = ""
				    		MainForm.MainFormInstance.SaveFileSelect.ShowDialog()
						End If
						'Exit if save failed - don't clear unchanged flag!
						Exit Sub
				 	End Try
					
				End If		
			End If
			
			RaiseEvent ThisWindowSelected
			
			Me.Dispose
		End Sub
		
		Private Sub ProgramSelected(sender As System.Object, e As System.EventArgs)
			RaiseEvent ThisWindowSelected
		End Sub
		
		Private Sub ProgramEditorSubAdded()
			RaiseEvent SubAdded
		End Sub
		
		Private Sub ProgramEditorRefreshUndoRedo()
			RaiseEvent RefreshUndoRedo
		End Sub
		
		Private Sub ProgramEditorRefreshSubList()
			RaiseEvent RefreshSubList
		End Sub
		
		Public Sub ClipboardOp(ByVal Operation As String) Implements ProgramContainer.ClipboardOp
			programEditor.ClipboardOp(Operation)
		End Sub
		
		Public Sub DoUndoRedo(ByVal RedoMode As Boolean) Implements ProgramContainer.DoUndoRedo
			programEditor.DoUndoRedo(RedoMode)
		End Sub
		
		Public Sub AddAfterSelected(NewCode As String, Optional AppendUndo As Boolean = False) Implements ProgramContainer.AddAfterSelected
			programEditor.AddAfterSelected(NewCode, AppendUndo)
		End Sub
		
		Public Function GetCurrentCommand As String Implements ProgramContainer.GetCurrentCommand
			Return programEditor.GetCurrentCommand
		End Function
		
		Public Function CloseProgram As Boolean Implements ProgramContainer.CloseProgram
			cancelClose = False
			Me.Close
			
			If cancelClose Then
				Return False
			Else
				Return True
			End If
		End Function
		
		Private Sub ProgramWindowKeyUp(sender As System.Object, e As System.Windows.Forms.KeyEventArgs)
			programEditor.EditorPanelKeyUp(sender, e)
			
		End Sub
		
		Public Sub SyncEditorToProgram Implements ProgramContainer.SyncEditorToProgram
			programEditor.SyncEditorToProgram
		End Sub
		
		Public Sub SyncProgramToEditor Implements ProgramContainer.SyncProgramToEditor
			programEditor.SyncProgramToEditor
		End Sub
		
		Public Sub SetIconView Implements ProgramContainer.SetIconView
			'Sync
			programEditor.SyncEditorToProgram
			
			'Create new editor
			Dim newEditor As New GuiEditorPanel
			Dim oldProgram As GCBProgram = programEditor.Program
			newEditor.Init(oldProgram)
			
			'Remove old editor
			If TypeOf programEditor Is GuiEditorPanel Then
				Dim oldEditor As GuiEditorPanel = programEditor
				
				RemoveHandler oldEditor.RefreshSubList, AddressOf Me.ProgramEditorRefreshSubList
				RemoveHandler oldEditor.RefreshUndoRedo, AddressOf Me.ProgramEditorRefreshUndoRedo
				RemoveHandler oldEditor.SubAdded, AddressOf Me.ProgramEditorSubAdded
				
			End If
			Me.Controls.Remove(programEditor)
			programEditor.Dispose
			
			'Show new editor
			programEditor = newEditor
			Me.Controls.Add(programEditor)
			Me.programEditor.Dock = System.Windows.Forms.DockStyle.Fill
			Me.programEditor.Location = New System.Drawing.Point(0, 0)
			Me.programEditor.Name = "programEditor"
			Me.programEditor.Size = New System.Drawing.Size(492, 453)
			Me.programEditor.TabIndex = 0
			If TypeOf programEditor Is GuiEditorPanel Then
				Dim newGuiEditor As GuiEditorPanel = programEditor
				AddHandler newGuiEditor.RefreshSubList, AddressOf Me.ProgramEditorRefreshSubList
				AddHandler newGuiEditor.RefreshUndoRedo, AddressOf Me.ProgramEditorRefreshUndoRedo
				AddHandler newGuiEditor.SubAdded, AddressOf Me.ProgramEditorSubAdded
			End If
			Me.programEditor.Select
			Me.Select
			
			RaiseEvent ViewModeChanged
		End Sub
		
		Public Sub SetTextView Implements ProgramContainer.SetTextView
			'Sync
			programEditor.SyncEditorToProgram
			
			'Create new editor
			Dim newEditor As New TextEditorPanel
			Dim oldProgram As GCBProgram = programEditor.Program
			newEditor.Init(oldProgram)
			
			'Remove old editor
			If TypeOf programEditor Is GuiEditorPanel Then
				Dim oldEditor As GuiEditorPanel = programEditor
				RemoveHandler oldEditor.RefreshSubList, AddressOf Me.ProgramEditorRefreshSubList
				RemoveHandler oldEditor.RefreshUndoRedo, AddressOf Me.ProgramEditorRefreshUndoRedo
				RemoveHandler oldEditor.SubAdded, AddressOf Me.ProgramEditorSubAdded
				oldEditor.RemoveLibraryHandler
			End If
			Me.Controls.Remove(programEditor)
			programEditor.Dispose
			
			'Show new editor
			programEditor = newEditor
			Me.Controls.Add(programEditor)
			Me.programEditor.Dock = System.Windows.Forms.DockStyle.Fill
			Me.programEditor.Location = New System.Drawing.Point(0, 0)
			Me.programEditor.Name = "programEditor"
			Me.programEditor.Size = New System.Drawing.Size(492, 453)
			Me.programEditor.TabIndex = 0
			If TypeOf programEditor Is GuiEditorPanel Then
				Dim newGuiEditor As GuiEditorPanel = programEditor
				AddHandler newGuiEditor.RefreshSubList, AddressOf Me.ProgramEditorRefreshSubList
				AddHandler newGuiEditor.RefreshUndoRedo, AddressOf Me.ProgramEditorRefreshUndoRedo
				AddHandler newGuiEditor.SubAdded, AddressOf Me.ProgramEditorSubAdded
			End If
			Me.programEditor.Select
			Me.Select
			
			RaiseEvent ViewModeChanged
		End Sub
		
	End Class
'End Namespace
