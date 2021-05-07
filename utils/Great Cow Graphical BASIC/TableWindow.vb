'
' Created by SharpDevelop.
' User: Administrator
' Date: 31/03/2007
' Time: 8:32 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.Windows.Forms

'Namespace Great_Cow_Graphical_BASIC
	
	Public Class TableWindow
		Inherits System.Windows.Forms.Form
		Private TableRename As System.Windows.Forms.Button
		Private ButtonCancel As System.Windows.Forms.Button
		Private ButtonOk As System.Windows.Forms.Button
		Private TableGroup As System.Windows.Forms.GroupBox
		Private TableAdd As System.Windows.Forms.Button
		Private TableHelp As System.Windows.Forms.HelpProvider
		Private TablesTab As System.Windows.Forms.TabControl
		Private TableDelete As System.Windows.Forms.Button
		
		Private TablePage(MainForm.MaxTables) As System.Windows.Forms.TabPage
		Private TableEditor(MainForm.MaxTables) As TableDataEdit
		
		'Private Dim FileID = MainForm.CurrentWindow
		Private Dim CurrentFile As GCBProgram
		Private Dim PageCount  As Integer = 0
		
		Public Sub New(CurrentFile As GCBProgram)
			MyBase.New
			'
			' The Me.InitializeComponent call is required for Windows Forms designer support.
			'
			Me.InitializeComponent
			
			Me.CurrentFile = CurrentFile
			
			InitDisplay
			
		End Sub
		
		#Region " Windows Forms Designer generated code "
		' This method is required for Windows Forms designer support.
		' Do not change the method contents inside the source code editor. The Forms designer might
		' not be able to load this method if it was changed manually.
		Private Sub InitializeComponent()
			Me.TableDelete = New System.Windows.Forms.Button
			Me.TablesTab = New System.Windows.Forms.TabControl
			Me.TableHelp = New System.Windows.Forms.HelpProvider
			Me.TableAdd = New System.Windows.Forms.Button
			Me.TableGroup = New System.Windows.Forms.GroupBox
			Me.ButtonOk = New System.Windows.Forms.Button
			Me.ButtonCancel = New System.Windows.Forms.Button
			Me.TableRename = New System.Windows.Forms.Button
			Me.TableGroup.SuspendLayout
			Me.SuspendLayout
			'
			'TableDelete
			'
			Me.TableDelete.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.TableHelp.SetHelpString(Me.TableDelete, "Deletes the currently displayed data table.")
			Me.TableDelete.Location = New System.Drawing.Point(8, 80)
			Me.TableDelete.Name = "TableDelete"
			Me.TableHelp.SetShowHelp(Me.TableDelete, true)
			Me.TableDelete.Size = New System.Drawing.Size(64, 24)
			Me.TableDelete.TabIndex = 2
			Me.TableDelete.Text = "Delete"
			AddHandler Me.TableDelete.Click, AddressOf Me.TableDeleteClick
			'
			'TablesTab
			'
			Me.TablesTab.Location = New System.Drawing.Point(96, 8)
			Me.TablesTab.Name = "TablesTab"
			Me.TablesTab.SelectedIndex = 0
			Me.TablesTab.Size = New System.Drawing.Size(224, 280)
			Me.TablesTab.TabIndex = 6
			'
			'TableAdd
			'
			Me.TableAdd.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.TableHelp.SetHelpString(Me.TableAdd, "Add a new data table.")
			Me.TableAdd.Location = New System.Drawing.Point(8, 16)
			Me.TableAdd.Name = "TableAdd"
			Me.TableHelp.SetShowHelp(Me.TableAdd, true)
			Me.TableAdd.Size = New System.Drawing.Size(64, 24)
			Me.TableAdd.TabIndex = 1
			Me.TableAdd.Text = "Add ..."
			AddHandler Me.TableAdd.Click, AddressOf Me.TableAddClick
			'
			'TableGroup
			'
			Me.TableGroup.Controls.Add(Me.TableRename)
			Me.TableGroup.Controls.Add(Me.TableDelete)
			Me.TableGroup.Controls.Add(Me.TableAdd)
			Me.TableGroup.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.TableGroup.Location = New System.Drawing.Point(8, 16)
			Me.TableGroup.Name = "TableGroup"
			Me.TableGroup.Size = New System.Drawing.Size(80, 112)
			Me.TableGroup.TabIndex = 2
			Me.TableGroup.TabStop = false
			Me.TableGroup.Text = "Tables"
			'
			'ButtonOk
			'
			Me.ButtonOk.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.ButtonOk.Location = New System.Drawing.Point(72, 296)
			Me.ButtonOk.Name = "ButtonOk"
			Me.ButtonOk.Size = New System.Drawing.Size(80, 24)
			Me.ButtonOk.TabIndex = 4
			Me.ButtonOk.Text = "OK"
			AddHandler Me.ButtonOk.Click, AddressOf Me.ButtonOkClick
			'
			'ButtonCancel
			'
			Me.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me.ButtonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.ButtonCancel.Location = New System.Drawing.Point(176, 296)
			Me.ButtonCancel.Name = "ButtonCancel"
			Me.ButtonCancel.Size = New System.Drawing.Size(80, 24)
			Me.ButtonCancel.TabIndex = 5
			Me.ButtonCancel.Text = "Cancel"
			'
			'TableRename
			'
			Me.TableRename.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.TableHelp.SetHelpString(Me.TableRename, "Renames the currently displayed data table.")
			Me.TableRename.Location = New System.Drawing.Point(8, 48)
			Me.TableRename.Name = "TableRename"
			Me.TableHelp.SetShowHelp(Me.TableRename, true)
			Me.TableRename.Size = New System.Drawing.Size(64, 24)
			Me.TableRename.TabIndex = 3
			Me.TableRename.Text = "Rename..."
			AddHandler Me.TableRename.Click, AddressOf Me.TableRenameClick
			'
			'TableWindow
			'
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.CancelButton = Me.ButtonCancel
			Me.ClientSize = New System.Drawing.Size(330, 327)
			Me.Controls.Add(Me.TablesTab)
			Me.Controls.Add(Me.ButtonCancel)
			Me.Controls.Add(Me.TableGroup)
			Me.Controls.Add(Me.ButtonOk)
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
			Me.HelpButton = true
			Me.MaximizeBox = false
			Me.MinimizeBox = false
			Me.Name = "TableWindow"
			Me.ShowInTaskbar = false
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
			Me.Text = "Data Tables"
			Me.TableGroup.ResumeLayout(false)
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
		Private Sub InitDisplay
			'Add existing tables to editor
			Dim Temp, AddItem As Integer
			For Temp = 1 To CurrentFile.TableCount
				
				'Add page
				AddPage(CurrentFile.ProgTables(Temp, 0))
				
				'Add data to page
				For AddItem = 1 To CurrentFile.TableSize(Temp)
					TableEditor(Temp - 1).DataList.Items.Add(CurrentFile.ProgTables(Temp, AddItem))
				Next
			Next
		End Sub
		
		Private Sub AddPage(PageName As String)
			Me.TablePage(PageCount) = New System.Windows.Forms.TabPage
			
			Me.TablePage(PageCount).Location = New System.Drawing.Point(4, 22)
			Me.TablePage(PageCount).Name = PageName
			Me.TablePage(PageCount).Size = New System.Drawing.Size(216, 254)
			Me.TablePage(PageCount).TabIndex = 0
			Me.TablePage(PageCount).Text = PageName
			Me.TablesTab.Controls.Add(Me.TablePage(PageCount))
			
			TableEditor(PageCount) = New TableDataEdit
			Me.TablePage(PageCount).Controls.Add(TableEditor(PageCount))
			
			PageCount += 1
		End Sub
		
		Private Sub DeletePage(PageLocation As Integer)
			Dim Temp As Integer
			
			Me.TablesTab.Controls.Remove(Me.TablePage(PageLocation))
			
			If PageLocation < (PageCount - 1) Then
				For Temp = PageLocation To PageCount - 2
					Me.TablePage(Temp) = Me.TablePage(Temp + 1)
				Next
			End If
			PageCount -= 1			
		End Sub
			
		Private Sub TableAddClick(sender As System.Object, e As System.EventArgs)
			Dim fNameIn As TableNameInput
			fNameIn = New TableNameInput("Add")
			fNameIn.ShowDialog
			If fNameIn.TableName = "" Then Exit Sub
			
			AddPage(fNameIn.TableName)
		End Sub
		
		Private Sub TableRenameClick(sender As System.Object, e As System.EventArgs)
			If Me.TablesTab.SelectedIndex = -1 Then Exit Sub
			
			Dim fNameIn As TableNameInput
			fNameIn = New TableNameInput("Rename", Me.TablePage(Me.TablesTab.SelectedIndex).Text)
			fNameIn.ShowDialog
			If fNameIn.TableName = "" Then Exit Sub
			
			Me.TablePage(Me.TablesTab.SelectedIndex).Text = fNameIn.TableName
		End Sub
		
		Private Sub TableDeleteClick(sender As System.Object, e As System.EventArgs)
			If Me.TablesTab.SelectedIndex = -1 Then Exit Sub
			
			Dim result As DialogResult = MessageBox.Show("Are you sure you want to delete this data table?", "Great Cow Graphical BASIC", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
			If result = DialogResult.No Then Exit Sub
			
			DeletePage (Me.TablesTab.SelectedIndex)
		End Sub
		
		Private Sub ButtonOkClick(sender As System.Object, e As System.EventArgs)
			Dim Temp, ClearData, ClearSubs As Integer
			
			With CurrentFile
				'Clear old data
				For ClearData = 0 To .TableCount
					For ClearSubs = 0 To .TableSize(ClearData)
						.ProgTables(ClearData, ClearSubs) = ""
					Next
					.TableSize(ClearData) = 0
				Next
				.TableCount = 0
				
				'Add new data
				Dim CopyItem As String
				For Temp = 0 To PageCount - 1
					.TableCount += 1
					.ProgTables(Temp + 1, 0) = Me.TablePage(Temp).Text
					For Each CopyItem In Me.TableEditor(Temp).DataList.Items
						.TableSize(Temp + 1) += 1
						.ProgTables(Temp + 1, .TableSize(Temp + 1)) = CopyItem
					Next
				Next
				
				'Exit
				.HasChanged = True
			
			End With
			Me.Close
			
		End Sub
		
	End Class
'End Namespace
