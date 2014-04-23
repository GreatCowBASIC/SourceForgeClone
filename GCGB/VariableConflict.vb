'
' Created by SharpDevelop.
' User: Hugh
' Date: 21/02/2010
' Time: 3:27 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.Windows.Forms

'Namespace Great_Cow_Graphical_BASIC
	
	Public Class VariableConflict
		Inherits System.Windows.Forms.Form
		Private buttonCancel As System.Windows.Forms.Button
		Private actionPrompt As System.Windows.Forms.Label
		Private buttonRename As System.Windows.Forms.Button
		Private buttonReplace As System.Windows.Forms.Button
		
		Private Dim oldActionPrompt As String
		Private Dim pResolution As Resolution
		
		Public Enum Resolution
			None
			Rename
			Replace
			Cancel
		End Enum
		
		Public Sub New()
			MyBase.New
			'
			' The Me.InitializeComponent call is required for Windows Forms designer support.
			'
			Me.InitializeComponent
			
			ApplyTranslation
			
			pResolution = Resolution.None
			oldActionPrompt = actionPrompt.Text
		End Sub
		
		#Region " Windows Forms Designer generated code "
		' This method is required for Windows Forms designer support.
		' Do not change the method contents inside the source code editor. The Forms designer might
		' not be able to load this method if it was changed manually.
		Private Sub InitializeComponent()
			Me.buttonReplace = New System.Windows.Forms.Button
			Me.buttonRename = New System.Windows.Forms.Button
			Me.actionPrompt = New System.Windows.Forms.Label
			Me.buttonCancel = New System.Windows.Forms.Button
			Me.SuspendLayout
			'
			'buttonReplace
			'
			Me.buttonReplace.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.buttonReplace.Location = New System.Drawing.Point(8, 80)
			Me.buttonReplace.Name = "buttonReplace"
			Me.buttonReplace.Size = New System.Drawing.Size(280, 24)
			Me.buttonReplace.TabIndex = 2
			Me.buttonReplace.Text = "Replace the existing variable with the new one"
			AddHandler Me.buttonReplace.Click, AddressOf Me.ButtonReplaceClick
			'
			'buttonRename
			'
			Me.buttonRename.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.buttonRename.Location = New System.Drawing.Point(8, 48)
			Me.buttonRename.Name = "buttonRename"
			Me.buttonRename.Size = New System.Drawing.Size(280, 24)
			Me.buttonRename.TabIndex = 1
			Me.buttonRename.Text = "Give the new variable a different name"
			AddHandler Me.buttonRename.Click, AddressOf Me.ButtonRenameClick
			'
			'actionPrompt
			'
			Me.actionPrompt.Location = New System.Drawing.Point(8, 8)
			Me.actionPrompt.Name = "actionPrompt"
			Me.actionPrompt.Size = New System.Drawing.Size(280, 32)
			Me.actionPrompt.TabIndex = 0
			Me.actionPrompt.Text = "A variable named %name% already exists. What would you like to do?"
			'
			'buttonCancel
			'
			Me.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.buttonCancel.Location = New System.Drawing.Point(8, 112)
			Me.buttonCancel.Name = "buttonCancel"
			Me.buttonCancel.Size = New System.Drawing.Size(280, 24)
			Me.buttonCancel.TabIndex = 3
			Me.buttonCancel.Text = "Cancel adding the new variable"
			AddHandler Me.buttonCancel.Click, AddressOf Me.ButtonCancelClick
			'
			'VariableConflict
			'
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 14)
			Me.ClientSize = New System.Drawing.Size(292, 144)
			Me.ControlBox = false
			Me.Controls.Add(Me.buttonCancel)
			Me.Controls.Add(Me.buttonReplace)
			Me.Controls.Add(Me.buttonRename)
			Me.Controls.Add(Me.actionPrompt)
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
			Me.MaximizeBox = false
			Me.MinimizeBox = false
			Me.Name = "VariableConflict"
			Me.ShowInTaskbar = false
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
			Me.Text = "Duplicate Variable Name"
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
		Public Sub ApplyTranslation
			
			If Not MainForm.Translator Is Nothing Then
				
			End If
			
		End Sub
		
		Public WriteOnly Property VarName As String
			Set
				actionPrompt.Text = oldActionPrompt.Replace("%name%", Value)
			End Set
		End Property
		
		Public ReadOnly Property Result As Resolution
			Get
				Return pResolution
			End Get
		End Property
		
		Private Sub ButtonRenameClick(sender As System.Object, e As System.EventArgs)
			pResolution = Resolution.Rename
			Me.Close
		End Sub
		
		Private Sub ButtonReplaceClick(sender As System.Object, e As System.EventArgs)
			pResolution = Resolution.Replace
			Me.Close
		End Sub
		
		Private Sub ButtonCancelClick(sender As System.Object, e As System.EventArgs)
			pResolution = Resolution.Cancel
			Me.Close	
		End Sub
		
	End Class
'End Namespace
