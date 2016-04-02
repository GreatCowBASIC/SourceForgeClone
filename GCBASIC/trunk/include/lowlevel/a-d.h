'    Analog to Digital conversion routines for Great Cow BASIC
'    Copyright (C) 2006 - 2013, 2016 Hugh Considine, Kent Schafer, William Roth, Evan Venn

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

'24/12/2015 - William Roth
'   1) ReadADx now returns correct values when changing between
'      ReadAD, ReadAD10, and ReadAd12 in same program.
'   2) Added optional parameter In ReadADx function setup
'      Negative AD Analog Port.
'   3) Negative ADC Values Supported for Differential ADC
'   4) 8-Bit Differential Reads supported with negative nunbers
'   5) NOTE: Negative numbers require user to declare
'       AD Read Variable as integer
'-------------------------------------------------
'27/12/2015 - William Roth
'	Revised ReadAD and ReadAD10 for greater compatibility.  Revised at RC37
'6/3/2016 - Evan Venn
' Revised ReadAD, ReadAD10 and ReadAD12 for ADPCH
'	Corrected typo in LLREADAD

'7/3/2016 - William Roth
'Added IFDEF's to prevent unnecessary select case code from compiling when

'AD Channel is not used. New Define "USE_ADx" where x is AD Channel
'Set to USE_AD0 FALSE to optimise the code.
' This can be changed if desired.

' Example:  #define USE_AD9
' This will save about 200 bytes
'8/3/2016 revised to USE_ADx support by default.

'14/3/2016 - Added spport for 16F168xx Chips
'See new section starting at line 637 for details

'Commands:
'var = ReadAD(port, optional port)	Reads port(s), and returns value.
'ADFormat(type)		Choose Left or Right justified
'ADOff			Set A/D converter off. Use if trouble is experienced when
'attempting to use ports in digital mode


#define Format_Left 0
#define Format_Right 255
#define SINGLE 255

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
#define AN28 28
#define AN29 39
#define AN30 30
#define AN31 31
#define AN32 32
#define AN33 33
#define AN34 34

#define ANA0 AN0
#define ANA1 AN1
#define ANA2 AN2
#define ANA3 AN3
#define ANA4 AN4
#define ANA5 AN5
#define ANA6 AN6
#define ANA7 AN7
#define ANB0 AN8
#define ANB1 AN9
#define ANB2 AN10
#define ANB3 AN11
#define ANB4 AN12
#define ANB5 AN13
#define ANB6 AN14
#define ANB7 AN15
#define ANC0 AN16
#define ANC1 AN17
#define ANC2 AN18
#define ANC3 AN19
#define ANC4 AN20
#define ANC5 AN21
#define ANC6 AN22
#define ANC7 AN23
#define AND0 AN24
#define AND1 AN25
#define AND2 AN26
#define AND3 AN27
#define AND4 AN28
#define AND5 AN29
#define AND6 AN30
#define AND7 AN31
#define ANE0 AN32
#define ANE1 AN33
#define ANE2 AN34

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

#define USE_AD0 TRUE
#define USE_AD1 TRUE
#define USE_AD2 TRUE
#define USE_AD2 TRUE
#define USE_AD3 TRUE
#define USE_AD4 TRUE
#define USE_AD5 TRUE
#define USE_AD6 TRUE
#define USE_AD7 TRUE
#define USE_AD8 TRUE
#define USE_AD9 TRUE
#define USE_AD10 TRUE
#define USE_AD11 TRUE
#define USE_AD12 TRUE
#define USE_AD13 TRUE
#define USE_AD14 TRUE
#define USE_AD15 TRUE
#define USE_AD16 TRUE
#define USE_AD17 TRUE
#define USE_AD18 TRUE
#define USE_AD19 TRUE
#define USE_AD20 TRUE
#define USE_AD21 TRUE
#define USE_AD22 TRUE
#define USE_AD23 TRUE
#define USE_AD24 TRUE
#define USE_AD25 TRUE
#define USE_AD26 TRUE
#define USE_AD27 TRUE
#define USE_AD28 TRUE
#define USE_AD29 TRUE
#define USE_AD30 TRUE
#define USE_AD31 TRUE
#define USE_AD32 TRUE
#define USE_AD33 TRUE
#define USE_AD34 TRUE

'16f1688x
#define USE_ADA0 TRUE
#define USE_ADA1 TRUE
#define USE_ADA2 TRUE
#define USE_ADA3 TRUE
#define USE_ADA4 TRUE
#define USE_ADA5 TRUE
#define USE_ADA6 TRUE
#define USE_ADA7 TRUE
#define USE_ADB0 TRUE
#define USE_ADB1 TRUE
#define USE_ADB2 TRUE
#define USE_ADB3 TRUE
#define USE_ADB4 TRUE
#define USE_ADB5 TRUE
#define USE_ADB6 TRUE
#define USE_ADB7 TRUE
#define USE_ADC0 TRUE
#define USE_ADC1 TRUE
#define USE_ADC2 TRUE
#define USE_ADC3 TRUE
#define USE_ADC4 TRUE
#define USE_ADC5 TRUE
#define USE_ADC6 TRUE
#define USE_ADC7 TRUE
#define USE_ADD0 TRUE
#define USE_ADD1 TRUE
#define USE_ADD2 TRUE
#define USE_ADD3 TRUE
#define USE_ADD4 TRUE
#define USE_ADD5 TRUE
#define USE_ADD6 TRUE
#define USE_ADD7 TRUE
#define USE_ADE0 TRUE
#define USE_ADE1 TRUE
#define USE_ADE2 TRUE



macro LLReadAD (ADLeftAdjust)

	#IFDEF PIC
      'Set up A/D
      'Make necessary ports analog
      'Code for PICs with older A/D (No ANSEL register)
    #IFDEF NoVar(ADCON3)  ' is not 16F188xx

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

                #ifdef USE_AD0 TRUE
                  Case 0: Set ANSELA.0 On
                #endif
                #ifdef USE_AD1 TRUE
                  Case 1: Set ANSELA.1 On
                #endif
                #ifdef USE_AD2 TRUE
                  Case 2: Set ANSELA.2 On
                #endif
                #ifdef USE_AD3 TRUE
                  Case 3: Set ANSELA.3 On
                #endif
                #ifdef USE_AD4 TRUE
                  Case 4: Set ANSELA.4 On
                #endif
                #ifdef USE_AD11 TRUE
                  Case 11: Set ANSELB.1 On
                #endif
                #ifdef USE_AD10 TRUE
                  Case 10: Set ANSELB.2 On
                #endif
                #ifdef USE_AD9 TRUE
                  Case 9: Set ANSELB.3 On
                #endif
                #ifdef USE_AD8 TRUE
                  Case 8: Set ANSELB.4 On
                #endif
                #ifdef USE_AD7 TRUE
                  Case 7: Set ANSELB.5 On
                #endif
                #ifdef USE_AD5 TRUE
                  Case 5: Set ANSELB.6 On
                #endif
                #ifdef USE_AD6 TRUE
                  Case 6: Set ANSELB.7 On
                #endif
              #ENDIF

              #IF ChipPins = 28 Or ChipPins = 40

                #Ifdef USE_AD0 TRUE
                  Case 0: Set ANSELA.0 On
                #endif
                #Ifdef USE_AD1 TRUE
                  Case 1: Set ANSELA.1 On
                #endif
                #Ifdef USE_AD2 TRUE
                  Case 2: Set ANSELA.2 On
                #endif
                #Ifdef USE_AD3 TRUE
                  Case 3: Set ANSELA.3 On
                #endif
                #Ifdef USE_AD4 TRUE
                  Case 4: Set ANSELA.5 On
                #endif

                #IFDEF Var(ANSELB)

                  #Ifdef USE_AD12 TRUE
                     Case 12: Set ANSELB.0 On
                  #endif

                  #Ifdef USE_AD10 TRUE
                    Case 10: Set ANSELB.1 On
                  #endif

                  #Ifdef USE_AD8 TRUE
                    Case 8: Set ANSELB.2 On
                  #endif

                  #Ifdef USE_AD9 TRUE
                    Case 9: Set ANSELB.3 On
                  #endif

                  #Ifdef USE_AD11 TRUE
                    Case 11: Set ANSELB.4 On
                  #endif

                  #Ifdef USE_AD13 TRUE
                    Case 13: Set ANSELB.5 On
                  #endif

                #ENDIF

                #IFDEF Var(ANSELC)

                  #Ifdef USE_AD14 TRUE
                    Case 14: Set ANSELC.2 On
                  #endif
                  #Ifdef USE_AD15 TRUE
                    Case 15: Set ANSELC.3 On
                  #endif
                  #Ifdef USE_AD16 TRUE
                    Case 16: Set ANSELC.4 On
                  #endif
                  #Ifdef USE_AD17 TRUE
                    Case 17: Set ANSELC.5 On
                  #endif
                  #Ifdef USE_AD18 TRUE
                    Case 18: Set ANSELC.6 On
                  #endif
                  #Ifdef USE_AD19 TRUE
                    Case 19: Set ANSELC.7 On
                  #endif
                #ENDIF

                #IFDEF Var(ANSELD)
                  #Ifdef USE_AD20 TRUE
                    Case 20: Set ANSELD.0 On
                  #endif
                  #Ifdef USE_AD21 TRUE
                    Case 21: Set ANSELD.1 On
                  #endif
                  #Ifdef USE_AD22 TRUE
                    Case 22: Set ANSELD.2 On
                  #endif
                  #Ifdef USE_AD23 TRUE
                    Case 23: Set ANSELD.3 On
                  #endif
                  #Ifdef USE_AD24 TRUE
                    Case 24: Set ANSELD.4 On
                  #endif
                  #Ifdef USE_AD25 TRUE
                    Case 25: Set ANSELD.5 On
                  #endif
                  #Ifdef USE_AD26 TRUE
                    Case 26: Set ANSELD.6 On
                  #endif
                  #Ifdef USE_AD27 TRUE
                    Case 27: Set ANSELD.7 On
                  #endif
                #ENDIF

                #IFDEF Var(ANSELE)

                  #Ifdef USE_AD5 TRUE
                    Case 5: Set ANSELE.0 On
                  #endif
                  #Ifdef USE_AD6 TRUE
                    Case 6: Set ANSELE.1 On
                  #endif
                  #Ifdef USE_AD7 TRUE
                    Case 7: Set ANSELE.2 On
                  #endif
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

     #ENDIF

     '***  'Special section for 16F1688x Chips ***
     #IFDEF Var(ADCON3)  ' then must be 16F1688x

'       'Configure ANSELA/B/C/D for 16F1885x
        Select Case ADReadPort

            #ifdef USE_ADA0 TRUE
              Case 0: Set ANSELA.0 On
            #endif
            #ifdef USE_ADA1 TRUE
              Case 1: Set ANSELA.1 On
            #endif
            #ifdef USE_ADA2 TRUE
              Case 2: Set ANSELA.2 On
            #endif
            #ifdef USE_ADA3 TRUE
              Case 3: Set ANSELA.3 On
            #endif
            #ifdef USE_ADA4 TRUE
              Case 4: Set ANSELA.4 ON
            #endif
            #ifdef USE_ADA5 TRUE
              Case 5: Set ANSELA.5 On
            #endif
            #ifdef USE_ADA6 TRUE
              Case 6: Set ANSELA.6 On
            #endif
            #ifdef USE_ADA7 TRUE
              Case 7: Set ANSELA.7 On
            #endif

            #ifdef USE_ADB0 TRUE
              Case 8: Set ANSELB.0 On
            #endif
            #ifdef USE_ADB1 TRUE
              Case 9: Set ANSELB.1 On
            #endif
            #ifdef USE_ADB2 TRUE
              Case 10: Set ANSELB.2 On
            #endif
            #ifdef USE_ADB3 TRUE
              Case 11: Set ANSELB.3 On
            #endif
             #Ifdef USE_ADB4 TRUE
              Case 12: Set ANSELB.4 On
            #endif
            #Ifdef USE_ADB5 TRUE
              Case 13: Set ANSELB.5 On
            #endif
            #Ifdef USE_ADB6 TRUE
              Case 14: Set ANSELB.6 On
            #endif
            #Ifdef USE_ADB7 TRUE
              Case 15: Set ANSELB.7 On
            #endif

            #Ifdef USE_ADC0 TRUE
              Case 16: Set ANSELC.0 On
            #endif
            #Ifdef USE_ADC1 TRUE
              Case 17: Set ANSELC.1 On
            #endif
            #Ifdef USE_ADC2 TRUE
              Case 18: Set ANSELC.2 On
            #endif
            #Ifdef USE_ADC3 TRUE
              Case 19: Set ANSELC.3 On
            #endif
            #Ifdef USE_ADC4 TRUE
              Case 20: Set ANSELC.4 On
            #endif
            #Ifdef USE_ADC5 TRUE
              Case 21: Set ANSELC.5 On
            #endif
            #Ifdef USE_ADC6 TRUE
              Case 22: Set ANSELC.6 On
            #endif
            #Ifdef USE_ADC7 TRUE
              Case 23: Set ANSELC.7 On
            #endif

            #IFDEF Var(ANSELD)

                #Ifdef USE_ADD0 TRUE
                  Case 24: Set ANSELD.0 On
                #endif
                #Ifdef USE_ADD1 TRUE
                  Case 25: Set ANSELD.1 On
                #endif
                #Ifdef USE_ADD2 TRUE
                  Case 26: Set ANSELD.2 On
                #endif
                #Ifdef USE_ADD3 TRUE
                  Case 27: Set ANSELD.3 On
                #endif
                #Ifdef USE_ADD4 TRUE
                  Case 28: Set ANSELD.4 On
                #endif
                #Ifdef USE_ADD5 TRUE
                  Case 29: Set ANSELD.5 On
                #endif
                #Ifdef USE_ADD6 TRUE
                  Case 30: Set ANSELD.6 On
                #endif
                #Ifdef USE_ADD7 TRUE
                  Case 31: Set ANSELD.7 On
                #endif
            #ENDIF

            #IFDEF Var(ANSELE)

              #Ifdef USE_AD32 TRUE
                Case 32: Set ANSELE.0 On
              #endif
              #Ifdef USE_AD33 TRUE
                Case 33: Set ANSELE.1 On
              #endif
              #Ifdef USE_AD34 TRUE
                Case 34: Set ANSELE.2 On
              #endif
            #ENDIF
        End Select
        '*** ANSEL Bits are now set ***

       'Set voltage reference
       'ADREF = 0  'Default = 0 /Vref+ = Vdd/ Vref-  = Vss

        'Configure AD clock defaults
        Set ADCS off 'Clock source = FOSC/ADCLK
        ADCLK = 1 ' default to FOSC/2

        'Conversion Clock Speed
        #IFDEF ADSpeed HighSpeed
         Set ADCS OFF   ' ADCON0.4
         ADCLK = 1      ' FOSC/2
        #ENDIF

        #IFDEF ADSpeed MediumSpeed
         SET ADCS OFF  'ADCON0.4
         ADCLK = 15    'FOSC/16

        #ENDIF

        #IFDEF ADSpeed LowSpeed
         SET ADCS OFF  ' ADCON0.4
         ADCLK = 31    ' FOSC/32
        #ENDIF

        #IFDEF ADSpeed InternalClock
          SET ADCS ON 'ADCLK has no effect
        #ENDIF

       'Result formatting
        if ADLeftadjust = 0 then
           Set ADCON0.2 ON     '10-bit
        Else
           Set ADCON.2 off     '8-bit
        End if

       'Select Channel
        ADCPH = ADReadPort  'Configure AD read Channel

     #ENDIF   'End of 16F188x Section

		'Enable A/D
		SET ADCON0.ADON ON

		'Acquisition Delay
		Wait AD_Delay

		'Read A/D
		#ifdef bit(GO_NOT_DONE)
			SET ADCON0.GO_NOT_DONE ON
			nop
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

function ReadAD(ADReadPort, in OPTIONAL ADN_PORT = 255) as integer
 'added optional ADN_PORT to support differential ADC

  #IFDEF PIC
      #IFDEF Bit(CHSN0)

      	#IFDEF Bit(ADFM)
						SET ADFM ON
				#ENDIF

      	;set AD Result Mode to 12-Bit
  			#IFDEF Bit(ADRMD)      ;Added for 16F178x
    			SET ADRMD OFF      ; WMR
  			#ENDIF

         	configNegativeChannel

      #ENDIF

      'for 16F1885x and possibly future others
      #IFDEF VAR(ADPCH)
      		ADPCH = ADReadPort
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
function ReadAD10(ADReadPort, OPTIONAL ADN_PORT = 255) As integer

	#IFDEF PIC
	   	#IFDEF Bit(ADFM)
					SET ADFM ON
			#ENDIF


     	#IFDEF Bit(CHSN0)
      		;set AD Result Mode to 10-Bit
  				#IFDEF Bit(ADRMD)      ;Added for 16F178x
    					SET ADRMD ON      ; WMR
  				#ENDIF
         	configNegativeChannel
      #ENDIF

      #IFDEF VAR(ADPCH)
      		ADPCH = ADReadPort
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

			#IFDEF Bit(CHSN0)'18F PIC with 12=bit ADC do not support 10-bit result
     	' Added DIV/4 to return 10 bit value -WMR
 					#IFNDEF Bit(CHSN3)
        	   READAD10 = READAD10 / 4
        	#ENDIF
			#ENDIF
 	#ENDIF

	#IFDEF AVR
			ReadAD10 = LLADResult
	#ENDIF

end function

'Larger ReadAD
function ReadAD12(ADReadPort, OPTIONAL ADN_PORT = 255) As integer

	#IFDEF PIC

   'Set up A/D format
			#IFDEF Bit(ADFM)
		  		SET ADFM ON
	  	#ENDIF

      'Set A/D Result Mode to 12-Bit  (16F178x) -WMR
			#IFDEF Bit(ADRMD)
      		SET ADRMD OFF
    	#ENDIF

      #IFDEF Bit(CHSN0)'Chip has Differential ADC Module
         configNegativeChannel
      #ENDIF

      #IFDEF VAR(ADPCH)
      		ADPCH = ADReadPort
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

Sub ConfigNegativeChannel
   'Sub added to support PICS with Differential ADC MOdule- WMR

   #IFNDEF Bit(CHSN3) '18F
        'Configure Negative A/D Channel
        'Only AN0 - AN6 allowed
       IF ADN_PORT >=0 and ADN_PORT <=6 then
             N_CHAN = ADN_PORT + 1
             ADCON1 = ADCON1 AND b'11111000' OR N_CHAN
       END IF

       IF ADN_PORT > 6  then
            set CHSN2 OFF
    	      set CHSN1 OFF
    				set CHSN0 OFF
       END IF
   #ENDIF

   #IFDEF Bit(CHSN3) '16F178x
      ' AN0 - AN13 and AN21 Allowed
      'configure Negative A/D Channel
			IF ADN_PORT >= 0 and ADN_PORT <= 13 then
           N_CHAN = ADN_PORT
           ADCON2 = ADCON2 AND b'11110000' OR N_CHAN
      END IF

      'exception for (16F1789) where AN21 = channel 14
			IF ADN_PORT = 21 then
           N_CHAN = 14
           ADCON2 = ADCON2 AND b'11110000' OR N_CHAN
			END IF

      IF ADN_PORT > 13 and ADN_PORT <> 21 then
          set CHSN3 ON
    	    set CHSN2 ON
    	    set CHSN1 ON
          set CHSN0 ON
      END IF
   #ENDIF

End Sub
