'    Graphical LCD routines for the GCBASIC compiler
'    Copyright (C) 2017 Evan Venn

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
' Supports ILI9486L controller only.

'Changes
' 21/04/2017:      Initial release

'
'Hardware settings
'Type
'''@hardware All; Controller Type; GLCD_TYPE; "GLCD_TYPE_ILI9486L"

'Serial lines (ILI9486L only)
'''@hardware GLCD_TYPE GLCD_TYPE_ILI9486L; Data/Command; GLCD_DC; IO_Pin
'''@hardware GLCD_TYPE GLCD_TYPE_ILI9486L; Chip Select; GLCD_CS; IO_Pin
'''@hardware GLCD_TYPE GLCD_TYPE_ILI9486L; Data In (LCD Out) GLCD_DI; IO_Pin
'''@hardware GLCD_TYPE GLCD_TYPE_ILI9486L; Data Out (LCD In) GLCD_DO; IO_Pin
'''@hardware GLCD_TYPE GLCD_TYPE_ILI9486L; Clock; GLCD_SCK; IO_Pin

'''@hardware GLCD_TYPE GLCD_TYPE_ILI9486L; Reset; GLCD_RESET; IO_Pin

'Pin mappings for ILI9486L
#define ILI9486L_DC GLCD_DC
#define ILI9486L_CS GLCD_CS
#define ILI9486L_RST GLCD_RESET

#define ILI9486L_DI GLCD_DI
#define ILI9486L_DO GLCD_DO
#define ILI9486L_SCK GLCD_SCK



#define ILI9486L_NOP     0x00
#define ILI9486L_SWRESET 0x01
#define ILI9486L_RDDID   0x04
#define ILI9486L_RDDST   0x09

#define ILI9486L_SLPIN   0x10
#define ILI9486L_SLPOUT  0x11
#define ILI9486L_PTLON   0x12
#define ILI9486L_NORON   0x13

#define ILI9486L_RDMODE  0x0A
#define ILI9486L_RDMADCTL  0x0B
#define ILI9486L_RDPIXFMT  0x0C
#define ILI9486L_RDIMGFMT  0x0D
#define ILI9486L_RDSELFDIAG  0x0F

#define ILI9486L_INVOFF  0x20
#define ILI9486L_INVON   0x21
#define ILI9486L_GAMMASET 0x26
#define ILI9486L_DISPOFF 0x28
#define ILI9486L_DISPON  0x29

#define ILI9486L_CASET   0x2A
#define ILI9486L_PASET   0x2B
#define ILI9486L_RAMWR   0x2C
#define ILI9486L_RAMRD   0x2E

#define ILI9486L_PTLAR   0x30
#define ILI9486L_VSCRDEF 0x33
#define ILI9486L_MADCTL  0x36

#define ILI9486L_MADCTL_MY  0x80
#define ILI9486L_MADCTL_MX  0x40
#define ILI9486L_MADCTL_MV  0x20
#define ILI9486L_MADCTL_ML  0x10
#define ILI9486L_MADCTL_RGB 0x00
#define ILI9486L_MADCTL_BGR 0x08
#define ILI9486L_MADCTL_MH  0x04

#define ILI9486L_PIXFMT  0x3A

#define ILI9486L_FRMCTR1 0xB1
#define ILI9486L_FRMCTR2 0xB2
#define ILI9486L_FRMCTR3 0xB3
#define ILI9486L_INVCTR  0xB4
#define ILI9486L_DFUNCTR 0xB6

#define ILI9486L_PWCTR1  0xC0
#define ILI9486L_PWCTR2  0xC1
#define ILI9486L_PWCTR3  0xC2
#define ILI9486L_PWCTR4  0xC3
#define ILI9486L_PWCTR5  0xC4
#define ILI9486L_VMCTR1  0xC5
#define ILI9486L_VMCTR2  0xC7

#define ILI9486L_RDID1   0xDA
#define ILI9486L_RDID2   0xDB
#define ILI9486L_RDID3   0xDC
#define ILI9486L_RDID4   0xDD

#define ILI9486L_GMCTRP1 0xE0
#define ILI9486L_GMCTRN1 0xE1

#define ILI9486L_PWCTR6  0xFC

#define ILI9486L_WHITE   0xFFFF - TFT_WHITE
#define ILI9486L_BLACK   0xFFFF - TFT_BLACK
#define ILI9486L_RED     0xFFFF - TFT_RED
#define ILI9486L_GREEN   0xFFFF - TFT_GREEN
#define ILI9486L_BLUE    0xFFFF - TFT_BLUE
#define ILI9486L_YELLOW  0xFFFF - TFT_YELLOW
#define ILI9486L_CYAN    0xFFFF - TFT_CYAN
#define ILI9486L_MAGENTA 0xFFFF - TFT_MAGENTA
#define ILI9486L_NAVY          0xFFFF - TFT_NAVY
#define ILI9486L_DARKGREEN     0xFFFF - TFT_DARKGREEN
#define ILI9486L_DARKCYAN      0xFFFF - TFT_DARKCYAN
#define ILI9486L_MAROON        0xFFFF - TFT_MAROON
#define ILI9486L_PURPLE        0xFFFF - TFT_PURPLE
#define ILI9486L_OLIVE         0xFFFF - TFT_OLIVE
#define ILI9486L_LIGHTGREY     0xFFFF - TFT_LIGHTGREY
#define ILI9486L_DARKGREY      0xFFFF - TFT_DARKGREY
#define ILI9486L_ORANGE        0xFFFF - TFT_ORANGE
#define ILI9486L_GREENYELLOW   0xFFFF - TFT_GREENYELLOW
#define ILI9486L_PINK          0xFFFF - TFT_PINK




#startup InitGLCD_ILI9486L




'''Initialise the GLCD device
Sub InitGLCD_ILI9486L

  '  Mapped to global variables to same RAM
  dim ILI9486L_GLCD_HEIGHT, ILI9486L_GLCD_WIDTH as word


  'Setup code for ILI9486L controllers
  #if GLCD_TYPE = GLCD_TYPE_ILI9486L

    'Pin directions
    Dir ILI9486L_CS Out
    Dir ILI9486L_DC Out
    Dir ILI9486L_RST Out

    Dir ILI9486L_DI In
    Dir ILI9486L_DO Out
    Dir ILI9486L_SCK Out

    #ifdef ILI9486L_HardwareSPI
      ' harware SPI mode
      SPIMode MasterFast, 0
    #endif

   Set ILI9486L_CS On
   Set ILI9486L_DC On


    'Reset display
    Wait 50 ms
    Set ILI9486L_RST On
    Wait 5 ms
    'Reset sequence (lower line for at least 10 us)
    Set ILI9486L_RST Off
    Wait 20 us
    Set ILI9486L_RST On
    Wait 150 ms

    SendCommand_ILI9486L(0xC0)
    SendData_ILI9486L(0x08)
    SendData_ILI9486L(0x06)

    SendCommand_ILI9486L(0xC1)
    SendData_ILI9486L(0x43)

    SendCommand_ILI9486L(0XC5)
    SendData_ILI9486L(0x00)
    SendData_ILI9486L(0x40)

    SendCommand_ILI9486L(0xC2)
    SendData_ILI9486L(0x33)

    SendCommand_ILI9486L(0xB1)
    SendData_ILI9486L(0xB0)
    SendData_ILI9486L(0x11)

    SendCommand_ILI9486L(0xB4)
    SendData_ILI9486L(0x02)

    SendCommand_ILI9486L(0xB7)
    SendData_ILI9486L(0x86)

    SendCommand_ILI9486L(0xE0)
    SendData_ILI9486L(0x0F)
    SendData_ILI9486L(0x29)
    SendData_ILI9486L(0x25)
    SendData_ILI9486L(0x0B)
    SendData_ILI9486L(0x0E)
    SendData_ILI9486L(0x07)
    SendData_ILI9486L(0x42)
    SendData_ILI9486L(0x87)
    SendData_ILI9486L(0x2C)
    SendData_ILI9486L(0x06)
    SendData_ILI9486L(0x0F)
    SendData_ILI9486L(0x02)
    SendData_ILI9486L(0x0B)
    SendData_ILI9486L(0x07)
    SendData_ILI9486L(0x00)

    SendCommand_ILI9486L(0XE1)
    SendData_ILI9486L(0x0F)
    SendData_ILI9486L(0x38)
    SendData_ILI9486L(0x34)
    SendData_ILI9486L(0x0D)
    SendData_ILI9486L(0x10)
    SendData_ILI9486L(0x09)
    SendData_ILI9486L(0x53)
    SendData_ILI9486L(0x87)
    SendData_ILI9486L(0x3D)
    SendData_ILI9486L(0x08)
    SendData_ILI9486L(0x11)
    SendData_ILI9486L(0x04)
    SendData_ILI9486L(0x1A)
    SendData_ILI9486L(0x16)
    SendData_ILI9486L(0x00)

    SendCommand_ILI9486L(0XF2)
    SendData_ILI9486L(0x1E)
    SendData_ILI9486L(0xA3)
    SendData_ILI9486L(0x32)
    SendData_ILI9486L(0x02)
    SendData_ILI9486L(0xB2)
    SendData_ILI9486L(0x52)
    SendData_ILI9486L(0xFF)
    SendData_ILI9486L(0x10)
    SendData_ILI9486L(0x00)

    SendCommand_ILI9486L(0XF8)
    SendData_ILI9486L(0x21)
    SendData_ILI9486L(0x04)

    SendCommand_ILI9486L(0x36)
    SendData_ILI9486L(0x08) 'was 08 optionally 48

    SendCommand_ILI9486L(0x3A)
    SendData_ILI9486L(0x55)

    SendCommand_ILI9486L(0xF9)
    SendData_ILI9486L(0x00)
    SendData_ILI9486L(0x08)

    SendCommand_ILI9486L(0XF1)
    SendData_ILI9486L(0x36)
    SendData_ILI9486L(0x04)
    SendData_ILI9486L(0x00)
    SendData_ILI9486L(0x3C)
    SendData_ILI9486L(0X0F)
    SendData_ILI9486L(0x8F)
    '/**********set rgb interface mode******************/
    SendCommand_ILI9486L(0xB6)
    SendData_ILI9486L(0x00) '30 set rgb, was 00
    SendData_ILI9486L(0x22) 'GS,SS 02,42,62 was 22
    SendData_ILI9486L(0x3B)
    ' /**************************************************/
    SendCommand_ILI9486L(0x2A) 'Frame rate control
    SendData_ILI9486L(0x00)
    SendData_ILI9486L(0x00)
    SendData_ILI9486L(0x01)
    SendData_ILI9486L(0x3F)

    SendCommand_ILI9486L(0x2B) 'Display function control
    SendData_ILI9486L(0x00)
    SendData_ILI9486L(0x00)
    SendData_ILI9486L(0x01)
    SendData_ILI9486L(0xDF)

    SendCommand_ILI9486L(0x21)

    SendCommand_ILI9486L(0x11)
    wait 120 ms
    SendCommand_ILI9486L(0x29) ' display on
    SendCommand_ILI9486L(0x2c)
    'Default Colours
    GLCDBackground = ILI9486L_BLACK
    GLCDForeground = ILI9486L_WHITE

    'Variables required for device
    ILI9486L_GLCD_WIDTH = GLCD_WIDTH
    ILI9486L_GLCD_HEIGHT = GLCD_HEIGHT
    GLCDFontWidth = 6
    GLCDfntDefault = 0
    GLCDfntDefaultsize = 2

  #endif

End Sub


Sub DSTB_ILI9486L( Optional In PowerMode = On  )

  if PowerMode = Off Then

       SendCommand_ILI9486L( ILI9486L_DISPOFF )
       SendCommand_ILI9486L( ILI9486L_SLPIN )

    Else

       SendCommand_ILI9486L( ILI9486L_SLPOUT )
       wait 120 ms
       SendCommand_ILI9486L( ILI9486L_DISPON )

    End if


End Sub

'Subs
'''Clears the GLCD screen
Sub GLCDCLS_ILI9486L ( Optional In  GLCDBackground as word = GLCDBackground )

    dim ILI9486LSendWord as word
    ' initialise global variable. Required variable for Circle in all DEVICE DRIVERS- DO NOT DELETE
    GLCD_yordinate = 0

    SetAddressWindow_ILI9486L ( 0, 0, ILI9486L_GLCD_WIDTH -1 , ILI9486L_GLCD_HEIGHT-1 )
    ILI9486LSendWord = GLCDBackground

    set ILI9486L_CS OFF
    set ILI9486L_DC ON
    repeat ILI9486L_GLCD_WIDTH

      repeat ILI9486L_GLCD_HEIGHT

        #ifdef ILI9486L_HardwareSPI
'         Could use these as an alternative
'         FastHWSPITransfer  ILI9486LSendWord_h
'         FastHWSPITransfer  ILI9486LSendWord

          #ifdef PIC
            #ifndef Var(SSPCON1)
              #ifdef Var(SSPCON)
                Dim SSPCON1 Alias SSPCON
              #endif
            #endif
            'Clear WCOL
            Set SSPCON1.WCOL Off
            'Put byte to send into buffer
            'Will start transfer
            SSPBUF = ILI9486LSendWord_h
            Wait While SSPSTAT.BF = Off
            Set SSPSTAT.BF Off
            #if ChipFamily 16
              ILI9486LTempOut = SSPBUF
            #endif


            'Clear WCOL
            Set SSPCON1.WCOL Off
            'Put byte to send into buffer
            'Will start transfer
            SSPBUF = ILI9486LSendWord
            Wait While SSPSTAT.BF = Off
            Set SSPSTAT.BF Off
            #if ChipFamily 16
              ILI9486LTempOut = SSPBUF
            #endif

          #endif
          #ifdef AVR
            FastHWSPITransfer  ILI9486LSendWord_h
            FastHWSPITransfer  ILI9486LSendWord
          #endif

        #endif

        #ifndef ILI9486L_HardwareSPI
          SendWord_ILI9486L ( GLCDBackground )
        #endif
      end repeat

    end repeat
    set ILI9486L_CS ON;


End Sub

'''Draws a string at the specified location on the ST7920 GLCD
'''@param StringLocX X coordinate for message
'''@param CharLocY Y coordinate for message
'''@param Chars String to display
'''@param LineColour Line Color, either 1 or 0
Sub GLCDDrawString_ILI9486L( In StringLocX as word, In CharLocY as word, In Chars as string, Optional In LineColour as word = GLCDForeground )
    dim TargetCharCol as word
    for xchar = 1 to Chars(0)
      ' June 2014
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
Sub GLCDDrawChar_ILI9486L(In CharLocX as word, In CharLocY as word, In CharCode, Optional In LineColour as word = GLCDForeground )


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


End Sub


'''Draws a filled box on the GLCD screen
'''@param LineX1 Top left corner X location
'''@param LineY1 Top left corner Y location
'''@param LineX2 Bottom right corner X location
'''@param LineY2 Bottom right corner Y location
'''@param LineColour Colour of box (0 = erase, 1 = draw, default is 1)
Sub FilledBox_ILI9486L(In LineX1 as word, In LineY1 as word, In LineX2 as word, In LineY2 as word, Optional In LineColour As Word = GLCDForeground)
  dim GLCDTemp as word
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


    'Set address window
'   SetAddress_ILI9486L ILI9486L_COLUMN, LineX1, LineX2
'   SetAddress_ILI9486L ILI9486L_ROW, LineY1, LineY2
    SetAddressWindow_ILI9486L (  LineX1, LineY1, LineX2, LineY2 )
    'Fill with colour
    Dim GLCDPixelCount As Word
    GLCDPixelCount = (LineX2 - LineX1 + 1) * (LineY2 - LineY1 + 1)
    Repeat GLCDPixelCount
      SendWord_ILI9486L LineColour
    End Repeat

End Sub

'''Draws a pixel on the GLCD
'''@param GLCDX X coordinate of pixel
'''@param GLCDY Y coordinate of pixel
'''@param GLCDColour State of pixel
Sub PSet_ILI9486L(In GLCDX as word, In GLCDY as word, In GLCDColour As Word)

    SetAddressWindow_ILI9486L ( GLCDX, GLCDY, GLCDX, GLCDY )
    SendWord_ILI9486L GLCDColour

End Sub




'''Send a command to the ILI9486L GLCD
'''@param ILI9486LSendByte Command to send
'''@hide
sub  SendCommand_ILI9486L( IN ILI9486LSendByte as byte )

  set ILI9486L_CS OFF;
  set ILI9486L_DC OFF;

  #ifdef ILI9486L_HardwareSPI
     SPITransfer  ILI9486LSendByte,  ILI9486LTempOut
     set ILI9486L_CS ON;
     exit sub
  #endif

  #ifndef ILI9486L_HardwareSPI
  repeat 8

    if ILI9486LSendByte.7 = ON  then
      set ILI9486L_DO ON;
    else
      set ILI9486L_DO OFF;
    end if
    SET GLCD_SCK On;
    rotate ILI9486LSendByte left
    set GLCD_SCK Off;

  end repeat
  set ILI9486L_CS ON;
  #endif

end Sub

'''Send a data byte to the ILI9486L GLCD
'''@param ILI9486LSendByte Byte to send
'''@hide
sub  SendData_ILI9486L( IN ILI9486LSendByte as byte )

  set ILI9486L_CS OFF;
  set ILI9486L_DC ON;

  #ifdef ILI9486L_HardwareSPI
     SPITransfer  ILI9486LSendByte,  ILI9486LTempOut
     set ILI9486L_CS ON;
     exit sub
  #endif

  #ifndef ILI9486L_HardwareSPI
  repeat 8

    if ILI9486LSendByte.7 = ON then
      set ILI9486L_DO ON;
    else
      set ILI9486L_DO OFF;
    end if
    SET GLCD_SCK On;
    rotate ILI9486LSendByte left
    set GLCD_SCK Off;

  end Repeat
  set ILI9486L_CS ON;
  #endif

end Sub

'''Send a data word (16 bits) to the ILI9486L GLCD
'''@param ILI9486LSendByte Word to send
'''@hide
Sub SendWord_ILI9486L(In ILI9486LSendWord As Word)
  set ILI9486L_CS OFF;
  set ILI9486L_DC ON;

  #ifdef ILI9486L_HardwareSPI
     SPITransfer  ILI9486LSendWord_H,  ILI9486LTempOut
     SPITransfer  ILI9486LSendWord,  ILI9486LTempOut
     set ILI9486L_CS ON;
     exit sub
  #endif

  #ifndef ILI9486L_HardwareSPI
  repeat 16

    if ILI9486LSendWord.15 = ON then
      set ILI9486L_DO ON;
    else
      set ILI9486L_DO OFF;
    end if
    SET GLCD_SCK On;
    rotate ILI9486LSendWord left
    set GLCD_SCK Off;

  end repeat
  set ILI9486L_CS ON;
  #endif
End Sub


'''Set the row or column address range for the ILI9486L GLCD
'''@param ILI9486LAddressType Address Type (ILI9486L_ROW or ILI9486L_COLUMN)
'''@param ILI9486LStart Starting address
'''@param ILI9486LEnd Ending address
'''@hide
Sub SetAddressWindow_ILI9486L( In _x1 as word, in _y1 as word, in _x2 as word, in _y2 as word)

  SendCommand_ILI9486L(ILI9486L_CASET)  'Column addr set
  SendData_ILI9486L _x1_H
  SendData_ILI9486L _x1
  SendData_ILI9486L _x2_H
  SendData_ILI9486L _x2

  SendCommand_ILI9486L(ILI9486L_PASET)  'Row addr set
  SendData_ILI9486L _y1_H
  SendData_ILI9486L _y1
  SendData_ILI9486L _y2_H
  SendData_ILI9486L _y2

  SendCommand_ILI9486L(ILI9486L_RAMWR)  'write to RAM

End Sub



Sub SetCursorPosition_ILI9486L( In _x1 as word, in _y1 as word, in _x2 as word, in _y2 as word)
  SendCommand_ILI9486L(ILI9486L_CASET)  Column addr set
  SendData_ILI9486L _x1_H
  SendData_ILI9486L _x1
  SendData_ILI9486L _x2_H
  SendData_ILI9486L _x2

  SendCommand_ILI9486L(ILI9486L_PASET)  Row addr set
  SendData_ILI9486L _y1_H
  SendData_ILI9486L _y1
  SendData_ILI9486L _y2_H
  SendData_ILI9486L _y2

End Sub


''@hide
sub   GLCDRotate_ILI9486L ( in ILI9341AddressType as byte )


  select case ILI9341AddressType
        case PORTRAIT

              SendCommand_ILI9486L(0xB6)
              SendData_ILI9486L(0x00)
              SendData_ILI9486L(0x22)
              SendData_ILI9486L(0x3B)

        'unsupported
'        case LANDSCAPE

              'unsupported

        case PORTRAIT_REV

              SendCommand_ILI9486L(0xB6)
              SendData_ILI9486L(0x00)
              SendData_ILI9486L(0x42)
              SendData_ILI9486L(0x3B)


        'unsupported
'        case LANDSCAPE_REV

              'unsupported

        case else
              SendCommand_ILI9486L(0xB6)
              SendData_ILI9486L(0x00)
              SendData_ILI9486L(0x22)
              SendData_ILI9486L(0x3B)

  end select

end sub

'''Displays a string in a larger fonti.e.  BigPrint_ILI9486L ( 30, ILI9486L_GLCD_HEIGHT - 20 , "BigPrint_ILI9486L" )
'''@param PrintLocX X coordinate for message
'''@param PrintLocY Y coordinate for message
'''@param PrintData String to display
'''@param Color Optional color
Sub BigPrint_ILI9486L(In PrintLocX as Word, In PrintLocY as Word,  PrintData As String, Optional In  Color as word = GLCDForeground)
  Dim GLCDPrintLoc as word
  PrintLen = PrintData(0)
  If PrintLen = 0 Then Exit Sub
  GLCDPrintLoc = PrintLocX
  For SysPrintTemp = 1 To PrintLen
    DrawBigChar_ILI9486L GLCDPrintLoc, PrintLocY, PrintData(SysPrintTemp), Color
    GLCDPrintLoc += 13
  Next
End Sub

'''Draws a Vertical Line on the GLCD with a
'''@param GLCDY1 Y coordinate of one end of the line
'''@param GLCDY2 Y coordinate of the other end of the line
'''@param GLCDX1 X coordinate of the line
'''@param LineColour color
Sub DrawBigChar_ILI9486L (In CharLocX as Word, In CharLocY as Word, In CharCode, Optional In  Color as word = GLCDForeground )

    dim Locx, Locy, CurrCol as Word


    if CharCode <=126 Then
       CharCode -=32
       Goto GCBBigTables
    end if
    if CharCode <=210 Then
       CharCode -=33
       Goto GCBBigTables
    end if
    if CharCode <= 250 Then
       CharCode -=34
       Goto GCBBigTables
    end if
    GCBBigTables:
    For CurrCharCol = 1 to 24
      CurrCol=CurrCharCol+CharCode*24-(CharCode/10)*240
      if CharCode>=0 and CharCode<=9 then ReadTable BigFont32_41 , CurrCol, CurrCharVal
      if CharCode>=10 and CharCode<=19 then ReadTable BigFont42_51 , CurrCol, CurrCharVal
      if CharCode>=20 and CharCode<=29 then ReadTable BigFont52_61 , CurrCol, CurrCharVal
      if CharCode>=30 and CharCode<=39 then ReadTable BigFont62_71 , CurrCol, CurrCharVal
      if CharCode>=40 and CharCode<=49 then ReadTable BigFont72_81 , CurrCol, CurrCharVal
      if CharCode>=50 and CharCode<=59 then ReadTable BigFont82_91 , CurrCol, CurrCharVal
      if CharCode>=60 and CharCode<=69 then ReadTable BigFont92_101 , CurrCol, CurrCharVal
      if CharCode>=70 and CharCode<=79 then ReadTable BigFont102_111 , CurrCol, CurrCharVal
      if CharCode>=80 and CharCode<=89 then ReadTable BigFont112_121 , CurrCol, CurrCharVal
      if CharCode>=90 and CharCode<=99 then ReadTable BigFont122_126 , CurrCol, CurrCharVal
      if CharCode>=160 and CharCode<=169 then ReadTable BigFont193_202 , CurrCol, CurrCharVal
      if CharCode>=170 and CharCode<=179 then ReadTable BigFont203_212 , CurrCol, CurrCharVal
      if CharCode>=180 and CharCode<=183 then ReadTable BigFont213_216 , CurrCol, CurrCharVal

        if CurrCharVal=36 then CurrCharVal=33
        For CurrCharRow = 1 to 8
          LocX=[word]CharLocX+CurrCharCol
          LocY=[Word]CharLocY+CurrCharRow
          if CurrCharCol>12 then
             LocX= LocX - 12
             LocY= LocY + 8
          end if
          if CurrCharVal.0=1 then
             PSet_ILI9486L LocX , LocY , Color
          end if
          Rotate CurrCharVal Right
        Next
    Next
End Sub
