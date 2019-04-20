'    System routines for Great Cow BASIC
'    Copyright (C) 2006 - 2019 Hugh Considine,  William Roth and Evan Venn

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

;    12-12-15 by William Roth
;    Added support for PIC18F25K22 @ 32MHz and 64 MHz
;    14-12-15
;    Added support for PIC18F(L)xx20 Chips
;    Added support for family 12 Chips and option_reg
;    Fixed CMCON for 18f chips
;    27072016 - Added support for OSCCON1 config
;    01082016 - Removed new command support
;    04102016 - Added OSCCAL Support
;    12112016 - Revised to correct comparator registers NOT being set for new chips
;    14112016 - Revised OSCCAL Support
;    01012017 - Added Oscillator Support for PIC18FxxK40
;    01012017 - Added NVMREG fix for PIC18FxxK40 A2/A3 Silicon
;    11042017 - Corrected Typo on Line 750 Changed AN9 to AN8
;    23072017 - Added #ifdef Bit(IRCF2) protection
;    12092017 - Revised #IFDEF bit(C1EN): C1EN = 0: #ENDIF an C2EN
;    22-09-2017 - Corrected IRCF Bits for 18FxxK20 (Fixes Speeds < 8 MHz) -WMR
'    2017-09-23: Corrected speed of 18Fxx20 chips, hopefully without breaking 18FxxK20 chips
'    2017-09-24: Revised to add samevar for HFIOFS and to correctly set SPLLEN after IRF0..3
'    25092017 - Revised as follows:
'                INITSYS now handles two true tables for frequency... between 16 and 0.125x``
'                Test on 18f2425 (type1 max frq of 8mhz) and 18f26k22 (type2 max frq of 16mhz)
'                Assumes that testing the ChipMaxMHz >= 48 is a valid test for type2 microcontrollers
'                Supports IntOsc MaxMhz of >=64 and not 64 ... there may me others true tables
'    25092017 - Revised as follows:
'                Assumes that testing the ChipMaxMHz >= 48 is a valid test for type2 microcontrollers
'               Supports IntOsc MaxMhz of >=48 and not <48 ... there may me others true tables
'    26092017 - Added  NOPs to assist support.  This will cause Extraneous arguments on the line when using MPLAB but we need the information for support.
'               Added [canskip] where appropiate
'    26042018 - Comments tidy up. No functional changes.
'    27052018 - Added 48mhz clock treatment for 18f USB parts for type 104 oscillator
'    07062018 - Added 24mhz  clock treatment for 16f USB parts for type 102 oscillator
'    08062018 - Added 0.0625mhz  clock treatment for 16f USB parts for type 102 oscillator
'    09012018 - Updated type 103 to support 18f25j53 class oscillator
'    19042019 - Updated to remove script message and comment tidyup only




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

#startup InitSys, 80

#samebit SPLLEN, PLLEN
#samebit HFIOFS, IOFS

'Calculate intosc division (needed to sort between 18F2620 and 18F26K20, possibly others)
#script
  SYS_CLOCK_DIV_NEEDED = ChipIntOsc / ChipMHz
  SYS_CLOCK_INT_PLL_USED = False
  If ChipMHz > 16 And ChipIntOsc > 16 Then
    'PLL used
    SYS_CLOCK_DIV_NEEDED = SYS_CLOCK_DIV_NEEDED * 4
    SYS_CLOCK_INT_PLL_USED = True
  End If

#endscript

'********************************************************************************
'System initialisation routine
Sub InitSys

   #Ifdef PIC
       #IFDEF Oneof(CHIP_18F24K40,CHIP_18F25K40,CHIP_18F26K40,CHIP_18F27K40,CHIP_18F45K40,CHIP_18F46K40,CHIP_18F47K40,CHIP_18F65K40,CHIP_18F66K40,CHIP_18LF24K40, CHIP_18LF25K40, CHIP_18LF26K40, CHIP_18LF27K40, CHIP_18LF45K40, CHIP_18LF46K40, CHIP_18LF47K40, CHIP_18LF65K40, CHIP_18LF66K40)
           ' Added (Per  Chip Errata Sheets) to correctly support table reads on specific chips)
           ' Sets NVRAM pointer to Static RAM as default location.
            #Ifdef Var(NVMCON1)
               NVMCON1.7 = 1
               NVMCON1.6 = 0
            #endif
       #ENDIF

    #ENDIF

    #ifdef PIC
        #ifdef Var(OSCCAL)
            'Resolve OSCCAL calibration
            'GCB does not load saved calibration data into OSCCAL register on Baseline PIC. Therefore the FOSC is inaccurate when using internal RC oscillator.
            'Solution:  Add following as the FIRST lines in the INITSYS sub in the System.h file
            'This loads the saved calibration data from the last flash memory location at POR or any time the chip is reset.
            #ifdef  chipfamily 14
                 CALL 0x3ff
            #endif
            movwf osccal
        #endif
    #endif


  #IFNDEF Var(OSCCON)
      #ifdef PIC
     'Set up internal oscillator
     'Handle OSCCON1 register for parts that have this register
      #endif
      #IFDEF Var(OSCCON1)

        asm showdebug OSCCON type is 100 'This is the routine to support OSCCON1 config addresss
        OSCCON1 = 0x60 ' NOSC HFINTOSC; NDIV 1 - Common as this simply sets the HFINTOSC

        OSCCON3 = 0x00 ' CSWHOLD may proceed; SOSCPWR Low power

        OSCEN = 0x00   ' MFOEN disabled; LFOEN disabled; ADOEN disabled; SOSCEN disabled; EXTOEN disabled; HFOEN disabled

        OSCTUNE = 0x00 ' HFTUN 0



         #IFDEF ChipFamily 16
            'The MCU is a ChipFamily16
           'Section support many MCUs, 18FxxK40, 18FxxK42 etc etc all have NDIV3 bit
            #IFDEF Bit(NDIV3)

                asm showdebug OSCCON type is 101 ' ChipFamily16 and NDIV3 bit

                'Clear NDIV3:0
                NDIV3 = 0
                NDIV2 = 0
                NDIV1 = 0
                NDIV0 = 0

                #IFDEF ChipMHz 64       'No Div
                   OSCFRQ = 0b00001000  '64mhz
                #Endif

                #IFDEF ChipMHz 48        'No Div
                   OSCFRQ = 0b00000111   '48mhz
                #Endif

                #IFDEF ChipMHz 32        'No Div
                   OSCFRQ = 0b00000110   '32mhz
                #Endif

                #IFDEF ChipMHz 24        '48Mhz / 2
                   OSCFRQ = 0b00000111   '24mhz
                   NDIV0 = 1
                #Endif

                #IFDEF ChipMHz 16        'No Div
                   OSCFRQ = 0b00000101   '16mhz
                #Endif

                #IFDEF ChipMHz 12        'No Div
                   OSCFRQ = 0b00000100   '12mhz
                #Endif

                #IFDEF ChipMHz 8          'No Div
                   OSCFRQ = 0b00000011    '8mhz
                #Endif

                #IFDEF ChipMHz 6          '12 Mhz / 2
                   OSCFRQ = 0b00000100    '6mhz
                   NDIV0 = 1
                #Endif

                #IFDEF ChipMHz 4          'No Div
                   OSCFRQ = 0b00000010    '4mhz
                #Endif

                #IFDEF ChipMHz 3          '12mhz / 4
                     OSCFRQ = 0b00000100  '3mhz
                     NDIV1 = 1
                #Endif

                #IFDEF ChipMHz 2         'No DIV
                   OSCFRQ = 0b00000001   '2mhz
                #Endif

               #IFDEF ChipMHz 1          'No Div
                   OSCFRQ = 0b00000000   '1mhz
               #Endif

                #IFDEF ChipMHz 0.5       '1MHz / 2
                    OSCFRQ = 0b00000000  '0.5mhz
                    NDIV0 = 1
                #ENDIF

                #IFDEF ChipMHz 0.25     '1MHZ / 4
                    OSCFRQ = 0b00000000 '0.25mhz
                    NDIV1 = 1
                #ENDIF

                #IFDEF ChipMHz 0.125   '1 MHz / 8
                  OSCFRQ = 0b00000000  '0.125mhz
                  NDIV0 = 1
                  NDIV1 = 1
                #ENDIF


            #ENDIF

         #Endif

         #IFNDEF CHIPFamily 16
                asm showdebug The MCU is a chip family ChipFamily

                asm showdebug OSCCON type is 102

                #IFDEF ChipMHz 32
                  #IFDEF Var(OSCSTAT)
                     'Set OSCFRQ values for MCUs with OSCSTAT... the 16F18855 MCU family
                     OSCFRQ = 0b00000110
                  #ENDIF

                  #IFDEF Var(OSCSTAT1)
                    'Set OSCFRQ values for MCUs with OSCSTAT1 chip... the 16F18326/18346 MCU family
                    OSCFRQ = 0b00000111
                  #ENDIF

                #ENDIF

                #IFDEF ChipMHz 24
                  asm showdebug 24hz
                  #IFDEF Var(OSCSTAT1)
                    OSCFRQ = 0b00000101
                    NOSC0 = 0
                    NOSC1 = 0
                    NOSC2 = 0
                  #ENDIF
                  #IFDEF Var(OSCSTAT)
                    OSCFRQ = 0b00000100
                    NOSC0 = 1
                    NOSC1 = 0
                    NOSC2 = 0
                  #ENDIF
                #ENDIF

                #IFDEF ChipMHz 16
                  #IFDEF Var(OSCSTAT)
                    OSCFRQ = 0b00000101
                  #ENDIF
                  #IFDEF Var(OSCSTAT1)
                    OSCFRQ = 0b00000110
                  #ENDIF
                #ENDIF

                #IFDEF ChipMHz 12
                  #IFDEF Var(OSCSTAT1)
                    OSCFRQ = 0b00000101
                  #ENDIF
                #ENDIF

                #IFDEF ChipMHz 8
                  #IFDEF Var(OSCSTAT)
                    OSCFRQ = 0b00000011
                  #ENDIF
                  #IFDEF Var(OSCSTAT1)
                    OSCFRQ = 0b00000100
                  #ENDIF
                #ENDIF

                #IFDEF ChipMHz 4
                  #IFDEF Var(OSCSTAT)
                    OSCFRQ = 0b00000010
                  #ENDIF
                  #IFDEF Var(OSCSTAT1)
                    OSCFRQ = 0b00000011
                  #ENDIF
                #ENDIF

                #IFDEF ChipMHz 2
                  #IFDEF Var(OSCSTAT)
                    OSCFRQ = 0b00000001
                  #ENDIF
                  #IFDEF Var(OSCSTAT1)
                    OSCFRQ = 0b00000001
                  #ENDIF
                #ENDIF

                #IFDEF ChipMHz 1
                    OSCFRQ = 0b00000000
                #ENDIF

                #IFDEF ChipMHz 0.5
                    OSCFRQ = 0b00000000
                    OSCCON1 = OSCCON1 OR 0b00000001
                #ENDIF

                #IFDEF ChipMHz 0.25
                    OSCFRQ = 0b00000000
                    OSCCON1 = OSCCON1 OR 0b00000010
                #ENDIF

                #IFDEF ChipMHz 0.125
                  OSCFRQ = 0b00000000
                  OSCCON1 = OSCCON1 OR 0b00000011
                #ENDIF

                #IFDEF ChipMHz 0.0625
                  OSCFRQ = 0b00000000
                  OSCCON1 = OSCCON1 OR 0b00000100
                #ENDIF

          #Endif
      #ENDIF
  #ENDIF


  #IFDEF Var(OSCCON)

    #IFDEF Bit(FOSC4)
    ' This is the routine for the OSCCON config
      Set FOSC4 Off
    #ENDIF

    #if NoBit(SPLLEN) And NoBit(IRCF3) Or Bit(INTSRC)

      #ifndef Bit(HFIOFS)

        asm showdebug 'OSCCON type is 103 - This part does not have Bit HFIOFS @ ifndef Bit(HFIOFS)

        #IFDEF SYS_CLOCK_DIV_NEEDED 1
          'added for 18F(L)K20 -WMR
          OSCCON = OSCCON OR b'01110000'
        #ENDIF

        #IFDEF SYS_CLOCK_DIV_NEEDED 2
          OSCCON = OSCCON AND b'10001111'
          OSCCON = OSCCON OR  b'01100000'
        #ENDIF

        #IFDEF SYS_CLOCK_DIV_NEEDED 4
          OSCCON = OSCCON AND b'10001111'
          OSCCON = OSCCON OR  b'01010000'
        #ENDIF

        #IFDEF SYS_CLOCK_DIV_NEEDED 8
          OSCCON = OSCCON AND b'10001111'
          OSCCON = OSCCON OR  b'01000000'
        #ENDIF

        #IFDEF SYS_CLOCK_DIV_NEEDED 16
          OSCCON = OSCCON AND b'10001111'
          OSCCON = OSCCON OR  b'00110000'
        #ENDIF

        #IFDEF SYS_CLOCK_DIV_NEEDED 32
          OSCCON = OSCCON AND b'10001111'
          OSCCON = OSCCON OR  b'00100000'
        #ENDIF

        #IFDEF SYS_CLOCK_DIV_NEEDED 64
          OSCCON = OSCCON AND b'10001111'
          OSCCON = OSCCON OR  b'00010000'
        #ENDIF


        #if SYS_CLOCK_INT_PLL_USED
          'PLL for higher speeds
          [canskip] PLLMULT, SPLLEN = b'11'
        #endif



        #IF SYS_CLOCK_DIV_NEEDED FALSE
            #IFDEF ChipMHz 8
              OSCCON = OSCCON AND b'10001111'
              'Address the two true tables for IRCF
             [canskip] IRCF2, IRCF1, IRCF0 = b'111'    ;111 = 8 MHz (INTOSC drives clock directly)
            #ENDIF

            #IFDEF ChipMHz 4
              OSCCON = OSCCON AND b'10001111'
             [canskip] IRCF2, IRCF1, IRCF0 = b'110'    ;110 = 4 MHz
            #ENDIF

            #IFDEF ChipMHz 2
              OSCCON = OSCCON AND b'10001111'
             [canskip] IRCF2, IRCF1, IRCF0 = b'101'    ;101 = 2 MHz
            #ENDIF

            #IFDEF ChipMHz 1
              OSCCON = OSCCON AND b'10001111'
             [canskip] IRCF2, IRCF1, IRCF0 = b'100'         ;100 = 1 MHz(3)
            #ENDIF

            #IFDEF ChipMHz 0.5
              OSCCON = OSCCON AND b'10001111'
             [canskip] IRCF2, IRCF1, IRCF0 = b'011'         ;011 = 500 kHz
            #ENDIF

            #IFDEF ChipMHz 0.25
              OSCCON = OSCCON AND b'10001111'
             [canskip] IRCF2, IRCF1, IRCF0 = b'010'         ;010 = 250 kHz
            #ENDIF

            #IFDEF ChipMHz 0.125
              OSCCON = OSCCON AND b'10001111'
             [canskip] IRCF2, IRCF1, IRCF0 = b'001'         ;001 = 125 kHz
            #ENDIF
        #ENDIF


        'End of type 103 init

      #endif


      #ifdef Bit(HFIOFS)
        'The section now handles two true tables for frequency
        'Supports 16f and 18f (type1 max frq of 8mhz) classes and 18f (type2 max frq of 16mhz) classes
        'Assumes that testing the ChipMaxMHz >= 48 is a valid test for type2 microcontrollers
        'Supports IntOsc MaxMhz of 64 and not 64 ... there may be others true tables that GCB needs to support in the future

        asm showdebug OSCCON type is 104' NoBit(SPLLEN) And NoBit(IRCF3) Or Bit(INTSRC)) and ifdef Bit(HFIOFS)

        #IFDEF ChipMHz 64 'the SPLLEN needs to set after the IRCF
            [canskip] IRCF2, IRCF1, IRCF0, SPLLEN = b'1111'
            #ifdef Bit(SPLLMULT)
              Set SPLLMULT On
            #endif
        #ENDIF


        #IFDEF ChipMHz 48 'the PLLEN needs to set after the IRCF
          asm showdebug The chip mhz is 48, therefore probably an 18f USB part

          [canskip] IRCF2, IRCF1, IRCF0 = b'111'   ;'111' for ChipMHz 48

          #ifdef Bit(SPLLMULT)
            Set SPLLMULT On
          #endif

          #ifdef Bit(PLLEN)
            Set PLLEN On
          #endif

          'Wait for PLL to stabilize
          #ifdef Bit(PLLRDY)
              wait while (PLLRDY = 0)
          #endif

        #ENDIF



        #IFDEF ChipMHz 32 'the SPLLEN needs to set after the IRCF
          #if ChipIntOSCCONFormat = 1
              [canskip] IRCF2, IRCF1, IRCF0, SPLLEN = b'1101'   ;'1101' with PLL for ChipMHz 32
              #ifdef Bit(SPLLMULT)
                Set SPLLMULT Off
              #endif
          #endif

          #ifndef ChipIntOSCCONFormat
              [canskip] IRCF2, IRCF1, IRCF0, SPLLEN = b'1111'   ;'1111' with PLL for ChipMHz 32
          #endif

        #ENDIF

        #IFDEF ChipMHz 16
          OSCCON = OSCCON OR b'01110000'
          'Address the two true tables for IRCF
          #if ChipIntOSCCONFormat = 1
            [canskip] IRCF2, IRCF1, IRCF0 = b'111'    ;111 = 16 MHz (HFINTOSC drives clock directly)
          #endif

          #ifndef ChipIntOSCCONFormat
             [canskip]IRCF2, IRCF1, IRCF0, SPLLEN = b'1101'    ;110 = 4 MHz with PLL
          #endif

        #ENDIF

        #IFDEF ChipMHz 8
          OSCCON = OSCCON AND b'10001111'
          'Address the two true tables for IRCF
          #if ChipIntOSCCONFormat = 1
             [canskip] IRCF2, IRCF1, IRCF0 = b'110'    ;110 = 8 MHz
          #endif

          #ifndef ChipIntOSCCONFormat
             [canskip] IRCF2, IRCF1, IRCF0 = b'111'    ;111 = 8 MHz (INTOSC drives clock directly)
          #endif
        #ENDIF

        #IFDEF ChipMHz 4
          OSCCON = OSCCON AND b'10001111'
          'Address the two true tables for IRCF
          #if ChipIntOSCCONFormat = 1
             [canskip] IRCF2, IRCF1, IRCF0 = b'101'    ;101 = 4 MHz
          #endif

          #ifndef ChipIntOSCCONFormat
             [canskip] IRCF2, IRCF1, IRCF0 = b'110'    ;110 = 4 MHz
          #endif
        #ENDIF

        #IFDEF ChipMHz 2
          OSCCON = OSCCON AND b'10001111'
          'Address the two true tables for IRCF
          #if ChipIntOSCCONFormat = 1
             [canskip] IRCF2, IRCF1, IRCF0 = b'100'    ;100 = 2 MHz
          #endif

          #ifndef ChipIntOSCCONFormat
             [canskip] IRCF2, IRCF1, IRCF0 = b'101'    ;101 = 2 MHz
          #endif
        #ENDIF

        #IFDEF ChipMHz 1
          OSCCON = OSCCON AND b'10001111'
          'Address the two true tables for IRCF
          #if ChipIntOSCCONFormat = 1
             [canskip] IRCF2, IRCF1, IRCF0 = b'011'          ;011 = 1 MHz(3)
          #endif

          #ifndef ChipIntOSCCONFormat
             [canskip] IRCF2, IRCF1, IRCF0 = b'100'         ;100 = 1 MHz(3)
          #endif
        #ENDIF

        #IFDEF ChipMHz 0.5
          OSCCON = OSCCON AND b'10001111'
          'Address the two true tables for IRCF
          #if ChipIntOSCCONFormat = 1
             [canskip] IRCF2, IRCF1, IRCF0 = b'010'          ;010 = 500 kHz
          #endif

          #ifndef ChipIntOSCCONFormat
             [canskip] IRCF2, IRCF1, IRCF0 = b'011'         ;011 = 500 kHz
          #endif
        #ENDIF

        #IFDEF ChipMHz 0.25
          OSCCON = OSCCON AND b'10001111'
          'Address the two true tables for IRCF
          #if ChipIntOSCCONFormat = 1
             [canskip] IRCF2, IRCF1, IRCF0 = b'001'          ;001 = 250 kHz
          #endif

          #ifndef ChipIntOSCCONFormat
             [canskip] IRCF2, IRCF1, IRCF0 = b'010'         ;010 = 250 kHz
          #endif
        #ENDIF

        #IFDEF ChipMHz 0.125
          OSCCON = OSCCON AND b'10001111'
          #ifndef ChipIntOSCCONFormat
             [canskip] IRCF2, IRCF1, IRCF0 = b'001'         ;001 = 125 kHz
          #endif
        #ENDIF


      #endif
    #endif

    #if Bit(SPLLEN) Or Bit(IRCF3) And NoBit(INTSRC)

      #ifdef Bit(IRCF3)

        asm showdebug OSCCON type is 105 'Bit(SPLLEN) Or Bit(IRCF3) And NoBit(INTSRC) and ifdef Bit(IRCF3)

        #IFDEF ChipMHz 64
          'Same as for 16, assuming 64 MHz clock is 16 MHz x 4
          'OSCCON = OSCCON OR b'01111000'
          Set IRCF3 On
          Set IRCF2 On
          Set IRCF1 On
          Set IRCF0 On
        #ENDIF
        #IFDEF ChipMHz 48
          'Same as for 16, assuming 48 MHz clock is 16 MHz x 3
          'OSCCON = OSCCON OR b'01111000'
          Set IRCF3 On
          Set IRCF2 On
          Set IRCF1 On
          Set IRCF0 On
          #ifdef Bit(SPLLMULT)
            Set SPLLMULT On
          #endif
          #ifdef Bit(SPLLEN)
            Set SPLLEN On
          #endif
        #ENDIF
        #IFDEF ChipMHz 32
          'Same as for 8, assuming 32 MHz clock is 8 MHz x 4
          'OSCCON = OSCCON AND b'10000111'
          'OSCCON = OSCCON OR b'11110000'
          Set IRCF3 On
          Set IRCF2 On
          Set IRCF1 On
          Set IRCF0 Off
          #ifdef Bit(SPLLMULT)
            Set SPLLMULT Off
          #endif
          #ifdef Bit(SPLLEN)
            Set SPLLEN On
          #endif
        #ENDIF
        #IFDEF ChipMHz 24
          'Same as for 8, assuming 24 MHz clock is 8 MHz x 3
          Set IRCF3 On
          Set IRCF2 On
          Set IRCF1 On
          Set IRCF0 Off
          #ifdef Bit(SPLLMULT)
            Set SPLLMULT On
          #endif
          #ifdef Bit(SPLLEN)
            Set SPLLEN On
          #endif
        #ENDIF
        #IFDEF ChipMHz 16
          'OSCCON = OSCCON OR b'01111000'
          Set IRCF3 On
          Set IRCF2 On
          Set IRCF1 On
          Set IRCF0 On
          #ifdef Bit(SPLLEN)
            Set SPLLEN Off
          #endif
        #ENDIF
        #IFDEF ChipMHz 8
          'OSCCON = OSCCON AND b'10000111'
          'OSCCON = OSCCON OR b'01110000'
          Set IRCF3 On
          Set IRCF2 On
          Set IRCF1 On
          Set IRCF0 Off
          #ifdef Bit(SPLLEN)
            Set SPLLEN Off
          #endif
        #ENDIF
        #IFDEF ChipMHz 4
          'OSCCON = OSCCON AND b'10000111'
          'OSCCON = OSCCON OR b'01101000'
          Set IRCF3 On
          Set IRCF2 On
          Set IRCF1 Off
          Set IRCF0 On
          #ifdef Bit(SPLLEN)
            Set SPLLEN Off
          #endif
        #ENDIF
        #IFDEF ChipMHz 2
          'OSCCON = OSCCON AND b'10000111'
          'OSCCON = OSCCON OR b'01100000'
          Set IRCF3 On
          Set IRCF2 On
          Set IRCF1 Off
          Set IRCF0 Off
          #ifdef Bit(SPLLEN)
            Set SPLLEN Off
          #endif
        #ENDIF
        #IFDEF ChipMHz 1
          'OSCCON = OSCCON AND b'10000111'
          'OSCCON = OSCCON OR b'01011000'
          Set IRCF3 On
          Set IRCF2 Off
          Set IRCF1 On
          Set IRCF0 On
          #ifdef Bit(SPLLEN)
            Set SPLLEN Off
          #endif
        #ENDIF
        #IFDEF ChipMHz 0.5
          'OSCCON = OSCCON AND b'10000111'
          'OSCCON = OSCCON OR b'00111000'
          Set IRCF3 Off
          Set IRCF2 On
          Set IRCF1 On
          Set IRCF0 On
          #ifdef Bit(SPLLEN)
            Set SPLLEN Off
          #endif
        #ENDIF
        #IFDEF ChipMHz 0.25
          'OSCCON = OSCCON AND b'10000111'
          'OSCCON = OSCCON OR b'00110000'
          Set IRCF3 Off
          Set IRCF2 On
          Set IRCF1 On
          Set IRCF0 Off
          #ifdef Bit(SPLLEN)
            Set SPLLEN Off
          #endif
        #ENDIF
        #IFDEF ChipMHz 0.125
          'OSCCON = OSCCON AND b'10000111'
          'OSCCON = OSCCON OR b'00101000'
          Set IRCF3 Off
          Set IRCF2 On
          Set IRCF1 Off
          Set IRCF0 On
          #ifdef Bit(SPLLEN)
            Set SPLLEN Off
          #endif
        #ENDIF
      #endif

      #ifndef Bit(IRCF3)

        asm showdebug OSCCON type is  106 'Bit(SPLLEN) Or Bit(IRCF3) And NoBit(INTSRC) and ifNdef Bit(IRCF3)

        #IFDEF ChipMHz 64
          'OSCCON = OSCCON AND b'10001111'
          'OSCCON = OSCCON OR  b'01100000'
          Set IRCF2 On    '- WMR
          Set IRCF1 On    '- WMR
          Set IRCF0 On    ' -WMR
          #ifdef Bit(SPLLEN)
              Set SPLLEN On
          #endif
        #ENDIF
        #IFDEF ChipMHz 32
          OSCCON = OSCCON AND b'10001111'
          OSCCON = OSCCON OR  b'01100000'
          #ifdef Bit(SPLLEN)
              Set SPLLEN On
          #endif
        #ENDIF

        #IFDEF ChipMHz 16
          OSCCON = OSCCON OR b'01110000'
        #ENDIF
        #IFDEF ChipMHz 8
          OSCCON = OSCCON AND b'10001111'
          OSCCON = OSCCON OR b'01100000'
          #ifdef Bit(SPLLEN)
            Set SPLLEN Off
          #endif
        #ENDIF
        #IFDEF ChipMHz 4
          OSCCON = OSCCON AND b'10001111'
          OSCCON = OSCCON OR b'01010000'
        #ENDIF
        #IFDEF ChipMHz 2
          OSCCON = OSCCON AND b'10001111'
          OSCCON = OSCCON OR b'01000000'
        #ENDIF
        #IFDEF ChipMHz 1
          OSCCON = OSCCON AND b'10001111'
          OSCCON = OSCCON OR b'00110000'
        #ENDIF
        #IFDEF ChipMHz 0.5
          OSCCON = OSCCON AND b'10001111'
          OSCCON = OSCCON OR b'00100000'
        #ENDIF
        #IFDEF ChipMHz 0.25
          OSCCON = OSCCON AND b'10001111'
          OSCCON = OSCCON OR b'00010000'
        #ENDIF
        #IFDEF ChipMHz 0.031
          OSCCON = OSCCON AND b'10001111'
        #ENDIF
      #endif
    #endif

  #ENDIF

  #IFDEF ChipFamily 16
    'Clear BSR on ChipFamily16 MCUs
    BSR = 0
  #ENDIF

  #IFDEF Var(TBLPTRU)
    'Clear TBLPTRU on MCUs with this bit as this must be zero
    TBLPTRU = 0
  #ENDIF


  #IF Var(ADCON0) OR Var(ADCON)
    'Ensure all ports are set for digital I/O and, turn off A/D
    #IFDEF Bit(ADFM)
      SET ADFM OFF
    #ENDIF

    #ifdef NoVar(ADCON0)
      'Switch off A/D with NoVar(ADCON0)
      Set ADCON.ADON Off
    #endif
    #IFDEF Var(ADCON0)
      'Switch off A/D Var(ADCON0)
      SET ADCON0.ADON OFF
      #IF NoVar(ANSEL) AND NoVar(ANSELA) AND NoVar(ANSEL0)
        #IFDEF NoBit(PCFG4)
          #IFDEF NoVar(ADCON2)
            #IFDEF NoBit(ANS0)
              [canskip] PCFG3, PCFG2, PCFG1, PCFG0 = b'0110'
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


        #IFDEF Bit(PCFG4)
          'For 18F1320, which uses ADCON1 as an ANSEL register
          ADCON1 = 0
        #ENDIF
      #ENDIF
    #ENDIF

    'Commence clearing any ANSEL variants in the part
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
    'End clearing any ANSEL variants in the part
  #ENDIF


  #IFDEF VAR(ANCON0)
   'For 18f devices
    #IFDEF BIT(ANSEL0)
      Set ANSEL0 off
    #ENDIF
    #IFDEF BIT(ANSEL1)
      Set ANSEL1 off
    #ENDIF
    #IFDEF BIT(ANSEL2)
      Set ANSEL2 off
    #ENDIF
    #IFDEF BIT(ANSEL3)
      Set ANSEL3 off
    #ENDIF
    #IFDEF BIT(ANSEL4)
      Set ANSEL4 off
    #ENDIF
    #IFDEF BIT(ANSEL5)
      Set ANSEL5 off
    #ENDIF
    #IFDEF BIT(ANSEL6)
      Set ANSEL6 off
    #ENDIF
    #IFDEF BIT(ANSEL7)
      Set ANSEL7 off
    #ENDIF

  #ENDIF

  #IFDEF VAR(ANCON1)
    #IFDEF BIT(ANSEL8)
      Set ANSEL8 off
    #ENDIF
    #IFDEF BIT(ANSEL9)
      Set ANSEL9 off
    #ENDIF
    #IFDEF BIT(ANSEL10)
      Set ANSEL10 off
    #ENDIF
    #IFDEF BIT(ANSEL11)
      Set ANSEL11 off
    #ENDIF
    #IFDEF BIT(ANSEL12)
      Set ANSEL12 off
    #ENDIF
    #IFDEF BIT(ANSEL13)
      Set ANSEL13 off
    #ENDIF
    #IFDEF BIT(ANSEL14)
      Set ANSEL14 off
    #ENDIF
    #IFDEF BIT(ANSEL15)
      Set ANSEL15 off
    #ENDIF

  #ENDIF


  #IFDEF Var(CMCON)

    #IFNDEF BIT(CMEN0)
      'Turn off comparator - this is the default setting
      CMCON = 7
    #ENDIF

    #IFDEF BIT(CMEN0)
      'Applied for following MCU types 18f1xxx
      CMEN0 = 0
      CMEN1 = 0
      CMEN2 = 0
    #ENDIF

  #ENDIF
  #IFDEF Var(CMCON0)
    CMCON0 = 7
  #ENDIF

  #IFDEF Var(CM1CON0)
    'Set comparator register bits for many MCUs with register CM2CON0
    #IFDEF Var(CM2CON0)
      #IFDEF bit(C2ON)
        C2ON = 0
      #ENDIF
      #IFDEF bit(C2EN)
        C2EN = 0
      #ENDIF
    #ENDIF

    #IFDEF bit(C1ON)
      C1ON = 0
    #ENDIF
    #IFDEF bit(C1EN)
      C1EN = 0
    #ENDIF

  #ENDIF


  #IFDEF ChipFamily 12
    'The MCU is a chip family ChipNameStr, so, set GPIO.2 to digital by clear T0CS bit
    #IFDEF Bit(T0CS)
      movlw b'11000111'
      option
      option_reg = b'11000111'
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
' SysStringA = Source string
' SysStringB = Dest string
'On extended 14 and 16 bit:
' FSR0 = Source string
' FSR1 = Dest string

sub SysCopyString

 Dim SysCalcTempA As Byte
 Dim SysStringLength As Byte

#IFDEF ChipFamily 12,14

 Dim SysCalcTempB As Byte

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

 Dim SysReadA As Byte

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
' SysStringA = Source address
' SysStringB = Dest string
'On enhanced 14 bit:
' SysStringA = Source address
' FSR1 = Dest string
'On 16 bit:
' TBLPTRL/TBLPTRH = Source string
' FSR1 = Dest string

sub SysReadString

  Dim SysCalcTempA As Byte
  Dim SysStringLength As Byte

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
      addfsr 1,1
      movwf INDF1

    decfsz SysCalcTempA, F
    goto SysStringRead

  #endif

 #ifdef ChipFamily 16

  'Get length
  TBLRD*+
  movff TABLAT,SysCalcTempA
  movff TABLAT,INDF1
  goto SysStringReadCheck

SysReadStringPart:
  TBLRD*+
  movf TABLAT, W
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

   'Copy char
   TBLRD*+
   movff TABLAT,PREINC1

  decfsz SysCalcTempA, F
  goto SysStringRead

 #endif

 #IFDEF AVR
  Dim SysCalcTempX As Byte

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

 Dim SysByteTempA As Byte
 Dim SysByteTempX As Byte

 SysByteTempX = 0

#IFDEF ChipFamily 12,14

 Dim SysByteTempB As Byte

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
 movwf SysByteTempA

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
 subwf SysByteTempA, W
 btfss STATUS, Z
 return
 'Exit true if length = 0
 movf SysByteTempA, F
 btfsc STATUS, Z
 goto SCEStrTrue

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
  movwf SysByteTempB

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
  subwf SysByteTempB, W
  btfss STATUS, Z
  return

 decfsz SysByteTempA, F
 goto SysStringComp

SCEStrTrue:
 movlw TRUE
 movwf SysByteTempX

#ENDIF

  #ifdef ChipFamily 15
    'Check length matches
    movf INDF0, W
    movwf SysByteTempA
    subwf INDF1, W
    btfss STATUS, Z
    return
    'Check if empty
    movf SysByteTempA, F
    btfsc STATUS, Z
    goto SCEStrTrue

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

    decfsz SysByteTempA, F
    goto SysStringComp

    SCEStrTrue:
    comf SysByteTempX, F
  #endif

#IFDEF ChipFamily 16

 'Check length matches
 movf INDF0, W
 cpfseq POSTINC1
 return
 'Check if empty
 movf INDF0, F
 bz SCEStrTrue

 'Check each char, exit if not equal
 movff POSTINC0, SysByteTempA
SysStringComp:

 'Compare, exit if <>
 movf POSTINC0, W
 cpfseq POSTINC1
 return

 decfsz SysByteTempA, F
 goto SysStringComp

SCEStrTrue:
 setf SysByteTempX
#ENDIF

#IFDEF AVR

  Dim SysReadA As Word
  Dim SysByteTempA As Byte

  SysByteTempX = 0

 'Check length matches
 ld SysReadA, X+
 ld SysReadA_H, Y+
 cpse SysReadA, SysReadA_H
 ret
 mov SysByteTempA, SysReadA
 'Check if length 0
 tst SysReadA
 breq SCEStrTrue

 'Check each char, exit if not equal
 SysStringComp:

 'Compare, exit if <>
 ld SysReadA, X+
 ld SysReadA_H, Y+
 cpse SysReadA, SysReadA_H
 ret

 dec SysByteTempA
 brne SysStringComp

SCEStrTrue:
 com SysByteTempX
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

Sub SysConvIntegerToSingle
  'Return correct value for 0
  If SysIntegerTemp = 0 Then
    SysSingleTemp = 0
    Exit Sub
  End If

  'Get sign, make integer positive
  SysSingleTemp.0 = 0
  If SysIntegerTemp.15 Then
    SysIntegerTemp = -SysIntegerTemp
    SysSingleTemp.0 = 1
  End If

  'Rotate left until there is an invisible leading 1
  SysSingleTemp_E = 127 + 15
  Do While SysIntegerTemp.15 = 0
    Set C Off
    Rotate SysIntegerTemp Left
    SysSingleTemp_E -= 1
  Loop

  Set C Off
  If SysSingleTemp.0 Then
    Set C On
  End If
  Rotate SysSingleTemp_E Right
  SysIntegerTemp_H.7 = C
  SysSingleTemp_U = SysIntegerTemp_H
  SysSingleTemp_H = SysIntegerTemp
  [byte]SysSingleTemp = 0
End Sub

Sub SysConvIntegerToDouble
  Dim SysCalcExpA As Word Alias SysDoubleTemp_D, SysDoubleTemp_C

  'Return correct value for 0
  If SysIntegerTemp = 0 Then
    SysDoubleTemp = 0
    Exit Sub
  End If

  'Get sign, make integer positive
  SysDoubleTemp.0 = 0
  If SysIntegerTemp.15 Then
    SysIntegerTemp = -SysIntegerTemp
    SysDoubleTemp.0 = 1
  End If

  'Rotate left until there is an invisible leading 1
  SysCalcExpA = 1023 + 15
  Do While SysIntegerTemp.15 = 0
    Set C Off
    Rotate SysIntegerTemp Left
    SysCalcExpA -= 1
  Loop

  'Set exponent and sign
  Repeat 4
    Set C Off
    Rotate SysCalcExpA Left
  End Repeat
  SysDoubleTemp_D.7 = SysDoubleTemp.0

  'Set mantissa
  SysDoubleTemp_A = 0
  SysIntegerTemp.15 = 0
  Repeat 3
    Set C Off
    Rotate SysIntegerTemp Right
    Rotate SysDoubleTemp_A Right
  End Repeat
  SysDoubleTemp_C = SysDoubleTemp_C Or SysIntegerTemp_H
  SysDoubleTemp_B = SysIntegerTemp
  'A set above
  SysDoubleTemp_E = 0
  SysDoubleTemp_U = 0
  SysDoubleTemp_H = 0
  [byte]SysDoubleTemp = 0
End Sub

Sub SysConvLongToSingle
  'Return correct value for 0
  If SysLongTemp = 0 Then
    SysSingleTemp = 0
    Exit Sub
  End If

  'Rotate left until there is an invisible leading 1
  SysSingleTemp_E = 127 + 31
  Do While SysLongTemp.31 = 0
    Set C Off
    Rotate SysLongTemp Left
    SysSingleTemp_E -= 1
  Loop

  Set C Off
  Rotate SysSingleTemp_E Right
  SysLongTemp_E.7 = C
  SysSingleTemp_U = SysLongTemp_E
  SysSingleTemp_H = SysLongTemp_U
  [byte]SysSingleTemp = SysLongTemp_H

End Sub

Sub SysConvLongToDouble
  Dim SysCalcExpA As Word Alias SysDoubleTemp_D, SysDoubleTemp_C

  'Return correct value for 0
  If SysLongTemp = 0 Then
    SysDoubleTemp = 0
    Exit Sub
  End If

  'Rotate left until there is an invisible leading 1
  SysCalcExpA = 1023 + 31
  Do While SysLongTemp.31 = 0
    Set C Off
    Rotate SysLongTemp Left
    SysCalcExpA -= 1
  Loop

  'Set exponent and sign
  Repeat 4
    Set C Off
    Rotate SysCalcExpA Left
  End Repeat
  SysDoubleTemp_D.7 = 0
  'Set mantissa
  SysDoubleTemp_U = 0
  Repeat 3
    Set C Off
    Rotate SysLongTemp Right
    Rotate SysDoubleTemp_U Right
  End Repeat
  SysDoubleTemp_C = SysDoubleTemp_C Or SysLongTemp_E
  SysDoubleTemp_B = SysLongTemp_U
  SysDoubleTemp_A = SysLongTemp_H
  SysDoubleTemp_E = [byte]SysLongTemp
  'U set above
  SysDoubleTemp_H = 0
  [byte]SysDoubleTemp = 0

End Sub

Sub SysConvSingleToInteger
  'Get mantissa
  SysIntegerTemp_H = SysSingleTemp_U
  [byte]SysIntegerTemp = SysSingleTemp_H
  Set SysIntegerTemp_H.7 On
  Set C Off
  Rotate SysIntegerTemp Right

  'Get exponent
  SysSingleTemp_H.7 = SysSingleTemp_E.7 'Save sign bit
  Rotate SysSingleTemp_U Left
  Rotate SysSingleTemp_E Left
  Do While SysSingleTemp_E > 141
    Set C Off
    Rotate SysIntegerTemp Left
    SysSingleTemp_E -= 1
  Loop
  Do While SysSingleTemp_E < 141
    Set C Off
    Rotate SysIntegerTemp Right
    SysSingleTemp_E += 1
  Loop

  If SysSingleTemp_H.7 Then
    SysIntegerTemp = -SysIntegerTemp
  End If
End Sub

Sub SysConvSingleToLong

  'Get mantissa
  SysLongTemp_E = 0
  SysLongTemp_U = SysSingleTemp_U
  SysLongTemp_H = SysSingleTemp_H
  [byte]SysLongTemp = [byte]SysSingleTemp
  Set SysLongTemp_U.7 On

  'Get exponent
  Rotate SysSingleTemp_U Left
  Rotate SysSingleTemp_E Left
  Do While SysSingleTemp_E > 150
    Set C Off
    Rotate SysLongTemp Left
    SysSingleTemp_E -= 1
  Loop
  Do While SysSingleTemp_E < 150
    Set C Off
    Rotate SysLongTemp Right
    SysSingleTemp_E += 1
  Loop

End Sub

Sub SysConvSingleToDouble
  Dim SysCalcExpA As Word Alias SysDoubleTemp_D, SysDoubleTemp_C
  Dim SysCalcMantA As Long

  'Get exponent and mantissa
  SysCalcMantA_E = SysSingleTemp_U
  Set SysCalcMantA_E.7 Off
  SysCalcMantA_U = SysSingleTemp_H
  SysCalcMantA_H = [byte]SysSingleTemp
  [byte]SysCalcMantA = 0
  SysCalcExpA = SysSingleTemp_E
  Rotate SysSingleTemp_U Left
  Rotate [byte]SysCalcExpA Left

  'Rotate mantissa right 3, add (1023-127) to exponent if not 0
  Repeat 3
    Set C Off
    Rotate SysCalcMantA Right
  End Repeat
  If [byte]SysCalcExpA <> 0 Then
    SysCalcExpA += 896
  End If

  'Put into double
  Repeat 4
    Set C off
    Rotate SysCalcExpA Left
  End Repeat
  SysCalcExpA.15 = SysSingleTemp_E.7
  SysDoubleTemp_C += SysCalcMantA_E
  SysDoubleTemp_B = SysCalcMantA_U
  SysDoubleTemp_A = SysCalcMantA_H
  SysDoubleTemp_E = SysCalcMantA
  SysDoubleTemp_U = 0
  SysDoubleTemp_H = 0
  [byte]SysDoubleTemp = 0

End Sub

Sub SysConvDoubleToInteger
  Dim SysCalcExpA As Word Alias SysDoubleTemp_D, SysDoubleTemp_C

  'Get mantissa
  SysIntegerTemp_H = SysDoubleTemp_C And 0x0F
  [byte]SysIntegerTemp = SysDoubleTemp_B
  Set SysIntegerTemp_H.4 On
  'A will also hold some of it, and need to set high bit

  'Get exponent
  SysDoubleTemp_H.7 = SysDoubleTemp_D.7 'Save sign bit
  SysDoubleTemp_D.7 = 0
  'Get exponent
  Repeat 4
    Set C Off
    Rotate SysCalcExpA Right
  End Repeat

  Do While SysCalcExpA > 1035
    Set C Off
    Rotate SysDoubleTemp_A Left
    Rotate SysIntegerTemp Left
    SysCalcExpA -= 1
  Loop
  Do While SysCalcExpA < 1035
    Set C Off
    Rotate SysIntegerTemp Right
    SysCalcExpA += 1
  Loop

  If SysDoubleTemp_H.7 Then
    SysIntegerTemp = -SysIntegerTemp
  End If
End Sub

Sub SysConvDoubleToLong
  Dim SysCalcExpA As Word Alias SysDoubleTemp_D, SysDoubleTemp_C

  'Get mantissa (split between output var, and with extra bits in SysCalcMantA)
  'This has essentially been shifted right 3 into SysCalcMantA
  SysLongTemp_E = SysDoubleTemp_C And 0x0F
  SysLongTemp_U = SysDoubleTemp_B
  SysLongTemp_H = SysDoubleTemp_A
  [byte]SysLongTemp = SysDoubleTemp_E
  SysCalcMantA = SysDoubleTemp_U
  Set SysLongTemp_E.4 On

  'Get exponent
  Repeat 4
    Set C Off
    Rotate SysCalcExpA Right
  End Repeat

  '1071 = bias + fraction size + current rotation = 1023 + 24 + 3
  Do While SysCalcExpA > 1051
    Set C Off
    Rotate SysCalcMantA Left
    Rotate SysLongTemp Left
    SysCalcExpA -= 1
  Loop
  Do While SysCalcExpA < 1051
    Set C Off
    Rotate SysLongTemp Right
    SysCalcExpA += 1
  Loop
End Sub

Sub SysConvDoubleToSingle
  Dim SysCalcExpA As Word Alias SysDoubleTemp_D, SysDoubleTemp_C
  Dim SysCalcMantA As Long

  'Get mantissa, shift left 4
  '(Need to shift left 3, but it gets shifted right 1 below)
  SysCalcMantA_E = SysDoubleTemp_C
  SysCalcMantA_U = SysDoubleTemp_B
  SysCalcMantA_H = SysDoubleTemp_A
  [byte]SysCalcMantA = SysDoubleTemp_E
  Repeat 4
    Set C Off
    Rotate SysCalcMantA Left
  End Repeat
  'Move sign bit to soon to be discarded low byte of mantissa
  SysCalcMantA.0 = SysDoubleTemp_D.7
  SysDoubleTemp_D.7 = 0
  'Get exponent, subtract bias
  Repeat 4
    Set C Off
    Rotate SysCalcExpA Right
  End Repeat
  If SysCalcExpA <> 0 Then
    SysCalcExpA -= 896
  End If
  'If exponent is over 8 bits, an overflow is about to occur. Detect this?

  'Put into single
  SysSingleTemp_E = [byte]SysCalcExpA
  SysSingleTemp_U = SysCalcMantA_E
  SysSingleTemp_H = SysCalcMantA_U
  [byte]SysSingleTemp = SysCalcMantA_H
  C = SysCalcMantA.0
  Rotate [long]SysSingleTemp Right

End Sub

'********************************************************************************
'Addition subroutines

Sub SysAddSubSingle
  'Aliases to do actual addition
  Dim SysCalcSingMantA As Long Alias SysSingleTempA_E, SysSingleTempA_U, SysSingleTempA_H, SysSingleTempA
  Dim SysCalcSingMantB As Long Alias SysSingleTempB_E, SysSingleTempB_U, SysSingleTempB_H, SysSingleTempB

  'Get signs (Put in X_H.7 and 6)
  SysSingleTempX_H.7 = SysSingleTempA_E.7
  SysSingleTempX_H.6 = SysSingleTempB_E.7

  'Get exponents (Put in X_E and X_U)
  SysSingleTempX_E = SysSingleTempA_E
  C = SysSingleTempA_U.7
  Rotate SysSingleTempX_E Left
  SysSingleTempX_U = SysSingleTempB_E
  C = SysSingleTempB_U.7
  Rotate SysSingleTempX_U Left

  'Prepare mantissa aliases
  SysCalcSingMantA_E = 0
  SysCalcSingMantA_U.7 = 1
  SysCalcSingMantB_E = 0
  SysCalcSingMantB_U.7 = 1

  'Ensure same exponent used for both aliases
  Do While SysSingleTempX_E > SysSingleTempX_U
    'While A has higher exponent, make B's bigger by shifting it right
    Set C Off
    Rotate SysCalcSingMantB Right
    SysSingleTempX_U += 1
  Loop
  Do While SysSingleTempX_E < SysSingleTempX_U
    'While B has higher exponent, make A's bigger by shifting it right
    Set C Off
    Rotate SysCalcSingMantA Right
    SysSingleTempX_E += 1
  Loop
  'Negate
  If SysSingleTempX_H.7 Then
    SysCalcSingMantA = Not SysCalcSingMantA
    SysCalcSingMantA += 1
  End If
  If SysSingleTempX_H.6 Then
    SysCalcSingMantB = Not SysCalcSingMantB
    SysCalcSingMantB += 1
  End If

  'Add
  SysCalcSingMantA = SysCalcSingMantA + SysCalcSingMantB

  'Negative? (store in B.31)
  SysCalcSingMantB.31 = 0
  If SysCalcSingMantA.31 Then
    'Must have positive mantissa
    SysCalcSingMantA = Not SysCalcSingMantA
    SysCalcSingMantA += 1
    SysCalcSingMantB.31 = 1
  End If

  'Normalise result (shifted 8 bits left)
  SysSingleTempX_E += 8
  Do While SysCalcSingMantA.31 <> 1
    Set C Off
    Rotate SysCalcSingMantA Left
    SysSingleTempX_E -= 1
  Loop

  'Set result
  SysSingleTempX_U = SysSingleTempA_E
  SysSingleTempX_H = SysSingleTempA_U
  [byte]SysSingleTempX = SysSingleTempA_H
  C = SysCalcSingMantB.31
  Rotate SysSingleTempX_E Right
  SysSingleTempX_U.7 = C

End Sub

Sub SysAddSubDouble
  'Aliases to do actual addition
  Dim SysCalcDoubMantA As LongInt Alias SysDoubleTempA_D, SysDoubleTempA_C, SysDoubleTempA_B, SysDoubleTempA_A, SysDoubleTempA_E, SysDoubleTempA_U, SysDoubleTempA_H, SysDoubleTempA
  Dim SysCalcDoubMantB As LongInt Alias SysDoubleTempB_D, SysDoubleTempB_C, SysDoubleTempB_B, SysDoubleTempB_A, SysDoubleTempB_E, SysDoubleTempB_U, SysDoubleTempB_H, SysDoubleTempB

  Dim SysCalcDoubExpA As Word Alias SysDoubleTempX_D, SysDoubleTempX_C
  Dim SysCalcDoubExpB As Word Alias SysDoubleTempX_B, SysDoubleTempX_A

  'Get signs (Put in X_H.7 and 6)
  SysDoubleTempX_H.7 = SysDoubleTempA_D.7
  SysDoubleTempX_H.6 = SysDoubleTempB_D.7

  'Get exponents (Put in X_DC and X_BA)
  SysCalcDoubExpA_H = SysDoubleTempA_D
  [byte]SysCalcDoubExpA = SysDoubleTempA_C
  SysCalcDoubExpB_H = SysDoubleTempB_D
  [byte]SysCalcDoubExpB = SysDoubleTempB_C
  Repeat 4
    Set C Off
    Rotate SysCalcDoubExpA Right
  End Repeat
  SysCalcDoubExpA.11 = 0
  Repeat 4
    Set C Off
    Rotate SysCalcDoubExpB Right
  End Repeat
  SysCalcDoubExpB.11 = 0

  'Prepare mantissa aliases
  SysCalcDoubMantA_D = 0
  SysCalcDoubMantA_C = SysCalcDoubMantA_C And 15
  SysCalcDoubMantA_C.4 = 1
  SysCalcDoubMantB_D = 0
  SysCalcDoubMantB_C = SysCalcDoubMantB_C And 15
  SysCalcDoubMantB_C.4 = 1

  'Ensure same exponent used for both aliases
  Do While SysCalcDoubExpA > SysCalcDoubExpB
    'While A has higher exponent, make B's bigger by shifting it right
    Set C Off
    Rotate SysCalcDoubMantB Right
    SysCalcDoubExpB += 1
  Loop
  Do While SysCalcDoubExpA < SysCalcDoubExpB
    'While B has higher exponent, make A's bigger by shifting it right
    Set C Off
    Rotate SysCalcDoubMantA Right
    SysCalcDoubExpA += 1
  Loop
  'Negate
  If SysDoubleTempX_H.7 Then
    SysCalcDoubMantA = Not SysCalcDoubMantA
    SysCalcDoubMantA += 1
  End If
  If SysDoubleTempX_H.6 Then
    SysCalcDoubMantB = Not SysCalcDoubMantB
    SysCalcDoubMantB += 1
  End If

  'Add
  SysCalcDoubMantA = SysCalcDoubMantA + SysCalcDoubMantB

  'Negative? (store in B.31)
  SysCalcDoubMantB.63 = 0
  If SysCalcDoubMantA.63 Then
    'Must have positive mantissa
    SysCalcDoubMantA = Not SysCalcDoubMantA
    SysCalcDoubMantA += 1
    SysCalcDoubMantB.63 = 1
  End If

  'Normalise result (shifted 8 bits left)
  SysDoubleTempX_E += 8
  Do While SysCalcDoubMantA.60 <> 1
    Set C Off
    Rotate SysCalcDoubMantA Left
    SysDoubleTempX_E -= 1
  Loop

  'Set result
  Repeat 4
    Set C Off
    Rotate SysCalcDoubExpA Left
  End Repeat
  SysDoubleTempX_D.7 = SysCalcDoubMantB.63
  SysDoubleTempX_C = SysDoubleTempX_C Or SysCalcDoubMantA_D
  SysDoubleTempX_B = SysCalcDoubMantA_C
  SysDoubleTempX_A = SysCalcDoubMantA_B
  SysDoubleTempX_E = SysCalcDoubMantA_A
  SysDoubleTempX_U = SysCalcDoubMantA_E
  SysDoubleTempX_H = SysCalcDoubMantA_U
  [byte]SysDoubleTempX = SysCalcDoubMantA_H
End Sub

Sub SysSubSubSingle
  'Negate B, call addition sub
  If SysSingleTempB_E.7 Then
    SysSingleTempB_E.7 = 0
  Else
    SysSingleTempB_E.7 = 1
  End If

  SysAddSubSingle
End Sub

Sub SysSubSubDouble
  'Negate B, call addition sub
  If SysDoubleTempB_D.7 Then
    SysDoubleTempB_D.7 = 0
  Else
    SysDoubleTempB_D.7 = 1
  End If

  SysAddSubDouble
End Sub

'********************************************************************************
'Multiply subroutines

'8 bit
sub SysMultSub
  dim SysByteTempA as byte
  dim SysByteTempB as byte
  dim SysByteTempX as byte

  #IFDEF PIC
    #IFDEF ChipFamily 12, 14, 15
      clrf SysByteTempX
    MUL8LOOP:
      movf SysByteTempA, W
      btfsc SysByteTempB, 0
      addwf SysByteTempX, F
      bcf STATUS, C
      rrf SysByteTempB, F
      bcf STATUS, C
      rlf SysByteTempA, F
      movf SysByteTempB, F
      btfss STATUS, Z
      goto MUL8LOOP
    #ENDIF

    #IFDEF ChipFamily 16
      movf SysByteTempA, W
      mulwf SysByteTempB
      movff PRODL,SysByteTempX
    #ENDIF
  #ENDIF

  #IFDEF AVR
    #IFNDEF HardwareMult
      clr SysByteTempX
    MUL8LOOP:
      sbrc SysByteTempB,0
      add SysByteTempX,SysByteTempA
      lsr SysByteTempB
      lsl SysByteTempA
      tst SysByteTempB
      brne MUL8LOOP
    #ENDIF
    #IFDEF HardwareMult
      mul SysByteTempA,SysByteTempB
    #ENDIF
  #ENDIF

end sub

'16 bit
sub SysMultSub16

  dim SysWordTempA as word
  dim SysWordTempB as word
  dim SysWordTempX as word

  #IFDEF PIC
    #IFDEF ChipFamily 12, 14, 15
      dim SysDivMultA as word
      dim SysDivMultB as word
      dim SysDivMultX as word

      SysDivMultA = SysWordTempA
      SysDivMultB = SysWordTempB
      SysDivMultX = 0

      MUL16LOOP:
        IF SysDivMultB.0 ON then SysDivMultX += SysDivMultA
        set STATUS.C OFF
        rotate SysDivMultB right
        set STATUS.C off
        rotate SysDivMultA left
      if SysDivMultB > 0 then goto MUL16LOOP

      SysWordTempX = SysDivMultX
    #ENDIF

    #IFDEF ChipFamily 16
      'X = LowA * LowB
      movf SysWordTempA, W
      mulwf SysWordTempB
      movff PRODL, SysWordTempX
      movff PRODH, SysWordTempX_H

      'HighX += LowA * HighB
      movf SysWordTempA, W
      mulwf SysWordTempB_H
      movf PRODL, W
      addwf SysWordTempX_H, F

      'HighX += HighA * LowB
      movf SysWordTempA_H, W
      mulwf SysWordTempB
      movf PRODL, W
      addwf SysWordTempX_H, F

      'PRODL = HighA * HighB
      movf SysWordTempA_H, F
      mulwf SysWordTempB_H
    #ENDIF
  #ENDIF

  #IFDEF AVR
    #IFNDEF HardwareMult
      dim SysDivMultA as word
      dim SysDivMultB as word
      dim SysDivMultX as word

      SysDivMultA = SysWordTempA
      SysDivMultB = SysWordTempB
      SysDivMultX = 0

      MUL16LOOP:
        IF SysDivMultB.0 ON then SysDivMultX += SysDivMultA
        Set C Off
        rotate SysDivMultB right
        Set C Off
        rotate SysDivMultA left
      if SysDivMultB > 0 then goto MUL16LOOP

      SysWordTempX = SysDivMultX
    #ENDIF

    #IFDEF HardwareMult
      'Need to keep result in here because SysWordTempX[_H] gets overwritten by mul
      dim SysDivMultX as word ' alias SysWordTempX_U, SysWordTempX_H

      'X = LowA * LowB
      mul SysWordTempA, SysWordTempB
      'movff PRODL, SysWordTempX
      'movff PRODH, SysWordTempX_H
      SysDivMultX = SysWordTempX

      'HighX += LowA * HighB
      mul SysWordTempA, SysWordTempB_H
      add SysDivMultX_H, SysWordTempX

      'HighX += HighA * LowB
      mul SysWordTempA_H, SysWordTempB
      add SysDivMultX_H, SysWordTempX

      'Copy result back
      SysWordTempX = SysDivMultX
    #ENDIF
  #ENDIF

end sub

sub SysMultSubInt

  Dim SysIntegerTempA, SysIntegerTempB, SysIntegerTempX As Integer
  Dim SysSignByte As Byte

  'Make both inputs positive, decide output type
  SysSignByte = SysIntegerTempA_H xor SysIntegerTempB_H
  if SysIntegerTempA.15 then SysIntegerTempA = -SysIntegerTempA
  if SysIntegerTempB.15 then SysIntegerTempB = -SysIntegerTempB

  'Call word multiply routine
  SysMultSub16

  'Negate result if necessary
  if SysSignByte.7 then SysIntegerTempX = -SysIntegerTempX

end sub

'32 bit
sub SysMultSub32

  dim SysLongTempA as long
  dim SysLongTempB as long
  dim SysLongTempX as long

  #IFDEF PIC
    'Can't use normal SysDivMult variables for 32 bit, they overlap with
    'SysLongTemp variables
    dim SysLongDivMultA as long
    dim SysLongDivMultB as long
    dim SysLongDivMultX as long

    SysLongDivMultA = SysLongTempA
    SysLongDivMultB = SysLongTempB
    SysLongDivMultX = 0

    MUL32LOOP:
      IF SysLongDivMultB.0 ON then SysLongDivMultX += SysLongDivMultA
      set STATUS.C OFF
      rotate SysLongDivMultB right
      set STATUS.C off
      rotate SysLongDivMultA left
    if SysLongDivMultB > 0 then goto MUL32LOOP

    SysLongTempX = SysLongDivMultX

  #ENDIF

  #IFDEF AVR
    dim SysLongDivMultA as long
    dim SysLongDivMultB as long
    dim SysLongDivMultX as long

    SysLongDivMultA = SysLongTempA
    SysLongDivMultB = SysLongTempB
    SysLongDivMultX = 0

    MUL32LOOP:
      IF SysLongDivMultB.0 ON then SysLongDivMultX += SysLongDivMultA
      Set C Off
      rotate SysLongDivMultB right
      Set C Off
      rotate SysLongDivMultA left
    if SysLongDivMultB > 0 then goto MUL32LOOP

    SysLongTempX = SysLongDivMultX

  #ENDIF

end sub

sub SysMultSubSingle

end sub

'********************************************************************************
'Divide subroutines

'8 bit
sub SysDivSub

  #IFDEF PIC
    dim SysByteTempA as byte
    dim SysByteTempB as byte
    dim SysByteTempX as byte

    #ifdef CheckDivZero TRUE
      'Check for div/0
      movf SysByteTempB, F
      btfsc STATUS, Z
      return
    #endif

    'Main calc routine
    SysByteTempX = 0
    SysDivLoop = 8
    SysDiv8Start:

      bcf STATUS, C
      rlf SysByteTempA, F
      rlf SysByteTempX, F
      movf SysByteTempB, W
      subwf SysByteTempX, F

      bsf SysByteTempA, 0
      btfsc STATUS, C
      goto Div8NotNeg
      bcf SysByteTempA, 0
      movf SysByteTempB, W
      addwf SysByteTempX, F
    Div8NotNeg:

    decfsz SysDivLoop, F
    goto SysDiv8Start

  #ENDIF

  #IFDEF AVR
    #ifdef CheckDivZero TRUE
      'Check for div/0
      tst SysByteTempB
      brne DIV8Cont
      ret
      DIV8Cont:
    #endif

    'Main calc routine
    clr SysByteTempX
    SysDivLoop = 8
    SysDiv8Start:
      lsl SysByteTempA
      rol SysByteTempX
      asm sub SysByteTempX,SysByteTempB 'asm needed, or else sub will be used as start of sub

      sbr SysByteTempA,1
      brsh Div8NotNeg
      cbr SysByteTempA,1
      add SysByteTempX,SysByteTempB
    Div8NotNeg:

    dec SysDivLoop
    brne SysDiv8Start
  #ENDIF

end sub

'16 bit
sub SysDivSub16

  dim SysWordTempA as word
  dim SysWordTempB as word
  dim SysWordTempX as word

  dim SysDivMultA as word
  dim SysDivMultB as word
  dim SysDivMultX as word

  SysDivMultA = SysWordTempA
  SysDivMultB = SysWordTempB
  SysDivMultX = 0

  'Avoid division by zero
  if SysDivMultB = 0 then
    SysWordTempA = 0
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

  SysWordTempA = SysDivMultA
  SysWordTempX = SysDivMultX

end sub

sub SysDivSubInt

  Dim SysIntegerTempA, SysIntegerTempB, SysIntegerTempX As Integer
  Dim SysSignByte As Byte

  'Make both inputs positive, decide output type
  SysSignByte = SysIntegerTempA_H xor SysIntegerTempB_H
  If SysIntegerTempA.15 Then SysIntegerTempA = -SysIntegerTempA
  If SysIntegerTempB.15 Then SysIntegerTempB = -SysIntegerTempB

  'Call word divide routine
  SysDivSub16

  'Negate result if necessary
  If SysSignByte.7 Then
    SysIntegerTempA = -SysIntegerTempA
    SysIntegerTempX = -SysIntegerTempX
  End If

end sub

'32 bit
sub SysDivSub32

  dim SysLongTempA as long
  dim SysLongTempB as long
  dim SysLongTempX as long

  '#ifdef PIC
    dim SysLongDivMultA as long
    dim SysLongDivMultB as long
    dim SysLongDivMultX as long
  '#endif

  SysLongDivMultA = SysLongTempA
  SysLongDivMultB = SysLongTempB
  SysLongDivMultX = 0

  'Avoid division by zero
  if SysLongDivMultB = 0 then
    SysLongTempA = 0
    exit sub
  end if

  'Main calc routine
  SysDivLoop = 32
  SysDiv32Start:

    set C off
    Rotate SysLongDivMultA Left
    Rotate SysLongDivMultX Left
    SysLongDivMultX = SysLongDivMultX - SysLongDivMultB
    Set SysLongDivMultA.0 On

    #IFDEF PIC
      If C Off Then
        Set SysLongDivMultA.0 Off
        SysLongDivMultX = SysLongDivMultX + SysLongDivMultB
      End If

      decfsz SysDivLoop, F
      goto SysDiv32Start
    #ENDIF
    #IFDEF AVR
      If C On Then
        Set SysLongDivMultA.0 Off
        SysLongDivMultX = SysLongDivMultX + SysLongDivMultB
      End If

      dec SysDivLoop
      breq SysDiv32End
      goto SysDiv32Start
      SysDiv32End:
    #ENDIF

  SysLongTempA = SysLongDivMultA
  SysLongTempX = SysLongDivMultX

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
  Dim SysByteTempA, SysByteTempB, SysByteTempX as byte

  #IFDEF ChipFamily 12,14,15
    clrf SysByteTempX
    movf SysByteTempA, W
    subwf SysByteTempB, W
    btfsc STATUS, Z
    comf SysByteTempX,F
  #ENDIF

  #IFDEF ChipFamily 16
    setf SysByteTempX
    movf SysByteTempB, W
    cpfseq SysByteTempA
    clrf SysByteTempX
  #ENDIF

  #IFDEF AVR
    clr SysByteTempX
    cpse SysByteTempA, SysByteTempB
    return
    com SysByteTempX
  #ENDIF
end sub

sub SysCompEqual16

  dim SysWordTempA as word
  dim SysWordTempB as word
  dim SysByteTempX as byte

  #IFDEF ChipFamily 12,14,15
    clrf SysByteTempX

    'Test low, exit if false
    movf SysWordTempA, W
    subwf SysWordTempB, W
    btfss STATUS, Z
    return

    'Test high, exit if false
    movf SysWordTempA_H, W
    subwf SysWordTempB_H, W
    btfss STATUS, Z
    return

    comf SysByteTempX,F
  #ENDIF

  #IFDEF ChipFamily 16
    clrf SysByteTempX

    'Test low, exit if false
    movf SysWordTempB, W
    cpfseq SysWordTempA
    return

    'Test high, exit if false
    movf SysWordTempB_H, W
    cpfseq SysWordTempA_H
    return

    setf SysByteTempX

  #ENDIF
  #IFDEF AVR
    clr SysByteTempX

    cp SysWordTempA, SysWordTempB
    brne SCE16False

    cp SysWordTempA_H, SysWordTempB_H
    brne SCE16False

    com SysByteTempX
    SCE16False:
  #ENDIF
end sub

sub SysCompEqual32

  dim SysLongTempA as long
  dim SysLongTempB as long
  dim SysByteTempX as byte

  #IFDEF ChipFamily 12,14,15
    clrf SysByteTempX

    'Test low, exit if false
    movf SysLongTempA, W
    subwf SysLongTempB, W
    btfss STATUS, Z
    return

    'Test high, exit if false
    movf SysLongTempA_H, W
    subwf SysLongTempB_H, W
    btfss STATUS, Z
    return

    'Test upper, exit if false
    movf SysLongTempA_U, W
    subwf SysLongTempB_U, W
    btfss STATUS, Z
    return

    'Test exp, exit if false
    movf SysLongTempA_E, W
    subwf SysLongTempB_E, W
    btfss STATUS, Z
    return

    comf SysByteTempX,F
  #ENDIF

  #IFDEF ChipFamily 16
    clrf SysByteTempX

    'Test low, exit if false
    movf SysLongTempB, W
    cpfseq SysLongTempA
    return

    'Test high, exit if false
    movf SysLongTempB_H, W
    cpfseq SysLongTempA_H
    return

    'Test upper, exit if false
    movf SysLongTempB_U, W
    cpfseq SysLongTempA_U
    return

    'Test exp, exit if false
    movf SysLongTempB_E, W
    cpfseq SysLongTempA_E
    return

    setf SysByteTempX

  #ENDIF
  #IFDEF AVR
    clr SysByteTempX

    cp SysLongTempA, SysLongTempB
    brne SCE32False

    cp SysLongTempA_H, SysLongTempB_H
    brne SCE32False

    cp SysLongTempA_U, SysLongTempB_U
    brne SCE32False

    cp SysLongTempA_E, SysLongTempB_E
    brne SCE32False

    com SysByteTempX
    SCE32False:
  #ENDIF
end sub

'Less than
'A-B - if negative, C is off
'if A is 4 and B is 2, C is on
'if A is 3 and B is 3, C is on
'if A is 2 and B is 4, C is off
sub SysCompLessThan
  Dim SysByteTempA, SysByteTempB, SysByteTempX as byte

  #IFDEF ChipFamily 12,14,15
    clrf SysByteTempX
    bsf STATUS, C
    movf SysByteTempB, W
    subwf SysByteTempA, W
    btfss STATUS, C
    comf SysByteTempX,F
  #ENDIF

  #IFDEF ChipFamily 16
    setf SysByteTempX
    movf SysByteTempB, W
    cpfslt SysByteTempA
    clrf SysByteTempX
  #ENDIF

  #IFDEF AVR
    clr SysByteTempX
    cp SysByteTempA,SysByteTempB
    brlo SCLTTrue
    ret

    SCLTTrue:
    com SysByteTempX
  #ENDIF
end sub

Sub SysCompLessThan16
  #IFDEF PIC
    dim SysWordTempA as word
    dim SysWordTempB as word
    dim SysByteTempX as byte

    clrf SysByteTempX

    'Test High, exit if more
    movf SysWordTempA_H,W
    subwf SysWordTempB_H,W
    btfss STATUS,C
    return

    'Test high, exit true if less
    movf SysWordTempB_H,W
    subwf SysWordTempA_H,W
    #IFDEF ChipFamily 12,14,15
      btfss STATUS,C
      goto SCLT16True
    #ENDIF
    #IFDEF ChipFamily 16
      bnc SCLT16True
    #ENDIF

    'Test Low, exit if more or equal
    movf SysWordTempB,W
    subwf SysWordTempA,W
    btfsc STATUS,C
    return

    SCLT16True:
    comf SysByteTempX,F
  #ENDIF

  #IFDEF AVR
    clr SysByteTempX

    'Test High, exit false if more
    cp SysWordTempB_H,SysWordTempA_H
    brlo SCLT16False

    'Test high, exit true if less
    cp SysWordTempA_H,SysWordTempB_H
    brlo SCLT16True

    'Test Low, exit if more or equal
    cp SysWordTempA,SysWordTempB
    brlo SCLT16True
    ret

    SCLT16True:
    com SysByteTempX
    SCLT16False:
  #ENDIF

End Sub

Sub SysCompLessThan32
  #IFDEF PIC
    dim SysLongTempA as long
    dim SysLongTempB as long
    dim SysByteTempX as byte

    clrf SysByteTempX

    'Test Exp, exit if more
    movf SysLongTempA_E,W
    subwf SysLongTempB_E,W
    btfss STATUS,C
    return
    'If not more and not zero, is less
    #IFDEF ChipFamily 12,14,15
      btfss STATUS,Z
      goto SCLT32True
    #ENDIF
    #IFDEF ChipFamily 16
      bnz SCLT32True
    #ENDIF

    'Test Upper, exit if more
    movf SysLongTempA_U,W
    subwf SysLongTempB_U,W
    btfss STATUS,C
    return
    'If not more and not zero, is less
    #IFDEF ChipFamily 12,14,15
      btfss STATUS,Z
      goto SCLT32True
    #ENDIF
    #IFDEF ChipFamily 16
      bnz SCLT32True
    #ENDIF

    'Test High, exit if more
    movf SysLongTempA_H,W
    subwf SysLongTempB_H,W
    btfss STATUS,C
    return
    'If not more and not zero, is less
    #IFDEF ChipFamily 12,14,15
      btfss STATUS,Z
      goto SCLT32True
    #ENDIF
    #IFDEF ChipFamily 16
      bnz SCLT32True
    #ENDIF

    'Test Low, exit if more or equal
    movf SysLongTempB,W
    subwf SysLongTempA,W
    btfsc STATUS,C
    return

    SCLT32True:
    comf SysByteTempX,F
  #ENDIF

  #IFDEF AVR
    clr SysByteTempX

    'Test Exp, exit false if more
    cp SysLongTempB_E,SysLongTempA_E
    brlo SCLT32False
    'Test Exp, exit true not equal
    brne SCLT32True

    'Test Upper, exit false if more
    cp SysLongTempB_U,SysLongTempA_U
    brlo SCLT32False
    'Test Upper, exit true not equal
    brne SCLT32True

    'Test High, exit false if more
    cp SysLongTempB_H,SysLongTempA_H
    brlo SCLT32False
    'Test high, exit true not equal
    brne SCLT32True

    'Test Low, exit if more or equal
    cp SysLongTempA,SysLongTempB
    brlo SCLT32True
    ret

    SCLT32True:
    com SysByteTempX
    SCLT32False:
  #ENDIF

End Sub

'Returns true if A < B
sub SysCompLessThanInt

  Dim SysIntegerTempA, SysIntegerTempB, SysDivMultA as Integer

  'Clear result
  SysByteTempX = 0

  'Compare sign bits
  '-A
  If SysIntegerTempA.15 = On Then
    '-A, +B, return true
    If SysIntegerTempB.15 = Off Then
      'Set SysByteTempX to 255
      SysByteTempX = Not SysByteTempX
      Exit Sub
    End If
    '-A, -B, negate both and swap
    SysDivMultA = -SysIntegerTempA
    SysIntegerTempA = -SysIntegerTempB
    SysIntegerTempB = SysDivMultA
  '+A
  Else
    '+A, -B, return false
    If SysIntegerTempB.15 = On Then
      Exit Sub
    End If
  End If

  #IFDEF PIC

    'Test High, exit if more
    movf SysIntegerTempA_H,W
    subwf SysIntegerTempB_H,W
    btfss STATUS,C
    return

    'Test high, exit true if less
    movf SysIntegerTempB_H,W
    subwf SysIntegerTempA_H,W
    #IFDEF ChipFamily 12,14,15
      btfss STATUS,C
      goto SCLTIntTrue
    #ENDIF
    #IFDEF ChipFamily 16
      bnc SCLTIntTrue
    #ENDIF

    'Test Low, exit if more or equal
    movf SysIntegerTempB,W
    subwf SysIntegerTempA,W
    btfsc STATUS,C
    return

  SCLTIntTrue:
    comf SysByteTempX,F
  #ENDIF

  #IFDEF AVR

    'Test High, exit false if more
    cp SysIntegerTempB_H,SysIntegerTempA_H
    brlo SCLTIntFalse

    'Test high, exit true if less
    cp SysIntegerTempA_H,SysIntegerTempB_H
    brlo SCLTIntTrue

    'Test Low, exit if more or equal
    cp SysIntegerTempA,SysIntegerTempB
    brlo SCLTIntTrue
    ret

  SCLTIntTrue:
    com SysByteTempX
  SCLTIntFalse:
  #ENDIF

end sub
