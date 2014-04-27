'
' Created by SharpDevelop.
' User: Hugh
' Date: 15/06/2012
' Time: 8:56 PM
'
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System.IO
Imports System.Threading

Public Partial Class MainForm
	Private Dim CurrSong As Song
	
	Public Sub New()
		' The Me.InitializeComponent call is required for Windows Forms designer support.
		Me.InitializeComponent()
		
		CurrSong = Nothing
	End Sub
	
	Sub SaveLibraryClick(sender As Object, e As EventArgs)
		'Make sure that a song is ready to save
		If CurrSong Is Nothing Then
			MessageBox.Show("Please convert a song before saving", "Song Converter", MessageBoxButtons.OK, MessageBoxIcon.Information)
		Else
			SaveLibraryChooser.ShowDialog
		End If
	End Sub
	
	Sub InputRTTTLTextChanged(sender As Object, e As EventArgs)
		If InputRTTTL.Text = "" Then
			CurrSong = Nothing
			Me.OutputCode.Text = ""
			SaveLibrary.Enabled = False
			Preview.Enabled = False
		Else
			CurrSong = New Song(InputRTTTL.Text)
			Me.OutputCode.Text = CurrSong.Code
			SaveLibrary.Enabled = True
			Preview.Enabled = True
		End If
	End Sub
	
	Sub SaveLibraryChooserFileOk(sender As Object, e As System.ComponentModel.CancelEventArgs)
		Dim SaveLib As New StreamWriter(SaveLibraryChooser.FileName)
		SaveLib.Write(OutputCode.Text)
		SaveLib.Close
	End Sub
	
	Sub PreviewClick(sender As Object, e As EventArgs)
		'CurrSong.Preview
		Dim playThread As New Thread(AddressOf CurrSong.Preview)
		playThread.Start
		
	End Sub
	
	Sub ReadMidiClick(sender As Object, e As EventArgs)
		Dim fMidiTranslator As New MidiTranslator
		fMidiTranslator.ShowDialog
	End Sub
End Class
