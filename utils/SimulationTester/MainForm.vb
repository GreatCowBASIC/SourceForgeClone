'
' Created by SharpDevelop.
' User: Hugh
' Date: 15/04/2015
' Time: 10:17 PM
'
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Imports System.Threading

Public Partial Class MainForm
	
	Private Shared Dim pInstallDir As String
	Private Shared Dim CurrSettings As Settings
	
	Private Dim Tests As List(Of GCBTest)
	Private Dim Targets As List(Of TestTarget)
	
	Private Dim HardwareSimAvailable As Boolean
	Private Dim pHardwareSim As HardwareSimulator
	
	Private Dim pRunningTests As Boolean = False
	
	Public ReadOnly Property RunningTests As Boolean
		Get
			Return pRunningTests
		End Get
	End Property
	
	Public Shared ReadOnly Property Settings As Settings
		Get
			Return CurrSettings
		End Get
	End Property
	
	Public Shared ReadOnly Property InstallDir As String
		Get
			Return pInstallDir
		End Get
	End Property
	
	Public ReadOnly Property HardwareSim As HardwareSimulator
		Get
			Return pHardwareSim
		End Get
	End Property
	
	Public Sub New()
		' The Me.InitializeComponent call is required for Windows Forms designer support.
		Me.InitializeComponent()
		
		'Find install dir
		pInstallDir = Application.ExecutablePath
 		If pInstallDir.IndexOf("\") <> -1 Then pInstallDir = pInstallDir.Substring(0, pInstallDir.LastIndexOf("\"))
		If pInstallDir.IndexOf("/") <> -1 Then pInstallDir = pInstallDir.Substring(0, pInstallDir.LastIndexOf("/"))
		If pInstallDir.ToLower.IndexOf("\bin") <> -1 Then pInstallDir = pInstallDir.Substring(0, pInstallDir.ToLower.IndexOf("\bin"))
		If pInstallDir.ToLower.IndexOf("/bin") <> -1 Then pInstallDir = pInstallDir.Substring(0, pInstallDir.ToLower.IndexOf("/bin"))
		
		'Get settings
		CurrSettings = New Settings(InstallDir + "\settings.ini")
		
		'Check for hardware simulator
		FindHardwareSim
		
		'Load tests and targets
		LoadTests
		LoadTargets
		
		AppendStatus("Ready!")
		
	End Sub
	
	'Load tests from directory
	Sub LoadTests
		Tests = New List(Of GCBTest)
		
		'Read list of files
		Dim sourceDir As New IO.DirectoryInfo(InstallDir + "/Tests")
		Dim fileList As IO.FileInfo() = sourceDir.GetFiles("*.dat")
		Dim fileName As IO.FileInfo
		For Each fileName In fileList
			'Load files
			Dim Test As New GCBTest(fileName.FullName)
			Tests.Add(Test)
			
			'Display
			SelectTests.Items.Add(Test)
			
		Next
		
	End Sub
	
	'Load targets from directory
	Sub LoadTargets
		Targets = New List(Of TestTarget)
		
		'Read list of files
		Dim sourceDir As New IO.DirectoryInfo(InstallDir + "/Targets")
		Dim fileList As IO.FileInfo() = sourceDir.GetFiles("*.dat")
		Dim fileName As IO.FileInfo
		For Each fileName In fileList
			'Load files
			Dim NewTarget As New TestTarget(fileName.FullName)
			
			'Display
			'If hardware simulator required, make sure it is present
			If Not NewTarget.RequiresHardware Or HardwareSimAvailable Then
				Targets.Add(NewTarget)
				SelectTargets.Items.Add(NewTarget)
			Else
				AppendStatus("Disabled target " + NewTarget.ToString + ": No hardware simulator")
			End If
			
		Next
	End Sub
	
	'Check if hardware simulator is available
	Sub FindHardwareSim
		HardwareSimAvailable = False
		
		Try
			pHardwareSim = New HardwareSimulator(CurrSettings)
			If HardwareSim.DevicesFound Then
				AppendStatus("Hardware Simulator found")
				HardwareSimAvailable = True
				
				HardwareSim.SetLCD("Ready ...")
				
			Else If Not HardwareSim.PICkit2Found Then
				AppendStatus("Hardware Simulator unavailable: No PICkit 2 found")
			Else
				AppendStatus("Hardware Simulator unavailable: No control microcontroller found")
			End If
			
		Catch Ex As Exception
			MessageBox.Show(Ex.ToString, "Error creating hardware simulator")
			AppendStatus("Hardware Simulator unavailable: Unknown error (Missing LibUsbDotNet.dll?)")
		End Try
		
	End Sub
	
	Sub AppendStatus(NewMessage As String)
		With StatusBox
			If .Text = "" Then
				.Text = NewMessage
			Else
				.Text = StatusBox.Text + Environment.NewLine + NewMessage
				.Select(StatusBox.Text.Length, 0)
				.ScrollToCaret
				
			End If
		End With
	End Sub
	
	Sub BAllTestsClick(sender As Object, e As EventArgs)
		Dim CurrItem As Integer
		For CurrItem = 0 To SelectTests.Items.Count - 1
			SelectTests.SetItemChecked(CurrItem, True)
		Next
	End Sub
	
	Sub BInvertTestsClick(sender As Object, e As EventArgs)
		Dim CurrItem As Integer
		For CurrItem = 0 To SelectTests.Items.Count - 1
			SelectTests.SetItemChecked(CurrItem, Not SelectTests.GetItemChecked(CurrItem))
		Next
	End Sub
	
	Sub BAllTargetsClick(sender As Object, e As EventArgs)
		Dim CurrItem As Integer
		For CurrItem = 0 To SelectTargets.Items.Count - 1
			SelectTargets.SetItemChecked(CurrItem, True)
		Next
	End Sub
	
	Sub BInvertTargetsClick(sender As Object, e As EventArgs)
		Dim CurrItem As Integer
		For CurrItem = 0 To SelectTargets.Items.Count - 1
			SelectTargets.SetItemChecked(CurrItem, Not SelectTargets.GetItemChecked(CurrItem))
		Next
	End Sub
	
	Sub StartTestsClick(sender As Object, e As EventArgs)
		CancelTests.Enabled = True
		StartTests.Enabled = False
		pRunningTests = True
		
		Dim RunnerThread As New Thread(AddressOf Me.Runner)
		RunnerThread.Start
		
	End Sub
	
	Sub CancelTestsClick(sender As Object, e As EventArgs)
		pRunningTests = False
		
	End Sub
	
	Sub Runner
		AppendStatus("Starting tests at " + DateTime.Now.ToString("HH:mm:ss") + " ...")
		Dim TestCount As Integer = 0
		Dim PassCount As Integer = 0
		
		'Entire block must be in Try ... Catch, or exceptions disappear
		Try
			
			'Prepare test list
			Dim TestList As New List(Of TestRun)
			Dim SearchTest As Object
			Dim SearchTarget As Object
			Dim NewTest As GCBTest
			Dim NewTarget As TestTarget
			For Each SearchTest In SelectTests.CheckedItems
				For Each SearchTarget In SelectTargets.CheckedItems
					If TypeOf SearchTest Is GCBTest And TypeOf SearchTarget Is TestTarget Then
						NewTest = SearchTest
						NewTarget = SearchTarget
						TestList.Add(New TestRun(Me, NewTest, NewTarget))
					End If
				Next
			Next
			
			'Run tests
			Dim CurrTest As TestRun
			For Each CurrTest In TestList
				If CurrTest.CanRun Then
					'Start
					AppendStatus("Running " + CurrTest.Test.ToString + " on " + CurrTest.Target.ToString)
					CurrTest.Run
					
					'Check
					TestCount += 1
					If CurrTest.Passed Then
						AppendStatus("Pass")
						PassCount += 1
					Else
						AppendStatus("Failed:")
						Dim ErrMessage As String
						For Each ErrMessage In CurrTest.Errors
							AppendStatus("    " + ErrMessage)
						Next
					End If
					
				Else
					'Cannot run test
					AppendStatus("Cannot run " + CurrTest.Test.ToString + " on " + CurrTest.Target.ToString)
				End If
				
				'Abort if tests stopped
				If Not pRunningTests Then
					Exit For
				End If
				
			Next
			
		Catch Ex As Exception
			MessageBox.Show(Ex.ToString, "Error running test")
		End Try
		
		'Tests finished
		If pRunningTests Then
			AppendStatus("Tests completed at " + DateTime.Now.ToString("HH:mm:ss") + ", " + PassCount.ToString + "/" + TestCount.ToString + " passed")
			pRunningTests = False
		Else
			AppendStatus("Tests stopped at " + DateTime.Now.ToString("HH:mm:ss") + ", " + PassCount.ToString + "/" + TestCount.ToString + " passed")
		End If
		
		Me.StartTests.Enabled = True
		Me.CancelTests.Enabled = False
	End Sub
	
	Sub MainFormFormClosed(sender As Object, e As FormClosedEventArgs)
		'On closing main form, need to release hardware simulator (if present)
		If Not HardwareSim Is Nothing Then
			HardwareSim.Close
		End If
	End Sub
	
End Class
