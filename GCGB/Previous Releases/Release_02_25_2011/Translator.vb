'
' Created by SharpDevelop.
' User: Hugh
' Date: 21/12/2009
' Time: 5:07 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Collections
Imports System.IO

Namespace Great_Cow_Graphical_BASIC
	
	Public Class LanguageTranslator
		
		'Use Setting and SettingSection
		Private Dim TranslateSections As New ArrayList
		Private Dim Language As String
		Private Dim LanguageFile As String
		
		Private Dim Loaded As Boolean 'True if translation should be used, false if no data loaded
		
		Public Sub New
			
			Language = Preferences.GetPref("gcbasic", "language")
			
			Me.Loaded = False
			If Language <> "" Then
				LoadTranslation(MainForm.InstallDir + "/messages-gcgb-" + Language.ToLower + ".dat")
			End If
			
		End Sub
		
		Public Sub New(Language As String)
			
			Me.Language = Language
			
			'Attempt to load file
			Me.Loaded = False
			LoadTranslation(MainForm.InstallDir + "/messages-gcgb-" + Language.ToLower + ".dat")
			
		End Sub
		
		'Returns true if the translation can be used
		Public ReadOnly Property CanTranslate
			Get
				Return Loaded
			End Get
		End Property
		
		Private Sub LoadTranslation(LanguageFile As String)
			
	   		Dim TempData As String = ""
	   		Dim CurrentTag, MsgName, MsgValue As String
	   		Dim CurrentSection As SettingSection
	   		
	   		Me.LanguageFile = LanguageFile
	   		
	   		Try
	   			Dim ReadFile As New StreamReader(LanguageFile)
	   			
	   			Do While ReadFile.Peek() <> -1
		     		TempData = ReadFile.ReadLine().Replace(Microsoft.VisualBasic.Chr(9), " ").Trim
		     		
		     		If TempData.Length > 0 And TempData.IndexOf("'") <> 0 Then
		     			
		     			If TempData.StartsWith("[") And TempData.EndsWith("]") Then
		     				CurrentTag = TempData.Substring(1, TempData.Length - 2).Trim.ToLower
		     				'Check if section exists
		     				Dim FoundSection As Boolean = False
		     				For Each CurrentSection In TranslateSections
		     					If CurrentSection.Name = CurrentTag Then
		     						FoundSection = True
		     						Exit For
		     					End If
		     				Next
		     				'If not found, add it
		     				If Not FoundSection Then
		     					CurrentSection = New SettingSection(CurrentTag)
		     					TranslateSections.Add(CurrentSection)
		     				End If
		     				
		     			Else If TempData.IndexOf("=") <> -1 Then
		     				MsgName = TempData.Substring(0, TempData.IndexOf("=")).Trim.ToLower
		     				MsgValue = TempData.Substring(TempData.IndexOf("=") + 1).Trim
		     				
		     				'if quotes found, get text inside
		     				If MsgValue.IndexOf("""") <> -1 Then
		     					MsgValue = MsgValue.Substring(MsgValue.IndexOf("""") + 1)
		     					MsgValue = MsgValue.Substring(0, MsgValue.IndexOf(""""))
		     				End If
		     				
		     				CurrentSection.AddSetting(MsgName, MsgValue)
		     				
		     			End If
		     			
	     			End If	     		
	     		Loop
	     		
	     		ReadFile.Close()
	     		
	     		'Set used flag to indicate successful load
	     		Me.Loaded = True
	     		
	     	Catch
	   		End Try
		    
	    End Sub
		
		Public Function GetMessage(ByVal SectionName As String, ByVal PrefName As String, Optional DefValue As String = "") As String
			
			'If no settings loaded, return nothing
			If Not Loaded Then Return DefValue
			
			Dim Temp As Integer = 0
			Dim PrefLoc As Integer = 0
			
			SectionName = SectionName.Trim.ToLower
			PrefName = PrefName.Trim.ToLower
			
			'Find specified section
			Dim section As SettingSection
			For Each section In TranslateSections
				If section.Name.ToLower = SectionName Then
					
					'Find specified message
					Dim s As Setting
					For Each s In section.Settings
						If s.Name = PrefName Then
							Return s.Value
						End If
					Next
					
				End If
			Next
			
			'If no message found, return default
			Return DefValue
		End Function
		
		Public Sub TryTranslate(ByVal SectionName As String, ByVal PrefName As String, ByRef OutValue As String)
			
			'If no translate, don't do anything
			If Not Loaded Then Exit Sub
			
			'Try to get message
			Dim TempValue As String
			TempValue = GetMessage(SectionName, PrefName)
			
			'If message found, apply it
			If TempValue <> "" Then
				OutValue = TempValue
			End If
			
		End Sub
		
	End Class
End Namespace
