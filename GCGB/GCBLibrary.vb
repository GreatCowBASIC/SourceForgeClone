'
' Created by SharpDevelop.
' User: Administrator
' Date: 9/03/2008
' Time: 12:44 PM
'
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Collections.Generic
Public Class GCBLibrary
	
	Private Dim pFileName As String
	Private Dim pDisplayName As String
	
	Public Dim Subroutines As List(Of GCBSubroutine)
	Public Dim Constants As Dictionary(Of String, String)
	Public Dim DataTables As List(Of String)
	Public Dim Variables As List(Of VariableListItem)
	
	Public Dim Device As HardwareDevice
	
	Public Dim ChipName As String
	Public Dim ChipSpeed As Double
	
	Public Sub New(FileName As String)
		Me.FileName = FileName
		
		Subroutines = New List(Of GCBSubroutine)
		Constants = New Dictionary(Of String, String)(StringComparer.CurrentCultureIgnoreCase)
		DataTables = New List(Of String)
		Variables = New List(Of VariableListItem)
	End Sub
	
	Public Sub ClearData
		ChipName = ""
		ChipSpeed = 0
		
		Subroutines.Clear
		Constants.Clear
		DataTables.Clear
		Variables.Clear
		Device = Nothing
	End Sub
	
	Public Property FileName As String
		Get
			Return pFileName
		End Get
		Set
			pFileName = Value
			
			'Decide display name
			Dim TrimChars As Char() = {"<", ">", """"}
			Dim TempName As String = FileName.Trim(TrimChars)
			
			'Check for empty filename
			If TempName = "" Then
				pDisplayName = ""
				
			'Check for filename with / before file part
			Else If TempName.LastIndexOf("/") > TempName.LastIndexOf("\") Then
				pDisplayName = TempName.Substring(TempName.LastIndexOf("/") + 1).Trim
				
			'Check for filename with \ before file part
			Else If TempName.LastIndexOf("\") > TempName.LastIndexOf("/") Then
				pDisplayName = TempName.Substring(TempName.LastIndexOf("\") + 1).Trim
				
			'Check for name without full path
			Else
				pDisplayName = TempName
				
			End If
			
		End Set
	End Property
	
	Public ReadOnly Property DisplayName As String
		Get
			Return pDisplayName
		End Get
	End Property
	
End Class
