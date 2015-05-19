' Great Cow Graphical BASIC - an icon based programming language for microcontrollers
' Copyright (C) 2006 - 2014 Hugh Considine
'
'	This program is free software; you can redistribute it and/or modify
'	it under the terms of the GNU General Public License as published by
'	the Free Software Foundation; either version 2 of the License, or
'	(at your option) any later version.
'
'	This program is distributed in the hope that it will be useful,
'	but WITHOUT ANY WARRANTY; without even the implied warranty of
'	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'	GNU General Public License for more details.
'
'	You should have received a copy of the GNU General Public License
'	along with this program; if not, write to the Free Software
'	Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
'
' If you have any questions about the source code, please email me: hconsidine@internode.on.net
' Any other questions, please email me or see the GCBASIC forums.

Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Collections.Generic
Imports System.Threading

''Namespace Great_Cow_Graphical_BASIC

    Public Class MainForm
		Inherits System.Windows.Forms.Form
		Private components As System.ComponentModel.IContainer
		Private MainFormToolTip As System.Windows.Forms.ToolTip
		Private menuItem2 As System.Windows.Forms.MenuItem
		Private AddCommandGroup As System.Windows.Forms.GroupBox
		Public StatusBar As System.Windows.Forms.StatusBar
		Public SaveFileSelect As System.Windows.Forms.SaveFileDialog
		Private AddCommandButton As System.Windows.Forms.Button
		Private menuItem4 As System.Windows.Forms.MenuItem
		Private LabelAvailableIcons As System.Windows.Forms.Label
		Private EnterCommand As System.Windows.Forms.TextBox
		Private IconCategories As System.Windows.Forms.ComboBox
		Private IconPanel As System.Windows.Forms.GroupBox
		Private LabelIconCat As System.Windows.Forms.Label
		Private OpenFileSelect As System.Windows.Forms.OpenFileDialog
		Private IconListPanel As System.Windows.Forms.Panel
		Private newIconPanel As System.Windows.Forms.Panel
		
		'Program Size Constants
		Public Const MaxSubSize As Integer = 2000
		Public Const MaxSubCount As Integer = 200
		Public Const ConfigSize As Integer = 10
		Public Const MaxIconsPerCategory As Integer = 1000
		Public Const MaxIconParams As Integer = 20
		Public Const MaxUndo As Integer = 1000
		Public Const MaxTables As Integer = 50
		Public Const MaxTableElements As Integer = 10000
		
		'Version constants
		Public Const ProgVersion As String = "1.0 2015-05-19"
		Public Const FileVersion As String = "20100130"
		Public Const ShortVersion As String = "Version 1.0"
		
		'Filename constants
		'Windows:
		'Public Const FilenameCommands = "\commands.dat"
		'Public Const FilenameIcons = "\icons\icons.dat"
		'Public Const FilenamePrefs = "\prgdata\prefs.dat"
		'Public Const FilenameIconsDir = "\icons\"
		'Public Const FilenameJoiner = "\icons\joiner.png"
		'Public Const FilenameIncludeDir = "\include\"
		'Public Const FilenameChipDir = "\chipdata\"
		'Public Const FilenameConverters = "\converters\"
		'Public Const FilenameExamplesDir = "\demos\"
		'Linux:
		Public Const FilenameCommands As String = "/commands.dat"
		Public Const FilenameIcons As String = "/icons/icons.dat"
		Public Const FilenamePrefs As String = "/prefs.dat"
		Public Const FilenameUserPrefs As String = "%appdata%/gcgb.ini"
		Public Const FilenameIconsDir As String = "/icons/"
		Public Const FilenameJoiner As String = "/icons/joiner.png"
		Public Const FilenameIncludeDir As String = "/include/"
		Public Const FilenameChipDir As String = "/chipdata/"
		Public Const FilenameConverters As String = "/converters/"
		Public Const FilenameLicense As String = "/docs/license.txt"
		Public Const FilenameExamplesDir As String = "/demos/"
		Public Const FilePathSplitter As String = "/"
		
		Public Const URL_GCB As String = "http://gcbasic.sourceforge.net/"
		Public Const URL_Forum As String = "https://sourceforge.net/projects/gcbasic/forums"
		
		'Command storage arrays
		Public Dim Shared IconCategory As List(Of String)
		Public Dim Shared CommandList As List(Of GCBCommand)
        Public Dim Shared IconList As List(Of GCBIcon)
		
		'Constant groups
		Public Dim Shared ConstantGroups As New List(Of ConstGroup)
		
		'Functions
		Public Dim Shared GCBFunctions As New List(Of GCBFunction)
		
		'Hardware settings
		'Stores ArrayList of HardwareDevice items
		Public Dim Shared HardwareConf As New List(Of HardwareDevice)
		
		'Icon selection panel
		Private IconDisplay(MaxIconsPerCategory) As IconDisplay
		
		'Paths
		Public Dim Shared InstallDir As String
		
		Public Dim Shared CommandLine As String
		Public Dim Shared CancelClose As Integer
		
		'MDI Windows
		Public Dim Shared UntitledCount As Integer = 0
		Public Dim Shared MainformInstance As MainForm
		
		'Events
		Public Shared Event RefreshCommandList
		
		'Preferences
		Private Dim Shared Prefs As Preferences
		
		'File Importers
		Public Dim Shared FileConverters As New List(Of FileConverter)
		
		'Translator
		Public Dim Shared Translator As LanguageTranslator
		
		'Icon joining arrow
		Public Dim Shared JoinerArrow As Image
		
		Public Shared Sub Main
			
			'Command Line
			Try
				CommandLine = System.Environment.GetCommandLineArgs(1)
			Catch
			End Try
			
			'Get install directory
			Dim RegDir, RegVersion As String
			Try
 				Dim ReadRegKey As Microsoft.Win32.RegistryKey
				ReadRegKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\GCBASIC")
				RegDir = ReadRegKey.GetValue("InstallDir", "").ToString
				RegVersion = ReadRegKey.GetValue("FileVersion", "").ToString
				ReadRegKey.Close
			Catch
				RegDir = ""
				RegVersion = ""
			End Try
		 	
		 	If RegDir <> "" And RegVersion.Trim = FileVersion.Trim Then
		 		InstallDir = RegDir
		 	Else
		 		'InstallDir = System.Environment.GetCommandLineArgs(0)
		 		InstallDir = Application.ExecutablePath
		 		If InstallDir.IndexOf("\") <> -1 Then InstallDir = InstallDir.Substring(0, InstallDir.LastIndexOf("\"))
				If InstallDir.IndexOf("/") <> -1 Then InstallDir = InstallDir.Substring(0, InstallDir.LastIndexOf("/"))
				If InstallDir.ToLower.IndexOf("\bin") <> -1 Then InstallDir = InstallDir.Substring(0, InstallDir.ToLower.IndexOf("\bin"))
				If InstallDir.ToLower.IndexOf("/bin") <> -1 Then InstallDir = InstallDir.Substring(0, InstallDir.ToLower.IndexOf("/bin"))
		 	End If
		 	
			'Initialize
			
			'Load preferences
			Prefs = New Preferences
			Prefs.LoadPreferences
			
			'Load translation
			Translator = New LanguageTranslator(Prefs)
			
			'Load icons and commands
	 		FileIO.LoadCommands
	 		FileIO.LoadIcons
	 		
	 		'Load file converters
	 		FileIO.LoadConverters
		 	
		 	'Load icon joining image
		 	JoinerArrow = Image.FromFile(InstallDir + FilenameJoiner)
		 	
			'Show Main Form
			Dim fMainForm As New MainForm
			fMainForm.ShowDialog()
			
		End Sub
		
		Public Sub New()
			MyBase.New
			'
			' The Me.InitializeComponent call is required for Windows Forms designer support.
			'
			Me.InitializeComponent
			
			MainformInstance = Me
			
			'Default exception handlers
			Dim Domain As AppDomain = AppDomain.CurrentDomain
			AddHandler Domain.UnhandledException, AddressOf UnhandledExceptionHandler
			AddHandler Application.ThreadException, AddressOf ThreadExceptionHandler
			
		End Sub
		
		#Region " Windows Forms Designer generated code "
		' This method is required for Windows Forms designer support.
		' Do not change the method contents inside the source code editor. The Forms designer might
		' not be able to load this method if it was changed manually.
		Private Sub InitializeComponent()
			Me.components = New System.ComponentModel.Container
			Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
			Me.newIconPanel = New System.Windows.Forms.Panel
			Me.AddCommandGroup = New System.Windows.Forms.GroupBox
			Me.AddCommandButton = New System.Windows.Forms.Button
			Me.EnterCommand = New System.Windows.Forms.TextBox
			Me.IconPanel = New System.Windows.Forms.GroupBox
			Me.LabelAvailableIcons = New System.Windows.Forms.Label
			Me.LabelIconCat = New System.Windows.Forms.Label
			Me.IconCategories = New System.Windows.Forms.ComboBox
			Me.IconListPanel = New System.Windows.Forms.Panel
			Me.OpenFileSelect = New System.Windows.Forms.OpenFileDialog
			Me.menuItem4 = New System.Windows.Forms.MenuItem
			Me.SaveFileSelect = New System.Windows.Forms.SaveFileDialog
			Me.StatusBar = New System.Windows.Forms.StatusBar
			Me.menuItem2 = New System.Windows.Forms.MenuItem
			Me.MainFormToolTip = New System.Windows.Forms.ToolTip(Me.components)
			Me.MainformMenuStrip = New System.Windows.Forms.MenuStrip
			Me.FileMenu = New System.Windows.Forms.ToolStripMenuItem
			Me.FileNew = New System.Windows.Forms.ToolStripMenuItem
			Me.FileOpen = New System.Windows.Forms.ToolStripMenuItem
			Me.FileOpenExample = New System.Windows.Forms.ToolStripMenuItem
			Me.toolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator
			Me.FileSave = New System.Windows.Forms.ToolStripMenuItem
			Me.FileSaveAs = New System.Windows.Forms.ToolStripMenuItem
			Me.toolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator
			Me.FileClose = New System.Windows.Forms.ToolStripMenuItem
			Me.FileExit = New System.Windows.Forms.ToolStripMenuItem
			Me.EditMenu = New System.Windows.Forms.ToolStripMenuItem
			Me.EditCut = New System.Windows.Forms.ToolStripMenuItem
			Me.EditCopy = New System.Windows.Forms.ToolStripMenuItem
			Me.EditPaste = New System.Windows.Forms.ToolStripMenuItem
			Me.toolStripMenuItem3 = New System.Windows.Forms.ToolStripSeparator
			Me.EditUndo = New System.Windows.Forms.ToolStripMenuItem
			Me.EditRedo = New System.Windows.Forms.ToolStripMenuItem
			Me.toolStripMenuItem4 = New System.Windows.Forms.ToolStripSeparator
			Me.EditPrefs = New System.Windows.Forms.ToolStripMenuItem
			Me.ViewMenu = New System.Windows.Forms.ToolStripMenuItem
			Me.ViewIcons = New System.Windows.Forms.ToolStripMenuItem
			Me.ViewText = New System.Windows.Forms.ToolStripMenuItem
			Me.toolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
			Me.ViewZoomIn = New System.Windows.Forms.ToolStripMenuItem
			Me.ViewZoomOut = New System.Windows.Forms.ToolStripMenuItem
			Me.ViewZoomReset = New System.Windows.Forms.ToolStripMenuItem
			Me.toolStripMenuItem5 = New System.Windows.Forms.ToolStripSeparator
			Me.ProgramMenu = New System.Windows.Forms.ToolStripMenuItem
			Me.ProgramLibs = New System.Windows.Forms.ToolStripMenuItem
			Me.ProgramVars = New System.Windows.Forms.ToolStripMenuItem
			Me.ProgramConsts = New System.Windows.Forms.ToolStripMenuItem
			Me.ProgramTables = New System.Windows.Forms.ToolStripMenuItem
			Me.ProgramInterrupt = New System.Windows.Forms.ToolStripMenuItem
			Me.toolStripMenuItem6 = New System.Windows.Forms.ToolStripSeparator
			Me.ProgramChip = New System.Windows.Forms.ToolStripMenuItem
			Me.ProgramAdvanced = New System.Windows.Forms.ToolStripMenuItem
			Me.ToolsMenu = New System.Windows.Forms.ToolStripMenuItem
			Me.ToolsASM = New System.Windows.Forms.ToolStripMenuItem
			Me.ToolsSummary = New System.Windows.Forms.ToolStripMenuItem
			Me.toolStripMenuItem7 = New System.Windows.Forms.ToolStripSeparator
			Me.ToolsCompile = New System.Windows.Forms.ToolStripMenuItem
			Me.ToolsDownload = New System.Windows.Forms.ToolStripMenuItem
			Me.ToolsDownloadWith = New System.Windows.Forms.ToolStripMenuItem
			Me.toolStripMenuItem8 = New System.Windows.Forms.ToolStripSeparator
			Me.ToolsExternal = New System.Windows.Forms.ToolStripMenuItem
			Me.HelpMenu = New System.Windows.Forms.ToolStripMenuItem
			Me.HelpContents = New System.Windows.Forms.ToolStripMenuItem
			Me.HelpIndex = New System.Windows.Forms.ToolStripMenuItem
			Me.toolStripMenuItem9 = New System.Windows.Forms.ToolStripSeparator
			Me.HelpIcon = New System.Windows.Forms.ToolStripMenuItem
			Me.toolStripMenuItem10 = New System.Windows.Forms.ToolStripSeparator
			Me.HelpAbout = New System.Windows.Forms.ToolStripMenuItem
			Me.newIconPanel.SuspendLayout
			Me.AddCommandGroup.SuspendLayout
			Me.IconPanel.SuspendLayout
			Me.MainformMenuStrip.SuspendLayout
			Me.SuspendLayout
			'
			'newIconPanel
			'
			Me.newIconPanel.Controls.Add(Me.AddCommandGroup)
			Me.newIconPanel.Controls.Add(Me.IconPanel)
			Me.newIconPanel.Dock = System.Windows.Forms.DockStyle.Left
			Me.newIconPanel.Location = New System.Drawing.Point(0, 24)
			Me.newIconPanel.Name = "newIconPanel"
			Me.newIconPanel.Padding = New System.Windows.Forms.Padding(8)
			Me.newIconPanel.Size = New System.Drawing.Size(200, 582)
			Me.newIconPanel.TabIndex = 14
			'
			'AddCommandGroup
			'
			Me.AddCommandGroup.Controls.Add(Me.AddCommandButton)
			Me.AddCommandGroup.Controls.Add(Me.EnterCommand)
			Me.AddCommandGroup.Dock = System.Windows.Forms.DockStyle.Bottom
			Me.AddCommandGroup.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.AddCommandGroup.Location = New System.Drawing.Point(8, 526)
			Me.AddCommandGroup.Name = "AddCommandGroup"
			Me.AddCommandGroup.Size = New System.Drawing.Size(184, 48)
			Me.AddCommandGroup.TabIndex = 12
			Me.AddCommandGroup.TabStop = false
			Me.AddCommandGroup.Text = "Add Command Manually"
			'
			'AddCommandButton
			'
			Me.AddCommandButton.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
									Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.AddCommandButton.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.AddCommandButton.Location = New System.Drawing.Point(144, 13)
			Me.AddCommandButton.Name = "AddCommandButton"
			Me.AddCommandButton.Size = New System.Drawing.Size(32, 27)
			Me.AddCommandButton.TabIndex = 12
			Me.AddCommandButton.Text = "Add"
			Me.MainFormToolTip.SetToolTip(Me.AddCommandButton, "Click to add the typed command to the program, under the current icon")
			AddHandler Me.AddCommandButton.Click, AddressOf Me.AddCommandButtonClick
			'
			'EnterCommand
			'
			Me.EnterCommand.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
									Or System.Windows.Forms.AnchorStyles.Left)  _
									Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.EnterCommand.Location = New System.Drawing.Point(3, 17)
			Me.EnterCommand.Name = "EnterCommand"
			Me.EnterCommand.Size = New System.Drawing.Size(141, 20)
			Me.EnterCommand.TabIndex = 11
			Me.EnterCommand.Text = "Command"
			Me.MainFormToolTip.SetToolTip(Me.EnterCommand, "Type the GCBASIC code for a command here, then click Add to add it")
			AddHandler Me.EnterCommand.KeyPress, AddressOf Me.EnterCommandKeyPress
			'
			'IconPanel
			'
			Me.IconPanel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
									Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
			Me.IconPanel.Controls.Add(Me.LabelAvailableIcons)
			Me.IconPanel.Controls.Add(Me.LabelIconCat)
			Me.IconPanel.Controls.Add(Me.IconCategories)
			Me.IconPanel.Controls.Add(Me.IconListPanel)
			Me.IconPanel.FlatStyle = System.Windows.Forms.FlatStyle.System
			Me.IconPanel.Location = New System.Drawing.Point(8, 8)
			Me.IconPanel.Name = "IconPanel"
			Me.IconPanel.Size = New System.Drawing.Size(184, 511)
			Me.IconPanel.TabIndex = 9
			Me.IconPanel.TabStop = false
			Me.IconPanel.Text = "Icons"
			'
			'LabelAvailableIcons
			'
			Me.LabelAvailableIcons.Location = New System.Drawing.Point(8, 72)
			Me.LabelAvailableIcons.Name = "LabelAvailableIcons"
			Me.LabelAvailableIcons.Size = New System.Drawing.Size(168, 16)
			Me.LabelAvailableIcons.TabIndex = 3
			Me.LabelAvailableIcons.Text = "Available Icons in Category:"
			'
			'LabelIconCat
			'
			Me.LabelIconCat.Location = New System.Drawing.Point(8, 24)
			Me.LabelIconCat.Name = "LabelIconCat"
			Me.LabelIconCat.Size = New System.Drawing.Size(168, 16)
			Me.LabelIconCat.TabIndex = 2
			Me.LabelIconCat.Text = "Category:"
			'
			'IconCategories
			'
			Me.IconCategories.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me.IconCategories.Location = New System.Drawing.Point(8, 40)
			Me.IconCategories.MaxDropDownItems = 16
			Me.IconCategories.Name = "IconCategories"
			Me.IconCategories.Size = New System.Drawing.Size(168, 21)
			Me.IconCategories.TabIndex = 1
			Me.MainFormToolTip.SetToolTip(Me.IconCategories, "Change the category to see more icons")
			AddHandler Me.IconCategories.SelectedValueChanged, AddressOf Me.IconCategoriesSelectedValueChanged
			'
			'IconListPanel
			'
			Me.IconListPanel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
									Or System.Windows.Forms.AnchorStyles.Left)  _
									Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.IconListPanel.AutoScroll = true
			Me.IconListPanel.BackColor = System.Drawing.SystemColors.Window
			Me.IconListPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
			Me.IconListPanel.Location = New System.Drawing.Point(8, 95)
			Me.IconListPanel.Name = "IconListPanel"
			Me.IconListPanel.Size = New System.Drawing.Size(168, 408)
			Me.IconListPanel.TabIndex = 0
			AddHandler Me.IconListPanel.MouseEnter, AddressOf Me.IconListPanelMouseEnter
			'
			'OpenFileSelect
			'
			Me.OpenFileSelect.DefaultExt = "gcb"
			Me.OpenFileSelect.Filter = "GCBASIC Programs (*.gcb)|*.gcb|GCBASIC Libraries (*.h)|*.h|Text Files (*.txt)|*.t"& _ 
			"xt|All Files (*.*)|*.*"
			Me.OpenFileSelect.Title = "Open"
			AddHandler Me.OpenFileSelect.FileOk, AddressOf Me.OpenFileSelectFileOk
			'
			'menuItem4
			'
			Me.menuItem4.Index = -1
			Me.menuItem4.Text = "Compile and Download"
			'
			'SaveFileSelect
			'
			Me.SaveFileSelect.DefaultExt = "gcb"
			Me.SaveFileSelect.Filter = "GCBASIC Program (*.gcb)|*.gcb|GCBASIC Library (*.h)|*.h|All Files (*.*)|*.*"
			AddHandler Me.SaveFileSelect.FileOk, AddressOf Me.SaveFileSelectFileOk
			'
			'StatusBar
			'
			Me.StatusBar.Location = New System.Drawing.Point(0, 606)
			Me.StatusBar.Name = "StatusBar"
			Me.StatusBar.Size = New System.Drawing.Size(784, 16)
			Me.StatusBar.TabIndex = 5
			Me.StatusBar.Text = "Move the mouse over something to see what it does. For more help, press F1."
			'
			'menuItem2
			'
			Me.menuItem2.Index = -1
			Me.menuItem2.Text = "Compile and Download"
			'
			'MainformMenuStrip
			'
			Me.MainformMenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileMenu, Me.EditMenu, Me.ViewMenu, Me.ProgramMenu, Me.ToolsMenu, Me.HelpMenu})
			Me.MainformMenuStrip.Location = New System.Drawing.Point(0, 0)
			Me.MainformMenuStrip.MdiWindowListItem = Me.ViewMenu
			Me.MainformMenuStrip.Name = "MainformMenuStrip"
			Me.MainformMenuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
			Me.MainformMenuStrip.Size = New System.Drawing.Size(784, 24)
			Me.MainformMenuStrip.TabIndex = 16
			Me.MainformMenuStrip.Text = "MainformMenuStrip"
			'
			'FileMenu
			'
			Me.FileMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileNew, Me.FileOpen, Me.FileOpenExample, Me.toolStripMenuItem1, Me.FileSave, Me.FileSaveAs, Me.toolStripMenuItem2, Me.FileClose, Me.FileExit})
			Me.FileMenu.Name = "FileMenu"
			Me.FileMenu.Size = New System.Drawing.Size(37, 20)
			Me.FileMenu.Text = "File"
			'
			'FileNew
			'
			Me.FileNew.Name = "FileNew"
			Me.FileNew.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.N),System.Windows.Forms.Keys)
			Me.FileNew.Size = New System.Drawing.Size(158, 22)
			Me.FileNew.Text = "New"
			AddHandler Me.FileNew.Click, AddressOf Me.FileNew_Click
			'
			'FileOpen
			'
			Me.FileOpen.Name = "FileOpen"
			Me.FileOpen.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.O),System.Windows.Forms.Keys)
			Me.FileOpen.Size = New System.Drawing.Size(158, 22)
			Me.FileOpen.Text = "Open ..."
			AddHandler Me.FileOpen.Click, AddressOf Me.FileOpen_Click
			'
			'FileOpenExample
			'
			Me.FileOpenExample.Name = "FileOpenExample"
			Me.FileOpenExample.Size = New System.Drawing.Size(158, 22)
			Me.FileOpenExample.Text = "Open Example"
			'
			'toolStripMenuItem1
			'
			Me.toolStripMenuItem1.Name = "toolStripMenuItem1"
			Me.toolStripMenuItem1.Size = New System.Drawing.Size(155, 6)
			'
			'FileSave
			'
			Me.FileSave.Name = "FileSave"
			Me.FileSave.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.S),System.Windows.Forms.Keys)
			Me.FileSave.Size = New System.Drawing.Size(158, 22)
			Me.FileSave.Text = "Save"
			AddHandler Me.FileSave.Click, AddressOf Me.FileSave_Click
			'
			'FileSaveAs
			'
			Me.FileSaveAs.Name = "FileSaveAs"
			Me.FileSaveAs.Size = New System.Drawing.Size(158, 22)
			Me.FileSaveAs.Text = "Save As ..."
			AddHandler Me.FileSaveAs.Click, AddressOf Me.FileSaveAs_Click
			'
			'toolStripMenuItem2
			'
			Me.toolStripMenuItem2.Name = "toolStripMenuItem2"
			Me.toolStripMenuItem2.Size = New System.Drawing.Size(155, 6)
			'
			'FileClose
			'
			Me.FileClose.Name = "FileClose"
			Me.FileClose.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.W),System.Windows.Forms.Keys)
			Me.FileClose.Size = New System.Drawing.Size(158, 22)
			Me.FileClose.Text = "Close"
			AddHandler Me.FileClose.Click, AddressOf Me.FileCloseClick
			'
			'FileExit
			'
			Me.FileExit.Name = "FileExit"
			Me.FileExit.ShortcutKeys = CType((System.Windows.Forms.Keys.Alt Or System.Windows.Forms.Keys.F4),System.Windows.Forms.Keys)
			Me.FileExit.Size = New System.Drawing.Size(158, 22)
			Me.FileExit.Text = "Exit"
			AddHandler Me.FileExit.Click, AddressOf Me.FileExit_Click
			'
			'EditMenu
			'
			Me.EditMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.EditCut, Me.EditCopy, Me.EditPaste, Me.toolStripMenuItem3, Me.EditUndo, Me.EditRedo, Me.toolStripMenuItem4, Me.EditPrefs})
			Me.EditMenu.Name = "EditMenu"
			Me.EditMenu.Size = New System.Drawing.Size(39, 20)
			Me.EditMenu.Text = "Edit"
			'
			'EditCut
			'
			Me.EditCut.Name = "EditCut"
			Me.EditCut.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.X),System.Windows.Forms.Keys)
			Me.EditCut.Size = New System.Drawing.Size(147, 22)
			Me.EditCut.Text = "Cut"
			AddHandler Me.EditCut.Click, AddressOf Me.EditCut_Click
			'
			'EditCopy
			'
			Me.EditCopy.Name = "EditCopy"
			Me.EditCopy.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.C),System.Windows.Forms.Keys)
			Me.EditCopy.Size = New System.Drawing.Size(147, 22)
			Me.EditCopy.Text = "Copy"
			AddHandler Me.EditCopy.Click, AddressOf Me.EditCopy_Click
			'
			'EditPaste
			'
			Me.EditPaste.Name = "EditPaste"
			Me.EditPaste.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.V),System.Windows.Forms.Keys)
			Me.EditPaste.Size = New System.Drawing.Size(147, 22)
			Me.EditPaste.Text = "Paste"
			AddHandler Me.EditPaste.Click, AddressOf Me.EditPaste_Click
			'
			'toolStripMenuItem3
			'
			Me.toolStripMenuItem3.Name = "toolStripMenuItem3"
			Me.toolStripMenuItem3.Size = New System.Drawing.Size(144, 6)
			'
			'EditUndo
			'
			Me.EditUndo.Name = "EditUndo"
			Me.EditUndo.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Z),System.Windows.Forms.Keys)
			Me.EditUndo.Size = New System.Drawing.Size(147, 22)
			Me.EditUndo.Text = "Undo"
			AddHandler Me.EditUndo.Click, AddressOf Me.EditUndo_Click
			'
			'EditRedo
			'
			Me.EditRedo.Name = "EditRedo"
			Me.EditRedo.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Y),System.Windows.Forms.Keys)
			Me.EditRedo.Size = New System.Drawing.Size(147, 22)
			Me.EditRedo.Text = "Redo"
			AddHandler Me.EditRedo.Click, AddressOf Me.EditRedo_Click
			'
			'toolStripMenuItem4
			'
			Me.toolStripMenuItem4.Name = "toolStripMenuItem4"
			Me.toolStripMenuItem4.Size = New System.Drawing.Size(144, 6)
			'
			'EditPrefs
			'
			Me.EditPrefs.Name = "EditPrefs"
			Me.EditPrefs.Size = New System.Drawing.Size(147, 22)
			Me.EditPrefs.Text = "Preferences ..."
			AddHandler Me.EditPrefs.Click, AddressOf Me.EditPrefs_Click
			'
			'ViewMenu
			'
			Me.ViewMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ViewIcons, Me.ViewText, Me.toolStripSeparator1, Me.ViewZoomIn, Me.ViewZoomOut, Me.ViewZoomReset, Me.toolStripMenuItem5})
			Me.ViewMenu.Name = "ViewMenu"
			Me.ViewMenu.Size = New System.Drawing.Size(44, 20)
			Me.ViewMenu.Text = "View"
			'
			'ViewIcons
			'
			Me.ViewIcons.Name = "ViewIcons"
			Me.ViewIcons.ShortcutKeys = CType((System.Windows.Forms.Keys.Alt Or System.Windows.Forms.Keys.F1),System.Windows.Forms.Keys)
			Me.ViewIcons.Size = New System.Drawing.Size(186, 22)
			Me.ViewIcons.Text = "View as Icons"
			AddHandler Me.ViewIcons.Click, AddressOf Me.ViewIconsClick
			'
			'ViewText
			'
			Me.ViewText.Name = "ViewText"
			Me.ViewText.ShortcutKeys = CType((System.Windows.Forms.Keys.Alt Or System.Windows.Forms.Keys.F2),System.Windows.Forms.Keys)
			Me.ViewText.Size = New System.Drawing.Size(186, 22)
			Me.ViewText.Text = "View as Text"
			AddHandler Me.ViewText.Click, AddressOf Me.ViewTextClick
			'
			'toolStripSeparator1
			'
			Me.toolStripSeparator1.Name = "toolStripSeparator1"
			Me.toolStripSeparator1.Size = New System.Drawing.Size(183, 6)
			'
			'ViewZoomIn
			'
			Me.ViewZoomIn.Name = "ViewZoomIn"
			Me.ViewZoomIn.ShortcutKeyDisplayString = "Ctrl++"
			Me.ViewZoomIn.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Oemplus),System.Windows.Forms.Keys)
			Me.ViewZoomIn.Size = New System.Drawing.Size(186, 22)
			Me.ViewZoomIn.Text = "Zoom In"
			AddHandler Me.ViewZoomIn.Click, AddressOf Me.ViewZoomInClick
			'
			'ViewZoomOut
			'
			Me.ViewZoomOut.Name = "ViewZoomOut"
			Me.ViewZoomOut.ShortcutKeyDisplayString = "Ctrl+-"
			Me.ViewZoomOut.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.OemMinus),System.Windows.Forms.Keys)
			Me.ViewZoomOut.Size = New System.Drawing.Size(186, 22)
			Me.ViewZoomOut.Text = "Zoom Out"
			AddHandler Me.ViewZoomOut.Click, AddressOf Me.ViewZoomOutClick
			'
			'ViewZoomReset
			'
			Me.ViewZoomReset.Name = "ViewZoomReset"
			Me.ViewZoomReset.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.D0),System.Windows.Forms.Keys)
			Me.ViewZoomReset.Size = New System.Drawing.Size(186, 22)
			Me.ViewZoomReset.Text = "Reset Zoom"
			AddHandler Me.ViewZoomReset.Click, AddressOf Me.ViewZoomResetClick
			'
			'toolStripMenuItem5
			'
			Me.toolStripMenuItem5.Name = "toolStripMenuItem5"
			Me.toolStripMenuItem5.Size = New System.Drawing.Size(183, 6)
			'
			'ProgramMenu
			'
			Me.ProgramMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ProgramLibs, Me.ProgramVars, Me.ProgramConsts, Me.ProgramTables, Me.ProgramInterrupt, Me.toolStripMenuItem6, Me.ProgramChip, Me.ProgramAdvanced})
			Me.ProgramMenu.Name = "ProgramMenu"
			Me.ProgramMenu.Size = New System.Drawing.Size(65, 20)
			Me.ProgramMenu.Text = "Program"
			'
			'ProgramLibs
			'
			Me.ProgramLibs.Name = "ProgramLibs"
			Me.ProgramLibs.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.E),System.Windows.Forms.Keys)
			Me.ProgramLibs.Size = New System.Drawing.Size(224, 22)
			Me.ProgramLibs.Text = "Libraries ..."
			AddHandler Me.ProgramLibs.Click, AddressOf Me.ProgramLibsClick
			'
			'ProgramVars
			'
			Me.ProgramVars.Name = "ProgramVars"
			Me.ProgramVars.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.R),System.Windows.Forms.Keys)
			Me.ProgramVars.Size = New System.Drawing.Size(224, 22)
			Me.ProgramVars.Text = "Variables ..."
			AddHandler Me.ProgramVars.Click, AddressOf Me.ProgramVars_Click
			'
			'ProgramConsts
			'
			Me.ProgramConsts.Name = "ProgramConsts"
			Me.ProgramConsts.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Q),System.Windows.Forms.Keys)
			Me.ProgramConsts.Size = New System.Drawing.Size(224, 22)
			Me.ProgramConsts.Text = "Constants ..."
			AddHandler Me.ProgramConsts.Click, AddressOf Me.ProgramConstsClick
			'
			'ProgramTables
			'
			Me.ProgramTables.Name = "ProgramTables"
			Me.ProgramTables.Size = New System.Drawing.Size(224, 22)
			Me.ProgramTables.Text = "Data Tables ..."
			AddHandler Me.ProgramTables.Click, AddressOf Me.ProgramTablesClick
			'
			'ProgramInterrupt
			'
			Me.ProgramInterrupt.Name = "ProgramInterrupt"
			Me.ProgramInterrupt.Size = New System.Drawing.Size(224, 22)
			Me.ProgramInterrupt.Text = "Interrupts ..."
			AddHandler Me.ProgramInterrupt.Click, AddressOf Me.ProgramInterruptsClick
			'
			'toolStripMenuItem6
			'
			Me.toolStripMenuItem6.Name = "toolStripMenuItem6"
			Me.toolStripMenuItem6.Size = New System.Drawing.Size(221, 6)
			'
			'ProgramChip
			'
			Me.ProgramChip.Name = "ProgramChip"
			Me.ProgramChip.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.D),System.Windows.Forms.Keys)
			Me.ProgramChip.Size = New System.Drawing.Size(224, 22)
			Me.ProgramChip.Text = "Hardware Settings ..."
			AddHandler Me.ProgramChip.Click, AddressOf Me.ProgramChipClick
			'
			'ProgramAdvanced
			'
			Me.ProgramAdvanced.Name = "ProgramAdvanced"
			Me.ProgramAdvanced.Size = New System.Drawing.Size(224, 22)
			Me.ProgramAdvanced.Text = "Advanced Settings ..."
			AddHandler Me.ProgramAdvanced.Click, AddressOf Me.ProgramAdvancedClick
			'
			'ToolsMenu
			'
			Me.ToolsMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolsASM, Me.ToolsSummary, Me.toolStripMenuItem7, Me.ToolsCompile, Me.ToolsDownload, Me.ToolsDownloadWith, Me.toolStripMenuItem8, Me.ToolsExternal})
			Me.ToolsMenu.Name = "ToolsMenu"
			Me.ToolsMenu.Size = New System.Drawing.Size(48, 20)
			Me.ToolsMenu.Text = "Tools"
			'
			'ToolsASM
			'
			Me.ToolsASM.Name = "ToolsASM"
			Me.ToolsASM.Size = New System.Drawing.Size(230, 22)
			Me.ToolsASM.Text = "View Assembly Code ..."
			AddHandler Me.ToolsASM.Click, AddressOf Me.ToolsASMClick
			'
			'ToolsSummary
			'
			Me.ToolsSummary.Name = "ToolsSummary"
			Me.ToolsSummary.Size = New System.Drawing.Size(230, 22)
			Me.ToolsSummary.Text = "View Compilation Report ..."
			AddHandler Me.ToolsSummary.Click, AddressOf Me.ToolsSummaryClick
			'
			'toolStripMenuItem7
			'
			Me.toolStripMenuItem7.Name = "toolStripMenuItem7"
			Me.toolStripMenuItem7.Size = New System.Drawing.Size(227, 6)
			'
			'ToolsCompile
			'
			Me.ToolsCompile.Name = "ToolsCompile"
			Me.ToolsCompile.Size = New System.Drawing.Size(230, 22)
			Me.ToolsCompile.Text = "Compile ..."
			AddHandler Me.ToolsCompile.Click, AddressOf Me.ToolsCompileClick
			'
			'ToolsDownload
			'
			Me.ToolsDownload.Name = "ToolsDownload"
			Me.ToolsDownload.ShortcutKeys = System.Windows.Forms.Keys.F9
			Me.ToolsDownload.Size = New System.Drawing.Size(230, 22)
			Me.ToolsDownload.Text = "Compile and Download ..."
			AddHandler Me.ToolsDownload.Click, AddressOf Me.ToolsDownloadClick
			'
			'ToolsDownloadWith
			'
			Me.ToolsDownloadWith.Name = "ToolsDownloadWith"
			Me.ToolsDownloadWith.Size = New System.Drawing.Size(230, 22)
			Me.ToolsDownloadWith.Text = "Compile and Download With"
			'
			'toolStripMenuItem8
			'
			Me.toolStripMenuItem8.Name = "toolStripMenuItem8"
			Me.toolStripMenuItem8.Size = New System.Drawing.Size(227, 6)
			'
			'ToolsExternal
			'
			Me.ToolsExternal.Name = "ToolsExternal"
			Me.ToolsExternal.Size = New System.Drawing.Size(230, 22)
			Me.ToolsExternal.Text = "External Tools"
			'
			'HelpMenu
			'
			Me.HelpMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.HelpContents, Me.HelpIndex, Me.toolStripMenuItem9, Me.HelpIcon, Me.toolStripMenuItem10, Me.HelpAbout})
			Me.HelpMenu.Name = "HelpMenu"
			Me.HelpMenu.Size = New System.Drawing.Size(44, 20)
			Me.HelpMenu.Text = "Help"
			'
			'HelpContents
			'
			Me.HelpContents.Name = "HelpContents"
			Me.HelpContents.ShortcutKeys = System.Windows.Forms.Keys.F1
			Me.HelpContents.Size = New System.Drawing.Size(195, 22)
			Me.HelpContents.Text = "Contents ..."
			AddHandler Me.HelpContents.Click, AddressOf Me.HelpContentsClick
			'
			'HelpIndex
			'
			Me.HelpIndex.Name = "HelpIndex"
			Me.HelpIndex.Size = New System.Drawing.Size(195, 22)
			Me.HelpIndex.Text = "Index ..."
			AddHandler Me.HelpIndex.Click, AddressOf Me.HelpIndexClick
			'
			'toolStripMenuItem9
			'
			Me.toolStripMenuItem9.Name = "toolStripMenuItem9"
			Me.toolStripMenuItem9.Size = New System.Drawing.Size(192, 6)
			'
			'HelpIcon
			'
			Me.HelpIcon.Name = "HelpIcon"
			Me.HelpIcon.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.H),System.Windows.Forms.Keys)
			Me.HelpIcon.Size = New System.Drawing.Size(195, 22)
			Me.HelpIcon.Text = "Current Icon ..."
			AddHandler Me.HelpIcon.Click, AddressOf Me.HelpIconClick
			'
			'toolStripMenuItem10
			'
			Me.toolStripMenuItem10.Name = "toolStripMenuItem10"
			Me.toolStripMenuItem10.Size = New System.Drawing.Size(192, 6)
			'
			'HelpAbout
			'
			Me.HelpAbout.Name = "HelpAbout"
			Me.HelpAbout.Size = New System.Drawing.Size(195, 22)
			Me.HelpAbout.Text = "About"
			AddHandler Me.HelpAbout.Click, AddressOf Me.HelpAbout_Click
			'
			'MainForm
			'
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.AutoSize = true
			Me.ClientSize = New System.Drawing.Size(784, 622)
			Me.Controls.Add(Me.newIconPanel)
			Me.Controls.Add(Me.StatusBar)
			Me.Controls.Add(Me.MainformMenuStrip)
			Me.Icon = CType(resources.GetObject("$this.Icon"),System.Drawing.Icon)
			Me.IsMdiContainer = true
			Me.MainMenuStrip = Me.MainformMenuStrip
			Me.MinimumSize = New System.Drawing.Size(600, 300)
			Me.Name = "MainForm"
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
			Me.Text = "Great Cow Graphical BASIC"
			AddHandler Load, AddressOf Me.MainFormLoad
			AddHandler MdiChildActivate, AddressOf Me.MainFormMdiChildActivate
			AddHandler Closing, AddressOf Me.MainFormClosing
			Me.newIconPanel.ResumeLayout(false)
			Me.AddCommandGroup.ResumeLayout(false)
			Me.AddCommandGroup.PerformLayout
			Me.IconPanel.ResumeLayout(false)
			Me.MainformMenuStrip.ResumeLayout(false)
			Me.MainformMenuStrip.PerformLayout
			Me.ResumeLayout(false)
			Me.PerformLayout
		End Sub
		Private FileOpenExample As System.Windows.Forms.ToolStripMenuItem
		Private ViewZoomReset As System.Windows.Forms.ToolStripMenuItem
		Private ViewZoomOut As System.Windows.Forms.ToolStripMenuItem
		Private ViewZoomIn As System.Windows.Forms.ToolStripMenuItem
		Private toolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
		Private MainformMenuStrip As System.Windows.Forms.MenuStrip
		Private FileMenu As System.Windows.Forms.ToolStripMenuItem
		Private HelpAbout As System.Windows.Forms.ToolStripMenuItem
		Private toolStripMenuItem10 As System.Windows.Forms.ToolStripSeparator
		Private HelpIcon As System.Windows.Forms.ToolStripMenuItem
		Private toolStripMenuItem9 As System.Windows.Forms.ToolStripSeparator
		Private HelpIndex As System.Windows.Forms.ToolStripMenuItem
		Private HelpContents As System.Windows.Forms.ToolStripMenuItem
		Private HelpMenu As System.Windows.Forms.ToolStripMenuItem
		Private ToolsExternal As System.Windows.Forms.ToolStripMenuItem
		Private toolStripMenuItem8 As System.Windows.Forms.ToolStripSeparator
		Private ToolsDownloadWith As System.Windows.Forms.ToolStripMenuItem
		Private ToolsDownload As System.Windows.Forms.ToolStripMenuItem
		Private ToolsCompile As System.Windows.Forms.ToolStripMenuItem
		Private toolStripMenuItem7 As System.Windows.Forms.ToolStripSeparator
		Private ToolsSummary As System.Windows.Forms.ToolStripMenuItem
		Private ToolsASM As System.Windows.Forms.ToolStripMenuItem
		Private ToolsMenu As System.Windows.Forms.ToolStripMenuItem
		Private ProgramAdvanced As System.Windows.Forms.ToolStripMenuItem
		Private ProgramChip As System.Windows.Forms.ToolStripMenuItem
		Private toolStripMenuItem6 As System.Windows.Forms.ToolStripSeparator
		Private ProgramInterrupt As System.Windows.Forms.ToolStripMenuItem
		Private ProgramTables As System.Windows.Forms.ToolStripMenuItem
		Private ProgramConsts As System.Windows.Forms.ToolStripMenuItem
		Private ProgramVars As System.Windows.Forms.ToolStripMenuItem
		Private ProgramLibs As System.Windows.Forms.ToolStripMenuItem
		Private ProgramMenu As System.Windows.Forms.ToolStripMenuItem
		Private toolStripMenuItem5 As System.Windows.Forms.ToolStripSeparator
		Private ViewText As System.Windows.Forms.ToolStripMenuItem
		Private ViewIcons As System.Windows.Forms.ToolStripMenuItem
		Private ViewMenu As System.Windows.Forms.ToolStripMenuItem
		Private EditPrefs As System.Windows.Forms.ToolStripMenuItem
		Private toolStripMenuItem4 As System.Windows.Forms.ToolStripSeparator
		Private EditRedo As System.Windows.Forms.ToolStripMenuItem
		Private EditUndo As System.Windows.Forms.ToolStripMenuItem
		Private toolStripMenuItem3 As System.Windows.Forms.ToolStripSeparator
		Private EditPaste As System.Windows.Forms.ToolStripMenuItem
		Private EditCopy As System.Windows.Forms.ToolStripMenuItem
		Private EditCut As System.Windows.Forms.ToolStripMenuItem
		Private EditMenu As System.Windows.Forms.ToolStripMenuItem
		Private FileExit As System.Windows.Forms.ToolStripMenuItem
		Private FileClose As System.Windows.Forms.ToolStripMenuItem
		Private toolStripMenuItem2 As System.Windows.Forms.ToolStripSeparator
		Private FileSaveAs As System.Windows.Forms.ToolStripMenuItem
		Private FileSave As System.Windows.Forms.ToolStripMenuItem
		Private toolStripMenuItem1 As System.Windows.Forms.ToolStripSeparator
		Private FileOpen As System.Windows.Forms.ToolStripMenuItem
		Private FileNew As System.Windows.Forms.ToolStripMenuItem
		
		#End Region
		
		'Handler for anything that goes wrong and doesn't get handled elsewhere
		Private Sub UnhandledExceptionHandler(sender as Object, args As UnhandledExceptionEventArgs)
			Dim Ex As Exception = args.ExceptionObject
			'MessageBox.Show(Ex.Message + Environment.NewLine + Ex.StackTrace, "Internal error in " + Ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Warning)
			
			Dim errorWindow As New InternalErrorDialog
			errorWindow.ErrorMessage = Ex.Message + Environment.NewLine + Ex.StackTrace
			errorWindow.ShowDialog
			
		End Sub
		
		'Handler for anything that goes wrong in underlying system
		Private Sub ThreadExceptionHandler(sender As Object, args As ThreadExceptionEventArgs)
			Dim Ex As Exception = args.Exception
			
			Dim errorWindow As New InternalErrorDialog
			errorWindow.ErrorMessage = Ex.Message + Environment.NewLine + Ex.StackTrace
			errorWindow.ShowDialog
			
		End Sub
		
		Private Sub MainFormLoad(sender As System.Object, e As System.EventArgs)
		 	
		 	'Redraw Programmer list
		 	UpdateProgrammerList
		 	
		 	'Update file load extension list
		 	UpdateFileLoadTypeList
		 	
		 	'Get list of demo files
		 	UpdateExamplesList
		 	
		 	'If file is specified, open it
		 	If CommandLine <> "" Then
				'MessageBox.Show(CommandLine, "Message", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1)
				
    			Dim fProgContainer As ProgramContainer
    			fProgContainer = CreateProgramContainer(CommandLine)
    			'fProgContainer.WindowState = WindowState.Maximized
    			
			End If
			
			'If no file is specified, show a new blank window but don't maximise
			If CommandLine = "" Then
				
				Dim fProgContainer As ProgramContainer
				fProgContainer = CreateProgramContainer("")
			End If
			
			'Add event handlers
			'AddHandler GetCurrentProgram.LibraryListChanged, AddressOf Me.CommandListChanged
			
			'Translate
			ApplyTranslation
			
		End Sub
		
		'Applies the current translation to the window
		Private Sub ApplyTranslation
			
			With Translator
				If .CanTranslate Then
					
					'Menus
					.TryTranslate("MainForm", "FileMenu", FileMenu.Text)
					.TryTranslate("MainForm", "FileNew", FileNew.Text)
					.TryTranslate("MainForm", "FileOpen", FileOpen.Text)
					.TryTranslate("MainForm", "FileOpenExample", FileOpenExample.Text)
					.TryTranslate("MainForm", "FileSave", FileSave.Text)
					.TryTranslate("MainForm", "FileSaveAs", FileSaveAs.Text)
					.TryTranslate("MainForm", "FileClose", FileClose.Text)
					.TryTranslate("MainForm", "FileExit", FileExit.Text)
					
					.TryTranslate("MainForm", "EditMenu", EditMenu.Text)
					.TryTranslate("MainForm", "EditCut", EditCut.Text)
					.TryTranslate("MainForm", "EditCopy", EditCopy.Text)
					.TryTranslate("MainForm", "EditPaste", EditPaste.Text)
					.TryTranslate("MainForm", "EditUndo", EditUndo.Text)
					.TryTranslate("MainForm", "EditRedo", EditRedo.Text)
					.TryTranslate("MainForm", "EditPrefs", EditPrefs.Text)
					
					.TryTranslate("MainForm", "ViewMenu", ViewMenu.Text)
					.TryTranslate("MainForm", "ViewIcons", ViewIcons.Text)
					.TryTranslate("MainForm", "ViewText", ViewText.Text)
					.TryTranslate("MainForm", "ViewZoomIn", ViewZoomIn.Text)
					.TryTranslate("MainForm", "ViewZoomOut", ViewZoomOut.Text)
					.TryTranslate("MainForm", "ViewZoomReset", ViewZoomReset.Text)
					
					.TryTranslate("MainForm", "ProgramMenu", ProgramMenu.Text)
					.TryTranslate("MainForm", "ProgramLibs", ProgramLibs.Text)
					.TryTranslate("MainForm", "ProgramConsts", ProgramConsts.Text)
					.TryTranslate("MainForm", "ProgramVars", ProgramVars.Text)
					.TryTranslate("MainForm", "ProgramInterrupt", ProgramInterrupt.Text)
					.TryTranslate("MainForm", "ProgramTables", ProgramTables.Text)
					.TryTranslate("MainForm", "ProgramChip", ProgramChip.Text)
					.TryTranslate("MainForm", "ProgramAdvanced", ProgramAdvanced.Text)
					
					.TryTranslate("MainForm", "ToolsMenu", ToolsMenu.Text)
					.TryTranslate("MainForm", "ToolsSummary", ToolsSummary.Text)
					.TryTranslate("MainForm", "ToolsASM", ToolsASM.Text)
					.TryTranslate("MainForm", "ToolsCompile", ToolsCompile.Text)
					.TryTranslate("MainForm", "ToolsDownload", ToolsDownload.Text)
					.TryTranslate("MainForm", "ToolsDownloadWith", ToolsDownloadWith.Text)
					.TryTranslate("MainForm", "ToolsExternal", ToolsExternal.Text)
					
					.TryTranslate("MainForm", "HelpMenu", HelpMenu.Text)
					.TryTranslate("MainForm", "HelpIcon", HelpIcon.Text)
					.TryTranslate("MainForm", "HelpContents", HelpContents.Text)
					.TryTranslate("MainForm", "HelpIndex", HelpIndex.Text)
					.TryTranslate("MainForm", "HelpAbout", HelpAbout.Text)
					
					'Icon chooser
					.TryTranslate("MainForm", "IconPanel", IconPanel.Text)
					.TryTranslate("MainForm", "AddCommandGroup", AddCommandGroup.Text)
					.TryTranslate("MainForm", "AddCommandButton", AddCommandButton.Text)
					.TryTranslate("MainForm", "LabelIconCat", LabelIconCat.Text)
					.TryTranslate("MainForm", "LabelAvailableIcons", LabelAvailableIcons.Text)
					
				End If
			End With
			
		End Sub
		
		'Get the current program's container
		Public Function GetCurrentContainer As ProgramContainer
			Return Me.ActiveMdiChild
		End Function
		
		'Get the GCBProgram object for the current displayed file
		Public Shared Function GetCurrentProgram As GCBProgram
			Dim fProgWindow As ProgramContainer
			fProgWindow = MainForm.MainFormInstance.GetCurrentContainer
			
			If fProgWindow Is Nothing Then
				Return Nothing
			Else
				fProgWindow.Program.Editor.SyncEditorToProgram
				Return fProgWindow.Program
			End If
			
		End Function
		
		Public Shared Function GetPreferences As Preferences
			Return Prefs
		End Function
		
		Public Shared Function GetFileConverter(ByVal InFile As String) As FileConverter
			Dim FileExtension As String
			Dim Temp As Integer
			
			'Get the file extension
			FileExtension = ""
			For Temp = InFile.Length - 1 To 0 Step -1
				If InFile.Substring(Temp, 1) = "." Then
					FileExtension = InFile.Substring(Temp + 1).ToLower.Trim
					Exit For
				End If
			Next
			'Return nothing if this is a native format
			If FileExtension = "" or FileExtension = "gcb" Or FileExtension = "h" Or FileExtension = "txt" Then
				Return Nothing
			End If
			
			'Get the converter
			Dim ThisConverter As FileConverter
			For Each ThisConverter In FileConverters
				With ThisConverter
					If FileExtension = .GetInType.Trim.ToLower Then
						Return ThisConverter
					End If
				End With
			Next
			
			Return Nothing
		End Function
		
		Private Sub FileExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
			Me.Close()
		End Sub
		
		Private Sub FileNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
			
			Dim fProgContainer As ProgramContainer
			fProgContainer = CreateProgramContainer("")
			
		End Sub
		
		Public Function CreateProgramContainer(InProgram As GCBProgram) As ProgramContainer
			'Create window
			Dim fProgWindow As New ProgramWindow
			fProgWindow.Program = InProgram
			If InProgram.FileName = "" Then
				InProgram.FileName = "Untitled" + Mainform.UntitledCount.ToString
				Mainform.UntitledCount += 1
			End If
			
			fProgWindow.SyncProgramToEditor
			
			'Set up and display window
			With fProgWindow
				.MdiParent = Me
				AddHandler .RefreshUndoRedo, AddressOf Me.UpdateUndoRedo
				AddHandler .ThisWindowSelected, AddressOf Me.ProgWindowChanged
				AddHandler .SubAdded, AddressOf Me.RedrawSubList
				AddHandler .Program.LibraryListChanged, AddressOf Me.CommandListChanged
				AddHandler .ViewModeChanged, AddressOf Me.RenderCommandList
				.WindowState = FormWindowState.Maximized
	    		.Show()
	    	End With
	    	
	    	Return fProgWindow
		End Function
		
		Public Function CreateProgramContainer(ByVal InFile As String) As ProgramContainer
			
			'Check if InFile needs to be converted to another file type
			If InFile <> "" Then
				Dim Converter As FileConverter
				Converter = GetFileConverter(InFile)
				'File needs to be converted, so convert it
				If Not Converter Is Nothing Then
					InFile = Converter.Convert(InFile)
					If InFile = "" Then
						MessageBox.Show("Unable to convert file", "Great Cow Graphical BASIC", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
					End If
				End If
			End If
			
			'Create window
			Dim fProgWindow As ProgramWindow
			fProgWindow = New ProgramWindow(InFile)
			
			'Set up and display window
			With fProgWindow
				.MdiParent = Me
				AddHandler .RefreshUndoRedo, AddressOf Me.UpdateUndoRedo
				AddHandler .ThisWindowSelected, AddressOf Me.ProgWindowChanged
				AddHandler .SubAdded, AddressOf Me.RedrawSubList
				AddHandler .Program.LibraryListChanged, AddressOf Me.CommandListChanged
				AddHandler .ViewModeChanged, AddressOf Me.RenderCommandList
				.WindowState = FormWindowState.Maximized
	    		.Show()
	    	End With
	    	
	    	Return fProgWindow
		End Function
		
		Private sub FileOpen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
		    OpenFileSelect.ShowDialog()		    
		End Sub
		
		Private sub EditCut_Click(ByVal sender as System.Object, Byval e As System.EventArgs)
			Dim fProgContainer As ProgramContainer
			fProgContainer = GetCurrentContainer
			If Not fProgContainer Is Nothing Then
				fProgContainer.ClipboardOp("x")
			End If
		End Sub
		
		Private sub EditCopy_Click(ByVal sender as System.Object, Byval e As System.EventArgs)
			Dim fProgContainer As ProgramContainer
			fProgContainer = GetCurrentContainer
			If Not fProgContainer Is Nothing Then
				fProgContainer.ClipboardOp("c")
			End If
		End Sub
		
		Private sub EditPaste_Click(ByVal sender as System.Object, Byval e As System.EventArgs)
			Dim fProgContainer As ProgramContainer
			fProgContainer = GetCurrentContainer
			If Not fProgContainer Is Nothing Then
				fProgContainer.ClipboardOp("v")
			End If
		End Sub
		
		Private sub EditUndo_Click(ByVal sender as System.Object, Byval e As System.EventArgs)
			Dim fProgContainer As ProgramContainer
			fProgContainer = GetCurrentContainer
			If Not fProgContainer Is Nothing Then
				fProgContainer.DoUndoRedo(0)
			End If
			
		End Sub
		
		Private sub EditRedo_Click(ByVal sender as System.Object, Byval e As System.EventArgs)
			Dim fProgContainer As ProgramContainer
			fProgContainer = GetCurrentContainer
			If Not fProgContainer Is Nothing Then
				fProgContainer.DoUndoRedo(1)
			End If
		End Sub
		
		Private Sub EditPrefs_Click(ByVal sender as System.Object, Byval e As System.EventArgs)
			Dim fPreferences As New PreferencesWindow
			fPreferences.Preferences = GetPreferences
			fPreferences.ShowDialog
		End Sub
		
		Private Sub ProgramVars_Click(ByVal sender as System.Object, Byval e As System.EventArgs)
			Dim fVariables As New Variables(GetCurrentProgram)
			fVariables.ShowDialog()
		End Sub
		
		Private sub HelpAbout_Click(ByVal sender as System.Object, Byval e As System.EventArgs)
			dim fAboutForm As New AboutForm
			fAboutForm.ShowDialog()
			
			'This is a good spot to put test code
			'Dim testForm As New TypeChooserTest
			'testForm.ShowDialog
		End Sub
		
		Private Sub RedrawSubList()
			If GetCurrentProgram.CurrentCat = IconCategory.Count Then RenderCommandList
		End Sub
		
		Private Sub CommandListChanged()
			RenderCommandList
		End Sub
		
		Private Sub RenderCommandList()
						
			Dim Temp As Integer
			
			Dim fProgContainer As ProgramContainer
			fProgContainer = GetCurrentContainer
			
			If fProgContainer Is Nothing Then
				'Remove old icons
				For Temp = 1 To MaxIconsPerCategory
					Me.IconListPanel.Controls.Remove(Me.IconDisplay(Temp))
				Next
				'IconCategories, IconListPanel
				Me.IconCategories.Enabled = False
				Me.IconListPanel.BackColor = System.Drawing.SystemColors.InactiveBorder
				Me.EnterCommand.Enabled = False
				Me.AddCommandButton.Enabled = False
				
				'Disable menu items
				Me.FileSave.Enabled = False
				Me.FileSaveAs.Enabled = False
				Me.FileClose.Enabled = False
				Me.EditCut.Enabled = False
				Me.EditCopy.Enabled = False
				Me.EditPaste.Enabled = False
				Me.EditUndo.Enabled = False
				Me.EditRedo.Enabled = False
				Me.ViewText.Enabled = False
				Me.ViewIcons.Enabled = False
				Me.ViewZoomIn.Enabled = False
				Me.ViewZoomOut.Enabled = False
				Me.ViewZoomReset.Enabled = False
				Me.ProgramConsts.Enabled = False
				Me.ProgramChip.Enabled = False
				Me.ProgramLibs.Enabled = False
				Me.ProgramConsts.Enabled = False
				Me.ProgramTables.Enabled = False
				Me.ProgramVars.Enabled = False
				Me.ProgramInterrupt.Enabled = False
				Me.ProgramAdvanced.Enabled = False
				Me.ToolsSummary.Enabled = False
				Me.ToolsASM.Enabled = False
				Me.ToolsCompile.Enabled = False
				Me.ToolsDownloadWith.Enabled = False
				Me.ToolsDownload.Enabled = False
				Me.ToolsExternal.Enabled = False
				Me.HelpIcon.Enabled = False
				
				Exit Sub
			End If
			
			'Draw Category List
			Me.IconCategories.Enabled = True
			Me.IconListPanel.BackColor = System.Drawing.Color.White
			Me.EnterCommand.Enabled = True
			Me.AddCommandButton.Enabled = True
			IconCategories.Items.Clear()
			
			'Standard Icons
			Dim currCat As String
			For Each currCat In IconCategory
				Me.IconCategories.Items.Add(currCat)
	     	Next
	     	'Subs in program
	     	Me.IconCategories.Items.Add("Subroutines")
			'Included files
			With GetCurrentProgram
				Dim LibTemp As LibraryType
				For Each LibTemp in .Libraries
					Me.IconCategories.Items.Add("Library: " + LibTemp.DisplayName)
				Next
				
				If .CurrentCat >= Me.IconCategories.Items.Count Then
					.CurrentCat = Me.IconCategories.Items.Count - 1
				End If
				Me.IconCategories.SelectedIndex = .CurrentCat
				
			End With
			
			'Enable Menu Items
			Dim CanEditProperties As Boolean = False
			If Not GetCurrentProgram Is Nothing Then
				CanEditProperties = GetCurrentProgram.Editor.CanEditProperties
			End If
			Me.FileSave.Enabled = True
			Me.FileSaveAs.Enabled = True
			Me.FileClose.Enabled = True
			Me.EditCut.Enabled = True
			Me.EditCopy.Enabled = True
			Me.EditPaste.Enabled = True
			'Me.EditUndo.Enabled = True
			'Me.EditRedo.Enabled = True
			
			Me.ViewText.Enabled = (TypeOf GetCurrentProgram.Editor Is GuiEditorPanel)
			Me.ViewIcons.Enabled = (TypeOf GetCurrentProgram.Editor Is TextEditorPanel)
			Me.ViewZoomIn.Enabled = True
			Me.ViewZoomOut.Enabled = True
			Me.ViewZoomReset.Enabled = True
			
			Me.ProgramConsts.Enabled = CanEditProperties
			me.ProgramTables.Enabled = CanEditProperties
			Me.ProgramChip.Enabled = CanEditProperties
			Me.ProgramLibs.Enabled = CanEditProperties
			Me.ProgramConsts.Enabled = CanEditProperties
			Me.ProgramVars.Enabled = CanEditProperties
			Me.ProgramInterrupt.Enabled = CanEditProperties
			Me.ProgramAdvanced.Enabled = CanEditProperties
			Me.ToolsSummary.Enabled = True
			Me.ToolsASM.Enabled = True
			Me.ToolsCompile.Enabled = True
			Me.ToolsDownload.Enabled = True
			Me.ToolsDownloadWith.Enabled = True
			Me.ToolsExternal.Enabled = True
			Me.HelpIcon.Enabled = True
			
			UpdateUndoRedo
			
		End Sub
		
		Private sub FileSaveAs_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
		    SaveFileSelect.FileName = ""
		    SaveFileSelect.ShowDialog()
		End Sub
		
		Private Sub FileSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
			Dim currFile As GCBProgram
			currFile = GetCurrentProgram
			
			If currFile.Filename.ToLower.Substring(0,8) = "untitled" Then
				 SaveFileSelect.FileName = ""
				 SaveFileSelect.ShowDialog
				 Exit Sub
			End If
			Try
		 		currFile.SaveFile(currFile.Filename)
		 	Catch ex as UnauthorizedAccessException
		 		'Could not save file, offer to let user choose new location
		 		Dim result As DialogResult = MessageBox.Show("GCGB cannot save your file in the chosen location. Would you like to choose a new name or folder?", "Great Cow Graphical BASIC", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
				If result = DialogResult.Yes Then
					SaveFileSelect.FileName = ""
		    		SaveFileSelect.ShowDialog()
				End If
				'Exit if save failed - don't clear unchanged flag!
				Exit Sub
		 	End Try
			
			currFile.HasChanged = False
						
		End Sub
		
		Private Sub SaveFileSelectFileOk(sender As System.Object, e As System.ComponentModel.CancelEventArgs)
			Dim OutFile As String
			Dim fProgContainer As ProgramContainer
			fProgContainer = GetCurrentContainer
			If fProgContainer Is Nothing Then
				Exit Sub
			End If
			Dim currFile As GCBProgram = fProgContainer.Program
			
			OutFile = SaveFileSelect.FileName
		 	currFile.Filename = OutFile
		 	
		 	fProgContainer.ProgramName = OutFile
		 	
		 	Try
		 		currFile.SaveFile(OutFile)
		 	
		 	Catch ex as UnauthorizedAccessException
		 		'Could not save file, offer to let user choose new location
		 		Dim result As DialogResult = MessageBox.Show("GCGB cannot save your file in the chosen location. Would you like to choose a new name or folder?", "Great Cow Graphical BASIC", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1)
				If result = DialogResult.Yes Then
					'Clear filename and cancel save (reshows save dialog)
					SaveFileSelect.FileName = ""
		    		e.Cancel = True
				End If
				Exit Sub
		 	End Try
		 	
		 	currFile.ProgramDir = System.IO.Path.GetDirectoryName(OutFile)
		 	currFile.HasChanged = False
		 	
		End Sub
		
		Private Sub OpenFileSelectFileOk(sender As System.Object, e As System.ComponentModel.CancelEventArgs)
			
			Dim fProgContainer As ProgramContainer
			fProgContainer = CreateProgramContainer(OpenFileSelect.FileName)
			
		End Sub
		
		Public Shared Function GetStatus As String
			Return MainformInstance.StatusBar.Text
		End Function
		
		'Change the currently displayed icon category to the chosen one
		'NewCategory must match the name exactly
		Public Sub SetIconCategory(NewCategory As String)
			
			Me.IconCategories.SelectedItem = NewCategory
	     	
		End Sub
		
		Public Shared Sub SetStatus(Message As String)
			MainformInstance.StatusBar.Text = Message
		End Sub
		
		Private Sub UpdateUndoRedo
			Me.EditUndo.Enabled = False
			Me.EditRedo.Enabled = False
			
			Dim fProgContainer As ProgramContainer
			fProgContainer = GetCurrentContainer
			If fProgContainer Is Nothing Then
				Exit Sub
			End If
			
			With GetCurrentProgram
				If .UndoRedoLocation > 0 Then EditUndo.Enabled = True
				If .UndoRedoLocation < .UndoRedoCount Then EditRedo.Enabled = True
			End With
			
		End Sub
		
		Public Sub UpdateExamplesList
			'Update list of example files
			Dim ExampleDir As New IO.DirectoryInfo(MainForm.InstallDir + Mainform.FilenameExamplesDir)
			Dim ExampleList As IO.FileInfo() = ExampleDir.GetFiles("*.gcb")
			Dim ExampleFileName As IO.FileInfo
			Dim NewItem As toolStripMenuItem
			For Each ExampleFileName In ExampleList
				NewItem = New toolStripMenuItem(ExampleFileName.Name.Substring(0, ExampleFileName.Name.Length - 4))
				AddHandler NewItem.Click, AddressOf FileOpenExampleItemClick
				Me.FileOpenExample.DropDownItems.Add(NewItem)
			Next
			
			'Add ExampleChooser test
			NewItem = New ToolStripMenuItem("Choose another example ...")
			AddHandler NewItem.Click, AddressOf FileOpenExampleClick
			Me.FileOpenExample.DropDownItems.Add(New ToolStripSeparator)
			Me.FileOpenExample.DropDownItems.Add(NewItem)
			
		End Sub
		
		Private Sub FileOpenExampleItemClick(sender As System.Object, e As System.EventArgs)
			'Get example name
			Dim ClickedItem As toolStripMenuItem
			Dim ExampleFile As String
			If TypeOf sender Is toolStripMenuItem Then
				ClickedItem = sender
				ExampleFile = MainForm.InstallDir + Mainform.FilenameExamplesDir + ClickedItem.Text + ".gcb"
				
				'Load example file
				Dim fProgContainer As ProgramContainer
				fProgContainer = CreateProgramContainer(ExampleFile)
				
			End If
		End Sub
		
		Public Sub UpdateFileLoadTypeList
			
			'Remove "All Files"
			Dim FileFilter, AllFiles As String
			FileFilter = OpenFileSelect.Filter
			AllFiles = FileFilter.Substring(FileFilter.ToLower.IndexOf("|all files"))
			FileFilter = FileFilter.Substring(0, FileFilter.ToLower.IndexOf("|all files"))
			
			'Find all importers, add names and types to File Open dialog
			Dim ThisConverter As FileConverter
			For Each ThisConverter In FileConverters
				With ThisConverter
					FileFilter += ("|" + .GetDescription + "|" + "*." + .GetInType)
				End With
			Next
			
			'Re-add "All Files"
			OpenFileSelect.Filter = FileFilter + AllFiles
			
		End Sub
		
		Public Sub UpdateProgrammerList
			
			'Clear list of programmers on Tools menu
			'Dim OldItem As MenuItem
			'For Each OldItem In ToolsDownloadWith.MenuItems
			'	RemoveHandler OldItem.Click, AddressOf Me.ToolsDownloadWithItem_Click
			'Next
			Me.ToolsDownloadWith.DropDownItems.Clear
			
			'Add new list
			'Find all tools
			Dim section As SettingSection
			Dim ProgName As String
			Dim ProgrammersAdded As Boolean = False
			Dim NewProg As toolStripMenuItem
			For Each section In GetPreferences.PrefGroups
				If section.Name.ToLower.StartsWith("tool") Then
					If section.GetValue("type") = "programmer" Then
						
						'Create menu item
						NewProg = New toolStripMenuItem(section.GetValue("desc"))
						'Should item be checked?
						ProgName = section.Name
						ProgName = ProgName.Substring(ProgName.IndexOf("=") + 1).Trim
						If ProgName = GetPreferences.GetPref("GCBASIC", "Programmer") Then
							NewProg.Checked = True
						End If
						
						'Add
						AddHandler NewProg.Click, AddressOf Me.ToolsDownloadWithItem_Click
						ToolsDownloadWith.DropDownItems.Add(NewProg)
						ProgrammersAdded = True
					End If
				End If
			Next
			
			'If in restricted mode, we shouldn't allow the user access to any command prompts
			If Preferences.PrefIsYes(Prefs.GetPref("GCGB", "Restricted")) Then
				ProgrammersAdded = False
			End If
			'Show edit programmer option
			If ProgrammersAdded Then
				ToolsDownloadWith.DropDownItems.Add("-")
				NewProg = New ToolStripMenuItem(Translator.GetMessage("MainForm", "ToolsEditProgrammer", "Edit Selected Programmer ..."))
				AddHandler NewProg.Click, AddressOf Me.ToolsEditProgrammer_Click
				ToolsDownloadWith.DropDownItems.Add(NewProg)
			End If
			
			'Update list of external tools
			'Clear list of external tools on Tools menu
			'For Each OldItem In ToolsExternal.MenuItems
			'	RemoveHandler OldItem.Click, AddressOf Me.ToolsExternalItem_Click
			'Next
			'ToolsExternal.MenuItems.Clear
			Me.ToolsExternal.DropDownItems.Clear
			
			'Add new list
			'Find all tools
			For Each section In GetPreferences.PrefGroups
				If section.Name.ToLower.StartsWith("tool") Then
					If section.GetValue("type") = "external" Then
						
						'Create menu item
						NewProg = New toolStripMenuItem(section.GetValue("desc"))
						
						'Add
						AddHandler NewProg.Click, AddressOf Me.ToolsExternalItem_Click
						Me.ToolsExternal.DropDownItems.Add(NewProg)
					End If
				End If
			Next
			
		End Sub
		
		Private Sub ToolsDownloadWithItem_Click(ByVal sender as System.Object, Byval e As System.EventArgs)
			'MessageBox.Show(sender.Text, "Testing")
			Dim section As SettingSection
			Dim progName As String
			
			'Get programmer ID from display name
			section = GetPreferences.GetToolSettings(sender.Text)
			progName = section.Name
			progName = progName.Substring(progName.IndexOf("=") + 1).Trim
			
			'Update settings
			GetPreferences.SetPref("GCBASIC", "programmer", progName)
			UpdateProgrammerList
			GetPreferences.SavePreferences
			
			'Download program
			DownloadProgramNow
			'Dim CompilerThread As New Thread(AddressOf DownloadProgramNow)
			'CompilerThread.Start
			
		End Sub
		
		Private Sub ToolsEditProgrammer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
			'Open programmer editor for current programmer
			'Find current programmer settings
			Dim CurrentProgrammer As String = GetPreferences.GetPref("GCBASIC", "Programmer")
			Dim ProgrammerSection As SettingSection = GetPreferences.GetToolSettingsFromID(CurrentProgrammer)
			
			'Launch editor
			Dim fProgEditor As New ProgrammerEditor(GetPreferences, ProgrammerSection)
			fProgEditor.ShowDialog
			
			'Store settings
			If fProgEditor.WasAccepted Then
				'Just reading the Section will update saved preferences
				ProgrammerSection = fProgEditor.Section
				UpdateProgrammerList
				GetPreferences.SavePreferences
			End If
		End Sub
		
		Private Sub ToolsExternalItem_Click(ByVal sender as System.Object, Byval e As System.EventArgs)
			Dim section As SettingSection
			Dim progName, progParams, progDir As String
			
			'Get tool ID from display name
			section = GetPreferences.GetToolSettings(sender.Text)
			
			'Get name and params
			progName = section.GetValue("command")
			progParams = section.GetValue("params")
			progDir = section.GetValue("workingdir")
			progName = Lowlevel.ReplaceToolVariables(progName, "", "", GetCurrentProgram)
			progParams = Lowlevel.ReplaceToolVariables(progParams, "", "", GetCurrentProgram)
			progDir = Lowlevel.ReplaceToolVariables(progDir, "", "", GetCurrentProgram)
			
			If progName = "" Then Exit Sub
			
			'Run tool
			Dim runTool As New System.Diagnostics.Process
			runTool.StartInfo.FileName = progName
			runTool.StartInfo.Arguments = progParams
			If progDir <> "" Then
				runTool.StartInfo.WorkingDirectory = progDir
			End If
			runTool.Start
			
		End Sub
		
		Private Sub MainFormClosing(sender As System.Object, e As System.ComponentModel.CancelEventArgs)
		 	'Find and close open windows
			Dim fProgContainer As ProgramContainer
			Do
				Try
					fProgContainer = GetCurrentContainer
					If Not fProgContainer.CloseProgram Then
						e.Cancel = True
						Exit Sub
					End If
				Catch
					Exit Do
				End Try
			Loop
		End Sub
		
		Private Sub ProgramChipClick(sender As System.Object, e As System.EventArgs)
			Dim CurrProgram As GCBProgram = GetCurrentProgram
			If Not CurrProgram Is Nothing Then
				dim fChipConf As New ChipSettings(CurrProgram)
				fChipConf.ShowDialog()
				
				GetCurrentContainer.SyncProgramToEditor
			End If
		End Sub
		
		Private Sub ProgramConstsClick(sender As System.Object, e As System.EventArgs)
			Dim CurrProgram As GCBProgram = GetCurrentProgram
			If Not CurrProgram Is Nothing Then
				Dim fConstants As New ConstantWindow(CurrProgram)
				fConstants.ShowDialog
			End If
		End Sub
		
		Private Sub ToolsSummaryClick(sender As System.Object, e As System.EventArgs)
			Dim CurrProgram As GCBProgram = GetCurrentProgram
			If Not CurrProgram Is Nothing Then
				'Dim fSummary As New ProgSummary(CurrProgram)
				'fSummary.ShowDialog
				
				'Do compile, and open report if successful
				If Compile(0,, True) Then
					Dim fReport As New CompReportViewer
					
					'Get asm filename
					fReport.ReportFile = GetCurrentProgram.FindGeneratedFile(".html")
					
					'Display
					fReport.ShowDialog
				End If
			End If
			
		End Sub
		
		Private Sub ProgramTablesClick(sender As System.Object, e As System.EventArgs)
			Dim CurrProgram As GCBProgram = GetCurrentProgram
			If Not CurrProgram Is Nothing Then
				Dim fTableWindow As New TableWindow(CurrProgram)
				fTableWindow.ShowDialog()
			End If
		End Sub
		
		Private Sub FileCloseClick(sender As System.Object, e As System.EventArgs)
			Dim fProgContainer As ProgramContainer = GetCurrentContainer
			If fProgContainer Is Nothing Then
				Exit Sub
			Else
				fProgContainer.CloseProgram
			End If
		End Sub
		
		Private Sub ProgWindowChanged()
			RenderCommandList
			UpdateUndoRedo
		End Sub
		
		Private Sub MainFormMdiChildActivate(sender As System.Object, e As System.EventArgs)
			RenderCommandList
			UpdateUndoRedo
		End Sub
		
		Private Sub IconCategoriesSelectedValueChanged(sender As System.Object, e As System.EventArgs)
			Dim CategoryName, CommandName As String
			Dim Temp, IconLocation As Integer
			Dim NewSub As GCBSubroutine
			
			Dim fProgContainer As ProgramContainer = GetCurrentContainer
			If fProgContainer Is Nothing Then Exit Sub
			
			GetCurrentProgram.CurrentCat = Me.IconCategories.SelectedIndex
			
			'Remove old icons
			For Temp = 1 To MaxIconsPerCategory
				Try
					RemoveHandler Me.IconDisplay(Temp).MouseMove, AddressOf Me.IconListMouseMove
				Catch
				End Try
				Me.IconListPanel.Controls.Remove(Me.IconDisplay(Temp))
			Next
			
			'Draw Command list
			IconLocation = 0
			CategoryName = Me.IconCategories.SelectedItem.ToLower.Trim
			'Standard Icon
			If GetCurrentProgram.CurrentCat < IconCategory.Count Then
				For Temp = 0 To CommandList.Count - 1
					With CommandList(Temp)
						CommandName = .Category.ToLower.Trim
						If CommandName = CategoryName Then
							IconLocation += 1
							Me.DrawIcon(.Icon, .Name, IconLocation, .Name, .Tooltip)
						End If
					End With
				Next
			
			'Subroutine
			ElseIf GetCurrentProgram.CurrentCat = IconCategory.Count Then
				'For Temp = 1 to GetCurrentProgram.SubCount
				For Each NewSub In GetCurrentProgram.Subroutines
					If NewSub.IsSub And Not GetCurrentProgram.IsMainSub(NewSub) And Not NewSub.HideSub Then
						IconLocation += 1
						Me.DrawIcon("Subroutine", NewSub.Name, IconLocation, NewSub.Name, NewSub.Description)
					End If
	     		Next
			
			'Library
			ElseIf GetCurrentProgram.CurrentCat > IconCategory.Count Then
				Dim LibTemp As LibraryType
				Dim CaptionWithSource As String
				LibTemp = GetCurrentProgram.Libraries.Item(GetCurrentProgram.CurrentCat - IconCategory.Count - 1)
				For Each NewSub In LibTemp.Subroutines
					If NewSub.IsSub And Not NewSub.HideSub Then
						IconLocation += 1
						CaptionWithSource = NewSub.Description
						If NewSub.IncludedFrom <> "" Then
							CaptionWithSource = (NewSub.Description + " (From " + NewSub.IncludedFrom + ")").Trim
						End If
						Me.DrawIcon("Subroutine", NewSub.Name, IconLocation, NewSub.Name, CaptionWithSource)
					End If
				Next
				
			End If
			
			'Select container
			If Me.IconCategories.ContainsFocus Then
				Me.IconListPanel.Select
			End If
			
		End Sub
		
		Private Sub DrawIcon(ByVal IconName As String, ByVal ParamList As String, Pos As Integer, ByVal IconHandler As String, Optional Tooltip As String = "", Optional Compact As Boolean = False)
			Dim SearchIcon, CurrIcon As GCBIcon
			
			IconHandler = Pos.ToString + ";" + IconHandler
			
			'Get the icon
			If IconName = "" Then
				IconName = "default"
			Else
				IconName = IconName.Trim.ToLower
			End If
			Do
				
				'Find icon data
				CurrIcon = Nothing
				For Each SearchIcon In IconList
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
				'Show icon
				Me.IconDisplay(Pos) = New IconDisplay(CurrIcon, Nothing, IconHandler, True, ParamList)
				With Me.IconDisplay(Pos)
				 	'In compact display mode, icons are half sized and in two columns
				 	If Compact Then
				 		.Zoom = 0.5
				 		.FontZoom = 0.8
				 		If (Pos Mod 2) = 1 Then
				 			.Location = New System.Drawing.Point((75 - .ActualX) / 2, -15 + Pos * 25)
				 		Else
				 			.Location = New System.Drawing.Point(75 + (75 - .ActualX) / 2, -40 + Pos * 25)
				 		End If
				 		
				 	Else
				 		.Location = New System.Drawing.Point((150 - CurrIcon.XSize) / 2, -90 + Pos * 100)
				 	End If
				 	AddHandler .MouseMove, AddressOf Me.IconListMouseMove
				End With
				Me.IconListPanel.Controls.Add(Me.IconDisplay(Pos))
				
				'Add tooltip
				'MainFormToolTip.SetToolTip(Me.IconDisplay(Pos), "Drag and drop icon into program to add")
				If Tooltip <> "" Then
					MainFormToolTip.SetToolTip(Me.IconDisplay(Pos), Tooltip)
				End If
				
			End If
						
		End Sub
		
		Private Sub IconListMouseMove(sender As System.Object, e As System.Windows.Forms.MouseEventArgs)
			Dim IconNo As Integer
			Dim TempData As String
			
			TempData = sender.Name
			IconNo = TempData.Substring(0,TempData.IndexOf(";"))
			TempData = "IsName:" + TempData.Substring(TempData.IndexOf(";") + 1)
			
			If e.Button = MouseButtons.Left Then
				'MessageBox.Show(IconNo.ToString + "<>" + TempData, "Message", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1)
				IconDisplay(IconNo).DoDragDrop(TempData, DragDropEffects.All)
				
			End If
			
		End Sub
		
		Private Sub EnterCommandKeyPress(sender As System.Object, e As System.Windows.Forms.KeyPressEventArgs)
			If e.KeyChar = Convert.ToChar(13) Then
				Dim fProgContainer As ProgramContainer = GetCurrentContainer
				If fProgContainer Is Nothing Then Exit Sub
				
				e.Handled = True
				fProgContainer.AddAfterSelected(Me.EnterCommand.Text)
				Me.EnterCommand.SelectAll()
			End If
			
		End Sub
		
		Private Function Compile(CompileMode As Integer, Optional ExtraParams As String = "", Optional ExitWait As Boolean = False) As Boolean
		
			'Returns true if compile successful, false if error
			'CompileMode: 0 - compile, 1 - compile and assemble, 2 - do it all
			
			If GetCurrentContainer Is Nothing Then Exit Function
			
			'Check that chip model is specified
			If GetCurrentProgram.GetCurrentChip = "" Then
				If TypeOf GetCurrentProgram.Editor Is GuiEditorPanel Then
					Dim result As DialogResult = MessageBox.Show("The program cannot be compiled unless a chip model is specified! Do this now?", "Great Cow Graphical BASIC", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
					If result = DialogResult.No Then Return False
				Else
					MessageBox.Show("The program cannot be compiled unless a chip model is specified", "Great Cow Graphical BASIC", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
					Return False
				End If
				
				dim fChipConf As New ChipSettings(GetCurrentProgram)
				fChipConf.ShowDialog()
				If GetCurrentProgram.GetCurrentChip = "" Then
					Exit Function
				End If
				
			End If
			
			'Save temp file
			Dim AlternateFile As String
			Dim SecondTry, UsingOriginalFile As Boolean
			
			Do
				SecondTry = False
				
				'Get filename
				'If first try using .Filename fails, second try will have .FileLocked set
				If GetCurrentProgram.Filename.ToLower.Substring(0,8) = "untitled" Or GetCurrentProgram.FileLocked Then
					AlternateFile = System.Environment.GetEnvironmentVariable("Temp") + "\GCGB.tmp"
					UsingOriginalFile = False
				Else
					AlternateFile = GetCurrentProgram.Filename
					UsingOriginalFile = True
				End If
				
				'Try to save file
				Try
			 		GetCurrentProgram.SaveFile(AlternateFile)
			 	
			 	Catch ex as UnauthorizedAccessException
			 		'Could not save file, try using temp file
			 		If UsingOriginalFile Then
			 			GetCurrentProgram.FileLocked = True
			 			SecondTry = True
			 		End If
			 		
			 	End Try
				
			Loop While SecondTry = True
			
			'Start new thread to run compiler
			Dim startComp As New CompilerStarter(Me, AlternateFile, CompileMode, ExtraParams)
			Dim CompilerThread As New Thread(AddressOf startComp.Start)
			CompilerThread.Start
			
			'If exit wait flag set, wait for compile to complete
			If ExitWait Then
				Do While startComp.Completed = False
					Application.DoEvents
					Thread.Sleep(20)
				Loop
				Return startComp.Result
			End If
			
			Return True
		End Function
		
		Private Class CompilerStarter
			Private Dim Parent As MainForm
			Private Dim FileToCompile As String
			Private Dim ExtraParams As String
			Private Dim CompileMode As Integer
			
			Private Dim pCompleted As Boolean
			Private Dim pResult As Boolean
			Public ReadOnly Property Completed As Boolean
				Get
					Return pCompleted
				End Get
			End Property
			Public ReadOnly Property Result As Boolean
				Get
					Return pResult
				End Get
			End Property
			
			Public Sub New(Parent As MainForm, FileToCompile As String, CompileMode As Integer, ExtraParams As String)
				Me.Parent = Parent
				Me.FileToCompile = FileToCompile
				Me.CompileMode = CompileMode
				Me.ExtraParams = ExtraParams
				
				pCompleted = False
			End Sub
			
			Public Sub Start
				pResult = Parent.RunCompiler(FileToCompile, CompileMode, ExtraParams)
				pCompleted = True
			End Sub
			
		End Class
		
		Public Function RunCompiler(AlternateFile As String, CompileMode As Integer, ExtraParams As String) As Boolean
			
			'Call GCBASIC for gcb > asm, and GCASM for asm > hex
			Dim DoCompile As New System.Diagnostics.Process
			DoCompile.StartInfo.FileName = InstallDir + "\gcbasic.exe"
			'DoCompile.StartInfo.WorkingDirectory = InstallDir
			DoCompile.StartInfo.WorkingDirectory = System.Environment.GetEnvironmentVariable("Temp")
			If AlternateFile.IndexOf(" ") <> -1 Then
				AlternateFile = """" + AlternateFile + """"
			End If
			If CompileMode = 0 Then DoCompile.StartInfo.Arguments = AlternateFile + " /S:prefs.dat /NP /P:none /GCGB"
			If CompileMode = 1 Then DoCompile.StartInfo.Arguments = AlternateFile + " /S:prefs.dat /NP /GCGB /P:none /A:GCASM"
			If CompileMode = 2 Then DoCompile.StartInfo.Arguments = AlternateFile + " /S:prefs.dat /NP /GCGB /A:GCASM " + ExtraParams
			
			'MessageBox.Show(DoCompile.StartInfo.Arguments, "Command Line", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1)
			DoCompile.Start()
			
			DoCompile.WaitForExit(90000)
			
			'Check for errors.txt, and exit sub if found
			Dim FileCheck As New IO.FileInfo(System.Environment.GetEnvironmentVariable("Temp") + "\errors.txt")
	   		If FileCheck.Exists Then
	   			'Show error list
	   			Me.BeginInvoke(CallDisplayErrorList)
				Return False
	   		End If
	   		
	   		Return True
		End Function
		
		'Need to use a delegate like this to avoid error when updating UI from compiler thread
		Private Dim CallDisplayErrorList As New MethodInvoker(AddressOf DisplayErrorList)
		Private Sub DisplayErrorList
			Dim ShowError As ErrorListBox
			ShowError = New ErrorListBox(GetCurrentProgram, System.Environment.GetEnvironmentVariable("Temp") + "/errors.txt")
			ShowError.Show
		End Sub
		
		Private Sub ProgramLibsClick(sender As System.Object, e As System.EventArgs)
			If GetCurrentContainer Is Nothing Then Exit Sub
			
			'Get new list
			dim LibChoose As New LibraryWindow(GetCurrentProgram)
			LibChoose.ShowDialog()
		End Sub
		
		Private Sub ToolsASMClick(sender As System.Object, e As System.EventArgs)
			
			If GetCurrentContainer Is Nothing Then Exit Sub
			
			'Do compile
			Compile(0,, True)
				
			'Open .asm
			Dim ViewASMCode As New System.Diagnostics.Process
			ViewASMCode.StartInfo.FileName = "notepad.exe"
			ViewASMCode.StartInfo.Arguments = GetCurrentProgram.FindGeneratedFile(".asm")
			ViewASMCode.Start()
			
		End Sub
		
		Private Sub ToolsCompileClick(sender As System.Object, e As System.EventArgs)
			
			Dim CompileResult As Boolean = Compile(1)
			'Dim CompilerThread As New Thread(AddressOf Me.CompileProgramNow)
			'CompilerThread.Start
			
		End Sub
		
		Private Sub ToolsDownloadClick(sender As System.Object, e As System.EventArgs)
			'DownloadProgramNow
			'Dim CompilerThread As New Thread(AddressOf Me.DownloadProgramNow)
			'CompilerThread.Start
			StartProgramDownload
			
		End Sub
		
		'Method to allow other classes to initiate download
		Public Shared Sub StartProgramDownload
			MainformInstance.DownloadProgramNow
		End Sub
		
		Private Sub DownloadProgramNow
			If GetCurrentContainer Is Nothing Then Exit Sub
			
			'Compile and download to chip if successful
			 Compile(2)
		End Sub
		
		Private Sub CompileProgramNow
			'Compile program
			'Provides a way to call compile with no arguments
			
			Dim CompileResult As Boolean = Compile(1)
			
		End Sub
		
		Private Sub HelpContentsClick(sender As System.Object, e As System.EventArgs)
			Help.ShowHelp(Me, InstallDir + "\Docs\Great Cow Graphical BASIC.chm")
			
		End Sub
		
		Private Sub HelpIndexClick(sender As System.Object, e As System.EventArgs)
			Help.ShowHelpIndex(Me, InstallDir + "\Docs\Great Cow Graphical BASIC.chm")
			
		End Sub
		
		Private Sub HelpIconClick(sender As System.Object, e As System.EventArgs)
			'MessageBox.Show(fProgWindow(CurrentWindow).GetCurrentCommand, "Message", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1)
			Dim fProgContainer As ProgramContainer = GetCurrentContainer
			If fProgContainer Is Nothing Then Exit Sub
			
			Dim CurrentCommand As String = fProgContainer.GetCurrentCommand.Trim.ToLower
			'If CurrentCommand.IndexOf("(") <> -1 Then CurrentCommand = CurrentCommand.Substring(0, CurrentCommand.IndexOf("(")).Trim
			'If CurrentCommand.IndexOf(" ") <> -1 Then CurrentCommand = CurrentCommand.Substring(0, CurrentCommand.IndexOf(" ")).Trim
			
			ShowCommandHelp(CurrentCommand)
		End Sub
		
		Public Sub ShowCommandHelp(CurrentCommand As String)	
			
			Dim CommandIndex As Integer = 0
			Dim TempData As String = ""
			Dim HelpArticle As String = ""
			
			If CurrentCommand = "mainicon" Then
				HelpArticle = ""
			Else
				TempData = GetCurrentProgram.GetCommandData(CurrentCommand).ToLower
				
				If TempData.IndexOf("command:") <> -1 Or TempData.IndexOf("match:") <> -1 Then
					CommandIndex = TempData.Substring(TempData.IndexOf(":") + 1).Trim.ToLower
					With CommandList(CommandIndex)
						If .HelpPage = "" Then
							CurrentCommand = .Syntax
							If CurrentCommand.IndexOf("(") <> -1 Then CurrentCommand = CurrentCommand.Substring(0, CurrentCommand.IndexOf("(")).Trim
							If CurrentCommand.IndexOf(" ") <> -1 Then CurrentCommand = CurrentCommand.Substring(0, CurrentCommand.IndexOf(" ")).Trim
							HelpArticle = CurrentCommand + ".htm"
						Else
							HelpArticle = .HelpPage
						End If
					End With
				Else If TempData.IndexOf("label:") <> -1 Then
					HelpArticle = "goto.htm"
				Else If TempData.IndexOf("comment:") <> -1 Then
					HelpArticle = "comment.htm"
				End If
				
			End If
			
			ShowHelpArticle(HelpArticle)
			
		End Sub
		
		Public Sub ShowHelpArticle(HelpArticle As String)
			If HelpArticle = "" Then Exit Sub
				
			Help.ShowHelp(Me, InstallDir + "\Docs\Great Cow Graphical BASIC.chm", HelpArticle)
		End Sub
		
		Private Sub IconListPanelMouseEnter(sender As System.Object, e As System.EventArgs)
			'IconListPanel.Select
		End Sub
		
		Private Sub ProgramInterruptsClick(sender As System.Object, e As System.EventArgs)
			Dim CurrProgram As GCBProgram
			CurrProgram = GetCurrentProgram
			If Not CurrProgram Is Nothing Then
				'Check that chip model is defined, can't show interrupts otherwise
				CurrProgram.CheckChipData
				If CurrProgram.ChipData Is Nothing Then
					Dim result As DialogResult = MessageBox.Show("A chip model must be selected before interrupts can be defined. Would you like to do this now?", "Great Cow Graphical BASIC", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
					If result = DialogResult.Yes Then
						
						dim fChipConf As New ChipSettings(CurrProgram)
						fChipConf.ShowDialog()
						'Make sure that user has selected a chip model
						CurrProgram.CheckChipData
						If CurrProgram.ChipData Is Nothing Then
							Exit Sub
						End If
						
					Else
						Exit Sub
					End If
				End If
				
				'Show interrupt editing window
				Dim fInterruptWindow As New InterruptWindow(CurrProgram)
				fInterruptWindow.ShowDialog
			End If
			
		End Sub
		
		Private Sub AddCommandButtonClick(sender As System.Object, e As System.EventArgs)
			Dim fProgContainer As ProgramContainer = GetCurrentContainer
			If fProgContainer Is Nothing Then Exit Sub
			
			fProgContainer.AddAfterSelected(Me.EnterCommand.Text)
			Me.EnterCommand.SelectAll()
		End Sub
		
		Private Sub ViewIconsClick(sender As System.Object, e As System.EventArgs)
			'Change program display to icons
			GetCurrentContainer.SetIconView
		End Sub
		
		Private Sub ViewTextClick(sender As System.Object, e As System.EventArgs)
			'Change program display to text
			GetCurrentContainer.SetTextView
		End Sub
		
		Private Sub ProgramAdvancedClick(sender As System.Object, e As System.EventArgs)
			Dim CurrProgram As GCBProgram
			CurrProgram = GetCurrentProgram
			If Not CurrProgram Is Nothing Then
				
				Dim fAdvancedSettings As New AdvancedSettings(CurrProgram)
				fAdvancedSettings.ShowDialog
				
			End If
		End Sub
		
		
		Private Sub ViewZoomInClick(sender As Object, e As EventArgs)
			Me.GetCurrentContainer.Program.Editor.ZoomIn
		End Sub
		
		Private Sub ViewZoomOutClick(sender As Object, e As EventArgs)
			Me.GetCurrentContainer.Program.Editor.ZoomOut
		End Sub
		
		Private Sub ViewZoomResetClick(sender As Object, e As EventArgs)
			Me.GetCurrentContainer.Program.Editor.ZoomReset
		End Sub
		
		Sub FileOpenExampleClick(sender As Object, e As EventArgs)
			'Activated through double click while experimental
			'(Change to click when finalised)
			
			Dim chooser As New ExampleChooser(InstallDir + "/demos/new")
			chooser.ShowDialog
			
		End Sub
	End Class
''End Namespace


