'    Analog to Digital conversion routines for Great Cow BASIC
'    Copyright (C) 2006 - 2013 Hugh Considine, Kent Schafer

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

'Original code by Hugh Considine
'18Fxx31 code by Kent Schafer
'added ReadAD12 2/12/2015

'21/12/2015 'Added additional support for 16F178x - William Roth
'    1) ReadADx now returns correct values when changing between
'       ReadAD, ReadAD10, and ReadAd12 in same program.
'    2) Moved CHSN Bit settings from ReadAD Functions to #Startup
'       so that User CHSN bit settings in source are not overwritten
'       when ReadAD function is called.
'    3) Differential ADC Reads are now supported by setting CHSN
'       bits in GCB Source Code
'    4) Negative ADC Values Supported for DIfferential ADC
'    5) 8-Bit Differential Reads supported with negative nunbers
'    6) NOTE: Negative numbers require user to declare
'       AD Read Variable as integer
'-------------------------------------------------

'Commands:
'var = ReadAD(port)	Reads port, and returns value.
'ADFormat(type)		Choose Left or Right justified
'ADOff			Set A/D converter off. Use if trouble is experienced when
'			attempting to use ports in digital mode

#startup SetCHSN

#define Format_Left 0
#define Format_Right 255
#define AD_SINGLE ' default

'Acquisition time. Can be reduced in some circumstances - see PIC manual for details
#define AD_Delay 2 10us

'Delay for AVR VRef capacitor charging from internal reference
'Increase if reference voltage is too low, or decrease for faster readings
'(ATmega328p datasheet suggests 70 us, but this isn't enough for Arduino)
'Only applies whe AD_REF_SOURCE is AD_REF_256
#define AD_VREF_DELAY 1 ms

'added for differential ADC


'Optimisation
#define ADSpeed MediumSpeed

#define HighSpeed 255
#define MediumSpeed 128
#define LowSpeed 0
#define InternalClock 192

'ADC reference sources (AVR only)
#define AD_REF_SOURCE AD_REF_AVCC
#define AD_REF_AREF 0
#define AD_REF_AVCC 1
#define AD_REF_256 3

'Port names
'PIC style
#define AN0 0
#define AN1 1
#define AN2 2
#define AN3 3
#define AN4 4
#define AN5 5
#define AN6 6
#define AN7 7
#define AN8 8
#define AN9 9
#define AN10 10
#define AN11 11
#define AN12 12
#define AN13 13
#define AN14 14
#define AN15 15
#define AN16 16
#define AN17 17
#define AN18 18
#define AN19 19
#define AN20 20
#define AN21 21
#define AN22 22
#define AN23 23
#define AN24 24
#define AN25 25
#define AN26 26
#define AN27 27

'AVR style
#define ADC0 0
#define ADC1 1
#define ADC2 2
#define ADC3 3
#define ADC4 4
#define ADC5 5
#define ADC6 6
#define ADC7 7
#define ADC8 8
#define ADC9 9
#define ADC10 10
#define ADC11 11
#define ADC12 12
#define ADC13 13

macro LLReadAD (ADLeftAdjust)

	#IFDEF PIC

		'Set up A/D
		'Make necessary ports analog
		'Code for PICs with older A/D (No ANSEL register)
		#IFDEF NoVar(ANSEL)
			#IFDEF NoVar(ANSEL0)
				#IFDEF NoVar(ANSELA)
					#IFDEF NoVar(ANSELB)

						#IFDEF NoBit(PCFG4)
							#IFDEF NoVar(ADCON2)
								#IFDEF NoBit(ANS0)
									'Example: 16F877A
									#IFDEF Bit(PCFG3)
										SET PCFG3 OFF
									#ENDIF
									SET PCFG2 OFF
									SET PCFG1 OFF
									SET PCFG0 OFF
								#ENDIF
								'Example: 10F220
								#IFDEF Bit(ANS0)
									SET ANS0 OFF
									SET ANS1 OFF
								#ENDIF
							#ENDIF

							#IFDEF Var(ADCON2)
								'Example: 18F4620
								#IFDEF BIT(PCFG3)
									SET PCFG3 OFF
									SET PCFG2 OFF
									SET PCFG1 OFF
									SET PCFG0 OFF
								#ENDIF
							#ENDIF
						#ENDIF

						'PICs with PCFG4 and higher use ADCON1 as an ANSEL type register
						'Example: 18F1320
						#IFDEF Bit(PCFG4)
							'Some 18F8xxxx chips have error in chip definition
							'They claim to have PCFG4, but actually don't, can spot them by presence of ADCON2
							Dim AllANSEL As Byte Alias ADCON1
							AllANSEL = 0
							ADTemp = ADReadPort + 1
							Set C On
							Do
								Rotate AllANSEL Left
								decfsz ADTemp,F
							Loop
						#ENDIF

					'ANSELB/A
					#ENDIF
				#ENDIF

				'Code for 16F193x chips (and others?) with ANSELA/ANSELB/ANSELE registers
				#IFDEF Var(ANSELA)
					Select Case ADReadPort
						#IF ChipPins = 18
							Case 0: Set ANSELA.0 On
							Case 1: Set ANSELA.1 On
							Case 2: Set ANSELA.2 On
							Case 3: Set ANSELA.3 On
							Case 4: Set ANSELA.4 On

							Case 11: Set ANSELB.1 On
							Case 10: Set ANSELB.2 On
							Case 9: Set ANSELB.3 On
							Case 8: Set ANSELB.4 On
							Case 7: Set ANSELB.5 On
							Case 5: Set ANSELB.6 On
							Case 6: Set ANSELB.7 On
						#ENDIF

						#IF ChipPins = 28 Or ChipPins = 40
							Case 0: Set ANSELA.0 On
							Case 1: Set ANSELA.1 On
							Case 2: Set ANSELA.2 On
							Case 3: Set ANSELA.3 On
							Case 4: Set ANSELA.5 On

							#IFDEF Var(ANSELB)
								Case 12: Set ANSELB.0 On
								Case 10: Set ANSELB.1 On
								Case 8: Set ANSELB.2 On
								Case 9: Set ANSELB.3 On
								Case 11: Set ANSELB.4 On
								Case 13: Set ANSELB.5 On
							#ENDIF

							#IFDEF Var(ANSELC)
								Case 14: Set ANSELC.2 On
								Case 15: Set ANSELC.3 On
								Case 16: Set ANSELC.4 On
								Case 17: Set ANSELC.5 On
								Case 18: Set ANSELC.6 On
								Case 19: Set ANSELC.7 On
							#ENDIF

							#IFDEF Var(ANSELD)
								Case 20: Set ANSELD.0 On
								Case 21: Set ANSELD.1 On
								Case 22: Set ANSELD.2 On
								Case 23: Set ANSELD.3 On
								Case 24: Set ANSELD.4 On
								Case 25: Set ANSELD.5 On
								Case 26: Set ANSELD.6 On
								Case 27: Set ANSELD.7 On
							#ENDIF

							#IFDEF Var(ANSELE)
								Case 5: Set ANSELE.0 On
								Case 6: Set ANSELE.1 On
								Case 7: Set ANSELE.2 On
							#ENDIF
						#ENDIF

					End Select
				#ENDIF

				'ANSEL0/ANSEL
			#ENDIF
		#ENDIF

		'Code for PICs with newer A/D (with ANSEL register)
		#IFDEF Var(ANSEL)
			#IFDEF Var(ANSELH)
				Dim AllANSEL As Word Alias ANSELH, ANSEL
			#ENDIF
			#IFDEF NoVar(ANSELH)
				Dim AllANSEL As Byte Alias ANSEL
			#ENDIF
			AllANSEL = 0
			ADTemp = ADReadPort + 1
			Set C On
			Do
				Rotate AllANSEL Left
				decfsz ADTemp,F
			Loop

		#ENDIF
		'Code for 18F4431, uses ANSEL0 and ANSEL1
		#IFDEF Var(ANSEL0)
			#IFDEF Var(ANSEL1)
				Dim AllANSEL As Word Alias ANSEL1, ANSEL0
			#ENDIF
			#IFDEF NoVar(ANSEL1)
				Dim AllANSEL As Byte Alias ANSEL0
			#ENDIF
			AllANSEL = 0
			ADTemp = ADReadPort + 1
			Set C On
			Do
				Rotate AllANSEL Left
				decfsz ADTemp,F
			Loop

		#ENDIF

		'Set Auto or Single Convert Mode
		#IFDEF Bit(ACONV)
			SET ACONV OFF  'Single shot mode
			SET ACSCH OFF  'Single channel CONVERSION
			'GroupA
			IF ADReadPort = 0 OR ADReadPort = 4 OR ADReadPort = 8 Then
				SET ACMOD1 OFF
				SET ACMOD0 OFF
			END IF
			'GroupB
			IF ADReadPort = 1 OR ADReadPort = 5 Then
				SET ACMOD1 OFF
				SET ACMOD0 ON
			END IF
			'GroupC
			IF ADReadPort = 2 OR ADReadPort = 6 Then
				SET ACMOD1 ON
				SET ACMOD0 OFF
			END IF
			'GroupD
			IF ADReadPort = 3 OR ADReadPort = 7 Then
				SET ACMOD1 ON
				SET ACMOD0 ON
			END IF

		#ENDIF

		'Set conversion clock
		#IFDEF Bit(ADCS0)
			#IFDEF ADSpeed HighSpeed
				SET ADCS1 OFF
				SET ADCS0 OFF
			#ENDIF
			#IFDEF ADSpeed MediumSpeed
				SET ADCS1 OFF
				SET ADCS0 ON
			#ENDIF
			#IFDEF ADSpeed LowSpeed
				SET ADCS1 ON
				SET ADCS0 ON
			#ENDIF
			#IFDEF ADSpeed InternalClock
				SET ADCS1 ON
				SET ADCS0 ON
			#ENDIF
		#ENDIF

		'Choose port
		#IFDEF Bit(CHS0)
			SET ADCON0.CHS0 OFF
			SET ADCON0.CHS1 OFF
			#IFDEF Bit(CHS2)
				SET ADCON0.CHS2 OFF
				#IFDEF Bit(CHS3)
					SET ADCON0.CHS3 OFF
					#IFDEF Bit(CHS4)
						SET ADCON0.CHS4 OFF
					#ENDIF
				#ENDIF
			#ENDIF

			IF ADReadPort.0 On Then Set ADCON0.CHS0 On
			IF ADReadPort.1 On Then Set ADCON0.CHS1 On
			#IFDEF Bit(CHS2)
				IF ADReadPort.2 On Then Set ADCON0.CHS2 On
				#IFDEF Bit(CHS3)
					If ADReadPort.3 On Then Set ADCON0.CHS3 On
					#IFDEF Bit(CHS4)
						If ADReadPort.4 On Then Set ADCON0.CHS4 On
					#ENDIF
				#ENDIF
			#ENDIF
		#ENDIF
		#IFDEF BIT(GASEL0)
			'GROUP A SELECT BITS
			IF ADReadPort = 0 THEN
				SET GASEL1 OFF
				SET GASEL0 OFF
			END IF
			IF ADReadPort = 4 THEN
				SET GASEL1 OFF
				SET GASEL0 ON
			END IF
			IF ADReadPort = 8 THEN
				SET GASEL1 ON
				SET GASEL0 OFF
			END IF
			'GROUP C SELECT BITS
			IF ADReadPort = 2 THEN
				SET GCSEL1 OFF
				SET GCSEL0 OFF
			END IF
			IF ADReadPort = 6 THEN
				SET GCSEL1 OFF
				SET GCSEL0 ON
			END IF
			'GROUP B SELECT BITS
			IF ADReadPort = 1 THEN
				SET GBSEL1 OFF
				SET GBSEL0 OFF
			END IF
			IF ADReadPort = 5 THEN
				SET GBSEL1 OFF
				SET GBSEL0 ON
			END IF
			'GROUP D SELECT BITS
			IF ADReadPort = 3 THEN
				SET GDSEL1 OFF
				SET GDSEL0 OFF
			END IF
			IF ADReadPort = 7 THEN
				SET GDSEL1 OFF
				SET GDSEL0 ON
			END IF
		#ENDIF

		'Enable A/D
		SET ADCON0.ADON ON

		'Acquisition Delay
		Wait AD_Delay

		'Read A/D
		#ifdef bit(GO_NOT_DONE)
			SET ADCON0.GO_NOT_DONE ON
			Wait While ADCON0.GO_NOT_DONE ON
		#endif
		#ifndef bit(GO_NOT_DONE)
			#IFDEF Bit(GO_DONE)
				SET ADCON0.GO_DONE ON
				Wait While ADCON0.GO_DONE ON
			#ENDIF
			#IFNDEF Bit(GO_DONE)
				#IFDEF Bit(GO)
					SET ADCON0.GO ON
					Wait While ADCON0.GO ON
				#ENDIF
			#ENDIF
		#endif

		'Switch off A/D
		#IFDEF Var(ADCON0)
			SET ADCON0.ADON OFF
			#IFDEF NoVar(ANSEL)
				#IFDEF NoVar(ANSELA)

					#IFDEF NoBit(PCFG4)
						#IFDEF NoVar(ADCON2)
							#IFDEF NoBit(ANS0)
								#IFDEF Bit(PCFG3)
									SET PCFG3 OFF
								#ENDIF
								SET PCFG2 ON
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
		#IFDEF Var(ANSELC)
			ANSELC = 0
		#ENDIF
		#IFDEF Var(ANSELD)
			ANSELD = 0
		#ENDIF
		#IFDEF Var(ANSELE)
			ANSELE = 0
		#ENDIF

	#ENDIF

	#IFDEF AVR

		'Select channel
		#IFNDEF Bit(MUX5)
			ADMUX = ADReadPort
		#endif
		#ifdef Bit(MUX5)
			#ifdef NoBit(ADATE)
				ADCSRB.MUX5 = ADReadPort.5
				ADMUX = ADReadPort And 0x1F
			#endif
			#ifdef Bit(ADATE)
				ADMUX = 0
				MUX5 = ADReadPort.5
				MUX4 = ADReadPort.4
				MUX3 = ADReadPort.3
				MUX2 = ADReadPort.2
				MUX1 = ADReadPort.1
				MUX0 = ADReadPort.0
			#endif
		#endif
		#ifdef Bit(ADLAR)
			If ADLeftAdjust = 1 Then
				Set ADMUX.ADLAR On
			End If
			If ADLeftAdjust = 0 Then
				Set ADMUX.ADLAR Off
			End If
		#endif

		'Select reference source
		#ifndef Bit(REFS2)
			If AD_REF_SOURCE = AD_REF_AVCC Then
				Set ADMUX.REFS0 On
			End If
			If AD_REF_SOURCE = AD_REF_256 Then
				Set ADMUX.REFS0 On
				Set ADMUX.REFS1 On
			End If
		#endif
		#ifdef Bit(REFS2)
			If AD_REF_SOURCE = AD_REF_AREF Then
				Set ADMUX.REFS0 On
			End If
			If AD_REF_SOURCE = AD_REF_256 Then
				Set ADMUX.REFS0 On
				Set ADMUX.REFS1 On
				Set REFS2 On
			End If
		#endif

		'Set conversion clock
		#IFDEF Bit(ADPS2)
			#IFDEF ADSpeed HighSpeed
				SET ADPS2 Off
				SET ADPS1 Off
			#ENDIF
			#IFDEF ADSpeed MediumSpeed
				SET ADPS2 On
				SET ADPS1 Off
			#ENDIF
			#IFDEF ADSpeed LowSpeed
				SET ADPS2 On
				SET ADPS1 On
			#ENDIF
		#ENDIF

		'Acquisition Delay
		Wait AD_Delay

		'Take reading
		Set ADEN On
		'After turning on ADC, let internal Vref stabilise
		If AD_REF_SOURCE = AD_REF_256 Then
			Wait AD_VREF_DELAY
		End If
		Set ADSC On
		Wait While ADSC On
		Set ADEN Off

	#ENDIF

end macro

function ReadAD(ADReadPort) as integer


  #IFDEF PIC
      #IFDEF Bit(ADFM)
					SET ADFM ON
			#ENDIF

      ;set AD Result Mode to 12-Bit
  		#IFDEF Bit(ADRMD)      ;Added for 16F178x
    			SET ADRMD OFF      ; WMR
  		#ENDIF
 #ENDIF

'***************************************

'Perform conversion
LLReadAD 1

'Write output
 #IFDEF PIC

    'Chips with no differential ADC (MOST CHIPS) - WMR
    #IFNDEF Bit(CHSN0)
    		#IFDEF Var(ADRESH)
						ReadAD = ADRESH
				#ENDIF
				#IFDEF NoVar(ADRESH)
						ReadAD = ADRES
				#ENDIF
		#ENDIF

       'Allow 8-bit differential reads - WMR
  	#IFDEF Bit(CHSN0) 'Chip has differential ADC

     	'Write output
				#IFDEF NoVar(ADRESL)
						ReadAD = ADRES
				#ENDIF

   			#IFDEF Var(ADRESL)
						ReadAD = ADRESL
				#ENDIF

  			#IFDEF Var(ADRESH)
						ReadAD_H = ADRESH
				#ENDIF

     		#IFDEF Bit(ADFM)
						SET ADFM OFF
				#ENDIF

        ReadAd = ReadAd / 16
 		#ENDIF
 #ENDIF

 #IFDEF AVR
	  ReadAD = ADCH
 #ENDIF

End Function


'Large ReadAD
function ReadAD10(ADReadPort) As integer

	#IFDEF PIC
	'Set up A/D format
			#IFDEF Bit(ADFM)
					SET ADFM ON
			#ENDIF

      ;Set AD Result Mode to 10-Bit for 16F178x
  		;Not supported by 18F66K80 Family
      #IF Bit(ADRMD)
    			SET ADRMD ON
  		#ENDIF
	#ENDIF

 	#IFDEF AVR
			Dim LLADResult As Word Alias ADCH, ADCL
	#ENDIF

'Do conversion
LLReadAD 0

	#IFDEF PIC
			'Write output
			#IFDEF NoVar(ADRESL)
				ReadAD10 = ADRES
			#ENDIF

   		#IFDEF Var(ADRESL)
					ReadAD10 = ADRESL
			#ENDIF

  		#IFDEF Var(ADRESH)
					ReadAD10_H = ADRESH
			#ENDIF

			'Put A/D format back to normal
			#IFDEF Bit(ADFM)
					SET ADFM OFF
			#ENDIF

     '18F PIC with 12=bit ADC do not support 10-bit result
     ' Added DIV/4 to return 10 bit value -WMR
     #IFNDEF Bit(CHSN3)
        READAD10 = READAD10 / 4
     #ENDIF

  #ENDIF

	#IFDEF AVR
			ReadAD10 = LLADResult
	#ENDIF

end function

'Larger ReadAD
function ReadAD12(ADReadPort) As integer

	#IFDEF PIC

   'Set up A/D format
			#IFDEF Bit(ADFM)
		  		SET ADFM ON
	  	#ENDIF

   'Set A/D Result Mode to 12-Bit  (16F178x) -WMR
			#IFDEF Bit(ADRMD)
      		SET ADRMD OFF
    	#ENDIF

  #ENDIF

'Do conversion
LLReadAD 0

#IFDEF PIC
	'Write output
	#IFDEF NoVar(ADRESL)
		ReadAD12 = ADRES
	#ENDIF

	#IFDEF Var(ADRESL)
		ReadAD12 = ADRESL
	#ENDIF

	#IFDEF Var(ADRESH)
		ReadAD12_H = ADRESH
	#ENDIF

	'Put A/D format back to normal
	#IFDEF Bit(ADFM)
		SET ADFM OFF
	#ENDIF
#ENDIF

end function

'This sub is deprecated
sub ADFormat(ADReadFormat)
 SET ADFM OFF
 IF ADReadFormat.1 ON THEN SET ADFM ON
end sub

'This sub is deprecated
sub ADOff
'Disable the A/D converter, and set all ports to digital.
'This sub is deprecated, InitSys automatically turns off A/D

 SET ADCON0.ADON OFF
#IFDEF NoBit(PCFG4)
 #IFDEF NoVar(ANSEL)
  #IFDEF NoVar(ADCON2)
   #IFDEF Bit(PCFG3)
    SET PCFG3 OFF
   #ENDIF
   SET PCFG2 ON
   SET PCFG1 ON
   SET PCFG0 OFF
  #ENDIF
  #IFDEF Var(ADCON2)
   SET PCFG3 ON
   SET PCFG2 ON
   SET PCFG1 ON
   SET PCFG0 ON
  #ENDIF
 #ENDIF
#ENDIF

 #IFDEF Bit(PCFG4)
  #IFDEF Bit(PCFG6)
   SET PCFG6 ON
  #ENDIF
  #IFDEF Bit(PCFG5)
   SET PCFG5 ON
  #ENDIF
  SET PCFG4 ON
  SET PCFG3 ON
  SET PCFG2 ON
  SET PCFG1 ON
  SET PCFG0 ON
 #ENDIF

 #IFDEF Var(ANSEL)
  ANSEL = 0
 #ENDIF
 #IFDEF Var(ANSELH)
  ANSELH = 0
 #ENDIF

end sub

sub SetCHSN
 'sets CHSN (Negative Channel) BITs for PICS
 ' with 12-Bit differential ADC Modules
 ' enables single-sided reads as default
 ' Moved from ReadADx Functions to allow user to configure
 ' CHSN bits in Source GCB file for differential operation. -WMR
 #IFDEF PIC

    #IFDEF bit(CHSN0)
  				'set single-ended as default for PIC18F Chips -WMR
 				'default is OFF for these bits so this is redundant
        #IFNDEF bit(CHSN3)  'PIC is 18F with 12-bit adc
						set CHSN0 OFF   'Default (Redundant)
    				set CHSN1 OFF   'Default
    				set CHSN2 OFF   'Defauly
    		#ENDIF

        'set single-ended as default for PIC 16F178x Chips -WMR
        #IFDEF bit(CHSN3)
						Set ADNREF OFF   'Default (Redundant)
            Set ADPREF0 OFF  'Default
            SET ADPREF1 OFF  'Default

            set CHSN0 ON     'Necessary for 16F178x
						set CHSN1 ON
						set CHSN2 ON
						set CHSN3 ON
				#ENDIF
  	#ENDIF
 #ENDIF

End Sub