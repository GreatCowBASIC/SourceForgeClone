'    Hardware I2C2 routines for Great Cow BASIC
'    Copyright (C) 2015 Evan R. Venn
'    Version 1.1

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


'SPI mode constants ALSO used by hardware I2C:
' Define HI2C2 settings - CHANGE PORTS
'  #define MasterFast 13
'  #define Master 12             ; Used in this module
'  #define MasterSlow 11
'  #define SlaveSS 2
'  #define Slave 1               ; Used in this module
'

'HI2C Mode constants
'#define Slave10 3
'use Slave for 7-bit slave mode and Master for master mode


'Setup
'        ;Master
'        #define HI2C2_DATA PORTC.4
'        #define HI2C2_CLOCK PORTC.3
'        ;I2C pins need to be input for SSP module on a PIC
'        Dir HI2C2_DATA in
'        Dir HI2C2_CLOCK in
'
'				 Type
'        HI2C2Mode Master


'      ' SLAVE
'      '  'Buffer for incoming HI2C messages for the slave operation
'        'Each message takes 4 bytes
'        Dim HI22CBuffer(10)
'        HI2C2BufferSize = 0
'        OldHI2C2BufferSize = 0
'        HI2C2Mode Slave
'        HI2C2SetAddress 128
'        'Set up interrupt to process I2C
'        On Interrupt SSP2Ready Call HI2C2HandlerSlave


'Baud rate in KHz (Change to desired frequency)
#define HI2C2_BAUD_RATE 100


#startup HI2C2Init

#script
	HI2C2_BAUD_TEMP = int((ChipMhz * 1000000)/(4000 * HI2C2_BAUD_RATE)) - 1

	If PIC Then
             HI2C2HasData = "SSP2STAT_BF = On"
	End If

#endscript


Sub HI2C2Mode (In HI2C2CurrentMode)

	#ifdef PIC

		set SSP2STAT_SMP on
		set SSP2CON1_CKP on
		set SSP2CON1_WCOL Off

		'Select mode and clock
		If HI2C2CurrentMode = Master Then
			set SSP2CON1_SSPM3 on
			set SSP2CON1_SSPM2 off
			set SSP2CON1_SSPM1 off
			set SSP2CON1_SSPM0 off
			SSP2ADD = HI2C2_BAUD_TEMP And 127
		end if

		if HI2C2CurrentMode = Slave then
			set SSP2CON1_SSPM3 off
			set SSP2CON1_SSPM2 on
			set SSP2CON1_SSPM1 on
			set SSP2CON1_SSPM0 off
		end if

		if HI2C2CurrentMode = Slave10 then
			set SSP2CON1_SSPM3 off
			set SSP2CON1_SSPM2 on
			set SSP2CON1_SSPM1 on
			set SSP2CON1_SSPM0 on
		end if

		'Enable I2C
		set SSP2CON1_SSPEN on
	#ENDIF

End Sub

Sub HI2C2SetAddress(In HI2C2Address)
	#ifdef PIC
		'Slave mode only
		If HI2C2CurrentMode <= 10 Then
    	SSP2ADD = HI2C2Address
		End If
	#endif
End Sub

Sub HI2C2Start

	'Master mode
	If HI2C2CurrentMode > 10 Then
		#ifdef PIC
		  #ifdef Var(SSP2CON2)
        Set SSP2CON2_SEN On
        HI2C2WaitMSSP
		  #endif
		#endif

	'Slave mode
	Else
		#ifdef PIC
			Wait Until SSP2STAT_S = On
		#endif

	End If

End Sub

Sub HI2C2ReStart

	'Master mode
	If HI2C2CurrentMode > 10 Then
		#ifdef PIC
			#ifdef Var(SSP2CON2)
      	Set SSP2CON2_RSEN On
        HI2C2WaitMSSP
			#endif
		#endif
	End If

End Sub




Sub HI2C2Stop

	'Master mode
	If HI2C2CurrentMode > 10 Then
		#ifdef PIC
			#ifdef Var(SSP2CON2)

      	wait while SSP2STAT_R_NOT_W = 1   'wait for completion of activities
        Set SSP2CON2_PEN On
        HI2C2WaitMSSP
			#endif
		#endif

	'Slave mode
	Else
		#ifdef PIC
			Wait Until SSP2STAT_P = On
		#endif

	End If

End Sub

Function HI2C2StartOccurred
	'Master mode
	'Always return true
	If HI2C2CurrentMode > 10 Then
		HI2C2StartOccurred = TRUE
		Exit Function

	'Slave mode, check if start condition received last
	Else
		HI2C2StartOccurred = FALSE
		#ifdef PIC
			#ifdef Var(SSP2STAT)
				If SSP2STAT_S = On Then HI2C2StartOccurred = TRUE
			#endif
		#endif

	End If
End Function

Function HI2C2Stopped
	'Master mode
	'Always return false
	If HI2C2CurrentMode > 10 Then
		HI2C2Stopped = FALSE
		Exit Function

	'Slave mode, check if start condition received last
	Else
		HI2C2Stopped = FALSE
		#ifdef PIC
			#ifdef Var(SSP2STAT)
				If SSP2STAT_P = On Then HI2C2Stopped = TRUE
			#endif
		#endif

	End If
End Function

Sub HI2C2Send(In I2CByte)

	#ifdef PIC

		RetryHI2C2Send:
			'Clear WCOL
			SET SSP2CON1_WCOL OFF
			'Load data to send
			SSP2BUF = I2CByte
      HI2C2WaitMSSP

			if SSP2CON2_ACKSTAT =  1 then
      	HI2C2AckPollState = true
      else
      	HI2C2AckPollState = false
      end if

		If SSP2CON1_WCOL  = On Then
			If HI2C2CurrentMode <= 10 Then Goto RetryHI2C2Send
		End If

		'Release clock (only needed by slave)
		If HI2C2CurrentMode <= 10 Then Set SSP2CON1_CKP On

	#endif

End Sub

Sub HI2C2Receive (Out HI2CByte, Optional In HI2CGetAck = 1 )

	#ifdef PIC

'		#ifndef Var(SSPCON1)
'			#ifdef Var(SSPCON)
'				Dim SSPCON1 Alias SSPCON
'			#endif
'		#endif

		'Enable receive
      #ifdef Var(SSP2CON2)
			'Master mode
			If HI2C2CurrentMode > 10 Then
        if HI2CGetAck.0 = 1 then
           ' Acknowledge
            SSP2CON2_ACKDT = 0
        else
            ' Not Acknowledge
            SSP2CON2_ACKDT = 1
        end if
        SSP2CON2_RCEN = 1
			'Slave mode
			Else
			  SET SSP2STAT_R_NOT_W ON
			End If
		#endif

		'Clear Collisions
		SET SSP2CON1_WCOL OFF
		SET SSP2CON1_SSPOV Off

		'Wait for receive
		Wait Until SSP2STAT_BF = 1

		HI2CByte = SSP2BUF
    SSP2IF = 0
    SSP2CON2_ACKEN = 1
    ' Clear flag - this is required
    SSP2STAT_BF = 0
    HI2C2WaitMSSP

		'Disable receive (master mode)
		#ifdef Var(SSP2CON2)
			'Master mode
			If HI2C2CurrentMode > 10 Then
				Set SSP2CON2_RCEN Off
			'Slave mode
			Else
			  SET SSP2STAT_R_NOT_W Off
			End If
		#endif
	#endif

End Sub

; This routine waits for the last I2C operation to complete.
; It does this by polling the SSPIF flag in PIR1.
; Then, it clears SSPIF
sub HI2C2WaitMSSP

		' adapted for 18f - v0.94
		Dim HI2CWaitMSSPTimeout as byte
		HI2C2WaitMSSPTimeout = 0
		HI2C2WaitMSSPWait:
    HI2C2WaitMSSPTimeout++
    if HI2C2WaitMSSPTimeout < 255 then
        #ifdef bit(SSP2IF)
            ''Support for SSP1IF
            if SSP2IF = 0 then goto HI2C2WaitMSSPWait
            SSP2IF = 0
        #endif

        #ifndef  bit(SSP2IF)
                    ' no int flag so wait a while and exit
          wait 1 us
        #endif
    end if


end sub

sub HI2C2Init
    HI2C2CurrentMode = 0
end sub