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
'Imports System.IO
'Imports System.Windows.Forms

''Namespace Great_Cow_Graphical_BASIC
	
	Public Class ConstGroup
		'Class to store a constant group
		Private Dim GroupName As String
		Public Dim Items As List(Of String)
		
		Public ReadOnly Property Name As String
			Get
				Return GroupName
			End Get
		End Property
		
		Public Sub New(GroupName As String)
			Me.GroupName = GroupName
			Items = New List(Of String)
		End Sub
		
	End Class
	
	Public Class DocSection
		'Stores a section of code documentation
		Public Dim Description As String
		Public Dim HideItem As Boolean
		Public Dim IconFile As String
		Public Dim Params As SettingSection
		Public Dim HardwareSetting As String
		Public Dim IncludedFrom As String
		
		'Flags to indicate content
		Public Dim IsHardwareSetting As Boolean = False
		
		Public Sub New
			'Create SettingSection to store parameter list
			'Store parameter name as setting name and description as setting value
			Params = New SettingSection("Parameters")
			
			'Initialise other data items
			Description = ""
			IncludedFrom = ""
			IconFile = ""
			HideItem = False
		End Sub
		
		Public Sub ParseLine(ByVal LineIn As String)
			
			'Trim leading '''
			LineIn = LineIn.Trim
			If LineIn.StartsWith("'''") Then LineIn = LineIn.Substring(3).Trim
			
			'Check for tag
			If LineIn.StartsWith("@") Then
				If LineIn.ToLower.StartsWith("@hide") Then
					HideItem = True
					
				Else If LineIn.ToLower.StartsWith("@icon ") Then
					IconFile = LineIn.Substring(6).Trim
					
				Else If LineIn.ToLower.StartsWith("@hardware ") Then
					HardwareSetting = LineIn.Substring(10).Trim
					IsHardwareSetting = True
					
				Else If LineIn.ToLower.StartsWith("@param") Then
					Dim ParamName, ParamDesc As String
					'Get name and desc
					'format:
					'@param name desc
					ParamName = LineIn.Substring(LineIn.IndexOf(" ")).Trim
					ParamDesc = ParamName.Substring(ParamName.IndexOf(" ")).Trim
					ParamName = ParamName.Substring(0, ParamName.IndexOf(" ")).Trim
					Params.AddSetting(ParamName, ParamDesc)
					
				End If
			Else
				'If no tag, add new line to description
				Description = (Description + " " + LineIn).Trim
			End If
			
		End Sub
		
	End Class
	
	Public Class GCBFunction
		
		Public Dim Name As String
		Public Dim ReturnType As String
		Public Dim Description As String
		Public Dim HelpPage As String
		
		'Public Dim Param(10) As GCBCommandParam
		Public Dim Params As List(Of GCBCommandParam)
		
		Public Sub New (NameIn As String)
			Me.Name = NameIn
			Params = New List(Of GCBCommandParam)
		End Sub
		
	End Class
	
	Public Class HardwareDevice
		Public Dim Name As String
		Public Dim CodeName As String
		
		Public Dim Settings As New List(Of HardwareDeviceSetting)
		
		Public Sub FindControllers
			'Find hardware settings that control others
		 	Dim ParentSetting As String
		 	Dim CurrentDeviceSetting As HardwareDeviceSetting
		 	Dim ConditionSetting As HardwareDeviceSetting
		 	Dim TempData As String
		 	For Each CurrentDeviceSetting In Settings
	 			'Get item conditions
	 			TempData = CurrentDeviceSetting.Condition
	 			If TempData <> "all" And TempData.IndexOf(" ") <> -1 Then
	 				'Find controlling item
	 				ParentSetting = TempData.Substring(0, TempData.IndexOf(" ")).Trim
	 				For Each ConditionSetting In Settings
	 					If ParentSetting = ConditionSetting.Constant Then
	 						ConditionSetting.ControlsOther = True
	 						Exit For
	 					End If
	 				Next
	 			End If
	 		Next
		End Sub
		
	End Class
	
	Public Class HardwareDeviceSetting
		
		'Public Dim Shared HardwareConf(MaxHardware, MaxConfPerItem, 4) As String
		'Public Dim Shared HControlsOther(MaxHardware, MaxConfPerItem) As Boolean
		'List, List item, data (1 - description, 2 - constant, 3 - type, 4 - conditions to display)
		'List, 0, 0 stores short name, 0, 1 stores full name
		
		Public Dim Name As String
		Public Dim Description As String
		Public Dim Constant As String
		Public Dim ParamType As String
		Public Dim Condition As String
		Public Dim ControlsOther As Boolean
		
	End Class
	
	Public Class InterruptHandler
		Public Dim IntEvent As String
		Public Dim HandlerSub As String
	End Class
	
	Public Class LibraryType
		
		Private Dim pFileName As String
		Private Dim pDisplayName As String
		
		Public Dim Subroutines As List(Of GCBSubroutine)
		Public Dim Constants As List(Of Setting)
		'Public Dim ChipSettings As ArrayList
		Public Dim DataTables As List(Of String)
		Public Dim Variables As List(Of VariableListItem)
		
		Public Dim Device As HardwareDevice
		
		Public Dim ChipName As String
		Public Dim ChipSpeed As Double
		
		Public Sub New(FileName As String)
			Me.FileName = FileName
			
			Subroutines = New List(Of GCBSubroutine)
			Constants = New List(Of Setting)
			'ChipSettings = New ArrayList
			DataTables = New List(Of String)
			Variables = New List(Of VariableListItem)
		End Sub
		
		Public Sub ClearData
			ChipName = ""
			ChipSpeed = 0
			
			Subroutines.Clear
			Constants.Clear
			'ChipSettings.Clear
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
	
	Public Class ParamConstraint
		
		Private Dim Controlled, Controller As String
		Private Dim Actions As List(Of String)
		Private Dim Values As List(Of String)
		
		Public Sub New(InLine As String)
			
			Dim InLineTokens() As String = InLine.ToLower.Split(" ")
			Dim CurrRead, Temp As Integer
			
			Actions = New List(Of String)
			Values = New List(Of String)
			
			Controlled = InLineTokens(0)
			CurrRead = 1
			Do While InLineTokens(CurrRead) <> "when"
				Actions.Add(InLineTokens(CurrRead))
				CurrRead += 1
			Loop
			Controller = InLineTokens(CurrRead + 1)
			For Temp = CurrRead + 2 To InLineTokens.Length - 1
				Values.Add(InLineTokens(Temp))
			Next
			
		End Sub
		
		Public Function GetControlled As String
			Return Controlled
		End Function
		
		Public Function HasAction (ByVal Action As String) As Boolean
			Dim TestAction As String
			Action = Action.ToLower.Trim
			For Each TestAction In Actions
				If TestAction = Action Then
					Return True
				End If
			Next
			Return False
		End Function
		
		Public Function GetController As String
			Return Controller
		End Function
		
		Public Function ConditionTrue(ParamName As String, ParamValue As String) As Boolean
			'Check that we have the right parameter
			If ParamName.ToLower <> Controller Then
				Return False
			End If
			
			'Check values
			Dim TestValue As String
			ParamValue = ParamValue.ToLower.Trim
			For Each TestValue In Values
				If TestValue = ParamValue Then
					Return True
				End If
			Next
			
			'Haven't found anything that matches
			Return False
		End Function
		
	End Class
	
	Public Class UndoHistoryItem
		
		'Type
		Public Const Add As Integer = 0
		Public Const Delete As Integer = 1
		Public Const Change As Integer = 2
		Public Dim Action As Integer
		
		Public Dim OldCmd As String
		Public Dim NewCmd As String
		Public Dim SubNo As GCBSubroutine
		Public Dim LineNo As Integer
		Public Dim ParamNo As Integer = -1
		
	End Class
	
''End Namespace
