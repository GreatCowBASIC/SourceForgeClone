'
' Created by SharpDevelop.
' User: Hugh
' Date: 30/09/2012
' Time: 6:28 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Partial Class TypeChooserTest
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
		Me.comboTest = New Great_Cow_Graphical_BASIC.CaptionedComboBox
		Me.textBox1 = New System.Windows.Forms.TextBox
		Me.SuspendLayout
		'
		'comboTest
		'
		Me.comboTest.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.comboTest.ListFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
		Me.comboTest.Location = New System.Drawing.Point(8, 64)
		Me.comboTest.Name = "comboTest"
		Me.comboTest.SelectedID = ""
		Me.comboTest.SelectedIndex = -1
		Me.comboTest.SelectedObject = Nothing
		Me.comboTest.Size = New System.Drawing.Size(256, 87)
		Me.comboTest.TabIndex = 0
		'
		'textBox1
		'
		Me.textBox1.Location = New System.Drawing.Point(16, 24)
		Me.textBox1.Name = "textBox1"
		Me.textBox1.Size = New System.Drawing.Size(240, 20)
		Me.textBox1.TabIndex = 1
		'
		'TypeChooserTest
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(273, 162)
		Me.Controls.Add(Me.textBox1)
		Me.Controls.Add(Me.comboTest)
		Me.Name = "TypeChooserTest"
		Me.ShowInTaskbar = false
		Me.Text = "Type Chooser Test"
		Me.ResumeLayout(false)
		Me.PerformLayout
	End Sub
	Private textBox1 As System.Windows.Forms.TextBox
	Private comboTest As Great_Cow_Graphical_BASIC.CaptionedComboBox
End Class
