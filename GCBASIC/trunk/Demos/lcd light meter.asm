;Program compiled by Great Cow BASIC (0.9 4/4/2007)

;********************************************************************************

;Set up the assembler options (Chip type, clock source, other bits and pieces)
 LIST p=16F877A, r=DEC
#include <P16F877A.inc>
 __CONFIG _HS_OSC & _WDT_OFF & _LVP_OFF

;********************************************************************************

;Set aside memory locations for variables
ADREADPORT	equ	32
AVERAGE	equ	33
DataPointer	equ	34
DELAYTEMP	equ	35
DELAYTEMP2	equ	36
DELAYTEMP4	equ	37
LCDBYTE	equ	38
LCDCOLUMN	equ	39
LCDLINE	equ	40
LCDREADY	equ	41
LCDVALUE	equ	42
LCDVALUETEMP	equ	43
PRINTLEN	equ	44
READAD	equ	45
StringPointer	equ	46
SysArrayTemp1	equ	47
SysCalcTempA	equ	48
SysCalcTempB	equ	49
SysCalcTempX	equ	50
SysDivLoop	equ	51
SysIFTemp	equ	52
SYSLCDTEMP	equ	53
SysPRINTDATAHandler	equ	54
SysPRINTDATAHandler_H	equ	55
SYSPRINTTEMP	equ	56
SysTemp1	equ	57
SysTemp2	equ	58
SysTemp3	equ	59
SysWaitTemp10US	equ	60
SysWaitTempMS	equ	61
SysWaitTempS	equ	62
TEMP1	equ	63
TEMP2	equ	64

;********************************************************************************

;Jump to initialisation code when PIC is reset
	ORG	0
	call	INITSYS
	goto	SystemInitialise

;********************************************************************************

;Interrupt vector
	ORG	4
	retfie

;********************************************************************************

;Various initialisation routines, automatically called by GCBASIC
SystemInitialise
	call	INITLCD

;********************************************************************************

;Start of the main program
	bsf	STATUS, IRP
	movlw	218
	movwf	FSR
	clrf	StringPointer
SysReadString1
	call	StringLookup1
	movwf	INDF
	incf	StringPointer, F
	incf	FSR, F
	movf	StringPointer, W
	sublw	13
	btfss	STATUS, Z
	goto	SysReadString1
	movlw	218
	movwf	SysPRINTDATAHandler
	bsf	SysPRINTDATAHandler_H,0
	call	PRINT 
	movlw	1
	movwf	LCDLINE
	movlw	2
	movwf	LCDCOLUMN
	call	LOCATE 
	bsf	STATUS, IRP
	movlw	218
	movwf	FSR
	clrf	StringPointer
SysReadString2
	call	StringLookup2
	movwf	INDF
	incf	StringPointer, F
	incf	FSR, F
	movf	StringPointer, W
	sublw	15
	btfss	STATUS, Z
	goto	SysReadString2
	movlw	218
	movwf	SysPRINTDATAHandler
	bsf	SysPRINTDATAHandler_H,0
	call	PRINT 
	movlw	2
	movwf	SysWaitTempS
	call	Delay_S
MAIN
	clrf	ADREADPORT
	call	FN_READAD
	movf	READAD,W
	movwf	TEMP1
	movlw	25
	movwf	SysWaitTempMS
	call	Delay_MS
	clrf	ADREADPORT
	call	FN_READAD
	movf	READAD,W
	movwf	TEMP2
	call	CLS
	bsf	STATUS, IRP
	movlw	218
	movwf	FSR
	clrf	StringPointer
SysReadString3
	call	StringLookup3
	movwf	INDF
	incf	StringPointer, F
	incf	FSR, F
	movf	StringPointer, W
	sublw	14
	btfss	STATUS, Z
	goto	SysReadString3
	movlw	218
	movwf	SysPRINTDATAHandler
	bsf	SysPRINTDATAHandler_H,0
	call	PRINT 
	movf	TEMP1,W
	movwf	SYSCALCTEMPA
	movf	TEMP2,W
	movwf	SYSCALCTEMPB
	call	FN_AVERAGE
	movf	AVERAGE,W
	movwf	LCDVALUE
	call	LCDINT 
	movlw	100
	movwf	SysWaitTempMS
	call	Delay_MS
	goto	MAIN
BASPROGRAMEND
	sleep
	goto	$

;********************************************************************************

; String Lookup Tables
StringLookup1
	bcf	STATUS, C
	movf	StringPointer, W
	addlw	1
	addlw	low StringTable1
	movwf	DataPointer
	movlw	high StringTable1
	btfsc	STATUS, C
	addlw	1
	movwf	PCLATH
	movf	DataPointer, W
StringTable1
	movwf	PCL
	retlw	12
	retlw	76	;L
	retlw	105	;i
	retlw	103	;g
	retlw	104	;h
	retlw	116	;t
	retlw	32	; 
	retlw	77	;M
	retlw	101	;e
	retlw	116	;t
	retlw	101	;e
	retlw	114	;r
	retlw	58	;
	return

StringLookup2
	bcf	STATUS, C
	movf	StringPointer, W
	addlw	1
	addlw	low StringTable2
	movwf	DataPointer
	movlw	high StringTable2
	btfsc	STATUS, C
	addlw	1
	movwf	PCLATH
	movf	DataPointer, W
StringTable2
	movwf	PCL
	retlw	14
	retlw	65	;A
	retlw	32	; 
	retlw	71	;G
	retlw	67	;C
	retlw	66	;B
	retlw	65	;A
	retlw	83	;S
	retlw	73	;I
	retlw	67	;C
	retlw	32	; 
	retlw	68	;D
	retlw	101	;e
	retlw	109	;m
	retlw	111	;o
	return

StringLookup3
	bcf	STATUS, C
	movf	StringPointer, W
	addlw	1
	addlw	low StringTable3
	movwf	DataPointer
	movlw	high StringTable3
	btfsc	STATUS, C
	addlw	1
	movwf	PCLATH
	movf	DataPointer, W
StringTable3
	movwf	PCL
	retlw	13
	retlw	76	;L
	retlw	105	;i
	retlw	103	;g
	retlw	104	;h
	retlw	116	;t
	retlw	32	; 
	retlw	76	;L
	retlw	101	;e
	retlw	118	;v
	retlw	101	;e
	retlw	108	;l
	retlw	58	;
	retlw	32	; 
	return

;********************************************************************************
;Subroutines included in program
;********************************************************************************

CLS
	bcf	PORTD,0
	movlw	B'00000001'
	movwf	LCDBYTE
	call	LCDWRITEBYTE 
	movlw	2
	movwf	SysWaitTempMS
	call	Delay_MS
	movlw	0X80
	movwf	LCDBYTE
	call	LCDWRITEBYTE
	movlw	10
	movwf	SysWaitTemp10US
	call	Delay_10US
	return

;********************************************************************************

DELAY_10US
D10US_START
	movlw	16
	movwf	DELAYTEMP
D10US_LOOP
	decfsz	DELAYTEMP, F
	goto	D10US_LOOP
	decfsz	SysWaitTemp10US, F
	goto	D10US_START
	return

;********************************************************************************

Delay_MS
DMS_START
	movlw	10
	movwf	DELAYTEMP2
DMS_OUTER
	movlw	166
	movwf	DELAYTEMP
DMS_INNER
	decfsz	DELAYTEMP, F
	goto	DMS_INNER
	decfsz	DELAYTEMP2, F
	goto	DMS_OUTER
	decfsz	SysWaitTempMS, F
	goto	DMS_START
	return

;********************************************************************************

Delay_S
DS_START
	movlw	10
	movwf	DELAYTEMP4
DS_OUTER
	movlw	100
	movwf	SysWaitTempMS
	call	Delay_MS
	decfsz	DELAYTEMP4, F
	goto	DS_OUTER
	decfsz	SysWaitTempS, F
	goto	DS_START
	return

;********************************************************************************

FN_AVERAGE
	bcf	STATUS,C
	movf	SYSCALCTEMPB,W
	addwf	SYSCALCTEMPA,W
	movwf	AVERAGE
	rrf	AVERAGE,F
	return

;********************************************************************************

FN_LCDREADY
	clrf	LCDREADY
	bcf	SYSLCDTEMP,2
	btfsc	PORTD,0
	bsf	SYSLCDTEMP,2
	bsf	PORTD,1
	bcf	PORTD,0
	bsf	PORTD,2
	movlw	5
	movwf	SysWaitTemp10US
	call	Delay_10US
	movlw	255
	banksel	TRISC
	movwf	TRISC
	banksel	PORTC
	btfsc	PORTC,7
	goto	ENDIF1
	movlw	255
	movwf	LCDREADY
ENDIF1
	bcf	PORTD,2
	movlw	25
	movwf	SysWaitTemp10US
	call	Delay_10US
	bcf	PORTD,0
	btfsc	SYSLCDTEMP,2
	bsf	PORTD,0
	return

;********************************************************************************

FN_READAD
	banksel	ADCON1
	bcf	ADCON1,PCFG3
	bcf	ADCON1,PCFG2
	bcf	ADCON1,PCFG1
	bcf	ADCON1,PCFG0
	banksel	ADCON0
	bcf	ADCON0,ADCS1
	bsf	ADCON0,ADCS0
	bcf	ADCON0,CHS0
	bcf	ADCON0,CHS1
	bcf	ADCON0,CHS2
	movlw	1
	subwf	ADREADPORT,W
	btfsc	STATUS, Z
	bsf	ADCON0,CHS0
	movlw	2
	subwf	ADREADPORT,W
	btfsc	STATUS, Z
	bsf	ADCON0,CHS1
	movlw	3
	subwf	ADREADPORT,W
	btfss	STATUS, Z
	goto	ENDIF4
	bsf	ADCON0,CHS0
	bsf	ADCON0,CHS1
ENDIF4
	movlw	3
	movwf	SysIFTemp
	movf	ADREADPORT,W
	subwf	SysIFTemp,W
	btfss	STATUS, C
	bsf	ADCON0,CHS2
	movlw	5
	subwf	ADREADPORT,W
	btfsc	STATUS, Z
	bsf	ADCON0,CHS0
	movlw	6
	subwf	ADREADPORT,W
	btfsc	STATUS, Z
	bsf	ADCON0,CHS1
	movlw	7
	subwf	ADREADPORT,W
	btfss	STATUS, Z
	goto	ENDIF8
	bsf	ADCON0,CHS0
	bsf	ADCON0,CHS1
ENDIF8
	bsf	ADCON0,ADON
	movlw	2
	movwf	SysWaitTemp10US
	call	Delay_10US
	bsf	ADCON0,GO_DONE
SysWaitLoop1
	btfsc	ADCON0,GO_DONE
	goto	SysWaitLoop1
	bcf	ADCON0,ADON
	banksel	ADCON1
	bcf	ADCON1,PCFG3
	bsf	ADCON1,PCFG2
	bsf	ADCON1,PCFG1
	bcf	ADCON1,PCFG0
	banksel	ADRESH
	movf	ADRESH,W
	movwf	READAD
	return

;********************************************************************************

INITLCD
	banksel	TRISD
	bcf	TRISD,0
	bcf	TRISD,1
	bcf	TRISD,2
	movlw	10
	banksel	SysWaitTempMS
	movwf	SysWaitTempMS
	call	Delay_MS
SysWaitLoop2
	call	FN_LCDREADY
	movf	LCDREADY,W
	movwf	SysIFTemp
	btfsc	STATUS, Z
	goto	SysWaitLoop2
	bcf	PORTD,0
	movlw	B'00111000'
	movwf	LCDBYTE
	call	LCDWRITEBYTE
	bcf	PORTD,0
	movlw	B'00000110'
	movwf	LCDBYTE
	call	LCDWRITEBYTE
	movlw	5
	movwf	SysWaitTemp10US
	call	Delay_10US
	movlw	B'00001100'
	movwf	LCDBYTE
	call	LCDWRITEBYTE
	movlw	5
	movwf	SysWaitTemp10US
	call	Delay_10US
	call	CLS
	return

;********************************************************************************

INITSYS
	clrf	PORTA
	clrf	PORTB
	clrf	PORTC
	clrf	PORTD
	clrf	PORTE
	movlw	7
	banksel	CMCON
	movwf	CMCON
	banksel	ADCON0
	bcf	ADCON0,ADON
	banksel	ADCON1
	bcf	ADCON1,ADFM
	bcf	ADCON1,PCFG3
	bsf	ADCON1,PCFG2
	bsf	ADCON1,PCFG1
	bcf	ADCON1,PCFG0
	banksel	STATUS
	return

;********************************************************************************

LCDINT
	clrf	LCDVALUETEMP
	bsf	PORTD,0
	movlw	100
	subwf	LCDVALUE,W
	btfss	STATUS, C
	goto	ENDIF9
	movf	LCDVALUE,W
	movwf	SysCalcTempA
	movlw	100
	movwf	SysCalcTempB
	call	SysDivSub
	movf	SysCalcTempA,W
	movwf	LCDVALUETEMP
	movf	SYSCALCTEMPX,W
	movwf	LCDVALUE
	movlw	48
	addwf	LCDVALUETEMP,W
	movwf	LCDBYTE
	call	LCDWRITEBYTE
ENDIF9
	movf	LCDVALUETEMP,W
	movwf	SysCalcTempA
	clrf	SysCalcTempB
	call	SysCompMoreThan
	movf	SysCalcTempX,W
	movwf	SysTemp1
	movf	LCDVALUE,W
	movwf	SysCalcTempA
	movlw	10
	movwf	SysCalcTempB
	call	SysCompMoreOrEqual
	movf	SysCalcTempX,W
	movwf	SysTemp2
	movf	SysTemp1,W
	iorwf	SysTemp2,W
	movwf	SysIFTemp
	btfsc	STATUS, Z
	goto	ENDIF10
	movf	LCDVALUE,W
	movwf	SysCalcTempA
	movlw	10
	movwf	SysCalcTempB
	call	SysDivSub
	movf	SysCalcTempA,W
	movwf	LCDVALUETEMP
	movf	SYSCALCTEMPX,W
	movwf	LCDVALUE
	movlw	48
	addwf	LCDVALUETEMP,W
	movwf	LCDBYTE
	call	LCDWRITEBYTE
ENDIF10
	movlw	48
	addwf	LCDVALUE,W
	movwf	LCDBYTE
	call	LCDWRITEBYTE 
	return

;********************************************************************************

LCDWRITEBYTE
SysWaitLoop3
	call	FN_LCDREADY
	movf	LCDREADY,W
	movwf	SysIFTemp
	btfsc	STATUS, Z
	goto	SysWaitLoop3
	bcf	PORTD,1
	banksel	TRISC
	clrf	TRISC
	banksel	LCDBYTE
	movf	LCDBYTE,W
	movwf	PORTC
	bsf	PORTD,2
	movlw	5
	movwf	SysWaitTemp10US
	call	Delay_10US
	bcf	PORTD,2
	movlw	5
	movwf	SysWaitTemp10US
	call	Delay_10US
	clrf	PORTC
	return

;********************************************************************************

LOCATE
	bcf	PORTD,0
	movlw	1
	movwf	SysIFTemp
	movf	LCDLINE,W
	subwf	SysIFTemp,W
	btfsc	STATUS, C
	goto	ENDIF11
	movlw	2
	subwf	LCDLINE,F
	movlw	20
	addwf	LCDCOLUMN,F
ENDIF11
	movlw	64
	movwf	SysCalcTempA
	movf	LCDLINE,W
	movwf	SysCalcTempB
	call	SysMultSub
	movf	SysCalcTempX,W
	movwf	SysTemp1
	movf	LCDCOLUMN,W
	addwf	SysTemp1,W
	movwf	SysTemp2
	movlw	128
	iorwf	SysTemp2,W
	movwf	LCDBYTE
	call	LCDWRITEBYTE
	movlw	5
	movwf	SysWaitTemp10US
	call	Delay_10US
	return

;********************************************************************************

PRINT
	movlw	0
	addwf	SysPRINTDATAHandler,W
	movwf	FSR
	bcf	STATUS, IRP
	btfsc	SysPRINTDATAHandler_H,0
	bsf	STATUS, IRP
	movf	INDF,W
	movwf	SysArrayTemp1
	movf	SysArrayTemp1,W
	movwf	PRINTLEN
	movlw	0
	subwf	PRINTLEN,W
	btfsc	STATUS, Z
	goto	NOPRINTDATA
	bsf	PORTD,0
	clrf	SYSPRINTTEMP
SysForLoop1
	incf	SYSPRINTTEMP,F
	movf	SYSPRINTTEMP,W
	addwf	SysPRINTDATAHandler,W
	movwf	FSR
	bcf	STATUS, IRP
	btfsc	SysPRINTDATAHandler_H,0
	bsf	STATUS, IRP
	movf	INDF,W
	movwf	SysArrayTemp1
	movf	SysArrayTemp1,W
	movwf	LCDBYTE
	call	LCDWRITEBYTE
	movf	PRINTLEN,W
	subwf	SYSPRINTTEMP,W
	btfss	STATUS, C
	goto	SysForLoop1
NOPRINTDATA
	return

;********************************************************************************

SYSCOMPMOREOREQUAL
	clrf	SYSCALCTEMPX
	movf	SYSCALCTEMPB, W
	subwf	SYSCALCTEMPA, W
	btfss	STATUS, C
	return
	movlw	255
	movwf	SYSCALCTEMPX
	return

;********************************************************************************

SYSCOMPMORETHAN
	clrf	SYSCALCTEMPX
	bsf	STATUS, C
	movf	SYSCALCTEMPA, W
	subwf	SYSCALCTEMPB, W
	btfsc	STATUS, C
	return
	movlw	255
	movwf	SYSCALCTEMPX
	return

;********************************************************************************

SYSDIVSUB
	clrf	SYSCALCTEMPX
	movlw	8
	movwf	SYSDIVLOOP
SYSDIV8START
	bcf	STATUS, C
	rlf	SYSCALCTEMPA, F
	rlf	SYSCALCTEMPX, F
	movf	SYSCALCTEMPB, W
	subwf	SYSCALCTEMPX, F
	bsf	SYSCALCTEMPA, 0
	btfsc	STATUS, C
	goto	DIV8NOTNEG
	bcf	SYSCALCTEMPA, 0
	movf	SYSCALCTEMPB, W
	addwf	SYSCALCTEMPX, F
DIV8NOTNEG
	decfsz	SYSDIVLOOP, F
	goto	SYSDIV8START
	return

;********************************************************************************

SYSMULTSUB
	clrf	SYSCALCTEMPX
MUL8LOOP
	movf	SYSCALCTEMPA, W
	btfsc	SYSCALCTEMPB, 0
	addwf	SYSCALCTEMPX, F
	bcf	STATUS, C
	rrf	SYSCALCTEMPB, F
	bcf	STATUS, C
	rlf	SYSCALCTEMPA, F
	movf	SYSCALCTEMPB, F
	btfss	STATUS, Z
	goto	MUL8LOOP
	return

;********************************************************************************

 END
