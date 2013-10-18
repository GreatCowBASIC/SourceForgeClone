'
' Created by SharpDevelop.
' User: Hugh
' Date: 14/07/2009
' Time: 10:48 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System

Namespace Great_Cow_Graphical_BASIC

	'Public Interface EditorPanel
	Public MustInherit Class EditorPanel
		Inherits System.Windows.Forms.UserControl
		
		'Program storage
		Public Dim Program As GCBProgram
		
		Public MustOverride Function CanEditProperties As Boolean
		
		'Synchronisation subs, used to ensure editor display matches program
		Public MustOverride Sub SyncEditorToProgram
		Public MustOverride Sub SyncProgramToEditor
		
		'Load file, create new program
		Public MustOverride Overloads Sub Init(InFile As String)
		Public MustOverride Overloads Sub Init(InProgram As GCBProgram)
		
		'Modification functions
		Public MustOverride Sub ClipboardOp(Operation As String)
		Public MustOverride Sub DoUndoRedo(ByVal RedoMode As Boolean)
		Public MustOverride Sub AddAfterSelected(NewCode As String, Optional AppendUndo As Boolean = False)
		
		Public MustOverride Function GetCurrentCommand As String
		Public MustOverride Sub SetCurrentLocation(SubNo As Integer, LineNo As Integer)
		
		Public MustOverride Sub EditorPanelKeyUp(sender As System.Object, e As System.Windows.Forms.KeyEventArgs)
		
	End Class
	
End Namespace
