'    Graphical LCD routines for the GCBASIC compiler
'    Copyright (C) 2012 - 2015 Hugh Considine and Evan Venn

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
'    9/11/14	New revised version.  Requires GLCD.H.  Do not call hardware files directly.  Always load via GLCD.H
'

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
     FilledBox = FilledBox_SSD1306
     Pset = Pset_SSD1306
     glcd_type_string = "SSD1306"
     GLCD_WIDTH = 128
     GLCD_HEIGHT = 64
     SSD1306_GLCD_HEIGHT = GLCDDeviceHeight
     SSD1306_GLCD_WIDTH = GLCDDeviceWidth

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
     Line =  Line_SSD1289
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

	'Loads extended fonts set ASCII characters 31-254
  'Requires 1358 bytes of program memory
  If GLCD_EXTENDEDFONTSET1 then

     GLCDCharCol3 = GLCDCharCol3Extended1
     GLCDCharCol4 = GLCDCharCol4Extended1
     GLCDCharCol5 = GLCDCharCol5Extended1
     GLCDCharCol6 = GLCDCharCol6Extended1
     GLCDCharCol7 = GLCDCharCol7Extended1

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



'''Draws a string at the specified location on the ST7920 GLCD
'''@param StringLocX X coordinate for message
'''@param CharLocY Y coordinate for message
'''@param Chars String to display
'''@param LineColour Line Color, either 1 or 0
Sub GLCDDrawString( In StringLocX, In CharLocY, In Chars as string, Optional In LineColour as word = GLCDForeground )
    Dim TargetCharCol as Word
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
Sub GLCDDrawChar(In CharLocX as word, In CharLocY as word, In CharCode, Optional In LineColour as word = GLCDForeground )

	'CharCode needs to have 16 subtracted, table starts at char 16 not char 0
	CharCode -= 15

	'invert colors if required

          if LineColour <> GLCDForeground  then
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
'''@param xradius radius of circle
'''@param LineColour Colour of line (0 = blank, 1 = show, default is 1)
'''@param yordinate (optional) rounding
#define GCLDCircle Circle
sub Circle ( in xoffset as word, in yoffset as word, in xradius as integer, Optional In LineColour as word = GLCDForeground , Optional In yordinate = GLCD_yordinate)


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
sub FilledCircle( in xoffset as word, in yoffset as word, in xradius as word, Optional In LineColour as word = GLCDForeground)

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
0     '134 ¢
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
0
248   '193 Á
254   '194 B
254   '195 Ã
248   '196 Ä
254   '197 E
194   '198 Z
254   '199 H
124   '200 È
0     '201 É
254   '202 Ê
248   '203 Ë
254   '204 Ì
254   '205 Í
130   '206 Î
124   '207 Ï
254   '208 Ð
254   '209 Ñ
0
198   '211 Ó
2     '212 Ô
14    '213 Õ
56    '214 Ö
198   '215 ×
14    '216 Ø
188   '217 Ù
0
0
56    '220 Ü
40    '221 Ý
124   '222 Þ
0     '223 ß
0
56    '225 á
254   '226 â
4     '227 ã
48    '228 ä
40    '229 å
1     '230 æ
124   '231 ç
48    '232 è
0     '233 é
124   '234 ê
64    '235 ë
252   '236 ì
28    '237 í
0     '238 î
56    '239 ï
4     '240 ð
248   '241 ñ
24    '242 ò
56    '243 ó
4     '244 ô
60    '245 õ
56    '246 ö
68    '247 ÷
60    '248 ø
60    '249 ù
0     '250 ú
60    '251 û
56    '252 ü
60    '253 ý
60    '254 þ
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
36    'Á
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
36    '193 Á
146   '194 B
2     '195 Ã
132   '196 Ä
146   '197 E
162   '198 Z
16    '199 H
146   '200 È
130   '201 É
16    '202 Ê
4     '203 Ë
4     '204 Ì
8     '205 Í
146   '206 Î
130   '207 Ï
2     '208 Ð
18    '209 Ñ
0
170   '211 Ó
2     '212 Ô
16    '213 Õ
68    '214 Ö
40    '215 ×
16    '216 Ø
194   '217 Ù
0
0
68    '220 Ü
84    '221 Ý
8     '222 Þ
0     '223 ß
0
68    '225 á
73    '226 â
104   '227 ã
73    '228 ä
84    '229 å
25    '230 æ
8     '231 ç
74    '232 è
0     '233 é
16    '234 ê
50    '235 ë
64    '236 ì
32    '237 í
1     '238 î
68    '239 ï
124   '240 ð
36    '241 ñ
36    '242 ò
68    '243 ó
4     '244 ô
64    '245 õ
68    '246 ö
40    '247 ÷
64    '248 ø
64    '249 ù
1     '250 ú
65    '251 û
68    '252 ü
64    '253 ý
64    '254 þ
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
34   '193 Á
146  '194 B
2    '195 Ã
130  '196 Ä
146  '197 E
146  '198 Z
16   '199 H
146  '200 È
254  '201 É
40   '202 Ê
2    '203 Ë
24   '204 Ì
16   '205 Í
146  '206 Î
130  '207 Ï
2    '208 Ð
18   '209 Ñ
0
146  '211 Ó
254  '212 Ô
224  '213 Õ
254  '214 Ö
16   '215 ×
254  '216 Ø
2    '217 Ù
0
0
69   '220 Ü
84   '221 Ý
5    '222 Þ
61   '223 ß
0
68    '225 á
73    '226 â
144   '227 ã
75    '228 ä
84    '229 å
165   '230 æ
4     '231 ç
77    '232 è
60    '233 é
40    '234 ê
9     '235 ë
64    '236 ì
64    '237 í
149   '238 î
68    '239 ï
4     '240 ð
36    '241 ñ
164   '242 ò
68    '243 ó
124   '244 ô
64    '245 õ
254   '246 ö
16    '247 ÷
248   '248 ø
56    '249 ù
60    '250 ú
64    '251 û
69    '252 ü
66    '253 ý
58    '254 þ
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
36   '193 Á
146  '194 B
2    '195 Ã
132  '196 Ä
146  '197 E
138  '198 Z
16   '199 H
146  '200 È
130  '201 É
68   '202 Ê
4    '203 Ë
4    '204 Ì
32   '205 Í
146  '206 Î
130  '207 Ï
2    '208 Ð
18   '209 Ñ
0
130  '211 Ó
2    '212 Ô
16   '213 Õ
68   '214 Ö
40   '215 ×
16   '216 Ø
194  '217 Ù
0
0
56   '220 Ü
85   '221 Ý
4    '222 Þ
64   '223 ß
0
56    '225 á
78    '226 â
104   '227 ã
77    '228 ä
84    '229 å
67    '230 æ
4     '231 ç
73    '232 è
64    '233 é
68    '234 ê
9     '235 ë
32    '236 ì
32    '237 í
171   '238 î
68    '239 ï
124   '240 ð
36    '241 ñ
164   '242 ò
76    '243 ó
4     '244 ô
64    '245 õ
68    '246 ö
40    '247 ÷
64    '248 ø
64    '249 ù
65    '250 ú
65    '251 û
68    '252 ü
65    '253 ý
65    '254 þ
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
248    '193 Á
108    '194 B
2      '195 Ã
248    '196 Ä
130    '197 E
134    '198 Z
254    '199 H
124    '200 È
0      '201 É
130    '202 Ê
248    '203 Ë
254    '204 Ì
254    '205 Í
130    '206 Î
124    '207 Ï
254    '208 Ð
12     '209 Ñ
0
130    '211 Ó
2      '212 Ô
14     '213 Õ
56     '214 Ö
198    '215 ×
14     '216 Ø
188    '217 Ù
0
0
68     '220 Ü
68     '221 Ý
120    '222 Þ
32     '223 ß
0
68    '225 á
48    '226 â
4     '227 ã
49    '228 ä
68    '229 å
1     '230 æ
120   '231 ç
62    '232 è
32    '233 é
0     '234 ê
126   '235 ë
124   '236 ì
28    '237 í
65    '238 î
56    '239 ï
4     '240 ð
24    '241 ñ
72    '242 ò
52    '243 ó
4     '244 ô
60    '245 õ
56    '246 ö
68    '247 ÷
60    '248 ø
60    '249 ù
32    '250 ú
60    '251 û
56    '252 ü
60    '253 ý
60    '254 þ
End Table
