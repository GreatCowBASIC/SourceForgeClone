'
' Created by SharpDevelop.
' User: Hugh
' Date: 20/07/2009
' Time: 4:22 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.ComponentModel
Imports System.Windows.Forms

Namespace Great_Cow_Graphical_BASIC
	
	Public Class TextEditorPanel
		Inherits EditorPanel
		'Inherits System.Windows.Forms.UserControl
		Private editorBox As System.Windows.Forms.RichTextBox
		
		Private Dim ChangedSinceSync As Boolean
		
		Public Sub New()
			MyBase.New
			' Must be called for initialization
			Me.InitializeComponent
			
			editorBox.Select
			
		End Sub
		
		#Region " Windows Forms Designer generated code "
		' This method is required for Windows Forms designer support.
		' Do not change the method contents inside the source code editor. The Forms designer might
		' not be able to load this method if it was changed manually.
		Private Sub InitializeComponent()
			Me.editorBox = New System.Windows.Forms.RichTextBox
			Me.SuspendLayout
			'
			'editorBox
			'
			Me.editorBox.AcceptsTab = true
			Me.editorBox.AllowDrop = true
			Me.editorBox.Dock = System.Windows.Forms.DockStyle.Fill
			Me.editorBox.Font = New System.Drawing.Font("Courier New", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
			Me.editorBox.Location = New System.Drawing.Point(0, 0)
			Me.editorBox.Name = "editorBox"
			Me.editorBox.Size = New System.Drawing.Size(440, 368)
			Me.editorBox.TabIndex = 0
			Me.editorBox.Text = ""
			Me.editorBox.WordWrap = false
			AddHandler Me.editorBox.KeyDown, AddressOf Me.EditorBoxKeyDown
			AddHandler Me.editorBox.KeyPress, AddressOf Me.EditorBoxKeyPress
			AddHandler Me.editorBox.TextChanged, AddressOf Me.EditorBoxTextChanged
			'
			'TextEditorPanel
			'
			Me.Controls.Add(Me.editorBox)
			Me.Name = "TextEditorPanel"
			Me.Size = New System.Drawing.Size(440, 368)
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
		Public Overrides Overloads Sub Init(InProgram As GCBProgram)
			'Display an existing program
			Program = InProgram
			'Me.Text = Program.ProgCode(0, 0)
			
			'Take ownership of program
			Program.Editor = Me
			
			SyncProgramToEditor
		End Sub
		
		Public Overrides Overloads Sub Init(InFile As String)
			Dim Temp As Integer
			
			Program = New GCBProgram(Me)
			
			If InFile = "" Then
				With Program
					.Filename = "Untitled" + MainForm.UntitledCount.ToString
					Me.Text = .Filename
					MainForm.UntitledCount += 1
					
					.ChipName = ""
					.ChipConfig = ""
					.ChipSpeed = 0
				End With
				
				SyncProgramToEditor
			Else
				Program.LoadFile(InFile)
				Me.Text = Program.Filename
				
			End If
			
			'No longer needed, loading will trigger redraw
			'ProgramRedraw
			
			'AddHandler Program.LibraryListChanged, AddressOf Me.SyncEditorToProgram
			
		End Sub
		
		Private Sub DoHighlight
			Exit Sub
			
			Dim TempData As String = "{\rtf1\ansi " + ApplyFormat(Me.editorBox.Text) + "}"
			Dim SelStart As Integer = editorBox.SelectionStart
			Dim SelSize As Integer = editorBox.SelectionLength
			
			Me.editorBox.Text = ""
			Me.editorBox.Rtf = TempData
			
			Try
				editorBox.SelectionStart = SelStart
				editorBox.SelectionLength = SelSize
			Catch
			End Try
			
		End Sub
		
		Private Function ApplyFormat(InData As String) As String
			Dim OutData As String
			OutData = InData
			
			OutData = OutData.Replace("#include", "{\b #include}")
			OutData = OutData.Replace("set", "{\b set}")
			OutData = OutData.Replace("pulseout", "{\b pulseout}")
			'OutData = OutData.Replace(Convert.ToChar(13) + Convert.ToChar(10), "\par ")
			
			Return OutData
		End Function
		
		Public Overrides Sub ClipboardOp(ByVal Operation As String)
			
			If Operation = "x" Then
				editorBox.Cut
				ChangedSinceSync = True
			Else If Operation = "c" Then
				editorBox.Copy
			Else If Operation = "v" Then
				editorBox.Paste
				ChangedSinceSync = True
			End If
			
		End Sub
		
		Public Overrides Function CanEditProperties As Boolean
			'Prevent any editing outside of text box
			Return False
		End Function
		
		Public Overrides Sub SetCurrentLocation(SubNo As Integer, LineNo As Integer)
			
		End Sub
		
		Public Overrides Sub SyncProgramToEditor
			'Update editor from program
			editorBox.Text = Program.GetTextProgram
			ChangedSinceSync = False
			
			DoHighlight
		End Sub
		
		Public Overrides Sub SyncEditorToProgram
			'Update program from editor
			If ChangedSinceSync Then
				ChangedSinceSync = False
				Program.ReadTextProgram(editorBox.Text)
			End If
		End Sub
		
		Public Overrides Sub DoUndoRedo(RedoMode As Boolean)
			
		End Sub
		
		Public Overrides Sub AddAfterSelected(NewCode As String, Optional AppendUndo As Boolean = False)
			EditorBox.SelectedText = NewCode + Environment.NewLine
		End Sub
		
		Public Overrides Function GetCurrentCommand As String
			Return EditorBox.SelectedText
		End Function
		
		Public Overrides Sub EditorPanelKeyUp(sender As System.Object, e As System.Windows.Forms.KeyEventArgs)
			
		End Sub
		
		Private Sub EditorBoxKeyPress(sender As System.Object, e As System.Windows.Forms.KeyPressEventArgs)
			ChangedSinceSync = True
			Program.HasChanged = True
			
			If e.KeyChar = " " Then
				doHighLight
			
			Else If e.KeyChar = Convert.ToChar(10) Then
				e.Handled = True
			
			Else If e.KeyChar = Convert.ToChar(13) Then
				e.Handled = True
				
				'Get curr line indent
				'Find end of prev line
				Dim FindLineStart, LineStartPos, FindFirstChar As Integer
				Dim CurrChar, LastIndent As String
				LineStartPos = 0
				
				'Find start of current line
				For FindLineStart = editorBox.SelectionStart - 2 To 0 Step -1
					If editorBox.Text.Substring(FindLineStart).StartsWith(Convert.ToChar(10)) Then
						LineStartPos = FindLineStart + 1
						Exit For
					End If
				Next
				
				'Find whitespace between start of line and first non-whitespace character
				LastIndent = ""
				For FindFirstChar = LineStartPos To editorBox.Text.Length - 1
					CurrChar = editorBox.Text.Substring(FindFirstChar, 1)
					If CurrChar = Convert.ToChar(9) Or CurrChar = Convert.ToChar(32) Then
						LastIndent += CurrChar
					Else
						Exit For
					End If
				Next
				
				'Add a new line, then the right indent at the current cursor location
				editorBox.SelectedText = Convert.ToChar(10) + LastIndent
				
				'Move the cursor to the start of the new line
				editorBox.Select(editorBox.SelectionStart + editorBox.SelectionLength, 0)
				doHighLight
			End If
			
		End Sub
		
		Private Sub EditorBoxKeyDown(sender As System.Object, e As System.Windows.Forms.KeyEventArgs)
			If e.KeyCode = Keys.Enter Then
				e.Handled = True
			End If
			
		End Sub
		
		Private Sub EditorBoxTextChanged(sender As System.Object, e As System.EventArgs)
			'If text has changed, set some flags
			ChangedSinceSync = True
			Program.HasChanged = True
		End Sub
		
	End Class
End Namespace
