'
' Created by SharpDevelop.
' User: Administrator
' Date: 8/05/2007
' Time: 9:06 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Data

'Namespace Great_Cow_Graphical_BASIC
	
	Public Class ProgSummary
		Inherits System.Windows.Forms.Form
		Private SummaryStats As System.Windows.Forms.DataGrid
		Private SummarySet As System.Data.DataSet
		
		Dim SummaryDataTable As DataTable
		Dim SummarySubColumn As DataColumn
		Dim SummaryIconsColumn As DataColumn
		
		Dim SummaryRow As DataRow
		Private Dim CurrentFile As GCBProgram
		
		Public Sub New (CurrentFile As GCBProgram)
			MyBase.New
			'
			' The Me.InitializeComponent call is required for Windows Forms designer support.
			'
			Me.InitializeComponent
			
			Me.CurrentFile = CurrentFile
			
			'Get translator
			Dim Translator As LanguageTranslator
			Try
				Translator = MainForm.Translator
			Catch
				Translator = Nothing
			End Try
			
			'Get strings for Subroutine and Icons
			Dim Subroutine, Icons, Summary, Total As String
			If Translator Is Nothing Then
				Subroutine = "Subroutine"
				Icons = "Icons"
				Summary = "Summary"
				Total = "Total"
			Else
				Translator.TryTranslate("ProgSummary", "Title", Me.Text)
				
				Subroutine = Translator.GetMessage("ProgSummary", "Subroutine", "Subroutine")
				Icons = Translator.GetMessage("ProgSummary", "Icons", "Icons")
				Summary = Translator.GetMessage("ProgSummary", "Summary", "Summary")
				Total = Translator.GetMessage("ProgSummary", "Total", "Total")
			End If
			
			SummaryDataTable = New DataTable(Summary)
			SummarySubColumn = New DataColumn(Subroutine)
			SummaryIconsColumn = New DataColumn(Icons)
			
			Dim AddItem As GCBSubroutine
			Dim IconCount As Integer = 0
			Dim NameTemp, SizeTemp As String
			
			'Set up data table
			SummaryDataTable.Columns.Add(SummarySubColumn)
			SummaryDataTable.Columns.Add(SummaryIconsColumn)
			SummarySet.Tables.Add(SummaryDataTable)
			SummaryStats.SetDataBinding(SummarySet, Summary)
						
			'Copy across existing values
			If Not CurrentFile Is Nothing Then
				With CurrentFile
					'For AddItem = 0 To .SubCount
					For Each AddItem In .Subroutines
						NameTemp = AddItem.Name
						SizeTemp = AddItem.Lines.Count
						IconCount += SizeTemp
						
						SummaryRow = SummaryDataTable.NewRow
						SummaryRow.Item(Subroutine) = NameTemp
						SummaryRow.Item(Icons) = SizeTemp
						SummaryDataTable.Rows.Add(SummaryRow)
					Next
				End With
			End If
			
			SummaryRow = SummaryDataTable.NewRow
			SummaryRow.Item(Subroutine) = Total
			SummaryRow.Item(Icons) = IconCount.ToString
			SummaryDataTable.Rows.Add(SummaryRow)
			
		End Sub
		
		#Region " Windows Forms Designer generated code "
		' This method is required for Windows Forms designer support.
		' Do not change the method contents inside the source code editor. The Forms designer might
		' not be able to load this method if it was changed manually.
		Private Sub InitializeComponent()
			Me.SummarySet = New System.Data.DataSet
			Me.SummaryStats = New System.Windows.Forms.DataGrid
			CType(Me.SummarySet,System.ComponentModel.ISupportInitialize).BeginInit
			CType(Me.SummaryStats,System.ComponentModel.ISupportInitialize).BeginInit
			Me.SuspendLayout
			'
			'SummarySet
			'
			Me.SummarySet.DataSetName = "NewDataSet"
			Me.SummarySet.Locale = New System.Globalization.CultureInfo("en-AU")
			'
			'SummaryStats
			'
			Me.SummaryStats.CaptionVisible = false
			Me.SummaryStats.DataMember = ""
			Me.SummaryStats.DataSource = Me.SummarySet
			Me.SummaryStats.HeaderForeColor = System.Drawing.SystemColors.ControlText
			Me.SummaryStats.Location = New System.Drawing.Point(8, 8)
			Me.SummaryStats.Name = "SummaryStats"
			Me.SummaryStats.PreferredColumnWidth = 125
			Me.SummaryStats.ReadOnly = true
			Me.SummaryStats.RowHeadersVisible = false
			Me.SummaryStats.Size = New System.Drawing.Size(280, 248)
			Me.SummaryStats.TabIndex = 0
			'
			'ProgSummary
			'
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.ClientSize = New System.Drawing.Size(292, 266)
			Me.Controls.Add(Me.SummaryStats)
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
			Me.MaximizeBox = false
			Me.MinimizeBox = false
			Me.Name = "ProgSummary"
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
			Me.ShowInTaskbar = False
			Me.Text = "Program Summary"
			CType(Me.SummarySet,System.ComponentModel.ISupportInitialize).EndInit
			CType(Me.SummaryStats,System.ComponentModel.ISupportInitialize).EndInit
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
	End Class
'End Namespace
