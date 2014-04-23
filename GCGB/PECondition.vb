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

'Namespace Great_Cow_Graphical_BASIC
	
	Public Class PECondition
		Inherits System.Windows.Forms.UserControl
		Implements ParamEditPage
		Private label2 As System.Windows.Forms.Label
		Private label1 As System.Windows.Forms.Label
		Private secondValue As Great_Cow_Graphical_BASIC.ParamEditBox
		Private panel2 As System.Windows.Forms.Panel
		Private compareMode As System.Windows.Forms.ComboBox
		Private panel4 As System.Windows.Forms.Panel
		Private firstValue As Great_Cow_Graphical_BASIC.ParamEditBox
		
		Dim Frame As ParameterEditor
		Dim OutVal As String
		
		'Stores the next page ID. Empty string if no next page
		Public ReadOnly Property NextPage As String Implements ParamEditPage.NextPage
			Get
				Return ""
			End Get
		End Property
		
		'Stores the output value for parameter. Empty string if not last page
		Public ReadOnly Property OutputValue As String Implements ParamEditPage.OutputValue
			Get
				Return OutVal
			End Get
		End Property
		
		Public Sub Init (Frame As ParameterEditor, ParamType As String, CurrentSource As String) Implements ParamEditPage.Init
			Me.Frame = Frame
			
			firstValue.Init(Frame.Program, Frame.CurrentSub, "First Value", "", "bit|number|string", "", False) 
			secondValue.Init(Frame.Program, Frame.CurrentSub, "Second Value", "", "bit|number|string", "", False) 
			
			compareMode.SelectedIndex = 0
			
			Frame.NextIsFinish = True
			Frame.AllowNext = False
			
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
			Me.firstValue = New Great_Cow_Graphical_BASIC.ParamEditBox
			Me.panel4 = New System.Windows.Forms.Panel
			Me.compareMode = New System.Windows.Forms.ComboBox
			Me.panel2 = New System.Windows.Forms.Panel
			Me.secondValue = New Great_Cow_Graphical_BASIC.ParamEditBox
			Me.label1 = New System.Windows.Forms.Label
			Me.label2 = New System.Windows.Forms.Label
			Me.panel4.SuspendLayout
			Me.panel2.SuspendLayout
			Me.SuspendLayout
			'
			'firstValue
			'
			Me.firstValue.BackColor = System.Drawing.SystemColors.Control
			Me.firstValue.Dock = System.Windows.Forms.DockStyle.Fill
			Me.firstValue.DockPadding.All = 2
			Me.firstValue.HasDescription = false
			Me.firstValue.HasDivider = false
			Me.firstValue.Location = New System.Drawing.Point(0, 0)
			Me.firstValue.Name = "firstValue"
			Me.firstValue.Size = New System.Drawing.Size(232, 56)
			Me.firstValue.TabIndex = 1
			Me.firstValue.TitleFont = New System.Drawing.Font("Tahoma", 11!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World)
			AddHandler Me.firstValue.ParamChanged, AddressOf Me.FirstValueParamChanged
			'
			'panel4
			'
			Me.panel4.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.panel4.Controls.Add(Me.secondValue)
			Me.panel4.Location = New System.Drawing.Point(16, 136)
			Me.panel4.Name = "panel4"
			Me.panel4.Size = New System.Drawing.Size(232, 56)
			Me.panel4.TabIndex = 3
			'
			'compareMode
			'
			Me.compareMode.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.compareMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me.compareMode.Items.AddRange(New Object() {"equal to", "not equal to", "less than", "more than", "less than or equal to", "more than or equal to"})
			Me.compareMode.Location = New System.Drawing.Point(40, 104)
			Me.compareMode.Name = "compareMode"
			Me.compareMode.Size = New System.Drawing.Size(208, 21)
			Me.compareMode.TabIndex = 4
			'
			'panel2
			'
			Me.panel2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.panel2.Controls.Add(Me.firstValue)
			Me.panel2.Location = New System.Drawing.Point(16, 32)
			Me.panel2.Name = "panel2"
			Me.panel2.Size = New System.Drawing.Size(232, 56)
			Me.panel2.TabIndex = 3
			'
			'secondValue
			'
			Me.secondValue.BackColor = System.Drawing.SystemColors.Control
			Me.secondValue.Dock = System.Windows.Forms.DockStyle.Fill
			Me.secondValue.DockPadding.All = 2
			Me.secondValue.HasDescription = false
			Me.secondValue.HasDivider = true
			Me.secondValue.Location = New System.Drawing.Point(0, 0)
			Me.secondValue.Name = "secondValue"
			Me.secondValue.Size = New System.Drawing.Size(232, 56)
			Me.secondValue.TabIndex = 1
			Me.secondValue.TitleFont = New System.Drawing.Font("Tahoma", 11!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World)
			AddHandler Me.secondValue.ParamChanged, AddressOf Me.SecondValueParamChanged
			'
			'label1
			'
			Me.label1.Location = New System.Drawing.Point(8, 8)
			Me.label1.Name = "label1"
			Me.label1.Size = New System.Drawing.Size(120, 16)
			Me.label1.TabIndex = 0
			Me.label1.Text = "Condition will be true if"
			'
			'label2
			'
			Me.label2.Location = New System.Drawing.Point(16, 104)
			Me.label2.Name = "label2"
			Me.label2.Size = New System.Drawing.Size(24, 16)
			Me.label2.TabIndex = 3
			Me.label2.Text = "is"
			Me.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
			'
			'PECondition
			'
			Me.Controls.Add(Me.compareMode)
			Me.Controls.Add(Me.label2)
			Me.Controls.Add(Me.label1)
			Me.Controls.Add(Me.panel4)
			Me.Controls.Add(Me.panel2)
			Me.Name = "PECondition"
			Me.Size = New System.Drawing.Size(264, 208)
			Me.panel4.ResumeLayout(false)
			Me.panel2.ResumeLayout(false)
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
		Private Function GetOperator As String
			Select Case compareMode.SelectedIndex
				Case 0: Return " = "
				Case 1: Return " <> "
				Case 2: Return " < "
				Case 3: Return " > "
				Case 4: Return " <= "
				Case 5: Return " >= "
				Case Else: Return ""
			End Select
		End Function
		
		Private Sub FirstValueParamChanged()
			OutVal = firstValue.GetValue + GetOperator + secondValue.GetValue
			
			If firstValue.GetValue <> "" And secondValue.GetValue <> "" Then
				Frame.AllowNext = True
			Else
				Frame.AllowNext = False
			End If
		End Sub
		
		Private Sub SecondValueParamChanged()
			OutVal = firstValue.GetValue + GetOperator + secondValue.GetValue
			
			If firstValue.GetValue <> "" And secondValue.GetValue <> "" Then
				Frame.AllowNext = True
			Else
				Frame.AllowNext = False
			End If
			
		End Sub
		
	End Class
'End Namespace
