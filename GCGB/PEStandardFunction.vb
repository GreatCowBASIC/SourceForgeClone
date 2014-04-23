'
' Created by SharpDevelop.
' User: Hugh
' Date: 18/01/2010
' Time: 12:45 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.ComponentModel
Imports System.Windows.Forms

'Namespace Great_Cow_Graphical_BASIC
	
	Public Class PEStandardFunction
		Inherits System.Windows.Forms.UserControl
		Implements ParamEditPage
		Private rightPanel As System.Windows.Forms.Panel
		Private helpButton As System.Windows.Forms.Button
		Private fnLabel As System.Windows.Forms.Label
		Private functionList As Great_Cow_Graphical_BASIC.CaptionedListBox
		Private fnSettingsLabel As System.Windows.Forms.Label
		Private settingsPanel As System.Windows.Forms.Panel
		Private leftPanel As System.Windows.Forms.Panel
		Private functionMessage As System.Windows.Forms.Label
		Private panelSplitter As System.Windows.Forms.Splitter
		
		Dim Frame As ParameterEditor
		Dim currentFunction As GCBFunction
		
		'Stores the next page ID. Empty string if no next page
		Public ReadOnly Property NextPage As String Implements ParamEditPage.NextPage
			Get
				Return ""
			End Get
		End Property
		
		'Stores the output value for parameter. Empty string if not last page
		Public ReadOnly Property OutputValue As String Implements ParamEditPage.OutputValue
			Get
				Dim OutVal As String
				
				'Function name
				OutVal = currentFunction.Name
				
				'Function parameters
				If currentFunction.Params.Count > 0 Then
					OutVal += "("
					Dim currControl As Control
					Dim currEditor As ParamEditBox
					Dim isFirstParam As Boolean = True
					For Each currControl In settingsPanel.Controls
						If TypeOf currControl Is ParamEditBox Then
							
							If Not isFirstParam Then
								OutVal += ", "
							Else
								isFirstParam = False
							End If
							
							currEditor = currControl
							OutVal += currEditor.GetValue
						End If
					Next
					OutVal += ")"
				End If
				
				Return OutVal
			End Get
		End Property
		
		Public Sub Init (Frame As ParameterEditor, ParamType As String, CurrentSource As String) Implements ParamEditPage.Init
			Me.Frame = Frame
			
			Frame.AllowNext = False
			Frame.NextIsFinish = True
			currentFunction = Nothing
			
			'Fill function list
			Dim CurrFunction As GCBFunction
			For Each CurrFunction In MainForm.GCBFunctions
				With CurrFunction
					If Lowlevel.CanCopy(.ReturnType, ParamType) Then
						functionList.Add(New CaptionedListItem(.Name, .Name, .Description, CurrFunction))
					End If
				End With
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
			Me.panelSplitter = New System.Windows.Forms.Splitter
			Me.functionMessage = New System.Windows.Forms.Label
			Me.leftPanel = New System.Windows.Forms.Panel
			Me.settingsPanel = New System.Windows.Forms.Panel
			Me.fnSettingsLabel = New System.Windows.Forms.Label
			Me.functionList = New Great_Cow_Graphical_BASIC.CaptionedListBox
			Me.fnLabel = New System.Windows.Forms.Label
			Me.helpButton = New System.Windows.Forms.Button
			Me.rightPanel = New System.Windows.Forms.Panel
			Me.leftPanel.SuspendLayout
			Me.settingsPanel.SuspendLayout
			Me.rightPanel.SuspendLayout
			Me.SuspendLayout
			'
			'panelSplitter
			'
			Me.panelSplitter.Dock = System.Windows.Forms.DockStyle.Right
			Me.panelSplitter.Location = New System.Drawing.Point(213, 0)
			Me.panelSplitter.Name = "panelSplitter"
			Me.panelSplitter.Size = New System.Drawing.Size(3, 288)
			Me.panelSplitter.TabIndex = 1
			Me.panelSplitter.TabStop = false
			'
			'functionMessage
			'
			Me.functionMessage.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.functionMessage.Location = New System.Drawing.Point(8, 8)
			Me.functionMessage.Name = "functionMessage"
			Me.functionMessage.Size = New System.Drawing.Size(172, 216)
			Me.functionMessage.TabIndex = 0
			Me.functionMessage.Text = "Please choose a function"
			'
			'leftPanel
			'
			Me.leftPanel.BackColor = System.Drawing.SystemColors.Control
			Me.leftPanel.Controls.Add(Me.functionList)
			Me.leftPanel.Controls.Add(Me.fnLabel)
			Me.leftPanel.Dock = System.Windows.Forms.DockStyle.Fill
			Me.leftPanel.Location = New System.Drawing.Point(0, 0)
			Me.leftPanel.Name = "leftPanel"
			Me.leftPanel.Size = New System.Drawing.Size(213, 288)
			Me.leftPanel.TabIndex = 2
			'
			'settingsPanel
			'
			Me.settingsPanel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.settingsPanel.AutoScroll = true
			Me.settingsPanel.BackColor = System.Drawing.SystemColors.Window
			Me.settingsPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
			Me.settingsPanel.Controls.Add(Me.functionMessage)
			Me.settingsPanel.Location = New System.Drawing.Point(4, 24)
			Me.settingsPanel.Name = "settingsPanel"
			Me.settingsPanel.Size = New System.Drawing.Size(188, 232)
			Me.settingsPanel.TabIndex = 1
			'
			'fnSettingsLabel
			'
			Me.fnSettingsLabel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.fnSettingsLabel.Location = New System.Drawing.Point(4, 8)
			Me.fnSettingsLabel.Name = "fnSettingsLabel"
			Me.fnSettingsLabel.Size = New System.Drawing.Size(184, 16)
			Me.fnSettingsLabel.TabIndex = 0
			Me.fnSettingsLabel.Text = "Function Settings:"
			'
			'functionList
			'
			Me.functionList.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.functionList.AutoScroll = true
			Me.functionList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
			Me.functionList.ListFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
			Me.functionList.Location = New System.Drawing.Point(8, 24)
			Me.functionList.Name = "functionList"
			Me.functionList.SelectedID = ""
			Me.functionList.SelectedIndex = -1
			Me.functionList.Size = New System.Drawing.Size(200, 256)
			Me.functionList.TabIndex = 1
			AddHandler Me.functionList.SelectedIndexChanged, AddressOf Me.FunctionListSelectedIndexChanged
			'
			'fnLabel
			'
			Me.fnLabel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.fnLabel.Location = New System.Drawing.Point(8, 8)
			Me.fnLabel.Name = "fnLabel"
			Me.fnLabel.Size = New System.Drawing.Size(192, 16)
			Me.fnLabel.TabIndex = 0
			Me.fnLabel.Text = "Function:"
			'
			'helpButton
			'
			Me.helpButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.helpButton.Enabled = false
			Me.helpButton.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.helpButton.Location = New System.Drawing.Point(84, 264)
			Me.helpButton.Name = "helpButton"
			Me.helpButton.Size = New System.Drawing.Size(108, 24)
			Me.helpButton.TabIndex = 2
			Me.helpButton.Text = "Show Help ..."
			AddHandler Me.helpButton.Click, AddressOf Me.HelpButtonClick
			'
			'rightPanel
			'
			Me.rightPanel.BackColor = System.Drawing.SystemColors.Control
			Me.rightPanel.Controls.Add(Me.helpButton)
			Me.rightPanel.Controls.Add(Me.settingsPanel)
			Me.rightPanel.Controls.Add(Me.fnSettingsLabel)
			Me.rightPanel.Dock = System.Windows.Forms.DockStyle.Right
			Me.rightPanel.Location = New System.Drawing.Point(216, 0)
			Me.rightPanel.Name = "rightPanel"
			Me.rightPanel.Size = New System.Drawing.Size(200, 288)
			Me.rightPanel.TabIndex = 0
			'
			'PEStandardFunction
			'
			Me.Controls.Add(Me.leftPanel)
			Me.Controls.Add(Me.panelSplitter)
			Me.Controls.Add(Me.rightPanel)
			Me.Name = "PEStandardFunction"
			Me.Size = New System.Drawing.Size(416, 288)
			Me.leftPanel.ResumeLayout(false)
			Me.settingsPanel.ResumeLayout(false)
			Me.rightPanel.ResumeLayout(false)
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
		Private Sub FunctionListSelectedIndexChanged(sender As System.Object, e As System.EventArgs)
			'Clear existing settings
			settingsPanel.Controls.Clear
			
			'Get current function
			Dim checkFunction As GCBFunction
			currentFunction = Nothing
			For Each checkFunction In MainForm.GCBFunctions
				If checkFunction.Name = FunctionList.SelectedID Then
					currentFunction = checkFunction
					Exit For
				End If
			Next
			If currentFunction Is Nothing Then
				Frame.AllowNext = False
				settingsPanel.Controls.Add(functionMessage)
				Exit Sub
			End If
			Frame.AllowNext = True
			helpButton.Enabled = True
			
			If currentFunction.Params.Count = 0 Then
				functionMessage.Text = "This function has no settings"
				settingsPanel.Controls.Add(functionMessage)
				Exit Sub
			End If
			
			'Draw new settings
			Dim locY As Integer = 0
			Dim sizeY As Integer
			Dim thisParam As GCBCommandParam
			For Each thisParam In currentFunction.Params
				Dim newEditor As New ParamEditBox
				newEditor.Init(Frame.Program, Frame.CurrentSub, thisParam.Name, "", thisParam.ParamType, thisParam.Description, True)
				
				settingsPanel.Controls.Add(newEditor)
				sizeY = newEditor.Height
				newEditor.Location = New Point(4, locY + 4)
				newEditor.Size = New Size(settingsPanel.Width - 8, sizeY)
				newEditor.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
				locY += sizeY + 10
			Next
			
		End Sub
		
		Private Sub HelpButtonClick(sender As System.Object, e As System.EventArgs)
			Dim HelpPage As String = FunctionList.SelectedID
			
			If Not FunctionList.SelectedObject Is Nothing Then
				Dim CurrFunction As GCBFunction = FunctionList.SelectedObject
				If CurrFunction.HelpPage <> "" Then
					HelpPage = CurrFunction.HelpPage
				End If
			End If
			
			Mainform.MainformInstance.ShowHelpArticle(HelpPage + ".htm")
			
		End Sub
		
	End Class
'End Namespace
