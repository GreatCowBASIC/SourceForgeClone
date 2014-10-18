'
' Created by SharpDevelop.
' User: Hugh
' Date: 28/01/2010
' Time: 12:05 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Collections.Generic

'Namespace Great_Cow_Graphical_BASIC
	
	Public Class VariableListItem
		Public Dim Name As String
		Public Dim Type As String
		Public Dim IsArray As Boolean
		Public Dim Length As Integer
		Public Dim Hidden As Boolean
		Public Dim VarAlias As String 'Don't expose, but do store
		Public Dim VarAt As String 'Don't expose, but do store
		
		Private Sub New
			
		End Sub
		
		Public Sub New (CopyVar As VariableListItem)
			With CopyVar
				Name = .Name
				Type = .Type
				IsArray = .IsArray
				Length = .Length
				Hidden = .Hidden
				VarAlias = .VarAlias
				VarAt = .VarAt
			End With
		End Sub
		
		Public Sub New (VarName As String, VarType As String, Optional IsArray As Boolean = False, Optional Length As Integer = 0)
			Me.Name = VarName
			Me.Type = VarType
			Me.IsArray = IsArray
			Me.Length = Length
			
			Me.Hidden = False
			Me.VarAlias = ""
			Me.VarAt = ""
		End Sub
		
		Public Property Code As String
			Get
				Dim OutData As String
				If IsArray Then
					OutData = "Dim " + Name + "(" + Length.ToString + ")"
				Else
					OutData = "Dim " + Name + " As " + Type
					If Length > 0 Then
						OutData += " * " + Length.ToString
					End If
				End If
				If VarAlias <> "" Then
					OutData += " Alias " + VarAlias
				End If
				If VarAt <> "" Then
					OutData += " At " + VarAt
				End If
				Return OutData
			End Get
			
			Set
				Dim TempData As String
				TempData = Value.Trim
				If TempData.ToLower.StartsWith("dim ") Then
					TempData = TempData.Substring(4).Trim
				End If
				
	     		Name = TempData.Trim
     			Type = "byte"
     			IsArray = False
     			Length = 0
     			VarAlias = ""
     			VarAt = ""
     			
     			'Get fixed location
     			If Name.ToLower.IndexOf(" at ") <> -1 Then
     				VarAt = Name.Substring(Name.ToLower.IndexOf(" at ") + 4).Trim
	     			Name = Name.Substring(0, Name.ToLower.IndexOf(" at ")).Trim
	     			
	     			If VarAt.ToLower.IndexOf(" as ") <> -1 Then
		     			Type = VarAt.Substring(VarAt.ToLower.IndexOf(" as ") + 4).Trim
		     			VarAt = VarAt.Substring(0, VarAt.ToLower.IndexOf(" as ")).Trim
		     		End If
     			End If
     			
     			'Get alias
     			If Name.ToLower.IndexOf(" alias ") <> -1 Then
     				VarAlias = Name.Substring(Name.ToLower.IndexOf(" alias ") + 7).Trim
	     			Name = Name.Substring(0, Name.ToLower.IndexOf(" alias ")).Trim
	     			
	     			If VarAlias.ToLower.IndexOf(" as ") <> -1 Then
		     			Type = VarAlias.Substring(VarAlias.ToLower.IndexOf(" as ") + 4).Trim
		     			VarAlias = VarAlias.Substring(0, VarAlias.ToLower.IndexOf(" as ")).Trim
		     		End If
     			End If
     			
     			'Get type
	     		If Name.ToLower.IndexOf(" as ") <> -1 Then
	     			Type = Name.Substring(Name.ToLower.IndexOf(" as ") + 4).Trim
	     			Name = Name.Substring(0, Name.ToLower.IndexOf(" as ")).Trim
	     		End If
	     		
	     		'Get string size
	     		If Type.IndexOf("*") <> -1 Then
	     			Length = Type.Substring(Type.IndexOf("*") + 1).Trim
	     			Type = Type.Substring(0, Type.IndexOf("*")).Trim
	     		End If
	     		
	     		'Get array size
	     		If Name.IndexOf("(") <> -1 Then
	     			IsArray = True
	     			TempData = Name.Substring(Name.IndexOf("(") + 1)
	     			TempData = TempData.Substring(0, TempData.IndexOf(")"))
	     			If TempData.Length > 0 Then
	     				Length = TempData
	     			End If
	     			
	     			Name = Name.Substring(0, Name.IndexOf("(")).Trim
	     		End If
			End Set
		End Property
		
		Public Shared Function FromCode(CodeIn As String) As VariableListItem
			'Creates a new variable list item from code
			Dim outItem As New VariableListItem
			outItem.Code = CodeIn
			Return outItem
		End Function
		
	End Class
	
	Public Class VariableList
		
		Private Dim Variables As List(Of VariableListItem)
		
		Public Sub New()
			
			Variables = New List(Of VariableListItem)
			
		End Sub
		
		Public Sub Clear
			Variables = New List(Of VariableListItem)
		End Sub
		
		Public ReadOnly Property Count As Integer
			Get
				Return Variables.Count
			End Get
		End Property
		
		Public Function GetItem(ItemNo As Integer) As VariableListItem
			If ItemNo < 0 Or ItemNo >= Variables.Count Then
				Return Nothing
			End If
			
			Return Variables(ItemNo)
		End Function
		
		Public Function GetVar(VarName As String) As VariableListItem
			'Find and return a variable with the given name
			'Return Nothing if no variable found
			Dim searchVar As VariableListItem
			
			For Each searchVar In Variables
				If searchVar.Name.Trim.ToLower = VarName.Trim.ToLower Then
					Return searchVar
				End If
			Next
			
			Return Nothing
			
		End Function
		
'		Public Sub AddVar(NewVar As VariableListItem)
'			With NewVar
'				AddVar(.Name, .Type, .IsArray, .Length, .Hidden)
'			End With
'		End Sub
		
		Public Sub AddVar(ByVal VarName As String, ByVal VarType As String, Optional IsArray As Boolean = False, Optional Length As Integer = 0, Optional Hidden As Boolean = False)
			Dim NewVar As New VariableListItem(VarName, VarType, IsArray, Length)
			NewVar.Hidden = Hidden
			AddVar(NewVar)
		End Sub
		
		'Public Sub AddVar(ByVal VarName As String, ByVal VarType As String, Optional IsArray As Boolean = False, Optional Length As Integer = 0, Optional Hidden As Boolean = False)
		Public Sub AddVar(NewVar As VariableListItem)
			Dim searchVar, foundVar As VariableListItem
			Dim DupLoc As Integer = 0
			
			'Check for duplicate
			foundVar = Nothing
			For Each searchVar In Variables
				If searchVar.Name.Trim.ToLower = NewVar.Name.Trim.ToLower Then
					foundVar = searchVar
					Exit For
				End If
			Next
			
			'Default type
			If NewVar.Type = "" Then NewVar.Type = "byte"
			
			'Add
			If foundVar Is Nothing Then
				foundVar = NewVar
				Variables.Add(foundVar)
				'Throw New Exception("Adding variable " + VarName)
			Else
				If Lowlevel.CastOrder(foundVar.Type) > Lowlevel.CastOrder(NewVar.Type) Then NewVar.Type = foundVar.Type
				If foundVar.Length > NewVar.Length Then NewVar.Length = foundVar.Length
			End If
			
			With foundVar
				.Name = NewVar.Name
				.Type = NewVar.Type.ToLower
				.Length = NewVar.Length
				.IsArray = NewVar.IsArray
				.Hidden = NewVar.Hidden
			End With
		End Sub
		
		Public Sub DeleteVar(ByVal VarName As String)
			Dim Temp As Integer = 0
			Dim searchVar, delVar As VariableListItem
			
			'Find location
			delVar = Nothing
			For Each searchVar In Variables
				If searchVar.Name.Trim.ToLower = VarName.Trim.ToLower Then
					delVar = searchVar
					Exit For
				End If
			Next
			
			'Exit if var does not exist
			If delVar Is Nothing Then Exit Sub
			
			'Delete
			Variables.Remove(delVar)
			
		End Sub
		
		
	End Class
'End Namespace
