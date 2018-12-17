'
' Created by SharpDevelop.
' User: Administrator
' Date: 19/11/2007
' Time: 3:12 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.ComponentModel
Imports System.Windows.Forms
Imports System.Collections.Generic

''Namespace Great_Cow_Graphical_BASIC
	
	Public Class ChipDiagram
		Inherits System.Windows.Forms.UserControl
		
		Public Enum Modes
			Pin
			Port
		End Enum
		
		Private Dim DrawMode As Modes
		Private Dim CurrentChip As ChipInfo
		Private Dim OutValue As String
		Private Dim Program As GCBProgram
		
		Public Sub New()
			MyBase.New
			' Must be called for initialization
			Me.InitializeComponent
			
			'Enable double buffering
			SetStyle(ControlStyles.UserPaint Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.DoubleBuffer, true)
			
			DrawMode = Modes.Pin
			SetChip(Nothing)
			
		End Sub
		
		Public Sub Init(ModeIn As Modes, CurrentChip As ChipInfo, Program As GCBProgram)
			
			DrawMode = ModeIn
			Me.Program = Program
			Program.CheckChipData
			SetChip(CurrentChip)
			
		End Sub
		
		Public Property Value As String
			Get
				Return OutValue
			End Get
			Set
				OutValue = Value
				Me.Invalidate
			End Set
		End Property
		
		Private Sub SetChip(NewChip As ChipInfo)
			
			CurrentChip = NewChip
			Me.Invalidate
			
		End Sub
		
		#Region " Windows Forms Designer generated code "
		' This method is required for Windows Forms designer support.
		' Do not change the method contents inside the source code editor. The Forms designer might
		' not be able to load this method if it was changed manually.
		Private Sub InitializeComponent()
			'
			'ChipDiagram
			'
			Me.BackColor = System.Drawing.Color.White
			Me.Name = "ChipDiagram"
			Me.Size = New System.Drawing.Size(320, 264)
			AddHandler Paint, AddressOf Me.ChipDiagramPaint
		End Sub
		#End Region
		
		
		Private Sub ChipDiagramPaint(sender As System.Object, e As System.Windows.Forms.PaintEventArgs)
			'Get Graphics object
			Dim g As Graphics = e.Graphics
			
			'Get pens
			Dim labelFont As New Font("Microsoft Sans Serif", 8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
			Dim selectFont As New Font("Microsoft Sans Serif", 8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
			Dim pinLabelFont As Font
			Dim labelBrush As Brush = Brushes.Black
			Dim labelPen As Pen = Pens.Black
			Dim selectBrush As Brush = Brushes.LightSteelBlue
			Dim pinNameWidth As Integer = Me.Size.Width / 3
			
			'If no chip, just show an error
			If CurrentChip Is Nothing Then
				Dim errMsg As String = "No chip diagram can be shown because a chip model has not been set."
				
				errMsg = LowLevel.SplitToFit(errMsg, labelFont, g, Me.Size.Width - 8)
				Dim labelSize As SizeF = g.MeasureString(errMsg, labelFont)
				g.DrawString(errMsg, labelFont, labelBrush, (Me.Size.Width - labelSize.Width) / 2, (Me.Size.Height - labelSize.Height) / 2)
				Exit Sub
			End If
			
			'If no pins, show error
			If CurrentChip.PinCount = 0 Then
				Dim errMsg As String = "No chip diagram can be shown due to an error in the chip data (pins = 0)"
				
				errMsg = LowLevel.SplitToFit(errMsg, labelFont, g, Me.Size.Width - 8)
				Dim labelSize As SizeF = g.MeasureString(errMsg, labelFont)
				g.DrawString(errMsg, labelFont, labelBrush, (Me.Size.Width - labelSize.Width) / 2, (Me.Size.Height - labelSize.Height) / 2)
				Exit Sub
			End If
			
			'Make sure control is big enough
			If Me.Size.Width < pinNameWidth * 2 + 40 Or Me.Size.Height < 40 Then
				Dim errMsg As String = "Not enough space"
				
				errMsg = LowLevel.SplitToFit(errMsg, labelFont, g, Me.Size.Width - 8)
				Dim labelSize As SizeF = g.MeasureString(errMsg, labelFont)
				g.DrawString(errMsg, labelFont, labelBrush, (Me.Size.Width - labelSize.Width) / 2, (Me.Size.Height - labelSize.Height) / 2)
				Exit Sub
			End If
			
			'Calculate display sizes and locations
			Dim chipHeight, chipWidth As Integer
			Dim pinHeight As Single
			
			Dim chipBodyX, chipBodyY As Integer
			
			'Decide chip height and pin height
			chipHeight = Me.Size.Height - 20
			pinHeight = chipHeight / (CurrentChip.PinCount / 2)
			
			'Decide chip width
			If CurrentChip.PinCount > 28 Then
				'Want width to be about 6 pins
				chipWidth = pinHeight * 6
			Else
				'Want width to be about 3 pins
				chipWidth = pinHeight * 3
			End If
			
			'Scale chip body if it will be too big
			If chipWidth > Me.Size.Width - pinNameWidth * 2
				Dim scaleFactor As Double = chipWidth / (Me.Size.Width - pinNameWidth * 2)
				
				chipWidth /= scaleFactor
				chipHeight /= scaleFactor
				pinHeight /= scaleFactor
				
			End If
			
			'Decide chip body location
			chipBodyX = (Me.Size.Width - chipWidth) / 2
			chipBodyY = (Me.Size.Height - chipHeight) / 2
			
			'Draw chip body
			g.DrawRectangle(labelPen, chipBodyX, chipBodyY, chipWidth, chipHeight)
			
			'Draw chip notch
			If pinHeight > 0 Then
				g.DrawArc(labelPen, chipBodyX + (chipWidth - pinHeight) / 2, _
				          chipBodyY - pinHeight / 2, pinHeight, pinHeight, 0, 180)
			End If
			
			'Draw pin names
			Dim CurrPinNo As Integer
			Dim displayName As String
			'Get first pin on second side of DIP package
			Dim SecondSide As Integer = CurrentChip.PinCount / 2 + 1
			Dim PinPosX, PinPosY, LabelPosX, NamePosX As Integer
			Dim IsSelectedPin As Boolean
			'Draw each pin
			For CurrPinNo = 1 to CurrentChip.PinCount
				
				'Get pin label sizes
				Dim pinNoSize As SizeF = g.MeasureString(CurrPinNo.ToString, labelFont)
				
				'Calculate positions
				'Second side (numbers go up)
				If CurrPinNo >= SecondSide Then
					'Pin
					PinPosX = chipBodyX + chipWidth
					PinPosY = chipBodyY + chipHeight + (SecondSide - CurrPinNo - 1) * pinHeight
					'Number label
					LabelPosX = PinPosX - 5 - pinNoSize.Width
					'Name label
					NamePosX = chipBodyX + chipWidth + pinHeight / 2 + 5
				'First side (numbers go down)
				Else
					'Pin
					PinPosX = chipBodyX - pinHeight / 2
					PinPosY = chipBodyY + (CurrPinNo - 1) * pinHeight
					'Number label
					LabelPosX = chipBodyX + 5
				End If
				
				'Draw the pin
				g.DrawRectangle(labelPen, pinPosX, pinPosY + pinHeight / 4, pinHeight / 2, pinHeight / 2)
				'Draw pin number
				g.DrawString(CurrPinNo.ToString, labelFont, labelBrush, LabelPosX, pinPosY + pinHeight / 4 + (pinHeight / 2 - pinNoSize.Height) / 2)
				
			Next
			'Draw pin labels and show selection
			Dim CurrPin As ChipPinInfo
			For Each CurrPin In CurrentChip.PinList
				With CurrPin
					'Get pin no
					CurrPinNo = .PinNo
					
					'Clear selected flag
					'Will be set if current output pin is selected
					IsSelectedPin = False
					
					'Get pin display name
					'Should show pin name, and any associated constants
					Dim PinNames As New List(Of String)
					
					Dim PinName As String
					
					'If pin is not a port bit, just show its name
					If .PortBit = -1 Then
						PinNames.Add(.PinName)
						
					Else
						PinNames.Add(.PortName + "." + .PortBit.ToString)
						If .PortName.ToUpper + "." + .PortBit.ToString = OutValue.ToUpper Then
							IsSelectedPin = True
						End If
						
						'Add constants that refer to IO pins
						'This program
						For Each Constant As KeyValuePair(Of String, String) In Program.Constants
							'Check if constant refers to a pin
							If LowLevel.IsBitConst(Constant.Value) Then
								PinName = .PortName + "." + .PortBit.ToString
								If Constant.Value.ToLower = PinName.ToLower Then
									'Add pin name to list
									PinNames.Add(Constant.Key)
									If Constant.Key.ToUpper = OutValue.ToUpper Then IsSelectedPin = True
								End If
							End If
						Next
						'Libraries
						Dim CheckLib As GCBLibrary
						For Each CheckLib In Program.Libraries
							For Each Constant As KeyValuePair(Of String, String) In CheckLib.Constants
								If LowLevel.IsBitConst(Constant.Value) Then
									PinName = .PortName + "." + .PortBit.ToString
									If Constant.Value.ToLower = PinName.ToLower Then
										'Add pin name to list
										PinNames.Add(Constant.Key)
										If Constant.Key.ToUpper = OutValue.ToUpper Then IsSelectedPin = True
									End If
								End If
							Next
						Next
					End If
					
					'Set pin label font
					If IsSelectedPin Then
						pinLabelFont = selectFont
					Else
						pinLabelFont = labelFont
					End If
					
					'Combine names
					displayName = ""
					For Each PinName In PinNames
						If displayName = "" Then
							displayName = PinName
						Else
							displayName += (" / " + PinName)
						End If
					Next
					displayName = Lowlevel.SplitToFit(displayName, pinLabelFont, g, pinNameWidth)
					
					'Get pin label sizes
					Dim pinNoSize As SizeF = g.MeasureString(CurrPinNo.ToString, labelFont)
					Dim pinNameSize As SizeF = g.MeasureString(displayName, pinLabelFont)
					
					'Calculate positions
					'Second side (numbers go up)
					If CurrPinNo >= SecondSide Then
						'Pin
						PinPosX = chipBodyX + chipWidth
						PinPosY = chipBodyY + chipHeight + (SecondSide - CurrPinNo - 1) * pinHeight
						'Number label
						LabelPosX = PinPosX - 5 - pinNoSize.Width
						'Name label
						NamePosX = chipBodyX + chipWidth + pinHeight / 2 + 5
					'First side (numbers go down)
					Else
						'Pin
						PinPosX = chipBodyX - pinHeight / 2
						PinPosY = chipBodyY + (CurrPinNo - 1) * pinHeight
						'Number label
						LabelPosX = chipBodyX + 5
						'Name label
						NamePosX = chipBodyX - pinHeight / 2 - 5 - pinNameSize.Width
					End If
					
					'Draw the pin
					If IsSelectedPin Then
						g.FillRectangle(selectBrush, pinPosX, pinPosY + pinHeight / 4, pinHeight / 2, pinHeight / 2)
					End If
					g.DrawRectangle(labelPen, pinPosX, pinPosY + pinHeight / 4, pinHeight / 2, pinHeight / 2)
					
					'Draw pin number
					g.DrawString(CurrPinNo.ToString, labelFont, labelBrush, LabelPosX, pinPosY + pinHeight / 4 + (pinHeight / 2 - pinNoSize.Height) / 2)
					
					'Draw pin display name/s
					g.DrawString(displayName, pinLabelFont, labelBrush, NamePosX, pinPosY + pinHeight / 4 + (pinHeight / 2 - pinNameSize.Height) / 2)
					
				End With
			Next
			
			
		End Sub
		
	End Class
''End Namespace
