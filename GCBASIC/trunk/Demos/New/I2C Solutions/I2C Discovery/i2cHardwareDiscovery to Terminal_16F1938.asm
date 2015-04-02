;Program compiled by Great Cow BASIC (0.9 2015-03-30)
;Need help? See the GCBASIC forums at http://sourceforge.net/projects/gcbasic/forums,
;check the documentation or email w_cholmondeley at users dot sourceforge dot net.

;********************************************************************************

;Set up the assembler options (Chip type, clock source, other bits and pieces)
 LIST p=16F1938, r=DEC
#include <P16F1938.inc>
 __CONFIG _CONFIG1, _MCLRE_ON & _WDTE_OFF & _FOSC_INTOSC
 __CONFIG _CONFIG2, _LVP_OFF & _PLLEN_ON & _VCAPEN_OFF

;********************************************************************************

;Set aside memory locations for variables
DELAYTEMP	EQU	112
DELAYTEMP2	EQU	113
SysStringA	EQU	119
SysStringA_H	EQU	120
SysWaitTempMS	EQU	114
SysWaitTempMS_H	EQU	115
HEX	EQU	9194
DEVICEID	EQU	32
HI2CACKPOLLSTATE	EQU	33
HI2CCURRENTMODE	EQU	34
HSERPRINTCRLFCOUNT	EQU	35
I2CBYTE	EQU	36
PR2_TEMP	EQU	37
PR2_TEMP_H	EQU	38
PRINTLEN	EQU	39
SERDATA	EQU	40
SLAVE10	EQU	41
SYSPRINTTEMP	EQU	42
SYSSTRINGTEMP	EQU	43
SYSVALTEMP	EQU	44
StringPointer	EQU	45
SysPRINTDATAHandler	EQU	46
SysPRINTDATAHandler_H	EQU	47
SysRepeatTemp1	EQU	48
TEMPVAR	EQU	49
TEMPVAR_H	EQU	50
TESTID	EQU	51

;********************************************************************************

;Alias variables
AFSR0	EQU	4
AFSR0_H	EQU	5
SYSHEX_0	EQU	1610
SYSHEX_1	EQU	1611
SYSHEX_2	EQU	1612

;********************************************************************************

;Vectors
	ORG	0
	goto	BASPROGRAMSTART
	ORG	4
	retfie

;********************************************************************************

;Start of program memory page 0
	ORG	5
BASPROGRAMSTART
;Call initialisation routines
	call	INITSYS
	call	INITUSART
	call	AVRHIC2INIT

;Start of the main program
;''A demonstration program for GCGB and GCB.
;''--------------------------------------------------------------------------------------------------------------------------------
;''This program searches for I2C devices and display their adresses to the attached PC terminal.
;''This program uses the hardware implementation of I2C and TWI for the Microchip and ATMEL microprocessors.  The microprocessor must have a suitable I2C or TWI module.
;''The hardware serial connections are shown in the program.
;''This program requires "DisplayDevices.h" in the same folder.
;''@author 	EvanV
;''@licence	GPL
;''@version	1.0a
;''@date   	14.02.15
;''********************************************************************************
;----- Configuration
;----- Define Hardware settings
;#define HI2C_BAUD_RATE 100
;#define HI2C_DATA PORTC.4
;#define HI2C_CLOCK PORTC.3
;Dir HI2C_DATA in
	banksel	TRISC
	bsf	TRISC,4
;Dir HI2C_CLOCK in
	bsf	TRISC,3
;HI2CMode Master
	movlw	12
	banksel	HI2CCURRENTMODE
	movwf	HI2CCURRENTMODE
	call	HI2CMODE
;#define USART_BAUD_RATE 9600
;Dir PORTc.6 Out
	banksel	TRISC
	bcf	TRISC,6
;Dir PORTc.7 In
	bsf	TRISC,7
;#define USART_DELAY 5 ms
;#define SerSendDelayms 10
;#define USART_BLOCKING
;wait 500 ms
	movlw	244
	movwf	SysWaitTempMS
	movlw	1
	movwf	SysWaitTempMS_H
	banksel	STATUS
	call	Delay_MS
;----- Main body of program commences here.
;HSerPrintCRLF
	movlw	1
	movwf	HSERPRINTCRLFCOUNT
	call	HSERPRINTCRLF
;HSerPrint "Hardware I2C Device Search for Microchip"
	movlw	low StringTable1
	movwf	SysPRINTDATAHandler
	movlw	(high StringTable1) | 128
	movwf	SysPRINTDATAHandler_H
	call	HSERPRINT170
;HSerPrintCRLF 2
	movlw	2
	movwf	HSERPRINTCRLFCOUNT
	call	HSERPRINTCRLF
;for deviceID = 0 to 255
	movlw	255
	movwf	DEVICEID
SysForLoop1
	incf	DEVICEID,F
;HI2CStart
	call	HI2CSTART
;HI2CSend ( deviceID )
	movf	DEVICEID,W
	movwf	I2CBYTE
	call	HI2CSEND
;if HI2CAckPollState = false then
	movf	HI2CACKPOLLSTATE,F
	btfss	STATUS, Z
	goto	ENDIF1
;HSerPrint   "ID: 0x"
	movlw	low StringTable2
	movwf	SysPRINTDATAHandler
	movlw	(high StringTable2) | 128
	movwf	SysPRINTDATAHandler_H
	call	HSERPRINT170
;HSerPrint   hex(deviceID)
	movf	DEVICEID,W
	movwf	SYSVALTEMP
	call	FN_HEX
	movlw	low HEX
	movwf	SysPRINTDATAHandler
	movlw	high HEX
	movwf	SysPRINTDATAHandler_H
	call	HSERPRINT170
;HSerSend 9
	movlw	9
	movwf	SERDATA
	call	HSERSEND
;testid = deviceID | 1
	movlw	1
	iorwf	DEVICEID,W
	movwf	TESTID
;DisplayDevices
	call	DISPLAYDEVICES
;HI2CSend ( 0 )
	clrf	I2CBYTE
	call	HI2CSEND
;HI2CSend ( 0 )
	clrf	I2CBYTE
	call	HI2CSEND
;HSerPrintCRLF
	movlw	1
	movwf	HSERPRINTCRLFCOUNT
	call	HSERPRINTCRLF
;end if
ENDIF1
;HI2CStop
	call	HI2CSTOP
;next
	movlw	255
	subwf	DEVICEID,W
	btfss	STATUS, C
	goto	SysForLoop1
SysForLoopEnd1
;HSerPrint   "End of Device Search"
	movlw	low StringTable3
	movwf	SysPRINTDATAHandler
	movlw	(high StringTable3) | 128
	movwf	SysPRINTDATAHandler_H
	call	HSERPRINT170
;HSerPrintCRLF 2
	movlw	2
	movwf	HSERPRINTCRLFCOUNT
	call	HSERPRINTCRLF
;#define LCD_I2C_Address_1 0x4E
;#define slow_us 40
;#define medium_us 20
;#define fast_us 10
BASPROGRAMEND
	sleep
	goto	BASPROGRAMEND

;********************************************************************************

AVRHIC2INIT
;HI2CCurrentMode = 0
	clrf	HI2CCURRENTMODE
	return

;********************************************************************************

DISPLAYDEVICES
;select case testid
;case 49
SysSelect1Case1
	movlw	49
	subwf	TESTID,W
	btfss	STATUS, Z
	goto	SysSelect1Case2
;Hserprint "DS2482_1Channel_1Wire Master"
	movlw	low StringTable4
	movwf	SysPRINTDATAHandler
	movlw	(high StringTable4) | 128
	movwf	SysPRINTDATAHandler_H
	call	HSERPRINT170
;case 65
	goto	SysSelectEnd1
SysSelect1Case2
	movlw	65
	subwf	TESTID,W
	btfss	STATUS, Z
	goto	SysSelect1Case3
;Hserprint "Serial_Expander_Device"
	movlw	low StringTable5
	movwf	SysPRINTDATAHandler
	movlw	(high StringTable5) | 128
	movwf	SysPRINTDATAHandler_H
	call	HSERPRINT170
;case 73
	goto	SysSelectEnd1
SysSelect1Case3
	movlw	73
	subwf	TESTID,W
	btfss	STATUS, Z
	goto	SysSelect1Case4
;Hserprint "IC2/Serial_Expander_Device"
	movlw	low StringTable6
	movwf	SysPRINTDATAHandler
	movlw	(high StringTable6) | 128
	movwf	SysPRINTDATAHandler_H
	call	HSERPRINT170
;case 75
	goto	SysSelectEnd1
SysSelect1Case4
	movlw	75
	subwf	TESTID,W
	btfss	STATUS, Z
	goto	SysSelect1Case5
;Hserprint "IC2/Serial_Expander_Device"
	movlw	low StringTable6
	movwf	SysPRINTDATAHandler
	movlw	(high StringTable6) | 128
	movwf	SysPRINTDATAHandler_H
	call	HSERPRINT170
;case 77
	goto	SysSelectEnd1
SysSelect1Case5
	movlw	77
	subwf	TESTID,W
	btfss	STATUS, Z
	goto	SysSelect1Case6
;Hserprint "IC2/Serial_Expander_Device"
	movlw	low StringTable6
	movwf	SysPRINTDATAHandler
	movlw	(high StringTable6) | 128
	movwf	SysPRINTDATAHandler_H
	call	HSERPRINT170
;case 79
	goto	SysSelectEnd1
SysSelect1Case6
	movlw	79
	subwf	TESTID,W
	btfss	STATUS, Z
	goto	SysSelect1Case7
;Hserprint "IC2/Serial_Expander_Device"
	movlw	low StringTable6
	movwf	SysPRINTDATAHandler
	movlw	(high StringTable6) | 128
	movwf	SysPRINTDATAHandler_H
	call	HSERPRINT170
;case 97
	goto	SysSelectEnd1
SysSelect1Case7
	movlw	97
	subwf	TESTID,W
	btfss	STATUS, Z
	goto	SysSelect1Case8
;Hserprint "GCB Slave Device"
	movlw	low StringTable7
	movwf	SysPRINTDATAHandler
	movlw	(high StringTable7) | 128
	movwf	SysPRINTDATAHandler_H
	call	HSERPRINT170
;case 161
	goto	SysSelectEnd1
SysSelect1Case8
	movlw	161
	subwf	TESTID,W
	btfss	STATUS, Z
	goto	SysSelect1Case9
;Hserprint "EEProm_Device_Device"
	movlw	low StringTable8
	movwf	SysPRINTDATAHandler
	movlw	(high StringTable8) | 128
	movwf	SysPRINTDATAHandler_H
	call	HSERPRINT170
;case 163
	goto	SysSelectEnd1
SysSelect1Case9
	movlw	163
	subwf	TESTID,W
	btfss	STATUS, Z
	goto	SysSelect1Case10
;Hserprint "EEProm_Device_Device"
	movlw	low StringTable8
	movwf	SysPRINTDATAHandler
	movlw	(high StringTable8) | 128
	movwf	SysPRINTDATAHandler_H
	call	HSERPRINT170
;case 165
	goto	SysSelectEnd1
SysSelect1Case10
	movlw	165
	subwf	TESTID,W
	btfss	STATUS, Z
	goto	SysSelect1Case11
;Hserprint "EEProm_Device_Device"
	movlw	low StringTable8
	movwf	SysPRINTDATAHandler
	movlw	(high StringTable8) | 128
	movwf	SysPRINTDATAHandler_H
	call	HSERPRINT170
;case 167
	goto	SysSelectEnd1
SysSelect1Case11
	movlw	167
	subwf	TESTID,W
	btfss	STATUS, Z
	goto	SysSelect1Case12
;Hserprint "EEProm_Device_Device"
	movlw	low StringTable8
	movwf	SysPRINTDATAHandler
	movlw	(high StringTable8) | 128
	movwf	SysPRINTDATAHandler_H
	call	HSERPRINT170
;case 169
	goto	SysSelectEnd1
SysSelect1Case12
	movlw	169
	subwf	TESTID,W
	btfss	STATUS, Z
	goto	SysSelect1Case13
;Hserprint "EEProm_Device_Device"
	movlw	low StringTable8
	movwf	SysPRINTDATAHandler
	movlw	(high StringTable8) | 128
	movwf	SysPRINTDATAHandler_H
	call	HSERPRINT170
;case 171
	goto	SysSelectEnd1
SysSelect1Case13
	movlw	171
	subwf	TESTID,W
	btfss	STATUS, Z
	goto	SysSelect1Case14
;Hserprint "EEProm_Device_Device"
	movlw	low StringTable8
	movwf	SysPRINTDATAHandler
	movlw	(high StringTable8) | 128
	movwf	SysPRINTDATAHandler_H
	call	HSERPRINT170
;case 173
	goto	SysSelectEnd1
SysSelect1Case14
	movlw	173
	subwf	TESTID,W
	btfss	STATUS, Z
	goto	SysSelect1Case15
;Hserprint "EEProm_Device_Device"
	movlw	low StringTable8
	movwf	SysPRINTDATAHandler
	movlw	(high StringTable8) | 128
	movwf	SysPRINTDATAHandler_H
	call	HSERPRINT170
;case 175
	goto	SysSelectEnd1
SysSelect1Case15
	movlw	175
	subwf	TESTID,W
	btfss	STATUS, Z
	goto	SysSelect1Case16
;Hserprint "EEProm_Device_Device"
	movlw	low StringTable8
	movwf	SysPRINTDATAHandler
	movlw	(high StringTable8) | 128
	movwf	SysPRINTDATAHandler_H
	call	HSERPRINT170
;case 199
	goto	SysSelectEnd1
SysSelect1Case16
	movlw	199
	subwf	TESTID,W
	btfss	STATUS, Z
	goto	SysSelect1Case17
;Hserprint "AXE033 I2C LCD Device"
	movlw	low StringTable9
	movwf	SysPRINTDATAHandler
	movlw	(high StringTable9) | 128
	movwf	SysPRINTDATAHandler_H
	call	HSERPRINT170
;case 209
	goto	SysSelectEnd1
SysSelect1Case17
	movlw	209
	subwf	TESTID,W
	btfss	STATUS, Z
	goto	SysSelect1Case18
;Hserprint "DS1307_RTC_Device"
	movlw	low StringTable10
	movwf	SysPRINTDATAHandler
	movlw	(high StringTable10) | 128
	movwf	SysPRINTDATAHandler_H
	call	HSERPRINT170
;case 249
	goto	SysSelectEnd1
SysSelect1Case18
	movlw	249
	subwf	TESTID,W
	btfss	STATUS, Z
	goto	SysSelect1Case19
;Hserprint "FRAM_Device"
	movlw	low StringTable11
	movwf	SysPRINTDATAHandler
	movlw	(high StringTable11) | 128
	movwf	SysPRINTDATAHandler_H
	call	HSERPRINT170
;case else
	goto	SysSelectEnd1
SysSelect1Case19
;Hserprint "Unknown_Device"
	movlw	low StringTable12
	movwf	SysPRINTDATAHandler
	movlw	(high StringTable12) | 128
	movwf	SysPRINTDATAHandler_H
	call	HSERPRINT170
;end select
SysSelect1Case20
SysSelectEnd1
	return

;********************************************************************************

Delay_MS
	incf	SysWaitTempMS_H, F
DMS_START
	movlw	14
	movwf	DELAYTEMP2
DMS_OUTER
	movlw	189
	movwf	DELAYTEMP
DMS_INNER
	decfsz	DELAYTEMP, F
	goto	DMS_INNER
	decfsz	DELAYTEMP2, F
	goto	DMS_OUTER
	decfsz	SysWaitTempMS, F
	goto	DMS_START
	decfsz	SysWaitTempMS_H, F
	goto	DMS_START
	return

;********************************************************************************

FN_HEX
;Hex(0) = 2
	movlw	2
	banksel	SYSHEX_0
	movwf	SYSHEX_0
;SysStringTemp = SysValTemp And 0x0F
	movlw	15
	banksel	SYSVALTEMP
	andwf	SYSVALTEMP,W
	movwf	SYSSTRINGTEMP
;If SysStringTemp > 9 Then SysStringTemp = SysStringTemp + 7
	sublw	9
	btfsc	STATUS, C
	goto	ENDIF3
	movlw	7
	addwf	SYSSTRINGTEMP,F
ENDIF3
;Hex(2) = SysStringTemp + 48
	movlw	48
	addwf	SYSSTRINGTEMP,W
	banksel	SYSHEX_2
	movwf	SYSHEX_2
;For SysStringTemp = 1 to 4
	banksel	SYSSTRINGTEMP
	clrf	SYSSTRINGTEMP
SysForLoop2
	incf	SYSSTRINGTEMP,F
;Rotate SysValTemp Right
	rrf	SYSVALTEMP,F
;Next
	movlw	4
	subwf	SYSSTRINGTEMP,W
	btfss	STATUS, C
	goto	SysForLoop2
SysForLoopEnd2
;SysStringTemp = SysValTemp And 0x0F
	movlw	15
	andwf	SYSVALTEMP,W
	movwf	SYSSTRINGTEMP
;If SysStringTemp > 9 Then SysStringTemp = SysStringTemp + 7
	sublw	9
	btfsc	STATUS, C
	goto	ENDIF5
	movlw	7
	addwf	SYSSTRINGTEMP,F
ENDIF5
;Hex(1) = SysStringTemp + 48
	movlw	48
	addwf	SYSSTRINGTEMP,W
	banksel	SYSHEX_1
	movwf	SYSHEX_1
	banksel	STATUS
	return

;********************************************************************************

HI2CMODE
;set SSPSTAT.SMP on
	banksel	SSPSTAT
	bsf	SSPSTAT,SMP
;set SSPCON1.CKP on
	bsf	SSPCON1,CKP
;set SSPCON1.WCOL Off
	bcf	SSPCON1,WCOL
;If HI2CCurrentMode = Master Then
	movlw	12
	banksel	HI2CCURRENTMODE
	subwf	HI2CCURRENTMODE,W
	btfss	STATUS, Z
	goto	ENDIF9
;set SSPCON1.SSPM3 on
	banksel	SSPCON1
	bsf	SSPCON1,SSPM3
;set SSPCON1.SSPM2 off
	bcf	SSPCON1,SSPM2
;set SSPCON1.SSPM1 off
	bcf	SSPCON1,SSPM1
;set SSPCON1.SSPM0 off
	bcf	SSPCON1,SSPM0
;SSPADD = HI2C_BAUD_TEMP And 127
	movlw	79
	movwf	SSPADD
;end if
ENDIF9
;if HI2CCurrentMode = Slave then
	banksel	HI2CCURRENTMODE
	movf	HI2CCURRENTMODE,F
	btfss	STATUS, Z
	goto	ENDIF10
;set SSPCON1.SSPM3 off
	banksel	SSPCON1
	bcf	SSPCON1,SSPM3
;set SSPCON1.SSPM2 on
	bsf	SSPCON1,SSPM2
;set SSPCON1.SSPM1 on
	bsf	SSPCON1,SSPM1
;set SSPCON1.SSPM0 off
	bcf	SSPCON1,SSPM0
;end if
ENDIF10
;if HI2CCurrentMode = Slave10 then
	banksel	SLAVE10
	movf	SLAVE10,W
	subwf	HI2CCURRENTMODE,W
	btfss	STATUS, Z
	goto	ENDIF11
;set SSPCON1.SSPM3 off
	banksel	SSPCON1
	bcf	SSPCON1,SSPM3
;set SSPCON1.SSPM2 on
	bsf	SSPCON1,SSPM2
;set SSPCON1.SSPM1 on
	bsf	SSPCON1,SSPM1
;set SSPCON1.SSPM0 on
	bsf	SSPCON1,SSPM0
;end if
ENDIF11
;set SSPCON1.SSPEN on
	banksel	SSPCON1
	bsf	SSPCON1,SSPEN
	banksel	STATUS
	return

;********************************************************************************

HI2CSEND
RETRYHI2CSEND
;SET SSPCON1.WCOL OFF
	banksel	SSPCON1
	bcf	SSPCON1,WCOL
;SSPBUF = I2CByte
	banksel	I2CBYTE
	movf	I2CBYTE,W
	banksel	SSPBUF
	movwf	SSPBUF
;HI2CWaitMSSP
	banksel	STATUS
	call	HI2CWAITMSSP
;if ACKSTAT =  1 then
	banksel	SSPCON2
	btfss	SSPCON2,ACKSTAT
	goto	ELSE14_1
;HI2CAckPollState = true
	movlw	255
	banksel	HI2CACKPOLLSTATE
	movwf	HI2CACKPOLLSTATE
;else
	goto	ENDIF14
ELSE14_1
;HI2CAckPollState = false
	banksel	HI2CACKPOLLSTATE
	clrf	HI2CACKPOLLSTATE
;end if
ENDIF14
;If SSPCON1.WCOL = On Then
	banksel	SSPCON1
	btfss	SSPCON1,WCOL
	goto	ENDIF15
;If HI2CCurrentMode <= 10 Then Goto RetryHI2CSend
	banksel	HI2CCURRENTMODE
	movf	HI2CCURRENTMODE,W
	sublw	10
	btfsc	STATUS, C
	goto	RETRYHI2CSEND
;End If
ENDIF15
;If HI2CCurrentMode <= 10 Then Set SSPCON1.CKP On
	banksel	HI2CCURRENTMODE
	movf	HI2CCURRENTMODE,W
	sublw	10
	btfss	STATUS, C
	goto	ENDIF16
	banksel	SSPCON1
	bsf	SSPCON1,CKP
ENDIF16
	banksel	STATUS
	return

;********************************************************************************

HI2CSTART
;If HI2CCurrentMode > 10 Then
	movf	HI2CCURRENTMODE,W
	sublw	10
	btfsc	STATUS, C
	goto	ELSE12_1
;Set SSPCON2.SEN On
	banksel	SSPCON2
	bsf	SSPCON2,SEN
;HI2CWaitMSSP
	banksel	STATUS
	call	HI2CWAITMSSP
;Else
	goto	ENDIF12
ELSE12_1
;Wait Until SSPSTAT.S = On
SysWaitLoop2
	banksel	SSPSTAT
	btfss	SSPSTAT,S
	goto	SysWaitLoop2
;End If
ENDIF12
	banksel	STATUS
	return

;********************************************************************************

HI2CSTOP
;If HI2CCurrentMode > 10 Then
	movf	HI2CCURRENTMODE,W
	sublw	10
	btfsc	STATUS, C
	goto	ELSE13_1
;wait while R_NOT_W = 1   'wait for completion of activities
SysWaitLoop3
	banksel	SSPSTAT
	btfsc	SSPSTAT,R_NOT_W
	goto	SysWaitLoop3
;Set SSPCON2.PEN On
	bsf	SSPCON2,PEN
;HI2CWaitMSSP
	banksel	STATUS
	call	HI2CWAITMSSP
;Else
	goto	ENDIF13
ELSE13_1
;Wait Until SSPSTAT.P = On
SysWaitLoop4
	banksel	SSPSTAT
	btfss	SSPSTAT,P
	goto	SysWaitLoop4
;End If
ENDIF13
	banksel	STATUS
	return

;********************************************************************************

HI2CWAITMSSP
;if SSPIF = 0 then goto $-1
	btfss	PIR1,SSPIF
	goto	$-1
;SSPIF = 0
	bcf	PIR1,SSPIF
	return

;********************************************************************************

;Overloaded signature: STRING:
HSERPRINT170
;PrintLen = PrintData(0)
	movf	SysPRINTDATAHandler,W
	movwf	AFSR0
	movf	SysPRINTDATAHandler_H,W
	movwf	AFSR0_H
	movf	INDF0,W
	movwf	PRINTLEN
;If PrintLen <> 0 then
	movf	PRINTLEN,F
	btfsc	STATUS, Z
	goto	ENDIF6
;for SysPrintTemp = 1 to PrintLen
	clrf	SYSPRINTTEMP
	movlw	1
	subwf	PRINTLEN,W
	btfss	STATUS, C
	goto	SysForLoopEnd3
SysForLoop3
	incf	SYSPRINTTEMP,F
;HSerSend(PrintData(SysPrintTemp))
	movf	SYSPRINTTEMP,W
	addwf	SysPRINTDATAHandler,W
	movwf	AFSR0
	movlw	0
	addwfc	SysPRINTDATAHandler_H,W
	movwf	AFSR0_H
	movf	INDF0,W
	movwf	SERDATA
	call	HSERSEND
;Wait USART_DELAY
	movlw	5
	movwf	SysWaitTempMS
	clrf	SysWaitTempMS_H
	call	Delay_MS
;next
	movf	PRINTLEN,W
	subwf	SYSPRINTTEMP,W
	btfss	STATUS, C
	goto	SysForLoop3
SysForLoopEnd3
;End If
ENDIF6
	return

;********************************************************************************

HSERPRINTCRLF
;repeat HSerPrintCRLFCount
	movf	HSERPRINTCRLFCOUNT,W
	movwf	SysRepeatTemp1
	btfsc	STATUS,Z
	goto	SysRepeatLoopEnd1
SysRepeatLoop1
;HSerSend(13)
	movlw	13
	movwf	SERDATA
	call	HSERSEND
;Wait USART_DELAY
	movlw	5
	movwf	SysWaitTempMS
	clrf	SysWaitTempMS_H
	call	Delay_MS
;HSerSend(10)
	movlw	10
	movwf	SERDATA
	call	HSERSEND
;end Repeat
	decfsz	SysRepeatTemp1,F
	goto	SysRepeatLoop1
SysRepeatLoopEnd1
	return

;********************************************************************************

HSERSEND
;HSerSendBlocker
;HSerSendBlockCheck
;Wait While TXIF = Off
SysWaitLoop1
	btfss	PIR1,TXIF
	goto	SysWaitLoop1
;TXREG = SerData
	movf	SERDATA,W
	banksel	TXREG
	movwf	TXREG
	banksel	STATUS
	return

;********************************************************************************

INITSYS
;Set IRCF3 On
	banksel	OSCCON
	bsf	OSCCON,IRCF3
;Set IRCF2 On
	bsf	OSCCON,IRCF2
;Set IRCF1 On
	bsf	OSCCON,IRCF1
;Set IRCF0 Off
	bcf	OSCCON,IRCF0
;Set SPLLEN On
	bsf	OSCCON,SPLLEN
;SET ADCON0.ADON OFF
	bcf	ADCON0,ADON
;SET ADFM OFF
	bcf	ADCON1,ADFM
;ANSELA = 0
	banksel	ANSELA
	clrf	ANSELA
;ANSELB = 0
	clrf	ANSELB
;C2ON = 0
	banksel	CM2CON0
	bcf	CM2CON0,C2ON
;C1ON = 0
	bcf	CM1CON0,C1ON
;PORTA = 0
	banksel	PORTA
	clrf	PORTA
;PORTB = 0
	clrf	PORTB
;PORTC = 0
	clrf	PORTC
;PORTE = 0
	clrf	PORTE
	return

;********************************************************************************

INITUSART
;SPBRG = SPBRGL_TEMP
	movlw	64
	banksel	SPBRG
	movwf	SPBRG
;SPBRGH = SPBRGH_TEMP
	movlw	3
	movwf	SPBRGH
;BRG16 = BRG16_TEMP
	bsf	BAUDCON,BRG16
;BRGH = BRGH_TEMP
	bsf	TXSTA,BRGH
;Set SYNC Off
	bcf	TXSTA,SYNC
;Set SPEN On
	bsf	RCSTA,SPEN
;Set CREN On
	bsf	RCSTA,CREN
;Set TXEN On
	bsf	TXSTA,TXEN
	banksel	STATUS
	return

;********************************************************************************

SysStringTables
	movf	SysStringA_H,W
	movwf	PCLATH
	movf	SysStringA,W
	incf	SysStringA,F
	btfsc	STATUS,Z
	incf	SysStringA_H,F
	movwf	PCL

StringTable1
	retlw	40
	retlw	72	;H
	retlw	97	;a
	retlw	114	;r
	retlw	100	;d
	retlw	119	;w
	retlw	97	;a
	retlw	114	;r
	retlw	101	;e
	retlw	32	; 
	retlw	73	;I
	retlw	50	;2
	retlw	67	;C
	retlw	32	; 
	retlw	68	;D
	retlw	101	;e
	retlw	118	;v
	retlw	105	;i
	retlw	99	;c
	retlw	101	;e
	retlw	32	; 
	retlw	83	;S
	retlw	101	;e
	retlw	97	;a
	retlw	114	;r
	retlw	99	;c
	retlw	104	;h
	retlw	32	; 
	retlw	102	;f
	retlw	111	;o
	retlw	114	;r
	retlw	32	; 
	retlw	77	;M
	retlw	105	;i
	retlw	99	;c
	retlw	114	;r
	retlw	111	;o
	retlw	99	;c
	retlw	104	;h
	retlw	105	;i
	retlw	112	;p


StringTable2
	retlw	6
	retlw	73	;I
	retlw	68	;D
	retlw	58	;:
	retlw	32	; 
	retlw	48	;0
	retlw	120	;x


StringTable3
	retlw	20
	retlw	69	;E
	retlw	110	;n
	retlw	100	;d
	retlw	32	; 
	retlw	111	;o
	retlw	102	;f
	retlw	32	; 
	retlw	68	;D
	retlw	101	;e
	retlw	118	;v
	retlw	105	;i
	retlw	99	;c
	retlw	101	;e
	retlw	32	; 
	retlw	83	;S
	retlw	101	;e
	retlw	97	;a
	retlw	114	;r
	retlw	99	;c
	retlw	104	;h


StringTable4
	retlw	28
	retlw	68	;D
	retlw	83	;S
	retlw	50	;2
	retlw	52	;4
	retlw	56	;8
	retlw	50	;2
	retlw	95	;_
	retlw	49	;1
	retlw	67	;C
	retlw	104	;h
	retlw	97	;a
	retlw	110	;n
	retlw	110	;n
	retlw	101	;e
	retlw	108	;l
	retlw	95	;_
	retlw	49	;1
	retlw	87	;W
	retlw	105	;i
	retlw	114	;r
	retlw	101	;e
	retlw	32	; 
	retlw	77	;M
	retlw	97	;a
	retlw	115	;s
	retlw	116	;t
	retlw	101	;e
	retlw	114	;r


StringTable5
	retlw	22
	retlw	83	;S
	retlw	101	;e
	retlw	114	;r
	retlw	105	;i
	retlw	97	;a
	retlw	108	;l
	retlw	95	;_
	retlw	69	;E
	retlw	120	;x
	retlw	112	;p
	retlw	97	;a
	retlw	110	;n
	retlw	100	;d
	retlw	101	;e
	retlw	114	;r
	retlw	95	;_
	retlw	68	;D
	retlw	101	;e
	retlw	118	;v
	retlw	105	;i
	retlw	99	;c
	retlw	101	;e


StringTable6
	retlw	26
	retlw	73	;I
	retlw	67	;C
	retlw	50	;2
	retlw	47	;/
	retlw	83	;S
	retlw	101	;e
	retlw	114	;r
	retlw	105	;i
	retlw	97	;a
	retlw	108	;l
	retlw	95	;_
	retlw	69	;E
	retlw	120	;x
	retlw	112	;p
	retlw	97	;a
	retlw	110	;n
	retlw	100	;d
	retlw	101	;e
	retlw	114	;r
	retlw	95	;_
	retlw	68	;D
	retlw	101	;e
	retlw	118	;v
	retlw	105	;i
	retlw	99	;c
	retlw	101	;e


StringTable7
	retlw	16
	retlw	71	;G
	retlw	67	;C
	retlw	66	;B
	retlw	32	; 
	retlw	83	;S
	retlw	108	;l
	retlw	97	;a
	retlw	118	;v
	retlw	101	;e
	retlw	32	; 
	retlw	68	;D
	retlw	101	;e
	retlw	118	;v
	retlw	105	;i
	retlw	99	;c
	retlw	101	;e


StringTable8
	retlw	20
	retlw	69	;E
	retlw	69	;E
	retlw	80	;P
	retlw	114	;r
	retlw	111	;o
	retlw	109	;m
	retlw	95	;_
	retlw	68	;D
	retlw	101	;e
	retlw	118	;v
	retlw	105	;i
	retlw	99	;c
	retlw	101	;e
	retlw	95	;_
	retlw	68	;D
	retlw	101	;e
	retlw	118	;v
	retlw	105	;i
	retlw	99	;c
	retlw	101	;e


StringTable9
	retlw	21
	retlw	65	;A
	retlw	88	;X
	retlw	69	;E
	retlw	48	;0
	retlw	51	;3
	retlw	51	;3
	retlw	32	; 
	retlw	73	;I
	retlw	50	;2
	retlw	67	;C
	retlw	32	; 
	retlw	76	;L
	retlw	67	;C
	retlw	68	;D
	retlw	32	; 
	retlw	68	;D
	retlw	101	;e
	retlw	118	;v
	retlw	105	;i
	retlw	99	;c
	retlw	101	;e


StringTable10
	retlw	17
	retlw	68	;D
	retlw	83	;S
	retlw	49	;1
	retlw	51	;3
	retlw	48	;0
	retlw	55	;7
	retlw	95	;_
	retlw	82	;R
	retlw	84	;T
	retlw	67	;C
	retlw	95	;_
	retlw	68	;D
	retlw	101	;e
	retlw	118	;v
	retlw	105	;i
	retlw	99	;c
	retlw	101	;e


StringTable11
	retlw	11
	retlw	70	;F
	retlw	82	;R
	retlw	65	;A
	retlw	77	;M
	retlw	95	;_
	retlw	68	;D
	retlw	101	;e
	retlw	118	;v
	retlw	105	;i
	retlw	99	;c
	retlw	101	;e


StringTable12
	retlw	14
	retlw	85	;U
	retlw	110	;n
	retlw	107	;k
	retlw	110	;n
	retlw	111	;o
	retlw	119	;w
	retlw	110	;n
	retlw	95	;_
	retlw	68	;D
	retlw	101	;e
	retlw	118	;v
	retlw	105	;i
	retlw	99	;c
	retlw	101	;e


;********************************************************************************

;Start of program memory page 1
	ORG	2048
;Start of program memory page 2
	ORG	4096
;Start of program memory page 3
	ORG	6144
;Start of program memory page 4
	ORG	8192
;Start of program memory page 5
	ORG	10240
;Start of program memory page 6
	ORG	12288
;Start of program memory page 7
	ORG	14336

 END
