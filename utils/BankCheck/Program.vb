'	BankCheck, a program to check bank and page selection in PIC assembly
'	Main File
'	Copyright (C) 2014 Hugh Considine
'
'	This program is free software; you can redistribute it and/or modify
'	it under the terms of the GNU General Public License as published by
'	the Free Software Foundation; either version 2 of the License, or
'	(at your option) any later version.
'
'	This program is distributed in the hope that it will be useful,
'	but WITHOUT ANY WARRANTY; without even the implied warranty of
'	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'	GNU General Public License for more details.
'
'	You should have received a copy of the GNU General Public License
'	along with this program; if not, write to the Free Software
'	Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
'
' Created by SharpDevelop.
' User: Hugh
' Date: 24/04/2014
' Time: 8:22 PM

Module Program
	Sub Main()
		
		If My.Application.CommandLineArgs.Count < 1 Or My.Application.CommandLineArgs.Count > 2 Then
			Console.WriteLine("Bank selection checker")
			Console.WriteLine()
			Console.WriteLine("    bankcheck filename.asm [MPASM path]")
			Console.WriteLine("    Where")
			Console.WriteLine("      filename.asm is the file to check")
			Console.WriteLine("      MPASM path is the path to folder containing .inc files")
			Console.WriteLine("        (Default: C:\Program Files (x86)\Microchip\MPLABX\mpasmx)")
			Console.WriteLine()
			Console.WriteLine("Errors will be displayed. No output means no errors found")
			Console.WriteLine
			Console.WriteLine("By Hugh Considine for GCBASIC, 2014. Released under GNU GPL, V2")
			
		Else
			'Console.WriteLine("Checking " + My.Application.CommandLineArgs(0))
			Dim fileName As String = My.Application.CommandLineArgs(0)
			Dim incPath As String = "C:\Program Files (x86)\Microchip\MPLABX\mpasmx\"
			If My.Application.CommandLineArgs.Count = 2 Then
				incPath = My.Application.CommandLineArgs(1)
				If Not incPath.EndsWith("\") Then incPath += "\"
			End If
			
			Dim checker As New BankChecker(incPath, fileName)
			
		End If
		
		'Console.Write("Press any key to continue . . . ")
		'Console.ReadKey(True)
	End Sub
End Module
