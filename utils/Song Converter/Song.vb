'
' Created by SharpDevelop.
' User: Hugh
' Date: 15/06/2012
' Time: 10:13 PM
'
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Imports System.Collections.Generic
Imports System.Text

Public Class Song
	
	Private Tones As List(Of RequiredTone)
	Private NoteCommands As StringBuilder
	
	Private PreviewNotes As List(Of PreviewNote)
	
	Private Name As String
	
	'Tone generators used
	Private HighGeneratorUsed, LowGeneratorUsed As Boolean
	
	'Song play parameters
	Private Dim SoundPlayLength, SoundPlayOctave, SoundPlayTempo As Integer
	
	'Create new song from RTTTL code
	Public Sub New (SongRTTTL As String)
		NoteCommands = New StringBuilder
		Tones = New List(Of RequiredTone)
		PreviewNotes = New List(Of PreviewNote)
		HighGeneratorUsed = False
		LowGeneratorUsed = False
		
		'Defaults
		SoundPlayOctave = 4
		SoundPlayLength = 4
		SoundPlayTempo = 120
		
		'Get song name
		Name = "RTTTLSong"
		If SongRTTTL.IndexOf(":") <> -1 Then
			Name = SongRTTTL.Substring(0, SongRTTTL.IndexOf(":")).Trim
			Do While Name.IndexOf(" ") <> -1
				Name = Name.Replace(" ", "")
			Loop
		End If
		NoteCommands.AppendLine("Sub " + Name)
		
		'Parse song
		PlayRTTTL (SongRTTTL)
		NoteCommands.AppendLine("End Sub")
		NoteCommands.AppendLine
		
		'Add tone generators
		NoteCommands.AppendLine("'Tone generators")
		If HighGeneratorUsed Then
			NoteCommands.AppendLine("'''@hide")
			NoteCommands.AppendLine("Sub HighToneGeneratorD")
			NoteCommands.AppendLine("	Repeat ToneLoopCount")
			NoteCommands.AppendLine("		PulseOut SoundOut, ToneLen 10us")
			NoteCommands.AppendLine("		Wait ToneLen 10us")
			NoteCommands.AppendLine("	End Repeat")
			'NoteCommands.AppendLine("	Wait " + Convert.ToInt16(240000 / SoundPlayTempo / 16).ToString.Trim + " ms")
			NoteCommands.AppendLine("	If ToneLoopCount <> 255 Then Wait 10 ms")
			NoteCommands.AppendLine("End Sub")
			NoteCommands.AppendLine
		End If
		
		If LowGeneratorUsed Then
			NoteCommands.AppendLine("'''@hide")
			NoteCommands.AppendLine("Sub LowToneGeneratorD")
			NoteCommands.AppendLine(Chr(9) + "Repeat ToneLoopCount")
			NoteCommands.AppendLine(Chr(9) + Chr(9) + "PulseOut SoundOut, ToneLen ms")
			NoteCommands.AppendLine(Chr(9) + Chr(9) + "Wait ToneLen ms")
			NoteCommands.AppendLine(Chr(9) + "End Repeat")
			NoteCommands.AppendLine("End Sub")
			NoteCommands.AppendLine
		End If
		
		'Add note playing subroutines
		NoteCommands.AppendLine("'Subroutines for notes/delays")
		NoteCommands.AppendLine
		Dim CurrTone As RequiredTone
		For Each CurrTone In Tones
			NoteCommands.AppendLine("'''@hide")
			NoteCommands.AppendLine(CurrTone.Code)
		Next
		
	End Sub
	
	Private Const SOUND_CMD_NONE As Char = ""
	Private Const SOUND_NOTE_NORMAL As Byte = 0
	Private Const SOUND_NOTE_SHARP As Byte = 1
	Private Const SOUND_NOTE_FLAT As Byte = 2
	
	''' Plays a sequence of RTTTL notes
	''' @param SoundPlayData RTTTL sequence to play
	' Adapted from routine in GCBASIC on 16/6/2012
	Private Sub PlayRTTTL(SoundPlayDataIn As String)
		
		Dim SoundPlayMode, SoundPlayBytes, SoundPlayNumber, SoundPlayNumber2 As Integer
		Dim SoundPlayNoteType As Integer
		Dim SoundPlayNoteLen, SoundPlayDots, SoundPlayNoteOctave As Integer
		Dim SoundPlayNote As Integer
		Dim SoundPlayCommand, SoundPlayByte As Char
		Dim SoundPlayData As Char() = SoundPlayDataIn.ToCharArray
		
		'Count colons, decide what mode
		'Due to RAM limitations, this sub must accept partial songs
		'If 2 colons present, assume name:params:song
		'If 1 colon, assume params:song
		'If no colon, assume song only (probably continuation of previous)
		SoundPlayMode = 2
		SoundPlayBytes = SoundPlayDataIn.Length - 1
		For SoundPlayCurrNote = 1 to SoundPlayBytes
			If SoundPlayData(SoundPlayCurrNote) = ":" Then
				SoundPlayMode -= 1
			End If
		Next
		
		'Parse commands
		SoundPlayNumber = 0
		SoundPlayBytes += 1
		For SoundPlayCurrNote = 1 To SoundPlayBytes
			
			'Get command
			If SoundPlayCurrNote = SoundPlayBytes Then
				SoundPlayByte = ","
			Else
				SoundPlayByte = SoundPlayData(SoundPlayCurrNote)
			End If
			
			'Convert to all lower case
			SoundPlayByte = Char.ToLower(SoundPlayByte)
			
			'Parse byte
			
			'Command
			If SoundPlayByte >= "a" Then
				If SoundPlayByte <= "z" Then
					SoundPlayCommand = SoundPlayByte
					SoundPlayNumber2 = SoundPlayNumber
					SoundPlayNumber = 0
					Goto SoundPlayRTTTLNextNote
				End If
			End If
			
			'Number
			If SoundPlayByte >= "0" Then
				If SoundPlayByte <= "9" Then
					SoundPlayNumber = SoundPlayNumber * 10 + Convert.ToInt16(SoundPlayByte) - 48
					Goto SoundPlayRTTTLNextNote
				End If
			End If
			
			'What parsing mode?
			Select Case SoundPlayMode
				'Parsing name
				Case 0
					If SoundPlayByte = ":" Then
						SoundPlayMode += 1
						SoundPlayCommand = SOUND_CMD_NONE
						SoundPlayNumber = 0
					End If
					
				'Parsing default values
				Case 1
					'Colon, save setting, change to song mode
					If SoundPlayByte = ":" Then
						SoundPlayMode += 1
						Goto SoundPlayRTTTLComma
					End If
					'Comma, save setting
					If SoundPlayByte = "," Then
						SoundPlayRTTTLComma:
						
						If SoundPlayCommand = "d" Then SoundPlayLength = SoundPlayNumber
						'RTTTL octave 4 = QBASIC octave 2
						If SoundPlayCommand = "o" Then SoundPlayOctave = SoundPlayNumber - 2
						If SoundPlayCommand = "b" Then
							If SoundPlayNumber <> 0 Then
								SoundPlayTempo = SoundPlayNumber
							End If
						End If
						SoundPlayNumber = 0
						SoundPlayCommand = SOUND_CMD_NONE
					End If
					
				'Parsing song
				Case 2
					'Sharp note
					If SoundPlayByte = "#" Then SoundPlayNoteType = SOUND_NOTE_SHARP
					'Increase length
					If SoundPlayByte = "." Then SoundPlayDots += 1
					
					'Play note
					If SoundPlayByte = "," Then
						'SoundPlayNumber2: Duration
						'SoundPlayCommand: Letter of note
						'SoundPlayNumber: Octave
						
						If SoundPlayCommand <> SOUND_CMD_NONE Then
							'Pause
							If SoundPlayCommand = "p" Then
								'Treat as N0 command, but set length
								If SoundPlayNumber2 = 0 Then
									SoundPlayNoteLen = SoundPlayLength
								Else
									SoundPlayNoteLen = SoundPlayNumber2
								End If
								SoundPlayTone (0, SoundPlayNoteLen)
								
							'Play note
							Else
								'Get note octave and length
								If SoundPlayNumber = 0 Then
									SoundPlayNoteOctave = SoundPlayOctave
								Else
									SoundPlayNoteOctave = SoundPlayNumber - 2
								End If
								If SoundPlayNumber2 = 0 Then
									SoundPlayNoteLen = SoundPlayLength
								Else
									SoundPlayNoteLen = SoundPlayNumber2
								End If
								'Play note
								'Translate to note number
								'ReadTable SoundPlayNoteNumbers, SoundPlayCommand - 96, SoundPlayNote
								Select Case SoundPlayCommand
									Case "c": SoundPlayNote = 1
									Case "d": SoundPlayNote = 3
									Case "e": SoundPlayNote = 5
									Case "f": SoundPlayNote = 6
									Case "g": SoundPlayNote = 8
									Case "a": SoundPlayNote = 10
									Case "b": SoundPlayNote = 12
								End Select
								
								'Get note number
								SoundPlayNote = SoundPlayNoteOctave * 12 + SoundPlayNote
								'SoundPlayToneNumber SoundPlayCommand, SoundPlayNoteType, SoundPlayNoteLen
								'Get note duration
								If SoundPlayNoteLen = 0 Then
									SoundPlayNoteLen = SoundPlayLength
								End If
								'Is note sharp or flat?
								If SoundPlayNoteType = SOUND_NOTE_SHARP Then
									SoundPlayNote += 1
									If SoundPlayNote = 85 Then
										SoundPlayNote = 84
									End If
								End If
								
								SoundPlayTone (SoundPlayNote, SoundPlayNoteLen, SoundPlayDots)
								
								'If SoundPlayDots <> 0 Then
								'	SoundPlayTone (SoundPlayNote, SoundPlayNoteLen * 2)
								'End If
								
							End If
							
							SoundPlayNumber = 0
							SoundPlayNumber2 = 0
							SoundPlayNoteType = SOUND_NOTE_NORMAL
							SoundPlayCommand = SOUND_CMD_NONE
							SoundPlayDots = 0
						End If
						
					End If
					
			End Select
			SoundPlayRTTTLNextNote:
			
		Next
	End Sub
	
	'"Play" a tone (Add the tone to the song)
	Private Sub SoundPlayTone(Note As Integer, NoteLength As Integer, Optional Dots As Integer = 0)
		'Need to convert from note number (0 - 84) and length (full, half, etc)
		'to frequency and length (ms)
		
		Dim Frequency, Length As Integer
		
		'Cannot have length 0
		If NoteLength = 0 Then Exit Sub
		
		'Note 0 is a rest
		If Note = 0 Then
			Frequency = 0
			
		'Not a rest, so create the note
		Else
			'In QB, Note 1 = C0
			' C0 = 65 Hz = Note 1
			' A1 = 220 Hz = Note 22 (wikipedia: A3/key 37)
			' A2 = 440 Hz = Note 34 (RTTTL A4)
			' A3 = 880 Hz = Note 46 (RTTTL A5)
			' B6 = 7902 Hz = Note 84
			'RTTTL octave 4 = QBASIC octave 2
			Frequency = 440 * 2 ^ ((Note - 49 + 15) / 12)
			
		End If
		
		'At tempo 140, there are 140 L4s/min
		'So 35 L1s/min, or 1.71 s/L1
		'or 0.85 s/L2 or 0.428 s/L4 or 0.214 s/L8
		
		Length = 240000 / SoundPlayTempo / NoteLength
		If Dots > 0 Then Length += Length / 2
		AddNote (Frequency, Length)
		PreviewNotes.Add(New PreviewNote(Frequency, Length))
		
	End Sub
	
	'Add note to the song
	Private Sub AddNote(Frequency As Integer, Length As Integer)
		'Is there a Tone that is already suitable?
		Dim CurrTone, NewTone As RequiredTone
		NewTone = Nothing
		For Each CurrTone In Tones
			If CurrTone.Frequency = Frequency And CurrTone.Length = Length Then
				NewTone = CurrTone
				Exit For
			End If
		Next
		'Was a tone found?
		If NewTone Is Nothing Then
			NewTone = New RequiredTone(Frequency, Length)
			Tones.Add(NewTone)
			
			'Which tone generators are needed?
			HighGeneratorUsed = HighGeneratorUsed Or NewTone.HighGeneratorUsed
			LowGeneratorUsed = LowGeneratorUsed Or NewTone.LowGeneratorUsed
		End If
		
		'Add note to song
		NoteCommands.AppendLine("	" + NewTone.SubName)
	End Sub
	
	Public ReadOnly Property Code As String
		Get
			Return NoteCommands.ToString
		End Get
	End Property
	
	Public Sub Preview
		Dim CurrNote As PreviewNote
		For Each CurrNote In PreviewNotes
			CurrNote.Play
		Next
	End Sub
	
End Class
