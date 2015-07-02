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
'Imports System.Windows.Forms

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
		
		Public Dim PinFunctions() As String
		
		Public Sub New(PinNo As Integer)
			Me.PinNo = PinNo
			PortBit = -1
		End Sub
		
	End Class
	
	Public Class MemPageInfo
		Public Dim StartLoc As Integer
		Public Dim EndLoc As Integer
	End Class
	
	Public Class SFRVar
		
		Public Sub New (Name As String, Location As Integer)
			Me.Name = Name
			Me.Location = Location
		End Sub
		
		Public Dim Name As String
		Public Dim Location As Integer
		
		'Some variables to allow faster simulation
		'Public Dim IsPortRead As Boolean 'Not suitable for AVR
		Public Dim IsPortRead As Boolean
		Public Dim IsPortWrite As Boolean
		Public Dim IsPortDir As Boolean '(aka TRIS)
		Public Dim TrisVar As SFRVar 'Stores TRIS for PORT or LAT
		Public Dim PortVar As SFRVar 'Stores PORT for LAT or TRIS
		Public Dim LatVar As SFRVar 'Stores LAT for PORT or TRIS
		
	End Class
	
	Public Class SFRVarBit
		Public Sub New (Name As String, Var As String, Bit As Integer)
			Me.Name = Name
			Me.Var = Var
			Me.Bit = Bit
		End Sub
		
		Public Dim Name As String
		Public Dim Var As String
		Public Dim Bit As Integer
	End Class
	
	Public Class ChipInterrupt
		Public Dim EventName As String
		Public Dim Flag As String
		Public Dim Enable As String
		Public Dim VectorName As String
		Public Dim VectorLoc As Integer
		
		Public Overrides Function ToString As String
			Return EventName
		End Function
		
		Public Sub New(EventName As String, Enable As String, Flag As String)
			Me.EventName = EventName
			Me.Enable = Enable
			Me.Flag = Flag
			VectorName = ""
			VectorLoc = 4
		End Sub
		
		Public Sub New(EventName As String, VectorName As String, VectorLoc As Integer, Enable As String, Flag As String)
			Me.EventName = EventName
			Me.Enable = Enable
			Me.Flag = Flag
			Me.VectorName = VectorName
			Me.VectorLoc = VectorLoc
		End Sub
		
	End Class
	
	Public Class ChipInfo
		'Stores info on the current chip
		
		'Name of chip
		Public Dim ChipName As String
		
		'Chip technical details
		Public Dim ChipFamily As Integer
		Public Dim ChipRAM As Integer
		Public Dim ChipEEPROM As Integer
		Public Dim ChipProgMem As Integer
		Public Dim MaxAddress As Integer
		Public Dim IntOscMHz As Single
		Public Dim IO As Integer
		
		Public Dim MaxMhz As Integer
		Public Dim PSP As String
		Public Dim ConfigWords As Integer
		Public Dim GPR As Integer
		Public Dim HardwareMult As Boolean
		
		Public Dim ChipNotValid As Boolean
		
		'Misc chip data
		Public Dim ADCInputs As Integer
		
		'Stores config settings
		Public Dim ConfigSettings as List(Of ChipConfigOption)
		Public Dim NoConfig As Integer
		Public Dim PinCount As Integer
		
		'Stores special function registers
		Public Dim SFRVars As Dictionary(Of String, SFRVar)
		Public Dim SFRVarBits As Dictionary(Of String, SFRVarBit)
		
		'List of RAM locations
		Public Dim FreeRAM As List(Of MemPageInfo)
		
		'List of non-banked locations
		Public Dim NoBankRAM As List(Of MemPageInfo)
		
		'Stores pinout
		Public Dim PinList As List(Of ChipPinInfo)
		Public Dim PackageType As String
		
		'Stores interrupts
		Public Dim InterruptList As Dictionary(Of String, ChipInterrupt)
		
		Private Sub DisplayError(ErrorMessage As String)
			'MessageBox.Show(ErrorMessage, "Great Cow Graphical BASIC", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
			Console.WriteLine(ErrorMessage)
		End Sub
		
		Public Sub New(SourceDir As String, ChipName As String)
			Dim TempData, OpName, OpData, CurrentSection, ChipDataFile, PinName As String
			Dim NewSetting As ChipConfigOption
			
			'Flag to indicate whether DIP pinout has been loaded
			'Should load DIP if possible, if not then load whatever else there is
			Dim PinsDipLoaded As Boolean = False
			
			If Not SourceDir.EndsWith("\") Then
				SourceDir += "\"
			End If
			
			'Initialise
			Me.ChipName = ChipName
			ChipDataFile = SourceDir + ChipName.ToLower + ".dat"
	    	NoConfig = 0
	    	ChipNotValid = False
	    	ConfigSettings = New List(Of ChipConfigOption)
	    	InterruptList = New Dictionary(Of String, ChipInterrupt)(StringComparer.CurrentCultureIgnoreCase)
	    	PinList = New List(Of ChipPinInfo)
	    	SFRVars = New Dictionary(Of String, SFRVar)(StringComparer.CurrentCultureIgnoreCase)
	    	SFRVarBits = New Dictionary(Of String, SFRVarBit)(StringComparer.CurrentCultureIgnoreCase)
	    	CurrentSection = ""
	    	
	    	FreeRAM = New List(Of MemPageInfo)
	    	NoBankRAM = New List(Of MemPageInfo)
	    	
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
			    				
			    				Select Case OpName
			    					Case "FAMILY": ChipFamily = OpData
			    					Case "PROG": ChipProgMem = OpData
			    					Case "RAM": ChipRAM = OpData
			    					Case "EEPROM": ChipEEPROM = OpData
			    					Case "ADC": ADCInputs = OpData
			    					Case "MAXADDRESS": MaxAddress = OpData
			    					Case "INTOSC"
			    						If OpData.IndexOf(",") <> -1 Then
			    							IntOscMHz = OpData.Substring(0, OpData.IndexOf(","))
			    						Else
			    							IntOscMHz = OpData
			    						End If
			    					Case "PINS": PinCount = OpData
			    					
			    					Case "I/O": IO = OpData
			    					Case "MAXMHZ": MaxMhz = OpData
			    					Case "PSP": PSP = OpData
			    					Case "CONFIGWORDS": ConfigWords = OpData
			    					Case "GPR": GPR = OpData
			    					Case "HARDWAREMULT": HardwareMult = Utils.PrefIsYes(OpData)
			    				End Select
			    				
			    			End If
		    			
		    			'SFR variables
		    			Else If CurrentSection = "[REGISTERS]" Then
		    				If TempData.IndexOf(",") <> -1 Then
		    					Dim LineSections() As String = TempData.Split(",")
		    					Try
		    						SFRVars.Add(LineSections(0), New SFRVar(LineSections(0), LineSections(1)))
		    					Catch Ex As Exception
		    						DisplayError("Error in chip data file, duplicate register " + LineSections(0))
		    					End Try
		    				End If
		    				
		    			'SFR variable bits
		    			Else If CurrentSection = "[BITS]" Then
		    				If TempData.IndexOf(",") <> -1 Then
		    					Dim LineSections() As String = TempData.Split(",")
		    					SFRVarBits.Add(LineSections(0), New SFRVarBit(LineSections(0), LineSections(1), LineSections(2)))
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
		    					'InterruptList.Add(TempData.Substring(0, TempData.IndexOf(":")).Trim)
		    					Dim IntElements() As String
		    					IntElements = TempData.Split(New Char(){",", ":"})
		    					If IntElements.Length = 3 Then
		    						InterruptList.Add(IntElements(0), New ChipInterrupt(IntElements(0), IntElements(1), IntElements(2)))
		    					ElseIf IntElements.Length = 5 Then
		    						InterruptList.Add(IntElements(0), New ChipInterrupt(IntElements(0), IntElements(1), IntElements(2), IntElements(3), IntElements(4)))
		    					Else
		    						Console.WriteLine("Unrecognised interrupt event")
		    					End If
		    				End If
		    			
		    			Else If CurrentSection = "[FREERAM]" Then
		    				If TempData.IndexOf(":") <> -1 Then
		    					Dim NewBank As New MemPageInfo
		    					NewBank.StartLoc = LowLevel.Other2Dec(TempData.Substring(0, TempData.IndexOf(":")), 16)
		    					NewBank.EndLoc = LowLevel.Other2Dec(TempData.Substring(TempData.IndexOf(":") + 1), 16)
		    					FreeRAM.Add(NewBank)
		    				End If
		    			
		    			Else If CurrentSection = "[NOBANKRAM]" Then
		    				If TempData.IndexOf(":") <> -1 Then
		    					Dim NewBank As New MemPageInfo
		    					NewBank.StartLoc = LowLevel.Other2Dec(TempData.Substring(0, TempData.IndexOf(":")), 16)
		    					NewBank.EndLoc = LowLevel.Other2Dec(TempData.Substring(TempData.IndexOf(":") + 1), 16)
		    					NoBankRAM.Add(NewBank)
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
		    					
		    					'Store list of all pin functions
		    					NewPin.PinFunctions = PinFunctions
		    					
		    					'Add to list of pins
		    					PinList.Add(NewPin)
		    					
		    				End If
		    				
		    			End If
		    			
		    		End If
				Loop
				
				ReadFile.Close
				
			Catch Ex As Exception
				'If chip model isn't valid, show error once and set error flag
				If Not ChipNotValid Then
					DisplayError("The chip model currently selected for this program is not valid. Please check that the model of chip is correct.")
					Console.WriteLine(ChipName + ":" + Ex.Message + Environment.NewLine + Ex.StackTrace)
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
		
		Public Function GetSFR(VarName As String) As SFRVar
			'Find SFRVar for VarName
			'Return Nothing if not found
			
			If SFRVars.ContainsKey(VarName) Then
				Return SFRVars(VarName)
			Else
				Return Nothing
			End If
			
		End Function
		
		Public Function GetSFRLoc(VarName As String) As Integer
			'Find location of VarName
			'Return -1 if not found
			
			If SFRVars.ContainsKey(VarName) Then
				Return SFRVars(VarName).Location
			Else
				Return -1
			End If
			
		End Function
		
		'Public Function GetSFRBit(BitNameIn As String, Optional VarNameIn As String = "") As SFRVarBit
		Public Function GetSFRBit(BitName As String) As SFRVarBit
			'Find location of BitName
			'Return Nothing if not found
			
			If SFRVarBits.ContainsKey(BitName) Then
				Return SFRVarBits(BitName)
			Else
				Return Nothing
			End If
			
		End Function
		
	End Class
	
''End Namespace
