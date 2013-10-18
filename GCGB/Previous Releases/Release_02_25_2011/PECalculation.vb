'
' Created by SharpDevelop.
' User: Hugh
' Date: 21/01/2010
' Time: 1:03 AM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.ComponentModel
Imports System.Windows.Forms

Namespace Great_Cow_Graphical_BASIC
	
	Public Class PECalculation
		Inherits System.Windows.Forms.UserControl
		Implements ParamEditPage
		Private addSubtract As System.Windows.Forms.Button
		Private addCloseBracket As System.Windows.Forms.Button
		Private addXor As System.Windows.Forms.Button
		Private InsertValue As System.Windows.Forms.Button
		Private addMoreEqual As System.Windows.Forms.Button
		Private addOpenBracket As System.Windows.Forms.Button
		Private operatorGroup As System.Windows.Forms.GroupBox
		Private addAnd As System.Windows.Forms.Button
		Private addModulo As System.Windows.Forms.Button
		Private addLess As System.Windows.Forms.Button
		Private tipDisplay As System.Windows.Forms.TextBox
		Private addMultiply As System.Windows.Forms.Button
		Private addNot As System.Windows.Forms.Button
		Private valueEditor As System.Windows.Forms.TextBox
		Private labelCalculation As System.Windows.Forms.Label
		Private addMore As System.Windows.Forms.Button
		Private addOr As System.Windows.Forms.Button
		Private addDivide As System.Windows.Forms.Button
		Private addLessEqual As System.Windows.Forms.Button
		Private addEqual As System.Windows.Forms.Button
		Private addAdd As System.Windows.Forms.Button
		Private addNotEqual As System.Windows.Forms.Button
		
		
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
				Return valueEditor.Text
			End Get
		End Property
		
		Public Sub Init (Frame As ParameterEditor, ParamType As String, CurrentSource As String) Implements ParamEditPage.Init
			Me.Frame = Frame
			
			Frame.NextIsFinish = True
			
			If CurrentSource <> "" Then
				valueEditor.Text = CurrentSource
			End If
			
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
			Me.addNotEqual = New System.Windows.Forms.Button
			Me.addAdd = New System.Windows.Forms.Button
			Me.addEqual = New System.Windows.Forms.Button
			Me.addLessEqual = New System.Windows.Forms.Button
			Me.addDivide = New System.Windows.Forms.Button
			Me.addOr = New System.Windows.Forms.Button
			Me.addMore = New System.Windows.Forms.Button
			Me.labelCalculation = New System.Windows.Forms.Label
			Me.valueEditor = New System.Windows.Forms.TextBox
			Me.addNot = New System.Windows.Forms.Button
			Me.addMultiply = New System.Windows.Forms.Button
			Me.tipDisplay = New System.Windows.Forms.TextBox
			Me.addLess = New System.Windows.Forms.Button
			Me.addModulo = New System.Windows.Forms.Button
			Me.addAnd = New System.Windows.Forms.Button
			Me.operatorGroup = New System.Windows.Forms.GroupBox
			Me.addOpenBracket = New System.Windows.Forms.Button
			Me.addMoreEqual = New System.Windows.Forms.Button
			Me.InsertValue = New System.Windows.Forms.Button
			Me.addXor = New System.Windows.Forms.Button
			Me.addCloseBracket = New System.Windows.Forms.Button
			Me.addSubtract = New System.Windows.Forms.Button
			Me.operatorGroup.SuspendLayout
			Me.SuspendLayout
			'
			'addNotEqual
			'
			Me.addNotEqual.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.addNotEqual.Location = New System.Drawing.Point(184, 48)
			Me.addNotEqual.Name = "addNotEqual"
			Me.addNotEqual.Size = New System.Drawing.Size(32, 23)
			Me.addNotEqual.TabIndex = 9
			Me.addNotEqual.Text = "<>"
			AddHandler Me.addNotEqual.Click, AddressOf Me.AddNotEqualClick
			AddHandler Me.addNotEqual.MouseEnter, AddressOf Me.AddNotEqualMouseEnter
			'
			'addAdd
			'
			Me.addAdd.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.addAdd.Location = New System.Drawing.Point(136, 24)
			Me.addAdd.Name = "addAdd"
			Me.addAdd.Size = New System.Drawing.Size(40, 23)
			Me.addAdd.TabIndex = 6
			Me.addAdd.Text = "+"
			AddHandler Me.addAdd.Click, AddressOf Me.AddAddClick
			AddHandler Me.addAdd.MouseEnter, AddressOf Me.AddAddMouseEnter
			'
			'addEqual
			'
			Me.addEqual.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.addEqual.Location = New System.Drawing.Point(184, 24)
			Me.addEqual.Name = "addEqual"
			Me.addEqual.Size = New System.Drawing.Size(32, 23)
			Me.addEqual.TabIndex = 8
			Me.addEqual.Text = "="
			AddHandler Me.addEqual.Click, AddressOf Me.AddEqualClick
			AddHandler Me.addEqual.MouseEnter, AddressOf Me.AddEqualMouseEnter
			'
			'addLessEqual
			'
			Me.addLessEqual.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.addLessEqual.Location = New System.Drawing.Point(248, 24)
			Me.addLessEqual.Name = "addLessEqual"
			Me.addLessEqual.Size = New System.Drawing.Size(32, 23)
			Me.addLessEqual.TabIndex = 12
			Me.addLessEqual.Text = "<="
			AddHandler Me.addLessEqual.Click, AddressOf Me.AddLessEqualClick
			AddHandler Me.addLessEqual.MouseEnter, AddressOf Me.AddLessEqualMouseEnter
			'
			'addDivide
			'
			Me.addDivide.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.addDivide.Location = New System.Drawing.Point(56, 48)
			Me.addDivide.Name = "addDivide"
			Me.addDivide.Size = New System.Drawing.Size(40, 23)
			Me.addDivide.TabIndex = 5
			Me.addDivide.Text = "/"
			AddHandler Me.addDivide.Click, AddressOf Me.AddDivideClick
			AddHandler Me.addDivide.MouseEnter, AddressOf Me.AddDivideMouseEnter
			'
			'addOr
			'
			Me.addOr.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.addOr.Location = New System.Drawing.Point(328, 24)
			Me.addOr.Name = "addOr"
			Me.addOr.Size = New System.Drawing.Size(40, 23)
			Me.addOr.TabIndex = 16
			Me.addOr.Text = "Or"
			AddHandler Me.addOr.Click, AddressOf Me.AddOrClick
			AddHandler Me.addOr.MouseEnter, AddressOf Me.AddOrMouseEnter
			'
			'addMore
			'
			Me.addMore.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.addMore.Location = New System.Drawing.Point(216, 48)
			Me.addMore.Name = "addMore"
			Me.addMore.Size = New System.Drawing.Size(32, 23)
			Me.addMore.TabIndex = 11
			Me.addMore.Text = ">"
			AddHandler Me.addMore.Click, AddressOf Me.AddMoreClick
			AddHandler Me.addMore.MouseEnter, AddressOf Me.AddMoreMouseEnter
			'
			'labelCalculation
			'
			Me.labelCalculation.Location = New System.Drawing.Point(8, 8)
			Me.labelCalculation.Name = "labelCalculation"
			Me.labelCalculation.Size = New System.Drawing.Size(112, 16)
			Me.labelCalculation.TabIndex = 1
			Me.labelCalculation.Text = "Calculation:"
			'
			'valueEditor
			'
			Me.valueEditor.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.valueEditor.Location = New System.Drawing.Point(8, 24)
			Me.valueEditor.Name = "valueEditor"
			Me.valueEditor.Size = New System.Drawing.Size(376, 21)
			Me.valueEditor.TabIndex = 0
			Me.valueEditor.Text = ""
			AddHandler Me.valueEditor.MouseEnter, AddressOf Me.ValueEditorMouseEnter
			'
			'addNot
			'
			Me.addNot.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.addNot.Location = New System.Drawing.Point(288, 24)
			Me.addNot.Name = "addNot"
			Me.addNot.Size = New System.Drawing.Size(40, 23)
			Me.addNot.TabIndex = 14
			Me.addNot.Text = "Not"
			AddHandler Me.addNot.Click, AddressOf Me.AddNotClick
			AddHandler Me.addNot.MouseEnter, AddressOf Me.AddNotMouseEnter
			'
			'addMultiply
			'
			Me.addMultiply.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.addMultiply.Location = New System.Drawing.Point(56, 24)
			Me.addMultiply.Name = "addMultiply"
			Me.addMultiply.Size = New System.Drawing.Size(40, 23)
			Me.addMultiply.TabIndex = 4
			Me.addMultiply.Text = "*"
			AddHandler Me.addMultiply.Click, AddressOf Me.AddMultiplyClick
			AddHandler Me.addMultiply.MouseEnter, AddressOf Me.AddMultiplyMouseEnter
			'
			'tipDisplay
			'
			Me.tipDisplay.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.tipDisplay.Location = New System.Drawing.Point(8, 176)
			Me.tipDisplay.Multiline = true
			Me.tipDisplay.Name = "tipDisplay"
			Me.tipDisplay.ReadOnly = true
			Me.tipDisplay.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
			Me.tipDisplay.Size = New System.Drawing.Size(376, 80)
			Me.tipDisplay.TabIndex = 5
			Me.tipDisplay.Text = "Create a calculation by entering values and operators."&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&Microsoft.VisualBasic.ChrW(13)&Microsoft.VisualBasic.ChrW(10)&"Move the mouse over an "& _ 
"item on the window to see an explanation of what it does here"
			'
			'addLess
			'
			Me.addLess.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.addLess.Location = New System.Drawing.Point(216, 24)
			Me.addLess.Name = "addLess"
			Me.addLess.Size = New System.Drawing.Size(32, 23)
			Me.addLess.TabIndex = 10
			Me.addLess.Text = "<"
			AddHandler Me.addLess.Click, AddressOf Me.AddLessClick
			AddHandler Me.addLess.MouseEnter, AddressOf Me.AddLessMouseEnter
			'
			'addModulo
			'
			Me.addModulo.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.addModulo.Location = New System.Drawing.Point(96, 24)
			Me.addModulo.Name = "addModulo"
			Me.addModulo.Size = New System.Drawing.Size(40, 23)
			Me.addModulo.TabIndex = 18
			Me.addModulo.Text = "%"
			AddHandler Me.addModulo.Click, AddressOf Me.AddModuloClick
			AddHandler Me.addModulo.MouseEnter, AddressOf Me.AddModuloMouseEnter
			'
			'addAnd
			'
			Me.addAnd.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.addAnd.Location = New System.Drawing.Point(288, 48)
			Me.addAnd.Name = "addAnd"
			Me.addAnd.Size = New System.Drawing.Size(40, 23)
			Me.addAnd.TabIndex = 15
			Me.addAnd.Text = "And"
			AddHandler Me.addAnd.Click, AddressOf Me.AddAndClick
			AddHandler Me.addAnd.MouseEnter, AddressOf Me.AddAndMouseEnter
			'
			'operatorGroup
			'
			Me.operatorGroup.Anchor = System.Windows.Forms.AnchorStyles.Top
			Me.operatorGroup.Controls.Add(Me.addModulo)
			Me.operatorGroup.Controls.Add(Me.addXor)
			Me.operatorGroup.Controls.Add(Me.addOr)
			Me.operatorGroup.Controls.Add(Me.addAnd)
			Me.operatorGroup.Controls.Add(Me.addNot)
			Me.operatorGroup.Controls.Add(Me.addMoreEqual)
			Me.operatorGroup.Controls.Add(Me.addLessEqual)
			Me.operatorGroup.Controls.Add(Me.addMore)
			Me.operatorGroup.Controls.Add(Me.addLess)
			Me.operatorGroup.Controls.Add(Me.addNotEqual)
			Me.operatorGroup.Controls.Add(Me.addEqual)
			Me.operatorGroup.Controls.Add(Me.addSubtract)
			Me.operatorGroup.Controls.Add(Me.addAdd)
			Me.operatorGroup.Controls.Add(Me.addDivide)
			Me.operatorGroup.Controls.Add(Me.addMultiply)
			Me.operatorGroup.Controls.Add(Me.addCloseBracket)
			Me.operatorGroup.Controls.Add(Me.addOpenBracket)
			Me.operatorGroup.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.operatorGroup.Location = New System.Drawing.Point(8, 88)
			Me.operatorGroup.Name = "operatorGroup"
			Me.operatorGroup.Size = New System.Drawing.Size(376, 80)
			Me.operatorGroup.TabIndex = 4
			Me.operatorGroup.TabStop = false
			Me.operatorGroup.Text = "Insert operator"
			'
			'addOpenBracket
			'
			Me.addOpenBracket.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.addOpenBracket.Location = New System.Drawing.Point(8, 24)
			Me.addOpenBracket.Name = "addOpenBracket"
			Me.addOpenBracket.Size = New System.Drawing.Size(40, 23)
			Me.addOpenBracket.TabIndex = 2
			Me.addOpenBracket.Text = "("
			AddHandler Me.addOpenBracket.Click, AddressOf Me.AddOpenBracketClick
			AddHandler Me.addOpenBracket.MouseEnter, AddressOf Me.AddOpenBracketMouseEnter
			'
			'addMoreEqual
			'
			Me.addMoreEqual.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.addMoreEqual.Location = New System.Drawing.Point(248, 48)
			Me.addMoreEqual.Name = "addMoreEqual"
			Me.addMoreEqual.Size = New System.Drawing.Size(32, 23)
			Me.addMoreEqual.TabIndex = 13
			Me.addMoreEqual.Text = ">="
			AddHandler Me.addMoreEqual.Click, AddressOf Me.AddMoreEqualClick
			AddHandler Me.addMoreEqual.MouseEnter, AddressOf Me.AddMoreEqualMouseEnter
			'
			'InsertValue
			'
			Me.InsertValue.Anchor = System.Windows.Forms.AnchorStyles.Top
			Me.InsertValue.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.InsertValue.Location = New System.Drawing.Point(128, 56)
			Me.InsertValue.Name = "InsertValue"
			Me.InsertValue.Size = New System.Drawing.Size(136, 24)
			Me.InsertValue.TabIndex = 3
			Me.InsertValue.Text = "Insert value..."
			AddHandler Me.InsertValue.Click, AddressOf Me.InsertValueClick
			AddHandler Me.InsertValue.MouseEnter, AddressOf Me.InsertValueMouseEnter
			'
			'addXor
			'
			Me.addXor.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.addXor.Location = New System.Drawing.Point(328, 48)
			Me.addXor.Name = "addXor"
			Me.addXor.Size = New System.Drawing.Size(40, 23)
			Me.addXor.TabIndex = 17
			Me.addXor.Text = "Xor"
			AddHandler Me.addXor.Click, AddressOf Me.AddXorClick
			AddHandler Me.addXor.MouseEnter, AddressOf Me.AddXorMouseEnter
			'
			'addCloseBracket
			'
			Me.addCloseBracket.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.addCloseBracket.Location = New System.Drawing.Point(8, 48)
			Me.addCloseBracket.Name = "addCloseBracket"
			Me.addCloseBracket.Size = New System.Drawing.Size(40, 23)
			Me.addCloseBracket.TabIndex = 3
			Me.addCloseBracket.Text = ")"
			AddHandler Me.addCloseBracket.Click, AddressOf Me.AddCloseBracketClick
			AddHandler Me.addCloseBracket.MouseEnter, AddressOf Me.AddCloseBracketMouseEnter
			'
			'addSubtract
			'
			Me.addSubtract.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.addSubtract.Location = New System.Drawing.Point(136, 48)
			Me.addSubtract.Name = "addSubtract"
			Me.addSubtract.Size = New System.Drawing.Size(40, 23)
			Me.addSubtract.TabIndex = 7
			Me.addSubtract.Text = "-"
			AddHandler Me.addSubtract.Click, AddressOf Me.AddSubtractClick
			AddHandler Me.addSubtract.MouseEnter, AddressOf Me.AddSubtractMouseEnter
			'
			'PECalculation
			'
			Me.Controls.Add(Me.tipDisplay)
			Me.Controls.Add(Me.operatorGroup)
			Me.Controls.Add(Me.InsertValue)
			Me.Controls.Add(Me.labelCalculation)
			Me.Controls.Add(Me.valueEditor)
			Me.Name = "PECalculation"
			Me.Size = New System.Drawing.Size(392, 264)
			Me.operatorGroup.ResumeLayout(false)
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
		Private Sub Insert(NewValue As String)
			'Insert the given text at the current cursor location in the textbox
			
			With valueEditor
				.SelectedText = NewValue
				.SelectionStart = .SelectionStart + .SelectionLength
				.SelectionLength = 0
				.Select
			End With
			
		End Sub
		
		Private Sub InsertValueClick(sender As System.Object, e As System.EventArgs)
			'Display parameter editor to get new value
			Dim editor As New ParameterEditor
			editor.Init(Frame.Program, "New Value", "bit|number|string", "", Frame.CurrentSub)
			editor.ShowDialog
			
			'Insert new value
			Insert(editor.ParamValue)
		End Sub
		
		Private Sub InsertValueMouseEnter(sender As System.Object, e As System.EventArgs)
			tipDisplay.Text = "Opens another parameter editor, which will allow a value (such as a variable, number, or function) to be inserted in to this calculation."
			
		End Sub
		
		Private Sub AddOpenBracketClick(sender As System.Object, e As System.EventArgs)
			Insert(" (")
		End Sub
		
		Private Sub AddOpenBracketMouseEnter(sender As System.Object, e As System.EventArgs)
			tipDisplay.Text = "Add an opening bracket. The part of the calculation in the brackets will be worked out first." + Environment.NewLine + _
			                  Environment.NewLine + _
			                  "GCGB will use this order of operations when performing a calculation:" + Environment.NewLine + _
			                  "1. Brackets" + Environment.NewLine + _
			                  "2. Not (inverts a value)" + Environment.NewLine + _
			                  "3. Multiply and Divide" + Environment.NewLine + _
			                  "4. Add and Subtract" + Environment.NewLine + _
			                  "5. Comparison (=, <>, <, >, <=, >=)" + Environment.NewLine + _
			                  "6. Logic (And, Or, Xor)"
			
		End Sub
		
		Private Sub AddCloseBracketClick(sender As System.Object, e As System.EventArgs)
			Insert(") ")
		End Sub
		
		Private Sub AddCloseBracketMouseEnter(sender As System.Object, e As System.EventArgs)
			tipDisplay.Text = "Add a closing bracket. The part of the calculation in the brackets will be worked out first." + Environment.NewLine + _
			                  Environment.NewLine + _
			                  "GCGB will use this order of operations when performing a calculation:" + Environment.NewLine + _
			                  "1. Brackets" + Environment.NewLine + _
			                  "2. Not (inverts a value)" + Environment.NewLine + _
			                  "3. Multiply and Divide" + Environment.NewLine + _
			                  "4. Add and Subtract" + Environment.NewLine + _
			                  "5. Comparison (=, <>, <, >, <=, >=)" + Environment.NewLine + _
			                  "6. Logic (And, Or, Xor)"
			
		End Sub
		
		Private Sub ValueEditorMouseEnter(sender As System.Object, e As System.EventArgs)
			tipDisplay.Text = "This is the calculation. You can type in here, or use the buttons below to add things to the calculation."
			
		End Sub
		
		Private Sub AddMultiplyClick(sender As System.Object, e As System.EventArgs)
			Insert(" * ")
		End Sub
		
		Private Sub AddMultiplyMouseEnter(sender As System.Object, e As System.EventArgs)
			tipDisplay.Text = "Inserts a multiply operation."
			
		End Sub
		
		Private Sub AddDivideClick(sender As System.Object, e As System.EventArgs)
			Insert(" / ")
		End Sub
		
		Private Sub AddDivideMouseEnter(sender As System.Object, e As System.EventArgs)
			tipDisplay.Text = "Inserts a divide operation."
			
		End Sub
		
		
		Private Sub AddModuloMouseEnter(sender As System.Object, e As System.EventArgs)
			tipDisplay.Text = "Inserts a modulo operation. This will divide the first number by the second, and give the remainder."
			
		End Sub
		
		Private Sub AddModuloClick(sender As System.Object, e As System.EventArgs)
			Insert(" %")
			
		End Sub
		
		
		Private Sub AddAddClick(sender As System.Object, e As System.EventArgs)
			Insert(" + ")
		End Sub
		
		Private Sub AddAddMouseEnter(sender As System.Object, e As System.EventArgs)
			tipDisplay.Text = "Inserts an add operation."
			
		End Sub
		
		Private Sub AddSubtractClick(sender As System.Object, e As System.EventArgs)
			Insert(" - ")
		End Sub
		
		Private Sub AddSubtractMouseEnter(sender As System.Object, e As System.EventArgs)
			tipDisplay.Text = "Inserts a subtract operation."
			
		End Sub
		
		Private Sub AddEqualClick(sender As System.Object, e As System.EventArgs)
			Insert(" = ")
		End Sub
		
		Private Sub AddEqualMouseEnter(sender As System.Object, e As System.EventArgs)
			tipDisplay.Text = "Add an equal operation. If the value on the left of the = is the same as the value on the right of the =, then this calculation will result in 255. Otherwise, it will result in 0." + _
			                  Environment.NewLine + _
			                  Environment.NewLine + _
			                  "Note that if this calculation is going to be used for a condition, then the condition will be true when the calculation results in 255 (or anything else other than 0), and false when the calculation results in 0." + _
			                  Environment.NewLine + _
			                  Environment.NewLine + _
			                  "Examples:" + Environment.NewLine + _
			                  "123 = 0 results in 0" + Environment.NewLine + _
			                  "12 = 12 results in 255"
			
		End Sub
		
		Private Sub AddNotEqualClick(sender As System.Object, e As System.EventArgs)
			Insert(" <> ")
		End Sub
		
		Private Sub AddNotEqualMouseEnter(sender As System.Object, e As System.EventArgs)
			tipDisplay.Text = "Add a not equal operation. If the value on the left of the <> is the same as the value on the right, then this calculation will result in 0. Otherwise, it will result in 255." + _
			                  Environment.NewLine + _
			                  Environment.NewLine + _
			                  "Note that if this calculation is going to be used for a condition, then the condition will be true when the calculation results in 255 (or anything else other than 0), and false when the calculation results in 0." + _
			                  Environment.NewLine + _
			                  Environment.NewLine + _
			                  "Examples:" + Environment.NewLine + _
			                  "2 <> 3 results in 255" + Environment.NewLine + _
			                  "34 <> 34 results in 0"
			
		End Sub
		
		Private Sub AddLessClick(sender As System.Object, e As System.EventArgs)
			Insert(" < ")
		End Sub
		
		Private Sub AddLessMouseEnter(sender As System.Object, e As System.EventArgs)
			tipDisplay.Text = "Add a less than operation. If the value on the left of the < is less than the value on the right, then this calculation will result in 255. Otherwise, it will result in 0." + _
			                  Environment.NewLine + _
			                  Environment.NewLine + _
			                  "Note that if this calculation is going to be used for a condition, then the condition will be true when the calculation results in 255 (or anything else other than 0), and false when the calculation results in 0." + _
			                  Environment.NewLine + _
			                  Environment.NewLine + _
			                  "Examples:" + Environment.NewLine + _
			                  "70 < 80 results in 255" + Environment.NewLine + _
			                  "80 < 80 results in 0" + Environment.NewLine + _
			                  "90 < 80 results in 0"
			
		End Sub
		
		Private Sub AddMoreClick(sender As System.Object, e As System.EventArgs)
			Insert(" > ")
		End Sub
		
		Private Sub AddMoreMouseEnter(sender As System.Object, e As System.EventArgs)
			tipDisplay.Text = "Add a more than operation. If the value on the left of the > is more than the value on the right, then this calculation will result in 255. Otherwise, it will result in 0." + _
			                  Environment.NewLine + _
			                  Environment.NewLine + _
			                  "Note that if this calculation is going to be used for a condition, then the condition will be true when the calculation results in 255 (or anything else other than 0), and false when the calculation results in 0." + _
			                  Environment.NewLine + _
			                  Environment.NewLine + _
			                  "Examples:" + Environment.NewLine + _
			                  "70 > 80 results in 0" + Environment.NewLine + _
			                  "80 > 80 results in 0" + Environment.NewLine + _
			                  "90 > 80 results in 255"
			
		End Sub
		
		Private Sub AddLessEqualClick(sender As System.Object, e As System.EventArgs)
			Insert(" <= ")
		End Sub
		
		Private Sub AddLessEqualMouseEnter(sender As System.Object, e As System.EventArgs)
			tipDisplay.Text = "Add a less than or equal operation. If the value on the left of the <= is less than or the same as the value on the right, then this calculation will result in 255. Otherwise, it will result in 0." + _
			                  Environment.NewLine + _
			                  Environment.NewLine + _
			                  "Note that if this calculation is going to be used for a condition, then the condition will be true when the calculation results in 255 (or anything else other than 0), and false when the calculation results in 0." + _
			                  Environment.NewLine + _
			                  Environment.NewLine + _
			                  "Examples:" + Environment.NewLine + _
			                  "70 <= 80 results in 255" + Environment.NewLine + _
			                  "80 <= 80 results in 255" + Environment.NewLine + _
			                  "90 <= 80 results in 0"
			
		End Sub
		
		Private Sub AddMoreEqualClick(sender As System.Object, e As System.EventArgs)
			Insert(" >= ")
		End Sub
		
		Private Sub AddMoreEqualMouseEnter(sender As System.Object, e As System.EventArgs)
			tipDisplay.Text = "Add a more than or equal operation. If the value on the left of the >= is more than or the same as the value on the right, then this calculation will result in 255. Otherwise, it will result in 0." + _
			                  Environment.NewLine + _
			                  Environment.NewLine + _
			                  "Note that if this calculation is going to be used for a condition, then the condition will be true when the calculation results in 255 (or anything else other than 0), and false when the calculation results in 0." + _
			                  Environment.NewLine + _
			                  Environment.NewLine + _
			                  "Examples:" + Environment.NewLine + _
			                  "70 >= 80 results in 0" + Environment.NewLine + _
			                  "80 >= 80 results in 255" + Environment.NewLine + _
			                  "90 >= 80 results in 255"
			
		End Sub
		
		Private Sub AddNotClick(sender As System.Object, e As System.EventArgs)
			Insert("Not ")
		End Sub
		
		Private Sub AddNotMouseEnter(sender As System.Object, e As System.EventArgs)
			tipDisplay.Text = "Add a logic Not (complement) operation. It can be used in conditions, so that the condition will be true when what is after the Not is false." + Environment.NewLine + _
			                  Environment.NewLine + _
			                  "Note that Not only applies to the value after it, not to the value before it." + Environment.NewLine + _
			                  Environment.NewLine + _
			                  "Examples:" + Environment.NewLine + _
			                  "Not 0 results in 255" + Environment.NewLine + _
			                  "Not 255 results in 0"
			
		End Sub
		
		Private Sub AddAndClick(sender As System.Object, e As System.EventArgs)
			Insert(" And ")
		End Sub
		
		Private Sub AddAndMouseEnter(sender As System.Object, e As System.EventArgs)
			tipDisplay.Text = "Add a logic And operation. It can be used in conditions, so that the condition will only be true when what is before And and what is after And are both true." + Environment.NewLine + _
			                  Environment.NewLine + _
			                  "Examples:" + Environment.NewLine + _
			                  "0 And 0 results in 0" + Environment.NewLine + _
			                  "255 And 0 results in 0" + Environment.NewLine + _
			                  "255 And 255 results in 255"
			
		End Sub
		
		Private Sub AddOrClick(sender As System.Object, e As System.EventArgs)
			Insert(" Or ")
		End Sub
		
		Private Sub AddOrMouseEnter(sender As System.Object, e As System.EventArgs)
			tipDisplay.Text = "Add a logic Or operation. It can be used in conditions, so that the condition will be true if what is before Or and/or what is after Or are true." + Environment.NewLine + _
			                  Environment.NewLine + _
			                  "Examples:" + Environment.NewLine + _
			                  "0 Or 0 results in 0" + Environment.NewLine + _
			                  "255 Or 0 results in 255" + Environment.NewLine + _
			                  "255 Or 255 results in 255"
			
		End Sub
		
		Private Sub AddXorClick(sender As System.Object, e As System.EventArgs)
			Insert(" Xor ")
		End Sub
		
		Private Sub AddXorMouseEnter(sender As System.Object, e As System.EventArgs)
			tipDisplay.Text = "Add a logic Exclusive Or (Xor) operation. It can be used in conditions, so that the condition will only be true when either what is before Xor or what is after Xor is true." + Environment.NewLine + _
			                  Environment.NewLine + _
			                  "Examples:" + Environment.NewLine + _
			                  "0 Xor 0 results in 0" + Environment.NewLine + _
			                  "255 Xor 0 results in 255" + Environment.NewLine + _
			                  "255 Xor 255 results in 0"
			
		End Sub
		
	End Class
End Namespace
