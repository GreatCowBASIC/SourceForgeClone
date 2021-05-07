'
' Created by SharpDevelop.
' User: Hugh
' Date: 15/07/2009
' Time: 12:00 AM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System

'Namespace Great_Cow_Graphical_BASIC

	Public Interface ProgramContainer
		
		Property Program As GCBProgram
		
		Property ProgramName As String
		
		Event RefreshUndoRedo
		Event SubAdded
		Event RefreshSubList
		Event ThisWindowSelected
		Event ViewModeChanged
		
		Sub ClipboardOp(ByVal Operation As String)
		Sub DoUndoRedo(ByVal RedoMode As Boolean)
		Sub AddAfterSelected(NewCode As String, Optional AppendUndo As Boolean = False)
		Function GetCurrentCommand As String
		Function CloseProgram As Boolean
		
		'Synchronisation subs, used to ensure editor display matches program
		Sub SyncEditorToProgram
		Sub SyncProgramToEditor
		
		'View changing modes
		Sub SetIconView
		Sub SetTextView
		
	End Interface
'End Namespace
