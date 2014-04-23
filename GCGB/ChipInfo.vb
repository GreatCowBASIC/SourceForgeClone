'
' Created by SharpDevelop.
' User: Hugh
' Date: 20/01/2010
' Time: 1:24 AM
'
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Windows.Forms

''Namespace Great_Cow_Graphical_BASIC

	Public Class ChipConfigOption
		Public Dim Name As String
		Public Dim Options() As String
	End Class
	
	Public Class ChipPinInfo
		Public Dim PinNo As Integer
		Public Dim PinName As String
		Public Dim PortName As String
		Public Dim PortBit As Integer
		
		Public Sub New(PinNo As Integer)
			Me.PinNo = PinNo
			PortBit = -1
		End Sub
		
	End Class
	
	Public Class ChipInfo
		'Stores info on the current chip
		
		'Name of chip
		Public Dim ChipName As String
		
		Public Dim ChipNotValid As Boolean
		
		'Misc chip data
		Public Dim ADCInputs As Integer
		Public Dim RAMBytes As Integer
		
		'Stores config settings
		Public Dim ConfigSettings as List(Of ChipConfigOption)
		Public Dim NoConfig As Integer
		Public Dim PinCount As Integer
		
		'Stores pinout
		Public Dim PinList As List(Of ChipPinInfo)
		Public Dim PackageType As String
		
		'Stores interrupts
		Public Dim InterruptList As List(Of String)
		
		Public Sub New(ChipName As String)
			Dim TempData, OpName, OpData, CurrentSection, ChipDataFile, PinName As String
			Dim NewSetting As ChipConfigOption
			
			'Flag to indicate whether DIP pinout has been loaded
			'Should load DIP if possible, if not then load whatever else there is
			Dim PinsDipLoaded As Boolean = False
			
			'Initialise
			Me.ChipName = ChipName
			ChipDataFile = MainForm.InstallDir + MainForm.FilenameChipDir + ChipName.ToLower + ".dat"
	    	NoConfig = 0
	    	ChipNotValid = False
	    	ConfigSettings = New List(Of ChipConfigOption)
	    	InterruptList = New List(Of String)
	    	PinList = New List(Of ChipPinInfo)
	    	CurrentSection = ""
	    	
	    	ADCInputs = 0
	    	
	    	'Read data file
	    	Try
		    	Dim ReadFile As New StreamReader(ChipDataFile)
		    	Do While ReadFile.Peek() <> -1
		    		TempData = ReadFile.ReadLine().Trim
		    		If TempData <> "" Then
		    			'Has current section changed?
		    			If TempData.IndexOf("[") <> -1 Then
		    				CurrentSection = TempData.ToUpper
		    				
		    				'Check if we are looking at a pinout section
		    				If CurrentSection.StartsWith("[PINS-") Then
		    					'Get package type
			    				If CurrentSection = "[PINS-DIP]" Then
			    					PinList = New List(Of ChipPinInfo)
			    					PinsDipLoaded = True
			    					PackageType = "DIP"
			    				Else
			    					PackageType = CurrentSection.Substring(CurrentSection.IndexOf("-") + 1)
			    					PackageType = PackageType.Substring(0, PackageType.IndexOf("]"))
			    				End If
		    				End If
		    				
		    			'Misc chip data section
		    			Else If CurrentSection = "[CHIPDATA]" Then
		    				If TempData.IndexOf("=") <> -1 Then
			    				OpName = TempData.Substring(0, TempData.IndexOf("=")).Trim.ToUpper
			    				OpData = TempData.Substring(TempData.IndexOf("=") + 1).Trim.ToUpper
			    				
			    				If OpName = "ADC" Then
			    					ADCInputs = OpData
			    				ElseIf OpName = "PINS" Then
			    					PinCount = OpData
			    				ElseIf OpName = "RAM" Then
			    					RamBytes = OpData
			    				End If
			    				
			    			End If
		    				
		    			'Config operations section
		    			Else If CurrentSection = "[CONFIGOPS]" Then
		    				'This line is found on chips with no config settings
			    			If TempData.ToUpper.StartsWith("NOCONFIG") Then
			    				NoConfig = 1
			    				If TempData.ToUpper.IndexOf("-PRG") <> -1 Then NoConfig = 2
			    			End If
			    			'Have found a setting
			    			If TempData.IndexOf("=") <> -1 Then
			    				OpName = TempData.Substring(0, TempData.IndexOf("=")).Trim.ToUpper
			    				OpData = TempData.Substring(TempData.IndexOf("=") + 1).Trim.ToUpper
			    				
			    				NewSetting = New ChipConfigOption
			    				NewSetting.Name = OpName
			    				NewSetting.Options = OpData.Split(",")
			    				ConfigSettings.Add(NewSetting)
			    			End If
		    			
		    			Else If CurrentSection = "[INTERRUPTS]" Then
		    				If TempData.IndexOf(":") <> -1 Then
		    					InterruptList.Add(TempData.Substring(0, TempData.IndexOf(":")).Trim)
		    				End If
		    				
		    			Else If CurrentSection = "[PINS-DIP]" Or (Not PinsDipLoaded And CurrentSection.StartsWith("[PINS-")) Then
		    				If TempData.IndexOf(",") <> -1 Then
		    					'Split line at commas
		    					Dim PinFunctions() As String
		    					PinFunctions = TempData.ToUpper.Split(",")
		    					
		    					'Create new pin
		    					Dim NewPin As New ChipPinInfo(PinFunctions(0))
		    					
		    					'Get name/function
		    					PinName = PinFunctions(1)
		    					If PinName.IndexOf("(") <> -1 Then
		    						PinName = PinName.Substring(0, PinName.IndexOf("("))
		    					End If
		    					With NewPin
		    						.PinName = PinName
			    					If LowLevel.IsIOPinName(PinName) Then
			    						If PinName.StartsWith("GP") Then
			    							.PortName = "GPIO"
			    							.PortBit = PinName.Substring(2, 1)
			    						Else
			    							.PortName = "PORT" + PinName.Substring(1, 1)
			    							.PortBit = PinName.Substring(2, 1)
			    						End If
			    					End If
		    					End With
		    					PinList.Add(NewPin)
		    					
		    				End If
		    				
		    			End If
		    			
		    		End If
				Loop
				
				ReadFile.Close
				
			Catch
				'If chip model isn't valid, show error once and set error flag
				If Not ChipNotValid Then
					MessageBox.Show("The chip model currently selected for this program is not valid. Please check that the model of chip is correct.", "Great Cow Graphical BASIC", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
					ChipNotValid = True
				End If
			End Try
			
		End Sub
		
		'Check if a given pin name is that of an IO pin on the current chip
		Public Function IsIOPin(InData As String) As Boolean
			
			Dim CurrPin As ChipPinInfo
			Dim PinName As String
			'Is input data a bit constant?
			If LowLevel.IsBitConst(InData) Then
				'If so, check if input data is a pin
				For Each CurrPin In PinList
					If CurrPin.PortBit <> -1 Then
						PinName = CurrPin.PortName + "." + CurrPin.PortBit.ToString
						If InData.ToLower = PinName.ToLower Then
							Return True
						End If
					End If
				Next
			End If
			
			Return False
		End Function
		
	End Class
	
''End Namespace
