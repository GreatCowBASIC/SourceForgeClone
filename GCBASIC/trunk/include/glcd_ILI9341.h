'    Graphical LCD routines for the GCBASIC compiler
'    Copyright (C) 2015, 2016 Evan Venn and Paolo Iocco

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
' Supports ILI9341 controller only.

'Changes
' 04/10/2015:      Revised to add ReadID.           Evan R Venn
' 14/07/2016:			 Revised to resolve Linux build.	Paolo Iocco edited by Evan R Venn

'
'Hardware settings
'Type
'''@hardware All; Controller Type; GLCD_TYPE; "GLCD_TYPE_ILI9341"

'Serial lines (ILI9341 only)
'''@hardware GLCD_TYPE GLCD_TYPE_ILI9341; Data/Command; GLCD_DC; IO_Pin
'''@hardware GLCD_TYPE GLCD_TYPE_ILI9341; Chip Select; GLCD_CS; IO_Pin
'''@hardware GLCD_TYPE GLCD_TYPE_ILI9341; Data In (LCD Out) GLCD_DI; IO_Pin
'''@hardware GLCD_TYPE GLCD_TYPE_ILI9341; Data Out (LCD In) GLCD_DO; IO_Pin
'''@hardware GLCD_TYPE GLCD_TYPE_ILI9341; Clock; GLCD_SCK; IO_Pin

'''@hardware GLCD_TYPE GLCD_TYPE_ILI9341; Reset; GLCD_RESET; IO_Pin

'Pin mappings for ILI9341
#define ILI9341_DC GLCD_DC
#define ILI9341_CS GLCD_CS
#define ILI9341_RST GLCD_RESET

#define ILI9341_DI GLCD_DI
#define ILI9341_DO GLCD_DO
#define ILI9341_SCK GLCD_SCK



#define ILI9341_NOP     0x00
#define ILI9341_SWRESET 0x01
#define ILI9341_RDDID   0x04
#define ILI9341_RDDST   0x09

#define ILI9341_SLPIN   0x10
#define ILI9341_SLPOUT  0x11
#define ILI9341_PTLON   0x12
#define ILI9341_NORON   0x13

#define ILI9341_RDMODE  0x0A
#define ILI9341_RDMADCTL  0x0B
#define ILI9341_RDPIXFMT  0x0C
#define ILI9341_RDIMGFMT  0x0D
#define ILI9341_RDSELFDIAG  0x0F

#define ILI9341_INVOFF  0x20
#define ILI9341_INVON   0x21
#define ILI9341_GAMMASET 0x26
#define ILI9341_DISPOFF 0x28
#define ILI9341_DISPON  0x29

#define ILI9341_CASET   0x2A
#define ILI9341_PASET   0x2B
#define ILI9341_RAMWR   0x2C
#define ILI9341_RAMRD   0x2E

#define ILI9341_PTLAR   0x30
#define ILI9341_VSCRDEF	0x33
#define ILI9341_MADCTL  0x36

#define ILI9341_MADCTL_MY  0x80
#define ILI9341_MADCTL_MX  0x40
#define ILI9341_MADCTL_MV  0x20
#define ILI9341_MADCTL_ML  0x10
#define ILI9341_MADCTL_RGB 0x00
#define ILI9341_MADCTL_BGR 0x08
#define ILI9341_MADCTL_MH  0x04

#define ILI9341_PIXFMT  0x3A

#define ILI9341_FRMCTR1 0xB1
#define ILI9341_FRMCTR2 0xB2
#define ILI9341_FRMCTR3 0xB3
#define ILI9341_INVCTR  0xB4
#define ILI9341_DFUNCTR 0xB6

#define ILI9341_PWCTR1  0xC0
#define ILI9341_PWCTR2  0xC1
#define ILI9341_PWCTR3  0xC2
#define ILI9341_PWCTR4  0xC3
#define ILI9341_PWCTR5  0xC4
#define ILI9341_VMCTR1  0xC5
#define ILI9341_VMCTR2  0xC7

#define ILI9341_RDID1   0xDA
#define ILI9341_RDID2   0xDB
#define ILI9341_RDID3   0xDC
#define ILI9341_RDID4   0xDD

#define ILI9341_GMCTRP1 0xE0
#define ILI9341_GMCTRN1 0xE1

#define ILI9341_PWCTR6  0xFC



' Color definitions
' Define Colors
#define ILI9341_BLACK   0x0000
#define ILI9341_RED     0xF800
#define ILI9341_GREEN   0x07E0
#define ILI9341_BLUE    0x001F
#define ILI9341_WHITE   0xFFFF
#define ILI9341_PURPLE  0xF11F
#define ILI9341_YELLOW  0xFFE0
#define ILI9341_CYAN    0x07FF
#define ILI9341_D_GRAY  0x528A
#define ILI9341_L_GRAY  0x7997
#define ILI9341_SILVER  0xC618
#define ILI9341_MAROON  0x8000
#define ILI9341_OLIVE   0x8400
#define ILI9341_LIME    0x07E0
#define ILI9341_AQUA    0x07FF
#define ILI9341_TEAL    0x0410
#define ILI9341_NAVY    0x0010
#define ILI9341_FUCHSIA 0xF81F




#startup InitGLCD_ILI9341




'''Initialise the GLCD device
Sub InitGLCD_ILI9341

	'	 Mapped to global variables to same RAM
	'	dim ILI9341_GLCD_HEIGHT, ILI9341_GLCD_WIDTH as word


	'Setup code for ILI9341 controllers
	#if GLCD_TYPE = GLCD_TYPE_ILI9341

	' Init required if using serial for early state debug
  #if USART_BAUD_RATE
  		INITUSART
  #endif

    'Pin directions
    Dir ILI9341_CS Out
    Dir ILI9341_DC Out
    Dir ILI9341_RST Out

    Dir ILI9341_DI In
    Dir ILI9341_DO Out
    Dir ILI9341_SCK Out

    #ifdef ILI9341_HardwareSPI
      ' harware SPI mode
      SPIMode MasterFast, 0
    #endif

   Set ILI9341_CS On
   Set ILI9341_DC On


		'Reset display
		Wait 50 ms
    Set ILI9341_RST On
		Wait 5 ms
		'Reset sequence (lower line for at least 10 us)
		Set ILI9341_RST Off
		Wait 20 us
		Set ILI9341_RST On
		Wait 150 ms

  SendCommand_ILI9341(0xEF)
  SendData_ILI9341(0x03)
  SendData_ILI9341(0x80)
  SendData_ILI9341(0x02)

  SendCommand_ILI9341(0xCF)
  SendData_ILI9341(0x00)
  SendData_ILI9341(0XC1)
  SendData_ILI9341(0X30)

  SendCommand_ILI9341(0xED)
  SendData_ILI9341(0x64)
  SendData_ILI9341(0x03)
  SendData_ILI9341(0X12)
  SendData_ILI9341(0X81)

  SendCommand_ILI9341(0xE8)
  SendData_ILI9341(0x85)
  SendData_ILI9341(0x00)
  SendData_ILI9341(0x78)

  SendCommand_ILI9341(0xCB)
  SendData_ILI9341(0x39)
  SendData_ILI9341(0x2C)
  SendData_ILI9341(0x00)
  SendData_ILI9341(0x34)
  SendData_ILI9341(0x02)

  SendCommand_ILI9341(0xF7)
  SendData_ILI9341(0x20)

  SendCommand_ILI9341(0xEA)
  SendData_ILI9341(0x00)
  SendData_ILI9341(0x00)

  SendCommand_ILI9341(ILI9341_PWCTR1)    'Power control
  SendData_ILI9341(0x23)   'VRH[5:0]

  SendCommand_ILI9341(ILI9341_PWCTR2)    'Power control
  SendData_ILI9341(0x10)   'SAP[2:0];BT[3:0]

  SendCommand_ILI9341(ILI9341_VMCTR1)    'VCM control
  SendData_ILI9341(0x3e) '???????
  SendData_ILI9341(0x28)

  SendCommand_ILI9341(ILI9341_VMCTR2)    'VCM control2
  SendData_ILI9341(0x86)  '--

  SendCommand_ILI9341(ILI9341_MADCTL)    ' Memory Access Control
  SendData_ILI9341(ILI9341_MADCTL_MX | ILI9341_MADCTL_BGR)

  SendCommand_ILI9341(ILI9341_PIXFMT)
  SendData_ILI9341(0x55)

  SendCommand_ILI9341(ILI9341_FRMCTR1)
  SendData_ILI9341(0x00)
  SendData_ILI9341(0x18)

  SendCommand_ILI9341(ILI9341_DFUNCTR)    ' Display Function Control
  SendData_ILI9341(0x08)
  SendData_ILI9341(0x82)
  SendData_ILI9341(0x27)

  SendCommand_ILI9341(0xF2)    ' 3Gamma Function Disable
  SendData_ILI9341(0x00)

  SendCommand_ILI9341(ILI9341_GAMMASET)    'Gamma curve selected
  SendData_ILI9341(0x01)

  SendCommand_ILI9341(ILI9341_GMCTRP1)    'Set Gamma
  SendData_ILI9341(0x0F)
  SendData_ILI9341(0x31)
  SendData_ILI9341(0x2B)
  SendData_ILI9341(0x0C)
  SendData_ILI9341(0x0E)
  SendData_ILI9341(0x08)
  SendData_ILI9341(0x4E)
  SendData_ILI9341(0xF1)
  SendData_ILI9341(0x37)
  SendData_ILI9341(0x07)
  SendData_ILI9341(0x10)
  SendData_ILI9341(0x03)
  SendData_ILI9341(0x0E)
  SendData_ILI9341(0x09)
  SendData_ILI9341(0x00)

  SendCommand_ILI9341(ILI9341_GMCTRN1)    'Set Gamma
  SendData_ILI9341(0x00)
  SendData_ILI9341(0x0E)
  SendData_ILI9341(0x14)
  SendData_ILI9341(0x03)
  SendData_ILI9341(0x11)
  SendData_ILI9341(0x07)
  SendData_ILI9341(0x31)
  SendData_ILI9341(0xC1)
  SendData_ILI9341(0x48)
  SendData_ILI9341(0x08)
  SendData_ILI9341(0x0F)
  SendData_ILI9341(0x0C)
  SendData_ILI9341(0x31)
  SendData_ILI9341(0x36)
  SendData_ILI9341(0x0F)

  SendCommand_ILI9341(ILI9341_SLPOUT)    'Exit Sleep
  wait 120 ms
  SendCommand_ILI9341(ILI9341_DISPON)    'Display on

  'Default Colours
  GLCDBackground = ILI9341_BLACK
  GLCDForeground = ILI9341_WHITE

  'Variables required for device
  ILI9341_GLCD_WIDTH = GLCD_WIDTH
  ILI9341_GLCD_HEIGHT = GLCD_HEIGHT
  GLCDFontWidth = 6
  GLCDfntDefault = 0
  GLCDfntDefaultsize = 2

	#endif

	'Clear screen
	GLCDCLS

End Sub


'Read IC device code.
'The 1st parameter means the IC version.
'The 2rd and 3rd parameter mean the IC model name, should be 9341

Function ILI9341_ReadID as long

        SendCommand_ILI9341( 0xd9 )
        'write data
        SendData_ILI9341( 0x11 )
        set ILI9341_CS OFF
        set ILI9341_DC OFF
        SPITransfer  0xd3,  ILI9341TempOut
        set ILI9341_DC ON
        SPITransfer  0x00,  SysCalcTempA
        set ILI9341_CS ON

        SendCommand_ILI9341( 0xd9 )
        'write data
        SendData_ILI9341( 0x12 )
        set ILI9341_CS OFF
        set ILI9341_DC OFF
        SPITransfer  0xd3,  ILI9341TempOut
        set ILI9341_DC ON
        SPITransfer  0x00,  SysCalcTempB
        set ILI9341_CS ON

        SendCommand_ILI9341( 0xd9 )
        'write data
        SendData_ILI9341( 0x13 )
        set ILI9341_CS OFF
        set ILI9341_DC OFF
        SPITransfer  0xd3,  ILI9341TempOut
        set ILI9341_DC ON
        SPITransfer  0x00,  SysCalcTempX
        set ILI9341_CS ON
        epwrite 2, ILI9341TempOut

        'Rebuild result to the long variable and return
        ILI9341_ReadID = SysCalcTempX
        ILI9341_ReadID_H = SysCalcTempB
        ILI9341_ReadID_U = SysCalcTempA





End Function

Sub DSTB_ILI9341( Optional In PowerMode = On  )

    if PowerMode = Off Then

       SendCommand_ILI9341( 0xb7 )
       SendData_ILI9341(0x08)

    Else

       SendCommand_ILI9341( 0xb7 )
       SendData_ILI9341(0b10110111)

       repeat 2
              set ILI9341_CS OFF
              wait 100 us
              set ILI9341_CS ON
              wait 100 us
       End Repeat
       wait 1 ms
       repeat 4
              set ILI9341_CS OFF
              wait 100 us
              set ILI9341_CS ON
              wait 100 us
       End Repeat

   Set ILI9341_CS On
   Set ILI9341_DC On


		'Reset display
		Wait 50 ms
    Set ILI9341_RST On
		Wait 5 ms
		'Reset sequence (lower line for at least 10 us)
		Set ILI9341_RST Off
		Wait 20 us
		Set ILI9341_RST On
		Wait 150 ms

  SendCommand_ILI9341(0xEF)
  SendData_ILI9341(0x03)
  SendData_ILI9341(0x80)
  SendData_ILI9341(0x02)

  SendCommand_ILI9341(0xCF)
  SendData_ILI9341(0x00)
  SendData_ILI9341(0XC1)
  SendData_ILI9341(0X30)

  SendCommand_ILI9341(0xED)
  SendData_ILI9341(0x64)
  SendData_ILI9341(0x03)
  SendData_ILI9341(0X12)
  SendData_ILI9341(0X81)

  SendCommand_ILI9341(0xE8)
  SendData_ILI9341(0x85)
  SendData_ILI9341(0x00)
  SendData_ILI9341(0x78)

  SendCommand_ILI9341(0xCB)
  SendData_ILI9341(0x39)
  SendData_ILI9341(0x2C)
  SendData_ILI9341(0x00)
  SendData_ILI9341(0x34)
  SendData_ILI9341(0x02)

  SendCommand_ILI9341(0xF7)
  SendData_ILI9341(0x20)

  SendCommand_ILI9341(0xEA)
  SendData_ILI9341(0x00)
  SendData_ILI9341(0x00)

  SendCommand_ILI9341(ILI9341_PWCTR1)    'Power control
  SendData_ILI9341(0x23)   'VRH[5:0]

  SendCommand_ILI9341(ILI9341_PWCTR2)    'Power control
  SendData_ILI9341(0x10)   'SAP[2:0];BT[3:0]

  SendCommand_ILI9341(ILI9341_VMCTR1)    'VCM control
  SendData_ILI9341(0x3e) '???????
  SendData_ILI9341(0x28)

  SendCommand_ILI9341(ILI9341_VMCTR2)    'VCM control2
  SendData_ILI9341(0x86)  '--

  SendCommand_ILI9341(ILI9341_MADCTL)    ' Memory Access Control
  SendData_ILI9341(ILI9341_MADCTL_MX | ILI9341_MADCTL_BGR)

  SendCommand_ILI9341(ILI9341_PIXFMT)
  SendData_ILI9341(0x55)

  SendCommand_ILI9341(ILI9341_FRMCTR1)
  SendData_ILI9341(0x00)
  SendData_ILI9341(0x18)

  SendCommand_ILI9341(ILI9341_DFUNCTR)    ' Display Function Control
  SendData_ILI9341(0x08)
  SendData_ILI9341(0x82)
  SendData_ILI9341(0x27)

  SendCommand_ILI9341(0xF2)    ' 3Gamma Function Disable
  SendData_ILI9341(0x00)

  SendCommand_ILI9341(ILI9341_GAMMASET)    'Gamma curve selected
  SendData_ILI9341(0x01)

  SendCommand_ILI9341(ILI9341_GMCTRP1)    'Set Gamma
  SendData_ILI9341(0x0F)
  SendData_ILI9341(0x31)
  SendData_ILI9341(0x2B)
  SendData_ILI9341(0x0C)
  SendData_ILI9341(0x0E)
  SendData_ILI9341(0x08)
  SendData_ILI9341(0x4E)
  SendData_ILI9341(0xF1)
  SendData_ILI9341(0x37)
  SendData_ILI9341(0x07)
  SendData_ILI9341(0x10)
  SendData_ILI9341(0x03)
  SendData_ILI9341(0x0E)
  SendData_ILI9341(0x09)
  SendData_ILI9341(0x00)

  SendCommand_ILI9341(ILI9341_GMCTRN1)    'Set Gamma
  SendData_ILI9341(0x00)
  SendData_ILI9341(0x0E)
  SendData_ILI9341(0x14)
  SendData_ILI9341(0x03)
  SendData_ILI9341(0x11)
  SendData_ILI9341(0x07)
  SendData_ILI9341(0x31)
  SendData_ILI9341(0xC1)
  SendData_ILI9341(0x48)
  SendData_ILI9341(0x08)
  SendData_ILI9341(0x0F)
  SendData_ILI9341(0x0C)
  SendData_ILI9341(0x31)
  SendData_ILI9341(0x36)
  SendData_ILI9341(0x0F)

  SendCommand_ILI9341(ILI9341_SLPOUT)    'Exit Sleep
  wait 120 ms
  SendCommand_ILI9341(ILI9341_DISPON)    'Display on


    End if


End Sub

'Subs
'''Clears the GLCD screen
Sub GLCDCLS_ILI9341 ( Optional In  GLCDBackground as word = GLCDBackground )

    dim ILI9341SendWord as word
    ' initialise global variable. Required variable for Circle in all DEVICE DRIVERS- DO NOT DELETE
    GLCD_yordinate = 0

    SetAddressWindow_ILI9341 ( 0, 0, ILI9341_GLCD_WIDTH -1 , ILI9341_GLCD_HEIGHT-1 )
		ILI9341SendWord = GLCDBackground

		set ILI9341_CS OFF
    set ILI9341_DC ON
    repeat ILI9341_GLCD_WIDTH

			repeat ILI9341_GLCD_HEIGHT

        #ifdef ILI9341_HardwareSPI
'					Could use these as an alternative
'        	FastHWSPITransfer  ILI9341SendWord_h
'         FastHWSPITransfer  ILI9341SendWord

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
            SSPBUF = ILI9341SendWord_h
						Wait While SSPSTAT.BF = Off
            Set SSPSTAT.BF Off

            'Clear WCOL
            Set SSPCON1.WCOL Off
            'Put byte to send into buffer
            'Will start transfer
            SSPBUF = ILI9341SendWord
						Wait While SSPSTAT.BF = Off
            Set SSPSTAT.BF Off
          #endif
					#ifdef AVR
          	FastHWSPITransfer  ILI9341SendWord_h
          	FastHWSPITransfer  ILI9341SendWord
					#endif

        #endif

				#ifndef ILI9341_HardwareSPI
					SendWord_ILI9341 ( GLCDBackground )
        #endif
			end repeat

    end repeat
    set ILI9341_CS ON;


End Sub

'''Draws a string at the specified location on the ST7920 GLCD
'''@param StringLocX X coordinate for message
'''@param CharLocY Y coordinate for message
'''@param Chars String to display
'''@param LineColour Line Color, either 1 or 0
Sub GLCDDrawString_ILI9341( In StringLocX as word, In CharLocY as word, In Chars as string, Optional In LineColour as word = GLCDForeground )
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
Sub GLCDDrawChar_ILI9341(In CharLocX as word, In CharLocY as word, In CharCode, Optional In LineColour as word = GLCDForeground )

'	'CharCode needs to have 16 subtracted, table starts at char 16 not char 0
'	CharCode -= 15
'
'  'Need to read characters from CharColn (n = 0:7) tables
'	'(First 3, ie 0:2 are blank, so can ignore)
'	For CurrCharCol = 1 to 5
'		Select Case CurrCharCol
'			Case 1: ReadTable GLCDCharCol3, CharCode, CurrCharVal
'			Case 2: ReadTable GLCDCharCol4, CharCode, CurrCharVal
'			Case 3: ReadTable GLCDCharCol5, CharCode, CurrCharVal
'			Case 4: ReadTable GLCDCharCol6, CharCode, CurrCharVal
'			Case 5: ReadTable GLCDCharCol7, CharCode, CurrCharVal
'		End Select
'                    For CurrCharRow = 1 to 8
'                              If CurrCharVal.0 = 0 Then
'                                        PSet CharLocX + CurrCharCol, CharLocY + CurrCharRow, GLCDBackground
'                              Else
'                                        PSet CharLocX + CurrCharCol, CharLocY + CurrCharRow, GLCDForeground
'                              End If
'                              Rotate CurrCharVal Right
'                    Next
'
'	Next
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
Sub FilledBox_ILI9341(In LineX1 as word, In LineY1 as word, In LineX2 as word, In LineY2 as word, Optional In LineColour As Word = GLCDForeground)
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
'		SetAddress_ILI9341 ILI9341_COLUMN, LineX1, LineX2
'		SetAddress_ILI9341 ILI9341_ROW, LineY1, LineY2
		SetAddressWindow_ILI9341 (  LineX1, LineY1, LineX2, LineY2 )
		'Fill with colour
		Dim GLCDPixelCount As Word
		GLCDPixelCount = (LineX2 - LineX1 + 1) * (LineY2 - LineY1 + 1)
		Repeat GLCDPixelCount
			SendWord_ILI9341 LineColour
		End Repeat

End Sub

'''Draws a pixel on the GLCD
'''@param GLCDX X coordinate of pixel
'''@param GLCDY Y coordinate of pixel
'''@param GLCDColour State of pixel
Sub PSet_ILI9341(In GLCDX as word, In GLCDY as word, In GLCDColour As Word)

		SetAddressWindow_ILI9341 ( GLCDX, GLCDY, GLCDX, GLCDY )
		SendWord_ILI9341 GLCDColour

End Sub




'''Send a command to the ILI9341 GLCD
'''@param ILI9341SendByte Command to send
'''@hide
sub  SendCommand_ILI9341( IN ILI9341SendByte as byte )

  set ILI9341_CS OFF;
  set ILI9341_DC OFF;

  #ifdef ILI9341_HardwareSPI
     SPITransfer  ILI9341SendByte,  ILI9341TempOut
     set ILI9341_CS ON;
     exit sub
  #endif

  #ifndef ILI9341_HardwareSPI
  repeat 8

    if ILI9341SendByte.7 = ON  then
      set ILI9341_DO ON;
    else
      set ILI9341_DO OFF;
    end if
    SET GLCD_SCK On;
    rotate ILI9341SendByte left
    set GLCD_SCK Off;

  end repeat
  set ILI9341_CS ON;
  #endif

end Sub

'''Send a data byte to the ILI9341 GLCD
'''@param ILI9341SendByte Byte to send
'''@hide
sub  SendData_ILI9341( IN ILI9341SendByte as byte )

  set ILI9341_CS OFF;
  set ILI9341_DC ON;

  #ifdef ILI9341_HardwareSPI
     SPITransfer  ILI9341SendByte,  ILI9341TempOut
     set ILI9341_CS ON;
     exit sub
  #endif

  #ifndef ILI9341_HardwareSPI
  repeat 8

    if ILI9341SendByte.7 = ON then
      set ILI9341_DO ON;
    else
      set ILI9341_DO OFF;
    end if
    SET GLCD_SCK On;
    rotate ILI9341SendByte left
    set GLCD_SCK Off;

  end Repeat
  set ILI9341_CS ON;
  #endif

end Sub

'''Send a data word (16 bits) to the ILI9341 GLCD
'''@param ILI9341SendByte Word to send
'''@hide
Sub SendWord_ILI9341(In ILI9341SendWord As Word)
  set ILI9341_CS OFF;
  set ILI9341_DC ON;

  #ifdef ILI9341_HardwareSPI
     SPITransfer  ILI9341SendWord_H,  ILI9341TempOut
     SPITransfer  ILI9341SendWord,  ILI9341TempOut
     set ILI9341_CS ON;
     exit sub
  #endif

  #ifndef ILI9341_HardwareSPI
  repeat 16

    if ILI9341SendWord.15 = ON then
      set ILI9341_DO ON;
    else
      set ILI9341_DO OFF;
    end if
    SET GLCD_SCK On;
    rotate ILI9341SendWord left
    set GLCD_SCK Off;

  end repeat
  set ILI9341_CS ON;
  #endif
End Sub


'''Set the row or column address range for the ILI9341 GLCD
'''@param ILI9341AddressType Address Type (ILI9341_ROW or ILI9341_COLUMN)
'''@param ILI9341Start Starting address
'''@param ILI9341End Ending address
'''@hide
Sub SetAddressWindow_ILI9341( In x1 as word, in y1 as word, in x2 as word, in y2 as word)

	SendCommand_ILI9341(ILI9341_CASET); // Column addr set
	SendData_ILI9341 x1_H
	SendData_ILI9341 x1
  SendData_ILI9341 x2_H
	SendData_ILI9341 x2

  SendCommand_ILI9341(ILI9341_PASET); // Row addr set
	SendData_ILI9341 y1_H
	SendData_ILI9341 y1
  SendData_ILI9341 y2_H
	SendData_ILI9341 y2

  SendCommand_ILI9341(ILI9341_RAMWR); // write to RAM

End Sub



Sub SetCursorPosition_ILI9341( In x1 as word, in y1 as word, in x2 as word, in y2 as word)
	SendCommand_ILI9341(ILI9341_CASET); // Column addr set
	SendData_ILI9341 x1_H
	SendData_ILI9341 x1
  SendData_ILI9341 x2_H
	SendData_ILI9341 x2

  SendCommand_ILI9341(ILI9341_PASET); // Row addr set
	SendData_ILI9341 y1_H
	SendData_ILI9341 y1
  SendData_ILI9341 y2_H
	SendData_ILI9341 y2

End Sub


'''@hide
sub   GLCDRotate_ILI9341 ( in ILI9341AddressType )

  SendCommand_ILI9341 ( ILI9341_MADCTL )
  select case ILI9341AddressType
        case PORTRAIT
             ILI9341_GLCD_WIDTH = GLCD_WIDTH
             ILI9341_GLCD_HEIGHT = GLCD_HEIGHT
             SendData_ILI9341( ILI9341_MADCTL_MX | ILI9341_MADCTL_BGR)
        case LANDSCAPE
             ILI9341_GLCD_WIDTH = GLCD_HEIGHT
             ILI9341_GLCD_HEIGHT = GLCD_WIDTH
             SendData_ILI9341( ILI9341_MADCTL_MV | ILI9341_MADCTL_BGR )

        case PORTRAIT_REV
             ILI9341_GLCD_WIDTH = GLCD_WIDTH
             ILI9341_GLCD_HEIGHT = GLCD_HEIGHT
             SendData_ILI9341( ILI9341_MADCTL_MY | ILI9341_MADCTL_BGR)
        case LANDSCAPE_REV
             ILI9341_GLCD_WIDTH = GLCD_HEIGHT
             ILI9341_GLCD_HEIGHT = GLCD_WIDTH
             SendData_ILI9341(  ILI9341_MADCTL_MV | ILI9341_MADCTL_MY | ILI9341_MADCTL_MX | ILI9341_MADCTL_BGR )

        case else
             ILI9341_GLCD_WIDTH = GLCD_WIDTH
             ILI9341_GLCD_HEIGHT = GLCD_HEIGHT
             SendData_ILI9341( ILI9341_MADCTL_MX | ILI9341_MADCTL_BGR )
  end select

end sub



'''Displays a string in a larger fonti.e.  BigPrint_ILI9341 ( 30, ILI9341_GLCD_HEIGHT - 20 , "BigPrint_ILI9341" )
'''@param PrintLocX X coordinate for message
'''@param PrintLocY Y coordinate for message
'''@param PrintData String to display
'''@param Color Optional color
Sub BigPrint_ILI9341(In PrintLocX as Word, In PrintLocY as Word,  PrintData As String, Optional In  Color as word = GLCDForeground)
	Dim GLCDPrintLoc as word
  PrintLen = PrintData(0)
	If PrintLen = 0 Then Exit Sub
	GLCDPrintLoc = PrintLocX
	For SysPrintTemp = 1 To PrintLen
		DrawBigChar_ILI9341 GLCDPrintLoc, PrintLocY, PrintData(SysPrintTemp), Color
		GLCDPrintLoc += 13
	Next
End Sub

'''Draws a Vertical Line on the GLCD with a
'''@param GLCDY1 Y coordinate of one end of the line
'''@param GLCDY2 Y coordinate of the other end of the line
'''@param GLCDX1 X coordinate of the line
'''@param LineColour color
Sub DrawBigChar_ILI9341 (In CharLocX as Word, In CharLocY as Word, In CharCode, Optional In  Color as word = GLCDForeground )

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
             PSet_ILI9341 LocX , LocY , Color
          end if
					Rotate CurrCharVal Right
				Next
	  Next
End Sub

