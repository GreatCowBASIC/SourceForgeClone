'	Programmer Editor, a program to edit GCBASIC settings files
'	Main File
'	Copyright (C) 2017 Hugh Considine
'
'	This program is free software; you can redistribute it and/or modify
'	it under the terms of the GNU General Public License as published by
'	the Free Software Foundation; either version 2 of the License, or
'	(at your option) any later version.
'
'	This program is distributed in the hope that it will be useful,
'	but WITHOUT ANY WARRANTY; without even the implied warranty of
'	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'	GNU General Public License for more details.
'
'	You should have received a copy of the GNU General Public License
'	along with this program; if not, write to the Free Software
'	Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
'
' Created by SharpDevelop.
' User: Hugh
' Date: 19/12/2017
' Time: 9:47 PM
'
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Public Partial Class MainForm
	Public Const FilenamePrefs As String = "/prefs.dat"
	Public Const FilenameUserPrefs As String = "%appdata%/gcgb.ini"
	Public Dim Shared InstallDir As String
	
	'Preferences
	Private Dim Shared Prefs As Preferences
	
	'Translator
	Public Dim Shared Translator As LanguageTranslator
	
	Public Dim Shared MainformInstance As MainForm
	
	Public Sub New()
		' The Me.InitializeComponent call is required for Windows Forms designer support.
		Me.InitializeComponent()
		
		'Command Line
		Dim CommandLine As String = ""
		Try
			CommandLine = System.Environment.GetCommandLineArgs(1)
		Catch
		End Try
		
		'Get install dir
		InstallDir = Application.ExecutablePath
 		If InstallDir.IndexOf("\") <> -1 Then InstallDir = InstallDir.Substring(0, InstallDir.LastIndexOf("\"))
		If InstallDir.IndexOf("/") <> -1 Then InstallDir = InstallDir.Substring(0, InstallDir.LastIndexOf("/"))
		If InstallDir.ToLower.IndexOf("\bin") <> -1 Then InstallDir = InstallDir.Substring(0, InstallDir.ToLower.IndexOf("\bin"))
		If InstallDir.ToLower.IndexOf("/bin") <> -1 Then InstallDir = InstallDir.Substring(0, InstallDir.ToLower.IndexOf("/bin"))
		MainformInstance = Me
		
		'Get preferences object
		Prefs = New Preferences(CommandLine, Preferences.PrefsModes.GCB)
		Prefs.LoadPreferences
		
		'Load translation
		Translator = New LanguageTranslator(Prefs)
		
		Dim fPrefsWindow As New PreferencesWindow
		fPrefsWindow.Preferences = Prefs
		fPrefsWindow.StartPosition = FormStartPosition.CenterScreen
		fPrefsWindow.ShowInTaskbar = True
		fPrefsWindow.HideGCGBOptions
		fPrefsWindow.ShowDialog
		
		Application.Exit
		End
		
	End Sub
	
	Public Sub UpdateProgrammerList
		'Do nothing, just here so code from GCGB behaves
	End Sub
End Class

Public Class LowLevel
	'Selected routines taken from GCGB, required for preference editing code to work
	
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
		
	Public Shared Function ReplaceToolVariables(InData As String, Optional FNExtension As String = "", Optional FileNameIn As String = "") As String
		
		Dim FileName, FileNameNoExt, OutData As String
		Dim PD As Integer
		OutData = InData
		
		FileName = FileNameIn
		
		'Get filename without extension
		FileNameNoExt = ""
		For PD = FileName.Length To 1 Step -1
			If FileName.Substring(PD - 1, 1) = "." Then
				FileNameNoExt = FileName.Substring(0, PD - 1)
				Exit For
			End If
		Next
		'If requested, change filename extension
		If FNExtension <> "" Then
			FileName = FileNameNoExt + "." + FNExtension
		End If
		
		'Items typically found in paths
		Do While OutData.ToUpper.IndexOf("%APPDATA%") <> -1: Replace (OutData, "%appdata%", Environ("APPDATA")): Loop
		Do While OutData.ToUpper.IndexOf("%ALLUSERSAPPDATA%") <> -1: Replace (OutData, "%allusersappdata%", Environ("ALLUSERSPROFILE") + "\Application Data"): Loop
		Do While OutData.ToUpper.IndexOf("%TEMP%") <> -1: Replace (OutData, "%temp%", Environ("TEMP")): Loop
		Do While OutData.ToUpper.IndexOf("%INSTDIR%") <> -1: Replace (OutData, "%instdir%", MainForm.InstallDir): Loop
		
		Return OutData
	End Function

End Class