'    Graphical LCD routines for the GCBASIC compiler
'    Copyright (C) 2018 Evan Venn

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

'    https://nextion.itead.cc/resources/documents/instruction-set/#s1

'Notes:
' Supports NX240x320L controller only.

'Changes
'08/04/2018 Initial Release


#define GLCD_NEXIONSerialPrint HSerPrint
#define GLCD_NEXIONSerialSend  HSerSend
#define NextionFont0      0, 8, 16    'Arial 8x16
#define NextionFont1      1, 12, 24   '24point 12x24 charset
#define NextionFont2      2, 16, 32   '32point 16x32 charset

#startup InitGLCD_NXN240x320L

dim GLCDDeviceWidth, GLCDDeviceHeight as word

Sub InitGLCD_NXN240x320L

  'Setup code for NX240x320L controllers

   dim PrintLocX, PrintLocY as word
   ' initialise global variable. Required variable for Circle in all DEVICE DRIVERS- DO NOT DELETE
   GLCD_yordinate = 0
   PrintLocX = 0
   PrintLocY = 0


  wait 1 s
  GLCD_NEXIONSerialSend 255
  GLCD_NEXIONSerialSend 255
  GLCD_NEXIONSerialSend 255
  SendParam_NXN240x320L ( "rest", true )  '  reset the device
  GLCDBackground = TFT_BLACK
  GLCDForeground = TFT_WHITE

  #ifndef GLCD_OLED_FONT
    GLCDFontWidth = 6
  #endif

  #ifdef GLCD_OLED_FONT
    GLCDFontWidth = 5
  #endif

  GLCDfntDefault = 0
  GLCDfntDefaultsize = 1



End Sub



'Subs
'''Clears the GLCD screen
Sub GLCDCLS_NXN240x320L ( Optional In  GLCDBackground as word = GLCDBackground )

  ' initialise global variable. Required variable for Circle in all DEVICE DRIVERS- DO NOT DELETE
  GLCD_yordinate = 0

  #if GLCD_TYPE = GLCD_TYPE_NEXION240x320L

    GLCD_NEXIONSerialPrint "cls "+str(GLCDBackground)
    GLCD_NEXIONSerialSend 255
    GLCD_NEXIONSerialSend 255
    GLCD_NEXIONSerialSend 255

  #endif

End Sub

Sub SendParam_NXN240x320L ( in outstring as string, Optional In outstate  = False  )

  GLCD_NEXIONSerialPrint outstring
  if outstate = false then
      GLCD_NEXIONSerialSend 44
  else
      GLCD_NEXIONSerialSend 255
      GLCD_NEXIONSerialSend 255
      GLCD_NEXIONSerialSend 255
  end if

end sub

Sub SendParam_NXN240x320L ( in outval as word, Optional In outstate  = False  )

  GLCD_NEXIONSerialPrint outval
  if outstate = false then
      GLCD_NEXIONSerialSend 44
  else
      GLCD_NEXIONSerialSend 255
      GLCD_NEXIONSerialSend 255
      GLCD_NEXIONSerialSend 255
  end if

end sub

'''Draws a pixel on the GLCD
'''@param GLCDX X coordinate of pixel
'''@param GLCDY Y coordinate of pixel
'''@param GLCDColour State of pixel (0 = erase, 1 = display)
Sub PSet_NXN240x320L(In GLCDX as word , In GLCDY as word, In GLCDColour As Word)

  GLCD_NEXIONSerialPrint "draw "
  SendParam_NXN240x320L GLCDX
  SendParam_NXN240x320L GLCDY
  SendParam_NXN240x320L GLCDX
  SendParam_NXN240x320L GLCDY
  SendParam_NXN240x320L GLCDColour, true

End Sub


'''Draws a string at the specified location on the ST7920 GLCD
'''@param StringLocX X coordinate for message
'''@param CharLocY Y coordinate for message
'''@param Chars String to display
'''@param LineColour Line Color, either 1 or 0
Sub GLCDDrawString_NXN240x320L( In StringLocX as word, In CharLocY as word, In Chars as string, Optional In LineColour as word = GLCDForeground )
    dim TargetCharCol as word
    for xchar = 1 to Chars(0)
      TargetCharCol = StringLocX + ((xchar*( GLCDFontWidth * GLCDfntDefaultsize ))-( GLCDFontWidth * GLCDfntDefaultsize ))
      GLCDDrawChar TargetCharCol , CharLocY , Chars(xchar), LineColour
    next
end sub

'''Draws a character at the specified location on the ST7920 GLCD
'''@param StringLocX X coordinate for message
'''@param CharLocY Y coordinate for message
'''@param Chars String to display
'''@param LineColour Line Color, either 1 or 0
Sub GLCDDrawChar_NXN240x320L(In CharLocX as word, In CharLocY as word, In CharCode, Optional In LineColour as word = GLCDForeground )

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


Sub GLCDRotate_NXN240x320L( in ILI9481AddressType as byte )

  set ILI9481_GLCD_CS OFF
  select case ILI9481AddressType
        case PORTRAIT  '0 degree

              SendCommand_NXN240x320L(0x36)
              SendData_NXN240x320L(0x0E)
              GLCDDeviceHeight = GLCD_HEIGHT - 1
              GLCDDeviceWidth  = GLCD_WIDTH - 1

        case LANDSCAPE

              SendCommand_NXN240x320L(0x36)
              SendData_NXN240x320L(0x2b)
              GLCDDeviceHeight = GLCD_WIDTH - 1
              GLCDDeviceWidth  = GLCD_HEIGHT - 1


        case PORTRAIT_REV

              SendCommand_NXN240x320L(0x36)
              SendData_NXN240x320L(0x0D)
              GLCDDeviceHeight = GLCD_HEIGHT - 1
              GLCDDeviceWidth  = GLCD_WIDTH - 1


        case LANDSCAPE_REV

              SendCommand_NXN240x320L(0x36)
              SendData_NXN240x320L(0x2c)
              GLCDDeviceHeight = GLCD_WIDTH - 1
              GLCDDeviceWidth  = GLCD_HEIGHT - 1

        case else
              SendCommand_NXN240x320L(0x36)
              SendData_NXN240x320L(0x0E)
              GLCDDeviceHeight = GLCD_HEIGHT - 1
              GLCDDeviceWidth  = GLCD_WIDTH - 1


  end select
  set ILI9481_GLCD_CS On

end sub


Sub Circle_NXN240x320L ( in xoffset as word, in yoffset as word, in Inxradius as word, Optional In LineColour as word = GLCDForeground )

  GLCD_NEXIONSerialPrint "cir "
  SendParam_NXN240x320L xoffset
  SendParam_NXN240x320L  yoffset
  SendParam_NXN240x320L Inxradius
  SendParam_NXN240x320L LineColour, True

end Sub

Sub FilledCircle_NXN240x320L ( in xoffset as word, in yoffset as word, in Inxradius as word, Optional In LineColour as word = GLCDForeground )

  GLCD_NEXIONSerialPrint "cirs "
  SendParam_NXN240x320L xoffset
  SendParam_NXN240x320L  yoffset
  SendParam_NXN240x320L Inxradius
  SendParam_NXN240x320L LineColour, True

end Sub

Sub Box_NXN240x320L(In LineX1 as word, In LineY1 as word, In LineX2 as word, In LineY2 as word, Optional In LineColour As Word = GLCDForeground)

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

  GLCD_NEXIONSerialPrint "draw "
  SendParam_NXN240x320L LineX1
  SendParam_NXN240x320L LineY1
  SendParam_NXN240x320L LineX2
  SendParam_NXN240x320L LineY2
  SendParam_NXN240x320L LineColour, true

End Sub

Sub FilledBox_NXN240x320L(In LineX1 as word, In LineY1 as word, In LineX2 as word, In LineY2 as word, Optional In LineColour As Word = GLCDForeground)

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

  GLCD_NEXIONSerialPrint "fill "
  SendParam_NXN240x320L LineX1
  SendParam_NXN240x320L LineY1
  SendParam_NXN240x320L LineX2 - LineX1
  SendParam_NXN240x320L LineY2 - LineY1
  SendParam_NXN240x320L LineColour, true

End Sub

sub Line_NXN240x320L (In LineX1 as word, In LineY1 as word, In LineX2 as word, In LineY2 as word, Optional In LineColour as word = GLCDForeground)

  GLCD_NEXIONSerialPrint "line "
  SendParam_NXN240x320L LineX1
  SendParam_NXN240x320L LineY1
  SendParam_NXN240x320L LineX2
  SendParam_NXN240x320L LineY2
  SendParam_NXN240x320L LineColour, true

end sub


'''Displays a message
'''@param PrintLocX X coordinate for message
'''@param PrintLocY Y coordinate for message
'''@param PrintData Message to display
Sub GLCDPrint_NXN240x320L (In PrintLocX as word, In PrintLocY as word, in LCDPrintData as string,  In GLCDForeground as word , in GLCDfntDefault,  in GLCDFontWidth , In  GLCDfntDefaultsize )

  Dim GLCDPrintLoc  as word
  Dim GLCDPrint_String_Counter, GLCDPrintLen as byte
  GLCDPrintLen = LCDPrintData(0)
  If GLCDPrintLen = 0 Then Exit Sub

  GLCD_NEXIONSerialPrint "xstr "
  SendParam_NXN240x320L PrintLocX
  SendParam_NXN240x320L PrintLocY

  SendParam_NXN240x320L ( GLCDPrintLen+1)*GLCDFontWidth
  SendParam_NXN240x320L GLCDfntDefaultsize
  SendParam_NXN240x320L GLCDfntDefault
  SendParam_NXN240x320L GLCDForeground
  SendParam_NXN240x320L GLCDBackground
  SendParam_NXN240x320L 0
  SendParam_NXN240x320L 0
  SendParam_NXN240x320L 1
  GLCD_NEXIONSerialSend 34
  GLCD_NEXIONSerialPrint LCDPrintData
  GLCD_NEXIONSerialSend 34
  GLCD_NEXIONSerialSend 255
  GLCD_NEXIONSerialSend 255
  GLCD_NEXIONSerialSend 255

End Sub

'stub to handle the various methods of calling - this one handles the Colour being omitted, so, the color is assumed
Sub GLCDPrint_NXN240x320L (In PrintLocX as word, In PrintLocY as word, in LCDPrintData as string, In GLCDfntDefault, In GLCDFontWidth, In  GLCDfntDefaultsize  )

    GLCDPrint_NXN240x320L ( PrintLocX, PrintLocY , LCDPrintData , GLCDForeground, GLCDfntDefault,  GLCDFontWidth , GLCDfntDefaultsize )

End Sub


'stub to handle the various methods of calling - this one handles just the string
Sub GLCDPrint_NXN240x320L (In PrintLocX as word, In PrintLocY as word, in LCDPrintData as string  )

    GLCDPrint_NXN240x320L ( PrintLocX, PrintLocY , LCDPrintData , GLCDForeground, GLCDfntDefault,  GLCDFontWidth , GLCDfntDefaultsize )

End Sub

'stub to handle the various methods of calling - this one handles the Colour
Sub GLCDPrint_NXN240x320L (In PrintLocX as word, In PrintLocY as word, in LCDPrintData as string , In GLCDForeground as word  )

    GLCDPrint_NXN240x320L ( PrintLocX, PrintLocY , LCDPrintData , GLCDForeground, GLCDfntDefault,  GLCDFontWidth , GLCDfntDefaultsize )

End Sub

'''Displays a message
'''@param PrintLocX X coordinate for message
'''@param PrintLocY Y coordinate for message
'''@param PrintData Message to display
Sub GLCDPrint_NXN240x320L (In PrintLocX as word, In PrintLocY as word, In LCDValue As Long,  In GLCDForeground as word , in GLCDfntDefault,  in GLCDFontWidth , In  GLCDfntDefaultsize )

  Dim GLCDPrintLen as byte
  GLCDPrintLen = str(LCDValue)
  If GLCDPrintLen = 0 Then Exit Sub

  GLCD_NEXIONSerialPrint "xstr "
  SendParam_NXN240x320L PrintLocX
  SendParam_NXN240x320L PrintLocY

  SendParam_NXN240x320L ( GLCDPrintLen+1)*GLCDFontWidth
  SendParam_NXN240x320L GLCDfntDefaultsize
  SendParam_NXN240x320L GLCDfntDefault
  SendParam_NXN240x320L GLCDForeground
  SendParam_NXN240x320L GLCDBackground
  SendParam_NXN240x320L 0
  SendParam_NXN240x320L 0
  SendParam_NXN240x320L 1
  GLCD_NEXIONSerialSend 34
  GLCD_NEXIONSerialPrint str(LCDValue)
  GLCD_NEXIONSerialSend 34
  GLCD_NEXIONSerialSend 255
  GLCD_NEXIONSerialSend 255
  GLCD_NEXIONSerialSend 255

End Sub

'stub to handle the various methods of calling - this one handles the Colour being omitted, so, the color is assumed
Sub GLCDPrint_NXN240x320L (In PrintLocX as word, In PrintLocY as word, In LCDValue As Long, In GLCDfntDefault, In GLCDFontWidth, In  GLCDfntDefaultsize  )

    GLCDPrint_NXN240x320L ( PrintLocX, PrintLocY , LCDValue , GLCDForeground, GLCDfntDefault,  GLCDFontWidth , GLCDfntDefaultsize )

End Sub


'stub to handle the various methods of calling - this one handles just the string
Sub GLCDPrint_NXN240x320L (In PrintLocX as word, In PrintLocY as word, in In LCDValue As Long  )

    GLCDPrint_NXN240x320L ( PrintLocX, PrintLocY , LCDValue , GLCDForeground, GLCDfntDefault,  GLCDFontWidth , GLCDfntDefaultsize )

End Sub

'stub to handle the various methods of calling - this one handles the Colour
Sub GLCDPrint_NXN240x320L (In PrintLocX as word, In PrintLocY as word, in In LCDValue As Long , In GLCDForeground as word  )

    GLCDPrint_NXN240x320L ( PrintLocX, PrintLocY , LCDValue , GLCDForeground, GLCDfntDefault,  GLCDFontWidth , GLCDfntDefaultsize )

End Sub

Sub GLCDPrintString_NXN240x320L ( in LCDPrintData as string )
    dim PrintLocX as word
    GLCDPrint_NXN240x320L( PrintLocX , PrintLocY , LCDPrintData )
    PrintLocX = PrintLocX + ( GLCDFontWidth * LCDPrintData (0) )+2  '2 extra pixels
End Sub


Sub GLCDPrintStringLn_NXN240x320L( in LCDPrintData as string )
    dim PrintLocX, PrintLocY as word

    GLCDPrint_NXN240x320L( PrintLocX , PrintLocY , LCDPrintData )
    PrintLocX = 0
    PrintLocY = ( PrintLocY +  GLCDfntDefaultSize ) mod GLCD_HEIGHT

End Sub

Sub GLCDLocateString_NXN240x320L( in PrintLocX as word, in PrintLocY as word )
    dim PrintLocY as word

'    dim xxx, yyy as word
'    xxx=PrintLocX
'    yyy=PrintLocY
'GLCDPrint_NXN240x320L ( 30, 50, xxx )
'GLCDPrint_NXN240x320L ( 30, 70, xxx )

    PrintLocY = GLCDfntDefaultSize * ( PrintLocY - 1 )
'    yyy=PrintLocY
'GLCDPrint_NXN240x320L ( 30, 50, xxx )
'GLCDPrint_NXN240x320L ( 30, 70, xxx )

End Sub

Sub GLCDPrintDefaultFont_NXN240x320L ( in GLCDfntDefault,  in GLCDFontWidth , In  GLCDfntDefaultsize )
  'this will the set the defaults by purely calling this method
End Sub

sub GLCDUpdateObject_NXN240x320L ( in nextionobject as string , in nextionstringData as string )

  GLCD_NEXIONSerialPrint nextionobject
  GLCD_NEXIONSerialPrint"="
  GLCD_NEXIONSerialSend 34
  GLCD_NEXIONSerialPrint nextionstringData
  GLCD_NEXIONSerialSend 34
  GLCD_NEXIONSerialSend 255
  GLCD_NEXIONSerialSend 255
  GLCD_NEXIONSerialSend 255


end sub

sub GLCDUpdateObject_NXN240x320L ( in nextionobject as string , in nextionnumberData as long )

  GLCD_NEXIONSerialPrint nextionobject
  GLCD_NEXIONSerialPrint"="
  GLCD_NEXIONSerialPrint nextionnumberData
  GLCD_NEXIONSerialSend 255
  GLCD_NEXIONSerialSend 255
  GLCD_NEXIONSerialSend 255

end sub

sub GLCDUpdateObject_NXN240x320L ( in nextionobject as string , in nextionnumberData as integer )

  GLCD_NEXIONSerialPrint nextionobject
  GLCD_NEXIONSerialPrint"="
  GLCD_NEXIONSerialPrint nextionnumberData
  GLCD_NEXIONSerialSend 255
  GLCD_NEXIONSerialSend 255
  GLCD_NEXIONSerialSend 255

end sub

sub GLCDSendOpInstruction_Nexion ( in nextionobject as string , in nextionstringData as string )

  GLCD_NEXIONSerialPrint nextionobject
  GLCD_NEXIONSerialPrint " "
  GLCD_NEXIONSerialPrint nextionstringData
  GLCD_NEXIONSerialSend 255
  GLCD_NEXIONSerialSend 255
  GLCD_NEXIONSerialSend 255

end sub
