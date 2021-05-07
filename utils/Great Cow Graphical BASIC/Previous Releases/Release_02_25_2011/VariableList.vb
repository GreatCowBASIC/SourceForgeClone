'
' Created by SharpDevelop.
' User: Hugh
' Date: 28/01/2010
' Time: 12:05 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Collections

Namespace Great_Cow_Graphical_BASIC
	
	Public Class VariableListItem
		Public Dim Name As String
		Public Dim Type As String
		Public Dim IsArray As Boolean
		Public Dim Length As Integer
		Public Dim Hidden As Boolean
		Public Dim VarAlias As String 'Don't expose, but do store
		
		Public Sub New
			
		End Sub
		
		Public Sub New (CopyVar As VariableListItem)
			With CopyVar
				Name = .Name
				Type = .Type
				IsArray = .IsArray
				Length = .Length
				Hidden = .Hidden
				VarAlias = .VarAlias
			End With
		End Sub
		
		Public Sub New (VarName As String, VarType As String, Optional IsArray As Boolean = False, Optional Length As Integer = 0)
			Me.Name = VarName
			Me.Type = VarType
			Me.IsArray = IsArray
			Me.Length = Length
			
			Me.Hidden = False
			Me.VarAlias = ""
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
					If VarAlias <> "" Then
						OutData += " Alias " + VarAlias
					End If
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
		
		Private Dim Variables As ArrayList
		
		Public Sub New()
			
			Variables = New ArrayList
			
		End Sub
		
		Public Sub Clear
			Variables = New ArrayList
		End Sub
		
		Public ReadOnly Property Count
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
		
		Public Sub AddVar(NewVar As VariableListItem)
			With NewVar
				AddVar(.Name, .Type, .IsArray, .Length, .Hidden)
			End With
		End Sub
		
		Public Sub AddVar(ByVal VarName As String, ByVal VarType As String, Optional IsArray As Boolean = False, Optional Length As Integer = 0, Optional Hidden As Boolean = False)
			Dim searchVar, newVar As VariableListItem
			Dim DupLoc As Integer = 0
			
			'Check for duplicate
			newVar = Nothing
			For Each searchVar In Variables
				If searchVar.Name.Trim.ToLower = VarName.Trim.ToLower Then
					newVar = searchVar
					Exit For
				End If
			Next
			
			'Default type
			If VarType = "" Then VarType = "byte"
			
			'Add
			If newVar Is Nothing Then
				newVar = New VariableListItem
				Variables.Add(newVar)
			Else
				If Lowlevel.CastOrder(newVar.Type) > Lowlevel.CastOrder(VarType) Then VarType = newVar.Type
				If newVar.Length > Length Then Length = newVar.Length
			End If
			
			With newVar
				.Name = VarName
				.Type = VarType.ToLower
				.Length = Length
				.IsArray = IsArray
				.Hidden = Hidden
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
End Namespace
