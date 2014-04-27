'
' Created by SharpDevelop.
' User: Hugh
' Date: 17/12/2012
' Time: 2:43 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System.IO
Imports System.Media

Public Class Beep
	'Stolen from http://social.msdn.microsoft.com/Forums/en-US/vbgeneral/thread/2816f52a-92ae-4ce1-8d9a-bf37dbd840b4/
  'Post by http://social.msdn.microsoft.com/profile/johnwein/?ws=usercard-mini
  
  Public Shared Sub Beep(Amplitude As Integer, Frequency As Integer, Duration As Integer)
    Dim A As Double = ((Amplitude * 2 ^ 15) / 1000) - 1
    Dim DeltaFT As Double = 2 * Math.PI * Frequency / 44100

    Dim Samples As Integer = 441 * Duration \ 10
    Dim Bytes As Integer = Samples * 4
    Dim Hdr() As Integer = {&H46464952, 36 + Bytes, &H45564157, _
                            &H20746D66, 16, &H20001, 44100, _
                             176400, &H100004, &H61746164, Bytes}
    Using MS As New MemoryStream(44 + Bytes)
      Using BW As New BinaryWriter(MS)
        For I As Integer = 0 To Hdr.Length - 1
          BW.Write(Hdr(I))
        Next
        For T As Integer = 0 To Samples - 1
          'Dim Sample As Short = CShort(A * Math.Sin(DeltaFT * T))
          Dim Sample As Short = CShort(A * If(Math.Sin(DeltaFT * T) > 0.7, 1, 0))
          BW.Write(Sample)
          BW.Write(Sample)
        Next
        BW.Flush()
        MS.Seek(0, SeekOrigin.Begin)
        Using SP As New SoundPlayer(MS)
          SP.PlaySync()
        End Using
      End Using
    End Using
  End Sub
End Class
