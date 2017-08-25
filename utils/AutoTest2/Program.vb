'Automatic compiler testing program for GCBASIC
'Passes assembly files generated through MPASM to check for any syntax errors

'Changes:
'18/1/2007: First version (maybe earlier)
'28/3/2007: Changed to use C:\GCBASIC
'2/10/2013: New VB.NET version

Imports System.IO
Imports System.Collections.Generic

Module Program
	
	Dim FileList As List(Of String)
	
	Public Const ID As String = "C:\Final\utils\AutoTest2"
	Public Const TestProgDir As String = ID + "\programs"
	Public Const WorkingDir As String = ID + "\generated"
	Public Const GoodDir As String = ID + "\good"
	
	Public Const CompilerDir As String = "C:\Final\Great Cow Graphical BASIC"
	Public Const PICAssemblerDir As String = ID + "\mpasmx"
	Public Const AVRAssemblerDir As String = ID + "\avrassembler"
	Public Const DiffExe As String = "C:\programs\MinGW\msys\1.0\bin\diff.exe"
	Public Const BankCheckExe As String = ID + "\BankCheck.exe"
	
	Public Enum ChipTypes
		AVR
		PIC
		UNKNOWN
	End Enum
	
	Dim LastChipType As ChipTypes
	Dim LastChipModel As String
	
	Dim SpecificFiles As List(Of String)
	Dim TestLog As String
	
	Sub Main()
		Console.WriteLine("AutoTest2 starting")
		
		Dim CurrentFile As String
		Dim CurrentFileNo As Integer
		
		'Initialise
		SpecificFiles = New List(Of String)
		Dim CurrArgNo As Integer = 0
		For Each CurrArg In Environment.GetCommandLineArgs
			If CurrArgNo > 0 Then
				SpecificFiles.Add(CurrArg.ToLower)
			End If
			CurrArgNo += 1
		Next
		
		If SpecificFiles.Count > 0 Then
			Console.WriteLine("Testing with only specific files:")
			For Each CurrentFile In SpecificFiles
				Console.WriteLine(CurrentFile)
				TestLog = ID + "\partialtestlog.txt"
			Next
		Else
			TestLog = ID + "\testlog.txt"
		End If
		
		'Get list of files
		FileList = New List(Of String)
		Console.WriteLine("Building File List ...")
		Dim LibDir As New IO.DirectoryInfo(TestProgDir)
		Dim LibList As IO.FileInfo() = LibDir.GetFiles("*.gcb")
		Dim LibFileName As IO.FileInfo
		For Each LibFileName In LibList
			If SpecificFiles.Count = 0 Or SpecificFiles.Contains(LibFileName.Name.ToLower) Then
				FileList.Add(LibFileName.Name)
			End If
		Next
		
		'Create test log file
		Dim logFile As New StreamWriter(TestLog)
		logFile.WriteLine("GCBASIC Test Log " + DateTime.Now.ToString)
		logFile.WriteLine
		logFile.Close
		
		'Main routine
		CurrentFileNo = 0
		For Each CurrentFile In FileList
			CurrentFileNo += 1
			
			Console.WriteLine
			WriteLog("Compiling " + CurrentFile + " (" + Convert.ToString(CurrentFileNo) + "/" + Convert.ToString(FileList.Count) + ")", True, True)
			
			'Clear chip model
			LastChipModel = ""
			LastChipType = ChipTypes.UNKNOWN
			
			'Get names of asm and hex files
			Dim FileNoExt As String
			FileNoExt = CurrentFile
			If FileNoExt.IndexOf(".") <> -1 Then
				FileNoExt = FileNoExt.Substring(0, FileNoExt.LastIndexOf("."))
			End If
			
			'Compile file
			'FileTemp = Chr(34) + TestProgDir + "\" + FileList(CurrentFile) + CHR(34)
			'shell CompileCommand + " " + FileTemp + " >" + CompileErr
			'Dim startTime As DateTime = DateTime.Now
			If RunCompiler(CurrentFile, FileNoExt) Then
				
				'Compile succeeded, assemble with MPASM
				If RunAssembler(FileNoExt) Then
					
					'Compare generated hex
					CompareHexFile(FileNoExt + "_gcasm.hex", FileNoExt + ".hex")
					
					'Compare generated asm with old asm
					CompareAsmFile(GoodDir + "\" + FileNoExt + ".asm", WorkingDir + "\" + FileNoExt + ".asm")
					
					'Check bank and page selection
					CheckBankSelection(WorkingDir + "\" + FileNoExt)
					
					'Console.WriteLine("Checked in " + (DateTime.Now - startTime).ToString)
					
				End If
			End If
			
			'Get errors
			'shell "copy " + TestLog + "+" + ID + "\stars.txt+" + CompileErr + " " + TestLog + " >nul"
		
		Next
		
		Do
			Console.WriteLine("Press x to exit")
		Loop Until Console.ReadKey(True).Key = ConsoleKey.X
	End Sub
	
	Public Function RunCompiler(GcbFile As String, FileNoExt As String) As Boolean
		
		Dim AsmFile, HexFile As String
		AsmFile = FileNoExt + ".asm"
		HexFile = FileNoExt + ".hex"
		
		Dim GcbFileQ, AsmFileQ, HexFileQ As String
		GcbFileQ = AddQuotes(TestProgDir + "\" + GcbFile)
		AsmFileQ = AddQuotes(AsmFile)
		HexFileQ = AddQuotes(HexFile)
		
		'Call GCBASIC for gcb > asm, and GCASM for asm > hex
		Dim DoCompile As New System.Diagnostics.Process
		With DoCompile.StartInfo
			.FileName = CompilerDir + "\gcbasic.exe"
			.WorkingDirectory = WorkingDir
			.Arguments = GcbFileQ + " /S:" + ID + "\gcbasic.ini /NP /A:gcasm /P:none /O:" + AsmFileQ
			.UseShellExecute = False
			.RedirectStandardOutput = True
			'.RedirectStandardError = True
		End With
		
		'MessageBox.Show(DoCompile.StartInfo.Arguments, "Command Line", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1)
		DoCompile.Start()
		DoCompile.WaitForExit(30000)
		Dim GcbOut As String = DoCompile.StandardOutput.ReadToEnd
		WriteLog(GcbOut, False)
		
		'Check for errors.txt, and exit sub if found
		Dim FileCheck As New IO.FileInfo(WorkingDir + "\errors.txt")
   		If FileCheck.Exists Then
   			Console.WriteLine(GcbOut)
   			'Check to see if errors found, or just warnings
   			If GcbOut.IndexOf("Error:") <> -1 Then
   				Return False
   			End If
   		End If
   		
   		'Rename hex file to prevent overwriting
   		IO.File.Copy(WorkingDir + "\" + HexFile, WorkingDir + "\" + FileNoExt + "_gcasm.hex", True)
   		'IO.File.Delete(WorkingDir + "\" + HexFile)
   		
   		'Find chip type
   		Dim AsmReader As New StreamReader(WorkingDir + "\" + AsmFile)
   		Dim InLine As String
   		Do While AsmReader.Peek <> -1
   			InLine = AsmReader.ReadLine.Trim
   			'PIC
   			If InLine.StartsWith("LIST p") Then
   				LastChipType = ChipTypes.PIC
   				LastChipModel = InLine.Substring(InLine.IndexOf("p=") + 2)
   				If LastChipModel.IndexOf(",") <> -1 Then
   					LastChipModel = LastChipModel.Substring(0, LastChipModel.IndexOf(","))
   				End If
   				'Console.WriteLine("Found PIC:" + LastChipModel)
   				Exit Do
   			
   			'AVR
   			Else If InLine.StartsWith(".INCLUDE ") Then
   				LastChipType = ChipTypes.AVR
   				LastChipModel = InLine.Substring(10)
   				LastChipModel = LastChipModel.Substring(0, LastChipModel.IndexOf("def"))
   				'Console.WriteLine("Found AVR:" + LastChipModel)
   				Exit Do
   			End If
   		Loop
   		AsmReader.Close
   		
   		Return True
	End Function
	
	Public Function RunAssembler(FileNoExt As String) As Boolean
		
		Dim AsmFile, HexFile, ErrFile As String
		AsmFile = FileNoExt + ".asm"
		HexFile = FileNoExt + ".hex"
		ErrFile = FileNoExt + ".err"
		
		Dim AsmFileQ, HexFileQ As String
		AsmFileQ = AddQuotes(AsmFile)
		HexFileQ = AddQuotes(HexFile)
		
		Dim DoAssemble As New System.Diagnostics.Process
		If LastChipType = ChipTypes.PIC Then
			With DoAssemble.StartInfo
				.FileName = PICAssemblerDir + "\Mpasmx.exe"
				.Arguments = "/c- /o- /q+ /l+ /x- /w1 " + AsmFileQ
				.WorkingDirectory = WorkingDir
				.WindowStyle = ProcessWindowStyle.Hidden
				.CreateNoWindow = True
			End With
			
			DoAssemble.Start
			DoAssemble.WaitForExit(30000)
			
			Dim FileCheck As New IO.FileInfo(WorkingDir + "\" + ErrFile)
	   		If FileCheck.Exists Then
	   			'Log error
	   			Dim errorReader As New StreamReader(WorkingDir + "\" + ErrFile)
	   			Dim errorList As String = errorReader.ReadToEnd
	   			If errorList <> "" Then
		   			WriteLog(errorList, True)
		   			'Errors, or just warnings?
		   			If errorList.Contains("Error:") Then
		   				Return False
		   			Else
		   				Return True
		   			End If
		   		End If
	   		End If
			
		Else If LastChipType = ChipTypes.AVR Then
			With DoAssemble.StartInfo
				.FileName = AVRAssemblerDir + "\avrasm2.exe"
				.Arguments = "-v0 -fI " + AsmFileQ
				.WorkingDirectory = WorkingDir
				.UseShellExecute = False
				.RedirectStandardOutput = True
				.RedirectStandardError = True
			End With
			DoAssemble.Start
			DoAssemble.WaitForExit(30000)
			
			Dim AsmOut As String = DoAssemble.StandardOutput.ReadToEnd + DoAssemble.StandardError.ReadToEnd
			If AsmOut <> "" Then
				WriteLog(AsmOut, True)
				Return False
			End If
			
		Else
			WriteLog("Error: Could not find suitable external assembler", True)
			Return False
		End If
		
		Return True
	End Function
	
	Public Function CompareHexFile(gcasmHexFile As String, mpasmHexFile As String) As Boolean
		
		Dim gcasmHex, mpasmHex As IntelHexFile
		Try
			mpasmHex = New IntelHexFile(WorkingDir + "\" + mpasmHexFile)
		Catch
			WriteLog("Error: Unable to load mpasm hex file", True)
			Return False
		End Try
		Try
			gcasmHex = New IntelHexFile(WorkingDir + "\" + gcasmHexFile)
		Catch Ex As Exception
			WriteLog("Error: Unable to load gcasm hex file:" + Ex.ToString, True)
			Return False
		End Try
		
		'Check file sizes
		'Console.WriteLine("File max location:" + Convert.ToString(gcasmHex.MaxLocation))
		Dim fileSize As Integer = gcasmHex.MaxLocation
		If fileSize < mpasmHex.MaxLocation Then
			fileSize = mpasmHex.MaxLocation
		End If
		
		Dim check16401 As Boolean = True
		If LastChipModel.StartsWith("10F") Or LastChipModel.StartsWith("12F") Or LastChipModel.StartsWith("16F") Then
			check16401 = False
		End If
		
		Dim currLocation As Integer
		Dim gcasmByte, mpasmByte As Byte
		For currLocation = 0 To fileSize
			'Console.WriteLine(Convert.ToString(currLocation) + ":" + Convert.ToString(gcasmHex.GetByte(currLocation)))
			gcasmByte = gcasmHex.GetByte(currLocation, 255)
			mpasmByte = mpasmHex.GetByte(currLocation, 255)
			If gcasmByte <> mpasmByte Then
				'Ignore any differences at location 16401 on 12F/16F, 3F = FF, just a minor difference in the assemblers
				If check16401 Then
					WriteLog("Error: Hex file mismatch at " + Convert.ToString(currLocation) + ", GCASM:" + Utils.Dec2Other(gcasmByte, 16) + " Other:" + Utils.Dec2Other(mpasmByte, 16), True)
				Else
					If currLocation <> 16401 Or gcasmByte <> &H3F Or mpasmByte <> &HFF Then
						WriteLog("Error: Hex file mismatch at " + Convert.ToString(currLocation) + ", GCASM:" + Utils.Dec2Other(gcasmByte, 16) + " Other:" + Utils.Dec2Other(mpasmByte, 16), True)
					End If
				End If
			End If
		Next
		
		Return True
	End Function
	
	Public Function CompareAsmFile(OldFile As String, NewFile As String) As Boolean
		'Compare new file with (assumed good) old file, report changes
		'Could be improvements, or could be bugs. Either way, should check!
		
		'Check if files exist
		If Not (New IO.FileInfo(OldFile)).Exists Then
			Return True
		End If
		If Not (New IO.FileInfo(NewFile)).Exists Then
			Return False
		End If
		
		Dim OldFileQ, NewFileQ As String
		OldFileQ = AddQuotes(OldFile)
		NewFileQ = AddQuotes(NewFile)
		
		'Console.WriteLine("Comparing asm files")
		Dim DoDiff As New System.Diagnostics.Process
		With DoDiff.StartInfo
			.FileName = DiffExe
			.Arguments = "-i -E -w -B -I '^;Program' " + OldFileQ + " " + NewFileQ
			.UseShellExecute = False
			.RedirectStandardOutput = True
		End With
		DoDiff.Start
		DoDiff.WaitForExit(30000)
		
		Dim DiffOut As String
		DiffOut = DoDiff.StandardOutput.ReadToEnd
		If DiffOut <> "" Then
			WriteLog("Warning: diff found difference:" + Environment.NewLine + DiffOut, True)
			Return False
		End If
		
		Return True
	End Function
	
	Public Sub CheckBankSelection(FileNoExt As String)
		'Use CheckBank to verify bank and page selection
		'Only do this on PIC12F/16F
		Dim ChipModelLower As String = LastChipModel.Trim.ToLower
		If (ChipModelLower.Contains("12f") Or ChipModelLower.Contains("16f")) And Not ChipModelLower.Contains("f5") Then
			
			Dim AsmFile As String
			AsmFile = FileNoExt + ".asm"
			
			'Check if files exist
			If Not (New IO.FileInfo(AsmFile)).Exists Then
				Return
			End If
			
			Dim AsmFileQ As String
			AsmFileQ = AddQuotes(AsmFile)
			
			'Console.WriteLine("Comparing asm files")
			Dim CheckBank As New System.Diagnostics.Process
			With CheckBank.StartInfo
				.FileName = BankCheckExe
				.Arguments = AsmFileQ + " """ + PICAssemblerDir + """"
				.UseShellExecute = False
				.RedirectStandardOutput = True
			End With
			CheckBank.Start
			CheckBank.WaitForExit(30000)
			
			Dim CheckBankOut As String
			CheckBankOut = CheckBank.StandardOutput.ReadToEnd
			If CheckBankOut <> "" Then
				WriteLog("Warning: CheckBank produced messages:" + Environment.NewLine + CheckBankOut, True)
				Return
			End If
		End If	
	End Sub
	
	Public Function AddQuotes(FileName As String) As String
		If FileName.IndexOf(" ") <> -1 Then
			FileName = """" + FileName + """"
		End If
		Return FileName
	End Function
	
	Public Sub WriteLog(LogItem As String, EchoMessage As Boolean, Optional StarsBefore As Boolean = False)
		If EchoMessage Then
			Console.WriteLine(LogItem)
		End If
		
		Dim output As New StreamWriter(TestLog, True)
		If StarsBefore Then
			output.WriteLine
			output.WriteLine("********************************************************************************")
		End If
		output.WriteLine(LogItem)
		output.Close
		
	End Sub
	
End Module

