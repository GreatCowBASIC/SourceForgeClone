'    Hardware I2C routines for Great Cow BASIC
'    Copyright (C) 2010 Hugh Considine
'    Copyright (C) 2014 & 2015, 2016 Evan R. Venn & Jacques Erdemaal
'    Version 1.1f
'    Version 1.1g
'    Version 1.1h
'    Version 1.1i
'    Version 1.1j
'    Version 1.1k
'    Version 1.1l



'    Updated Feb 2015 by Jacques Erdemaal to improve (to remove the guess work) from the configuration for AVR
'            and to improve the initialisation of the AVR
'
'    Updated Feb 2015 to support AVR and correct HI2CReceive parameter error.
'    Moved defines ACK/NACK to sysen.ini to
'
'    Updated May 2015 - enhance hi2cwaitmssp
'    Updated Oct 2015 - enhance hi2cwaitmssp
'    Updated Jan 2016 - enhance hi2cwaitmssp
'    Updated May 2016 - resolved AVR TWINT lockup issues
'    Updated Sept 2016 - resolve 16f18855 register mapping
'    Updated Oct 2016  - for Option Explicit and to fix the script issue
'    Updated Oct 2016  - ... Slave10 was NOT defined.


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
' Define HI2C settings - CHANGE PORTS
'  #define MasterFast 13
'  #define Master 12             ; Used in this module
'  #define MasterSlow 11
'  #define SlaveSS 2
'  #define Slave 1               ; Used in this module
'

'HI2C Mode constants
#define Slave10 3
'use Slave for 7-bit slave mode and Master for master mode


'Setup
'        ;Master and AVR
'        #define I2C_DATA PORTC.4
'        #define I2C_CLOCK PORTC.3
'        ;I2C pins need to be input for SSP module on a PIC
'        Dir HI2C_DATA in
'        Dir HI2C_CLOCK in
'        ;Type
'        HI2CMode Master


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

  ' Move to HWI2C.h when completed
  #define AVR_I2C_START 0x08
  #define AVR_I2C_ReSTART 0x10

  #define MT_SLA_ACK 0x18
  #define MT_SLA_NACK_REC 0x20
  #define MT_DATA_ACK 0x28
  #define MT_DATA_NACK_REC 0x30
  #define ARB_LOST 0x38

  #define MR_SLA_ACK 0x40
  #define MR_SLA_NACK_REC 0x48
  #define MR_DATA_ACK 0x50
  #define MR_DATA_NACK_REC 0x58


#startup HIC2Init

#script
  HI2C_BAUD_TEMP = int((ChipMhz * 1000000)/(4000 * HI2C_BAUD_RATE)) - 1

  If PIC Then
             HI2CHasData = "BF = On"
  End If



          IF AVR then
              'Redirects to AVR CODE
              HI2CMode = AVRHI2CMode
              HI2CStart = AVRHI2CStart
              HI2CStop = AVRHI2CStop
              HI2CReStart = AVRHI2CReStart
              HI2CSend = AVRHI2CSend
              HI2CReceive = AVRHI2CReceive


              TWIMode = AVRHI2CMode
              TWIStart = AVRHI2CStart
              TWIStop = AVRHI2CStop
              TWICReStart = AVRHI2CReStart
              TWISend = AVRHI2CSend
              TWIReceive = AVRHI2CReceive
              TWIByte = I2CByte
              TWIAckPollState = HI2CAckPollState
              TWIStartOccurred = HI2CStartOccurred


               ' SCL CLOCK
               ' ---------
               ' TWBR= (CPU_CLK - SCL_BD * 16)/2 * SCL_BD * PRESC       (328P  PDF 21.5.2)
               CPU_Clk = ChipMhz * 1e6
               BD_RATE=1000 * HI2C_BAUD_RATE

               CST_TWBR = Int((CPU_Clk - (16*BD_RATE))/(2*BD_RATE*1))
               If CST_TWBR <= 255 Then
                   CST_PRESCALER = 0
               End If
               If CST_TWBR > 255 Then
                   CST_TWBR = Int((CPU_Clk - (16*BD_RATE))/(2*BD_RATE*4))
                   If CST_TWBR <= 255 Then
                      CST_PRESCALER = 1
                   End If
                   If CST_TWBR > 255 Then
                      CST_TWBR = Int((CPU_Clk - (16*BD_RATE))/(2*BD_RATE*16))
                      If CST_TWBE <= 255 Then
                         CST_PRESCALER = 2
                      End If
                      If CST_TWBR > 255 Then
                         CST_TWBR = Int((CPU_Clk - (16*BD_RATE))/(2*BD_RATE*64))
                         If CST_TWBR <=255 Then
                            CST_PRESCALER = 3
                         End If
                         If CST_TWBR > 255 Then
                             Compiler Will Show error
                            Error "Error Computing TWI-I2C BAUD RATE PARAMETERS"
                            Error "    CPU FREQ    = " ChipMhz " Mhz"
                            Error "    I2C BAUD RATE " HI2C_BAUD_RATE " kHz"
                         End If
                      End If
                   End If
               End If
              ' Uncommented Displays Results In GCB Output Window
              ' warning " CST_PRESCALER = "  CST_PRESCALER  "    CST_TWBR = "  CST_TWBR
          END IF

         if novar(SSPCON1) then

            if var(SSP1CON1) then
                SSPCON1 = SSP1CON1
                SSPSTAT = SSP1STAT
                SSPBUF  = SSP1BUF
                SSPCON2 = SSP1CON2
                SSPADD  = SSP1ADD
            end if

         end if


         if novar(SSPCON1) then
            if var(SSPCON) then
                SSPCON1 = SSPCON
            end if
         end if

#endscript

Sub HI2CMode (In HI2CCurrentMode)

  #ifdef PIC





'    #ifndef Var(SSPCON1)
'      #ifdef Var(SSPCON)
'        Dim SSPCON1 Alias SSPCON
'      #endif
'    #endif

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

      #ifdef bit(SEN)
          Set SEN On
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
      #ifdef BIT(RSEN)
                                        Set RSEN On
                                        HI2CWaitMSSP
      #endif
    #endif
  End If

End Sub




Sub HI2CStop

  'Master mode
  If HI2CCurrentMode > 10 Then
    #ifdef PIC
      #ifdef BIT(PEN)
                                     ' set SSPIE OFF; disable SSP interrupt, tested by Anobium but not implemented.
                                     wait while R_NOT_W = 1   'wait for completion of activities
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
      #ifdef BIT(S)
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
      #ifdef BIT(P)
        If SSPSTAT.P = On Then HI2CStopped = TRUE
      #endif
    #endif

  End If
End Function

Dim HI2CACKPOLLSTATE  as Byte

Sub HI2CSend(In I2CByte)

  #ifdef PIC

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

Sub HI2CReceive (Out I2CByte, Optional In HI2CGetAck = 1 )

  #ifdef PIC



    'Enable receive

      'Master mode
      If HI2CCurrentMode > 10 Then
                                if HI2CGetAck.0 = 1 then
                                   ' Acknowledge
                                    ACKDT = 0
                                else
                                    ' Not Acknowledge
                                    ACKDT = 1
                                end if
                                RCEN = 1
      'Slave mode
      Else
        SET SSPSTAT.R_NOT_W ON
      End If


    'Clear Collisions
    SET SSPCON1.WCOL OFF
    SET SSPCON1.SSPOV Off

    'Wait for receive
    Wait Until SSPSTAT.BF = 1' AND SSPIF = 1

    I2CByte = SSPBUF
                    SSPIF = 0
                    ACKEN = 1; Send ACK DATA now. ' bsf SSPCON2,ACKEN
                    ' Clear flag - this is required
                    SSPSTAT.BF = 0
                    HI2CWaitMSSP

    'Disable receive (master mode)

      'Master mode
      If HI2CCurrentMode > 10 Then
        Set SSPCON2.RCEN Off
      'Slave mode
      Else
        SET SSPSTAT.R_NOT_W Off
      End If

  #endif

End Sub

; This routine waits for the last I2C operation to complete.
; It does this by polling the SSPIF flag in PIR1.
; Then, it clears SSPIF
; Updated at v0.95.010 Option Explicit
Dim HI2CWaitMSSPTimeout as byte
sub HI2CWaitMSSP


    HI2CWaitMSSPTimeout = 0
    HI2CWaitMSSPWait:
    HI2CWaitMSSPTimeout++
    if HI2CWaitMSSPTimeout < 255 then
        #ifdef bit(SSP1IF)
            ''Support for SSP1IF
            if SSP1IF = 0 then goto HI2CWaitMSSPWait
            SSP1IF = 0
            exit Sub
        #endif
        #ifdef bit(SSPIF)
            ''Support for SSPIF
            if SSPIF = 0 then goto HI2CWaitMSSPWait
            SSPIF = 0
            exit Sub
        #endif

        #ifndef  bit(SSP1IF)
          #ifndef  bit(SSPIF)
            ' no int flag so wait a while and exit
            wait 1 us
          #endif
        #endif
    end if

end sub





Sub AVRHI2CMode ( In HI2CCurrentMode )

          #ifdef AVR
                 If HI2CCurrentMode = Master Then

                    TWSR = CST_PRESCALER
                    TWBR = CST_TWBR

                 end if

                 if HI2CCurrentMode = Slave then
                 '  [tbd]
                 end if
                 ldi  SysValueCopy, 0|(1<<TWEN)
                 sts  TWCR,SysValueCopy

                 HI2CWaitMSSPTimeout = 0
            #endif


End Sub

Sub AVRHI2CStart


    if HI2CCurrentMode = 0 then
       'assume MASTER
       HI2CMode Master
    end if

    lds SysValueCopy,TWCR
    sbr   SysValueCopy, (1<<TWINT)|(1<<TWSTA)| (1<<TWEN)
    sts TWCR,SysValueCopy

    do while TWINT = 0
    loop

    I2CByte = TWSR & 0xF8

    if I2CByte = AVR_I2C_START then
       HI2CStartOccurred = true
    else
       HI2CStartOccurred = false
    end if

End sub

Sub AVRHI2CReStart

    lds SysValueCopy,TWCR
    sbr   SysValueCopy, (1<<TWINT)|(1<<TWSTA)| (1<<TWEN)
    sts TWCR,SysValueCopy

    do while TWINT = 0
    loop


    I2CByte = TWSR & 0xF8

    ' extended code to ensure compiler gets the correctly.
    if I2CByte = AVR_I2C_START then
       HI2CStartOccurred = true
       goto exitAVRHI2CReStart
    else
       HI2CStartOccurred = false
    end if


    if I2CByte = AVR_I2C_ReSTART then
       HI2CStartOccurred = true
       goto exitAVRHI2CReStart
    else
       HI2CStartOccurred = false
    end if

exitAVRHI2CReStart:
End sub


Sub AVRHI2CStop

    lds SysValueCopy,TWCR
    sbr   SysValueCopy, (1<<TWINT)|(1<<TWEN)|(1<<TWSTO)
    sts TWCR,SysValueCopy

End Sub


Sub AVRHI2CSend ( In I2CByte )

        lds       SysValueCopy,I2CBYTE
        sts       TWDR,SysValueCopy
        ldi       SysValueCopy, (1<<TWINT) | (1<<TWEN)
        sts       TWCR, SysValueCopy

        nop
        nop
        do while TWINT = 0
        loop


        'Destructive use of I2CByte to save memory
        I2CByte = TWSR & 0xF8

        HI2CAckPollState = true
        Select case I2CByte
               case AVR_I2C_START
                    ' dummy
               case MT_SLA_ACK
                  HI2CAckPollState = false
               case MT_DATA_ACK
                  HI2CAckPollState = false
               case MT_SLA_NACK_REC
                   HI2CAckPollState = true
               case MT_DATA_NACK_REC
                   HI2CAckPollState = true
               case MR_SLA_NACK_REC
                   HI2CAckPollState = true
               case MR_SLA_ACK
                   HI2CAckPollState = false
               case MR_DATA_NACK_REC
                   HI2CAckPollState = true
               case else
                    ' bad event!!
        end select

        HI2CStartOccurred = false

End Sub


Sub AVRHI2CReceive (Out I2CByte, Optional In HI2CGetAck = 1 )

        if HI2CGetAck.0 = 0 then

          lds SysValueCopy,TWCR
          sbr       SysValueCopy, (1<<TWINT)
          cbr       SysValueCopy, (1<<TWSTA) | (1<<TWSTO) | (1<<TWEA)
          sts TWCR,SysValueCopy

        else
            ' Acknowledge
          lds SysValueCopy,TWCR
          cbr       SysValueCopy, (1<<TWSTA) | (1<<TWSTO)
          sbr       SysValueCopy, (1<<TWINT) | (1<<TWEA)
          sts TWCR,SysValueCopy
        end if

        nop
        nop
        do while TWINT = 0
        loop


        'Get byte
        I2CByte = TWDR

End Sub



sub HIC2Init
    HI2CCurrentMode = 0
end sub
