'
' Created by SharpDevelop.
' User: Hugh
' Date: 2/05/2009
' Time: 1:08 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.ComponentModel
Imports System.Windows.Forms

''Namespace Great_Cow_Graphical_BASIC
	
	Public Class ParamEditBox
		Inherits System.Windows.Forms.UserControl
		Private ParamValue As System.Windows.Forms.ComboBox
		Private DividerLine As System.Windows.Forms.Label
		Private Description As System.Windows.Forms.Label
		Private EditButton As System.Windows.Forms.Button
		Private ParamName As System.Windows.Forms.Label
		
		Private Dim ThisParamChanged As Boolean = False
		Private Dim MinorParamChange As Boolean = False
		Public Event ParamChanged
		
		Public Property TitleFont As Font
			Get
				Return ParamName.Font
			End Get
			Set
				ParamName.Font = Value
			End Set
		End Property
		
		Public Property HasDivider As Boolean
			Get
				Return DividerLine.Visible
			End Get
			Set
				DividerLine.Visible = Value
			End Set
		End Property
		
		Private Dim pHasDescription As Boolean = True
		Public Property HasDescription As Boolean
			Get
				Return pHasDescription
			End Get
			Set
				pHasDescription = Value
				If Value <> Description.Visible Then
					If Value Then
						Me.Size = New System.Drawing.Size(Me.Size.Width, Me.Size.Height + 40)
					Else
						Me.Size = New System.Drawing.Size(Me.Size.Width, Me.Size.Height - 40)
					End If
				End If
				Description.Visible = Value
			End Set
		End Property
		
		Dim ParamType As String
		Dim Program As GCBProgram
		Dim CurrentSub As GCBSubroutine
		
		Public Sub New
			MyBase.New
			
			' Must be called for initialization
			Me.InitializeComponent
			
			'Translate
			If Not MainForm.Translator Is Nothing Then
				MainForm.Translator.TryTranslate("ParamEditBox", "Edit", EditButton.Text)
			End If
			
		End Sub
		
		Public Sub Init(Program As GCBProgram, CurrentSub As GCBSubroutine, ParamName As String, ParamValue As String, ParamType As String, Description As String, ShowDivider As Boolean, Optional ParamDest As String = "")
			Me.Program = Program
			Me.CurrentSub = CurrentSub
			
			Me.ParamName.Text = ParamName
			Me.ParamValue.Text = ParamValue
			Me.ParamType = ParamType
			Me.Description.Text = Description
			
			DividerLine.Visible = ShowDivider
			
			'Can the parameter be typed in?
			If LowLevel.CanTypeParams(ParamType) Then
				Me.ParamValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown
			Else
				Me.ParamValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			End If
			
			'Add or remove edit button as appropriate
			If Not LowLevel.CanEditParams(ParamType) Then
				Me.ParamValue.Size = New System.Drawing.Size(150, 21)
				Me.EditButton.Visible = False
			End If
			
			Program.FillParamList(CurrentSub, Me.ParamValue, ParamType, ParamValue, ParamDest)
			
		End Sub
		
		#Region " Windows Forms Designer generated code "
		' This method is required for Windows Forms designer support.
		' Do not change the method contents inside the source code editor. The Forms designer might
		' not be able to load this method if it was changed manually.
		Private Sub InitializeComponent()
			Me.ParamName = New System.Windows.Forms.Label
			Me.EditButton = New System.Windows.Forms.Button
			Me.Description = New System.Windows.Forms.Label
			Me.DividerLine = New System.Windows.Forms.Label
			Me.ParamValue = New System.Windows.Forms.ComboBox
			Me.SuspendLayout
			'
			'ParamName
			'
			Me.ParamName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.ParamName.Font = New System.Drawing.Font("Microsoft Sans Serif", 11!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World)
			Me.ParamName.Location = New System.Drawing.Point(0, 0)
			Me.ParamName.Name = "ParamName"
			Me.ParamName.Size = New System.Drawing.Size(152, 16)
			Me.ParamName.TabIndex = 0
			Me.ParamName.Text = "Name"
			AddHandler Me.ParamName.Click, AddressOf Me.ParamNameClick
			'
			'EditButton
			'
			Me.EditButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.EditButton.BackColor = System.Drawing.SystemColors.Control
			Me.EditButton.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.EditButton.Location = New System.Drawing.Point(104, 22)
			Me.EditButton.Name = "EditButton"
			Me.EditButton.Size = New System.Drawing.Size(48, 24)
			Me.EditButton.TabIndex = 2
			Me.EditButton.Text = "Edit..."
			AddHandler Me.EditButton.Click, AddressOf Me.EditButtonClick
			'
			'Description
			'
			Me.Description.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.Description.Location = New System.Drawing.Point(0, 56)
			Me.Description.Name = "Description"
			Me.Description.Size = New System.Drawing.Size(152, 40)
			Me.Description.TabIndex = 3
			Me.Description.Text = "Description"
			'
			'DividerLine
			'
			Me.DividerLine.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.DividerLine.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
			Me.DividerLine.Location = New System.Drawing.Point(0, 95)
			Me.DividerLine.Name = "DividerLine"
			Me.DividerLine.Size = New System.Drawing.Size(152, 1)
			Me.DividerLine.TabIndex = 4
			'
			'ParamValue
			'
			Me.ParamValue.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.ParamValue.Location = New System.Drawing.Point(0, 24)
			Me.ParamValue.Name = "ParamValue"
			Me.ParamValue.Size = New System.Drawing.Size(104, 21)
			Me.ParamValue.Sorted = true
			Me.ParamValue.TabIndex = 1
			AddHandler Me.ParamValue.TextChanged, AddressOf Me.ParamValueTextChanged
			AddHandler Me.ParamValue.SelectedIndexChanged, AddressOf Me.ParamValueSelectedIndexChanged
			'
			'ParamEditBox
			'
			Me.BackColor = System.Drawing.SystemColors.Window
			Me.Controls.Add(Me.DividerLine)
			Me.Controls.Add(Me.Description)
			Me.Controls.Add(Me.EditButton)
			Me.Controls.Add(Me.ParamValue)
			Me.Controls.Add(Me.ParamName)
			Me.Name = "ParamEditBox"
			Me.Size = New System.Drawing.Size(152, 96)
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
		Private Sub ParamNameClick(sender As System.Object, e As System.EventArgs)
			
			
		End Sub
		
		Private Sub EditButtonClick(sender As System.Object, e As System.EventArgs)
			Dim fParamEditor As New ParameterEditor
			fParamEditor.Init(Program, ParamName.Text, ParamType, ParamValue.Text, CurrentSub)
			fParamEditor.ShowDialog
			
			If fParamEditor.Accepted Then
				ParamValue.Text = fParamEditor.ParamValue
			End If
			
		End Sub
		
		Public Sub SetValue(Value As String, Optional MinorParamChange As Boolean = False)
			If ParamValue.Text <> Value Then
				ParamValue.Text = Value
				ThisParamChanged = True
				Me.MinorParamChange = MinorParamChange
				RaiseEvent ParamChanged
			End If
		End Sub
		
		Public Function GetValue As String
			Return ParamValue.Text
		End Function
		
		Private Sub ParamValueTextChanged(sender As System.Object, e As System.EventArgs)
			ThisParamChanged = True
			MinorParamChange = True
			RaiseEvent ParamChanged
			
		End Sub
		
		Private Sub ParamValueSelectedIndexChanged(sender As System.Object, e As System.EventArgs)
			ThisParamChanged = True
			RaiseEvent ParamChanged
		End Sub
		
		Public Function HasParamChanged As Boolean
			Return ThisParamChanged
		End Function
		
		Public Function IsMinorChange As Boolean
			Return MinorParamChange
		End Function
		
		Public Sub ClearParamChanged
			ThisParamChanged = False
			MinorParamChange = False
		End Sub
		
	End Class
''End Namespace
