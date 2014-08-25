'    Liquid Crystal Display routines for Great Cow BASIC
'    Copyright (C) 2006 - 2014 Hugh Considine, Stefano Bonomi, Evan Venn, William Roth

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

'********************************************************************************
'IMPORTANT:
'THIS FILE IS ESSENTIAL FOR SOME OF THE COMMANDS IN GCBASIC. DO NOT ALTER THIS FILE
'UNLESS YOU KNOW WHAT YOU ARE DOING. CHANGING THIS FILE COULD RENDER SOME GCBASIC
'COMMANDS UNUSABLE!
'********************************************************************************

'Credits:
' 4 and 8 bit routines        Hugh Considine
' 2 bit routines    Stefano Bonomi
' Testing           Stefano Adami
' Revised 4 bit routines    Evan Venn
' adapted to support LAT port support for fast devices

'*************************************************************************
'*************************************************************************
'06-08-2014
'This file (lcd_alt.h) is based upon LCD.H, modified to improve
' performance in 8 bit and 4 bit modes.
'
'  1. All changes were made to subroutine  LCDnormal_write_byte
'
'  2. The speed is improved from about 5,000 chars per second to either
'     10,000 to 20,000 chars per second depending upon a new define in
'     the main program.
'
'  3. The new default character rate is 20K chars per seconds.
'     To slow the speed down to 10K chars per second, add the following
'     in  the main program:   #define LCD_SLOW
'
'  4. Initial testing was done using a PIC16F690 and a PIC16F1829 (LAT)
'
'   William Roth
'**********************************************************************


#startup InitLCD

'I/O Ports
#define LCD_IO 4 'Number of pins used for LCD data bus (2, 4 or 8)

'Sub used to write to LCD. Can be altered to allow custom LCD interfaces.
#define LCDWriteByte LCDNormalWriteByte
#define LCDReadByte LCDNormalReadByte

#define LCD_DB SysLCDTemp.0 'Data bit. Used in 2-bit mode
#define LCD_CB SysLCDTemp.0 'Clock bit. Used in 2-bit mode

#define LCD_DATA_PORT SysLCDTemp 'Port to connect to LCD data bus. Used in 8-bit mode

#define LCD_DB4 SysLCDTemp.0 'Output bit to interface with LCD data bus. Used in 4-bit mode
#define LCD_DB5 SysLCDTemp.0 'Output bit to interface with LCD data bus. Used in 4-bit mode
#define LCD_DB6 SysLCDTemp.0 'Output bit to interface with LCD data bus. Used in 4-bit mode
#define LCD_DB7 SysLCDTemp.0 'Output bit to interface with LCD data bus. Used in 4-bit mode

#define LCD_RS SysLCDTemp.1 'Output bit to control LCD Register Select. Used as variable by 2-bit mode, and as a pin in 4 and 8 bit mode. DO NOT CHANGE FOR 2-BIT MODE.
#define LCD_RW SysLCDTemp.0 'Output bit to select read/write
#define LCD_Enable SysLCDTemp.0 'Output bit to enable/disable LCD

#define LCD_RSTemp SysLCDTemp.2

'Misc Settings

#define LCD_Write_Delay 2 us  ' now change all delays

#define FLASH 2
#define LCDOFF 3
#define FLASHON 4
'#define LCD_NO_RW
'Lines for bar graph
#define LCD_BAR_EMPTY b'00010001'
#define LCD_BAR_FULL 255

Sub PUT (In LCDPutLine, In LCDPutColumn, In LCDChar)
          LOCATE LCDPutLine, LCDPutColumn
          Set LCD_RS on
          LCDWriteByte(LCDChar)
End Sub

'GET not available in 2-bit mode
Function GET (LCDPutLine, LCDPutColumn)
          Locate LCDPutLine, LCDPutColumn
          Set LCD_RS on
          GET = LCDReadByte
End Function

'LCDColumn is 0 to screen width-1, LCDLine is 0 to screen height-1
Sub LOCATE (In LCDLine, In LCDColumn)
          Set LCD_RS Off
          If LCDLine > 1 Then
                    LCDLine = LCDLine - 2
                    LCDColumn = LCDColumn + 20
          End If

          LCDWriteByte(0x80 or 0x40 * LCDLine + LCDColumn)
          'wait 5 10us
End Sub

Sub CLS
          SET LCD_RS OFF

          'Clear screen
          LCDWriteByte (b'00000001')
          Wait 2 ms

          'Move to start of visible DDRAM
          LCDWriteByte(0x80)
          'Wait 10 10us
End Sub

Sub LCDHOME
          SET LCD_RS OFF

          'Return CURSOR to home
          LCDWriteByte (b'00000010')
          Wait 10 10us
End Sub

Sub LCDcmd ( In LCDValue )
          SET LCD_RS OFF

          'Send Command.... this is unvalidated.. send whatever is passed!
          LCDWriteByte ( LCDValue)
          Wait 4 10us
end sub

'Compatibility with older programs
#define LCDInt Print
#define LCDWord Print

sub InitLCD

          #IFDEF LCD_IO 2
                    SET LCD_DB OFF
                    SET LCD_CB OFF
                    DIR LCD_DB OUT
                    DIR LCD_CB OUT
                    WAIT 35 MS
                    SET LCD_RS OFF
                    LCD2_NIBBLEOUT 0X03
                    Wait 5 ms
                    LCD2_NIBBLEOUT 0X03
                    WAIT 1 MS
                    LCD2_NIBBLEOUT 0X03
                    WAIT 1 MS
                    LCD2_NIBBLEOUT 0X02
                    WAIT 1 MS
                    LCDWriteByte 0x28
                    WAIT 1 MS
                    LCDWriteByte 0x08
                    WAIT 1 MS
                    LCDWriteByte 0x01
                    WAIT 5 MS
                    LCDWriteByte 0x06
                    WAIT 1 MS
                    LCDWriteByte 0x0C
                    WAIT 1 MS
          #ENDIF

          #IFDEF LCD_IO 4,8

                    #IFNDEF LCD_LAT
                           DIR LCD_RS OUT
                    #ENDIF
                    #IFDEF LCD_LAT
                           DIR _LCD_RS OUT
                    #ENDIF

                    #IFNDEF LCD_NO_RW
                              #IFNDEF LCD_LAT
                                     DIR LCD_RW OUT
                              #ENDIF
                              #IFDEF LCD_LAT
                                     DIR _LCD_RW OUT
                              #ENDIF
                    #ENDIF


                    #IFNDEF LCD_LAT
                           DIR LCD_Enable OUT
                    #ENDIF
                    #IFDEF LCD_LAT
                           DIR _LCD_Enable OUT
                    #ENDIF


                    wait 10 ms
                    Wait until LCDReady

                    'Set data bus width
                    SET LCD_RS OFF
          #ENDIF
          #IFDEF LCD_IO 8
                    wait 15 ms    ' added ERV 16082014
                    LCDWriteByte(b'00111000')
                    wait 5 ms    ' added ERV 16082014
                    LCDWriteByte(b'00111000')
                    wait 11  10us    ' added ERV 16082014
                    LCDWriteByte(b'00111000')


          #ENDIF
          #IFDEF LCD_IO 4

          ' revised LCDINIT Evan Venn March 2014
          ' modified by William Roth  July 2104
                    'Set pins to output
                    #IFNDEF LCD_LAT
                        DIR LCD_DB4 OUT
                        DIR LCD_DB5 OUT
                        DIR LCD_DB6 OUT
                        DIR LCD_DB7 OUT
                    #ENDIF
                    #IFDEF LCD_LAT
                        DIR _LCD_DB4 OUT
                        DIR _LCD_DB5 OUT
                        DIR _LCD_DB6 OUT
                        DIR _LCD_DB7 OUT
                    #ENDIF

                    set LCD_RS OFF
                    #IFNDEF LCD_NO_RW
                              set LCD_RW OFF
                    #ENDIF

                    wait 15 ms
                    '0011
                    set LCD_DB4 ON
                    set LCD_DB5 ON
                    set LCD_DB6 OFF
                    set LCD_DB7 OFF
                    wait 2 us
                    PULSEOUT LCD_Enable, 2 us
                    wait 5 ms
                    Repeat 3
                      PULSEOUT LCD_Enable, 2 us
                      Wait 40 us
                      #IFNDEF LCD_NO_RW
                              set LCD_RW OFF
                      #ENDIF
                    end repeat
                    Wait 5 ms


'function set       set 4 bit mode   0x20
                    set LCD_DB4 OFF
                    set LCD_DB5 ON
                    set LCD_DB6 OFF
                    set LCD_DB7 OFF
                    wait 2 us
                    PULSEOUT LCD_Enable, 2 us
                    Wait 50 us

      '================== now in 4 bit mode =================

                    LCDWriteByte(b'011000000') 0x28
                    wait 50 us
                    LCDWriteByte(b'00000001') 0x01 clear
                     wait 4 ms


          #ENDIF
          #IFDEF LCD_IO 4,8

                    'Set Cursor movement
                    SET LCD_RS OFF
                    LCDWriteByte(b'00000110')
                    wait 5 10us
                    'Turn off cursor
                    LCDWriteByte(b'00001100')
                    wait 5 10us

                    'Clear screen
                    CLS
          #ENDIF
end sub

'String output
sub Print (In PrintData As String)
          'PrintLen = LEN(PrintData$)
          PrintLen = PrintData(0)

          If PrintLen = 0 Then Exit Sub
          Set LCD_RS On

          'Write Data
          For SysPrintTemp = 1 To PrintLen
                    LCDWriteByte PrintData(SysPrintTemp)
          Next

End Sub

Sub Print (In LCDValue)

          LCDValueTemp = 0
          Set LCD_RS On

          IF LCDValue >= 100 Then
                    LCDValueTemp = LCDValue / 100
                    LCDValue = SysCalcTempX
                    LCDWriteByte(LCDValueTemp + 48)
          End If
          If LCDValueTemp > 0 Or LCDValue >= 10 Then
                    LCDValueTemp = LCDValue / 10
                    LCDValue = SysCalcTempX
                    LCDWriteByte(LCDValueTemp + 48)
          End If
          LCDWriteByte (LCDValue + 48)
End Sub

Sub Print (In LCDValue As Word)
          Dim SysCalcTempX As Word

          Set LCD_RS On
          LCDValueTemp = 0

          If LCDValue >= 10000 then
                    LCDValueTemp = LCDValue / 10000 [word]
                    LCDValue = SysCalcTempX
                    LCDWriteByte(LCDValueTemp + 48)
                    Goto LCDPrintWord1000
          End If

          If LCDValue >= 1000 then
                    LCDPrintWord1000:
                    LCDValueTemp = LCDValue / 1000 [word]
                    LCDValue = SysCalcTempX
                    LCDWriteByte(LCDValueTemp + 48)
                    Goto LCDPrintWord100
          End If

          If LCDValue >= 100 then
                    LCDPrintWord100:
                    LCDValueTemp = LCDValue / 100 [word]
                    LCDValue = SysCalcTempX
                    LCDWriteByte(LCDValueTemp + 48)
                    Goto LCDPrintWord10
          End If

          If LCDValue >= 10 then
                    LCDPrintWord10:
                    LCDValueTemp = LCDValue / 10 [word]
                    LCDValue = SysCalcTempX
                    LCDWriteByte(LCDValueTemp + 48)
          End If

          LCDWriteByte (LCDValue + 48)
End Sub

Sub Print (In LCDValueInt As Integer)
          Dim LCDValue As Word

          'If sign bit is on, print - sign and then negate
          If LCDValueInt.15 = On Then
                    LCDWriteChar("-")
                    LCDValue = -LCDValueInt

          'Sign bit off, so just copy value
          Else
                    LCDValue = LCDValueInt
          End If

          'Use Print(word) to display value
          Print LCDValue
End Sub

Sub Print (In LCDValue As Long)
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
          Set LCD_RS On
          For SysPrintTemp = SysPrintBuffLen To 1 Step -1
                    LCDWriteByte(SysPrintBuffer(SysPrintTemp) + 48)
          Next

End Sub

sub LCDHex(In LCDValue)
          LCDValueTemp = 0
          Set LCD_RS On

          IF LCDValue >= 0x10 then
                    LCDValueTemp = LCDValue / 0x10
                    LCDValue = SysCalcTempX
                    LCDHexValueTemp = LCDValueTemp
                    if LCDHexValueTemp > 9 then LCDHexValueTemp = LCDHexValueTemp + 7
                    LCDWriteByte(LCDHexValueTemp + 48)
          end if

          LCDHexValueTemp = LCDValue
          if LCDHexValueTemp > 9 then LCDHexValueTemp = LCDHexValueTemp + 7
          LCDWriteByte (LCDHexValueTemp + 48)

end sub

sub LCDWriteChar(In LCDChar)
          set LCD_RS on
          LCDWriteByte(LCDChar)
end sub

function LCDReady
          #IFDEF LCD_NO_RW
                    Wait 10 ms
                    LCDReady = TRUE
                    exit function
          #ENDIF

          #IFNDEF LCD_NO_RW
                    #IFDEF LCD_IO 2
                              LCDReady = TRUE
                              exit function
                    #ENDIF

                    #IFDEF LCD_IO 4,8
                              LCDReady = FALSE
                              LCD_RSTemp = LCD_RS

                              SET LCD_RW ON
                              SET LCD_RS OFF
                              Wait 5 10us
                              SET LCD_Enable ON
                              wait 5 10us
                              #IFDEF LCD_IO 4

                                        #IFNDEF LCD_LAT
                                               dir LCD_DB7 IN
                                        #ENDIF
                                        #IFDEF LCD_LAT
                                               dir _LCD_DB7 IN
                                        #ENDIF



                                        if LCD_DB7 OFF THEN LCDReady = TRUE
                              #ENDIF
                              #IFDEF LCD_IO 8


                                        #IFNDEF LCD_LAT
                                               dir LCD_DATA_PORT In
                                        #ENDIF
                                        #IFDEF LCD_LAT
                                               dir _LCD_DATA_PORT In
                                        #ENDIF
                                        if LCD_DATA_PORT.7 OFF THEN LCDReady = TRUE
                              #ENDIF
                              SET LCD_Enable OFF
                              wait 25 10us
                              LCD_RS = LCD_RSTemp
                    #ENDIF
          #ENDIF
end function

sub LCDNormalWriteByte(In LCDByte)

' *****    This sub modified by William Roth *****

'Reduced enable pulse to 1 us
'Added conditional delay for for LCD_SLOW

          #IFNDEF LCD_NO_RW
                    #IFDEF LCD_IO 4,8
                              wait until LCDReady
                              set LCD_RW OFF 'Write mode
                    #ENDIF
          #ENDIF

          #IFDEF LCD_IO 2
                    LCD2_NIBBLEOUT Swap4(LCDByte)           'Swap byte; Most significant NIBBLE sent first
                    LCD2_NIBBLEOUT LCDByte                  'Less Significant NIBBLE output
          #ENDIF

          #IFDEF LCD_IO 4
                    'Set pins to output
                    #IFNDEF LCD_LAT
                        DIR LCD_DB4 OUT
                        DIR LCD_DB5 OUT
                        DIR LCD_DB6 OUT
                        DIR LCD_DB7 OUT
                    #ENDIF
                    #IFDEF LCD_LAT
                        DIR _LCD_DB4 OUT
                        DIR _LCD_DB5 OUT
                        DIR _LCD_DB6 OUT
                        DIR _LCD_DB7 OUT
                    #ENDIF


                    'Write upper nibble to output pins
                    set LCD_DB4 OFF
                    set LCD_DB5 OFF
                    set LCD_DB6 OFF
                    set LCD_DB7 OFF
                    if LCDByte.7 ON THEN SET LCD_DB7 ON
                    if LCDByte.6 ON THEN SET LCD_DB6 ON
                    if LCDByte.5 ON THEN SET LCD_DB5 ON
                    if LCDByte.4 ON THEN SET LCD_DB4 ON
                    wait 1 us
                  '************************************
                        pulseout LCD_enable, 2 us
                        Wait 2 us
                  '************************************

                    'Write lower nybble to output pins
                    set LCD_DB4 OFF
                    set LCD_DB5 OFF
                    set LCD_DB6 OFF
                    set LCD_DB7 OFF
                    if LCDByte.3 ON THEN SET LCD_DB7 ON
                    if LCDByte.2 ON THEN SET LCD_DB6 ON
                    if LCDByte.1 ON THEN SET LCD_DB5 ON
                    if LCDByte.0 ON THEN SET LCD_DB4 ON
                    wait 1 us
                    'Pulse enable pin
                 '*********************************
                       pulseout LCD_enable, 2 us
                       wait 2 us
                 '***********************************

                    'Set data pins low again
                    SET LCD_DB7 OFF
                    SET LCD_DB6 OFF
                    SET LCD_DB5 OFF
                    SET LCD_DB4 OFF
              '*******************************************

               '
                    #IFDEF LCD_SPEED FAST
                        wait 30  us
                    #ENDIF

                    #IFDEF LCD_SPEED MEDIUM
                        wait 50 us
                    #ENDIF

                    #IFDEF LCD_SPEED SLOW  'Charcter rate = 20K
                        wait 80 us      'character delay time
                    #ENDIF

                    #IFNDEF LCD_Speed  ' default to slow
                       wait 80 us
                    #ENDIF
               '*********************************************

          #ENDIF

          #IFDEF LCD_IO 8
                    'Set data port to output, and write a value to it
                    #IFNDEF LCD_LAT
                            DIR LCD_DATA_PORT out
                    #ENDIF
                    #IFDEF LCD_LAT
                           DIR _LCD_DATA_PORT out
                    #ENDIF
                    LCD_DATA_PORT = LCDByte

               '********Added / Changed *************************

                     wait 2 us
                     pulseout LCD_enable, 2 us
                     Wait 2  us
                     LCD_DATA_PORT = 0

                    #IFDEF LCD_Speed FAST   'Char Rate ~20K
                       wait 30 us
                    #ENDIF

                    #IFDEF LCD_SPEED MEDIUM  'Char Rate ~15K
                        wait 50 us
                    #ENDIF

                    #IFDEF LCD_SPEED SLOW  ' Char Rate ~ 10K
                        wait 80 us
                    #ENDIF

                    #IFNDEF LCD_SPEED
                       wait 80 us
                    #ENDIF
                 '*********************************

          #ENDIF

end sub

SUB LCD2_NIBBLEOUT (In LCD2BYTE)
 'Set Data and Clock bits off
 SET LCD_DB OFF
 SET LCD_CB OFF

 'Clear Shift Register With six 0s prior to loading
 FOR LCDTemp = 1 TO 6
  SET LCD_CB ON  ' STROBE
  SET LCD_CB OFF ' ======
 NEXT

 SET LCD_DB ON  ' First bit out to Shift register, always 1 for E gate
 SET LCD_CB ON  ' STROBE
 SET LCD_CB OFF ' ======
 IF LCD_RS OFF THEN SET LCD_DB OFF ' Second bit out equal to R/S
 SET LCD_CB ON  ' STROBE
 SET LCD_CB OFF ' ======

 '4 bits Data out to Shift register, starting from Nibble most significant bit
 FOR LCDTemp = 1 to 4
  SET LCD_DB OFF
  IF LCD2Byte.3 ON THEN SET LCD_DB ON
  SET LCD_CB ON
  SET LCD_CB OFF
  ROTATE LCD2Byte LEFT
 NEXT

SET LCD_DB ON                                     ' Last pulse for Nibble output. Not for Shift register
WAIT 1 MS                               ' Put E pin on LCD to one through an AND operation
SET LCD_DB OFF                                    ' with the first bit outputted (E)

END SUB

function LCDNormalReadByte
          #IFDEF LCD_NO_RW
                    LCDReadByte = 0
                    Exit Function
          #ENDIF

          #IFNDEF LCD_NO_RW

                    set LCD_RW ON 'Read mode
                    LCDReadByte = 0

                    #IFDEF LCD_IO 4
                              'Set pins to input
                              DIR LCD_DB4 IN
                              DIR LCD_DB5 IN
                              DIR LCD_DB6 IN
                              DIR LCD_DB7 IN

                              'Read upper nibble from input pins
                              SET LCD_Enable ON
                              Wait LCD_Write_Delay
                              if LCD_DB7 ON then SET LCDReadByte.7 ON
                              if LCD_DB6 ON THEN SET LCDReadByte.6 ON
                              if LCD_DB5 ON then SET LCDReadByte.5 ON
                              if LCD_DB4 ON THEN SET LCDReadByte.4 ON
                              SET LCD_Enable OFF
                              Wait 2 10us

                              'Read lower nibble from input pins
                              SET LCD_Enable ON
                              Wait LCD_Write_Delay
                              if LCD_DB7 ON then SET LCDReadByte.3 ON
                              if LCD_DB6 ON THEN SET LCDReadByte.2 ON
                              if LCD_DB5 ON then SET LCDReadByte.1 ON
                              if LCD_DB4 ON THEN SET LCDReadByte.0 ON
                              SET LCD_Enable OFF
                              Wait 2 10us
                    #ENDIF

                    #IFDEF LCD_IO 8
                              DIR LCD_DATA_PORT 255
                              SET LCD_Enable ON
                              Wait LCD_Write_Delay
                              LCDReadByte = LCD_DATA_PORT
                              SET LCD_Enable OFF
                              Wait 2 10us
                    #ENDIF
          #ENDIF
end function

sub LCDCursor(In LCDCRSR)
                 ' Revised Evan Venn March 2014
          'Can be ON, FLASH or OFF, FLASHON
          Set LCD_RS OFF
          LCDTemp = 12 '0 or 12
          If LCDCRSR = 11 Then LCDTemp = 11
          If LCDCRSR = OFF Then LCDTemp = 12
          If LCDCRSR = ON Then LCDTemp = 14 '2 or 12
          If LCDCRSR = FLASH Then LCDTemp = 13 '3 or 12
          If LCDCRSR = FLASHON Then LCDTemp = 15
          LCDWriteByte(LCDTemp) '(LCDTemp or 12)
          ;Wait 5 10us

end sub

'Create a custom character in CGRAM
sub LCDCreateChar (In LCDCharLoc, LCDCharData())

          'Store old location
          #IFDEF LCD_IO 4,8
                    Set LCD_RS Off
                    LCDLoc = LCDReadByte
                    Set LCDLoc.7 On
          #ENDIF

          'Select location
          Set LCD_RS Off
          LCDWriteByte (64 + LCDCharLoc * 8)
          wait 5 10us

          'Write char
          Set LCD_RS On
          For LCDTemp = 1 to 8
                    LCDWriteByte LCDCharData(LCDTemp)
                    wait 5 10us
          Next

          'Restore location
          #IFDEF LCD_IO 2
                    set LCD_RS off
                    LCDWriteByte(0x80)
                    wait 5 10us
          #ENDIF
          #IFDEF LCD_IO 4,8
                    Set LCD_RS Off
                    LCDWriteByte (LCDLoc)
                    wait 5 10us
          #ENDIF

end sub

'Create a graph character in CGRAM
Sub LCDCreateGraph(In LCDCharLoc, In LCDValue)

          'Store old location
          #IFDEF LCD_IO 4,8
                    Set LCD_RS Off
                    LCDLoc = LCDReadByte
                    Set LCDLoc.7 On
          #ENDIF

          'Select location
          Set LCD_RS Off
          LCDWriteByte (64 + LCDCharLoc * 8)
          wait 5 10us

          'Write char for graph
          Set LCD_RS On
          For LCDTemp = 8 to 1
                    If LCDTemp > LCDValue Then
                              LCDWriteByte LCD_BAR_EMPTY
                    Else
                              LCDWriteByte LCD_BAR_FULL
                    End If
                    wait 5 10us
          Next

          'Restore location
          #IFDEF LCD_IO 2
                    set LCD_RS off
                    LCDWriteByte(0x80)
                    wait 5 10us
          #ENDIF
          #IFDEF LCD_IO 4,8
                    Set LCD_RS Off
                    LCDWriteByte (LCDLoc)
                    wait 5 10us
          #ENDIF

End Sub

' Sub to print a number of spaces - upto 40
sub LCDSpace(in LCDValue)
          set LCD_RS on
          if LCDValue > 40 then LCDValue = 40
          do until LCDValue = 0
                    LCDWriteByte(32)
                    LCDValue --
          loop
end sub

sub LCDOn
   set LCD_RS OFF
    lcdwritebyte (b'00001100')
    wait 5 ms
end Sub

sub LCDOn
    set LCD_RS OFF
    lcdwritebyte (b'00001000')
    wait 5 ms  'intentional delay for better effect. Not necessary
end sub
