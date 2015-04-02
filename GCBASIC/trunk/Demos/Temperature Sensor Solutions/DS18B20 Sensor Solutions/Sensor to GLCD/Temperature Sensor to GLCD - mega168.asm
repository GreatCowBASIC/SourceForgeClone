;Program compiled by Great Cow BASIC (0.9 2015-03-30)
;Need help? See the GCBASIC forums at http://sourceforge.net/projects/gcbasic/forums,
;check the documentation or email w_cholmondeley at users dot sourceforge dot net.

;********************************************************************************

;Assembler header file
.INCLUDE "m168def.inc"

;SREG bit names (for AVR Assembler compatibility, GCBASIC uses different names)
#define I 7
#define T 6
#define H 5
#define S 4
#define V 3
#define N 2
#define Z 1
#define C 0

;********************************************************************************

;Set aside memory locations for variables
.EQU	DSDEC=256
.EQU	SYSSTRINGPARAM1=297
.EQU	KS0108=338
.EQU	STR=346
.EQU	HEX=352
.EQU	CCOUNT=356
.EQU	CHARCODE=357
.EQU	CHARLOCX=358
.EQU	CHARLOCY=359
.EQU	CLOCKSA=360
.EQU	CLOCKSB=361
.EQU	COMMAND=362
.EQU	CURRCHARCOL=363
.EQU	CURRCHARROW=364
.EQU	CURRCHARVAL=365
.EQU	CURRCOL=366
.EQU	CURRPAGE=367
.EQU	DSDATA=368
.EQU	DSDATA_H=369
.EQU	DSDATA×9=371
.EQU	DSINT=372
.EQU	DSINT_H=373
.EQU	FRACTION=374
.EQU	GLCDBACKGROUND=375
.EQU	GLCDBACKGROUND_H=376
.EQU	GLCDBITNO=377
.EQU	GLCDCHANGE=378
.EQU	GLCDCOLOUR=379
.EQU	GLCDCOLOUR_H=380
.EQU	GLCDDATATEMP=381
.EQU	GLCDFNTDEFAULT=382
.EQU	GLCDFONTWIDTH=383
.EQU	GLCDFOREGROUND=384
.EQU	GLCDFOREGROUND_H=385
.EQU	GLCDPRINTLOC=386
.EQU	GLCDREADBYTE_KS0108=387
.EQU	GLCDX=388
.EQU	GLCDY=389
.EQU	GLCD_COUNT=390
.EQU	GLCD_YORDINATE=391
.EQU	GLCD_YORDINATE_H=392
.EQU	KS0108_CLOCK_DELAY=393
.EQU	LCDBYTE=394
.EQU	LINECOLOUR=395
.EQU	LINECOLOUR_H=396
.EQU	PR2_TEMP=397
.EQU	PR2_TEMP_H=398
.EQU	PRINTLEN=399
.EQU	PRINTLOCX=400
.EQU	PRINTLOCY=401
.EQU	READTEMP=402
.EQU	READTEMP_H=403
.EQU	RXDATA=404
.EQU	RXDATA_H=405
.EQU	SIGNBIT=406
.EQU	ST7735_GLCD_HEIGHT=407
.EQU	ST7735_GLCD_WIDTH=408
.EQU	SYSBITVAR0=409
.EQU	SYSCHARCOUNT=410
.EQU	SYSPRINTTEMP=411
.EQU	SYSREPEATTEMP1=412
.EQU	SYSSTRDATA=413
.EQU	SYSSTRINGTEMP=414
.EQU	SYSVALTEMP=415
.EQU	SYSVALTEMP_H=416
.EQU	StringPointer=417
.EQU	SysLCDPRINTDATAHandler=418
.EQU	SysLCDPRINTDATAHandler_H=419
.EQU	SysPointerX=420
.EQU	TEMPC_100=421
.EQU	TEMPC_100_H=422
.EQU	TEMPVAR=423
.EQU	TEMPVAR_H=424
.EQU	WHOLE=425

;********************************************************************************

;Register variables
.DEF	DELAYTEMP=r25
.DEF	DELAYTEMP2=r26
.DEF	SYSCALCTEMPX=r0
.DEF	SYSCALCTEMPX_H=r1
.DEF	SYSDIVMULTA=r24
.DEF	SYSDIVMULTA_H=r25
.DEF	SYSDIVMULTB=r30
.DEF	SYSDIVMULTB_H=r31
.DEF	SYSDIVMULTX=r2
.DEF	SYSDIVMULTX_H=r3
.DEF	SYSINTEGERTEMPA=r22
.DEF	SYSINTEGERTEMPA_H=r23
.DEF	SYSINTEGERTEMPA_H=r23
.DEF	SYSINTEGERTEMPB=r28
.DEF	SYSINTEGERTEMPB_H=r29
.DEF	SYSINTEGERTEMPB_H=r29
.DEF	SYSINTEGERTEMPX=r0
.DEF	SYSINTEGERTEMPX_H=r1
.DEF	SYSSIGNBYTE=r4
.DEF	SysBYTETempA=r22
.DEF	SysBYTETempB=r28
.DEF	SysBYTETempX=r0
.DEF	SysBitTest=r5
.DEF	SysCalcTempA=r22
.DEF	SysCalcTempB=r28
.DEF	SysDivLoop=r5
.DEF	SysReadA=r30
.DEF	SysReadA_H=r31
.DEF	SysStringA=r26
.DEF	SysStringA_H=r27
.DEF	SysStringB=r28
.DEF	SysStringB_H=r29
.DEF	SysStringLength=r25
.DEF	SysValueCopy=r21
.DEF	SysWORDTempA=r22
.DEF	SysWORDTempA_H=r23
.DEF	SysWORDTempB=r28
.DEF	SysWORDTempB_H=r29
.DEF	SysWORDTempX=r0
.DEF	SysWORDTempX_H=r1
.DEF	SysWaitTemp10US=r27
.DEF	SysWaitTempMS=r29
.DEF	SysWaitTempMS_H=r30
.DEF	SysWaitTempS=r31
.DEF	SysWaitTempUS=r27
.DEF	SysWaitTempUS_H=r28
.DEF	SysTemp1=r6
.DEF	SysTemp1_H=r7
.DEF	SysTemp2=r16
.DEF	SysTemp3=r8
.DEF	SysTemp4=r17

;********************************************************************************

;Alias variables
#define	SYSHEX_0	352
#define	SYSHEX_1	353
#define	SYSHEX_2	354
#define	SYSSTR_0	346

;********************************************************************************

;Vectors
;Interrupt vectors
.ORG	0
	rjmp	BASPROGRAMSTART ;Reset
.ORG	2
	reti	;INT0
.ORG	4
	reti	;INT1
.ORG	6
	reti	;PCINT0
.ORG	8
	reti	;PCINT1
.ORG	10
	reti	;PCINT2
.ORG	12
	reti	;WDT
.ORG	14
	reti	;TIMER2_COMPA
.ORG	16
	reti	;TIMER2_COMPB
.ORG	18
	reti	;TIMER2_OVF
.ORG	20
	reti	;TIMER1_CAPT
.ORG	22
	reti	;TIMER1_COMPA
.ORG	24
	reti	;TIMER1_COMPB
.ORG	26
	reti	;TIMER1_OVF
.ORG	28
	reti	;TIMER0_COMPA
.ORG	30
	reti	;TIMER0_COMPB
.ORG	32
	reti	;TIMER0_OVF
.ORG	34
	reti	;SPI_STC
.ORG	36
	reti	;USART_RX
.ORG	38
	reti	;USART_UDRE
.ORG	40
	reti	;USART_TX
.ORG	42
	reti	;ADC
.ORG	44
	reti	;EE_READY
.ORG	46
	reti	;ANALOG_COMP
.ORG	48
	reti	;TWI
.ORG	50
	reti	;SPM_READY

;********************************************************************************

;Start of program memory page 0
.ORG	52
BASPROGRAMSTART:
;Initialise stack
	ldi	SysValueCopy,high(RAMEND)
	out	SPH, SysValueCopy
	ldi	SysValueCopy,low(RAMEND)
	out	SPL, SysValueCopy
;Call initialisation routines
	rcall	INITSYS
	rcall	INITGLCD_KS0108

;Start of the main program
;''A demonstration program for GCGB and GCB.
;''--------------------------------------------------------------------------------------------------------------------------------
;''This program reads the value of a DS18B20 connected to Portc.3.  The DS18B20 must be setup to correctly.
;''The GLCD display connection must be to the correct ports as shown in the hardware section.
;''The GLCD shows the integer value and the decimal value of the temperature.
;''@author 	EvanV plus works of others
;''@licence	GPL
;''@version	1.0a
;''@date   	31.01.2015
;''********************************************************************************
;----- Configuration
;----- Define Hardware settings
;----- Constants
;#define GLCD_TYPE GLCD_TYPE_KS0108               ' This is the Default value, not required.
;#define GLCD_RW PORTB.1
;#define GLCD_RESET PORTB.5
;#define GLCD_CS1 PORTB.3
;#define GLCD_CS2 PORTB.4
;#define GLCD_RS PORTB.0
;#define GLCD_ENABLE PORTB.2
;#define GLCD_DB0 PORTD.0
;#define GLCD_DB1 PORTD.1
;#define GLCD_DB2 PORTD.2
;#define GLCD_DB3 PORTD.3
;#define GLCD_DB4 PORTD.4
;#define GLCD_DB5 PORTD.5
;#define GLCD_DB6 PORTD.6
;#define GLCD_DB7 PORTD.7
;----- Constants
;#define DQ PortC.3
;----- Variables
;dim TempC_100 as word
;----- Main body of program commences here.
;ccount = 1
	ldi	SysValueCopy,1
	sts	CCOUNT,SysValueCopy
;GLCDCLS
	rcall	GLCDCLS_KS0108
;GLCDPrint(0, 0, "GCBasic 2015" )
	ldi	SysValueCopy,0
	sts	PRINTLOCX,SysValueCopy
	ldi	SysValueCopy,0
	sts	PRINTLOCY,SysValueCopy
	ldi	SysStringB,low(SYSSTRINGPARAM1)
	ldi	SysStringB_H,high(SYSSTRINGPARAM1)
	ldi	SysReadA,low(StringTable1<<1)
	ldi	SysReadA_H,high(StringTable1<<1)
	rcall	SysReadString
	ldi	SysValueCopy,low(SYSSTRINGPARAM1)
	sts	SysLCDPRINTDATAHandler,SysValueCopy
	ldi	SysValueCopy,high(SYSSTRINGPARAM1)
	sts	SysLCDPRINTDATAHandler_H,SysValueCopy
	rcall	GLCDPRINT3
;GLCDPrint(0, 8, "DS18B20 Demo" )
	ldi	SysValueCopy,0
	sts	PRINTLOCX,SysValueCopy
	ldi	SysValueCopy,8
	sts	PRINTLOCY,SysValueCopy
	ldi	SysStringB,low(SYSSTRINGPARAM1)
	ldi	SysStringB_H,high(SYSSTRINGPARAM1)
	ldi	SysReadA,low(StringTable2<<1)
	ldi	SysReadA_H,high(StringTable2<<1)
	rcall	SysReadString
	ldi	SysValueCopy,low(SYSSTRINGPARAM1)
	sts	SysLCDPRINTDATAHandler,SysValueCopy
	ldi	SysValueCopy,high(SYSSTRINGPARAM1)
	sts	SysLCDPRINTDATAHandler_H,SysValueCopy
	rcall	GLCDPRINT3
;do forever
SysDoLoop_S1:
;DSdata = readtemp
	rcall	FN_READTEMP
	lds	SysValueCopy,READTEMP
	sts	DSDATA,SysValueCopy
	lds	SysValueCopy,READTEMP_H
	sts	DSDATA_H,SysValueCopy
;GLCDPrint(0, 24, "Reading" )
	ldi	SysValueCopy,0
	sts	PRINTLOCX,SysValueCopy
	ldi	SysValueCopy,24
	sts	PRINTLOCY,SysValueCopy
	ldi	SysStringB,low(SYSSTRINGPARAM1)
	ldi	SysStringB_H,high(SYSSTRINGPARAM1)
	ldi	SysReadA,low(StringTable3<<1)
	ldi	SysReadA_H,high(StringTable3<<1)
	rcall	SysReadString
	ldi	SysValueCopy,low(SYSSTRINGPARAM1)
	sts	SysLCDPRINTDATAHandler,SysValueCopy
	ldi	SysValueCopy,high(SYSSTRINGPARAM1)
	sts	SysLCDPRINTDATAHandler_H,SysValueCopy
	rcall	GLCDPRINT3
;GLCDPrint(48, 24,  hex(ccount) )
	ldi	SysValueCopy,48
	sts	PRINTLOCX,SysValueCopy
	ldi	SysValueCopy,24
	sts	PRINTLOCY,SysValueCopy
	lds	SysValueCopy,CCOUNT
	sts	SYSVALTEMP,SysValueCopy
	rcall	FN_HEX
	ldi	SysVALueCopy,low(HEX)
	sts	SysLCDPRINTDATAHandler,SysValueCopy
	ldi	SysValueCopy,high(HEX)
	sts	SysLCDPRINTDATAHandler_H,SysValueCopy
	rcall	GLCDPRINT3
;GLCDPrint(66, 24,  str(DSdata) + "c" )
	ldi	SysValueCopy,66
	sts	PRINTLOCX,SysValueCopy
	ldi	SysValueCopy,24
	sts	PRINTLOCY,SysValueCopy
	lds	SysValueCopy,DSDATA
	sts	SYSVALTEMP,SysValueCopy
	lds	SysValueCopy,DSDATA_H
	sts	SYSVALTEMP_H,SysValueCopy
	rcall	FN_STR
	ldi	SysStringB,low(SYSSTRINGPARAM1)
	ldi	SysStringB_H,high(SYSSTRINGPARAM1)
	clr	SysStringLength
	ld	SysValueCopy, Y+
	ldi	SysStringA,low(STR)
	ldi	SysStringA_H,high(STR)
	rcall	SysCopyStringPart
	ldi	SysReadA,low(StringTable4<<1)
	ldi	SysReadA_H,high(StringTable4<<1)
	rcall	SysReadStringPart
	ldi	SysStringA,low(SYSSTRINGPARAM1)
	ldi	SysStringA_H,high(SYSSTRINGPARAM1)
	st	X, SysStringLength
	ldi	SysValueCopy,low(SYSSTRINGPARAM1)
	sts	SysLCDPRINTDATAHandler,SysValueCopy
	ldi	SysValueCopy,high(SYSSTRINGPARAM1)
	sts	SysLCDPRINTDATAHandler_H,SysValueCopy
	rcall	GLCDPRINT3
;DSdata = ((( DSdata × 9) /5) + 32 )
	lds	SysBYTETempA,DSDATA×9
	ldi	SysBYTETempB,5
	rcall	SysDivSub
	mov	SysTemp1,SysBYTETempA
	ldi	SysTemp2,32
	add	SysTemp1,SysTemp2
	sts	DSDATA,SysTemp1
	clr	SysTemp1
	brcc	PC + 2
	inc	SysTemp1
	sts	DSDATA_H,SysTemp1
;GLCDPrint(66, 32,  str(DSdata) + "f" )
	ldi	SysValueCopy,66
	sts	PRINTLOCX,SysValueCopy
	ldi	SysValueCopy,32
	sts	PRINTLOCY,SysValueCopy
	lds	SysValueCopy,DSDATA
	sts	SYSVALTEMP,SysValueCopy
	lds	SysValueCopy,DSDATA_H
	sts	SYSVALTEMP_H,SysValueCopy
	rcall	FN_STR
	ldi	SysStringB,low(SYSSTRINGPARAM1)
	ldi	SysStringB_H,high(SYSSTRINGPARAM1)
	clr	SysStringLength
	ld	SysValueCopy, Y+
	ldi	SysStringA,low(STR)
	ldi	SysStringA_H,high(STR)
	rcall	SysCopyStringPart
	ldi	SysReadA,low(StringTable5<<1)
	ldi	SysReadA_H,high(StringTable5<<1)
	rcall	SysReadStringPart
	ldi	SysStringA,low(SYSSTRINGPARAM1)
	ldi	SysStringA_H,high(SYSSTRINGPARAM1)
	st	X, SysStringLength
	ldi	SysValueCopy,low(SYSSTRINGPARAM1)
	sts	SysLCDPRINTDATAHandler,SysValueCopy
	ldi	SysValueCopy,high(SYSSTRINGPARAM1)
	sts	SysLCDPRINTDATAHandler_H,SysValueCopy
	rcall	GLCDPRINT3
;wait 2 s
	ldi	SysWaitTempS,2
	rcall	Delay_S
;ccount++
	lds	SysTemp1,CCOUNT
	inc	SysTemp1
	sts	CCOUNT,SysTemp1
;loop
	rjmp	SysDoLoop_S1
SysDoLoop_E1:
;#define KS0108ReadDelay   9     ; 2 normal usage, 3 for 32 mhz!
;#define KS0108WriteDelay  1     ; 1 normal usage, 0 works
;#define KS0108ClockDelay  1     ; 1 normal usage, 0 works
;#define ST7920ReadDelay = 20
;#define ST7920WriteDelay = 2
;#define LCD_I2C_Address_1 0x4E
;#define slow_us 40
;#define medium_us 20
;#define fast_us 10
BASPROGRAMEND:
	sleep
	rjmp	BASPROGRAMEND

;********************************************************************************

DS18B20DEVICEREAD:
;wait 250 ms ' Not really needed but left in just in case ***
	ldi	SysWaitTempMS,250
	ldi	SysWaitTempMS_H,0
	rcall	Delay_MS
;MasterRST   ' Reset ds18b20
	rcall	MASTERRST
;PPulse      ' Request a presence pulse
	rcall	PPULSE
;wait 1 ms
	ldi	SysWaitTempMS,1
	ldi	SysWaitTempMS_H,0
	rcall	Delay_MS
;OWout SkipRom  ' Bypass ds18b20 serial number read
	ldi	SysValueCopy,204
	sts	COMMAND,SysValueCopy
	rcall	OWOUT
;wait 1 ms
	ldi	SysWaitTempMS,1
	ldi	SysWaitTempMS_H,0
	rcall	Delay_MS
;OWout ConvertT  ' Instruct ds18b20 to begin temperature conversion to digital
	ldi	SysValueCopy,68
	sts	COMMAND,SysValueCopy
	rcall	OWOUT
;Do
SysDoLoop_S2:
;dir DQ out
	sbi	DDRC,3
;Set DQ Off
	cbi	PORTC,3
;wait 4 us
	ldi	DELAYTEMP,21
DelayUS2:
	dec	DELAYTEMP
	brne	DelayUS2
	nop
;Dir DQ In
	cbi	DDRC,3
;wait 7 us
	ldi	DELAYTEMP,37
DelayUS3:
	dec	DELAYTEMP
	brne	DelayUS3
	nop
;If DQ = 1 Then Exit Do
	sbic	PINC,3
	rjmp	SysDoLoop_E2
;wait 60 us    'minimum 60 us time slots between reads
	ldi	DELAYTEMP2,1
DelayUSO4:
	ldi	DELAYTEMP,0
DelayUS4:
	dec	DELAYTEMP
	brne	DelayUS4
	dec	DELAYTEMP2
	brne	DelayUSO4
	ldi	DELAYTEMP,63
DelayUS5:
	dec	DELAYTEMP
	brne	DelayUS5
;Loop
	rjmp	SysDoLoop_S2
SysDoLoop_E2:
;MasterRST
	rcall	MASTERRST
;PPulse
	rcall	PPULSE
;wait 1 ms
;OWout SkipRom         ' Bypass ds18b20 serial number read
	ldi	SysValueCopy,204
	sts	COMMAND,SysValueCopy
	rcall	OWOUT
;wait 1 ms
;OWout ReadScratch     ' Read temperature from db18b20
	ldi	SysValueCopy,190
	sts	COMMAND,SysValueCopy
	rcall	OWOUT
;wait 1 ms
;Owin                  ' Receive temperature low byte from db18b20
	rcall	OWIN
;DSdata = RxData       ' First byte  is low byte
	lds	SysValueCopy,RXDATA
	sts	DSDATA,SysValueCopy
	lds	SysValueCopy,RXDATA_H
	sts	DSDATA_H,SysValueCopy
;wait 1 ms
;Owin                  ' Receive temperature high byte from db18b20
	rcall	OWIN
;DSdata_H = RxData     ' Second byte is high byte  (auto combined into 16 bit word)
	lds	SysValueCopy,RXDATA
	sts	DSDATA_H,SysValueCopy
	ret

;********************************************************************************

Delay_10US:
D10US_START:
	ldi	DELAYTEMP,52
DelayUS1:
	dec	DELAYTEMP
	brne	DelayUS1
	nop
	dec	SysWaitTemp10US
	brne	D10US_START
	ret

;********************************************************************************

Delay_MS:
	inc	SysWaitTempMS_H
DMS_START:
	ldi	DELAYTEMP2,254
DMS_OUTER:
	ldi	DELAYTEMP,20
DMS_INNER:
	dec	DELAYTEMP
	brne	DMS_INNER
	dec	DELAYTEMP2
	brne	DMS_OUTER
	dec	SysWaitTempMS
	brne	DMS_START
	dec	SysWaitTempMS_H
	brne	DMS_START
	ret

;********************************************************************************

Delay_S:
DS_START:
	ldi	SysWaitTempMS,232
	ldi	SysWaitTempMS_H,3
	rcall	Delay_MS
	dec	SysWaitTempS
	brne	DS_START
	ret

;********************************************************************************

Delay_US:
	inc	SysWaitTempUS_H
	tst	SysWaitTempUS
	breq	DUS_END
DUS_START:
	ldi	DELAYTEMP,4
DelayUS0:
	dec	DELAYTEMP
	brne	DelayUS0
	nop
	dec	SysWaitTempUS
	brne	DUS_START
DUS_END:
	dec	SysWaitTempUS_H
	brne	DUS_START
	ret

;********************************************************************************

GLCDCHARCOL3:
	cpi	SysStringA, 113
	brlo	PC + 3
	clr	SysByteTempX
	ret
	ldi	SysReadA, low(TableGLCDCHARCOL3<<1)
	ldi	SysReadA_H, high(TableGLCDCHARCOL3<<1)
	add	SysReadA, SysStringA
	brcc	PC + 2
	inc	SysReadA_H
	lpm
	ret
TableGLCDCHARCOL3:
	.DB	112,0,16,12,10,136,34,56,32,8,32,16,16,128,128,64,4,0,0,0,40,72,70,108,0,0,0,40
	.DB	16,0,16,0,64,124,0,132,130,48,78,120,6,108,12,0,0,16,40,0,4,100,248,254,124,254
	.DB	254,254,124,254,0,64,254,254,254,254,124,254,124,254,76,2,126,62,126,198,14,194
	.DB	0,4,0,8,128,0,64,254,112,112,112,16,16,254,0,64,254,0,248,248,112,248,16,248
	.DB	144,16,120,56,120,136,24,136,0,0,0,32,120

;********************************************************************************

GLCDCHARCOL4:
	cpi	SysStringA, 113
	brlo	PC + 3
	clr	SysByteTempX
	ret
	ldi	SysReadA, low(TableGLCDCHARCOL4<<1)
	ldi	SysReadA_H, high(TableGLCDCHARCOL4<<1)
	add	SysReadA, SysStringA
	brcc	PC + 2
	inc	SysReadA_H
	lpm
	ret
TableGLCDCHARCOL4:
	.DB	112,254,56,10,6,204,102,124,112,4,64,16,56,136,162,112,28,0,0,14,254,84,38,146
	.DB	10,56,130,16,16,160,16,192,32,162,132,194,130,40,138,148,2,146,146,108,172,40
	.DB	40,130,2,146,36,146,130,130,146,18,130,16,130,128,16,128,4,8,130,18,130,18,146,2
	.DB	128,64,128,40,16,162,254,8,130,4,128,2,168,144,136,136,168,252,168,16,144,128
	.DB	32,130,8,16,136,40,40,16,168,124,128,64,128,80,160,200,16,0,130,16,68

;********************************************************************************

GLCDCHARCOL5:
	cpi	SysStringA, 113
	brlo	PC + 3
	clr	SysByteTempX
	ret
	ldi	SysReadA, low(TableGLCDCHARCOL5<<1)
	ldi	SysReadA_H, high(TableGLCDCHARCOL5<<1)
	add	SysReadA, SysStringA
	brcc	PC + 2
	inc	SysReadA_H
	lpm
	ret
TableGLCDCHARCOL5:
	.DB	112,124,124,0,0,238,238,124,168,254,254,84,84,148,148,124,124,0,158,0,40,254,16,170
	.DB	6,68,68,124,124,96,16,192,16,146,254,162,138,36,138,146,226,146,146,108,108,68
	.DB	40,68,162,242,34,146,130,130,146,18,146,16,254,130,40,128,24,16,130,18,162,50
	.DB	146,254,128,128,112,16,224,146,130,16,130,2,128,4,168,136,136,136,168,18,168,8
	.DB	250,136,80,254,240,8,136,40,40,8,168,144,128,128,96,32,160,168,108,254,108,16
	.DB	66

;********************************************************************************

GLCDCHARCOL6:
	cpi	SysStringA, 113
	brlo	PC + 3
	clr	SysByteTempX
	ret
	ldi	SysReadA, low(TableGLCDCHARCOL6<<1)
	ldi	SysReadA_H, high(TableGLCDCHARCOL6<<1)
	add	SysReadA, SysStringA
	brcc	PC + 2
	inc	SysReadA_H
	lpm
	ret
TableGLCDCHARCOL6:
	.DB	112,56,254,12,10,204,102,124,32,4,64,56,16,162,136,112,28,0,0,14,254,84,200,68
	.DB	0,130,56,16,16,0,16,0,8,138,128,146,150,254,138,146,18,146,82,0,0,130,40,40,18,130
	.DB	36,146,130,68,146,18,146,16,130,126,68,128,4,32,130,18,66,82,146,2,128,64,128,40
	.DB	16,138,130,32,254,4,128,8,168,136,136,144,168,2,168,8,128,122,136,128,8,8,136,40
	.DB	48,8,168,128,64,64,128,80,160,152,130,0,16,32,68

;********************************************************************************

GLCDCHARCOL7:
	cpi	SysStringA, 113
	brlo	PC + 3
	clr	SysByteTempX
	ret
	ldi	SysReadA, low(TableGLCDCHARCOL7<<1)
	ldi	SysReadA_H, high(TableGLCDCHARCOL7<<1)
	add	SysReadA, SysStringA
	brcc	PC + 2
	inc	SysReadA_H
	lpm
	ret
TableGLCDCHARCOL7:
	.DB	112,16,0,10,6,136,34,56,62,8,32,16,16,128,128,64,4,0,0,0,40,36,196,160,0,0,0,40
	.DB	16,0,16,0,4,124,0,140,98,32,114,96,14,108,60,0,0,0,40,16,12,124,248,108,68,56
	.DB	130,2,244,254,0,2,130,128,254,254,124,12,188,140,100,2,126,62,126,198,14,134,0,64
	.DB	0,8,128,0,240,112,64,254,48,4,120,240,0,0,0,0,240,240,112,16,248,16,64,64,248,56
	.DB	120,136,120,136,0,0,0,16,120

;********************************************************************************

GLCDCLS_KS0108:
;GLCD_yordinate = 0
	ldi	SysValueCopy,0
	sts	GLCD_YORDINATE,SysValueCopy
	sts	GLCD_YORDINATE_H,SysValueCopy
;Set GLCD_CS1 On
	sbi	PORTB,3
;Set GLCD_CS2 Off
	cbi	PORTB,4
;for GLCD_Count = 1 to 2
	ldi	SysValueCopy,0
	sts	GLCD_COUNT,SysValueCopy
SysForLoop6:
	lds	SysTemp3,GLCD_COUNT
	inc	SysTemp3
	sts	GLCD_COUNT,SysTemp3
;For CurrPage = 0 to 7
	ldi	SysValueCopy,255
	sts	CURRPAGE,SysValueCopy
SysForLoop7:
	lds	SysTemp3,CURRPAGE
	inc	SysTemp3
	sts	CURRPAGE,SysTemp3
;Set GLCD_RS Off
	cbi	PORTB,0
;GLCDWriteByte b'10111000' Or CurrPage
	ldi	SysTemp2,184
	lds	SysTemp3,CURRPAGE
	or	SysTemp3,SysTemp2
	sts	LCDBYTE,SysTemp3
	rcall	GLCDWRITEBYTE_KS0108
;For CurrCol = 0 to 63
	ldi	SysValueCopy,255
	sts	CURRCOL,SysValueCopy
SysForLoop8:
	lds	SysTemp3,CURRCOL
	inc	SysTemp3
	sts	CURRCOL,SysTemp3
;Set GLCD_RS Off
	cbi	PORTB,0
;GLCDWriteByte 64 Or CurrCol
	ldi	SysTemp2,64
	lds	SysTemp3,CURRCOL
	or	SysTemp3,SysTemp2
	sts	LCDBYTE,SysTemp3
	rcall	GLCDWRITEBYTE_KS0108
;Set GLCD_RS On
	sbi	PORTB,0
;GLCDWriteByte 0
	ldi	SysValueCopy,0
	sts	LCDBYTE,SysValueCopy
	rcall	GLCDWRITEBYTE_KS0108
;Next
	lds	SysCalcTempA,CURRCOL
	cpi	SysCalcTempA,63
	brlo	SysForLoop8
SysForLoopEnd8:
;Next
	lds	SysCalcTempA,CURRPAGE
	cpi	SysCalcTempA,7
	brlo	SysForLoop7
SysForLoopEnd7:
;Set GLCD_CS1 Off
	cbi	PORTB,3
;Set GLCD_CS2 On
	sbi	PORTB,4
;next
	lds	SysCalcTempA,GLCD_COUNT
	cpi	SysCalcTempA,2
	brlo	SysForLoop6
SysForLoopEnd6:
;Set GLCD_CS1 OFF
	cbi	PORTB,3
;Set GLCD_CS2 Off
	cbi	PORTB,4
	ret

;********************************************************************************

GLCDDRAWCHAR:
;CharCode -= 15
	lds	SysTemp1,CHARCODE
	ldi	SysTemp2,15
	sub	SysTemp1,SysTemp2
	sts	CHARCODE,SysTemp1
;if LineColour <> GLCDForeground  then
	lds	SysWORDTempA,LINECOLOUR
	lds	SysWORDTempA_H,LINECOLOUR_H
	lds	SysWORDTempB,GLCDFOREGROUND
	lds	SysWORDTempB_H,GLCDFOREGROUND_H
	rcall	SysCompEqual16
	com	SysByteTempX
	sbrs	SysByteTempX,0
	rjmp	ENDIF4
;GLCDBackground = 1
	ldi	SysValueCopy,1
	sts	GLCDBACKGROUND,SysValueCopy
	ldi	SysValueCopy,0
	sts	GLCDBACKGROUND_H,SysValueCopy
;GLCDForeground = 0
	ldi	SysValueCopy,0
	sts	GLCDFOREGROUND,SysValueCopy
	sts	GLCDFOREGROUND_H,SysValueCopy
;end if
ENDIF4:
;For CurrCharCol = 1 to 5
	ldi	SysValueCopy,0
	sts	CURRCHARCOL,SysValueCopy
SysForLoop2:
	lds	SysTemp1,CURRCHARCOL
	inc	SysTemp1
	sts	CURRCHARCOL,SysTemp1
;Select Case CurrCharCol
;Case 1: ReadTable GLCDCharCol3, CharCode, CurrCharVal
SysSelect1Case1:
	lds	SysCalcTempA,CURRCHARCOL
	cpi	SysCalcTempA,1
	brne	SysSelect1Case2
	lds	SysStringA,CHARCODE
	rcall	GLCDCHARCOL3
	sts	CURRCHARVAL,SysByteTempX
;Case 2: ReadTable GLCDCharCol4, CharCode, CurrCharVal
	rjmp	SysSelectEnd1
SysSelect1Case2:
	lds	SysCalcTempA,CURRCHARCOL
	cpi	SysCalcTempA,2
	brne	SysSelect1Case3
	lds	SysStringA,CHARCODE
	rcall	GLCDCHARCOL4
	sts	CURRCHARVAL,SysByteTempX
;Case 3: ReadTable GLCDCharCol5, CharCode, CurrCharVal
	rjmp	SysSelectEnd1
SysSelect1Case3:
	lds	SysCalcTempA,CURRCHARCOL
	cpi	SysCalcTempA,3
	brne	SysSelect1Case4
	lds	SysStringA,CHARCODE
	rcall	GLCDCHARCOL5
	sts	CURRCHARVAL,SysByteTempX
;Case 4: ReadTable GLCDCharCol6, CharCode, CurrCharVal
	rjmp	SysSelectEnd1
SysSelect1Case4:
	lds	SysCalcTempA,CURRCHARCOL
	cpi	SysCalcTempA,4
	brne	SysSelect1Case5
	lds	SysStringA,CHARCODE
	rcall	GLCDCHARCOL6
	sts	CURRCHARVAL,SysByteTempX
;Case 5: ReadTable GLCDCharCol7, CharCode, CurrCharVal
	rjmp	SysSelectEnd1
SysSelect1Case5:
	lds	SysCalcTempA,CURRCHARCOL
	cpi	SysCalcTempA,5
	brne	SysSelect1Case6
	lds	SysStringA,CHARCODE
	rcall	GLCDCHARCOL7
	sts	CURRCHARVAL,SysByteTempX
;End Select
SysSelect1Case6:
SysSelectEnd1:
;For CurrCharRow = 1 to 8
	ldi	SysValueCopy,0
	sts	CURRCHARROW,SysValueCopy
SysForLoop3:
	lds	SysTemp1,CURRCHARROW
	inc	SysTemp1
	sts	CURRCHARROW,SysTemp1
;If CurrCharVal.0 = 0 Then
	lds	SysBitTest,CURRCHARVAL
	sbrc	SysBitTest,0
	rjmp	ELSE5_1
;PSet CharLocX + CurrCharCol, CharLocY + CurrCharRow, GLCDBackground
	lds	SysTemp1,CHARLOCX
	lds	SysTemp2,CURRCHARCOL
	add	SysTemp1,SysTemp2
	sts	GLCDX,SysTemp1
	lds	SysTemp1,CHARLOCY
	lds	SysTemp2,CURRCHARROW
	add	SysTemp1,SysTemp2
	sts	GLCDY,SysTemp1
	lds	SysValueCopy,GLCDBACKGROUND
	sts	GLCDCOLOUR,SysValueCopy
	lds	SysValueCopy,GLCDBACKGROUND_H
	sts	GLCDCOLOUR_H,SysValueCopy
	rcall	PSET_KS0108
;Else
	rjmp	ENDIF5
ELSE5_1:
;PSet CharLocX + CurrCharCol, CharLocY + CurrCharRow, GLCDForeground
	lds	SysTemp1,CHARLOCX
	lds	SysTemp2,CURRCHARCOL
	add	SysTemp1,SysTemp2
	sts	GLCDX,SysTemp1
	lds	SysTemp1,CHARLOCY
	lds	SysTemp2,CURRCHARROW
	add	SysTemp1,SysTemp2
	sts	GLCDY,SysTemp1
	lds	SysValueCopy,GLCDFOREGROUND
	sts	GLCDCOLOUR,SysValueCopy
	lds	SysValueCopy,GLCDFOREGROUND_H
	sts	GLCDCOLOUR_H,SysValueCopy
	rcall	PSET_KS0108
;End If
ENDIF5:
;Rotate CurrCharVal Right
	lds	SysBYTETempA,CURRCHARVAL
	ror	SysBYTETempA
	sts	CURRCHARVAL,SysBYTETempA
;Next
	lds	SysCalcTempA,CURRCHARROW
	cpi	SysCalcTempA,8
	brsh	ENDIF6
	rjmp	SysForLoop3
ENDIF6:
SysForLoopEnd3:
;Next
	lds	SysCalcTempA,CURRCHARCOL
	cpi	SysCalcTempA,5
	brsh	ENDIF7
	rjmp	SysForLoop2
ENDIF7:
SysForLoopEnd2:
;GLCDBackground = 0
	ldi	SysValueCopy,0
	sts	GLCDBACKGROUND,SysValueCopy
	sts	GLCDBACKGROUND_H,SysValueCopy
;GLCDForeground = 1
	ldi	SysValueCopy,1
	sts	GLCDFOREGROUND,SysValueCopy
	ldi	SysValueCopy,0
	sts	GLCDFOREGROUND_H,SysValueCopy
	ret

;********************************************************************************

;Overloaded signature: BYTE:BYTE:STRING:
GLCDPRINT3:
;PrintLen = LCDPrintData(0)
	lds	SysStringA,SysLCDPRINTDATAHandler
	lds	SysStringA_H,SysLCDPRINTDATAHandler_H
	ld	SysValueCopy,X
	sts	PRINTLEN,SysValueCopy
;If PrintLen = 0 Then Exit Sub
	lds	SysCalcTempA,PRINTLEN
	tst	SysCalcTempA
	brne	ENDIF1
	ret
ENDIF1:
;GLCDPrintLoc = PrintLocX
	lds	SysValueCopy,PRINTLOCX
	sts	GLCDPRINTLOC,SysValueCopy
;For SysPrintTemp = 1 To PrintLen
	ldi	SysValueCopy,0
	sts	SYSPRINTTEMP,SysValueCopy
	lds	SysCalcTempA,PRINTLEN
	cpi	SysCalcTempA,1
	brlo	SysForLoopEnd1
SysForLoop1:
	lds	SysTemp1,SYSPRINTTEMP
	inc	SysTemp1
	sts	SYSPRINTTEMP,SysTemp1
;GLCDDrawChar GLCDPrintLoc, PrintLocY, LCDPrintData(SysPrintTemp)
	lds	SysValueCopy,GLCDPRINTLOC
	sts	CHARLOCX,SysValueCopy
	lds	SysValueCopy,PRINTLOCY
	sts	CHARLOCY,SysValueCopy
	lds	SysTemp1,SysLCDPRINTDATAHandler
	lds	SysTemp2,SYSPRINTTEMP
	add	SysTemp1,SysTemp2
	mov	SysStringA,SysTemp1
	lds	SysTemp1,SysLCDPRINTDATAHandler_H
	ldi	SysTemp2,0
	adc	SysTemp1,SysTemp2
	mov	SysStringA_H,SysTemp1
	ld	SysValueCopy,X
	sts	CHARCODE,SysValueCopy
	lds	SysValueCopy,GLCDFOREGROUND
	sts	LINECOLOUR,SysValueCopy
	lds	SysValueCopy,GLCDFOREGROUND_H
	sts	LINECOLOUR_H,SysValueCopy
	rcall	GLCDDRAWCHAR
;GLCDPrintLoc += GLCDFontWidth
	lds	SysTemp1,GLCDPRINTLOC
	lds	SysTemp2,GLCDFONTWIDTH
	add	SysTemp1,SysTemp2
	sts	GLCDPRINTLOC,SysTemp1
;Next
	lds	SysCalcTempA,SYSPRINTTEMP
	lds	SysCalcTempB,PRINTLEN
	cp	SysCalcTempA,SysCalcTempB
	brlo	SysForLoop1
SysForLoopEnd1:
	ret

;********************************************************************************

FN_GLCDREADBYTE_KS0108:
;Dir GLCD_DB7 In
	cbi	DDRD,7
;Dir GLCD_DB6 In
	cbi	DDRD,6
;Dir GLCD_DB5 In
	cbi	DDRD,5
;Dir GLCD_DB4 In
	cbi	DDRD,4
;Dir GLCD_DB3 In
	cbi	DDRD,3
;Dir GLCD_DB2 In
	cbi	DDRD,2
;Dir GLCD_DB1 In
	cbi	DDRD,1
;Dir GLCD_DB0 In
	cbi	DDRD,0
;Set GLCD_RW On
	sbi	PORTB,1
;Set GLCD_ENABLE On
	sbi	PORTB,2
;Wait 3 us
	ldi	DELAYTEMP,16
DelayUS18:
	dec	DELAYTEMP
	brne	DelayUS18
;GLCDReadByte.7 = GLCD_DB7
	lds	SysValueCopy,GLCDREADBYTE_KS0108
	cbr	SysValueCopy,1<<7
	sbic	PIND,7
	sbr	SysValueCopy,1<<7
	sts	GLCDREADBYTE_KS0108,SysValueCopy
;GLCDReadByte.6 = GLCD_DB6
	cbr	SysValueCopy,1<<6
	sbic	PIND,6
	sbr	SysValueCopy,1<<6
	sts	GLCDREADBYTE_KS0108,SysValueCopy
;GLCDReadByte.5 = GLCD_DB5
	cbr	SysValueCopy,1<<5
	sbic	PIND,5
	sbr	SysValueCopy,1<<5
	sts	GLCDREADBYTE_KS0108,SysValueCopy
;GLCDReadByte.4 = GLCD_DB4
	cbr	SysValueCopy,1<<4
	sbic	PIND,4
	sbr	SysValueCopy,1<<4
	sts	GLCDREADBYTE_KS0108,SysValueCopy
;GLCDReadByte.3 = GLCD_DB3
	cbr	SysValueCopy,1<<3
	sbic	PIND,3
	sbr	SysValueCopy,1<<3
	sts	GLCDREADBYTE_KS0108,SysValueCopy
;GLCDReadByte.2 = GLCD_DB2
	cbr	SysValueCopy,1<<2
	sbic	PIND,2
	sbr	SysValueCopy,1<<2
	sts	GLCDREADBYTE_KS0108,SysValueCopy
;GLCDReadByte.1 = GLCD_DB1
	cbr	SysValueCopy,1<<1
	sbic	PIND,1
	sbr	SysValueCopy,1<<1
	sts	GLCDREADBYTE_KS0108,SysValueCopy
;GLCDReadByte.0 = GLCD_DB0
	cbr	SysValueCopy,1<<0
	sbic	PIND,0
	sbr	SysValueCopy,1<<0
	sts	GLCDREADBYTE_KS0108,SysValueCopy
;Set GLCD_ENABLE Off
	cbi	PORTB,2
;Wait 2 us
	ldi	DELAYTEMP,10
DelayUS19:
	dec	DELAYTEMP
	brne	DelayUS19
	rjmp	PC + 1
	ret

;********************************************************************************

GLCDWRITEBYTE_KS0108:
;Dim GLCDSaveRS As Bit
;Dim GLCDSaveCS2 As Bit
;GLCDSaveRS = GLCD_RS
	lds	SysValueCopy,SYSBITVAR0
	cbr	SysValueCopy,1<<0
	sbic	PINB,0
	sbr	SysValueCopy,1<<0
	sts	SYSBITVAR0,SysValueCopy
;GLCDSaveCS2 = GLCD_CS2
	cbr	SysValueCopy,1<<1
	sbic	PINB,4
	sbr	SysValueCopy,1<<1
	sts	SYSBITVAR0,SysValueCopy
;If GLCD_CS1 = 1 Then
	sbic	PINB,3
;GLCD_CS2 = 0
	cbi	PORTB,4
;End If
;Set GLCD_RS Off
	cbi	PORTB,0
;Wait Until GLCDReadByte.7 = Off
SysWaitLoop1:
	rcall	FN_GLCDREADBYTE_KS0108
	lds	SysBitTest,GLCDREADBYTE_KS0108
	sbrc	SysBitTest,7
	rjmp	SysWaitLoop1
;GLCD_RS = GLCDSaveRS
	cbi	PORTB,0
	lds	SysBitTest,SYSBITVAR0
	sbrc	SysBitTest,0
	sbi	PORTB,0
;GLCD_CS2 = GLCDSaveCS2
	cbi	PORTB,4
	lds	SysBitTest,SYSBITVAR0
	sbrc	SysBitTest,1
	sbi	PORTB,4
;Set GLCD_RW Off
	cbi	PORTB,1
;DIR GLCD_DB0 OUT
	sbi	DDRD,0
;DIR GLCD_DB1 OUT
	sbi	DDRD,1
;DIR GLCD_DB2 OUT
	sbi	DDRD,2
;DIR GLCD_DB3 OUT
	sbi	DDRD,3
;DIR GLCD_DB4 OUT
	sbi	DDRD,4
;DIR GLCD_DB5 OUT
	sbi	DDRD,5
;DIR GLCD_DB6 OUT
	sbi	DDRD,6
;DIR GLCD_DB7 OUT
	sbi	DDRD,7
;GLCD_DB7 = LCDByte.7
	cbi	PORTD,7
	lds	SysBitTest,LCDBYTE
	sbrc	SysBitTest,7
	sbi	PORTD,7
;GLCD_DB6 = LCDByte.6
	cbi	PORTD,6
	lds	SysBitTest,LCDBYTE
	sbrc	SysBitTest,6
	sbi	PORTD,6
;GLCD_DB5 = LCDByte.5
	cbi	PORTD,5
	lds	SysBitTest,LCDBYTE
	sbrc	SysBitTest,5
	sbi	PORTD,5
;GLCD_DB4 = LCDByte.4
	cbi	PORTD,4
	lds	SysBitTest,LCDBYTE
	sbrc	SysBitTest,4
	sbi	PORTD,4
;GLCD_DB3 = LCDByte.3
	cbi	PORTD,3
	lds	SysBitTest,LCDBYTE
	sbrc	SysBitTest,3
	sbi	PORTD,3
;GLCD_DB2 = LCDByte.2
	cbi	PORTD,2
	lds	SysBitTest,LCDBYTE
	sbrc	SysBitTest,2
	sbi	PORTD,2
;GLCD_DB1 = LCDByte.1
	cbi	PORTD,1
	lds	SysBitTest,LCDBYTE
	sbrc	SysBitTest,1
	sbi	PORTD,1
;GLCD_DB0 = LCDByte.0
	cbi	PORTD,0
	lds	SysBitTest,LCDBYTE
	sbrc	SysBitTest,0
	sbi	PORTD,0
;Wait KS0108WriteDelay us
	ldi	DELAYTEMP,5
DelayUS16:
	dec	DELAYTEMP
	brne	DelayUS16
	nop
;Set GLCD_ENABLE On
	sbi	PORTB,2
;Wait KS0108_Clock_Delay us
	lds	SysWaitTempUS,KS0108_CLOCK_DELAY
	ldi	SysWaitTempUS_H,0
	rcall	Delay_US
;Set GLCD_ENABLE Off
	cbi	PORTB,2
;Wait KS0108WriteDelay us
	ldi	DELAYTEMP,5
DelayUS17:
	dec	DELAYTEMP
	brne	DelayUS17
	nop
	ret

;********************************************************************************

FN_HEX:
;Hex(0) = 2
	ldi	SysValueCopy,2
	sts	SYSHEX_0,SysValueCopy
;SysStringTemp = SysValTemp And 0x0F
	ldi	SysTemp2,15
	lds	SysTemp3,SYSVALTEMP
	and	SysTemp3,SysTemp2
	sts	SYSSTRINGTEMP,SysTemp3
;If SysStringTemp > 9 Then SysStringTemp = SysStringTemp + 7
	ldi	SysCalcTempA,9
	lds	SysCalcTempB,SYSSTRINGTEMP
	cp	SysCalcTempA,SysCalcTempB
	brsh	ENDIF37
	lds	SysTemp3,SYSSTRINGTEMP
	ldi	SysTemp2,7
	add	SysTemp3,SysTemp2
	sts	SYSSTRINGTEMP,SysTemp3
ENDIF37:
;Hex(2) = SysStringTemp + 48
	lds	SysTemp3,SYSSTRINGTEMP
	ldi	SysTemp2,48
	add	SysTemp3,SysTemp2
	sts	SYSHEX_2,SysTemp3
;For SysStringTemp = 1 to 4
	ldi	SysValueCopy,0
	sts	SYSSTRINGTEMP,SysValueCopy
SysForLoop9:
	lds	SysTemp3,SYSSTRINGTEMP
	inc	SysTemp3
	sts	SYSSTRINGTEMP,SysTemp3
;Rotate SysValTemp Right
	lds	SysBYTETempA,SYSVALTEMP
	ror	SysBYTETempA
	sts	SYSVALTEMP,SysBYTETempA
;Next
	lds	SysCalcTempA,SYSSTRINGTEMP
	cpi	SysCalcTempA,4
	brlo	SysForLoop9
SysForLoopEnd9:
;SysStringTemp = SysValTemp And 0x0F
	ldi	SysTemp2,15
	lds	SysTemp3,SYSVALTEMP
	and	SysTemp3,SysTemp2
	sts	SYSSTRINGTEMP,SysTemp3
;If SysStringTemp > 9 Then SysStringTemp = SysStringTemp + 7
	ldi	SysCalcTempA,9
	lds	SysCalcTempB,SYSSTRINGTEMP
	cp	SysCalcTempA,SysCalcTempB
	brsh	ENDIF39
	lds	SysTemp3,SYSSTRINGTEMP
	ldi	SysTemp2,7
	add	SysTemp3,SysTemp2
	sts	SYSSTRINGTEMP,SysTemp3
ENDIF39:
;Hex(1) = SysStringTemp + 48
	lds	SysTemp3,SYSSTRINGTEMP
	ldi	SysTemp2,48
	add	SysTemp3,SysTemp2
	sts	SYSHEX_1,SysTemp3
	ret

;********************************************************************************

INITGLCD_KS0108:
;Dir GLCD_RS Out
	sbi	DDRB,0
;Dir GLCD_RW Out
	sbi	DDRB,1
;Dir GLCD_ENABLE Out
	sbi	DDRB,2
;Dir GLCD_CS1 Out
	sbi	DDRB,3
;Dir GLCD_CS2 Out
	sbi	DDRB,4
;Dir GLCD_RESET Out
	sbi	DDRB,5
;Set GLCD_RESET Off
	cbi	PORTB,5
;Wait 1 ms
	ldi	SysWaitTempMS,1
	ldi	SysWaitTempMS_H,0
	rcall	Delay_MS
;Set GLCD_RESET On
	sbi	PORTB,5
;Wait 1 ms
	ldi	SysWaitTempMS,1
	ldi	SysWaitTempMS_H,0
	rcall	Delay_MS
;Set GLCD_CS1 On
	sbi	PORTB,3
;Set GLCD_CS2 On
	sbi	PORTB,4
;Set GLCD_RS Off
	cbi	PORTB,0
;GLCDWriteByte 63
	ldi	SysValueCopy,63
	sts	LCDBYTE,SysValueCopy
	rcall	GLCDWRITEBYTE_KS0108
;GLCDWriteByte 192
	ldi	SysValueCopy,192
	sts	LCDBYTE,SysValueCopy
	rcall	GLCDWRITEBYTE_KS0108
;Set GLCD_CS1 Off
	cbi	PORTB,3
;Set GLCD_CS2 Off
	cbi	PORTB,4
;GLCDBackground = 0
	ldi	SysValueCopy,0
	sts	GLCDBACKGROUND,SysValueCopy
	sts	GLCDBACKGROUND_H,SysValueCopy
;GLCDForeground = 1
	ldi	SysValueCopy,1
	sts	GLCDFOREGROUND,SysValueCopy
	ldi	SysValueCopy,0
	sts	GLCDFOREGROUND_H,SysValueCopy
;GLCDFontWidth = 6
	ldi	SysValueCopy,6
	sts	GLCDFONTWIDTH,SysValueCopy
;GLCDfntDefault = 0
	ldi	SysValueCopy,0
	sts	GLCDFNTDEFAULT,SysValueCopy
;GLCDCLS_KS0108
	rjmp	GLCDCLS_KS0108

;********************************************************************************

INITSYS:
;PORTB = 0
	ldi	SysValueCopy,0
	out	PORTB,SysValueCopy
;PORTC = 0
	ldi	SysValueCopy,0
	out	PORTC,SysValueCopy
;PORTD = 0
	ldi	SysValueCopy,0
	out	PORTD,SysValueCopy
	ret

;********************************************************************************

MASTERRST:
;Dir DQ In
	cbi	DDRC,3
;Dir DQ Out
	sbi	DDRC,3
;Set DQ Off
	cbi	PORTC,3
;wait 60 10us   ' Set to 60 10us for longer wires
	ldi	SysWaitTemp10US,60
	rcall	Delay_10US
;Dir DQ In      ' HiZ
	cbi	DDRC,3
	ret

;********************************************************************************

OWIN:
;For ClocksB = 1 to 8
	ldi	SysValueCopy,0
	sts	CLOCKSB,SysValueCopy
SysForLoop5:
	lds	SysTemp3,CLOCKSB
	inc	SysTemp3
	sts	CLOCKSB,SysTemp3
;Rotate RxData Right
	lds	SysWORDTempA,RXDATA
	lds	SysWORDTempA_H,RXDATA_H
	ror	SysWORDTempA_H
	ror	SysWORDTempA
	sts	RXDATA,SysWORDTempA
	sts	RXDATA_H,SysWORDTempA_H
;Dir DQ Out
	sbi	DDRC,3
;Set DQ Off                   ' Read time slot
	cbi	PORTC,3
;wait 6 us                    ' Minimum is 1uS
	ldi	DELAYTEMP,32
DelayUS12:
	dec	DELAYTEMP
	brne	DelayUS12
;Dir DQ In                    ' Release bus for one wire Reception
	cbi	DDRC,3
;wait 4 us
	ldi	DELAYTEMP,21
DelayUS13:
	dec	DELAYTEMP
	brne	DelayUS13
	nop
;If DQ On  Then RxData.7 = 1
	sbis	PINC,3
	rjmp	ENDIF14
	lds	SysValueCopy,RXDATA
	sbr	SysValueCopy,1<<7
	sts	RXDATA,SysValueCopy
ENDIF14:
;If DQ Off Then RxData.7 = 0
	sbic	PINC,3
	rjmp	ENDIF15
	lds	SysValueCopy,RXDATA
	cbr	SysValueCopy,1<<7
	sts	RXDATA,SysValueCopy
ENDIF15:
;wait 60 us                   ' Set to 60 uS  for use with longer wires
	ldi	DELAYTEMP2,1
DelayUSO14:
	ldi	DELAYTEMP,0
DelayUS14:
	dec	DELAYTEMP
	brne	DelayUS14
	dec	DELAYTEMP2
	brne	DelayUSO14
	ldi	DELAYTEMP,63
DelayUS15:
	dec	DELAYTEMP
	brne	DelayUS15
;Next ClocksB
	lds	SysCalcTempA,CLOCKSB
	cpi	SysCalcTempA,8
	brlo	SysForLoop5
SysForLoopEnd5:
	ret

;********************************************************************************

OWOUT:
;For ClocksA = 1 to 8
	ldi	SysValueCopy,0
	sts	CLOCKSA,SysValueCopy
SysForLoop4:
	lds	SysTemp3,CLOCKSA
	inc	SysTemp3
	sts	CLOCKSA,SysTemp3
;Dir DQ Out
	sbi	DDRC,3
;Set DQ Off
	cbi	PORTC,3
;wait 3 us    ' Need to release bus within 15 us
	ldi	DELAYTEMP,16
DelayUS8:
	dec	DELAYTEMP
	brne	DelayUS8
;If Command.0 On then
	lds	SysBitTest,COMMAND
	sbrc	SysBitTest,0
;Dir DQ In
	cbi	DDRC,3
;End if
;wait 100 us  ' Set to 60 us per AN2420 datasheet indicates MAX delay is 120 us . changed to 100uS for longer wires. Only relevant for writing zeros
	ldi	DELAYTEMP2,2
DelayUSO9:
	ldi	DELAYTEMP,0
DelayUS9:
	dec	DELAYTEMP
	brne	DelayUS9
	dec	DELAYTEMP2
	brne	DelayUSO9
	ldi	DELAYTEMP,19
DelayUS10:
	dec	DELAYTEMP
	brne	DelayUS10
	nop
;Dir DQ In    ' HiZ the bus if still pulled low
	cbi	DDRC,3
;wait 3 us
	ldi	DELAYTEMP,16
DelayUS11:
	dec	DELAYTEMP
	brne	DelayUS11
;ROTATE Command Right  'The DS18B20 wants data LSB first
	lds	SysBYTETempA,COMMAND
	ror	SysBYTETempA
	sts	COMMAND,SysBYTETempA
;Next ClocksA
	lds	SysCalcTempA,CLOCKSA
	cpi	SysCalcTempA,8
	brlo	SysForLoop4
SysForLoopEnd4:
	ret

;********************************************************************************

PPULSE:
;wait 100 us             ' Set to 100us for long bus wires				  'test code
	ldi	DELAYTEMP2,2
DelayUSO6:
	ldi	DELAYTEMP,0
DelayUS6:
	dec	DELAYTEMP
	brne	DelayUS6
	dec	DELAYTEMP2
	brne	DelayUSO6
	ldi	DELAYTEMP,19
DelayUS7:
	dec	DELAYTEMP
	brne	DelayUS7
	nop
;wait 43 10us
	ldi	SysWaitTemp10US,43
	rcall	Delay_10US
;Dir DQ In
	cbi	DDRC,3
	ret

;********************************************************************************

PSET_KS0108:
;If GLCDX.6 = Off Then Set GLCD_CS2 On:Set GLCD_CS1 off
	lds	SysBitTest,GLCDX
	sbrc	SysBitTest,6
	rjmp	ENDIF20
	sbi	PORTB,4
	cbi	PORTB,3
ENDIF20:
;If GLCDX.6 = On Then Set GLCD_CS1 On: GLCDX -= 64: Set GLCD_CS2 off
	lds	SysBitTest,GLCDX
	sbrs	SysBitTest,6
	rjmp	ENDIF21
	sbi	PORTB,3
	lds	SysTemp3,GLCDX
	ldi	SysTemp2,64
	sub	SysTemp3,SysTemp2
	sts	GLCDX,SysTemp3
	cbi	PORTB,4
ENDIF21:
;CurrPage = GLCDY / 8
	lds	SysBYTETempA,GLCDY
	ldi	SysBYTETempB,8
	rcall	SysDivSub
	sts	CURRPAGE,SysBYTETempA
;Set GLCD_RS Off
	cbi	PORTB,0
;GLCDWriteByte b'10111000' Or CurrPage
	ldi	SysTemp2,184
	lds	SysTemp3,CURRPAGE
	or	SysTemp3,SysTemp2
	sts	LCDBYTE,SysTemp3
	rcall	GLCDWRITEBYTE_KS0108
;Set GLCD_RS Off
	cbi	PORTB,0
;GLCDWriteByte 64 Or GLCDX
	ldi	SysTemp2,64
	lds	SysTemp3,GLCDX
	or	SysTemp3,SysTemp2
	sts	LCDBYTE,SysTemp3
	rcall	GLCDWRITEBYTE_KS0108
;Set GLCD_RS On
	sbi	PORTB,0
;GLCDDataTemp = GLCDReadByte
	rcall	FN_GLCDREADBYTE_KS0108
	lds	SysValueCopy,GLCDREADBYTE_KS0108
	sts	GLCDDATATEMP,SysValueCopy
;Set GLCD_RS On
	sbi	PORTB,0
;GLCDDataTemp = GLCDReadByte
	rcall	FN_GLCDREADBYTE_KS0108
	lds	SysValueCopy,GLCDREADBYTE_KS0108
	sts	GLCDDATATEMP,SysValueCopy
;GLCDBitNo = GLCDY And 7
	ldi	SysTemp2,7
	lds	SysTemp3,GLCDY
	and	SysTemp3,SysTemp2
	sts	GLCDBITNO,SysTemp3
;If GLCDColour.0 = 0 Then
	lds	SysBitTest,GLCDCOLOUR
	sbrc	SysBitTest,0
	rjmp	ELSE22_1
;GLCDChange = 254
	ldi	SysValueCopy,254
	sts	GLCDCHANGE,SysValueCopy
;Set C On
	sec
;Else
	rjmp	ENDIF22
ELSE22_1:
;GLCDChange = 1
	ldi	SysValueCopy,1
	sts	GLCDCHANGE,SysValueCopy
;Set C Off
	clc
;End If
ENDIF22:
;Repeat GLCDBitNo
	lds	SysValueCopy,GLCDBITNO
	sts	SysRepeatTemp1,SysValueCopy
	lds	SysCalcTempA,SYSREPEATTEMP1
	tst	SysCalcTempA
	brne	PC + 2
	rjmp	SysRepeatLoopEnd1
SysRepeatLoop1:
;Rotate GLCDChange Left
	lds	SysBYTETempA,GLCDCHANGE
	rol	SysBYTETempA
	sts	GLCDCHANGE,SysBYTETempA
;End Repeat
	lds	SysTemp3,SysRepeatTemp1
	dec	SysTemp3
	sts	SysRepeatTemp1,SysTemp3
	brne	SysRepeatLoop1
SysRepeatLoopEnd1:
;If GLCDColour.0 = 0 Then
	lds	SysBitTest,GLCDCOLOUR
	sbrc	SysBitTest,0
	rjmp	ELSE23_1
;GLCDDataTemp = GLCDDataTemp And GLCDChange
	lds	SysTemp3,GLCDDATATEMP
	lds	SysTemp1,GLCDCHANGE
	and	SysTemp1,SysTemp3
	sts	GLCDDATATEMP,SysTemp1
;Else
	rjmp	ENDIF23
ELSE23_1:
;GLCDDataTemp = GLCDDataTemp Or GLCDChange
	lds	SysTemp3,GLCDDATATEMP
	lds	SysTemp1,GLCDCHANGE
	or	SysTemp1,SysTemp3
	sts	GLCDDATATEMP,SysTemp1
;End If
ENDIF23:
;Set GLCD_RS Off
	cbi	PORTB,0
;GLCDWriteByte 64 Or GLCDX
	ldi	SysTemp2,64
	lds	SysTemp3,GLCDX
	or	SysTemp3,SysTemp2
	sts	LCDBYTE,SysTemp3
	rcall	GLCDWRITEBYTE_KS0108
;Set GLCD_RS On
	sbi	PORTB,0
;GLCDWriteByte GLCDDataTemp
	lds	SysValueCopy,GLCDDATATEMP
	sts	LCDBYTE,SysValueCopy
	rcall	GLCDWRITEBYTE_KS0108
;Set GLCD_CS1 Off
	cbi	PORTB,3
;Set GLCD_CS2 Off
	cbi	PORTB,4
	ret

;********************************************************************************

FN_READTEMP:
;DS18B20DeviceRead
	rcall	DS18B20DEVICEREAD
;If DSdata.15 = 1 then
	lds	SysBitTest,DSDATA_H
	sbrs	SysBitTest,7
	rjmp	ELSE8_1
;DSdata = ( DSdata # 0xffff ) + 1 ' take twos comp
	ldi	SysTemp2,255
	lds	SysTemp3,DSDATA
	eor	SysTemp3,SysTemp2
	mov	SysTemp1,SysTemp3
	ldi	SysTemp2,255
	lds	SysTemp3,DSDATA_H
	eor	SysTemp3,SysTemp2
	sts	SysTemp1_H,SysTemp3
	ldi	SysTemp2,1
	add	SysTemp1,SysTemp2
	sts	DSDATA,SysTemp1
	lds	SysTemp3,SysTemp1_H
	ldi	SysTemp2,0
	adc	SysTemp3,SysTemp2
	sts	DSDATA_H,SysTemp3
;SignBit = true
	ldi	SysValueCopy,255
	sts	SIGNBIT,SysValueCopy
;else
	rjmp	ENDIF8
ELSE8_1:
;SignBit = false
	ldi	SysValueCopy,0
	sts	SIGNBIT,SysValueCopy
;end if
ENDIF8:
;readtemp =  DSdata * 6		' convert raw data 12 bit value of 0.0625
	lds	SysINTEGERTempA,DSDATA
	lds	SysINTEGERTempA_H,DSDATA_H
	ldi	SysValueCopy,6
	mov	SysINTEGERTempB,SysValueCopy
	ldi	SysValueCopy,0
	mov	SysINTEGERTempB_H,SysValueCopy
	rcall	SysMultSubINT
	sts	READTEMP,SysINTEGERTempX
	sts	READTEMP_H,SysINTEGERTempX_H
;DSdata = ( DSdata * 25 ) / 100
	lds	SysWORDTempA,DSDATA
	lds	SysWORDTempA_H,DSDATA_H
	ldi	SysWORDTempB,25
	ldi	SysWORDTempB_H,0
	rcall	SysMultSub16
	mov	SysTemp1,SysWORDTempX
	mov	SysTemp1_H,SysWORDTempX_H
	mov	SysWORDTempA,SysTemp1
	mov	SysWORDTempA_H,SysTemp1_H
	ldi	SysWORDTempB,100
	ldi	SysWORDTempB_H,0
	rcall	SysDivSub16
	sts	DSDATA,SysWORDTempA
	sts	DSDATA_H,SysWORDTempA_H
;readtemp = readtemp + DSdata
	lds	SysTemp3,READTEMP
	lds	SysTemp1,DSDATA
	add	SysTemp3,SysTemp1
	sts	READTEMP,SysTemp3
	lds	SysTemp3,READTEMP_H
	lds	SysTemp1,DSDATA_H
	adc	SysTemp3,SysTemp1
	sts	READTEMP_H,SysTemp3
;Whole = readtemp / 100
	lds	SysINTEGERTempA,READTEMP
	lds	SysINTEGERTempA_H,READTEMP_H
	ldi	SysValueCopy,100
	mov	SysINTEGERTempB,SysValueCopy
	ldi	SysValueCopy,0
	mov	SysINTEGERTempB_H,SysValueCopy
	rcall	SysDivSubINT
	sts	WHOLE,SysINTEGERTempA
;fraction = TempC_100 % 100
	lds	SysWORDTempA,TEMPC_100
	lds	SysWORDTempA_H,TEMPC_100_H
	ldi	SysWORDTempB,100
	ldi	SysWORDTempB_H,0
	rcall	SysDivSub16
	sts	FRACTION,SysWORDTempX
;if fraction > 50 then
	ldi	SysCalcTempA,50
	lds	SysCalcTempB,FRACTION
	cp	SysCalcTempA,SysCalcTempB
	brsh	ENDIF9
;Whole++
	lds	SysTemp3,WHOLE
	inc	SysTemp3
	sts	WHOLE,SysTemp3
;end if
ENDIF9:
;if SignBit = true then
	lds	SysCalcTempA,SIGNBIT
	cpi	SysCalcTempA,255
	brne	ELSE10_1
;readtemp = Whole + 128
	lds	SysTemp3,WHOLE
	ldi	SysTemp2,128
	add	SysTemp3,SysTemp2
	sts	READTEMP,SysTemp3
	clr	SysTemp3
	brcc	PC + 2
	inc	SysTemp3
	sts	READTEMP_H,SysTemp3
;else
	rjmp	ENDIF10
ELSE10_1:
;readtemp =  Whole
	lds	SysValueCopy,WHOLE
	sts	READTEMP,SysValueCopy
	ldi	SysValueCopy,0
	sts	READTEMP_H,SysValueCopy
;end if
ENDIF10:
	ret

;********************************************************************************

FN_STR:
;SysCharCount = 0
	ldi	SysValueCopy,0
	sts	SYSCHARCOUNT,SysValueCopy
;Dim SysCalcTempX As Word
;IF SysValTemp >= 10000 then
	lds	SysWORDTempA,SYSVALTEMP
	lds	SysWORDTempA_H,SYSVALTEMP_H
	ldi	SysWORDTempB,16
	ldi	SysWORDTempB_H,39
	rcall	SysCompLessThan16
	com	SysByteTempX
	sbrs	SysByteTempX,0
	rjmp	ENDIF33
;SysStrData = SysValTemp / 10000
	lds	SysWORDTempA,SYSVALTEMP
	lds	SysWORDTempA_H,SYSVALTEMP_H
	ldi	SysWORDTempB,16
	ldi	SysWORDTempB_H,39
	rcall	SysDivSub16
	sts	SYSSTRDATA,SysWORDTempA
;SysValTemp = SysCalcTempX
	sts	SYSVALTEMP,SYSCALCTEMPX
	sts	SYSVALTEMP_H,SYSCALCTEMPX_H
;SysCharCount += 1
	lds	SysTemp3,SYSCHARCOUNT
	inc	SysTemp3
	sts	SYSCHARCOUNT,SysTemp3
;Str(SysCharCount) = SysStrData + 48
	ldi	SysTemp2,low(STR)
	add	SysTemp3,SysTemp2
	mov	SysStringA,SysTemp3
	ldi	SysTemp2,0
	ldi	SysTemp4,high(STR)
	adc	SysTemp2,SysTemp4
	mov	SysStringA_H,SysTemp2
	lds	SysTemp3,SYSSTRDATA
	ldi	SysTemp2,48
	add	SysTemp3,SysTemp2
	st	X,SysTemp3
;Goto SysValThousands
	rjmp	SYSVALTHOUSANDS
;End If
ENDIF33:
;IF SysValTemp >= 1000 then
	lds	SysWORDTempA,SYSVALTEMP
	lds	SysWORDTempA_H,SYSVALTEMP_H
	ldi	SysWORDTempB,232
	ldi	SysWORDTempB_H,3
	rcall	SysCompLessThan16
	com	SysByteTempX
	sbrs	SysByteTempX,0
	rjmp	ENDIF34
SYSVALTHOUSANDS:
;SysStrData = SysValTemp / 1000
	lds	SysWORDTempA,SYSVALTEMP
	lds	SysWORDTempA_H,SYSVALTEMP_H
	ldi	SysWORDTempB,232
	ldi	SysWORDTempB_H,3
	rcall	SysDivSub16
	sts	SYSSTRDATA,SysWORDTempA
;SysValTemp = SysCalcTempX
	sts	SYSVALTEMP,SYSCALCTEMPX
	sts	SYSVALTEMP_H,SYSCALCTEMPX_H
;SysCharCount += 1
	lds	SysTemp3,SYSCHARCOUNT
	inc	SysTemp3
	sts	SYSCHARCOUNT,SysTemp3
;Str(SysCharCount) = SysStrData + 48
	ldi	SysTemp2,low(STR)
	add	SysTemp3,SysTemp2
	mov	SysStringA,SysTemp3
	ldi	SysTemp2,0
	ldi	SysTemp4,high(STR)
	adc	SysTemp2,SysTemp4
	mov	SysStringA_H,SysTemp2
	lds	SysTemp3,SYSSTRDATA
	ldi	SysTemp2,48
	add	SysTemp3,SysTemp2
	st	X,SysTemp3
;Goto SysValHundreds
	rjmp	SYSVALHUNDREDS
;End If
ENDIF34:
;IF SysValTemp >= 100 then
	lds	SysWORDTempA,SYSVALTEMP
	lds	SysWORDTempA_H,SYSVALTEMP_H
	ldi	SysWORDTempB,100
	ldi	SysWORDTempB_H,0
	rcall	SysCompLessThan16
	com	SysByteTempX
	sbrs	SysByteTempX,0
	rjmp	ENDIF35
SYSVALHUNDREDS:
;SysStrData = SysValTemp / 100
	lds	SysWORDTempA,SYSVALTEMP
	lds	SysWORDTempA_H,SYSVALTEMP_H
	ldi	SysWORDTempB,100
	ldi	SysWORDTempB_H,0
	rcall	SysDivSub16
	sts	SYSSTRDATA,SysWORDTempA
;SysValTemp = SysCalcTempX
	sts	SYSVALTEMP,SYSCALCTEMPX
	sts	SYSVALTEMP_H,SYSCALCTEMPX_H
;SysCharCount += 1
	lds	SysTemp3,SYSCHARCOUNT
	inc	SysTemp3
	sts	SYSCHARCOUNT,SysTemp3
;Str(SysCharCount) = SysStrData + 48
	ldi	SysTemp2,low(STR)
	add	SysTemp3,SysTemp2
	mov	SysStringA,SysTemp3
	ldi	SysTemp2,0
	ldi	SysTemp4,high(STR)
	adc	SysTemp2,SysTemp4
	mov	SysStringA_H,SysTemp2
	lds	SysTemp3,SYSSTRDATA
	ldi	SysTemp2,48
	add	SysTemp3,SysTemp2
	st	X,SysTemp3
;Goto SysValTens
	rjmp	SYSVALTENS
;End If
ENDIF35:
;IF SysValTemp >= 10 Then
	lds	SysWORDTempA,SYSVALTEMP
	lds	SysWORDTempA_H,SYSVALTEMP_H
	ldi	SysWORDTempB,10
	ldi	SysWORDTempB_H,0
	rcall	SysCompLessThan16
	com	SysByteTempX
	sbrs	SysByteTempX,0
	rjmp	ENDIF36
SYSVALTENS:
;SysStrData = SysValTemp / 10
	lds	SysWORDTempA,SYSVALTEMP
	lds	SysWORDTempA_H,SYSVALTEMP_H
	ldi	SysWORDTempB,10
	ldi	SysWORDTempB_H,0
	rcall	SysDivSub16
	sts	SYSSTRDATA,SysWORDTempA
;SysValTemp = SysCalcTempX
	sts	SYSVALTEMP,SYSCALCTEMPX
	sts	SYSVALTEMP_H,SYSCALCTEMPX_H
;SysCharCount += 1
	lds	SysTemp3,SYSCHARCOUNT
	inc	SysTemp3
	sts	SYSCHARCOUNT,SysTemp3
;Str(SysCharCount) = SysStrData + 48
	ldi	SysTemp2,low(STR)
	add	SysTemp3,SysTemp2
	mov	SysStringA,SysTemp3
	ldi	SysTemp2,0
	ldi	SysTemp4,high(STR)
	adc	SysTemp2,SysTemp4
	mov	SysStringA_H,SysTemp2
	lds	SysTemp3,SYSSTRDATA
	ldi	SysTemp2,48
	add	SysTemp3,SysTemp2
	st	X,SysTemp3
;End If
ENDIF36:
;SysCharCount += 1
	lds	SysTemp3,SYSCHARCOUNT
	inc	SysTemp3
	sts	SYSCHARCOUNT,SysTemp3
;Str(SysCharCount) = SysValTemp + 48
	ldi	SysTemp2,low(STR)
	add	SysTemp3,SysTemp2
	mov	SysStringA,SysTemp3
	ldi	SysTemp2,0
	ldi	SysTemp4,high(STR)
	adc	SysTemp2,SysTemp4
	mov	SysStringA_H,SysTemp2
	lds	SysTemp3,SYSVALTEMP
	ldi	SysTemp2,48
	add	SysTemp3,SysTemp2
	st	X,SysTemp3
;SysValTemp = SysCalcTempX
	sts	SYSVALTEMP,SYSCALCTEMPX
	sts	SYSVALTEMP_H,SYSCALCTEMPX_H
;Str(0) = SysCharCount
	lds	SysValueCopy,SYSCHARCOUNT
	sts	SYSSTR_0,SysValueCopy
	ret

;********************************************************************************

SYSCOMPEQUAL16:
;dim SysWordTempA as word
;dim SysWordTempB as word
;dim SysByteTempX as byte
;clr SysByteTempX
	clr	SYSBYTETEMPX
;cp SysWordTempA, SysWordTempB
	cp	SYSWORDTEMPA, SYSWORDTEMPB
;brne SCE16False
	brne	SCE16FALSE
;cp SysWordTempA_H, SysWordTempB_H
	cp	SYSWORDTEMPA_H, SYSWORDTEMPB_H
;brne SCE16False
	brne	SCE16FALSE
;com SysByteTempX
	com	SYSBYTETEMPX
SCE16FALSE:
	ret

;********************************************************************************

SYSCOMPLESSTHAN16:
;clr SysByteTempX
	clr	SYSBYTETEMPX
;cp SysWordTempB_H,SysWordTempA_H
	cp	SYSWORDTEMPB_H,SYSWORDTEMPA_H
;brlo SCLT16False
	brlo	SCLT16FALSE
;cp SysWordTempA_H,SysWordTempB_H
	cp	SYSWORDTEMPA_H,SYSWORDTEMPB_H
;brlo SCLT16True
	brlo	SCLT16TRUE
;cp SysWordTempA,SysWordTempB
	cp	SYSWORDTEMPA,SYSWORDTEMPB
;brlo SCLT16True
	brlo	SCLT16TRUE
;ret
	ret
SCLT16TRUE:
;com SysByteTempX
	com	SYSBYTETEMPX
SCLT16FALSE:
	ret

;********************************************************************************

SYSCOPYSTRING:
;Dim SysCalcTempA As Byte
;Dim SysStringLength As Byte
;Dim SysReadA As Byte
;ld SysCalcTempA, X+
	ld	SYSCALCTEMPA, X+
;st Y+, SysCalcTempA
	st	Y+, SYSCALCTEMPA
;rjmp SysCopyStringCheck
	rjmp	SYSCOPYSTRINGCHECK
;When appending, add length to counter
SYSCOPYSTRINGPART:
;ld SysCalcTempA, X+
	ld	SYSCALCTEMPA, X+
;add SysStringLength, SysCalcTempA
	add	SYSSTRINGLENGTH, SYSCALCTEMPA
SYSCOPYSTRINGCHECK:
;cpi SysCalcTempA,0
	cpi	SYSCALCTEMPA,0
;brne SysStringCopy
	brne	SYSSTRINGCOPY
;ret
	ret
SYSSTRINGCOPY:
;ld SysReadA, X+
	ld	SYSREADA, X+
;st Y+, SysReadA
	st	Y+, SYSREADA
;dec SysCalcTempA
	dec	SYSCALCTEMPA
;brne SysStringCopy
	brne	SYSSTRINGCOPY
	ret

;********************************************************************************

SYSDIVSUB:
;clr SysByteTempX
	clr	SYSBYTETEMPX
;SysDivLoop = 8
	ldi	SysValueCopy,8
	mov	SYSDIVLOOP,SysValueCopy
SYSDIV8START:
;lsl SysByteTempA
	lsl	SYSBYTETEMPA
;rol SysByteTempX
	rol	SYSBYTETEMPX
;asm sub SysByteTempX,SysByteTempB 'asm needed, or else sub will be used as start of sub
	sub	SYSBYTETEMPX,SYSBYTETEMPB
;sbr SysByteTempA,1
	sbr	SYSBYTETEMPA,1
;brsh Div8NotNeg
	brsh	DIV8NOTNEG
;cbr SysByteTempA,1
	cbr	SYSBYTETEMPA,1
;add SysByteTempX,SysByteTempB
	add	SYSBYTETEMPX,SYSBYTETEMPB
DIV8NOTNEG:
;dec SysDivLoop
	dec	SYSDIVLOOP
;brne SysDiv8Start
	brne	SYSDIV8START
	ret

;********************************************************************************

SYSDIVSUB16:
;dim SysWordTempA as word
;dim SysWordTempB as word
;dim SysWordTempX as word
;dim SysDivMultA as word
;dim SysDivMultB as word
;dim SysDivMultX as word
;SysDivMultA = SysWordTempA
	mov	SYSDIVMULTA,SYSWORDTEMPA
	mov	SYSDIVMULTA_H,SYSWORDTEMPA_H
;SysDivMultB = SysWordTempB
	mov	SYSDIVMULTB,SYSWORDTEMPB
	mov	SYSDIVMULTB_H,SYSWORDTEMPB_H
;SysDivMultX = 0
	ldi	SysValueCopy,0
	mov	SYSDIVMULTX,SysValueCopy
	ldi	SysValueCopy,0
	mov	SYSDIVMULTX_H,SysValueCopy
;if SysDivMultB = 0 then
	mov	SysWORDTempA,SYSDIVMULTB
	mov	SysWORDTempA_H,SYSDIVMULTB_H
	ldi	SysWORDTempB,0
	ldi	SysWORDTempB_H,0
	rcall	SysCompEqual16
	sbrs	SysByteTempX,0
	rjmp	ENDIF28
;SysWordTempA = 0
	ldi	SysValueCopy,0
	mov	SYSWORDTEMPA,SysValueCopy
	ldi	SysValueCopy,0
	mov	SYSWORDTEMPA_H,SysValueCopy
;exit sub
	ret
;end if
ENDIF28:
;SysDivLoop = 16
	ldi	SysValueCopy,16
	mov	SYSDIVLOOP,SysValueCopy
SYSDIV16START:
;set C off
	clc
;Rotate SysDivMultA Left
	rol	SYSDIVMULTA
	rol	SYSDIVMULTA_H
;Rotate SysDivMultX Left
	rol	SYSDIVMULTX
	rol	SYSDIVMULTX_H
;SysDivMultX = SysDivMultX - SysDivMultB
	mov	SysTemp3,SYSDIVMULTX
	sub	SysTemp3,SYSDIVMULTB
	mov	SYSDIVMULTX,SysTemp3
	mov	SysTemp3,SYSDIVMULTX_H
	sbc	SysTemp3,SYSDIVMULTB_H
	mov	SYSDIVMULTX_H,SysTemp3
;Set SysDivMultA.0 On
	sbr	 SYSDIVMULTA,1<<0
;If C On Then
	brbc	C,ENDIF29
;Set SysDivMultA.0 Off
	cbr	 SYSDIVMULTA,1<<0
;SysDivMultX = SysDivMultX + SysDivMultB
	mov	SysTemp3,SYSDIVMULTX
	add	SysTemp3,SYSDIVMULTB
	mov	SYSDIVMULTX,SysTemp3
	mov	SysTemp3,SYSDIVMULTX_H
	adc	SysTemp3,SYSDIVMULTB_H
	mov	SYSDIVMULTX_H,SysTemp3
;End If
ENDIF29:
;dec SysDivLoop
	dec	SYSDIVLOOP
;brne SysDiv16Start
	brne	SYSDIV16START
;SysWordTempA = SysDivMultA
	mov	SYSWORDTEMPA,SYSDIVMULTA
	mov	SYSWORDTEMPA_H,SYSDIVMULTA_H
;SysWordTempX = SysDivMultX
	mov	SYSWORDTEMPX,SYSDIVMULTX
	mov	SYSWORDTEMPX_H,SYSDIVMULTX_H
	ret

;********************************************************************************

SYSDIVSUBINT:
;Dim SysIntegerTempA, SysIntegerTempB, SysIntegerTempX As Integer
;Dim SysSignByte As Byte
;SysSignByte = SysIntegerTempA_H xor SysIntegerTempB_H
	mov	SysTemp3,SYSINTEGERTEMPB_H
	eor	SysTemp3,SYSINTEGERTEMPA_H
	mov	SYSSIGNBYTE,SysTemp3
;If SysIntegerTempA.15 Then SysIntegerTempA = -SysIntegerTempA
	sbrs	SYSINTEGERTEMPA_H,7
	rjmp	ENDIF30
	mov	SysTemp1,SYSINTEGERTEMPA
	mov	SysTemp1_H,SYSINTEGERTEMPA_H
	com	SysTemp1
	com	SysTemp1_H
	inc	SysTemp1
	brne	PC + 2
	inc	SysTemp1_H
	mov	SYSINTEGERTEMPA,SysTemp1
	mov	SYSINTEGERTEMPA_H,SysTemp1_H
ENDIF30:
;If SysIntegerTempB.15 Then SysIntegerTempB = -SysIntegerTempB
	sbrs	SYSINTEGERTEMPB_H,7
	rjmp	ENDIF31
	mov	SysTemp1,SYSINTEGERTEMPB
	mov	SysTemp1_H,SYSINTEGERTEMPB_H
	com	SysTemp1
	com	SysTemp1_H
	inc	SysTemp1
	brne	PC + 2
	inc	SysTemp1_H
	mov	SYSINTEGERTEMPB,SysTemp1
	mov	SYSINTEGERTEMPB_H,SysTemp1_H
ENDIF31:
;SysDivSub16
	rcall	SYSDIVSUB16
;If SysSignByte.7 Then
	sbrs	SYSSIGNBYTE,7
	rjmp	ENDIF32
;SysIntegerTempA = -SysIntegerTempA
	mov	SysTemp1,SYSINTEGERTEMPA
	mov	SysTemp1_H,SYSINTEGERTEMPA_H
	com	SysTemp1
	com	SysTemp1_H
	inc	SysTemp1
	brne	PC + 2
	inc	SysTemp1_H
	mov	SYSINTEGERTEMPA,SysTemp1
	mov	SYSINTEGERTEMPA_H,SysTemp1_H
;SysIntegerTempX = -SysIntegerTempX
	mov	SysTemp1,SYSINTEGERTEMPX
	mov	SysTemp1_H,SYSINTEGERTEMPX_H
	com	SysTemp1
	com	SysTemp1_H
	inc	SysTemp1
	brne	PC + 2
	inc	SysTemp1_H
	mov	SYSINTEGERTEMPX,SysTemp1
	mov	SYSINTEGERTEMPX_H,SysTemp1_H
;End If
ENDIF32:
	ret

;********************************************************************************

SYSMULTSUB16:
;dim SysWordTempA as word
;dim SysWordTempB as word
;dim SysWordTempX as word
;dim SysDivMultX as word ' alias SysWordTempX_U, SysWordTempX_H
;mul SysWordTempA, SysWordTempB
	mul	SYSWORDTEMPA, SYSWORDTEMPB
;SysDivMultX = SysWordTempX
	mov	SYSDIVMULTX,SYSWORDTEMPX
	mov	SYSDIVMULTX_H,SYSWORDTEMPX_H
;mul SysWordTempA, SysWordTempB_H
	mul	SYSWORDTEMPA, SYSWORDTEMPB_H
;add SysDivMultX_H, SysWordTempX
	add	SYSDIVMULTX_H, SYSWORDTEMPX
;mul SysWordTempA_H, SysWordTempB
	mul	SYSWORDTEMPA_H, SYSWORDTEMPB
;add SysDivMultX_H, SysWordTempX
	add	SYSDIVMULTX_H, SYSWORDTEMPX
;SysWordTempX = SysDivMultX
	mov	SYSWORDTEMPX,SYSDIVMULTX
	mov	SYSWORDTEMPX_H,SYSDIVMULTX_H
	ret

;********************************************************************************

SYSMULTSUBINT:
;Dim SysIntegerTempA, SysIntegerTempB, SysIntegerTempX As Integer
;Dim SysSignByte As Byte
;SysSignByte = SysIntegerTempA_H xor SysIntegerTempB_H
	mov	SysTemp3,SYSINTEGERTEMPB_H
	eor	SysTemp3,SYSINTEGERTEMPA_H
	mov	SYSSIGNBYTE,SysTemp3
;if SysIntegerTempA.15 then SysIntegerTempA = -SysIntegerTempA
	sbrs	SYSINTEGERTEMPA_H,7
	rjmp	ENDIF25
	mov	SysTemp1,SYSINTEGERTEMPA
	mov	SysTemp1_H,SYSINTEGERTEMPA_H
	com	SysTemp1
	com	SysTemp1_H
	inc	SysTemp1
	brne	PC + 2
	inc	SysTemp1_H
	mov	SYSINTEGERTEMPA,SysTemp1
	mov	SYSINTEGERTEMPA_H,SysTemp1_H
ENDIF25:
;if SysIntegerTempB.15 then SysIntegerTempB = -SysIntegerTempB
	sbrs	SYSINTEGERTEMPB_H,7
	rjmp	ENDIF26
	mov	SysTemp1,SYSINTEGERTEMPB
	mov	SysTemp1_H,SYSINTEGERTEMPB_H
	com	SysTemp1
	com	SysTemp1_H
	inc	SysTemp1
	brne	PC + 2
	inc	SysTemp1_H
	mov	SYSINTEGERTEMPB,SysTemp1
	mov	SYSINTEGERTEMPB_H,SysTemp1_H
ENDIF26:
;SysMultSub16
	rcall	SYSMULTSUB16
;if SysSignByte.7 then SysIntegerTempX = -SysIntegerTempX
	sbrs	SYSSIGNBYTE,7
	rjmp	ENDIF27
	mov	SysTemp1,SYSINTEGERTEMPX
	mov	SysTemp1_H,SYSINTEGERTEMPX_H
	com	SysTemp1
	com	SysTemp1_H
	inc	SysTemp1
	brne	PC + 2
	inc	SysTemp1_H
	mov	SYSINTEGERTEMPX,SysTemp1
	mov	SYSINTEGERTEMPX_H,SysTemp1_H
ENDIF27:
	ret

;********************************************************************************

SYSREADSTRING:
;Dim SysCalcTempA As Byte
;Dim SysStringLength As Byte
;Dim SysCalcTempX As Byte
;lpm
	lpm
;mov SysCalcTempA, SysCalcTempX
	mov	SYSCALCTEMPA, SYSCALCTEMPX
;SysReadA += 1
	inc	SYSREADA
	brne	PC + 2
	inc	SYSREADA_H
;st Y+, SysCalcTempA
	st	Y+, SYSCALCTEMPA
;rjmp SysStringReadCheck
	rjmp	SYSSTRINGREADCHECK
SYSREADSTRINGPART:
;lpm
	lpm
;mov SysCalcTempA, SysCalcTempX
	mov	SYSCALCTEMPA, SYSCALCTEMPX
;SysReadA += 1
	inc	SYSREADA
	brne	PC + 2
	inc	SYSREADA_H
;add SysStringLength, SysCalcTempA
	add	SYSSTRINGLENGTH, SYSCALCTEMPA
SYSSTRINGREADCHECK:
;cpi SysCalcTempA, 0
	cpi	SYSCALCTEMPA, 0
;brne SysStringRead
	brne	SYSSTRINGREAD
;ret
	ret
SYSSTRINGREAD:
;lpm
	lpm
;SysReadA += 1
	inc	SYSREADA
	brne	PC + 2
	inc	SYSREADA_H
;st Y+, SysCalcTempX
	st	Y+, SYSCALCTEMPX
;dec SysCalcTempA
	dec	SYSCALCTEMPA
;brne SysStringRead
	brne	SYSSTRINGREAD
	ret

;********************************************************************************

SysStringTables:

StringTable1:
.DB	12,71,67,66,97,115,105,99,32,50,48,49,53,0


StringTable2:
.DB	12,68,83,49,56,66,50,48,32,68,101,109,111,0


StringTable3:
.DB	7,82,101,97,100,105,110,103


StringTable4:
.DB	1,99


StringTable5:
.DB	1,102


;********************************************************************************


