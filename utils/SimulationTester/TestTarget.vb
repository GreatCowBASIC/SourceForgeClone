'
' Created by SharpDevelop.
' User: Hugh
' Date: 15/04/2015
' Time: 10:52 PM
'
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Imports System.Io

Public Class TestTarget
	
	Public Enum TargetTypes
		SOFTWARE
		HARDWARE
	End Enum
	
	Public Enum ChipTypes
		PIC
		AVR
		UNKNOWN
	End Enum
	
	'Common to all
	Private Dim Name As String
	Private Dim Type As TargetTypes
	Private Dim pInitCode As List(Of String)
	
	'Capabilities
	Private Dim pRam As Integer = 0
	Private Dim pFlash As Integer = 0
	
	'Software simulator only
	Private Dim pSimRobot As String
	
	'Hardware simulator only
	Private Dim pSlot As Integer
	Private Dim pChipName As String
	Private Dim pPowerUpDelay As Integer = 10
	
	Public ReadOnly Property TargetType As TargetTypes
		Get
			Return Type
		End Get
	End Property
	
	Public ReadOnly Property RequiresHardware As Boolean
		Get
			Return Type = TargetTypes.HARDWARE
		End Get
	End Property
	
	Public ReadOnly Property InitCode As IList(Of String)
		Get
			Return pInitCode.AsReadOnly
		End Get
	End Property
	
	Public ReadOnly Property Ram As Integer
		Get
			Return pRam
		End Get
	End Property
	
	Public ReadOnly Property Flash As Integer
		Get
			Return pFlash
		End Get
	End Property
	
	Public ReadOnly Property PowerUpDelay As Integer
		Get
			Return pPowerUpDelay
		End Get
	End Property
	
	Public ReadOnly Property Slot As Integer
		Get
			Return pSlot
		End Get
	End Property
	
	Public ReadOnly Property ChipName As String
		Get
			Return pChipName
		End Get
	End Property
	
	Public ReadOnly Property ChipType As ChipTypes
		Get
			With ChipName.ToLower
				If .StartsWith("10f") Or .StartsWith("12f") Or .StartsWith("16f") Or .StartsWith("18f") Then
					Return ChipTypes.PIC
				Else If .StartsWith("90") Or .StartsWith("tiny") Or .StartsWith("mega") Then
					Return ChipTypes.AVR
				Else
					Return ChipTypes.UNKNOWN
				End If
			End With
		End Get
	End Property
	
	Public ReadOnly Property SimRobot As String
		Get
			Return pSimRobot
		End Get
	End Property
	
	Public Sub New(SourceFile As String)
		
		pInitCode = New List(Of String)
		
		Dim TempData, CurrentTag, ItemName, ItemValue, InLine As String
		
		currentTag = ""
			
		Dim ReadFile As New StreamReader(SourceFile)
		Do While ReadFile.Peek() <> -1
     		InLine = ReadFile.ReadLine()
     		TempData = InLine.Replace(Convert.ToChar(9), " ").Trim
     		'If line in is not blank or a comment, read
     		If TempData.Length > 0 And Not TempData.StartsWith("'") Then
     			
     			'Line read is a section start
     			If TempData.StartsWith("[") And TempData.EndsWith("]") Then
     				CurrentTag = TempData.Substring(1, TempData.Length - 2).Trim.ToLower
     				
     				
     			'Line read is an attribute
     			Else If CurrentTag = "target" And TempData.IndexOf("=") <> -1 Then
     				ItemName = TempData.Substring(0, TempData.IndexOf("=")).Trim.ToLower
     				ItemValue = TempData.Substring(TempData.IndexOf("=") + 1).Trim
     				
     				'Reading a target section
     				If CurrentTag = "target" Then
     					Select Case ItemName
     						Case "name": Name = ItemValue
     						Case "type"
     							If ItemValue.ToLower.StartsWith("s") Then
     								Type = TargetTypes.SOFTWARE
     							Else If ItemValue.ToLower.StartsWith("h") Then
     								Type = TargetTypes.HARDWARE
     							Else
     								MessageBox.Show("Unknown target type:" + ItemValue)
     							End If
     						Case "ram": pRam = Integer.Parse(ItemValue)
     						Case "flash": pFlash = Integer.Parse(ItemValue)
     						Case "simrobot": pSimRobot = ItemValue
     						Case "powerupdelay": pPowerUpDelay = Integer.Parse(ItemValue)
     						Case "slot": pSlot = Integer.Parse(ItemValue)
     						Case "chipname": pChipName = ItemValue
     						Case Else:
	     						MessageBox.Show("Unrecognised item in target file section " + currentTag + ": " + ItemName)
     					End Select
     					
     				End If
     				
     			Else If CurrentTag = "initcode" Then
     				pInitCode.Add(InLine)
     			End If
 			End If
 		Loop
 		ReadFile.Close()
		
	End Sub
	
	Public Overrides Function ToString As String
		Return Name
	End Function
	
End Class
