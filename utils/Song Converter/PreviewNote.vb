'
' Created by SharpDevelop.
' User: Hugh
' Date: 17/12/2012
' Time: 2:46 PM
'
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Public Class PreviewNote
	
	Private Dim Frequency, Duration As Integer
	
	Public Sub New(Frequency As Integer, Duration As Integer)
		Me.Frequency = Frequency
		Me.Duration = Duration
	End Sub
	
	Public Sub Play
		If Frequency = 0 Then
			'Rest
			System.Threading.Thread.Sleep(Duration)
			
		Else
			'Play note
			Beep.Beep(250, Frequency, Duration)
		End If
	End Sub
	
End Class
