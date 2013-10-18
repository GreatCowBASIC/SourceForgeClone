'
' Created by SharpDevelop.
' User: Hugh
' Date: 13/12/2009
' Time: 1:47 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.ComponentModel
Imports System.Windows.Forms

Namespace Great_Cow_Graphical_BASIC
	
	Public Class PELiteral
		Inherits System.Windows.Forms.UserControl
		Implements ParamEditPage
		Private sourceConstant As System.Windows.Forms.RadioButton
		Private label1 As System.Windows.Forms.Label
		Private groupBox1 As System.Windows.Forms.GroupBox
		Private numberEntry As System.Windows.Forms.NumericUpDown
		Private constantEntryGroup As System.Windows.Forms.GroupBox
		Private sourceTyped As System.Windows.Forms.RadioButton
		Private convertFromNonDec As System.Windows.Forms.Button
		Private numberEntryGroup As System.Windows.Forms.GroupBox
		Private constList As System.Windows.Forms.ListBox
		
		Dim Frame As ParameterEditor
		Dim OutVal As String
		
		Dim MinVal, MaxVal As Integer
		
		'Stores the next page ID. Empty string if no next page
		Public ReadOnly Property NextPage As String Implements ParamEditPage.NextPage
			Get
				Return ""
			End Get
		End Property
		
		'Stores the output value for parameter. Empty string if not last page
		Public ReadOnly Property OutputValue As String Implements ParamEditPage.OutputValue
			Get
				If sourceTyped.Checked Then
					Return numberEntry.Value.ToString
				Else
					Return OutVal
				End If
			End Get
		End Property
		
		Public Sub Init (Frame As ParameterEditor, ParamType As String, CurrentSource As String) Implements ParamEditPage.Init
			Me.Frame = Frame
			
			'Set min and max
			Dim ThisType As String
			MaxVal = 255
			MinVal = 0
			For Each ThisType In ParamType.Split("|")
				ThisType = ThisType.Trim.ToLower
				If ThisType.EndsWith("byte") Then
					If MaxVal < 255 Then MaxVal = 255
					If MinVal > 0 Then MinVal = 0
				ElseIf ThisType.EndsWith("word") Then
					If MaxVal < 65535 Then MaxVal = 65535
					If MinVal > 0 Then MinVal = 0
				ElseIf ThisType.EndsWith("integer") Then
					If MaxVal < 32767 Then MaxVal = 32767
					If MinVal > -32768 Then MinVal = -32768
				ElseIf ThisType.StartsWith("range") Then
					Dim TempStr As String = ThisType.Substring(5)
					MinVal = TempStr.Substring(0, TempStr.IndexOf("-"))
					MaxVal = TempStr.Substring(TempStr.IndexOf("-") + 1)
				ElseIf ThisType = "number" Then
					If MaxVal < 65535 Then MaxVal = 65535
					If MinVal > -32768 Then MinVal = -32768
				End If
			Next
			
			numberEntry.Maximum = MaxVal
			numberEntry.Minimum = MinVal
			
			'Populate constant list
			Dim tempComboBox As New ComboBox
			Frame.Program.FillParamList(Frame.CurrentSub, tempComboBox, "range" + MinVal.ToString + ":" + MaxVal.ToString, "")
			Dim currItem As String
			constList.Sorted = True
			For Each currItem In tempComboBox.Items
				constList.Items.Add(currItem)
				'If this pin is the current one, select it
				If CurrentSource <> "" Then
					If CurrentSource.Trim.ToLower = currItem.Trim.ToLower Then
						'chipPinsList.SelectedIndex = chipPinsList.Items.Count - 1
						constList.SelectedItem = currItem
					End If
				End If
			Next
			
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
			Me.constList = New System.Windows.Forms.ListBox
			Me.numberEntryGroup = New System.Windows.Forms.GroupBox
			Me.convertFromNonDec = New System.Windows.Forms.Button
			Me.sourceTyped = New System.Windows.Forms.RadioButton
			Me.constantEntryGroup = New System.Windows.Forms.GroupBox
			Me.numberEntry = New System.Windows.Forms.NumericUpDown
			Me.groupBox1 = New System.Windows.Forms.GroupBox
			Me.label1 = New System.Windows.Forms.Label
			Me.sourceConstant = New System.Windows.Forms.RadioButton
			Me.numberEntryGroup.SuspendLayout
			Me.constantEntryGroup.SuspendLayout
			CType(Me.numberEntry,System.ComponentModel.ISupportInitialize).BeginInit
			Me.groupBox1.SuspendLayout
			Me.SuspendLayout
			'
			'constList
			'
			Me.constList.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.constList.Enabled = false
			Me.constList.IntegralHeight = false
			Me.constList.Location = New System.Drawing.Point(8, 23)
			Me.constList.Name = "constList"
			Me.constList.Size = New System.Drawing.Size(264, 49)
			Me.constList.Sorted = true
			Me.constList.TabIndex = 0
			AddHandler Me.constList.SelectedIndexChanged, AddressOf Me.ConstListSelectedIndexChanged
			'
			'numberEntryGroup
			'
			Me.numberEntryGroup.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.numberEntryGroup.Controls.Add(Me.convertFromNonDec)
			Me.numberEntryGroup.Controls.Add(Me.label1)
			Me.numberEntryGroup.Controls.Add(Me.numberEntry)
			Me.numberEntryGroup.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.numberEntryGroup.Location = New System.Drawing.Point(8, 72)
			Me.numberEntryGroup.Name = "numberEntryGroup"
			Me.numberEntryGroup.Size = New System.Drawing.Size(280, 88)
			Me.numberEntryGroup.TabIndex = 3
			Me.numberEntryGroup.TabStop = false
			Me.numberEntryGroup.Text = "Enter a number"
			'
			'convertFromNonDec
			'
			Me.convertFromNonDec.Anchor = System.Windows.Forms.AnchorStyles.Top
			Me.convertFromNonDec.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.convertFromNonDec.Location = New System.Drawing.Point(16, 55)
			Me.convertFromNonDec.Name = "convertFromNonDec"
			Me.convertFromNonDec.Size = New System.Drawing.Size(248, 24)
			Me.convertFromNonDec.TabIndex = 3
			Me.convertFromNonDec.Text = "Convert from binary/hex/ASCII ..."
			AddHandler Me.convertFromNonDec.Click, AddressOf Me.ConvertFromNonDecClick
			'
			'sourceTyped
			'
			Me.sourceTyped.Checked = true
			Me.sourceTyped.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.sourceTyped.Location = New System.Drawing.Point(16, 24)
			Me.sourceTyped.Name = "sourceTyped"
			Me.sourceTyped.Size = New System.Drawing.Size(112, 24)
			Me.sourceTyped.TabIndex = 0
			Me.sourceTyped.TabStop = true
			Me.sourceTyped.Text = "Enter a number"
			AddHandler Me.sourceTyped.CheckedChanged, AddressOf Me.SourceTypedCheckedChanged
			'
			'constantEntryGroup
			'
			Me.constantEntryGroup.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.constantEntryGroup.Controls.Add(Me.constList)
			Me.constantEntryGroup.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.constantEntryGroup.Location = New System.Drawing.Point(8, 168)
			Me.constantEntryGroup.Name = "constantEntryGroup"
			Me.constantEntryGroup.Size = New System.Drawing.Size(280, 80)
			Me.constantEntryGroup.TabIndex = 4
			Me.constantEntryGroup.TabStop = false
			Me.constantEntryGroup.Text = "Use a constant"
			'
			'numberEntry
			'
			Me.numberEntry.Location = New System.Drawing.Point(80, 24)
			Me.numberEntry.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
			Me.numberEntry.Name = "numberEntry"
			Me.numberEntry.Size = New System.Drawing.Size(112, 20)
			Me.numberEntry.TabIndex = 1
			Me.numberEntry.Value = New Decimal(New Integer() {1, 0, 0, 0})
			AddHandler Me.numberEntry.ValueChanged, AddressOf Me.NumberEntryValueChanged
			'
			'groupBox1
			'
			Me.groupBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.groupBox1.Controls.Add(Me.sourceConstant)
			Me.groupBox1.Controls.Add(Me.sourceTyped)
			Me.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.groupBox1.Location = New System.Drawing.Point(8, 8)
			Me.groupBox1.Name = "groupBox1"
			Me.groupBox1.Size = New System.Drawing.Size(280, 56)
			Me.groupBox1.TabIndex = 2
			Me.groupBox1.TabStop = false
			Me.groupBox1.Text = "Number Source"
			'
			'label1
			'
			Me.label1.Location = New System.Drawing.Point(16, 24)
			Me.label1.Name = "label1"
			Me.label1.Size = New System.Drawing.Size(48, 16)
			Me.label1.TabIndex = 2
			Me.label1.Text = "Number:"
			Me.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
			'
			'sourceConstant
			'
			Me.sourceConstant.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.sourceConstant.Location = New System.Drawing.Point(152, 24)
			Me.sourceConstant.Name = "sourceConstant"
			Me.sourceConstant.Size = New System.Drawing.Size(112, 24)
			Me.sourceConstant.TabIndex = 1
			Me.sourceConstant.Text = "Use a constant"
			AddHandler Me.sourceConstant.CheckedChanged, AddressOf Me.SourceConstantCheckedChanged
			'
			'PELiteral
			'
			Me.Controls.Add(Me.constantEntryGroup)
			Me.Controls.Add(Me.numberEntryGroup)
			Me.Controls.Add(Me.groupBox1)
			Me.Name = "PELiteral"
			Me.Size = New System.Drawing.Size(296, 256)
			Me.numberEntryGroup.ResumeLayout(false)
			Me.constantEntryGroup.ResumeLayout(false)
			CType(Me.numberEntry,System.ComponentModel.ISupportInitialize).EndInit
			Me.groupBox1.ResumeLayout(false)
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
		Private Sub ConvertFromNonDecClick(sender As System.Object, e As System.EventArgs)
			Dim decConverter As New LiteralConverter
			decConverter.Minimum = minVal
			decConverter.Maximum = maxVal
			decConverter.ShowDialog
			
			If decConverter.Accepted Then
				numberEntry.Value = decConverter.Value
				OutVal = decConverter.Value.ToString
			End If
			
		End Sub
		
		Private Sub ConstListSelectedIndexChanged(sender As System.Object, e As System.EventArgs)
			Frame.AllowNext = True
			OutVal = ConstList.SelectedItem
		End Sub
		
		Private Sub SourceTypedCheckedChanged(sender As System.Object, e As System.EventArgs)
			If sourceTyped.Checked Then
				convertFromNonDec.Enabled = True
				numberEntry.Enabled = True
				Frame.AllowNext = True
				
				OutVal = numberEntry.Value.ToString
			Else
				convertFromNonDec.Enabled = False
				numberEntry.Enabled = False
			End If
			
		End Sub
		
		Private Sub SourceConstantCheckedChanged(sender As System.Object, e As System.EventArgs)
			If sourceConstant.Checked Then
				constList.Enabled = True
				
				If constList.SelectedIndex <> -1 Then
					OutVal = constList.SelectedItem
					Frame.AllowNext = True
				Else
					Frame.AllowNext = False
				End If
			Else
				constList.Enabled = False
			End If
			
		End Sub
		
		Private Sub NumberEntryValueChanged(sender As System.Object, e As System.EventArgs)
			OutVal = numberEntry.Value.ToString
			
		End Sub
		
	End Class
End Namespace
