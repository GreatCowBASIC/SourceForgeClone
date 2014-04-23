'
' Created by SharpDevelop.
' User: Hugh
' Date: 13/12/2009
' Time: 6:15 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.ComponentModel
Imports System.Windows.Forms

'Namespace Great_Cow_Graphical_BASIC
	
	Public Class PEChooseSource
		Inherits System.Windows.Forms.UserControl
		Implements ParamEditPage
		Private label1 As System.Windows.Forms.Label
		Private sourceChoices As Great_Cow_Graphical_BASIC.CaptionedListBox
		
		Private Frame As ParameterEditor
		
		Private Dim ParamCondAdded, ParamStrAdded, ParamArrayAdded, ParamIOPortAdded, ParamIOPinAdded As Boolean
		Private Dim ParamBitAdded, ParamLiteralAdded, ParamVarAdded, ParamArrayElementAdded As Boolean
		Private Dim ParamFNAdded, ParamConstAdded, ParamCalcAdded As Boolean
		
		Public ReadOnly Property NextPage As String Implements ParamEditPage.NextPage
			Get
				Return sourceChoices.SelectedID
			End Get
		End Property
		
		Public ReadOnly Property OutputValue As String Implements ParamEditPage.OutputValue
			Get
				Return ""
			End Get
		End Property
		
		'Returns true if there is only a single option
		Public ReadOnly Property SingleOption As Boolean
			Get
				If sourceChoices.Count = 1 Then
					Return True
				Else
					Return False
				End If
			End Get
		End Property
		
		'Returns the first option
		Public ReadOnly Property FirstOption As String
			Get
				Return sourceChoices.FirstID
			End Get
		End Property
		
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
			Me.sourceChoices = New Great_Cow_Graphical_BASIC.CaptionedListBox
			Me.label1 = New System.Windows.Forms.Label
			Me.SuspendLayout
			'
			'sourceChoices
			'
			Me.sourceChoices.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.sourceChoices.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
			Me.sourceChoices.ListFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
			Me.sourceChoices.Location = New System.Drawing.Point(8, 32)
			Me.sourceChoices.Name = "sourceChoices"
			Me.sourceChoices.SelectedID = ""
			Me.sourceChoices.SelectedIndex = -1
			Me.sourceChoices.Size = New System.Drawing.Size(296, 240)
			Me.sourceChoices.TabIndex = 0
			AddHandler Me.sourceChoices.ListDoubleClick, AddressOf Me.SourceChoicesListDoubleClick
			AddHandler Me.sourceChoices.SelectedIndexChanged, AddressOf Me.SourceChoicesSelectedIndexChanged
			'
			'label1
			'
			Me.label1.Location = New System.Drawing.Point(8, 8)
			Me.label1.Name = "label1"
			Me.label1.Size = New System.Drawing.Size(232, 16)
			Me.label1.TabIndex = 1
			Me.label1.Text = "Where should the value come from?"
			'
			'PEChooseSource
			'
			Me.Controls.Add(Me.label1)
			Me.Controls.Add(Me.sourceChoices)
			Me.Name = "PEChooseSource"
			Me.Size = New System.Drawing.Size(312, 280)
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
		Public Sub Init (Frame As ParameterEditor, ParamType As String, CurrentSource As String) Implements ParamEditPage.Init
			'Need to show only relevant choices based on ParamType
			'Should also highlight the currently selected data source
			
			Me.Frame = Frame
			
			'Add items
			Dim ThisType As String
			For Each ThisType In ParamType.Split("|")
				ThisType = ThisType.Trim.ToLower
								
				'Condition
				If ThisType = "condition" And Not ParamCondAdded Then
					ParamCondAdded = True
					
					'Options: simple condition, calculation, or function
					sourceChoices.Add(New CaptionedListItem("simplecondition", "Simple Condition", "Compare two values, and check if they are the same or if one is lower or higher"))
					sourceChoices.Add(New CaptionedListItem("calculation", "Complex Condition", "Perform a calculation. The condition will be true if the result is not 0."))
					sourceChoices.Add(New CaptionedListItem("function", "Function", "Run a function. The condition will be true if the result is not 0."))
					
				End If
				
				'String
				If ThisType = "string" And Not ParamStrAdded Then
					ParamStrAdded = True
					sourceChoices.Add(New CaptionedListItem("const_string", "Fixed string", "Use a fixed message"))
					If Not ParamVarAdded Then
						sourceChoices.Add(New CaptionedListItem("var_string", "String variable", "Get a message from a String type variable"))
					End If
					
				End If
				
				'Array
				If ThisType = "array" And Not ParamArrayAdded Then
					ParamArrayAdded = True
					sourceChoices.Add(New CaptionedListItem("var_array", "Array variable", "Use an existing array variable, or make a new one"))
					
				End If
				
				'IOPin
				If ThisType = "io_pin" Or ThisType = "bit" And Not ParamIOPinAdded Then
					ParamIOPinAdded = True
					sourceChoices.Add(New CaptionedListItem("io_pin", "IO Pin", "Use a single input/output pin."))
				End If
				
				'IOPort
				If ThisType = "io_port" And Not ParamIOPortAdded Then
					ParamIOPortAdded = True
					sourceChoices.Add(New CaptionedListItem("io_port", "IO Port", "Select an entire input/output port."))
					
				End If
				
				'Bit variable
				If ThisType = "bit" And Not ParamBitAdded Then
					ParamBitAdded = True
					sourceChoices.Add(New CaptionedListItem("bit", "Bit from a variable", "Use a single bit from a variable"))
					If Not ParamVarAdded Then
						sourceChoices.Add(New CaptionedListItem("var_bit", "Variable", "Use a Bit type variable"))
					End If
					
				End If
				
				'Literal
				If ThisType = "const" Or ThisType = "const_byte" Or ThisType = "const_word" Or _
				   ThisType = "const_integer" Or ThisType = "integer" Or _
				   ThisType = "const_long" Or ThisType = "long" Or _
				   ThisType = "number" Or ThisType = "byte" Or ThisType = "word" Or ThisType.StartsWith("range") Then
					If Not ParamLiteralAdded Then
						ParamLiteralAdded = True
						sourceChoices.Add(New CaptionedListItem("const_num", "Fixed number", "Use a fixed number"))
						
					End If
				End If
				
				'Variable
				If ThisType = "var" Or ThisType = "var_byte" Or ThisType = "var_word" Or _
				   ThisType = "var_integer" Or ThisType = "integer" Or _
				   ThisType = "var_long" Or ThisType = "long" Or _
				   ThisType = "number" Or ThisType = "byte" Or ThisType = "word" Then
					
					If Not ParamVarAdded Then
						ParamVarAdded = True
						sourceChoices.Add(New CaptionedListItem("var", "Variable", "Use a suitable variable"))
						sourceChoices.Remove("var_string")
						sourceChoices.Remove("var_bit")
						
					End If
				End If
				
				'Calculation
				If ThisType = "number" Or ThisType = "byte" Or ThisType = "word" Or _
				   ThisType = "integer" Or ThisType = "long" Then
					
					If Not ParamCalcAdded Then
						ParamCalcAdded = True
						sourceChoices.Add(New CaptionedListItem("calculation", "Calculation", "Perform a calculation, and use the result."))
					End If
					
				End If
				
				'Array Element
				If ThisType = "var" Or ThisType = "var_byte" Or ThisType = "var_word" Or _
				   ThisType = "var_integer" Or ThisType = "integer" Or _
				   ThisType = "var_long" Or ThisType = "long" Or _
				   ThisType = "number" Or ThisType = "byte" Or ThisType = "word" Then
					
					If Not ParamArrayElementAdded Then
						ParamArrayElementAdded = True
						sourceChoices.Add(New CaptionedListItem("array_element", "Array Element", "Use a particular element from an array."))
						
					End If
				End If
				
				'Function
				If ThisType = "byte" Or ThisType = "word" Or ThisType = "number" Or _
				   ThisType = "integer" Or ThisType = "long" Or ThisType = "string" Then
					
					If Not ParamFNAdded Then
						ParamFNAdded = True
						sourceChoices.Add(New CaptionedListItem("gcbfunction", "Built-in Function", "Run a built-in function from GCGB, and use the result from it."))
						sourceChoices.Add(New CaptionedListItem("function", "Function", "Run a function from this program or a library, and use the result from it."))
						
					End If
				End If
				
			Next
			
			
			'Disable next button until a choice is made
			Frame.AllowNext = False
			
		End Sub
		
		Private Sub SourceChoicesSelectedIndexChanged(sender As System.Object, e As System.EventArgs)
			
			Frame.AllowNext = True
			
		End Sub
		
		Private Sub SourceChoicesListDoubleClick()
			'If an item in the list is double clicked, load next page in parameter editor
			Frame.GoToNextPage
			
		End Sub
		
	End Class
'End Namespace
