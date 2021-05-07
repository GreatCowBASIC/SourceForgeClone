'
' Created by SharpDevelop.
' User: Hugh
' Date: 23/01/2010
' Time: 4:10 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Collections

Namespace Great_Cow_Graphical_BASIC
	
	Public Class SubParam
		
		Public Dim Name As String
		Public Dim Description As String
	 	Public Dim DataType As String
	 	Public Dim DataDir As String
	 	Public Dim DefValue As String
	 	Public Dim DataArray As Boolean
	 	Public Dim CurValue As String
	 	
	 	Public Sub New()
	 		Description = ""
	 		
	 	End Sub
	 	
	 	Public Sub New (ByVal InData As String, ForceIn As Boolean)
	 		Dim LoopMore As Boolean
	 		
	 		InData = InData.Trim
	 		
	 		DefValue = ""
	 		If InData.IndexOf("=") <> -1 Then
	 			DefValue = InData.Substring(InData.IndexOf("=") + 1).Trim
	 			InData = InData.Substring(0, InData.IndexOf("=")).Trim
	 		End If
	 		
	 		If ForceIn Then
	 			DataDir = "I"
	 		Else
	 			DataDir = "IO"
	 		End If
	 		Do
		 		LoopMore = False
		 		If InData.ToLower.StartsWith("in ") Then
		 			DataDir = "I"
		 			InData = InData.Substring(3)
		 			LoopMore = True
		 		ElseIf InData.ToLower.StartsWith("out ") Then
		 			DataDir = "O"
		 			InData = InData.Substring(4)
		 			LoopMore = True
		 		ElseIf InData.ToLower.StartsWith("optional ") Then
		 			InData = InData.Substring(9)
		 			LoopMore = True
		 		End If
		 	Loop While LoopMore
	 		
	 		DataArray = False
	 		If InData.IndexOf("()") <> -1 Then
	 			DataArray = True
	 			InData = InData.Replace("()", "")
	 		End If
	 		
	 		If InData.ToLower.IndexOf(" as ") <> -1 Then
	 			DataType = InData.Substring(InData.ToLower.IndexOf(" as ") + 4).Trim.ToLower
	 			InData = InData.Substring(0, InData.ToLower.IndexOf(" as ")).Trim
	 		ElseIf InData.IndexOf("$") <> -1 Then
	 			DataType = "string"
	 			InData = InData.Replace("$", "")
	 		Else
	 			DataType = "byte"
	 		End If
	 		
	 		Name = InData
			Description = ""
			
	 	End Sub
	 	
	 	Public Function GetCode (Optional OmitDirection As Boolean = False) As String
	 		Dim OutVal As String
	 		
	 		OutVal = Name
			If DataArray Then
				OutVal = OutVal + "()"
			End If
			
			If DataType <> "byte" Then
				OutVal = OutVal + " As " + DataType
			End If
			
			If Not OmitDirection Then
				If DataDir = "I" Then
					OutVal = "In " + OutVal
				ElseIf DataDir = "O" Then
					OutVal = "Out " + OutVal
				End If
			End If
			
			If DefValue <> "" Then
				OutVal = "Optional " + OutVal + " = " + DefValue
			End If
			
			Return OutVal
	 	End Function
	 	
	End Class
	
	Public Class GCBSubroutine
		'Stores a subroutine
		'Sub name
		Private Dim pName As String
		'Description
		Public Dim Description As String
		'Hide in subroutine list?
		Public Dim HideSub As Boolean
		'Sub return type (used for functions)
		Public Dim ReturnType As String
		'Parameter list
		'List of SubParams
		Public Dim Parameters As ArrayList
		'Code listing
		'List of Strings
		Public Dim Lines As ArrayList
		
		Public Sub New(Name As String)
			pName = Name
			
			Description = ""
			Parameters = New ArrayList
			Lines = New ArrayList
		End Sub
		
		Public Property Name
			Get
				Return pName
			End Get
			Set
				pName = Value
			End Set
		End Property
		
		'Parameter list conversion
		Public Property StartCode
			'Generate parameter string
			Get
				Dim OutVal As String
				
				'Routine name
				If ReturnType = "" Then
					OutVal = "Sub " + pName
				Else
					OutVal = "Function " + pName
				End If
				
				'Parameters
				If Parameters.Count > 0 Then
					Dim isFirstParam As Boolean = True
					Dim thisParam As SubParam
					For Each thisParam In Parameters
						If isFirstParam Then
							isFirstParam = False
							OutVal += " ("
						Else
							OutVal += ", "
						End If
						OutVal += thisParam.GetCode(ReturnType <> "")
					Next
					OutVal += ")"
				End If
				
				If ReturnType <> "" Then
					If ReturnType.Trim.ToLower <> "byte" Then
						OutVal += " As " + ReturnType
					End If
				End If
				
				Return OutVal
			End Get
			
			'Parse parameter string
			Set
				Dim SubTemp, ParamTemp, NewName As String
				Dim ForceIn As Boolean = False
				
				Parameters = New ArrayList
				
				'Format: {sub | function} name [(param1 [as type][, param2 [as type][, ...]])] [as type] [#nr]
				SubTemp = Value
				Dim oldTemp As String = SubTemp
				If SubTemp.ToLower.IndexOf("#nr") <> -1 Then
					SubTemp = SubTemp.Substring(0, SubTemp.ToLower.IndexOf("#nr")).Trim
					ForceIn = True
				End If
				
				'Get sub/function
				If SubTemp.ToLower.StartsWith("sub ") Then
					ReturnType = ""
					SubTemp = SubTemp.Substring(4).Trim
				Else If SubTemp.ToLower.StartsWith("function ") Then
					ReturnType = "byte"
					SubTemp = SubTemp.Substring(9).Trim
					ForceIn = True 'Functions cannot return values in params
				End If
				
				'Remove parameters from name
				If SubTemp.IndexOf("(") <> -1 Then
					
					Dim FindStart, FindEnd, CurrLevel As Integer
					CurrLevel = 0
					FindStart = SubTemp.IndexOf("(")
					For FindEnd = FindStart To SubTemp.Length - 1
						If SubTemp.Substring(FindEnd, 1) = "(" Then CurrLevel += 1
						If SubTemp.Substring(FindEnd, 1) = ")" Then CurrLevel -= 1
						If CurrLevel = 0 Then
							ParamTemp = SubTemp.Substring(FindStart + 1, FindEnd - FindStart - 1)
							SubTemp = Subtemp.Substring(0, FindStart) + SubTemp.Substring(FindEnd + 1)
							Exit For
						End If
					Next
					
					If CurrLevel <> 0 Then
						'Bracket mismatch
						ParamTemp = SubTemp.Substring(FindStart + 1)
						SubTemp = Subtemp.Substring(0, FindStart)
					End If
					
				End If
				
				'Get Type
				If SubTemp.ToLower.IndexOf(" as ") <> - 1 Then
					ReturnType = SubTemp.Substring(SubTemp.ToLower.IndexOf(" as ") + 4).ToLower
					SubTemp = SubTemp.Substring(0, SubTemp.ToLower.IndexOf(" as "))
				End If
				
				pName = SubTemp.Trim
				
				'Get parameters
				If ParamTemp <> "" Then
					Dim paramList() As String = ParamTemp.Split(",")
					Dim thisParam As String
					For Each thisParam In paramList
						Parameters.Add(New SubParam(thisParam, ForceIn))
					Next
				End If
			End Set
		End Property
		
		Public ReadOnly Property EndCode As String
			Get
				If ReturnType = "" Then
					Return "End Sub"
				Else
					Return "End Function"
				End If
			End Get
		End Property
		
		Public ReadOnly Property IsFunction As Boolean
			Get
				Return ReturnType <> ""
			End Get
		End Property
		
		Public ReadOnly Property IsSub As Boolean
			Get
				Return ReturnType = ""
			End Get
		End Property
		
		Public Shared Function FromCode(CodeIn As String, Optional DocIn As DocSection = Nothing) As GCBSubroutine
			'Return a new subroutine from a line of code
			Dim outSub As New GCBSubroutine("")
			outSub.StartCode = CodeIn
			
			'Add documentation (if present)
			If Not DocIn Is Nothing Then
				outSub.Description = DocIn.Description
				outSub.HideSub = DocIn.HideItem
				
				Dim currParam As SubParam
				Dim paramDesc As String
				For Each currParam In outSub.Parameters
					currParam.Description = DocIn.Params.GetValue(currParam.Name)
				Next
				
			End If
			
			Return outSub
		End Function
		
	End Class
	
End Namespace
