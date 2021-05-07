'
' Created by SharpDevelop.
' User: Hugh
' Date: 13/01/2010
' Time: 2:44 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.ComponentModel
Imports System.Windows.Forms

Namespace Great_Cow_Graphical_BASIC
	
	Public Class PEChipPin
		Inherits System.Windows.Forms.UserControl
		Implements ParamEditPage
		Private chipImage As Great_Cow_Graphical_BASIC.ChipDiagram
		Private panel1 As System.Windows.Forms.Panel
		Private labelPreview As System.Windows.Forms.Label
		Private chipPinsList As System.Windows.Forms.ListBox
		Private labelPin As System.Windows.Forms.Label
		Private choosePinPrompt As System.Windows.Forms.Label
		
		Private Dim Frame As ParameterEditor
		Private Dim CurrentChip As ChipInfo
		Private Dim Value As String
		
		'Stores the next page ID. Empty string if no next page
		Public ReadOnly Property NextPage As String Implements ParamEditPage.NextPage
			Get
				Return ""
			End Get
		End Property
		
		'Stores the output value for parameter. Empty string if not last page
		Public ReadOnly Property OutputValue As String Implements ParamEditPage.OutputValue
			Get
				Return Value
			End Get
		End Property
		
		Public Sub Init (Frame As ParameterEditor, ParamType As String, CurrentSource As String) Implements ParamEditPage.Init
			Me.Frame = Frame
			
			'Init chip display
			CurrentChip = Frame.Program.ChipData
			chipImage.Init(ChipDiagram.Modes.Pin, CurrentChip, Frame.Program)
			
			'Change Next button to Finish
			Frame.NextIsFinish = True
			
			'Fill pin list
			'Program.FillParamList requires a combobox, but we have a listbox
			'Easiest solution is a hackish method: create combobox, fill, copy items to listbox
			Dim tempComboBox As New ComboBox
			Frame.Program.FillParamList(Frame.CurrentSub, tempComboBox, "io_pin", "")
			Dim currItem As String
			chipPinsList.Sorted = True
			For Each currItem In tempComboBox.Items
				chipPinsList.Items.Add(currItem)
				'If this pin is the current one, select it
				If CurrentSource <> "" Then
					If CurrentSource.Trim.ToLower = currItem.Trim.ToLower Then
						'chipPinsList.SelectedIndex = chipPinsList.Items.Count - 1
						chipPinsList.SelectedItem = currItem
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
			Me.choosePinPrompt = New System.Windows.Forms.Label
			Me.labelPin = New System.Windows.Forms.Label
			Me.chipPinsList = New System.Windows.Forms.ListBox
			Me.labelPreview = New System.Windows.Forms.Label
			Me.panel1 = New System.Windows.Forms.Panel
			Me.chipImage = New Great_Cow_Graphical_BASIC.ChipDiagram
			Me.panel1.SuspendLayout
			Me.SuspendLayout
			'
			'choosePinPrompt
			'
			Me.choosePinPrompt.Location = New System.Drawing.Point(8, 8)
			Me.choosePinPrompt.Name = "choosePinPrompt"
			Me.choosePinPrompt.Size = New System.Drawing.Size(224, 16)
			Me.choosePinPrompt.TabIndex = 1
			Me.choosePinPrompt.Text = "Which input/output pin should be used?"
			'
			'labelPin
			'
			Me.labelPin.Location = New System.Drawing.Point(8, 32)
			Me.labelPin.Name = "labelPin"
			Me.labelPin.Size = New System.Drawing.Size(104, 16)
			Me.labelPin.TabIndex = 5
			Me.labelPin.Text = "Pin:"
			'
			'chipPinsList
			'
			Me.chipPinsList.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
			Me.chipPinsList.IntegralHeight = false
			Me.chipPinsList.Location = New System.Drawing.Point(8, 48)
			Me.chipPinsList.Name = "chipPinsList"
			Me.chipPinsList.Size = New System.Drawing.Size(128, 264)
			Me.chipPinsList.TabIndex = 4
			AddHandler Me.chipPinsList.DoubleClick, AddressOf Me.ChipPinsListDoubleClick
			AddHandler Me.chipPinsList.SelectedIndexChanged, AddressOf Me.ChipPinsListSelectedIndexChanged
			'
			'labelPreview
			'
			Me.labelPreview.Location = New System.Drawing.Point(144, 32)
			Me.labelPreview.Name = "labelPreview"
			Me.labelPreview.Size = New System.Drawing.Size(112, 16)
			Me.labelPreview.TabIndex = 3
			Me.labelPreview.Text = "Preview:"
			'
			'panel1
			'
			Me.panel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
			Me.panel1.Controls.Add(Me.chipImage)
			Me.panel1.Location = New System.Drawing.Point(144, 48)
			Me.panel1.Name = "panel1"
			Me.panel1.Size = New System.Drawing.Size(288, 264)
			Me.panel1.TabIndex = 2
			'
			'chipImage
			'
			Me.chipImage.BackColor = System.Drawing.Color.White
			Me.chipImage.Dock = System.Windows.Forms.DockStyle.Fill
			Me.chipImage.Location = New System.Drawing.Point(0, 0)
			Me.chipImage.Name = "chipImage"
			Me.chipImage.Size = New System.Drawing.Size(284, 260)
			Me.chipImage.TabIndex = 0
			Me.chipImage.Value = ""
			'
			'PEChipPin
			'
			Me.Controls.Add(Me.labelPin)
			Me.Controls.Add(Me.chipPinsList)
			Me.Controls.Add(Me.labelPreview)
			Me.Controls.Add(Me.panel1)
			Me.Controls.Add(Me.choosePinPrompt)
			Me.Name = "PEChipPin"
			Me.Size = New System.Drawing.Size(440, 320)
			Me.panel1.ResumeLayout(false)
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
		Private Sub ChipPinsListSelectedIndexChanged(sender As System.Object, e As System.EventArgs)
			Value = ChipPinsList.SelectedItem
			chipImage.Value = Value
			
			If ChipPinsList.SelectedIndex <> -1 Then
				Frame.AllowNext = True
			Else
				Frame.AllowNext = False
			End If
			
		End Sub
		
		Private Sub ChipPinsListDoubleClick(sender As System.Object, e As System.EventArgs)
			Frame.GoToNextPage
			
		End Sub
		
	End Class
End Namespace
