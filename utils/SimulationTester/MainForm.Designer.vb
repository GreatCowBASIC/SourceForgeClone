'
' Created by SharpDevelop.
' User: Hugh
' Date: 15/04/2015
' Time: 10:17 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Partial Class MainForm
	Inherits System.Windows.Forms.Form
	
	''' <summary>
	''' Designer variable used to keep track of non-visual components.
	''' </summary>
	Private components As System.ComponentModel.IContainer
	
	''' <summary>
	''' Disposes resources used by the form.
	''' </summary>
	''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
	Protected Overrides Sub Dispose(ByVal disposing As Boolean)
		If disposing Then
			If components IsNot Nothing Then
				components.Dispose()
			End If
		End If
		MyBase.Dispose(disposing)
	End Sub
	
	''' <summary>
	''' This method is required for Windows Forms designer support.
	''' Do not change the method contents inside the source code editor. The Forms designer might
	''' not be able to load this method if it was changed manually.
	''' </summary>
	Private Sub InitializeComponent()
		Me.SelectTests = New System.Windows.Forms.CheckedListBox
		Me.StartTests = New System.Windows.Forms.Button
		Me.CancelTests = New System.Windows.Forms.Button
		Me.bAllTests = New System.Windows.Forms.Button
		Me.bInvertTests = New System.Windows.Forms.Button
		Me.groupBox1 = New System.Windows.Forms.GroupBox
		Me.groupBox2 = New System.Windows.Forms.GroupBox
		Me.SelectTargets = New System.Windows.Forms.CheckedListBox
		Me.bAllTargets = New System.Windows.Forms.Button
		Me.bInvertTargets = New System.Windows.Forms.Button
		Me.tableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
		Me.StatusBox = New System.Windows.Forms.TextBox
		Me.groupBox1.SuspendLayout
		Me.groupBox2.SuspendLayout
		Me.tableLayoutPanel1.SuspendLayout
		Me.SuspendLayout
		'
		'SelectTests
		'
		Me.SelectTests.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.SelectTests.CheckOnClick = true
		Me.SelectTests.FormattingEnabled = true
		Me.SelectTests.IntegralHeight = false
		Me.SelectTests.Location = New System.Drawing.Point(8, 16)
		Me.SelectTests.Name = "SelectTests"
		Me.SelectTests.Size = New System.Drawing.Size(242, 227)
		Me.SelectTests.TabIndex = 0
		'
		'StartTests
		'
		Me.StartTests.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.StartTests.Location = New System.Drawing.Point(312, 301)
		Me.StartTests.Name = "StartTests"
		Me.StartTests.Size = New System.Drawing.Size(104, 23)
		Me.StartTests.TabIndex = 3
		Me.StartTests.Text = "Start Tests ..."
		Me.StartTests.UseVisualStyleBackColor = true
		AddHandler Me.StartTests.Click, AddressOf Me.StartTestsClick
		'
		'CancelTests
		'
		Me.CancelTests.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.CancelTests.Enabled = false
		Me.CancelTests.Location = New System.Drawing.Point(424, 301)
		Me.CancelTests.Name = "CancelTests"
		Me.CancelTests.Size = New System.Drawing.Size(104, 23)
		Me.CancelTests.TabIndex = 4
		Me.CancelTests.Text = "Cancel Tests"
		Me.CancelTests.UseVisualStyleBackColor = true
		AddHandler Me.CancelTests.Click, AddressOf Me.CancelTestsClick
		'
		'bAllTests
		'
		Me.bAllTests.Anchor = System.Windows.Forms.AnchorStyles.Bottom
		Me.bAllTests.Location = New System.Drawing.Point(49, 247)
		Me.bAllTests.Name = "bAllTests"
		Me.bAllTests.Size = New System.Drawing.Size(72, 24)
		Me.bAllTests.TabIndex = 5
		Me.bAllTests.Text = "Select All"
		Me.bAllTests.UseVisualStyleBackColor = true
		AddHandler Me.bAllTests.Click, AddressOf Me.BAllTestsClick
		'
		'bInvertTests
		'
		Me.bInvertTests.Anchor = System.Windows.Forms.AnchorStyles.Bottom
		Me.bInvertTests.Location = New System.Drawing.Point(137, 247)
		Me.bInvertTests.Name = "bInvertTests"
		Me.bInvertTests.Size = New System.Drawing.Size(72, 24)
		Me.bInvertTests.TabIndex = 6
		Me.bInvertTests.Text = "Invert"
		Me.bInvertTests.UseVisualStyleBackColor = true
		AddHandler Me.bInvertTests.Click, AddressOf Me.BInvertTestsClick
		'
		'groupBox1
		'
		Me.groupBox1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.groupBox1.Controls.Add(Me.SelectTests)
		Me.groupBox1.Controls.Add(Me.bAllTests)
		Me.groupBox1.Controls.Add(Me.bInvertTests)
		Me.groupBox1.Location = New System.Drawing.Point(3, 3)
		Me.groupBox1.Name = "groupBox1"
		Me.groupBox1.Size = New System.Drawing.Size(258, 279)
		Me.groupBox1.TabIndex = 7
		Me.groupBox1.TabStop = false
		Me.groupBox1.Text = "Tests to run"
		'
		'groupBox2
		'
		Me.groupBox2.Controls.Add(Me.SelectTargets)
		Me.groupBox2.Controls.Add(Me.bAllTargets)
		Me.groupBox2.Controls.Add(Me.bInvertTargets)
		Me.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill
		Me.groupBox2.Location = New System.Drawing.Point(267, 3)
		Me.groupBox2.Name = "groupBox2"
		Me.groupBox2.Size = New System.Drawing.Size(258, 279)
		Me.groupBox2.TabIndex = 8
		Me.groupBox2.TabStop = false
		Me.groupBox2.Text = "Run tests with"
		'
		'SelectTargets
		'
		Me.SelectTargets.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.SelectTargets.CheckOnClick = true
		Me.SelectTargets.FormattingEnabled = true
		Me.SelectTargets.IntegralHeight = false
		Me.SelectTargets.Location = New System.Drawing.Point(8, 16)
		Me.SelectTargets.Name = "SelectTargets"
		Me.SelectTargets.Size = New System.Drawing.Size(242, 227)
		Me.SelectTargets.TabIndex = 0
		'
		'bAllTargets
		'
		Me.bAllTargets.Anchor = System.Windows.Forms.AnchorStyles.Bottom
		Me.bAllTargets.Location = New System.Drawing.Point(49, 247)
		Me.bAllTargets.Name = "bAllTargets"
		Me.bAllTargets.Size = New System.Drawing.Size(72, 24)
		Me.bAllTargets.TabIndex = 5
		Me.bAllTargets.Text = "Select All"
		Me.bAllTargets.UseVisualStyleBackColor = true
		AddHandler Me.bAllTargets.Click, AddressOf Me.BAllTargetsClick
		'
		'bInvertTargets
		'
		Me.bInvertTargets.Anchor = System.Windows.Forms.AnchorStyles.Bottom
		Me.bInvertTargets.Location = New System.Drawing.Point(137, 247)
		Me.bInvertTargets.Name = "bInvertTargets"
		Me.bInvertTargets.Size = New System.Drawing.Size(72, 24)
		Me.bInvertTargets.TabIndex = 6
		Me.bInvertTargets.Text = "Invert"
		Me.bInvertTargets.UseVisualStyleBackColor = true
		AddHandler Me.bInvertTargets.Click, AddressOf Me.BInvertTargetsClick
		'
		'tableLayoutPanel1
		'
		Me.tableLayoutPanel1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.tableLayoutPanel1.ColumnCount = 2
		Me.tableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50!))
		Me.tableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50!))
		Me.tableLayoutPanel1.Controls.Add(Me.groupBox2, 1, 0)
		Me.tableLayoutPanel1.Controls.Add(Me.groupBox1, 0, 0)
		Me.tableLayoutPanel1.Location = New System.Drawing.Point(8, 8)
		Me.tableLayoutPanel1.Name = "tableLayoutPanel1"
		Me.tableLayoutPanel1.RowCount = 1
		Me.tableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50!))
		Me.tableLayoutPanel1.Size = New System.Drawing.Size(528, 285)
		Me.tableLayoutPanel1.TabIndex = 9
		'
		'StatusBox
		'
		Me.StatusBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.StatusBox.Location = New System.Drawing.Point(8, 336)
		Me.StatusBox.Multiline = true
		Me.StatusBox.Name = "StatusBox"
		Me.StatusBox.ReadOnly = true
		Me.StatusBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
		Me.StatusBox.Size = New System.Drawing.Size(528, 112)
		Me.StatusBox.TabIndex = 10
		'
		'MainForm
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(542, 456)
		Me.Controls.Add(Me.StatusBox)
		Me.Controls.Add(Me.tableLayoutPanel1)
		Me.Controls.Add(Me.CancelTests)
		Me.Controls.Add(Me.StartTests)
		Me.MinimumSize = New System.Drawing.Size(480, 440)
		Me.Name = "MainForm"
		Me.Text = "GCBASIC Simulation Test"
		AddHandler FormClosed, AddressOf Me.MainFormFormClosed
		Me.groupBox1.ResumeLayout(false)
		Me.groupBox2.ResumeLayout(false)
		Me.tableLayoutPanel1.ResumeLayout(false)
		Me.ResumeLayout(false)
		Me.PerformLayout
	End Sub
	Private StatusBox As System.Windows.Forms.TextBox
	Private bInvertTargets As System.Windows.Forms.Button
	Private bAllTargets As System.Windows.Forms.Button
	Private SelectTargets As System.Windows.Forms.CheckedListBox
	Private bInvertTests As System.Windows.Forms.Button
	Private bAllTests As System.Windows.Forms.Button
	Private CancelTests As System.Windows.Forms.Button
	Private StartTests As System.Windows.Forms.Button
	Private SelectTests As System.Windows.Forms.CheckedListBox
	Private tableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
	Private groupBox2 As System.Windows.Forms.GroupBox
	Private groupBox1 As System.Windows.Forms.GroupBox
End Class
