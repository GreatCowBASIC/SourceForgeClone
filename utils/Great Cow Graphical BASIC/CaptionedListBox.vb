'
' Created by SharpDevelop.
' User: Hugh
' Date: 16/12/2009
' Time: 11:03 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.ComponentModel
Imports System.Windows.Forms
Imports System.Collections.Generic

''Namespace Great_Cow_Graphical_BASIC
	
	Public Class CaptionedListItem
		Public Title As String
		Public Caption As String
		Public ID As String
		Public Data As Object
		
		Public Sub New(ID As String, Title As String, Caption As String)
			Me.ID = ID
			Me.Title = Title
			Me.Caption = Caption
		End Sub
		
		Public Sub New(ID As String, Title As String, Caption As String, Data As Object)
			Me.ID = ID
			Me.Title = Title
			Me.Caption = Caption
			Me.Data = Data
		End Sub
	End Class
	
	Public Class CaptionedListBox
		Inherits System.Windows.Forms.UserControl
		Private listDisplay As System.Windows.Forms.ListBox
		
		'List to store items
		Private Dim listItems As List(Of CaptionedListItem)
		
		'Fonts
		Private Dim TitleFont, CaptionFont As Font
		
		Public Property ListFont As Font
			Get
				Return listDisplay.Font
			End Get
			Set
				listDisplay.Font = Value
			End Set
		End Property
		
		Public Overloads Property BorderStyle As BorderStyle
			Get
				Return listDisplay.BorderStyle
			End Get
			Set
				listDisplay.BorderStyle = Value
			End Set
		End Property
		
		'Events
		Public Event SelectedIndexChanged(sender As System.Object, e As System.EventArgs)
		Public Event ListDoubleClick
		
		Public Sub New()
			MyBase.New
			' Must be called for initialization
			Me.InitializeComponent
			
			'Create new item list
			listItems = New List(Of CaptionedListItem)
			
			'Set fonts
			CaptionFont = listDisplay.Font
			TitleFont = New Font(CaptionFont, FontStyle.Bold)
			
		End Sub
		
		#Region " Windows Forms Designer generated code "
		' This method is required for Windows Forms designer support.
		' Do not change the method contents inside the source code editor. The Forms designer might
		' not be able to load this method if it was changed manually.
		Private Sub InitializeComponent()
			Me.listDisplay = New System.Windows.Forms.ListBox
			Me.SuspendLayout
			'
			'listDisplay
			'
			Me.listDisplay.Dock = System.Windows.Forms.DockStyle.Fill
			Me.listDisplay.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable
			Me.listDisplay.IntegralHeight = false
			Me.listDisplay.Location = New System.Drawing.Point(0, 0)
			Me.listDisplay.Name = "listDisplay"
			Me.listDisplay.Size = New System.Drawing.Size(150, 150)
			Me.listDisplay.TabIndex = 0
			AddHandler Me.listDisplay.DoubleClick, AddressOf Me.ListDisplayDoubleClick
			AddHandler Me.listDisplay.MeasureItem, AddressOf Me.ListDisplayMeasureItem
			AddHandler Me.listDisplay.DrawItem, AddressOf Me.ListDisplayDrawItem
			AddHandler Me.listDisplay.SelectedIndexChanged, AddressOf Me.ListDisplaySelectedIndexChanged
			'
			'CaptionedListBox
			'
			Me.Controls.Add(Me.listDisplay)
			Me.Name = "CaptionedListBox"
			AddHandler Resize, AddressOf Me.CaptionedListBoxResize
			Me.ResumeLayout(false)
		End Sub
		#End Region
		
		Private Sub ListDisplayDrawItem(sender As System.Object, e As System.Windows.Forms.DrawItemEventArgs)
			'Draws a list item
			
			Dim g As Graphics = e.Graphics
			Dim myItem As CaptionedListItem
			Dim splitCaption As String
			Dim locX, locY As Integer
			
			'Get item
			If e.Index = -1 Then Exit Sub
			Try
				myItem = listItems(e.Index)
			Catch
				myItem = New CaptionedListItem("ERROR", "Error", "List contains bad item")
			End Try
			
			'Split caption to fit
			splitCaption = LowLevel.SplitToFit(myItem.Caption, CaptionFont, g, listDisplay.Width - 4)
			
			'Check heights
			Dim TitleHeight, CaptionHeight As Integer
			TitleHeight = g.MeasureString(myItem.Title, TitleFont).Height
			CaptionHeight = g.MeasureString(splitCaption, CaptionFont).Height
			
			'Get brush
			Dim b As Brush = New SolidBrush(e.ForeColor)
			
			'Get location
			locX = e.Bounds.Location.X
			locY = e.Bounds.Location.Y
			
			'Draw
			e.DrawBackground
			g.DrawString(myItem.Title, TitleFont, b, locX + 4, locY + 4)
			g.DrawString(splitCaption, CaptionFont, b, locX + 4, locY + 8 + TitleHeight)
			e.DrawFocusRectangle
			
		End Sub
		
		
		Private Sub ListDisplayMeasureItem(sender As System.Object, e As System.Windows.Forms.MeasureItemEventArgs)
			
			Dim myItem As CaptionedListItem
			Dim g As Graphics = e.Graphics
			Dim splitCaption As String
			
			'Get item
			Try
				myItem = listItems(e.Index)
			Catch
				myItem = New CaptionedListItem("ERROR", "Error", "List contains bad item")
			End Try
			
			'Split caption to fit
			splitCaption = LowLevel.SplitToFit(myItem.Caption, CaptionFont, g, listDisplay.Width - 4)
			
			'Check heights
			Dim TitleHeight, CaptionHeight As Integer
			TitleHeight = g.MeasureString(myItem.Title, TitleFont).Height
			CaptionHeight = g.MeasureString(splitCaption, CaptionFont).Height
			
			e.ItemHeight = TitleHeight + CaptionHeight + 12
			e.ItemWidth = listDisplay.Width
			
		End Sub
		
		Public Sub Add(newItem As CaptionedListItem)
			'Add a new item to the list
			listItems.add(newItem)
			listDisplay.Items.Add(newItem)
		End Sub
		
		Public Sub Remove(RemoveID As String)
			Dim SearchPos As Integer
			Dim CurrItem As CaptionedListItem
			
			For SearchPos = 0 To listItems.Count - 1
				CurrItem = listItems(SearchPos)
				If CurrItem.ID = RemoveID Then
					listItems.Remove(CurrItem)
					listDisplay.Items.RemoveAt(SearchPos)
					Exit For
				End If
			Next
			
		End Sub
		
		Public Sub Clear
			'Remove all items
			listItems.Clear
			listDisplay.Items.Clear
		End Sub
		
		Public ReadOnly Property Count As Integer
			Get
				Return listDisplay.Items.Count
			End Get
		End Property
		
		Public ReadOnly Property FirstID As String
			Get
				If listItems.Count = 0 Then
					Return ""
				Else
					Dim firstItem As CaptionedListItem = listItems(0)
					Return firstItem.ID
				End If
			End Get
		End Property
		
		Public Property SelectedIndex As Integer
			Get
				Return listDisplay.SelectedIndex
			End Get
			Set
				listDisplay.SelectedIndex = Value
			End Set
		End Property
		
		Public Property SelectedID As String
			'Gets/sets the currently selected item with a particular ID
			Get
				'Return ID or empty string if no selection
				If listDisplay.SelectedIndex = -1 Then
					Return ""
				Else
					Dim OutItem As CaptionedListItem = listItems(listDisplay.SelectedIndex)
					Return OutItem.ID
				End If
			End Get
			
			Set
				'Find item with ID and select it
				listDisplay.SelectedIndex = -1
				
				Dim searchPos As Integer
				For searchPos = 0 To listItems.Count - 1
					If listItems(searchPos).ID.ToLower = Value.ToLower Then
						listDisplay.SelectedIndex = searchPos
						Exit For
					End If
				Next
				
			End Set
		End Property
		
		Public Property SelectedObject As Object
			'Gets/sets the currently selected item with a particular ID
			Get
				'Return ID or empty string if no selection
				If listDisplay.SelectedIndex = -1 Then
					Return Nothing
				Else
					Dim OutItem As CaptionedListItem = listItems(listDisplay.SelectedIndex)
					Return OutItem.Data
				End If
			End Get
			
			Set
				'Find item with ID and select it
				listDisplay.SelectedIndex = -1
				
				Dim searchPos As Integer
				For searchPos = 0 To listItems.Count - 1
					If listItems(searchPos).Data.Equals(Value) Then
						listDisplay.SelectedIndex = searchPos
						Exit For
					End If
				Next
				
			End Set
		End Property
		
		Private Sub ListDisplaySelectedIndexChanged(sender As System.Object, e As System.EventArgs)
			RaiseEvent SelectedIndexChanged(sender, e)
		End Sub
		
		Private Sub CaptionedListBoxResize(sender As System.Object, e As System.EventArgs)
			'Need to force .NET to redraw listbox
			'To do this, remove and re-add all items
			'Hackish, but it works!
'			Dim tempList() As Object = New List(Of Object)(ListDisplay.Items).ToArray
'			Dim selected As Integer
'			With listDisplay
'				.BeginUpdate
'				selected = .SelectedIndex
'				.Items.Clear
'				.Items.AddRange(tempList)
'				.SelectedIndex = selected
'				.EndUpdate
'			End With
			
			'Now that GCGB is ported to .NET 2.0, we can just do this. Easy!
			listDisplay.Invalidate
			
		End Sub
		
		Private Sub ListDisplayDoubleClick(sender As System.Object, e As System.EventArgs)
			RaiseEvent ListDoubleClick
			
		End Sub
		
	End Class
''End Namespace
