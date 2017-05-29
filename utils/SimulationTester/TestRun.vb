'
' Created by SharpDevelop.
' User: Hugh
' Date: 16/04/2015
' Time: 1:19 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Imports System.IO
Imports System.Threading

Public Class TestRun
	
	Private Dim parent As MainForm
	Private Dim pTest As GCBTest
	Private Dim pTarget As TestTarget
	
	Private Dim BaseFileName As String
	
	Private Dim CompilerOutput As String
	
	Private Dim CapturedOutput As String
	Private Dim pErrors As List(Of String)
	Private Dim pSkipped As Boolean
	
	Public ReadOnly Property Test As GCBTest
		Get
			Return pTest
		End Get
	End Property
	
	Public ReadOnly Property Target As TestTarget
		Get
			Return pTarget
		End Get
	End Property
	
	Public ReadOnly Property Errors As IList(Of String)
		Get
			Return pErrors.AsReadOnly
		End Get
	End Property
	
	Public Sub New(Parent As MainForm, Test As GCBTest, Target As TestTarget)
		Me.parent = Parent
		pTest = Test
		pTarget = Target
		
		CapturedOutput = ""
		
		pSkipped = False
		pErrors = New List(Of String)
		
	End Sub
	
	Public Function CanRun As Boolean
		'Can test run on this target?
		If pSkipped Then Return False
		
		If pTest.RequiredFlash > pTarget.Flash Then
			pSkipped = True
			pErrors.Add("Target does not have enough flash")
		End If
		If pTest.RequiredRam > pTarget.Ram Then
			pSkipped = True
			pErrors.Add("Target does not have enough RAM")
		End If
		
		Return Not pSkipped
	End Function
	
	Public Sub Run
		'Prepare test
		Prepare
		
		'Compile
		If Not Compile Then
			pErrors.Add(CompilerOutput)
			Exit Sub
		End If
		
		'Run and capture output
		If Target.TargetType = TestTarget.TargetTypes.SOFTWARE Then
			RunSoftware
		Else If Target.TargetType = TestTarget.TargetTypes.HARDWARE Then
			RunHardware
		End If
		
		'Check output
		CheckOutput
		
	End Sub
	
	Private Sub Prepare
		'Combine header from target file with code from test file
		Dim outLine As String
		BaseFileName = MainForm.InstallDir + "\generated\" + Test.ToString + "-" + Target.ToString
		Dim GCBFile As New StreamWriter(BaseFileName + ".gcb")
		
		'Write header
		For Each outLine In Target.InitCode
			GCBFile.WriteLine(outLine)
		Next
		
		'Write blank line
		GCBFile.WriteLine
		
		'Write test
		For Each outLine In Test.SourceCode
			GCBFile.WriteLine(outLine)
		Next
		
		GCBFile.Close
		
	End Sub
	
	Private Function Compile As Boolean
		Dim DoCompile As New System.Diagnostics.Process
		With DoCompile.StartInfo
			.FileName = MainForm.Settings.GCBASICPath + "\gcbasic.exe"
			.WorkingDirectory = MainForm.InstallDir + "\generated\"
			.Arguments = """" + BaseFileName + ".gcb"" /NP /K:A /A:GCASM /P:none"
			.UseShellExecute = False
			.RedirectStandardOutput = True
			.CreateNoWindow = True
		End With
		DoCompile.Start()
		DoCompile.WaitForExit(30000)
		
		CompilerOutput = DoCompile.StandardOutput.ReadToEnd
		
		'Check for errors.txt, and exit sub if found
		Dim FileCheck As New IO.FileInfo(MainForm.InstallDir + "\generated\errors.txt")
   		If FileCheck.Exists Then
   			'Console.WriteLine(GcbOut)
   			'Check to see if errors found, or just warnings
   			If CompilerOutput.IndexOf("Error:") <> -1 Then
   				Return False
   			End If
   		End If
   		
   		Return True
	End Function
	
	'Run this test on the hardware simulator
	Private Sub RunHardware
		Dim sim As HardwareSimulator = parent.HardwareSim
		
		'Select chip
		sim.SelectTarget(Target)
		
		'Download program
		If Not sim.DownloadProgram(BaseFileName + ".hex") Then
			pErrors.Add("Program download failed")
			Exit Sub
		End If
		
		'Start serial monitor
		sim.StartSerialMonitor
		'Clear buffer on simulator
		Thread.Sleep(150)
		sim.ClearSerialMonitor
		
		'Run
		sim.SetPower(True)
		Thread.Sleep(Test.RunTime + Target.PowerUpDelay)
		sim.SetPower(False)
		
		'Stop serial monitor
		sim.StopSerialMonitor
		
		'Capture results
		CapturedOutput = sim.TextLog
		Dim LogWriter As New StreamWriter(BaseFileName + ".log")
		LogWriter.WriteLine(CapturedOutput)
		LogWriter.Close
		
	End Sub
	
	'Run this test in the software simulator
	Private Sub RunSoftware
		'Prepare scenario
		Dim Scenario As New StreamWriter(BaseFileName + ".gcs")
		With Scenario
			.WriteLine("[Scenario]")
			.WriteLine("world = Worlds\A5 sheet.gcw")
			.WriteLine
			.WriteLine("[Robot]")
			.WriteLine("robot = " + Target.SimRobot)
			.WriteLine("program = " + BaseFileName + ".hex")
			.WriteLine
			.WriteLine("[RunOptions]")
			.WriteLine("MaxRunTime = " + Test.RunTime.ToString)
			.WriteLine("AutoRun = y")
			.WriteLine("AutoExit = y")
			.WriteLine("LogFile = " + BaseFileName + ".log")
			.Close
		End With
		
		'Launch software simulator
		Dim RunSim As New System.Diagnostics.Process
		With RunSim.StartInfo
			.FileName = MainForm.Settings.GCSIMExe
			.Arguments = "/hide """ + BaseFileName + ".gcs"""
		End With
		RunSim.Start()
		RunSim.WaitForExit(30000)
		
		Dim LogReader As New StreamReader(BaseFileName + ".log")
		CapturedOutput = LogReader.ReadToEnd
		LogReader.Close
		
	End Sub
	
	Public Function Passed As Boolean
		Return (pErrors.Count = 0 Or pSkipped)
	End Function
	
	Public Function Skipped As Boolean
		Return pSkipped
	End Function
	
	'Return true if test passed, false if it didn't
	Public Sub CheckOutput
		Dim CheckItem As String
		
		'Things that must be found
		For Each CheckItem In Test.MustFind
			If Not CapturedOutput.ToLower.Contains(CheckItem.ToLower) Then
				pErrors.Add("Did not find " + CheckItem)
			End If
		Next
		
		'Things that must not be found
		For Each CheckItem In Test.MustNotFind
			If CapturedOutput.ToLower.Contains(CheckItem.ToLower) Then
				pErrors.Add("Found unwanted " + CheckItem)
			End If
		Next
		
	End Sub
	
End Class
