'
' Created by SharpDevelop.
' User: Hugh
' Date: 12/05/2015
' Time: 4:57 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Partial Class InternalErrorDialog
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
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(InternalErrorDialog))
		Me.label1 = New System.Windows.Forms.Label
		Me.label2 = New System.Windows.Forms.Label
		Me.errorList = New System.Windows.Forms.TextBox
		Me.label3 = New System.Windows.Forms.Label
		Me.buttonContinue = New System.Windows.Forms.Button
		Me.buttonCopyAndPost = New System.Windows.Forms.Button
		Me.SuspendLayout
		'
		'label1
		'
		Me.label1.Location = New System.Drawing.Point(8, 8)
		Me.label1.Name = "label1"
		Me.label1.Size = New System.Drawing.Size(448, 16)
		Me.label1.TabIndex = 0
		Me.label1.Text = "An internal error has occurred in Great Cow Graphical BASIC."
		'
		'label2
		'
		Me.label2.Location = New System.Drawing.Point(8, 32)
		Me.label2.Name = "label2"
		Me.label2.Size = New System.Drawing.Size(336, 16)
		Me.label2.TabIndex = 1
		Me.label2.Text = "Error details:"
		'
		'errorList
		'
		Me.errorList.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.errorList.Location = New System.Drawing.Point(8, 48)
		Me.errorList.Multiline = true
		Me.errorList.Name = "errorList"
		Me.errorList.ReadOnly = true
		Me.errorList.Size = New System.Drawing.Size(448, 184)
		Me.errorList.TabIndex = 2
		'
		'label3
		'
		Me.label3.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.label3.Location = New System.Drawing.Point(8, 240)
		Me.label3.Name = "label3"
		Me.label3.Size = New System.Drawing.Size(448, 88)
		Me.label3.TabIndex = 3
		Me.label3.Text = resources.GetString("label3.Text")
		'
		'buttonContinue
		'
		Me.buttonContinue.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.buttonContinue.DialogResult = System.Windows.Forms.DialogResult.Cancel
		Me.buttonContinue.Location = New System.Drawing.Point(384, 336)
		Me.buttonContinue.Name = "buttonContinue"
		Me.buttonContinue.Size = New System.Drawing.Size(75, 23)
		Me.buttonContinue.TabIndex = 4
		Me.buttonContinue.Text = "Continue"
		Me.buttonContinue.UseVisualStyleBackColor = true
		'
		'buttonCopyAndPost
		'
		Me.buttonCopyAndPost.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.buttonCopyAndPost.Location = New System.Drawing.Point(88, 336)
		Me.buttonCopyAndPost.Name = "buttonCopyAndPost"
		Me.buttonCopyAndPost.Size = New System.Drawing.Size(288, 24)
		Me.buttonCopyAndPost.TabIndex = 5
		Me.buttonCopyAndPost.Text = "Copy errors to clipboard and visit forum ..."
		Me.buttonCopyAndPost.UseVisualStyleBackColor = true
		AddHandler Me.buttonCopyAndPost.Click, AddressOf Me.ButtonCopyAndPostClick
		'
		'InternalErrorDialog
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.CancelButton = Me.buttonContinue
		Me.ClientSize = New System.Drawing.Size(466, 365)
		Me.Controls.Add(Me.buttonCopyAndPost)
		Me.Controls.Add(Me.buttonContinue)
		Me.Controls.Add(Me.label3)
		Me.Controls.Add(Me.errorList)
		Me.Controls.Add(Me.label2)
		Me.Controls.Add(Me.label1)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
		Me.MaximizeBox = false
		Me.MinimizeBox = false
		Me.Name = "InternalErrorDialog"
		Me.ShowIcon = false
		Me.ShowInTaskbar = false
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
		Me.Text = "Internal Error"
		Me.ResumeLayout(false)
		Me.PerformLayout
	End Sub
	Private buttonCopyAndPost As System.Windows.Forms.Button
	Private buttonContinue As System.Windows.Forms.Button
	Private label3 As System.Windows.Forms.Label
	Private errorList As System.Windows.Forms.TextBox
	Private label2 As System.Windows.Forms.Label
	Private label1 As System.Windows.Forms.Label
End Class
