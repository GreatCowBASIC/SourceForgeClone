'    Timer control routines for Great Cow BASIC
'    Copyright (C) 2006-2015 Hugh Considine, Evan Venn and William Roth

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
' 10/07/2009: Added AVR support
' 24/08/2014: Added SetTimer subroutine
' 16/08/2014: Added support for Timer4 and Timer6
' 03/06/2015: Added support for PIC Timer 5
' 12/06/2015: Added support for PIC Timer 7/8/10/12 - WMR
' 12/06/2015: Changed "cleartimer" to not stop timers 1/3/5/7 -WMR
' 12/06/2015: Added support for PICs that use SOSCEN instead of OSCEN
' 12/06/2015: Added Starttimer/Stoptimer support for Timer 0 for 18F PICS
' 12/06/2015: Added support for Timer 0 16-Bit mode
' 12/06/2015: Corrected some typos - WMR
' 24/06/2015: Corrected "Timer 0 Notes" formatting
' 25/08/2015: Added redirection of TIMER2OVERFLOW to TIMER2MATCH for handling deprecated commands - ERV
' 25/08/2015: Added Functions for Timer8/10&12 - ERV
' 25/08/2015: Removed errors and improved handles timers and added Inittimer7- ERV


'Subroutines:
' InitTimer0 (Source, Prescaler)
' InitTimer1 (Source, Prescaler)
' InitTimer2 (Prescaler, Postscaler)
' InitTimer3 (Source, Prescaler)
' InitTimer4 (Prescaler,Postascaler)
' InitTimer5 (Source, Prescaler)
' InitTimer6 (Prescaler,Postscaler)
' InitTimer7 (Source, Prescaler)
' InitTimer8 (Prescaler.Postscaler)
' InitTimer10 (Prescaler,Postscaler)
' InitTimer12 (Prescaler,Postscaler)

' ClearTimer(TimerNumber)
' StartTimer(TimerNumber)
' StopTimer(TimerNumber)
' SetTimer(TimerNumber, Value)

'**************  Timer 0 Notes ***********************
' PIC Timer0 is a free running timer on midrange and
' baseline PICs. On these chips timer 0 always runs
' and cannot be stopped.   However on most 18Fxxx PICs,
' timer 0 can be started or stopped via the T0CON.7 Bit.
' Therefore, on these chips The timer MUST be started with
' StartTimer 0.
'
'
' On Many 18F PICS, Timer 0 can be either an 8-Bit or 16-Bit
' timer.  On these chips this is determined by TC0CON.6
' (T08BIT). The timer defaults to 8-bit. To set the timer
' to 16-Bit mode the GCB source code must include the
' following line.
'
'              #define TMR0_16BIT
'
' Using 16 bit mode with the 1:256 prescaler allows for very
' long timer periods ( > 8 sec @ 8Mhz)
'*********************************************************

'Some simpler names for the timers (use to read)




#ifdef PIC

    #ifndef TMR0_16BIT   'Test for 16Bit timer0 mode
		 #define Timer0 TMR0
    #endif
    ' IF TMR0_16bit is defined in the source code
    ' a function is then used for Timer0 - WMR

    'PIC Timer1/3/5/7 are all functions

    #ifdef Var(TMR2)
        Dim Timer2 Alias TMR2
    #endif
    #ifdef Var(TMR4)
		Dim Timer4 Alias TMR4
	#endif
	#ifdef Var(TMR6)
		Dim Timer6 Alias TMR6
	#endif
	#ifdef Var(TMR8)
		Dim Timer8 Alias TMR8
	#endif
	#ifdef Var(TMR10)
		Dim Timer10 Alias TMR10
	#endif
    #ifdef Var(TMR12)
		Dim Timer12 Alias TMR12
	#endif
#endif

' Removed as functions now replace these ALIASes
'#ifdef AVR
'	Dim Timer0 Alias TCNT0
'	Dim Timer2 Alias TCNT2
'#endif

'Sources
#define Osc 1
#define Ext 2
#define ExtOsc 3


' timer for AVR - ERV

'timer 0
  		#define PS_0 0							' no clock source
  		#define PS_1 1
      #define PS_8 2
      #define PS_64 3
      #define PS_256 4
      #define PS_1024 5

  		#define PS_0_0 0							' no clock source
  		#define PS_0_1 1
      #define PS_0_8 2
      #define PS_0_64 3
      #define PS_0_256 4
      #define PS_0_1024 5

    	#define PS_1_0 0							' no clock source
  		#define PS_1_1 1
      #define PS_1_8 2
      #define PS_1_64 3
      #define PS_1_256 4
      #define PS_1_1024 5

    	#define PS_3_0 0							' no clock source
  		#define PS_3_1 1
      #define PS_3_8 2
      #define PS_3_64 3
      #define PS_3_256 4
      #define PS_3_1024 5

    	#define PS_4_0 0							' no clock source
  		#define PS_4_1 1
      #define PS_4_8 2
      #define PS_4_64 3
      #define PS_4_256 4
      #define PS_4_1024 5

    	#define PS_5_0 0							' no clock source
  		#define PS_5_1 1
      #define PS_5_8 2
      #define PS_5_64 3
      #define PS_5_256 4
      #define PS_5_1024 5


#script
	if var(TCCR2) then

    	PS_2_0 = 0							' no clock source
  		PS_2_1 = 1
      PS_2_8 = 2
      PS_2_64 = 3
      PS_2_256 = 4
      PS_2_1024 = 5

	end if

	if var(TCCR2B) then

  		PS_2_0 = 0							' no clock source
  		PS_2_1 = 1
      PS_2_8 = 2
      PS_2_32 = 3
      PS_2_64	= 4
      PS_2_128 = 5
      PS_2_256 = 6
      PS_2_1024 = 7

	end if
#endscript

#define AVR_EXT_TMR_0_FE 6		'external, clock on falling edge
#define AVR_EXT_TMR_0_RE 7		'external, clock on rising edge

#define AVR_EXT_TMR_1_FE 6		'external, clock on falling edge
#define AVR_EXT_TMR_1_RE 7		'external, clock on rising edge

#define AVR_EXT_TMR_2_FE 6		'external, clock on falling edge
#define AVR_EXT_TMR_2_RE 7		'external, clock on rising edge

#define AVR_EXT_TMR_3_FE 6		'external, clock on falling edge
#define AVR_EXT_TMR_3_RE 7		'external, clock on rising edge

#define AVR_EXT_TMR_4_FE 6		'external, clock on falling edge
#define AVR_EXT_TMR_4_RE 7		'external, clock on rising edge

#define AVR_EXT_TMR_5_FE 6		'external, clock on falling edge
#define AVR_EXT_TMR_5_RE 7		'external, clock on rising edge



''AVR prescaler settings
'#define PS_1 1
'#define PS_8 2
'#define PS_64 3
'#define PS_256 4
'#define PS_1024 5
''Also worth noting here,
''0: stop timer


'Timer 0 prescales
#define PS0_1/2 0
#define PS0_1/4 1
#define PS0_1/8 2
#define PS0_1/16 3
#define PS0_1/32 4
#define PS0_1/64 5
#define PS0_1/128 6
#define PS0_1/256 7
#define PS0_2 0
#define PS0_4 1
#define PS0_8 2
#define PS0_16 3
#define PS0_32 4
#define PS0_64 5
#define PS0_128 6
#define PS0_256 7

'Timer 1 prescales
#define PS1_1/1 0
#define PS1_1/2 16
#define PS1_1/4 32
#define PS1_1/8 48
#define PS1_1 0
#define PS1_2 16
#define PS1_4 32
#define PS1_8 48

'Timer 2 prescales
#define PS2_1/1 0
#define PS2_1/4 1
#define PS2_1/16 2
#define PS2_1/64 3  'Added WMR
#define PS2_1 0
#define PS2_4 1
#define PS2_16 2
#define PS2_64 3    'Added WMR

'Timer 3 prescales
#define PS3_1/1 0
#define PS3_1/2 16
#define PS3_1/4 32
#define PS3_1/8 48
#define PS3_1 0
#define PS3_2 16
#define PS3_4 32
#define PS3_8 48

'Timer 4 prescales
#define PS4_1/1 0
#define PS4_1/4 1
#define PS4_1/16 2
#define PS4_1/64 3
#define PS4_1 0
#define PS4_4 1
#define PS4_16 2
#define PS4_64 3

'Timer 5 prescales
#define PS5_1/1 0
#define PS5_1/2 16
#define PS5_1/4 32
#define PS5_1/8 48
#define PS5_1 0
#define PS5_2 16
#define PS5_4 32
#define PS5_8 48

'Timer 6 prescales
#define PS6_1/1 0
#define PS6_1/4 1
#define PS6_1/16 2
#define PS6_1/64 3
#define PS6_1 0
#define PS6_4 1
#define PS6_16 2
#define PS6_64 3

'Timer 7 prescales -wmr
#define PS7_1/1 0
#define PS7_1/2 16
#define PS7_1/4 32
#define PS7_1/8 48
#define PS7_1 0
#define PS7_2 16
#define PS7_4 32
#define PS7_8 48

'Timer 8 prescales -wmr
#define PS8_1/1 0
#define PS8_1/4 1
#define PS8_1/16 2
#define PS8_1/64 3
#define PS8_1 0
#define PS8_4 1
#define PS8_16 2
#define PS8_64 3

'Timer 10 prescales -wmr
#define PS10_1/1 0
#define PS10_1/4 1
#define PS10_1/16 2
#define PS10_1/64 3
#define PS10_1 0
#define PS10_4 1
#define PS10_16 2
#define PS10_64 3

'Timer 12 prescales -wmr
#define PS12_1/1 0
#define PS12_1/4 1
#define PS12_1/16 2
#define PS12_1/64 3
#define PS12_1 0
#define PS12_4 1
#define PS12_16 2
#define PS12_64 3


function Timer0 As Word  'Added to support 16-bit mode
	#ifdef PIC
        #ifdef TMR0_16BIT
			#Ifdef Bit(T08BIT)
        		[byte]Timer0 = TMR0L
				Timer0_H = TMR0H
        	#endif
        #endif
    #endif

	#ifdef AVR
  	#ifdef var(TCNT0)
			[byte]Timer0 = TCNT0
    #endif
    #ifdef var(TCNT0L)
			[byte]Timer0 = TCNT0L
    #endif

		#ifdef var(TCNT0H)
			Timer0_H = TCNT0H
    #endif
    #ifdnef var(TCNT0H)
			Timer0_H = 0
    #endif
	#endif

end function

Function Timer1 As Word
	#ifdef PIC
		[byte]Timer1 = TMR1L
		Timer1_H = TMR1H
	#endif

	#ifdef AVR
  	#ifdef var(TCNT1)
			[byte]Timer1 = TCNT1
    #endif
    #ifdef var(TCNT1L)
			[byte]Timer1 = TCNT1L
    #endif

		#ifdef var(TCNT1H)
			Timer1_H = TCNT1H
    #endif
    #ifndef var(TCNT1H)
			Timer1_H = 0
    #endif
	#endif

End Function

Function Timer2 As Word

	#ifdef AVR
  	#ifdef var(TCNT2)
			[byte]Timer2 = TCNT2
    #endif
    #ifdef var(TCNT2L)
			[byte]Timer2 = TCNT2L
    #endif

		#ifdef var(TCNT2H)
			Timer2_H = TCNT2H
    #endif
    #ifndef var(TCNT2H)
			Timer2_H = 0
    #endif
	#endif

End Function

Function Timer3 As Word
	#ifdef PIC
		[byte]Timer3 = TMR3L
		Timer3_H = TMR3H
	#endif

	#ifdef AVR
  	#ifdef var(TCNT3)
			[byte]Timer3 = TCNT3
    #endif
    #ifdef var(TCNT3L)
			[byte]Timer3 = TCNT3L
    #endif

		#ifdef var(TCNT3H)
			Timer3_H = TCNT3H
    #endif
    #ifndef var(TCNT3H)
			Timer3_H = 0
    #endif
	#endif

End Function

Function Timer4 As Word

	#ifdef AVR
  	#ifdef var(TCNT4)
			[byte]Timer4 = TCNT4
    #endif
    #ifdef var(TCNT4L)
			[byte]Timer4 = TCNT4L
    #endif

		#ifdef var(TCNT4H)
			Timer4_H = TCNT4H
    #endif
    #ifndef var(TCNT4H)
			Timer4_H = 0
    #endif
	#endif

End Function

Function Timer5 As Word
	#ifdef PIC
		[byte]Timer5 = TMR5L
		Timer5_H = TMR5H
	#endif

	#ifdef AVR
  	#ifdef var(TCNT5)
			[byte]Timer5 = TCNT5
    #endif
    #ifdef var(TCNT5L)
			[byte]Timer5 = TCNT5L
    #endif

		#ifdef var(TCNT5H)
			Timer5_H = TCNT5H
    #endif
    #ifndef var(TCNT5H)
			Timer5_H = 0
    #endif
	#endif

End Function


Function Timer6 As Word
	#ifdef PIC
		[byte]Timer6 = TMR6L
		Timer6_H = TMR6H
	#endif
End Function


Function Timer7 As Word
	#ifdef PIC
		[byte]Timer7 = TMR7L
		Timer7_H = TMR7H
	#endif
End Function

Function Timer8 As Word
	#ifdef PIC
		[byte]Timer8 = TMR8L
		Timer8_H = TMR8H
	#endif
End Function

Function Timer10 As Word
	#ifdef PIC
		[byte]Timer10 = TMR10L
		Timer10_H = TMR10H
	#endif
End Function

Function Timer12 As Word
	#ifdef PIC
		[byte]Timer12 = TMR12L
		Timer12_H = TMR12H
	#endif
End Function


'Start/Clear/Stop subs
Sub StartTimer(In TMRNumber)
	#ifdef PIC

		#ifdef bit(TMR0ON)
			If TMRNumber = 0 Then
				Set TMR0ON on
			end if
		#endif

		#ifdef Var(T1CON)
			If TMRNumber = 1 Then
				Set TMR1ON On
			End If
		#endif

		#ifdef Var(T2CON)
			If TMRNumber = 2 Then
				T2CON = TMR2Pres
				Set TMR2ON On
			End If
		#endif

		#ifdef Var(T3CON)
			If TMRNumber = 3 Then
				Set TMR3ON On
			End If
		#endif

		#ifdef Var(T4CON)
			If TMRNumber = 4 Then
				T4CON = TMR4Pres
				Set TMR4ON On
			End If
		#endif

		#ifdef Var(T5CON)
			If TMRNumber = 5 Then
				Set TMR5ON On
			End If
		#endif

		#ifdef Var(T6CON)
			If TMRNumber = 6 Then
				T6CON = TMR6Pres
				Set TMR6ON On
			End If
		#endif

		#ifdef Var(T7CON)
			If TMRNumber = 7 Then
				Set TMR7ON On
			End If
		#endif

		#ifdef Var(T8CON)
			If TMRNumber = 8 Then
				T8CON = TMR8Pres
				Set TMR8ON On
			End If
		#endif

		#ifdef Var(T10CON)
			If TMRNumber = 10 Then
				T10CON = TMR10Pres
				Set TMR10ON On
			End If
		#endif
		#ifdef Var(T12CON)
			If TMRNumber = 12 Then
				T12CON = TMR12Pres
				Set TMR12ON On
			End If
		#endif
	#endif

	#ifdef AVR
		'Need to set clock select bits to 0
		#ifndef Var(TCCR0B)
			#ifdef Var(TCCR0)
			If TMRNumber = 0 Then
				TCCR0 = TMR0Pres
			End If
			#endif
		#endif

		#ifdef Var(TCCR0B)
			If TMRNumber = 0 Then
				TCCR0B = TCCR0B And 248 Or TMR0Pres
			End If
		#endif

		#ifdef Var(TCCR1B)
			If TMRNumber = 1 Then
				TCCR1B = TCCR1B And 248 Or TMR1Pres
			End If
		#endif

		#ifdef Var(TCCR2B)
			If TMRNumber = 2 Then
				TCCR2B = TCCR2B And 248 Or TMR2Post
			End If
		#endif

		#ifdef Var(TCCR2)
			If TMRNumber = 2 Then
				TCCR2 = TCCR2 And 248 Or TMR2Post
			End If
		#endif

		#ifdef Var(TCCR3B)
			If TMRNumber = 3 Then
				TCCR3B = TCCR3B And 248 Or TMR3Pres
			End If
		#endif

		#ifdef Var(TCCR4B)
			If TMRNumber = 4 Then
				TCCR4B = TCCR4B And 248 Or TMR4Pres
			End If
		#endif

		#ifdef Var(TCCR5B)
			If TMRNumber = 5 Then
				TCCR5B = TCCR5B And 248 Or TMR5Pres
			End If
		#endif

	#endif
End Sub

Sub ClearTimer (In TMRNumber)
	#ifdef PIC
		#ifndef bit(T08bit)
 			If TMRNumber = 0 Then
				 'SInitTimer0  'Unnecessary overhead ?
            	TMR0 = 0
			end If
		#endif
		#ifdef bit(T08BIT)
    		if TMRNumber = 0 then
				TRM0H = 0
				TMR0L = 0
			end if
		#endif
		#ifdef Var(T1CON)
			If TMRNumber = 1 Then
				'SInitTimer1  'Unnecessary / stops timer
				TMR1H = 0
				TMR1L = 0
			End If
		#endif
		#ifdef Var(T2CON)
			If TMRNumber = 2 Then
				'TMR2Pres = T2CON
				TMR2 = 0
				'T2CON = TMR2Pres
			End If
		#endif
		#ifdef Var(T3CON)
			If TMRNumber = 3 then
				'SInitTimer3   'Unnecessary / stops timer
				TMR3H = 0
				TMR3L = 0
			End If
		#endif
		#ifdef Var(T4CON)
			If TMRNumber = 4 Then
				'TMR4Pres = T4CON
				TMR4 = 0
				'T4CON = TMR4Pres
			End If
		#endif
		#ifdef Var(T5CON)
			If TMRNumber = 5 then
				'SInitTimer5   'Unnecessary / stops timer
				TMR5H = 0
				TMR5L = 0
			End If
		#endif
		#ifdef Var(T6CON)
			If TMRNumber = 6 Then
				'TMR6Pres = T6CON
				TMR6 = 0
				'T6CON = TMR6Pres
			End If
		#endif
		#ifdef Var(T7CON)
			If TMRNumber = 7 then
				'SInitTimer7   'Unnecessary / stops timer
				TMR7H = 0
				TMR7L = 0
			End If
		#endif
		#ifdef Var(T8CON)
			If TMRNumber = 8 Then
				'TMR8Pres = T8CON
				TMR6 = 0
				'T8CON = TMR8Pres
			End If
		#endif
		#ifdef Var(T10CON)
			If TMRNumber = 10 Then
				'TMR10Pres = T10CON
				TMR10 = 0
				'T10CON = TMR10Pres
			End If
		#endif

	#endif
	#ifdef AVR
		#ifdef Var(TCNT0)
			If TMRNumber = 0 Then
				TCNT0 = 0
			End If
		#endif
		#ifdef Var(TCNT1L)
			If TMRNumber = 1 Then
				TCNT1H = 0
				TCNT1L = 0
			End If
		#endif
		#ifdef Var(TCNT2)
			If TMRNumber = 2 Then
				TCNT2 = 0
			End If
		#endif
		#ifdef Var(TCNT3L)
			If TMRNumber = 3 Then
				TCNT3H = 0
				TCNT3L = 0
			End If
		#endif
		#ifdef Var(TCNT4L)
			If TMRNumber = 4 Then
				TCNT4H = 0
				TCNT4L = 0
			End If
		#endif
		#ifdef Var(TCNT5L)
			If TMRNumber = 5 Then
				TCNT5H = 0
				TCNT5L = 0
			End If
		#endif
	#endif
End Sub

Sub SetTimer (In TMRNumber, In TMRValue As Word)
	#ifdef PIC
		If TMRNumber = 0 Then
			TMR0 = TMRValue
		End If
		#ifdef Var(T1CON)
			If TMRNumber = 1 then
				TMR1L = 0
				TMR1H = TMRValue_H
				TMR1L = TMRValue
			End If
		#endif
		#ifdef Var(T2CON)
			If TMRNumber = 2 Then
				TMR2 = TMRValue
			End If
		#endif
		#ifdef Var(T3CON)
			If TMRNumber = 3 then
				TMR3L = 0
				TMR3H = TMRValue_H
				TMR3L = TMRValue
			End If
		#endif

		#ifdef Var(T4CON)
			If TMRNumber = 4 Then
				TMR4 = TMRValue
			End If
		#endif
		#ifdef Var(T5CON)
			If TMRNumber = 5 then
				TMR5L = 0
				TMR5H = TMRValue_H
				TMR5L = TMRValue
			End If
		#endif
		#ifdef Var(T6CON)
			If TMRNumber = 6 Then
				TMR6 = TMRValue
			End If
		#endif
		#ifdef Var(T7CON)
			If TMRNumber = 7 then
				TMR7L = 0
				TMR7H = TMRValue_H
				TMR7L = TMRValue
			End If
		#endif
        #ifdef Var(T8CON)
			If TMRNumber = 8 Then
				TMR8 = TMRValue
			End If
		#endif
		#ifdef Var(T10CON)
			If TMRNumber = 10 Then
				TMR10 = TMRValue
			End If
		#endif

		#ifdef Var(T12CON)
			If TMRNumber = 12 Then
				TMR12 = TMRValue
			End If
		#endif


	#endif
	#ifdef AVR
		#ifdef Var(TCNT0)
			If TMRNumber = 0 Then
				TCNT0 = TMRValue
			End If
		#endif
		#ifdef Var(TCNT1L)
			If TMRNumber = 1 Then
				TCNT1H = TMRValue_H
				TCNT1L = TMRValue
			End If
		#endif
		#ifdef Var(TCNT2)
			If TMRNumber = 2 Then
				TCNT2 = TMRValue
			End If
		#endif
		#ifdef Var(TCNT3L)
			If TMRNumber = 3 Then
				TCNT3H = TMRValue_H
				TCNT3L = TMRValue
			End If
		#endif
		#ifdef Var(TCNT4L)
			If TMRNumber = 4 Then
				TCNT4H = TMRValue_H
				TCNT4L = TMRValue
			End If
		#endif
		#ifdef Var(TCNT5L)
			If TMRNumber = 5 Then
				TCNT5H = TMRValue_H
				TCNT5L = TMRValue
			End If
		#endif
	#endif
End Sub

Sub StopTimer (In TMRNumber)
	#ifdef PIC
		'Timer 0 always runs
		' Not Always. Below is for PICs that
        ' Support Timer0 on/off
         #ifdef bit(TMR0ON)
           If TMRNumber = 0 Then
              Set TMR0ON off
           end if
        #endif

		If TMRNumber = 1 Then
			Set TMR1ON OFF
		End If
		#ifdef Var(T2CON)
			If TMRNumber = 2 Then
				Set TMR2ON OFF
			End If
		#endif
		#ifdef Var(T3CON)
			If TMRNumber = 3 Then
				Set TMR3ON OFF
			End If
		#endif
		#ifdef Var(T4CON)
			If TMRNumber = 4 Then
				Set TMR4ON OFF
			End If
		#endif
		#ifdef Var(T5CON)
			If TMRNumber = 5 Then
				Set TMR5ON OFF
			End If
		#endif
		#ifdef Var(T6CON)
			If TMRNumber = 6 Then
				Set TMR6ON OFF
			End If
		#endif
        #ifdef Var(T7CON)
			If TMRNumber = 7 Then
				Set TMR7ON OFF
			End If
		#endif
		#ifdef Var(T8CON)
			If TMRNumber = 8 Then
				Set TMR8ON OFF
			End If
		#endif
		#ifdef Var(T10CON)
			If TMRNumber = 10 Then
				Set TMR10ON OFF
			End If
		#endif
        #ifdef Var(T12CON)
			If TMRNumber = 12 Then
				Set TMR12ON OFF
			End If
		#endif

	#endif
	#ifdef AVR
		'Need to set clock select bits to 0
		#ifdef Var(TCCR0B)
			If TMRNumber = 0 Then
				TCCR0B = TCCR0B And 248
			End If
		#endif
		#ifdef Var(TCCR1B)
			If TMRNumber = 1 Then
				TCCR1B = TCCR1B And 248
			End If
		#endif
		#ifdef Var(TCCR2B)
			If TMRNumber = 2 Then
				TCCR2B = TCCR2B And 248
			End If
		#endif
		#ifdef Var(TCCR3B)
			If TMRNumber = 3 Then
				TCCR3B = TCCR3B And 248
			End If
		#endif
		#ifdef Var(TCCR4B)
			If TMRNumber = 4 Then
				TCCR4B = TCCR4B And 248
			End If
		#endif
		#ifdef Var(TCCR5B)
			If TMRNumber = 5 Then
				TCCR5B = TCCR5B And 248
			End If
		#endif
	#endif
End Sub

'Initialise subs for individual timers
Sub InitTimer0(In TMR0Source, In TMR0Pres)
	#ifdef PIC
		SInitTimer0
	#endif
	#ifdef AVR
		'Just need to buffer TMR0Pres
		'(And change it for external clock)
		If TMR0Source = Ext Then
			TMR0Pres = AVR_EXT_TMR_0_RE
		End If
	#endif
End Sub

Sub SInitTimer0
	#ifndef Var(T0CON)
		OPTION_REG = OPTION_REG AND 192
		OPTION_REG = OPTION_REG OR TMR0Pres
	#endif
	#ifdef Var(T0CON)
	    #ifndef bit(TMR0ON)	' Modified 14/08/2105
        	T0CON = T0CON And 192
			T0CON = T0CON OR TMR0Pres
	    #endif
        #ifdef bit(TMR0ON)  'Added for PIC 18Fxxx
            T0CON = T0CON And 64  'Don't start the timer
            T0CON = T0CON OR TMR0Pres
			#ifdef TMR0_16BIT  ' Added for TMR0 16-bit mode
               #ifdef bit(T08BIT)
					Set T08BIT OFF
                    TMR0H = 0: TMR0L = 0
            	#endif
			#endif
        #endif
    #endif
	SET PSA OFF
	if TMR0Source = Osc THEN SET T0SE OFF
	if TMR0Source = Ext THEN SET T0SE ON
	clrwdt

End Sub

Sub InitTimer1(In TMR1Source, In TMR1Pres)
	#ifdef PIC
		SInitTimer1
	#endif

  #ifdef AVR
		If TMR1Source = Ext Then
			TMR1Pres = AVR_EXT_TMR_1_RE
		End If
	#endif

End Sub

Sub SInitTimer1
	T1CON = TMR1Pres  'Will stop timer if already started !
	#Ifndef Bit(TMR1CS1)
		If TMR1Source = Ext Then
			Set TMR1CS On
		End If
		If TMR1Source = ExtOsc Then
        	Set TMR1CS On
        	#ifdef Bit(T1SOSCEN) ' Added -WMR
				Set T1SOSCEN On
        	#endif
        	#ifdef Bit(T1OSCEN)
        		set T1OSCEN on
        	end If
		#Endif
	#Endif

	#Ifdef Bit(TMR1CS1)
		If TMR1Source = Ext Then
			Set TMR1CS1 On
		End If
		If TMR1Source = ExtOsc Then
			Set TMR1CS1 On
	        #ifdef bit(T1SOSCEN) ' Added -WMR
            	Set T1SOSCEN On
            #endif
			#ifdef Bit(T1OSCEN)
                Set T1OSCEN ON
            #endif
		End If
	#Endif
End Sub



Sub InitTimer2 (In TMR2Pres, In TMR2Post)


    #ifdef PIC
    		#ifndef TIMER2OVERFLOW
          TIMER2OVERFLOW = TIMER2MATCH
        #endif
    #endif


	#ifdef PIC
		swapf TMR2Post,F
		rrf TMR2Post,W
		andlw 120
		iorwf TMR2Pres,F
	#endif



	#ifdef AVR
		'Some wacky swapping of variable names here
		If TMR2Pres = Ext Then
			TMR2Post = AVR_EXT_TMR_2_RE
		End If
	#endif

End Sub

Sub InitTimer3(In TMR3Source, In TMR3Pres)
	#ifdef PIC
		SInitTimer3
	#endif
	#ifdef AVR
		If TMR3Source = Ext Then
			TMR3Pres = AVR_EXT_TMR_3_RE
		End If
	#endif
End Sub

Sub SInitTimer3
	T3CON = TMR3Pres  'Will stop timer if already started !
	#ifndef Bit(TMR3CS1)
		If TMR3Source = Ext Then
    	#ifdef TMR3CS
				Set TMR3CS On
      #endif
		End If
		If TMR3Source = ExtOsc Then
    	#ifdef TMR3CS
				Set TMR3CS On
      #endif
			#ifdef Bit(T3SOSCEN) ' Added -WMR
				Set T3SOSCEN On
		    #endif
        	#ifdef Bit(T3OSCEN)
				Set T3OSCEN On
            #endif
		End If
	#endif

	#ifdef Bit(TMR3CS1)
		If TMR3Source = Ext Then
			Set TMR3CS1 On
		End If
		If TMR3Source = ExtOsc Then
			Set TMR3CS1 On
			#ifdef bit(T3SOSCEN) ' Added -WMR
				Set T3SOSCEN On
			#endif
			#ifdef Bit(T3OSCEN)
                Set T3OSCEN ON
            #endif
		End If
	#endif
End Sub


#ifdef PIC
  Sub InitTimer4 (In TMR4Pres, In TMR4Post)

      swapf TMR4Post,F
      rrf TMR4Post,W
      andlw 120
      iorwf TMR4Pres,F

	End Sub
#endif

#ifdef AVR
  Sub InitTimer4(In TMR4Source, In TMR4Pres)
      If TMR4Source = Ext Then
        TMR4Pres = AVR_EXT_TMR_4_RE
      End If
	End Sub
#endif


Sub InitTimer5(In TMR5Source, In TMR5Pres)
	#ifdef PIC
		SInitTimer5
	#endif

    #ifdef AVR
		If TMR5Source = Ext Then
			TMR5Pres = AVR_EXT_TMR_5_RE
		End If
	#endif
End Sub

Sub SInitTimer5
	T5CON = TMR5Pres  'Will stop timer if already started !
	#ifndef Bit(TMR5CS1)
		If TMR5Source = Ext Then
    	#ifdef TMR5CS
				Set TMR5CS On
      #endif
      #ifndef TMR5CS
				'No timer5 defined please use alternate timer
      #endif
		End If
		If TMR5Source = ExtOsc Then
    	#ifdef TMR5CS
				Set TMR5CS On
      #endif
      #ifndef TMR5CS
				'No timer5 defined please use alternate timer
      #endif
			#ifdef Bit(T5SOSCEN) ' Added -WMR
				Set T5SOSCEN On
			#endIf
			#ifdef Bit(T5OSCEN)
				Set T5OSCEN On
			#endif
    End if
	#Endif

	#ifdef Bit(TMR5CS1)
		If TMR5Source = Ext Then
			Set TMR5CS1 On
		End If
		If TMR5Source = ExtOsc Then
			Set TMR5CS1 On
	        #ifdef bit(T5SOSCEN) 'Added - WMR
            	Set T5SOSCEN On
            #endif
			#ifdef Bit(T5OSCEN)
                Set T5OSCEN ON
            #endif
		End If
	#endif
End Sub

#ifdef PIC
	Sub InitTimer6 (In TMR6Pres, In TMR6Post)
		swapf TMR6Post,F
		rrf TMR6Post,W
		andlw 120
		iorwf TMR6Pres,F
	End Sub
#endif



Sub InitTimer7(In TMR7Source, In TMR7Pres)
	#ifdef PIC
		SInitTimer7
	#endif

End Sub

Sub SInitTimer7
	T7CON = TMR7Pres  'Will stop timer if already started !
	#ifndef Bit(TMR7CS1)
		If TMR7Source = Ext Then
    	#ifdef TMR7CS
				Set TMR7CS On
      #endif
		End If
		If TMR7Source = ExtOsc Then
    	#ifdef TMR7CS
				Set TMR7CS On
      #endif
			#ifdef Bit(T7SOSCEN) ' Added -WMR
				Set T7SOSCEN On
			#endIf
			#ifdef Bit(T7OSCEN)
				Set T7OSCEN On
			#endif
    end if
	#Endif

	#ifdef Bit(TMR7CS1)
		If TMR7Source = Ext Then
			Set TMR7CS1 On
		End If
		If TMR7Source = ExtOsc Then
			Set TMR7CS1 On
	        #ifdef bit(T7SOSCEN) ' Added -WMR
            	Set T7SOSCEN On
            #endif
			#ifdef Bit(T7OSCEN)
                Set T7OSCEN ON
            #endif
		End If
	#endif
End Sub

#ifdef PIC
	Sub InitTimer8 (In TMR8Pres, In TMR8Post)
		swapf TMR8Post,F
		rrf TMR8Post,W
		andlw 120
		iorwf TMR8Pres,F
	End Sub

#endif


#ifdef PIC
	Sub InitTimer10 (In TMR10Pres, In TMR10Post)
		swapf TMR10Post,F
		rrf TMR10Post,W
		andlw 120
		iorwf TMR10Pres,F
	End Sub
#endif


#ifdef PIC
	Sub InitTimer12 (In TMR12Pres, In TMR12Post)
		swapf TMR12Post,F
		rrf TMR12Post,W
		andlw 120
		iorwf TMR12Pres,F
	End Sub
#endif

