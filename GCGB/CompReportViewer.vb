'
' Created by SharpDevelop.
' User: Hugh
' Date: 13/10/2012
' Time: 5:26 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Public Partial Class CompReportViewer
	
	'Name of report file
	Private Dim pReportFile As String
	Public Property ReportFile As String
		Get
			Return pReportFile
		End Get
		Set
			pReportFile = Value
			reportBrowser.Url = New System.Uri(pReportFile)
		End Set
	End Property
	
	Public Sub New()
		' The Me.InitializeComponent call is required for Windows Forms designer support.
		Me.InitializeComponent()
		
		
	End Sub
End Class
