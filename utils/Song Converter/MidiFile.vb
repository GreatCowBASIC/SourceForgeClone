'
' Created by SharpDevelop.
' User: Hugh
' Date: 18/12/2012
' Time: 12:16 AM
'
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System.IO

Public Class RevBinaryReader
	Inherits BinaryReader
	'Binary read which can handle opposite endianness in Int16 and Int32 values
	
	Public Sub New(input As Stream, encoding As System.Text.Encoding)
		Mybase.New(input, encoding)
	End Sub
	
	Public Function ReadInt16Rev As Integer
		Dim bytes As Byte()
		bytes = ReadBytes(2)
		Return bytes(0) * 256 + bytes(1)
	End Function
	
	Public Function ReadInt32Rev As Integer
		Dim bytes As Byte()
		bytes = ReadBytes(4)
		Return bytes(0) * 256 ^ 3 + bytes(1) * 256 ^ 2 + bytes(2) * 256 + bytes(3)
	End Function
	
	Public Function ReadIntVar As Integer
		'Read variable length record from midi file
		Dim currByteNo As Integer
		Dim currByte As Byte
		Dim outVal As Integer = 0
		For currByteNo = 1 To 4
			'Read a byte
			currByte = ReadByte
			'Is bit 7 set?
			If (currByte And 128) <> 0 Then
				'Bit 7 set, another byte follows
				outVal = outVal * 128 + (currByte And 127)
			Else
				'Bit 7 clear, no following byte
				outVal = outVal * 128 + (currByte And 127)
				Return outVal
			End If
		Next
		
		Return outVal
	End Function
	
End Class

Public MustInherit Class MidiEvent
	Public Dim DeltaTime As Integer
	
	Public Overrides MustOverride Function ToString As String
	
End Class

Public Class MidiChannelEvent
	Inherits MidiEvent
	
	Public Dim Type As EventTypes
	Public Dim Channel As Integer
	Public Dim Params(2) As Byte
	
	Public Sub New(Type As EventTypes)
		Me.Type = Type
	End Sub
	
	'Possible types for a midi event
	Public Enum EventTypes
		NOTE_OFF = 8
		NOTE_ON = 9
		NOTE_AFTERTOUCH = 10
		CONTROLLER = 11
		PROGRAM_CHANGE = 12
		CHANNEL_AFTERTOUCH = 13
		PITCH_BEND = 14
	End Enum
	
	'Different midi events have different parameter counts
	'Must read correct number from file
	Public ReadOnly Property ParamCount As Integer
		Get
			If Type = EventTypes.PROGRAM_CHANGE Then Return 1
			If Type = EventTypes.CHANNEL_AFTERTOUCH Then Return 1
			Return 2
		End Get
	End Property
	
	Public Overrides Function ToString As String
		Dim outVal As String = "CHANNEL "
		outVal += DeltaTime.ToString + " "
		outVal += Type.ToString + " "
		Try
			outVal += Params(0).ToString + " "
			If Params.Length = 2 Then outVal += Params(1).ToString + " "
		Catch
			outVal += "param error"
		End Try
		Return outVal
	End Function
	
End Class

Public Class MidiMetaEvent
	Inherits MidiEvent
	
	Public Dim Type As MetaTypes
	Public Dim Length As Integer
	Public Dim Data() As Byte
	
	'Possible times for a meta event
	Public Enum MetaTypes
		SEQ_NUM = 0
		TEXT = 1
		COPYRIGHT = 2
		SEQ_NAME = 3
		INST_NAME = 4
		LYRICS = 5
		MARKER = 6
		CUE_POINT = 7
		CHAN_PREFIX = 32
		END_OF_TRACK = 47
		SET_TEMPO = 81
		SMPTE_OFFSET = 84
		TIME_SIG = 88
		KEY_SIG = 89
		SEQ_SPECIFIC = 127
	End Enum
	
	Public Sub New(Type As MetaTypes)
		Me.Type = Type
	End Sub
	
	Public Function HasStringData As Boolean
		Select Case Type
			Case MetaTypes.TEXT, _
			     MetaTypes.COPYRIGHT, _
			     MetaTypes.SEQ_NAME, _
			     MetaTypes.INST_NAME, _
			     MetaTypes.LYRICS, _
			     MetaTypes.MARKER, _
			     MetaTypes.CUE_POINT
			     
				Return True
			Case Else
				Return False
		End Select
		
	End Function
	
	Public ReadOnly Property StringData As String
		Get
			If HasStringData Then
				Return System.Text.Encoding.UTF8.GetString(Data)
			Else
				Return ""
			End If
		End Get
	End Property
	
	Public Overrides Function ToString As String
		Dim outVal As String = "META "
		outVal += Type.ToString + " "
		outVal += Length.ToString + " "
		outVal += StringData + " "
		Return outVal
	End Function
	
End Class

Public Class MidiTrack
	Public Dim Number As Integer
	Public Dim Events As List(Of MidiEvent)
	
	Public Sub New(Number As Integer)
		Me.Number = Number
		Events = New List(Of MidiEvent)
	End Sub
	
End Class

Public Class MidiFile
	
	Public Dim fileFormat, trackCount, ticksPerBeat As Integer
	Public Dim SMPTEFrames, ticksPerFrame As Integer
	
	Public Dim Tracks As List(Of MidiTrack)
	
	'Create from file on disk
	Public Sub New(FileName As String)
		Dim stream As FileStream = File.OpenRead(FileName)
		Dim reader As New RevBinaryReader(stream, System.Text.Encoding.UTF8)
		
		'Check file header
		Dim header As String
		header = New String(reader.ReadChars(4))
		If header <> "MThd" Then
			Throw New Exception("File not loaded: missing header")
		End If
		
		'Chunk size, should be 6
		Dim chunkSize As Integer = reader.ReadInt32Rev
		If chunkSize <> 6 Then
			Throw New Exception("File not loaded: incorrect chunk size " + chunkSize.ToString)
		End If
		
		'Create list for tracks
		Tracks = New List(Of MidiTrack)
		
		'Format
		fileFormat = reader.ReadInt16Rev
		'Track count
		trackCount = reader.ReadInt16Rev
		
		'Time division
		Dim timeDivIn As Integer = reader.ReadInt16Rev
		'Ticks per beat or frames per second? Check bit 15
		If (timeDivIn And 32768) <> 0 Then
			'Bit 15 set, ticks per beat
			ticksPerBeat = timeDivIn And 32767
		Else
			'Bit 15 clear, frames per second
			ticksPerBeat = 0
			SMPTEFrames = (timeDivIn And &H7F00) \ 256
			ticksPerFrame = timeDivIn And 255
		End If
		
		'Read tracks from file
		Dim CurrTrackNo, trackEndPos As Integer
		Dim CurrTrack As MidiTrack
		Dim NewChannelEvent As MidiChannelEvent
		Dim NewMetaEvent As MidiMetaEvent
		
		For CurrTrackNo = 1 To trackCount
			'Read header
			header = New String(reader.ReadChars(4))
			If header <> "MTrk" Then
				Throw New Exception("File not loaded: missing track header")
			End If
			
			'Chunk size
			chunkSize = reader.ReadInt32Rev
			trackEndPos = stream.Position + chunkSize
			
			'Create track object
			CurrTrack = New MidiTrack(CurrTrackNo)
			Tracks.Add(CurrTrack)
			
			'Read track events
			Try
				Dim deltaTime, eventType, lastEventType As Integer
				lastEventType = 0
				Dim metaType, metaLen As Integer
				Do While stream.Position < trackEndPos
					'Read delta time
					deltaTime = reader.ReadIntVar
					eventType = reader.ReadByte
					
					'Running mode?
					'In running mode if MSB of eventType = 0
					If eventType < 128 Then
						'Repeat last command
						NewChannelEvent = New MidiChannelEvent((lastEventType And 240) \ 16)
						With NewChannelEvent
							.DeltaTime = deltaTime
							.Channel = lastEventType And 15
							'.Params = reader.ReadBytes(.ParamCount)
							Dim newData(.ParamCount) As Byte
							newData(0) = eventType
							If .ParamCount = 2 Then
								newData(1) = reader.ReadByte
							End If
							.Params = newData
						End With
						CurrTrack.Events.Add(NewChannelEvent)
						
					'Meta event, system event or regular midi event?
					Else If eventType = 255 Then
						'Meta event
						'Get type and length
						metaType = reader.ReadByte
						metaLen = reader.ReadIntVar
						NewMetaEvent = New MidiMetaEvent(metaType)
						With NewMetaEvent
							.DeltaTime = deltaTime
							.Length = metaLen
							.Data = reader.ReadBytes(metaLen)
						End With
						CurrTrack.Events.Add(NewMetaEvent)
						
					Else If eventType = 240 Then
						'System event
						metaType = reader.ReadByte
						metaLen = reader.ReadIntVar
						
						reader.ReadBytes(metaLen)
					Else
						'Normal event
						lastEventType = eventType
						NewChannelEvent = New MidiChannelEvent((lastEventType And 240) \ 16)
						With NewChannelEvent
							.DeltaTime = deltaTime
							.Channel = lastEventType And 15
							.Params = reader.ReadBytes(.ParamCount)
						End With
						CurrTrack.Events.Add(NewChannelEvent)
					End If
					
				Loop
			Catch Ex As System.IO.EndOfStreamException
				
			End Try
			
			'Seek to end of track
			stream.Position = trackEndPos
		Next
		
		reader.Close
	End Sub
	
End Class
