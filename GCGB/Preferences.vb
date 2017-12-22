'
' Created by SharpDevelop.
' User: Hugh
' Date: 21/09/2013
' Time: 7:23 PM
'
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Imports System.Collections.Generic
Imports System.IO

Public Class SettingSection
	Public Dim Name As String
	Public Dim Settings As List(Of Setting)
	
	Public Sub New (Name As String)
		Me.Name = Name
		Settings = New List(Of Setting)
	End Sub
	
	Public Sub AddSetting(Name As String, Value As String)
		'Check to see if setting already exists
		Dim s as Setting
		For Each s In Settings
			'If setting exists, change its value and exit
			If s.Name.ToLower = Name.ToLower Then
				s.Value = Value
				Exit Sub
			End If
		Next
		
		'Add the new setting
		Settings.Add(New Setting(Name, Value))
	End Sub
	
	Public Sub RemoveSetting(Name As String)
		Dim s as Setting
		For Each s In Settings
			'If setting exists, remove
			If s.Name.ToLower = Name.ToLower Then
				Settings.Remove(s)
				Exit Sub
			End If
		Next
	End Sub
	
	Public Function GetValue(Name As String) As String
		Dim s as Setting
		For Each s In Settings
			If s.Name.ToLower = Name.ToLower Then Return s.Value
		Next
		Return ""
	End Function
	
End Class

Public Class Setting
	Public Dim Name As String
	Public Dim Value As String
	
	Public Sub New (Name As String, Value As String)
		Me.Name = Name
		Me.Value = Value
	End Sub
End Class


Public Class Preferences
	
	Public Event Changed
	
	Private Dim PrefFile As String = ""
	Public Dim PrefGroups As New List(Of SettingSection)
	Private Dim PrefsMode As PrefsModes
	
	Public Enum PrefsModes
		GCGB
		GCB
	End Enum
	
	Public Sub FireChanged
		'Notify listeners of change to preferences
		RaiseEvent Changed
	End Sub
	
	Public Sub New(Optional PrefFile As String = "", Optional PrefsMode As PrefsModes = PrefsModes.GCGB)
		If PrefFile <> "" Then
			Me.PrefFile = PrefFile
		End If
		Me.PrefsMode = PrefsMode
	End Sub
	
	Public Shared Function PrefIsYes(PrefValue As String) As Boolean
		PrefValue = PrefValue.ToLower
		If PrefValue.StartsWith("y") Or PrefValue.StartsWith("t") Or PrefValue = "1" Then
			Return True
		Else
			Return False
		End If
	End Function
	
	Public Sub LoadPreferences
   		Dim TempData As String = ""
   		Dim CurrentTag, MsgName, MsgValue As String
   		Dim CurrentSection As SettingSection = Nothing
   		
   		Dim PrefFile(20) As String
   		Dim PrefFiles As Integer
   		Dim CurrFile, CheckFile As Integer
   		Dim FileNotFound As Boolean
   		
   		If Me.PrefFile = "" Then
   			Me.PrefFile = MainForm.InstallDir + Mainform.FilenamePrefs
   		End If
   		PrefFile(1) = Me.PrefFile
   		PrefFiles = 1
   		
   		CurrFile = 0
   		Do
   			CurrFile += 1
	   		Try
	   			Dim ReadFile As New StreamReader(PrefFile(CurrFile))
	   			
	   			Do While ReadFile.Peek() <> -1
		     		TempData = ReadFile.ReadLine().Replace(Microsoft.VisualBasic.Chr(9), " ").Trim
		     		
		     		If TempData.Length > 0 And TempData.IndexOf("'") <> 0 Then
		     			
		     			If TempData.StartsWith("[") And TempData.EndsWith("]") Then
		     				CurrentTag = TempData.Substring(1, TempData.Length - 2).Trim.ToLower
		     				'Check if section exists
		     				Dim FoundSection As Boolean = False
		     				For Each CurrentSection In PrefGroups
		     					If CurrentSection.Name = CurrentTag Then
		     						FoundSection = True
		     						Exit For
		     					End If
		     				Next
		     				'If not found, add it
		     				If Not FoundSection Then
		     					CurrentSection = New SettingSection(CurrentTag)
		     					PrefGroups.Add(CurrentSection)
		     				End If
		     				
		     			Else If TempData.IndexOf("=") <> -1 Then
		     				MsgName = TempData.Substring(0, TempData.IndexOf("=")).Trim.ToLower
		     				MsgValue = TempData.Substring(TempData.IndexOf("=") + 1).Trim
		     				
		     				CurrentSection.AddSetting(MsgName, MsgValue)
		     				
		     			Else If TempData.ToLower.StartsWith("include ") Then
		     				MsgValue = LowLevel.ReplaceToolVariables(TempData.Substring(TempData.IndexOf(" ") + 1).Trim)
		     				FileNotFound = True
		     				For CheckFile = 1 To PrefFiles
		     					If MsgValue.ToLower = PrefFile(CheckFile).ToLower Then
		     						FileNotFound = False
		     						Exit For
		     					End If
		     				Next
		     				
		     				If FileNotFound And File.Exists(MsgValue) Then
		     					PrefFiles += 1
		     					PrefFile(PrefFiles) = MsgValue
		     				End If
		     			End If
		     			
	     			End If
	     		Loop
	     		
	     		ReadFile.Close()
	     		
	     	Catch
	   		End Try
	    Loop While CurrFile < PrefFiles
	    
    End Sub
	
	Public Sub SavePreferences
		Dim Temp As Integer = 0
		Dim UserIni As StreamWriter
		
		'Save in Application Data?
		If PrefIsYes(GetPref("GCGB", "UseAppdata")) Then
			'Open Files
			'User/Application Data
			UserIni = New StreamWriter(LowLevel.ReplaceToolVariables(MainForm.FileNameUserPrefs))
			'Write header
    		If PrefsMode = PrefsModes.GCGB Then
    			UserIni.WriteLine("'Preferences file for Great Cow Graphical BASIC")
    		Else
    			UserIni.WriteLine("'Preferences file for Great Cow BASIC")
    		End If
    		UserIni.WriteLine("'Location: [user]/Application Data")
    		UserIni.WriteLine("")
			
		Else
			'Open File
    		UserIni = New StreamWriter(PrefFile)
    		'Write header
    		If PrefsMode = PrefsModes.GCGB Then
    			UserIni.WriteLine("'Preferences file for Great Cow Graphical BASIC")
    			UserIni.WriteLine("'Location: GCGB install dir")
    		Else
    			UserIni.WriteLine("'Preferences file for Great Cow BASIC")
    			UserIni.WriteLine("'Location: GCB install (or custom) dir")
    		End If
    		UserIni.WriteLine("")
    	End If
    	
    	'Data
    	Dim section As SettingSection
		For Each section In PrefGroups
			UserIni.WriteLine("[" + section.Name + "]")
				
			Dim s As Setting
			For Each s In section.Settings
				UserIni.WriteLine(s.Name + " = " + s.Value)
			Next
				
			UserIni.WriteLine()
		Next
    	
    	'Close
    	UserIni.Close()
    	
	End Sub
			
	Public Function GetPref(ByVal SectionName As String, ByVal PrefName As String) As String
		Dim Temp As Integer = 0
		Dim PrefLoc As Integer = 0
		
		SectionName = SectionName.Trim.ToLower
		PrefName = PrefName.Trim.ToLower
		
		If PrefGroups Is Nothing Then
			Return ""
		End If
		
		Dim section As SettingSection
		For Each section In PrefGroups
			If section.Name.ToLower = SectionName Then
				
				Dim s As Setting
				For Each s In section.Settings
					If s.Name.ToLower = PrefName Then
						Return s.Value
					End If
				Next
				
			End If
		Next
		
		Return ""
		
	End Function
	
	Public Sub SetPref(ByVal SectionName As String, ByVal PrefName As String, ByVal PrefValue As String)
		Dim Temp As Integer = 0
		Dim PrefLoc As Integer = 0
		
		Dim section As SettingSection
		For Each section In PrefGroups
			If section.Name.ToLower = SectionName.ToLower Then
				
				Dim s As Setting
				For Each s In section.Settings
					If s.Name.ToLower = PrefName.ToLower Then
						s.Value = PrefValue
						Return
					End If
				Next
				
				section.AddSetting(PrefName, PrefValue)
				Return
			End If
		Next
		
		section = New SettingSection(SectionName)
		section.AddSetting(PrefName, PrefValue)
		PrefGroups.Add(section)
	End Sub
	
	Public Function GetProgrammerID(ByVal BaseName As String) As String
		Dim ToolName, OutName As String
		Dim Pos As Integer
		
		'Get a possible name from the filename, stripping off the path and extension
		For Pos = BaseName.Length - 1 To 0 Step -1
			If "\/".IndexOf(BaseName.Substring(Pos, 1)) <> -1 Then
				BaseName = BaseName.Substring(Pos + 1)
				Exit For
			Else If BaseName.Substring(Pos, 1) = "." Then
				BaseName = BaseName.Substring(0, Pos)
			End If
		Next
		BaseName = BaseName.Trim.ToLower.Replace(" ", "_")
		
		'Check for existing programmer with name
		Dim UniqueID As Integer = 0
		OutName = BaseName
		Dim section As SettingSection
		StartToolSearchAgain:
		For Each section In PrefGroups
			If Section.name.ToLower.StartsWith("tool") And Section.Name.IndexOf("=") <> -1 Then
				ToolName = Section.Name.Substring(Section.Name.IndexOf("=") + 1).Trim()
				If ToolName = OutName Then
					OutName = BaseName + UniqueID.ToString.Trim
					UniqueID += 1
					Goto StartToolSearchAgain
				End If
			End If
		Next
		
		Return OutName
	End Function
	
	'Accepts the name (aka description, not ID) of a tool and finds the setting section for it
	Public Function GetToolSettings(ToolName As String) As SettingSection
		Dim section As SettingSection
		
		For Each section In PrefGroups
			If Section.name.ToLower.StartsWith("tool") And Section.Name.IndexOf("=") <> -1 Then
				if section.GetValue("desc").ToLower = ToolName.ToLower Then Return section
			End If
		Next
		
		Return Nothing
	End Function
	
	'Accepts the ID (not description) of a tool and finds the setting section for it
	Public Function GetToolSettingsFromID(ToolID As String) As SettingSection
		Dim section As SettingSection
		Dim sectionName As String
		
		For Each section In PrefGroups
			If Section.name.ToLower.StartsWith("tool") And Section.Name.IndexOf("=") <> -1 Then
				sectionName = section.Name.Substring(section.Name.IndexOf("=") + 1).Trim.ToLower
				If sectionName = ToolID Then Return Section
			End If
		Next
		
		Return Nothing
	End Function
	
End Class
