' Simple program to get programs onto the Arduino Robot
' Responsible for resetting (if possible), then waiting for a new COM port to
' appear, and finally calling avrdude to download a hex file into the chip.
'
' User: Hugh
' Date: 23/09/2013
' Time: 10:54 PM

Imports System.IO.Ports

Module Program
	Sub Main()
		
		Dim ExitStatus As Integer = 0
		
		'Get command line parameters
		Dim HexFile As String = ""
		Dim OldPort As String = ""
		Dim CurrArg As String
		Dim CurrArgNo As Integer = 0
		For Each CurrArg In Environment.GetCommandLineArgs
			If CurrArg.ToLower.StartsWith("/p:") Then
				OldPort = CurrArg.Substring(3)
			Else If CurrArgNo > 0 Then
				HexFile = CurrArg
			End If
			CurrArgNo += 1
		Next
		
		If HexFile = "" Then
			Console.WriteLine("ArduinoRobotLoader - Loads hex files into the Arduino Robot")
			Console.WriteLine
			Console.WriteLine(" ArduinoRobotLoader [/P:oldport] filename")
			Console.WriteLine
			Console.WriteLine("Where:")
			Console.WriteLine("  /P:oldport   Current port (used to attempt reset)")
			Console.WriteLine("  filename     Name of hex file to load")
			Console.WriteLine
			End
		End If
		
		Console.WriteLine("ArduinoRobotLoader is preparing to load robot ...")
		Console.WriteLine
		
		'Reset (if possible)
		If OldPort <> "" Then
			Console.WriteLine("Attempting to reset robot on port " + OldPort)
			Dim resetPort As SerialPort
			resetPort = My.Computer.Ports.OpenSerialPort(OldPort, 1200)
			resetPort.Close
		End If
		Console.WriteLine("Waiting for robot ...")
		Console.WriteLine("(Please double-press reset button within 30 seconds if robot does not reset)")
		
		'Wait for new serial port to appear
		Dim EndTime As DateTime = DateTime.Now.AddSeconds(30)
		Dim OriginalPorts, NewPorts As Collections.ObjectModel.ReadOnlyCollection(Of String)
		Dim NewPort As String = ""
		OriginalPorts = My.Computer.Ports.SerialPortNames
		Do While DateTime.Now < EndTime
			System.Threading.Thread.Sleep(50)
			NewPorts = My.Computer.Ports.SerialPortNames
			
			'New port added?
			Dim TestPort As String
			For Each TestPort In NewPorts
				If Not OriginalPorts.Contains(TestPort) Then
					NewPort = TestPort
					Exit Do
				End If
			Next
		Loop
		
		'No new port appeared in timeout period, exit
		If NewPort = "" Then
			Console.WriteLine
			Console.WriteLine("Arduino Robot was not detected within 30 seconds. Quitting.")
		
		'New port appeared, use it!
		Else
			Console.WriteLine
			Console.WriteLine("Arduino Robot detected on port " + NewPort + ", starting avrdude")
			Try
				Dim AVRDude As New System.Diagnostics.Process
				With AVRDude.StartInfo
					.FileName = "avrdude.exe"
					.Arguments = "-cavr109 -b57600 -P " + NewPort + " -p ATmega32u4 -D -U flash:w:" + HexFile + ":i"
					.RedirectStandardOutput = True
					.UseShellExecute = False
				End With
				AVRDude.Start
				AVRDude.WaitForExit(60000)
				ExitStatus = AVRDude.ExitCode
				Console.WriteLine(AVRDude.StandardOutput.ReadToEnd)
			Catch Ex As Exception
				Console.WriteLine("An error occurred while running avrdude!")
				Console.WriteLine(Ex.ToString)
				Console.WriteLine("Press any key to exit")
				Console.ReadKey
			End Try
		End If
		
		Console.WriteLine("Done!")
		
		Environment.ExitCode = ExitStatus
	End Sub
End Module
