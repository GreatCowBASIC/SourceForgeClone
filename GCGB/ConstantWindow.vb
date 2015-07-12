'
' Created by SharpDevelop.
' User: Hugh
' Date: 19/10/2014
' Time: 12:23 AM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Imports System.Windows.Forms

Public Partial Class ConstantWindow
	Private Dim Program As GCBProgram
	
	Public Sub New(Program As GCBProgram)
		' The Me.InitializeComponent call is required for Windows Forms designer support.
		Me.InitializeComponent()
		
		'Store program
		Me.Program = Program
		
		'Load table with constants
		Dim currConst As Setting
		For Each currConst In Program.Constants
			constantsView.Rows.Add(New String(){currConst.Name, currConst.Value})
		Next
	End Sub
	
	Sub HelpLinkLinkClicked(sender As Object, e As System.Windows.Forms.LinkLabelLinkClickedEventArgs)
		Mainform.MainformInstance.ShowHelpArticle("constants.htm")
	End Sub
	
	Sub OK_ButtonClick(sender As Object, e As System.EventArgs)
		'Store new constants
		With Program
			.Constants.Clear
			
			Dim currRow As DataGridViewRow
			Dim newName, newValue As String
			For Each currRow In constantsView.Rows
				newName = currRow.Cells(0).Value
				newValue = currRow.Cells(1).Value
				If newValue Is Nothing Then
					newValue = ""
				End If
				If newName <> "" Then
					.Constants.Add(New Setting(newName, newValue))
				End If
			Next
			
			.HasChanged = True
			Me.Close
		End With
		
	End Sub
	
	Sub ConstantsViewCellContentClick(sender As Object, e As System.Windows.Forms.DataGridViewCellEventArgs)
		'Detect click of edit button
		If e.ColumnIndex = 2 Then
			'Edit column clicked
			Dim editRow As DataGridViewRow = constantsView.Rows(e.RowIndex)
			Dim constName As String = editRow.Cells(0).Value
			Dim constValue As String = editRow.Cells(1).Value
			If constName <> "" Then
				Dim paramEditor As New ParameterEditor
				paramEditor.Init(Program, constName, "any|long", constValue, Program.Subroutines(0))
				paramEditor.ShowDialog
				
				If paramEditor.Accepted Then
					editRow.Cells(1).Value = paramEditor.ParamValue
				End If
			End If
			
		End If
		
	End Sub
End Class
