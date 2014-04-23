'
' Created by SharpDevelop.
' User: Hugh
' Date: 17/12/2009
' Time: 7:32 PM
'
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.Windows.Forms

''Namespace Great_Cow_Graphical_BASIC
	
	Public Class LiteralConverter
		Inherits System.Windows.Forms.Form
		Private label3 As System.Windows.Forms.Label
		Private label2 As System.Windows.Forms.Label
		Private label1 As System.Windows.Forms.Label
		Private formatInput As System.Windows.Forms.ComboBox
		Private buttonOK As System.Windows.Forms.Button
		Private customBase As System.Windows.Forms.NumericUpDown
		Private buttonCancel As System.Windows.Forms.Button
		Private decimalOutput As System.Windows.Forms.Label
		Private numberInput As System.Windows.Forms.TextBox
		
		Private Dim minVal, maxVal As Long
		
		Public Sub New()
			MyBase.New
			'
			' The Me.InitializeComponent call is required for Windows Forms designer support.
			'
			Me.InitializeComponent
			
			formatInput.SelectedIndex = 0
			
			MinVal = 0
			MaxVal = 65535
			
		End Sub
		
		Private Dim OutVal As Integer
		Private Dim pAccepted As Boolean = False
		
		Public Property Minimum As Long
			Set
				MinVal = Value
			End Set
			Get
				Return MinVal
			End Get
		End Property
		
		Public Property Maximum As Long
			Set
				MaxVal = Value
			End Set
			Get
				Return MaxVal
			End Get
		End Property
		
		Public ReadOnly Property Value As Integer
			Get
				Return OutVal
			End Get
		End Property
		
		Public ReadOnly Property Accepted As Boolean
			Get
				Return pAccepted
			End Get
		End Property
		
		#Region " Windows Forms Designer generated code "
		' This method is required for Windows Forms designer support.
		' Do not change the method contents inside the source code editor. The Forms designer might
		' not be able to load this method if it was changed manually.
		Private Sub InitializeComponent()
			Me.numberInput = New System.Windows.Forms.TextBox
			Me.decimalOutput = New System.Windows.Forms.Label
			Me.buttonCancel = New System.Windows.Forms.Button
			Me.customBase = New System.Windows.Forms.NumericUpDown
			Me.buttonOK = New System.Windows.Forms.Button
			Me.formatInput = New System.Windows.Forms.ComboBox
			Me.label1 = New System.Windows.Forms.Label
			Me.label2 = New System.Windows.Forms.Label
			Me.label3 = New System.Windows.Forms.Label
			CType(Me.customBase,System.ComponentModel.ISupportInitialize).BeginInit
			Me.SuspendLayout
			'
			'numberInput
			'
			Me.numberInput.Location = New System.Drawing.Point(80, 8)
			Me.numberInput.Name = "numberInput"
			Me.numberInput.Size = New System.Drawing.Size(136, 21)
			Me.numberInput.TabIndex = 1
			Me.numberInput.Text = ""
			AddHandler Me.numberInput.TextChanged, AddressOf Me.NumberInputTextChanged
			'
			'decimalOutput
			'
			Me.decimalOutput.Location = New System.Drawing.Point(8, 128)
			Me.decimalOutput.Name = "decimalOutput"
			Me.decimalOutput.Size = New System.Drawing.Size(208, 16)
			Me.decimalOutput.TabIndex = 3
			Me.decimalOutput.Text = "Decimal equivalent:"
			'
			'buttonCancel
			'
			Me.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.buttonCancel.Location = New System.Drawing.Point(120, 160)
			Me.buttonCancel.Name = "buttonCancel"
			Me.buttonCancel.Size = New System.Drawing.Size(96, 24)
			Me.buttonCancel.TabIndex = 8
			Me.buttonCancel.Text = "Cancel"
			'
			'customBase
			'
			Me.customBase.Enabled = false
			Me.customBase.Location = New System.Drawing.Point(80, 72)
			Me.customBase.Maximum = New Decimal(New Integer() {36, 0, 0, 0})
			Me.customBase.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
			Me.customBase.Name = "customBase"
			Me.customBase.Size = New System.Drawing.Size(80, 21)
			Me.customBase.TabIndex = 6
			Me.customBase.Value = New Decimal(New Integer() {1, 0, 0, 0})
			AddHandler Me.customBase.ValueChanged, AddressOf Me.CustomBaseValueChanged
			'
			'buttonOK
			'
			Me.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.buttonOK.Location = New System.Drawing.Point(16, 160)
			Me.buttonOK.Name = "buttonOK"
			Me.buttonOK.Size = New System.Drawing.Size(96, 24)
			Me.buttonOK.TabIndex = 7
			Me.buttonOK.Text = "OK"
			AddHandler Me.buttonOK.Click, AddressOf Me.ButtonOKClick
			'
			'formatInput
			'
			Me.formatInput.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me.formatInput.Items.AddRange(New Object() {"Binary", "Hexadecimal", "ASCII", "Custom ..."})
			Me.formatInput.Location = New System.Drawing.Point(80, 40)
			Me.formatInput.Name = "formatInput"
			Me.formatInput.Size = New System.Drawing.Size(136, 21)
			Me.formatInput.TabIndex = 4
			AddHandler Me.formatInput.SelectedIndexChanged, AddressOf Me.FormatInputSelectedIndexChanged
			'
			'label1
			'
			Me.label1.Location = New System.Drawing.Point(8, 8)
			Me.label1.Name = "label1"
			Me.label1.Size = New System.Drawing.Size(72, 16)
			Me.label1.TabIndex = 0
			Me.label1.Text = "Input value:"
			Me.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
			'
			'label2
			'
			Me.label2.Location = New System.Drawing.Point(8, 40)
			Me.label2.Name = "label2"
			Me.label2.Size = New System.Drawing.Size(72, 23)
			Me.label2.TabIndex = 2
			Me.label2.Text = "Input format:"
			Me.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
			'
			'label3
			'
			Me.label3.Location = New System.Drawing.Point(8, 72)
			Me.label3.Name = "label3"
			Me.label3.Size = New System.Drawing.Size(56, 23)
			Me.label3.TabIndex = 5
			Me.label3.Text = "Base:"
			Me.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
			'
			'LiteralConverter
			'
			Me.AcceptButton = Me.buttonOK
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 14)
			Me.CancelButton = Me.buttonCancel
			Me.ClientSize = New System.Drawing.Size(226, 192)
			Me.Controls.Add(Me.buttonCancel)
			Me.Controls.Add(Me.buttonOK)
			Me.Controls.Add(Me.customBase)
			Me.Controls.Add(Me.label3)
			Me.Controls.Add(Me.formatInput)
			Me.Controls.Add(Me.decimalOutput)
			Me.Controls.Add(Me.label2)
			Me.Controls.Add(Me.numberInput)
			Me.Controls.Add(Me.label1)
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
			Me.MaximizeBox = false
			Me.MinimizeBox = false
			Me.Name = "LiteralConverter"
			Me.ShowInTaskbar = false
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
			Me.Text = "Convert to decimal"
			CType(Me.customBase,System.ComponentModel.ISupportInitialize).EndInit
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
		Private Sub NumberInputTextChanged(sender As System.Object, e As System.EventArgs)
			UpdateConversion
			
		End Sub
		
		Private Sub FormatInputSelectedIndexChanged(sender As System.Object, e As System.EventArgs)
			
			customBase.Enabled = False
			
			Select Case formatInput.SelectedIndex
				Case 0: customBase.Value = 2
				Case 1: customBase.Value = 16
				Case 2: UpdateConversion
				Case 3: customBase.Enabled = True
			End Select
			
		End Sub
		
		Private Sub CustomBaseValueChanged(sender As System.Object, e As System.EventArgs)
			
			UpdateConversion
			
		End Sub
		
		Private Sub UpdateConversion
			
			Dim NewVal As Long
			
			
			If formatInput.SelectedIndex = 2 Then
				If numberInput.Text <> "" Then
					NewVal = Convert.ToInt32(Convert.ToChar(numberInput.Text.Substring(0, 1)))
				Else
					NewVal = -99999
				End If
			Else
				Try
					NewVal = Lowlevel.Other2Dec(numberInput.Text, customBase.Value)
				Catch
					NewVal = -99999
				End Try
			End If
			
			If NewVal <> -99999 Then
				OutVal = NewVal
				decimalOutput.Text = "Decimal equivalent: " + OutVal.ToString
				
				If OutVal < MinVal Or OutVal > MaxVal Then
					buttonOK.Enabled = False
				Else
					buttonOK.Enabled = True
				End If
			End If
		End Sub
		
		Private Sub ButtonOKClick(sender As System.Object, e As System.EventArgs)
			pAccepted = True
			Close
		End Sub
		
	End Class
''End Namespace
