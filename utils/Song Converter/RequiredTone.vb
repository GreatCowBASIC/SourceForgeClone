'
' Created by SharpDevelop.
' User: Hugh
' Date: 15/06/2012
' Time: 10:08 PM
'
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System.Text

Public Class RequiredTone
	
	Private Dim pFrequency, pLength As Integer
	Private Dim HighUsed, LowUsed As Boolean
	Private Dim pSubName As String
	Private Dim OutputCode As StringBuilder
	
	Public Sub New (Frequency As Integer, Length As Integer)
		'Store frequency and duration of note
		'Frequency is in Hz
		'Length is in ms
		pFrequency = Frequency
		pLength = Length
		
		'Clear flags which indicate which tone generator subroutine is used
		HighUsed = False
		LowUsed = False
		
		Dim Period, PulseTime, LoopCount As Double
		Dim TotalLoopCount, ThisLoopCount As Integer
		
		'Generate code
		OutputCode = New StringBuilder
		'Delay or tone?
		If pFrequency <= 0 Then
			pSubName =  "ToneDelay" + pLength.ToString + "ms"
			OutputCode.AppendLine("Sub " + pSubName)
			OutputCode.AppendLine("	Wait " + pLength.ToString + " ms")
			
		Else
			pSubName = "Tone" + pFrequency.ToString + "For" + pLength.ToString + "ms"
			OutputCode.AppendLine("Sub " + pSubName)
			
			Period = 1000 / pFrequency
			PulseTime = Period / 2
			LoopCount = Length / Period
			TotalLoopCount = LoopCount
			
			If PulseTime <= 2.55 Then
				OutputCode.AppendLine("	ToneLen = " + Convert.ToInt16(PulseTime * 100).ToString.Trim)
			Else
				OutputCode.AppendLine("	ToneLen = " + Convert.ToInt16(PulseTime).ToString.Trim)
			End If
			
			Do While TotalLoopCount > 0
				ThisLoopCount = TotalLoopCount
				If ThisLoopCount > 255 Then
					ThisLoopCount = 255
				End If
				TotalLoopCount -= ThisLoopCount
				
				OutputCode.AppendLine("	ToneLoopCount = " + ThisLoopCount.ToString.Trim)
				If PulseTime <= 2.55 Then
					OutputCode.AppendLine("	HighToneGeneratorD")
					HighUsed = True
				Else
					OutputCode.AppendLine("	LowToneGeneratorD")
					LowUsed = True
				End If
			Loop
			
		End If
		
		OutputCode.AppendLine("End Sub")
		
	End Sub
	
	Public ReadOnly Property Frequency As Integer
		Get
			Return pFrequency
		End Get
	End Property
	
	Public ReadOnly Property Length As Integer
		Get
			Return pLength
		End Get
	End Property
	
	Public ReadOnly Property LowGeneratorUsed As Boolean
		Get
			Return LowUsed
		End Get
	End Property
	
	Public ReadOnly Property HighGeneratorUsed As Boolean
		Get
			Return HighUsed
		End Get
	End Property
	
	Public ReadOnly Property SubName As String
		Get
			'Return name of subroutine for this note
			Return pSubName
		End Get
	End Property
	
	Public ReadOnly Property Code As String
		Get
			'Return code for subroutine to play note
			Return OutputCode.ToString
		End Get
	End Property
	
End Class
