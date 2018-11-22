'    Graphical LCD routines for the GCBASIC compiler
'    Copyright (C) 2012 - 2018 Hugh Considine and Evan Venn

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
' Supports NT7108C controller only.

'Changes
'  19/11/18  Initial release
'  22/11/18  Revised to improve timings and read byte



'Hardware settings
'Type
'''@hardware All; Controller Type; GLCD_TYPE; "GLCD_TYPE_NT7108C"
'Parallel lines (NT7108C only)
'''@hardware GLCD_TYPE GLCD_TYPE_NT7108C; Data Bus; GLCD_DATA_PORT; IO_Port
'Control lines (NT7108C only)
'''@hardware GLCD_TYPE GLCD_TYPE_NT7108C; Chip Select 1; GLCD_CS1; IO_Pin
'''@hardware GLCD_TYPE GLCD_TYPE_NT7108C; Chip Select 2; GLCD_CS2; IO_Pin
'''@hardware GLCD_TYPE GLCD_TYPE_NT7108C; Register Select; GLCD_RS; IO_Pin
'''@hardware GLCD_TYPE GLCD_TYPE_NT7108C; Read/Write; GLCD_RW; IO_Pin
'''@hardware GLCD_TYPE GLCD_TYPE_NT7108C; Enable; GLCD_ENABLE; IO_Pin

'''@hardware GLCD_TYPE GLCD_TYPE_NT7108C; Reset; GLCD_RESET; IO_Pin

#startup InitGLCD_NT7108C

Sub InitGLCD_NT7108C

  'Setup code for NT7108C controllers
  #if GLCD_TYPE = GLCD_TYPE_NT7108C
     ' Timing for 32 mhz device - typically you can use the defaults and not state these constants
     #define NT7108CReadDelay    7      ; = 7 normal usage, 5 or above is OK at 32 mhz!
     #define NT7108CWriteDelay   7      ; = 7 normal usage you may get away with other lower values
     #define NT7108CClockDelay   7      ; = 7 normal usage you may get away with other lower values
     #define GLCDDirection     0     ; 0 normal mode

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
    Set GLCD_CS1 Off
    Set GLCD_CS2 Off
    'Reset
    Set GLCD_RESET Off
    Wait 1 ms
    Set GLCD_RESET On
    Wait 1 ms
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
    GLCDFontWidth = 5
    GLCDfntDefault = 0
    GLCDfntDefaultsize = 1

  #endif

  'Clear screen
  GLCDCLS_NT7108C

End Sub



'Subs
'''Clears the GLCD screen
Sub GLCDCLS_NT7108C
          ' initialise global variable. Required variable for Circle in all DEVICE DRIVERS- DO NOT DELETE
          GLCD_yordinate = 0

  #if GLCD_TYPE = GLCD_TYPE_NT7108C
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

End Sub

'''Draws a filled box on the GLCD screen
'''@param LineX1 Top left corner X location
'''@param LineY1 Top left corner Y location
'''@param LineX2 Bottom right corner X location
'''@param LineY2 Bottom right corner Y location
'''@param LineColour Colour of box (0 = erase, 1 = draw, default is 1)
Sub FilledBox_NT7108C(In LineX1, In LineY1, In LineX2, In LineY2, Optional In LineColour As Word = GLCDForeground)

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

  #if GLCD_TYPE = GLCD_TYPE_NT7108C
    'Draw lines going across
    For DrawLine = LineX1 To LineX2
      For GLCDTemp = LineY1 To LineY2
        PSet DrawLine, GLCDTemp, LineColour
      Next
    Next
  #endif

End Sub

'''Draws a pixel on the GLCD
'''@param GLCDX X coordinate of pixel
'''@param GLCDY Y coordinate of pixel
'''@param GLCDColour State of pixel (0 = erase, 1 = display)
Sub PSet_NT7108C(In GLCDX, In GLCDY, In GLCDColour As Word)

  #if GLCD_TYPE = GLCD_TYPE_NT7108C

    GLCDY = GLCDY and GLCD_HEIGHT - 1

    GLCDX = GLCDX and GLCD_WIDTH - 1

    'Set pixel at X, Y on LCD to State
    'X is 0 to 127
    'Y is 0 to 63
    'Origin in top left

    #ifdef  GLCDDirection 1
      if GLCDDirection=1 then
        GLCDX=127-GLCDX
        GLCDY=63-GLCDY
      end if
    #endif

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

    'Select column
    Set GLCD_RS Off
    GLCDWriteByte 64 Or GLCDX
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

End Sub

#define GLCDWriteByte GLCDWriteByte_NT7108C
'''Write byte to LCD
'''@hide
Sub GLCDWriteByte_NT7108C (In LCDByte)


  'Set LCD data direction
  Set GLCD_RW Off
  'Set data port direction
  dir GLCD_DATA_PORT out
  'Set output data
  GLCD_DATA_PORT = LCDByte
  'Latch data
  Wait NT7108CWriteDelay us
  Set GLCD_ENABLE On
  Wait NT7108CClockDelay us
  Set GLCD_ENABLE Off
'  Wait NT7108CWriteDelay us
End Sub

#define GLCDReadByte GLCDReadByte_NT7108C
'''Read byte from LCD
'''@hide
Function GLCDReadByte_NT7108C

  'Set data port direction
  dir GLCD_DATA_PORT in

  'Set LCD data direction
  Set GLCD_RW On
  'Read
  Set GLCD_ENABLE On
  Wait NT7108CClockDelay  US
  'Get input data
  Set GLCD_ENABLE Off
  Wait NT7108CReadDelay  US
  GLCDReadByte = GLCD_DATA_PORT

End Function
