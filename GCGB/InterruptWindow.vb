'
' Created by SharpDevelop.
' User: Hugh
' Date: 3/05/2009
' Time: 7:11 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.Windows.Forms

''Namespace Great_Cow_Graphical_BASIC
	
	Public Class InterruptWindow
		Inherits System.Windows.Forms.Form
		Private ErrorMessage As System.Windows.Forms.Label
		Private ButtonOK As System.Windows.Forms.Button
		Private ButtonCancel As System.Windows.Forms.Button
		Private HandlerPanel As System.Windows.Forms.Panel
		Private groupBox1 As System.Windows.Forms.GroupBox
		
		Private Dim CurrProgram As GCBProgram
		Private Dim IntName() As System.Windows.Forms.Label
		Private Dim IntHandler() As System.Windows.Forms.ComboBox
		
		Public Sub New(CurrProgram As GCBProgram)
			MyBase.New
			
			'
			' The Me.InitializeComponent call is required for Windows Forms designer support.
			'
			Me.InitializeComponent
			
			Me.CurrProgram = CurrProgram
			
			'Check chip data file
			CurrProgram.CheckChipData
			If CurrProgram.ChipData Is Nothing Then
				ErrorMessage.Text = "No chip data file could be loaded for the current chip, and so no interrupts can be displayed"
				ErrorMessage.Visible = True
			Else
				'Chip data file loaded
				'Check that the chip actually has interrupts
				If CurrProgram.ChipData.InterruptList.Count = 0 Then
					'Chip does not have interrupts
					ErrorMessage.Text = "The current chip does not support interrupts." + Environment.NewLine + Environment.NewLine + "If interrupt handlers were defined and the chip model has been changed since, clicking the OK button will erase the handler settings."
					ErrorMessage.Visible = True
				Else
					'Chip does have interrupts
					Dim Subroutine As GCBSubroutine
					Dim TempSubName As String
					Dim ExistingHandler As InterruptHandler
					'Create labels and combo boxes
					Dim IntName(CurrProgram.ChipData.InterruptList.Count - 1) As System.Windows.Forms.Label
					Dim IntHandler(CurrProgram.ChipData.InterruptList.Count - 1) As System.Windows.Forms.ComboBox
					Me.IntName = IntName
					Me.IntHandler = IntHandler
					
					'Row Titles
					Dim TitleIntName As New System.Windows.Forms.Label
					HandlerPanel.Controls.Add(TitleIntName)
					With TitleIntName
						.BackColor = System.Drawing.Color.White
						.Font = New System.Drawing.Font("Microsoft Sans Serif", 8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
						.ForeColor = System.Drawing.Color.Black
						.Location = New System.Drawing.Point(4, 4)
						.Size = New System.Drawing.Size(100, 20)
						.TabIndex = 0
						.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
						.Text = "Interrupt Event"
					End With
					Dim TitleIntHandler As New System.Windows.Forms.Label
					HandlerPanel.Controls.Add(TitleIntHandler)
					With TitleIntHandler
						.BackColor = System.Drawing.Color.White
						.Font = New System.Drawing.Font("Microsoft Sans Serif", 8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
						.ForeColor = System.Drawing.Color.Black
						.Location = New System.Drawing.Point(108, 4)
						.Size = New System.Drawing.Size(156, 20)
						.TabIndex = 0
						.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
						.Text = "Interrupt Handler Sub"
					End With
					
					'Create label and box for every possible interrupt
					Dim HandlerName As String
					Dim ExistingHandlerSub As String
					Dim CurrLoc As Integer
					CurrLoc = 0
					For Each HandlerName In CurrProgram.ChipData.InterruptList
						'Select existing handler sub
						ExistingHandlerSub = ""
						For Each ExistingHandler In CurrProgram.ProgInterrupts
							If ExistingHandler.IntEvent.ToLower = HandlerName.ToLower Then
								ExistingHandlerSub = ExistingHandler.HandlerSub.Trim.ToLower
								Exit For
							End If
						Next
						
						'Show interrupt name
						IntName(CurrLoc) = New System.Windows.Forms.Label
						HandlerPanel.Controls.Add(IntName(CurrLoc))
						With IntName(CurrLoc)
							.BackColor = System.Drawing.Color.White
							.Font = New System.Drawing.Font("Microsoft Sans Serif", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
							.ForeColor = System.Drawing.Color.Black
							.Location = New System.Drawing.Point(4, 34 + CurrLoc * 30)
							.Size = New System.Drawing.Size(100, 20)
							.TabIndex = 0
							.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
							.Text = HandlerName
						End With
						'Show interrupt handler sub
						IntHandler(CurrLoc) = New System.Windows.Forms.ComboBox
						HandlerPanel.Controls.Add(IntHandler(CurrLoc))
						With IntHandler(CurrLoc)
							.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
							.Location = New System.Drawing.Point(108, 34 + CurrLoc * 30)
							.MaxDropDownItems = 10
							.Items.Clear
							.Size = New System.Drawing.Size(156, 20)
							.TabIndex = 0
							.Items.Add("None")
							.SelectedIndex = 0
							
							'Add all subroutines in the program to the list
							For Each Subroutine In CurrProgram.Subroutines
								If Not CurrProgram.IsMainSub(Subroutine) And Subroutine.IsSub Then
									TempSubName = Subroutine.Name
									If TempSubName <> "" Then
										 .Items.Add(TempSubName)
										 If ExistingHandlerSub = TempSubName.Trim.ToLower Then
										 	.SelectedIndex = .Items.Count - 1
										 End If
									End If
								End If
							Next
							
						End With
						
						CurrLoc += 1
					Next
					
				End If
				
			End If
			
		End Sub
		
		#Region " Windows Forms Designer generated code "
		' This method is required for Windows Forms designer support.
		' Do not change the method contents inside the source code editor. The Forms designer might
		' not be able to load this method if it was changed manually.
		Private Sub InitializeComponent()
			Me.groupBox1 = New System.Windows.Forms.GroupBox
			Me.HandlerPanel = New System.Windows.Forms.Panel
			Me.ButtonCancel = New System.Windows.Forms.Button
			Me.ButtonOK = New System.Windows.Forms.Button
			Me.ErrorMessage = New System.Windows.Forms.Label
			Me.groupBox1.SuspendLayout
			Me.HandlerPanel.SuspendLayout
			Me.SuspendLayout
			'
			'groupBox1
			'
			Me.groupBox1.Controls.Add(Me.HandlerPanel)
			Me.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.groupBox1.Location = New System.Drawing.Point(8, 8)
			Me.groupBox1.Name = "groupBox1"
			Me.groupBox1.Size = New System.Drawing.Size(296, 264)
			Me.groupBox1.TabIndex = 2
			Me.groupBox1.TabStop = false
			Me.groupBox1.Text = "Interrupt Handlers"
			'
			'HandlerPanel
			'
			Me.HandlerPanel.AutoScroll = true
			Me.HandlerPanel.BackColor = System.Drawing.SystemColors.Window
			Me.HandlerPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
			Me.HandlerPanel.Controls.Add(Me.ErrorMessage)
			Me.HandlerPanel.Dock = System.Windows.Forms.DockStyle.Fill
			Me.HandlerPanel.DockPadding.All = 4
			Me.HandlerPanel.Location = New System.Drawing.Point(3, 16)
			Me.HandlerPanel.Name = "HandlerPanel"
			Me.HandlerPanel.Size = New System.Drawing.Size(290, 245)
			Me.HandlerPanel.TabIndex = 0
			AddHandler Me.HandlerPanel.MouseEnter, AddressOf Me.HandlerPanelMouseEnter
			'
			'ButtonCancel
			'
			Me.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me.ButtonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.ButtonCancel.Location = New System.Drawing.Point(176, 280)
			Me.ButtonCancel.Name = "ButtonCancel"
			Me.ButtonCancel.Size = New System.Drawing.Size(96, 24)
			Me.ButtonCancel.TabIndex = 1
			Me.ButtonCancel.Text = "Cancel"
			'
			'ButtonOK
			'
			Me.ButtonOK.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.ButtonOK.Location = New System.Drawing.Point(40, 280)
			Me.ButtonOK.Name = "ButtonOK"
			Me.ButtonOK.Size = New System.Drawing.Size(96, 24)
			Me.ButtonOK.TabIndex = 0
			Me.ButtonOK.Text = "OK"
			AddHandler Me.ButtonOK.Click, AddressOf Me.ButtonOKClick
			'
			'ErrorMessage
			'
			Me.ErrorMessage.Dock = System.Windows.Forms.DockStyle.Fill
			Me.ErrorMessage.Location = New System.Drawing.Point(4, 4)
			Me.ErrorMessage.Name = "ErrorMessage"
			Me.ErrorMessage.Size = New System.Drawing.Size(278, 233)
			Me.ErrorMessage.TabIndex = 0
			Me.ErrorMessage.Text = "Error message goes here"
			Me.ErrorMessage.Visible = false
			'
			'InterruptWindow
			'
			Me.AcceptButton = Me.ButtonOK
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.CancelButton = Me.ButtonCancel
			Me.ClientSize = New System.Drawing.Size(314, 312)
			Me.Controls.Add(Me.groupBox1)
			Me.Controls.Add(Me.ButtonCancel)
			Me.Controls.Add(Me.ButtonOK)
			Me.DockPadding.All = 4
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
			Me.MaximizeBox = false
			Me.MinimizeBox = false
			Me.Name = "InterruptWindow"
			Me.ShowInTaskbar = false
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
			Me.Text = "Interrupts"
			Me.groupBox1.ResumeLayout(false)
			Me.HandlerPanel.ResumeLayout(false)
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
		Private Sub HandlerPanelMouseEnter(sender As System.Object, e As System.EventArgs)
			HandlerPanel.Select
			
		End Sub
		
		Private Sub ButtonOKClick(sender As System.Object, e As System.EventArgs)
			
			'Clear existing handler list
			CurrProgram.ProgInterrupts.Clear
			
			'Create new handler list
			'Need to check that interrupt list in window exists
			If Not IntHandler Is Nothing Then
				Dim CurrHandler As Integer
				Dim NewHandler As InterruptHandler
				For CurrHandler = 0 To IntHandler.Length - 1
					If IntHandler(CurrHandler).SelectedIndex <> 0 Then
						NewHandler = New InterruptHandler
						NewHandler.IntEvent = IntName(CurrHandler).Text
						NewHandler.HandlerSub = IntHandler(CurrHandler).SelectedItem
						CurrProgram.ProgInterrupts.Add(NewHandler)
					End If
				Next
			End If
			
			'Set program changed flag and exit
			CurrProgram.HasChanged = True
			Close
		End Sub
		
	End Class
''End Namespace
