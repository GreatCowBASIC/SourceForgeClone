'    USART routines for Great Cow BASIC
'    Copyright (C) 2009 - 2013, 2015, 2016 Hugh Considine, William Roth, Evan Venn and Mike Otte

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

'Changes:
' 12/12/2009: Corrected TXIF checking
' 8/2/2010: Added delay after each byte in print
' 7/6/2011: Added HSerPrint(integer)
' 17/7/2011: Fixes for ATmega328P
' 12/2/2012: Added HSerPrint(long)
' 2/6/2013: Added In directive to HSerPrint(string)
' 10/7/2013: Added USART_TX_BLOCKING option, fixes for chips with 2 modules
' 29/9/2013: Fixes for ATmega32u4
' 16/2/2014: Fixed HERPRINT Long Bug
' 28/6/2014: Changed HSerPrintCRLF to have a parameter... you can have lots of CRLF's
' 04/07/2015:  Improved timing
' 31/7/2015: Fix Compile Error for 16F1705 1709  and other Pics - WMR
'            See lines 311 - 320
' 02/10/2015: Fix for missing SPBRG - mapped to SPBRGL
' 11/10/2015: Fixed bug for missing SPBRG - mapped to SPBRGL in script
' 31/10/2015: Added second usart capabilities, HSerGetNum,HSerGetString -mlo
' 9/12/2015: Changed default USART_DELAY from 12 ms back to 1 ms - fix very slow USART default!
'            Corrected dates above
' 18/12/2015:Added new function.methods HSerReceive1, HSerReceive2, HSerReceiveFrom
' 18/12/2015: Repositioned functions
' 19/12/2015: Revised to support CREN2 and SPEN2
'	07/01/2016:	Added AVR 4 channel USARTs
' 24/01/2016: Added default value to comport


'For compatibility with USART routines in Contributors forum, add this line:
'#define USART_BLOCKING
'This will cause send and receive to wait until data can be sent or received
'To make only the send routines blocking, add this line:

'USART settings
'#define USART_BAUD_RATE 9600
'#define USART_BLOCKING   'waits for byte to be received and transmitted
'#define USART_TX_BLOCKING ' waits for empty Tx input so it won't overwrite
'#define USART2_BAUD_RATE 9600
'#define USART2_BLOCKING	'Usart 2 waits for byte to be received
'#define USART2_TX_BLOCKING ' Usart 2 waits for empty Tx input so it won't overwrite
'#define USART3_BAUD_RATE 9600
'#define USART3_BLOCKING	'Usart 3 waits for byte to be received
'#define USART3_TX_BLOCKING ' Usart 3 waits for empty Tx input so it won't overwrite
'#define USART4_BAUD_RATE 9600
'#define USART4_BLOCKING	'Usart 4 waits for byte to be received
'#define USART4_TX_BLOCKING ' Usart 4 waits for empty Tx input so it won't overwrite

'#define SerPrintCR  'prints a CR at the end of every print Sub
'#define SerPrintLF  'prints a LF at the end of every print Sub

'Remember also to set the appropriate Comport Pins for direction In and Out

'Note that the HSerReceive routine is implemented differently. In this
'library, it is a subroutine, as with SerReceive in rs232.h. This is purely
'a design decision, and has been made to keep included routines consistent
'with each other.

'To slow down print, set this delay:
'(Setting to 0 ms will remove all delays)
#define USART_DELAY 1 ms

'When Not using USART_BLOCKING the return value is defined by
#define DefaultUsartReturnValue = 255

'Some wrappers for compatibility with Contributors USART routines
#define HserPrintByte HSerPrint

Sub HserPrintByteCRLF(In PrintValue,optional In comport =1)
	HSerPrint(PrintValue)
	HSerSend(13,comport)
  HSerSend(10,comport)
End Sub

Sub HserPrintCRLF  ( Optional in HSerPrintCRLFCount = 1,Optional In comport =1 )
    repeat HSerPrintCRLFCount
			HSerSend(13,comport)
			Wait USART_DELAY
			HSerSend(10,comport)
			wait USART_DELAY
    end Repeat
End Sub

'Script to calculate baud rate generator values
'Also sets constants to check if byte received
#script
	If PIC Then
		If Bit(RC1IF) Then
			USARTHasData = "RC1IF = On"
		End If
		If NoBit(RC1IF) Then
			USARTHasData = "RCIF = On"
		End If
		If Bit(RC2IF) Then
			USART2HasData = "RC2IF = On"
		End If
' USART 1 baud calculation
		If USART_BAUD_RATE Then
			'Find best baud rate
			If Bit(BRG16) Then
         'Try 16 bit /4 (H = 1, 16 = 1)
				SPBRG_TEMP = ChipMHz / USART_BAUD_RATE / 4 * 1000000 - 1
				BRGH_TEMP = 1
				BRG16_TEMP = 1

        If SPBRG_TEMP > 65535 Then
           'If too high, try 16 bit /16 (H = 0, 16 = 1)
					SPBRG_TEMP = ChipMHz / USART_BAUD_RATE / 16 * 1000000 - 1
					BRGH_TEMP = 0
					BRG16_TEMP = 1
					If SPBRG_TEMP < 256 Then
						'If low enough, use 8 bit /16 (H = 1, 16 = 0)
						BRGH_TEMP = 1
						BRG16_TEMP = 0
					End If
					'If value is too high, baud rate is too low
					If SPBRG_TEMP > 65535 Then
						Error Msg(UsartBaudTooLow)
					End If
				End If
			End If
			If NoBit(BRG16) Then
         'Try /16
				SPBRG_TEMP = ChipMHz / USART_BAUD_RATE / 16 * 1000000 - 1
				BRGH_TEMP = 1
				'If too high, try /64
				If SPBRG_TEMP > 255 Then
					SPBRG_TEMP = ChipMHz / USART_BAUD_RATE / 64 * 1000000 - 1
					BRGH_TEMP = 0
					'If still too high, error
					If SPBRG_TEMP > 255 Then
						Error Msg(UsartBaudTooLow)
					End If
				End If
			End If

			'Get high and low bytes
			SPBRGL_TEMP = Int(SPBRG_TEMP) And 255
			SPBRGH_TEMP = Int(SPBRG_TEMP / 256)
		End If


          'added 02.10.2015 to resolve PIC devices without SPBRG define.  SPBRG needs to map to SPBRGL
      'fixes the list of known devices as follows
      '                16f1454.dat
      '                16f1455.dat
      '                16f1458.dat
      '                16f1459.dat
      '                16f1508.dat
      '                16f1509.dat
    if  NoVar(SPBRG) then
    	SPBRG = SPBRGL
    end if
'USART2
		If USART2_BAUD_RATE Then
			'Find best baud rate
			If Bit(BAUDCON2_BRG16) Then
         'Try 16 bit /4 (H = 1, 16 = 1)
				SPBRG_TEMP2 = ChipMHz / USART2_BAUD_RATE / 4 * 1000000 - 1
				BRGH_TEMP2 = 1
				BRG16_TEMP2 = 1

				If SPBRG_TEMP2 > 65535 Then
           'If too high, try 16 bit /16 (H = 0, 16 = 1)
					SPBRG_TEMP2 = ChipMHz / USART2_BAUD_RATE / 16 * 1000000 - 1
					BRGH_TEMP2 = 0
					BRG16_TEMP2 = 1
					If SPBRG_TEMP2 < 256 Then
						'If low enough, use 8 bit /16 (H = 1, 16 = 0)
						BRGH_TEMP2 = 1
						BRG16_TEMP2 = 0
					End If
					'If value is too high, baud rate is too low
					If SPBRG_TEMP2 > 65535 Then
						Error Msg(UsartBaud2_TooLow)
					End If
				End If
			End If
					'Get high and low bytes
			SPBRGL_TEMP2 = Int(SPBRG_TEMP2) And 255
			SPBRGH_TEMP2 = Int(SPBRG_TEMP2 / 256)
		End If
	End If
	If AVR Then
'AVR Usart has data flags setup
		If Bit(RXC0) Then
			USARTHasData = "RXC0 = On"
		End If
		If NoBit(RXC0) Then
			USARTHasData = "RXC = On"
		End If
		If Bit(RXC1) Then
			USART2HasData = "RXC1 = On"
		End If
		If Bit(RXC2) Then
			USART3HasData = "RXC2 = On"
		End If
		If Bit(RXC3) Then
			USART4HasData = "RXC3 = On"
		End If
'AVR USART 0 baud calc
		If USART_BAUD_RATE Then
			UBRR_TEMP = ChipMHz * 1000000 / (16 * USART_BAUD_RATE) - 1
			U2X0_TEMP = 0
			'If using a high speed, use double speed mode
			If UBRR_TEMP < 16384 Then
				UBRR_TEMP = ChipMHz * 1000000 / (8 * USART_BAUD_RATE) - 1
				U2X0_TEMP = 1
			End If

			'Check that rate will work
			If UBRR_TEMP > 65535 Then
				Error Msg(UsartBaudTooLow)
			End If

			'Get high and low bytes
			UBRRL_TEMP = Int(UBRR_TEMP) And 255
			UBRRH_TEMP = Int(UBRR_TEMP / 256)
		End If
'AVR USART 1 baud calc
		If USART2_BAUD_RATE Then
			UBRR2_TEMP = ChipMHz * 1000000 / (16 * USART2_BAUD_RATE) - 1
			U2X02_TEMP = 0
			'If using a high speed, use double speed mode
			If UBRR2_TEMP < 16384 Then
				UBRR2_TEMP = ChipMHz * 1000000 / (8 * USART2_BAUD_RATE) - 1
				U2X02_TEMP = 1
			End If

			'Check that rate will work
			If UBRR2_TEMP > 65535 Then
				Error Msg(UsartBaudTooLow)
			End If

			'Get high and low bytes
			UBRRL2_TEMP = Int(UBRR2_TEMP) And 255
			UBRRH2_TEMP = Int(UBRR2_TEMP / 256)
		End If
'AVR USART 2 baud clac
    If USART3_BAUD_RATE Then
			UBRR3_TEMP = ChipMHz * 1000000 / (16 * USART3_BAUD_RATE) - 1
			U2X03_TEMP = 0
			'If using a high speed, use double speed mode
			If UBRR3_TEMP < 16384 Then
				UBRR3_TEMP = ChipMHz * 1000000 / (8 * USART3_BAUD_RATE) - 1
				U2X03_TEMP = 1
			End If

			'Check that rate will work
			If UBRR3_TEMP > 65535 Then
				Error Msg(UsartBaudTooLow)
			End If

			'Get high and low bytes
			UBRRL3_TEMP = Int(UBRR3_TEMP) And 255
			UBRRH3_TEMP = Int(UBRR3_TEMP / 256)
		End If
'AVR USART 2 baud clac
		If USART4_BAUD_RATE Then
			UBRR4_TEMP = ChipMHz * 1000000 / (16 * USART4_BAUD_RATE) - 1
			U2X04_TEMP = 0
			'If using a high speed, use double speed mode
			If UBRR4_TEMP < 16384 Then
				UBRR4_TEMP = ChipMHz * 1000000 / (8 * USART4_BAUD_RATE) - 1
				U2X04_TEMP = 1
			End If

			'Check that rate will work
			If UBRR4_TEMP > 65535 Then
				Error Msg(UsartBaudTooLow)
			End If

			'Get high and low bytes
			UBRRL4_TEMP = Int(UBRR4_TEMP) And 255
			UBRRH4_TEMP = Int(UBRR4_TEMP / 256)
		End If
	End If
#endscript

#startup InitUSART

Sub InitUSART

	'Set the default value for comport
	comport = 1

	#ifdef PIC
  	#If USART_BAUD_RATE Then
      #ifndef Bit(TXEN1)
        'Set baud rate
        SPBRG = SPBRGL_TEMP
        #ifdef Bit(BRG16)
          SPBRGH = SPBRGH_TEMP
          BRG16 = BRG16_TEMP
        #endif
        BRGH = BRGH_TEMP

        'Enable async mode
        Set SYNC Off
        Set SPEN On

        'Enable TX and RX
        Set CREN On
        Set TXEN On
      #endif
      #ifdef Bit(TXEN1)
        'Set baud rate
        SPBRG1 = SPBRGL_TEMP
        #ifdef Bit(BRG16)
          SPBRGH1 = SPBRGH_TEMP
          BAUDCON1.BRG16 = BRG16_TEMP
        #endif

    ;  Section added to support chips with TXSTA1 instead of TX1STA
        #ifdef Var(TX1STA)
        TX1STA.BRGH = BRGH_TEMP
        #endif

        #ifdef  Var(TXSTA1)
         TXSTA1.BRGH = BRGH_TEMP
        #endif

        'Enable async mode
        Set SYNC1 Off
        Set SPEN1 On

        'Enable TX and RX
        Set CREN1 On
        Set TXEN1 On
      #endif
    #endif
' PIC USART 2 Init
    #if USART2_BAUD_RATE Then
      #ifdef Bit(TXSTA2_TXEN)
			'Set baud rate
				SPBRG2 = SPBRGL_TEMP2
				#ifdef Bit(BAUDCON2_BRG16)
					SPBRGH2 = SPBRGH_TEMP2
					BAUDCON2_BRG16 = BRG16_TEMP2
				#endif
				TXSTA2_BRGH = BRGH_TEMP2

				'Enable async mode
				Set TXSTA2_SYNC Off
				Set SPEN2 On  ' SPEN2

				'Enable TX and RX
				Set CREN2 On
				Set TXEN2 On
			#endif
      #ifdef Bit(TX2STA_TXEN)
			'Set baud rate
				SPBRG2 = SPBRGL_TEMP2
				#ifdef Bit(BAUD2CON_BRG16)
					SPBRGH2 = SPBRGH_TEMP2
					BAUD2CON_BRG16 = BRG16_TEMP2
				#endif
				TX2STA_BRGH = BRGH_TEMP2

				'Enable async mode
				Set TX2STA_SYNC Off
				Set RC2STA_SPEN On

				'Enable TX and RX
				Set RC2STA_CREN On
				Set TX2STA_TXEN On
			#endif
    #endif
	#endif

	#ifdef AVR
		#If USART_BAUD_RATE Then
      'Set baud rate
      #ifndef Bit(U2X0)

        #ifndef Bit(U2X1)				'Check for second usart? why
          U2X = U2X0_TEMP				'U2X in tiny2313 has one usart
          UBRRL = UBRRL_TEMP
          UBRRH = UBRRH_TEMP
        #endif
      #endif
      #ifdef Bit(U2X0)
        U2X0 = U2X0_TEMP				'U2X0 ex: mega328P
        UBRR0L = UBRRL_TEMP
        UBRR0H = UBRRH_TEMP
      #endif

      'Enable TX and RX
      #ifndef Bit(RXEN0)
        RXEN = On
        TXEN = On
      #endif
      #ifdef Bit(RXEN0)
        RXEN0 = On
        TXEN0 = On
      #endif
		#endif

    #If USART2_BAUD_RATE Then
    		' set baud reg
       #ifdef Bit(U2X1)				'Multiple port chips
          U2X1 = U2X02_TEMP			'speed doubling bit ex:Mega 128,2560
          UBRR1L = UBRRL2_TEMP		'baudrate register low
          UBRR1H = UBRRH2_TEMP		'baudrate register h
       #endif
       'Enable TX and RX
       #ifdef Bit(RXEN1)
       	RXEN1 = On
        TXEN1 = On
       #endif
    #endif

    #If USART3_BAUD_RATE Then
    		' set baud reg
       #ifdef Bit(U2X2)				'Multiple port chips
          U2X2 = U2X03_TEMP			'speed doubling bit ex:Mega 128,2560
          UBRR2L = UBRRL3_TEMP		'baudrate register low
          UBRR2H = UBRRH3_TEMP		'baudrate register h
       #endif
       'Enable TX and RX
       #ifdef Bit(RXEN2)
       	RXEN2 = On
        TXEN2 = On
       #endif
    #endif

    #If USART4_BAUD_RATE Then
    		' set baud reg
       #ifdef Bit(U2X3)				'Multiple port chips
          U2X3 = U2X04_TEMP			'speed doubling bit ex:Mega 128,2560
          UBRR3L = UBRRL4_TEMP		'baudrate register low
          UBRR3H = UBRRH4_TEMP		'baudrate register h
       #endif
       'Enable TX and RX
       #ifdef Bit(RXEN3)
       	RXEN3 = On
        TXEN3 = On
       #endif
    #endif

	#endif
End Sub

sub HSerSend(In SerData, optional In comport = 1)
	'Block before sending (if needed)
	'Send byte
	#ifdef PIC
  	#If USART_BAUD_RATE Then
      if comport = 1 Then
        'HSerSendBlocker
        #ifdef USART_BLOCKING
          #ifndef Bit(TX1IF)
            Wait While TXIF = Off
          #endif
          #ifdef Bit(TX1IF)
            Wait While TX1IF = Off
          #endif
        #endif
        #ifdef USART_TX_BLOCKING
          #ifndef Bit(TX1IF)
            Wait While TXIF = Off
          #endif
          #ifdef Bit(TX1IF)
            Wait While TX1IF = Off
          #endif
        #endif
        #ifndef Var(TXREG1)
          TXREG = SerData
        #endif
        #ifdef Var(TXREG1)
          TXREG1 = SerData
        #endif
      end if
    #endif
    #If USART2_BAUD_RATE Then
      if comport = 2 Then
        #ifdef USART_BLOCKING     'Blocking TX and RX
          #ifdef Bit(TX2IF)
            Wait While TX2IF = Off
          #endif
        #endif
        #ifdef USART_TX_BLOCKING     'TX blocking only
          #ifdef Bit(TX2IF)
            Wait While TX2IF = Off
          #endif
        #endif
        #ifndef Var(TXREG2)		'? TX2REG
          TXREG2 = SerData		'?
        #endif
        #ifdef Var(TX2REG)
          TX2REG = SerData
        #endif

      end if
    #endif
	#endif

	#ifdef AVR
'AVR USART1 Send
		#If USART_BAUD_RATE Then
     if comport = 1 Then
      'HSerSendBlocker		'comport 1 blocker
      #ifdef USART_BLOCKING
        #ifdef Bit(UDRE0)
          Wait While UDRE0 = Off		'Blocking Both Transmit buffer empty ,ready for data
        #endif
        #ifndef Bit(UDRE0)
					Wait While UDRE = Off
				#endif
      #endif
      #ifdef  USART_TX_BLOCKING
        #ifdef Bit(UDRE0)
          Wait While UDRE0 = Off		'Blocking Transmit buffer empty ,ready for data
        #endif
        #ifndef Bit(UDRE0)
					Wait While UDRE = Off
				#endif
      #endif
      #ifdef Var(UDR)
          UDR = SerData
      #endif
      '#ifndef Var(UDR0)
      '	#ifdef Var(UDR1)
      '		UDR1 = SerData		'? second comport
      '	#endif
      '#endif
      #ifdef Var(UDR0)
        UDR0 = SerData
      #endif
      End If
		#endif
'AVR USART 2 send
	  #If USART2_BAUD_RATE Then
      if comport = 2 Then
        #ifdef USART2_BLOCKING
          #ifdef Bit(UDRE1)				'comport 2 TX and Rxblocker
            Wait While UDRE1 = Off		'Transmit buffer empty ,ready for data
          #endif
        #endif
        #ifdef USART2_TX_BLOCKING
          #ifdef Bit(UDRE1)				'comport 2 TX blocker
            Wait While UDRE1 = Off		'Transmit buffer empty ,ready for data
          #endif
        #endif
        #ifdef Var(UDR1)
            UDR1 = SerData
        #endif
      End If
    #endif
'AVR USART 3 send
    #If USART3_BAUD_RATE Then
      if comport = 3 Then
        #ifdef USART3_BLOCKING
          #ifdef Bit(UDRE2)				'comport 3 TX and Rx blocker
            Wait While UDRE2 = Off		'Transmit buffer empty ,ready for data
          #endif
        #endif
        #ifdef USART3_TX_BLOCKING
          #ifdef Bit(UDRE2)				'comport 3 TX blocker
            Wait While UDRE2 = Off		'Transmit buffer empty ,ready for data
          #endif
        #endif
        #ifdef Var(UDR2)
            UDR2 = SerData
        #endif
      End If
    #endif
'AVR USART 4 send
    #If USART4_BAUD_RATE Then
      if comport = 4 Then
        #ifdef USART4_BLOCKING
          #ifdef Bit(UDRE3)				'comport 4 TX and RX  blocker
            Wait While UDRE3 = Off		'Transmit buffer empty ,ready for data
          #endif
        #endif
        #ifdef USART4_TX_BLOCKING
          #ifdef Bit(UDRE3)				'comport 4 TX blocker
            Wait While UDRE3 = Off		'Transmit buffer empty ,ready for data
          #endif
        #endif
        #ifdef Var(UDR3)
            UDR3 = SerData
        #endif
      End If
    #endif
	#endif
end sub

Function HSerReceive
	Comport = 1
  HSerReceive( SerData )
	HSerReceive = SerData
End Function

Function HSerReceive1
	Comport = 1
  HSerReceive( SerData )
	HSerReceive1 = SerData
End Function


Function HSerReceive2
	Comport = 2
	HSerReceive( SerData )
	HSerReceive2 = SerData
End Function


Function HSerReceiveFrom ( Optional in comport = 1 )
	Comport = comport
	HSerReceive( SerData )
	HSerReceiveFrom = SerData
End Function


Sub HSerReceive(Out SerData)
	'Needs comport to be set first
	#ifdef PIC
  	#If USART_BAUD_RATE Then
      if comport = 1 Then
      	SerData = DefaultUsartReturnValue
        'If set up to block, wait for data
        #ifdef USART_BLOCKING
        Wait Until USARTHasData
        #endif
        #ifndef Var(RCREG1)
          'Get a bytes from FIFO
          If USARTHasData Then
            SerData = RCREG
          End if

          'Clear error
          If OERR Then
            Set CREN Off
            Set CREN On
          End If
        #endif

        #ifdef Var(RCREG1)
          'Get a bytes from FIFO
          If USARTHasData Then
            SerData = RCREG1
          End if

          #ifdef bit(OERR1)
          'Clear error
            If OERR1 Then
              Set CREN1 Off
              Set CREN1 On
            End If
          #endif
          #Ifndef bit(OERR1) '  For Chips with RCREG1 but no OEER1
            #IFDEF Bit(OERR)
              IF OERR then
                Set CREN off
                Set CREN On
              END IF
            #ENDIF
            #ENDIF
        #endif
      end if
    #endif
'PIC USART 2 receive
    #If USART2_BAUD_RATE Then
      if comport = 2 Then
				SerData = DefaultUsartReturnValue
        #ifdef USART2_BLOCKING
        Wait Until USART2HasData
        #endif
        #ifdef Var(RC2REG)
          'Get a bytes from FIFO
          If USART2HasData Then
            SerData = RC2REG
          End if

        #endif
        #ifndef Var(RC2REG)
          'Get a bytes from FIFO
          If USART2HasData Then
            SerData = RCREG2
          End if
				#endif
        #ifdef bit(OERR2)
          'Clear error
            If OERR2 Then
              Set CREN2 Off
              Set CREN2 On
            End If
        #endif

      end if
    #endif
	#endif

'AVR USART 1 receive
	#ifdef AVR
  	#If USART_BAUD_RATE Then
      if comport = 1 Then
      	SerData = DefaultUsartReturnValue
        'If set up to block, wait for data
        #ifdef USART_BLOCKING
        Wait Until USARTHasData
        #endif
        If USARTHasData Then
          #ifndef Var(UDR0)
            #ifdef Var(UDR1)
              SerData = UDR1
            #endif
            #ifndef Var(UDR1)
              SerData = UDR
            #endif
          #endif
          #ifdef Var(UDR0)
            SerData = UDR0
          #endif
        End If
      End If
    #endif

'AVR USART 2 receive
    #If USART2_BAUD_RATE Then
        if comport = 2 Then
          SerData = DefaultUsartReturnValue
          'If set up to block, wait for data
          #ifdef USART2_BLOCKING
          Wait Until USART2HasData
          #endif
          If USART2HasData Then
            #ifdef Var(UDR1)
              SerData = UDR1
            #endif
          End If
        End If
    #endif

'AVR USART 3 receive
    #If USART3_BAUD_RATE Then
        if comport = 3 Then
          SerData = DefaultUsartReturnValue
          'If set up to block, wait for data
          #ifdef USART3_BLOCKING
          Wait Until USART3HasData
          #endif
          If USART3HasData Then
            #ifdef Var(UDR2)
              SerData = UDR2
            #endif
          End If
        End If
      #endif

'AVR USART 4 receive
    #If USART4_BAUD_RATE Then
        if comport = 4 Then
          SerData = DefaultUsartReturnValue
          'If set up to block, wait for data
          #ifdef USART4_BLOCKING
          Wait Until USART4HasData
          #endif
          If USART4HasData Then
            #ifdef Var(UDR3)
              SerData = UDR3
            #endif
          End If
        End If
      #endif
	#endif
End Sub




'Added  11/4/2015 by mlo
' A number is input to a USART as a series of ASCII digits with a CR at the end
'Output Value is in range of 0 to 65535 (Dec)
'Input value is entered as decimal digits
sub HSerGetNum (Out HSerNum As Word, optional In comport = 1)
	Dim HSerDataIn(7)
	HSerIndex = 0
  HSerNum = 0

	Do
  	comport = comport 'not really required but added for clarity
    HSerReceive( HSerInByte )
    'Enter key?
    If HSerInByte = 13 Then
    	For HSerTemp = 1 to HSerIndex
     		HSerNum = HSerNum * 10 + HSerDataIn(HSerTemp) - 48
  		Next
      Exit Sub
		End If
    'Number?
    If HSerInByte >= 48 and HSerInByte <= 57 Then
    		HSerIndex++
        HSerDataIn(HSerIndex) = HSerInByte
    End If
	Loop
End Sub


'Added  11/4/2015 by mlo
' A string is input to a USART as a series of ASCII chars with a CR at the end
'Output Value is a string
'Input value is entered as digits,letters and most punctuation
sub HSerGetString (Out HSerString As String, optional In comport = 1)
	HSerIndex = 0
  Do
  	comport = comport 'not really required but added for clarity
    HSerReceive ( HSerInByte )
    'Enter key?
    If HSerInByte = 13 Then
    	 Exit Sub
		End If
    'letters,numbers,punctuation
    If HSerInByte >= 32 and HSerInByte <= 126 Then
    		HSerIndex++
        HSerString(HSerIndex) = HSerInByte
        HSerString(0) = HSerIndex
    End If
	Loop
End Sub


sub HSerPrint (In PrintData As String, optional In comport = 1)
	'PrintLen = LEN(PrintData$)
	PrintLen = PrintData(0)

	If PrintLen <> 0 then
		'Write Data
		for SysPrintTemp = 1 to PrintLen
			HSerSend(PrintData(SysPrintTemp),comport )
			Wait USART_DELAY
		next
	End If

	'CR
	#IFDEF SerPrintCR
		HSerSend(13,comport)
		Wait USART_DELAY
	#ENDIF
	#IFDEF SerPrintLF
		HSerSend(10,comport)
		Wait USART_DELAY
	#ENDIF
end sub

sub HSerPrint (In SerPrintVal, optional In comport = 1)

	OutValueTemp = 0

	IF SerPrintVal >= 100 Then
		OutValueTemp = SerPrintVal / 100
		SerPrintVal = SysCalcTempX
		HSerSend(OutValueTemp + 48 ,comport )
		Wait USART_DELAY
	End If
	If OutValueTemp > 0 Or SerPrintVal >= 10 Then
		OutValueTemp = SerPrintVal / 10
		SerPrintVal = SysCalcTempX
		HSerSend(OutValueTemp + 48 ,comport )
		Wait USART_DELAY
	End If
	HSerSend(SerPrintVal + 48 ,comport)
	Wait USART_DELAY

	'CR
	#IFDEF SerPrintCR
		HSerSend(13,comport)
		Wait USART_DELAY
	#ENDIF
	#IFDEF SerPrintLF
		HSerSend(10,comport)
		Wait USART_DELAY
	#ENDIF

end sub

Sub HSerPrint (In SerPrintVal As Word, optional In comport = 1)
	Dim SysCalcTempX As Word

	OutValueTemp = 0

	If SerPrintVal >= 10000 then
		OutValueTemp = SerPrintVal / 10000 [word]
		SerPrintVal = SysCalcTempX
		HSerSend(OutValueTemp + 48 ,comport )
		Wait USART_DELAY
		Goto HSerPrintWord1000
	End If

	If SerPrintVal >= 1000 then
	HSerPrintWord1000:
		OutValueTemp = SerPrintVal / 1000 [word]
		SerPrintVal = SysCalcTempX
		HSerSend(OutValueTemp + 48 ,comport  )
		Wait USART_DELAY
		Goto HSerPrintWord100
	End If

	If SerPrintVal >= 100 then
	HSerPrintWord100:
		OutValueTemp = SerPrintVal / 100 [word]
		SerPrintVal = SysCalcTempX
		HSerSend(OutValueTemp + 48 ,comport )
		Wait USART_DELAY
		Goto HSerPrintWord10
	End If

	If SerPrintVal >= 10 then
	HSerPrintWord10:
		OutValueTemp = SerPrintVal / 10 [word]
		SerPrintVal = SysCalcTempX
		HSerSend(OutValueTemp + 48 ,comport )
		Wait USART_DELAY
	End If

	HSerSend(SerPrintVal + 48 ,comport  )
	Wait USART_DELAY

	'CR
	#IFDEF SerPrintCR
		HSerSend(13,comport)
		Wait USART_DELAY
	#ENDIF
	#IFDEF SerPrintLF
		HSerSend(10,comport)
		Wait USART_DELAY
	#ENDIF

End Sub

Sub HSerPrint (In SerPrintValInt As Integer, optional In comport = 1)
	Dim SerPrintVal As Word

	'If sign bit is on, print - sign and then negate
	If SerPrintValInt.15 = On Then
		HSerSend("-",comport )
		Wait USART_DELAY
		SerPrintVal = -SerPrintValInt

	'Sign bit off, so just copy value
	Else
		SerPrintVal = SerPrintValInt
	End If

	'Use Print(word) to display value
	HSerPrint SerPrintVal,comport
End Sub

Sub HSerPrint (In SerPrintVal As Long, optional In comport = 1)
              ' Updated 16th Feb 2014.  Incorrect variable defined
              ' Changed to SysCalcTempA

	Dim SysCalcTempA As Long
	Dim SysPrintBuffer(10)
	SysPrintBuffLen = 0

	Do
		'Divide number by 10, remainder into buffer
		SysPrintBuffLen += 1
		SysPrintBuffer(SysPrintBuffLen) = SerPrintVal % 10
		SerPrintVal = SysCalcTempA
	Loop While SerPrintVal <> 0

	'Display
	For SysPrintTemp = SysPrintBuffLen To 1 Step -1
		HSerSend(SysPrintBuffer(SysPrintTemp) + 48, comport  )
		Wait USART_DELAY
	Next

	'CR
	#IFDEF SerPrintCR
		HSerSend(13 )
		Wait USART_DELAY
	#ENDIF
	#IFDEF SerPrintLF
		HSerSend(10 )
		Wait USART_DELAY
	#ENDIF

End Sub

