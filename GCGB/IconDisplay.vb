'
' Created by SharpDevelop.
' User: Hugh
' Date: 13/07/2009
' Time: 11:30 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'

Imports System
Imports System.Drawing
Imports System.ComponentModel
Imports System.Windows.Forms
Imports System.Collections.Generic

''Namespace Great_Cow_Graphical_BASIC
	
	Public Class GCBIcon
		Public Dim Name, TextColour As String
		Public Dim XSize, YSize, Lines As Integer
		Public Dim Line(10) As String
		Public Dim HasJoiner As Boolean = True
		Public Dim TextureImage As Image
		
		Private Dim pTexture As String
		Public Property Texture As String
			Get
				Return pTexture
			End Get
			Set
				pTexture = Value
				TextureImage = Image.FromFile(pTexture)
			End Set
		End Property
		
		Public Sub New(Name As String)
			Me.Name = Name.ToLower
			Lines = 0
		End Sub
		
	End Class
	
	Public Class IconDisplay
		Inherits System.Windows.Forms.UserControl
		
		Private Dim Icon As GCBIcon
		Private Dim Parameters As List(Of String)
		Private Dim HideJoiner As Boolean
		Private Dim IconName As String
		Private Dim CustomLabel As String
		Private Dim pZoom As Single
		Private Dim pFontZoom As Single
		
		Public Sub New(Icon As GCBIcon, Parameters As List(Of String), IconName As String, Optional HideJoiner As Boolean = False, Optional CustomLabel As String = "")
			MyBase.New
			' Must be called for initialization
			Me.InitializeComponent
			
			Me.SetStyle(ControlStyles.DoubleBuffer Or ControlStyles.UserPaint Or ControlStyles.AllPaintingInWmPaint, True)
			
			Me.Icon = Icon
			Me.Parameters = Parameters
			Me.HideJoiner = HideJoiner
			Me.IconName = IconName
			Me.CustomLabel = CustomLabel
			
			Me.pZoom = 1
			Me.pFontZoom = 1
			
			If HideJoiner Then
				Me.Size = New System.Drawing.Size(Icon.XSize, Icon.YSize)
			Else
				Me.Size = New System.Drawing.Size(Icon.XSize, Icon.YSize + 20)
			End If
			
			Me.Name = IconName
			
		End Sub
		
		#Region " Windows Forms Designer generated code "
		' This method is required for Windows Forms designer support.
		' Do not change the method contents inside the source code editor. The Forms designer might
		' not be able to load this method if it was changed manually.
		Private Sub InitializeComponent()
			'
			'IconDisplay
			'
			Me.BackColor = System.Drawing.Color.White
			Me.Name = "IconDisplay"
			Me.Size = New System.Drawing.Size(176, 128)
			AddHandler Paint, AddressOf Me.IconDisplayPaint
		End Sub
		#End Region
		
		Private Dim pIconSelected As Boolean = False
		Public Property IconSelected As Boolean
			Set
				Dim redrawNeeded As Boolean = False
				If pIconSelected <> value Then
					redrawNeeded = True
				End If
				pIconSelected = value
				If redrawNeeded Then
					Me.Invalidate
				End If
			End Set
			Get
				Return pIconSelected
			End Get
		End Property
		
		Public Property Zoom As Single
			Get
				Return pZoom
			End Get
			Set
				pZoom = Value
				pFontZoom = Value
				
				If HideJoiner Then
					Me.Size = New System.Drawing.Size(Icon.XSize * pZoom, Icon.YSize * pZoom)
				Else
					Me.Size = New System.Drawing.Size(Icon.XSize * pZoom, (Icon.YSize + 20) * pZoom)
				End If
				
				Me.Invalidate
			End Set
		End Property
		
		Public Property FontZoom As Single
			Get
				Return pFontZoom
			End Get
			Set
				pFontZoom = Value
				Me.Invalidate
			End Set
		End Property
		
		Public ReadOnly Property ActualX As Integer
			Get
				Return Icon.XSize * pZoom
			End Get
		End Property
		
		Public ReadOnly Property ActualY As Integer
			Get
				Return Icon.YSize * pZoom
			End Get
		End Property
		
		Private Sub IconDisplayPaint(sender As System.Object, e As System.Windows.Forms.PaintEventArgs)
			
			Dim Temp, TempNo, LabelX, LabelY, LabelXSize, LabelYSize As Integer
			Dim TempData, CurrLine As String
			
			Me.SetStyle(ControlStyles.SupportsTransparentBackColor, True)
			Me.BackColor = Color.Transparent
			
			'Get Graphics object
			Dim g As Graphics = e.Graphics
			
			'Show icon texture
			Dim TextureXSize, TextureYSize As Integer
			TextureXSize = Icon.TextureImage.Width
			TextureYSize = Icon.TextureImage.Height
			'g.DrawImage(Icon.TextureImage, Convert.ToInt32((Icon.XSize - TextureXSize) / 2), Convert.ToInt32((Icon.YSize - TextureYSize) / 2))
			g.DrawImage(Icon.TextureImage, 0, 0, (Icon.XSize - 1) * pZoom, (Icon.YSize - 1) * pZoom)
			
			'Show joining arrow
			If Icon.HasJoiner And Not HideJoiner Then
				'g.DrawImage(Image.FromFile(MainForm.InstallDir + MainForm.FilenameJoiner), Convert.ToInt32((Icon.XSize - 20) / 2), Icon.YSize)
				g.DrawImage(MainForm.JoinerArrow, Convert.ToInt32((Icon.XSize - 20) / 2 * pZoom), Icon.YSize * pZoom, MainForm.JoinerArrow.Width * pZoom, MainForm.JoinerArrow.Height * pZoom)
			End If
			
			'Show labels
			If Icon.Lines > 0 Or CustomLabel <> "" Then
				
				'Get brushes for label
				Dim labelFont As New Font("Microsoft Sans Serif", (8 * pFontZoom), System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
				Dim labelBrush As Brush
				Dim labelSize As SizeF
				labelBrush = Brushes.White
				If Icon.TextColour <> "" Then
					If Icon.TextColour.ToLower = "black" Then labelBrush = Brushes.Black
				End If
				
				'If a custom label has been provided, show that instead of standard lines
				'This is used for the icon list in MainForm
				If CustomLabel <> "" Then
					'Calc label location
		 			LabelX = 5 * pZoom
		 			LabelXSize = (Icon.XSize - 2 * LabelX) * pZoom
		 			LabelYSize = (Icon.YSize - 20) * pZoom
		 			LabelY = 10 * pZoom
					
					'Get label text
					CurrLine = CustomLabel
					
					'Write Label
					labelSize = g.MeasureString(CurrLine, labelFont)
					If labelSize.Width > Icon.XSize Then
						Dim splitPos As Integer
						splitPos = CurrLine.Length / 2
						CurrLine = CurrLine.Substring(0, splitPos) + Environment.NewLine + CurrLine.Substring(splitPos)
						labelSize = g.MeasureString(CurrLine, labelFont)
					End If
					g.DrawString(CurrLine, labelFont, labelBrush, LabelX + (LabelXSize - labelSize.Width) / 2, LabelY + (LabelYSize - labelSize.Height) / 2)
					
				'No custom label, so show the proper lines
				Else
					For Temp = 1 To Icon.Lines
			 			If Icon.Line(Temp) <> "" Then
				 			
				 			'Calc label location
				 			LabelX = 5' * pZoom
				 			LabelXSize = (Icon.XSize - 2 * LabelX) * pZoom
				 			LabelYSize = ((Icon.YSize - 20) / Icon.Lines)' * pZoom
				 			LabelY = (10 + (Temp - 1) * LabelYSize) * pZoom
							LabelX *= pZoom
							LabelYSize *= pZoom
							
							'Get label text, and make needed replacements
							CurrLine = Icon.Line(Temp)
							Do While CurrLine.ToUpper.IndexOf("%P") <> -1
								TempData = CurrLine.Substring(CurrLine.ToUpper.IndexOf("%P") + 2)
								TempNo = TempData.Substring(0, TempData.IndexOf("%"))
								TempData = ""
								Try
									If TempNo = 99 Then
										TempData = Parameters(0)
									Else
										TempData = Parameters(TempNo)
									End If
								Catch
								End Try
								CurrLine = CurrLine.Replace("%P" + TempNo.ToString.Trim + "%", TempData)
							Loop
							
							'If label is too long, split into multiple lines
							labelSize = g.MeasureString(CurrLine, labelFont)
							If labelSize.Width > Icon.XSize * pZoom Then
								'Split line to make it fit
								CurrLine = Lowlevel.SplitToFit(CurrLine, labelFont, g, Icon.XSize * pZoom)
								
								'Measure new size
								labelSize = g.MeasureString(CurrLine, labelFont)
							End If
							
							'Draw label with each line centered
							Dim LabelLines() As String = CurrLine.Split(Environment.NewLine)
							Dim LabelLine As Integer
							Dim LineHeight As Single
							Dim LabelLineSize As SizeF
							LineHeight = LabelYSize / LabelLines.Length
							
							For LabelLine = 0 To LabelLines.Length - 1
								'Remove char 10 from start of line (if present)
								If LabelLines(LabelLine).StartsWith(Convert.ToChar(10)) Then
									LabelLines(LabelLine) = LabelLines(LabelLine).Substring(1)
								End If
								
								'Measure new line size
								labelLineSize = g.MeasureString(LabelLines(LabelLine), labelFont)
								
								'Draw
								g.DrawString(LabelLines(LabelLine), labelFont, labelBrush, _
								             LabelX + (LabelXSize - labelLineSize.Width) / 2, _
								             LabelY + LineHeight * LabelLine + (LineHeight - labelLineSize.Height) / 2)
							Next
			 			End If
					Next
					
				End If
				
			End If
			
			'Show selection
			If pIconSelected Then
				Dim rectPen As Pen = Pens.Black
				g.DrawRectangle(rectPen, 0, 0, (Icon.XSize - 1) * pZoom, (Icon.YSize - 1) * pZoom)
			End If
			
		End Sub
		
	End Class
''End Namespace
