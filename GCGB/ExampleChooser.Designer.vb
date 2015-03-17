'
' Created by SharpDevelop.
' User: Hugh
' Date: 8/02/2015
' Time: 11:59 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Partial Class ExampleChooser
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
		Me.label1 = New System.Windows.Forms.Label
		Me.exampleTree = New System.Windows.Forms.TreeView
		Me.dialogSplit = New System.Windows.Forms.SplitContainer
		Me.buttonSearch = New System.Windows.Forms.Button
		Me.tableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
		Me.labelAuthor = New System.Windows.Forms.Label
		Me.labelVersion = New System.Windows.Forms.Label
		Me.LicenceLink = New System.Windows.Forms.LinkLabel
		Me.groupBox2 = New System.Windows.Forms.GroupBox
		Me.connectionsDisplay = New System.Windows.Forms.TextBox
		Me.groupBox1 = New System.Windows.Forms.GroupBox
		Me.writtenFor = New System.Windows.Forms.Label
		Me.labelUseWith = New System.Windows.Forms.Label
		Me.newChipModel = New System.Windows.Forms.ComboBox
		Me.chipModelMessage = New System.Windows.Forms.Label
		Me.summaryDisplay = New System.Windows.Forms.TextBox
		Me.labelExampleName = New System.Windows.Forms.Label
		Me.buttonCancel = New System.Windows.Forms.Button
		Me.buttonOpenExample = New System.Windows.Forms.Button
		Me.dialogSplit.Panel1.SuspendLayout
		Me.dialogSplit.Panel2.SuspendLayout
		Me.dialogSplit.SuspendLayout
		Me.tableLayoutPanel1.SuspendLayout
		Me.groupBox2.SuspendLayout
		Me.groupBox1.SuspendLayout
		Me.SuspendLayout
		'
		'label1
		'
		Me.label1.Location = New System.Drawing.Point(8, 0)
		Me.label1.Name = "label1"
		Me.label1.Size = New System.Drawing.Size(128, 16)
		Me.label1.TabIndex = 0
		Me.label1.Text = "Examples"
		'
		'exampleTree
		'
		Me.exampleTree.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.exampleTree.Location = New System.Drawing.Point(8, 16)
		Me.exampleTree.Name = "exampleTree"
		Me.exampleTree.Size = New System.Drawing.Size(199, 344)
		Me.exampleTree.TabIndex = 1
		AddHandler Me.exampleTree.AfterSelect, AddressOf Me.ExampleTreeAfterSelect
		'
		'dialogSplit
		'
		Me.dialogSplit.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.dialogSplit.Location = New System.Drawing.Point(8, 8)
		Me.dialogSplit.Name = "dialogSplit"
		'
		'dialogSplit.Panel1
		'
		Me.dialogSplit.Panel1.Controls.Add(Me.buttonSearch)
		Me.dialogSplit.Panel1.Controls.Add(Me.exampleTree)
		Me.dialogSplit.Panel1.Controls.Add(Me.label1)
		'
		'dialogSplit.Panel2
		'
		Me.dialogSplit.Panel2.Controls.Add(Me.tableLayoutPanel1)
		Me.dialogSplit.Panel2.Controls.Add(Me.groupBox2)
		Me.dialogSplit.Panel2.Controls.Add(Me.groupBox1)
		Me.dialogSplit.Panel2.Controls.Add(Me.summaryDisplay)
		Me.dialogSplit.Panel2.Controls.Add(Me.labelExampleName)
		Me.dialogSplit.Size = New System.Drawing.Size(640, 400)
		Me.dialogSplit.SplitterDistance = 211
		Me.dialogSplit.TabIndex = 2
		'
		'buttonSearch
		'
		Me.buttonSearch.Anchor = System.Windows.Forms.AnchorStyles.Bottom
		Me.buttonSearch.Location = New System.Drawing.Point(49, 368)
		Me.buttonSearch.Name = "buttonSearch"
		Me.buttonSearch.Size = New System.Drawing.Size(112, 24)
		Me.buttonSearch.TabIndex = 2
		Me.buttonSearch.Text = "Search ..."
		Me.buttonSearch.UseVisualStyleBackColor = true
		'
		'tableLayoutPanel1
		'
		Me.tableLayoutPanel1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.tableLayoutPanel1.ColumnCount = 3
		Me.tableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
		Me.tableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334!))
		Me.tableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334!))
		Me.tableLayoutPanel1.Controls.Add(Me.LicenceLink, 2, 0)
		Me.tableLayoutPanel1.Controls.Add(Me.labelAuthor, 0, 0)
		Me.tableLayoutPanel1.Controls.Add(Me.labelVersion, 1, 0)
		Me.tableLayoutPanel1.Location = New System.Drawing.Point(8, 368)
		Me.tableLayoutPanel1.Name = "tableLayoutPanel1"
		Me.tableLayoutPanel1.RowCount = 1
		Me.tableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100!))
		Me.tableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28!))
		Me.tableLayoutPanel1.Size = New System.Drawing.Size(408, 28)
		Me.tableLayoutPanel1.TabIndex = 14
		'
		'labelAuthor
		'
		Me.labelAuthor.AutoSize = true
		Me.labelAuthor.Dock = System.Windows.Forms.DockStyle.Fill
		Me.labelAuthor.Location = New System.Drawing.Point(3, 0)
		Me.labelAuthor.Name = "labelAuthor"
		Me.labelAuthor.Size = New System.Drawing.Size(129, 28)
		Me.labelAuthor.TabIndex = 2
		Me.labelAuthor.Text = "Author label"
		Me.labelAuthor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		'
		'labelVersion
		'
		Me.labelVersion.AutoSize = true
		Me.labelVersion.Dock = System.Windows.Forms.DockStyle.Fill
		Me.labelVersion.Location = New System.Drawing.Point(138, 0)
		Me.labelVersion.Name = "labelVersion"
		Me.labelVersion.Size = New System.Drawing.Size(130, 28)
		Me.labelVersion.TabIndex = 1
		Me.labelVersion.Text = "Version label"
		Me.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		'
		'LicenceLink
		'
		Me.LicenceLink.AutoSize = true
		Me.LicenceLink.DisabledLinkColor = System.Drawing.Color.Black
		Me.LicenceLink.Dock = System.Windows.Forms.DockStyle.Fill
		Me.LicenceLink.Location = New System.Drawing.Point(274, 0)
		Me.LicenceLink.Name = "LicenceLink"
		Me.LicenceLink.Size = New System.Drawing.Size(131, 28)
		Me.LicenceLink.TabIndex = 3
		Me.LicenceLink.TabStop = true
		Me.LicenceLink.Text = "Licence: Unknown"
		Me.LicenceLink.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		AddHandler Me.LicenceLink.LinkClicked, AddressOf Me.LicenceLinkLinkClicked
		'
		'groupBox2
		'
		Me.groupBox2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.groupBox2.Controls.Add(Me.connectionsDisplay)
		Me.groupBox2.Location = New System.Drawing.Point(200, 232)
		Me.groupBox2.Name = "groupBox2"
		Me.groupBox2.Size = New System.Drawing.Size(216, 136)
		Me.groupBox2.TabIndex = 13
		Me.groupBox2.TabStop = false
		Me.groupBox2.Text = "Required Connections"
		'
		'connectionsDisplay
		'
		Me.connectionsDisplay.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.connectionsDisplay.Location = New System.Drawing.Point(8, 16)
		Me.connectionsDisplay.Multiline = true
		Me.connectionsDisplay.Name = "connectionsDisplay"
		Me.connectionsDisplay.ReadOnly = true
		Me.connectionsDisplay.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
		Me.connectionsDisplay.Size = New System.Drawing.Size(200, 112)
		Me.connectionsDisplay.TabIndex = 11
		'
		'groupBox1
		'
		Me.groupBox1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
		Me.groupBox1.Controls.Add(Me.writtenFor)
		Me.groupBox1.Controls.Add(Me.labelUseWith)
		Me.groupBox1.Controls.Add(Me.newChipModel)
		Me.groupBox1.Controls.Add(Me.chipModelMessage)
		Me.groupBox1.Location = New System.Drawing.Point(8, 232)
		Me.groupBox1.Name = "groupBox1"
		Me.groupBox1.Size = New System.Drawing.Size(184, 136)
		Me.groupBox1.TabIndex = 12
		Me.groupBox1.TabStop = false
		Me.groupBox1.Text = "Microcontroller"
		'
		'writtenFor
		'
		Me.writtenFor.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
		Me.writtenFor.Location = New System.Drawing.Point(8, 24)
		Me.writtenFor.Name = "writtenFor"
		Me.writtenFor.Size = New System.Drawing.Size(144, 16)
		Me.writtenFor.TabIndex = 6
		Me.writtenFor.Text = "Written for chip name"
		'
		'labelUseWith
		'
		Me.labelUseWith.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
		Me.labelUseWith.Location = New System.Drawing.Point(8, 40)
		Me.labelUseWith.Name = "labelUseWith"
		Me.labelUseWith.Size = New System.Drawing.Size(56, 24)
		Me.labelUseWith.TabIndex = 7
		Me.labelUseWith.Text = "Use with"
		Me.labelUseWith.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
		'
		'newChipModel
		'
		Me.newChipModel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
		Me.newChipModel.FormattingEnabled = true
		Me.newChipModel.Location = New System.Drawing.Point(64, 40)
		Me.newChipModel.Name = "newChipModel"
		Me.newChipModel.Size = New System.Drawing.Size(88, 21)
		Me.newChipModel.TabIndex = 8
		AddHandler Me.newChipModel.SelectedIndexChanged, AddressOf Me.NewChipModelSelectedIndexChanged
		AddHandler Me.newChipModel.TextUpdate, AddressOf Me.NewChipModelTextUpdate
		'
		'chipModelMessage
		'
		Me.chipModelMessage.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
		Me.chipModelMessage.Location = New System.Drawing.Point(8, 72)
		Me.chipModelMessage.Name = "chipModelMessage"
		Me.chipModelMessage.Size = New System.Drawing.Size(168, 56)
		Me.chipModelMessage.TabIndex = 9
		Me.chipModelMessage.Text = "Example will work on this chip"
		'
		'summaryDisplay
		'
		Me.summaryDisplay.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.summaryDisplay.Location = New System.Drawing.Point(8, 32)
		Me.summaryDisplay.Multiline = true
		Me.summaryDisplay.Name = "summaryDisplay"
		Me.summaryDisplay.ReadOnly = true
		Me.summaryDisplay.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
		Me.summaryDisplay.Size = New System.Drawing.Size(408, 192)
		Me.summaryDisplay.TabIndex = 5
		'
		'labelExampleName
		'
		Me.labelExampleName.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
		Me.labelExampleName.Location = New System.Drawing.Point(8, 8)
		Me.labelExampleName.Name = "labelExampleName"
		Me.labelExampleName.Size = New System.Drawing.Size(432, 23)
		Me.labelExampleName.TabIndex = 0
		Me.labelExampleName.Text = "Select an example"
		'
		'buttonCancel
		'
		Me.buttonCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
		Me.buttonCancel.Location = New System.Drawing.Point(569, 416)
		Me.buttonCancel.Name = "buttonCancel"
		Me.buttonCancel.Size = New System.Drawing.Size(76, 24)
		Me.buttonCancel.TabIndex = 3
		Me.buttonCancel.Text = "Cancel"
		Me.buttonCancel.UseVisualStyleBackColor = true
		'
		'buttonOpenExample
		'
		Me.buttonOpenExample.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.buttonOpenExample.Location = New System.Drawing.Point(440, 416)
		Me.buttonOpenExample.Name = "buttonOpenExample"
		Me.buttonOpenExample.Size = New System.Drawing.Size(120, 24)
		Me.buttonOpenExample.TabIndex = 4
		Me.buttonOpenExample.Text = "Open Example"
		Me.buttonOpenExample.UseVisualStyleBackColor = true
		AddHandler Me.buttonOpenExample.Click, AddressOf Me.ButtonOpenExampleClick
		'
		'ExampleChooser
		'
		Me.AcceptButton = Me.buttonOpenExample
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.CancelButton = Me.buttonCancel
		Me.ClientSize = New System.Drawing.Size(656, 452)
		Me.Controls.Add(Me.buttonOpenExample)
		Me.Controls.Add(Me.buttonCancel)
		Me.Controls.Add(Me.dialogSplit)
		Me.MaximizeBox = false
		Me.MinimizeBox = false
		Me.Name = "ExampleChooser"
		Me.ShowInTaskbar = false
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
		Me.Text = "Open Example"
		Me.dialogSplit.Panel1.ResumeLayout(false)
		Me.dialogSplit.Panel2.ResumeLayout(false)
		Me.dialogSplit.Panel2.PerformLayout
		Me.dialogSplit.ResumeLayout(false)
		Me.tableLayoutPanel1.ResumeLayout(false)
		Me.tableLayoutPanel1.PerformLayout
		Me.groupBox2.ResumeLayout(false)
		Me.groupBox2.PerformLayout
		Me.groupBox1.ResumeLayout(false)
		Me.ResumeLayout(false)
	End Sub
	Private tableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
	Private groupBox1 As System.Windows.Forms.GroupBox
	Private groupBox2 As System.Windows.Forms.GroupBox
	Private writtenFor As System.Windows.Forms.Label
	Private labelUseWith As System.Windows.Forms.Label
	Private newChipModel As System.Windows.Forms.ComboBox
	Private chipModelMessage As System.Windows.Forms.Label
	Private connectionsDisplay As System.Windows.Forms.TextBox
	Private summaryDisplay As System.Windows.Forms.TextBox
	Private dialogSplit As System.Windows.Forms.SplitContainer
	Private labelVersion As System.Windows.Forms.Label
	Private labelAuthor As System.Windows.Forms.Label
	Private LicenceLink As System.Windows.Forms.LinkLabel
	Private labelExampleName As System.Windows.Forms.Label
	Private buttonOpenExample As System.Windows.Forms.Button
	Private buttonCancel As System.Windows.Forms.Button
	Private buttonSearch As System.Windows.Forms.Button
	Private exampleTree As System.Windows.Forms.TreeView
	Private label1 As System.Windows.Forms.Label
End Class
