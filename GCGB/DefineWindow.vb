'
' Created by SharpDevelop.
' User: Administrator
' Date: 12/09/2006
' Time: 9:57 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Data

''Namespace Great_Cow_Graphical_BASIC
	
	Public Class DefineWindow
		Inherits System.Windows.Forms.Form
		Private ConstGrid As System.Windows.Forms.DataGrid
		Private ConstOK As System.Windows.Forms.Button
		Private ConstCancel As System.Windows.Forms.Button
		Private ConstDataSet As System.Data.DataSet
		
		'Dim FileNo As Integer = MainForm.CurrentWindow
		Dim CurrentFile As GCBProgram

		Dim ConstDataTable As DataTable = New DataTable("Constants")
		Dim ConstNameColumn As DataColumn = New DataColumn("Name")
		Dim ConstValueColumn As DataColumn = New DataColumn("Value")
		
		'Dim ConstRow(MainForm.MaxConstants) As DataRow
		
		Public Sub New(CurrentFile As GCBProgram)
			MyBase.New
			'
			' The Me.InitializeComponent call is required for Windows Forms designer support.
			'
			Me.InitializeComponent
			
			Me.CurrentFile = CurrentFile
			
			'Set up data table
			ConstDataTable.Columns.Add(ConstNameColumn)
			ConstDataTable.Columns.Add(ConstValueColumn)
			ConstDataSet.Tables.Add(ConstDataTable)
			ConstGrid.SetDataBinding(ConstDataSet, "Constants")
			
			'Copy across existing values
			Dim currConst As Setting
			For Each currConst In CurrentFile.Constants
				Dim newRow As DataRow
				newRow = ConstDataTable.NewRow
				newRow.Item("Name") = currConst.Name
				newRow.Item("Value") = currConst.Value
				ConstDataTable.Rows.Add(newRow)
			Next
			
		End Sub
		
		#Region " Windows Forms Designer generated code "
		' This method is required for Windows Forms designer support.
		' Do not change the method contents inside the source code editor. The Forms designer might
		' not be able to load this method if it was changed manually.
		Private Sub InitializeComponent()
			Me.ConstDataSet = New System.Data.DataSet
			Me.ConstCancel = New System.Windows.Forms.Button
			Me.ConstOK = New System.Windows.Forms.Button
			Me.ConstGrid = New System.Windows.Forms.DataGrid
			CType(Me.ConstDataSet,System.ComponentModel.ISupportInitialize).BeginInit
			CType(Me.ConstGrid,System.ComponentModel.ISupportInitialize).BeginInit
			Me.SuspendLayout
			'
			'ConstDataSet
			'
			Me.ConstDataSet.DataSetName = "NewDataSet"
			Me.ConstDataSet.Locale = New System.Globalization.CultureInfo("en")
			'
			'ConstCancel
			'
			Me.ConstCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me.ConstCancel.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.ConstCancel.Location = New System.Drawing.Point(144, 288)
			Me.ConstCancel.Name = "ConstCancel"
			Me.ConstCancel.Size = New System.Drawing.Size(96, 24)
			Me.ConstCancel.TabIndex = 2
			Me.ConstCancel.Text = "Cancel"
			'
			'ConstOK
			'
			Me.ConstOK.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.ConstOK.Location = New System.Drawing.Point(24, 288)
			Me.ConstOK.Name = "ConstOK"
			Me.ConstOK.Size = New System.Drawing.Size(96, 24)
			Me.ConstOK.TabIndex = 1
			Me.ConstOK.Text = "OK"
			AddHandler Me.ConstOK.Click, AddressOf Me.ConstOKClick
			'
			'ConstGrid
			'
			Me.ConstGrid.CaptionVisible = false
			Me.ConstGrid.DataMember = ""
			Me.ConstGrid.DataSource = Me.ConstDataSet
			Me.ConstGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText
			Me.ConstGrid.Location = New System.Drawing.Point(8, 8)
			Me.ConstGrid.Name = "ConstGrid"
			Me.ConstGrid.PreferredColumnWidth = 95
			Me.ConstGrid.RowHeaderWidth = 34
			Me.ConstGrid.Size = New System.Drawing.Size(248, 272)
			Me.ConstGrid.TabIndex = 5
			'
			'DefineWindow
			'
			Me.AcceptButton = Me.ConstOK
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.AutoScroll = true
			Me.CancelButton = Me.ConstCancel
			Me.ClientSize = New System.Drawing.Size(266, 319)
			Me.Controls.Add(Me.ConstGrid)
			Me.Controls.Add(Me.ConstCancel)
			Me.Controls.Add(Me.ConstOK)
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
			Me.MaximizeBox = false
			Me.MinimizeBox = false
			Me.Name = "DefineWindow"
			Me.ShowInTaskbar = false
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
			Me.Text = "Constants"
			CType(Me.ConstDataSet,System.ComponentModel.ISupportInitialize).EndInit
			CType(Me.ConstGrid,System.ComponentModel.ISupportInitialize).EndInit
			Me.ResumeLayout(false)
		End Sub
		#End Region
				
		Private Sub ConstOKClick(sender As System.Object, e As System.EventArgs)
			Dim MoveData As Integer = 0
			Dim ConstCount As Integer = 0
			
			Dim ConstName, ConstValue As String
			
			With CurrentFile
				'Clear old data
				.Constants.Clear
				
				'Get new data
				Dim TempRow As DataRow
				For Each TempRow In ConstDataTable.Rows
					If Not TypeOf TempRow.Item("Name") Is DBNull Then
						ConstName = TempRow.Item("Name")
						If TypeOf TempRow.Item("Value") Is DBNull Then
							ConstValue = ""
						Else
							ConstValue = TempRow.Item("Value")
						End If
						.Constants.Add(New Setting(ConstName, ConstValue))
					End If
				Next
				
				'Set changed flag
				.HasChanged = True
			End With
			
			'Exit
			Me.Close()
			
		End Sub
				
	End Class
''End Namespace
