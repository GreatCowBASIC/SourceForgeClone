'
' Created by SharpDevelop.
' User: Hugh
' Date: 13/12/2009
' Time: 2:10 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Collections

Namespace Great_Cow_Graphical_BASIC
	
	Public Class WizardPage
		Public Dim Name As String
		Public Dim Page As UserControl
		
	End Class
	
	Public Interface ParamEditPage
		'Initialises the page
		Sub Init (Frame As ParameterEditor, ParamType As String, CurrentSource As String)
		
		'Stores the next page ID. Empty string if no next page
		ReadOnly Property NextPage As String
		'Stores the output value for parameter. Empty string if not last page
		ReadOnly Property OutputValue As String
		
	End Interface
	
	Public Class ParameterEditor
		Inherits System.Windows.Forms.Form
		Private pageChooseSource As Great_Cow_Graphical_BASIC.PEChooseSource
		Private buttonCancel As System.Windows.Forms.Button
		Private buttonNext As System.Windows.Forms.Button
		Private buttonBack As System.Windows.Forms.Button
		
		Private Dim pAccepted As Boolean = False
		Private Dim pProgram As GCBProgram
		Private Dim ParamName, ParamType, NewParamValue As String
		Private Dim pCurrentSub As GCBSubroutine
		
		Private Dim PageDisplayed As Control
		Private Dim CurrentPage As Integer
		Private Dim PageHistory(20) As WizardPage
		
		Public Sub New()
			MyBase.New
			'
			' The Me.InitializeComponent call is required for Windows Forms designer support.
			'
			Me.InitializeComponent
			
		End Sub
		
		#Region " Windows Forms Designer generated code "
		' This method is required for Windows Forms designer support.
		' Do not change the method contents inside the source code editor. The Forms designer might
		' not be able to load this method if it was changed manually.
		Private Sub InitializeComponent()
			Me.buttonBack = New System.Windows.Forms.Button
			Me.buttonNext = New System.Windows.Forms.Button
			Me.buttonCancel = New System.Windows.Forms.Button
			Me.pageChooseSource = New Great_Cow_Graphical_BASIC.PEChooseSource
			Me.SuspendLayout
			'
			'buttonBack
			'
			Me.buttonBack.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.buttonBack.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.buttonBack.Location = New System.Drawing.Point(318, 336)
			Me.buttonBack.Name = "buttonBack"
			Me.buttonBack.Size = New System.Drawing.Size(72, 24)
			Me.buttonBack.TabIndex = 1
			Me.buttonBack.Text = "< Back"
			AddHandler Me.buttonBack.Click, AddressOf Me.ButtonBackClick
			'
			'buttonNext
			'
			Me.buttonNext.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.buttonNext.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.buttonNext.Location = New System.Drawing.Point(390, 336)
			Me.buttonNext.Name = "buttonNext"
			Me.buttonNext.Size = New System.Drawing.Size(72, 24)
			Me.buttonNext.TabIndex = 2
			Me.buttonNext.Text = "Next >"
			AddHandler Me.buttonNext.Click, AddressOf Me.ButtonNextClick
			'
			'buttonCancel
			'
			Me.buttonCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.buttonCancel.Location = New System.Drawing.Point(470, 336)
			Me.buttonCancel.Name = "buttonCancel"
			Me.buttonCancel.Size = New System.Drawing.Size(72, 24)
			Me.buttonCancel.TabIndex = 3
			Me.buttonCancel.Text = "Cancel"
			'
			'pageChooseSource
			'
			Me.pageChooseSource.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.pageChooseSource.Location = New System.Drawing.Point(4, 4)
			Me.pageChooseSource.Name = "pageChooseSource"
			Me.pageChooseSource.Size = New System.Drawing.Size(544, 312)
			Me.pageChooseSource.TabIndex = 0
			'
			'ParameterEditor
			'
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 14)
			Me.CancelButton = Me.buttonCancel
			Me.ClientSize = New System.Drawing.Size(552, 366)
			Me.ControlBox = false
			Me.Controls.Add(Me.buttonCancel)
			Me.Controls.Add(Me.buttonNext)
			Me.Controls.Add(Me.buttonBack)
			Me.Controls.Add(Me.pageChooseSource)
			Me.MaximizeBox = false
			Me.MinimizeBox = false
			Me.MinimumSize = New System.Drawing.Size(396, 320)
			Me.Name = "ParameterEditor"
			Me.ShowInTaskbar = false
			Me.Text = "Parameter Editor"
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
		Public Sub Init(Program As GCBProgram, ParamName As String, ParamType As String, ParamValue As String, CurrentSub As GCBSubroutine)
			'Initialise editor
			
			'Load initial data
			Me.pProgram = Program
			Me.ParamName = ParamName
			Me.NewParamValue = ParamValue
			Me.ParamType = ParamType
			Me.pCurrentSub = CurrentSub
			
			'Check chip data
			Program.CheckChipData
			
			CurrentPage = 1
			PageHistory(1) = New WizardPage
			PageHistory(1).Name = "choose_source"
			PageHistory(1).Page = pageChooseSource
			
			PageDisplayed = pageChooseSource
			
			'Set window title
			Me.Text = Me.Text + ": " + ParamName
			
			'Choose appropriate first page option
			Dim CurrentSource As String
			CurrentSource = ""
			
			pageChooseSource.Init(Me, ParamType, CurrentSource)
			
			'If there is only one option on the first page, choose it
			If pageChooseSource.SingleOption Then
				SetPage(pageChooseSource.FirstOption)
			End If
			
			buttonBack.Enabled = False
			
		End Sub
		
		Public ReadOnly Property Program As GCBProgram
			Get
				Return pProgram
			End Get
		End Property
		
		Public ReadOnly Property CurrentSub As GCBSubroutine
			Get
				Return pCurrentSub
			End Get
		End Property
		
		'Go to a particular page of the wizard
		Private Sub GoToPage(NewPage As String)
			
			'Move to next history location
			CurrentPage += 1
			
			'Set new page
			SetPage(NewPage)
			
		End Sub
		
		'Displays a new page without saving any history
		Private Sub SetPage(NewPage As String)
			
			'Save page history
			Dim NewHistory As New WizardPage
			NewHistory.Name = NewPage
			PageHistory(CurrentPage) = NewHistory
			
			'Find new page
			Select Case NewPage.ToLower
				Case "simplecondition"
					PageHistory(CurrentPage).Page = New PECondition
					
				Case "function"
					PageHistory(CurrentPage).Page = New PEFunction
					
				Case "const_string"
					PageHistory(CurrentPage).Page = New PEString
					
				Case "const_num"
					PageHistory(CurrentPage).Page = New PELiteral
					
				Case "var_string", "var_array", "var_bit", "var"
					PageHistory(CurrentPage).Page = New PEVariable
					
				Case "io_pin"
					PageHistory(CurrentPage).Page = New PEChipPin
					
				Case "io_port"
					
				Case "bit"
					PageHistory(CurrentPage).Page = New PEBit
					
				Case "calculation"
					PageHistory(CurrentPage).Page = New PECalculation
					
				Case "gcbfunction"
					PageHistory(CurrentPage).Page = New PEStandardFunction
					
				Case "array_element"
					PageHistory(CurrentPage).Page = New PEArrayElement
					
			End Select
			
			
			'Reset next
			NextIsFinish = False
			
			Dim TempPage As ParamEditPage = PageHistory(CurrentPage).Page
			If Not TempPage Is Nothing Then
				TempPage.Init(Me, ParamType, NewParamValue)
			End If
			
			'Render new page
			DisplayPage
			
		End Sub
		
		'Redisplays the current page
		Private Sub DisplayPage
			'Remove existing page from form
			Try
				Me.Controls.Remove(PageDisplayed)
				PageDisplayed = Nothing
			Catch
			End Try
			
			'Get new page
			Dim NewPage As UserControl
			NewPage = PageHistory(CurrentPage).Page
			If Not NewPage Is Nothing Then
				
				'Display new page
				PageDisplayed = NewPage
				Me.Controls.Add(NewPage)
				With NewPage
					.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
							Or System.Windows.Forms.AnchorStyles.Left)  _
							Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
					.Location = New System.Drawing.Point(4, 4)
					.Name = PageHistory(CurrentPage).Name
					
					'Reset size
					.Size = New System.Drawing.Size(Me.ClientSize.Width - 8, Me.ClientSize.Height - 54)
					
				End With
			End If
			
			'Update back button
			If CurrentPage > 1 Then
				buttonBack.Enabled = True
			Else
				buttonBack.Enabled = False
			End If
			
		End Sub
		
		Public ReadOnly Property Accepted As Boolean
			Get
				Return pAccepted
			End Get
		End Property
		
		Public ReadOnly Property ParamValue As String
			Get
				Return NewParamValue
			End Get
		End Property
		
		Public Property AllowNext As Boolean
			Get
				Return buttonNext.Enabled
			End Get
			Set
				buttonNext.Enabled = Value
			End Set
		End Property
		
		Public Property NextIsFinish As Boolean
			Get
				Return buttonNext.Text = "Finish"
			End Get
			Set
				If Value Then
					buttonNext.Text = "Finish"
				Else
					buttonNext.Text = "Next >"
				End If
			End Set
		End Property
		
		Private Sub ButtonBackClick(sender As System.Object, e As System.EventArgs)
			
			If CurrentPage > 1 Then
				CurrentPage -= 1
				AllowNext = True
				DisplayPage
				NextIsFinish = False
			End If
			
		End Sub
		
		Private Sub ButtonNextClick(sender As System.Object, e As System.EventArgs)
			
			GoToNextPage
			
		End Sub
		
		Public Sub GoToNextPage
			'Need to choose page based on current page selections
			'Make sure page is displayed
			If Not PageDisplayed Is Nothing Then
				'Get current page
				Dim NewPage As ParamEditPage
				NewPage = PageDisplayed
				
				'Check for next page
				If NewPage.NextPage <> "" Then
					GoToPage(pageChooseSource.NextPage)
				
				'Check for output value
				ElseIf NewPage.OutputValue <> "" Then
					'If value found, return it and exit
					NewParamValue = NewPage.OutputValue
					pAccepted = True
					
					Me.Close
					
				End If
			End If
		End Sub
		
	End Class
End Namespace
