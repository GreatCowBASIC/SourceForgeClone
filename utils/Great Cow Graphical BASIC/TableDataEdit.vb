'
' Created by SharpDevelop.
' User: Administrator
' Date: 29/05/2007
' Time: 11:15 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.ComponentModel
Imports System.Windows.Forms
Imports Microsoft.VisualBasic

'Namespace Great_Cow_Graphical_BASIC
	
	Public Class TableDataEdit
		Inherits System.Windows.Forms.UserControl
		Public DataList As System.Windows.Forms.ListBox
		Private DataDelete As System.Windows.Forms.Button
		Private DataAdd As System.Windows.Forms.Button
		Private TableEditHelp As System.Windows.Forms.HelpProvider
		Private NewData As System.Windows.Forms.TextBox
		Private DataUp As System.Windows.Forms.Button
		Private DataDown As System.Windows.Forms.Button
		
		Public Sub New()
			MyBase.New
			' Must be called for initialization
			Me.InitializeComponent
			
			ApplyTranslation
			
		End Sub
		
		#Region " Windows Forms Designer generated code "
		' This method is required for Windows Forms designer support.
		' Do not change the method contents inside the source code editor. The Forms designer might
		' not be able to load this method if it was changed manually.
		Private Sub InitializeComponent()
			Me.DataDown = New System.Windows.Forms.Button
			Me.DataUp = New System.Windows.Forms.Button
			Me.NewData = New System.Windows.Forms.TextBox
			Me.TableEditHelp = New System.Windows.Forms.HelpProvider
			Me.DataAdd = New System.Windows.Forms.Button
			Me.DataDelete = New System.Windows.Forms.Button
			Me.DataList = New System.Windows.Forms.ListBox
			Me.SuspendLayout
			'
			'DataDown
			'
			Me.DataDown.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.TableEditHelp.SetHelpKeyword(Me.DataDown, "Moves the selected item down in the data table.")
			Me.TableEditHelp.SetHelpString(Me.DataDown, "Moves the selected item down in the data table.")
			Me.DataDown.Location = New System.Drawing.Point(136, 40)
			Me.DataDown.Name = "DataDown"
			Me.TableEditHelp.SetShowHelp(Me.DataDown, true)
			Me.DataDown.Size = New System.Drawing.Size(72, 24)
			Me.DataDown.TabIndex = 10
			Me.DataDown.Text = "Move Down"
			AddHandler Me.DataDown.Click, AddressOf Me.DataDownClick
			'
			'DataUp
			'
			Me.DataUp.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.TableEditHelp.SetHelpString(Me.DataUp, "Moves the selected item up in the data table.")
			Me.DataUp.Location = New System.Drawing.Point(136, 8)
			Me.DataUp.Name = "DataUp"
			Me.TableEditHelp.SetShowHelp(Me.DataUp, true)
			Me.DataUp.Size = New System.Drawing.Size(72, 24)
			Me.DataUp.TabIndex = 9
			Me.DataUp.Text = "Move Up"
			AddHandler Me.DataUp.Click, AddressOf Me.DataUpClick
			'
			'NewData
			'
			Me.TableEditHelp.SetHelpString(Me.NewData, "To add an element, type it here and then press ENTER or click Add.")
			Me.NewData.Location = New System.Drawing.Point(8, 224)
			Me.NewData.Name = "NewData"
			Me.TableEditHelp.SetShowHelp(Me.NewData, true)
			Me.NewData.Size = New System.Drawing.Size(120, 20)
			Me.NewData.TabIndex = 7
			Me.NewData.Text = ""
			AddHandler Me.NewData.KeyPress, AddressOf Me.NewDataKeyPress
			'
			'DataAdd
			'
			Me.DataAdd.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.TableEditHelp.SetHelpString(Me.DataAdd, "Adds a new element to the end of the data table.")
			Me.DataAdd.Location = New System.Drawing.Point(136, 224)
			Me.DataAdd.Name = "DataAdd"
			Me.TableEditHelp.SetShowHelp(Me.DataAdd, true)
			Me.DataAdd.Size = New System.Drawing.Size(72, 24)
			Me.DataAdd.TabIndex = 0
			Me.DataAdd.Text = "Add"
			AddHandler Me.DataAdd.Click, AddressOf Me.DataAddClick
			'
			'DataDelete
			'
			Me.DataDelete.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.TableEditHelp.SetHelpString(Me.DataDelete, "Deletes the selected item from the data table.")
			Me.DataDelete.Location = New System.Drawing.Point(136, 184)
			Me.DataDelete.Name = "DataDelete"
			Me.TableEditHelp.SetShowHelp(Me.DataDelete, true)
			Me.DataDelete.Size = New System.Drawing.Size(72, 24)
			Me.DataDelete.TabIndex = 11
			Me.DataDelete.Text = "Delete"
			AddHandler Me.DataDelete.Click, AddressOf Me.DataDeleteClick
			'
			'DataList
			'
			Me.TableEditHelp.SetHelpString(Me.DataList, "The current contents of the data table")
			Me.DataList.Location = New System.Drawing.Point(8, 8)
			Me.DataList.Name = "DataList"
			Me.TableEditHelp.SetShowHelp(Me.DataList, true)
			Me.DataList.Size = New System.Drawing.Size(120, 199)
			Me.DataList.TabIndex = 6
			'
			'TableDataEdit
			'
			Me.Controls.Add(Me.DataList)
			Me.Controls.Add(Me.NewData)
			Me.Controls.Add(Me.DataAdd)
			Me.Controls.Add(Me.DataDelete)
			Me.Controls.Add(Me.DataDown)
			Me.Controls.Add(Me.DataUp)
			Me.Name = "TableDataEdit"
			Me.Size = New System.Drawing.Size(216, 256)
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
		Private Sub ApplyTranslation
			'Apply translation to control
			With MainForm.Translator
				If .CanTranslate Then
					
					
					
				End If
			End With
		End Sub
		
		Private Function ValidateValue(NewEntry As String) As Boolean
			Dim Temp As Integer
			
			'Check that there is an entry
			If NewEntry.Trim = "" Then Return False
			'Check that value is number
			If Not LowLevel.IsValue(NewEntry) Or NewEntry.IndexOf(".") <> -1 Then
				MessageBox.Show("Data Tables can only contain numbers between 0 and 65535", "Great Cow Graphical BASIC", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
				Return False
			End If
			'Check that value is in correct range
			Temp = LowLevel.MakeDec(NewEntry)
			If Temp < -32768 Or Temp > 65535 Then
				MessageBox.Show("Data Tables can only contain numbers between 0 and 65535", "Great Cow Graphical BASIC", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
				Return False
			End If
			
			Return True
		End Function
		
		Private Sub DataAddClick(sender As System.Object, e As System.EventArgs)
			If Not ValidateValue(NewData.Text) Then Exit Sub
			DataList.Items.Add(NewData.Text.Trim)
			DataList.SelectedIndex = DataList.Items.Count - 1
		End Sub
		
		Private Sub DataDeleteClick(sender As System.Object, e As System.EventArgs)
			If DataList.SelectedIndex = -1 Then Exit Sub
			
			Dim Temp As Integer = DataList.SelectedIndex
			DataList.Items.RemoveAt(DataList.SelectedIndex)
			If Temp = DataList.Items.Count Then Temp -= 1
			DataList.SelectedIndex = Temp
			
		End Sub
		
		Private Sub NewDataKeyPress(sender As System.Object, e As System.Windows.Forms.KeyPressEventArgs)
			If e.KeyChar = Chr(13) Then
				e.Handled = True
				NewData.SelectAll()
				
				If Not ValidateValue(NewData.Text) Then Exit Sub
				DataList.Items.Add(NewData.Text.Trim)				
			End If
			
		End Sub
		
		Private Sub DataUpClick(sender As System.Object, e As System.EventArgs)
			If DataList.SelectedIndex < 1 Then Exit Sub
			
			Dim Temp As String
			Temp = DataList.Items(DataList.SelectedIndex - 1)
			DataList.Items(DataList.SelectedIndex - 1) = DataList.Items(DataList.SelectedIndex)
			DataList.Items(DataList.SelectedIndex) = Temp
			DataList.SelectedIndex -= 1
			
		End Sub
		
		Private Sub DataDownClick(sender As System.Object, e As System.EventArgs)
			If DataList.SelectedIndex = -1 Or DataList.SelectedIndex = (DataList.Items.Count - 1) Then Exit Sub
			
			Dim Temp As String
			Temp = DataList.Items(DataList.SelectedIndex + 1)
			DataList.Items(DataList.SelectedIndex + 1) = DataList.Items(DataList.SelectedIndex)
			DataList.Items(DataList.SelectedIndex) = Temp
			DataList.SelectedIndex += 1
			
		End Sub
		
	End Class
'End Namespace
