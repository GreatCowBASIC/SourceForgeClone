'    Graphical LCD routines for the GCBASIC compiler
'    Copyright (C) 2012 - 2017 Hugh Considine, Evan Venn, and Joseph Realmuto

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
'
'    9/11/14  New revised version.  Requires GLCD.H.  Do not call hardware files directly.  Always load via GLCD.H
'    26/7/16  Moved code from ILIxxxx to support GLCDfntDefaultsize = 1,2,3 etc. in Drawstring and DrawChar
'    22/8/16  removal of X1 and Y1 varibles. these clashed with register addresses
'    08/4/17  added GLCD_TYPE_SSD1306_32 support
'    01/5/17  added GLCD_TYPE_ILI9481 support
'    2/05/17  Reverted to standard line routine. Local version overan Byte values
'    3/07/17  Add overloaded function to cater for GLCDPrint with color
'    25/07/17 Added support for low memory SSD1306 usage by adding GLCDDrawChar_SSD1306
'    17/08/17 Added support for low memory SSD1306 and SH1106 GLCD by adding GLCD_Open_PageTransaction
'             and GLCD_Close_PageTransaction
'    21/08/17 Added support for Ellipse methods
'    22/08/17 Added stubs for Triangle
'    23/08/17 Revised Circle and FilledCircle, updated Draw_Filled_Ellipse_Points
'    28/08/17 to resolve non-ANSI characters

'Constants that might need to be set
'#define GLCD_TYPE GLCD_TYPE_KS0108 | GLCD_TYPE_ST7735 | GLCD_TYPE_ST7920 | GLCD_TYPE_PCD8544 | GLCD_TYPE_SSD1306

' Circle edge overdraw protection
' #define GLCD_PROTECTOVERRUN
' Defaults to KS0108

#define GLCD_TYPE GLCD_TYPE_KS0108
#define GLCD_WIDTH 128
#define GLCD_HEIGHT 64
dim GLCDFontWidth,GLCDfntDefault, GLCDfntDefaultsize as byte
#define GLCDLineWidth 1

'''default font size
'GLCDfntDefaultsize = 1


' Screen rotation
#define LANDSCAPE 1
#define PORTRAIT_REV 2
#define LANDSCAPE_REV 3
#define PORTRAIT 4

'GLCD types - add new types here!
#define GLCD_TYPE_KS0108  1
#define GLCD_TYPE_ST7735  2
#define GLCD_TYPE_ST7920  3
#define GLCD_TYPE_PCD8544 4
#define GLCD_TYPE_SSD1306 5
#define GLCD_TYPE_ILI9340 6
#define GLCD_TYPE_SSD1289 7
#define GLCD_TYPE_ILI9341 8
#define GLCD_TYPE_SH1106  9
#define GLCD_TYPE_SSD1306_32 10
#define GLCD_TYPE_ILI9486L 11
#define GLCD_TYPE_ILI9481 12


' Color definitions
' Define Colors
#define TFT_BLACK       0x0000
#define TFT_NAVY        0x000F
#define TFT_DARKGREEN   0x03E0
#define TFT_DARKCYAN    0x03EF
#define TFT_MAROON      0x7800
#define TFT_PURPLE      0x780F
#define TFT_OLIVE       0x7BE0
#define TFT_LIGHTGREY   0xC618
#define TFT_DARKGREY    0x7BEF
#define TFT_BLUE        0x001F
#define TFT_GREEN       0x07E0
#define TFT_CYAN        0x07FF
#define TFT_RED         0xF800
#define TFT_MAGENTA     0xF81F
#define TFT_YELLOW      0xFFE0
#define TFT_WHITE       0xFFFF
#define TFT_ORANGE      0xFD20
#define TFT_GREENYELLOW 0xAFE5
#define TFT_PINK        0xF81F



' Do not remove - global variable required for Circles.
dim GLCD_yordinate as integer
dim glcd_type_string as string * 7

'Foreground and background colours
Dim GLCDBackground As Word
Dim GLCDForeground As Word
'to minimise memory map to the these word variables
Dim GLCDDeviceHeight as Word
Dim GLCDDeviceWidth as Word


#script

  ' add new type here!
  If GLCD_TYPE = GLCD_TYPE_KS0108 Then

     #include <glcd_KS0108.h>
     InitGLCD = InitGLCD_KS0108
     GLCDCLS = GLCDCLS_KS0108
     FilledBox = FilledBox_KS0108
     Pset = Pset_KS0108
     glcd_type_string = "KS0108"
     GLCD_WIDTH = 128
     GLCD_HEIGHT = 64
     KS0108_GLCD_HEIGHT = GLCDDeviceHeight
     KS0108_GLCD_WIDTH = GLCDDeviceWidth

  End If

  If GLCD_TYPE = GLCD_TYPE_SSD1306 Then
     #include <glcd_SSD1306.h>
     InitGLCD = InitGLCD_SSD1306
     GLCDCLS = GLCDCLS_SSD1306
     GLCDDrawChar = GLCDDrawChar_SSD1306
     FilledBox = FilledBox_SSD1306
     Pset = Pset_SSD1306
     GLCD_Open_PageTransaction = GLCD_Open_PageTransaction_SSD1306
     GLCD_Close_PageTransaction = GLCD_Close_PageTransaction_SSD1306
     glcd_type_string = "SSD1306"
     GLCD_WIDTH = 128
     GLCD_HEIGHT = 64
     SSD1306_GLCD_HEIGHT = GLCDDeviceHeight
     SSD1306_GLCD_WIDTH = GLCDDeviceWidth

  End If


  If GLCD_TYPE = GLCD_TYPE_SSD1306_32 Then
     #include <glcd_SSD1306.h>
     InitGLCD = InitGLCD_SSD1306
     GLCDCLS = GLCDCLS_SSD1306
     GLCDDrawChar = GLCDDrawChar_SSD1306
     FilledBox = FilledBox_SSD1306
     Pset = Pset_SSD1306
     GLCD_Open_PageTransaction = GLCD_Open_PageTransaction_SSD1306
     GLCD_Close_PageTransaction = GLCD_Close_PageTransaction_SSD1306
     glcd_type_string = "SSD1306_32"
     GLCD_WIDTH = 128
     GLCD_HEIGHT = 32
     SSD1306_GLCD_HEIGHT = GLCDDeviceHeight
     SSD1306_GLCD_WIDTH = GLCDDeviceWidth

  End If

  If GLCD_TYPE = GLCD_TYPE_ILI9341 Then

     #include <glcd_ILI9341.h>
     InitGLCD = InitGLCD_ILI9341
     GLCDCLS = GLCDCLS_ILI9341
     GLCDDrawChar = GLCDDrawChar_ILI9341
     GLCDDrawString = GLCDDrawString_ILI9341
     FilledBox = FilledBox_ILI9341
     Pset = Pset_ILI9341
     GLCDRotate = GLCDRotate_ILI9341
     glcd_type_string = "ILI9341"
     GLCD_WIDTH = 240
     GLCD_HEIGHT = 320
     ILI9341_GLCD_HEIGHT = GLCDDeviceHeight
     ILI9341_GLCD_WIDTH = GLCDDeviceWidth
  End If


  If GLCD_TYPE = GLCD_TYPE_ILI9340 Then

     #include <glcd_ILI9340.h>
     InitGLCD = InitGLCD_ILI9340
     GLCDCLS = GLCDCLS_ILI9340
     GLCDDrawChar = GLCDDrawChar_ILI9340
     GLCDDrawString = GLCDDrawString_ILI9340
     FilledBox = FilledBox_ILI9340
     Pset = Pset_ILI9340
     GLCDRotate = GLCDRotate_ILI9340
     glcd_type_string = "ILI9340"
     GLCD_WIDTH = 240
     GLCD_HEIGHT = 320
     ILI9340_GLCD_HEIGHT = GLCDDeviceHeight
     ILI9340_GLCD_WIDTH = GLCDDeviceWidth
  End If


  If GLCD_TYPE = GLCD_TYPE_SSD1289 Then

     #include <glcd_SSD1289.h>
     InitGLCD = InitGLCD_SSD1289
     GLCDCLS = GLCDCLS_SSD1289
     GLCDDrawChar = DrawChar_SSD1289
     GLCDPrint = Print_SSD1289
     FilledBox = FilledBox_SSD1289
     Box = Box_SSD1289
     GLCDDrawString = Print_SSD1289
     Circle = Circle_SSD1289
     FilledCircle  = FilledCircle_SSD1289
'     Line =  Line_SSD1289
     Pset = PSet_SSD1289
      GLCDRotate = GLCDRotate_SSD1289
     glcd_type_string = "SSD1289"
     GLCD_WIDTH = 240
     GLCD_HEIGHT = 320
     SSD1289_GLCD_HEIGHT = GLCDDeviceHeight
     SSD1289_GLCD_WIDTH = GLCDDeviceWidth

  End If

  If GLCD_TYPE = GLCD_TYPE_ST7735 Then

     #include <glcd_ST7735.h>
     InitGLCD = InitGLCD_ST7735
     GLCDCLS = GLCDCLS_ST7735
     GLCDDrawChar = GLCDDrawChar_ST7735
     FilledBox = FilledBox_ST7735
     Pset = Pset_ST7735
     GLCDRotate = GLCDRotate_ST7735
     glcd_type_string = "ST7735"
     GLCD_WIDTH = 128
     GLCD_HEIGHT = 160
     ST7735_GLCD_HEIGHT = GLCDDeviceHeight
     ST7735_GLCD_WIDTH = GLCDDeviceWidth


  End If

  If GLCD_TYPE = GLCD_TYPE_ST7920 Then

     #include <glcd_ST7920.h>
     InitGLCD = InitGLCD_ST7920
     GLCDCLS = GLCDCLS_ST7920
     FilledBox = FilledBox_ST7920
     Pset = Pset_ST7920
     GLCDPrint = GLCDPrint_ST7920
     glcd_type_string = "ST7920"
     GLCD_WIDTH = 128
     GLCD_HEIGHT = 64
     ST7920_GLCD_HEIGHT = GLCDDeviceHeight
     ST7920_GLCD_WIDTH = GLCDDeviceWidth
  End If

  If GLCD_TYPE = GLCD_TYPE_PCD8544 Then

     #include <glcd_PCD8544.h>
     InitGLCD = InitGLCD_PCD8544
     GLCDCLS = GLCDCLS_PCD8544
     GLCDDrawChar = GLCDDrawChar_PCD8544
     FilledBox = FilledBox_PCD8544
     Pset = Pset_PCD8544
     glcd_type_string = "PCD8544"
     PCD8544WriteDelay = 0
     PCD8544ClockDelay = 0
     GLCD_WIDTH = 84
     GLCD_HEIGHT = 48
     PCD8544_GLCD_HEIGHT = GLCDDeviceHeight
     PCD8544_GLCD_WIDTH = GLCDDeviceWidth
  End If

  If GLCD_TYPE = GLCD_TYPE_SH1106 Then
     #include <glcd_SH1106.h>
     InitGLCD = InitGLCD_SH1106
     GLCDCLS = GLCDCLS_SH1106
     GLCDDrawChar = GLCDDrawChar_SH1106
     FilledBox = FilledBox_SH1106
     Pset = Pset_SH1106
     GLCD_Open_PageTransaction = GLCD_Open_PageTransaction_SH1106
     GLCD_Close_PageTransaction = GLCD_Close_PageTransaction_SH1106
     GLCDSetContrast = GLCDSetContrast_SSH1106
     GLCDSetDisplayNormalMode = GLCDSetDisplayNormalMode_SSH1106
     GLCDSetDisplayInvertMode = GLCDSetDisplayInvertMode_SSH1106
     glcd_type_string = "SH1106"
     GLCD_WIDTH = 128
     GLCD_HEIGHT = 64
     SH1106_GLCD_HEIGHT = GLCDDeviceHeight
     SH1106_GLCD_WIDTH = GLCDDeviceWidth

  End If

  'Loads extended fonts set ASCII characters 31-254
  'Requires 1358 bytes of program memory
  If GLCD_EXTENDEDFONTSET1 then

     GLCDCharCol3 = GLCDCharCol3Extended1
     GLCDCharCol4 = GLCDCharCol4Extended1
     GLCDCharCol5 = GLCDCharCol5Extended1
     GLCDCharCol6 = GLCDCharCol6Extended1
     GLCDCharCol7 = GLCDCharCol7Extended1

  End If


If GLCD_TYPE = GLCD_TYPE_ILI9486L Then

     #include <glcd_ILI9486L.h>
     InitGLCD = InitGLCD_ili9486L
     GLCDCLS = GLCDCLS_ili9486L
     GLCDDrawChar = GLCDDrawChar_ili9486L
     GLCDDrawString = GLCDDrawString_ili9486L
     FilledBox = FilledBox_ili9486L
     Pset = Pset_ili9486L
     GLCDRotate = GLCDRotate_ili9486L
     glcd_type_string = "ili9486L"
     GLCD_WIDTH = 320
     GLCD_HEIGHT = 480
     ili9486L_GLCD_HEIGHT = GLCDDeviceHeight
     ili9486L_GLCD_WIDTH = GLCDDeviceWidth
  End If

If GLCD_TYPE = GLCD_TYPE_ILI9481 Then

     #include <glcd_ILI9481.h>
     InitGLCD = InitGLCD_ili9481
     GLCDCLS = GLCDCLS_ili9481
     GLCDDrawChar = GLCDDrawChar_ili9481
     GLCDDrawString = GLCDDrawString_ili9481
     FilledBox = FilledBox_ili9481
     Pset = Pset_ili9481
     GLCDRotate = GLCDRotate_ili9481
     glcd_type_string = "ili9481"
     GLCD_WIDTH = 320
     GLCD_HEIGHT = 480
     ili9481_GLCD_HEIGHT = GLCDDeviceHeight
     ili9481_GLCD_WIDTH = GLCDDeviceWidth
  End If

#endscript


#define GLCDInit InitGLCD
'''Initialise the GLCD device
Sub InitGLCD
    ' Empty sub DO NOT DELETE
end sub


'Subs
'''Clears the GLCD screen
Sub GLCDCLS
          ' initialise global variable. Required variable for Circle - DO NOT DELETE
          GLCD_yordinate = 0

End Sub

'''Displays a message
'''@param PrintLocX X coordinate for message
'''@param PrintLocY Y coordinate for message
'''@param PrintData Message to display
Sub GLCDPrint(In PrintLocX as word, In PrintLocY as word, in LCDPrintData as string )
  Dim GLCDPrintLoc as word
  PrintLen = LCDPrintData(0)
  If PrintLen = 0 Then Exit Sub

  GLCDPrintLoc = PrintLocX
  'Write Data
  For SysPrintTemp = 1 To PrintLen
    GLCDDrawChar GLCDPrintLoc, PrintLocY, LCDPrintData(SysPrintTemp)
    GLCDPrintLoc += GLCDFontWidth * GLCDfntDefaultsize
  Next
End Sub

'''@hide
Sub GLCDPrint(In PrintLocX as word, In PrintLocY as word, in LCDPrintData as string, In LineColour as word )
  Dim GLCDPrintLoc as word
  PrintLen = LCDPrintData(0)
  If PrintLen = 0 Then Exit Sub

  GLCDPrintLoc = PrintLocX
  'Write Data
  For SysPrintTemp = 1 To PrintLen
    GLCDDrawChar GLCDPrintLoc, PrintLocY, LCDPrintData(SysPrintTemp), LineColour
    GLCDPrintLoc += GLCDFontWidth * GLCDfntDefaultsize
  Next
End Sub




'''Displays a number
'''@param PrintLocX X coordinate for message
'''@param PrintLocY Y coordinate for message
'''@param LCDValue Number to display
Sub GLCDPrint(In PrintLocX as word, In PrintLocY as word, In LCDValue As Long)
  Dim SysCalcTempA As Long
  Dim GLCDPrintLoc as word
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
    GLCDPrintLoc += GLCDFontWidth * GLCDfntDefaultsize
  Next

End Sub

'''@hide
Sub GLCDPrint(In PrintLocX as word, In PrintLocY as word, In LCDValue As Long, In LineColour as word )
  Dim SysCalcTempA As Long
  Dim GLCDPrintLoc as word
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
    GLCDDrawChar GLCDPrintLoc, PrintLocY, SysPrintBuffer(SysPrintTemp) + 48, LineColour
    GLCDPrintLoc += GLCDFontWidth * GLCDfntDefaultsize
  Next

End Sub

'''Draws a string at the specified location on the ST7920 GLCD
'''@param StringLocX X coordinate for message
'''@param CharLocY Y coordinate for message
'''@param Chars String to display
'''@param LineColour Line Color, either 1 or 0
Sub GLCDDrawString( In StringLocX, In CharLocY, In Chars as string, Optional In LineColour as word = GLCDForeground )

    dim TargetCharCol as word
    for xchar = 1 to Chars(0)
      ' Corrected error X calcaluation. It was adding an Extra 1!
      TargetCharCol = StringLocX + ((xchar*( GLCDFontWidth * GLCDfntDefaultsize ))-( GLCDFontWidth * GLCDfntDefaultsize ))
      GLCDDrawChar TargetCharCol , CharLocY , Chars(xchar), LineColour
    next

end sub

'''Draws a character at the specified location on the ST7920 GLCD
'''@param StringLocX X coordinate for message
'''@param CharLocY Y coordinate for message
'''@param Chars String to display
'''@param LineColour Line Color, either 1 or 0
Sub GLCDDrawChar(In CharLocX as word, In CharLocY as word, In CharCode, Optional In LineColour as word = GLCDForeground )
  'moved code from ILIxxxx to support GLCDfntDefaultsize = 1,2,3 etc.
  'CharCode needs to have 16 subtracted, table starts at char 16 not char 0

    'invert colors if required
    if LineColour <> GLCDForeground  then
      'Inverted Colours
      GLCDBackground = 1
      GLCDForeground = 0
    end if

   dim CharCol, CharRow as word
   CharCode -= 15

    if CharCode>=178 and CharCode<=202 then
       CharLocY=CharLocY-1
    end if

    CharCol=1

    For CurrCharCol = 1 to 5
      Select Case CurrCharCol
        Case 1: ReadTable GLCDCharCol3, CharCode, CurrCharVal
        Case 2: ReadTable GLCDCharCol4, CharCode, CurrCharVal
        Case 3: ReadTable GLCDCharCol5, CharCode, CurrCharVal
        Case 4: ReadTable GLCDCharCol6, CharCode, CurrCharVal
        Case 5: ReadTable GLCDCharCol7, CharCode, CurrCharVal
      End Select
      CharRow=0
      For CurrCharRow = 1 to 8
          CharColS=0
          For Col=1 to GLCDfntDefaultsize
                CharColS +=1
                CharRowS=0
                For Row=1 to GLCDfntDefaultsize
                    CharRowS +=1
                    if CurrCharVal.0=1 then
                       PSet [word]CharLocX + CharCol+ CharColS, [word]CharLocY + CharRow+CharRowS, LineColour
                    Else
                       PSet [word]CharLocX + CharCol+ CharColS, [word]CharLocY + CharRow+CharRowS, GLCDBackground
                    End if
                Next Row
          Next Col
        Rotate CurrCharVal Right
        CharRow +=GLCDfntDefaultsize
      Next
      CharCol +=GLCDfntDefaultsize
    Next

    'Restore
    GLCDBackground = 0
    GLCDForeground = 1

End Sub

'''Draws a box on the GLCD screen
'''@param LineX1 Top left corner X location
'''@param LineY1 Top left corner Y location
'''@param LineX2 Bottom right corner X location
'''@param LineY2 Bottom right corner Y location
'''@param LineColour Colour of box border (0 = erase, 1 = draw, default is 1)
#define GLCDBox Box
Sub Box(In LineX1 as word, In LineY1 as word, In LineX2 as word, In LineY2 as word, Optional In LineColour As Word = GLCDForeground)

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

  dim DrawLine as word
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
Sub FilledBox(In LineX1 as word, In LineY1 as word, In LineX2 as word, In LineY2 as word, Optional In LineColour As Word = GLCDForeground)
    ' Each Device has a specific FilledBox routine
End Sub

'''Draws a circle on the GLCD screen
'''@param Xoffset X point of circle
'''@param Yoffset Y point of circle
'''@param Inxradius radius of circle
'''@param LineColour Colour of line (0 = blank, 1 = show, default is 1)
'''@param yordinate (optional) rounding
#define GCLDCircle Circle
'sub Circle ( in xoffset as word, in yoffset as word, in Inxradius as integer, Optional In LineColour as word = GLCDForeground , Optional In yordinate = GLCD_yordinate)
'
'
'    dim  radiusErr, xradius as Integer
'    xradius = Inxradius
'    radiusErr = -(xradius/2)
'    Do While xradius >=  yordinate
'       Pset ((xoffset + xradius), (yoffset + yordinate), LineColour)
'       Pset ((xoffset + yordinate), (yoffset + xradius), LineColour)
'       Pset ((xoffset - xradius), (yoffset + yordinate), LineColour)
'       Pset ((xoffset - yordinate), (yoffset + xradius), LineColour)
'       Pset ((xoffset - xradius), (yoffset - yordinate), LineColour)
'       Pset ((xoffset - yordinate), (yoffset - xradius), LineColour)
'       Pset ((xoffset + xradius), (yoffset - yordinate), LineColour)
'       Pset ((xoffset + yordinate), (yoffset - xradius), LineColour)
'       yordinate ++
'       If radiusErr < 0 Then
'          radiusErr = radiusErr + 2 * yordinate + 1
'       else
'          xradius --
'          radiusErr = radiusErr + 2 * (yordinate - xradius + 1)
'       end if
'    Loop
'
'end sub
'
'
''''Fills a circle on the GLCD screen
''''@param Xoffset X point of circle
''''@param Yoffset Y point of circle
''''@param xradius radius of circle
''''@param LineColour Colour of line (0 = blank, 1 = show, default is 1)
'#define GLCDFilledCircle FilledCircle
'sub FilledCircle( in xoffset as word, in yoffset as word, in xradius as word, Optional In LineColour as word = GLCDForeground)
'
'    'Circle fill Code is merely a modification of the midpoint
'    ' circle algorithem which is an adaption of Bresenham's line algorithm
'    ' http://en.wikipedia.org/wiki/Midpoint_circle_algorithm
'    ' http://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm
'
'  dim ff, ddF_x, ddF_y as integer
'  dim YCalc2, YCalc1 as word
'  ff = 1 - xradius
'  ddF_x = 1
'  ddF_y = -2 * xradius
'  FillCircleXX = 0
'  FillCircleYY = xradius
'
'  ' Fill in the center between the two halves
'          YCalc2 = yoffset+xradius
'          YCalc1 = yoffset-xradius
'  Line( xoffset, YCalc1 , xoffset, YCalc2, LineColour)
'
'  do while (FillCircleXX < FillCircleYY)
'             if ff >= 0 then
'                FillCircleYY--
'                ddF_y += 2
'                ff += ddF_y
'             end if
'     FillCircleXX++
'     ddF_x += 2
'     ff += ddF_x
'             ' Now draw vertical lines between the points on the circle rather than
'             ' draw the points of the circle. This draws lines between the
'             ' perimeter points on the upper and lower quadrants of the 2 halves of the circle.
'
'     Line(xoffset+FillCircleXX, yoffset+FillCircleYY, xoffset+FillCircleXX, yoffset-FillCircleYY, LineColour);
'     Line(xoffset-FillCircleXX, yoffset+FillCircleYY, xoffset-FillCircleXX, yoffset-FillCircleYY, LineColour);
'     Line(xoffset+FillCircleYY, yoffset+FillCircleXX, FillCircleYY+xoffset, yoffset-FillCircleXX, LineColour);
'     Line(xoffset-FillCircleYY, yoffset+FillCircleXX, xoffset-FillCircleYY, yoffset-FillCircleXX, LineColour);
'    loop
'end sub

Sub Circle( in xoffset as word, in yoffset as word, in Inxradius as word,Optional In LineColour as word = GLCDForeground )

'Version 1.00 (08/21/2017) by Joseph Realmuto
'draws an circle outline at location (xoffset, yoffset)
'Inxradius is radius of circle

  filled_circle = 0
  DrawCircleRoutine( xoffset, yoffset, Inxradius, LineColour )

End Sub

Sub FilledCircle( in xoffset as word, in yoffset as word, in Inxradius as word,Optional In LineColour as word = GLCDForeground )

'Version 1.00 (08/21/2017) by Joseph Realmuto
'draws a filled circle at location (xoffset, yoffset)
'Inxradius is radius of circle

  filled_circle = 1
  DrawCircleRoutine( xoffset, yoffset, Inxradius, LineColour )

End Sub

Sub DrawCircleRoutine( in xoffset as word, in yoffset as word, in Inxradius as word,Optional In LineColour as word = GLCDForeground )

'Version 1.00 (08/21/2017) by Joseph Realmuto
'draws an circle outline at location (xoffset, yoffset) if filled_circle = 0
'draws a filled circle at location (xoffset, yoffset) if filled_circle = 1
'Inxradius is radius of circle

  IF Inxradius<2 THEN exit sub

  dim circle_xx, circle_yy, circle_pp as Word

  circle_xx = 0
  circle_yy = Inxradius

  circle_pp = 1 - Inxradius

  DO WHILE circle_xx <= circle_yy

    IF filled_circle = 0 THEN
     Draw_Circle_Points
    ELSE
     Draw_Filled_Circle_Points
    END IF

    circle_xx++

    IF circle_pp_H.7 = 0 THEN
     circle_yy--
     'circle_pp = circle_pp - 2*circle_yy
     circle_pp = circle_pp - circle_yy
     circle_pp = circle_pp - circle_yy
    END IF

    'circle_pp = circle_pp + 2*circle_xx + 1
    circle_pp = circle_pp + circle_xx
    circle_pp = circle_pp + circle_xx
    circle_pp = circle_pp + 1

  LOOP

End Sub

Sub Draw_Circle_Points

  Pset ((xoffset + circle_xx), (yoffset + circle_yy), LineColour)
  Pset ((xoffset - circle_xx), (yoffset + circle_yy), LineColour)
  Pset ((xoffset - circle_xx), (yoffset - circle_yy), LineColour)
  Pset ((xoffset + circle_xx), (yoffset - circle_yy), LineColour)
  Pset ((xoffset + circle_yy), (yoffset + circle_xx), LineColour)
  Pset ((xoffset - circle_yy), (yoffset + circle_xx), LineColour)
  Pset ((xoffset - circle_yy), (yoffset - circle_xx), LineColour)
  Pset ((xoffset + circle_yy), (yoffset - circle_xx), LineColour)

End Sub

Sub Draw_Filled_Circle_Points

  FOR circle_xx1 = yoffset to (yoffset + 2*circle_yy)
   Pset ((xoffset + circle_xx), (circle_xx1 - circle_yy), LineColour)
   Pset ((xoffset - circle_xx), (circle_xx1 - circle_yy), LineColour)
  NEXT

  FOR circle_xx1 = yoffset to (yoffset + 2*circle_xx)
   Pset ((xoffset + circle_yy), (circle_xx1 - circle_xx), LineColour)
   Pset ((xoffset - circle_yy), (circle_xx1 - circle_xx), LineColour)
  NEXT

End Sub


'''Draws a line on the GLCD screen
'''@param LineX1 Starting X point of line
'''@param LineY1 Starting Y point of line
'''@param LineX2 Ending X point of line
'''@param LineY2 Ending Y point of line
'''@param LineColour Colour of line (0 = blank, 1 = show, default is 1)
#define GLCDLine Line
Sub Line(In LineX1 as word, In LineY1 as word, In LineX2 as word, In LineY2 as word, Optional In LineColour as word = GLCDForeground)

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


'''Draws a pixel on the GLCD
'''@param GLCDX X coordinate of pixel
'''@param GLCDY Y coordinate of pixel
'''@param GLCDColour State of pixel (0 = erase, 1 = display)
Sub PSet(In GLCDX, In GLCDY, In GLCDColour As Word)
    ' Each Device has a specific PSET routine
End Sub


sub GLCDRotate ( in AddressType )
    ' Empty routine  - do not delete
end sub

'''Draws a Horizontal Line on the GLCD with a
'''@param GLCDX1 X coordinate of one end of the line
'''@param GLCDX2 X coordinate of the other end of the line
'''@param GLCDY1 Y coordinate of the line
'''@param LineColour color
Sub HLine(In GLCDX1 as word, In GLCDX2 as word, In GLCDY1 as word, Optional In LineColour As Word = GLCDForeground)
 GLCDTemp=[Word]GLCDY1+GLCDLineWidth-1
 Box( GLCDX1, GLCDY1, GLCDX2, GLCDTemp, LineColour )
end Sub

'''Draws a Vertical Line on the GLCD with a
'''@param GLCDY1 Y coordinate of one end of the line
'''@param GLCDY2 Y coordinate of the other end of the line
'''@param GLCDX1 X coordinate of the line
'''@param LineColour color
Sub VLine(In GLCDY1 as word, In GLCDY2 as word, In  GLCDX1 as word, Optional In LineColour As Word = GLCDForeground)
 GLCDTemp=[Word]GLCDX1+GLCDLineWidth-1
 Box( GLCDX1, GLCDY1, GLCDTemp, GLCDY2, LineColour )
End Sub




Sub RoundRect(In  RX1 as Word, In  RY1 as Word, In  RX2 as Word, In  RY2 as Word, Optional In  Color as Word=GLCDForeground)
    if RX1>RX2 then SWAP (RX1 , RX2)
    if RY1>RY2 then SWAP (RY1 , RY2)
    if (RX2-RX1>4) and (RY2-RY1>4) Then
       Pset (RX1+1, RY1+2 , Color)
       Pset (RX1+2, RY1+1 , Color)
       Pset (RX2-2, RY1+1 , Color)
       Pset (RX2-1, RY1+2 , Color)
       Pset (RX1+1, RY2-2 , Color)
       Pset (RX1+2, RY2-1 , Color)
       Pset (RX2-2, RY2-1 , Color)
       Pset (RX2-1, RY2-2 , Color)
       Line RX1+3, RY1 , RX2-3, RY1, Color
       Line RX1+3, RY2 , RX2-3, RY2, Color
       Line RX1 , RY1+3, RX1 , RY2-3, Color
       Line RX2 , RY1+3, RX2 , RY2-3, Color
    end if
End Sub

Sub FillRoundRect(In  RX1 as Word, In  RY1 as Word, In  RX2 as Word, In  RY2 as Word, Optional In  Color as Word=GLCDForeground)
    Dim FRI as Word
    if RX1>RX2 then SWAP (RX1 , RX2)
    if RY1>RY2 then SWAP (RY1 , RY2)
    if (RX2-RX1>4) and (RY2-RY1>4) Then
       for FRI=0 to (RY2-RY1)/2
           Select Case FRI
               Case 0
                  Line RX1+3 , RY1 , RX2-3, RY1 , Color
                  Line RX1+3 , RY2 , RX2-3, RY2 , Color
               Case 1
                  Line RX1+2 , RY1+1 , RX2-2, RY1+1 , Color
                  Line RX1+2 , RY2-1 , RX2-1, RY2-1 , Color
               Case 2
                  Line RX1+1 , RY1+2 , RX2-1, RY1+2 , Color
                  Line RX1+1 , RY2-2 , RX2-1, RY2-2 , Color
               Case Else
                  Line RX1 , RY1+FRI , RX2, RY1+FRI , Color
                  Line RX1 , RY2-FRI , RX2, RY2-FRI , Color
           End Select
       Next
    End if
End Sub

' CreateButton_SSD1289 method creates Botton on screen.
'Parameters
'BX1,BY1,BX2,BY2 coordinates of Botton
'FillColor ,
'BorderColor the colors of Botton
'PrintData the text in center of Botton
'FColor The color of text and
'Size the size of characters
Sub CreateButton (In BX1 as Word, In BY1 as Word , In BX2 as Word , In BY2 as Word , In FillColor as Word, In BorderColor as Word, In PrintData As String, In FColor as Word, In Size = 1)
    Dim TempColor1 , TempColor2 as Word
    Dim XCalc1 , YCalc1 as Word

    FillRoundRect(BX1, BY1, BX2, BY2, FillColor)
    RoundRect(BX1, BY1, BX2, BY2, BorderColor)
    XCalc1=(BX1+BX2)/2-PrintData(0)*6*Size/2
    YCalc1=(BY1+BY2)/2-4*Size
    TempColor1=GLCDBackground
    TempColor2= GLCDForeground
    GLCDBackground=FillColor

    GLCDPrint( XCalc1, YCalc1, PrintData, FColor, Size )
    GLCDBackground=TempColor1
    GLCDForeground=TempColor2
End Sub

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


'Character bitmaps for print routines
Table GLCDCharCol3Extended1
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
248  'A
254  'B
124  'C
254  'D
254  'E
254  'F
124  'G
254  'H
0    'I
64   'J
254  'K
254  'L
254  'M
254  'N
124  'O
254  'P
124  'Q
254  'R
76   'S
2    'T
126  'U
62   'V
126  'W
198  'X
14   'Y
194  'Z
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
120 '127
0
0
0
0
0
0
0     '134 - THE CENT sign
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
248   '193 �
254   '194 B
254   '195 �
248   '196 �
254   '197 E
194   '198 Z
254   '199 H
124   '200 �
0     '201 �
254   '202 �
248   '203 �
254   '204 �
254   '205 �
130   '206 �
124   '207 �
254   '208 �
254   '209 �
0
198   '211 �
2     '212 �
14    '213 �
56    '214 �
198   '215 �
14    '216 �
188   '217 �
0
0
56    '220 �
40    '221 �
124   '222 �
0     '223 �
0
56    '225 �
254   '226 �
4     '227 �
48    '228 �
40    '229 �
1     '230 �
124   '231 �
48    '232 �
0     '233 �
124   '234 �
64    '235 �
252   '236 �
28    '237 �
0     '238 �
56    '239 �
4     '240 �
248   '241 �
24    '242 �
56    '243 �
4     '244 �
60    '245 �
56    '246 �
68    '247 �
60    '248 �
60    '249 �
0     '250 �
60    '251 �
56    '252 �
60    '253 �
60    '254 �
End Table

Table GLCDCharCol4Extended1
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
36    '�
146   'B
130   'C
130   'D
146   'E
18    'F
130   'G
16    'H
130   'I
128   'J
16    'K
128   'L
4     'M
8     'N
130   'O
18    'P
130   'Q
18    'R
146   'S
2     'T
128   'U
64    'V
128   'W
40    'X
16    'Y
162   'Z
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
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
36    '193 �
146   '194 B
2     '195 �
132   '196 �
146   '197 E
162   '198 Z
16    '199 H
146   '200 �
130   '201 �
16    '202 �
4     '203 �
4     '204 �
8     '205 �
146   '206 �
130   '207 �
2     '208 �
18    '209 �
0
170   '211 �
2     '212 �
16    '213 �
68    '214 �
40    '215 �
16    '216 �
194   '217 �
0
0
68    '220 �
84    '221 �
8     '222 �
0     '223 �
0
68    '225 �
73    '226 �
104   '227 �
73    '228 �
84    '229 �
25    '230 �
8     '231 �
74    '232 �
0     '233 �
16    '234 �
50    '235 �
64    '236 �
32    '237 �
1     '238 �
68    '239 �
124   '240 �
36    '241 �
36    '242 �
68    '243 �
4     '244 �
64    '245 �
68    '246 �
40    '247 �
64    '248 �
64    '249 �
1     '250 �
65    '251 �
68    '252 �
64    '253 �
64    '254 �
End Table

Table GLCDCharCol5Extended1
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
34   'A
146  'B
130  'C
130  'D
146  'E
18   'F
146  'G
16   'H
254  'I
130  'J
40   'K
128  'L
24   'M
16   'N
130  'O
18   'P
162  'Q
50   'R
146  'S
254  'T
128  'U
128  'V
112  'W
16   'X
224  'Y
146  'Z
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
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
34   '193 �
146  '194 B
2    '195 �
130  '196 �
146  '197 E
146  '198 Z
16   '199 H
146  '200 �
254  '201 �
40   '202 �
2    '203 �
24   '204 �
16   '205 �
146  '206 �
130  '207 �
2    '208 �
18   '209 �
0
146  '211 �
254  '212 �
224  '213 �
254  '214 �
16   '215 �
254  '216 �
2    '217 �
0
0
69   '220 �
84   '221 �
5    '222 �
61   '223 �
0
68    '225 �
73    '226 �
144   '227 �
75    '228 �
84    '229 �
165   '230 �
4     '231 �
77    '232 �
60    '233 �
40    '234 �
9     '235 �
64    '236 �
64    '237 �
149   '238 �
68    '239 �
4     '240 �
36    '241 �
164   '242 �
68    '243 �
124   '244 �
64    '245 �
254   '246 �
16    '247 �
248   '248 �
56    '249 �
60    '250 �
64    '251 �
69    '252 �
66    '253 �
58    '254 �
End Table

Table GLCDCharCol6Extended1
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
36   'A
146  'B
130  'C
68   'D
146  'E
18   'F
146  'G
16   'H
130  'I
126  'J
68   'K
128  'L
4    'M
32   'N
130  'O
18   'P
66   'Q
82   'R
146  'S
2    'T
128  'U
64   'V
128  'W
40   'X
16   'Y
138  'Z
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
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
36   '193 �
146  '194 B
2    '195 �
132  '196 �
146  '197 E
138  '198 Z
16   '199 H
146  '200 �
130  '201 �
68   '202 �
4    '203 �
4    '204 �
32   '205 �
146  '206 �
130  '207 �
2    '208 �
18   '209 �
0
130  '211 �
2    '212 �
16   '213 �
68   '214 �
40   '215 �
16   '216 �
194  '217 �
0
0
56   '220 �
85   '221 �
4    '222 �
64   '223 �
0
56    '225 �
78    '226 �
104   '227 �
77    '228 �
84    '229 �
67    '230 �
4     '231 �
73    '232 �
64    '233 �
68    '234 �
9     '235 �
32    '236 �
32    '237 �
171   '238 �
68    '239 �
124   '240 �
36    '241 �
164   '242 �
76    '243 �
4     '244 �
64    '245 �
68    '246 �
40    '247 �
64    '248 �
64    '249 �
65    '250 �
65    '251 �
68    '252 �
65    '253 �
65    '254 �
End Table

Table GLCDCharCol7Extended1
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
248   'A
108   'B
68    'C
56    'D
130   'E
2     'F
244   'G
254   'H
0     'I
2     'J
130   'K
128   'L
254   'M
254   'N
124   'O
12    'P
188   'Q
140   'R
100   'S
2     'T
126   'U
62    'V
126   'W
198   'X
14    'Y
134   'Z
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
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
248    '193 �
108    '194 B
2      '195 �
248    '196 �
130    '197 E
134    '198 Z
254    '199 H
124    '200 �
0      '201 �
130    '202 �
248    '203 �
254    '204 �
254    '205 �
130    '206 �
124    '207 �
254    '208 �
12     '209 �
0
130    '211 �
2      '212 �
14     '213 �
56     '214 �
198    '215 �
14     '216 �
188    '217 �
0
0
68     '220 �
68     '221 �
120    '222 �
32     '223 �
0
68    '225 �
48    '226 �
4     '227 �
49    '228 �
68    '229 �
1     '230 �
120   '231 �
62    '232 �
32    '233 �
0     '234 �
126   '235 �
124   '236 �
28    '237 �
65    '238 �
56    '239 �
4     '240 �
24    '241 �
72    '242 �
52    '243 �
4     '244 �
60    '245 �
56    '246 �
68    '247 �
60    '248 �
60    '249 �
32    '250 �
60    '251 �
56    '252 �
60    '253 �
60    '254 �
End Table


Sub Ellipse( in xoffset as word, in yoffset as word, in Inxradius as word,in Inyradius as word,Optional In LineColour as word = GLCDForeground )

'Version 1.00 (08/20/2017) by Joseph Realmuto
'draws an ellipse outline at location (xoffset, yoffset)
'Inxradius is x radius of ellipse
'Inyradius is y radius of ellipse

  filled_ellipse = 0
  DrawEllipseRoutine( xoffset, yoffset, Inxradius, Inyradius, LineColour )

End Sub

Sub FilledEllipse( in xoffset as word, in yoffset as word, in Inxradius as word,in Inyradius as word,Optional In LineColour as word = GLCDForeground )

'Version 1.00 (08/20/2017) by Joseph Realmuto
'draws a filled ellipse at location (xoffset, yoffset)
'Inxradius is x radius of ellipse
'Inyradius is y radius of ellipse

  filled_ellipse = 1
  DrawEllipseRoutine( xoffset, yoffset, Inxradius, Inyradius, LineColour )

End Sub

Sub DrawEllipseRoutine( in xoffset as word, in yoffset as word, in Inxradius as word,in Inyradius as word,Optional In LineColour as word = GLCDForeground )

'Version 1.00 (08/20/2017) by Joseph Realmuto
'draws an ellipse outline at location (xoffset, yoffset) if filled_ellipse = 0
'draws a filled ellipse at location (xoffset, yoffset) if filled_ellipse = 1
'Inxradius is x radius of ellipse
'Inyradius is y radius of ellipse

  'IF Inxradius = Inyradius THEN
  '  IF filled_ellipse = 0 THEN
  '    Circle Xoffset, Yoffset, Inxradius, LineColour
  '  ELSE
  '    FilledCircle Xoffset, Yoffset, Inxradius, LineColour
  '  END IF
  '    GLCD_exit sub
  'END IF

  IF Inxradius<2 THEN exit sub
  IF Inyradius<2 THEN exit sub

  dim GLCD_xx, GLCD_yy, GLCD_rx2, GLCD_ry2, GLCD_fx2, GLCD_fy2, GLCD_ex2, GLCD_ey2 as Word
  dim GLCD_px, GLCD_py, GLCD_pp, GLCD_pp_temp as Long

  GLCD_rx2 = Inxradius * Inxradius
  GLCD_ry2 = Inyradius * Inyradius

  'GLCD_fx2 = 4 * GLCD_rx2
  GLCD_fx2 = GLCD_rx2 + GLCD_rx2
  GLCD_fx2 = GLCD_fx2 + GLCD_fx2

  'GLCD_fy2 = 4 * GLCD_ry2
  GLCD_fy2 = GLCD_ry2 + GLCD_ry2
  GLCD_fy2 = GLCD_fy2 + GLCD_fy2

  'GLCD_ex2 = 2 * GLCD_fx2
  GLCD_ex2 = GLCD_fx2 + GLCD_fx2

  'GLCD_ey2 = 2 * GLCD_fy2
  GLCD_ey2 = GLCD_fy2 + GLCD_fy2

  GLCD_xx = 0
  GLCD_yy = Inyradius
  GLCD_px = 0
  GLCD_py = GLCD_ex2 * GLCD_yy

  'GLCD_pp = 2 + GLCD_fy2 -  GLCD_fx2 * Inyradius + GLCD_rx2
  GLCD_pp_temp = GLCD_fx2 * Inyradius
  GLCD_pp = 2 + GLCD_fy2
  GLCD_pp = GLCD_pp + GLCD_rx2
  GLCD_pp = GLCD_pp - GLCD_pp_temp

  IF filled_ellipse = 0 THEN
   Draw_Ellipse_Points
  ELSE
   Draw_Filled_Ellipse_Points
  END IF

  DO WHILE GLCD_px < GLCD_py

    GLCD_xx++

    GLCD_px = GLCD_px + GLCD_ey2

    IF GLCD_pp_E.7 = 0 THEN
     GLCD_yy--
     GLCD_py = GLCD_py - GLCD_ex2
     GLCD_pp = GLCD_pp - GLCD_py
    END IF

    'GLCD_pp = GLCD_pp + GLCD_fy2 + GLCD_px
    GLCD_pp = GLCD_pp + GLCD_fy2
    GLCD_pp = GLCD_pp + GLCD_px

    IF filled_ellipse = 0 THEN
     Draw_Ellipse_Points
    ELSE
     Draw_Filled_Ellipse_Points
    END IF

  LOOP

  'GLCD_pp = 2 + GLCD_ry2 * (2 * GLCD_xx + 1) * (2 * GLCD_xx + 1) + GLCD_fx2 * (GLCD_yy - 1) * (GLCD_yy - 1) -  GLCD_fx2 * GLCD_ry2

  'change GLCD_xx and GLCD_yy to do calculation
  GLCD_xx = GLCD_xx + GLCD_xx
  GLCD_xx++
  GLCD_yy--
  GLCD_pp_temp = GLCD_xx * GLCD_xx
  GLCD_pp_temp = GLCD_ry2 * GLCD_pp_temp
  GLCD_pp = 2 + GLCD_pp_temp
  GLCD_pp_temp = GLCD_yy * GLCD_yy
  GLCD_pp_temp = GLCD_fx2 * GLCD_pp_temp
  GLCD_pp = GLCD_pp + GLCD_pp_temp
  GLCD_pp_temp = GLCD_fx2 * GLCD_ry2
  GLCD_pp = GLCD_pp - GLCD_pp_temp

  'restore original GLCD_xx and GLCD_yy values
  GLCD_xx--
  'GLCD_xx/2
  set C off
  rotate GLCD_xx_H right
  rotate GLCD_xx right
  GLCD_yy++

  DO WHILE GLCD_yy > 0

    GLCD_yy--

    GLCD_py = GLCD_py - GLCD_ex2

    IF GLCD_pp_E.7 = 1 THEN
     GLCD_xx++
     GLCD_px = GLCD_px + GLCD_ey2
     GLCD_pp = GLCD_pp + GLCD_px
    END IF

    'GLCD_pp = GLCD_pp + GLCD_fx2 - GLCD_py
    GLCD_pp = GLCD_pp + GLCD_fx2
    GLCD_pp = GLCD_pp - GLCD_py

    IF filled_ellipse = 0 THEN
     Draw_Ellipse_Points
    ELSE
     Draw_Filled_Ellipse_Points
    END IF

  LOOP

End Sub

Sub Draw_Ellipse_Points

  Pset ((xoffset + GLCD_xx), (yoffset + GLCD_yy), LineColour)
  Pset ((xoffset - GLCD_xx), (yoffset + GLCD_yy), LineColour)
  Pset ((xoffset - GLCD_xx), (yoffset - GLCD_yy), LineColour)
  Pset ((xoffset + GLCD_xx), (yoffset - GLCD_yy), LineColour)

End Sub

Sub Draw_Filled_Ellipse_Points

  FOR GLCD_yy1 = (yoffset) to (yoffset + 2 * GLCD_yy)
   Pset ((xoffset + GLCD_xx), (GLCD_yy1 - GLCD_yy), LineColour)
   Pset ((xoffset - GLCD_xx), (GLCD_yy1 - GLCD_yy), LineColour)
  NEXT

End Sub

'***************************************************************************************
'** Function name:           Triangle
'** Description:             Draw a triangle outline using 3 arbitrary points
'***************************************************************************************
' Draw a triangle
Sub Triangle(  in xoffset as word, in yoffset as word, in xoffset2 as word, in yoffset2 as word,in xoffset3 as word, in yoffset3 as word ,Optional In LineColour as word = GLCDForeground )
  Line(xoffset, yoffset,   xoffset2, yoffset2, LineColour)
  Line(xoffset2, yoffset2, xoffset3, yoffset3, LineColour)
  Line(xoffset3, yoffset3, xoffset,  yoffset,  LineColour)
End Sub

'***************************************************************************************
'** Function name:           FilledTriangle
'** Description:             Draw a filled triangle using 3 arbitrary points
'***************************************************************************************
' Fill a triangle
Sub FilledTriangle (  in xoffset as word, in yoffset as word, in xoffset2 as word, in yoffset2 as word,in xoffset3 as word, in yoffset3 as word ,Optional In LineColour as word = GLCDForeground )

  dim  GLCD_aa, GLCD_bb, GLCD_last as word

  ' Sort coordinates by Y order (yoffset3 >= yoffset2 >= yoffset)
  if (yoffset > yoffset2) then
    swap(yoffset, yoffset2)
    swap(xoffset, xoffset2)
  end if
  if (yoffset2 > yoffset3) then
    swap(yoffset3, yoffset2)
    swap(xoffset3, xoffset2)
  end if
  if (yoffset > yoffset2) then
    swap(yoffset, yoffset2)
    swap(xoffset, xoffset2)
  end if


  ' Handle awkward all-on-same-line case as its own thing
  if (yoffset = yoffset3) then

          GLCD_bb = xoffset
          GLCD_aa = xoffset

          if (xoffset2 < GLCD_aa ) Then
            GLCD_aa = xoffset2
          else if (xoffset2 > GLCD_bb ) then
            GLCD_bb = xoffset2
          end if

          if (xoffset3 < GLCD_aa) Then
            GLCD_aa = xoffset3
          else if ( xoffset3 > GLCD_bb) then
            GLCD_bb = xoffset3
          end if
          Line( GLCD_aa, yoffset, GLCD_bb - GLCD_aa + 1, yoffset, LineColour)
          exit sub
  end if

  dim dxoffset01, dyoffset01, dxoffset02, dyoffset02, dxoffset12, dyoffset12, glcd_y as word
  dim glcd_sa, glcd_sb as Integer
  dxoffset01 = xoffset2 - xoffset
  dyoffset01 = yoffset2 - yoffset
  dxoffset02 = xoffset3 - xoffset
  dyoffset02 = yoffset3 - yoffset
  dxoffset12 = xoffset3 - xoffset2
  dyoffset12 = yoffset3 - yoffset2
  glcd_sa   = 0
  glcd_sb   = 0

'
'   For upper part of triangle, find scanline crossings for segments
'   0-1 and 0-2.  If yoffset2=yoffset3 (flat-bottomed triangle), the scanline yoffset2
'   is included here (and second loop will be skipped, avoiding a /0
'   error there), otherwise scanline yoffset2 is skipped here and handled
'   in the second loop...which also avoids a /0 error here if yoffset=yoffset2
'   (flat-topped triangle).

  if ( yoffset2 = yoffset3) then
    GLCD_last = yoffset2 ;   Include yoffset2 scanline
  else
    GLCD_last = yoffset2 - 1;  Skip it
  end if

   for glcd_y = yoffset to glcd_last

    glcd_aa   = xoffset +  glcd_sa / dyoffset01
    glcd_bb   = xoffset +  glcd_sb / dyoffset02
    glcd_sa = glcd_sa + dxoffset01
    glcd_sb = glcd_sb + dxoffset02

    if (glcd_aa > glcd_bb)  then
      swap(glcd_aa, glcd_bb)
    end if

    Line(glcd_aa, glcd_y, glcd_bb , glcd_y, LineColour)
   next

'   For lower part of triangle, find scanline crossings for segments
'   0-2 and 1-2.  This loop is skipped if yoffset2=yoffset3.
   glcd_sa = dxoffset12 * (glcd_y - yoffset2);
   glcd_sb = dxoffset02 * (glcd_y - yoffset);
   for glcd_y = glcd_last to yoffset3
    glcd_aa   = xoffset2 +  glcd_sa / dyoffset12
    glcd_bb   = xoffset +  glcd_sb / dyoffset02
    glcd_sa = glcd_sa + dxoffset12
    glcd_sb = glcd_sb + dxoffset02
      if (glcd_aa > glcd_bb)  then
        swap(glcd_aa, glcd_bb)
      end if
     Line(glcd_aa, glcd_y, glcd_bb , glcd_y, LineColour)
    next
End Sub
