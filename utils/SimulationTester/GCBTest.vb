'
' Created by SharpDevelop.
' User: Hugh
' Date: 15/04/2015
' Time: 10:52 PM
'
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Imports System.IO

Public Class GCBTest
	
	Public Enum CaptureSources
		UART
		LCD
		NONE
	End Enum
	
	'Test name
	Private Dim Name As String
	
	'Source file that contains test code
	Private Dim pSourceFile As String
	Private Dim pSourceCode As List(Of String)
	
	'Run time (ms)
	Private Dim pRunTime As Integer = 2000
	
	'Requirements for test to work
	Private Dim pRequiredRam As Integer = -1
	Private Dim pRequiredFlash As Integer = -1
	
	'Capture/verification
	Private Dim CaptureSource As CaptureSources
	Private Dim pMustFind As List(Of String)
	Private Dim pMustNotFind As List(Of String)
	
	Public ReadOnly Property MustFind As IList(Of String)
		Get
			Return pMustFind.AsReadOnly
		End Get
	End Property
	
	Public ReadOnly Property MustNotFind As IList(Of String)
		Get
			Return pMustNotFind.AsReadOnly
		End Get
	End Property
	
	Public ReadOnly Property SourceFile As String
		Get
			Return pSourceFile
		End Get
	End Property
	
	Public ReadOnly Property SourceCode As IList(Of String)
		Get
			Return pSourceCode.AsReadOnly
		End Get
	End Property
	
	Public ReadOnly Property RunTime As Integer
		Get
			Return pRunTime
		End Get
	End Property
	
	Public ReadOnly Property RequiredRam As Integer
		Get
			Return pRequiredRam
		End Get
	End Property
	
	Public ReadOnly Property RequiredFlash As Integer
		Get
			Return pRequiredFlash
		End Get
	End Property
	
	Public Sub New(SourceFile As String)
		pMustFind = New List(Of String)
		pMustNotFind = New List(Of String)
		CaptureSource = CaptureSources.NONE
		
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
     				
     				'Reading a test section
     				If CurrentTag = "test" Then
     					Select Case ItemName
     						Case "name": Name = ItemValue
     						Case "sourcefile": pSourceFile = ItemValue
     						Case "capturesource"
     							If ItemValue.ToLower = "uart" Then
     								CaptureSource = CaptureSources.UART
     							Else If ItemValue.ToLower = "lcd" Then
     								CaptureSource = CaptureSources.LCD
     							End If
     						Case "runtime": pRunTime = ItemValue
     						Case "requiredram": pRequiredRam = ItemValue
     						Case "requiredflash": pRequiredFlash = ItemValue
     							
     						Case Else:
	     						MessageBox.Show("Unrecognised item in test file section " + currentTag + ": " + ItemName)
     					End Select
     					
     				End If
     				
     			Else If CurrentTag = "mustfind" Then
     				pMustFind.Add(TempData)
     				
     			Else If CurrentTag = "mustnotfind" Then
     				pMustNotFind.Add(TempData)
     				
     			End If
 			End If
 		Loop
 		ReadFile.Close()
		
		'Load source code
		pSourceCode = New List(Of String)
		Try
			ReadFile = New StreamReader(Utils.AddFullPath(pSourceFile, MainForm.InstallDir + "\Tests\"))
			Do While ReadFile.Peek() <> -1
	     		pSourceCode.Add(ReadFile.ReadLine())
	     	Loop
			ReadFile.Close
		Catch Ex As FileNotFoundException
			MessageBox.Show("Could not find " + SourceFile, "Test loading error")
		End Try
		
	End Sub
	
	Public Overrides Function ToString As String
		Return Name
	End Function
	
End Class
