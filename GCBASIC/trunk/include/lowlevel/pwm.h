'    Pulse Width Modulation routines for Great Cow BASIC
'    Copyright (C) 2006 - 2017 Hugh Considine, William Roth, Kent Schafer and Evan R. Venn


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

'''Changed position of 'Dim PRx_Temp as word' to remove declaration of variables when not required
''' 14/3/16   Added support for Hardware PMW and revised CCP PWM to support 16f18855 series
''' 01/08/16  Added support for 16f18326* series
''' 08/08/16  Revised to remove the silly error instroduced above when I called bit() with a leading space!
''' 09/08/16  Revised to remove fix the all case ENable option
''' 13/08/16  Added HPWM5 missing constant
''' 10/10/16  Added by EvanV CCP5 support with great help from Isay Goltman
''' 12/10/16  Revised to sort out that the work of 14/3/16 had broken all the legacy chips!  Added alternative BIT() Test
'''           Also, revised calcs to improve performance
''' 1/12/2016 Revised to enhance variable usafe when specific registers do not exist
''' 3/12/2016 Revised to remove script that is not required
''' 4/12/2016 Improved to remove creation of unneeded variables when the register  does not exist
''' 20/2/2017 Revised to add T2CLKCON to resolve T2CLK issues when using GPASM
''' 3/5/2017  Added AVR microcontrollers KentS
''' 13/3/2017 Revised to add HPWM3 10bit and HPWM4 10bit
''' 24/5/2017 Added HPWMOn to reference PWMOn and correct CCPTMRS settings for HPWM3 and HPWM4
''' 25/5/2017 Added support for PWMTimerVariant1 to address timer addressing issue. Added further optimisation to NOT include code when the PWM channel does not exist
''' 26/5/2017 Further improvements of PWMTimerVariant1 to address timer addressing issue
''' 26/5/2017 Revised to add full 10bit Frequency support to the 10bit HPWM function and to add improved HPWMOFF
''' 27/5/2017 Revised 10bit duty to be 10bit duty and rename of PWMTimerVariant1 to ChipPWMTimerVariant
''' 27/5/2017 Add byte support for a 10bit duty method
''' 28/5/2017 Added HPWMUpdate and changed calculateDuty to a sub to save memory
''' 29/5/2017 Improved support as follows:
'''           New constants for user use PRxPeriodRegister and TimerXprescaleselectValue
'''           Support for 8bit call to 10bit PWM (non-CCP)
'''           Memory improvements in terms of program size
'''           Support for HPWM_Fast within 10bit PWM (non-CCP) method
'''           Removal of script to calculate PR2Temp. Replaced with Sub-routine this reduces code size
'''           Removal of complexity by removal of FnLSL in calculateDuty method
'''           Added support for HPWMOff ( channel, PWMHardware ). Turns of 10bit PWM (non-CCP) channel
'''           Improved documentation
''' 06/6/2017 Corrected missing Dim of variable in CCP/PWM
''' 03/7/2017 Added HPWM3 and HPWM4 constants
''' 15/7/2017 Moved the default constants to prevent entry into ASM
''' 17/8/2017 Revised to protect CCP1M0-3 being set when no CCP module exists.
''' 08/9/2017 Revised to futher isolate CCP1M0-3 being set when no CCP module exists and added PWM1 and PWM2 support - 
'''           PWM1 and PWM2 support is disabled by default


  'define the defaults
  #define AVRTC0
  #define AVRCHAN2
  #define PWM_Freq 38      'Frequency of PWM in KHz
  #define PWM_Duty 50      'Duty cycle of PWM (%)

  #IFDEF PIC

    #define HPWM1 1
    #define HPWM2 2
    #define HPWM3 3
    #define HPWM4 4
    #define HPWM5 5
    #define HPWM6 6
    #define HPWM7 7


    #define USE_HPWMCCP1 TRUE
    #define USE_HPWMCCP2 TRUE
    #define USE_HPWMCCP3 TRUE
    #define USE_HPWMCCP4 TRUE
    #define USE_HPWMCCP5 TRUE

    #define USE_HPWM1 FALSE
    #define USE_HPWM2 FAlSE
    #define USE_HPWM3 TRUE
    #define USE_HPWM4 TRUE
    #define USE_HPWM5 TRUE
    #define USE_HPWM6 TRUE
    #define USE_HPWM7 TRUE

    #define USE_HPWM_TIMER2 TRUE
    #define USE_HPWM_TIMER4 TRUE
    #define USE_HPWM_TIMER6 TRUE

  #ENDIF

  #script
    If AVR Then
      'redirects to AVR code
      HPWM = AVRHPWM
      PWMOn = AVRPWMOn
      PWMOff = AVRPWMOff
    End If
  #endscript


#startup InitPWM



Sub InitPWM


  #ifdef PIC

      Dim PRx_Temp as LONG

      'Script to calculate constants required for given Frequency and Duty Cycle
      #script

      'revised to support new chips like 16f18326*, 16f18555 and related microcontrollers"
       if bit(CCP1MODE0) then

            if nobit(CCP1M0) Then

    '          warning "Supporting microcontrollers like the 16f18326 and related microcontrollers for CCPxMODEx"
              CCP1M0 = CCP1MODE0
              CCP1M1 = CCP1MODE1
              CCP1M2 = CCP1MODE2
              CCP1M3 = CCP1MODE3

              CCP2M0 = CCP2MODE0
              CCP2M1 = CCP2MODE1
              CCP2M2 = CCP2MODE2
              CCP2M3 = CCP2MODE3

              CCP3M0 = CCP3MODE0
              CCP3M1 = CCP3MODE1
              CCP3M2 = CCP3MODE2
              CCP3M3 = CCP3MODE3

              CCP4M0 = CCP4MODE0
              CCP4M1 = CCP4MODE1
              CCP4M2 = CCP4MODE2
              CCP4M3 = CCP4MODE3

              CCP5M0 = CCP5MODE0
              CCP5M1 = CCP5MODE1
              CCP5M2 = CCP5MODE2
              CCP5M3 = CCP5MODE3

            end if

        end if

        'remapped for consistency
        if bit(CCP4EN) then
    '        warning "Supporting microcontrollers like the 16f18326 and related microcontrollers for CCPxEN"
            CCP2CON_EN = CCP2EN
            CCP3CON_EN = CCP3EN
            CCP4CON_EN = CCP4EN
            CCP5CON_EN = CCP5EN
        end if

        'T2PR was used in these routines, but, that is now a register, so, Changed to TxPR
          PR2Temp = int((1/PWM_Freq)/(4*(1/(ChipMHz*1000))))
          TxPR = 1
        if PR2Temp > 255 then
            PR2Temp = int((1/PWM_Freq)/(16*(1/(ChipMHz*1000))))
            TxPR = 4
            if PR2Temp > 255 then
              PR2Temp = int((1/PWM_Freq)/(64*(1/(ChipMHz*1000))))
              TxPR = 16
            if PR2Temp > 255 then
              'error TxPR, PR2Temp
              error msg(BadPWMFreq)
            end if
          end if
        end if


        'Global constant for users to use
        PRxPeriodRegister = PR2Temp
        TimerXprescaleselectValue = TxPR

        'Code from Early days
        'DutyCycle = (PWM_Duty*10.24)*PR2Temp/1024
        DutyCycle = (PWM_Duty / 25) * (PR2Temp + 1)
        DutyCycleH = (DutyCycle AND 1020)/4
        DutyCycleL = DutyCycle AND 3
        'End of Code from Early days

        PWMOsc1 = int(60000/(240/ChipMHz))    'This is used in the calculations
        PWMOsc4 = int(60000/(960/ChipMHz))    '*** Unused constant ***
        PWMOsc16 = int(60000/(3840/ChipMHz))  '*** Unused constant ***

      #endscript


        'If CCP1CON does not exist then there is NO CCP1 so no point in setting, as all this is to set up the CCP1 using constants method
        #ifdef Var(CCP1CON)


            DIM CCPCONCache as BYTE  'Added 27.04.2105 - WMR
            CCPCONCache = 0          'Added 27.04.2015 = WMR

            'Set PWM Period
            PR2 = PR2Temp
            #ifdef TxPR 1
              SET T2CON.T2CKPS0 OFF
              SET T2CON.T2CKPS1 OFF
            #endif
            #ifdef TxPR 4
              SET T2CON.T2CKPS0 ON
              SET T2CON.T2CKPS1 OFF
            #endif
            #ifdef TxPR 16
              SET T2CON.T2CKPS0 OFF
              SET T2CON.T2CKPS1 ON
            #endif

            'Set Duty cycle
            CCPR1L = DutyCycleH

            #ifdef DutyCycleL 0

              #ifdef Bit(CCP1X)               '***  NOt Defined
                SET CCPCONCache.CCP1Y OFF
                SET CCPCONCache.CCP1X OFF
              #endif
              #ifdef Bit(DC1B1)              '*** DEFINED
                SET CCPCONCache.DC1B1 OFF
                SET CCPCONCache.DC1B0 OFF
              #endif

            #endif

            #ifdef DutyCycleL 1

              #ifdef Bit(CCP1X)
                SET CCPCONCache.CCP1Y ON
                SET CCPCONCache.CCP1X OFF
              #endif
              #ifdef Bit(DC1B1)
                SET CCPCONCache.DC1B1 OFF
                SET CCPCONCache.DC1B0 ON
              #endif

            #endif


            #ifdef DutyCycleL 2
              #ifdef Bit(CCP1X)
                SET CCPCONCache.CCP1Y OFF
                SET CCPCONCache.CCP1X ON
              #endif

              #ifdef Bit(DC1B1)
                SET CCPCONCache.DC1B1 ON
                SET CCPCONCache.DC1B0 OFF
              #endif
            #endif

            #ifdef DutyCycleL 3
              #ifdef Bit(CCP1X)
                SET CCPCONCache.CCP1Y ON
                SET CCPCONCache.CCP1X ON
              #endif
              #ifdef Bit(DC1B1)
                SET CCPCONCache.DC1B1 ON
                SET CCPCONCache.DC1B0 ON
              #endif
            #endif

            'Finish preparing CCP*CON
            SET CCPCONCache.CCP1M3 ON
            SET CCPCONCache.CCP1M2 ON
            SET CCPCONCache.CCP1M1 OFF
            SET CCPCONCache.CCP1M0 OFF

            'Enable Timer 2
            SET T2CON.TMR2ON ON

        #endif



      #ifdef HPWM_FAST
        PWMFreqOld = 0
        PWMTimerOld = 0
      #endif

  #endif


  #IFDEF AVR

    #script
      ICR1temp = int(((chipMHZ*1000) / PWM_Freq) - 1)
      PWMPrescaleTemp = 1
      If ICR1temp > 255 Then
        ICR1temp = int(ICR1temp / 8)
        PWMPrescaleTemp = 8

        If ICR1temp > 255 Then
          ICR1temp = int(ICR1temp / 8)
          PWMPrescaleTemp = 64
        End If

          If ICR1temp > 255 Then
            error msg(BadHPWMFreq)
          End If
      End If
      PWM_Duty = int((PWM_Duty * 255)/100)
      PWMDutyTempAVR = int((PWM_Duty * ICR1temp) / 255)
    #endscript

    'Set all defined channel modes, and default channel 2 mode
    'Mode must be set first
    AVRSetHPWMMode
    PWMPrescale = PWMPrescaleTemp

    'Set prescale and duty cycle on timer0 OCR0B (channel 2)
    'when using PWMON/PWMOFF
    #IFDEF AVRCHAN2
      AVRTimer = 0
      AVRSetPrescale (AVRTimer)
      'OCR2A is used as the period register
      #IFNDEF VAR(OCR0AL)
        OCR0A = ICR1temp
        OCR0B = PWMDutyTempAVR
      #ENDIF

      #IFDEF VAR(OCR0AL)
        OCR0AL = ICR1temp
        OCR0BL = PWMDutyTempAVR
      #ENDIF
    #ENDIF

    AVRPWMFreqOld = 0

  #ENDIF


End Sub


'Enables the CCP1 capability - the setup was completed in the Init script
#Define HPWMOn PWMOn
sub PWMOn
  CCP1CON = CCPCONCache
end sub

'Disables the CCP1 capability
sub PWMOff
  CCP1CON = 0
end sub


'Supports CCPx [1-5] the clock souce is always Timer2
'This is an 8bit resolution
sub HPWM (In PWMChannel, In PWMFreq, PWMDuty)  '8 bit resolution on timer 2

  Dim PRx_Temp as LONG
  Dim PRx_Temp_Cache as Long
  dim PWMDuty as word
  dim PWMResolution as word: PWMResolution = 255


  'If HPWM_FAST operation selected, only recalculate timer prescaler when
  'needed. Gives faster operation, but uses extra byte of RAM and may cause
  'problems if HPWM and PWMOn are used together in a program.
  '(No issues using HPWM and PWMOff in the same program with HPWM_FAST.)
  #ifdef HPWM_FAST
      If PWMFreq <> PWMFreqOld Then
  #endif

          ' Commence calculations of PMW parameters
          ' This figures out Postscaler required.  We can set to 1, 4 or 16 to set Tx_PR
          ' So, start with 1 - check the remainder. If the remainder in the high byte is greater then zero then do same with a prescaler value of 4
          ' So, using 4 - check the remainder. If the remainder in the high byte is greater then zero then do same with a prescaler value of 16
          ' So, using 16
          ' This simply set Tx_PR to 1,4 or 16

          Tx_PR = 1
          PRx_Temp = PWMOsc1 / PWMFreq
          IF PRx_Temp_H > 0 then
            Tx_PR = 4
            'Divide by 4
            set STATUS.C off
            rotate PRx_Temp right
            set STATUS.C off
            rotate PRx_Temp right
          end if

          IF PRx_Temp_H > 0 then
            Tx_PR = 16
            'Divide by 4
            set STATUS.C off
            rotate PRx_Temp right
            set STATUS.C off
            rotate PRx_Temp right
          end if

          PR2 = PRx_Temp

          'Set the Bits for the Postscaler
          SET T2CON.T2CKPS0 OFF
          SET T2CON.T2CKPS1 OFF
          if Tx_PR = 4 then SET T2CON.T2CKPS0 ON
          if Tx_PR = 16 then SET T2CON.T2CKPS1 ON
          SET T2CON.TMR2ON ON

          'Set Clock Source, if required
          #ifdef var(T2CLKCON)
              'Set to FOSC/4 for backward compatibility
              T2CLKCON.T2CS0 = 1
              T2CLKCON.T2CS1 = 0
              T2CLKCON.T2CS2 = 0
              T2CLKCON.T2CS3 = 0
          #endif

  #ifdef HPWM_FAST
          PWMFreqOld = PWMFreq
          'Cache the PRx_Temp to support HPWM_FAST.
          PRx_Temp_Cache = PRx_Temp
      Else
        'As wierd as this looks.... restore the cache to PRX_temp as we need it when in HPWM_Fast mode
        'PRx_Temp is used in channel setting section, so, if we did not restore we would not have a consistent set of information.
        PRx_Temp = PRx_Temp_Cache
      End If
  #endif

  'this code can be optimised by using defines USE_HPWMCCP1|2|3|4|5
  'and, you can define user setup and exit commands using AddHPWMCCPSetupN and  AddHPWMCCPExitN
  '   These can be used to FIX little errors!

  #ifdef USE_HPWMCCP1 TRUE

    #ifdef AddHPWMCCPSetup1
      AddHPWMCCPSetup1
    #endif

    #ifdef NoVar(CCP2CON)   'We assumne there is is only 1 CCP module on Chip
      PRx_Temp = PWMDuty * (PRx_Temp + 2)
      CCPR1L = PRx_Temp_H
      If PWMDuty = 0 Then CCPR1L = 0  ' Assure OFF at Zero - WMR
      SET CCP1CON.CCP1M3 ON
      SET CCP1CON.CCP1M2 ON
      SET CCP1CON.CCP1M1 OFF
      SET CCP1CON.CCP1M0 OFF
    #endif

    #ifdef AddHPWMCCPExit1
      AddHPWMCCPExit1
    #endif

  #endif


  'Devices with more than one CCP
  #ifdef Var(CCP2CON)

    #ifdef USE_HPWMCCP1 TRUE 'USE_HPWMCCP1 TRUE

      #ifdef AddHPWMCCPSetup1
        AddHPWMCCPSetup1
      #endif

      if PWMChannel = 1 then


        'Testing this bit is to identify the 2016 chip that use CCPR1H and CCPR1L of PWM
        #ifndef BIT(CCP1FMT)
            PRx_Temp = PWMDuty * (PRx_Temp + 2)  'Correction  - WMR
            CCPR1L = PRx_Temp_H
            If PWMDuty = 0 Then CCPR1L = 0  ' Assure OFF at Zero - WMR
            SET CCP1M3 ON
            SET CCP1M2 ON
            SET CCP1M1 OFF
            SET CCP1M0 OFF
        #ENDIF

        'Testing this bit is to identify the 2016 chip that use CCPR1H and CCPR1L of PWM
        #ifdef BIT(CCP1FMT)
            calculateDuty 'Sets PRx_Temp  to the duty value for bits 15-8 and 7-6
            CCPR1H = PRx_Temp_H
            CCPR1L = PRx_Temp
            SET CCP1M3 ON
            SET CCP1M2 ON
            SET CCP1M1 ON
            SET CCP1M0 ON

            #ifdef bit(CCP1EN)
              SET CCP1EN ON
            #endif

            #ifdef bit(CCP1CON_EN)
              SET CCP1CON_EN ON
            #endif

            #ifdef bit(CCP1FMT)
              SET CCP1FMT ON
            #endif
        #ENDIF

      end if

      #ifdef AddHPWMCCPExit1
        AddHPWMCCPExit1
      #endif

    #endif

    #ifdef USE_HPWMCCP2 TRUE 'USE_HPWMCCP2 TRUE

      #ifdef AddHPWMCCPSetup2
        AddHPWMCCPSetup2
      #endif

      if PWMChannel = 2 then


        #ifndef BIT(CCP2FMT)
            PRx_Temp = PWMDuty * ( PRx_Temp + 2)  'Correction  - WMR
            CCPR2L = PRx_Temp_H
            If PWMDuty = 0 Then CCPR2L = 0  ' Assure OFF at Zero - WMR
            SET CCP2M3 ON
            SET CCP2M2 ON
            SET CCP2M1 OFF
            SET CCP2M0 OFF
        #ENDIF

        #ifdef BIT(CCP2FMT)
            calculateDuty 'Sets PRx_Temp  to the duty value for bits 15-8 and 7-6
            CCPR2H = PRx_Temp_H
            CCPR2L = PRx_Temp
            SET CCP2M3 ON
            SET CCP2M2 ON
            SET CCP2M1 ON
            SET CCP2M0 ON

            #ifdef bit(CCP2EN)
              SET CCP2EN ON
            #endif

            #ifdef bit(CCP2CON_EN)
              SET CCP2CON_EN ON
            #endif

            #ifdef bit(CCP2FMT)
              SET CCP2FMT ON
            #endif
        #ENDIF

      end if

      #ifdef AddHPWMCCPExit2
        AddHPWMCCPExit2
      #endif

    #endif

  #endif

  #ifdef USE_HPWMCCP3 TRUE

    #ifdef AddHPWMCCPSetup3
      AddHPWMCCPSetup3
    #endif

    #ifdef Var(CCP3CON)
      if PWMChannel = 3 then 'USE_HPWMCCP3 TRUE

        #ifndef BIT(CCP3FMT)
            PRx_Temp = PWMDuty * (PRx_Temp + 2)  'Correction  - WMR
            CCPR3L = PRx_Temp_H
            If PWMDuty = 0 Then CCPR3L = 0  ' Assure OFF at Zero - WMR
            SET CCP3M3 ON
            SET CCP3M2 ON
            SET CCP3M1 OFF
            SET CCP3M0 OFF
        #ENDIF

        #ifdef BIT(CCP3FMT)
            calculateDuty 'Sets PRx_Temp  to the duty value for bits 15-8 and 7-6
            CCPR3H = PRx_Temp_H
            CCPR3L = PRx_Temp
            SET CCP3M3 ON
            SET CCP3M2 ON
            SET CCP3M1 ON
            SET CCP3M0 ON

            #ifdef bit(CCP3EN)
              SET CCP3EN ON
            #endif


            #ifdef bit(CCP3CON_EN)
              SET CCP3CON_EN ON
            #endif

            #ifdef bit(CCP3FMT)
              SET CCP3FMT ON
            #endif

        #ENDIF

      end if
    #endif

    #ifdef AddHPWMCCPExit3
      AddHPWMCCPExit3
    #endif

  #endif

  #ifdef USE_HPWMCCP4 TRUE

    #ifdef AddHPWMCCPSetup4
      AddHPWMCCPSetup4
    #endif

    #ifdef Var(CCP4CON)    'Added 17.04.2105 - WMR
      if PWMChannel = 4 then  'USE_HPWMCCP4 TRUE

        #ifndef BIT(CCP4FMT)
            PRx_Temp = PWMDuty * (PRx_Temp + 2)  '
            CCPR4L = PRx_Temp_H
            If PWMDuty = 0 Then CCPR4L = 0  ' Assure OFF at Zero - WMR

            SET CCP4M3 ON'These my have been remapped using a script - do check ASM and script in INITPWM @1@CCP4
            SET CCP4M2 ON'These my have been remapped using a script - do check ASM and script in INITPWM
            SET CCP4M1 OFF'These my have been remapped using a script - do check ASM and script in INITPWM
            SET CCP4M0 OFF'These my have been remapped using a script - do check ASM and script in INITPWM
        #ENDIF

        #ifdef BIT(CCP4FMT)
            calculateDuty 'Sets PRx_Temp  to the duty value for bits 15-8 and 7-6.  ifdef VAR(CCPR1H)
            CCPR4H = PRx_Temp_H
            CCPR4L = PRx_Temp
            SET CCP4M3 ON'These my have been remapped using a script - do check ASM and script in INITPWM @2@CCP4
            SET CCP4M2 ON'These my have been remapped using a script - do check ASM and script in INITPWM
            SET CCP4M1 ON'These my have been remapped using a script - do check ASM and script in INITPWM
            SET CCP4M0 ON'These my have been remapped using a script - do check ASM and script in INITPWM

            #ifdef bit(CCP4EN)
              SET CCP4EN ON
            #endif

            #ifdef bit(CCP4CON_EN)
              SET CCP4CON_EN ON
            #endif

            #ifdef bit(CCP4FMT)
              SET CCP4FMT ON
            #endif

        #ENDIF
      end if
    #endif

    #ifdef AddHPWMCCPExit4
      AddHPWMCCPExit4
    #endif

  #endif

  #ifdef USE_HPWMCCP5 TRUE

    #ifdef AddHPWMCCPSetup5
      AddHPWMCCPSetup5
    #endif

    #ifdef Var(CCP5CON)    'ADDED 10.10.2016 erv
      if PWMChannel = 5 then  'USE_HPWMCCP5 TRUE

        #ifndef BIT(CCP5FMT)
            PRx_Temp = PWMDuty * (PRx_Temp + 2)
            CCPR5L = PRx_Temp_H
            If PWMDuty = 0 Then CCPR5L = 0  ' Assure OFF at Zero - WMR

            SET CCP5M3 ON'These my have been remapped using a script - do check ASM and script in INITPWM @1@CCP5
            SET CCP5M2 ON'These my have been remapped using a script - do check ASM and script in INITPWM
            SET CCP5M1 OFF'These my have been remapped using a script - do check ASM and script in INITPWM
            SET CCP5M0 OFF'These my have been remapped using a script - do check ASM and script in INITPWM
        #ENDIF

        #ifdef BIT(CCP5FMT)
            calculateDuty 'Sets PRx_Temp  to the duty value for bits 15-8 and 7-6.  ifdef VAR(CCPR1H)
            CCPR5H = PRx_Temp_H
            CCPR5L = PRx_Temp
            SET CCP5M3 ON'These my have been remapped using a script - do check ASM and script in INITPWM @2@CCP5
            SET CCP5M2 ON'These my have been remapped using a script - do check ASM and script in INITPWM
            SET CCP5M1 ON'These my have been remapped using a script - do check ASM and script in INITPWM
            SET CCP5M0 ON'These my have been remapped using a script - do check ASM and script in INITPWM

            #ifdef bit(CCP5EN)
              SET CCP5EN ON
            #endif

            #ifdef bit(CCP5CON_EN)
              SET CCP5CON_EN ON
            #endif

            #ifdef bit(CCP5FMT)
              SET CCP5FMT ON
            #endif

        #ENDIF
      end if
    #endif

    #ifdef AddHPWMCCPExit5
      AddHPWMCCPExit5
    #endif

  #endif

end sub

'Disable the CCP channel
#Define HPWMOff PWMOff
SUB PWMOff (IN Channel)   'Added 27.04.2015

    If Channel > 5 OR Channel < 1 then channel = 1

    #IF bit(SET CCP1EN ON)
      Select Case Channel
          Case 1
              SET CCP1EN OFF
          Case 2
              SET CCP2CON_EN OFF
          Case 3
              SET CCP3CON_EN OFF
          Case 4
              SET CCP4CON_EN OFF
          Case 5
              SET CCP5CON_EN OFF
      End Select
      Exit Sub
    #ENDIF

    #IFNDEF VAR(CCP2CON)  'There is only 1 CCP Module
     if channel = 1 then
          CCP1CON = 0
          exit sub
     end if
    #ENDIF

    #IFDEF VAR(CCP2CON)   '2 or more CCP modules
       IF Channel = 1 then CCP1CON = 0
       IF Channel = 2 then CCP2CON = 0
    #ENDIF

    #IFDEF VAR(CCP3CON)
       IF Channel = 3 then CCP3CON = 0
    #ENDIF

    #IFDEF VAR(CCP4CON)
       IF Channel = 4 then CCP4CON = 0
    #ENDIF

    #IFDEF VAR(CCP5CON)
       IF Channel = 5 then CCP5CON = 0
    #ENDIF

END SUB

'Disable the PWM module channel. Not the CCP!! see previous method for CCP
SUB PWMOff (IN Channel, IN PWMHardware)

    'PWMHardware is used to purely identify this method as the hardware OFF not the CCP OFF

      Select Case Channel
          #IFDEF BIT(PWM1EN) 'this simply stops error messages when the does not exit
            Case 1
              Set PWM1EN Off

              'Disable PWM output enable bit on chips like the 10f322
              #IFDEF BIT(PWM1OE)
                PWM1OE = 0
              #ENDIF

          #ENDIF
          #IFDEF BIT(PWM2EN) 'this simply stops error messages when the does not exit
            Case 2
              Set PWM2EN Off

              'Disable PWM output enable bit
              #IFDEF BIT(PWM2OE)
                PWM2OE = 0
              #ENDIF

          #ENDIF
          #IFDEF BIT(PWM3EN) 'this simply stops error messages when the does not exit
            Case 3
              Set PWM3EN Off
          #ENDIF
          #IFDEF BIT(PWM4EN) 'this simply stops error messages when the does not exit
            Case 4
              Set PWM4EN Off
          #ENDIF
          #IFDEF BIT(PWM5EN) 'this simply stops error messages when the does not exit
            Case 5
              Set PWM5EN Off
          #ENDIF
          #IFDEF BIT(PWM6EN) 'this simply stops error messages when the does not exit
            Case 6
              Set PWM6EN Off
          #ENDIF
          #IFDEF BIT(PWM7EN) 'this simply stops error messages when the does not exit
            Case 7
              Set PWM7EN Off
          #ENDIF

      End Select

END SUB

' Byte support for DUTY... folks may not know that they need to pass a 10bit number
' Takes a byte in the range of 0-255 and passes the the 10bit method with the PWMResolution set to 255
sub HPWM (In PWMChannel, In PWMFreq as WORD, in PWMDuty as byte, in TimerSelected as byte, Optional in PWMResolution as word = 255 )
    dim PWMDuty as word
    HPWM ( PWMChannel, PWMFreq, [word]PWMDuty, TimerSelected, PWMResolution  )
end Sub

'Word for DUTY also defaults the PWMResolution to 1023 (10bit support)
;Overloaded signature: BYTE:WORD:WORD:BYTE:word:
sub HPWM (In PWMChannel, In PWMFreq as WORD, in PWMDuty as WORD , in TimerSelected  as byte, Optional in PWMResolution as word = 1023)

    Dim PRx_Temp as LONG
    #ifdef ChipPWMTimerVariant
      Dim TimerSelectionBits as byte
    #endif

  'If HPWM_FAST operation selected, only recalculate duty timer prescaler when
  'needed.
  #ifdef HPWM_FAST
      If ( PWMFreq <> PWMFreqOld )  or ( TimerSelected <> PWMTimerOld ) Then
  #endif


    ' Commence calculations of PMW parameters
    ' This figures out Postscaler required.  We can set to 1, 4 or 16 to set Tx_PR
    ' So, start with 1 - check the remainder. If the remainder in the high byte is greater then zero then do same with a prescaler value of 4
    ' So, using 4 - check the remainder. If the remainder in the high byte is greater then zero then do same with a prescaler value of 16
    ' So, using 16
    'This simply set Tx_PR to 1,4 or 16

    PRx_Temp = PWMOsc1 / PWMFreq
    Tx_PR = 1
    IF PRx_Temp_H > 0 then
      Tx_PR = 4
      'Divide by 4
      set STATUS.C off
      rotate PRx_Temp right
      set STATUS.C off
      rotate PRx_Temp right
    end if

    IF PRx_Temp_H > 0 then
      Tx_PR = 16
      'Divide by 4
      set STATUS.C off
      rotate PRx_Temp right
      set STATUS.C off
      rotate PRx_Temp right
    end if

    IF PRx_Temp_H > 0 then
      Tx_PR = 64
      'Divide by 4
      set STATUS.C off
      rotate PRx_Temp right
      set STATUS.C off
      rotate PRx_Temp right
    end if

'This section setups the timer

    'Set up PRx dependent on timer selected
    Select Case TimerSelected

        #ifdef USE_HPWM_TIMER2 TRUE
          case 2 'Select timer2 as the driver for PWM
            'Set PR2
            PR2 = PRx_Temp  'This is required in the next sction of code, and as will not know which timer has been selected

            'Set Clock Source
            'Set to FOSC/4 for backward compatibility
            #ifdef var(T2CLKCON): T2CLKCON = 0x01: #ENDIF

            'T2PSYNC Not Synchronized; T2MODE Software control; T2CKPOL Rising Edge; T2CKSYNC Not Synchronized
            'T2HLT = 0x00
            #ifdef var(T2HLT): T2HLT = 0x00: #ENDIF

            'T2RSEL T2CKIPPS pin
            'T2RST = 0x00
            #ifdef var(T2RST): T2RST = 0x00: #ENDIF

            'TMR2.  Holding Register for the 8-bit TMR2 Register
            'T2TMR = 0x00
            #ifdef var(T2TMR): T2TMR = 0x00: #ENDIF

            'Setup Timerx by clearing the Prescaler bits - it is set next....
            #ifdef bit(T2CKPS2)
                SET T2CKPS0 OFF
                SET T2CKPS1 OFF
                SET T2CKPS2 OFF
                'Set Prescaler bits
                if Tx_PR = 4  then SET T2CKPS1 ON
                if Tx_PR = 16 then SET T2CKPS2 ON
                if Tx_PR = 64 then SET T2CKPS2 ON: SET T2CKPS1 ON
            #endif

            #ifndef bit(T2CKPS2)
                SET T2CKPS0 OFF
                SET T2CKPS1 OFF
                'Set Prescaler bits
                if Tx_PR = 4  then SET T2CKPS0 ON
                if Tx_PR = 16 then SET T2CKPS1 ON
                if Tx_PR = 64 then SET T2CKPS0 ON: SET T2CKPS1 ON
            #endif

            'Clearing IF flag.
            SET TMR2IF OFF

            'Start Timer2
            SET TMR2ON ON
        #endif

        #ifdef USE_HPWM_TIMER4 TRUE
            #ifdef var(T4CON)
              case 4
                'Set PR4
                PR4 = PRx_Temp  'This is required in the next sction of code, and as will not know which timer has been selected

              'Set Clock Source
              'Set to FOSC/4 for backward compatibility
              #ifdef var(T4CLKCON): T4CLKCON = 0x01: #ENDIF

              'T4PSYNC Not Synchronized; T4MODE Software control; T4CKPOL Rising Edge; T4CKSYNC Not Synchronized
              'T4HLT = 0x00
              #ifdef var(T4HLT): T4HLT = 0x00: #ENDIF

              'T4RSEL T4CKIPPS pin
              'T4RST = 0x00
              #ifdef var(T4RST): T4RST = 0x00: #ENDIF

              'TMR4. Holding Register for the 8-bit TMR4` Register
              'T4TMR = 0x00
              #ifdef var(T4TMR): T4TMR = 0x00: #ENDIF

                #ifdef bit(T4CKPS2)
                    SET T4CKPS0 OFF
                    SET T4CKPS1 OFF
                    SET T4CKPS2 OFF
                    'Set Prescaler bits
                    if Tx_PR = 4  then SET T4CKPS1 ON
                    if Tx_PR = 16 then SET T4CKPS2 ON
                    if Tx_PR = 64 then SET T4CKPS2 ON: SET T4CKPS1 ON
                #endif

                #ifndef bit(T4CKPS2)
                    SET T4CKPS0 OFF
                    SET T4CKPS1 OFF
                    'Set Prescaler bits
                    if Tx_PR = 4  then SET T4CKPS0 ON
                    if Tx_PR = 16 then SET T4CKPS1 ON
                    if Tx_PR = 64 then SET T4CKPS0 ON: SET T4CKPS1 ON
                #endif
                'Clearing IF flag.
                SET TMR4IF OFF

                'Start Timer2
                SET TMR4ON ON
            #endif
        #endif

        #ifdef USE_HPWM_TIMER6 TRUE

          #ifdef var(T6CON)
              case 6

              'Set PR6
                PR6 = PRx_Temp  'This is required in the next sction of code, and will not know which timer has been selected

                'Set Clock Source
                'Set to FOSC/4 for backward compatibility
                #ifdef var(T6CLKCON): T6CLKCON = 0x01: #ENDIF

                'T6PSYNC Not Synchronized; T6MODE Software control; T6CKPOL Rising Edge; T6CKSYNC Not Synchronized
                'T6HLT = 0x00
                #ifdef var(T6HLT): T6HLT = 0x00: #ENDIF

                'T6RSEL T6CKIPPS pin
                'T6RST = 0x00
                #ifdef var(T6RST): T6RST = 0x00: #ENDIF

                'TMR6.  Holding Register for the 8-bit TMR6 Register
                'T6TMR = 0x00
                #ifdef var(T6TMR): T6TMR = 0x00: #ENDIF

                #ifdef bit(T6CKPS2)
                    SET T6CKPS0 OFF
                    SET T6CKPS1 OFF
                    SET T6CKPS2 OFF
                    'Set Prescaler bits
                    if Tx_PR = 4  then SET T6CKPS1 ON
                    if Tx_PR = 16 then SET T6CKPS2 ON
                    if Tx_PR = 64 then SET T6CKPS2 ON: SET T6CKPS1 ON
                #endif

                #ifndef bit(T6CKPS2)
                    SET T6CKPS0 OFF
                    SET T6CKPS1 OFF
                    'Set Prescaler bits
                    if Tx_PR = 4  then SET T6CKPS0 ON
                    if Tx_PR = 16 then SET T6CKPS1 ON
                    if Tx_PR = 64 then SET T6CKPS0 ON: SET T6CKPS1 ON
                #endif

                'Clearing IF flag.
                SET TMR6IF OFF

                'Start Timer6
                SET TMR6ON ON
            #endif
        #endif

    End Select

  #ifdef HPWM_FAST
          PWMFreqOld = PWMFreq
          PWMTimerOld = TimerSelected
          'Cache the PRx_Temp to support HPWM_FAST.
          PRx_Temp_Cache = PRx_Temp
      Else
        'As wierd as this looks.... restore the cache to PRX_temp as we need it when in HPWM_Fast mode
        'PRx_Temp is used in channel setting section, so, if we did not restore we would not have a consistent set of information.
        PRx_Temp = PRx_Temp_Cache
      End if
  #endif

 TenBitPMWSection:
'This section setups up the PWM Duty

 'The following code can be optimised via 'defines'

    Dim PRx_Temp_Cache as Long

    'Cache the PRx_Temp value ONLY when NOT HPWM_FAST - this can be then used by HPWMUpdate
    #ifndef HPWM_FAST
        PRx_Temp_Cache = PRx_Temp
    #endif


    #ifdef USE_HPWM1 TRUE

      #ifdef AddHPWMSetup1
        AddHPWMSetup1
      #endif

      #if var(PWM1DCH)   'If no channel.... no-point in compiling the code

        if PWMChannel = 1 then  'in section USE_HPWM1
            ' calculates duty, assisgns duty to  bits 15-8 and 7-6 of PMWxDH(H&L) and links this PWM to the correct timer
            calculateDuty 'Sets PRx_Temp  to the duty value for bits 15-8 and 7-6
            PWM1DCH = PRx_Temp_H
            PWM1DCL = PRx_Temp

            ' Select timer by updating CCPTMRS register
            #if ChipPWMTimerVariant = 1
              TimerSelectionBits =  (TimerSelected / 2 )-1 'This is a ChipPWMTimerVariant chip.  Move the timer bits to the left by one bit to cater for different addressing

              SetWith ( P1TSEL0, TimerSelectionBits.0 )
              SetWith ( P1TSEL1, TimerSelectionBits.1 )
            #endif

            #ifndef ChipPWMTimerVariant

              #IFDEF BIT(P1TSEL0)
                SetWith ( P1TSEL0, TimerSelected.1 )
                SetWith ( P1TSEL1, TimerSelected.2 )
              #ENDIF

              #IFDEF BIT(PWM1OE)
                PWM1OE = 1
              #ENDIF

            #endif

            #IFDEF BIT(PWM1EN) 'this simply stops error messages when the does not exit
              'Start PMW1
              Set PWM1EN On
            #ENDIF

        End if

      #endif

      #ifdef AddHPWMExit1
        AddHPWMExit1
      #endif

    #endif

    #ifdef USE_HPWM2 TRUE

      #ifdef AddHPWMSetup2
        AddHPWMSetup2
      #endif

      #if var(PWM2DCH)   'If no channel.... no-point in compiling the code

        if PWMChannel = 2 then  'in section USE_HPWM2
            ' calculates duty, assisgns duty to  bits 15-8 and 7-6 of PMWxDH(H&L) and links this PWM to the correct timer
            calculateDuty 'Sets PRx_Temp  to the duty value for bits 15-8 and 7-6
            PWM2DCH = PRx_Temp_H
            PWM2DCL = PRx_Temp

            ' Select timer by updating CCPTMRS register
            #if ChipPWMTimerVariant = 1
              TimerSelectionBits =  (TimerSelected / 2 )-1 'This is a ChipPWMTimerVariant chip.  Move the timer bits to the left by one bit to cater for different addressing

              SetWith ( P2TSEL0, TimerSelectionBits.0 )
              SetWith ( P2TSEL1, TimerSelectionBits.1 )
            #endif

            #ifndef ChipPWMTimerVariant

              #IFDEF BIT(P2TSEL0)
                SetWith ( P2TSEL0, TimerSelected.1 )
                SetWith ( P2TSEL1, TimerSelected.2 )
              #ENDIF

              #IFDEF BIT(PWM2OE)
                PWM2OE = 1
              #ENDIF

            #endif

            #IFDEF BIT(PWM2EN) 'this simply stops error messages when the does not exit
              'Start PMW2
              Set PWM2EN On
            #ENDIF

        End if

      #endif

      #ifdef AddHPWMExit2
        AddHPWMExit2
      #endif

    #endif

    #ifdef USE_HPWM3 TRUE

      #ifdef AddHPWMSetup3
        AddHPWMSetup3
      #endif

      #if var(PWM3DCH)   'If no channel.... no-point in compiling the code

        if PWMChannel = 3 then  'in section USE_HPWM3
            ' calculates duty, assisgns duty to  bits 15-8 and 7-6 of PMWxDH(H&L) and links this PWM to the correct timer
            calculateDuty 'Sets PRx_Temp  to the duty value for bits 15-8 and 7-6
            PWM3DCH = PRx_Temp_H
            PWM3DCL = PRx_Temp

            ' Select timer by updating CCPTMRS register
            #if ChipPWMTimerVariant = 1
              TimerSelectionBits =  (TimerSelected / 2 )-1 'This is a ChipPWMTimerVariant chip.  Move the timer bits to the left by one bit to cater for different addressing

              SetWith ( P3TSEL0, TimerSelectionBits.0 )
              SetWith ( P3TSEL1, TimerSelectionBits.1 )
            #endif

            #ifndef ChipPWMTimerVariant
              SetWith ( P3TSEL0, TimerSelected.1 )
              SetWith ( P3TSEL1, TimerSelected.2 )
            #endif

            #IFDEF BIT(PWM3EN) 'this simply stops error messages when the does not exit
              'Start PMW3
              Set PWM3EN On
            #ENDIF

        End if

      #endif

      #ifdef AddHPWMExit3
        AddHPWMExit3
      #endif

    #endif


    #ifdef USE_HPWM4 TRUE

      #ifdef AddHPWMSetup4
        AddHPWMSetup4
      #endif

      #if var(PWM4DCH)   'If no channel.... no-point in compiling the code

        if PWMChannel = 4 then  'in section USE_HPWM4
            ' calculates duty, assisgns duty to  bits 15-8 and 7-6 of PMWxDH(H&L) and links this PWM to the correct timer
            calculateDuty 'Sets PRx_Temp  to the duty value for bits 15-8 and 7-6
            PWM4DCH = PRx_Temp_H
            PWM4DCL = PRx_Temp

            ' Select timer by updating CCPTMRS1 register
            #if ChipPWMTimerVariant = 1
              TimerSelectionBits =  (TimerSelected / 2 )-1 'This is a ChipPWMTimerVariant chip.  Move the timer bits to the left by one bit to cater for different addressing

              SetWith ( P4TSEL0, TimerSelectionBits.0 )
              SetWith ( P4TSEL1, TimerSelectionBits.1 )
            #endif

            #ifndef ChipPWMTimerVariant
              SetWith ( P4TSEL0, TimerSelected.1 )
              SetWith ( P4TSEL1, TimerSelected.2 )
            #endif

            #IFDEF BIT(PWM4EN) 'this simply stops error messages when the does not exit
              'Start PMW4
              Set PWM4EN On
            #ENDIF

        End if

      #endif

      #ifdef AddHPWMExit4
        AddHPWMExit4
      #endif

    #endif


    #ifdef USE_HPWM5 TRUE

      #ifdef AddHPWMSetup5
        AddHPWMSetup5
      #endif

      #if var(PWM5DCH)   'If no channel.... no-point in compiling the code

        if PWMChannel = 5 then  'in section USE_HPWM5
            ' calculates duty, assisgns duty to  bits 15-8 and 7-6 of PMWxDH(H&L) and links this PWM to the correct timer
            calculateDuty 'Sets PRx_Temp  to the duty value for bits 15-8 and 7-6
            PWM5DCH = PRx_Temp_H
            PWM5DCL = PRx_Temp

            ' Select timer by updating CCPTMRS1 register
            #ifdef bit(P5TSEL0)
              SetWith ( P5TSEL0, TimerSelected.1 )
              SetWith ( P5TSEL1, TimerSelected.2 )
            #endif

            #IFDEF BIT(PWM5EN) 'this simply stops error messages when the does not exit
              'Start PMW5
              Set PWM5EN On
            #ENDIF

        End if

      #endif

      #ifdef AddHPWMExit5
        AddHPWMExit5
      #endif

    #endif


    #ifdef USE_HPWM6 TRUE

      #ifdef AddHPWMSetup6
        AddHPWMSetup6
      #endif

      #if var(PWM6DCH)   'If no channel.... no-point in compiling the code

        if PWMChannel = 6 then  'in section USE_HPWM6
            ' calculates duty, assisgns duty to  bits 15-8 and 7-6 of PMWxDH(H&L) and links this PWM to the correct timer
            calculateDuty 'Sets PRx_Temp  to the duty value for bits 15-8 and 7-6
            PWM6DCH = PRx_Temp_H
            PWM6DCL = PRx_Temp

            ' Select timer by updating CCPTMRS1 register
            #ifdef bit(P6TSEL0)
              SetWith ( P6TSEL0, TimerSelected.1 )
              SetWith ( P6TSEL1, TimerSelected.2 )
            #endif

            #IFDEF BIT(PWM6EN) 'this simply stops error messages when the does not exit
              'Start PMW6
              Set PWM6EN On
            #ENDIF


        End if

      #endif

      #ifdef AddHPWMExit6
        AddHPWMExit6
      #endif

    #endif


    #ifdef USE_HPWM7 TRUE

      #ifdef AddHPWMSetup7
        AddHPWMSetup7
      #endif

      #if var(PWM7DCH)   'If no channel.... no-point in compiling the code

        if PWMChannel = 7 then   'in section USE_HPWM7

            ' calculates duty, assisgns duty to  bits 15-8 and 7-6 of PMWxDH(H&L) and links this PWM to the correct timer
            calculateDuty 'Sets PRx_Temp  to the duty value for bits 15-8 and 7-6
            PWM7DCH = PRx_Temp_H
            PWM7DCL = PRx_Temp

            ' Select timer by updating CCPTMRS1 register
            #ifdef bit(P7TSEL0)
              SetWith ( P7TSEL0, TimerSelected.1 )
              SetWith ( P7TSEL1, TimerSelected.2 )
            #endif
            #IFDEF BIT(PWM7EN) 'this simply stops error messages when the does not exit
              'Start PMW7
              Set PWM7EN On
            #ENDIF

        end if

      #endif

      #ifdef AddHPWMExit7
        AddHPWMExit7
      #endif

    #endif


end sub


' Update the Duty Only.
' Assumes that PRx_Temp_Cache  and PWMResolution are defined by previous calls to the non-CCP HPWM method
Sub HPWMUpdate (In PWMChannel, in PWMDuty as WORD  )
    'The following code can be optimised via 'defines'

    dim PRx_Temp as long
    dim PWMDuty as word
    dim PWMResolution as word

    dim PRx_Temp as long
    dim PWMDuty as word

    #ifdef USE_HPWM3 TRUE

      #ifdef AddHPWMUpdate3
        AddHPWMUpdate3
      #endif

      #if var(PWM3DCH)   'If no channel.... no-point in compiling the code

        if PWMChannel = 3 then  'in section USE_HPWM3

            PRx_Temp = PRx_Temp_Cache
            ' calculates duty, assisgns duty to  bits 15-8 and 7-6 of PMWxDH(H&L) and links this PWM to the correct timer
            calculateDuty 'Sets PRx_Temp  to the duty value for bits 15-8 and 7-6
            'assumes PRx_Temp and PWMDuty are valid
            PWM3DCH = PRx_Temp_H
            PWM3DCL = PRx_Temp

        End if

      #endif

      #ifdef AddHPWMExit3
        AddHPWMExit3
      #endif

    #endif


    #ifdef USE_HPWM4 TRUE

      #ifdef AddHPWMUpdate4
        AddHPWMUpdate4
      #endif

      #if var(PWM4DCH)   'If no channel.... no-point in compiling the code

        if PWMChannel = 4 then  'in section USE_HPWM4
            ' calculates duty, assisgns duty to  bits 15-8 and 7-6 of PMWxDH(H&L) and links this PWM to the correct timer
            calculateDuty 'Sets PRx_Temp  to the duty value for bits 15-8 and 7-6
            PWM4DCH = PRx_Temp_H
            PWM4DCL = PRx_Temp

        End if

      #endif

      #ifdef AddHPWMExit4
        AddHPWMExit4
      #endif

    #endif


    #ifdef USE_HPWM5 TRUE

      #ifdef AddHPWMUpdate5
        AddHPWMUpdate5
      #endif

      #if var(PWM5DCH)   'If no channel.... no-point in compiling the code

        if PWMChannel = 5 then  'in section USE_HPWM5
            ' calculates duty, assisgns duty to  bits 15-8 and 7-6 of PMWxDH(H&L) and links this PWM to the correct timer
            calculateDuty 'Sets PRx_Temp  to the duty value for bits 15-8 and 7-6
            PWM5DCH = PRx_Temp_H
            PWM5DCL = PRx_Temp

        End if

      #endif

      #ifdef AddHPWMExit5
        AddHPWMExit5
      #endif

    #endif


    #ifdef USE_HPWM6 TRUE

      #ifdef AddHPWMUpdate6
        AddHPWMUpdate6
      #endif

      #if var(PWM6DCH)   'If no channel.... no-point in compiling the code

        if PWMChannel = 6 then  'in section USE_HPWM6
            ' calculates duty, assisgns duty to  bits 15-8 and 7-6 of PMWxDH(H&L) and links this PWM to the correct timer
            calculateDuty 'Sets PRx_Temp  to the duty value for bits 15-8 and 7-6
            PWM6DCH = PRx_Temp_H
            PWM6DCL = PRx_Temp

        End if

      #endif

      #ifdef AddHPWMExit6
        AddHPWMExit6
      #endif

    #endif


    #ifdef USE_HPWM7 TRUE

      #ifdef AddHPWMUpdate7
        AddHPWMUpdate7
      #endif

      #if var(PWM7DCH)   'If no channel.... no-point in compiling the code

        if PWMChannel = 7 then   'in section USE_HPWM7

            ' calculates duty, assisgns duty to  bits 15-8 and 7-6 of PMWxDH(H&L) and links this PWM to the correct timer
            calculateDuty 'Sets PRx_Temp  to the duty value for bits 15-8 and 7-6
            PWM7DCH = PRx_Temp_H
            PWM7DCL = PRx_Temp

        end if

      #endif

      #ifdef AddHPWMExit7
        AddHPWMExit7
      #endif

    #endif


end sub



'''@Hidden
' Simply calculates the value of PRx_Temp from the Duty and PRx_Temp (destroying the value of PRx_Temp hence we need the variable PRx_Temp_Cache)
sub calculateDuty
    dim PRx_Temp as long
    dim PWMDuty as word
    dim PWMResolution as word

    'assumes PRx_Temp and PWMDuty are valid
    PRx_Temp = PWMDuty  * ( PRx_Temp + 1 )

    Set C off
    Repeat 2
      rotate PRx_Temp left
    End Repeat
    PRx_Temp = PRx_Temp / PWMResolution    'where PWMResolution is 255 or 1023

    Set C off
    Repeat 6
      rotate PRx_Temp left
    End Repeat

    'exits with PRx_Temp calculated
end sub


sub AVRHPWM (In AVRPWMChannel, In AVRPWMFreq, IN AVRPWMDuty)

Dim  ICRxTemp as Word   'ChipMHz <= 64

    'AVR fast pwm has a small spike on period register rollover
    If AVRPWMDuty = 0 Then
      AVRPWMOff (AVRPWMChannel)
      exit sub
    End If

    If AVRPWMFreq <> AVRPWMFreqOld Then

      ICRxTemp = ((chipMHZ*1000) / AVRPWMFreq) - 1
      PWMPrescale = 1
      If ICRxTemp > 255 Then
        ICRxTemp = ICRxTemp / 8
        PWMPrescale = 8
        If ICRxTemp > 255 Then
          ICRxTemp = ICRxTemp / 8
          PWMPrescale = 64
        End If
          If ICRxTemp > 255 Then
            'error msg(BadHPWMFreq)
          End If
      End If
      AVRPWMFreqOld = AVRPWMFreq

    End If

    AVRPWMDuty = (AVRPWMDuty * ICRxTemp) / 255
    'Set Timer prescale and period, Channel Duty Cycle,
    'and then enable channel
    #IFDEF AVRTC0
      AVRTimer = 0
      AVRSetPrescale (AVRTimer)
      #IFDEF AVRCHAN2   'OC0B
        IF  AVRPWMChannel = 2 Then
          #IFNDEF VAR(OCR0)
            #IFDEF VAR(OCR0AL)    'ICR0L
              'ATtiny10
              OCR0AL = ICRxTemp
              OCR0BL = AVRPWMDuty
            #ENDIF

            #IFNDEF VAR(OCR0AL)
              'OCR0A is used as the period register
              OCR0A = ICRxTemp
              OCR0B = AVRPWMDuty
            #ENDIF
          #ENDIF

          #IFDEF VAR(OCR0)   'OC2
            '8bit single channel devices
            'no period register like mega128
            'error msg(TMR2 HPWMNot Available)
          #ENDIF
          Set COM0B1 On
          Set COM0B0 Off
        END IF
      #ENDIF
    #ENDIF

    #IFDEF AVRTC1
      AVRTimer = 1
      AVRSetPrescale (AVRTimer)
      #IFNDEF BIT(PWM1A)
        ICR1L = ICRxTemp
      #ENDIF

      #IFDEF BIT(PWM1A)
        #IFNDEF BIT(PWM11)
          OCR1C = ICRxTemp
        #ENDIF

        #IFDEF BIT(PWM11)
          ICR1L = ICRxTemp
        #ENDIF
      #ENDIF

      #IFDEF AVRCHAN3   'OC1A
        IF  AVRPWMChannel = 3 Then
          #IFDEF BIT(PWM1A)
            #IFNDEF BIT(PWM11)
              'ATtiny25 and ATtiny261A series
              OCR1A = AVRPWMDuty
            #ENDIF

            #IFDEF BIT(PWM11)
              'ATmega103 16bit register
              OCR1AL = AVRPWMDuty
            #ENDIF
          #ENDIF

          #IFNDEF BIT(PWM1A)
            '16bit register
            OCR1AL = AVRPWMDuty
          #ENDIF
          Set COM1A1 On
          Set COM1A0 Off
        END IF
      #ENDIF

      #IFDEF AVRCHAN4   'OC1B
        IF  AVRPWMChannel = 4 Then
          #IFDEF BIT(PWM1B)
            #IFNDEF BIT(PWM11)
              'ATtiny25 and ATtiny261A series
              OCR1B = AVRPWMDuty
            #ENDIF

            #IFDEF BIT(PWM11)
              'ATmega103 16bit register
              'no period register
              OCR1BL = AVRPWMDuty
            #ENDIF
          #ENDIF

          #IFNDEF BIT(PWM1B)
            #IFDEF VAR(ICR1H)
              OCR1BL = AVRPWMDuty
            #ENDIF
          #ENDIF
          Set COM1B1 ON
          Set COM1B0 OFF
        END IF
      #ENDIF

      #IFDEF AVRCHAN5   'OC1C larger megas and OC1D larger/newer tinys
        If AVRPWMChannel = 5 Then
          #IFDEF BIT(PWM1D)
            'ATtiny261A series
            OCR1D = AVRPWMDuty
            Set COM1D1 ON
            Set COM1D0 OFF
          #ENDIF

          #IFNDEF BIT(PWM1D)
            OCR1CL = AVRPWMDuty
            Set COM1C1 ON
            Set COM1C0 OFF
          #ENDIF
        End If
      #ENDIF
    #ENDIF

    #IFDEF AVRTC2
      AVRTimer = 2
      AVRSetPrescale (AVRTimer)
      #IFDEF AVRCHAN7   'OC2B
        IF  AVRPWMChannel = 7 Then
          #IFNDEF OCR2
          'OCR2A is used as the period register
            OCR2A = ICRxTemp
            OCR2B = AVRPWMDuty
            Set COM2B1 ON
            Set COM2B0 OFF
          #ENDIF

          #IFDEF OCR2   'OC2
            '8bit single channel devices
            'no period register like mega128
            'error msg(TMR2 HPWMNot Available)
          #ENDIF
        END IF
      #ENDIF
    #ENDIF

    #IFDEF AVRTC3
      AVRTimer = 3
      AVRSetPrescale (AVRTimer)
      ICR3L = ICRxTemp
      #IFDEF AVRCHAN8   'OC3A
        IF  AVRPWMChannel = 8 Then
          OCR3AL = AVRPWMDuty
          Set COM3A1 ON
          Set COM3A0 OFF
        END IF
      #ENDIF

      #IFDEF AVRCHAN9   'OC3B
        IF  AVRPWMChannel = 9 Then
          OCR3BL = AVRPWMDuty
          Set COM3B1 ON
          Set COM3B0 OFF
        END IF
      #ENDIF

      #IFDEF AVRCHAN10    'OC3C
        IF  AVRPWMChannel = 10 Then
          OCR3CL = AVRPWMDuty
          Set COM3C1 ON
          Set COM3C0 OFF
        END IF
      #ENDIF
    #ENDIF

    #IFDEF AVRTC4
      AVRTimer = 4
      AVRSetPrescale (AVRTimer)
      ICR4L = ICRxTemp
      #IFDEF AVRCHAN11   'OC4A
        IF  AVRPWMChannel = 11 Then
          OCR4AL = AVRPWMDuty
          Set COM4A1 ON
          Set COM4A0 OFF
        END IF
      #ENDIF

      #IFDEF AVRCHAN12   'OC4B
        IF  AVRPWMChannel = 12 Then
          OCR4BL = AVRPWMDuty
          Set COM4B1 ON
          Set COM4B0 OFF
        END IF
      #ENDIF

      #IFDEF AVRCHAN13    'OC4C
        IF  AVRPWMChannel = 13 Then
          OCR4CL = AVRPWMDuty
          Set COM4C1 ON
          Set COM4C0 OFF
        END IF
      #ENDIF
    #ENDIF

    #IFDEF AVRTC5
      AVRTimer = 5
      AVRSetPrescale (AVRTimer)
      ICR5L = ICRxTemp
      #IFDEF AVRCHAN14   'OC5A
        IF  AVRPWMChannel = 14 Then
          OCR5AL = AVRPWMDuty
          Set COM5A1 ON
          Set COM5A0 OFF
        END IF
      #ENDIF

      #IFDEF AVRCHAN15   'OC5B
        IF  AVRPWMChannel = 15 Then
          OCR5BL = AVRPWMDuty
          Set COM5B1 ON
          Set COM5B0 OFF
        END IF
      #ENDIF

      #IFDEF AVRCHAN16    'OC5C
        IF  AVRPWMChannel = 16 Then
          OCR5CL = AVRPWMDuty
          Set COM5C1 ON
          Set COM5C0 OFF
        END IF
      #ENDIF
    #ENDIF

end sub


sub AVRSetHPWMMode

  'AVR HPWM makes use of the Fast PWM mode by setting WGMnx bits
  'TC1,TC3,TC4,TC5 can have OCR1C and ICRxL as TOP
  'TC0 and TC2 have OCR0A and OCR2A as TOP
  #IFDEF AVRTC0
    #IFDEF BIT(WGM02)
      #IFDEF VAR(OCR0)
        '8bit single channel devices
        'no period register like mega128
        'error msg(TMR0 HPWMNot Available)
      #ENDIF

      #IFNDEF VAR(OCR0)
        #IFNDEF VAR(OCR0AL)
          '8bit 2 channel devices
          TCCR0B = b'00001000'
          TCCR0A = b'00000011'
        #ENDIF

        #IFDEF VAR(OCR0AL)
          'ATtiny10
          '16bit 2 channel devices
          TCCR0B = b'00011000'
          TCCR0A = b'00000011'
        #ENDIF
      #ENDIF
    #ENDIF

    #IFNDEF BIT(WGM02)
      'no period register like ATtiny261A series
      'error msg(TMR0 HPWMNot Available)
    #ENDIF
  #ENDIF

  #IFDEF AVRTC1
    #IFDEF BIT(PWM1A)
      #IFDEF BIT(PWM11)
        'ATmega103 8bit mode
        SET PWM11 OFF
        SET PWM10 ON
      #ENDIF

      'ATtiny25, ATtiny261A  series
      #IFDEF AVRCHAN3
        SET PWM1A ON
      #ENDIF

      #IFDEF AVRCHAN4
        SET PWM1B ON
      #ENDIF

      'ATtiny261A  series
      #IFDEF PWM1D
        SET WGM11 ON
        SET WGM10 OFF
      #ENDIF

      #IFDEF AVRCHAN5
        SET PWM1D ON
      #ENDIF

    #ENDIF

    #IFNDEF BIT(PWM1A)
      '16 bit, 16 mode devices like ATmega328p series
      'Fast PWM
      TCCR1B = b'00011000'
      TCCR1A = b'00000010'
    #ENDIF
  #ENDIF

  #IFDEF AVRTC2
    #IFDEF BIT(WGM22)
      #IFDEF OCR2
        '8bit single channel devices
        'no period register like mega128
        'error msg(TMR2 HPWMNot Available)
      #ENDIF

      #IFNDEF OCR2
        #IFNDEF ICR2L
          '8bit 2 channel devices
          TCCR2B = b'00001000'
          TCCR2A = b'00000011'
        #ENDIF

        #IFDEF ICR2L
          'ATtiny102
          '16bit 2 channel devices
          TCCR2B = b'00011000'
          TCCR2A = b'00000010'
        #ENDIF
      #ENDIF
    #ENDIF

    #IFNDEF BIT(WGM22)
      'no period register like ATtiny261A series
      'error msg(TMR2 HPWMNot Available)
    #ENDIF
  #ENDIF

  #IFDEF AVRTC3
    TCCR3B = b'00011000'
    TCCR3A = b'00000010'
  #ENDIF

  #IFDEF AVRTC4
    TCCR4B = b'00011000'
    TCCR4A = b'00000010'
  #ENDIF

  #IFDEF AVRTC5
    TCCR5B = b'00011000'
    TCCR5A = b'00000010'
  #ENDIF

end sub

sub AVRSetPrescale (AVRTimer)

  'AVR prescale applied to timers
  If PWMPrescale = 1 Then
    #IFDEF AVRTC0
      If AVRTimer = 0 Then
        lds SysValueCopy,TCCR0B
        andi SysValueCopy, 0xf8
        inc SysValueCopy
        sts TCCR0B, SysValueCopy
      End If
    #ENDIF

    #IFDEF AVRTC1
      If AVRTimer = 1 Then
        #IFDEF BIT(PWM1A)
          #IFNDEF (PWM11)
            lds SysValueCopy,TCCR1
            andi SysValueCopy, 0xf0
            inc SysValueCopy
            sts TCCR1, SysValueCopy
          #ENDIF

          #IFDEF (PWM11)
            lds SysValueCopy,TCCR1B
            andi SysValueCopy, 0xf8
            inc SysValueCopy
            sts TCCR1B, SysValueCopy
          #ENDIF
        #ENDIF

        #IFNDEF BIT(PWM1A)
          #IFNDEF BIT(CS13)
            lds SysValueCopy,TCCR1B
            andi SysValueCopy, 0xf8
            inc SysValueCopy
            sts TCCR1B, SysValueCopy
          #ENDIF

          #IFDEF BIT(CS13)
            lds SysValueCopy,TCCR1B
            andi SysValueCopy, 0xf0
            inc SysValueCopy
            sts TCCR1B, SysValueCopy
          #ENDIF
        #ENDIF
      End If
    #ENDIF

    #IFDEF AVRTC2
      If AVRTimer = 2 Then
        lds SysValueCopy,TCCR2B
        andi SysValueCopy, 0xf8
        inc SysValueCopy
        sts TCCR2B, SysValueCopy
      End If
    #ENDIF

    #IFDEF AVRTC3
      If AVRTimer = 3 Then
        lds SysValueCopy,TCCR3B
        andi SysValueCopy, 0xf8
        inc SysValueCopy
        sts TCCR3B, SysValueCopy
      End If
    #ENDIF

    #IFDEF AVRTC4
      If AVRTimer = 4 Then
        lds SysValueCopy,TCCR4B
        andi SysValueCopy, 0xf8
        inc SysValueCopy
        sts TCCR4B, SysValueCopy
      End If
    #ENDIF

    #IFDEF AVRTC5
      If AVRTimer = 5 Then
        lds SysValueCopy,TCCR5B
        andi SysValueCopy, 0xf8
        inc SysValueCopy
        sts TCCR5B, SysValueCopy
      End If
    #ENDIF
  End If

  If PWMPrescale = 8 Then
    #IFDEF AVRTC0
      If AVRTimer = 0 Then
        lds SysValueCopy,TCCR0B
        andi SysValueCopy, 0xf8
        ori SysValueCopy, 0x02
        sts TCCR0B, SysValueCopy
      End If
    #ENDIF

    #IFDEF AVRTC1
      If AVRTimer = 1 Then
        #IFDEF BIT(PWM1A)
          #IFNDEF (PWM11)
            lds SysValueCopy,TCCR1
            andi SysValueCopy, 0xf0
            ori SysValueCopy, 0x04
            sts TCCR1, SysValueCopy
          #ENDIF

          #IFDEF (PWM11)
            lds SysValueCopy,TCCR1B
            andi SysValueCopy, 0xf0
            ori SysValueCopy, 0x04
            sts TCCR1B, SysValueCopy
          #ENDIF
        #ENDIF

        #IFNDEF BIT(PWM1A)
          #IFNDEF BIT(CS13)
            lds SysValueCopy,TCCR1B
            andi SysValueCopy, 0xf8
            ori SysValueCopy, 0x02
            sts TCCR1B, SysValueCopy
          #ENDIF

          #IFDEF BIT(CS13)
            lds SysValueCopy,TCCR1B
            andi SysValueCopy, 0xf0
            ori SysValueCopy, 0x04
            sts TCCR1B, SysValueCopy
          #ENDIF
        #ENDIF
      End If
    #ENDIF

    #IFDEF AVRTC2
      If AVRTimer = 2 Then
        lds SysValueCopy,TCCR2B
        andi SysValueCopy, 0xf8
        ori SysValueCopy, 0x02
        sts TCCR2B, SysValueCopy
      End If
    #ENDIF

    #IFDEF AVRTC3
      If AVRTimer = 3 Then
        lds SysValueCopy,TCCR3B
        andi SysValueCopy, 0xf8
        ori SysValueCopy, 0x02
        sts TCCR3B, SysValueCopy
      End If
    #ENDIF

    #IFDEF AVRTC4
      If AVRTimer = 4 Then
        lds SysValueCopy,TCCR4B
        andi SysValueCopy, 0xf8
        ori SysValueCopy, 0x02
        sts TCCR4B, SysValueCopy
      End If
    #ENDIF

    #IFDEF AVRTC5
      If AVRTimer = 5 Then
        lds SysValueCopy,TCCR5B
        andi SysValueCopy, 0xf8
        ori SysValueCopy, 0x02
        sts TCCR5B, SysValueCopy
      End If
    #ENDIF
  End If

  If PWMPrescale = 64 Then
    #IFDEF AVRTC0
      If AVRTimer = 0 Then
        lds SysValueCopy,TCCR0B
        andi SysValueCopy, 0xf8
        ori SysValueCopy, 0x03
        sts TCCR0B, SysValueCopy
      End If
    #ENDIF

    #IFDEF AVRTC1
      If AVRTimer = 1 Then
        #IFDEF BIT(PWM1A)
          #IFNDEF (PWM11)
            lds SysValueCopy,TCCR1
            andi SysValueCopy, 0xf0
            ori SysValueCopy, 0x07
            sts TCCR1, SysValueCopy
          #ENDIF

          #IFDEF (PWM11)
            lds SysValueCopy,TCCR1B
            andi SysValueCopy, 0xf0
            ori SysValueCopy, 0x07
            sts TCCR1B, SysValueCopy
          #ENDIF
        #ENDIF

        #IFNDEF BIT(PWM1A)
          #IFNDEF BIT(CS13)
            lds SysValueCopy,TCCR1B
            andi SysValueCopy, 0xf8
            ori SysValueCopy, 0x03
            sts TCCR1B, SysValueCopy
          #ENDIF

          #IFDEF BIT(CS13)
            lds SysValueCopy,TCCR1B
            andi SysValueCopy, 0xf0
            ori SysValueCopy, 0x07
            sts TCCR1B, SysValueCopy
          #ENDIF
        #ENDIF
      End If
    #ENDIF

    #IFDEF AVRTC2
      If AVRTimer = 2 Then
        lds SysValueCopy,TCCR2B
        andi SysValueCopy, 0xf8
        ori SysValueCopy, 0x04
        sts TCCR2B, SysValueCopy
      End If
    #ENDIF

    #IFDEF AVRTC3
      If AVRTimer = 3 Then
        lds SysValueCopy,TCCR3B
        andi SysValueCopy, 0xf8
        ori SysValueCopy, 0x03
        sts TCCR3B, SysValueCopy
      End If
    #ENDIF

    #IFDEF AVRTC4
      If AVRTimer = 4 Then
        lds SysValueCopy,TCCR4B
        andi SysValueCopy, 0xf8
        ori SysValueCopy, 0x03
        sts TCCR4B, SysValueCopy
      End If
    #ENDIF

    #IFDEF AVRTC5
      If AVRTimer = 5 Then
        lds SysValueCopy,TCCR5B
        andi SysValueCopy, 0xf8
        ori SysValueCopy, 0x03
        sts TCCR5B, SysValueCopy
      End If
    #ENDIF
  End If

end sub


sub AVRPWMOn

    Set COM0B1 ON
    Set COM0B0 OFF

end sub


sub AVRPWMOff

    Set COM0B1 OFF
    Set COM0B0 OFF

end sub


SUB AVRPWMOff (IN AVRPWMChannel)

  'channels 1 and 6 not available, see Help
  #IFDEF AVRCHAN2
    If AVRPWMChannel = 2 Then
      Set COM0B1 OFF
      Set COM0B0 OFF
    End If
  #ENDIF

  #IFDEF AVRCHAN3
    If AVRPWMChannel = 3 Then
      Set COM1A1 OFF
      Set COM1A0 OFF
    End If
  #ENDIF

  #IFDEF AVRCHAN4
    If AVRPWMChannel = 4 Then
      Set COM1B1 OFF
      Set COM1B0 OFF
    End If
  #ENDIF

  #IFDEF AVRCHAN5
    If AVRPWMChannel = 5 Then
      #IFDEF BIT(PWM1D)
        'ATtiny261A series
        Set COM1D1 OFF
        Set COM1D0 OFF
      #ENDIF

      #IFNDEF BIT(PWM1D)
        Set COM1C1 OFF
        Set COM1C0 OFF
      #ENDIF
    End If
  #ENDIF

  #IFDEF AVRCHAN7
    If AVRPWMChannel = 7 Then
      Set COM2B1 OFF
      Set COM2B0 OFF
    End If
  #ENDIF

  #IFDEF AVRCHAN8
    If AVRPWMChannel = 8 Then
        Set COM3A1 OFF
        Set COM3A0 OFF
    End If
  #ENDIF

  #IFDEF AVRCHAN9
    If AVRPWMChannel = 9 Then
        Set COM3B1 OFF
        Set COM3B0 OFF
    End If
  #ENDIF

  #IFDEF AVRCHAN10
    If AVRPWMChannel = 10 Then
        Set COM3C1 OFF
        Set COM3C0 OFF
    End If
  #ENDIF

  #IFDEF AVRCHAN11
    If AVRPWMChannel = 11 Then
        Set COM4A1 OFF
        Set COM4A0 OFF
    End If
  #ENDIF

  #IFDEF AVRCHAN12
    If AVRPWMChannel = 12 Then
        Set COM4B1 OFF
        Set COM4B0 OFF
    End If
  #ENDIF

  #IFDEF AVRCHAN13
    If AVRPWMChannel = 13 Then
        Set COM4C1 OFF
        Set COM4C0 OFF
    End If
  #ENDIF


  #IFDEF AVRCHAN14
    If AVRPWMChannel = 14 Then
        Set COM5A1 OFF
        Set COM5A0 OFF
    End If
  #ENDIF

  #IFDEF AVRCHAN15
    If AVRPWMChannel = 15 Then
        Set COM5B1 OFF
        Set COM5B0 OFF
    End If
  #ENDIF

  #IFDEF AVRCHAN16
    If AVRPWMChannel = 16 Then
        Set COM5C1 OFF
        Set COM5C0 OFF
    End If
  #ENDIF

END SUB
