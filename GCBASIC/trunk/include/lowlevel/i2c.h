'    Software I2C routines for the GCBASIC compiler
'    Copyright (C) 2009 Hugh Considine

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
'
'    Updated Evan R Venn - Oct 2013, thanks go to Microchip and then Application Note AN1488
'    Beta v0.89
'
'     0.81 - Initial release
'     0.82 - Improved ACKPOLL handling
'     0.83 - Improved ACKPOLL handling - added I2CReturnState to return error condition
'     0.83 - Added default sub values
'     0.84 - Added Optional I2CGETACK As byte = TRUE to maintain SLAVE compatibility
'     0.85 - Improved stability and corrections
'     0.86 - Delete v2 subs
'     0.87 - Added new sub I2CSlaveDeviceReceive to handle slave correctly.  I2CSlaveDeviceReceive thisI2CSlavedevice, readDeviceAddress, TRU
'     0.88 - Revised timings and ensured the DATA line goes low at end of process.	
'     0.89 - Revised I2CAckPollState = I2CSendState


'Notes:
'

'Initialisation routine
#startup InitI2C

'Constants (can change to suit application)
'#define I2C_BIT_DELAY 2 us
'#define I2C_CLOCK_DELAY 1 us
'#define I2C_END_DELAY 1 us
#define I2C_MODE Master
'
#define I2C_BIT_DELAY 20 us
#define I2C_CLOCK_DELAY 10 us
#define I2C_END_DELAY 10 us

'Uncomment to enable:
'#define I2C_USE_TIMEOUT

'Constants (shouldn't change)
#define I2C_DATA_HIGH Dir I2C_DATA In
#define I2C_DATA_LOW Dir I2C_DATA Out: Set I2C_DATA Off
#define I2C_CLOCK_HIGH Set I2C_CLOCK On
#define I2C_CLOCK_LOW  Set I2C_CLOCK Off

#define I2CStopped I2COldState

#define ACK       0x00
#define NACK      0x80
#define I2CReturnState as byte
'Subs
Sub InitI2C
	'Initialisation routine
	'Release data and clock lines
	 #if I2C_MODE = Master
              I2C_DATA_HIGH
              Dir I2C_CLOCK Out
	    I2C_CLOCK_HIGH
	#endif
	'Set old state flag (slave mode)
	#if I2C_MODE = Slave
                       Dir I2C_CLOCK in
	             I2COldState = 255
	#endif
	
End Sub



#define ACK       0x00
#define NACK      0x80


sub I2CSTART

        #if I2C_MODE = Master
                  'Disable interrupts. Make sure to call I2CStop to re-enable
                  'Optional INTOFF - Remove if required
                  #IFNDEF I2C_DISABLE_INTERRUPTS
                      IntOff
	        #endif
                  'For start, ensure SDA & SCL are high
                  I2C_DATA_HIGH
                  I2C_CLOCK_HIGH
                  I2C_DATA_LOW
                  Wait I2C_BIT_DELAY
                  I2C_CLOCK_LOW
                  Wait I2C_END_DELAY
        #endif

        ' same as v1 code
        #if I2C_MODE = Slave
                  I2CState = 255
                  Do
                            I2COldState = I2CState
                            If I2C_CLOCK = 1 Then
                                      'State 0, CK 1, DA 1
                                      If I2C_DATA = 1 Then
                                                I2CState = 0
                                      'State 1, CK 1, DA 0
                                      Else
                                                'Start happens when data drops while clock high, so:
                                                'start occurs on transition from CK 1 DA 1 (state 0) to CK 1 DA 0 (state 1)
                                                If I2COldState = 0 Then
                                                          Goto I2CStartReceived
                                                End If
                                                I2CState = 1
                                      End If
                            Else
                                      'State 2 when CK 0
                                      I2CState = 2
                            End If
			
                  Loop
		
                  I2CStartReceived:

		
                  'Disable interrupts
                  IntOff
        #endif

end sub


Sub I2CSTOP

	'In master mode, send stop
	#if I2C_MODE = Master
                  'For stop, need to raise data while clock high
                  I2C_CLOCK_LOW
                  I2C_DATA_LOW
                  Wait I2C_CLOCK_DELAY
                  I2C_CLOCK_HIGH
                  'Dir I2C_DATA Out
                  I2C_DATA_HIGH
                  Wait I2C_CLOCK_DELAY
                  I2C_CLOCK_LOW
                  'Have finished transfer, can re-enable interrupts
                  'Optional
                   #IFNDEF I2C_DISABLE_INTERRUPTS
                           IntOn
                   #endif
                   I2C_DATA_LOW
'                   Dir I2C_CLOCK in
'                   Dir I2C_DATA in
	#endif

	'In slave mode, just need to re-enable interrupt
	#if I2C_MODE = Slave
	    'Have finished transfer, can re-enable interrupts
              'Optional
               #IFNDEF I2C_DISABLE_INTERRUPTS
                       IntOn
               #endif
	#endif
end Sub

Sub I2CRESET
       ' added 0.84
       I2CSTART
       repeat 10
        I2CSEND ( 0xFF )
       end Repeat
       I2CSTART
       I2CSTOP
end Sub


Sub I2CRESTART
' added 0.84

        I2C_CLOCK_LOW
        Wait I2C_BIT_DELAY

        I2C_DATA_HIGH
        Wait I2C_BIT_DELAY

        I2C_CLOCK_HIGH
        Wait I2C_END_DELAY

        I2C_DATA_LOW
        Wait I2C_BIT_DELAY

end Sub




Function I2CStartOccurred
	'Check if a start condition has occurred since the last run of this function
	'Only used in slave mode
	
	#if I2C_MODE = Master
		I2CStartOccurred = TRUE
	#endif
	
	#if I2C_MODE = Slave
		I2CStartOccurred = FALSE
		
		If I2C_CLOCK = 1 Then
			'State 0, CK 1, DA 1
			If I2C_DATA = 1 Then
				I2CState = 0
			'State 1, CK 1, DA 0
			Else
				'Start happens when data drops while clock high, so:
				'start occurs on transition from CK 1 DA 1 (state 0) to CK 1 DA 0 (state 1)
				If I2COldState = 0 Then
					I2CStartOccurred = TRUE
				End If
				I2CState = 1
			End If
		Else
			'State 2 when CK 0
			I2CState = 2
		End If
		I2COldState = I2CState
		
	#endif
	
End Function






'//....................................................................
'// Writes a byte to the I2C bus
'//....................................................................
sub I2CSend( in i2cbyte as byte, Optional I2CGETACK = TRUE  )

        #if I2C_MODE = Master
             l_ack = 0x00
             For I2CCurrByte = 0 to 7
                             ' code expanded to remove stack

                                  I2C_CLOCK_LOW
                                  '' Dir I2C_DATA Out
                                  I2C_DATA_LOW
                                  If i2cbyte.7 = On Then
                                            Set I2C_DATA ON
                                  Else
                                            Set I2C_DATA Off
                                  End If
                                  Wait I2C_CLOCK_DELAY
                                  I2C_CLOCK_HIGH
                                  Wait I2C_CLOCK_DELAY
                                  I2C_CLOCK_LOW

                        Rotate I2CByte Left
              Next
'              CODE EXPANDED

              I2C_CLOCK_LOW
              I2C_DATA_LOW
              Dir I2C_DATA In
              I2C_CLOCK_HIGH
              ' Clock stretching - this is the ONLY CHange from the baseline code
               #IFDEF IC2CLOCKSTRETCHING
                      Wait While I2C_CLOCK = Off
               #ENDIF
               I2CSendState = I2C_DATA
               Wait I2C_CLOCK_DELAY
               I2C_CLOCK_LOW

        #Endif

        #if I2C_MODE = Slave

                  'Wait for clock to drop
                  #ifndef I2C_USE_TIMEOUT
                            Wait Until I2C_CLOCK = Off
                  #endif
                  #ifdef I2C_USE_TIMEOUT
                            I2CState = 0
                            Do Until I2C_CLOCK = Off
                                      I2CState += 1
                                      If I2CState = 0 Then
                                                I2CStopped = True
                                                Exit Sub
                                      End If
                            Loop
                  #endif
		
                  'Get data line
                  I2C_DATA_LOW
                  'Write bits
'                  I2CByte = 0
                  For I2CCurrByte = 1 To 8
                            'Clock will be low
			
                            'Write bit
                            If I2CByte.7 = On Then
                                      I2C_DATA_HIGH
                            Else
                                      I2C_DATA_LOW
                            End If
                            Rotate I2CByte Left
			
                            'Wait for clock pulse
                            'Wait for clock to rise
                             Wait Until I2C_CLOCK = On
                            'Wait for clock to drop (end of bit)
                            #ifndef I2C_USE_TIMEOUT
                                      Wait Until I2C_CLOCK = Off
                            #endif
                            #ifdef I2C_USE_TIMEOUT
                                      I2CState = 0
                                      Do Until I2C_CLOCK = Off
                                                I2CState += 1
                                                If I2CState = 0 Then
                                                          I2CStopped = True
                                                          Exit Sub
                                                End If
                                      Loop
                            #endif
                  Next
                  'Release data line ... erv changed from hugh's code
                  I2C_DATA_HIGH

		
                  'Read Ack
                  I2CStopped = FALSE
'                  If I2CGetAck Then
			
                            'Wait for clock to go high
                            Wait Until I2C_CLOCK = On

                            'Read ack
                            If I2C_DATA = On Then
                                      'If data on, have received NACK
                                      I2CStopped = TRUE
                            End If
                            'Wait for clock to go low
                            #ifndef I2C_USE_TIMEOUT

                                      Wait Until I2C_CLOCK = Off

                            #endif
                            #ifdef I2C_USE_TIMEOUT
                                      I2CState = 0
                                      Do Until I2C_CLOCK = Off
                                                I2CState += 1
                                                If I2CState = 0 Then
                                                          Exit Do
                                                End If
                                      Loop
                            #endif
'                  End If

        #endif

End Sub

'//....................................................................
'// Reads a byte from the I2C bus
Sub I2CReceive ( Out I2CByte, Optional In I2CGetAck = ACK )

        #if I2C_MODE = Master
            'Need to generate clock while checking input
            I2C_CLOCK_LOW
            'Setting data high will make data pin input
            I2C_DATA_HIGH
    		
            I2CByte = 0
            For I2CCurrByte = 1 To 8
                      Wait I2C_END_DELAY
                      I2C_CLOCK_HIGH
    			
                      'Clock stretching
                      Wait While I2C_CLOCK = Off
    			
                      Wait I2C_END_DELAY
                      Rotate I2CByte Left
                      Set I2CByte.0 Off
                      If I2C_DATA = On Then
                                Set I2CByte.0 On
                      End If
                      Wait I2C_END_DELAY
    			
                      I2C_CLOCK_LOW
                      Wait I2C_END_DELAY
    			
            Next
'             code below expand to remove stack

              ' ACK / NACK routine
            I2C_CLOCK_LOW
            '' Dir I2C_DATA Out
            I2C_DATA_LOW
            If I2CGetAck.7 = On Then
                      Set I2C_DATA ON
            Else
                      Set I2C_DATA Off
            End If
            Wait I2C_BIT_DELAY
            I2C_CLOCK_HIGH
            Wait I2C_CLOCK_DELAY
            I2C_CLOCK_LOW

        #endif

        'Slave mode
        #if I2C_MODE = Slave

                  'Assumes that start has just been received, so CK and DA high
  		
                  'Wait for clock to drop
                  #ifndef I2C_USE_TIMEOUT
                            Wait Until I2C_CLOCK = Off
                  #endif

                  #ifdef I2C_USE_TIMEOUT
                            I2CState = 0
                            Do Until I2C_CLOCK = Off
                                      I2CState += 1
                                      If I2CState = 0 Then
                                                I2CStopped = True
                                                Exit Sub
                                      End If
                            Loop
                  #endif

                  'Read bits
                  I2CByte = 0

                  For I2CCurrByte = 1 To 8
  			

                             'Wait for clock to rise
                            Wait Until I2C_CLOCK = On 			

                              'Get bit from data pin
                              Rotate I2CByte Left
                              Set I2CByte.0 Off
                              If I2C_DATA = On Then
                                        Set I2CByte.0 On
                              End If

                            'Wait for clock to drop (end of bit)
                            #ifndef I2C_USE_TIMEOUT
                                      Wait Until I2C_CLOCK = Off
                            #endif
                            #ifdef I2C_USE_TIMEOUT
                                      I2CState = 0
                                      Do Until I2C_CLOCK = Off
                                                I2CState += 1
                                                If I2CState = 0 Then
                                                          I2CStopped = True
                                                          Exit Sub
                                                End If
                                      Loop
                            #endif

                  Next





                  'Send Ack
                  If I2CGetAck Then		
                            'Send ack by pulling data down for 1 clock pulse

                            I2C_DATA_LOW

                            Wait Until I2C_CLOCK = On

                            #ifndef I2C_USE_TIMEOUT
                                      Wait Until I2C_CLOCK = Off
                            #endif
                            #ifdef I2C_USE_TIMEOUT
                                      I2CState = 0
                                      Do Until I2C_CLOCK = Off
                                                I2CState += 1
                                                If I2CState = 0 Then
                                                          Exit Do
                                                End If
                                      Loop
                            #endif
                            I2C_DATA_HIGH
                  End If
  		
                  I2CStopped = FALSE
  		
        #endif
End sub


'// Reads a byte from the I2C bus, and ONLY ACK when it is the correct address
Sub I2CSlaveDeviceReceive ( In thisDevice as Byte , Out I2CByte, Optional In I2CGetAck = ACK )


        'Slave mode only
        #if I2C_MODE = Slave

                  'Assumes that start has just been received, so CK and DA high
  		
                  'Wait for clock to drop
                  #ifndef I2C_USE_TIMEOUT
                            Wait Until I2C_CLOCK = Off
                  #endif

                  #ifdef I2C_USE_TIMEOUT
                            I2CState = 0
                            Do Until I2C_CLOCK = Off
                                      I2CState += 1
                                      If I2CState = 0 Then
                                                I2CStopped = True
                                                Exit Sub
                                      End If
                            Loop
                  #endif

                  'Read bits
                  I2CByte = 0

                  For I2CCurrByte = 1 To 8
  			

                             'Wait for clock to rise
                            Wait Until I2C_CLOCK = On 			

                              'Get bit from data pin
                              Rotate I2CByte Left
                              Set I2CByte.0 Off
                              If I2C_DATA = On Then
                                        Set I2CByte.0 On
                              End If

                            'Wait for clock to drop (end of bit)
                            #ifndef I2C_USE_TIMEOUT
                                      Wait Until I2C_CLOCK = Off
                            #endif
                            #ifdef I2C_USE_TIMEOUT
                                      I2CState = 0
                                      Do Until I2C_CLOCK = Off
                                                I2CState += 1
                                                If I2CState = 0 Then
                                                          I2CStopped = True
                                                          Exit Sub
                                                End If
                                      Loop
                            #endif

                  Next


                  if thisDevice = I2CByte then

                      'Send Ack
                      If I2CGetAck Then		
                                'Send ack by pulling data down for 1 clock pulse

                                I2C_DATA_LOW

                                Wait Until I2C_CLOCK = On

                                #ifndef I2C_USE_TIMEOUT
                                          Wait Until I2C_CLOCK = Off
                                #endif
                                #ifdef I2C_USE_TIMEOUT
                                          I2CState = 0
                                          Do Until I2C_CLOCK = Off
                                                    I2CState += 1
                                                    If I2CState = 0 Then
                                                              Exit Do
                                                    End If
                                          Loop
                                #endif
                                I2C_DATA_HIGH
                      End If
                  End if  		
                  I2CStopped = FALSE
  		
        #endif
End sub


'.............................................................................
'          Polls the bus for ACK from device
'.............................................................................
Sub I2CACKPOLL ( ack_addr )

    I2CSendState = 0
    l_attempts = 0
    Do While  I2CSendState = ACK and l_attempts < 63
       I2CStart
       I2CSend( ack_addr )
       wait 5 ms
       l_attempts++

    Loop
    I2CSTOP


    I2CAckPollState = I2CSendState
end Sub

