'
' Created by SharpDevelop.
' User: Hugh
' Date: 30/09/2012
' Time: 5:41 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Partial Class CaptionedComboBox
	''' <summary>
	''' Designer variable used to keep track of non-visual components.
	''' </summary>
	Private components As System.ComponentModel.IContainer
	
	''' <summary>
	''' Disposes resources used by the control.
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
		Me.comboDisplay = New System.Windows.Forms.ComboBox
		Me.SuspendLayout
		'
		'comboDisplay
		'
		Me.comboDisplay.Dock = System.Windows.Forms.DockStyle.Fill
		Me.comboDisplay.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable
		Me.comboDisplay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.comboDisplay.FlatStyle = System.Windows.Forms.FlatStyle.System
		Me.comboDisplay.FormattingEnabled = true
		Me.comboDisplay.IntegralHeight = false
		Me.comboDisplay.ItemHeight = 50
		Me.comboDisplay.Location = New System.Drawing.Point(0, 0)
		Me.comboDisplay.Name = "comboDisplay"
		Me.comboDisplay.Size = New System.Drawing.Size(268, 56)
		Me.comboDisplay.TabIndex = 0
		AddHandler Me.comboDisplay.DrawItem, AddressOf Me.ComboDisplayDrawItem
		AddHandler Me.comboDisplay.MeasureItem, AddressOf Me.ComboDisplayMeasureItem
		AddHandler Me.comboDisplay.SelectedIndexChanged, AddressOf Me.ComboDisplaySelectedIndexChanged
		'
		'CaptionedComboBox
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.Controls.Add(Me.comboDisplay)
		Me.Name = "CaptionedComboBox"
		Me.Size = New System.Drawing.Size(268, 81)
		AddHandler Resize, AddressOf Me.CaptionedComboBoxResize
		Me.ResumeLayout(false)
	End Sub
	Private comboDisplay As System.Windows.Forms.ComboBox
End Class
