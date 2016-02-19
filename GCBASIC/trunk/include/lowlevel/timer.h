'    Timer control routines for Great Cow BASIC
'    Copyright (C) 2006-2016 Hugh Considine, Evan Venn and William Roth
'    Release v0.95.004.2b Beta 5  (Candidate 2b)
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
' 10/10/2015: Added ifdef to protect StopTimer 1 error
' 10/10/2015: Change Stoptimer ifdef to T1CON to  TMR1ON etc. etc. TMR12ON
' 29/12/2015: Revised to resolve TMR0 16 bit and AVR addressing.
' 31/12/2015: Emergency changes to sort timer2, 4, 6, 7 8, 10 as words not byte.

' 04/01/2016: ALL PIC and ARV Timers now Aliases (no more functions)
'             Corrected/Changed  #IFEFs for 8-Bit/16-bit TIMER0
'             Corrected multiple typos -WMR
'
' 05/01/2016: Added support for ATtiny15/25/45/85/261/461/861
'             (register, prescaler, 16bit-timer0, 10bit-timer1)
'
'
' 15/01/2016  Eliminated unnecessary PIC "Sinitimer" Subs
'             Removed dead code (Some PIC Stuff)
'
' 15/01/2016  PIC Timer prescale and source settings now
'             written to TxCON register in Inittimer sub
'             instead of Starttimer.
'
' 15/01/2016  TMRPres used as mutipurpose variable. (PIC)
'
' 15/01/2016  Replaced  ASM code routines for Timers 2/4/6/8/10/12
'             to save memory (SYSTEMP variables)
'
' 15/01/2016  Added "failsafes" for some timers settings (PIC)
' 15/01/2016  Added missing Timer 12 entry (PIC Cleartimer)
'
' 15/01/2016  Corrected TYPO for AVR timer4 Alias
'
' 15/01/2016  No unique variables for TMRxSource, TMRxPres, or TMRxPost
' 15/01/2016  All Timers share TMRSource, TMRPres and TMRPost (Save Memory)
' 15/01/2016  Added (AVR)shadow variable TMRx_TMP for each timer
' 18/01/2016  Corrected Typo AVR inittimer4
' 15/02/2015  Added InitTimer0 support for baseline Pics with
'             with non-addressable / write only option_reg - WMR
' 18/02/2015  Added correct prescales for AVR with TCCR2A register -Wmr
' 18/02/2015  Added/corrected TCCR2A setting in Stop/starttimer -Wmr
'***********************************************************

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
' On most 18F PICS, Timer 0 can be either an 8-Bit or 16-Bit
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
'
 #ifdef PIC
    '*** Timer0 Test for 18F Chips ***
    #ifdef var(TMR0L)  'must be 18F w/selectable 8/16-Bit timer
        #ifdef TMR0_16BIT   'User defined 16 bit mode
              DIM Timer0 Alias TMR0H,TMR0L as Word
        #endif

        #ifndef TMR0_16BIT  'Default 8-bit mode
            DIM Timer0 Alias TMR0L as Byte
            ' TMR0 is aliased to "TMR0L" in
            '(some?) Chip datfiles. Best use TMR0L here
        #endif
    #endif

    'Test for other PICs with Timer0
    #ifdef Var(TMR0)         'Has a Timer0
        #ifndef var(TMR0H)   'And is not 16-bit
            DIM Timer0 Alias TMR0 as Byte
        #endif
    #endif

    #ifdef Var(TMR1H)  '16-bit Timer1
       DIM Timer1 Alias TMR1H,TMR1L as WORD
    #endif

    #ifdef Var(TMR2)   '8-bit Timer2
       DIM Timer2 Alias TMR2 as Byte
    #endif

    #ifdef Var(TMR3H)  '16-bit Timer3
       Dim Timer3 Alias TMR3H,TMR3L as WORD 'added "as WORD"
    #endif

    #ifdef Var(TMR4)   '8-bit Timer4
       Dim Timer4 Alias TMR4 as Byte
    #endif

    #ifdef Var(TMR5H)  '16-bit Timer5
       Dim Timer5 Alias TMR5H,TMR5L as WORD
    #endif

    #ifdef Var(TMR6)    '8-bit Timer6
       Dim Timer6 Alias TMR6 as Byte
    #endif

    #ifdef Var(TMR7H)  '16-bit Timer7
       Dim Timer7 Alias TMR7H,TMR7L as WORD
    #endif

    #ifdef Var(TMR8)    '8-bit Timer8
       Dim Timer8 Alias TMR8 as Byte
        'Timer8=0
    #endif

    #ifdef Var(TMR10)    '8-bit Timer10
       Dim Timer10 Alias TMR10 as Byte
    #endif

    #ifdef Var(TMR12)    '8-bit Timer12
       Dim Timer12 Alias TMR12 as Byte
    #endif
 #endif
     '********* END OF PIC TimerX Aliases *******************

 '******   AVR TIMER  DIM/ALIAS ****************
 #ifdef AVR

    #ifdef var(TCNT0) '8-bit Timer0
        DIM Timer0 Alias TCNT0 as Byte
    #endif

    '*** 05/01/2016 Timer0 Test for ATtiny 261/461/861 ***
    #ifdef var(TCNT0L)   'must be selectable 8/16-Bit timer
      #ifdef TMR0_16BIT  'user defined 16-bit Timer0
         DIM Timer0 Alias TCNT0H,TCNT0L as Word
      #endif
      #ifndef TMR0_16BIT  'Default 8-bit Timer0
         DIM Timer0 Alias TCNT0L as Byte
      #endif
    #endif

    #ifdef var(TCNT1)   '8-bit Timer1
       DIM Timer1 Alias TCNT1 as Byte
    #endif

    #ifdef var(TC1H)    '10-Bit Timer1 on ATtiny 261/461/861
      DIM Timer1 Alias TC1H,TCNT1 as WORD
    #endif

    #ifdef var(TCNT1L)  '16-Bit Timer1
      DIM Timer1 Alias TCNT1H,TCNT1L as WORD
    #endif

    #ifdef var(TCNT2)
       DIM Timer2 Alias TCNT2 as byte
    #endif

    #ifdef var(TCNT2L)
      DIM Timer2 Alias TCNT2H,TCNT2L as WORD
    #endif

    #ifdef var(TCNT3)
       DIM Timer3 Alias TCNT3 As Byte
    #endif

    #ifdef var(TCNT3L)
       DIM Timer3 Alias TCNT3H,TCNT3L as WORD
    #endif

    #ifdef var(TCNT4)
       DIM Timer4 Alias TCNT4 as Byte
    #endif

    #ifdef var(TCNT4L)
       DIM Timer4 Alias TCNT4H,TCNT4L as WORD 'Typo
    #endif

    #ifdef var(TCNT5)
       DIM Timer5 Alias TCNT5 as Word
    #endif

    #ifdef var(TCNT5L)
        DIM Timer5 Alias TCNT5H,TCNT5L as WORD
    #endif

 #endif

'Clock Sources
  #define Osc 1
  #define Ext 2
  #define ExtOsc 3

' timer for AVR - ERV
  'timer 0
  #define PS_0 0         ' no clock source
  #define PS_1 1
  #define PS_8 2
  #define PS_64 3
  #define PS_256 4
  #define PS_1024 5

  #define PS_0_0 0        ' no clock source
  #define PS_0_1 1
  #define PS_0_8 2
  #define PS_0_64 3
  #define PS_0_256 4
  #define PS_0_1024 5

  #define PS_1_0 0         ' no clock source
  #define PS_1_1 1
  #define PS_1_8 2
  #define PS_1_64 3
  #define PS_1_256 4
  #define PS_1_1024 5

  #define PS_3_0 0         ' no clock source
  #define PS_3_1 1
  #define PS_3_8 2
  #define PS_3_64 3
  #define PS_3_256 4
  #define PS_3_1024 5

  #define PS_4_0 0         ' no clock source
  #define PS_4_1 1
  #define PS_4_8 2
  #define PS_4_64 3
  #define PS_4_256 4
  #define PS_4_1024 5

  #define PS_5_0 0         ' no clock source
  #define PS_5_1 1
  #define PS_5_8 2
  #define PS_5_64 3
  #define PS_5_256 4
  #define PS_5_1024 5

#script

  if var(TCNT1) then  '05/01/2016 added for ATtiny15/25/45/85/216/461/861
    PS_1_0 = 0
    PS_1_1 = 1
    PS_1_2 = 2
    PS_1_4 = 3
    PS_1_8 = 4
    PS_1_16 = 5
    PS_1_32 = 6
    PS_1_64 = 7
    PS_1_128 = 8
    PS_1_256 = 9
    PS_1_512 = 10
    PS_1_1024 = 11
    PS_1_2048 = 12
    PS_1_4096 = 13
    PS_1_8192 = 11
    PS_1_16384 = 15
  end if

  'needs work- "some" AVR with TCCR2 also support 1:32 and 1:128
  if var(TCCR2) then
    PS_2_0 = 0              ' no clock source
    PS_2_1 = 1
    PS_2_8 = 2
    PS_2_64 = 3
    PS_2_256 = 4
    PS_2_1024 = 5
  end if

  if var(TCCR2B) then
    PS_2_0 = 0              ' no clock source
    PS_2_1 = 1
    PS_2_8 = 2
    PS_2_32 = 3
    PS_2_64 = 4
    PS_2_128 = 5
    PS_2_256 = 6
    PS_2_1024 = 7
  end if

  '// "ALL" AVR with TCCR2A support these prescales - WMR
  if var(TCCR2A) then
    PS_2_0 = 0              ' no clock source
    PS_2_1 = 1
    PS_2_8 = 2
    PS_2_32 = 3
    PS_2_64 = 4
    PS_2_128 = 5
    PS_2_256 = 6
    PS_2_1024 = 7
  end if


#endscript

#define AVR_EXT_TMR_0_FE 6    'external, clock on falling edge
#define AVR_EXT_TMR_0_RE 7    'external, clock on rising edge

#define AVR_EXT_TMR_1_FE 6    'external, clock on falling edge
#define AVR_EXT_TMR_1_RE 7    'external, clock on rising edge

#define AVR_EXT_TMR_2_FE 6    'external, clock on falling edge
#define AVR_EXT_TMR_2_RE 7    'external, clock on rising edge

#define AVR_EXT_TMR_3_FE 6    'external, clock on falling edge
#define AVR_EXT_TMR_3_RE 7    'external, clock on rising edge

#define AVR_EXT_TMR_4_FE 6    'external, clock on falling edge
#define AVR_EXT_TMR_4_RE 7    'external, clock on rising edge

#define AVR_EXT_TMR_5_FE 6    'external, clock on falling edge
#define AVR_EXT_TMR_5_RE 7    'external, clock on rising edge

''AVR prescaler settings
'#define PS_1 1
'#define PS_8 2
'#define PS_64 3
'#define PS_256 4
'#define PS_1024 5
''Also worth noting here,
''0: stop timer

'Timer 0 prescales (PIC)
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

'Start/Clear/Stop subs
Sub StartTimer(In TMRNumber)
  #ifdef PIC

     #ifdef bit(TMR0ON)
        IF TMRNumber = 0 then Set TMR0ON on
     #endif

     #ifdef bit(TMR1ON)
        IF TMRNumber = 1 then Set TMR1ON on
     #endif

     #ifdef bit(TMR2ON)
        IF TMRNumber = 2 then Set TMR2ON on
     #endif

     #ifdef bit(TMR3ON)
        IF TMRNumber = 3 then Set TMR3ON on
     #endif

     #ifdef bit(TMR4ON)
        IF TMRNumber = 4 then Set TMR4ON on
     #endif

     #ifdef bit(TMR5ON)
        IF TMRNumber = 5 then Set TMR5ON on
     #endif

     #ifdef bit(TMR6ON)
        IF TMRNumber = 6 then Set TMR6ON on
     #endif

     #ifdef bit(TMR7ON)
        IF TMRNumber = 7 then Set TMR7ON on
     #endif

     #ifdef bit(TMR8ON)
        IF TMRNumber = 8 then Set TMR8ON on
     #endif

     #ifdef bit(TMR10ON)
        IF TMRNumber = 10 then Set TMR10ON on
     #endif

     #ifdef bit(TMR12ON)
        IF TMRNumber = 12 then Set TMR12ON on
     #endif
  #endif

  #ifdef AVR

     'Need to set clock select bits to 0 (Stops Timer)
      #ifndef Var(TCCR0B)
        #ifdef Var(TCCR0)
           If TMRNumber = 0 Then
              TCCR0 = TMR0_TMP
            End If
        #endif
     #endif

     #ifdef Var(TCCR0B)
        If TMRNumber = 0 Then
          TCCR0B = TCCR0B And 248 Or TMR0_TMP
        End If
     #endif

     #ifdef Var(TCCR1B)
        If TMRNumber = 1 Then
           #ifndef Var(TC1H)
             TCCR1B = TCCR1B And 248 Or TMR1_TMP
           #endif

           #ifdef var(TC1H)  '05/01/2016 added for ATtiny216/461/861
               TCCR1B = TCCR1B And 240 Or TMR1_TMP  '4bit preset
           #endif
        End If
      #endif

     #ifdef Var(TCCR1)  '05/01/2016 added for ATtiny15/25/45/85
        If TMRNumber = 1 Then
          TCCR1 = TCCR1 And 240 Or TMR1_TMP  '4bit preset
        End If
     #endif

     #ifdef Var(TCCR2B)
        If TMRNumber = 2 Then
          TCCR2B = TCCR2B And 248 Or TMR2_TMP
        End If
     #endif

     #ifdef Var(TCCR2)
        If TMRNumber = 2 Then
          TCCR2 = TCCR2 And 248 Or TMR2_TMP
        End If
     #endif

      #ifdef Var(TCCR2A)
        If TMRNumber = 2 Then
          TCCR2A = TCCR2A And 248 Or TMR2_TMP
        End If
     #endif


     #ifdef Var(TCCR3B)
        If TMRNumber = 3 Then
          TCCR3B = TCCR3B And 248 Or TMR3_TMP
        End If
     #endif

     #ifdef Var(TCCR4B)
        If TMRNumber = 4 Then
          TCCR4B = TCCR4B And 248 Or TMR4_TMP
        End If
     #endif

           'Timer5
     #ifdef Var(TCCR5B)
        If TMRNumber = 5 Then
          TCCR5B = TCCR5B And 248 Or TMR5_TMP
        End If
     #endif

  #endif

End Sub

Sub ClearTimer (In TMRNumber)

  #ifdef PIC
     #ifndef Var(TMR0H)
        if TMRNumber = 0 then  TMR0 = 0
     #endif

      #ifdef Var(TMR0H)  '18F or other with 8/16 bit timer
        if TMRNumber = 0 then
            TMR0H = 0
            TMR0L = 0
         end if
     #endif

     #ifdef Var(T1CON)
        If TMRNumber = 1 Then
            TMR1H = 0
            TMR1L = 0
        End If
     #endif

     #ifdef Var(T2CON)
        If TMRNumber = 2 Then TMR2 = 0
     #endif

     #ifdef Var(T3CON)
        If TMRNumber = 3 then
            TMR3H = 0
            TMR3L = 0
        End If
     #endif

     #ifdef Var(T4CON)
        If TMRNumber = 4 Then TMR4 = 0
     #endif

     #ifdef Var(T5CON)
        If TMRNumber = 5 then
            TMR5H = 0
            TMR5L = 0
        End If
     #endif

     #ifdef Var(T6CON)
        If TMRNumber = 6 Then TMR6 = 0
     #endif

     #ifdef Var(T7CON)
        If TMRNumber = 7 then
           TMR7H = 0
           TMR7L = 0
        End If
     #endif

     #ifdef Var(T8CON)
        If TMRNumber = 8 Then TMR8 = 0
     #endif

     #ifdef Var(T10CON)
        If TMRNumber = 10 Then TMR10 = 0
     #endif

     #ifdef Var(T12CON)
        If TMRNumber = 12 Then TMR12 = 0
     #endif

  #endif


  #ifdef AVR

     If TMRNumber = 0 Then
        #ifdef Var(TCNT0)
          TCNT0 = 0
        #endif

        #ifdef Var(TCNT0L)  'added 05/01/2016 Timer0 16bit support (ATtiny 261/461/861)
           #ifdef TMR0_16BIT
             TCNT0H = 0
           #endif
           TCNT0L = 0
        #endif
     End If

     #ifdef Var(TCNT1L)
        If TMRNumber = 1 Then
          TCNT1H = 0
          TCNT1L = 0
        End If
     #endif

     #ifdef Var(TC1H)  'added 05/01/2016 for ATtiny 261/461/861
        If TMRNumber = 1 Then
          TC1H = 0      'must be written before TCNT1
        End If
     #endif

     #ifdef Var(TCNT1)  'added 05/01/2016 for ATtiny 15/25/45/85, 261/461/861
        If TMRNumber = 1 Then
          TCNT1 = 0
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
     If TMRNumber = 0 Then  'modified 07/01/2016
        #ifndef Var(TMR0H)   'not an 8/16 bit selectable timer0
           TMR0 = TMRValue    'so just write TMR0
        #endif

        #ifdef Var(TMR0H)
           #ifdef TMR0_16BIT
              TMR0H = TMRValue_H
           #endif
           TMR0L = TMRValue
        #endif
     End If

     #ifdef Var(T1CON)
        If TMRNumber = 1 then
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
     If TMRNumber = 0 Then
        #ifdef Var(TCNT0)
          TCNT0 = TMRValue
        #endif

        #ifdef Var(TCNT0L)  'added 05/01/2016 Timer0 16bit support (ATtiny 261/461/861)
           #ifdef TMR0_16BIT
             TCNT0H = TMRValue_H
           #endif
           TCNT0L = TMRValue
        #endif
     End If

     #ifdef Var(TCNT1L)
        If TMRNumber = 1 Then
          TCNT1H = TMRValue_H
          TCNT1L = TMRValue
        End If
     #endif

     #ifdef Var(TC1H)   'added 05/01/2016 for ATtiny 261/461/861
        If TMRNumber = 1 Then
          TC1H = TMRValue_H  'must be written before TCNT1
        End If
     #endif

     #ifdef Var(TCNT1)   'added 05/01/2016 for ATtiny 15/25/45/85, 261/461/861
        If TMRNumber = 1 Then
          TCNT1 = TMRValue
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
    'Note: Not Always!  Timer 0 on 18F Can be stopped/started

     #ifdef bit(TMR0ON)
         If TMRNumber = 0 Then Set TMR0ON off
     #endif

     #ifdef Bit(TMR1ON)
         If TMRNumber = 1 Then Set TMR1ON OFF
     #endif

     #ifdef Bit(TMR2ON)
         If TMRNumber = 2 Then Set TMR2ON OFF
     #endif

     #ifdef Bit(TMR3ON)
         If TMRNumber = 3 Then Set TMR3ON OFF
     #endif

     #ifdef bit(TMR4ON)
         If TMRNumber = 4 Then Set TMR4ON OFF
     #endif

     #ifdef bit(TMR5ON)
         If TMRNumber = 5 Then Set TMR5ON OFF
     #endif

     #ifdef bit(TMR6ON)
         If TMRNumber = 6 Then Set TMR6ON OFF
     #endif

     #ifdef bit(TMR7ON)
         If TMRNumber = 7 Then Set TMR7ON OFF
     #endif

     #ifdef bit(TMR8ON)
         If TMRNumber = 8 Then Set TMR8ON OFF
     #endif

     #ifdef bit(TMR10ON)
         If TMRNumber = 10 Then Set TMR10ON OFF
     #endif

     #ifdef bit(TMR12ON)
         If TMRNumber = 12 Then Set TMR12ON OFF
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
           #ifndef Var(TC1H)
             TCCR1B = TCCR1B And 248
           #endif

           #ifdef var(TC1H)  '05/01/2016 added for ATtiny216/461/861
             TCCR1B = TCCR1B And 240  '4bit prescaler
           #endif
        End If
     #endif

     #ifdef Var(TCCR1)  '05/01/2016 added for ATtiny15/25/45/85
        If TMRNumber = 1 Then
          TCCR1 = TCCR1 And 240  '4bit prescaler
        End If
     #endif

     #ifdef Var(TCCR2B)
        If TMRNumber = 2 Then
          TCCR2B = TCCR2B And 248
        End If
     #endif

     #ifdef Var(TCCR2A)
        If TMRNumber = 2 Then
          TCCR2A = TCCR2A And 248
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


Sub InitTimer0(In TMRSource, In TMRPres)

  #ifdef PIC
     'Some PICS (18F+) Use T0CON for timer0 Control
     'ALL OTHER PICS USE OPTION_REG for timer0 control
     'Sub modified to set all TMR Control bits at once
     ' * TMRPres, TMRSource & TMRPost now shared *

     if TMRPRes > 7 then TMRPRes = 0 'failsafe

     #Ifndef Var(T0CON) 'Timer0 uses OPTION_REG
       'Re-Use TMRPres as T0CON Temp_register
       'Keep T0CON 7:6 and write bits 2:0 from TMRPres
       'Bits 5,4 & 3 will be cleared!
        #IFNDEF ChipFamily 12
           TMRPres = (OPTION_REG AND 192) OR TMRPres

           IF TMRSource = EXT then
              Set TMRPres.5 ON   'EXT
           ELSE
              Set TMRPres.5 OFF  'OSC
           END IF
          'Now Write the OPTION_REG
           OPTION_REG = TMRPres
        #ENDIF

        'Added For baseline Chips with write only option_reg
        #IFDEF ChipFamily 12
           'option_reg (var) defaults to 'b11000111'

           TMRPRes = (OPTION_REG and 192) or TMRPres

           if TMRSource = EXT then
              Set TMRPres.5 ON
           Else
              Set TMRPres.5 Off
           End if

          OPTION_REG = TMRPres ' maintian option_reg (Var)

          clrwdt               'clear watchdog timer
          clrw                 'Clear "W" to zero
          addwf	tmrpres,W      'Add tmrpres to "W"
          option               'write "W" to option_reg

        #EndIF

     #endif

     'If Timer0 is 16-bit capable
     #ifdef  Var(T0CON)   '18F and a few 16F Chips
         'Re-Use TMRPres as T0CON Temp register
         'Keep T0CON 7:6 and write bits 2:0 from TMRPres
         'Bits 5,4 & 3 will be cleared!
          TMRPres = (T0CON And 192) OR TMRPres

          'Set the Source bit
        IF TMRSource <> OSC  THEN
           SET TMRPres.5 ON
        ELSE
           SET TMRPres.5 OFF
        END IF

       'Set TO8BIT (T0CON.6)
        #ifdef TMR0_16BIT
            Set TMRPres.6 OFF '16-bit Timer0
        #endif

        #IFNDEF TMR0_16BIT
           Set TMRpres.6 ON  '8-bit Timer0
        #endif

          'Write the TOCON register
           T0CON = TMRPres
     #endif


  #endif

  #ifdef AVR
    'Just need to buffer TMR0Pres
    '(And change it for external clock)
    ' TMRPres, TMRSource now shared and local - WMR
    ' TMR0_TMP now used as shadow register    - WMR
     If TMRSource = Ext Then
        TMRPres = AVR_EXT_TMR_0_RE
     End If

     #ifdef TMR0_16BIT    ' 05/01/2016 added for Timer0 16-bit mode
         #ifdef bit(TCW0) ' on ATtiny216/461/861 and some others
            Set TCW0 ON   '
         #endif
     #endif
     TMR0_TMP = TMRPres
  #endif

End Sub

Sub InitTimer1(In TMRSource, In TMRPres)

  #ifdef PIC
    'Pwr On Reset State of TxCON for 1/3/5/7 is "0"
    'TXCON Timer REGISTER for 1/3/5/7 are NOT the Same on ALL PICS
    'TMRON is Bit0 on all Chips

    '** OSCEN & SOSCEN are always TxCON.3 and perform the same function
    'See Datasheet for Detailed Register Information

     #ifdef Var(T1CON) 'ALL Pics w/Timer1 module have T1CON reg

        'Test for valid Pres parameter
        'uses less memory than multiple boulean "AND"
        If TMRPres <> 0 then
           IF TMRPres <> 16 then
              IF TMRPres <> 32 then
                 IF TMRPres <> 48 then
                    TMRPres = 0
                 END IF
              END IF
           END IF
        END IF

        'Re-Using TMRPres as TxCON Temp Register
        IF TMR1ON = 1 then Set TMRPres.0 ON  'The timer running/ Dont Stop !

       'Select Case uses too much memory - changed
        IF TMRSource = OSC then
           #ifdef bit(TMR1CS)    'TXCON.1 on some chips
               Set TMRPres.1 OFF
           #endif

           #ifdef Bit(TMR1CS1)   'TXcon.7 on other chips
               Set TMRPres.7 OFF
           #endif
           Set TMRPres.3 OFF  'SOSCEN and OSCEN are Always Bit 3
        END IF

        IF TMRSource = EXT then
           #ifdef bit(TMR1CS)
               Set TMRPres.1 ON
           #endif

           #ifdef Bit(TMR1CS1)
              Set TMRPres.7 ON
           #endif
           Set TMRPres.3 OFF
        END IF

        If TMRSource = ExtOsc Then
           #ifdef bit(TMR1CS)
               Set TMRPres.1 ON
           #endif

           #ifdef Bit(TMR1CS1)
               Set TMRPres.7 ON
           #endif
            Set TMRPres.3 ON
        END IF
       'Done building Temp Variable. Now write register
       T1CON = TMRPres
     #endif
   #endif

  #ifdef AVR
     TMR1_TMP = TMRPres
     If TMRSource = Ext Then
          TMR1_TMP = AVR_EXT_TMR_1_RE
     End If

     #ifdef var(TC1H) '05/01/2016 added for ATtiny216/461/861
         TC1H = 3       'needed to start timer1 in 10bit mode
         OCR1C = 255
     #endif

  #endif
End Sub


Sub InitTimer2 (In TMRPres, In TMRPost)
                   'AVR-Src    AVR-Pres
  #ifdef PIC
     'PIC TIMERs 2/4/6/8/10/12 are all the same.
     'T2CON Register
     'Bit7 = Dont Care/ Not implemented
     'Bits 6:3  (4-Bit Postscaler)
     'Bit 2  TMRON
     'Bits 1:0 'Prescaler

     #ifdef Var(T2CON) 'does the timer exist?
        'Valid Prescales are   0 - 3
         IF TMRPres > 3 then  TMRPres = 0 'failsafe
         'Valid PostScales are  0 - 15
         IF TMRPost > 15 then  TMRPost = 0 'failsafe

        'Start Building a Temporary Variable
        'Re-Using tmrpres as TxCON shadow register
        '
        ' Alternative Method to Previous inline ASM Method
         '(Saves 3 byte of Memory)
        TMRPres = TMRPres OR (TMRPost * 8)
        If TMR2ON = ON then SET TMRPres.2 ON

       'Write the control register
        T2CON = TMRPres
     #endif
  #endif

  #ifdef AVR
     'Some wacky swapping of variable names here
     'Using TMRPres for Source and TMRPost for Prescale
     TMR2_TMP = TMRPost   'Internal OSC
     If TMRPres = Ext Then
         TMR2_TMP = AVR_EXT_TMR_2_RE
     End If
  #endif
End Sub

Sub InitTimer3(In TMRSource, In TMRPres)

  #ifdef PIC
   'See Inittimer1 for notes
     #ifdef Var(T3CON)
        If TMRPres <> 0 then
           IF TMRPres <> 16 then
              IF TMRPres <> 32 then
                 IF TMRPres <> 48 then
                    TMRPres = 0
                 END IF
              END IF
           END IF
        END IF

        IF TMR3ON = 1 then Set TMRPres.0 ON

        IF TMRSource = OSC then
           #ifdef bit(TMR3CS)
               Set TMRPres.1 OFF
           #endif

           #ifdef Bit(TMR3CS1)
               Set TMRPres.7 OFF
           #endif
             Set TMRPres.3 OFF
        END IF

        IF TMRSource = EXT then
           #ifdef bit(TMR3CS)
               Set TMRPres.1 ON
           #endif

           #ifdef Bit(TMR3CS1)
              Set TMRPres.7 ON
           #endif
             Set TMRPres.3 OFF
        END IF

        If TMRSource = ExtOsc Then
           #ifdef bit(TMR3CS)
               Set TMRPres.1 ON
           #endif

           #ifdef Bit(TMR3CS1)
               Set TMRPres.7 ON
           #endif
             Set TMRPres.3 ON
        END IF

       T3CON = TMRPres  'write the register
     #endif
  #endif

   #ifdef AVR
      TMR3_TMP = TMRPres
      If TMRSource = Ext Then
        TMR3_TMP  = AVR_EXT_TMR_3_RE
      End If
   #endif
End Sub

Sub InitTimer4 (In TMRPres, In TMRPost)

  #ifdef PIC
      'See Intimer2 for notes
     #ifdef Var(T4CON)
        IF TMRPres > 3 then  TMRPres = 0
        IF TMRPost > 15 then  TMRPost = 0
        TMRPres = TMRPres OR (TMRPost * 8)
        If TMR4ON = ON then SET TMRPres.2 ON
        T4CON = TMRPres 'write the register
     #endif
  #endif

  #ifdef AVR
     TMR4_TMP = TMRPost
     If TMRSource = Ext Then
         TMR_TMP = AVR_EXT_TMR_4_RE
     End If
  #endif

End Sub

Sub InitTimer5(In TMRSource, In TMRPres)
  #ifdef PIC
      #ifdef Var(T5CON)
       'failsafe
        If TMRPres <> 0 then
           IF TMRPres <> 16 then
              IF TMRPres <> 32 then
                 IF TMRPres <> 48 then
                    TMRPres = 0
                 END IF
              END IF
           END IF
        END IF

        'Re-Using TMRPres as T5CON Temp Register
        IF TMR5ON = 1 then Set TMRPres.0 ON

        IF TMRSource = OSC then
             #ifdef bit(TMR5CS)
                 Set TMRPres.1 OFF
             #endif

             #ifdef Bit(TMR5CS1)
                 Set TMRPres.7 OFF
             #endif
             Set TMRPres.3 OFF
        END IF

        IF TMRSource = EXT then
             #ifdef bit(TMR5CS)
                 Set TMRPres.1 ON
             #endif

             #ifdef Bit(TMR5CS1)
                Set TMRPres.7 ON
             #endif
             Set TMRPres.3 OFF
        END IF

        If TMRSource = ExtOsc Then
             #ifdef bit(TMR5CS)
                 Set TMRPres.1 ON
             #endif

             #ifdef Bit(TMR5CS1)
                 Set TMRPres.7 ON
             #endif
            Set TMRPres.3 ON
        END IF
        T5CON = TMRPres 'Write the Register
     #endif
  #endif

  #ifdef AVR
     TMR5_TMP = TMRPres
     If TMRSource = Ext Then
         TMR5_TMP = AVR_EXT_TMR_5_RE
     End If
  #endif

End Sub

Sub InitTimer6 (In TMRPres, In TMRPost)
    'See Intimer2 for notes
  #ifdef PIC
     #ifdef Var(T6CON)
         IF TMRPres > 3 then  TMRPres = 0
         IF TMRPost > 15 then  TMRPost = 0
         TMRPres = TMRPres OR (TMRPost * 8)
         If TMR6ON = ON then SET TMRPres.2 ON
         T6CON = TMRPres 'write the register
     #endif
  #endif
End Sub

Sub InitTimer7(In TMRSource, In TMRPres)
  #ifdef PIC
     'See Intimer1 for notes
     #ifdef Var(T7CON)
       'Test for valid Pres parameter
        If TMRPres <> 0 then
           IF TMRPres <> 16 then
              IF TMRPres <> 32 then
                 IF TMRPres <> 48 then
                    TMRPres = 0
                 END IF
              END IF
           END IF
        END IF

         IF TMR7ON = 1 then Set TMRPres.0 ON

        IF TMRSource = OSC then
           #ifdef bit(TMR7CS)
               Set TMRPres.1 OFF
           #endif

           #ifdef Bit(TMR7CS1)
               Set TMRPres.7 OFF
           #endif
           Set TMRPres.3 OFF
         END IF

        IF TMRSource = EXT then
           #ifdef bit(TMR7CS)
                Set TMRPres.1 ON
            #endif

           #ifdef Bit(TMR7CS1)
               Set TMRPres.7 ON
           #endif
           Set TMRPres.3 OFF
         END IF

        If TMRSource = ExtOsc Then
           #ifdef bit(TMR7CS)
              Set TMRPres.1 ON
           #endif

           #ifdef Bit(TMR7CS1)
              Set TMRPres.7 ON
           #endif
           Set TMRPres.3 ON
        END IF
         T7CON = TMRPres   ' write register
     #endif
  #endif
End Sub

Sub InitTimer8 (In TMRPres, In TMRPost)
    'See Intimer2 for notes
  #ifdef PIC
     #ifdef Var(T8CON)
        IF TMRPres > 3 then  TMRPres = 0
        IF TMRPost > 15 then  TMRPost = 0
        TMRPres = TMRPres OR (TMRPost * 8)
        If TMR8ON = ON then SET TMRPres.2 ON
        T8CON = TMRPres 'write the register
     #endif
  #endif
End Sub

Sub InitTimer10 (In TMRPres, In TMRPost)
    'See Intimer2 for notes
  #ifdef PIC
     #ifdef Var(T10CON)
        IF TMRPres > 3 then  TMRPres = 0
        IF TMRPost > 15 then  TMRPost = 0
        TMRPres = TMRPres OR (TMRPost * 8)
        If TMR10ON = ON then SET TMRPres.2 ON
        T10CON = TMRPres 'write the register
     #endif
  #endif
End Sub

Sub InitTimer12 (In TMRPres, In TMRPost)
  #ifdef PIC
    'See Intimer2 for notes
     #ifdef Var(T12CON)
         IF TMRPres > 3 then  TMRPres = 0
         IF TMRPost > 15 then  TMRPost = 0
         TMRPres = TMRPres OR (TMRPost * 8)
         If TMR12ON = ON then SET TMRPres.2 ON
         T12CON = TMRPres 'write the register
     #endif
  #endif
End Sub
