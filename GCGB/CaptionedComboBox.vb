'
' Created by SharpDevelop.
' User: Hugh
' Date: 30/09/2012
' Time: 5:41 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Collections.Generic

Public Partial Class CaptionedComboBox
	Inherits System.Windows.Forms.UserControl
	
	'List to store items
	Private Dim listItems As List(Of CaptionedListItem)
	
	'Fonts
	Private Dim TitleFont, CaptionFont As Font
	
	Public Property ListFont As Font
		Get
			Return comboDisplay.Font
		End Get
		Set
			comboDisplay.Font = Value
		End Set
	End Property
	
	'Events
	Public Event SelectedIndexChanged(sender As System.Object, e As System.EventArgs)
	
	Public Sub New()
		MyBase.New
		' Must be called for initialization
		Me.InitializeComponent
		
		'Create new item list
		listItems = New List(Of CaptionedListItem)
		
		'Set fonts
		CaptionFont = comboDisplay.Font
		TitleFont = New Font(CaptionFont, FontStyle.Bold)
		
		comboDisplay.ItemHeight = Me.Height - 2
		
	End Sub
	
	Private Sub comboDisplayDrawItem(sender As System.Object, e As System.Windows.Forms.DrawItemEventArgs)
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
		splitCaption = LowLevel.SplitToFit(myItem.Caption, CaptionFont, g, comboDisplay.Width - 8)
		
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
	
	
	Private Sub comboDisplayMeasureItem(sender As System.Object, e As System.Windows.Forms.MeasureItemEventArgs)
		
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
		splitCaption = LowLevel.SplitToFit(myItem.Caption, CaptionFont, g, comboDisplay.Width - 8)
		
		'Check heights
		Dim TitleHeight, CaptionHeight As Integer
		TitleHeight = g.MeasureString(myItem.Title, TitleFont).Height
		CaptionHeight = g.MeasureString(splitCaption, CaptionFont).Height
		
		e.ItemHeight = TitleHeight + CaptionHeight + 12
		e.ItemWidth = comboDisplay.Width
		
	End Sub
	
	Public Sub Add(newItem As CaptionedListItem)
		'Add a new item to the list
		listItems.add(newItem)
		comboDisplay.Items.Add(newItem)
	End Sub
	
	Public Sub Remove(RemoveID As String)
		Dim SearchPos As Integer
		Dim CurrItem As CaptionedListItem
		
		For SearchPos = 0 To listItems.Count - 1
			CurrItem = listItems(SearchPos)
			If CurrItem.ID = RemoveID Then
				listItems.Remove(CurrItem)
				comboDisplay.Items.RemoveAt(SearchPos)
				Exit For
			End If
		Next
		
	End Sub
	
	Public Sub Clear
		'Remove all items
		listItems.Clear
		comboDisplay.Items.Clear
	End Sub
	
	Public ReadOnly Property Count As Integer
		Get
			Return comboDisplay.Items.Count
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
			Return comboDisplay.SelectedIndex
		End Get
		Set
			comboDisplay.SelectedIndex = Value
		End Set
	End Property
	
	Public Property SelectedID As String
		'Gets/sets the currently selected item with a particular ID
		Get
			'Return ID or empty string if no selection
			If comboDisplay.SelectedIndex = -1 Then
				Return ""
			Else
				Dim OutItem As CaptionedListItem = listItems(comboDisplay.SelectedIndex)
				Return OutItem.ID
			End If
		End Get
		
		Set
			'Find item with ID and select it
			comboDisplay.SelectedIndex = -1
			
			Dim searchPos As Integer
			For searchPos = 0 To listItems.Count - 1
				If listItems(searchPos).ID.ToLower = Value.ToLower Then
					comboDisplay.SelectedIndex = searchPos
					Exit For
				End If
			Next
			
		End Set
	End Property
	
	Public Property SelectedObject As Object
		'Gets/sets the currently selected item with a particular ID
		Get
			'Return ID or empty string if no selection
			If comboDisplay.SelectedIndex = -1 Then
				Return Nothing
			Else
				Dim OutItem As CaptionedListItem = listItems(comboDisplay.SelectedIndex)
				Return OutItem.Data
			End If
		End Get
		
		Set
			'Find item with ID and select it
			comboDisplay.SelectedIndex = -1
			
			Dim searchPos As Integer
			For searchPos = 0 To listItems.Count - 1
				If listItems(searchPos).Data.Equals(Value) Then
					comboDisplay.SelectedIndex = searchPos
					Exit For
				End If
			Next
			
		End Set
	End Property
	
	Private Sub comboDisplaySelectedIndexChanged(sender As System.Object, e As System.EventArgs)
		RaiseEvent SelectedIndexChanged(sender, e)
	End Sub
	
	Private Sub CaptionedComboBoxResize(sender As System.Object, e As System.EventArgs)
		'Need to change item height to force resize when this control is resized
		If Me.Height > 4 Then
			comboDisplay.ItemHeight = Me.Height - 4
		End If
		
	End Sub
	
End Class

