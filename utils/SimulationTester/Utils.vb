'
' Created by SharpDevelop.
' User: Administrator
' Date: 6/01/2007
' Time: 12:51 AM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Text
Imports System.Environment
Imports Microsoft.VisualBasic

Public Class LowLevel
	Public Shared Function IsIOPinName(PinName As String) As Boolean
		'Checks if PinName is that of an IO pin.
		'Format: [RP][A-Z][0-7] Or GP[0-7]
		If PinName.Length <> 3 Then Return False
		PinName = PinName.ToUpper
		
		If "RP".IndexOf(PinName.Substring(0, 1)) <> -1 Then
			If PinName.Substring(1, 1) < "A" Or PinName.Substring(1, 1) > "Z" Then Return False
			If "01234567".IndexOf(PinName.Substring(2, 1)) = -1 Then Return False
		Else
			If Not PinName.StartsWith("GP") Then Return False
			If "01234567".IndexOf(PinName.Substring(2, 1)) = -1 Then Return False
		End If
		
		Return True
	End Function
End Class

Public Class Location
	'Class to store a location
	
	Public Sub New(X As Single, Y As Single, Angle As Single)
		Me.X = X
		Me.Y = Y
		Me.Angle = Angle
	End Sub
	
	'X location in any units
	Public Dim X As Single
	
	'Y location in any units
	Public Dim Y As Single
	
	'Angle in degress
	Private Dim pAngle As Single
	
	Public Property Angle As Single
		Get
			Return pAngle
		End Get
		Set
			pAngle = Value
			Do While pAngle >= 360
				pAngle -= 360
			Loop
			Do While pAngle < 0
				pAngle += 360
			Loop
		End Set
	End Property
	
End Class

Public Class Utils

	Public Sub New()
	End Sub
	
	Public Shared Function CheckMatch(TestValue As Integer, TestOn As Integer, TestOff As Integer) As Boolean
		'Test bits in TestValue
		'If bits on in TestOn are on in TestValue, and bits on in TestOff are off in TestValue, return true
		Return (TestValue And TestOn) = TestOn And (Not TestValue And TestOff) = TestOff
	End Function
	
	Public Shared Sub Replace(ByRef InString As String, ByVal SearchString As String, ByVal ReplaceString As String)
		Dim StartPos, EndPos As Integer
		Dim SaveLeft, SaveRight As String
		SearchString = SearchString.ToLower
		
		ReplaceAgain:
		
		if InString.ToLower.IndexOf(SearchString) = -1 Then Exit Sub
		
		StartPos = InString.ToLower.IndexOf(SearchString)
		EndPos = SearchString.Length
		
		SaveLeft = InString.Substring(0, StartPos)
		SaveRight = InString.Substring(StartPos + EndPos)
		
		InString = SaveLeft + ReplaceString + SaveRight
		
		Goto ReplaceAgain
		
	End Sub
	
	Public Shared Function AddFullPath(CurrPath As String, FullPath As String) As String
		'Will return a fully qualified path from CurrPath.
		'FullPathIn is that path that should be added to CurrPath if not FQ
		
		Dim OutPath As String
		OutPath = CurrPath
		
		If FullPath = "" Then
			Return OutPath
		End If
		
		'Remove trailing slash from full path
		If FullPath.EndsWith("/") Or FullPath.EndsWith("\") Then FullPath = FullPath.Substring(0, FullPath.Length - 1)
		'If CurrPath not FQ, add full path
		If OutPath.Substring(1, 1) <> ":" And Not OutPath.StartsWith("\\") And Not OutPath.StartsWith("/") Then
			OutPath = FullPath + "\" + CurrPath
		End If
		
		Return OutPath
	End Function
	
	Public Shared Function GetDecValue(InChar As Char) As Integer
		If InChar >= "0" And InChar <= "9" Then Return Asc(InChar) - Asc("0")
		If InChar >= "A" And InChar <= "Z" Then Return Asc(InChar) - Asc("A") + 10
		If InChar >= "a" And InChar <= "z" Then Return Asc(InChar) - Asc("a") + 10
		Return 0
	End Function
	
	Public Shared Function GetDigitValue(InChar As Integer) As Char
		If InChar >= 0 And InChar <= 9 Then Return Chr(InChar + Asc("0"))
		If InChar >= 10 And InChar <= 36 Then Return Chr(InChar + Asc("A") - 10)
		Return ""
	End Function
	
	Public Shared Function Dec2Other(ByVal InVal As Integer, ByVal InBase As Integer) As String
		Dim OutVal As String
		Dim Temp, Place, CurVal, CurPlace As Integer
		OutVal = ""
		
		If InBase = 1 Then
			For Temp = 1 To InVal
				OutVal += "1"
			Next
			
		Else
			Place = 0
			Do While InBase ^ Place <= InVal
				Place += 1
			Loop
			Place -= 1
			
			For CurPlace = Place To 0 Step -1
				CurVal = InVal \ InBase ^ CurPlace
				InVal = InVal Mod InBase ^ CurPlace
				OutVal = OutVal + GetDigitValue(CurVal)
			Next
		End If
		Return OutVal
		
	End Function
	
	Public Shared Function Other2Dec(ByVal InVal As String, ByVal InBase As Integer) As Integer
		Dim OutVal, Temp, AddVal As Integer
		
		OutVal = 0
		For Temp = 0 To InVal.Length - 1
			AddVal = GetDecValue(InVal.Substring(Temp, 1))
			'Check if a character is too big for the current base
			If (AddVal >= InBase And InBase > 1) Or (AddVal <> 1 And InBase = 1) Then
				'MessageBox.Show(InVal.Substring(Temp, 1) + " is not a valid digit for base-" + InBase.ToString + " numbers", "Great Cow Graphical BASIC", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
				Return -99999
			End If
			
			OutVal = OutVal + AddVal * InBase ^ ((InVal.Length - 1) - Temp)
		Next
		
		Return OutVal
	End Function
	
	Public Shared Function PrefIsYes(PrefValue As String) As Boolean
		PrefValue = PrefValue.ToLower
		If PrefValue.StartsWith("y") Or PrefValue.StartsWith("t") Or PrefValue = "1" Then
			Return True
		Else
			Return False
		End If
	End Function
	
	Declare Unicode Function GetShortPathName Lib "kernel32.dll" Alias "GetShortPathNameW" (ByVal lpszLongPath As String, ByVal lpszShortPath As String, ByVal cchBuffer As Integer) As Integer
	Public Shared Function ShortFileName(LongName As String) As String
		Dim Spaces As New StringBuilder
		Dim SpaceCount As Integer
		For SpaceCount = 1 To 100
			Spaces.Append(" ")
		Next
		
		Dim OutputName As String = Spaces.ToString
		Dim TempData As Integer = GetShortPathName(LongName, OutputName, 100)
		Return OutputName.Substring(0, TempData)
	End Function
	
End Class
