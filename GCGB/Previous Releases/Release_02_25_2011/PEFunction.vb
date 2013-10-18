'
' Created by SharpDevelop.
' User: Hugh
' Date: 13/12/2009
' Time: 1:50 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.ComponentModel
Imports System.Windows.Forms

Namespace Great_Cow_Graphical_BASIC
	
	Public Class PEFunction
		Inherits System.Windows.Forms.UserControl
		Implements ParamEditPage
		Private rightPanel As System.Windows.Forms.Panel
		Private fnLabel As System.Windows.Forms.Label
		Private fnSourceList As System.Windows.Forms.ComboBox
		Private functionList As Great_Cow_Graphical_BASIC.CaptionedListBox
		Private fnSettingsLabel As System.Windows.Forms.Label
		Private settingsPanel As System.Windows.Forms.Panel
		Private leftPanel As System.Windows.Forms.Panel
		Private functionMessage As System.Windows.Forms.Label
		Private labelFnSource As System.Windows.Forms.Label
		Private panelSplitter As System.Windows.Forms.Splitter
		
		Private Dim Frame As ParameterEditor
		Private Dim currentFunction As GCBSubroutine
		Private Dim ParamType As String
		
		'Stores the next page ID. Empty string if no next page
		Public ReadOnly Property NextPage As String Implements ParamEditPage.NextPage
			Get
				Return ""
			End Get
		End Property
		
		'Stores the output value for parameter. Empty string if not last page
		Public ReadOnly Property OutputValue As String Implements ParamEditPage.OutputValue
			Get
				Dim OutVal, OutParams As String
				
				'Function name
				OutVal = currentFunction.Name
				
				'Function parameters
				Dim valueSet As Boolean = False
				If currentFunction.Parameters.Count > 0 Then
					OutParams = "("
					Dim currControl As Control
					Dim currEditor As ParamEditBox
					Dim isFirstParam As Boolean = True
					For Each currControl In settingsPanel.Controls
						If TypeOf currControl Is ParamEditBox Then
							
							If Not isFirstParam Then
								OutParams += ", "
							Else
								isFirstParam = False
							End If
							
							currEditor = currControl
							OutParams += currEditor.GetValue
							If currEditor.GetValue.Trim <> "" Then
								valueSet = True
							End If
						End If
					Next
					OutParams += ")"
				End If
				
				If valueSet Then
					OutVal += OutParams
				End If
				
				Return OutVal
			End Get
		End Property
		
		Public Sub Init (Frame As ParameterEditor, ParamType As String, CurrentSource As String) Implements ParamEditPage.Init
			Me.Frame = Frame
			
			Frame.AllowNext = False
			Frame.NextIsFinish = True
			currentFunction = Nothing
			Me.ParamType = ParamType
			
			'Fill function source list
			With Me.fnSourceList.Items
				.Add("All files")
				.Add("Main program")
				Dim CurrLib As LibraryType
				For Each CurrLib In Frame.Program.Libraries
					.Add("Library: " + CurrLib.FileName)
				Next
			End With
			
			'Select source (will fill function list)
			fnSourceList.SelectedIndex = 0
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
			Me.labelFnSource = New System.Windows.Forms.Label
			Me.functionMessage = New System.Windows.Forms.Label
			Me.leftPanel = New System.Windows.Forms.Panel
			Me.settingsPanel = New System.Windows.Forms.Panel
			Me.fnSettingsLabel = New System.Windows.Forms.Label
			Me.functionList = New Great_Cow_Graphical_BASIC.CaptionedListBox
			Me.fnSourceList = New System.Windows.Forms.ComboBox
			Me.fnLabel = New System.Windows.Forms.Label
			Me.rightPanel = New System.Windows.Forms.Panel
			Me.leftPanel.SuspendLayout
			Me.settingsPanel.SuspendLayout
			Me.rightPanel.SuspendLayout
			Me.SuspendLayout
			'
			'panelSplitter
			'
			Me.panelSplitter.Dock = System.Windows.Forms.DockStyle.Right
			Me.panelSplitter.Location = New System.Drawing.Point(212, 0)
			Me.panelSplitter.Name = "panelSplitter"
			Me.panelSplitter.Size = New System.Drawing.Size(4, 288)
			Me.panelSplitter.TabIndex = 1
			Me.panelSplitter.TabStop = false
			'
			'labelFnSource
			'
			Me.labelFnSource.Location = New System.Drawing.Point(8, 8)
			Me.labelFnSource.Name = "labelFnSource"
			Me.labelFnSource.Size = New System.Drawing.Size(200, 16)
			Me.labelFnSource.TabIndex = 3
			Me.labelFnSource.Text = "Show functions from:"
			'
			'functionMessage
			'
			Me.functionMessage.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.functionMessage.Location = New System.Drawing.Point(8, 8)
			Me.functionMessage.Name = "functionMessage"
			Me.functionMessage.Size = New System.Drawing.Size(172, 248)
			Me.functionMessage.TabIndex = 0
			Me.functionMessage.Text = "Please choose a function"
			'
			'leftPanel
			'
			Me.leftPanel.Controls.Add(Me.fnSourceList)
			Me.leftPanel.Controls.Add(Me.labelFnSource)
			Me.leftPanel.Controls.Add(Me.functionList)
			Me.leftPanel.Controls.Add(Me.fnLabel)
			Me.leftPanel.Dock = System.Windows.Forms.DockStyle.Fill
			Me.leftPanel.Location = New System.Drawing.Point(0, 0)
			Me.leftPanel.Name = "leftPanel"
			Me.leftPanel.Size = New System.Drawing.Size(212, 288)
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
			Me.settingsPanel.Location = New System.Drawing.Point(4, 20)
			Me.settingsPanel.Name = "settingsPanel"
			Me.settingsPanel.Size = New System.Drawing.Size(188, 264)
			Me.settingsPanel.TabIndex = 3
			'
			'fnSettingsLabel
			'
			Me.fnSettingsLabel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.fnSettingsLabel.Location = New System.Drawing.Point(4, 4)
			Me.fnSettingsLabel.Name = "fnSettingsLabel"
			Me.fnSettingsLabel.Size = New System.Drawing.Size(188, 16)
			Me.fnSettingsLabel.TabIndex = 2
			Me.fnSettingsLabel.Text = "Function Settings:"
			'
			'functionList
			'
			Me.functionList.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.functionList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
			Me.functionList.ListFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
			Me.functionList.Location = New System.Drawing.Point(8, 80)
			Me.functionList.Name = "functionList"
			Me.functionList.SelectedID = ""
			Me.functionList.SelectedIndex = -1
			Me.functionList.Size = New System.Drawing.Size(200, 200)
			Me.functionList.TabIndex = 2
			AddHandler Me.functionList.SelectedIndexChanged, AddressOf Me.FunctionListSelectedIndexChanged
			'
			'fnSourceList
			'
			Me.fnSourceList.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.fnSourceList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me.fnSourceList.Location = New System.Drawing.Point(8, 24)
			Me.fnSourceList.Name = "fnSourceList"
			Me.fnSourceList.Size = New System.Drawing.Size(200, 21)
			Me.fnSourceList.TabIndex = 4
			AddHandler Me.fnSourceList.SelectedIndexChanged, AddressOf Me.FnSourceListSelectedIndexChanged
			'
			'fnLabel
			'
			Me.fnLabel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.fnLabel.Location = New System.Drawing.Point(8, 64)
			Me.fnLabel.Name = "fnLabel"
			Me.fnLabel.Size = New System.Drawing.Size(196, 16)
			Me.fnLabel.TabIndex = 1
			Me.fnLabel.Text = "Function:"
			'
			'rightPanel
			'
			Me.rightPanel.Controls.Add(Me.settingsPanel)
			Me.rightPanel.Controls.Add(Me.fnSettingsLabel)
			Me.rightPanel.Dock = System.Windows.Forms.DockStyle.Right
			Me.rightPanel.Location = New System.Drawing.Point(216, 0)
			Me.rightPanel.Name = "rightPanel"
			Me.rightPanel.Size = New System.Drawing.Size(200, 288)
			Me.rightPanel.TabIndex = 0
			'
			'PEFunction
			'
			Me.Controls.Add(Me.leftPanel)
			Me.Controls.Add(Me.panelSplitter)
			Me.Controls.Add(Me.rightPanel)
			Me.Name = "PEFunction"
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
			Dim checkFunction As GCBSubroutine
			currentFunction = FunctionList.SelectedObject
			
			If currentFunction Is Nothing Then
				Frame.AllowNext = False
				functionMessage.Text = "Please choose a function"
				settingsPanel.Controls.Add(functionMessage)
				Exit Sub
			End If
			Frame.AllowNext = True
			
			If currentFunction.Parameters.Count = 0 Then
				functionMessage.Text = "This function has no settings"
				settingsPanel.Controls.Add(functionMessage)
				Exit Sub
			End If
			
			'Draw new settings
			Dim locY As Integer = 0
			Dim sizeY As Integer
			Dim thisParam As SubParam
			For Each thisParam In currentFunction.Parameters
				Dim newEditor As New ParamEditBox
				Dim paramDesc As String = MainForm.Translator.GetMessage("GuiEditorPanel", "type", "Type:") + " " + thisParam.DataType
				If thisParam.Description <> "" Then
					paramDesc = thisParam.Description
				End If
				newEditor.Init(Frame.Program, Frame.CurrentSub, thisParam.Name, "", thisParam.DataType, paramDesc, True)
				
				settingsPanel.Controls.Add(newEditor)
				sizeY = newEditor.Height
				newEditor.Location = New Point(4, locY + 4)
				newEditor.Size = New Size(settingsPanel.Width - 8, sizeY)
				newEditor.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
				locY += sizeY + 10
			Next
			
			
		End Sub
		
		Private Sub FnSourceListSelectedIndexChanged(sender As System.Object, e As System.EventArgs)
			
			'Clear existing function list
			functionList.Clear
			
			'Fill function list with list of all functions
			'Functions from main program
			If fnSourceList.SelectedIndex < 2 Then
				Dim searchSub As GCBSubroutine
				For Each searchSub In Frame.Program.Subroutines
					If searchSub.IsFunction Then
						If Lowlevel.CanCopy(searchSub.ReturnType, ParamType) Then functionList.Add(New CaptionedListItem(searchSub.Name, searchSub.Name, searchSub.Description, searchSub))
					End If
				Next
			End If
			
			'Functions from libraries
			If fnSourceList.SelectedIndex = 0 Then
				Dim CheckLib As LibraryType
				Dim searchSub As GCBSubroutine
				For Each CheckLib In Frame.Program.Libraries
					For Each searchSub In CheckLib.Subroutines
						If searchSub.IsFunction Then
							If Lowlevel.CanCopy(searchSub.ReturnType, ParamType) Then functionList.Add(New CaptionedListItem(searchSub.Name, searchSub.Name, searchSub.Description, searchSub))
						End If
					Next
				Next
			
			Else If fnSourceList.SelectedIndex > 1 Then
				Dim CheckLib As LibraryType = Frame.Program.Libraries(fnSourceList.SelectedIndex - 2)
				Dim searchSub As GCBSubroutine
				For Each searchSub In CheckLib.Subroutines
					If searchSub.IsFunction Then
						If Lowlevel.CanCopy(searchSub.ReturnType, ParamType) Then functionList.Add(New CaptionedListItem(searchSub.Name, searchSub.Name, searchSub.Description, searchSub))
					End If
				Next
			End If
			
		End Sub
		
	End Class
End Namespace
