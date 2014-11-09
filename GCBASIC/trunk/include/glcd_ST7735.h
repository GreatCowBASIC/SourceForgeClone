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
' Supports ST7735 controller only.

'Changes
' 29/9/2013: Added ST7735 code (and colour support)
' 14/4/2014: Added ST7920 Support
' 22/4/2014: Revised OLD_GLCD_TYPE_ST7920_GRAPHICS_MODE variable name
' 15/10/2014: Adapted for color support for ST7735
'             Added Defines and ST7735Rotation command
' 16/10/2014: Adapted to handle screen rotation and GLCDCLS for ST7735 device.
' 27/10/2014: Changed ST7735Rotation to GLCDRotate
'    
' 9/11/14	New revised version.  Requires GLCD.H.  Do not call directly.  Always load via GLCD.H
'
'Hardware settings
'Type
'''@hardware All; Controller Type; GLCD_TYPE; "GLCD_TYPE_ST7735"

'Serial lines (ST7735 only)
'''@hardware GLCD_TYPE GLCD_TYPE_ST7735; Data/Command; GLCD_DC; IO_Pin
'''@hardware GLCD_TYPE GLCD_TYPE_ST7735; Chip Select; GLCD_CS; IO_Pin
'''@hardware GLCD_TYPE GLCD_TYPE_ST7735; Data In (LCD Out); GLCD_DI; IO_Pin
'''@hardware GLCD_TYPE GLCD_TYPE_ST7735; Data Out (LCD In); GLCD_DO; IO_Pin
'''@hardware GLCD_TYPE GLCD_TYPE_ST7735; Clock; GLCD_SCK; IO_Pin

'''@hardware GLCD_TYPE GLCD_TYPE_ST7735; Reset; GLCD_RESET; IO_Pin

'Pin mappings for ST7735
#define ST7735_DC GLCD_DC
#define ST7735_CS GLCD_CS
#define ST7735_RST GLCD_RESET

' lat support
#define _ST7735_DC _GLCD_DC
#define _ST7735_CS _GLCD_CS
#define _ST7735_RST _GLCD_RESET

#define ST7735_DI GLCD_DI
#define ST7735_DO GLCD_DO
#define ST7735_SCK GLCD_SCK

' lat support
#define _ST7735_DI _GLCD_DI
#define _ST7735_DO _GLCD_DO
#define _ST7735_SCK _GLCD_SCK

'Column/row select commands for ST7735
#define ST7735_COLUMN 0x2A
#define ST7735_ROW 0x2B

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

#startup InitGLCD_ST7735


dim ST7735_GLCD_WIDTH, ST7735_GLCD_HEIGHT as byte


'''Initialise the GLCD device
Sub InitGLCD_ST7735

	
	'Setup code for ST7735 controllers
	#if GLCD_TYPE = GLCD_TYPE_ST7735
		'Pin directions
                    #IFNDEF GLCD_LAT
                        Dir ST7735_CS Out
                        Dir ST7735_DC Out
                        Dir ST7735_RST Out
    		
                        Dir ST7735_DI In
                        Dir ST7735_DO Out
                        Dir ST7735_SCK Out
                    #endif


                    #IFDEF GLCD_LAT
                        Dir _ST7735_CS Out
                        Dir _ST7735_DC Out
                        Dir _ST7735_RST Out
    		
                        Dir _ST7735_DI In
                        Dir _ST7735_DO Out
                        Dir _ST7735_SCK Out
                    #endif
		
                    #ifdef ST7735_HardwareSPI
                      ' harware SPI mode
                      SPIMode MasterFast, 0
		#endif

		'Reset display
		Set ST7735_RST On
		Wait 10 ms
		'Reset sequence (lower line for at least 10 us)
		Set ST7735_RST Off
		Wait 25 us
		Set ST7735_RST On
		Wait 10 ms
		'Software reset
		SendCommand_ST7735 ST7735_SWRESET
		Wait 200 ms

		'Software reset
		SendCommand_ST7735 ST7735_SWRESET
		Wait 200 ms		
		
                    'Out of sleep mode
		SendCommand_ST7735 ST7735_SLPOUT
		Wait 200 ms

                    SendCommand_ST7735(ST7735_COLMOD)   ; set color mode
                    SendData_ST7735(0x05)               ; 16-bit color
		Wait 10 ms

                    SendCommand_ST7735(ST7735_FRMCTR1); // frame rate control
                    SendData_ST7735(0x00);          // fastest refresh
                    SendData_ST7735(0x06);          // 6 lines front porch
                    SendData_ST7735(0x03);          // 3 lines backporch
                    Wait 10 ms

                    SendCommand_ST7735(ST7735_MADCTL);  // memory access control (directions)
                    SendData_ST7735(0xC8);          // row address/col address, bottom to top refresh
                    wait 10 ms

                    SendCommand_ST7735(ST7735_DISSET5); // display settings #5
                    SendData_ST7735(0x15);          // 1 clock cycle nonoverlap, 2 cycle gate rise, 3 cycle oscil. equalize
                    SendData_ST7735(0x02);          // fix on VTL
                    wait 10 ms;
                    SendCommand_ST7735(ST7735_INVCTR);  // display inversion control
                    SendData_ST7735(0x0);           // line inversion
                    wait 10 ms;
                    SendCommand_ST7735(ST7735_PWCTR1);  // power control
                    SendData_ST7735(0x02);          // GVDD = 4.7V
                    SendData_ST7735(0x70);          // 1.0uA
                    wait 10 ms;

'                    SendCommand_ST7735(ST7735_PWCTR2);  // power control
'                    SendData_ST7735(0x05);          // VGH = 14.7V, VGL = -7.35V
'                    SendCommand_ST7735(ST7735_PWCTR3);  // power control
'                    SendData_ST7735(0x01);          // Opamp current small
'                    SendData_ST7735(0x02);          // Boost frequency
'                    wait 10 ms;
'                    SendCommand_ST7735(ST7735_VMCTR1);  // power control
'                    SendData_ST7735(0x3C);          // VCOMH = 4V
'                    SendData_ST7735(0x38);          // VCOML = -1.1V
'                    wait 10 ms;

                    SendCommand_ST7735(ST7735_GMCTRP1);
                    SendData_ST7735(0x09);
                    SendData_ST7735(0x16);
                    SendData_ST7735(0x09);
                    SendData_ST7735(0x20);
                    SendData_ST7735(0x21);
                    SendData_ST7735(0x1B);
                    SendData_ST7735(0x13);
                    SendData_ST7735(0x19);
                    SendData_ST7735(0x17);
                    SendData_ST7735(0x15);
                    SendData_ST7735(0x1E);
                    SendData_ST7735(0x2B);
                    SendData_ST7735(0x04);
                    SendData_ST7735(0x05);
                    SendData_ST7735(0x02);
                    SendData_ST7735(0x0E);
                    wait 10 ms;
                    SendCommand_ST7735(ST7735_GMCTRN1);
                    SendData_ST7735(0x0B);
                    SendData_ST7735(0x14);
                    SendData_ST7735(0x08);
                    SendData_ST7735(0x1E);
                    SendData_ST7735(0x22);
                    SendData_ST7735(0x1D);
                    SendData_ST7735(0x18);
                    SendData_ST7735(0x1E);
                    SendData_ST7735(0x1B);
                    SendData_ST7735(0x1A);
                    SendData_ST7735(0x24);
                    SendData_ST7735(0x2B);
                    SendData_ST7735(0x06);
                    SendData_ST7735(0x06);
                    SendData_ST7735(0x02);
                    SendData_ST7735(0x0F);
                    wait 10 ms;

                    SendCommand_ST7735(ST7735_PWCTR6);  // power control
                    SendData_ST7735(0x11);
                    SendData_ST7735(0x15);
                    wait 10 ms;	

                    	
		'Gamma correction
		'SetGammaCorrection_ST7735
		
		'Scanning direction
		SendCommand_ST7735 0x36
		SendData_ST7735 0xC8
		wait 10 ms;
		'Set pixel mode to 16 bpp
		SendCommand_ST7735 0x3A
		SendData_ST7735 5
		wait 10 ms;
		'Display on
		SendCommand_ST7735 0x29
		Wait 100 ms
		
		'Colours
		GLCDBackground = ST7735_WHITE
		GLCDForeground = ST7735_BLACK

                    'Variables required for device
                    ST7735_GLCD_WIDTH = GLCD_WIDTH
                    ST7735_GLCD_HEIGHT = GLCD_HEIGHT
                    GLCDFontWidth = 6
                    GLCDfntDefault = 0
	#endif

          GLCDRotate ( PORTRAIT_REV )
	'Clear screen
	GLCDCLS
	
End Sub


'Subs
'''Clears the GLCD screen
Sub GLCDCLS_ST7735
          ' initialise global variable. Required variable for Circle in all DEVICE DRIVERS- DO NOT DELETE
          GLCD_yordinate = 0

	#if GLCD_TYPE = GLCD_TYPE_ST7735
		SetAddress_ST7735 ST7735_COLUMN, 0, ST7735_GLCD_WIDTH
                    wait 2 ms
                    SetAddress_ST7735 ST7735_ROW, 0, ST7735_GLCD_HEIGHT
                    wait 2 ms
                    SendCommand_ST7735 0x2C
                    wait 2 ms
                    Repeat [word]ST7735_GLCD_WIDTH * ST7735_GLCD_HEIGHT
			SendData_ST7735 GLCDBackground_h
                              SendData_ST7735 GLCDBackground
		End Repeat
'                    FilledBox( 0, 0, ST7735_GLCD_WIDTH, ST7735_GLCD_HEIGHT, GLCDBackground )
	#endif

End Sub

'''Draws a character at the specified location on the ST7920 GLCD
'''@param StringLocX X coordinate for message
'''@param CharLocY Y coordinate for message
'''@param Chars String to display
'''@param LineColour Line Color, either 1 or 0
Sub GLCDDrawChar_ST7735(In CharLocX, In CharLocY, In CharCode, Optional In LineColour as word = GLCDForeground )

	'CharCode needs to have 16 subtracted, table starts at char 16 not char 0
	CharCode -= 15

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



End Sub


'''Draws a filled box on the GLCD screen
'''@param LineX1 Top left corner X location
'''@param LineY1 Top left corner Y location
'''@param LineX2 Bottom right corner X location
'''@param LineY2 Bottom right corner Y location
'''@param LineColour Colour of box (0 = erase, 1 = draw, default is 1)
Sub FilledBox_ST7735(In LineX1, In LineY1, In LineX2, In LineY2, Optional In LineColour As Word = GLCDForeground)

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
	
	#if GLCD_TYPE = GLCD_TYPE_ST7735
		'Set address window
		SetAddress_ST7735 ST7735_COLUMN, LineX1, LineX2
		SetAddress_ST7735 ST7735_ROW, LineY1, LineY2
		'Fill with colour
		Dim GLCDPixelCount As Word
		GLCDPixelCount = (LineX2 - LineX1 + 1) * (LineY2 - LineY1 + 1)
		SendCommand_ST7735 0x2C
		Repeat GLCDPixelCount
			SendWord_ST7735 LineColour
		End Repeat
	#endif
End Sub

'''Draws a pixel on the GLCD
'''@param GLCDX X coordinate of pixel
'''@param GLCDY Y coordinate of pixel
'''@param GLCDColour State of pixel
Sub PSet_ST7735(In GLCDX, In GLCDY, In GLCDColour As Word)
	
	#if GLCD_TYPE = GLCD_TYPE_ST7735
		SetAddress_ST7735 ST7735_COLUMN, GLCDX, GLCDX
		SetAddress_ST7735 ST7735_ROW, GLCDY, GLCDY
		SendCommand_ST7735 0x2C
		SendWord_ST7735 GLCDColour
	#endif
End Sub



'''Transfer a byte
'''@hide
Sub Transfer_ST7735(In ST7735TempIn, Out ST7735TempOut)

	'Use mode 0 - CPOL = 0, CPHA = 0
          'Set SSPSTAT.CKE On
          'Set SSPCON1.CKP Off

    SET ST7735_SCK OFF
    repeat 8                      '8 data bits
      Wait 1 us
      if ST7735TempIn.7 = ON then      'put most significant bit on SDA line
        set ST7735_DO ON
      else
        set ST7735_DO OFF
      end if

      rotate ST7735TempIn left         'shift in bit for the next time
      SET ST7735_SCK ON                'now clock bit out
      Wait 1 us
      SET ST7735_SCK OFF               'done clocking that bit

    end repeat
    Wait 1 us
    SET ST7735_SCK OFF	
End Sub

'''Send a command to the ST7735 GLCD
'''@param ST7735SendByte Command to send
'''@hide
sub  SendCommand_ST7735( IN ST7735SendByte as byte )

  set ST7735_CS OFF;
  set ST7735_DC OFF;

  #ifdef ST7735_HardwareSPI
     SPITransfer  ST7735SendByte,  ST7735TempOut
     set ST7735_CS ON;
     exit sub
  #endif

  repeat 8

    if ST7735SendByte.7 = ON  then
      set ST7735_DO ON;
    else
      set ST7735_DO OFF;
    end if
    SET GLCD_SCK OFF;
    rotate ST7735SendByte left
    set GLCD_SCK ON;

  end repeat
  set ST7735_CS ON;

end Sub

'''Send a data byte to the ST7735 GLCD
'''@param ST7735SendByte Byte to send
'''@hide
sub  SendData_ST7735( IN ST7735SendByte as byte )

  set ST7735_CS OFF;
  set ST7735_DC ON;

  #ifdef ST7735_HardwareSPI
     SPITransfer  ST7735SendByte,  ST7735TempOut
     set ST7735_CS ON;
     exit sub
  #endif


  repeat 8

    if ST7735SendByte.7 = ON then
      set ST7735_DO ON;
    else
      set ST7735_DO OFF;
    end if
    SET GLCD_SCK OFF;
    rotate ST7735SendByte left
    set GLCD_SCK ON;

  end Repeat
  set ST7735_CS ON;

end Sub

'''Send a data word (16 bits) to the ST7735 GLCD
'''@param ST7735SendByte Word to send
'''@hide
Sub SendWord_ST7735(In ST7735SendWord As Word)
  set ST7735_CS OFF;
  set ST7735_DC ON;

  #ifdef ST7735_HardwareSPI
     SPITransfer  ST7735SendWord_H,  ST7735TempOut
     SPITransfer  ST7735SendWord,  ST7735TempOut
     set ST7735_CS ON;
     exit sub
  #endif

  repeat 16

    if ST7735SendWord.15 = ON then
      set ST7735_DO ON;
    else
      set ST7735_DO OFF;
    end if
    SET GLCD_SCK OFF;
    rotate ST7735SendWord left
    set GLCD_SCK ON;

  end repeat
  set ST7735_CS ON;
End Sub


'''Set the row or column address range for the ST7735 GLCD
'''@param ST7735AddressType Address Type (ST7735_ROW or ST7735_COLUMN)
'''@param ST7735Start Starting address
'''@param ST7735End Ending address
'''@hide
Sub SetAddress_ST7735(In ST7735AddressType, In ST7735Start As Word, In ST7735End As Word)
	SendCommand_ST7735 ST7735AddressType
	SendData_ST7735 ST7735Start_H
	SendData_ST7735 ST7735Start
	SendData_ST7735 ST7735End_H
	SendData_ST7735 ST7735End
End Sub

'''@hide
sub   GLCDRotate_ST7735 ( in ST7735AddressType )

  SendCommand_ST7735 ( ST7735_MADCTL )
  select case ST7735AddressType
        case LANDSCAPE
             SendData_ST7735( 0xB8 )
             ST7735_GLCD_WIDTH = GLCD_HEIGHT
             ST7735_GLCD_HEIGHT = GLCD_WIDTH
        case PORTRAIT_REV
             ST7735_GLCD_WIDTH = GLCD_WIDTH
             ST7735_GLCD_HEIGHT = GLCD_HEIGHT
             SendData_ST7735( 0xDC )
        case LANDSCAPE_REV
             SendData_ST7735( 0x6C )
             ST7735_GLCD_WIDTH = GLCD_HEIGHT
             ST7735_GLCD_HEIGHT = GLCD_WIDTH
        case PORTRAIT
             ST7735_GLCD_WIDTH = GLCD_WIDTH
             ST7735_GLCD_HEIGHT = GLCD_HEIGHT
             SendData_ST7735( 0x08 )
        case else
             ST7735_GLCD_WIDTH = GLCD_WIDTH
             ST7735_GLCD_HEIGHT = GLCD_HEIGHT
             SendData_ST7735( 0x08 )
  end select

end sub

'''@hide
Sub SetGammaCorrection_ST7735

	SendCommand_ST7735 0x26
	SendData_ST7735 0x04
	
	SendCommand_ST7735 0xF2
	SendData_ST7735 0x01
	
	'Send commands and values from table
	For ST7735ByteNo = 1 to 32
		ReadTable ST7735GammaCorrection, ST7735ByteNo, ST7735Byte
		If ST7735ByteNo = 1 Then
			SendCommand_ST7735 ST7735Byte
		Else
			If ST7735ByteNo = 17 Then
				Wait 50 ms
				SendCommand_ST7735 ST7735Byte
			Else
				SendData_ST7735 ST7735Byte
			End If
		End If
	Next
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


