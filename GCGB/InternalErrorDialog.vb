'
' Created by SharpDevelop.
' User: Hugh
' Date: 12/05/2015
' Time: 4:57 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Imports System.Windows.Forms

Public Partial Class InternalErrorDialog
	
	Public Property ErrorMessage As String
		Get
			Return errorList.Text
		End Get
		Set
			errorList.Text = Value
		End Set
	End Property
	
	Public Sub New()
		' The Me.InitializeComponent call is required for Windows Forms designer support.
		Me.InitializeComponent()
		
	End Sub
	
	
	Sub ButtonCopyAndPostClick(sender As Object, e As System.EventArgs)
		'Copy errors
		Clipboard.SetText(errorList.Text)
		
		'Open forum
		System.Diagnostics.Process.Start(MainForm.URL_Forum)
	End Sub
End Class
