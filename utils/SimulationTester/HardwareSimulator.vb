'
' Created by SharpDevelop.
' User: Hugh
' Date: 16/04/2015
' Time: 12:35 AM
'
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports LibUsbDotNet
Imports LibUsbDotNet.Info
Imports LibUsbDotNet.Main
Imports System.IO
Imports System.Threading

Public Class HardwareSimulator
	
	Private Dim Device As UsbDevice
	Private Dim Target As TestTarget
	Private Dim pPICkit2Found As Boolean
	
	Private Dim SerialMonitor As Thread
	Private Dim pSerialMonitorRunning As Boolean
	
	Private Dim pSerialLog As List(Of Byte)
	
	Public Class UARTReply
		Public Dim Size As Integer
		Public Dim Bytes(200) As Integer
	End Class
	
	Public ReadOnly Property PICkit2Found As Boolean
		Get
			Return pPICkit2Found
		End Get
	End Property
	
	Public ReadOnly Property DevicesFound As Boolean
		Get
			Return (Not Device Is Nothing) And pPICkit2Found
		End Get
	End Property
	
	Public Sub New(CurrSettings As Settings)
		'Find PICkit 2
		pPICkit2Found = False
		'Launch pk2cmd to detect
		Dim RunPK2 As New System.Diagnostics.Process
		With RunPK2.StartInfo
			.FileName = CurrSettings.PICkit2Path + "\pk2cmd.exe"
			.WorkingDirectory = CurrSettings.PICkit2Path
			.Arguments = "-?V"
			.RedirectStandardOutput = True
			.RedirectStandardError = True
			.UseShellExecute = False
			.CreateNoWindow = True
		End With
		RunPK2.Start()
		RunPK2.WaitForExit(30000)
		
		If RunPK2.HasExited Then
			Dim PK2Out As String = RunPK2.StandardOutput.ReadToEnd
			'If no PICkit 2 found, can't use hardware sim
			If PK2Out.Contains("not found") Then
				Return
			Else
				pPICkit2Found = True
			End If
		End If
		
		'Find Control microcontroller
		Dim Finder As New UsbDeviceFinder(&HF055, &H0101)
		Device = UsbDevice.OpenUsbDevice(Finder)
		
		'Create serial log
		pSerialLog = New List(Of Byte)
		
		'Default to no target
		Target = Nothing
	End Sub
	
	'Select a target microcontroller to program/run
	Public Sub SelectTarget(Target As TestTarget)
		Me.Target = Target
		SendCommand(17, Target.Slot)
	End Sub
	
	'Download a hex file to the target
	Public Function DownloadProgram(HexFile As String) As Boolean
		If Target Is Nothing Then
			Return False
		End If
		
		Dim FileDir, FileName As String
		FileDir = Path.GetDirectoryName(HexFile)
		FileName = Path.GetFileName(HexFile)
		
		'Launch pk2cmd to download program
		If Target.ChipType = TestTarget.ChipTypes.PIC Then
			Dim Programmer As New System.Diagnostics.Process
			With Programmer.StartInfo
				.FileName = MainForm.Settings.PICkit2Path + "\pk2cmd.exe"
				.WorkingDirectory = FileDir
				.Arguments = "-PPIC" + Target.ChipName + " -F""" + HexFile + """ -L2 -B""" + MainForm.Settings.PICkit2Path + """ -M"
				.RedirectStandardOutput = True
				.RedirectStandardError = True
				.UseShellExecute = False
				.CreateNoWindow = True
			End With
			Programmer.Start()
			Programmer.WaitForExit(30000)
			
			If Programmer.HasExited Then
				If Programmer.ExitCode = 0 Then
					Return True
				Else
					MessageBox.Show("Output:" + Environment.NewLine + Programmer.StandardOutput.ReadToEnd + Environment.NewLine + "Error:" + Environment.NewLine + Programmer.StandardError.ReadToEnd)
					Return False
				End If
			End If
			
		Else If Target.ChipType = TestTarget.ChipTypes.AVR Then
			Dim Programmer As New System.Diagnostics.Process
			With Programmer.StartInfo
				.FileName = MainForm.Settings.AVRDudePath + "\avrdude.exe"
				.WorkingDirectory = FileDir
				.Arguments = "-c pickit2 -p AT" + Target.ChipName + " -U ""flash:w:" + FileName + """"
				.RedirectStandardOutput = True
				.RedirectStandardError = True
				.UseShellExecute = False
				.CreateNoWindow = True
			End With
			Programmer.Start()
			Programmer.WaitForExit(30000)
			
			If Programmer.HasExited Then
				If Programmer.ExitCode = 0 Then
					Return True
				Else
					MessageBox.Show("Output:" + Environment.NewLine + Programmer.StandardOutput.ReadToEnd + Environment.NewLine + "Error:" + Environment.NewLine + Programmer.StandardError.ReadToEnd)
					Return False
				End If
			End If
		End If
		
		Return False
	End Function
	
	Public Sub SetPower(Status As Boolean)
		
		'Launch pk2cmd to turn power on/off
		Dim Programmer As New System.Diagnostics.Process
		With Programmer.StartInfo
			.FileName = MainForm.Settings.PICkit2Path + "\pk2cmd.exe"
			.WorkingDirectory = MainForm.Settings.PICkit2Path
			If Status Then
				.Arguments = "-PPIC16F819 -T -R -B""" + MainForm.Settings.PICkit2Path + """"
			Else
				.Arguments = "-PPIC16F819 -B""" + MainForm.Settings.PICkit2Path + """"
			End If
			.RedirectStandardOutput = True
			.RedirectStandardError = True
			.UseShellExecute = False
			.CreateNoWindow = True
		End With
		Programmer.Start()
		Programmer.WaitForExit(30000)
		
		If Programmer.HasExited And Programmer.ExitCode <> 0 Then
			MessageBox.Show("Output:" + Environment.NewLine + Programmer.StandardOutput.ReadToEnd + Environment.NewLine + "Error:" + Environment.NewLine + Programmer.StandardError.ReadToEnd)
		End If
	End Sub
	
	'Set LCD text
	Public Sub SetLCD(LCDText As String)
		Dim Lines() As String = LCDText.Split(Environment.NewLine)
		Dim ThisLine As String
		Dim CurrLine, CurrChar As Integer
		For CurrLine = 0 To 1
			If Lines.Length > CurrLine Then
				ThisLine = Lines(CurrLine).Trim
			Else
				ThisLine = ""
			End If
			
			SendCommand(3, CurrLine * 256)
			For CurrChar = 0 To 15
				If ThisLine.Length > CurrChar Then
					SendCommand(2, Convert.ToInt32(ThisLine.Chars(CurrChar)))
				Else
					SendCommand(2, 32)
				End If
			Next
		Next
	End Sub
	
	Public Sub StartSerialMonitor
		'Stop?
		If pSerialMonitorRunning Then
			StopSerialMonitor
			Thread.Sleep(100)
		End If
		
		pSerialMonitorRunning = True
		SerialMonitor = New Thread(AddressOf SerialMonitorRun)
		SerialMonitor.Start
		
	End Sub
	
	Public Sub StopSerialMonitor
		pSerialMonitorRunning = False
		Do While SerialMonitor.IsAlive
			Thread.Sleep(5)
		Loop
	End Sub
	
	Public Sub SerialMonitorRun
		'Capture data from UART
		If Device Is Nothing Then
			Exit Sub
		End If
		
		'Temp buffer
		Dim InBuffer(100) As Byte
		Dim inCount As Integer
		Dim copyByte As Integer
		
		Do While pSerialMonitorRunning
			SyncLock Device
				Dim SetupPacket As New UsbSetupPacket(194, 52, 0, 0, 64)
				Device.ControlTransfer(SetupPacket, InBuffer, 64, inCount)
			End SyncLock
			
			For copyByte = 0 To inCount - 1
				pSerialLog.Add(inBuffer(copyByte))
			Next
			
			Thread.Sleep(50)
		Loop
	End Sub
	
	Public Sub ClearSerialMonitor
		pSerialLog.Clear
	End Sub
	
	Public ReadOnly Property SerialLog As IList(Of Byte)
		Get
			Return pSerialLog.AsReadOnly
		End Get
	End Property
	
	Public ReadOnly Property TextLog As String
		Get
			Dim OutData As String = "Serial log:" + Environment.NewLine
			Dim CurrByte As Byte
			
			For Each CurrByte In pSerialLog
				If CurrByte <> 0 Then
					OutData += Convert.ToChar(CurrByte)
				End If
			Next
			
			Return OutData
		End Get
	End Property
	
	'Send a serial byte
	Public Sub SendUART(ByteOut As Integer)
		SendCommand(33, ByteOut)
	End Sub
	
	Public Function ReadUART As Integer
		Return ReadByteReply(49)
	End Function
	
	Public Function ReadUARTBuffer As UARTReply
		Dim reply As New UARTReply
		reply.Size = 0
		
		'Can't use this if no device or serial monitor running
		If Device Is Nothing Or pSerialMonitorRunning Then
			Return reply
		End If
		
		Dim InBuffer(100) As Byte
		Dim inCount As Integer
		SyncLock Device
			Dim SetupPacket As New UsbSetupPacket(194, 52, 0, 0, 64)
			Device.ControlTransfer(SetupPacket, InBuffer, 64, inCount)
		End SyncLock
		
		reply.Size = inCount
		Dim CopyPos As Integer
		For CopyPos = 0 To inCount - 1
			reply.Bytes(CopyPos) = InBuffer(CopyPos)
		Next
		
		Return reply
	End Function
	
	Private Sub SendCommand(Request As Integer, Param As Integer)
		If Device Is Nothing Then
			Exit Sub
		End If
		
		SyncLock Device
			Dim SetupPacket As New UsbSetupPacket(66, Request, Param, 0, 0)
			Dim InBuffer(1000) As Integer
			Dim inCount As Integer
			Device.ControlTransfer(SetupPacket, InBuffer, 1000, inCount)
		End SyncLock
	End Sub
	
	Private Function ReadByteReply(Request As Integer) As Integer
		If Device Is Nothing Then
			Return -1
		End If
		
		Dim outVal As Integer
		SyncLock Device
			Dim SetupPacket As New UsbSetupPacket(194, Request, 0, 0, 1)
			Dim InBuffer(10) As Integer
			Dim inCount As Integer
			Device.ControlTransfer(SetupPacket, InBuffer, 10, inCount)
			MessageBox.Show(inCount.ToString)
			outVal = InBuffer(0)
		End SyncLock
		
		Return outVal
	End Function
	
	Public Sub Close
		If Not Device Is Nothing Then
			Device.Close
		End If
	End Sub
	
End Class
