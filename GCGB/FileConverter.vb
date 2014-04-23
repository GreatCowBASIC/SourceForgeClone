'
' Created by SharpDevelop.
' User: Hugh
' Date: 20/06/2009
' Time: 1:37 AM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.IO
Imports System.Windows.Forms

''Namespace Great_Cow_Graphical_BASIC

	Public Class FileConverter
		Private Dim Description, InFormat, OutFormat, ExeName, ConvDir As String
		
		Public Function GetDescription As String
			Return Description
		End Function
		
		Public Function GetInType As String
			Return InFormat
		End Function
		
		Public Function GetOutType As String
			Return OutFormat
		End Function
		
		Public Sub New(ConvDir As String, ConverterFile As String)
			
			Dim TempData, ParamName, ParamValue As String
			
			Me.ConvDir = ConvDir
			
			Dim ReadFile As New StreamReader(ConverterFile)
			Do While ReadFile.Peek() <> -1
				TempData = ReadFile.ReadLine()
				If TempData.IndexOf("=") <> -1 Then
					'Get the parameter name and value, store
					ParamName = TempData.Substring(0, TempData.IndexOf("=")).Trim.ToLower
					ParamValue = TempData.Substring(TempData.IndexOf("=") + 1).Trim
					
					If ParamName = "desc" Then
						Description = ParamValue
					Else If ParamName = "in" Then
						InFormat = ParamValue.Trim.ToLower
					Else If ParamName = "out" Then
						OutFormat = ParamValue.Trim.ToLower
					Else If ParamName = "exe" Then
						ExeName = ParamValue
					End If
					
				End If
			Loop
			ReadFile.Close
		End Sub
		
		'Converts a file
		'Returns the filename of the converted file
		Public Function Convert(InFile As String) As String
			
			'Get name of converter exe and converted file
			Dim ConvFile, ConvExe As String
			ConvFile = Lowlevel.ReplaceToolVariables("%filename%", OutFormat, InFile)
			ConvExe = Lowlevel.AddFullPath(ExeName, ConvDir)
			
			'Run converter
			Dim DoConvert As New System.Diagnostics.Process
			With DoConvert
				With .StartInfo
					.FileName = ConvExe
					.WorkingDirectory = System.Environment.GetEnvironmentVariable("Temp")
					.Arguments = """" + InFile + """"
				End With
				.Start
				.WaitForExit(30000)
			End With
			
			'Check for existence of converted file
			Try
				Dim ReadFile As New StreamReader(ConvFile)
				ReadFile.Close
			Catch
				ConvFile = ""
			End Try
			
			Return ConvFile
		End Function
		
	End Class
''End Namespace
