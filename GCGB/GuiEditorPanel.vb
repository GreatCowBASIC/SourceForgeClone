'
' Created by SharpDevelop.
' User: Hugh
' Date: 14/07/2009
' Time: 10:37 PM
'
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.ComponentModel
Imports System.Windows.Forms
Imports System.Collections.Generic

''Namespace Great_Cow_Graphical_BASIC
	
	Public Class ParamListItem
		'Stores an icon parameter, used by drawing code
		Public Dim Name, Desc, Value, Options, Mode As String
	End Class
	
	Public Class GuiEditorPanel
		Inherits EditorPanel
		'Inherits System.Windows.Forms.UserControl
		Private components As System.ComponentModel.IContainer
		Private iconTipDisplay As System.Windows.Forms.ToolTip
		Private SubSelect As System.Windows.Forms.ListBox
		Private contextDelete As System.Windows.Forms.MenuItem
		Private ScrollTimer As System.Windows.Forms.Timer
		Private rightPanel As System.Windows.Forms.Panel
		Private formSplitter As System.Windows.Forms.Splitter
		Private contextCut As System.Windows.Forms.MenuItem
		Private CommandPropertiesGroup As System.Windows.Forms.GroupBox
		Private ProgEditPanel As System.Windows.Forms.Panel
		Private leftPanel As System.Windows.Forms.Panel
		Private iconRightMenu As System.Windows.Forms.ContextMenu
		Private menuItem1 As System.Windows.Forms.MenuItem
		Private menuItem7 As System.Windows.Forms.MenuItem
		Private IconPropertiesPanel As System.Windows.Forms.Panel
		Private menuItem5 As System.Windows.Forms.MenuItem
		Private EditSubButton As System.Windows.Forms.Button
		Private rightPaneSplitter As System.Windows.Forms.Splitter
		Private bottomRightPanel As System.Windows.Forms.Panel
		Private DeleteSubButton As System.Windows.Forms.Button
		Private contextHelp As System.Windows.Forms.MenuItem
		Private subroutinesGroup As System.Windows.Forms.GroupBox
		Private contextCopy As System.Windows.Forms.MenuItem
		Private AddSubButton As System.Windows.Forms.Button
		Private contextViewSub As System.Windows.Forms.MenuItem
		Private contextPaste As System.Windows.Forms.MenuItem
		
		
		'Icon Display
		Private Dim CurrentIconCommand As Integer = 0
		Private Dim IconDisplay(MainForm.MaxSubSize + 10) As IconDisplay
		Private Dim IconSpacing(MainForm.MaxSubSize + 10) As Integer
		Private Dim IconIndent(MainForm.MaxSubSize + 10, 2) As Integer
		Private Dim CurrentIconPlace As Integer
		Private Dim pDispScale As Single = 1
		Private Dim IndentSize As Integer = 20
		
		'Icon Parameters
		Private IconFullCommand As System.Windows.Forms.Label
		Private IconParamText As System.Windows.Forms.Label
		Private Dim IconParamBox(MainForm.MaxIconParams) As ParamEditBox
		Private Dim ParamCount As Integer = 0
		Private Dim SubParamList(50) As SubParam
		
		Private IconParamAdd As System.Windows.Forms.Button
		Private IconParamEdit As System.Windows.Forms.Button
		Private IconParamDelete As System.Windows.Forms.Button
		Private IconParamList As System.Windows.Forms.ListBox
		Private IconParamLabel As System.Windows.Forms.Label
		Private Dim ParamSubName As String = ""
		
		Private Dim NewCommandTemplate As String
		
		'Misc control signals
		Private Dim DraggedCommand As String = ""
		Private Dim DeleteDragged As Boolean = False
		Private Dim LastIconClicked As Integer = 0
		Public Event RefreshUndoRedo
		Public Event RefreshSubList
		Public Event ThisWindowSelected
		Public Event SubAdded
		
		Private Dim Title As String
		Private Dim CurrentSub As Integer = 0
		Private Dim CurrentSubLine(MainForm.MaxSubCount) As Integer
		Private Dim CurrentScroll(MainForm.MaxSubCount) As Integer
		
		'Program line
		Private Property CurrentCodeLine As String
			Get
				If CurrentSubLine(CurrentSub) > 0 And CurrentSubLine(CurrentSub) <= Program.Subroutines(CurrentSub).Lines.Count Then
					Return Program.Subroutines(CurrentSub).Lines(CurrentSubLine(CurrentSub) - 1)
				Else
					Return ""
				End If
			End Get
			Set
				If CurrentSubLine(CurrentSub) > 0
					Program.Subroutines(CurrentSub).Lines(CurrentSubLine(CurrentSub) - 1) = Value
				End If
			End Set
		End Property
		
		'Program zoom
		Public Property Zoom As Single
			Get
				Return pDispScale
			End Get
			Set
				pDispScale = Value
				ProgramRedraw
			End Set
		End Property
		
		Public Sub New()
			MyBase.New
			' Must be called for initialization
			Me.InitializeComponent
			
			Dim Temp As Integer = 0
			CurrentSub = 0
		 	
		 	'Set splitter positions
		 	If Not MainForm.GetPreferences Is Nothing Then
			 	Dim SettingsPanelWidth, SubListHeight As String
			 	SettingsPanelWidth = MainForm.GetPreferences.GetPref("GCGB", "SettingsPanelWidth")
			 	SubListHeight = MainForm.GetPreferences.GetPref("GCGB", "SubListHeight")
			 	If SettingsPanelWidth <> "" Then
			 		Me.formSplitter.SplitPosition = SettingsPanelWidth
			 	End If
			 	If SubListHeight <> "" Then
			 		Me.rightPaneSplitter.SplitPosition = SubListHeight
			 	End If
			End If
			
		 	'Draw Sub List
			DrawSubList
		 	
		 	'Clear scroll locations
		 	For Temp = 0 To MainForm.MaxSubCount
		 		CurrentScroll(Temp) = 0
		 		CurrentSubLine(Temp) = 0
		 	Next
		 			 	
		 	'Draw Icons
		 	CurrentSub = 0
		 	'Me.ProgramRedraw
			
			'Translate
			If Not MainForm.Translator Is Nothing Then
				With MainForm.Translator
					If .CanTranslate Then
						.TryTranslate("GuiEditorPanel", "subroutinesGroup", subroutinesGroup.Text)
						.TryTranslate("GuiEditorPanel", "CommandPropertiesGroup", CommandPropertiesGroup.Text)
						.TryTranslate("GuiEditorPanel", "AddSubButton", AddSubButton.Text)
						.TryTranslate("GuiEditorPanel", "EditSubButton", EditSubButton.Text)
						.TryTranslate("GuiEditorPanel", "DeleteSubButton", DeleteSubButton.Text)
					End If
				End With
			End If
			
			'Listen for preferences change
			AddHandler Mainform.GetPreferences.Changed, AddressOf Me.PreferencesChanged
			
			'Load preferences
			PreferencesChanged
		End Sub
		
		#Region " Windows Forms Designer generated code "
		' This method is required for Windows Forms designer support.
		' Do not change the method contents inside the source code editor. The Forms designer might
		' not be able to load this method if it was changed manually.
		Private Sub InitializeComponent()
			Me.components = New System.ComponentModel.Container
			Me.contextPaste = New System.Windows.Forms.MenuItem
			Me.contextViewSub = New System.Windows.Forms.MenuItem
			Me.AddSubButton = New System.Windows.Forms.Button
			Me.contextCopy = New System.Windows.Forms.MenuItem
			Me.subroutinesGroup = New System.Windows.Forms.GroupBox
			Me.contextHelp = New System.Windows.Forms.MenuItem
			Me.DeleteSubButton = New System.Windows.Forms.Button
			Me.bottomRightPanel = New System.Windows.Forms.Panel
			Me.rightPaneSplitter = New System.Windows.Forms.Splitter
			Me.EditSubButton = New System.Windows.Forms.Button
			Me.menuItem5 = New System.Windows.Forms.MenuItem
			Me.IconPropertiesPanel = New System.Windows.Forms.Panel
			Me.menuItem7 = New System.Windows.Forms.MenuItem
			Me.menuItem1 = New System.Windows.Forms.MenuItem
			Me.iconRightMenu = New System.Windows.Forms.ContextMenu
			Me.leftPanel = New System.Windows.Forms.Panel
			Me.ProgEditPanel = New System.Windows.Forms.Panel
			Me.CommandPropertiesGroup = New System.Windows.Forms.GroupBox
			Me.contextCut = New System.Windows.Forms.MenuItem
			Me.formSplitter = New System.Windows.Forms.Splitter
			Me.rightPanel = New System.Windows.Forms.Panel
			Me.ScrollTimer = New System.Windows.Forms.Timer(Me.components)
			Me.contextDelete = New System.Windows.Forms.MenuItem
			Me.SubSelect = New System.Windows.Forms.ListBox
			Me.iconTipDisplay = New System.Windows.Forms.ToolTip(Me.components)
			Me.subroutinesGroup.SuspendLayout
			Me.bottomRightPanel.SuspendLayout
			Me.leftPanel.SuspendLayout
			Me.CommandPropertiesGroup.SuspendLayout
			Me.rightPanel.SuspendLayout
			Me.SuspendLayout
			'
			'contextPaste
			'
			Me.contextPaste.Index = 4
			Me.contextPaste.Text = "Paste After"
			AddHandler Me.contextPaste.Click, AddressOf Me.ContextPasteClick
			'
			'contextViewSub
			'
			Me.contextViewSub.Index = 0
			Me.contextViewSub.Text = "View Contents"
			AddHandler Me.contextViewSub.Click, AddressOf Me.ContextViewSubClick
			'
			'AddSubButton
			'
			Me.AddSubButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.AddSubButton.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.AddSubButton.Location = New System.Drawing.Point(132, 15)
			Me.AddSubButton.Name = "AddSubButton"
			Me.AddSubButton.Size = New System.Drawing.Size(52, 24)
			Me.AddSubButton.TabIndex = 3
			Me.AddSubButton.Text = "Add..."
			Me.iconTipDisplay.SetToolTip(Me.AddSubButton, "Add a new subroutine or function to the program")
			AddHandler Me.AddSubButton.Click, AddressOf Me.AddSubButtonClick
			'
			'contextCopy
			'
			Me.contextCopy.Index = 3
			Me.contextCopy.Text = "Copy"
			AddHandler Me.contextCopy.Click, AddressOf Me.ContextCopyClick
			'
			'subroutinesGroup
			'
			Me.subroutinesGroup.Controls.Add(Me.EditSubButton)
			Me.subroutinesGroup.Controls.Add(Me.SubSelect)
			Me.subroutinesGroup.Controls.Add(Me.DeleteSubButton)
			Me.subroutinesGroup.Controls.Add(Me.AddSubButton)
			Me.subroutinesGroup.Dock = System.Windows.Forms.DockStyle.Top
			Me.subroutinesGroup.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.subroutinesGroup.Location = New System.Drawing.Point(0, 0)
			Me.subroutinesGroup.Name = "subroutinesGroup"
			Me.subroutinesGroup.Size = New System.Drawing.Size(192, 104)
			Me.subroutinesGroup.TabIndex = 4
			Me.subroutinesGroup.TabStop = false
			Me.subroutinesGroup.Text = "Subroutine/Function"
			'
			'contextHelp
			'
			Me.contextHelp.Index = 8
			Me.contextHelp.Text = "Help ..."
			AddHandler Me.contextHelp.Click, AddressOf Me.ContextHelpClick
			'
			'DeleteSubButton
			'
			Me.DeleteSubButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.DeleteSubButton.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.DeleteSubButton.Location = New System.Drawing.Point(132, 71)
			Me.DeleteSubButton.Name = "DeleteSubButton"
			Me.DeleteSubButton.Size = New System.Drawing.Size(52, 24)
			Me.DeleteSubButton.TabIndex = 4
			Me.DeleteSubButton.Text = "Delete"
			Me.iconTipDisplay.SetToolTip(Me.DeleteSubButton, "Delete the current subroutine or function from the program")
			AddHandler Me.DeleteSubButton.Click, AddressOf Me.DeleteSubButtonClick
			'
			'bottomRightPanel
			'
			Me.bottomRightPanel.Controls.Add(Me.CommandPropertiesGroup)
			Me.bottomRightPanel.Dock = System.Windows.Forms.DockStyle.Fill
			Me.bottomRightPanel.Location = New System.Drawing.Point(0, 112)
			Me.bottomRightPanel.Name = "bottomRightPanel"
			Me.bottomRightPanel.Size = New System.Drawing.Size(192, 371)
			Me.bottomRightPanel.TabIndex = 7
			'
			'rightPaneSplitter
			'
			Me.rightPaneSplitter.Dock = System.Windows.Forms.DockStyle.Top
			Me.rightPaneSplitter.Location = New System.Drawing.Point(0, 104)
			Me.rightPaneSplitter.Name = "rightPaneSplitter"
			Me.rightPaneSplitter.Size = New System.Drawing.Size(192, 8)
			Me.rightPaneSplitter.TabIndex = 6
			Me.rightPaneSplitter.TabStop = false
			AddHandler Me.rightPaneSplitter.SplitterMoved, AddressOf Me.RightPaneSplitterSplitterMoved
			'
			'EditSubButton
			'
			Me.EditSubButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.EditSubButton.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.EditSubButton.Location = New System.Drawing.Point(132, 43)
			Me.EditSubButton.Name = "EditSubButton"
			Me.EditSubButton.Size = New System.Drawing.Size(52, 24)
			Me.EditSubButton.TabIndex = 6
			Me.EditSubButton.Text = "Edit..."
			Me.iconTipDisplay.SetToolTip(Me.EditSubButton, "Change the name of the current subroutine or function")
			AddHandler Me.EditSubButton.Click, AddressOf Me.EditSubButtonClick
			'
			'menuItem5
			'
			Me.menuItem5.Index = 5
			Me.menuItem5.Text = "-"
			'
			'IconPropertiesPanel
			'
			Me.IconPropertiesPanel.AutoScroll = true
			Me.IconPropertiesPanel.BackColor = System.Drawing.SystemColors.Window
			Me.IconPropertiesPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
			Me.IconPropertiesPanel.Dock = System.Windows.Forms.DockStyle.Fill
			Me.IconPropertiesPanel.DockPadding.All = 4
			Me.IconPropertiesPanel.Location = New System.Drawing.Point(3, 16)
			Me.IconPropertiesPanel.Name = "IconPropertiesPanel"
			Me.IconPropertiesPanel.Size = New System.Drawing.Size(186, 352)
			Me.IconPropertiesPanel.TabIndex = 0
			'
			'menuItem7
			'
			Me.menuItem7.Index = 7
			Me.menuItem7.Text = "-"
			'
			'menuItem1
			'
			Me.menuItem1.Index = 1
			Me.menuItem1.Text = "-"
			'
			'iconRightMenu
			'
			Me.iconRightMenu.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.contextViewSub, Me.menuItem1, Me.contextCut, Me.contextCopy, Me.contextPaste, Me.menuItem5, Me.contextDelete, Me.menuItem7, Me.contextHelp})
			AddHandler Me.iconRightMenu.Popup, AddressOf Me.IconRightMenuPopup
			'
			'leftPanel
			'
			Me.leftPanel.Controls.Add(Me.ProgEditPanel)
			Me.leftPanel.Dock = System.Windows.Forms.DockStyle.Fill
			Me.leftPanel.Location = New System.Drawing.Point(2, 2)
			Me.leftPanel.Name = "leftPanel"
			Me.leftPanel.Size = New System.Drawing.Size(296, 483)
			Me.leftPanel.TabIndex = 9
			'
			'ProgEditPanel
			'
			Me.ProgEditPanel.AllowDrop = true
			Me.ProgEditPanel.AutoScroll = true
			Me.ProgEditPanel.BackColor = System.Drawing.SystemColors.Window
			Me.ProgEditPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
			Me.ProgEditPanel.Dock = System.Windows.Forms.DockStyle.Fill
			Me.ProgEditPanel.Location = New System.Drawing.Point(0, 0)
			Me.ProgEditPanel.Name = "ProgEditPanel"
			Me.ProgEditPanel.Size = New System.Drawing.Size(296, 483)
			Me.ProgEditPanel.TabIndex = 3
			Me.ProgEditPanel.TabStop = true
			AddHandler Me.ProgEditPanel.Click, AddressOf Me.ProgEditPanelClick
			AddHandler Me.ProgEditPanel.DragEnter, AddressOf Me.ProgEditPanelDragEnter
			AddHandler Me.ProgEditPanel.DragLeave, AddressOf Me.ProgEditPanelDragLeave
			AddHandler Me.ProgEditPanel.MouseEnter, AddressOf Me.ProgEditPanelMouseEnter
			AddHandler Me.ProgEditPanel.KeyUp, AddressOf Me.EditorPanelKeyUp
			AddHandler Me.ProgEditPanel.DragDrop, AddressOf Me.ProgEditPanelDragDrop
			AddHandler Me.ProgEditPanel.DragOver, AddressOf Me.ProgEditPanelDragOver
			AddHandler Me.ProgEditPanel.MouseMove, AddressOf Me.ProgEditPanelMouseMove
			'
			'CommandPropertiesGroup
			'
			Me.CommandPropertiesGroup.BackColor = System.Drawing.SystemColors.Control
			Me.CommandPropertiesGroup.Controls.Add(Me.IconPropertiesPanel)
			Me.CommandPropertiesGroup.Dock = System.Windows.Forms.DockStyle.Fill
			Me.CommandPropertiesGroup.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.CommandPropertiesGroup.Location = New System.Drawing.Point(0, 0)
			Me.CommandPropertiesGroup.Name = "CommandPropertiesGroup"
			Me.CommandPropertiesGroup.Size = New System.Drawing.Size(192, 371)
			Me.CommandPropertiesGroup.TabIndex = 5
			Me.CommandPropertiesGroup.TabStop = false
			Me.CommandPropertiesGroup.Text = "Icon Settings"
			'
			'contextCut
			'
			Me.contextCut.Index = 2
			Me.contextCut.Text = "Cut"
			AddHandler Me.contextCut.Click, AddressOf Me.ContextCutClick
			'
			'formSplitter
			'
			Me.formSplitter.Dock = System.Windows.Forms.DockStyle.Right
			Me.formSplitter.Location = New System.Drawing.Point(298, 2)
			Me.formSplitter.Name = "formSplitter"
			Me.formSplitter.Size = New System.Drawing.Size(8, 483)
			Me.formSplitter.TabIndex = 8
			Me.formSplitter.TabStop = false
			AddHandler Me.formSplitter.SplitterMoved, AddressOf Me.FormSplitterSplitterMoved
			'
			'rightPanel
			'
			Me.rightPanel.Controls.Add(Me.bottomRightPanel)
			Me.rightPanel.Controls.Add(Me.rightPaneSplitter)
			Me.rightPanel.Controls.Add(Me.subroutinesGroup)
			Me.rightPanel.Dock = System.Windows.Forms.DockStyle.Right
			Me.rightPanel.Location = New System.Drawing.Point(306, 2)
			Me.rightPanel.Name = "rightPanel"
			Me.rightPanel.Size = New System.Drawing.Size(192, 483)
			Me.rightPanel.TabIndex = 7
			'
			'ScrollTimer
			'
			Me.ScrollTimer.Interval = 50
			AddHandler Me.ScrollTimer.Tick, AddressOf Me.ScrollTimerTick
			'
			'contextDelete
			'
			Me.contextDelete.Index = 6
			Me.contextDelete.Text = "Delete"
			AddHandler Me.contextDelete.Click, AddressOf Me.ContextDeleteClick
			'
			'SubSelect
			'
			Me.SubSelect.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
						Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.SubSelect.IntegralHeight = false
			Me.SubSelect.Location = New System.Drawing.Point(8, 18)
			Me.SubSelect.Name = "SubSelect"
			Me.SubSelect.Size = New System.Drawing.Size(120, 78)
			Me.SubSelect.TabIndex = 5
			Me.iconTipDisplay.SetToolTip(Me.SubSelect, "These are the subroutines in the program. Click Main to return to the main subrou"& _
"tine")
			'
			'GuiEditorPanel
			'
			Me.Controls.Add(Me.leftPanel)
			Me.Controls.Add(Me.formSplitter)
			Me.Controls.Add(Me.rightPanel)
			Me.DockPadding.All = 2
			Me.Name = "GuiEditorPanel"
			Me.Size = New System.Drawing.Size(500, 487)
			AddHandler Load, AddressOf Me.GuiEditorPanelLoad
			AddHandler KeyUp, AddressOf Me.EditorPanelKeyUp
			Me.subroutinesGroup.ResumeLayout(false)
			Me.bottomRightPanel.ResumeLayout(false)
			Me.leftPanel.ResumeLayout(false)
			Me.CommandPropertiesGroup.ResumeLayout(false)
			Me.rightPanel.ResumeLayout(false)
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
		Public Overrides Overloads Sub Init(InProgram As GCBProgram)
			'Display an existing program
			Program = InProgram
			'Me.Text = Program.ProgCode(0, 0)
			
			Program.Editor = Me
			
			SyncProgramToEditor
			
			AddHandler Program.LibraryListChanged, AddressOf Me.LibraryListChanged
			
		End Sub
		
		Private Sub PreferencesChanged
			'Set background for program (if one selected)
			If MainForm.GetPreferences.GetPref("GCGB", "Background") <> "" Then
				Try
					Me.ProgEditPanel.BackgroundImage = Image.FromFile(LowLevel.ReplaceToolVariables(MainForm.GetPreferences.GetPref("GCGB", "Background")))
				Catch
				End Try
			End If
			
			'Indent spacing
			If MainForm.GetPreferences.GetPref("GCGB", "indentsize") <> "" Then
				Try
					IndentSize = MainForm.GetPreferences.GetPref("GCGB", "indentsize")
				Catch
				End Try
			End If
			
			'Redraw
			If Not Program Is Nothing Then ProgramRedraw
		End Sub
		
		Public Sub RemoveLibraryHandler
			RemoveHandler Program.LibraryListChanged, AddressOf Me.LibraryListChanged
		End Sub
		
		Public Overrides Overloads Sub Init(InFile As String)
			
			Program = New GCBProgram(Me)
			
			If InFile = "" Then
				With Program
					.Filename = "Untitled" + MainForm.UntitledCount.ToString
					Me.Text = .Filename
					MainForm.UntitledCount += 1
					
					.ChipName = ""
					.ChipConfig = ""
					.ChipSpeed = 0
				End With
				
				SyncProgramToEditor
			Else
				Program.LoadFile(InFile)
				Me.Text = Program.Filename
				
				'No longer needed, loading will trigger redraw
				'ProgramRedraw
			End If
			
			AddHandler Program.LibraryListChanged, AddressOf Me.LibraryListChanged
			
		End Sub
		
		Private Sub LibraryListChanged
			'Called when libraries are changed
			SyncProgramToEditor
		End Sub
		
		Private Sub ProgramClosed(sender As System.Object, e As System.ComponentModel.CancelEventArgs)
			
			If Program.HasChanged Then
				
				Dim UnsavedPrompt As String = "There are unsaved changes to this file. Save them?"
				MainForm.Translator.TryTranslate("Misc", "UnsavedChanges", UnsavedPrompt)
				
				Dim SaveChangesResult As DialogResult = MessageBox.Show(UnsavedPrompt, Program.Filename, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
				
				If SaveChangesResult = DialogResult.Cancel Then
					 e.Cancel = True
					 MainForm.CancelClose = 1
					 Exit Sub
				End If
				If SaveChangesResult = DialogResult.Yes Then
					If Program.Filename.ToLower.Substring(0, 8) = "untitled" Then
						Mainform.MainFormInstance.SaveFileSelect.FileName = ""
						Mainform.MainFormInstance.SaveFileSelect.ShowDialog
						Exit Sub
					End If
					Program.SaveFile(Program.Filename)
				End If
			End If
			
			RaiseEvent ThisWindowSelected
		End Sub
		
		Private Function ShiftKeyDown As Boolean
			'MainForm.StatusBar.Text = Control.ModifierKeys
			If (Keys.Shift And Control.ModifierKeys) <> 0 Then Return True
			Return False
		End Function
		
		Private Function CtrlKeyDown As Boolean
			If (Keys.Control And Control.ModifierKeys) <> 0 Then Return True
			Return False
		End Function
		
		Public Overrides Sub SyncProgramToEditor
			ProgramRedraw
		End Sub
		
		Public Overrides Sub SyncEditorToProgram
			'Do nothing
			'Editor will always update program
		End Sub
		
		Private Sub ProgramRedraw
			'Dim DataTemp, IconName, IconParameters, ProcessTemp as String
			Dim CurrentLine As Integer
			Dim SubNameTemp, OldStatus, Redrawing As String
			Dim Temp As Integer = 0
			
			Redrawing = "Redrawing"
			MainForm.Translator.TryTranslate("Misc", "Redrawing", Redrawing)
			
			OldStatus = MainForm.GetStatus
			MainForm.SetStatus(Redrawing + " (0%)")
			
			If CurrentSub = 0 Then
				Me.EditSubButton.Enabled = False
				Me.DeleteSubButton.Enabled = False
			Else
				Me.EditSubButton.Enabled = True
				Me.DeleteSubButton.Enabled = True
			End If
			
			'Remove old icons
			Me.ProgEditPanel.Controls.Clear
			For Temp = 0 To MainForm.MaxSubSize
				Try
					RemoveHandler Me.IconDisplay(Temp).MouseUp, AddressOf Me.ProgEditPanelIconClick
					RemoveHandler Me.IconDisplay(Temp).MouseMove, AddressOf Me.ProgIconMouseMove
				Catch Ex As Exception
				End Try
			Next
			
			'Scroll to top of panel
			me.ProgEditPanel.AutoScrollPosition = New System.Drawing.Point(0, 0)
			
			'Ensure that the program editor panel is selected
			Me.ProgEditPanel.Select
			
			'Clear indent and spacing arrays
			CurrentIconPlace = 0
			For Temp = 0 To MainForm.MaxSubSize + 10
				IconIndent(Temp, 1) = 0
				IconIndent(Temp, 2) = 0
				IconSpacing(Temp) = 0
			Next
			
			'Draw list of subroutines
	     	DrawSubList
	     	
			'Draw program
			SubNameTemp = Program.Subroutines(CurrentSub).Name
			
			Dim NameParams As New List(Of String)
			NameParams.Add(SubNameTemp)
			NameParams.Add(SubNameTemp)
			Me.DrawIcon("Start", NameParams, 0, 0, "The Great Cow - drop icons below to make a program")
			If Not Program Is Nothing Then
				Dim ProgSizeBig As Double = (Program.Subroutines(CurrentSub).Lines.Count + 1) / 100
				For CurrentLine = 1 To Program.Subroutines(CurrentSub).Lines.Count
					MainForm.SetStatus(Redrawing + " (" + Convert.ToInt32(CurrentLine / ProgSizeBig).ToString + "%)")
					Me.DrawCommand(Program.Subroutines(CurrentSub).Lines(CurrentLine - 1), CurrentLine)
				Next
				Me.ProgEditPanel.AutoScrollPosition = New System.Drawing.Point(0, CurrentScroll(CurrentSub))
			End If
			
			'Show Selection
			SelectIcon(CurrentSubLine(CurrentSub))
			
			MainForm.SetStatus(OldStatus)
			
		End Sub
		
		Private Sub ProgramPartialRedraw(ByVal StartLine As Integer)
			Dim CurrentLine As Integer
			Dim Temp As Integer = 0
			
			Dim placeSaver As New Label
			
			'Save scroll
			CurrentScroll(CurrentSub) = me.ProgEditPanel.AutoScrollPosition.Y * -1
			
			'Use full redraw?
			If StartLine = 0 Then
				ProgramRedraw
				Exit Sub
			End If
			
			placeSaver.Location = New Point(1, IconSpacing(Program.Subroutines(CurrentSub).Lines.Count))
			Me.ProgEditPanel.Controls.Add(placeSaver)
			
			'Clear indent array
			For Temp = StartLine + 1 To MainForm.MaxSubSize + 10
				IconIndent(Temp, 1) = 0
				IconIndent(Temp, 2) = 0
			Next
			
			'Erase and draw icons
			For CurrentLine = StartLine To Program.Subroutines(CurrentSub).Lines.Count
				Try
					RemoveHandler Me.IconDisplay(CurrentLine).MouseUp, AddressOf Me.ProgEditPanelIconClick
					RemoveHandler Me.IconDisplay(CurrentLine).MouseMove, AddressOf Me.ProgIconMouseMove
				Catch Ex As Exception
				End Try
				Try
					Me.ProgEditPanel.Controls.Remove(Me.IconDisplay(CurrentLine))
				Catch
				End Try
				
				Me.DrawCommand(Program.Subroutines(CurrentSub).Lines(CurrentLine - 1), CurrentLine)
			Next
			
			'Remove unused icons
			For CurrentLine = Program.Subroutines(CurrentSub).Lines.Count + 1 To MainForm.MaxSubSize + 10
				Try
					RemoveHandler Me.IconDisplay(CurrentLine).MouseUp, AddressOf Me.ProgEditPanelIconClick
					RemoveHandler Me.IconDisplay(CurrentLine).MouseMove, AddressOf Me.ProgIconMouseMove
				Catch Ex As Exception
				End Try
				Try
					Me.ProgEditPanel.Controls.Remove(Me.IconDisplay(CurrentLine))
				Catch
					Exit For
				End Try
			Next
			
			'Restore scroll
			me.ProgEditPanel.AutoScrollPosition = New System.Drawing.Point(0, CurrentScroll(CurrentSub))
			Me.ProgEditPanel.Controls.Remove(placeSaver)
			
		End Sub
		
		Private Sub DrawCommand(ByVal DataTemp As String, CurrentLine As Integer)
			Dim IconName As String = ""
			Dim ProcessTemp as String
			Dim CommandName As String
			Dim CommandPointer, IsMatch As Integer
			Dim IconToolTip As String
			Dim IconParameters As New List(Of String)
			
			IsMatch = 0
			IconToolTip = ""
			
			'Add complete command as first parameter
			IconParameters.Add(DataTemp)
			
			'Special cases
			'Comment
			If DataTemp.StartsWith("'") Then
				DataTemp = DataTemp.Replace("'", "")
				IconName = "Comment"
				IconParameters.Add(DataTemp)
				IconToolTip = DataTemp
				
			'Label
			Else If DataTemp.EndsWith(":") Then
				IconName = "Label"
				If DataTemp.IndexOf("SysBlankLabel") = -1 Then
					IconParameters.Add(DataTemp.Substring(0, DataTemp.Length - 1))
				End If
				
			'Subroutine
			Else If Program.IsSub(DataTemp) Then
				IconName = "Subroutine"
				IconParameters.Add(DataTemp)
				Try
					IconToolTip = Program.GetCalledSub(DataTemp).Description
				Catch
					IconToolTip = ""
				End Try
				
			'Get Command data
			Else
				CommandName = DataTemp.ToLower.Trim + " "
				
				'Get Command data Pointer
				ProcessTemp = Program.GetCommandData(CommandName)
				CommandPointer = -1
				IsMatch = 0
				If ProcessTemp.StartsWith("Command:") Then
					CommandPointer = ProcessTemp.Substring(8).Trim
				Else If ProcessTemp.StartsWith("Match:") Then
					CommandPointer = ProcessTemp.Substring(6).Trim
					IsMatch = -1
				End If
				
				'Found command data, draw
				If CommandPointer <> -1 Then
					With MainForm.CommandList(CommandPointer)
						'Get Parameters
						If IsMatch <> -1 Then IconParameters.AddRange(Program.GetParamString(DataTemp, .Syntax))
						'MessageBox.Show(IconParameters, "Message", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1)
						
						'Get Icon Name
						If IsMatch <> -1 Then IconName = .Icon
						If IsMatch = -1 Then IconName = .MatchIcon
						
						'Get tool tip
						If .Tooltip <> "" Then
							IconToolTip = .Tooltip
						End If
						
						'Get Indent
						If IsMatch <> -1 Then
							If .MatchSyntax <> "" Then IsMatch = 1
						End If
					End With
				End If
			End If
			
			'Draw
			'Calculate indent of icon
			'IsMatch values affect indent:
			'1 - move out after
			'0 - stay same
			'-1 - move in at
			'-2 - move in then out after
			If IsMatch = 1 Then
				IconIndent(CurrentLine, 1) = IconIndent(CurrentLine - 1, 2)
				IconIndent(CurrentLine, 2) = IconIndent(CurrentLine, 1) + 1
			Else If IsMatch = 0 Then
				IconIndent(CurrentLine, 1) = IconIndent(CurrentLine - 1, 2)
				IconIndent(CurrentLine, 2) = IconIndent(CurrentLine, 1)
			Else If IsMatch = -1 Then
				IconIndent(CurrentLine, 1) = IconIndent(CurrentLine - 1, 2) - 1
				IconIndent(CurrentLine, 2) = IconIndent(CurrentLine, 1)
			End If
			If CommandPointer <> -1 Then
				If Not MainForm.CommandList(CommandPointer) Is Nothing Then
					If MainForm.CommandList(CommandPointer).OnlyIn <> "" And MainForm.CommandList(CommandPointer).OnlyIn <> "sub" Then
						'IconIndent(CurrentLine) = IconIndent(CurrentLine - 1) - 1
						IconIndent(CurrentLine, 1) = IconIndent(CurrentLine - 1, 2) - 1
						IconIndent(CurrentLine, 2) = IconIndent(CurrentLine, 1) + 1
					End If
				End If
			End If
			
			'Draw icon
			Me.DrawIcon(IconName, IconParameters, CurrentLine, IconIndent(CurrentLine, 1), IconToolTip)
			
		End Sub
		
		Private Sub DrawSubList
			Dim SubNameTemp As String
			Dim CurrListItem As String
			Dim SubListData As New List(Of String)
			Dim Temp As Integer = 0
			Dim ScrollPos As Integer
			Dim thisSub As GCBSubroutine
			
			RemoveHandler Me.SubSelect.SelectedIndexChanged, AddressOf Me.SubSelectSelectedIndexChanged
			
			'Get sub list
			ScrollPos = Me.SubSelect.TopIndex
			Me.SubSelect.Items.Clear()
			Me.SubSelect.Items.Add("Main")
			If Program Is Nothing Then
				Me.SubSelect.SelectedIndex = 0
			Else
				
				'Get sub names
				'For Temp = 1 to Program.SubCount
				For Each thisSub in Program.Subroutines
					If Not Program.IsMainSub(thisSub) Then
						SubListData.Add(thisSub.Name)
					End If
		     	Next
		     	
		     	'Sort sub list
		     	SubListData.Sort
		     	
		     	'Add items
				For Each CurrListItem In SubListData
					SubSelect.Items.Add(CurrListItem)
				Next
		     	
		     	'Select current sub
		     	If CurrentSub = 0 Then
		     		Me.SubSelect.SelectedIndex = 0
		     		
		     	Else
		     		SubNameTemp = Program.Subroutines(CurrentSub).Name
					For Temp = 1 To SubListData.Count
			     		If SubListData(Temp - 1).Trim.ToLower = SubNameTemp.Trim.ToLower Then
			     			 Me.SubSelect.SelectedIndex = Temp
			     			 Exit For
			     		End If
			     	Next
		     	End If
			     	
			End If
			
			Me.SubSelect.TopIndex = ScrollPos
	     	AddHandler Me.SubSelect.SelectedIndexChanged, AddressOf Me.SubSelectSelectedIndexChanged
	     	
		End Sub
		
		Private Sub DrawIcon(ByVal IconName As String, ParamList As List(Of String), Pos As Integer, ByVal Indent As Integer, Optional IconToolTip As String = "")
			Dim IconHandler As String = Pos.ToString + ":" + IconName
			Dim Temp As Integer = 0
			Dim CurrIcon, SearchIcon As GCBIcon
			
			'Get the icon
			If IconName = "" Then
				IconName = "default"
			Else
				IconName = IconName.Trim.ToLower
			End If
			Do
							
				'Find icon data
				CurrIcon = Nothing
				For Each SearchIcon In MainForm.IconList
					If SearchIcon.Name = IconName Then
						CurrIcon = SearchIcon
						Exit For
					End If
				Next
				
				If CurrIcon Is Nothing Then
					If IconName = "default" Then
						Exit Sub
					Else
						IconName = "default"
					End If
				End If
			Loop While CurrIcon Is Nothing
			
			If Not CurrIcon Is Nothing Then
				'Decide where to place icon
				CurrentIconPlace = IconSpacing(Pos)
			 	If Indent < 100 Then IconSpacing(Pos + 1) = CurrentIconPlace + (20 + CurrIcon.YSize) * pDispScale
			 	If Indent > 100 Then Indent = Indent - 100 '92?
			 	CurrentIconPlace = CurrentIconPlace + me.ProgEditPanel.AutoScrollPosition.Y
			 	
			 	'Show icon
			 	Me.IconDisplay(Pos) = New IconDisplay(CurrIcon, ParamList, IconHandler)
			 	With Me.IconDisplay(Pos)
				 	.Visible = False
				 	'.Parent = Me.ProgEditPanel
				 	.Zoom = pDispScale
				 	.Location = New System.Drawing.Point((Indent * IndentSize + (200 - CurrIcon.XSize) / 2) * pDispScale, 20 + CurrentIconPlace)
				 	Me.ProgEditPanel.Controls.Add(Me.IconDisplay(Pos))
				 	
				 	'tipProvider.SetToolTip(Me.IconDisplay(Pos), IconTooltip)
				 	AddHandler .MouseUp, AddressOf Me.ProgEditPanelIconClick
				 	AddHandler .MouseMove, AddressOf Me.ProgIconMouseMove
				 	
				 	.ContextMenu = iconRightMenu
				End With
			 	If iconTooltip <> "" Then
					iconTipDisplay.SetToolTip(Me.IconDisplay(Pos), iconTooltip)
				End If
				Me.IconDisplay(Pos).Visible = True
				
				'Line to join matching icons
				'Dim level as Integer
				'Dim divLine As System.Windows.Forms.PictureBox
				'For Level = 1 to Indent
				'	divLine = New System.Windows.Forms.PictureBox
				'	divLine.Size = New Size(2, Me.IconDisplay(Pos).Height)
				'	divLine.BorderStyle = BorderStyle.FixedSingle
				'	divLine.Location = New Point(Level * 20, CurrentIconPlace)
				'	Me.ProgEditPanel.Controls.Add(divLine)
				'Next
				
			End If
			
		End Sub
		
		Private Sub SelectIcon(ByVal IconNo As Integer, Optional Exclusive As Boolean = False)
			'IconNo: 0 for sub start, 1 for first code, 2 for second code, etc
			Dim Temp As Integer = 0
			
			'If exclusive, clear old selection
			If Exclusive Then
				For Temp = 0 To MainForm.MaxSubSize
					Try
						IconDisplay(Temp).IconSelected = False
					Catch ex As NullReferenceException
						'Exit For
						If Temp > Program.Subroutines(CurrentSub).Lines.Count Then
							Exit For
						End If
					End Try
				Next
			End If
			
			'Correct invalid selection
			If IconNo < 0 Then
				IconNo = 0
			End If
			If IconNo > Program.Subroutines(CurrentSub).Lines.Count Then
				IconNo = Program.Subroutines(CurrentSub).Lines.Count
			End If
			
			'Draw new selection
			If Not IconDisplay(IconNo) Is Nothing Then
				IconDisplay(IconNo).IconSelected = True
				CurrentSubLine(CurrentSub) = IconNo
			End If
			
			'Display parameters
			DrawParameters
			'Me.ProgEditPanel.Select
				
			
		End Sub
		
		Public Overrides Function GetCurrentCommand As String
			If CurrentSub = 0 And CurrentSubLine(CurrentSub) = 0 Then Return "MainIcon"
			Return CurrentCodeLine
		End Function
		
		Private Sub EraseParameters
			
			Dim CurrObject As Object
			Dim CurrControl As UserControl
			For Each CurrObject In Me.IconPropertiesPanel.Controls
				If TypeOf CurrObject Is UserControl Then
					CurrControl = CurrObject
					Me.IconPropertiesPanel.Controls.Remove(CurrControl)
					CurrControl.Dispose
				End If
			Next
			
			Me.IconPropertiesPanel.Controls.Clear
		End Sub
		
		Private Sub DrawParameters
			'Sub to draw icon settings on right hand side of panel
			
			Dim DataTemp, CommandName, ParamTemp, CommandType As String
			Dim CommandIndex As Integer
			'Dim TextOnly As Boolean = False
			Dim ParamMode As Integer = 0
			Dim Temp As Integer = 0
			Dim CommandParams As List(Of String)
			
			Dim OptionList(20) As String
			Dim OptionCount As Integer = 0
			
			Dim ParamList(MainForm.MaxIconParams) As ParamListItem
			Dim ParamText As String = ""
			Dim ParamStartY As Integer = 0
			Dim OfferHelp As Boolean = False
			
			'Erase Old Parameters
			EraseParameters
			CommandName = ""
			
			If Program Is Nothing Then
				CommandIndex = -6
				
			Else
				'Get name and data location
				CommandName = CurrentCodeLine
				'If CommandName = "" Then Exit Sub
				
				DataTemp = Program.GetCommandData(CommandName)
				
				If DataTemp.ToLower = "unknown" Then CommandIndex = -999
			 	If DataTemp.ToLower.IndexOf("subroutine:") <> -1 Then CommandIndex = -1
			 	If DataTemp.ToLower.IndexOf("comment:") <> -1 Then CommandIndex = -2
			 	If DataTemp.ToLower.IndexOf("label:") <> -1 Then CommandIndex = -3
			 	If DataTemp.ToLower.IndexOf("command:") <> -1 Then
			 		CommandIndex = DataTemp.Substring(DataTemp.IndexOf(":") + 1)
			 	End If
			 	If DataTemp.ToLower.IndexOf("match:") <> -1 Then CommandIndex = -5
			 	If CurrentSubLine(CurrentSub) = 0 Then CommandIndex = -4
			 	If CurrentSubLine(CurrentSub) = 0 And CurrentSub = 0 Then CommandIndex = -6
			 	CommandType = DataTemp
			 	CurrentIconCommand = CommandIndex
			End If
			
			'Get parameters
			Select Case CommandIndex
			
			'Subroutine call
			Case -1
				NewCommandTemplate = CurrentCodeLine
				ParamMode = 0
				
				Dim SourceLib As Integer = 0
				Dim LineTemp As String
				Dim NameTemp, ValueTemp As String
				Dim DescTemp As String
				Dim BracketLevel As Integer
				Dim CalledSub As GCBSubroutine
				
				'Get SubTemp, LineTemp
				LineTemp = CurrentCodeLine.Trim
				LineTemp = LineTemp.Replace(" (", "(")
				CalledSub = Program.GetCalledSub(CurrentCodeLine)
				
				'Build parameter list
				ParamCount = 0
				If CalledSub.Parameters.Count > 0 Then
					'Add in brackets to LineTemp if not present
					If LineTemp.IndexOf(" ") <> -1 And LineTemp.IndexOf("(") > LineTemp.IndexOf(" ") Then
						For Temp = 0 To LineTemp.Length - 1
							If LineTemp.Substring(Temp, 1) = " " Then
								LineTemp = LineTemp.Substring(0, Temp) + "(" + LineTemp.Substring(Temp + 1) + ")"
								Exit For
							End If
						Next
					End If
					
					'Reduce to just the parameters
					If LineTemp.IndexOf(" ") = -1 And LineTemp.IndexOf("(") = -1 Then LineTemp = ""
					If LineTemp.IndexOf("(") = -1 And LineTemp.IndexOf(" ") <> -1 Then LineTemp = LineTemp.Substring(LineTemp.IndexOf(" ") + 1).Trim
					If LineTemp.IndexOf("(") <> -1 Then LineTemp = LineTemp.Substring(LineTemp.IndexOf("(") + 1).Trim
					If LineTemp.EndsWith(")") Then LineTemp = LineTemp.Substring(0, LineTemp.Length - 1)
					
					'Replace any commas used in functions
					BracketLevel = 0
					For Temp = 0 To LineTemp.Length - 1
						If LineTemp.Substring(Temp, 1) = "(" Then BracketLevel += 1
						If LineTemp.Substring(Temp, 1) = ")" Then BracketLevel -= 1
						If LineTemp.Substring(Temp, 1) = "," And BracketLevel > 0 Then
							Mid(LineTemp, Temp) = Convert.ToChar(28)
						End If
					Next
					
					'Generate parameter and value lists
					ParamCount = 0
					'Do While SubTemp.IndexOf(",") <> -1
					Dim CalledSubParam As SubParam
					For Each CalledSubParam In CalledSub.Parameters
						'NameTemp = SubTemp.Substring(0, SubTemp.IndexOf(",")).Trim
						'SubTemp = SubTemp.Substring(SubTemp.IndexOf(",") + 1)
						NameTemp = CalledSubParam.GetCode
						If LineTemp.IndexOf(",") <> -1 Then
							ValueTemp = LineTemp.Substring(0, LineTemp.IndexOf(",")).Trim
							LineTemp = LineTemp.Substring(LineTemp.IndexOf(",") + 1)
						Else
							ValueTemp = LineTemp.Trim
							LineTemp = ""
						End If
						
						ParamCount += 1: SubParamList(ParamCount) = New SubParam(NameTemp, False)
						SubParamList(ParamCount).Description = CalledSubParam.Description
						SubParamList(ParamCount).CurValue = ValueTemp
					Next
					
					'Copy data into editor form
					For Temp = 1 To ParamCount
						With SubParamList(Temp)
							If SubParamList(Temp).Description <> "" Then
								DescTemp = SubParamList(Temp).Description
								
							Else
								DescTemp = MainForm.Translator.GetMessage("GuiEditorPanel", "type", "Type:") + " " + .DataType
								If .DataArray Then
									DescTemp += " " + MainForm.Translator.GetMessage("GuiEditorPanel", "Array", "array")
								End If
								If .DataDir = "I" Then
									DescTemp += ", " + MainForm.Translator.GetMessage("GuiEditorPanel", "inputonly", "Input only")
								ElseIf .DataDir = "O" Then
									DescTemp += ", " + MainForm.Translator.GetMessage("GuiEditorPanel", "outputonly", "Output only")
								End If
								If .DefValue <> "" Then
									DescTemp += ", " + MainForm.Translator.GetMessage("GuiEditorPanel", "DefValue", "Default value:") + " " + .DefValue
								End If
							End If
							
							ParamList(Temp) = New ParamListItem
							ParamList(Temp).Name = .Name		'Name
							ParamList(Temp).Desc = DescTemp	'Description
							ParamList(Temp).Value = .CurValue	'Value
							If .DataArray Then
								ParamList(Temp).Options = "array"
							Else
								ParamList(Temp).Options = .DataType	'Valid types
							End If
							ParamList(Temp).Mode = "textbox"	'Editor type
						End With
					Next
					
					'Set template
					NewCommandTemplate = CalledSub.Name.Trim + " "
					For Temp = 1 To ParamCount
						NewCommandTemplate = NewCommandTemplate + "%P" + Temp.ToString + "%"
						If Temp < ParamCount Then NewCommandTemplate = NewCommandTemplate + ", "
					Next
					
				End If
				If ParamCount = 0 Then
					ParamMode = 1
					ParamText = MainForm.Translator.GetMessage("GuiEditorPanel", "SubNoParams", "This subroutine has no parameters")
				End If
				
			'Comment
			Case -2
				ParamTemp = CommandName
				'If ParamTemp <> "'" Then ParamTemp = ParamTemp.Substring(1)
				ParamTemp = ParamTemp.Substring(1)
				ParamCount = 1
				ParamList(1) = New ParamListItem
				ParamList(1).Name = MainForm.Translator.GetMessage("GuiEditorPanel", "CommentName", "Comment")
				ParamList(1).Desc = MainForm.Translator.GetMessage("GuiEditorPanel", "CommentDesc", "The text to show in the comment box")
				ParamList(1).Value = ParamTemp
				ParamList(1).Options = ""
				ParamList(1).Mode = "textbox"
				NewCommandTemplate = "'%P1%"
				OfferHelp = True
				
			'Label
			Case -3
				ParamCount = 1
				ParamList(1) = New ParamListItem
				ParamList(1).Name = MainForm.Translator.GetMessage("GuiEditorPanel", "LabelName", "Label")
				ParamList(1).Desc = MainForm.Translator.GetMessage("GuiEditorPanel", "LabelDesc", "The name of this label")
				ParamList(1).Value = CommandName.Replace(":", "")
				If ParamList(1).Value = "SysBlankLabel" Then ParamList(1).Value = ""
				ParamList(1).Options = ""
				ParamList(1).Mode = "textbox"
				NewCommandTemplate = "%P1%:[Label]"
				OfferHelp = True
			
			'Sub start icon
			Case -4
				'Dim SubTemp, TempParamData, SubName As String
				'Dim ForceIn As Boolean = False
				Dim thisSub As GCBSubroutine = Program.Subroutines(CurrentSub)
				Dim newParam As SubParam
				
				ParamCount = 0
				For Each newParam In thisSub.Parameters
					ParamCount += 1
					SubParamList(ParamCount) = newParam
				Next
				
				'Copy from sub param list into display
				For Temp = 1 To ParamCount
					With SubParamList(Temp)
						ParamList(Temp) = New ParamListItem
						ParamList(Temp).Name = .Name
						If .DataArray Then
							ParamList(Temp).Desc = MainForm.Translator.GetMessage("GuiEditorPanel", "Array", "array")
						Else
							ParamList(Temp).Desc = .DataType
						End If
					End With
				Next
				
				ParamMode = 2
				ParamSubName = thisSub.Name
				NewCommandTemplate = thisSub.Name + " ("
			
			'Match
			Case -5
				OfferHelp = True
				ParamMode = 1
				ParamText = MainForm.Translator.GetMessage("GuiEditorPanel", "IconNoSettings", "This icon has no settings")
				
			'Start of main program
			Case -6
				'Show welcome panel
				ParamMode = 3
				ParamText = MainForm.Translator.GetMessage("GuiEditorPanel", "IconNoSettings", "This icon has no settings")
			
			'Unknown Command
			Case -999
				ParamCount = 1
				ParamList(1) = New ParamListItem
				ParamList(1).Name = MainForm.Translator.GetMessage("GuiEditorPanel", "UnknownName", "Unknown Command")
				ParamList(1).Desc = MainForm.Translator.GetMessage("GuiEditorPanel", "UnknownDesc", "Please edit this command manually")
				ParamList(1).Value = CommandName
				ParamList(1).Options = ""
				ParamList(1).Mode = "textbox"
				NewCommandTemplate = "%P1%[NoTkNoWn]"
			
			'Command
			Case >= 0
				OfferHelp = True
				With MainForm.CommandList(CommandIndex)
					NewCommandTemplate = .Syntax
					'CommandParams = Program.GetParamString(CommandName, .Syntax)
					
					'Get parameters using old code or regex?
					If Not .RegExp Is Nothing Then
						CommandParams = Program.GetParams(CommandName, .RegExp)
					Else
						CommandParams = Program.GetParamString(CommandName, .Syntax)
					End If
					
					ParamCount = .Params
					For Temp = 1 To ParamCount
						With .Param(Temp)
							ParamList(Temp) = New ParamListItem
							ParamList(Temp).Name = .Name
							ParamList(Temp).Desc = .Description
							ParamList(Temp).Options = .ParamType
							ParamList(Temp).Mode = "combo"
							If ParamList(Temp).Options.IndexOf("|") = -1 And ParamList(Temp).Options.ToUpper.IndexOf("CGR_") <> -1 Then ParamList(Temp).Mode = "dropdown"
							If ParamList(Temp).Options.IndexOf("|") = -1 And ParamList(Temp).Options.ToUpper.IndexOf("LABEL") <> -1 Then ParamList(Temp).Mode = "dropdown"
							If ParamList(Temp).Options.IndexOf("|") = -1 And ParamList(Temp).Options.ToUpper.IndexOf("TABLE") <> -1 Then ParamList(Temp).Mode = "dropdown"
							
							ParamList(Temp).Value = ""
							Try
								ParamList(Temp).Value = CommandParams(Temp - 1)
							Catch
							End Try
						End With
					Next
				End With
				
				If ParamCount = 0 Then
					'TextOnly = True
					ParamMode = 1
					ParamText = MainForm.Translator.GetMessage("GuiEditorPanel", "IconNoSettings", "This icon has no settings")
				End If
			End Select
			
			'Draw parameters
			If ParamMode = 0 Then
				For Temp = 1 To ParamCount
					
					'Draw
					Me.IconParamBox(Temp) = New ParamEditBox
					Me.IconParamBox(Temp).Init(Program, Program.Subroutines(CurrentSub), ParamList(Temp).Name, ParamList(Temp).Value, ParamList(Temp).Options, ParamList(Temp).Desc, (Temp < ParamCount))
					Me.IconParamBox(Temp).Enabled = True
					Me.IconParamBox(Temp).Size = New Size(Me.IconPropertiesPanel.Width - 8, 96)
					Me.IconPropertiesPanel.Controls.Add(Me.IconParamBox(Temp))
					Me.IconParamBox(Temp).Location = New System.Drawing.Point(4, 4 + (Temp - 1) * 100 + ParamStartY)
					Me.IconParamBox(Temp).Anchor = AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Top
					AddHandler Me.IconParamBox(Temp).ParamChanged, AddressOf Me.ParameterChanged
				Next
				
				'Add link to relevant help
				If OfferHelp Then
					Dim helpLabel As New LinkLabel
					With helpLabel
						.Size = New Size(Me.IconPropertiesPanel.Width - 8, 24)
						.Dock = DockStyle.Bottom
						.Text = "Show help for this icon"
						AddHandler .LinkClicked, AddressOf Me.IconHelpLinkClicked
					End With
					Me.IconPropertiesPanel.Controls.Add(helpLabel)
				End If
				
				UpdateDisabledParams
			
			'Draw description only
			Else If ParamMode = 1 Then
				'Parameter Name
				Me.IconParamText = New System.Windows.Forms.Label
				Me.IconPropertiesPanel.Controls.Add(Me.IconParamText)
				With Me.IconParamText
					.BackColor = System.Drawing.Color.White
					.Font = New System.Drawing.Font("Microsoft Sans Serif", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
					.ForeColor = System.Drawing.Color.Black
					.Dock = DockStyle.Fill
					.TabIndex = 0
					.TextAlign = System.Drawing.ContentAlignment.TopLeft
					.Text = ParamText
				End With
				
				'Add link to relevant help
				If OfferHelp Then
					Dim helpLabel As New LinkLabel
					With helpLabel
						.Size = New Size(Me.IconPropertiesPanel.Width - 8, 24)
						.Dock = DockStyle.Bottom
						.Text = "Show help for this icon"
						AddHandler .LinkClicked, AddressOf Me.IconHelpLinkClicked
					End With
					Me.IconPropertiesPanel.Controls.Add(helpLabel)
				End If
				
			'Sub parameters
			Else If ParamMode = 2 Then
				'Sub Name
				Me.IconParamText = New System.Windows.Forms.Label
				Me.IconPropertiesPanel.Controls.Add(Me.IconParamText)
				With Me.IconParamText
					.BackColor = System.Drawing.Color.White
					.Font = New System.Drawing.Font("Microsoft Sans Serif", 8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
					.ForeColor = System.Drawing.Color.Black
					.Location = New System.Drawing.Point(4, 4)
					'.Size = New System.Drawing.Size(150, 20)
					.Size = New Size(Me.IconPropertiesPanel.Width - 8, 20)
					.Anchor = AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Top
					.TabIndex = 0
					.TextAlign = System.Drawing.ContentAlignment.TopLeft
					.Text = ParamSubName
				End With
				
				'A Label
				Me.IconParamLabel = New System.Windows.Forms.Label
				Me.IconPropertiesPanel.Controls.Add(Me.IconParamLabel)
				With Me.IconParamLabel
					.BackColor = System.Drawing.Color.White
					.Font = New System.Drawing.Font("Microsoft Sans Serif", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
					.ForeColor = System.Drawing.Color.Black
					.Location = New System.Drawing.Point(4, 30)
					.Size = New Size(Me.IconPropertiesPanel.Width - 8, 20)
					.Anchor = AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Top
					.TabIndex = 0
					.TextAlign = System.Drawing.ContentAlignment.TopLeft
					If Program.Subroutines(CurrentSub).IsFunction Then
						.Text = MainForm.Translator.GetMessage("GuiEditorPanel", "FnParams", "Function Parameters:")
					Else
						.Text = MainForm.Translator.GetMessage("GuiEditorPanel", "SubParams", "Subroutine Parameters:")
					End If
					
				End With
				
				'Parameter List
				Me.IconParamList = New System.Windows.Forms.ListBox
				Me.IconPropertiesPanel.Controls.Add(Me.IconParamList)
				'Format:
				'Name: Type [Array] [,Direction only]
				With Me.IconParamList
					.Location = New System.Drawing.Point(4, 50)
					'.Size = New System.Drawing.Size(150, 200)
					'is at 50, buttons take 24 + 16 = 40, so total 90, add 8 for docking = 98
					.Size = New Size(Me.IconPropertiesPanel.Width - 8, Me.IconPropertiesPanel.Height - 98)
					.Anchor = AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Top Or AnchorStyles.Bottom
					.IntegralHeight = False
					'Add items to Parameter list
					For Temp = 1 To ParamCount
						'.Items.Add(ParamList(Temp, 2) + ": " + ParamList(Temp, 1))
						With SubParamList(Temp)
							DataTemp = .DataType
							If .DataArray Then DataTemp = DataTemp + " " + MainForm.Translator.GetMessage("GuiEditorPanel", "Array", "array")
							DataTemp = DataTemp + ": " + .Name
							If .DataDir = "I" Then
								DataTemp = DataTemp + " [" + MainForm.Translator.GetMessage("GuiEditorPanel", "in", "in") + "]"
							ElseIf .DataDir = "O" Then
								DataTemp = DataTemp + " [" + MainForm.Translator.GetMessage("GuiEditorPanel", "out", "out") + "]"
							End If
							If .DefValue <> "" Then
								'DataTemp = DataTemp + " (Def:" + .DefValue + ")"
								DataTemp = DataTemp + " (" + MainForm.Translator.GetMessage("GuiEditorPanel", "DefaultShort", "Def") + ":" + .DefValue + ")"
							End If
						End With
						.Items.Add(DataTemp)
					Next
				End With
				
				Dim buttonX As Integer
				'Calc X location of first button (add)
				'Want all buttons centered
				'buttons add up to 3 x 50 + 2 x 8 + 2 * 4 = 174, plus 8 padding = 182
				buttonX = (Me.IconPropertiesPanel.Width - 182) / 2
				
				'Add button
				Me.IconParamAdd = New System.Windows.Forms.Button
				Me.IconPropertiesPanel.Controls.Add(Me.IconParamAdd)
				With Me.IconParamAdd
					.Size = New System.Drawing.Size(50, 24)
					'.Location = New System.Drawing.Point(4, 260)
					.Location = New System.Drawing.Point(buttonX + 4, Me.IconPropertiesPanel.Height - 28)
					.Anchor = AnchorStyles.Bottom
					'.Text = "Add"
					.Text = MainForm.Translator.GetMessage("GuiEditorPanel", "AddSubButton", "Add...")
					.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
					.BackColor = System.Drawing.SystemColors.Control
					.FlatStyle = System.Windows.Forms.FlatStyle.System
					AddHandler .Click, AddressOf Me.IconParamAddClick
				End With
				
				'Edit button
				Me.IconParamEdit = New System.Windows.Forms.Button
				Me.IconPropertiesPanel.Controls.Add(Me.IconParamEdit)
				With Me.IconParamEdit
					.Size = New System.Drawing.Size(50, 24)
					'.Location = New System.Drawing.Point(54, 260)
					.Location = New System.Drawing.Point(buttonX + 58, Me.IconPropertiesPanel.Height - 28)
					.Anchor = AnchorStyles.Bottom
					'.Text = "Edit"
					.Text = MainForm.Translator.GetMessage("GuiEditorPanel", "EditSubButton", "Edit...")
					.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
					.BackColor = System.Drawing.SystemColors.Control
					.FlatStyle = System.Windows.Forms.FlatStyle.System
					AddHandler .Click, AddressOf Me.IconParamEditClick
				End With
				
				'Delete button
				Me.IconParamDelete = New System.Windows.Forms.Button
				Me.IconPropertiesPanel.Controls.Add(Me.IconParamDelete)
				With Me.IconParamDelete
					.Size = New System.Drawing.Size(50, 24)
					'.Location = New System.Drawing.Point(104, 260)
					.Location = New System.Drawing.Point(buttonX + 112, Me.IconPropertiesPanel.Height - 28)
					.Anchor = AnchorStyles.Bottom
					'.Text = "Delete"
					.Text = MainForm.Translator.GetMessage("GuiEditorPanel", "DeleteSubButton", "Delete")
					.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
					.BackColor = System.Drawing.SystemColors.Control
					.FlatStyle = System.Windows.Forms.FlatStyle.System
					AddHandler .Click, AddressOf Me.IconParamDeleteClick
				End With
			
			'Draw welcome panel
			Else If ParamMode = 3 Then
				DrawWelcomePanel
				
			End If
			
		End Sub
		
		Private Sub DrawWelcomePanel
			
			'Clear anything currently in IconPropertiesPanel
			IconPropertiesPanel.Controls.Clear
			
			'Welcome message
			Dim WelcomeDisplay As New WelcomePanel
			Me.IconPropertiesPanel.Controls.Add(WelcomeDisplay)
			With WelcomeDisplay
				.Dock = DockStyle.Fill
				.TabIndex = 0
			End With
			
		End Sub
		
		Private Sub IconHelpLinkClicked(sender As System.Object, e As System.Windows.Forms.LinkLabelLinkClickedEventArgs)
			Mainform.MainformInstance.ShowCommandHelp(GetCurrentCommand)
			
		End Sub
		
		Private Sub UpdateDisabledParams
			'Update disabled param editors
			Dim Temp, ControllerID As Integer
			Dim ShowParam As Boolean
			
			If CurrentIconCommand < 1 Then Exit Sub
			
			For Temp = 1 to IconParamBox.Length
				If Not IconParamBox(Temp) Is Nothing
					'Should this param be shown?
					ShowParam = True
					With MainForm.CommandList(CurrentIconCommand)
						'Check for constraint that controls this param
						Dim Constraint As ParamConstraint
						For Each Constraint In .Constraints
							If Constraint.GetControlled = "p" + Temp.ToString.Trim Then
								'Get controller of constraint
								ControllerID = Constraint.GetController.Substring(1)
								If Constraint.ConditionTrue(Constraint.GetController, IconParamBox(ControllerID).GetValue) Then
									'Check conditions
									If Constraint.HasAction("hide") Then
										ShowParam = False
									End If
									If Constraint.HasAction("clear") Then
										IconParamBox(Temp).SetValue("", True)
									End If
									Exit For
								End If
							End If
						Next
					End With
					
					If ShowParam Then
						Me.IconParamBox(Temp).Enabled = True
					Else
						Me.IconParamBox(Temp).Enabled = False
					End If
				Else
					'Once have found one empty editor, have reached end of params
					Exit Sub
				End If
			Next
		End Sub
		
		Public Overrides Sub ZoomIn
			Zoom = Zoom * 1.2
		End Sub
		
		Public Overrides Sub ZoomOut
			Zoom = Zoom / 1.2
		End Sub
		
		Public Overrides Sub ZoomReset
			Zoom = 1
		End Sub
		
		Private Sub IconParamAddClick(sender As System.Object, e As System.EventArgs)
			Dim DataTemp As String
			
			Dim thisSub As GCBSubroutine = Program.Subroutines(CurrentSub)
			Dim fParam As New SubParamEditor(thisSub)
			fParam.ShowDialog
			Try
				SubParamList(ParamCount + 1) = fParam.GetOutput
				DataTemp = SubParamList(ParamCount + 1).Name.Substring(0, 1)
			Catch
				Exit Sub
			End Try
			
			ParamCount += 1
			With SubParamList(ParamCount)
				
				DataTemp = .DataType
				If .DataArray Then DataTemp = DataTemp + " " + MainForm.Translator.GetMessage("GuiEditorPanel", "Array", "array")
				DataTemp = DataTemp + ": " + .Name
				If .DataDir = "I" Then
					DataTemp = DataTemp + " [" + MainForm.Translator.GetMessage("GuiEditorPanel", "in", "in") + "]"
				ElseIf .DataDir = "O" Then
					DataTemp = DataTemp + " [" + MainForm.Translator.GetMessage("GuiEditorPanel", "out", "out") + "]"
				End If
				If .DefValue <> "" Then
					'DataTemp = DataTemp + " (Def:" + .DefValue + ")"
					DataTemp = DataTemp + " (" + MainForm.Translator.GetMessage("GuiEditorPanel", "DefaultShort", "Def") + ":" + .DefValue + ")"
				End If
				
			End With
			Me.IconParamList.Items.Add(DataTemp)
			
			SaveIconParams
		End Sub
		
		Private Sub IconParamEditClick(sender As System.Object, e As System.EventArgs)
			
			Dim Temp As Integer = Me.IconParamList.SelectedIndex
			If Temp = -1 Then Exit Sub
			Dim NewData As SubParam
			Dim DataTemp As String
			
			Dim thisSub As GCBSubroutine = Program.Subroutines(CurrentSub)
			Dim fParam As New SubParamEditor(thisSub, SubParamList(Temp + 1))
			fParam.ShowDialog
			NewData = fParam.GetOutput
			Try
				DataTemp = NewData.Name.Substring(0, 1)
			Catch
				Exit Sub
			End Try
			
			SubParamList(Temp + 1) = NewData
			With NewData
				DataTemp = .DataType
				If .DataArray Then DataTemp = DataTemp + " " + MainForm.Translator.GetMessage("GuiEditorPanel", "Array", "array")
				DataTemp = DataTemp + ": " + .Name
				If .DataDir = "I" Then
					DataTemp = DataTemp + " [" + MainForm.Translator.GetMessage("GuiEditorPanel", "in", "in") + "]"
				ElseIf .DataDir = "O" Then
					DataTemp = DataTemp + " [" + MainForm.Translator.GetMessage("GuiEditorPanel", "out", "out") + "]"
				End If
				If .DefValue <> "" Then
					'DataTemp = DataTemp + " (Def:" + .DefValue + ")"
					DataTemp = DataTemp + " (" + MainForm.Translator.GetMessage("GuiEditorPanel", "DefaultShort", "Def") + ":" + .DefValue + ")"
				End If
			End With
			
			Me.IconParamList.Items.RemoveAt(Temp)
			Me.IconParamList.Items.Insert(Temp, DataTemp)
			
			SaveIconParams
		End Sub
		
		Private Sub IconParamDeleteClick(sender As System.Object, e As System.EventArgs)
			Dim Temp As Integer = Me.IconParamList.SelectedIndex
			if Temp = -1 Then Exit Sub
			
			Dim result As DialogResult = MessageBox.Show(MainForm.Translator.GetMessage("GuiEditorPanel", "ConfirmParamDelete", "Are you sure that you want to delete this parameter?"), "Great Cow Graphical BASIC", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
			If result = DialogResult.Cancel Then Exit Sub
			Me.IconParamList.Items.RemoveAt(Temp)
			
			Dim DelLoop As Integer
			ParamCount -= 1
			For DelLoop = (Temp + 1) To ParamCount
				SubParamList(DelLoop) = SubParamList(DelLoop + 1)
			Next
			
			SaveIconParams
		End Sub
		
		Private Sub SaveIconParams
			Dim Temp As Integer = 0
			Dim NewCommand As String = NewCommandTemplate
			Dim thisSub As GCBSubroutine = Program.Subroutines(CurrentSub)
			
			'thisSub.Parameters = New ArrayList
			thisSub.Parameters = New List(Of SubParam)
			For Temp = 1 To ParamCount
				thisSub.Parameters.Add(SubParamList(Temp))
			Next
			
			Program.HasChanged = True
		End Sub
		
		Private Sub ParameterChanged
			Dim RefreshAll As Boolean = False
			Dim NewCommand As String = NewCommandTemplate
			Dim LabelTemp As String = ""
			Dim OldCommand As String = ""
			Dim ParamChanged As Integer
			Dim MinorParamChange As Boolean = False
			
			Dim MinorChange As Boolean = False
			
			If NewCommand.IndexOf("[NoTkNoWn]") <> -1 Then
				RefreshAll = True
				NewCommand = NewCommand.Replace("[NoTkNoWn]", "").Trim
			End If
			If NewCommand.IndexOf("[Label]") <> -1 Then
				NewCommand = NewCommand.Replace("[Label]", "")
				If IconParamBox(1).GetValue = "" Then Lowlevel.Replace(NewCommand, "%P1%", "SysBlankLabel")
			End If
			
			If NewCommand <> "" Then
								
				Dim TempData As String
				Dim TempNo As Integer
				
				ParamChanged = -1
				Dim StartPos As Integer = 0
				Do While NewCommand.ToUpper.IndexOf("%P", StartPos) <> -1
			 		TempData = NewCommand.Substring(NewCommand.ToUpper.IndexOf("%P") + 2)
			 		If TempData.Contains("%") Then
			 			TempNo = TempData.Substring(0, TempData.IndexOf("%"))
				 		With IconParamBox(TempNo)
				 			Lowlevel.Replace(NewCommand, "%P" + TempNo.ToString + "%", .GetValue)
				 			If .HasParamChanged Then
				 				MinorParamChange = .IsMinorChange
				 				.ClearParamChanged
				 				ParamChanged = TempNo
				 			End If
				 		End With
			 		Else
			 			StartPos = NewCommand.ToUpper.IndexOf("%P") + 2
			 		End If
			 		
			 	Loop
				
				OldCommand = CurrentCodeLine
				CurrentCodeLine = NewCommand
				Program.HasChanged = True
				
				'Add undo event
				Dim NewUndoEvent As UndoHistoryItem = New UndoHistoryItem
				With NewUndoEvent
					.Action = UndoHistoryItem.Change
					.SubNo = Program.Subroutines(CurrentSub)
					.LineNo = CurrentSubLine(CurrentSub) - 1
					.OldCmd = OldCommand
					.NewCmd = NewCommand
					.ParamNo = ParamChanged
				End With
				'If editing same parameter as last time and only 1 char altered, change is minor
				Dim LastEventList As List(Of UndoHistoryItem) = Program.UndoRedoHistory(Program.UndoRedoLocation)
				If Not LastEventList Is Nothing Then
					Dim LastUndoEvent As UndoHistoryItem = LastEventList(LastEventList.Count - 1)
					With LastUndoEvent
						If .SubNo Is Program.Subroutines(CurrentSub) And .LineNo = CurrentSubLine(CurrentSub) - 1 And .ParamNo <> -1 And .ParamNo = ParamChanged Then
							MinorChange = MinorParamChange
						End If
					End With
				End If
				'If change is minor, don't create a new undo event
				AddUndo(NewUndoEvent, MinorChange)
				
				'MessageBox.Show("Redrawing", "Param Changed")
				If RefreshAll = True Then
					ProgramPartialRedraw(CurrentSubLine(CurrentSub))
				Else
					Try
						RemoveHandler Me.IconDisplay(CurrentSubLine(CurrentSub)).MouseUp, AddressOf Me.ProgEditPanelIconClick
						RemoveHandler Me.IconDisplay(CurrentSubLine(CurrentSub)).MouseMove, AddressOf Me.ProgIconMouseMove
					Catch Ex As Exception
					End Try
					Try
						Me.ProgEditPanel.Controls.Remove(Me.IconDisplay(CurrentSubLine(CurrentSub)))
					Catch
					End Try
					
					Me.DrawCommand(CurrentCodeLine, CurrentSubLine(CurrentSub))
					IconDisplay(CurrentSubLine(CurrentSub)).IconSelected = True
				End If
				
			End If
			
			UpdateDisabledParams
			
		End Sub
		
		Private Sub ClearSelection
			Dim Temp As Integer = 0
			
			For Temp = 0 To MainForm.MaxSubSize
				IconDisplay(Temp).IconSelected = False
			Next
		End Sub
		
		Public Overrides Sub ClipboardOp(ByVal Operation As String)
			
			Dim ClipboardData As String = ""
			
			'Copy data to clipboard
			If Operation = "c" Or Operation = "x" Then
				ClipboardData = GetSelectedCode
				'MessageBox.Show(ClipboardData, "Message", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1)
				If GetSelectedCode <> "" Then
					Clipboard.SetDataObject(ClipboardData, True)
				End If
			End If
			
			'Delete selected data (cut)
			If Operation = "x" Then	DeleteSelection
			
			'Copy data from clipboard
			If Operation = "v" Then
				
				Dim ClipboardIn As IDataObject = Clipboard.GetDataObject()
				If ClipboardIn.GetDataPresent(DataFormats.Text) Then
					ClipboardData = ClipBoardIn.GetData(DataFormats.Text)
					AddAfterSelected(ClipBoardData)
				End If
				
			End If
			
		End Sub
		
		'Copies the currently selected icons into a string. Icons are separated by a newline.
		Private Function GetSelectedCode As String
			
			Dim Temp As Integer
			Dim IndentCount, BaseIndent As Integer
			Dim LineTemp As String
			Dim AddNewLine As Boolean = False
			Dim OutputCode As String = ""
			
			BaseIndent = -1
			'Need to check every icon in current sub to see if it's selected
			For Temp = 1 To Program.Subroutines(CurrentSub).Lines.Count
				If IconDisplay(Temp).IconSelected Then
					'Calculate indent
					If BaseIndent = -1 Then
						BaseIndent = IconIndent(Temp, 1)
					End If
					IndentCount = IconIndent(Temp, 1) - BaseIndent
					
					'Get the line
					LineTemp = Program.Subroutines(CurrentSub).Lines(Temp - 1)
					If LineTemp.ToUpper.StartsWith("LET ") Then LineTemp = LineTemp.Substring(4).Trim
					If LineTemp.ToLower.StartsWith("waitfor ") Then LineTemp = "Wait " + LineTemp.Substring(7).Trim
					
					'Add line to output
					If AddNewLine Then
						OutputCode = OutputCode + Environment.NewLine + LowLevel.GetIndentSpaces(IndentCount) + LineTemp
					Else
						OutputCode = LowLevel.GetIndentSpaces(IndentCount) + LineTemp
						AddNewLine = True
					End If
				End If
			Next
			
			Return OutputCode
		End Function
		
		Public Overrides Sub AddAfterSelected(NewCode As String, Optional AppendUndo As Boolean = False)
			
			'NewCode is code to add, lines are separated by newlines
			'AppendUndo is true when this operation is part of the previous one, and a
			'new undo event should not be created
			
			Dim PastedLines As List(Of String)
			Dim LastIconRemoved As Integer
			Dim NewCommand As String
			Dim CommandsAdded As Integer
			
			'Get commands to add
			'Replace tabs with spaces
			PastedLines = LowLevel.GetCommands(NewCode)
			
			'Add commands
			Dim NewUndoItem As UndoHistoryItem
			LastIconRemoved = CurrentSubLine(CurrentSub)
			Dim IsFirstPastedLine As Boolean = True
			CommandsAdded = 0
			For Each NewCommand In PastedLines
				
				'Try reading as directive, if not directive then add
				If Not Program.ReadDirective(NewCommand) Then
					'Check command
					If Not Program.CanInsertCommand(NewCommand, Program.Subroutines(CurrentSub), CurrentSubLine(CurrentSub) - 1) Then
						MessageBox.Show(NewCommand + " " + MainForm.Translator.GetMessage("GuiEditorPanel", "CannotInsert", "cannot be inserted here"), "Great Cow Graphical BASIC", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
						NewCommand = ""
					End If
					
					'Tidy command
					NewCommand = LowLevel.FixLine(NewCommand)
     				
					'Add command
					If NewCommand <> "" Then
						CurrentSubLine(CurrentSub) += 1
						Program.InsertLine(Program.Subroutines(CurrentSub), NewCommand, CurrentSubLine(CurrentSub) - 1)
						CommandsAdded += 1
						
						'Add Undo item
						NewUndoItem = New UndoHistoryItem
						With NewUndoItem
							.Action = UndoHistoryItem.Add
							.SubNo = Program.Subroutines(CurrentSub)
							.LineNo = CurrentSubLine(CurrentSub) - 1
							.OldCmd = ""
							.NewCmd = NewCommand
						End With
						If IsFirstPastedLine Then
							AddUndo(NewUndoItem, AppendUndo)
							IsFirstPastedLine = False
						Else
							AddUndo(NewUndoItem, True)
						End If
					End If
				
				Else
					'If last line was directive, may need to reload libraries
					Program.ReloadLibraries(True)
				
				End If
			Next
			
			'If commands added, update selection and redraw
			If CommandsAdded > 0 Then
				'Clear Selection
				'For Temp = 0 To MainForm.MaxSubSize
				'	IconSelected(Temp) = False
				'Next
				
				'Redraw
				ProgramPartialRedraw(LastIconRemoved)
				SelectIcon(CurrentSubLine(CurrentSub), True)
			End If
		End Sub
		
		Public Overrides Sub SetCurrentLocation(SubNo As Integer, LineNo As Integer)
			
			'Set sub
			ChangeToSub (SubNo)
			
			'Set line
			ProgEditPanel.ScrollControlIntoView(IconDisplay(LineNo + 1))
			SelectIcon(LineNo, True)
		End Sub
		
		Private Sub AddUndo(ByVal NewEvent As UndoHistoryItem, Optional AddToCurrent As Boolean = False)
			Dim Temp As Integer
			
			'MessageBox.Show(NewEvent.OldCmd + ">" + NewEvent.NewCmd, "New event", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1)
			
			With Program
				If AddToCurrent Then
					.UndoRedoHistory(.UndoRedoLocation).Add(NewEvent)
				Else
					'If out of space, remove oldest event
					If .UndoRedoCount >= MainForm.MaxUndo Then
						For Temp = 1 To .UndoRedoCount - 1
							.UndoRedoHistory(Temp) = .UndoRedoHistory(Temp + 1)
						Next
						.UndoRedoLocation -= 1
						.UndoRedoCount -= 1
					End If
					
					'Store event
					.UndoRedoLocation += 1
					.UndoRedoCount = .UndoRedoLocation
					.UndoRedoHistory(.UndoRedoCount) = New List(Of UndoHistoryItem)
					.UndoRedoHistory(.UndoRedoCount).Add(NewEvent)
					
					'Notify MainForm
					RaiseEvent RefreshUndoRedo
				End If
			End With
		End Sub
		
		Public Overrides Sub DoUndoRedo(ByVal RedoMode As Boolean)
			Dim RedrawStart As Integer = 9999
			Dim Temp As Integer
			Dim RedrawThisSub As Boolean = False
			
			With Program
				'Get input
				If RedoMode Then .UndoRedoLocation += 1
				'If Not RedoMode Then UndoRedoLocation -= 1
				
				'Act
				Dim CurrentUndo As UndoHistoryItem
				'For Each CurrentUndo In UndoRedoHistory(UndoRedoLocation)
				Dim LoopStart, LoopEnd, LoopStep As Integer
				If RedoMode Then
					LoopEnd = .UndoRedoHistory(.UndoRedoLocation).Count - 1
					LoopStart = 0
					LoopStep = 1
				Else
					LoopStart = .UndoRedoHistory(.UndoRedoLocation).Count - 1
					LoopEnd = 0
					LoopStep = -1
				End If
				For Temp = LoopStart To LoopEnd Step LoopStep
					CurrentUndo = .UndoRedoHistory(.UndoRedoLocation).Item(Temp)
					
					With CurrentUndo
						Select Case .Action
							Case UndoHistoryItem.Add
								If RedrawStart > .LineNo Then RedrawStart = .LineNo
								If .SubNo is Program.Subroutines(CurrentSub) Then RedrawThisSub = True
								If RedoMode Then
									Program.InsertLine(.SubNo, .NewCmd, .LineNo)
								Else
									Program.DeleteLine(.SubNo, .LineNo)
								End If
								
							Case UndoHistoryItem.Delete
								If .SubNo Is Program.Subroutines(CurrentSub) Then RedrawThisSub = True
								If RedrawStart > .LineNo Then RedrawStart = .LineNo
								If RedoMode Then
									Program.DeleteLine(.SubNo, .LineNo)
								Else
									Program.InsertLine(.SubNo, .OldCmd, .LineNo)
								End If
								
							Case UndoHistoryItem.Change
								If .SubNo Is Program.Subroutines(CurrentSub) Then RedrawThisSub = True
								If RedrawStart > .LineNo Then RedrawStart = .LineNo
								If RedoMode Then
									.SubNo.Lines(.LineNo) = .NewCmd
								Else
									.SubNo.Lines(.LineNo) = .OldCmd
								End If
								
						End Select
					End With
					
				Next
			End With
			
			'Redraw
			If RedrawThisSub Then
				ProgramPartialRedraw(RedrawStart + 1)
				SelectIcon(CurrentSubLine(CurrentSub))
			End If
			
			If Not RedoMode Then Program.UndoRedoLocation -= 1
			RaiseEvent RefreshUndoRedo
			
		End Sub
		
		Private Sub DeleteSelection(Optional ByRef SomeLocation As Integer = -1)
			'Will decrement SomeLocation, depending on how many lines before it are deleted
			Dim Temp As Integer = 0
			Dim LastIconRemoved As Integer = 0
			Dim OldProgramSize As Integer = Program.Subroutines(CurrentSub).Lines.Count
			
			'Delete
			Dim AddToCurrent As Boolean = False
			Dim redrawNeeded As Boolean = False
			Dim NewUndoItem As UndoHistoryItem
			For Temp = OldProgramSize To 1 Step -1
				If IconDisplay(Temp).IconSelected Then
					
					NewUndoItem = New UndoHistoryItem
					With NewUndoItem
						.Action = UndoHistoryItem.Delete
						.SubNo = Program.Subroutines(CurrentSub)
						.LineNo = Temp - 1
						.OldCmd = Program.Subroutines(CurrentSub).Lines(Temp - 1)
						.NewCmd = ""
					End With
					AddUndo(NewUndoItem, AddToCurrent)
					AddToCurrent = True
					
					Program.DeleteLine(Program.Subroutines(CurrentSub), Temp - 1)
					if CurrentSubLine(CurrentSub) >= Temp Then CurrentSubLine(CurrentSub) -= 1
					If CurrentSubLine(CurrentSub) = 0 And Program.Subroutines(CurrentSub).Lines.Count > 0 Then CurrentSubLine(CurrentSub) = 1
					
					if SomeLocation >= Temp Then SomeLocation -= 1
					If SomeLocation < 0 Then SomeLocation = 0
					
					LastIconRemoved = Temp
					'IconSelected(Temp) = False
					redrawNeeded = True
				End If
			Next
			
			'Check that CurrentSubLine is valid
			if CurrentSubLine(CurrentSub) > Program.Subroutines(CurrentSub).Lines.Count Then CurrentSubLine(CurrentSub) = Program.Subroutines(CurrentSub).Lines.Count
			
			'Ensure correct icon is selected, then redraw
			If redrawNeeded Then
				'Erase icon parameters before messing with icon
				EraseParameters
				
				'IconSelected(CurrentSubLine(CurrentSub)) = True
				ProgramPartialRedraw(LastIconRemoved)
				SelectIcon(CurrentSubLine(CurrentSub), True)
				ProgEditPanel.Select
			End If
			
		End Sub
		
		Private Sub SubSelectSelectedIndexChanged(sender As System.Object, e As System.EventArgs)
			If SubSelect.SelectedIndex <> -1 Then
				ChangeToSub(Program.EditSubIndex(Me.SubSelect.SelectedItem.ToString + "*"))
			End If
		End Sub
		
		Public Overrides Function CanEditProperties As Boolean
			'Returns true to show that properties like chip, data tables, libraries can be edited
			Return True
		End Function
		
		Private Sub ChangeToSub(NewSub As Integer)
			If CurrentSub <> NewSub Then
				CurrentScroll(CurrentSub) = Me.ProgEditPanel.AutoScrollPosition.Y * -1
				CurrentSub = NewSub
				Me.ProgramRedraw()
				
			End If
		End Sub
		
		Private Sub ProgEditPanelIconClick(sender As System.Object, e As System.Windows.Forms.MouseEventArgs)
			'Dim IconClicked As Integer
			Dim Temp As Integer = 0
			
			Dim TempData As String
			Dim IconClicked As Integer
			Dim LoopDir As Integer
			
			TempData = Sender.Name
			If e.Button = MouseButtons.Right Then
				
				Exit Sub
			End If
			
			If e.Button = MouseButtons.Left Then
				'Ctrl, shift both up, deselect existing and select clicked icon
				If (Not ShiftKeyDown) And (Not CtrlKeyDown) Then
					LastIconClicked = TempData.Substring(0, TempData.IndexOf(":"))
					SelectIcon(LastIconClicked, True)
					Me.ProgEditPanel.Select
		 		End If
		 		
		 		'Ctrl down, shift up, toggle selection of clicked icon
				If (Not ShiftKeyDown) And CtrlKeyDown Then
					LastIconClicked = TempData.Substring(0, TempData.IndexOf(":"))
					IconDisplay(LastIconClicked).IconSelected = (Not IconDisplay(LastIconClicked).IconSelected)
					
					CurrentSubLine(CurrentSub) = LastIconClicked
					SelectIcon(-999) 'SelectIcon(CurrentSubLine(CurrentSub))
		 			Me.ProgEditPanel.Select
		 		End If
		 		
		 		'Ctrl up, shift down, select all from last clicked icon to clicked icon
				If ShiftKeyDown And (Not CtrlKeyDown) Then
					IconClicked = TempData.Substring(0, TempData.IndexOf(":"))
					LoopDir = 1
					If IconClicked > LastIconClicked then LoopDir = -1
					
					For Temp = IconClicked To LastIconClicked Step LoopDir
						IconDisplay(Temp).IconSelected = True
					Next
					
					LastIconClicked = IconClicked
					CurrentSubLine(CurrentSub) = LastIconClicked
		 			SelectIcon(CurrentSubLine(CurrentSub))
		 			Me.ProgEditPanel.Select
		 		End If
		 		
		 	End If
			
		End Sub
		
		Private Sub ProgEditPanelClick(sender As System.Object, e As System.EventArgs)
			Dim Temp As Integer = 0
			
			SelectIcon(CurrentSubLine(CurrentSub), True)
			
			Me.ProgEditPanel.Select
			
		End Sub
		
		Private Sub DeleteSubButtonClick(sender As System.Object, e As System.EventArgs)
			If CurrentSub = 0 Then Exit Sub
			
			Dim Temp As Integer = 0
			Dim ConfirmDelete As DialogResult = MessageBox.Show(MainForm.Translator.GetMessage("GuiEditorPanel", "ConfirmSubDelete", "Are you sure that you want to delete this subroutine?"), _
			                                                    MainForm.Translator.GetMessage("GuiEditorPanel", "DeleteSub", "Delete Subroutine: " + Me.SubSelect.SelectedItem), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
			If ConfirmDelete = DialogResult.No Then Exit Sub
			
			Temp = CurrentScroll(Program.Subroutines.Count)
			Program.Subroutines.RemoveAt(CurrentSub)
			CurrentScroll(CurrentSub) = Temp
			CurrentSub = 0
			ProgramRedraw()
			RaiseEvent SubAdded
		End Sub
		
		Public Sub AddSubButtonClick(sender As System.Object, e As System.EventArgs)
			
			'Get Name, Type
			Dim GetSubName As New SubNameDialog(Program)
			GetSubName.ShowDialog()
			If GetSubName.IsAccepted = False Then Exit Sub
			
			If GetSubName.SubName = "" Then Exit Sub
			
			'Add
			Dim newSub As New GCBSubroutine(GetSubName.SubName)
			With newSub
				.ReturnType = GetSubName.SubType
				.Description = GetSubName.SubDesc
				.HideSub = GetSubName.SubHidden
			End With
			Program.Subroutines.Add(newSub)
			Program.HasChanged = True
			
			'Select and Redraw
			CurrentScroll(CurrentSub) = Me.ProgEditPanel.AutoScrollPosition.Y * -1
			CurrentSub = Program.EditSubIndex(GetSubName.SubName)
			ProgramRedraw()
			
			RaiseEvent SubAdded
			
		End Sub
		
		Private Sub EditSubButtonClick(sender As System.Object, e As System.EventArgs)
			Dim MySub As Integer
			Dim EditedSub As GCBSubroutine
			Dim OldName As String
			'Dim NewName As String = ""
			'Dim NewType As String = ""
			
			'Get current sub details
			If SubSelect.SelectedIndex = -1 Then Exit Sub
			MySub = Program.EditSubIndex(SubSelect.SelectedItem)
			If MySub < 1 Then Exit Sub
			EditedSub = Program.Subroutines(MySub)
			OldName = EditedSub.Name
			
			'Create and display dialog
			Dim GetSubName As New SubNameDialog(Program)
			With GetSubName
				.SetTitle("Edit Subroutine")
				.Init(EditedSub.Name, EditedSub.ReturnType, EditedSub.Description, EditedSub.HideSub)
				.ShowDialog()
				
				If Not .IsAccepted Then Exit Sub
				If .SubName = "" Then Exit Sub
				
				'Apply
				EditedSub.Name = .SubName
				EditedSub.ReturnType = .SubType
				EditedSub.Description = .SubDesc
				EditedSub.HideSub = .SubHidden
				Program.HasChanged = True
				
				'Fix calls
				Program.ReplaceInProgram(OldName, .SubName)
				
			End With
			
			'Select and Redraw
			ProgramRedraw()
			
			RaiseEvent SubAdded
			
		End Sub
		
		Private Sub ProgEditPanelDragEnter(sender As System.Object, e As System.Windows.Forms.DragEventArgs)
			
			If e.Data.GetDataPresent(DataFormats.Text) Then
				e.Effect = DragDropEffects.Copy
			Else
				e.Effect = DragDropEffects.None
			End If
		End Sub
		
		Private Sub ProgEditPanelDragDrop(sender As System.Object, e As System.Windows.Forms.DragEventArgs)
			Dim NewCommand As String = ""
			
			'Stop scrolling
			ScrollTimer.Stop
			
			If e.Data.GetDataPresent(DataFormats.Text) Then
				NewCommand = e.Data.GetData(DataFormats.Text)
				
				'AddCommand(NewCommand)
				DraggedCommand = NewCommand
				
				Me.ProgEditPanel.Select
				'Me.Activate
				
			End If
			
		End Sub
		
		Private Dim LastUpX, LastUpY, LastYScroll As Integer
		Private Sub ProgIconMouseMove(sender As System.Object, e As System.Windows.Forms.MouseEventArgs)
			Dim TempData As String
			Dim IconNo As Integer
			Dim CurrYScroll As Integer
			
			ScrollTimer.Stop
			
			If Not DraggedCommand Is Nothing Then
				If DraggedCommand <> "" Then
					'MainForm.SetStatus(sender.Name)
					'Find the location at which to insert the icon
					TempData = Sender.Name
					If TempData Is Nothing Then
						IconNo = 0
					Else
						IconNo = TempData.Substring(0, TempData.IndexOf(":"))
					End If
					AddIcon(DraggedCommand, IconNo)
					
					'Clear dragged command
					DraggedCommand = ""
				End If
			End If
			
			If e.Button = MouseButtons.Left Then
				CurrYScroll = Math.Abs(ProgEditPanel.AutoScrollPosition.Y)
				
				If (Math.Abs(LastUpX - e.X) > 4 And LastUpX <> 0) Or (Math.Abs(LastUpY - e.Y) - Math.Abs(CurrYScroll - LastYScroll) > 4 And LastUpY <> 0) Then
					TempData = Sender.Name
					IconNo = TempData.Substring(0, TempData.IndexOf(":"))
					If IconNo = 0 Then Exit Sub
					If Not IconDisplay(IconNo).IconSelected Then
						SelectIcon(IconNo, Not CtrlKeyDown)
					End If
					TempData = GetSelectedCode
					IconDisplay(IconNo).DoDragDrop(TempData, DragDropEffects.All)
					If ShiftKeyDown Then
						
					Else
						'Delete selection
						'DeleteSelection
						DeleteDragged = True
					End If
				End If
			End If
			
			'Clear DeleteDragged flag if mouse buttons released
			If e.Button = MouseButtons.None Then
				DeleteDragged = False
				LastUpX = e.X
				LastUpY = e.Y
				LastYScroll = Math.Abs(ProgEditPanel.AutoScrollPosition.Y)
			End If
			
		End Sub
		
		Private Sub ProgEditPanelMouseMove(sender As System.Object, e As System.Windows.Forms.MouseEventArgs)
			Dim MousePos, FindLocation, MouseOverIcon As Integer
			Dim MatchCommand As String = ""
			Dim IsName As Boolean = False
			Dim UndoTemp As String = ""
			
			If DraggedCommand <> "" Then
								
				'Find the location at which to insert the icon
				MousePos = e.Y + Math.Abs(Me.ProgEditPanel.AutoScrollPosition.Y)
				MouseOverIcon = Program.Subroutines(CurrentSub).Lines.Count
				For FindLocation = 0 To MainForm.MaxSubSize
					If MousePos < IconSpacing(FindLocation) + 20 Then
						MouseOverIcon = FindLocation - 1
						Exit For
					End If
				Next
				If MouseOverIcon < 0 Then MouseOverIcon = 0
				AddIcon(DraggedCommand, MouseOverIcon)
				
				'Clear dragged command
				DraggedCommand = ""
				
			End If
			
			'Clear DeleteDragged flag if mouse buttons released
			If e.Button = MouseButtons.None Then
				DeleteDragged = False
			End If
			
		End Sub
		
		Private Sub AddIcon(ByVal DraggedCommand As String, ByVal MouseOverIcon As Integer)
			Dim FindMatch As Integer
			Dim IsName As Boolean = False
			Dim AddMatch As Boolean = False
			
			Dim AddToCurrent As Boolean = False
			
			'Get and set current line
			If MouseOverIcon < 0 Then MouseOverIcon = 0
			If DeleteDragged Then
				DeleteSelection(MouseOverIcon)
				DeleteDragged = False
			End If
			CurrentSubLine(CurrentSub) = MouseOverIcon
			If CurrentSubLine(CurrentSub) > Program.Subroutines(CurrentSub).Lines.Count Then
				CurrentSubLine(CurrentSub) = Program.Subroutines(CurrentSub).Lines.Count
			End If
			
			'Get command
			If DraggedCommand.StartsWith("IsName:") Then
				DraggedCommand = DraggedCommand.Substring(7)
				IsName = True
				AddMatch = True
			End If
			
			'Special cases
			If DraggedCommand.Trim.ToLower = "label" Then
				DraggedCommand = "SysBlankLabel:"
				IsName = False
			End If
			If DraggedCommand.Trim.ToLower = "comment" Then
				DraggedCommand = "'"
				IsName = False
			End If
			
			'Find the correct syntax
			If IsName = True Then
				For FindMatch = 0 To MainForm.CommandList.Count - 1
					With MainForm.CommandList(FindMatch)
						If .Name.Trim.ToLower = DraggedCommand.Trim.ToLower Then
							DraggedCommand = .Syntax
							If DraggedCommand.IndexOf("%") <> -1 Then DraggedCommand = DraggedCommand.Substring(0, DraggedCommand.IndexOf("%"))
							Exit For
						End If
					End With
				Next
			End If
			
			'Check to see if the command has a match
			If AddMatch Then
				For FindMatch = 0 To MainForm.CommandList.Count - 1
					If MainForm.CommandList(FindMatch).Name.Trim.ToLower = DraggedCommand.Trim.ToLower Then
						'MatchCommand = MainForm.CommandList(FindMatch).MatchSyntax
						DraggedCommand += (Environment.NewLine + MainForm.CommandList(FindMatch).MatchSyntax)
						Exit For
					End If
				Next
			End If
			
			'Add Command
			AddAfterSelected(DraggedCommand, False)
			
			'Select first added line
			SelectIcon(MouseOverIcon + 1, False)
			
		End Sub
		
		Public Overrides Sub EditorPanelKeyUp(sender As System.Object, e As System.Windows.Forms.KeyEventArgs)
			Dim KeyReleased As String
			KeyReleased = e.KeyData.ToString.Trim.ToLower
			'MessageBox.Show(KeyReleased, "Message", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1)
			
			If KeyReleased = "up" Then
				If CurrentSubLine(CurrentSub) > 0 Then
					CurrentSubLine(CurrentSub) -= 1
					SelectIcon(CurrentSubLine(CurrentSub))
				End If
				e.Handled = True
			
			Else If KeyReleased = "down" Then
				If CurrentSubLine(CurrentSub) < Program.Subroutines(CurrentSub).Lines.Count Then
					 CurrentSubLine(CurrentSub) += 1
					 SelectIcon(CurrentSubLine(CurrentSub))
				End If
				e.Handled = True
			
			Else If KeyReleased = "delete" Then
				DeleteSelection
				e.Handled = True
			End If
		End Sub
		
		Private Sub ProgEditPanelMouseEnter(sender As System.Object, e As System.EventArgs)
			'Is this more annoying?
			'ProgEditPanel.Select
		End Sub
		
		Private Dim ScrollingDown As Boolean
		Private Dim ScrollDistance As Integer
		Private Sub ProgEditPanelDragOver(sender As System.Object, e As System.Windows.Forms.DragEventArgs)
			
			'Get current scroll position and mouse location
			Dim MouseLoc As Point = ProgEditPanel.PointToClient(New Point(e.X, e.Y))
			Dim PanelBottom As Integer = ProgEditPanel.Size.Height - 40
			
			If MouseLoc.Y < 40 Then
				ScrollingDown = False
				ScrollDistance = 41 - MouseLoc.Y
				ScrollTimer.Start
			ElseIf MouseLoc.Y > PanelBottom Then
				ScrollingDown = True
				ScrollDistance = MouseLoc.Y - PanelBottom
				ScrollTimer.Start
			Else
				ScrollTimer.Stop
			End If
			
		End Sub
		
		Private Sub ScrollTimerTick(sender As System.Object, e As System.EventArgs)
			'When timer ticks, scroll panel up or down
			
			Dim CurrScrollPos As Point = ProgEditPanel.AutoScrollPosition
			Dim NewScrollPos As Point
			
			If ScrollingDown Then
				NewScrollPos = New Point(CurrScrollPos.X, -1 * (CurrScrollPos.Y - ScrollDistance))
			Else
				NewScrollPos = New Point(CurrScrollPos.X, -1 * (CurrScrollPos.Y + ScrollDistance))
			End If
			ProgEditPanel.AutoScrollPosition = NewScrollPos
			
		End Sub
		
		Private Sub ProgEditPanelDragLeave(sender As System.Object, e As System.EventArgs)
			'Make sure to stop scrolling panel when mouse leaves
			ScrollTimer.Stop
			
		End Sub
		
		Private Sub GuiEditorPanelLoad(sender As System.Object, e As System.EventArgs)
			
		 	
		End Sub
		
		Private Sub FormSplitterSplitterMoved(sender As System.Object, e As System.Windows.Forms.SplitterEventArgs)
			MainForm.GetPreferences.SetPref("GCGB", "SettingsPanelWidth", formSplitter.SplitPosition)
			MainForm.GetPreferences.SavePreferences
			
		End Sub
		
		Private Sub RightPaneSplitterSplitterMoved(sender As System.Object, e As System.Windows.Forms.SplitterEventArgs)
			MainForm.GetPreferences.SetPref("GCGB", "SubListHeight", Me.RightPaneSplitter.SplitPosition)
			MainForm.GetPreferences.SavePreferences
			
		End Sub
		
		Private Sub IconRightMenuPopup(sender As System.Object, e As System.EventArgs)
			
			'Get icon that has been clicked
			Dim ControlName As String
			Dim IconClicked As Integer
			ControlName = sender.SourceControl.Name.ToString
			'Icons will have their name in the format location:iconname
			IconClicked = 0
			Dim TempData As String = ""
			If ControlName.IndexOf(":") <> -1 Then
				IconClicked = ControlName.Substring(0, ControlName.IndexOf(":"))
				
				If IconClicked < 0 Then Exit Sub
				
				'Get code for current icon
				If IconClicked > 0 And IconClicked <= Program.Subroutines(CurrentSub).Lines.Count Then
					TempData = Program.Subroutines(CurrentSub).Lines(IconClicked - 1)
				End If
			End If
			
			'First icon?
			If IconClicked = 0 Then
				contextCut.Enabled = False
				contextCopy.Enabled = False
				contextPaste.Enabled = True
				contextDelete.Enabled = False
				contextHelp.Enabled = False
			Else
				contextCut.Enabled = True
				contextCopy.Enabled = True
				contextPaste.Enabled = True
				contextDelete.Enabled = True
				contextHelp.Enabled = True
			End If
			
			'Sub?
			If Not Program.GetCalledSub(TempData) Is Nothing Then
				
				Dim NewSubIndex As Integer = Program.EditSubIndex(TempData)
				If NewSubIndex > 0 Then
					contextViewSub.Text = "View Contents"
					contextViewSub.Enabled = True
				Else
					'contextViewSub.Text = "View sub contents"
					contextViewSub.Enabled = False
				End If
				
			Else
				contextViewSub.Enabled = False
			End If
			
			'Remember clicked icon
			SelectIcon(IconClicked, True)
			
		End Sub
		
		Private Sub ContextViewSubClick(sender As System.Object, e As System.EventArgs)
			Dim NewSubIndex As Integer
			NewSubIndex = Program.EditSubIndex(GetSelectedCode)
			
			'Sub is in program, go to
			If NewSubIndex > 0 Then
				ChangeToSub(NewSubIndex)
			
			'Sub is in a library, show in text
			Else
				
			End If
			
		End Sub
		
		Private Sub ContextCutClick(sender As System.Object, e As System.EventArgs)
			ClipboardOp("x")
			
		End Sub
		
		Private Sub ContextCopyClick(sender As System.Object, e As System.EventArgs)
			ClipboardOp("c")
			
		End Sub
		
		Private Sub ContextPasteClick(sender As System.Object, e As System.EventArgs)
			ClipboardOp("v")
			
		End Sub
		
		Private Sub ContextDeleteClick(sender As System.Object, e As System.EventArgs)
			DeleteSelection
			
		End Sub
		
		Private Sub ContextHelpClick(sender As System.Object, e As System.EventArgs)
			Mainform.MainformInstance.ShowCommandHelp(GetCurrentCommand)
			
		End Sub
		
	End Class
''End Namespace
