'    Graphical LCD routines for the GCBASIC compiler
'    Copyright (C) 2012 - 2014 Hugh Considine and Evan Venn

'    This library is free software; you can redistribute it and/or
'    modify it under the terms of the GNU Lesser General Public
'    License as published by the Free Software Foundation; either
'    version 2.1 of the License, or (at your option) any later version.

'    This library is distributed in the hope that it will be useful,
'    but WITHOUT ANY WARRANTY; without even the implied warranty of
'    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
'    Lesser General Public License for more details.

'    You should have received a copy of the GNU Lesser General Public
'    License along with this library; if not, write to the Free Software
'    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

'Notes:
' Supports KS0108, ST7735 and ST7920 controllers.

'Changes
' 14/10/2012: First version (KS0108 only)
' 29/9/2013: Added ST7735 code (and colour support)
' 14/4/2014: Added ST7920 Support
' 14/4/2014: Fixed KS0108 GLCDCLS CS1/CS2 line set correctly.
' 22/4/2014: Fixed calc method in ST7920 routine
' 22/4/2014: Revised OLD_GLCD_TYPE_ST7920_GRAPHICS_MODE variable name
' 05/5/2014: Revised Line, and added Circle and FilledCircle
' 05/5/2014: Revised KS0108 read sub... timing was incorrect
' 05/5/2014: Revised GLCD to ensure the screen is cleared properly
' 08/5/2014: Revised to comply with documentation requirements
' 09/5/2014: Remove more silly variable names. No functional changes
' 09/5/2014: Fixed circle and line to ensure cross device compatibility
' 11/5/2014: Revided to expose old line drawing routines
' 17/6/2014: Revised to correct error in KS0108 PSET routine.
' 28/6/2104: Revised GLCDDrawString.  Xpos was always 1 extra pixel to right.
' 14/8/2104: Removed GLCDTimeDelay to improve timing and a tidy up.
' 15/10/2014: Adapted for color support for ST7735
'             Added Defines and ST7735Rotation command
' 16/10/2014: Adapted to handle screen rotation and GLCDCLS for ST7735 device.

'Initialisation routine
#startup InitGLCD

'Hardware settings
'Type
'''@hardware All; Controller Type; GLCD_TYPE; "GLCD_TYPE_KS0108" | "GLCD_TYPE_ST7735"
'Parallel lines (KS0108 only)
'''@hardware GLCD_TYPE GLCD_TYPE_KS0108; Data Bus 0; GLCD_DB0; IO_Pin
'''@hardware GLCD_TYPE GLCD_TYPE_KS0108; Data Bus 1; GLCD_DB1; IO_Pin
'''@hardware GLCD_TYPE GLCD_TYPE_KS0108; Data Bus 2; GLCD_DB2; IO_Pin
'''@hardware GLCD_TYPE GLCD_TYPE_KS0108; Data Bus 3; GLCD_DB3; IO_Pin
'''@hardware GLCD_TYPE GLCD_TYPE_KS0108; Data Bus 4; GLCD_DB4; IO_Pin
'''@hardware GLCD_TYPE GLCD_TYPE_KS0108; Data Bus 5; GLCD_DB5; IO_Pin
'''@hardware GLCD_TYPE GLCD_TYPE_KS0108; Data Bus 6; GLCD_DB6; IO_Pin
'''@hardware GLCD_TYPE GLCD_TYPE_KS0108; Data Bus 7; GLCD_DB7; IO_Pin
'Control lines (KS0108 only)
'''@hardware GLCD_TYPE GLCD_TYPE_KS0108; Chip Select 1; GLCD_CS1; IO_Pin
'''@hardware GLCD_TYPE GLCD_TYPE_KS0108; Chip Select 2; GLCD_CS2; IO_Pin
'''@hardware GLCD_TYPE GLCD_TYPE_KS0108; Register Select; GLCD_RS; IO_Pin
'''@hardware GLCD_TYPE GLCD_TYPE_KS0108; Read/Write; GLCD_RW; IO_Pin
'''@hardware GLCD_TYPE GLCD_TYPE_KS0108; Enable; GLCD_ENABLE; IO_Pin

'Serial lines (ST7735 only)
'''@hardware GLCD_TYPE GLCD_TYPE_ST7735; Data/Command; GLCD_DC; IO_Pin
'''@hardware GLCD_TYPE GLCD_TYPE_ST7735; Chip Select; GLCD_CS; IO_Pin
'''@hardware GLCD_TYPE GLCD_TYPE_ST7735; Data In (LCD Out); GLCD_DI; IO_Pin
'''@hardware GLCD_TYPE GLCD_TYPE_ST7735; Data Out (LCD In); GLCD_DO; IO_Pin
'''@hardware GLCD_TYPE GLCD_TYPE_ST7735; Clock; GLCD_SCK; IO_Pin

'Parallel lines (ST7920 only)
'''@hardware GLCD_TYPE GLCD_TYPE_ST7920; Data Port; GLCD_DATA_PORT; IO_Port
'Control lines (ST7920 only)
'''@hardware GLCD_TYPE GLCD_TYPE_ST7920; Register Select; GLCD_RS; IO_Pin
'''@hardware GLCD_TYPE GLCD_TYPE_ST7920; Read/Write; GLCD_RW; IO_Pin
'''@hardware GLCD_TYPE GLCD_TYPE_ST7920; Enable; GLCD_ENABLE; IO_Pin

'''@hardware All; Reset; GLCD_RESET; IO_Pin

'Constants that might need to be set
#define GLCD_TYPE GLCD_TYPE_KS0108
#define GLCD_WIDTH 128
#define GLCD_HEIGHT 160
#define GLCDFontWidth 6


' Circle edge overdraw protection
' #define GLCD_PROTECTOVERRUN

' Constants that might need to be set to support the ST7920
#define GLCD_ROW 	(GLCD_HEIGHT/1)
#define GLCD_COLS 	(GLCD_WIDTH/16)

' A few constants that might need to be set to support the ST7920
' 4.4v = 25, 5.0v 20 - You may need to adjust
#define ST7920ReadDelay 20
  ' if ChipMHz = 4 then 10
  ' if ChipMHz = 8 then 2
  ' if ChipMHz = 16 then 2
  ' if ChipMHz = 32 then 2
#define ST7920WriteDelay 2

'GLCD types
#define GLCD_TYPE_KS0108 1
#define GLCD_TYPE_ST7735 2
#define GLCD_TYPE_ST7920 3

'Pin mappings for ST7735
#define ST7735_DC GLCD_DC
#define ST7735_CS GLCD_CS
#define ST7735_RST GLCD_RESET
#define ST7735_DI GLCD_DI
#define ST7735_DO GLCD_DO
#define ST7735_SCK GLCD_SCK

'Column/row select commands for ST7735
#define ST7735_COLUMN 0x2A
#define ST7735_ROW 0x2B

'Chip select/deselect macros for ST7735
#define ST7735Select Set ST7735_CS Off
#define ST7735Deselect Set ST7735_CS On


' Defines for ST7735
#define INITR_GREENTAB 0x00
#define INITR_REDTAB   0x01
#define INITR_BLACKTAB   0x02

#define ST7735_TFTWIDTH  128
#define ST7735_TFTHEIGHT 160

#define ST7735_NOP 0x00
#define ST7735_SWRESET 0x01
#define ST7735_RDDID 0x04
#define ST7735_RDDST 0x09

#define ST7735_SLPIN  0x10
#define ST7735_SLPOUT  0x11
#define ST7735_PTLON  0x12
#define ST7735_NORON  0x13

#define ST7735_INVOFF 0x20
#define ST7735_INVON 0x21
#define ST7735_DISPOFF 0x28
#define ST7735_DISPON 0x29
#define ST7735_CASET 0x2A
#define ST7735_RASET 0x2B
#define ST7735_RAMWR 0x2C
#define ST7735_RAMRD 0x2E

#define ST7735_COLMOD 0x3A
#define ST7735_MADCTL 0x36


#define ST7735_FRMCTR1 0xB1
#define ST7735_FRMCTR2 0xB2
#define ST7735_FRMCTR3 0xB3
#define ST7735_INVCTR 0xB4
#define ST7735_DISSET5 0xB6

#define ST7735_PWCTR1 0xC0
#define ST7735_PWCTR2 0xC1
#define ST7735_PWCTR3 0xC2
#define ST7735_PWCTR4 0xC3
#define ST7735_PWCTR5 0xC4
#define ST7735_VMCTR1 0xC5

#define ST7735_RDID1 0xDA
#define ST7735_RDID2 0xDB
#define ST7735_RDID3 0xDC
#define ST7735_RDID4 0xDD

#define ST7735_PWCTR6 0xFC

#define ST7735_GMCTRP1 0xE0
#define ST7735_GMCTRN1 0xE1

' Screen rotation
#define ST7735_LANDSCAPE 1
#define ST7735_PORTRAIT_REV 2
#define ST7735_LANDSCAPE_REV 3
#define ST7735_PORTRAIT 4


' Color definitions
#define ST7735_BLACK   0x0000
#define ST7735_BLUE    0xF800
#define ST7735_RED     0x001F
#define ST7735_GREEN   0x07E0
#define ST7735_CYAN    0xFFE0
#define ST7735_MAGENTA 0xF81F
#define ST7735_YELLOW  0x07FF
#define ST7735_WHITE   0xFFFF


' Do not remove.
#define ST7920GLCDEnableCharacterMode ST7920GLCDDisableGraphics
dim GLCD_yordinate as integer



'Foreground and background colours
Dim GLCDBackground As Word
Dim GLCDForeground As Word

'Subs
'''Clears the GLCD screen
Sub GLCDCLS
          ' initialise variable
          GLCD_yordinate = 0

          #if GLCD_TYPE = GLCD_TYPE_ST7920
              if GLCD_TYPE_ST7920_GRAPHICS_MODE  = true then
                 ST7920GLCDClearGraphics
                 Old_ST7920_GRAPHICS_MODE = true
                 ST7920GLCDDisableGraphics
              end if
              'Clear screen
              SET GLCD_RS OFF
	    ST7920WriteByte(0x01)
              wait 80 ms

              'Move to start of visible DDRAM
               SET GLCD_RS OFF
	    ST7920WriteByte(0x80)
              wait 80 us
              if Old_ST7920_GRAPHICS_MODE  = true then
                 GLCD_TYPE_ST7920_GRAPHICS_MODE = true
                  ST7920WriteCommand(0x36)'
                  GLCD_TYPE_ST7920_GRAPHICS_MODE  = true
              end if

          #endif


	#if GLCD_TYPE = GLCD_TYPE_KS0108
		' fix for  not clearing screen
                    Set GLCD_CS1 On
                    Set GLCD_CS2 Off
                    for GLCD_Count = 1 to 2

                        For CurrPage = 0 to 7
                                  'Set page
                                  Set GLCD_RS Off

                                  GLCDWriteByte b'10111000' Or CurrPage
    			
                                  'Clear columns
                                  For CurrCol = 0 to 63
                                            'Select column
                                            Set GLCD_RS Off
                                            GLCDWriteByte 64 Or CurrCol
                                            'Clear
                                            Set GLCD_RS On
                                            GLCDWriteByte 0
                                  Next
                        Next

                    Set GLCD_CS1 Off
                    Set GLCD_CS2 On
                    next



                    Set GLCD_CS1 OFF
                    Set GLCD_CS2 Off
	#endif
	
	#if GLCD_TYPE = GLCD_TYPE_ST7735
		ST7735SetAddress ST7735_COLUMN, 0, ST7735_GLCD_WIDTH
		ST7735SetAddress ST7735_ROW, 0, ST7735_GLCD_HEIGHT
		ST7735SendCommand 0x2C
		Repeat [word]ST7735_GLCD_WIDTH * ST7735_GLCD_HEIGHT
			ST7735SendWord GLCDBackground
		End Repeat
	#endif
End Sub

'''Displays a message
'''@param PrintLocX X coordinate for message
'''@param PrintLocY Y coordinate for message
'''@param PrintData Message to display
Sub GLCDPrint(In PrintLocX, In PrintLocY, in LCDPrintData as string )

          PrintLen = LCDPrintData(0)
	If PrintLen = 0 Then Exit Sub

          #IF GLCD_TYPE = GLCD_TYPE_ST7920
                ' Is the device in GRAPHICS mode?
                if GLCD_TYPE_ST7920_GRAPHICS_MODE = false then
'                   ST7920Locate PrintLocX, PrintLocY
                   ' see SUB ST7920Locate, repeated here to reduce stack
                   	select case  PrintLocY
                         case 0
                              col = PrintLocX OR 0x80

                         case 1
                              col = PrintLocX OR 0x90

                         case 2
                              col = PrintLocX OR 0x88

                         case 3
                              col = PrintLocX OR 0x98

                         case else
                              col = PrintLocX OR 0x80
                      end select
'                      ST7920WriteCommand ( col)
                       SET GLCD_RS OFF
	             ST7920WriteByte( col )

                else
                   GLCDDrawString PrintLocX, PrintLocY, LCDPrintData
                   exit sub
                end if

                Set GLCD_RS On
                'Write Data
                For SysLCDPrintTemp = 1 To PrintLen
                     ST7920WriteByte ( LCDPrintData(SysLCDPrintTemp) )
                Next
                Exit Sub
          #endif


	GLCDPrintLoc = PrintLocX
	'Write Data
	For SysPrintTemp = 1 To PrintLen
		GLCDDrawChar GLCDPrintLoc, PrintLocY, LCDPrintData(SysPrintTemp)
		GLCDPrintLoc += 6
	Next
End Sub

'''Displays a number
'''@param PrintLocX X coordinate for message
'''@param PrintLocY Y coordinate for message
'''@param LCDValue Number to display
Sub GLCDPrint(In PrintLocX, In PrintLocY, In LCDValue As Long)
	Dim SysCalcTempA As Long
	Dim SysPrintBuffer(10)
	SysPrintBuffLen = 0
	
	Do
		'Divide number by 10, remainder into buffer
		SysPrintBuffLen += 1
		SysPrintBuffer(SysPrintBuffLen) = LCDValue % 10
		LCDValue = SysCalcTempA
	Loop While LCDValue <> 0
	
	'Display
	GLCDPrintLoc = PrintLocX
	For SysPrintTemp = SysPrintBuffLen To 1 Step -1
		GLCDDrawChar GLCDPrintLoc, PrintLocY, SysPrintBuffer(SysPrintTemp) + 48
		GLCDPrintLoc += GLCDFontWidth
	Next
	
End Sub



'''Draws a string at the specified location on the ST7920 GLCD
'''@param StringLocX X coordinate for message
'''@param CharLocY Y coordinate for message
'''@param Chars String to display
'''@param LineColour Line Color, either 1 or 0
Sub GLCDDrawString( In StringLocX, In CharLocY, In Chars as string, Optional In LineColour as word = GLCDForeground )
    for xchar = 1 to Chars(0)
      ' June 2014
      ' Corrected error X calcaluation. It was adding an Extra 1!
      TargetCharCol = StringLocX + ((xchar*GLCDFontWidth)-GLCDFontWidth)
      GLCDDrawChar TargetCharCol , CharLocY , Chars(xchar), LineColour
    next
end sub

'''Draws a character at the specified location on the ST7920 GLCD
'''@param StringLocX X coordinate for message
'''@param CharLocY Y coordinate for message
'''@param Chars String to display
'''@param LineColour Line Color, either 1 or 0
Sub GLCDDrawChar(In CharLocX, In CharLocY, In CharCode, Optional In LineColour as word = GLCDForeground )


	'CharCode needs to have 16 subtracted, table starts at char 16 not char 0
	CharCode -= 15

	'invert colors if required

          if LineColour <> GLCDForeground and GLCD_TYPE <> GLCD_TYPE_ST7735 then
            'Inverted Colours
            GLCDBackground = 1
            GLCDForeground = 0
          end if

	
          'Need to read characters from CharColn (n = 0:7) tables
	'(First 3, ie 0:2 are blank, so can ignore)
	For CurrCharCol = 1 to 5
		Select Case CurrCharCol
			Case 1: ReadTable GLCDCharCol3, CharCode, CurrCharVal
			Case 2: ReadTable GLCDCharCol4, CharCode, CurrCharVal
			Case 3: ReadTable GLCDCharCol5, CharCode, CurrCharVal
			Case 4: ReadTable GLCDCharCol6, CharCode, CurrCharVal
			Case 5: ReadTable GLCDCharCol7, CharCode, CurrCharVal
		End Select
		For CurrCharRow = 1 to 8
			If CurrCharVal.0 = 0 Then
				PSet CharLocX + CurrCharCol, CharLocY + CurrCharRow, GLCDBackground
			Else
				PSet CharLocX + CurrCharCol, CharLocY + CurrCharRow, GLCDForeground
			End If
			Rotate CurrCharVal Right
		Next
	Next
          'Colours
          'Restore colors
          'Colours
          if GLCD_TYPE <> GLCD_TYPE_ST7735 then
            GLCDBackground = 0
            GLCDForeground = 1
          end if
End Sub

'''Draws a box on the GLCD screen
'''@param LineX1 Top left corner X location
'''@param LineY1 Top left corner Y location
'''@param LineX2 Bottom right corner X location
'''@param LineY2 Bottom right corner Y location
'''@param LineColour Colour of box border (0 = erase, 1 = draw, default is 1)
#define GLCDBox Box
Sub Box(In LineX1, In LineY1, In LineX2, In LineY2, Optional In LineColour As Word = GLCDForeground)
	'Make sure that starting point (1) is always less than end point (2)
	If LineX1 > LineX2 Then
		GLCDTemp = LineX1
		LineX1 = LineX2
		LineX2 = GLCDTemp
	End If
	If LineY1 > LineY2 Then
		GLCDTemp = LineY1
		LineY1 = LineY2
		LineY2 = GLCDTemp
	End If
	
	'Draw lines going across
	For DrawLine = LineX1 To LineX2
		PSet DrawLine, LineY1, LineColour
		PSet DrawLine, LineY2, LineColour
	Next
	
	'Draw lines going down
	For DrawLine = LineY1 To LineY2
		PSet LineX1, DrawLine, LineColour
		PSet LineX2, DrawLine, LineColour
	Next
End Sub

'''Draws a filled box on the GLCD screen
'''@param LineX1 Top left corner X location
'''@param LineY1 Top left corner Y location
'''@param LineX2 Bottom right corner X location
'''@param LineY2 Bottom right corner Y location
'''@param LineColour Colour of box (0 = erase, 1 = draw, default is 1)
#define GCLDFilledBox FilledBox
Sub FilledBox(In LineX1, In LineY1, In LineX2, In LineY2, Optional In LineColour As Word = GLCDForeground)

	'Make sure that starting point (1) is always less than end point (2)
	If LineX1 > LineX2 Then
		GLCDTemp = LineX1
		LineX1 = LineX2
		LineX2 = GLCDTemp
	End If
	If LineY1 > LineY2 Then
		GLCDTemp = LineY1
		LineY1 = LineY2
		LineY2 = GLCDTemp
	End If
	
          #if GLCD_TYPE = GLCD_TYPE_ST7920
		'Draw lines going across
		For DrawLine = LineX1 To LineX2
			For GLCDTemp = LineY1 To LineY2
				PSet DrawLine, GLCDTemp, LineColour
			Next
		Next
	#endif


	#if GLCD_TYPE = GLCD_TYPE_KS0108
		'Draw lines going across
		For DrawLine = LineX1 To LineX2
			For GLCDTemp = LineY1 To LineY2
				PSet DrawLine, GLCDTemp, LineColour
			Next
		Next
	#endif
	#if GLCD_TYPE = GLCD_TYPE_ST7735
		'Set address window
		ST7735SetAddress ST7735_COLUMN, LineX1, LineX2
		ST7735SetAddress ST7735_ROW, LineY1, LineY2
		'Fill with colour
		Dim GLCDPixelCount As Word
		GLCDPixelCount = (LineX2 - LineX1 + 1) * (LineY2 - LineY1 + 1)
		ST7735SendCommand 0x2C
		Repeat GLCDPixelCount
			ST7735SendWord LineColour
		End Repeat
	#endif
End Sub


'''Draws a circle on the GLCD screen
'''@param Xoffset X point of circle
'''@param Yoffset Y point of circle
'''@param xradius radius of circle
'''@param LineColour Colour of line (0 = blank, 1 = show, default is 1)
'''@param yordinate (optional) rounding
#define GCLDCircle Circle
sub Circle ( in xoffset, in yoffset, in xradius as integer, Optional In LineColour as word = GLCDForeground , Optional In yordinate = GLCD_yordinate)


    dim  radiusErr as Integer
    radiusErr = -(xradius/2)
    Do While xradius >=  yordinate
       Pset ((xoffset + xradius), (yoffset + yordinate), LineColour)
       Pset ((xoffset + yordinate), (yoffset + xradius), LineColour)
       Pset ((xoffset - xradius), (yoffset + yordinate), LineColour)
       Pset ((xoffset - yordinate), (yoffset + xradius), LineColour)
       Pset ((xoffset - xradius), (yoffset - yordinate), LineColour)
       Pset ((xoffset - yordinate), (yoffset - xradius), LineColour)
       Pset ((xoffset + xradius), (yoffset - yordinate), LineColour)
       Pset ((xoffset + yordinate), (yoffset - xradius), LineColour)
       yordinate ++
       If radiusErr < 0 Then
          radiusErr = radiusErr + 2 * yordinate + 1
       else
          xradius --
          radiusErr = radiusErr + 2 * (yordinate - xradius + 1)
       end if
    Loop

end sub


'''Fills a circle on the GLCD screen
'''@param Xoffset X point of circle
'''@param Yoffset Y point of circle
'''@param xradius radius of circle
'''@param LineColour Colour of line (0 = blank, 1 = show, default is 1)
#define GLCDFilledCircle FilledCircle
sub FilledCircle( in xoffset, in yoffset, in xradius, Optional In LineColour as word = GLCDForeground)

    'Circle fill Code is merely a modification of the midpoint
    ' circle algorithem which is an adaption of Bresenham's line algorithm
    ' http://en.wikipedia.org/wiki/Midpoint_circle_algorithm
    ' http://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm

dim ff, ddF_x, ddF_y as integer
ff = 1 - xradius
ddF_x = 1
ddF_y = -2 * xradius
FillCircleXX = 0
FillCircleYY = xradius

	' Fill in the center between the two halves
          y2 = yoffset+xradius
          y1 = yoffset-xradius
	Line( xoffset, y1 , xoffset, y2, LineColour)

	do while (FillCircleXX < FillCircleYY)
             if ff >= 0 then
                FillCircleYY--
                ddF_y += 2
                ff += ddF_y
             end if
	   FillCircleXX++
	   ddF_x += 2
	   ff += ddF_x
             ' Now draw vertical lines between the points on the circle rather than
             ' draw the points of the circle. This draws lines between the
             ' perimeter points on the upper and lower quadrants of the 2 halves of the circle.

	   Line(xoffset+FillCircleXX, yoffset+FillCircleYY, xoffset+FillCircleXX, yoffset-FillCircleYY, LineColour);
	   Line(xoffset-FillCircleXX, yoffset+FillCircleYY, xoffset-FillCircleXX, yoffset-FillCircleYY, LineColour);
	   Line(xoffset+FillCircleYY, yoffset+FillCircleXX, FillCircleYY+xoffset, yoffset-FillCircleXX, LineColour);
	   Line(xoffset-FillCircleYY, yoffset+FillCircleXX, xoffset-FillCircleYY, yoffset-FillCircleXX, LineColour);
  	loop
end sub



'''Draws a line on the GLCD screen
'''@param LineX1 Starting X point of line
'''@param LineY1 Starting Y point of line
'''@param LineX2 Ending X point of line
'''@param LineY2 Ending Y point of line
'''@param LineColour Colour of line (0 = blank, 1 = show, default is 1)
#define GLCDLine Line
Sub Line(In LineX1, In LineY1, In LineX2, In LineY2, Optional In LineColour as word = GLCDForeground)

'   replaced by new line drawing code May 2014

'      #if GLCD_TYPE = GLCD_TYPE_ST7920
'
'          If LineX1 <> LineX2 Then
'             if LineY1 = LineY2 then
'               tempsys = LineX2 - LineX1
'               ST7920LineH LineX1 , LineY1 , tempsys , LineColour
'               exit sub
'             end if
'          end if
'          If Line1Y1 <> LineY2 Then
'              if LineX1 = LineX2 then
'                 tempsys = LineY2 - LineY1
'                 ST7920LineV LineX1 , LineY1 , tempsys , LineColour
'                 exit sub
'              end if
'          end if
'
'      #endif
      ' New line drawing code. May 2014, Evan Venn

      dim LineStepX as integer
      dim LineStepY as integer
      dim LineDiffX, LineDiffY as integer
      dim LineDiffX_x2, LineDiffY_x2 as integer
      dim LineErr as integer



      LineDiffX = 0
      LineDiffY = 0
      LineStepX = 0
      LineStepY = 0
      LineDiffX_x2 = 0
      LineDiffY_x2 = 0
      LineErr = 0


      LineDiffX =  LineX2 -   LineX1
      LineDiffY =  LineY2 -   LineY1

      if (LineDiffX > 0) then
              LineStepX = 1
      else
              LineStepX = -1
      end if

      if (LineDiffY > 0) then
          LineStepY = 1
       else
          LineStepY = -1
      end if

      LineDiffX = LineStepX * LineDiffX
      LineDiffY = LineStepY * LineDiffY

      LineDiffX_x2 = LineDiffX*2
      LineDiffY_x2 = LineDiffY*2
          	
      if ( LineDiffX >= LineDiffY) then

          LineErr = LineDiffY_x2 - LineDiffX

          do while (   LineX1 <>  LineX2 )

              PSet (   LineX1,   LineY1, LineColour )
              LineX1 += LineStepX
              if ( LineErr < 0) then
                  LineErr += LineDiffY_x2
              else
                  LineErr += ( LineDiffY_x2 - LineDiffX_x2 )
                  LineY1 += LineStepY
              end if
          loop

          PSet (   LineX1,   LineY1, LineColour )
      else

          LineErr = LineDiffX_x2 - LineDiffY
          do while (   LineY1 <>  LineY2)
              PSet (   LineX1,   LineY1, LineColour )
              LineY1 += LineStepY
              if ( LineErr < 0) then
                  LineErr += LineDiffX_x2
               else
                  LineErr += ( LineDiffX_x2 - LineDiffY_x2 )
                  LineX1 += LineStepX
              end if
          loop
          PSet (   LineX1,   LineY1, LineColour )

      end if



end sub

'''@hide
sub old_Line(In LineX1, In LineY1, In LineX2, In LineY2, Optional In LineColour = GLCDForeground)

      ' This old code replaced in May 2014.  Use if you have issues with the new code


	'Draw a line using Bresenham's algorithm and calls to PSet
	Dim LineErr, LineErr2 As Integer
	
	'Calculate step sizes and differences between start and end points
	If LineX1 < LineX2 Then
		LineDiffX = LineX2 - LineX1
		LineStepX = 1
	Else
		LineDiffX = LineX1 - LineX2
		LineStepX = 255
	End If
	If LineY1 < LineY2 Then
		LineDiffY = LineY2 - LineY1
		LineStepY = 1
	Else
		LineDiffY = LineY1 - LineY2
		LineStepY = 255
	End If
	'Calculate initial error
	LineErr = LineDiffX - LineDiffY
	
	'Drawing loop
	Do
		'Draw point at current location
		PSet LineX1, LineY1, LineColour
		
		'If at ending point, quit
		If LineX1 = LineX2 Then
			If LineY1 = LineY2 Then Exit Sub
		End If
		
		'Calculate new point to draw
		LineErr2 = LineErr * 2
		If LineErr2 >= LineDiffY Then
			LineErr += LineDiffY
			LineX1 += LineStepX
		End If
		If LineErr2 <= LineDiffX Then
			LineErr += LineDiffX
			LineY1 += LineStepY
		End If
	Loop
	
End Sub

'''Draws a pixel on the GLCD
'''@param GLCDX X coordinate of pixel
'''@param GLCDY Y coordinate of pixel
'''@param GLCDColour State of pixel (0 = erase, 1 = display)
Sub PSet(In GLCDX, In GLCDY, In GLCDColour As Word)



	#if GLCD_TYPE = GLCD_TYPE_ST7920

                    #ifdef GLCD_PROTECTOVERRUN
                           if GLCDY => GLCD_HEIGHT then exit sub
                           if GLCDX => GLCD_WIDTH then exit sub
                    #endif

		'Set pixel at X, Y on LCD to State
		'X is 0 to 127
		'Y is 0 to 63
		'Origin in top left
		dim GLCDDataTempWord as word
                    dim GLCDChangeWord as word
		'Select horizontal address, we address in unit of 16 pixels
                    ' One xUnit equals 16 pixels
                    ' addresses are b15, b14, b13, ..... to b0.  With B15 left justified.

                    ' calculate the xUnit as a word, therefore 16 bits.
                    xUnit = GLCDX/16

                    ' get the word data of the xUnit and yposition
                    dim SysCalcPositionY as byte
                    dim SysCalcPositionX as byte
                    if GLCDY < 32 then
                        ' Y Axis
	              SysCalcPositionY = 0x80 OR GLCDY
                        ' X Axis
                        SysCalcPositionX = 0x80 OR xUnit
                    else
                        ' Y Axis
                        SysCalcPositionY = 0x80 OR (GLCDY-32)
                        ' X Axis
                        SysCalcPositionX = 0x88 OR xUnit
                    end if

                    SET GLCD_RS OFF
                    ST7920WriteByte( SysCalcPositionY )
                    ST7920WriteByte( SysCalcPositionX )

                    ' read data
                    GLCDDataTempWord = ST7920GLCDReadByte
                    GLCDDataTempWord = ST7920GLCDReadByte
                    GLCDDataTempWord  = (GLCDDataTempWord*256) + ST7920GLCDReadByte

		'Select which bit we are to set/clear per pixel
                    GLCDBitNo = 15 - (GLCDX And 15)

                    If GLCDColour.0 = 0 Then
                              GLCDChangeWord = 65534
                              Set C On
                    Else
                              GLCDChangeWord = 1
                              Set C Off
                    End If

		Repeat GLCDBitNo
			Rotate GLCDChangeWord LEFT
		End Repeat
		If GLCDColour.0 = 0 Then
			GLCDDataTempWord =  GLCDDataTempWord  And GLCDChangeWord
		Else
			GLCDDataTempWord =  GLCDDataTempWord  Or  GLCDChangeWord
		End If
		
                    ' Locate again for the write operation
                     SET GLCD_RS OFF
                     ST7920WriteByte( SysCalcPositionY )
                     ST7920WriteByte( SysCalcPositionX )

                     hbyte= GLCDDataTempWord / 256
                     SET GLCD_RS ON
                     ST7920WriteByte(hbyte)
                     ST7920WriteByte( GLCDDataTempWord & 0x00FF )	
	#endif


	#if GLCD_TYPE = GLCD_TYPE_KS0108
		'Set pixel at X, Y on LCD to State
		'X is 0 to 127
		'Y is 0 to 63
		'Origin in top left
		
		'Select screen half
		If GLCDX.6 = Off Then Set GLCD_CS2 On:Set GLCD_CS1 off
		If GLCDX.6 = On Then Set GLCD_CS1 On: GLCDX -= 64: Set GLCD_CS2 off

		'Select page
		CurrPage = GLCDY / 8
		Set GLCD_RS Off
		GLCDWriteByte b'10111000' Or CurrPage
		
		'Select column
		Set GLCD_RS Off
		GLCDWriteByte 64 Or GLCDX
		'Dummy read first
		Set GLCD_RS On
		GLCDDataTemp = GLCDReadByte
		'Read current data
		Set GLCD_RS On
		GLCDDataTemp = GLCDReadByte
		
		'Change data to set/clear pixel
		GLCDBitNo = GLCDY And 7
		If GLCDColour.0 = 0 Then
			GLCDChange = 254
			Set C On
		Else
			GLCDChange = 1
			Set C Off
		End If
		Repeat GLCDBitNo
			Rotate GLCDChange Left
		End Repeat

		If GLCDColour.0 = 0 Then
			GLCDDataTemp = GLCDDataTemp And GLCDChange
		Else
			GLCDDataTemp = GLCDDataTemp Or GLCDChange
		End If
		
		'Select correct column again
		Set GLCD_RS Off
		GLCDWriteByte 64 Or GLCDX
		'Write data back
		Set GLCD_RS On
		GLCDWriteByte GLCDDataTemp
		
		Set GLCD_CS1 Off
		Set GLCD_CS2 Off
	#endif
	
	#if GLCD_TYPE = GLCD_TYPE_ST7735
		ST7735SetAddress ST7735_COLUMN, GLCDX, GLCDX
		ST7735SetAddress ST7735_ROW, GLCDY, GLCDY
		ST7735SendCommand 0x2C
		ST7735SendWord GLCDColour
	#endif
End Sub

'''Write a command then the byte
'''@hide
Sub ST7920WriteCommand ( In GLCDByte )
	SET GLCD_RS OFF
	ST7920WriteByte(GLCDByte)

end sub

'''Write some date then the byte
'''@hide
Sub ST7920WriteData( In GLCDByte as byte )
	SET GLCD_RS ON
	ST7920WriteByte(GLCDByte)

end sub

'  DESCRIPTIONS:
'  Enables Graphics Mode on the GLCD.
'  Make sure the screen is void of text before using this function.
'*******************************************************************************/
'''Enables Graphics Mode on the ST7920 GLCD.
'''  Make sure the screen is void of text before using this function.
sub ST7920GLCDEnableGraphics
    #if GLCD_TYPE = GLCD_TYPE_ST7920
'	ST7920WriteCommand(0x30)'		
'	ST7920WriteCommand(0x34)'			// Switch to extended instruction mode.	
	ST7920WriteCommand(0x36)'
          '// Enable graphics mode.
          GLCD_TYPE_ST7920_GRAPHICS_MODE  = true
     #endif
end sub


'''  Disable Graphics Mode on the ST9720 GLCD and return to text mode.
sub ST7920GLCDDisableGraphics
    #if GLCD_TYPE = GLCD_TYPE_ST7920
'	ST7920WriteByte(0x30)'		
'         ST7920WriteByte(0x34)'			// Switch to extended instruction mode.
	ST7920WriteCommand(0x30)'			// Graphics and extended instruction mode turned off.
          GLCD_TYPE_ST7920_GRAPHICS_MODE  = false
     #endif
end sub

'''Locates the cursor on the GLCD screen
'''@param PrintLocX X column
'''@param PrintLocY Y row
Sub ST7920Locate( In PrintLocX , In PrintLocY )

' DESCRIPTIONS:
' Place a character to the GLCD controller on the specified row and column.
' Due to the design of the ST7920 controller (to accomodate Mandarin and Cyrillic), you must place the text on the column
' according to the numbers above the diagram below:
'
' |--0--|--1--|--2--|...	  ...|--7--|
' +--+--+--+--+--+---------------------+
' |H |e |l |l |o |  ...                | <- row 0 (address 0x80)
' +--+--+--+--+--+---------------------+
' |T |h |i |s |  |i ...				   | <- row 1 (address 0x90)
' +--+--+--+--+--+---------------------+
' |' |' |' |' |' |' ...				   | <- row 2 (address 0x88)
' +--+--+--+--+--+---------------------+
' |- |- |- |- |- |- ...				   | <- row 3 (address 0x98)
' +--+--+--+--+--+---------------------+
'
' Example:
' Writing 'a' onto the 1st column, and 1st row:
' |--0--|--1--|--2--|...	  ...|--7--|
' +--+--+--+--+--+---------------------+
' |  |  |  |  |  |  ...                | <- row 0 (address 0x80)
' +--+--+--+--+--+---------------------+
' |  |  |a |  |  |  ...				   | <- row 1 (address 0x90)
' +--+--+--+--+--+---------------------+
' |  |  |  |  |  |  ...				   | <- row 2 (address 0x88)
' +--+--+--+--+--+---------------------+
' |  |  |  |  |  |  ...				   | <- row 3 (address 0x98)
' +--+--+--+--+--+---------------------+



	select case  PrintLocY
               case 0
                    col = PrintLocX OR 0x80

               case 1
                    col = PrintLocX OR 0x90

               case 2
                    col = PrintLocX OR 0x88

               case 3
                    col = PrintLocX OR 0x98

               case else
                    col = PrintLocX OR 0x80
            end select

            ST7920WriteCommand ( col)



end sub

'''Write the byte
'''@hide
Sub ST7920WriteByte( In GLCDByte )

	#IFNDEF GLCD_NO_RW
		#IFDEF GLCD_IO 4,8
			wait until GLCDReady
			set GLCD_RW OFF 'Write mode
		#ENDIF
	#ENDIF
	
	#IFDEF GLCD_IO 8
		'Set data port to output, and write a value to it
                    #IFNDEF GLCD_LAT
                        DIR GLCD_DATA_PORT OUT
                        GLCD_DATA_PORT = GLCDByte
                        ' retry on failure
                        if    GLCD_DATA_PORT <> GLCDByte then GLCD_DATA_PORT = GLCDByte
                        set GLCD_Enable on
                        wait ST7920WriteDelay us
                        set GLCD_Enable off
                        wait ST7920WriteDelay us
		#ENDIF

                    #IFDEF GLCD_LAT
                        DIR _GLCD_DATA_PORT OUT
                        latd = GLCDByte
                        ' retry on failure
                        if latd <> GLCDByte then latd = GLCDByte
                        set GLCD_Enable on
                        wait ST7920WriteDelay us
                        set GLCD_Enable off
                        wait ST7920WriteDelay us
		#ENDIF
	#ENDIF
	
end sub


'''Tile LCD screen with two datas in Graphic Mode
'''@hide
sub ST7920gTile( fst, snd)


         for GLCD_Count = 0 to ( GLCD_HEIGHT - 1 )
             ST7920gLocate(0, GLCD_Count)
             for GLCD_Count_2 = 0 to ( GLCD_COLS -1 )
                 ST7920WriteData( fst )'SendByte(iDat, fst)
                 ST7920WriteData( snd )'SendByte(iDat, snd)
             next

         next
end sub

'''Tile screen with one data accepts a byte!
'''@hide
sub ST7920Tile( tData as word)

    ST7920GLCDEnableCharacterMode
    for GLCD_Count  = 0 to GLCD_ROW
      ST7920Locate(0 , GLCD_Count)
      for GLCD_Count_2  = 0 to GLCD_COLS
          ST7920WriteData ( tData )
          ST7920WriteData ( tData )
      next
    next
end sub

''' Tile screen with a Chinese Symbol
''' Writes 2 bytes of data into DDRAM to display one 16x16 font.
''' A140H~D75FH are BIG5 code, A1A0H~F7FFH are GB code.
''' accepts a WORD!
''' Tile screen with one data accepts a byte!
'''@hide
sub ST7920cTile( Hanzi as word )

    ST7920GLCDEnableCharacterMode
    for GLCD_Count  = 0 to GLCD_ROW
      ST7920Locate(0 , GLCD_Count)
      for GLCD_Count_2  = 0 to GLCD_COLS
              ST7920WriteData( Hanzi_H )
              ST7920WriteData( Hanzi & 0x00FF )
      next
    next

end sub

'''Set Icon ON/OFF
'''@hide
sub ST7920SetIcon( Addr, dByte)

    ST7920GLCDEnableGraphics
    ST7920WriteCommand ( 0x40 + Addr ) 'Set ICON Address
    ST7920WriteData ( dByte ) ' Set ICON Data

end sub


''' Display Standard Testing Graphics
'''@hide
sub ST7920GraphicTest

    for GLCD_Count_2  = 0 to 3
        ReadTable graphicstestdata, (GLCD_Count_2*2)+1, char1
        ReadTable graphicstestdata, (GLCD_Count_2*2)+2, char2

        ST7920gTile( char1  , char2 )
        wait 200  ms
        ST7920GLCDClearGraphics

    next

    for GLCD_Count_2  = 4 to 7
        for GLCD_Count = 0 to 64 step 2
            ReadTable graphicstestdata, (GLCD_Count_2*2)+1, char1
            ReadTable graphicstestdata, (GLCD_Count_2*2)+2, char2
            ST7920LineHs(0, GLCD_Count, GLCD_COLS - 1 , char1 )
            ST7920LineHs(0, GLCD_Count + 1,  GLCD_COLS - 1, char2 )
        next
        wait 1  s
    next

    ST7920GLCDClearGraphics
end sub



''' Draw a Horizontal Line with special style
'''@hide
sub ST7920LineHs( X0, Y0, xUnits, Style)
    ST7920gLocate(X0, Y0)
    for GLCD_Count_2  = 0 to xUnits
        ST7920WriteData ( Style )
        ST7920WriteData ( Style )
   next
end Sub

table graphicstestdata Store Data
0xFF 'All
0xFF
0x00 'None
0x00
0xAA 'Vertical 1
0xAA
0x55 'Vertical 2
0x55
0xFF 'Horizontal 1
0x00
0x00 'Horizontal 2
0xFF
0xAA 'Stars 1
0x55
0x55 'Stars 2
0xAA
end table

'''Locate the cursor on the display
'''@param xUnitLocate GLCDY
sub ST7920gLocate( xUnitLocate, GLCDY )

   ' 1xUnit = 16 dots, 1yBit = 1 dot

    if GLCDY < 32 then
              ' Y Axis
              ST7920WriteCommand(0x80 OR GLCDY)
              ' X Axis
              ST7920WriteCommand(0x80 OR xUnitLocate )
    else
              ' Y Axis
              ST7920WriteCommand(0x80 OR (GLCDY-32))
              ' X Axis
              ST7920WriteCommand(0x88 OR xUnitLocate )
    end if

end sub


'''Clear the screen by filling the screen with zero bytes.
'''Enable Graphics Mode
'''before calling this function.
'''@param No parameters
sub ST7920GLCDClearGraphics
	' This function performs similar to the FillScreenGraphic but
	' only zeros are sent into the screen instead of data from an array.

          #if GLCD_TYPE <> GLCD_TYPE_ST7920
              GLCDCLS
              exit sub
          #endif

	for PrintLocY = 0 to 63
		if PrintLocY < 32 then
			ST7920WriteCommand(0x80 OR PrintLocY)
    		          ST7920WriteCommand(0x80)
		else
			ST7920WriteCommand(0x80 OR (PrintLocY-32))
    		          ST7920WriteCommand(0x88)
		end if
		for PrintLocX = 0 to 7
			ST7920WriteData(0x00)
			ST7920WriteData(0x00)
	          next          	
	next

end sub

'''@hide
function ST7920gReaddata ( in xUnit, in yBit ) as word

    ST7920gLocate(xUnit, yBit)
    ' Read a word from the display device, read is the Dummy read.
    ST7920gReaddata = ST7920GLCDReadByte

    ' Now read the two bytes to make up the word we are looking for
    ST7920gReaddata = ST7920GLCDReadByte

    ST7920gReaddata  = (ST7920gReaddata*256)+ST7920GLCDReadByte

end Sub

'''Draws a horizontal line on the GLCD screen
'''@param X0 Starting X point of line
'''@param Y0 Starting Y point of line
'''@param xDots number of pixels
'''@param LineColour Colour of line (0 = blank, 1 = show, default is 1)
sub ST7920lineh( in X0, in Y0, in xDots, Optional In GLCDColour As Word = GLCDForeground )

    if xDots = 0 then exit sub

    dim OldData as word
    dim NewData as word
    dim GLCDChangeWord as word
    OldData = 0
    NewData = 0

    for GLCD_Count = 0 to xDots
        xUnit = (GLCD_Count + X0)/16

                 ' set location
                  dim SysCalcPositionY as byte
                  dim SysCalcPositionX as byte
                  if Y0 < 32 then
                      ' Y Axis
                      SysCalcPositionY = 0x80 OR Y0
                      ' X Axis
                      SysCalcPositionX = 0x80 OR xUnit
                  else
                      ' Y Axis
                      SysCalcPositionY = 0x80 OR (Y0-32)
                      ' X Axis
                      SysCalcPositionX = 0x88 OR xUnit
                  end if

                  SET GLCD_RS OFF
                  ST7920WriteByte( SysCalcPositionY )
                  ST7920WriteByte( SysCalcPositionX )

                  ' read data dummy byte, then read the two bytes
                  OldData = ST7920GLCDReadByte
                  OldData = ST7920GLCDReadByte

                  OldData  = (OldData*256) + ST7920GLCDReadByte

		'Select which bit we are to set/clear per pixel
                    GLCDBitNo = 15 - ( (X0 + GLCD_Count ) And 15)

                    If GLCDColour.0 = 0 Then
                              GLCDChangeWord = 65534
                              Set C On
                    Else
                              GLCDChangeWord = 1
                              Set C Off
                    End If

		Repeat GLCDBitNo
			Rotate GLCDChangeWord LEFT
		End Repeat
		If GLCDColour.0 = 0 Then
			NewData =  OldData  And GLCDChangeWord
		Else
			NewData =  OldData  Or  GLCDChangeWord
		End If

                 ' set location
                    SET GLCD_RS OFF
                    ST7920WriteByte( SysCalcPositionY )
                    ST7920WriteByte( SysCalcPositionX )

                    hbyte= NewData / 256
                    SET GLCD_RS ON
                    ST7920WriteByte(hbyte)
                    ST7920WriteByte( NewData & 0x00FF )	

   next
end Sub

'''Draws a vertical line on the GLCD screen
'''@param X0 Starting X point of line
'''@param Y0 Starting Y point of line
'''@param xBits number of pixels
'''@param LineColour Colour of line (0 = blank, 1 = show, default is 1)
sub ST7920linev( in X0, in Y0, in yBits, Optional In GLCDColour As Word = GLCDForeground )

    if yBits = 0 then exit sub

    xUnit = X0/16
    dim OldData as word
    dim NewData as word
    dim GLCDChangeWord as word
    OldData = 0
    NewData = 0

    for GLCD_Count = 0 to  yBits

        yPixel = Y0 + GLCD_Count
       ' set location
        dim SysCalcPositionY as byte
        dim SysCalcPositionX as byte
        if yPixel < 32 then
            ' Y Axis
            SysCalcPositionY = 0x80 OR yPixel
            ' X Axis
            SysCalcPositionX = 0x80 OR xUnit
        else
            ' Y Axis
            SysCalcPositionY = 0x80 OR (yPixel-32)
            ' X Axis
            SysCalcPositionX = 0x88 OR xUnit
        end if

        SET GLCD_RS OFF
        ST7920WriteByte( SysCalcPositionY )
        ST7920WriteByte( SysCalcPositionX )

        ' read data dummy byte, then read the two bytes
        OldData = ST7920GLCDReadByte
        OldData = ST7920GLCDReadByte
        OldData  = (OldData*256) + ST7920GLCDReadByte

        'Select which bit we are to set/clear per pixel
        GLCDBitNo = 15 - ( (X0 ) And 15)

        If GLCDColour.0 = 0 Then
                  GLCDChangeWord = 65534
                  Set C On
        Else
                  GLCDChangeWord = 1
                  Set C Off
        End If

        Repeat GLCDBitNo
                  Rotate GLCDChangeWord LEFT
        End Repeat
        If GLCDColour.0 = 0 Then
                  NewData =  OldData  And GLCDChangeWord
        Else
                  NewData =  OldData  Or  GLCDChangeWord
        End If

     ' set location
        SET GLCD_RS OFF
        ST7920WriteByte( SysCalcPositionY )
        ST7920WriteByte( SysCalcPositionX )

        hbyte= NewData / 256
        SET GLCD_RS ON
        ST7920WriteByte(hbyte)
        ST7920WriteByte( NewData & 0x00FF )
    next
end sub


'''Read byte from LCD
'''@hide
Function ST7920GLCDReadByte
	
	#IFNDEF GLCD_NO_RW
		#IFDEF GLCD_IO 4,8
			wait until GLCDReady
			set GLCD_RW OFF 'Write mode
		#ENDIF
	#ENDIF

	'Set data pin directions
          #IFNDEF GLCD_LAT
            Dir GLCD_DATA_PORT In	
          #endif

          #IFDEF GLCD_LAT
            Dir _GLCD_DATA_PORT In	
          #endif
	
          set GLCD_RW ON
          Set GLCD_RS ON
	Set GLCD_ENABLE On
          Wait ST7920ReadDelay us

          'Get input data
          #IFNDEF GLCD_LAT
	        ST7920GLCDReadByte = GLCD_DATA_PORT
          #ENDIF
          #IFDEF GLCD_LAT
	       ST7920GLCDReadByte = _GLCD_DATA_PORT
          #ENDIF
          Set GLCD_ENABLE Off
End Function


'''Write byte to LCD
'''@hide
Sub GLCDWriteByte (In LCDByte)
	
	Dim GLCDSaveRS As Bit
	Dim GLCDSaveCS2 As Bit
	
	'Wait until LCD is available
	GLCDSaveRS = GLCD_RS
	GLCDSaveCS2 = GLCD_CS2
	If GLCD_CS1 = 1 Then
		GLCD_CS2 = 0
	End If
	Set GLCD_RS Off
	Wait Until GLCDReadByte.7 = Off
	GLCD_RS = GLCDSaveRS
	GLCD_CS2 = GLCDSaveCS2
	
	'Set LCD data direction
	Set GLCD_RW Off
	
	'Set data pin directions
          #IFNDEF GLCD_LAT
              DIR GLCD_DB0 OUT
              DIR GLCD_DB1 OUT
              DIR GLCD_DB2 OUT
              DIR GLCD_DB3 OUT
              DIR GLCD_DB4 OUT
              DIR GLCD_DB5 OUT
              DIR GLCD_DB6 OUT
              DIR GLCD_DB7 OUT
          #ENDIF
          #IFDEF GLCD_LAT
              DIR _GLCD_DB0 OUT
              DIR _GLCD_DB1 OUT
              DIR _GLCD_DB2 OUT
              DIR _GLCD_DB3 OUT
              DIR _GLCD_DB4 OUT
              DIR _GLCD_DB5 OUT
              DIR _GLCD_DB6 OUT
              DIR _GLCD_DB7 OUT
	#ENDIF
	
	'Set output data
	GLCD_DB7 = LCDByte.7
	GLCD_DB6 = LCDByte.6
	GLCD_DB5 = LCDByte.5
	GLCD_DB4 = LCDByte.4
	GLCD_DB3 = LCDByte.3
	GLCD_DB2 = LCDByte.2
	GLCD_DB1 = LCDByte.1
	GLCD_DB0 = LCDByte.0
	
	'Write
	Wait 2 us
	Set GLCD_ENABLE On
	Wait 2 us
	Set GLCD_ENABLE Off
	Wait 2 us
End Sub

'''Read byte from LCD
'''@hide
Function GLCDReadByte
	
	'Set data pin directions
	Dir GLCD_DB7 In
	Dir GLCD_DB6 In
	Dir GLCD_DB5 In
	Dir GLCD_DB4 In
	Dir GLCD_DB3 In
	Dir GLCD_DB2 In
	Dir GLCD_DB1 In
	Dir GLCD_DB0 In
	
	'Set LCD data direction
	Set GLCD_RW On
	
	'Read
	Set GLCD_ENABLE On
	Wait 2 us
	'Get input data
          ' corrected 7/05/2014
	GLCDReadByte.7 = GLCD_DB7
	GLCDReadByte.6 = GLCD_DB6
	GLCDReadByte.5 = GLCD_DB5
	GLCDReadByte.4 = GLCD_DB4
	GLCDReadByte.3 = GLCD_DB3
	GLCDReadByte.2 = GLCD_DB2
	GLCDReadByte.1 = GLCD_DB1
	GLCDReadByte.0 = GLCD_DB0
	Set GLCD_ENABLE Off
	Wait 2 us
	
End Function

'''Send a command to the ST7735 GLCD
'''@param ST7735SendByte Command to send
'''@hide
Sub ST7735SendCommand(In ST7735SendByte)
	Set ST7735_DC Off
	ST7735Select
	SPITransfer ST7735SendByte, ST7735Received
	ST7735Deselect
End Sub

'''Send a data byte to the ST7735 GLCD
'''@param ST7735SendByte Byte to send
'''@hide
Sub ST7735SendData(In ST7735SendByte)
	Set ST7735_DC On
	ST7735Select
	SPITransfer ST7735SendByte, ST7735Received
	ST7735Deselect
End Sub

'''Send a data word (16 bits) to the ST7735 GLCD
'''@param ST7735SendByte Word to send
'''@hide
Sub ST7735SendWord(In ST7735SendByte As Word)
	Set ST7735_DC On
	ST7735Select
	SPITransfer ST7735SendByte_H, ST7735Received
	SPITransfer ST7735SendByte, ST7735Received
	ST7735Deselect
End Sub

'''Set the row or column address range for the ST7735 GLCD
'''@param ST7735AddressType Address Type (ST7735_ROW or ST7735_COLUMN)
'''@param ST7735Start Starting address
'''@param ST7735End Ending address
'''@hide
Sub ST7735SetAddress(In ST7735AddressType, In ST7735Start As Word, In ST7735End As Word)
	ST7735SendCommand ST7735AddressType
	ST7735SendData ST7735Start_H
	ST7735SendData ST7735Start
	ST7735SendData ST7735End_H
	ST7735SendData ST7735End
End Sub

'''@hide
sub   ST7735Rotation ( in ST7735AddressType )

  ST7735SendCommand ( ST7735_MADCTL )
  select case ST7735AddressType
        case LANDSCAPE
             ST7735SendData( 0xB8 )
             ST7735_GLCD_WIDTH = GLCD_HEIGHT
             ST7735_GLCD_HEIGHT = GLCD_WIDTH
        case PORTRAIT_REV
             ST7735_GLCD_WIDTH = GLCD_WIDTH
             ST7735_GLCD_HEIGHT = GLCD_HEIGHT
             ST7735SendData( 0xDC )
        case LANDSCAPE_REV
             ST7735SendData( 0x6C )
             ST7735_GLCD_WIDTH = GLCD_HEIGHT
             ST7735_GLCD_HEIGHT = GLCD_WIDTH
        case PORTRAIT
             ST7735_GLCD_WIDTH = GLCD_WIDTH
             ST7735_GLCD_HEIGHT = GLCD_HEIGHT
             ST7735SendData( 0x08 )
        case else
             ST7735SendData( 0x08 )
  end select

end sub

'''@hide
Sub ST7735SetGammaCorrection
	'Is this needed?
	'Enable correction
	ST7735SendCommand 0x26
	ST7735SendData 0x04
	
	ST7735SendCommand 0xF2
	ST7735SendData 0x01
	
	'Send commands and values from table
	For ST7735ByteNo = 1 to 32
		ReadTable ST7735GammaCorrection, ST7735ByteNo, ST7735Byte
		If ST7735ByteNo = 1 Then
			ST7735SendCommand ST7735Byte
		Else
			If ST7735ByteNo = 17 Then
				Wait 50 ms
				ST7735SendCommand ST7735Byte
			Else
				ST7735SendData ST7735Byte
			End If
		End If
	Next
End Sub

'''@hide
function GLCDReady

         #IF GLCD_TYPE = GLCD_TYPE_ST7920

              #IFDEF GLCD_NO_RW
                        ' Wait 10 ms

                        GLCDReady = TRUE
                        set GLCD_RW OFF 'Write mode
                        exit function
              #ENDIF
    	
              #IFNDEF GLCD_NO_RW

                        #IFDEF GLCD_IO 4,8

                                  GLCDReady = FALSE
                                  GLCD_RSTemp = GLCD_RS
    			
                                  SET GLCD_RW ON
                                  SET GLCD_RS OFF
                                  set GLCD_DATA_PORT.7 off

                                  SET GLCD_Enable ON

                                  #IFDEF GLCD_IO 8
    				
                                            #IFNDEF GLCD_LAT
                                                   dir GLCD_DATA_PORT.7 IN

                                                   wait ST7920WriteDelay us
                                                   if GLCD_DATA_PORT.7 =  OFF THEN
                                                     GLCDReady = TRUE
                                                     dir GLCD_DATA_PORT.7 OUT
                                                     SET GLCD_Enable OFF
                                                     GLCD_RS = GLCD_RSTemp
                                                     exit sub
                                                   end if
                                            #ENDIF
                                            #IFDEF GLCD_LAT
                                                   dir _GLCD_DATA_PORT.7 IN
                                                   wait ST7920WriteDelay us
                                                   if _GLCD_DATA_PORT.7 =  OFF THEN
                                                     GLCDReady = TRUE
                                                     dir _GLCD_DATA_PORT.7 OUT
                                                     SET GLCD_Enable OFF
                                                     GLCD_RS = GLCD_RSTemp
                                                     exit sub
                                                   end if
                                            #ENDIF

                                  #ENDIF
                                  SET GLCD_Enable OFF
                                  wait ST7920WriteDelay us
                                  wait ST7920WriteDelay us
                                  GLCD_RS = GLCD_RSTemp
                        #ENDIF
              #ENDIF
         #endif

end function



'''Initialise the GLCD device
Sub InitGLCD

      #if GLCD_TYPE = GLCD_TYPE_ST7920

          #IFDEF GLCD_IO 4,8

                    #IFNDEF GLCD_LAT
                           DIR GLCD_RS OUT
                           Dir GLCD_RESET Out
		#ENDIF
                    #IFDEF GLCD_LAT
                           DIR _GLCD_RS OUT
                           Dir _GLCD_RESET Out
		#ENDIF

		#IFNDEF GLCD_NO_RW
                              #IFNDEF GLCD_LAT
                                     DIR GLCD_RW OUT
                              #ENDIF
                              #IFDEF GLCD_LAT
                                     DIR _GLCD_RW OUT
                              #ENDIF
		#ENDIF


                    #IFNDEF GLCD_LAT
                           DIR GLCD_Enable OUT
                          'Reset
                          Set GLCD_RESET Off
                          set GLCD_RESET off
                          Wait 1 s
                          Set GLCD_RESET On
                          Wait 50 ms
                          Set GLCD_RESET On
		#ENDIF
                    #IFDEF GLCD_LAT
                           DIR _GLCD_Enable OUT
                           'Reset
                           Set GLCD_RESET Off
                           Wait 1 ms
                           Set GLCD_RESET On
                           Wait 50 ms
		#ENDIF

		
		wait 10 ms
		Wait until GLCDReady

		'Set data bus width
		SET GLCD_RS OFF
	#ENDIF ' GLCD_IO 4,8

	#IFDEF GLCD_IO 8
                    SET GLCD_RS OFF
                    SET GLCD_RW OFF
                    SET GLCD_ENABLE OFF

                    wait 200 ms
		ST7920WriteCommand(0x56)
                    wait 1 ms
                    ST7920WriteCommand(0x56)
                    wait 1 ms
                    ST7920WriteByte(0x36
		wait 5 10us
                    ST7920WriteByte(0x0c)
		wait 5 10us
                    ST7920WriteByte(0x06)
		wait 5 10us
	#ENDIF

	#IFDEF GLCD_IO 4
		'Set pins to output
                    #IFNDEF GLCD_LAT
                        DIR GLCD_DB4 OUT
                        DIR GLCD_DB5 OUT
                        DIR GLCD_DB6 OUT
                        DIR GLCD_DB7 OUT
		#ENDIF
                    #IFDEF GLCD_LAT
                        DIR _GLCD_DB4 OUT
                        DIR _GLCD_DB5 OUT
                        DIR _GLCD_DB6 OUT
                        DIR _GLCD_DB7 OUT
		#ENDIF

		set GLCD_RS OFF
		#IFNDEF GLCD_NO_RW
			set GLCD_RW OFF
		#ENDIF

                    wait 15 ms
                    '0011
		set GLCD_DB4 ON
		set GLCD_DB5 ON
		set GLCD_DB6 OFF
		set GLCD_DB7 OFF
                    Repeat 3
                      PULSEOUT GLCD_Enable, 5 10us
                      Wait 5 ms
		  #IFNDEF GLCD_NO_RW
			set GLCD_RW OFF
                      #ENDIF
                    end repeat
		Wait 5 ms


                    'function set
		set GLCD_DB4 OFF
		set GLCD_DB5 ON
		set GLCD_DB6 OFF
		set GLCD_DB7 OFF
		PULSEOUT GLCD_Enable, 5 10us
		Wait 5 ms

                    'function set
		set GLCD_DB4 OFF
		set GLCD_DB5 OFF
		set GLCD_DB6 OFF
		set GLCD_DB7 OFF
		PULSEOUT GLCD_Enable, 5 10us
		Wait 5 ms

		set GLCD_DB4 OFF
		set GLCD_DB5 OFF
		set GLCD_DB6 OFF
		set GLCD_DB7 ON

		PULSEOUT GLCD_Enable, 5 10us
		Wait 5 ms

		ST7920WriteByte(b'10000000')
		wait 5 10us

		ST7920WriteByte(b'00010000')
		wait 5 ms

		ST7920WriteByte(b'011000000')
		wait 5 10us
	#ENDIF    'GLCD_TYPE = GLCD_TYPE_ST7920


	#IFDEF GLCD_IO 4,8

		'Set Cursor movement
		SET GLCD_RS OFF
		ST7920WriteCommand(b'00000110')
		wait 5 10us
		'Turn off cursor
		ST7920WriteCommand(b'00001100')
		wait 5 10us
		
		'Clear screen
                    GLCD_TYPE_ST7920_GRAPHICS_MODE = false
		GLCDCLS
	#ENDIF
          'Colours
          GLCDBackground = 0
          GLCDForeground = 1
      #ENDIF


	'Setup code for KS0108 controllers
	#if GLCD_TYPE = GLCD_TYPE_KS0108
		'Set pin directions
		Dir GLCD_RS Out
		Dir GLCD_RW Out
		Dir GLCD_ENABLE Out
		Dir GLCD_CS1 Out
		Dir GLCD_CS2 Out
		Dir GLCD_RESET Out
		
		'Reset
		Set GLCD_RESET Off
		Wait 1 ms
		Set GLCD_RESET On
		Wait 1 ms
		
		'Select both chips
		Set GLCD_CS1 On
		Set GLCD_CS2 On
		
		'Set on
		Set GLCD_RS Off
		GLCDWriteByte 63

		'Set Z to 0
		GLCDWriteByte 192
		
		'Deselect chips
		Set GLCD_CS1 Off
		Set GLCD_CS2 Off
		
		'Colours
		GLCDBackground = 0
		GLCDForeground = 1
		
	#endif
	
	'Setup code for ST7735 controllers
	#if GLCD_TYPE = GLCD_TYPE_ST7735
		'Pin directions
		Dir ST7735_CS Out
		Dir ST7735_DC Out
		Dir ST7735_RST Out
		
		Dir ST7735_DI In
		Dir ST7735_DO Out
		Dir ST7735_SCK Out
		
		'SPI mode
		SPIMode MasterFast, 0
		
		'Reset display
		Set ST7735_RST On
		Wait 10 ms
		'Reset sequence (lower line for at least 10 us)
		Set ST7735_RST Off
		Wait 25 us
		Set ST7735_RST On
		Wait 10 ms
		
		'Software reset
		ST7735SendCommand 0x01
		Wait 200 ms

		'Software reset
		ST7735SendCommand 0x01
		Wait 200 ms		
		
                    'Out of sleep mode
		ST7735SendCommand 0x11
		Wait 200 ms
		
		'Gamma correction
		'ST7735SetGammaCorrection
		
		'Scanning direction
		ST7735SendCommand 0x36
		ST7735SendData 0xC8
		
		'Set pixel mode to 16 bpp
		ST7735SendCommand 0x3A
		ST7735SendData 5
		
		'Display on
		ST7735SendCommand 0x29
		Wait 100 ms
		
		'Colours
		GLCDBackground = ST7735_WHITE
		GLCDForeground = ST7735_BLACK

                    'Variables required for device
                    ST7735_GLCD_WIDTH = GLCD_WIDTH
                    ST7735_GLCD_HEIGHT = GLCD_HEIGHT
	#endif
	




	'Clear screen
	GLCDCLS
	
End Sub


'Numbers taken from Arduino_LCD.cpp
Table ST7735GammaCorrection
	'For CMCTRP1 (command E0)
	0xE0
	0x28
	0x24
	0x22
	0x31
	0x2b
	0x0e
	0x53
	0xa5
	0x42
	0x16
	0x18
	0x12
	0x1a
	0x14
	0x03

    'For GMCTRN1 (command E1)
    0xE1
	0x17
	0x1b
	0x1d
	0x0e
	0x14
	0x11
	0x2c
	0xa5
	0x3d
	0x09
	0x27
	0x2d
	0x25
	0x2b
	0x3c
End Table

'''Reset GLCD
'''@hide
'Character bitmaps for print routines
Table GLCDCharCol3
0
16
12
10
136
34
56
32
8
32
16
16
128
128
64
4
0
0
0
40
72
70
108
0
0
0
40
16
0
16
0
64
124
0
132
130
48
78
120
6
108
12
0
0
16
40
0
4
100
248
254
124
254
254
254
124
254
0
64
254
254
254
254
124
254
124
254
76
2
126
62
126
198
14
194
0
4
0
8
128
0
64
254
112
112
112
16
16
254
0
64
254
0
248
248
112
248
16
248
144
16
120
56
120
136
24
136
0
0
0
32
120
End Table

Table GLCDCharCol4
254
56
10
6
204
102
124
112
4
64
16
56
136
162
112
28
0
0
14
254
84
38
146
10
56
130
16
16
160
16
192
32
162
132
194
130
40
138
148
2
146
146
108
172
40
40
130
2
146
36
146
130
130
146
18
130
16
130
128
16
128
4
8
130
18
130
18
146
2
128
64
128
40
16
162
254
8
130
4
128
2
168
144
136
136
168
252
168
16
144
128
32
130
8
16
136
40
40
16
168
124
128
64
128
80
160
200
16
0
130
16
68
End Table

Table GLCDCharCol5
124
124
0
0
238
238
124
168
254
254
84
84
148
148
124
124
0
158
0
40
254
16
170
6
68
68
124
124
96
16
192
16
146
254
162
138
36
138
146
226
146
146
108
108
68
40
68
162
242
34
146
130
130
146
18
146
16
254
130
40
128
24
16
130
18
162
50
146
254
128
128
112
16
224
146
130
16
130
2
128
4
168
136
136
136
168
18
168
8
250
136
80
254
240
8
136
40
40
8
168
144
128
128
96
32
160
168
108
254
108
16
66
End Table

Table GLCDCharCol6
56
254
12
10
204
102
124
32
4
64
56
16
162
136
112
28
0
0
14
254
84
200
68
0
130
56
16
16
0
16
0
8
138
128
146
150
254
138
146
18
146
82
0
0
130
40
40
18
130
36
146
130
68
146
18
146
16
130
126
68
128
4
32
130
18
66
82
146
2
128
64
128
40
16
138
130
32
254
4
128
8
168
136
136
144
168
2
168
8
128
122
136
128
8
8
136
40
48
8
168
128
64
64
128
80
160
152
130
0
16
32
68
End Table

Table GLCDCharCol7
16
0
10
6
136
34
56
62
8
32
16
16
128
128
64
4
0
0
0
40
36
196
160
0
0
0
40
16
0
16
0
4
124
0
140
98
32
114
96
14
108
60
0
0
0
40
16
12
124
248
108
68
56
130
2
244
254
0
2
130
128
254
254
124
12
188
140
100
2
126
62
126
198
14
134
0
64
0
8
128
0
240
112
64
254
48
4
120
240
0
0
0
0
240
240
112
16
248
16
64
64
248
56
120
136
120
136
0
0
0
16
120
End Table


