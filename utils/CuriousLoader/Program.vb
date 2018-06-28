' Simple program to get programs onto the Microchip Curiosity board
' Responsible for finding which drive letter it has been assignment, then
' copying over the hex file.
'
' Code is released under the GNU GPL licence, v2 or higher.
'
' User: Hugh and Evan
' Date: 20/06/2018
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
			Console.WriteLine("CuriosityLoader - Loads hex files onto Microchip Curiosity Nano board")
			Console.WriteLine
			Console.WriteLine(" CuriosityLoader filename")
			Console.WriteLine
			Console.WriteLine("Where:")
			Console.WriteLine("  filename     Name of hex file to load")
			Console.WriteLine
			End
		End If
		
		Console.WriteLine("CuriosityLoader is preparing to load board ...")
		
		'Need to find correct drive (should have name Curiosity, be removable, and be small)
		Dim destDir As DirectoryInfo = Nothing
		Dim driveList() As DriveInfo = DriveInfo.GetDrives
		For Each drive In driveList
			If drive.IsReady Then
				If drive.DriveType = DriveType.Removable And drive.VolumeLabel = "CURIOSITY" And drive.AvailableFreeSpace < 1110016 Then
					destDir = drive.RootDirectory
				End If
			End If
		Next
		
		'Drive found?
		If destDir Is Nothing Then
			Console.WriteLine("Error: Could not find Curiosity board. Make sure that the removable drive Curiosity is present!")
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
