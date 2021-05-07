'
' Created by SharpDevelop.
' User: Hugh
' Date: 30/09/2012
' Time: 6:28 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Public Partial Class TypeChooserTest
	Public Sub New()
		' The Me.InitializeComponent call is required for Windows Forms designer support.
		Me.InitializeComponent()
		
		comboTest.Add(New CaptionedListItem("BIT", "Bit", "Stores a 0 or a 1"))
		comboTest.Add(New CaptionedListItem("BYTE", "Byte", "Stores a whole number between 0 and 255"))
		
	End Sub
End Class
