'
' Created by SharpDevelop.
' User: Hugh
' Date: 17/12/2009
' Time: 9:15 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.ComponentModel
Imports System.Windows.Forms

'Namespace Great_Cow_Graphical_BASIC
	
	Public Class PEString
		Inherits System.Windows.Forms.UserControl
		Implements ParamEditPage
		Private FixedStringValue As System.Windows.Forms.TextBox
		Private StringLenDisplay As System.Windows.Forms.Label
		Private groupBox4 As System.Windows.Forms.GroupBox
		
		Dim Frame As ParameterEditor
		
		'Stores the next page ID. Empty string if no next page
		Public ReadOnly Property NextPage As String Implements ParamEditPage.NextPage
			Get
				Return ""
			End Get
		End Property
		
		'Stores the output value for parameter. Empty string if not last page
		Public ReadOnly Property OutputValue As String Implements ParamEditPage.OutputValue
			Get
				Return """" + FixedStringValue.Text + """"
			End Get
		End Property
		
		Public Sub Init (Frame As ParameterEditor, ParamType As String, CurrentSource As String) Implements ParamEditPage.Init
			Me.Frame = Frame
			
			Frame.NextIsFinish = True
			
		End Sub
		
		Public Sub New()
			MyBase.New
			' Must be called for initialization
			Me.InitializeComponent
			
		End Sub
		
		#Region " Windows Forms Designer generated code "
		' This method is required for Windows Forms designer support.
		' Do not change the method contents inside the source code editor. The Forms designer might
		' not be able to load this method if it was changed manually.
		Private Sub InitializeComponent()
			Me.groupBox4 = New System.Windows.Forms.GroupBox
			Me.StringLenDisplay = New System.Windows.Forms.Label
			Me.FixedStringValue = New System.Windows.Forms.TextBox
			Me.groupBox4.SuspendLayout
			Me.SuspendLayout
			'
			'groupBox4
			'
			Me.groupBox4.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.groupBox4.Controls.Add(Me.StringLenDisplay)
			Me.groupBox4.Controls.Add(Me.FixedStringValue)
			Me.groupBox4.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.groupBox4.Location = New System.Drawing.Point(8, 8)
			Me.groupBox4.Name = "groupBox4"
			Me.groupBox4.Size = New System.Drawing.Size(216, 64)
			Me.groupBox4.TabIndex = 1
			Me.groupBox4.TabStop = false
			Me.groupBox4.Text = "Message"
			'
			'StringLenDisplay
			'
			Me.StringLenDisplay.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.StringLenDisplay.Location = New System.Drawing.Point(8, 39)
			Me.StringLenDisplay.Name = "StringLenDisplay"
			Me.StringLenDisplay.Size = New System.Drawing.Size(200, 16)
			Me.StringLenDisplay.TabIndex = 1
			Me.StringLenDisplay.Text = "Current length: 0"
			'
			'FixedStringValue
			'
			Me.FixedStringValue.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.FixedStringValue.Location = New System.Drawing.Point(8, 16)
			Me.FixedStringValue.Name = "FixedStringValue"
			Me.FixedStringValue.Size = New System.Drawing.Size(200, 20)
			Me.FixedStringValue.TabIndex = 0
			Me.FixedStringValue.Text = ""
			AddHandler Me.FixedStringValue.TextChanged, AddressOf Me.FixedStringValueTextChanged
			'
			'PEString
			'
			Me.Controls.Add(Me.groupBox4)
			Me.Name = "PEString"
			Me.Size = New System.Drawing.Size(232, 200)
			Me.groupBox4.ResumeLayout(false)
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
		Private Sub FixedStringValueTextChanged(sender As System.Object, e As System.EventArgs)
			StringLenDisplay.Text = "Current length: " + FixedStringValue.Text.Length.ToString
			
		End Sub
		
	End Class
'End Namespace
