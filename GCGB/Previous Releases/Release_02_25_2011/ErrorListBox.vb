'
' Created by SharpDevelop.
' User: Hugh
' Date: 8/02/2009
' Time: 11:16 AM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports System.IO
Imports System.Collections

Namespace Great_Cow_Graphical_BASIC
	
	Public Class ErrorListBox
		Inherits System.Windows.Forms.Form
		Private ErrorList As System.Windows.Forms.ListBox
		
		Private Dim CurrFile As GCBProgram
		Private Dim ErrorFile As String
		Private Dim ErrorLocations As ArrayList
		
		Public Sub New(CurrFile As GCBProgram, ErrorFile As String)
			MyBase.New
			'
			' The Me.InitializeComponent call is required for Windows Forms designer support.
			'
			Me.InitializeComponent
			
			Me.CurrFile = CurrFile
			Me.ErrorFile = ErrorFile
			
			ErrorLocations = New ArrayList
			
			Reload
		End Sub
		
		Public Sub Reload
			Dim TempData, OutMessage, SubName As String
			Dim SubNo, LineNo As Integer
			Dim ResultSection() As String
			
			ErrorList.Items.Clear
			Dim ErrorReader As New StreamReader(ErrorFile)
			Do While ErrorReader.Peek() <> -1
				TempData = ErrorReader.ReadLine().Replace(Microsoft.VisualBasic.Chr(9), " ").Trim
				
				ResultSection = TempData.Split("|", 5)
				
				If ResultSection.Length = 5 Then
					'Error or warning
					If ResultSection(0) = "E" Then
						OutMessage = "Error in "
					Else If ResultSection(0) = "W" Then
						OutMessage = "Warning in "
					End If
					
					'Sub and location
					If CurrFile.Filename.EndsWith("\" + ResultSection(1)) Or CurrFile.Filename.EndsWith("/" + ResultSection(1)) Or ResultSection(1).ToLower = "gcgb.tmp" Then
						Try
							SubNo = ResultSection(2)
							LineNo = ResultSection(3)
						Catch
						End Try
'						If SubNo = 0 Then
'							SubName = "Main"
'						Else
'							SubName = CurrFile.ProgCode(SubNo, 0).Trim
'							SubName = SubName.Substring(SubName.IndexOf(" ") + 1)
'							If SubName.IndexOf("(") <> -1 Then SubName = SubName.Substring(0, SubName.IndexOf("("))
'							SubName = SubName.Trim
'						End If
						SubName = CurrFile.Subroutines(SubNo).Name
						LineNo = CurrFile.GetActualLine(SubNo, LineNo)
						OutMessage += (SubName + " at " + LineNo.ToString + ": " + ResultSection(4))
					Else
						OutMessage += ("library " + ResultSection(1) + ": " + ResultSection(4))
					End If
					
				'Have got error without location
				Else
					SubNo = -1
					LineNo = -1
					OutMessage = TempData
					
				End If
				ErrorLocations.Add(SubNo.ToString + "|" + LineNo.ToString)
				ErrorList.Items.Add(OutMessage)
			Loop
			     		
			ErrorReader.Close
		End Sub
		
		#Region " Windows Forms Designer generated code "
		' This method is required for Windows Forms designer support.
		' Do not change the method contents inside the source code editor. The Forms designer might
		' not be able to load this method if it was changed manually.
		Private Sub InitializeComponent()
			Me.ErrorList = New System.Windows.Forms.ListBox
			Me.SuspendLayout
			'
			'ErrorList
			'
			Me.ErrorList.Dock = System.Windows.Forms.DockStyle.Fill
			Me.ErrorList.IntegralHeight = false
			Me.ErrorList.Location = New System.Drawing.Point(0, 0)
			Me.ErrorList.Name = "ErrorList"
			Me.ErrorList.Size = New System.Drawing.Size(368, 118)
			Me.ErrorList.TabIndex = 0
			AddHandler Me.ErrorList.DoubleClick, AddressOf Me.ErrorListDoubleClick
			AddHandler Me.ErrorList.SelectedIndexChanged, AddressOf Me.ErrorListSelectedIndexChanged
			'
			'ErrorListBox
			'
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.ClientSize = New System.Drawing.Size(368, 118)
			Me.Controls.Add(Me.ErrorList)
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
			Me.MaximizeBox = false
			Me.MinimizeBox = false
			Me.Name = "ErrorListBox"
			Me.ShowInTaskbar = false
			Me.Text = "Compiler Output"
			Me.TopMost = true
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
		Private Sub ErrorListDoubleClick(sender As System.Object, e As System.EventArgs)
			
			'Get list item
			Dim ErrorListPos As Integer
			ErrorListPos = Me.ErrorList.SelectedIndex
			
			'Get sub and line no
			Dim SubNo, LineNo As Integer
			Dim PosData() As String
			PosData = ErrorLocations.Item(ErrorListPos).Split("|")
			If PosData(0) <> -1 Or PosData(1) <> -1 Then
				SubNo = PosData(0)
				LineNo = PosData(1)
				
				CurrFile.Editor.SetCurrentLocation(SubNo, LineNo)
			End If
		End Sub
		
		Private Sub ErrorListSelectedIndexChanged(sender As System.Object, e As System.EventArgs)
			
			
		End Sub
		
	End Class
End Namespace
