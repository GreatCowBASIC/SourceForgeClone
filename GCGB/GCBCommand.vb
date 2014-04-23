'
' Created by SharpDevelop.
' User: Hugh
' Date: 13/07/2009
' Time: 10:18 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Collections.Generic
Imports System.Text.RegularExpressions

''Namespace Great_Cow_Graphical_BASIC
	
	Public Class GCBCommandParam
		'1=Type 2=Name 3=Description 4=Tag
		Public Dim ParamType As String
		Public Dim Number As Integer
		Public Dim Name As String
		Public Dim Description As String
		Public Dim Tag As String
	End Class
	
	Public Class GCBCommand
		
		Public Dim Name, Icon As String
	 	Public Dim MatchSyntax, MatchIcon As String
	 	Public Dim Category, HelpPage As String
	 	Public Dim Params As Integer
	 	Public Dim OnlyIn As String
	 	Private Dim pSyntax As String
	 	Private Dim pRegex As Regex
	 	Public Dim Tooltip As String = ""
	 	
	 	Public Sub SetRegex(RegexIn As String)
	 		pRegex = New Regex(RegexIn, RegexOptions.Compiled + _
	 		                            RegexOptions.IgnoreCase + _
	 		                            RegexOptions.IgnorePatternWhitespace + _
	 		                            RegexOptions.ExplicitCapture)
	 	End Sub
	 	
	 	Public ReadOnly Property RegExp As RegEx
	 		Get
	 			Return pRegex
	 		End Get
	 	End Property
	 	
	 	Public Property Syntax As String
	 		Get
	 			Return pSyntax
	 		End Get
	 		Set
	 			pSyntax = value
	 			
	 		End Set
	 	End Property
	 	
	 	Public Dim Param(Mainform.MaxIconParams) As GCBCommandParam
		
		Public Dim Constraints As New List(Of ParamConstraint)
		
		Public Sub New()
		End Sub
		
	End Class
''End Namespace
