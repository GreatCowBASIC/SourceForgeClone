'
' Created by SharpDevelop.
' User: Hugh
' Date: 15/06/2012
' Time: 8:56 PM
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
		Me.label1 = New System.Windows.Forms.Label
		Me.InputRTTTL = New System.Windows.Forms.TextBox
		Me.label2 = New System.Windows.Forms.Label
		Me.OutputCode = New System.Windows.Forms.TextBox
		Me.SaveLibrary = New System.Windows.Forms.Button
		Me.saveLibraryChooser = New System.Windows.Forms.SaveFileDialog
		Me.Preview = New System.Windows.Forms.Button
		Me.ReadMidi = New System.Windows.Forms.Button
		Me.SuspendLayout
		'
		'label1
		'
		Me.label1.Location = New System.Drawing.Point(8, 8)
		Me.label1.Name = "label1"
		Me.label1.Size = New System.Drawing.Size(81, 17)
		Me.label1.TabIndex = 0
		Me.label1.Text = "RTTTL Tone"
		'
		'InputRTTTL
		'
		Me.InputRTTTL.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.InputRTTTL.Location = New System.Drawing.Point(8, 24)
		Me.InputRTTTL.Multiline = true
		Me.InputRTTTL.Name = "InputRTTTL"
		Me.InputRTTTL.Size = New System.Drawing.Size(488, 64)
		Me.InputRTTTL.TabIndex = 1
		AddHandler Me.InputRTTTL.TextChanged, AddressOf Me.InputRTTTLTextChanged
		'
		'label2
		'
		Me.label2.Location = New System.Drawing.Point(8, 96)
		Me.label2.Name = "label2"
		Me.label2.Size = New System.Drawing.Size(152, 18)
		Me.label2.TabIndex = 2
		Me.label2.Text = "GCBASIC code preview"
		'
		'OutputCode
		'
		Me.OutputCode.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.OutputCode.BackColor = System.Drawing.SystemColors.Window
		Me.OutputCode.Location = New System.Drawing.Point(8, 112)
		Me.OutputCode.Multiline = true
		Me.OutputCode.Name = "OutputCode"
		Me.OutputCode.ReadOnly = true
		Me.OutputCode.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
		Me.OutputCode.Size = New System.Drawing.Size(488, 264)
		Me.OutputCode.TabIndex = 3
		'
		'SaveLibrary
		'
		Me.SaveLibrary.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.SaveLibrary.Enabled = false
		Me.SaveLibrary.Location = New System.Drawing.Point(328, 384)
		Me.SaveLibrary.Name = "SaveLibrary"
		Me.SaveLibrary.Size = New System.Drawing.Size(168, 24)
		Me.SaveLibrary.TabIndex = 4
		Me.SaveLibrary.Text = "Save code as library ..."
		Me.SaveLibrary.UseVisualStyleBackColor = true
		AddHandler Me.SaveLibrary.Click, AddressOf Me.SaveLibraryClick
		'
		'saveLibraryChooser
		'
		Me.saveLibraryChooser.Filter = "GCBASIC Library (*.h) |*.h|GCBASIC Program (*.gcb)|*.gcb|All Files|*.*"
		Me.saveLibraryChooser.Title = "Save as Library"
		AddHandler Me.saveLibraryChooser.FileOk, AddressOf Me.SaveLibraryChooserFileOk
		'
		'Preview
		'
		Me.Preview.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.Preview.Enabled = false
		Me.Preview.Location = New System.Drawing.Point(224, 384)
		Me.Preview.Name = "Preview"
		Me.Preview.Size = New System.Drawing.Size(96, 24)
		Me.Preview.TabIndex = 5
		Me.Preview.Text = "Play song"
		Me.Preview.UseVisualStyleBackColor = true
		AddHandler Me.Preview.Click, AddressOf Me.PreviewClick
		'
		'ReadMidi
		'
		Me.ReadMidi.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
		Me.ReadMidi.Location = New System.Drawing.Point(8, 384)
		Me.ReadMidi.Name = "ReadMidi"
		Me.ReadMidi.Size = New System.Drawing.Size(96, 24)
		Me.ReadMidi.TabIndex = 6
		Me.ReadMidi.Text = "Read midi ..."
		Me.ReadMidi.UseVisualStyleBackColor = true
		AddHandler Me.ReadMidi.Click, AddressOf Me.ReadMidiClick
		'
		'MainForm
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(505, 417)
		Me.Controls.Add(Me.ReadMidi)
		Me.Controls.Add(Me.Preview)
		Me.Controls.Add(Me.SaveLibrary)
		Me.Controls.Add(Me.OutputCode)
		Me.Controls.Add(Me.label2)
		Me.Controls.Add(Me.InputRTTTL)
		Me.Controls.Add(Me.label1)
		Me.Name = "MainForm"
		Me.Text = "Song Converter"
		Me.ResumeLayout(false)
		Me.PerformLayout
	End Sub
	Private ReadMidi As System.Windows.Forms.Button
	Private Preview As System.Windows.Forms.Button
	Private saveLibraryChooser As System.Windows.Forms.SaveFileDialog
	Private SaveLibrary As System.Windows.Forms.Button
	Private InputRTTTL As System.Windows.Forms.TextBox
	Private OutputCode As System.Windows.Forms.TextBox
	Private label2 As System.Windows.Forms.Label
	Private label1 As System.Windows.Forms.Label
End Class
