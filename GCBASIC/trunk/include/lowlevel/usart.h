'    USART routines for Great Cow BASIC
'    Copyright (C) 2009 - 2013, 2105 Hugh Considine and William Roth


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
' 16/2/2104: Fixed HERPRINT Long Bug
' 28/6/2014: Changed HSerPrintCRLF to have a parameter... you can have lots of CRLF's
' 04/07/15:  Improved timing
' 31/7/2105: Fix Compile Error for 16F1705 1709  and other Pics - WMR
'            See lines 311 - 320
' 02/10/2015: Fix for missing SPBRG - mapped to SPBRGL

'For compatibility with USART routines in Contributors forum, add this line:
'#define USART_BLOCKING
'This will cause send and receive to wait until data can be sent or received
'To make only the send routines blocking, add this line:
'#define USART_TX_BLOCKING

'Note that the HSerReceive routine is implemented differently. In this
'library, it is a subroutine, as with SerReceive in rs232.h. This is purely
'a design decision, and has been made to keep included routines consistent
'with each other.

'To slow down print, set this delay:
'(Setting to 0 ms will remove all delays)
#define USART_DELAY 12 ms

'Some wrappers for compatibility with Contributors USART routines
#define HserPrintByte HSerPrint
Sub HserPrintByteCRLF(In PrintValue)
	HSerPrint(PrintValue)
	HSerSend(13)
  HSerSend(10)
End Sub

Sub HserPrintCRLF  ( Optional in HSerPrintCRLFCount = 1 )
    repeat HSerPrintCRLFCount
			HSerSend(13)
			Wait USART_DELAY
			HSerSend(10)
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
	End If
	If AVR Then
		If Bit(RXC0) Then
			USARTHasData = "RXC0 = On"
		End If
		If NoBit(RXC0) Then
			USARTHasData = "RXC = On"
		End If
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
	End If
#endscript

#startup InitUSART

Sub InitUSART
	#ifdef PIC
            'added 02.10.2015 to resolve PIC devices without SPBRG define.  SPBRG needs to map to SPBRGL
            'fixes the list of known devices as follows
'                16f1454.dat
'                16f1455.dat
'                16f1458.dat
'                16f1459.dat
'                16f1508.dat
'                16f1509.dat
            #ifndef SPBRG
                    #define SPBRG SPBRGL
            #endif

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

	#ifdef AVR

		'Set baud rate
		#ifndef Bit(U2X0)
			#ifdef Bit(U2X1)
				U2X1 = U2X0_TEMP
				UBRR1L = UBRRL_TEMP
				UBRR1H = UBRRH_TEMP
			#endif
			#ifndef Bit(U2X1)
				U2X = U2X0_TEMP
				UBRRL = UBRRL_TEMP
				UBRRH = UBRRH_TEMP
			#endif
		#endif
		#ifdef Bit(U2X0)
			U2X0 = U2X0_TEMP
			UBRR0L = UBRRL_TEMP
			UBRR0H = UBRRH_TEMP
		#endif

		'Enable TX and RX
		#ifndef Bit(RXEN0)
			#ifdef Bit(RXEN1)
				RXEN1 = On
				TXEN1 = On
			#endif
			#ifndef Bit(RXEN1)
				RXEN = On
				TXEN = On
			#endif
		#endif
		#ifdef Bit(RXEN0)
			RXEN0 = On
			TXEN0 = On
		#endif

	#endif
End Sub

sub HSerSend(In SerData)
	'Block before sending (if needed)
	HSerSendBlocker

	'Send byte
	#ifdef PIC
		#ifndef Var(TXREG1)
			TXREG = SerData
		#endif
		#ifdef Var(TXREG1)
			TXREG1 = SerData
		#endif
	#endif
	#ifdef AVR
		#ifndef Var(UDR0)
			#ifdef Var(UDR1)
				UDR1 = SerData
			#endif
			#ifndef Var(UDR1)
				UDR = SerData
			#endif
		#endif
		#ifdef Var(UDR0)
			UDR0 = SerData
		#endif

	#endif
end sub

Function HSerReceive
	HSerReceive(SerData)
	HSerReceive = SerData
End Function

Sub HSerReceive(Out SerData)
	'If set up to block, wait for data
	#ifdef USART_BLOCKING
		Wait Until USARTHasData
	#endif

	#ifdef PIC
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

			#ifdef bit(OEER1)
			'Clear error
				If OERR1 Then
					Set CREN1 Off
					Set CREN1 On
				End If
			#endif
			#Ifndef bit(OEER1) '  For Chips with RCREG1 but no OEER1
				#IFDEF Bit(OEER)
					IF OEER then
						Set CREN off
						Set CREN On
					END IF
                #ENDIF
			#ENDIF
		#endif
	#endif


	#ifdef AVR
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
	#endif
End Sub

sub HSerPrint (In PrintData As String)
	'PrintLen = LEN(PrintData$)
	PrintLen = PrintData(0)

	If PrintLen <> 0 then
		'Write Data
		for SysPrintTemp = 1 to PrintLen
			HSerSend(PrintData(SysPrintTemp))
			Wait USART_DELAY
		next
	End If

	'CR
	#IFDEF SerPrintCR
		HSerSend(13)
		Wait USART_DELAY
	#ENDIF
	#IFDEF SerPrintLF
		HSerSend(10)
		Wait USART_DELAY
	#ENDIF
end sub

sub HSerPrint (In SerPrintVal)

	OutValueTemp = 0

	IF SerPrintVal >= 100 Then
		OutValueTemp = SerPrintVal / 100
		SerPrintVal = SysCalcTempX
		HSerSend(OutValueTemp + 48)
		Wait USART_DELAY
	End If
	If OutValueTemp > 0 Or SerPrintVal >= 10 Then
		OutValueTemp = SerPrintVal / 10
		SerPrintVal = SysCalcTempX
		HSerSend(OutValueTemp + 48)
		Wait USART_DELAY
	End If
	HSerSend(SerPrintVal + 48)
	Wait USART_DELAY

	'CR
	#IFDEF SerPrintCR
		HSerSend(13)
		Wait USART_DELAY
	#ENDIF
	#IFDEF SerPrintLF
		HSerSend(10)
		Wait USART_DELAY
	#ENDIF

end sub

Sub HSerPrint (In SerPrintVal As Word)
	Dim SysCalcTempX As Word

	OutValueTemp = 0

	If SerPrintVal >= 10000 then
		OutValueTemp = SerPrintVal / 10000 [word]
		SerPrintVal = SysCalcTempX
		HSerSend(OutValueTemp + 48)
		Wait USART_DELAY
		Goto HSerPrintWord1000
	End If

	If SerPrintVal >= 1000 then
	HSerPrintWord1000:
		OutValueTemp = SerPrintVal / 1000 [word]
		SerPrintVal = SysCalcTempX
		HSerSend(OutValueTemp + 48)
		Wait USART_DELAY
		Goto HSerPrintWord100
	End If

	If SerPrintVal >= 100 then
	HSerPrintWord100:
		OutValueTemp = SerPrintVal / 100 [word]
		SerPrintVal = SysCalcTempX
		HSerSend(OutValueTemp + 48)
		Wait USART_DELAY
		Goto HSerPrintWord10
	End If

	If SerPrintVal >= 10 then
	HSerPrintWord10:
		OutValueTemp = SerPrintVal / 10 [word]
		SerPrintVal = SysCalcTempX
		HSerSend(OutValueTemp + 48)
		Wait USART_DELAY
	End If

	HSerSend(SerPrintVal + 48)
	Wait USART_DELAY

	'CR
	#IFDEF SerPrintCR
		HSerSend(13)
		Wait USART_DELAY
	#ENDIF
	#IFDEF SerPrintLF
		HSerSend(10)
		Wait USART_DELAY
	#ENDIF

End Sub

Sub HSerPrint (In SerPrintValInt As Integer)
	Dim SerPrintVal As Word

	'If sign bit is on, print - sign and then negate
	If SerPrintValInt.15 = On Then
		HSerSend("-")
		Wait USART_DELAY
		SerPrintVal = -SerPrintValInt

	'Sign bit off, so just copy value
	Else
		SerPrintVal = SerPrintValInt
	End If

	'Use Print(word) to display value
	HSerPrint SerPrintVal
End Sub

Sub HSerPrint (In SerPrintVal As Long)
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
		HSerSend(SysPrintBuffer(SysPrintTemp) + 48)
		Wait USART_DELAY
	Next

	'CR
	#IFDEF SerPrintCR
		HSerSend(13)
		Wait USART_DELAY
	#ENDIF
	#IFDEF SerPrintLF
		HSerSend(10)
		Wait USART_DELAY
	#ENDIF

End Sub

Macro HSerSendBlocker
	#ifdef USART_BLOCKING
		HSerSendBlockCheck
	#endif
	#ifndef USART_BLOCKING
		#ifdef USART_TX_BLOCKING
			HSerSendBlockCheck
		#endif
	#endif
End Macro

Macro HSerSendBlockCheck
	'Delay until send buffer empty
	#ifdef PIC
		#ifndef Bit(TX1IF)
			Wait While TXIF = Off
		#endif
		#ifdef Bit(TX1IF)
			Wait While TX1IF = Off
		#endif
	#endif
	#ifdef AVR
		#ifndef Bit(UDRE0)
			#ifdef Bit(UDRE1)
				Wait While UDRE1 = Off
			#endif
			#ifndef Bit(UDRE1)
				Wait While UDRE = Off
			#endif
		#endif
		#ifdef Bit(UDRE0)
			Wait While UDRE0 = Off
		#endif
	#endif
End Macro
