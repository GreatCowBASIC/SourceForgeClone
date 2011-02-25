'    System routines for Great Cow BASIC
'    Copyright (C) 2006 - 2010 Hugh Considine

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

'Constants
#define ON 1
#define OFF 0
#define TRUE 255
#define FALSE 0

'Names for symbols
#define AND &
#define OR |
#define XOR #
#define NOT !
#define MOD %

'Options
#define CheckDivZero TRUE

'********************************************************************************
'System initialisation routine
Sub InitSys
	
	'Set up internal oscillator
	#IFDEF Var(OSCCON)
		#IFDEF Bit(FOSC4)
			Set FOSC4 Off
		#ENDIF
		#ifndef Bit(SPLLEN)
			#IFDEF ChipMHz 8
				OSCCON = OSCCON OR b'01110000'
			#ENDIF
			#IFDEF ChipMHz 4
				OSCCON = OSCCON AND b'10001111'
				OSCCON = OSCCON OR b'01100000'
			#ENDIF
			#IFDEF ChipMHz 2
				OSCCON = OSCCON AND b'10001111'
				OSCCON = OSCCON OR b'01010000'
			#ENDIF
			#IFDEF ChipMHz 1
				OSCCON = OSCCON AND b'10001111'
				OSCCON = OSCCON OR b'01000000'
			#ENDIF
			#IFDEF ChipMHz .5
				OSCCON = OSCCON AND b'10001111'
				OSCCON = OSCCON OR b'00110000'
			#ENDIF
			#IFDEF ChipMHz .25
				OSCCON = OSCCON AND b'10001111'
				OSCCON = OSCCON OR b'00100000'
			#ENDIF
			#IFDEF ChipMHz .125
				OSCCON = OSCCON AND b'10001111'
				OSCCON = OSCCON OR b'00010000'
			#ENDIF
		#endif
		
		#ifdef Bit(SPLLEN)
			#IFDEF ChipMHz 64
				'Same as for 16, assuming 64 MHz clock is 16 MHz x 4
				OSCCON = OSCCON OR b'01111000'
			#ENDIF
			#IFDEF ChipMHz 32
				'Same as for 8, assuming 32 MHz clock is 8 MHz x 4
				OSCCON = OSCCON AND b'10000111'
				OSCCON = OSCCON OR b'11110000'
			#ENDIF
			#IFDEF ChipMHz 16
				OSCCON = OSCCON OR b'01111000'
			#ENDIF
			#IFDEF ChipMHz 8
				OSCCON = OSCCON AND b'10000111'
				OSCCON = OSCCON OR b'01110000'
			#ENDIF
			#IFDEF ChipMHz 4
				OSCCON = OSCCON AND b'10000111'
				OSCCON = OSCCON OR b'01101000'
			#ENDIF
			#IFDEF ChipMHz 2
				OSCCON = OSCCON AND b'10000111'
				OSCCON = OSCCON OR b'01100000'
			#ENDIF
			#IFDEF ChipMHz 1
				OSCCON = OSCCON AND b'10000111'
				OSCCON = OSCCON OR b'01011000'
			#ENDIF
			#IFDEF ChipMHz .5
				OSCCON = OSCCON AND b'10000111'
				OSCCON = OSCCON OR b'00111000'
			#ENDIF
			#IFDEF ChipMHz .25
				OSCCON = OSCCON AND b'10000111'
				OSCCON = OSCCON OR b'00110000'
			#ENDIF
			#IFDEF ChipMHz .125
				OSCCON = OSCCON AND b'10000111'
				OSCCON = OSCCON OR b'00101000'
			#ENDIF
		#endif
		
	#ENDIF
	
	'Clear BSR on 18F chips
	#IFDEF ChipFamily 16
		BSR = 0
	#ENDIF
	
	#IFDEF Var(TBLPTRU)
		TBLPTRU = 0
	#ENDIF
	
	'Ensure all ports are set for digital I/O
	'Turn off A/D
	#IFDEF Var(ADCON0)
		SET ADCON0.ADON OFF
		#IFDEF Bit(ADFM)
			SET ADFM OFF
		#ENDIF
		'Switch off A/D
		#IFDEF Var(ADCON0)
			#IF NoVar(ANSEL) AND NoVar(ANSELA) AND NoVar(ANSEL0)
				#IFDEF NoBit(PCFG4)
					#IFDEF NoVar(ADCON2)
						#IFDEF NoBit(ANS0)
							#IFDEF Bit(PCFG3)
								SET PCFG3 OFF
							#ENDIF
							#IFDEF Bit(PCFG2)
								SET PCFG2 ON
							#ENDIF
							SET PCFG1 ON
							SET PCFG0 OFF
						#ENDIF
						#IFDEF Bit(ANS0)
							SET ANS0 OFF
							SET ANS1 OFF
						#ENDIF
					#ENDIF
					
					#IFDEF Var(ADCON2)
						#IFDEF BIT(PCFG3)
							SET PCFG3 ON
							SET PCFG2 ON
							SET PCFG1 ON
							SET PCFG0 ON
						#ENDIF
					#ENDIF
				#ENDIF
				
				'For 18F1320, which uses ADCON1 as an ANSEL register
				#IFDEF Bit(PCFG4)
					ADCON1 = 0
				#ENDIF
			#ENDIF
		#ENDIF
		
		'Clear whatever ANSEL variants the chip has
		#IFDEF Var(ANSEL)
			ANSEL = 0
		#ENDIF
		#IFDEF Var(ANSELH)
			ANSELH = 0
		#ENDIF
		#IFDEF Var(ANSEL0)
			ANSEL0 = 0
		#ENDIF
		#IFDEF Var(ANSEL1)
			ANSEL1 = 0
		#ENDIF
		#IFDEF Var(ANSELA)
			ANSELA = 0
		#ENDIF
		#IFDEF Var(ANSELB)
			ANSELB = 0
		#ENDIF
	#ENDIF
	
	'Turn off comparator
	#IFDEF Var(CMCON)
		CMCON = 7
	#ENDIF
	#IFDEF Var(CMCON0)
		CMCON0 = 7
	#ENDIF
	'12F510,16F506 and other devices? (Thanks to Kent for suggesting these lines!)
	#IFDEF Var(CM1CON0) 
		#IFDEF Var(CM2CON0) 
			C2ON = 0 
		#ENDIF 
		C1ON = 0 
	#ENDIF 
	
	'Set GPIO.2 to digital (clear T0CS bit)
	#IFDEF ChipFamily 12
		#IFDEF Bit(T0CS)
			movlw b'11001111'
			option
		#ENDIF
	#ENDIF
	
	'Turn off all ports
	#IFDEF Var(GPIO)
		GPIO = 0
	#ENDIF
	#IFDEF Var(PORTA)
		PORTA = 0
	#ENDIF
	#IFDEF Var(PORTB)
		PORTB = 0
	#ENDIF
	#IFDEF Var(PORTC)
		PORTC = 0
	#ENDIF
	#IFDEF Var(PORTD)
		PORTD = 0
	#ENDIF
	#IFDEF Var(PORTE)
		PORTE = 0
	#ENDIF
	#IFDEF Var(PORTF)
		PORTF = 0
	#ENDIF
	#IFDEF Var(PORTG)
		PORTG = 0
	#ENDIF
	#IFDEF Var(PORTH)
		PORTH = 0
	#ENDIF
	#IFDEF Var(PORTI)
		PORTI = 0
	#ENDIF
	#IFDEF Var(PORTJ)
		PORTJ = 0
	#ENDIF
	
End Sub

'********************************************************************************
'String setting subroutines

'String parameter vars:
'On 12/14 bit:
'	SysStringA = Source string
'	SysStringB = Dest string
'On extended 14 and 16 bit:
'	FSR0 = Source string
'	FSR1 = Dest string

sub SysCopyString

#IFDEF PIC
 Dim SysCalcTempA As Byte
#ENDIF

#IFDEF ChipFamily 12,14

 Dim SysCalcTempB As Byte
 Dim SysStringLength As Byte

 'Get length
 movf SysStringA, W
 movwf FSR
 #IFDEF Bit(IRP)
  bcf STATUS, IRP
  btfsc SysStringA_H, 0
  bsf STATUS, IRP
 #ENDIF
 movf INDF, W
 movwf SysCalcTempA

 'Set length
 movf SysStringB, W
 movwf FSR
 #IFDEF Bit(IRP)
  bcf STATUS, IRP
  btfsc SysStringB_H, 0
  bsf STATUS, IRP
 #ENDIF
 movf SysCalcTempA, W
 movwf INDF
 
 goto SysCopyStringCheck

'When appending, add length to counter 
SysCopyStringPart:
 movf SysStringA, W
 movwf FSR
 #IFDEF Bit(IRP)
  bcf STATUS, IRP
  btfsc SysStringA_H, 0
  bsf STATUS, IRP
 #ENDIF
 movf INDF, W
 addwf SysStringLength, F
 movwf SysCalcTempA

 'if source length = 0, exit
SysCopyStringCheck:
 movf SysCalcTempA,W
 btfsc STATUS,Z
 return
 
 'Copy char-by-char
SysStringCopy:

  'Increment pointers
  incf SysStringA, F
  incf SysStringB, F
  'Strings cannot span banks, so no need to increment pointer high byte

  'Get char
  movf SysStringA, W
  movwf FSR
  #IFDEF Bit(IRP)
   bcf STATUS, IRP
   btfsc SysStringA_H, 0
   bsf STATUS, IRP
  #ENDIF
  movf INDF, W
  movwf SysCalcTempB

  'Set char
  movf SysStringB, W
  movwf FSR
  #IFDEF Bit(IRP)
   bcf STATUS, IRP
   btfsc SysStringB_H, 0
   bsf STATUS, IRP
  #ENDIF
  movf SysCalcTempB, W
  movwf INDF

 decfsz SysCalcTempA, F
 goto SysStringCopy

#ENDIF

	#ifdef ChipFamily 15
		'Get and copy length
		movf INDF0, W
		movwf SysCalcTempA
		movwf INDF1
		
		goto SysCopyStringCheck
		
		'When appending, add length to counter
		SysCopyStringPart:
		movf INDF0, W
		movwf SysCalcTempA
		addwf SysStringLength, F
		
		SysCopyStringCheck:
		'Exit if length = 0
		movf SysCalcTempA,F
		btfsc STATUS,Z
		return
		
		SysStringCopy:
			'Increment pointers
			addfsr 0, 1
			addfsr 1, 1
			
			'Copy character
			movf INDF0, W
			movwf INDF1
		
		decfsz SysCalcTempA, F
		goto SysStringCopy
	#endif

#IFDEF ChipFamily 16

 'Get and copy length
 movff INDF0, SysCalcTempA
 movff SysCalcTempA, INDF1
 
 goto SysCopyStringCheck
 
'When appending, add length to counter 
SysCopyStringPart:
 movf INDF0, W
 movwf SysCalcTempA
 addwf SysStringLength, F

 SysCopyStringCheck:
 'Exit if length = 0
 movf SysCalcTempA,F
 btfsc STATUS,Z
 return

 SysStringCopy:
 'Copy character
 movff PREINC0, PREINC1

 decfsz SysCalcTempA, F
 goto SysStringCopy

#ENDIF

#IFDEF AVR

 'SysStringA (X) stores source
 'SysStringB (Y) stores destination
 'SysStringLength is counter, keeps track of size of destination string
 'SysCalcTempA is loop counter
 
 'Get and copy length
 ld SysCalcTempA, X+
 st Y+, SysCalcTempA
 
 rjmp SysCopyStringCheck
 
'When appending, add length to counter 
SysCopyStringPart:
 ld SysCalcTempA, X+
 add SysStringLength, SysCalcTempA
 
 SysCopyStringCheck:
 'Exit if length = 0
 cpi SysCalcTempA,0
 brne SysStringCopy
 ret

 SysStringCopy:
 'Copy character
 ld SysReadA, X+
 st Y+, SysReadA
 
 dec SysCalcTempA
 brne SysStringCopy
#ENDIF

end sub

'Program Memory > String
'On 12/14 bit:
'	SysStringA = Source address
'	SysStringB = Dest string
'On enhanced 14 bit:
'	SysStringA = Source address
'	FSR1 = Dest string
'On 16 bit:
'	TBLPTRL/TBLPTRH = Source string
'	FSR1 = Dest string

sub SysReadString

 #IFDEF PIC
  Dim SysCalcTempA As Byte
  Dim SysStringLength As Byte
 #ENDIF

 #ifdef ChipFamily 12,14

  'Set pointer
  movf SysStringB, W
  movwf FSR
  #IFDEF Bit(IRP)
   bcf STATUS, IRP
   btfsc SysStringB_H, 0
   bsf STATUS, IRP
  #ENDIF

  'Get length
  call SysStringTables
  movwf SysCalcTempA
  movwf INDF
  addwf SysStringB, F
  
  goto SysStringReadCheck

SysReadStringPart:

  'Set pointer
  movf SysStringB, W
  movwf FSR
'  decf FSR,F
  #IFDEF Bit(IRP)
   bcf STATUS, IRP
   btfsc SysStringB_H, 0
   bsf STATUS, IRP
  #ENDIF

  'Get length
  call SysStringTables
  movwf SysCalcTempA
  addwf SysStringLength,F
  addwf SysStringB,F
  
  'Check length
SysStringReadCheck:
  'If length is 0, exit
  movf SysCalcTempA,F
  btfsc STATUS,Z
  return
  
  'Copy
SysStringRead:
  
   'Get char
   call SysStringTables
   
   'Set char
   incf FSR, F
   movwf INDF

  decfsz SysCalcTempA, F
  goto SysStringRead

 #endif
	
	#ifdef ChipFamily 15
		
		'Get length
		call SysStringTables
		movwf SysCalcTempA
		movwf INDF1
		addfsr 1,1
		
		goto SysStringReadCheck
		SysReadStringPart:
		
		'Get length
		call SysStringTables
		movwf SysCalcTempA
		addwf SysStringLength,F
		
		'Check length
		SysStringReadCheck:
		'If length is 0, exit
		movf SysCalcTempA,F
		btfsc STATUS,Z
		return
		
		'Copy
		SysStringRead:
			'Get char
			call SysStringTables
			
			'Set char
			movwf INDF1
			addfsr 1,1
			
		decfsz SysCalcTempA, F
		goto SysStringRead
		
	#endif
 
 #ifdef ChipFamily 16

  'Get length
  TBLRD*+
  movff TABLAT,SysCalcTempA
  movff TABLAT,INDF1
  TBLRD*+
  goto SysStringReadCheck
  
SysReadStringPart:
  TBLRD*+
  movf TABLAT, W
  movwf SysCalcTempA
  addwf SysStringLength,F
  TBLRD*+

  'Check length
SysStringReadCheck:
  'If length is 0, exit
  movf SysCalcTempA,F
  btfsc STATUS,Z
  return
  
  'Copy
SysStringRead:
  
   'Copy char
   TBLRD*+
   movff TABLAT,PREINC1

  decfsz SysCalcTempA, F
  goto SysStringRead

 #endif
 
 #IFDEF AVR
  
  'Get length
  'lpm SysCalcTempA, Z+
  lpm
  mov SysCalcTempA, SysCalcTempX
  SysReadA += 1
  st Y+, SysCalcTempA
  rjmp SysStringReadCheck
  
SysReadStringPart:
  'lpm SysCalcTempA, Z+
  lpm
  mov SysCalcTempA, SysCalcTempX
  SysReadA += 1
  add SysStringLength, SysCalcTempA

  'Check length
SysStringReadCheck:
  'If length is 0, exit
  cpi SysCalcTempA, 0
  brne SysStringRead
  ret
  
  'Copy
SysStringRead:
  
  'Copy char
  'lpm SysCalcTempX, Z+
  lpm
  SysReadA += 1
  st Y+, SysCalcTempX

  dec SysCalcTempA
  brne SysStringRead
  
 #ENDIF
 
end sub

'********************************************************************************
'String comparison subroutines
'SysStringA/FSR0 = String 1
'SysStringB/FSR1 = String 2

sub SysCompEqualString

 Dim SysCalcTempA As Byte
 Dim SysCalcTempX As Byte

 SysCalcTempX = 0

#IFDEF ChipFamily 12,14

 Dim SysCalcTempB As Byte

 'Get and check length
 'Get length A
 movf SysStringA, W
 movwf FSR
 #IFDEF Bit(IRP)
  bcf STATUS, IRP
  btfsc SysStringA_H, 0
  bsf STATUS, IRP
 #ENDIF
 movf INDF, W
 movwf SysCalcTempA

 'Get length B
 movf SysStringB, W
 movwf FSR
 #IFDEF Bit(IRP)
  bcf STATUS, IRP
  btfsc SysStringB_H, 0
  bsf STATUS, IRP
 #ENDIF

 'Exit if length <>
 movf INDF, W
 subwf SysCalcTempA, W
 btfss STATUS, Z
 return

 'Check char-by-char
SysStringComp:

  'Increment pointers
  incf SysStringA, F
  incf SysStringB, F
  'Strings cannot span banks, so no need to increment pointer high byte

  'Get char A
  movf SysStringA, W
  movwf FSR
  #IFDEF Bit(IRP)
   bcf STATUS, IRP
   btfsc SysStringA_H, 0
   bsf STATUS, IRP
  #ENDIF
  movf INDF, W
  movwf SysCalcTempB

  'Get char B
  movf SysStringB, W
  movwf FSR
  #IFDEF Bit(IRP)
   bcf STATUS, IRP
   btfsc SysStringB_H, 0
   bsf STATUS, IRP
  #ENDIF

  'Exit if char <>
  movf INDF, W
  subwf SysCalcTempB, W
  btfss STATUS, Z
  return

 decfsz SysCalcTempA, F
 goto SysStringComp

 movlw TRUE
 movwf SysCalcTempX

#ENDIF
	
	#ifdef ChipFamily 15
		'Check length matches
		movf INDF0, W
		movwf SysCalcTempA
		subwf INDF1, W
		btfss STATUS, Z
		return
		
		'Check each char, exit if not equal
		SysStringComp:
			
			'Move to next char
			addfsr 0, 1
			addfsr 1, 1
			
			'Compare, exit if <>
			movf INDF0, W
			subwf INDF1, W
			btfss STATUS, Z
			return
		
		decfsz SysCalcTempA, F
		goto SysStringComp
		
		comf SysCalcTempX, F
	#endif
	
#IFDEF ChipFamily 16

 'Check length matches
 movf INDF0, W
 cpfseq POSTINC1
 return

 'Check each char, exit if not equal
 movff POSTINC0, SysCalcTempA
SysStringComp:

 'Compare, exit if <>
 movf POSTINC0, W
 cpfseq POSTINC1
 return

 decfsz SysCalcTempA, F
 goto SysStringComp

 setf SysCalcTempX
#ENDIF

#IFDEF AVR

 clr SysCalcTempX

 'Check length matches
 ld SysReadA, X+
 ld SysReadA_H, Y+
 cpse SysReadA, SysReadA_H
 ret
 mov SysCalcTempA, SysReadA

 'Check each char, exit if not equal
 SysStringComp:

 'Compare, exit if <>
 ld SysReadA, X+
 ld SysReadA_H, Y+
 cpse SysReadA, SysReadA_H
 ret
 
 dec SysCalcTempA
 brne SysStringComp
 
 com SysCalcTempX
#ENDIF

end sub

'********************************************************************************
'Value conversion subroutines
'Note: String/int routines go in string.h

'All conversion subroutines use:
' - SysValTemp
' - SysSingleTemp

sub SysIntToString

end sub

sub SysSingleToString

end sub

sub SysStringToVal

end sub

sub SysStringToInt

end sub

sub SysStringToSingle

end sub

sub SysValToSingle

end sub

sub SysSingleToVal

end sub

sub SysIntToSingle

end sub

sub SysSingleToInt

end sub

'********************************************************************************
'Multiply subroutines

'8 bit
sub SysMultSub
	dim SysCalcTempA as byte
	dim SysCalcTempB as byte
	dim SysCalcTempX as byte
	
	#IFDEF PIC
		#IFDEF ChipFamily 12, 14, 15
			clrf SysCalcTempX
		MUL8LOOP:
			movf SysCalcTempA, W
			btfsc SysCalcTempB, 0
			addwf SysCalcTempX, F
			bcf STATUS, C
			rrf SysCalcTempB, F
			bcf STATUS, C
			rlf SysCalcTempA, F
			movf SysCalcTempB, F
			btfss STATUS, Z
			goto MUL8LOOP
		#ENDIF
		
		#IFDEF ChipFamily 16
			movf SysCalcTempA, W
			mulwf SysCalcTempB
			movff PRODL,SysCalcTempX
		#ENDIF
	#ENDIF
	
	#IFDEF AVR
		#IFNDEF HardwareMult
			clr SysCalcTempX
		MUL8LOOP:
			sbrc SysCalcTempB,0
			add SysCalcTempX,SysCalcTempA
			lsr SysCalcTempB
			lsl SysCalcTempA
			tst SysCalcTempB
			brne MUL8LOOP
		#ENDIF
		#IFDEF HardwareMult
			mul SysCalcTempA,SysCalcTempB
		#ENDIF
	#ENDIF

end sub

'16 bit
sub SysMultSub16
	
	dim SysCalcTempA as word
	dim SysCalcTempB as word
	dim SysCalcTempX as word
	
	#IFDEF PIC
		#IFDEF ChipFamily 12, 14, 15
			dim SysDivMultA as word
			dim SysDivMultB as word
			dim SysDivMultX as word
			
			SysDivMultA = SysCalcTempA
			SysDivMultB = SysCalcTempB
			SysDivMultX = 0
			
			MUL16LOOP:
				IF SysDivMultB.0 ON then SysDivMultX += SysDivMultA
				set STATUS.C OFF
				rotate SysDivMultB right
				set STATUS.C off
				rotate SysDivMultA left
			if SysDivMultB > 0 then goto MUL16LOOP
			
			SysCalcTempX = SysDivMultX
		#ENDIF
		
		#IFDEF ChipFamily 16
			'X = LowA * LowB
			movf SysCalcTempA, W
			mulwf SysCalcTempB
			movff PRODL, SysCalcTempX
			movff PRODH, SysCalcTempX_H
			
			'HighX += LowA * HighB
			movf SysCalcTempA, W
			mulwf SysCalcTempB_H
			movf PRODL, W
			addwf SysCalcTempX_H, F
			
			'HighX += HighA * LowB
			movf SysCalcTempA_H, W
			mulwf SysCalcTempB
			movf PRODL, W
			addwf SysCalcTempX_H, F
			
			'PRODL = HighA * HighB
			movf SysCalcTempA_H, F
			mulwf SysCalcTempB_H
		#ENDIF
	#ENDIF
	
	#IFDEF AVR
		#IFNDEF HardwareMult
			dim SysDivMultA as word
			dim SysDivMultB as word
			dim SysDivMultX as word
			
			SysDivMultA = SysCalcTempA
			SysDivMultB = SysCalcTempB
			SysDivMultX = 0
			
			MUL16LOOP:
				IF SysDivMultB.0 ON then SysDivMultX += SysDivMultA
				Set C Off
				rotate SysDivMultB right
				Set C Off
				rotate SysDivMultA left
			if SysDivMultB > 0 then goto MUL16LOOP
			
			SysCalcTempX = SysDivMultX
		#ENDIF
		
		#IFDEF HardwareMult
			'Need to keep result in here because SysCalcTempX[_H] gets overwritten by mul
			dim SysDivMultX as word ' alias SysCalcTempX_U, SysCalcTempX_H
			
			'X = LowA * LowB
			mul SysCalcTempA, SysCalcTempB
			'movff PRODL, SysCalcTempX
			'movff PRODH, SysCalcTempX_H
			SysDivMultX = SysCalcTempX
			
			'HighX += LowA * HighB
			mul SysCalcTempA, SysCalcTempB_H
			add SysDivMultX_H, SysCalcTempX
			
			'HighX += HighA * LowB
			mul SysCalcTempA_H, SysCalcTempB
			add SysDivMultX_H, SysCalcTempX
			
			'Copy result back
			SysCalcTempX = SysDivMultX
		#ENDIF
	#ENDIF
	
end sub

sub SysMultSubInt
	
	Dim SysCalcTempA, SysCalcTempB, SysCalcTempX As Integer
	Dim SysSignByte As Byte
	
	'Make both inputs positive, decide output type
	SysSignByte = SysCalcTempA_H xor SysCalcTempB_H
	if SysCalcTempA.15 then SysCalcTempA = -SysCalcTempA
	if SysCalcTempB.15 then SysCalcTempB = -SysCalcTempB
	
	'Call word multiply routine
	SysMultSub16
	
	'Negate result if necessary
	if SysSignByte.7 then SysCalcTempX = -SysCalcTempX
	
end sub

sub SysMultSubSingle

end sub

'********************************************************************************
'Divide subroutines

'8 bit
sub SysDivSub
	
	#IFDEF PIC
		dim SysCalcTempA as byte
		dim SysCalcTempB as byte
		dim SysCalcTempX as byte
		
		#ifdef CheckDivZero TRUE
			'Check for div/0
			movf SysCalcTempB, F
			btfsc STATUS, Z
			return
		#endif
		
		'Main calc routine
		SysCalcTempX = 0
		SysDivLoop = 8
		SysDiv8Start:
			
			bcf STATUS, C
			rlf SysCalcTempA, F
			rlf SysCalcTempX, F
			movf SysCalcTempB, W
			subwf SysCalcTempX, F
			
			bsf SysCalcTempA, 0
			btfsc STATUS, C
			goto Div8NotNeg
			bcf SysCalcTempA, 0
			movf SysCalcTempB, W
			addwf SysCalcTempX, F
		Div8NotNeg:
		
		decfsz SysDivLoop, F
		goto SysDiv8Start
		
	#ENDIF
	
	#IFDEF AVR
		#ifdef CheckDivZero TRUE
			'Check for div/0
			tst SysCalcTempB
			brne DIV8Cont
			ret
			DIV8Cont:
		#endif
		
		'Main calc routine
		clr SysCalcTempX
		ldi SysDivLoop, 8
		SysDiv8Start:
			lsl SysCalcTempA
			rol SysCalcTempX
			asm sub SysCalcTempX,SysCalcTempB 'asm needed, or else sub will be used as start of sub
			
			sbr SysCalcTempA,1
			brsh Div8NotNeg
			cbr SysCalcTempA,1
			add SysCalcTempX,SysCalcTempB
		Div8NotNeg:
		
		dec SysDivLoop
		brne SysDiv8Start
	#ENDIF
	
end sub

'16 bit
sub SysDivSub16
	
	dim SysCalcTempA as word
	dim SysCalcTempB as word
	dim SysCalcTempX as word
	
	'#ifdef PIC
		dim SysDivMultA as word
		dim SysDivMultB as word
		dim SysDivMultX as word
	'#endif
	
	SysDivMultA = SysCalcTempA
	SysDivMultB = SysCalcTempB
	SysDivMultX = 0
	
	'Avoid division by zero
	if SysDivMultB = 0 then
		SysCalcTempA = 0
		exit sub
	end if
	
	'Main calc routine
	SysDivLoop = 16
	SysDiv16Start:
		
		set C off
		Rotate SysDivMultA Left
		Rotate SysDivMultX Left
		SysDivMultX = SysDivMultX - SysDivMultB
		Set SysDivMultA.0 On
		
		#IFDEF PIC
			If C Off Then
				Set SysDivMultA.0 Off
				SysDivMultX = SysDivMultX + SysDivMultB
			End If
			
			decfsz SysDivLoop, F
			goto SysDiv16Start
		#ENDIF
		#IFDEF AVR
			If C On Then
				Set SysDivMultA.0 Off
				SysDivMultX = SysDivMultX + SysDivMultB
			End If
			
			dec SysDivLoop
			brne SysDiv16Start
		#ENDIF
		
	SysCalcTempA = SysDivMultA
	SysCalcTempX = SysDivMultX
	
end sub

sub SysDivSubInt
	
	Dim SysCalcTempA, SysCalcTempB, SysCalcTempX As Integer
	Dim SysSignByte As Byte
	
	'Make both inputs positive, decide output type
	SysSignByte = SysCalcTempA_H xor SysCalcTempB_H
	If SysCalcTempA.15 Then SysCalcTempA = -SysCalcTempA
	If SysCalcTempB.15 Then SysCalcTempB = -SysCalcTempB
	
	'Call word divide routine
	SysDivSub16
	
	'Negate result if necessary
	If SysSignByte.7 Then
		SysCalcTempA = -SysCalcTempA
		SysCalcTempX = -SysCalcTempX
	End If
 
end sub

sub SysDivSubSingle

end sub

'********************************************************************************
'Misc calculations

'Use:
' - SysValTemp
' - SysSingleTemp
'Result in same var as input

'Negate
'Only needed for Single, Integer negation compiled inline
sub SysNegateSingle

end sub

'********************************************************************************
'Condition checking subs

'Equal
sub SysCompEqual
	#IFDEF ChipFamily 12,14,15
		clrf SysCalcTempX
		movf SysCalcTempA, W
		subwf SysCalcTempB, W
		btfsc STATUS, Z
		comf SysCalcTempX,F
	#ENDIF

	#IFDEF ChipFamily 16
		setf SysCalcTempX
		movf SysCalcTempB, W
		cpfseq SysCalcTempA
		clrf SysCalcTempX
	#ENDIF
	
	#IFDEF AVR
		clr SysCalcTempX
		cpse SysCalcTempA, SysCalcTempB
		com SysCalcTempX
	#ENDIF
end sub

sub SysCompEqual16
	
	dim SysCalcTempA as word
	dim SysCalcTempB as word
	dim SysCalcTempX as byte
	
	#IFDEF ChipFamily 12,14,15
		clrf SysCalcTempX
		
		'Test low, exit if false
		movf SysCalcTempA, W
		subwf SysCalcTempB, W
		btfss STATUS, Z
		return
		
		'Test high, exit if false
		movf SysCalcTempA_H, W
		subwf SysCalcTempB_H, W
		btfss STATUS, Z
		return
		
		comf SysCalcTempX,F
	#ENDIF
	
	#IFDEF ChipFamily 16
		clrf SysCalcTempX
		
		'Test low, exit if false
		movf SysCalcTempB, W
		cpfseq SysCalcTempA
		return
		
		'Test high, exit if false
		movf SysCalcTempB_H, W
		cpfseq SysCalcTempA_H
		return
		
		setf SysCalcTempX
		
	#ENDIF
	#IFDEF AVR
		clr SysCalcTempX
		
		cp SysCalcTempA, SysCalcTempB
		brne SCE16False
		
		cp SysCalcTempA_H, SysCalcTempB_H
		brne SCE16False
		
		com SysCalcTempX
		SCE16False:
	#ENDIF
end sub

'Less than
'A-B - if negative, C is off
'if A is 4 and B is 2, C is on
'if A is 3 and B is 3, C is on
'if A is 2 and B is 4, C is off
sub SysCompLessThan
	
	#IFDEF ChipFamily 12,14,15
		clrf SysCalcTempX
		bsf STATUS, C
		movf SysCalcTempB, W
		subwf SysCalcTempA, W
		btfss STATUS, C
		comf SysCalcTempX,F
	#ENDIF
	
	#IFDEF ChipFamily 16
		setf SysCalcTempX
		movf SysCalcTempB, W
		cpfslt SysCalcTempA
		clrf SysCalcTempX
	#ENDIF
	
	#IFDEF AVR
		clr SysCalcTempX
		cp SysCalcTempA,SysCalcTempB
		brlo SCLTTrue
		ret
		
		SCLTTrue:
		com SysCalcTempX
	#ENDIF
end sub

sub SysCompLessThan16
#IFDEF PIC
 dim SysCalcTempA as word
 dim SysCalcTempB as word
 dim SysCalcTempX as byte
 
 clrf SysCalcTempX
 
 'Test High, exit if more
 movf SysCalcTempA_H,W
 subwf SysCalcTempB_H,W
 btfss STATUS,C
 return
 
 'Test high, exit true if less
 movf SysCalcTempB_H,W
 subwf SysCalcTempA_H,W
 #IFDEF ChipFamily 12,14,15
  btfss STATUS,C
  goto SCLT16True
 #ENDIF
 #IFDEF ChipFamily 16
  bnc SCLT16True
 #ENDIF
 
 'Test Low, exit if more or equal
 movf SysCalcTempB,W
 subwf SysCalcTempA,W
 btfsc STATUS,C
 return

 SCLT16True:
 #IFDEF ChipFamily 12,14,15
  comf SysCalcTempX,F
 #ENDIF
 #IFDEF ChipFamily 16
  setf SysCalcTempX
 #ENDIF
#ENDIF

#IFDEF AVR
 clr SysCalcTempX
 
 'Test High, exit false if more
 cp SysCalcTempB_H,SysCalcTempA_H
 brlo SCLT16False
 
 'Test high, exit true if less
 cp SysCalcTempA_H,SysCalcTempB_H
 brlo SCLT16True
 
 'Test Low, exit if more or equal
 cp SysCalcTempA,SysCalcTempB
 brlo SCLT16True
 ret

 SCLT16True:
 com SysCalcTempX
 SCLT16False:
#ENDIF

end sub

'Returns true if A < B
sub SysCompLessThanInt
	
	Dim SysCalcTempA, SysCalcTempB, SysDivMultA as Integer
	
	'Clear result
	SysCalcTempX = 0
	
	'Compare sign bits
	'-A
	If SysCalcTempA.15 = On Then
		'-A, +B, return true
		If SysCalcTempB.15 = Off Then
			'Set SysCalcTempX to 255
			SysCalcTempX = Not SysCalcTempX
			Exit Sub
		End If
		'-A, -B, negate both and swap
		SysDivMultA = -SysCalcTempA
		SysCalcTempA = -SysCalcTempB
		SysCalcTempB = SysDivMultA
	'+A
	Else
		'+A, -B, return false
		If SysCalcTempB.15 = On Then
			Exit Sub
		End If
	End If
	
	#IFDEF PIC
		
		'Test High, exit if more
		movf SysCalcTempA_H,W
		subwf SysCalcTempB_H,W
		btfss STATUS,C
		return
		
		'Test high, exit true if less
		movf SysCalcTempB_H,W
		subwf SysCalcTempA_H,W
		#IFDEF ChipFamily 12,14,15
			btfss STATUS,C
			goto SCLTIntTrue
		#ENDIF
		#IFDEF ChipFamily 16
			bnc SCLTIntTrue
		#ENDIF
		
		'Test Low, exit if more or equal
		movf SysCalcTempB,W
		subwf SysCalcTempA,W
		btfsc STATUS,C
		return
		
	SCLTIntTrue:
		comf SysCalcTempX,F
	#ENDIF
	
	#IFDEF AVR
		
		'Test High, exit false if more
		cp SysCalcTempB_H,SysCalcTempA_H
		brlo SCLTIntFalse
		
		'Test high, exit true if less
		cp SysCalcTempA_H,SysCalcTempB_H
		brlo SCLTIntTrue
		
		'Test Low, exit if more or equal
		cp SysCalcTempA,SysCalcTempB
		brlo SCLTIntTrue
		ret
		
	SCLTIntTrue:
		com SysCalcTempX
	SCLTIntFalse:
	#ENDIF
	
end sub
