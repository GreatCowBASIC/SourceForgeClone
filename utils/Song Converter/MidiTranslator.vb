'
' Created by SharpDevelop.
' User: Hugh
' Date: 18/12/2012
' Time: 12:09 AM
'
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Public Partial Class MidiTranslator
	Private Dim CurrFile As MidiFile
	
	Public Sub New()
		' The Me.InitializeComponent call is required for Windows Forms designer support.
		Me.InitializeComponent()
		
		'
		' TODO : Add constructor code after InitializeComponents
		'
	End Sub
	
	Sub ButtonOpenClick(sender As Object, e As EventArgs)
		Me.openMidiDialog.ShowDialog
	End Sub
	
	Sub OpenMidiDialogFileOk(sender As Object, e As System.ComponentModel.CancelEventArgs)
		CurrFile = New MidiFile(Me.openMidiDialog.FileName)
		
		Me.outText.Clear
		Me.DebugPrint("Loaded " + Me.openMidiDialog.FileName)
		Me.DebugPrint("Format: " + CurrFile.fileFormat.ToString + ", Tracks: " + CurrFile.trackCount.ToString)
		If CurrFile.ticksPerBeat = 0 Then
			Me.DebugPrint("SMPTE Frames: " + CurrFile.SMPTEFrames.ToString + ", Ticks per frame: " + CurrFile.ticksPerFrame.ToString)
		Else
			Me.DebugPrint("Ticks per beat: " + CurrFile.ticksPerBeat.ToString)
		End If
		
		Dim currTrack As MidiTrack
		Dim currEvent As MidiEvent
		Me.outText.Visible = False
		For Each currTrack In CurrFile.Tracks
			Me.DebugPrint("")
			Me.DebugPrint("Track " + currTrack.Number.ToString)
			For Each currEvent In currTrack.Events
				Me.DebugPrint(currEvent.ToString)
			Next
		Next
		Me.outText.Visible = True
		
	End Sub
	
	Private Sub DebugPrint(OutMessage As String)
		Me.outText.AppendText(OutMessage + Environment.NewLine)
	End Sub
End Class
