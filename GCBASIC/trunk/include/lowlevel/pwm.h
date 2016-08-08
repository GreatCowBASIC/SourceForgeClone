'    Pulse Width Modulation routines for Great Cow BASIC
'    Copyright (C) 2006 - 2014 Hugh Considine
'    Copyright (C) 2015 William Roth
'    Copyright (C) 2016 Evan Venn


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
''' 14/3/16		Added support for Hardware PMW and revised CCP PWM to support 16f18855 series
''' 01/08/16  Added support for 16f18326* series
'Defaults:
#define PWM_Freq 38      'Frequency of PWM in KHz
#define PWM_Duty 50      'Duty cycle of PWM (%)

#define HPWM6 6
#define HPWM7 7


#define USE_HPWMCCP1 TRUE
#define USE_HPWMCCP2 TRUE
#define USE_HPWMCCP3 TRUE
#define USE_HPWMCCP4 TRUE

#define USE_HPWM5 TRUE
#define USE_HPWM6 TRUE
#define USE_HPWM7 TRUE

#define USE_HPWM_TIMER2 TRUE
#define USE_HPWM_TIMER4 TRUE
#define USE_HPWM_TIMER6 TRUE



#startup InitPWM



Sub InitPWM

  DIM CCPCONCache as BYTE  'Added 27.04.2105 - WMR
  CCPCONCache = 0          'Added 27.04.2015 = WMR

  Dim PRx_Temp as LONG     'Moved here from Line 144

	'Script to calculate constants required for given Frequency and Duty Cycle
	#script
 	'revised to support new chips like 16f18855
   if bit(CCP1CON_MODE0) then

    		if nobit(CCP1M0) Then
					CCP1M0 = CCP1CON_MODE0
          CCP1M1 = CCP1CON_MODE1
          CCP1M2 = CCP1CON_MODE2
          CCP1M3 = CCP1CON_MODE3

					CCP2M0 = CCP2CON_MODE0
          CCP2M1 = CCP2CON_MODE1
          CCP2M2 = CCP2CON_MODE2
          CCP2M3 = CCP2CON_MODE3

					CCP3M0 = CCP3CON_MODE0
          CCP3M1 = CCP3CON_MODE1
          CCP3M2 = CCP3CON_MODE2
          CCP3M3 = CCP3CON_MODE3

					CCP4M0 = CCP4CON_MODE0
          CCP4M1 = CCP4CON_MODE1
          CCP4M2 = CCP4CON_MODE2
          CCP4M3 = CCP4CON_MODE3
        end if

    end if

 	'revised to support new chips like 16f18326*
   if bit(CCP1MODE0) then

    		if nobit(CCP1M0) Then
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

					CCP4M0 = CCP4MODE0		'remapped
          CCP4M1 = CCP4MODE1		'remapped
          CCP4M2 = CCP4MODE2		'remapped
          CCP4M3 = CCP4MODE3    'remapped

        end if

    end if

		'remapped for consistency
    if bit(CCP4EN) then
        CCP2CON_EN = CCP2EN
        CCP3CON_EN = CCP3EN
        CCP4CON_EN = CCP4EN
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

		'DutyCycle = (PWM_Duty*10.24)*PR2Temp/1024
		DutyCycle = (PWM_Duty / 25) * (PR2Temp + 1)
		DutyCycleH = (DutyCycle AND 1020)/4
		DutyCycleL = DutyCycle AND 3

		PWMOsc1 = int(60000/(240/ChipMHz))		'This is used in the calculations
		PWMOsc4 = int(60000/(960/ChipMHz))    '*** Dead Code ***
		PWMOsc16 = int(60000/(3840/ChipMHz))  '*** Dead Code ***

	#endscript

	#ifdef PIC

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
	#endif

End Sub

sub PWMOn
	CCP1CON = CCPCONCache
end sub

sub PWMOff
	CCP1CON = 0
end sub

sub HPWM (In PWMChannel, In PWMFreq, PWMDuty)

	Dim PRx_Temp as LONG

	'If HPWM_FAST operation selected, only recalculate timer prescaler when
	'needed. Gives faster operation, but uses extra byte of RAM and may cause
	'problems if HPWM and PWMOn are used together in a program.
	'(No issues using HPWM and PWMOff in the same program with HPWM_FAST.)
	#ifdef HPWM_FAST
	    If PWMFreq <> PWMFreqOld Then
	#endif


          Tx_PR = 1   ' Commence calculations of PMW parameters using CCP in HWPMW (pwm.h) @ calcpoint
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

          SET T2CON.T2CKPS0 OFF
          SET T2CON.T2CKPS1 OFF
          if Tx_PR = 4 then SET T2CON.T2CKPS0 ON
          if Tx_PR = 16 then SET T2CON.T2CKPS1 ON
          SET T2CON.TMR2ON ON

					'Set Clock Source, if required
          #ifdef var(T2CLKCON)
          		'Set to FOSC/4 for backward compatibility
          		T2CS0 = 1
              T2CS1 = 0
              T2CS2 = 0
              T2CS3 = 0
          #endif

  #ifdef HPWM_FAST
  		PWMFreqOld = PWMFreq
      End If
	#endif

  #ifdef USE_HPWMCCP1 TRUE

    #ifdef AddHPWMCCPSetup1
			AddHPWMCCPSetup1
    #endif

    #ifdef NoVar(CCP2CON)   'We assumne there is is only 1 CCP module on Chip
      PRx_Temp = PWMDuty * (PRx_Temp + 2)   'Correction  - WMR
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

        #ifndef VAR(CCPR1H)
            PRx_Temp = PWMDuty * (PRx_Temp + 2)  'Correction  - WMR
            CCPR1L = PRx_Temp_H
            If PWMDuty = 0 Then CCPR1L = 0  ' Assure OFF at Zero - WMR
            SET CCP1M3 ON
            SET CCP1M2 ON
            SET CCP1M1 OFF
            SET CCP1M0 OFF
        #ENDIF

        #ifdef VAR(CCPR1H)
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


        #ifndef VAR(CCPR2H) 'This is the general case
            PRx_Temp = PWMDuty * ( PR2 + 2)  'Correction  - WMR
            CCPR2L = PRx_Temp_H
            If PWMDuty = 0 Then CCPR2L = 0  ' Assure OFF at Zero - WMR
            SET CCP2M3 ON
            SET CCP2M2 ON
            SET CCP2M1 OFF
            SET CCP2M0 OFF
        #ENDIF

        #ifdef VAR(CCPR2H)
						calculateDuty 'Sets PRx_Temp  to the duty value for bits 15-8 and 7-6
            CCPR2H = PRx_Temp_H
            CCPR2L = PRx_Temp
            SET CCP2M3 ON
            SET CCP2M2 ON
            SET CCP2M1 ON
            SET CCP2M0 ON
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

        #ifndef VAR(CCPR3H)
            PRx_Temp = PWMDuty * (PRx_Temp + 2)  'Correction  - WMR
            CCPR3L = PRx_Temp_H
            If PWMDuty = 0 Then CCPR3L = 0  ' Assure OFF at Zero - WMR
            SET CCP3M3 ON
            SET CCP3M2 ON
            SET CCP3M1 OFF
            SET CCP3M0 OFF
        #ENDIF

        #ifdef VAR(CCPR3H)
						calculateDuty 'Sets PRx_Temp  to the duty value for bits 15-8 and 7-6
            CCPR3H = PRx_Temp_H
            CCPR3L = PRx_Temp
            SET CCP3M3 ON
            SET CCP3M2 ON
            SET CCP3M1 ON
            SET CCP3M0 ON
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

        #ifndef VAR(CCPR4H)
            PRx_Temp = PWMDuty * (PRx_Temp + 2)  'ifndef VAR(CCPR1H) Calculate PRx_Temp value ...
            CCPR4L = PRx_Temp_H
            If PWMDuty = 0 Then CCPR4L = 0  ' Assure OFF at Zero - WMR

            SET CCP4M3 ON'These my have been remapped using a script - do check ASM and script in INITPWM
            SET CCP4M2 ON'These my have been remapped using a script - do check ASM and script in INITPWM
            SET CCP4M1 OFF'These my have been remapped using a script - do check ASM and script in INITPWM
            SET CCP4M0 OFF'These my have been remapped using a script - do check ASM and script in INITPWM
        #ENDIF

        #ifdef VAR(CCPR4H)
            calculateDuty 'Sets PRx_Temp  to the duty value for bits 15-8 and 7-6.  ifdef VAR(CCPR1H)
            CCPR4H = PRx_Temp_H
            CCPR4L = PRx_Temp
            SET CCP4M3 ON
            SET CCP4M2 ON
            SET CCP4M1 ON
            SET CCP4M0 ON
            #ifdef bit(CCP4CON_EN)
            	SET CCP4CON_EN ON    'potentially rempped, check scripts
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

end sub

#Define HPWMOff PWMOff
SUB PWMOff (IN Channel)   'Added 27.04.2015

    If Channel > 4 OR Channel < 1 then channel = 1

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
END SUB


sub HPWM (In PWMChannel, In PWMFreq, PWMDuty, in TimerSelected  )


    Dim PRx_Temp as LONG

    PRx_Temp = PWMOsc1 / PWMFreq

    ' Calculate PWM parameters
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
            T2CLKCON = 0x01

            'T2PSYNC Not Synchronized; T2MODE Software control; T2CKPOL Rising Edge; T2CKSYNC Not Synchronized
            T2HLT = 0x00

            'T2RSEL T2CKIPPS pin
            T2RST = 0x00

            'TMR2.  Holding Register for the 8-bit TMR2 Register
            T2TMR = 0x00

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
                T4CLKCON = 0x01

                'T4PSYNC Not Synchronized; T4MODE Software control; T4CKPOL Rising Edge; T4CKSYNC Not Synchronized
                T4HLT = 0x00

                'T4RSEL T4CKIPPS pin
                T4RST = 0x00

                'TMR4.  Holding Register for the 8-bit TMR4 Register
                T4TMR = 0x00

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
                T6CLKCON = 0x01

                'T6PSYNC Not Synchronized; T6MODE Software control; T6CKPOL Rising Edge; T6CKSYNC Not Synchronized
                T6HLT = 0x00

                'T6RSEL T4CKIPPS pin
                T6RST = 0x00

                'TMR6.  Holding Register for the 8-bit TMR6 Register
                T6TMR = 0x00

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

'This section setups up the PWM Duty

		'The following code can be optimised via 'defines'

    #ifdef USE_HPWM5 TRUE

      #ifdef AddHPWMSetup5
        AddHPWMSetup5
      #endif
      if PWMChannel = 5 then  'in section USE_HPWM5
					' calculates duty, assisgns duty to  bits 15-8 and 7-6 of PMWxDH(H&L) and links this PWM to the correct timer
					calculateDuty 'Sets PRx_Temp  to the duty value for bits 15-8 and 7-6
          PWM5DCH = PRx_Temp_H
          PWM5DCL = PRx_Temp

          ' Select timer by updating CCPTMRS1 register
          SetWith ( P5TSEL0, TimerSelected.1 )
          SetWith ( P5TSEL1, TimerSelected.2 )

          #IFDEF BIT(PWM5EN) 'this simply stops error messages when the does not exit
            'Start PMW5
            Set PWM5EN On
					#ENDIF

      End if

      #ifdef AddHPWMExit5
        AddHPWMExit5
      #endif

    #endif



    #ifdef USE_HPWM6 TRUE

      #ifdef AddHPWMSetup6
        AddHPWMSetup6
      #endif
      if PWMChannel = 6 then  'in section USE_HPWM6
					' calculates duty, assisgns duty to  bits 15-8 and 7-6 of PMWxDH(H&L) and links this PWM to the correct timer
					calculateDuty 'Sets PRx_Temp  to the duty value for bits 15-8 and 7-6
          PWM6DCH = PRx_Temp_H
          PWM6DCL = PRx_Temp

          ' Select timer by updating CCPTMRS1 register
          SetWith ( P6TSEL0, TimerSelected.1 )
          SetWith ( P6TSEL1, TimerSelected.2 )

          #IFDEF BIT(PWM6EN) 'this simply stops error messages when the does not exit
            'Start PMW6
            Set PWM6EN On
					#ENDIF


      End if

      #ifdef AddHPWMExit6
        AddHPWMExit6
      #endif

    #endif


    #ifdef USE_HPWM7 TRUE

      #ifdef AddHPWMSetup7
        AddHPWMSetup7
      #endif

      if PWMChannel = 7 then   'in section USE_HPWM7

					' calculates duty, assisgns duty to  bits 15-8 and 7-6 of PMWxDH(H&L) and links this PWM to the correct timer
					calculateDuty 'Sets PRx_Temp  to the duty value for bits 15-8 and 7-6
          PWM7DCH = PRx_Temp_H
          PWM7DCL = PRx_Temp

          ' Select timer by updating CCPTMRS1 register
          SetWith ( P7TSEL0, TimerSelected.1 )
          SetWith ( P7TSEL1, TimerSelected.2 )

          #IFDEF BIT(PWM7EN) 'this simply stops error messages when the does not exit
            'Start PMW7
            Set PWM7EN On
					#ENDIF

      end if

      #ifdef AddHPWMExit7
        AddHPWMExit7
      #endif

    #endif


end sub

'''@Hidden
macro calculateDuty
		'assumes PRx_Temp and PWMDuty are valid
    PRx_Temp = PWMDuty  * ( PRx_Temp + 1 )
    PRx_Temp = FnLSL ( PRx_Temp, 2 )
    PRx_Temp = PRx_Temp / 255
    PRx_Temp = FnLSL ( PRx_Temp, 6 )
end macro