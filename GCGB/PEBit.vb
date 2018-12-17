'
' Created by SharpDevelop.
' User: Hugh
' Date: 13/12/2009
' Time: 1:51 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.ComponentModel
Imports System.Windows.Forms
Imports System.Collections.Generic

'Namespace Great_Cow_Graphical_BASIC
	
	Public Class PEBit
		Inherits System.Windows.Forms.UserControl
		Implements ParamEditPage
		Private sourceConstant As System.Windows.Forms.RadioButton
		Private varChooser As Great_Cow_Graphical_BASIC.ParamEditBox
		Private bitPrompt As System.Windows.Forms.Label
		Private sourceTyped As System.Windows.Forms.RadioButton
		Private constantEntryGroup As System.Windows.Forms.GroupBox
		Private bitSourceGroup As System.Windows.Forms.GroupBox
		Private constList As System.Windows.Forms.ListBox
		Private numberEntryGroup As System.Windows.Forms.GroupBox
		Private bitEntry As System.Windows.Forms.NumericUpDown
		
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
				If sourceTyped.Checked Then
					If varChooser.GetValue <> "" Then
						Return varChooser.GetValue + "." + bitEntry.Value.ToString
					Else
						Return ""
					End If
				Else
					If constList.SelectedIndex <> -1 Then
						Return constList.SelectedItem
					Else
						Return ""
					End If
				End If
			End Get
		End Property
		
		Public Sub Init (Frame As ParameterEditor, ParamType As String, CurrentSource As String) Implements ParamEditPage.Init
			Me.Frame = Frame
			
			Frame.NextIsFinish = True
			Frame.AllowNext = False
			
			'Fill variable list
			varChooser.Init(Frame.Program, Frame.CurrentSub, "Variable", "", "var", "", False)
			
			'Set bit limits
			UpdateBitLimits
			
			'Show existing value
			SetFromString(CurrentSource)
			
			'Populate constant list
			Dim tempComboBox As New ComboBox
			Frame.Program.FillParamList(Frame.CurrentSub, tempComboBox, "bit", "")
			Dim currItem As String
			constList.Sorted = True
			For Each currItem In tempComboBox.Items
				constList.Items.Add(currItem)
				'If this pin is the current one, select it
				If CurrentSource <> "" Then
					If CurrentSource.Trim.ToLower = currItem.Trim.ToLower Then
						'chipPinsList.SelectedIndex = chipPinsList.Items.Count - 1
						constList.SelectedItem = currItem
						SetFromConstName(currItem)
						SourceConstant.Checked = True
					End If
				End If
			Next
			
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
			Me.bitEntry = New System.Windows.Forms.NumericUpDown
			Me.numberEntryGroup = New System.Windows.Forms.GroupBox
			Me.constList = New System.Windows.Forms.ListBox
			Me.bitSourceGroup = New System.Windows.Forms.GroupBox
			Me.constantEntryGroup = New System.Windows.Forms.GroupBox
			Me.sourceTyped = New System.Windows.Forms.RadioButton
			Me.bitPrompt = New System.Windows.Forms.Label
			Me.varChooser = New Great_Cow_Graphical_BASIC.ParamEditBox
			Me.sourceConstant = New System.Windows.Forms.RadioButton
			CType(Me.bitEntry,System.ComponentModel.ISupportInitialize).BeginInit
			Me.numberEntryGroup.SuspendLayout
			Me.bitSourceGroup.SuspendLayout
			Me.constantEntryGroup.SuspendLayout
			Me.SuspendLayout
			'
			'bitEntry
			'
			Me.bitEntry.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.bitEntry.Location = New System.Drawing.Point(328, 48)
			Me.bitEntry.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
			Me.bitEntry.Name = "bitEntry"
			Me.bitEntry.Size = New System.Drawing.Size(112, 20)
			Me.bitEntry.TabIndex = 1
			Me.bitEntry.Value = New Decimal(New Integer() {1, 0, 0, 0})
			AddHandler Me.bitEntry.ValueChanged, AddressOf Me.BitEntryValueChanged
			'
			'numberEntryGroup
			'
			Me.numberEntryGroup.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.numberEntryGroup.Controls.Add(Me.varChooser)
			Me.numberEntryGroup.Controls.Add(Me.bitPrompt)
			Me.numberEntryGroup.Controls.Add(Me.bitEntry)
			Me.numberEntryGroup.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.numberEntryGroup.Location = New System.Drawing.Point(8, 72)
			Me.numberEntryGroup.Name = "numberEntryGroup"
			Me.numberEntryGroup.Size = New System.Drawing.Size(448, 88)
			Me.numberEntryGroup.TabIndex = 6
			Me.numberEntryGroup.TabStop = false
			Me.numberEntryGroup.Text = "Choose a variable bit"
			'
			'constList
			'
			Me.constList.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.constList.Enabled = false
			Me.constList.IntegralHeight = false
			Me.constList.Location = New System.Drawing.Point(8, 24)
			Me.constList.Name = "constList"
			Me.constList.Size = New System.Drawing.Size(432, 104)
			Me.constList.Sorted = true
			Me.constList.TabIndex = 0
			AddHandler Me.constList.SelectedIndexChanged, AddressOf Me.ConstListSelectedIndexChanged
			'
			'bitSourceGroup
			'
			Me.bitSourceGroup.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.bitSourceGroup.Controls.Add(Me.sourceConstant)
			Me.bitSourceGroup.Controls.Add(Me.sourceTyped)
			Me.bitSourceGroup.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.bitSourceGroup.Location = New System.Drawing.Point(8, 8)
			Me.bitSourceGroup.Name = "bitSourceGroup"
			Me.bitSourceGroup.Size = New System.Drawing.Size(448, 56)
			Me.bitSourceGroup.TabIndex = 5
			Me.bitSourceGroup.TabStop = false
			Me.bitSourceGroup.Text = "Bit Source"
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
			Me.constantEntryGroup.Size = New System.Drawing.Size(448, 136)
			Me.constantEntryGroup.TabIndex = 7
			Me.constantEntryGroup.TabStop = false
			Me.constantEntryGroup.Text = "Use a constant"
			'
			'sourceTyped
			'
			Me.sourceTyped.Checked = true
			Me.sourceTyped.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.sourceTyped.Location = New System.Drawing.Point(16, 24)
			Me.sourceTyped.Name = "sourceTyped"
			Me.sourceTyped.Size = New System.Drawing.Size(128, 24)
			Me.sourceTyped.TabIndex = 0
			Me.sourceTyped.TabStop = true
			Me.sourceTyped.Text = "Choose a variable bit"
			AddHandler Me.sourceTyped.CheckedChanged, AddressOf Me.SourceTypedCheckedChanged
			'
			'bitPrompt
			'
			Me.bitPrompt.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.bitPrompt.Location = New System.Drawing.Point(328, 23)
			Me.bitPrompt.Name = "bitPrompt"
			Me.bitPrompt.Size = New System.Drawing.Size(64, 16)
			Me.bitPrompt.TabIndex = 2
			Me.bitPrompt.Text = "Bit:"
			Me.bitPrompt.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
			'
			'varChooser
			'
			Me.varChooser.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.varChooser.BackColor = System.Drawing.SystemColors.Control
			Me.varChooser.HasDescription = false
			Me.varChooser.HasDivider = false
			Me.varChooser.Location = New System.Drawing.Point(8, 23)
			Me.varChooser.Name = "varChooser"
			Me.varChooser.Size = New System.Drawing.Size(296, 56)
			Me.varChooser.TabIndex = 3
			Me.varChooser.TitleFont = New System.Drawing.Font("Tahoma", 11!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World)
			AddHandler Me.varChooser.ParamChanged, AddressOf Me.VarChooserParamChanged
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
			'PEBit
			'
			Me.Controls.Add(Me.constantEntryGroup)
			Me.Controls.Add(Me.numberEntryGroup)
			Me.Controls.Add(Me.bitSourceGroup)
			Me.Name = "PEBit"
			Me.Size = New System.Drawing.Size(464, 312)
			CType(Me.bitEntry,System.ComponentModel.ISupportInitialize).EndInit
			Me.numberEntryGroup.ResumeLayout(false)
			Me.bitSourceGroup.ResumeLayout(false)
			Me.constantEntryGroup.ResumeLayout(false)
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
		Private Sub SetFromString(InData As String)
			'Set variable and bit from a string
			If Not Lowlevel.IsBitConst(InData) Then Exit Sub
			
			Dim VarName, VarBit As String
			VarName = InData.Substring(0, InData.IndexOf("."))
			VarBit = InData.Substring(InData.IndexOf(".") + 1)
			
			Try
				Dim BitNo As Integer = VarBit
				
				'If we get here with no exception, VarBit contains integer
				varChooser.SetValue(VarName, True)
				bitEntry.Value = BitNo
			Catch
			End Try
			
		End Sub
		
		Private Sub SetFromConstName(ConstName As String)
			'Set variable and bit from a constant name
			'Search program
			For Each Constant As KeyValuePair(Of String, String) In Frame.Program.Constants
				If Constant.Key.ToLower = ConstName.Trim.ToLower Then
					SetFromString(Constant.Value)
					Exit Sub
				End If
			Next
			
			'Search libraries
			Dim CheckLib As GCBLibrary
			For Each CheckLib In Frame.Program.Libraries
				For Each Constant As KeyValuePair(Of String, String) In CheckLib.Constants
					If Constant.Key.ToLower = ConstName.Trim.ToLower Then
						SetFromString(Constant.Value)
						Exit Sub
					End If
				Next
			Next
		End Sub
		
		Private Sub UpdateBitLimits
			'Update min/max bit based on type of currently selected variable
			Dim CurrVarType As String = ""
			
			'Search parameters of current sub
			Dim currParam As SubParam
			For Each currParam In Frame.CurrentSub.Parameters
				If currParam.Name.Trim.ToLower = varChooser.GetValue.Trim.ToLower Then
					CurrVarType = currParam.DataType.Trim.ToLower
					Exit For
				End If
			Next
			
			'Search variables
			If CurrVarType = "" Then
				Dim currVar As VariableListItem
				currVar = Frame.Program.Variables.GetVar(varChooser.GetValue)
				If Not currVar Is Nothing Then
					CurrVarType = currVar.Type.Trim.ToLower
				End If
			End If
			
			'Default to byte
			If CurrVarType = "" Then
				CurrVarType = "byte"
			End If
			
			Select Case CurrVarType
				Case "byte":
					bitEntry.Minimum = 0
					bitEntry.Maximum = 7
					
				Case "word", "integer":
					bitEntry.Minimum = 0
					bitEntry.Maximum = 15
					
				Case Else:
					
			End Select
			
		End Sub
		
		Private Sub ConstListSelectedIndexChanged(sender As System.Object, e As System.EventArgs)
			If constList.SelectedIndex <> -1 Then
				Frame.AllowNext = True
				
				'Show pin
				SetFromConstName(constList.SelectedItem)
				
			Else
				Frame.AllowNext = False
			End If
			
		End Sub
		
		Private Sub BitEntryValueChanged(sender As System.Object, e As System.EventArgs)
			
			
		End Sub
		
		Private Sub SourceTypedCheckedChanged(sender As System.Object, e As System.EventArgs)
			If sourceTyped.Checked Then
				Me.varChooser.Enabled = True
				Me.bitEntry.Enabled = True
				
				If varChooser.GetValue <> "" Then
					Frame.AllowNext = True
				End If
				
			Else
				Me.varChooser.Enabled = False
				Me.bitEntry.Enabled = False
			End If
			
		End Sub
		
		Private Sub SourceConstantCheckedChanged(sender As System.Object, e As System.EventArgs)
			If sourceConstant.Checked Then
				constList.Enabled = True
				
				If constList.SelectedIndex <> -1 Then
					Frame.AllowNext = True
				Else
					Frame.AllowNext = False
				End If
			Else
				constList.Enabled = False
			End If
			
			
		End Sub
		
		Private Sub VarChooserParamChanged()
			If sourceTyped.Checked Then
				Frame.AllowNext = True
			End If
			
			UpdateBitLimits
		End Sub
		
	End Class
'End Namespace
