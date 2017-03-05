'    Graphical LCD routines for the GCBASIC compiler
'    Copyright (C) 2015 - 2017 Evan Venn and Kent Schafer

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
' Supports SSD1306 controller only.

'Hardware settings using I2C
'  #define GLCD_TYPE GLCD_TYPE_SSD1306
'  #define GLCD_I2C_Address 0x78
'Hardware settings using SPI (4Wire,MOSI(D1),SCK(D0),DC,CS)
'  #define GLCD_TYPE GLCD_TYPE_SSD1306
'  #define S4Wire_Data

' 1.01 Added scrollspeed
' 1.02 Add soft SPI (Kent Schafer)
' 1.03 Revised S4Wire_Data to cater for i2c or hwi2c being used at same time as SPI
' 1.04 Revised for performance only. No functional changes


#define SSD1306_SETCONTRAST 0x81
#define SSD1306_DISPLAYALLON_RESUME 0xA4
#define SSD1306_DISPLAYALLON 0xA5
#define SSD1306_NORMALDISPLAY 0xA6
#define SSD1306_INVERTDISPLAY 0xA7
#define SSD1306_DISPLAYOFF 0xAE
#define SSD1306_DISPLAYON 0xAF

#define SSD1306_SETDISPLAYOFFSET 0xD3
#define SSD1306_SETCOMPINS 0xDA

#define SSD1306_SETVCOMDETECT 0xDB

#define SSD1306_SETDISPLAYCLOCKDIV 0xD5
#define SSD1306_SETPRECHARGE 0xD9

#define SSD1306_SETMULTIPLEX 0xA8

#define SSD1306_SETLOWCOLUMN 0x00
#define SSD1306_SETHIGHCOLUMN 0x10

#define SSD1306_SETSTARTLINE 0x40

#define SSD1306_MEMORYMODE 0x20
#define SSD1306_COLUMNADDR 0x21
#define SSD1306_PAGEADDR   0x22

#define SSD1306_COMSCANINC 0xC0
#define SSD1306_COMSCANDEC 0xC8

#define SSD1306_SEGREMAP 0xA0

#define SSD1306_CHARGEPUMP 0x8D

#define SSD1306_EXTERNALVCC 0x01
#define SSD1306_SWITCHCAPVCC 0x02

' Scrolling #defines
#define SSD1306_ACTIVATE_SCROLL 0x2F
#define SSD1306_DEACTIVATE_SCROLL 0x2E
#define SSD1306_SET_VERTICAL_SCROLL_AREA 0xA3
#define SSD1306_RIGHT_HORIZONTAL_SCROLL 0x26
#define SSD1306_LEFT_HORIZONTAL_SCROLL 0x27
#define SSD1306_VERTICAL_AND_RIGHT_HORIZONTAL_SCROLL 0x29
#define SSD1306_VERTICAL_AND_LEFT_HORIZONTAL_SCROLL 0x2A

#startup InitGLCD_SSD1306

'Setup code for SSD1306 controllers
#if GLCD_TYPE = GLCD_TYPE_SSD1306
    dim SSD1306_BufferLocationCalc as Word               ' mandated in main program for SSD1306

       If ChipRAM > 1024  Then
         Dim SSD1306_BufferAlias(1024)
       End if

#endif

'''@hide
Sub Write_Command_SSD1306 ( in SSD1306SendByte as byte )

    #ifdef S4Wire_DATA

      CS_SSD1306 = 0
      DC_SSD1306 = 0
      S4Wire_SSD1306 SSD1306SendByte
      DC_SSD1306 = 1
      CS_SSD1306 = 1
      Exit Sub

    #endif

    #ifdef I2C_DATA

      I2CStart
      I2CSend GLCD_I2C_Address
      I2CSend 0x00
      I2CSend SSD1306SendByte
      I2CStop

    #endif

    #ifdef HI2C_DATA

      HI2CStart
      HI2CSend GLCD_I2C_Address
      HI2CSend 0x00
      HI2CSend SSD1306SendByte
      HI2CStop

    #endif

End Sub

'''@hide
Sub Write_Data_SSD1306 ( in SSD1306SendByte as byte )

    #ifdef S4Wire_DATA

      CS_SSD1306 = 0
      DC_SSD1306 = 1
      S4Wire_SSD1306 SSD1306SendByte
      DC_SSD1306 = 0
      CS_SSD1306 = 1
      Exit Sub

    #endif

    #ifdef I2C_DATA

      I2CStart
      I2CSend GLCD_I2C_Address
      I2CSend 0x40
      I2CSend SSD1306SendByte
      I2CStop

    #endif

    #ifdef HI2C_DATA

      HI2CStart
      HI2CSend GLCD_I2C_Address
      HI2CSend 0x40
      HI2CSend SSD1306SendByte
      HI2CStop

    #endif


End Sub




'''@hide
Sub InitGLCD_SSD1306
    #IFDEF HI2C_DATA
           HIC2INIT
    #ENDIF
    #ifdef S4Wire_DATA
      dir MOSI_SSD1306 Out
      dir SCK_SSD1306 Out
      dir DC_SSD1306 Out
      dir CS_SSD1306 Out
      dir RES_SSD1306 Out
      RES_SSD1306 = 0
      wait 10 us
      RES_SSD1306 = 1
    #endif
    'Setup code for SSD1306 controllers
    'Init sequence for 128x64 OLED module
    Write_Command_SSD1306(SSD1306_DISPLAYOFF)                    ' 0xAE
    Write_Command_SSD1306(SSD1306_DEACTIVATE_SCROLL)
    Write_Command_SSD1306(SSD1306_SETDISPLAYCLOCKDIV)            ' 0xD5
    Write_Command_SSD1306(0x80)                                  ' the suggested ratio 0x80
    Write_Command_SSD1306(SSD1306_SETMULTIPLEX)                  ' 0xA8
    Write_Command_SSD1306(0x3F)
    Write_Command_SSD1306(SSD1306_SETDISPLAYOFFSET)              ' 0xD3
    Write_Command_SSD1306(0x00)                                   ' no offset
    Write_Command_SSD1306(SSD1306_SETSTARTLINE | 0x00)            ' line #0
    Write_Command_SSD1306(SSD1306_CHARGEPUMP)                    ' 0x8D
    if (vccstate = SSD1306_EXTERNALVCC) then
      Write_Command_SSD1306(0x10)
    else
      Write_Command_SSD1306(0x14)
    end if
    Write_Command_SSD1306(SSD1306_MEMORYMODE)                    ' 0x20
    'Write_Command_SSD1306(0x00)                                  ' 0x00 act like ks0108 - DO NOT SELECT!!
    Write_Command_SSD1306(0x10)                                  ' 0x01 act like PCD8544

    Write_Command_SSD1306(SSD1306_SEGREMAP | 0x1)
    Write_Command_SSD1306(SSD1306_COMSCANDEC)
    Write_Command_SSD1306(SSD1306_SETCOMPINS)                    ' 0xDA
    Write_Command_SSD1306(0x12)
    Write_Command_SSD1306(SSD1306_SETCONTRAST)                   ' 0x81
    if vccstate = SSD1306_EXTERNALVCC then
      Write_Command_SSD1306(0x9F)
    else
      Write_Command_SSD1306(0xCF)
    end if
    Write_Command_SSD1306(SSD1306_SETPRECHARGE)                  ' 0xd9
    if vccstate = SSD1306_EXTERNALVCC then
      Write_Command_SSD1306(0x22)
    else
      Write_Command_SSD1306(0xF1)
    end if
    Write_Command_SSD1306(SSD1306_SETVCOMDETECT)                 ' 0xDB
    Write_Command_SSD1306(0x40)
    Write_Command_SSD1306(SSD1306_DISPLAYALLON_RESUME)           ' 0xA4
    Write_Command_SSD1306(SSD1306_NORMALDISPLAY)                 ' 0xA6
    Write_Command_SSD1306(SSD1306_DISPLAYON)                     '--turn on oled panel


    'Colours
    GLCDBackground = 0
    GLCDForeground = 1
    GLCDFontWidth = 6
    GLCDfntDefault = 0
    GLCDfntDefaultsize = 1

    'Clear screen
    GLCDCLS_SSD1306

End Sub



'''Clears the GLCD screen
Sub GLCDCLS_SSD1306
 ' initialise global variable. Required variable for Circle in all DEVICE DRIVERS- DO NOT DELETE
  GLCD_yordinate = 0

  #ifndef GLCD_TYPE_SSD1306_CHARACTER_MODE_ONLY
    For SSD1306_BufferLocationCalc = 1 to 1024
        SSD1306_BufferAlias(SSD1306_BufferLocationCalc) = 0
    Next
  #endif

  for SSD1306_BufferLocationCalc = 0 to 63 step 8
      for GLCDTemp = 0 to 127
          Cursor_Position_SSD1306 ( GLCDTemp , SSD1306_BufferLocationCalc )
          Write_Data_SSD1306(GLCDBackground)
    Next
  next

  Cursor_Position_SSD1306 ( 0 , 0 )

End Sub

'''Draws a filled box on the GLCD screen
'''@param LineX1 Top left corner X location
'''@param LineY1 Top left corner Y location
'''@param LineX2 Bottom right corner X location
'''@param LineY2 Bottom right corner Y location
'''@param LineColour Colour of box (0 = erase, 1 = draw, default is 1)
Sub FilledBox_SSD1306(In LineX1, In LineY1, In LineX2, In LineY2, Optional In LineColour As Word = GLCDForeground)

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

  #if GLCD_TYPE = GLCD_TYPE_SSD1306
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
'''@param GLCDColour State of pixel ( GLCDBackground | GLCDForeground )
Sub PSet_SSD1306(In GLCDX, In GLCDY, In GLCDColour As Word)

    'Set pixel at X, Y on LCD to State
    'X is 0 to 127
    'Y is 0 to 63
    'Origin in top left

  #if GLCD_TYPE = GLCD_TYPE_SSD1306



          'SSD1306_BufferLocationCalc = ( GLCDY / 8 )* GLCD_WIDTH
          'faster than /8
          SSD1306_BufferLocationCalc = GLCDY
          Repeat 3
            Set C Off
            Rotate SSD1306_BufferLocationCalc Right
          End Repeat
          SSD1306_BufferLocationCalc = SSD1306_BufferLocationCalc * GLCD_WIDTH

          SSD1306_BufferLocationCalc = GLCDX + SSD1306_BufferLocationCalc+1
          GLCDDataTemp = SSD1306_BufferAlias(SSD1306_BufferLocationCalc)

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

          SSD1306_BufferAlias(SSD1306_BufferLocationCalc) = GLCDDataTemp
          Cursor_Position_SSD1306 ( GLCDX, GLCDY )
          Write_Data_SSD1306 ( GLCDDataTemp )

  #endif

End Sub


'''Takes raw pixel positions and translates to XY char positions
'''@param X coordinate of pixel
'''@param Y coordinate of pixel
sub Cursor_Position_SSD1306( in LocX as byte, in LocY as byte )
  dim  PosCharX, PosCharX as Word


  ' PosCharY = LocY / 8
  ' faster than /8
  PosCharY = LocY
  Repeat 3
    Set C Off
    Rotate PosCharY Right
  End Repeat


  Write_Command_SSD1306( 0xB0 + PosCharY )   ' set page address
  PosCharX = ( LocX  & 0x0f )  ' lower nibble
  Write_Command_SSD1306( PosCharX )

  PosCharX = LocX
  Repeat 4
         Set C off
         Rotate PosCharX Right
  End Repeat
  PosCharX = ( PosCharX & 0x0F ) + 0x10
  Write_Command_SSD1306 ( PosCharX )

end sub


'''Stops all scrolling
sub stopscroll_SSD1306
    Write_Command_SSD1306(SSD1306_DEACTIVATE_SCROLL)
end sub


'''Activate a right handed scroll for rows start through stop
''' Hint, the display is 16 rows tall. To scroll the whole display, run:
''' startscrollright_SSD1306(0x00, 0x0F)
'''@param Start row
'''@param End row
'''@param Set time interval between each scroll step in terms of frame frequency
SUB startscrollright_SSD1306 ( IN start , IN stop, Optional In scrollspeed As byte = 0 )
  Write_Command_SSD1306(SSD1306_RIGHT_HORIZONTAL_SCROLL)
  Write_Command_SSD1306(0X00)
  Write_Command_SSD1306(start)
  Write_Command_SSD1306(scrollspeed)
  Write_Command_SSD1306(stop)
  Write_Command_SSD1306(0X00)
  Write_Command_SSD1306(0XFF)
  Write_Command_SSD1306(SSD1306_ACTIVATE_SCROLL)
end sub



'''Activate a left handed scroll for rows start through stop
'''Hint, the display is 16 rows tall. To scroll the whole display, run:
'''startscrollleft_SSD1306(0x00, 0x0F)
'''@param Start row
'''@param End row
'''@param Set time interval between each scroll step in terms of frame frequency
SUB startscrollleft_SSD1306 ( IN start , IN stop, Optional In scrollspeed As byte = 0 )
  Write_Command_SSD1306(SSD1306_LEFT_HORIZONTAL_SCROLL)
  Write_Command_SSD1306(0X00)
  Write_Command_SSD1306(start)
  Write_Command_SSD1306(scrollspeed)
  Write_Command_SSD1306(stop)
  Write_Command_SSD1306(0X00)
  Write_Command_SSD1306(0XFF)
  Write_Command_SSD1306(SSD1306_ACTIVATE_SCROLL)
end sub



'''Activate a diagright handed scroll for rows start through stop
'''Hint, the display is 16 rows tall. To scroll the whole display, run:
'''startscrolldiagright_SSD1306(0x00, 0x0F)
'''@param Start row
'''@param End row
'''@param Set time interval between each scroll step in terms of frame frequency
SUB startscrolldiagright_SSD1306 ( IN start , IN stop, Optional In scrollspeed As byte = 0 )
  Write_Command_SSD1306(SSD1306_SET_VERTICAL_SCROLL_AREA)
  Write_Command_SSD1306(0X00)
  Write_Command_SSD1306(GLCD_HEIGHT)
  Write_Command_SSD1306(SSD1306_VERTICAL_AND_RIGHT_HORIZONTAL_SCROLL)
  Write_Command_SSD1306(scrollspeed)
  Write_Command_SSD1306(start)
  Write_Command_SSD1306(0X00)
  Write_Command_SSD1306(stop)
  Write_Command_SSD1306(0X01)
  Write_Command_SSD1306(SSD1306_ACTIVATE_SCROLL)
end sub


'''Activate a diagleft handed scroll for rows start through stop
'''Hint, the display is 16 rows tall. To scroll the whole display, run:
'''startscrolldiagleft_SSD1306(0x00, 0x0F)
'''@param Start row
'''@param End row
'''@param Set time interval between each scroll step in terms of frame frequency
SUB startscrolldiagleft_SSD1306 ( IN start , IN stop, Optional In scrollspeed As byte = 0 )
  Write_Command_SSD1306(SSD1306_SET_VERTICAL_SCROLL_AREA)
  Write_Command_SSD1306(0X00)
  Write_Command_SSD1306(GLCD_HEIGHT)
  Write_Command_SSD1306(SSD1306_VERTICAL_AND_LEFT_HORIZONTAL_SCROLL)
  Write_Command_SSD1306(scrollspeed)
  Write_Command_SSD1306(start)
  Write_Command_SSD1306(0X00)
  Write_Command_SSD1306(stop)
  Write_Command_SSD1306(0X01)
  Write_Command_SSD1306(SSD1306_ACTIVATE_SCROLL)
end sub

'''Sets the constrast to select 1 out of 256 contrast steps.
'''Contrast increases as the value increases.
'''@param Dim byte value
sub SetContrast_SSD1306 ( in dim_state )

  Write_Command_SSD1306(SSD1306_SETCONTRAST)
  Write_Command_SSD1306(dim_state)

end sub

'*********************Software SPI**********************

sub S4Wire_SSD1306(in SSD1306SendByte as byte)

  For clocks = 1 to 8
    If SSD1306SendByte.7 = 1 Then
      MOSI_SSD1306 = 1
    Else
      MOSI_SSD1306 = 0
    End if
    Rotate SSD1306SendByte Left Simple
    SCK_SSD1306 = 0
    SCK_SSD1306 = 1
  Next

end sub
