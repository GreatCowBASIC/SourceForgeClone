'    Graphical LCD routines for the GCBASIC compiler
'    Copyright (C) 2017 Marco Cariboni and Evan Venn

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
' Supports SH1106 controller only.

'Hardware settings uses I2C
'#define GLCD_I2C_Address 0x78 '8 bit addresses
'#define GLCD_I2C_Address 0x7A
'#define GLCD_I2C_Address 0x7B


#define SH1106_OLEDADJUSTMENT 0x02  ' required as the 132 bits are not all available.

#define SH1106_DISPLAYOFF 0xAE
#define SH1106_SETLOWCOLUMN 0x00
#define SH1106_SETHIGHCOLUMN 0x10
#define SH1106_SETSTARTLINE 0x40
#define SH1106_PAGEADDR   0xB0
#define SH1106_SETCONTRASTCRTL 0x81
#define SH1106_SEGREMAP 0xA1
#define SH1106_NORMALDISPLAY 0xA6
#define SH1106_INVERTDISPLAY 0xA7
#define SH1106_SETMULTIPLEX 0xA8
#define SH1106_CHARGEPUMP 0xAD
#define SH1106_EXTERNALVCC 0x8B
#define SH1106_PUMPVOLTAGE 0x30
#define SH1106_COMSCANDEC 0xC8
#define SH1106_SETDISPLAYOFFSET 0xD3
#define SH1106_SETDISPLAYCLOCKDIV 0xD5
#define SH1106_SETPRECHARGE 0xD9
#define SH1106_SETCOMPINS 0xDA
#define SH1106_SETVCOMDETECT 0xDB
#define SH1106_DISPLAYON 0xAF
#define SH1106_DISPLAYALLON_RESUME 0xA4
#define SH1106_DISPLAYALLON 0xA5


#startup InitGLCD_SH1106


'Setup code for SH1106 controllers
#if GLCD_TYPE = GLCD_TYPE_SH1106

    dim SH1106_BufferLocationCalc as Word               ' mandated in main program for SH1106

       If ChipRAM > 1024  Then
         Dim SH1106_BufferAlias(1024)
       End if

#endif

'''@hide
Sub Write_Command_SH1106 ( in SH1106SendByte as byte )

    #ifdef I2C_DATA

      I2CStart
      I2CSend GLCD_I2C_Address
      I2CSend 0x00
      I2CSend SH1106SendByte
      I2CStop

    #endif

    #ifdef HI2C_DATA
      HI2CStart
      HI2CSend GLCD_I2C_Address
      HI2CSend 0x00
      HI2CSend SH1106SendByte
      HI2CStop

    #endif

End Sub

'''@hide
Sub Write_Data_SH1106 ( in SH1106SendByte as byte )

    #ifdef I2C_DATA
      I2CStart
      I2CSend GLCD_I2C_Address
      I2CSend 0x40
      I2CSend SH1106SendByte
      I2CStop

    #endif

    #ifdef HI2C_DATA
      HI2CStart
      HI2CSend GLCD_I2C_Address
      HI2CSend 0x40
      HI2CSend SH1106SendByte
      HI2CStop

    #endif
End Sub


Sub InitGLCD_SH1106

debug = 0

    #IFDEF HI2C_DATA
           HIC2INIT
    #ENDIF

   'Setup code for SH1106 controllers
    'Init sequence for 132x64 OLED module

    Write_Command_SH1106(SH1106_DISPLAYOFF)                    ' 0xAE
    Write_Command_SH1106(SH1106_SETLOWCOLUMN)                  ' 0x02
    Write_Command_SH1106(SH1106_SETHIGHCOLUMN)                 ' 0x10
    Write_Command_SH1106(SH1106_SETSTARTLINE)                  ' 0x40
    Write_Command_SH1106(SH1106_PAGEADDR)                      ' 0xB0

    Write_Command_SH1106(SH1106_SETCONTRASTCRTL)               ' 0x81
    Write_Command_SH1106(0x80)                   ' Contrast = 128

    Write_Command_SH1106(SH1106_SEGREMAP)            ' 0xA1
    Write_Command_SH1106(SH1106_NORMALDISPLAY)                 ' 0xA6

    Write_Command_SH1106(SH1106_SETMULTIPLEX)                  ' 0xA8
    Write_Command_SH1106(0x3F)                   ' Duty = 1/32

    Write_Command_SH1106(SH1106_CHARGEPUMP)                    ' 0xAD
    Write_Command_SH1106(SH1106_EXTERNALVCC)                   ' 0x8B
    Write_Command_SH1106(SH1106_PUMPVOLTAGE)           ' 0x30
    Write_Command_SH1106(SH1106_COMSCANDEC)            ' 0xC8

    Write_Command_SH1106(SH1106_SETDISPLAYOFFSET)              ' 0xD3
    Write_Command_SH1106(0x00)                                ' No offset(Y position)
    Write_Command_SH1106(0x00)                                ' No offset(Y position)

    Write_Command_SH1106(SH1106_SETDISPLAYCLOCKDIV)            ' 0xD5
    Write_Command_SH1106(0x50)                                 ' Suggested ratio 0x50

    Write_Command_SH1106(SH1106_SETPRECHARGE)                  ' 0xd9
    Write_Command_SH1106(0x22)

    Write_Command_SH1106(SH1106_SETCOMPINS)                    ' 0xDA
    Write_Command_SH1106(0x12)

    Write_Command_SH1106(SH1106_SETVCOMDETECT)                 ' 0xDB
    Write_Command_SH1106(0x40)


    Write_Command_SH1106(SH1106_DISPLAYALLON_RESUME)           ' 0xA4
    Write_Command_SH1106(SH1106_DISPLAYON)                     ' 0xAF Turn ON oled panel


    'Colours
    GLCDBackground = 0
    GLCDForeground = 1
    GLCDFontWidth = 6
    GLCDfntDefault = 0
    GLCDfntDefaultsize = 1

    'Clear screen
    GLCDCLS_SH1106

End Sub


'''Clears the GLCD screen
Sub GLCDCLS_SH1106
 ' initialise global variable. Required variable for Circle in all DEVICE DRIVERS- DO NOT DELETE
  GLCD_yordinate = 0

  #ifndef GLCD_TYPE_SH1106_CHARACTER_MODE_ONLY
    For SH1106_BufferLocationCalc = 1 to ( GLCD_WIDTH * GLCD_HEIGHT / 8 )
        SH1106_BufferAlias(SH1106_BufferLocationCalc) = 0
    Next
  #endif

  for SH1106_BufferLocationCalc = 0 to ( GLCD_HEIGHT - 1 ) step 8
      for GLCDTemp = 0 to ( GLCD_WIDTH - 1 )
          Cursor_Position_SH1106 ( GLCDTemp , SH1106_BufferLocationCalc )
          Write_Data_SH1106(GLCDBackground)
    Next
  next

  Cursor_Position_SH1106 ( 0 , 0 )

End Sub

'''Draws a filled box on the GLCD screen
'''@param LineX1 Top left corner X location
'''@param LineY1 Top left corner Y location
'''@param LineX2 Bottom right corner X location
'''@param LineY2 Bottom right corner Y location
'''@param LineColour Colour of box (0 = erase, 1 = draw, default is 1)
Sub FilledBox_SH1106(In LineX1, In LineY1, In LineX2, In LineY2, Optional In LineColour As Word = GLCDForeground)

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

  #if GLCD_TYPE = GLCD_TYPE_SH1106
    'Draw lines going across
    For DrawLine = LineX1 To LineX2
      For GLCDTemp = LineY1 To LineY2
        PSet_SH1106 DrawLine, GLCDTemp, LineColour
      Next
    Next
  #endif

End Sub

'''Draws a pixel on the GLCD
'''@param GLCDX X coordinate of pixel
'''@param GLCDY Y coordinate of pixel
'''@param GLCDColour State of pixel ( GLCDBackground | GLCDForeground )
Sub PSet_SH1106(In GLCDX, In GLCDY, In GLCDColour As Word)

    'Set pixel at X, Y on LCD to State
    'X is 0 to 127
    'Y is 0 to 63
    'Origin in top left

  #if GLCD_TYPE = GLCD_TYPE_SH1106

          dim  PosCharX, PosCharY as Word

          'optimised /8 calculation
          PosCharY = GLCDY
          Repeat 3
            Set C Off
            Rotate PosCharY Right
          end Repeat

          'optimes * 128 calculation
          #if GLCD_WIDTH = 128
            Repeat 7
              Set C Off
              Rotate PosCharY Left
            end Repeat
            SH1106_BufferLocationCalc = PosCharY
          #endif

          #if GLCD_WIDTH <> 128
              'optimised /8 calculation
              PosCharY = GLCDY
              Repeat 3
                Set C Off
                Rotate PosCharY Right
              end Repeat
              SH1106_BufferLocationCalc = PosCharY * GLCD_WIDTH
          #endif

          'unoptimised is shown on line below.  All the code above... just makes this faster!
          ' SH1106_BufferLocationCalc = ( GLCDY / 8 )* GLCD_WIDTH

          SH1106_BufferLocationCalc = GLCDX + SH1106_BufferLocationCalc+1
          GLCDDataTemp = SH1106_BufferAlias(SH1106_BufferLocationCalc)

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

          SH1106_BufferAlias(SH1106_BufferLocationCalc) = GLCDDataTemp
          Cursor_Position_SH1106 ( GLCDX , GLCDY )
          Write_Data_SH1106 ( GLCDDataTemp )

  #endif

End Sub


'''Takes raw pixel positions and translates to XY char positions
'''@param X coordinate of pixel
'''@param Y coordinate of pixel
sub Cursor_Position_SH1106( in LocX as byte, in LocY as byte )

  dim  PosCharX, PosCharY as Word

  'PosCharY = LocY / 8 - optimised by rotate
  PosCharY = LocY
  Repeat 3
    Set C Off
    Rotate PosCharY Right
  end Repeat

  Write_Command_SH1106( 0xB0 + PosCharY )   ' set page address
  PosCharX = ( ( LocX + SH1106_OLEDADJUSTMENT ) & 0x0f )  ' lower nibble
  Write_Command_SH1106( PosCharX )

  PosCharX = LocX + SH1106_OLEDADJUSTMENT
  Repeat 4
         Set C off
         Rotate PosCharX Right
  End Repeat
  PosCharX = ( PosCharX & 0x0F ) + 0x10
  Write_Command_SH1106 ( PosCharX )

end sub

'''Sets the constrast to select 1 out of 256 contrast steps.
'''Contrast increases as the value increases.
'''@param Dim byte value

sub GLCDSetContrast_SSH1106 ( in dim_state )

  Write_Command_SH1106(SH1106_SETCONTRAST)
  Write_Command_SH1106(dim_state)

end sub


'''Set Display in normal mode
sub GLCDSetDisplayNormalMode_SSH1106
    Write_Command_SH1106(SH1106_SEGREMAP)            ' 0xA1
    Write_Command_SH1106(SH1106_NORMALDISPLAY)       ' 0xA6
end sub

'''Set Display in invert mode
sub GLCDSetDisplayInvertMode_SSH1106
    Write_Command_SH1106(SH1106_SEGREMAP)            ' 0xA1
    Write_Command_SH1106(SH1106_INVERTDISPLAY)       ' 0xA7
end sub
