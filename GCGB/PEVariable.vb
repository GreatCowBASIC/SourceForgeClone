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
	
	Public Class PEVariable
		Inherits System.Windows.Forms.UserControl
		Implements ParamEditPage
		Private AddVar As System.Windows.Forms.Button
		Private labelVars As System.Windows.Forms.Label
		Private VarList As System.Windows.Forms.ListBox
		
		Private Dim ArrayMode As Boolean = False
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
		
		Public Sub Init (Frame As ParameterEditor, ParamTypeIn As String, CurrentSource As String) Implements ParamEditPage.Init
			Me.Frame = Frame
			
			Frame.NextIsFinish = True
			Frame.AllowNext = False
			
			Dim Temp As Integer
			Dim CurrIsVar As Boolean = False
			
			Dim ParamTypes() As String = ParamTypeIn.Split("|")
			Dim ParamType As String
			
			For Each ParamType In ParamTypes
				
				If ParamType = "array" Then
					ArrayMode = True
				End If
				
				'Get variables in program
				With Frame.Program
					Dim CurrVar As VariableListItem
					For Temp = 0 To .Variables.Count - 1
						CurrVar = .Variables.GetItem(Temp)
						If ParamType = "array" Then
							If CurrVar.IsArray Then
								Me.VarList.Items.Add(CurrVar.Name)
								If CurrentSource.Trim.ToLower = CurrVar.Name.Trim.ToLower Then
									CurrIsVar = True
								End If
							End If
							
						Else
							If (ParamType = "var" Or ParamType = "number" Or (ParamType.StartsWith("var_") And ParamType.EndsWith(CurrVar.Type.ToLower)) Or ParamType.ToLower = CurrVar.Type.ToLower) And Not CurrVar.IsArray Then
								Me.VarList.Items.Add(CurrVar.Name)
								If CurrentSource.Trim.ToLower = CurrVar.Name.Trim.ToLower Then
									CurrIsVar = True
								End If
							End If
						End If
					Next
				End With
				
				'Are we in a subroutine/function?
				If Not Frame.CurrentSub Is Nothing Then
					'Get parameters of subroutine
					Dim currParam As SubParam
					For Each currParam In Frame.CurrentSub.Parameters
						If ParamType = "array" Then
							If currParam.DataArray Then
								Me.VarList.Items.Add(CurrParam.Name)
								If CurrentSource.Trim.ToLower = CurrParam.Name.Trim.ToLower Then
									CurrIsVar = True
								End If
							End If
							
						Else
							If (ParamType = "var" Or ParamType = "number" Or (ParamType.StartsWith("var_") And ParamType.EndsWith(currParam.DataType))) And Not currParam.DataArray Then
								Me.VarList.Items.Add(currParam.Name)
								If CurrentSource.Trim.ToLower = currParam.Name.Trim.ToLower Then
									CurrIsVar = True
								End If
							End If
						End If
					Next
					
					'Get function return value
					If ParamType <> "array" And Frame.CurrentSub.IsFunction Then
						If ParamType = "var" Or ParamType = "number" Or (ParamType.StartsWith("var_") And ParamType.EndsWith(Frame.CurrentSub.ReturnType)) Then
							Me.VarList.Items.Add(Frame.CurrentSub.Name)
						End If
					End If
				End If
				
				'Get variables from library
				Dim CheckLib As GCBLibrary
				For Each CheckLib In Frame.Program.Libraries
					Dim currVar As VariableListItem
					For Each currVar In CheckLib.Variables
						If Not currVar.Hidden Then
							
							If ParamType = "array" Then
								If CurrVar.IsArray Then
									Me.VarList.Items.Add(CurrVar.Name + " (Library:" + CheckLib.DisplayName + ")")
									If CurrentSource.Trim.ToLower = CurrVar.Name.Trim.ToLower Then
										CurrIsVar = True
									End If
								End If
								
							Else
								'MessageBox.Show("Needed:" + ParamType + ", var:" + currVar.Name + " type is " + currVar.Type)
								If (ParamType = "var" Or ParamType = "number" Or (ParamType.StartsWith("var_") And ParamType.EndsWith(CurrVar.Type.ToLower)) Or ParamType.ToLower = CurrVar.Type.ToLower) And Not CurrVar.IsArray Then
									Me.VarList.Items.Add(CurrVar.Name + " (Library:" + CheckLib.DisplayName + ")")
									If CurrentSource.Trim.ToLower = CurrVar.Name.Trim.ToLower Then
										CurrIsVar = True
									End If
								End If
							End If
						
						End If
					Next
				Next
				
			Next
			
			'Find and remove duplicates
			Dim Look1, Look2 As Integer
			If VarList.Items.Count > 2 Then
				Look1 = 1
				Do
					For Look2 = 0 To Look1 - 1
						If VarList.Items(Look1) = VarList.Items(Look2) Then
							VarList.Items.RemoveAt(Look1)
							Look1 -= 1
							Exit For
						End If
					Next
					Look1 += 1
				Loop While Look1 < VarList.Items.Count
			End If
			
			Me.VarList.Sorted = True
			
			If CurrIsVar Then
				Me.VarList.SelectedItem = CurrentSource
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
			Me.VarList = New System.Windows.Forms.ListBox
			Me.labelVars = New System.Windows.Forms.Label
			Me.AddVar = New System.Windows.Forms.Button
			Me.SuspendLayout
			'
			'VarList
			'
			Me.VarList.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.VarList.IntegralHeight = false
			Me.VarList.Location = New System.Drawing.Point(8, 24)
			Me.VarList.Name = "VarList"
			Me.VarList.Size = New System.Drawing.Size(264, 256)
			Me.VarList.TabIndex = 0
			AddHandler Me.VarList.DoubleClick, AddressOf Me.VarListDoubleClick
			AddHandler Me.VarList.SelectedIndexChanged, AddressOf Me.VarListSelectedIndexChanged
			'
			'labelVars
			'
			Me.labelVars.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.labelVars.Location = New System.Drawing.Point(8, 8)
			Me.labelVars.Name = "labelVars"
			Me.labelVars.Size = New System.Drawing.Size(264, 16)
			Me.labelVars.TabIndex = 4
			Me.labelVars.Text = "Variables:"
			'
			'AddVar
			'
			Me.AddVar.Anchor = System.Windows.Forms.AnchorStyles.Bottom
			Me.AddVar.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.AddVar.Location = New System.Drawing.Point(80, 288)
			Me.AddVar.Name = "AddVar"
			Me.AddVar.Size = New System.Drawing.Size(120, 24)
			Me.AddVar.TabIndex = 3
			Me.AddVar.Text = "Add New Variable ..."
			AddHandler Me.AddVar.Click, AddressOf Me.AddVarClick
			'
			'PEVariable
			'
			Me.Controls.Add(Me.labelVars)
			Me.Controls.Add(Me.AddVar)
			Me.Controls.Add(Me.VarList)
			Me.Name = "PEVariable"
			Me.Size = New System.Drawing.Size(280, 320)
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
		Private Sub AddVarClick(sender As System.Object, e As System.EventArgs)
			Dim SearchList As Integer = 0
			Dim VarExists As Boolean = False
			Dim TempData As String = ""
			
			'Display dialog
			Dim fEditVar As New EditVar
			fEditVar.Text = "Add Variable"
			If ArrayMode Then
				fEditVar.InitEdit("", "byte", "", EditVar.EditOptions.ARRAY_ONLY)
			Else
				fEditVar.InitEdit("", "byte", "", EditVar.EditOptions.NO_ARRAY)
			End If
			fEditVar.ShowDialog()
			
			'Apply
			If fEditVar.NewVarName <> "" Then
				TempData = fEditVar.NewVarType
				If TempData = "array" Then
					Frame.Program.Variables.AddVar(fEditVar.NewVarName.Trim, "byte", True, fEditVar.NewArraySize)
					If ArrayMode Then
						Me.VarList.Items.Add(fEditVar.NewVarName)
						'Me.VarList.SelectedIndex = Me.VarList.Items.Count - 1
						Me.VarList.SelectedItem = fEditVar.NewVarName
					End If
				Else
					Frame.Program.Variables.AddVar(fEditVar.NewVarName.Trim, TempData)
					If Not ArrayMode Then
						Me.VarList.Items.Add(fEditVar.NewVarName)
						'Me.VarList.SelectedIndex = Me.VarList.Items.Count - 1
						Me.VarList.SelectedItem = fEditVar.NewVarName
					End If
				End If
				
			End If
			
		End Sub
		
		Private Sub VarListSelectedIndexChanged(sender As System.Object, e As System.EventArgs)
			If VarList.SelectedIndex <> -1 Then
				OutVal = VarList.SelectedItem
				If OutVal.IndexOf("(") <> -1 Then
					OutVal = OutVal.Substring(0, OutVal.IndexOf("(")).Trim
				End If
				Frame.AllowNext = True
			Else
				Frame.AllowNext = False
			End If
			
			
		End Sub
		
		Private Sub VarListDoubleClick(sender As System.Object, e As System.EventArgs)
			'If an item in the list is double clicked, load next page in parameter editor
			Frame.GoToNextPage
		End Sub
		
	End Class
'End Namespace
