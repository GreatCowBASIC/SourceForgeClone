'
' Created by SharpDevelop.
' User: Hugh
' Date: 16/04/2015
' Time: 12:06 AM
'
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Imports System.IO

Public Class Settings
	
	Private Dim pPICkit2Path As String
	Private Dim pAVRDudePath As String
	Private Dim pGCBASICPath As String
	Private Dim pGCSIMExe As String
	
	Public ReadOnly Property PICkit2Path As String
		Get
			Return pPICkit2Path
		End Get
	End Property
	
	Public ReadOnly Property AVRDudePath As String
		Get
			Return pAVRDudePath
		End Get
	End Property
	
	Public ReadOnly Property GCBASICPath As String
		Get
			Return pGCBASICPath
		End Get
	End Property
	
	Public ReadOnly Property GCSIMExe As String
		Get
			Return pGCSIMExe
		End Get
	End Property
	
	Public Sub New(SourceFile As String)
		
		Dim TempData, CurrentTag, ItemName, ItemValue As String
		
		currentTag = ""
			
		Dim ReadFile As New StreamReader(SourceFile)
		Do While ReadFile.Peek() <> -1
     		TempData = ReadFile.ReadLine().Replace(Convert.ToChar(9), " ").Trim
     		'If line in is not blank or a comment, read
     		If TempData.Length > 0 And Not TempData.StartsWith("'") Then
     			
     			'Line read is a section start
     			If TempData.StartsWith("[") And TempData.EndsWith("]") Then
     				CurrentTag = TempData.Substring(1, TempData.Length - 2).Trim.ToLower
     				
     			'Line read is an attribute
     			Else If TempData.IndexOf("=") <> -1 Then
     				ItemName = TempData.Substring(0, TempData.IndexOf("=")).Trim.ToLower
     				ItemValue = TempData.Substring(TempData.IndexOf("=") + 1).Trim
     				
     				'Reading a paths section
     				If CurrentTag = "paths" Then
     					Select Case ItemName
     						Case "pickit2": pPICkit2Path = ItemValue
     						case "avrdude": pAVRDudePath = ItemValue
     						Case "gcbasic": pGCBASICPath = ItemValue
     						Case "gcsimexe": pGCSIMExe = ItemValue
     						
     						Case Else:
	     						MessageBox.Show("Unrecognised item in target file section " + currentTag + ": " + ItemName)
     					End Select
     					
     				End If
     				
     			End If
 			End If
 		Loop
 		ReadFile.Close()
		
	End Sub
	
End Class
