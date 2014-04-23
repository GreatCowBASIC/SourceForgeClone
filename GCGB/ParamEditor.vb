'This class replaced by ParameterEditor

Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports Microsoft.VisualBasic

Namespace Great_Cow_Graphical_BASIC
	
	Public Class ParamEditor
		Inherits System.Windows.Forms.Form
		Private label2 As System.Windows.Forms.Label
		Private CompLT As System.Windows.Forms.Button
		Private AddVar As System.Windows.Forms.Button
		Private ParamLiteral As System.Windows.Forms.TabPage
		Private groupBox4 As System.Windows.Forms.GroupBox
		Private groupBox3 As System.Windows.Forms.GroupBox
		Private groupBox2 As System.Windows.Forms.GroupBox
		Private groupBox1 As System.Windows.Forms.GroupBox
		Private AddArray As System.Windows.Forms.Button
		Private ParamEditTabs As System.Windows.Forms.TabControl
		Private CompOr As System.Windows.Forms.Button
		Private ParamVariable As System.Windows.Forms.TabPage
		Private Button_Cancel As System.Windows.Forms.Button
		Private ArrayList As System.Windows.Forms.ListBox
		Private ParamIOPort As System.Windows.Forms.TabPage
		Private [ParamArray] As System.Windows.Forms.TabPage
		Private CompEq As System.Windows.Forms.Button
		Private label1 As System.Windows.Forms.Label
		Private VarList As System.Windows.Forms.ListBox
		Private CondComplex As System.Windows.Forms.RadioButton
		Private StringLenDisplay As System.Windows.Forms.Label
		Private CondOp As System.Windows.Forms.ComboBox
		Private CondSimple As System.Windows.Forms.RadioButton
		Private ParamArrayElement As System.Windows.Forms.TabPage
		Private CompNot As System.Windows.Forms.Button
		Private CompAnd As System.Windows.Forms.Button
		Private CompXor As System.Windows.Forms.Button
		Private CompNE As System.Windows.Forms.Button
		Private CompGE As System.Windows.Forms.Button
		Private ParamConstant As System.Windows.Forms.TabPage
		Private CondParam1 As System.Windows.Forms.ComboBox
		Private ParamCondition As System.Windows.Forms.TabPage
		Private CompLE As System.Windows.Forms.Button
		Private ParamString As System.Windows.Forms.TabPage
		Private CompCondIn As System.Windows.Forms.TextBox
		Private ArrayBox As System.Windows.Forms.GroupBox
		Private ParamFunction As System.Windows.Forms.TabPage
		Private LiteralValue As System.Windows.Forms.NumericUpDown
		Private FixedStringValue As System.Windows.Forms.TextBox
		Private LiteralBase As System.Windows.Forms.NumericUpDown
		Private Button_OK As System.Windows.Forms.Button
		Private LiteralNonDecIn As System.Windows.Forms.TextBox
		Private ParamIOPin As System.Windows.Forms.TabPage
		Private CompGT As System.Windows.Forms.Button
		Private ParamBit As System.Windows.Forms.TabPage
		Private ParamEditorHelp As System.Windows.Forms.HelpProvider
		Private CondParam2 As System.Windows.Forms.ComboBox
		
		Private Dim ParamCondAdded, ParamStrAdded, ParamArrayAdded, ParamIOPortAdded, ParamIOPinAdded As Boolean
		Private Dim ParamBitAdded, ParamLiteralAdded, ParamVarAdded, ParamArrayElementAdded As Boolean
		Private Dim ParamFNAdded, ParamConstAdded As Boolean
		
		Private ParamName, ParamType As String
		Public ParamValue As String = ""
		Public Accepted As Boolean = False
		
		Private CurrentFile As GCBProgram
		
		Public Sub New(CurrentFile As GCBProgram, ParamName As String, ParamType As String, ParamValue As String)
			MyBase.New
			'
			' The Me.InitializeComponent call is required for Windows Forms designer support.
			'
			Me.InitializeComponent
			
			Me.CurrentFile = CurrentFile
			
			InitEditor(ParamName, ParamType, ParamValue)
			
		End Sub
		
		Private Sub InitEditor(ParamName As String, ParamType As String, ParamValue As String)
			'Show only desired tabs
			ParamEditTabs.Controls.Clear
			Dim ThisType As String
			Dim Temp, Count, Location As Integer
			Dim LastOp As String
			For Each ThisType In ParamType.Split("|")
				ThisType = ThisType.Trim.ToLower
								
				'Condition
				If ThisType = "condition" And Not ParamCondAdded Then
					CondDisableAll
					'Get condition type
					Count = 0
					Location = 0
					Do While Location < ParamValue.Length
						Try
							If ParamValue.Substring(Location, 2) = "<>" Or _
							   ParamValue.Substring(Location, 2) = "<=" Or _
							   ParamValue.Substring(Location, 2) = ">=" Then
								LastOp = ParamValue.Substring(Location, 2)
								Count += 1
								Location += 2
							ElseIf ParamValue.Substring(Location, 1) = "=" Or _
							       ParamValue.Substring(Location, 1) = "<" Or _
							       ParamValue.Substring(Location, 1) = ">" Then
							    LastOp = ParamValue.Substring(Location, 1)
							    Count += 1
							    Location += 1
							Else
							    Location += 1
							End If
						Catch
							Exit Do
						End Try
					Loop
					'Load editor with old condition
					If Count = 1 Or ParamValue.Trim = "" Then
						CondSimple.Checked = True
						Try
							Me.CondParam1.Text = ParamValue.Substring(0, ParamValue.IndexOf(LastOp)).Trim
							Me.CondOp.Text = ParamValue.Substring(ParamValue.IndexOf(LastOp), LastOp.Length)
							Me.CondParam2.Text = ParamValue.Substring(ParamValue.IndexOf(LastOp) + LastOp.Length).Trim
						Catch
						End Try
					ElseIf Count > 1 Then
						CondComplex.Checked = True
						Me.CompCondIn.Text = ParamValue.Trim
					End If
					
					Me.ParamEditTabs.Controls.Add(Me.ParamCondition)
					ParamCondAdded = True
				End If
				
				'String
				If ThisType = "string" And Not ParamStrAdded Then
					Me.ParamEditTabs.Controls.Add(Me.ParamString)
					ParamStrAdded = True
					
					If ParamValue.StartsWith(Chr(34)) And ParamValue.EndsWith(Chr(34)) Then
						Me.FixedStringValue.Text = ParamValue.Substring(1, ParamValue.Length - 2)
						Me.ParamString.Select
					End If
					
				End If
				
				'Array
				If ThisType = "array" And Not ParamArrayAdded Then
					
					Dim VarName, VarType As String
					
					With CurrentFile
						For Temp = 1 To .VarCount
							VarName = .ProgVariables(Temp, 1)
							VarType = .ProgVariables(Temp, 2)
							If VarType.IndexOf("(") <> -1 Then
								Me.ArrayList.Items.Add(VarName)
							End If
						Next
					End With
					Me.ArrayList.Sorted = True
					
					Me.ParamEditTabs.Controls.Add(Me.ParamArray)
					ParamArrayAdded = True
					
					If Variables.IsArray(CurrentFile, ParamValue) Then
						Me.ParamEditTabs.SelectedTab = Me.ParamArray
						Me.ArrayList.SelectedItem = ParamValue
					End If
				End If
				
				'IOPin
				If ThisType = "io_pin" And Not ParamIOPinAdded Then
					
					Me.ParamEditTabs.Controls.Add(Me.ParamIOPin)
					ParamIOPinAdded = True
				End If
				
				'IOPort
				If ThisType = "io_port" And Not ParamIOPortAdded Then
					
					Me.ParamEditTabs.Controls.Add(Me.ParamIOPort)
					ParamIOPortAdded = True
				End If
				
				'Bit
				If ThisType = "bit" And Not ParamBitAdded Then
					
					Me.ParamEditTabs.Controls.Add(Me.ParamBit)
					ParamBitAdded = True
				End If
				
				'Literal
				If ThisType = "const" Or ThisType = "const_byte" Or ThisType = "const_word" Or _
				   ThisType = "const_integer" Or ThisType = "integer" Or _
				   ThisType = "number" Or ThisType = "byte" Or ThisType = "word" Or ThisType.StartsWith("range") Then
					
					If Not ParamLiteralAdded Then
						Me.LiteralValue.Maximum = 0
					End If
					
					If ThisType.EndsWith("byte") Then
						With Me.LiteralValue
							If .Maximum < 255 Then .Maximum = 255
							If .Minimum > 0 Then .Minimum = 0
						End With
						
					ElseIf ThisType.EndsWith("word") Then
						With Me.LiteralValue
							If .Maximum < 65535 Then .Maximum = 65535
							If .Minimum > 0 Then .Minimum = 0
						End With
						
					ElseIf ThisType.EndsWith("integer") Then
						With Me.LiteralValue
							If .Maximum < 32767 Then .Maximum = 32767
							If .Minimum > -32768 Then .Minimum = -32768
						End With
						
					ElseIf ThisType.StartsWith("range") Then
						Dim TempStr As String = ThisType.Substring(5)
						With Me.LiteralValue
							.Minimum = TempStr.Substring(0, TempStr.IndexOf("-"))
							.Maximum = TempStr.Substring(TempStr.IndexOf("-") + 1)
						End With
						
					ElseIf ThisType = "number" Then
						With Me.LiteralValue
							If .Maximum < 65535 Then .Maximum = 65535
							If .Minimum > -32768 Then .Minimum = -32768
						End With
					End If
					
					If Not ParamLiteralAdded Then
						Me.ParamEditTabs.Controls.Add(Me.ParamLiteral)
						ParamLiteralAdded = True
					End If
					
					If LowLevel.IsValue(ParamValue) Then
						Try
							Me.LiteralValue.Value = ParamValue
						Catch
							Me.LiteralValue.Value = Me.LiteralValue.Minimum
						End Try
						Me.ParamEditTabs.SelectedTab = Me.ParamLiteral
					End If
					
				End If
				
				'Variable
				If ThisType = "var" Or ThisType = "var_byte" Or ThisType = "var_word" Or _
				   ThisType = "var_integer" Or ThisType = "integer" Or _
				   ThisType = "number" Or ThisType = "byte" Or ThisType = "word" Then
					Dim VarName, VarType As String
					
					With CurrentFile
						For Temp = 1 To .VarCount
							VarName = .ProgVariables(Temp, 1)
							VarType = .ProgVariables(Temp, 2)
							If (ThisType = "var" Or ThisType = "number" Or ThisType.EndsWith(VarType)) And VarType.IndexOf("(") = -1 Then
								Me.VarList.Items.Add(VarName)
							End If
						Next
					End With
					Me.VarList.Sorted = True
					
					If Not ParamVarAdded Then
						Me.ParamEditTabs.Controls.Add(Me.ParamVariable)
						ParamVarAdded = True
					End If
					
					If Variables.IsVariable(CurrentFile, ParamValue) Then
						Me.ParamEditTabs.SelectedTab = Me.ParamVariable
						Me.VarList.SelectedItem = ParamValue
					End If
				End If
				
				'Array Element
				If ThisType = "var" Or ThisType = "var_byte" Or ThisType = "var_word" Or _
				   ThisType = "var_integer" Or ThisType = "integer" Or _
				   ThisType = "number" Or ThisType = "byte" Or ThisType = "word" Then
					
					If Not ParamArrayElementAdded Then
						Me.ParamEditTabs.Controls.Add(Me.ParamArrayElement)
						ParamArrayElementAdded = True
					End If
				End If
				
				'Function
				If ThisType = "byte" Or ThisType = "word" Or ThisType = "number" Or ThisType = "condition" Or _
				   ThisType = "integer" Or ThisType = "string" Then
					
					If Not ParamFNAdded Then
						Me.ParamEditTabs.Controls.Add(Me.ParamFunction)
						ParamFNAdded = True
					End If
				End If
				
				'Constant
				'Use for all
				If Not ParamConstAdded Then
					Me.ParamEditTabs.Controls.Add(Me.ParamConstant)
					ParamConstAdded = True
				End If
				
			Next
			
			Me.ParamName = ParamName
			Me.ParamType = ParamType
			
			Me.Text = Me.Text + ": " + Me.ParamName
			
		End Sub
		
		#Region " Windows Forms Designer generated code "
		' This method is required for Windows Forms designer support.
		' Do not change the method contents inside the source code editor. The Forms designer might
		' not be able to load this method if it was changed manually.
		Private Sub InitializeComponent()
			Me.CondParam2 = New System.Windows.Forms.ComboBox
			Me.ParamEditorHelp = New System.Windows.Forms.HelpProvider
			Me.ParamBit = New System.Windows.Forms.TabPage
			Me.CompGT = New System.Windows.Forms.Button
			Me.ParamIOPin = New System.Windows.Forms.TabPage
			Me.LiteralNonDecIn = New System.Windows.Forms.TextBox
			Me.Button_OK = New System.Windows.Forms.Button
			Me.LiteralBase = New System.Windows.Forms.NumericUpDown
			Me.FixedStringValue = New System.Windows.Forms.TextBox
			Me.LiteralValue = New System.Windows.Forms.NumericUpDown
			Me.ParamFunction = New System.Windows.Forms.TabPage
			Me.ArrayBox = New System.Windows.Forms.GroupBox
			Me.CompCondIn = New System.Windows.Forms.TextBox
			Me.ParamString = New System.Windows.Forms.TabPage
			Me.CompLE = New System.Windows.Forms.Button
			Me.ParamCondition = New System.Windows.Forms.TabPage
			Me.CondParam1 = New System.Windows.Forms.ComboBox
			Me.ParamConstant = New System.Windows.Forms.TabPage
			Me.CompGE = New System.Windows.Forms.Button
			Me.CompNE = New System.Windows.Forms.Button
			Me.CompXor = New System.Windows.Forms.Button
			Me.CompAnd = New System.Windows.Forms.Button
			Me.CompNot = New System.Windows.Forms.Button
			Me.ParamArrayElement = New System.Windows.Forms.TabPage
			Me.CondSimple = New System.Windows.Forms.RadioButton
			Me.CondOp = New System.Windows.Forms.ComboBox
			Me.StringLenDisplay = New System.Windows.Forms.Label
			Me.CondComplex = New System.Windows.Forms.RadioButton
			Me.VarList = New System.Windows.Forms.ListBox
			Me.label1 = New System.Windows.Forms.Label
			Me.CompEq = New System.Windows.Forms.Button
			Me.ParamArray = New System.Windows.Forms.TabPage
			Me.ParamIOPort = New System.Windows.Forms.TabPage
			Me.ArrayList = New System.Windows.Forms.ListBox
			Me.Button_Cancel = New System.Windows.Forms.Button
			Me.ParamVariable = New System.Windows.Forms.TabPage
			Me.CompOr = New System.Windows.Forms.Button
			Me.ParamEditTabs = New System.Windows.Forms.TabControl
			Me.AddArray = New System.Windows.Forms.Button
			Me.groupBox1 = New System.Windows.Forms.GroupBox
			Me.groupBox2 = New System.Windows.Forms.GroupBox
			Me.groupBox3 = New System.Windows.Forms.GroupBox
			Me.groupBox4 = New System.Windows.Forms.GroupBox
			Me.ParamLiteral = New System.Windows.Forms.TabPage
			Me.AddVar = New System.Windows.Forms.Button
			Me.CompLT = New System.Windows.Forms.Button
			Me.label2 = New System.Windows.Forms.Label
			CType(Me.LiteralBase,System.ComponentModel.ISupportInitialize).BeginInit
			CType(Me.LiteralValue,System.ComponentModel.ISupportInitialize).BeginInit
			Me.ArrayBox.SuspendLayout
			Me.ParamString.SuspendLayout
			Me.ParamCondition.SuspendLayout
			Me.ParamArray.SuspendLayout
			Me.ParamVariable.SuspendLayout
			Me.ParamEditTabs.SuspendLayout
			Me.groupBox1.SuspendLayout
			Me.groupBox2.SuspendLayout
			Me.groupBox3.SuspendLayout
			Me.groupBox4.SuspendLayout
			Me.ParamLiteral.SuspendLayout
			Me.SuspendLayout
			'
			'CondParam2
			'
			Me.ParamEditorHelp.SetHelpString(Me.CondParam2, "Second value to compare")
			Me.CondParam2.Location = New System.Drawing.Point(168, 40)
			Me.CondParam2.Name = "CondParam2"
			Me.ParamEditorHelp.SetShowHelp(Me.CondParam2, true)
			Me.CondParam2.Size = New System.Drawing.Size(104, 21)
			Me.CondParam2.TabIndex = 19
			'
			'ParamBit
			'
			Me.ParamBit.Location = New System.Drawing.Point(4, 58)
			Me.ParamBit.Name = "ParamBit"
			Me.ParamBit.Size = New System.Drawing.Size(280, 322)
			Me.ParamBit.TabIndex = 7
			Me.ParamBit.Text = "Bit"
			'
			'CompGT
			'
			Me.CompGT.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.ParamEditorHelp.SetHelpString(Me.CompGT, "Greater Than")
			Me.CompGT.Location = New System.Drawing.Point(184, 160)
			Me.CompGT.Name = "CompGT"
			Me.ParamEditorHelp.SetShowHelp(Me.CompGT, true)
			Me.CompGT.Size = New System.Drawing.Size(40, 20)
			Me.CompGT.TabIndex = 30
			Me.CompGT.Text = ">"
			AddHandler Me.CompGT.Click, AddressOf Me.CondCompGTClick
			'
			'ParamIOPin
			'
			Me.ParamIOPin.Location = New System.Drawing.Point(4, 58)
			Me.ParamIOPin.Name = "ParamIOPin"
			Me.ParamIOPin.Size = New System.Drawing.Size(280, 322)
			Me.ParamIOPin.TabIndex = 5
			Me.ParamIOPin.Text = "I/O Pin"
			'
			'LiteralNonDecIn
			'
			Me.LiteralNonDecIn.Location = New System.Drawing.Point(56, 24)
			Me.LiteralNonDecIn.Name = "LiteralNonDecIn"
			Me.LiteralNonDecIn.Size = New System.Drawing.Size(192, 20)
			Me.LiteralNonDecIn.TabIndex = 0
			Me.LiteralNonDecIn.Text = ""
			AddHandler Me.LiteralNonDecIn.TextChanged, AddressOf Me.LiteralNonDecInTextChanged
			'
			'Button_OK
			'
			Me.Button_OK.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.Button_OK.Location = New System.Drawing.Point(48, 400)
			Me.Button_OK.Name = "Button_OK"
			Me.Button_OK.Size = New System.Drawing.Size(88, 24)
			Me.Button_OK.TabIndex = 0
			Me.Button_OK.Text = "OK"
			AddHandler Me.Button_OK.Click, AddressOf Me.Button_OKClick
			'
			'LiteralBase
			'
			Me.LiteralBase.Location = New System.Drawing.Point(56, 56)
			Me.LiteralBase.Maximum = New Decimal(New Integer() {36, 0, 0, 0})
			Me.LiteralBase.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
			Me.LiteralBase.Name = "LiteralBase"
			Me.LiteralBase.Size = New System.Drawing.Size(48, 20)
			Me.LiteralBase.TabIndex = 4
			Me.LiteralBase.Value = New Decimal(New Integer() {16, 0, 0, 0})
			AddHandler Me.LiteralBase.ValueChanged, AddressOf Me.LiteralBaseValueChanged
			'
			'FixedStringValue
			'
			Me.ParamEditorHelp.SetHelpString(Me.FixedStringValue, "Exact message that the program will use")
			Me.FixedStringValue.Location = New System.Drawing.Point(8, 16)
			Me.FixedStringValue.Name = "FixedStringValue"
			Me.ParamEditorHelp.SetShowHelp(Me.FixedStringValue, true)
			Me.FixedStringValue.Size = New System.Drawing.Size(248, 20)
			Me.FixedStringValue.TabIndex = 0
			Me.FixedStringValue.Text = ""
			AddHandler Me.FixedStringValue.TextChanged, AddressOf Me.FixedStringValueTextChanged
			'
			'LiteralValue
			'
			Me.LiteralValue.Location = New System.Drawing.Point(80, 16)
			Me.LiteralValue.Maximum = New Decimal(New Integer() {65535, 0, 0, 0})
			Me.LiteralValue.Name = "LiteralValue"
			Me.LiteralValue.Size = New System.Drawing.Size(104, 20)
			Me.LiteralValue.TabIndex = 0
			AddHandler Me.LiteralValue.ValueChanged, AddressOf Me.LiteralValueValueChanged
			'
			'ParamFunction
			'
			Me.ParamFunction.Location = New System.Drawing.Point(4, 58)
			Me.ParamFunction.Name = "ParamFunction"
			Me.ParamFunction.Size = New System.Drawing.Size(280, 322)
			Me.ParamFunction.TabIndex = 8
			Me.ParamFunction.Text = "Function"
			'
			'ArrayBox
			'
			Me.ArrayBox.Controls.Add(Me.ArrayList)
			Me.ArrayBox.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.ArrayBox.Location = New System.Drawing.Point(8, 7)
			Me.ArrayBox.Name = "ArrayBox"
			Me.ArrayBox.Size = New System.Drawing.Size(264, 273)
			Me.ArrayBox.TabIndex = 2
			Me.ArrayBox.TabStop = false
			Me.ArrayBox.Text = "Arrays"
			'
			'CompCondIn
			'
			Me.ParamEditorHelp.SetHelpString(Me.CompCondIn, "Enter the complex condition here")
			Me.CompCondIn.Location = New System.Drawing.Point(16, 104)
			Me.CompCondIn.Name = "CompCondIn"
			Me.ParamEditorHelp.SetShowHelp(Me.CompCondIn, true)
			Me.CompCondIn.Size = New System.Drawing.Size(256, 20)
			Me.CompCondIn.TabIndex = 20
			Me.CompCondIn.Text = ""
			'
			'ParamString
			'
			Me.ParamString.Controls.Add(Me.groupBox4)
			Me.ParamString.Location = New System.Drawing.Point(4, 58)
			Me.ParamString.Name = "ParamString"
			Me.ParamString.Size = New System.Drawing.Size(280, 322)
			Me.ParamString.TabIndex = 4
			Me.ParamString.Text = "String"
			'
			'CompLE
			'
			Me.CompLE.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.ParamEditorHelp.SetHelpString(Me.CompLE, "Less Than or Equal")
			Me.CompLE.Location = New System.Drawing.Point(224, 136)
			Me.CompLE.Name = "CompLE"
			Me.ParamEditorHelp.SetShowHelp(Me.CompLE, true)
			Me.CompLE.Size = New System.Drawing.Size(40, 20)
			Me.CompLE.TabIndex = 29
			Me.CompLE.Text = "<="
			AddHandler Me.CompLE.Click, AddressOf Me.CondCompLEClick
			'
			'ParamCondition
			'
			Me.ParamCondition.Controls.Add(Me.CondSimple)
			Me.ParamCondition.Controls.Add(Me.CondParam2)
			Me.ParamCondition.Controls.Add(Me.CondOp)
			Me.ParamCondition.Controls.Add(Me.CondParam1)
			Me.ParamCondition.Controls.Add(Me.CondComplex)
			Me.ParamCondition.Controls.Add(Me.CompCondIn)
			Me.ParamCondition.Controls.Add(Me.CompAnd)
			Me.ParamCondition.Controls.Add(Me.CompOr)
			Me.ParamCondition.Controls.Add(Me.CompXor)
			Me.ParamCondition.Controls.Add(Me.CompNot)
			Me.ParamCondition.Controls.Add(Me.CompEq)
			Me.ParamCondition.Controls.Add(Me.CompNE)
			Me.ParamCondition.Controls.Add(Me.CompLT)
			Me.ParamCondition.Controls.Add(Me.CompLE)
			Me.ParamCondition.Controls.Add(Me.CompGT)
			Me.ParamCondition.Controls.Add(Me.CompGE)
			Me.ParamCondition.Location = New System.Drawing.Point(4, 58)
			Me.ParamCondition.Name = "ParamCondition"
			Me.ParamCondition.Size = New System.Drawing.Size(280, 322)
			Me.ParamCondition.TabIndex = 0
			Me.ParamCondition.Text = "Condition"
			'
			'CondParam1
			'
			Me.ParamEditorHelp.SetHelpString(Me.CondParam1, "First value to compare")
			Me.CondParam1.Location = New System.Drawing.Point(16, 40)
			Me.CondParam1.Name = "CondParam1"
			Me.ParamEditorHelp.SetShowHelp(Me.CondParam1, true)
			Me.CondParam1.Size = New System.Drawing.Size(104, 21)
			Me.CondParam1.TabIndex = 18
			'
			'ParamConstant
			'
			Me.ParamConstant.Location = New System.Drawing.Point(4, 58)
			Me.ParamConstant.Name = "ParamConstant"
			Me.ParamConstant.Size = New System.Drawing.Size(280, 322)
			Me.ParamConstant.TabIndex = 9
			Me.ParamConstant.Text = "Constant"
			'
			'CompGE
			'
			Me.CompGE.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.ParamEditorHelp.SetHelpString(Me.CompGE, "Greater Than or Equal")
			Me.CompGE.Location = New System.Drawing.Point(224, 160)
			Me.CompGE.Name = "CompGE"
			Me.ParamEditorHelp.SetShowHelp(Me.CompGE, true)
			Me.CompGE.Size = New System.Drawing.Size(40, 20)
			Me.CompGE.TabIndex = 31
			Me.CompGE.Text = ">="
			AddHandler Me.CompGE.Click, AddressOf Me.CondCompGEClick
			'
			'CompNE
			'
			Me.CompNE.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.ParamEditorHelp.SetHelpString(Me.CompNE, "Not Equal")
			Me.CompNE.Location = New System.Drawing.Point(144, 160)
			Me.CompNE.Name = "CompNE"
			Me.ParamEditorHelp.SetShowHelp(Me.CompNE, true)
			Me.CompNE.Size = New System.Drawing.Size(40, 20)
			Me.CompNE.TabIndex = 27
			Me.CompNE.Text = "<>"
			AddHandler Me.CompNE.Click, AddressOf Me.CondCompNEClick
			'
			'CompXor
			'
			Me.CompXor.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.CompXor.Location = New System.Drawing.Point(64, 136)
			Me.CompXor.Name = "CompXor"
			Me.CompXor.Size = New System.Drawing.Size(40, 20)
			Me.CompXor.TabIndex = 24
			Me.CompXor.Text = "Xor"
			AddHandler Me.CompXor.Click, AddressOf Me.CondCompXorClick
			'
			'CompAnd
			'
			Me.CompAnd.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.CompAnd.Location = New System.Drawing.Point(24, 136)
			Me.CompAnd.Name = "CompAnd"
			Me.CompAnd.Size = New System.Drawing.Size(40, 20)
			Me.CompAnd.TabIndex = 22
			Me.CompAnd.Text = "And"
			AddHandler Me.CompAnd.Click, AddressOf Me.CondCompAndClick
			'
			'CompNot
			'
			Me.CompNot.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.CompNot.Location = New System.Drawing.Point(64, 160)
			Me.CompNot.Name = "CompNot"
			Me.CompNot.Size = New System.Drawing.Size(40, 20)
			Me.CompNot.TabIndex = 25
			Me.CompNot.Text = "Not"
			AddHandler Me.CompNot.Click, AddressOf Me.CondCompNotClick
			'
			'ParamArrayElement
			'
			Me.ParamArrayElement.Location = New System.Drawing.Point(4, 58)
			Me.ParamArrayElement.Name = "ParamArrayElement"
			Me.ParamArrayElement.Size = New System.Drawing.Size(280, 322)
			Me.ParamArrayElement.TabIndex = 10
			Me.ParamArrayElement.Text = "Array Element"
			'
			'CondSimple
			'
			Me.CondSimple.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.ParamEditorHelp.SetHelpString(Me.CondSimple, "A condition which only compares two values")
			Me.CondSimple.Location = New System.Drawing.Point(8, 16)
			Me.CondSimple.Name = "CondSimple"
			Me.ParamEditorHelp.SetShowHelp(Me.CondSimple, true)
			Me.CondSimple.Size = New System.Drawing.Size(112, 16)
			Me.CondSimple.TabIndex = 16
			Me.CondSimple.Text = "Simple Condition"
			AddHandler Me.CondSimple.CheckedChanged, AddressOf Me.CondSimpleCheckedChanged
			'
			'CondOp
			'
			Me.CondOp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me.ParamEditorHelp.SetHelpString(Me.CondOp, "Comparison mode for the two values")
			Me.CondOp.Items.AddRange(New Object() {"=", "<>", "<", ">", "<=", ">="})
			Me.CondOp.Location = New System.Drawing.Point(120, 40)
			Me.CondOp.Name = "CondOp"
			Me.ParamEditorHelp.SetShowHelp(Me.CondOp, true)
			Me.CondOp.Size = New System.Drawing.Size(48, 21)
			Me.CondOp.TabIndex = 21
			'
			'StringLenDisplay
			'
			Me.StringLenDisplay.Location = New System.Drawing.Point(8, 40)
			Me.StringLenDisplay.Name = "StringLenDisplay"
			Me.StringLenDisplay.Size = New System.Drawing.Size(128, 16)
			Me.StringLenDisplay.TabIndex = 1
			Me.StringLenDisplay.Text = "Current length: 0"
			'
			'CondComplex
			'
			Me.CondComplex.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.ParamEditorHelp.SetHelpString(Me.CondComplex, "A condition which compares multiple values, or which performs some calculation on"& _ 
" the values before comparing them")
			Me.CondComplex.Location = New System.Drawing.Point(8, 80)
			Me.CondComplex.Name = "CondComplex"
			Me.ParamEditorHelp.SetShowHelp(Me.CondComplex, true)
			Me.CondComplex.Size = New System.Drawing.Size(120, 16)
			Me.CondComplex.TabIndex = 17
			Me.CondComplex.Text = "Complex Condition"
			AddHandler Me.CondComplex.CheckedChanged, AddressOf Me.CondComplexCheckedChanged
			'
			'VarList
			'
			Me.VarList.IntegralHeight = false
			Me.VarList.Location = New System.Drawing.Point(8, 16)
			Me.VarList.Name = "VarList"
			Me.VarList.Size = New System.Drawing.Size(248, 248)
			Me.VarList.TabIndex = 0
			'
			'label1
			'
			Me.label1.Location = New System.Drawing.Point(16, 24)
			Me.label1.Name = "label1"
			Me.label1.Size = New System.Drawing.Size(40, 16)
			Me.label1.TabIndex = 2
			Me.label1.Text = "Value"
			Me.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
			'
			'CompEq
			'
			Me.CompEq.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.ParamEditorHelp.SetHelpString(Me.CompEq, "Equal")
			Me.CompEq.Location = New System.Drawing.Point(144, 136)
			Me.CompEq.Name = "CompEq"
			Me.ParamEditorHelp.SetShowHelp(Me.CompEq, true)
			Me.CompEq.Size = New System.Drawing.Size(40, 20)
			Me.CompEq.TabIndex = 26
			Me.CompEq.Text = "="
			AddHandler Me.CompEq.Click, AddressOf Me.CondCompEqClick
			'
			'ParamArray
			'
			Me.ParamArray.Controls.Add(Me.AddArray)
			Me.ParamArray.Controls.Add(Me.ArrayBox)
			Me.ParamArray.Location = New System.Drawing.Point(4, 58)
			Me.ParamArray.Name = "ParamArray"
			Me.ParamArray.Size = New System.Drawing.Size(280, 322)
			Me.ParamArray.TabIndex = 3
			Me.ParamArray.Text = "Array"
			'
			'ParamIOPort
			'
			Me.ParamIOPort.Location = New System.Drawing.Point(4, 58)
			Me.ParamIOPort.Name = "ParamIOPort"
			Me.ParamIOPort.Size = New System.Drawing.Size(280, 322)
			Me.ParamIOPort.TabIndex = 6
			Me.ParamIOPort.Text = "I/O Port"
			'
			'ArrayList
			'
			Me.ArrayList.IntegralHeight = false
			Me.ArrayList.Location = New System.Drawing.Point(8, 16)
			Me.ArrayList.Name = "ArrayList"
			Me.ArrayList.Size = New System.Drawing.Size(248, 248)
			Me.ArrayList.TabIndex = 0
			'
			'Button_Cancel
			'
			Me.Button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me.Button_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.Button_Cancel.Location = New System.Drawing.Point(168, 400)
			Me.Button_Cancel.Name = "Button_Cancel"
			Me.Button_Cancel.Size = New System.Drawing.Size(88, 24)
			Me.Button_Cancel.TabIndex = 1
			Me.Button_Cancel.Text = "Cancel"
			'
			'ParamVariable
			'
			Me.ParamVariable.Controls.Add(Me.groupBox1)
			Me.ParamVariable.Controls.Add(Me.AddVar)
			Me.ParamVariable.Location = New System.Drawing.Point(4, 58)
			Me.ParamVariable.Name = "ParamVariable"
			Me.ParamVariable.Size = New System.Drawing.Size(280, 322)
			Me.ParamVariable.TabIndex = 1
			Me.ParamVariable.Text = "Variable"
			'
			'CompOr
			'
			Me.CompOr.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.CompOr.Location = New System.Drawing.Point(24, 160)
			Me.CompOr.Name = "CompOr"
			Me.CompOr.Size = New System.Drawing.Size(40, 20)
			Me.CompOr.TabIndex = 23
			Me.CompOr.Text = "Or"
			AddHandler Me.CompOr.Click, AddressOf Me.CondCompOrClick
			'
			'ParamEditTabs
			'
			Me.ParamEditTabs.Controls.Add(Me.ParamCondition)
			Me.ParamEditTabs.Controls.Add(Me.ParamVariable)
			Me.ParamEditTabs.Controls.Add(Me.ParamLiteral)
			Me.ParamEditTabs.Controls.Add(Me.ParamFunction)
			Me.ParamEditTabs.Controls.Add(Me.ParamArray)
			Me.ParamEditTabs.Controls.Add(Me.ParamConstant)
			Me.ParamEditTabs.Controls.Add(Me.ParamIOPin)
			Me.ParamEditTabs.Controls.Add(Me.ParamString)
			Me.ParamEditTabs.Controls.Add(Me.ParamBit)
			Me.ParamEditTabs.Controls.Add(Me.ParamArrayElement)
			Me.ParamEditTabs.Controls.Add(Me.ParamIOPort)
			Me.ParamEditTabs.Location = New System.Drawing.Point(8, 8)
			Me.ParamEditTabs.Multiline = true
			Me.ParamEditTabs.Name = "ParamEditTabs"
			Me.ParamEditTabs.SelectedIndex = 0
			Me.ParamEditTabs.Size = New System.Drawing.Size(288, 384)
			Me.ParamEditTabs.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight
			Me.ParamEditTabs.TabIndex = 4
			'
			'AddArray
			'
			Me.AddArray.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.AddArray.Location = New System.Drawing.Point(88, 288)
			Me.AddArray.Name = "AddArray"
			Me.AddArray.Size = New System.Drawing.Size(112, 24)
			Me.AddArray.TabIndex = 3
			Me.AddArray.Text = "Add New Array"
			AddHandler Me.AddArray.Click, AddressOf Me.AddArrayClick
			'
			'groupBox1
			'
			Me.groupBox1.Controls.Add(Me.VarList)
			Me.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.groupBox1.Location = New System.Drawing.Point(8, 8)
			Me.groupBox1.Name = "groupBox1"
			Me.groupBox1.Size = New System.Drawing.Size(264, 272)
			Me.groupBox1.TabIndex = 0
			Me.groupBox1.TabStop = false
			Me.groupBox1.Text = "Variables"
			'
			'groupBox2
			'
			Me.groupBox2.Controls.Add(Me.LiteralValue)
			Me.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.groupBox2.Location = New System.Drawing.Point(8, 8)
			Me.groupBox2.Name = "groupBox2"
			Me.groupBox2.Size = New System.Drawing.Size(264, 48)
			Me.groupBox2.TabIndex = 0
			Me.groupBox2.TabStop = false
			Me.groupBox2.Text = "Value"
			'
			'groupBox3
			'
			Me.groupBox3.Controls.Add(Me.LiteralBase)
			Me.groupBox3.Controls.Add(Me.label2)
			Me.groupBox3.Controls.Add(Me.label1)
			Me.groupBox3.Controls.Add(Me.LiteralNonDecIn)
			Me.groupBox3.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.groupBox3.Location = New System.Drawing.Point(8, 64)
			Me.groupBox3.Name = "groupBox3"
			Me.groupBox3.Size = New System.Drawing.Size(264, 88)
			Me.groupBox3.TabIndex = 1
			Me.groupBox3.TabStop = false
			Me.groupBox3.Text = "Convert from non-decimal"
			'
			'groupBox4
			'
			Me.groupBox4.Controls.Add(Me.StringLenDisplay)
			Me.groupBox4.Controls.Add(Me.FixedStringValue)
			Me.groupBox4.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.groupBox4.Location = New System.Drawing.Point(8, 8)
			Me.groupBox4.Name = "groupBox4"
			Me.groupBox4.Size = New System.Drawing.Size(264, 64)
			Me.groupBox4.TabIndex = 0
			Me.groupBox4.TabStop = false
			Me.groupBox4.Text = "Message"
			'
			'ParamLiteral
			'
			Me.ParamLiteral.Controls.Add(Me.groupBox3)
			Me.ParamLiteral.Controls.Add(Me.groupBox2)
			Me.ParamLiteral.Location = New System.Drawing.Point(4, 58)
			Me.ParamLiteral.Name = "ParamLiteral"
			Me.ParamLiteral.Size = New System.Drawing.Size(280, 322)
			Me.ParamLiteral.TabIndex = 2
			Me.ParamLiteral.Text = "Fixed Value"
			'
			'AddVar
			'
			Me.AddVar.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.AddVar.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.AddVar.Location = New System.Drawing.Point(88, 288)
			Me.AddVar.Name = "AddVar"
			Me.AddVar.Size = New System.Drawing.Size(112, 24)
			Me.AddVar.TabIndex = 1
			Me.AddVar.Text = "Add New Variable"
			AddHandler Me.AddVar.Click, AddressOf Me.AddVarClick
			'
			'CompLT
			'
			Me.CompLT.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.ParamEditorHelp.SetHelpString(Me.CompLT, "Less Than")
			Me.CompLT.Location = New System.Drawing.Point(184, 136)
			Me.CompLT.Name = "CompLT"
			Me.ParamEditorHelp.SetShowHelp(Me.CompLT, true)
			Me.CompLT.Size = New System.Drawing.Size(40, 20)
			Me.CompLT.TabIndex = 28
			Me.CompLT.Text = "<"
			AddHandler Me.CompLT.Click, AddressOf Me.CondCompLTClick
			'
			'label2
			'
			Me.label2.Location = New System.Drawing.Point(16, 56)
			Me.label2.Name = "label2"
			Me.label2.Size = New System.Drawing.Size(32, 16)
			Me.label2.TabIndex = 3
			Me.label2.Text = "Base"
			'
			'ParamEditor
			'
			Me.AcceptButton = Me.Button_OK
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.CancelButton = Me.Button_Cancel
			Me.ClientSize = New System.Drawing.Size(306, 431)
			Me.Controls.Add(Me.ParamEditTabs)
			Me.Controls.Add(Me.Button_Cancel)
			Me.Controls.Add(Me.Button_OK)
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
			Me.HelpButton = true
			Me.MaximizeBox = false
			Me.MinimizeBox = false
			Me.Name = "ParamEditor"
			Me.ShowInTaskbar = false
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
			Me.Text = "Parameter Editor"
			CType(Me.LiteralBase,System.ComponentModel.ISupportInitialize).EndInit
			CType(Me.LiteralValue,System.ComponentModel.ISupportInitialize).EndInit
			Me.ArrayBox.ResumeLayout(false)
			Me.ParamString.ResumeLayout(false)
			Me.ParamCondition.ResumeLayout(false)
			Me.ParamArray.ResumeLayout(false)
			Me.ParamVariable.ResumeLayout(false)
			Me.ParamEditTabs.ResumeLayout(false)
			Me.groupBox1.ResumeLayout(false)
			Me.groupBox2.ResumeLayout(false)
			Me.groupBox3.ResumeLayout(false)
			Me.groupBox4.ResumeLayout(false)
			Me.ParamLiteral.ResumeLayout(false)
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
		'Condition editor subs
		Private Sub CondDisableAll
			Me.CondOp.Enabled = False
			Me.CondParam1.Enabled = False
			Me.CondParam2.Enabled = False
			
			Me.CompCondIn.Enabled = False
			Me.CompAnd.Enabled = False
			Me.CompOr.Enabled = False
			Me.CompXor.Enabled = False
			Me.CompNot.Enabled = False
			Me.CompEq.Enabled = False
			Me.CompNE.Enabled = False
			Me.CompLT.Enabled = False
			Me.CompGT.Enabled = False
			Me.CompLE.Enabled = False
			Me.CompGE.Enabled = False
		End Sub
		
		Private Sub CondCompAndClick(sender As System.Object, e As System.EventArgs)
			CondComplex.Checked = True
			CompCondIn.Text += " & "
			CompCondIn.Select
			CompCondIn.Select(CompCondIn.Text.Length, 0)
		End Sub
		
		Private Sub CondCompOrClick(sender As System.Object, e As System.EventArgs)
			CondComplex.Checked = True
			CompCondIn.Text += " | "
			CompCondIn.Select
			CompCondIn.Select(CompCondIn.Text.Length, 0)
		End Sub
		
		Private Sub CondCompXorClick(sender As System.Object, e As System.EventArgs)
			CondComplex.Checked = True
			CompCondIn.Text += " # "
			CompCondIn.Select
			CompCondIn.Select(CompCondIn.Text.Length, 0)
		End Sub
		
		Private Sub CondCompNotClick(sender As System.Object, e As System.EventArgs)
			CondComplex.Checked = True
			CompCondIn.Text += "!"
			CompCondIn.Select
			CompCondIn.Select(CompCondIn.Text.Length, 0)
		End Sub
		
		Private Sub CondCompEqClick(sender As System.Object, e As System.EventArgs)
			CondComplex.Checked = True
			CompCondIn.Text += " = "
			CompCondIn.Select
			CompCondIn.Select(CompCondIn.Text.Length, 0)
		End Sub
		
		Private Sub CondCompNEClick(sender As System.Object, e As System.EventArgs)
			CondComplex.Checked = True
			CompCondIn.Text += " <> "
			CompCondIn.Select
			CompCondIn.Select(CompCondIn.Text.Length, 0)
		End Sub
		
		Private Sub CondCompLTClick(sender As System.Object, e As System.EventArgs)
			CondComplex.Checked = True
			CompCondIn.Text += " < "
			CompCondIn.Select
			CompCondIn.Select(CompCondIn.Text.Length, 0)
		End Sub
		
		Private Sub CondCompGTClick(sender As System.Object, e As System.EventArgs)
			CondComplex.Checked = True
			CompCondIn.Text += " > "
			CompCondIn.Select
			CompCondIn.Select(CompCondIn.Text.Length, 0)
		End Sub
		
		Private Sub CondCompLEClick(sender As System.Object, e As System.EventArgs)
			CondComplex.Checked = True
			CompCondIn.Text += " <= "
			CompCondIn.Select
			CompCondIn.Select(CompCondIn.Text.Length, 0)
		End Sub
		
		Private Sub CondCompGEClick(sender As System.Object, e As System.EventArgs)
			CondComplex.Checked = True
			CompCondIn.Text += " >= "
			CompCondIn.Select
			CompCondIn.Select(CompCondIn.Text.Length, 0)
		End Sub
		
		Private Sub CondSimpleCheckedChanged(sender As System.Object, e As System.EventArgs)
			If CondSimple.Checked Then
				Me.CondOp.Enabled = True
				Me.CondParam1.Enabled = True
				Me.CondParam2.Enabled = True
			Else
				Me.CondOp.Enabled = False
				Me.CondParam1.Enabled = False
				Me.CondParam2.Enabled = False
			End If
			
		End Sub
		
		Private Sub CondComplexCheckedChanged(sender As System.Object, e As System.EventArgs)
			If CondComplex.Checked Then
				Me.CompCondIn.Enabled = True
				Me.CompAnd.Enabled = True
				Me.CompOr.Enabled = True
				Me.CompXor.Enabled = True
				Me.CompNot.Enabled = True
				Me.CompEq.Enabled = True
				Me.CompNE.Enabled = True
				Me.CompLT.Enabled = True
				Me.CompGT.Enabled = True
				Me.CompLE.Enabled = True
				Me.CompGE.Enabled = True
				If CompCondIn.Text.Trim = "" And CondParam1.Text <> "" And CondParam2.Text <> "" Then
					CompCondIn.Text = CondParam1.Text + " " + CondOp.Text + " " + CondParam2.Text
				End If
			Else
				Me.CompCondIn.Enabled = False
				Me.CompAnd.Enabled = False
				Me.CompOr.Enabled = False
				Me.CompXor.Enabled = False
				Me.CompNot.Enabled = False
				Me.CompEq.Enabled = False
				Me.CompNE.Enabled = False
				Me.CompLT.Enabled = False
				Me.CompGT.Enabled = False
				Me.CompLE.Enabled = False
				Me.CompGE.Enabled = False
			End If
			
		End Sub
		
		Private Sub Button_OKClick(sender As System.Object, e As System.EventArgs)
			Accepted = False
			'MainForm.SetStatus(Me.ParamEditTabs.SelectedTab.Text.Trim.ToLower)
			
			Select Case Me.ParamEditTabs.SelectedTab.Text.Trim.ToLower
				Case "condition":
					If CondSimple.Checked Then
						ParamValue = CondParam1.Text + " " + CondOp.Text + " " + CondParam2.Text
						Accepted = True
						
					ElseIf CondComplex.Checked Then
						ParamValue = CompCondIn.Text
						Accepted = True
						
					End If
					
				Case "variable":
					Try
						ParamValue = Me.VarList.SelectedItem.ToString
						Accepted = True
					Catch
						ParamValue = ""
					End Try
					
				Case "array":
					Try
						ParamValue = Me.ArrayList.SelectedItem.ToString
						Accepted = True
					Catch
						ParamValue = ""
					End Try
					
				Case "fixed value":
					ParamValue = Me.LiteralValue.Value.ToString
					Accepted = True
					
				Case "string":
					If Me.FixedStringValue.Text.IndexOf(Chr(34)) <> -1 Then
						MessageBox.Show("The " + Chr(34) + " symbol cannot be used in a string", "Great Cow Graphical BASIC", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
						Exit Sub
					End If
					ParamValue = Chr(34) + Me.FixedStringValue.Text + Chr(34)
					Accepted = True
					
			End Select
			
			Me.Close
			
		End Sub
		
		Private Sub AddVarClick(sender As System.Object, e As System.EventArgs)
			Dim SearchList As Integer = 0
			Dim VarExists As Boolean = False
			Dim TempData As String = ""
			
			'Display dialog
			Dim fEditVar As New EditVar
			fEditVar.Text = "Add Variable"
			fEditVar.InitEdit("", "byte", -1)
			fEditVar.ShowDialog()
			
			'Apply
			If fEditVar.NewVarName <> "" Then
				TempData = fEditVar.NewVarType
				If TempData = "array" Then
					TempData = fEditVar.NewArraySize
					TempData = "(" + TempData.Trim + ")"
				End If
				
				Variables.AddVar(CurrentFile, fEditVar.NewVarName.Trim, TempData)
				
				Me.VarList.Items.Add(fEditVar.NewVarName)
				Me.VarList.SelectedIndex = Me.VarList.Items.Count - 1
			End If
			
		End Sub
		
		Private Sub AddArrayClick(sender As System.Object, e As System.EventArgs)
			Dim SearchList As Integer = 0
			Dim VarExists As Boolean = False
			Dim TempData As String = ""
			
			'Display dialog
			Dim fEditVar As New EditVar
			fEditVar.Text = "Add Array"
			fEditVar.InitEdit("", "array", -3)
			fEditVar.ShowDialog()
			
			'Apply
			If fEditVar.NewVarName <> "" Then
				TempData = fEditVar.NewVarType
				If TempData = "array" Then
					TempData = fEditVar.NewArraySize
					TempData = "(" + TempData.Trim + ")"
				End If
				
				Variables.AddVar(CurrentFile, fEditVar.NewVarName.Trim, TempData)
				
				Me.ArrayList.Items.Add(fEditVar.NewVarName)
				Me.ArrayList.SelectedIndex = Me.ArrayList.Items.Count - 1
			End If
		End Sub
		
		Private Sub LiteralValueValueChanged(sender As System.Object, e As System.EventArgs)
			RemoveHandler LiteralNonDecIn.TextChanged, AddressOf LiteralNonDecInTextChanged
			LiteralNonDecIn.Text = Lowlevel.Dec2Other(LiteralValue.Value, LiteralBase.Value)
			AddHandler LiteralNonDecIn.TextChanged, AddressOf LiteralNonDecInTextChanged
		End Sub
		
		Private Sub LiteralBaseValueChanged(sender As System.Object, e As System.EventArgs)
			RemoveHandler LiteralNonDecIn.TextChanged, AddressOf LiteralNonDecInTextChanged
			LiteralNonDecIn.Text = Lowlevel.Dec2Other(LiteralValue.Value, LiteralBase.Value)
			AddHandler LiteralNonDecIn.TextChanged, AddressOf LiteralNonDecInTextChanged
		End Sub
		
		Private Sub LiteralNonDecInTextChanged(sender As System.Object, e As System.EventArgs)
			RemoveHandler LiteralBase.ValueChanged, AddressOf LiteralBaseValueChanged
			RemoveHandler LiteralValue.ValueChanged, AddressOf LiteralValueValueChanged
			Dim Temp As Integer = Lowlevel.Other2Dec(LiteralNonDecIn.Text, LiteralBase.Value)
			If Temp <> -99999 Then
				Try
					LiteralValue.Value = Temp
				Catch
					MessageBox.Show("The non-decimal value that you have entered is not suitable for this parameter.", "Great Cow Graphical BASIC", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
				End Try
			End If
			AddHandler LiteralBase.ValueChanged, AddressOf LiteralBaseValueChanged
			AddHandler LiteralValue.ValueChanged, AddressOf LiteralValueValueChanged
		End Sub
		
		Private Sub FixedStringValueTextChanged(sender As System.Object, e As System.EventArgs)
			StringLenDisplay.Text = "Current length: " + FixedStringValue.Text.Length.ToString
			
		End Sub
		
	End Class
End Namespace
