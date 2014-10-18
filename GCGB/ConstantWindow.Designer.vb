'
' Created by SharpDevelop.
' User: Hugh
' Date: 19/10/2014
' Time: 12:23 AM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Partial Class ConstantWindow
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
		Me.constantsView = New System.Windows.Forms.DataGridView
		Me.helpLink = New System.Windows.Forms.LinkLabel
		Me.Cancel_Button = New System.Windows.Forms.Button
		Me.OK_Button = New System.Windows.Forms.Button
		Me.ConstName = New System.Windows.Forms.DataGridViewTextBoxColumn
		Me.ConstValue = New System.Windows.Forms.DataGridViewTextBoxColumn
		Me.ConstEdit = New System.Windows.Forms.DataGridViewButtonColumn
		CType(Me.constantsView,System.ComponentModel.ISupportInitialize).BeginInit
		Me.SuspendLayout
		'
		'constantsView
		'
		Me.constantsView.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.constantsView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
		Me.constantsView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ConstName, Me.ConstValue, Me.ConstEdit})
		Me.constantsView.Location = New System.Drawing.Point(8, 8)
		Me.constantsView.Name = "constantsView"
		Me.constantsView.Size = New System.Drawing.Size(319, 313)
		Me.constantsView.TabIndex = 0
		AddHandler Me.constantsView.CellContentClick, AddressOf Me.ConstantsViewCellContentClick
		'
		'helpLink
		'
		Me.helpLink.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
		Me.helpLink.Location = New System.Drawing.Point(0, 327)
		Me.helpLink.Name = "helpLink"
		Me.helpLink.Size = New System.Drawing.Size(128, 24)
		Me.helpLink.TabIndex = 18
		Me.helpLink.TabStop = true
		Me.helpLink.Text = "What is a constant?"
		Me.helpLink.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		AddHandler Me.helpLink.LinkClicked, AddressOf Me.HelpLinkLinkClicked
		'
		'Cancel_Button
		'
		Me.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.Bottom
		Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
		Me.Cancel_Button.FlatStyle = System.Windows.Forms.FlatStyle.System
		Me.Cancel_Button.Location = New System.Drawing.Point(256, 328)
		Me.Cancel_Button.Name = "Cancel_Button"
		Me.Cancel_Button.Size = New System.Drawing.Size(72, 24)
		Me.Cancel_Button.TabIndex = 17
		Me.Cancel_Button.Text = "Cancel"
		'
		'OK_Button
		'
		Me.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.Bottom
		Me.OK_Button.FlatStyle = System.Windows.Forms.FlatStyle.System
		Me.OK_Button.Location = New System.Drawing.Point(176, 328)
		Me.OK_Button.Name = "OK_Button"
		Me.OK_Button.Size = New System.Drawing.Size(72, 24)
		Me.OK_Button.TabIndex = 16
		Me.OK_Button.Text = "OK"
		AddHandler Me.OK_Button.Click, AddressOf Me.OK_ButtonClick
		'
		'ConstName
		'
		Me.ConstName.HeaderText = "Name"
		Me.ConstName.Name = "ConstName"
		Me.ConstName.ToolTipText = "Name for the constant"
		'
		'ConstValue
		'
		Me.ConstValue.HeaderText = "Value"
		Me.ConstValue.Name = "ConstValue"
		Me.ConstValue.ToolTipText = "Value for the constant"
		'
		'ConstEdit
		'
		Me.ConstEdit.FlatStyle = System.Windows.Forms.FlatStyle.System
		Me.ConstEdit.HeaderText = "Edit"
		Me.ConstEdit.Name = "ConstEdit"
		Me.ConstEdit.Text = "..."
		Me.ConstEdit.ToolTipText = "Use Parameter Editor to choose a value for the constant"
		Me.ConstEdit.UseColumnTextForButtonValue = true
		Me.ConstEdit.Width = 75
		'
		'ConstantWindow
		'
		Me.AcceptButton = Me.OK_Button
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.CancelButton = Me.Cancel_Button
		Me.ClientSize = New System.Drawing.Size(335, 362)
		Me.Controls.Add(Me.helpLink)
		Me.Controls.Add(Me.Cancel_Button)
		Me.Controls.Add(Me.OK_Button)
		Me.Controls.Add(Me.constantsView)
		Me.MaximizeBox = false
		Me.MinimizeBox = false
		Me.MinimumSize = New System.Drawing.Size(351, 200)
		Me.Name = "ConstantWindow"
		Me.ShowIcon = false
		Me.ShowInTaskbar = false
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
		Me.Text = "Constants"
		CType(Me.constantsView,System.ComponentModel.ISupportInitialize).EndInit
		Me.ResumeLayout(false)
	End Sub
	Private constantsView As System.Windows.Forms.DataGridView
	Private OK_Button As System.Windows.Forms.Button
	Private Cancel_Button As System.Windows.Forms.Button
	Private helpLink As System.Windows.Forms.LinkLabel
	Private ConstEdit As System.Windows.Forms.DataGridViewButtonColumn
	Private ConstValue As System.Windows.Forms.DataGridViewTextBoxColumn
	Private ConstName As System.Windows.Forms.DataGridViewTextBoxColumn
End Class
