'
' Created by SharpDevelop.
' User: Hugh
' Date: 13/07/2009
' Time: 10:18 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Collections

Namespace Great_Cow_Graphical_BASIC
	
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
	 	Public Dim FormatList As New ArrayList
	 	Public Dim Tooltip As String = ""
	 	
	 	Public Property Syntax As String
	 		Get
	 			Return pSyntax
	 		End Get
	 		Set
	 			pSyntax = value
	 			
	 			'Get list of contents of command
				Dim InParam As Boolean = False
				Dim CurrLoc As Integer
				Dim CurrElement, CurrChar As String
				CurrElement = ""
				For CurrLoc = 0 To pSyntax.Length - 1
					CurrChar = pSyntax.Substring(CurrLoc, 1)
					If CurrChar = "%" Then
						'% will mark the boundary between param and not param
						'Need to make sure both % before and % after go onto param
						If InParam Then
							FormatList.Add(CurrElement + "%")
							CurrElement = ""
							InParam = False
						Else
							If CurrElement.Replace(" ", "") = "" Then
								CurrElement = " "
							Else
								CurrElement = CurrElement.Trim
							End If
							FormatList.Add(CurrElement)
							CurrElement = "%"
							InParam = True
						End If
					Else
						CurrElement += CurrChar
					End If
				Next
				If CurrElement <> "" Then
					If CurrElement.Replace(" ", "") = "" Then
						CurrElement = " "
					Else
						CurrElement = CurrElement.Trim
					End If
					FormatList.Add(CurrElement)
				End If
	 		End Set
	 	End Property
	 	
	 	Public Dim Param(10) As GCBCommandParam
		
		Public Dim Constraints As New ArrayList
		
		Public Sub New()
		End Sub
		
	End Class
End Namespace
