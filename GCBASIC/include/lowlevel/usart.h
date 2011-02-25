'    USART routines for Great Cow BASIC
'    Copyright (C) 2009 - 2010 Hugh Considine

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

'For compatibility with USART routines in Contributors forum, add this line:
'#define USART_BLOCKING
'This will cause send and receive to wait until data can be sent or received

'Note that the HSerReceive routine is implemented differently. In this
'library, it is a subroutine, as with SerReceive in rs232.h. This is purely
'a design decision, and has been made to keep included routines consistent
'with each other.

'To slow down print, set this delay:
#define USART_DELAY 1 ms

'Some wrappers for compatibility with Contributors USART routines
#define HserPrintByte HSerPrint
Sub HserPrintByteCRLF(In PrintValue) 
	HSerPrint(PrintValue)
	HSerPrintCRLF
End Sub
Sub HserPrintCRLF 
	HSerSend(13)
	Wait USART_DELAY
	HSerSend(10)
End Sub 

'Script to calculate baud rate generator values
'Also sets constants to check if byte received
#script
	If PIC Then
		USARTHasData = "RCIF = On"
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
		USARTHasData = "RXC = On"
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
	
	#ifdef AVR
		
		'Set baud rate
		U2X = U2X0_TEMP
		UBRRL = UBRRL_TEMP
		UBRRH = UBRRH_TEMP
		
		'Enable TX and RX
		RXEN = On
		TXEN = On
		
	#endif
End Sub

sub HSerSend(In SerData)
	#ifdef PIC
		#ifdef USART_BLOCKING
			Wait While TXIF = Off
		#endif
		TXREG = SerData
	#endif
	#ifdef AVR
		#ifdef USART_BLOCKING
			Wait While UDRE = Off
		#endif
		UDR = SerData
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
	#ifdef AVR
		If USARTHasData Then
			SerData = UDR
		End If
	#endif
End Sub

sub HSerPrint (PrintData As String)
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
