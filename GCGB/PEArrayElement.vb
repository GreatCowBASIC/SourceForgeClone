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

''Namespace Great_Cow_Graphical_BASIC
	
	Public Class PEArrayElement
		Inherits System.Windows.Forms.UserControl
		Implements ParamEditPage
		Private rightPanel As System.Windows.Forms.Panel
		Private varList As System.Windows.Forms.ListBox
		Private arrayElement As Great_Cow_Graphical_BASIC.ParamEditBox
		Private labelArrays As System.Windows.Forms.Label
		Private leftPanel As System.Windows.Forms.Panel
		Private addArray As System.Windows.Forms.Button
		Private panelSplitter As System.Windows.Forms.Splitter
		
		Private Dim Frame As ParameterEditor
		Private Dim elementOK, arrayOK As Boolean
		Private Dim outputArray As String = ""
		
		'Stores the next page ID. Empty string if no next page
		Public ReadOnly Property NextPage As String Implements ParamEditPage.NextPage
			Get
				Return ""
			End Get
		End Property
		
		'Stores the output value for parameter. Empty string if not last page
		Public ReadOnly Property OutputValue As String Implements ParamEditPage.OutputValue
			Get
				Return outputArray + "(" + ArrayElement.GetValue + ")"
			End Get
		End Property
		
		Public Sub Init (Frame As ParameterEditor, ParamType As String, CurrentSource As String) Implements ParamEditPage.Init
			Me.Frame = Frame
			
			Frame.NextIsFinish = True
			Frame.AllowNext = False
			
			arrayOK = False
			elementOK = False
			
			'Draw list of arrays
			Dim CurrVar As VariableListItem
			Dim Temp As Integer
			With Frame.Program
				For Temp = 0 To .Variables.Count - 1
					CurrVar = .Variables.GetItem(Temp)
					If CurrVar.IsArray Then
						Me.VarList.Items.Add(CurrVar.Name)
					End If
				Next
			End With
			If Not Frame.CurrentSub Is Nothing THen
				Dim currParam As SubParam
				For Each currParam In Frame.CurrentSub.Parameters
					If (ParamType = "var" Or ParamType = "number" Or ParamType.EndsWith(currParam.DataType)) And currParam.DataType.IndexOf("(") = -1 Then
						Me.VarList.Items.Add(currParam.Name)
					End If
				Next
			End If
			
			Me.VarList.Sorted = True
			
			ArrayElement.Init(Frame.Program, Frame.CurrentSub, "Array Element", "", "byte", "", False)
			
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
			Me.panelSplitter = New System.Windows.Forms.Splitter
			Me.addArray = New System.Windows.Forms.Button
			Me.leftPanel = New System.Windows.Forms.Panel
			Me.labelArrays = New System.Windows.Forms.Label
			Me.arrayElement = New Great_Cow_Graphical_BASIC.ParamEditBox
			Me.varList = New System.Windows.Forms.ListBox
			Me.rightPanel = New System.Windows.Forms.Panel
			Me.leftPanel.SuspendLayout
			Me.rightPanel.SuspendLayout
			Me.SuspendLayout
			'
			'panelSplitter
			'
			Me.panelSplitter.Dock = System.Windows.Forms.DockStyle.Right
			Me.panelSplitter.Location = New System.Drawing.Point(217, 0)
			Me.panelSplitter.Name = "panelSplitter"
			Me.panelSplitter.Size = New System.Drawing.Size(7, 272)
			Me.panelSplitter.TabIndex = 1
			Me.panelSplitter.TabStop = false
			'
			'addArray
			'
			Me.addArray.Anchor = System.Windows.Forms.AnchorStyles.Bottom
			Me.addArray.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.addArray.Location = New System.Drawing.Point(48, 232)
			Me.addArray.Name = "addArray"
			Me.addArray.Size = New System.Drawing.Size(120, 24)
			Me.addArray.TabIndex = 2
			Me.addArray.Text = "Add new array ..."
			AddHandler Me.addArray.Click, AddressOf Me.AddArrayClick
			'
			'leftPanel
			'
			Me.leftPanel.Controls.Add(Me.addArray)
			Me.leftPanel.Controls.Add(Me.varList)
			Me.leftPanel.Controls.Add(Me.labelArrays)
			Me.leftPanel.Dock = System.Windows.Forms.DockStyle.Fill
			Me.leftPanel.Location = New System.Drawing.Point(0, 0)
			Me.leftPanel.Name = "leftPanel"
			Me.leftPanel.Size = New System.Drawing.Size(217, 272)
			Me.leftPanel.TabIndex = 2
			'
			'labelArrays
			'
			Me.labelArrays.Location = New System.Drawing.Point(8, 8)
			Me.labelArrays.Name = "labelArrays"
			Me.labelArrays.Size = New System.Drawing.Size(200, 16)
			Me.labelArrays.TabIndex = 0
			Me.labelArrays.Text = "Array to use:"
			'
			'arrayElement
			'
			Me.arrayElement.BackColor = System.Drawing.SystemColors.Control
			Me.arrayElement.HasDescription = false
			Me.arrayElement.HasDivider = false
			Me.arrayElement.Location = New System.Drawing.Point(8, 8)
			Me.arrayElement.Name = "arrayElement"
			Me.arrayElement.Size = New System.Drawing.Size(184, 56)
			Me.arrayElement.TabIndex = 0
			Me.arrayElement.TitleFont = New System.Drawing.Font("Tahoma", 11!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World)
			AddHandler Me.arrayElement.ParamChanged, AddressOf Me.ArrayElementParamChanged
			'
			'varList
			'
			Me.varList.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.varList.IntegralHeight = false
			Me.varList.Location = New System.Drawing.Point(8, 24)
			Me.varList.Name = "varList"
			Me.varList.Size = New System.Drawing.Size(200, 200)
			Me.varList.TabIndex = 1
			AddHandler Me.varList.SelectedIndexChanged, AddressOf Me.VarListSelectedIndexChanged
			'
			'rightPanel
			'
			Me.rightPanel.Controls.Add(Me.arrayElement)
			Me.rightPanel.Dock = System.Windows.Forms.DockStyle.Right
			Me.rightPanel.Location = New System.Drawing.Point(224, 0)
			Me.rightPanel.Name = "rightPanel"
			Me.rightPanel.Size = New System.Drawing.Size(200, 272)
			Me.rightPanel.TabIndex = 0
			'
			'PEArrayElement
			'
			Me.Controls.Add(Me.leftPanel)
			Me.Controls.Add(Me.panelSplitter)
			Me.Controls.Add(Me.rightPanel)
			Me.Name = "PEArrayElement"
			Me.Size = New System.Drawing.Size(424, 272)
			Me.leftPanel.ResumeLayout(false)
			Me.rightPanel.ResumeLayout(false)
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
		Private Sub AddArrayClick(sender As System.Object, e As System.EventArgs)
			Dim SearchList As Integer = 0
			Dim VarExists As Boolean = False
			Dim TempData As String = ""
			
			'Display dialog
			Dim fEditVar As New EditVar
			fEditVar.Text = "Add Array Variable"
			fEditVar.InitEdit("", "array", -3)
			fEditVar.ShowDialog()
			
			'Apply
			If fEditVar.NewVarName <> "" Then
				TempData = fEditVar.NewVarType
				If TempData = "array" Then
					Frame.Program.Variables.AddVar(fEditVar.NewVarName.Trim, "byte", True, fEditVar.NewArraySize)
					Me.VarList.Items.Add(fEditVar.NewVarName)
					Me.VarList.SelectedIndex = Me.VarList.Items.Count - 1
				Else
					Frame.Program.Variables.AddVar(fEditVar.NewVarName.Trim, TempData)
				End If
				
			End If
			
		End Sub
		
		Private Sub ArrayElementParamChanged()
			elementOK = False
			If ArrayElement.GetValue <> "" Then elementOK = True
			Frame.AllowNext = (arrayOK And elementOK)
			
		End Sub
		
		Private Sub VarListSelectedIndexChanged(sender As System.Object, e As System.EventArgs)
			arrayOK = False
			If VarList.SelectedIndex <> -1 Then arrayOK = True
			Frame.AllowNext = (arrayOK And elementOK)
			outputArray = VarList.SelectedItem
		End Sub
		
	End Class
'End Namespace
