'
' Created by SharpDevelop.
' User: Hugh
' Date: 2/05/2010
' Time: 1:48 AM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.IO
Imports System.Collections

Namespace File_Compare
	
	Public Class IntelHexFile
		
		Private Class HexFileRecord
			Public Sub New(Location As Integer, Value As Byte)
				Me.Location = Location
				Me.Value = Value
			End Sub
			
			Public Dim Location As Integer
			Public Dim Value As Byte
		End Class
		
		Private Dim FileBytes As ArrayList
		
		Public Sub New (FileName As String)
			FileBytes = New ArrayList
			Load (FileName)
		End Sub
		
		Public Sub New()
			FileBytes = New ArrayList
		End Sub
		
		Public Sub SetByte(Location As Integer, Value As Byte)
			
			'Find location to insert byte at
			'Keep list sorted
			Dim currRec As HexFileRecord
			Dim currLoc As Integer
			For currLoc = FileBytes.Count - 1 to 0 Step -1
				currRec = fileBytes(currLoc)
				If currRec.Location > Location Then
					'Location found, insert and exit
					FileBytes.Insert(currLoc, New HexFileRecord(Location, Value))
					Exit Sub
				End If
			Next
			
			'If not inserted yet, add to end
			FileBytes.Add(New HexFileRecord(Location, Value))
			
		End Sub
		
		Public Function GetByte(Location As Integer, Optional DefValue As Byte = 0) As Byte
			'Should really use binary search here
			Dim currRec As HexFileRecord
			Dim currLoc As Integer
			For currLoc = 0 To FileBytes.Count - 1
				currRec = fileBytes(currLoc)
				If currRec.Location = Location Then
					Return currRec.Value
				End If
			Next
			
			Return DefValue
		End Function
		
		Public Sub Load (FileName As String)
			Dim TempData As String
			Dim ExtAddress As Integer = 0
			
			Dim ReadFile As New StreamReader(Utils.AddFullPath(FileName, MainForm.InstallDir))
   			Do While ReadFile.Peek() <> -1
	     		TempData = ReadFile.ReadLine().Trim
	     		If TempData.StartsWith(":") Then
	     			
	     			'Have read a record from the file, now parse it
	     			'Line format: :LLAAAATT[DD[DD[..]]]CC
	     			' LL is length in bytes of data
	     			' AAAA is address
	     			' TT is type, 0 normal data, 1 end record, 4 extended address
	     			' DD is data
	     			' CC is checksum
	     			
	     			'Get some items from the line
	     			Dim LineLen, LineAddress, LineType As Integer
	     			LineLen = Utils.Other2Dec(TempData.Substring(1, 2), 16)
	     			LineAddress = Utils.Other2Dec(TempData.Substring(3, 4), 16)
	     			LineType = Utils.Other2Dec(TempData.Substring(7, 2), 16)
	     			
	     			'Process differently depending on record type
	     			Select Case LineType
	     				'Normal data
	     				Case 0
	     					Dim currByte, currByteVal As Integer
	     					For currByte = 0 To LineLen - 1
	     						currByteVal = Utils.Other2Dec(TempData.Substring(9 + currByte * 2, 2), 16)
	     						SetByte(ExtAddress + LineAddress + currByte, currByteVal)
	     					Next
	     					
	     				'End record
	     				Case 1
	     					Exit Do
	     					
	     				'Extended address record
	     				Case 4
	     					If LineLen = 2 Then
	     						ExtAddress = Utils.Other2Dec(TempData.Substring(9, 4), 16)
	     					End If
	     					
	     			End Select
	     			
     			End If
     		Loop
     		
     		ReadFile.Close()
	     	
		End Sub
		
		Public Sub Save (FileName As String)
			
			
			
		End Sub
		
	End Class
End Namespace
