'
' Created by SharpDevelop.
' User: Administrator
' Date: 7/08/2006
' Time: 3:47 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms
Imports System.Collections

Namespace Great_Cow_Graphical_BASIC

	Public Class FileIO
		Inherits System.Windows.Forms.Form
		
	  	Shared Public Sub LoadCommands
			'Also reads Constant Lists
			
			Dim TempData As String
			Dim ItemName As String
		 	Dim ItemValue As String
		 	Dim ConstantLocation As Integer
		 	Dim ReadType As Integer '0=Command, 1=Constant, 2=Category, 4 = config
		 	Dim ParamNo, ParamCount As Integer
		 	
		 	Dim CurrentDevice As HardwareDevice
		 	Dim CurrentDeviceSetting As HardwareDeviceSetting
		 	
		 	Dim CurrentFunction As GCBFunction
		 	Dim CurrentConstGroup As ConstGroup
		 	Dim CurrentCommand As GCBCommand
		 	
		 	MainForm.CommandList = New ArrayList
		 	MainForm.IconCategory = New ArrayList
		 	
		 	Dim ReadFile As New StreamReader(MainForm.InstallDir + MainForm.FilenameCommands)
		 	Do While ReadFile.Peek() <> -1
		 		
		 		TempData = ReadFile.ReadLine().Trim
		 		
		 		If TempData.IndexOf("'") <> -1 Then TempData = TempData.Substring(0, TempData.IndexOf("'"))
		 		If TempData = "" Then GoTo LineRead
		 		
		 		If TempData.ToUpper.Trim = "[CATEGORIES]" Then
		 			ReadType = 2
		 		
		 		ElseIf TempData.ToUpper.IndexOf("[COM_") <> -1 Then
		 	 		
		 	 		ReadType = 0
		 	 		ItemName = TempData.Substring(5)
		 	 		ItemName = ItemName.Substring(0,ItemName.IndexOf("]"))
		 	 		CurrentCommand = New GCBCommand
		 	 		MainForm.CommandList.Add(CurrentCommand)
		 	 		With CurrentCommand
		 	 			.Name = ItemName
		 	 			.HelpPage = ""
		 	 			.MatchSyntax = ""
						.MatchIcon = ""
		 	 			.Params = 0
		 	 			.OnlyIn = ""
		 	 		End With
		 			
		 	 		
		 		ElseIf TempData.ToUpper.IndexOf("[CGR_") <> -1 Then
		 	 		ReadType = 1
		 	 		'MainForm.ConstantCount += 1
		 	 		ItemName = TempData.Substring(5)
		 	 		ItemName = ItemName.Substring(0,ItemName.IndexOf("]"))
		 	 		'MainForm.ConstantList(MainForm.ConstantCount, 0) = ItemName
		 	 		'ConstantLocation = 0
		 	 		CurrentConstGroup = New ConstGroup(ItemName)
		 	 		MainForm.ConstantGroups.Add(CurrentConstGroup)
		 	 		
		 		ElseIf TempData.ToUpper.IndexOf("[CONF_") <> -1 Then
		 	 		ReadType = 4
		 	 		
		 	 		ItemName = TempData.Substring(5)
		 			ItemName = ItemName.Substring(0,ItemName.IndexOf("]"))
		 			
		 			'MainForm.HardwareCount += 1
					'MainForm.HardwareConf(MainForm.HardwareCount, 0, 0) = ItemName.Trim.ToUpper
					CurrentDevice = New HardwareDevice
					CurrentDevice.CodeName = ItemName
					MainForm.HardwareConf.Add(CurrentDevice)
					
				ElseIf TempData.ToUpper.IndexOf("[FN_") <> -1 Then
		 	 		ReadType = 5
		 	 		
		 	 		ItemName = TempData.Substring(4)
		 			ItemName = ItemName.Substring(0, ItemName.IndexOf("]"))
		 			
		 			'MainForm.SysFunctions += 1
					'MainForm.SysFunction(MainForm.SysFunctions) = New GCBFunction(ItemName.Trim)
					'MainForm.SysFunction(MainForm.SysFunctions).Params = 0
					CurrentFunction = New GCBFunction(ItemName.Trim)
					MainForm.GCBFunctions.Add(CurrentFunction)
		 	 		
		 	 	ElseIf ReadType = 0 Then
		 			If TempData.IndexOf("=") <> -1 Then
		 				
		 				ItemName = TempData.Substring(0,TempData.IndexOf("=")).ToLower.Trim
						ItemValue = TempData.Substring(TempData.IndexOf("=") + 1).Trim
						
						'Name is set above
						With CurrentCommand
							If ItemName = "syntax" then .Syntax = ItemValue
				 	 		If ItemName = "icon" then .Icon = ItemValue
							If ItemName = "match" Then
								.MatchSyntax = ItemValue.SubString(0, ItemValue.IndexOf(",")).Trim
								.MatchIcon = ItemValue.SubString(ItemValue.IndexOf(",") + 1).Trim
							End If
							If ItemName = "onlyin" Then .OnlyIn = ItemValue
							If ItemName = "category" Then .Category = ItemValue
							If ItemName = "help" then .HelpPage = ItemValue
							If ItemName = "constraint" Then
								.Constraints.Add(New ParamConstraint(ItemValue))
							End If
							If ItemName = "tooltip" Then .Tooltip = ItemValue
							If ItemName.Substring(0,1) = "p" Then
								ParamNo = ItemName.Substring(1)
								.Param(ParamNo) = New GCBCommandParam
								
								With .Param(ParamNo)
									.Name = ItemValue.Substring(0,ItemValue.IndexOf(",")).Trim
									ItemValue = ItemValue.Substring(ItemValue.IndexOf(",") + 1).Trim
									.ParamType = ItemValue.Substring(0,ItemValue.IndexOf(",")).Trim
									ItemValue = ItemValue.Substring(ItemValue.IndexOf(",") + 1).Trim
									.Description = ItemValue.Trim
								End With
								
								.Params += 1
								
								'MessageBox.Show(ParamNo.ToString, "Message", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1)
							End If
						End With
					End If
					
				ElseIf ReadType = 1 Then
					'Constant groups
					'ConstantLocation += 1
					'MainForm.ConstantList(MainForm.ConstantCount, ConstantLocation) = TempData
					If TempData.Trim <> "" Then
						CurrentConstGroup.Items.Add(TempData.Trim)
					End If
				
				ElseIf ReadType = 2 Then
					MainForm.IconCategory.Add(TempData)
					
				
				ElseIf ReadType = 4 Then
			 	 	
				    'Data is name of config set
				    If TempData.ToLower.StartsWith("name") And TempData.ToLower.IndexOf("=") <> -1 Then
				    	CurrentDevice.Name = TempData.Substring(TempData.IndexOf("=") + 1).Trim
				    	Goto LineRead
				    End If
				    
				    'Data is part of the set
				    'MainForm.HConfCount(MainForm.HardwareCount) += 1
				    CurrentDeviceSetting = New HardwareDeviceSetting
				    CurrentDevice.Settings.Add(CurrentDeviceSetting)
				    
				    'Condition
				    CurrentDeviceSetting.Condition = TempData.Substring(0, TempData.IndexOf(";")).Trim
				    TempData = TempData.Substring(TempData.IndexOf(";") + 1).Trim
				    'Name
				    CurrentDeviceSetting.Name = TempData.Substring(0, TempData.IndexOf(";")).Trim
				    TempData = TempData.Substring(TempData.IndexOf(";") + 1).Trim
				    'Constant
				    CurrentDeviceSetting.Constant = TempData.Substring(0, TempData.IndexOf(";")).Trim
				    TempData = TempData.Substring(TempData.IndexOf(";") + 1).Trim
				    'Type
				    CurrentDeviceSetting.ParamType = TempData
				    
				'Functions
				ElseIf ReadType = 5 Then
					If TempData.IndexOf("=") <> -1 Then
						
						ItemName = TempData.Substring(0,TempData.IndexOf("=")).ToLower.Trim
						ItemValue = TempData.Substring(TempData.IndexOf("=")+1).Trim
						
						'Name is set above
						With CurrentFunction
							If ItemName = "returns" Then .ReturnType = ItemValue
				 	 		If ItemName = "desc" then .Description = ItemValue
							If ItemName = "help" then .HelpPage = ItemValue
							
							If ItemName.Substring(0,1) = "p" Then
								'ParamNo = ItemName.Substring(1)
								Dim thisParam As New GCBCommandParam
								.Params.Add(thisParam)
								
								With thisParam
									.Name = ItemValue.Substring(0,ItemValue.IndexOf(",")).Trim
									ItemValue = ItemValue.Substring(ItemValue.IndexOf(",") + 1).Trim
									.ParamType = ItemValue.Substring(0,ItemValue.IndexOf(",")).Trim
									ItemValue = ItemValue.Substring(ItemValue.IndexOf(",") + 1).Trim
									.Description = ItemValue.Trim
								End With
								
								'MessageBox.Show(ParamNo.ToString, "Message", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1)
							End If
						End With
					End If
				    
				End If
		 	 
		 	 	LineRead:
		 		
		 	Loop
		 	ReadFile.Close()
		 	
		 	'Find hardware settings that control others
		 	Dim ParentSetting As String
		 	Dim ConditionSetting As HardwareDeviceSetting
		 	For Each CurrentDevice In MainForm.HardwareConf
		 		For Each CurrentDeviceSetting In CurrentDevice.Settings
		 			'Get item conditions
		 			TempData = CurrentDeviceSetting.Condition
		 			If TempData <> "all" And TempData.IndexOf(" ") <> -1 Then
		 				'Find controlling item
		 				ParentSetting = TempData.Substring(0, TempData.IndexOf(" ")).Trim
		 				For Each ConditionSetting In CurrentDevice.Settings
		 					If ParentSetting = ConditionSetting.Constant Then
		 						ConditionSetting.ControlsOther = True
		 						Exit For
		 					End If
		 				Next
		 			End If
		 		Next
		 	Next
		 	
		End Sub
		
		Public Shared Sub LoadConverters
			'Load list of external file type converters
			Dim ConverterDir As New IO.DirectoryInfo(MainForm.InstallDir + Mainform.FilenameConverters)
			Dim ConverterList As IO.FileInfo() = ConverterDir.GetFiles()
			Dim ConverterFileName As IO.FileInfo
			
			'Find .ini files in converters directory
			MainForm.FileConverters.Clear
			For Each ConverterFileName In ConverterList
				If ConverterFileName.FullName.ToLower.EndsWith(".ini") Then
					MainForm.FileConverters.Add(New FileConverter(MainForm.InstallDir + Mainform.FilenameConverters, ConverterFileName.FullName))
				End If
			Next
			
		End Sub
		
		Public Shared Sub LoadIcons
			Dim TempData As String
			Dim IconName As String
			Dim DataName, DataValue As String
			Dim CurrIcon As GCBIcon
			
			MainForm.IconList = New ArrayList
			
			Dim ReadFile As New StreamReader(MainForm.InstallDir + MainForm.FilenameIcons)
			Do While ReadFile.Peek() <> -1
				TempData = ReadFile.ReadLine.Trim
				
				If TempData.IndexOf("'") <> -1 Then TempData = TempData.Substring(0, TempData.IndexOf("'"))
				
				If TempData.StartsWith("[") Then
					CurrIcon = New GCBIcon(TempData.Substring(1, TempData.Length - 2))
					MainForm.IconList.Add(CurrIcon)
				
				Else If TempData.IndexOf("=") <> -1 Then
					DataName = TempData.Substring(0, TempData.IndexOf("=")).Trim.ToLower
					DataValue = TempData.Substring(TempData.IndexOf("=") + 1).Trim
					
					With CurrIcon
						If DataName = "texture" Then
							.Texture = Lowlevel.AddFullPath(DataValue, MainForm.InstallDir + MainForm.FilenameIconsDir)
						Else If DataName = "lines" Then
						Else If DataName = "joiner" Then
							.HasJoiner = Preferences.PrefIsYes(DataValue)
						Else If DataName.StartsWith("line") Then
							.Lines += 1
							.Line(.Lines) = DataValue
						Else If DataName = "colour" Then
							.TextColour = DataValue
						Else If DataName = "size" Then
							.XSize = DataValue.Substring(0, DataValue.IndexOf(","))
							.YSize = DataValue.Substring(DataValue.IndexOf(",") + 1)
						End If
					End With
				End If
				
			Loop
			ReadFile.Close
		End Sub
		
		Public Sub New()
		End Sub
		
	End Class
End Namespace
