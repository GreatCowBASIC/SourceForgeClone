'    Hardware I2C routines for Great Cow BASIC
'    Copyright (C) 2010 Hugh Considine

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

'To make the PIC pause until it receives an SPI message while in slave mode, set the
'constant "WaitForSPI" at the start of the program. The value does not matter.

'SPI mode constants also used by hardware I2C:
' Define I2C settings - CHANGE PORTS
'  #define MasterFast 13
'  #define Master 12
'  #define MasterSlow 11
'  #define SlaveSS 2
'  #define Slave 1
'

'I2C Mode constants
'#define Slave10 3
'use Slave for 7-bit slave mode and Master for master mode


'Setup
     ' Master
'        #define I2C_DATA PORTC.4
'        #define I2C_CLOCK PORTC.3
'        'Initialise I2C Slave
'        'I2C pins need to be input for SSP module
'        Dir I2C_DATA in
'        Dir I2C_CLOCK in
'      ' MASTER type
'        HI2CMode MasterFast


'      ' SLAVE
'      '  'Buffer for incoming HI2C messages for the slave operation
'        'Each message takes 4 bytes
'        Dim HI2CBuffer(10)
'        HI2CBufferSize = 0
'        OldHI2CBufferSize = 0
'        HI2CMode Slave
'        HI2CSetAddress 128
'        'Set up interrupt to process I2C
'        On Interrupt SSP1Ready Call HI2CHandlerSlave


'Baud rate in KHz (Change to desired frequency)
#define HI2C_BAUD_RATE 100


#script
	HI2C_BAUD_TEMP = int((ChipMhz * 1000000)/(4000 * HI2C_BAUD_RATE)) - 1
	'error I2CBaudTemp
	
	If PIC Then
		HI2CHasData = "BF = On"
	End If
	
#endscript


Sub HI2CMode (In HI2CCurrentMode)
	
	
	#ifdef PIC
		#ifndef Var(SSPCON1)
			#ifdef Var(SSPCON)
				Dim SSPCON1 Alias SSPCON
			#endif
		#endif
		
		set SSPSTAT.SMP on
		set SSPCON1.CKP on
		set SSPCON1.WCOL Off
		
		'Select mode and clock
		If HI2CCurrentMode = Master Then
			set SSPCON1.SSPM3 on
			set SSPCON1.SSPM2 off
			set SSPCON1.SSPM1 off
			set SSPCON1.SSPM0 off
			SSPADD = HI2C_BAUD_TEMP And 127
		end if
		
		if HI2CCurrentMode = Slave then
			set SSPCON1.SSPM3 off
			set SSPCON1.SSPM2 on
			set SSPCON1.SSPM1 on
			set SSPCON1.SSPM0 off
		end if
		
		if HI2CCurrentMode = Slave10 then
			set SSPCON1.SSPM3 off
			set SSPCON1.SSPM2 on
			set SSPCON1.SSPM1 on
			set SSPCON1.SSPM0 on
		end if
		
		'Enable I2C
		set SSPCON1.SSPEN on
	#ENDIF
	
End Sub

Sub HI2CSetAddress(In I2CAddress)
	#ifdef PIC
		'Slave mode only
		If HI2CCurrentMode <= 10 Then
			SSPADD = I2CAddress
		End If
	#endif
End Sub

Sub HI2CStart

	'Master mode
	If HI2CCurrentMode > 10 Then
		#ifdef PIC
			#ifdef Var(SSPCON2)
                                        Set SSPCON2.SEN On
                                        HI2CWaitMSSP
			#endif
		#endif
		
	'Slave mode
	Else
		#ifdef PIC
			Wait Until SSPSTAT.S = On
		#endif
		
	End If
	
End Sub

Sub HI2CReStart

	'Master mode
	If HI2CCurrentMode > 10 Then
		#ifdef PIC
			#ifdef Var(SSPCON2)
                                        Set SSPCON2.RSEN On
                                        HI2CWaitMSSP
			#endif
		#endif
	End If
	
End Sub

Sub HI2CStop

	'Master mode
	If HI2CCurrentMode > 10 Then
		#ifdef PIC
			#ifdef Var(SSPCON2)
				Set SSPCON2.PEN On
                                        HI2CWaitMSSP
			#endif
		#endif
		
	'Slave mode
	Else
		#ifdef PIC
			Wait Until SSPSTAT.P = On
		#endif
		
	End If
	
End Sub

Function HI2CStartOccurred
	'Master mode
	'Always return true
	If HI2CCurrentMode > 10 Then
		HI2CStartOccurred = TRUE
		Exit Function
		
	'Slave mode, check if start condition received last
	Else
		HI2CStartOccurred = FALSE
		#ifdef PIC
			#ifdef Var(SSPSTAT)
				If SSPSTAT.S = On Then HI2CStartOccurred = TRUE
			#endif
		#endif
		
	End If
End Function

Function HI2CStopped
	'Master mode
	'Always return false
	If HI2CCurrentMode > 10 Then
		HI2CStopped = FALSE
		Exit Function
		
	'Slave mode, check if start condition received last
	Else
		HI2CStopped = FALSE
		#ifdef PIC
			#ifdef Var(SSPSTAT)
				If SSPSTAT.P = On Then HI2CStopped = TRUE
			#endif
		#endif
		
	End If
End Function

Sub HI2CSend(In I2CByte)
	
	#ifdef PIC
		#ifndef Var(SSPCON1)
			#ifdef Var(SSPCON)
				Dim SSPCON1 Alias SSPCON
			#endif
		#endif
		
		RetryHI2CSend:
			'Clear WCOL
			SET SSPCON1.WCOL OFF
			'Load data to send
			SSPBUF = I2CByte
                              HI2CWaitMSSP

			if ACKSTAT =  1 then
                              	HI2CAckPollState = true
                              else
                                        HI2CAckPollState = false
                              end if

		If SSPCON1.WCOL = On Then
			If HI2CCurrentMode <= 10 Then Goto RetryHI2CSend
		End If
		
		'Release clock (only needed by slave)
		If HI2CCurrentMode <= 10 Then Set SSPCON1.CKP On
		
	#endif

End Sub

Sub HI2CReceive (Out I2CByte, Optional In HI2CGetAck = NACK )
	
	#ifdef PIC

		#ifndef Var(SSPCON1)
			#ifdef Var(SSPCON)
				Dim SSPCON1 Alias SSPCON
			#endif
		#endif
		
		'Enable receive
                    #ifdef Var(SSPCON2)
			'Master mode
			If HI2CCurrentMode > 10 Then
                                if HI2CGetAck = NACK then
                                   ' Not Acknowledge
                                    ACKDT = 1 ; ACK DATA to send is 1 (NACK) ' bsf SSPCON2,ACKDT
                                else
                                    ' Acknowledge
                                    ACKDT = 0 ; ACK DATA to send is 1 (NACK) ' bsf SSPCON2,ACKDT
                                end if
                                RCEN = 1
			'Slave mode
			Else
				' Error!! SET SSPSTAT.R ON
			End If
		#endif

		'Clear Collisions
		SET SSPCON1.WCOL OFF
		SET SSPCON1.SSPOV Off
		
		'Wait for receive
		Wait Until SSPSTAT.BF = 1 AND SSPIF = 1

		I2CByte = SSPBUF
                    SSPIF = 0
                    ACKEN = 1; Send ACK DATA now. ' bsf SSPCON2,ACKEN
                    ' Clear flag - this is required
                    SSPSTAT.BF = 0
                    HI2CWaitMSSP

		'Disable receive (master mode)
		#ifdef SSPCON2
			'Master mode
			If HI2CCurrentMode > 10 Then
				Set SSPCON2.RCEN Off
			'Slave mode
			Else
				SET SSPSTAT.R Off
			End If
		#endif
	#endif

End Sub

; This routine waits for the last I2C operation to complete.
; It does this by polling the SSPIF flag in PIR1.
; Then, it clears SSPIF
sub HI2CWaitMSSP

     if SSPIF = 0 then goto $-1
     SSPIF = 0

end sub



Sub HI2CHandlerSlave
	'Handle I2C interrupt
	'SSPIF doesn't trigger for stop condition, only start!
	
	'If buffer full flag set, read
	
	Do While HI2CHasData
		HI2CReceive DataIn
		
		'Sending code
		If HI2CBufferSize = 0 Then
			LastI2CWasRead = False
			'Detect read address
			If DataIn = 129 Then
				LastI2CWasRead = True
				HI2CSend OutKey		
				Exit Sub
			End If
		End If
		
		If HI2CBufferSize < 10 Then HI2CBuffer(HI2CBufferSize) = DataIn
		HI2CBufferSize += 1
	Loop
	
End Sub


Sub ProcessHI2CSlave
	
	If HI2CStopped Then
		IntOff
		If LastI2CWasRead Then HI2CBufferSize = 0
		
		If HI2CBufferSize <> 0 Then
			OldHI2CBufferSize = BufferSize
			HI2CBufferSize = 0
		End If
		IntOn
	End If
	
	If OldHI2CBufferSize <> 0 Then
		
		CmdControl = HI2CBuffer(1)
		
                    ' This example sends data to the LCD

'		Set backlight
'		If CmdControl.6 = On Then
'			Set BACKLIGHT On
'		Else
'			Set BACKLIGHT Off
'		End If
'		
'		Set R/S bit
'		LCD_RS = CmdControl.4
'		Send bytes to LCD
'		LCDDataBytes = CmdControl And 0x0F
'		If LCDDataBytes > 0 Then
'			For CurrSendByte = 1 To LCDDataBytes
'				LCDWriteByte HI2CBuffer(LCDDataBytes + 1)
'			Next
'		End If	
'                    	
		OldHI2CBufferSize = 0
	End If
	
End Sub
