'
' Created by SharpDevelop.
' User: Hugh
' Date: 18/12/2012
' Time: 12:09 AM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Partial Class MidiTranslator
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
		Me.buttonOpen = New System.Windows.Forms.Button
		Me.openMidiDialog = New System.Windows.Forms.OpenFileDialog
		Me.outText = New System.Windows.Forms.TextBox
		Me.SuspendLayout
		'
		'buttonOpen
		'
		Me.buttonOpen.Location = New System.Drawing.Point(8, 8)
		Me.buttonOpen.Name = "buttonOpen"
		Me.buttonOpen.Size = New System.Drawing.Size(88, 24)
		Me.buttonOpen.TabIndex = 0
		Me.buttonOpen.Text = "Open ..."
		Me.buttonOpen.UseVisualStyleBackColor = true
		AddHandler Me.buttonOpen.Click, AddressOf Me.ButtonOpenClick
		'
		'openMidiDialog
		'
		Me.openMidiDialog.Filter = "Midi files|*.mid|All Files|*.*"
		Me.openMidiDialog.Title = "Open Midi File"
		AddHandler Me.openMidiDialog.FileOk, AddressOf Me.OpenMidiDialogFileOk
		'
		'outText
		'
		Me.outText.AcceptsReturn = true
		Me.outText.AcceptsTab = true
		Me.outText.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.outText.Location = New System.Drawing.Point(8, 40)
		Me.outText.Multiline = true
		Me.outText.Name = "outText"
		Me.outText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
		Me.outText.Size = New System.Drawing.Size(480, 312)
		Me.outText.TabIndex = 1
		'
		'MidiTranslator
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(498, 362)
		Me.Controls.Add(Me.outText)
		Me.Controls.Add(Me.buttonOpen)
		Me.Name = "MidiTranslator"
		Me.Text = "Midi Translator"
		Me.ResumeLayout(false)
		Me.PerformLayout
	End Sub
	Private outText As System.Windows.Forms.TextBox
	Private openMidiDialog As System.Windows.Forms.OpenFileDialog
	Private buttonOpen As System.Windows.Forms.Button
End Class
