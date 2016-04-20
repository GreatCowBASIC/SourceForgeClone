' Simple program to get programs onto the Microchip Xpress board
' Responsible for finding which drive letter it has been assignment, then
' copying over the hex file.
'
' Code is released under the GNU GPL licence, v2 or higher.
'
' User: Hugh
' Date: 20/04/2016
' Time: 9:25 PM

Imports System.IO

Module Program
	Sub Main()
		
		Dim ExitStatus As Integer = 0
		
		'Get command line parameters
		Dim HexFile As String = ""
		Dim CurrArg As String
		Dim CurrArgNo As Integer = 0
		For Each CurrArg In Environment.GetCommandLineArgs
			If CurrArgNo > 0 Then
				HexFile = CurrArg
			End If
			CurrArgNo += 1
		Next
		
		If HexFile = "" Then
			Console.WriteLine("XpressLoader - Loads hex files onto Microchip Xpress board")
			Console.WriteLine
			Console.WriteLine(" XpressLoader filename")
			Console.WriteLine
			Console.WriteLine("Where:")
			Console.WriteLine("  filename     Name of hex file to load")
			Console.WriteLine
			End
		End If
		
		Console.WriteLine("XpressLoader is preparing to load board ...")
		
		'Need to find correct drive (should have name XPRESS, be removable, and be small)
		Dim destDir As DirectoryInfo = Nothing
		Dim driveList() As DriveInfo = DriveInfo.GetDrives
		For Each drive In driveList
			If drive.IsReady Then
				If drive.DriveType = DriveType.Removable And drive.VolumeLabel = "XPRESS" And drive.AvailableFreeSpace < 1048576 Then
					destDir = drive.RootDirectory
				End If
			End If
		Next
		
		'Drive found?
		If destDir Is Nothing Then
			Console.WriteLine("Error: Could not find Xpress board. Make sure that the removable drive XPRESS is present!")
			Environment.ExitCode = 1
			Exit Sub
		End If
		
		Console.WriteLine("Copying file to board ...")
		Try
			File.Copy(HexFile, destDir.FullName + "\code.hex", True)
			Console.WriteLine("Done!")
		Catch Ex As Exception
			Console.WriteLine("An error occurred: " + Ex.ToString)
			Environment.ExitCode = 2
		End Try
		
	End Sub
End Module
