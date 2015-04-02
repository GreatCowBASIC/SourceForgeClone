;Program compiled by Great Cow BASIC (0.9 2015-03-30)
;Need help? See the GCBASIC forums at http://sourceforge.net/projects/gcbasic/forums,
;check the documentation or email w_cholmondeley at users dot sourceforge dot net.

;********************************************************************************

;Set up the assembler options (Chip type, clock source, other bits and pieces)
 LIST p=16F1938, r=DEC
#include <P16F1938.inc>
 __CONFIG _CONFIG1, _MCLRE_OFF & _WDTE_OFF & _FOSC_INTOSC
 __CONFIG _CONFIG2, _LVP_OFF & _PLLEN_OFF

;********************************************************************************

;Set aside memory locations for variables
DELAYTEMP	EQU	112
DELAYTEMP2	EQU	113
SYSDIVMULTA	EQU	119
SYSDIVMULTA_H	EQU	120
SYSDIVMULTB	EQU	123
SYSDIVMULTB_H	EQU	124
SYSDIVMULTX	EQU	114
SYSDIVMULTX_H	EQU	115
SysByteTempX	EQU	112
SysDivLoop	EQU	116
SysWORDTempA	EQU	117
SysWORDTempA_H	EQU	118
SysWORDTempB	EQU	121
SysWORDTempB_H	EQU	122
SysWORDTempX	EQU	112
SysWORDTempX_H	EQU	113
SysWaitTempMS	EQU	114
SysWaitTempMS_H	EQU	115
CCPCONCACHE	EQU	32
DUTY	EQU	33
PR2_TEMP	EQU	34
PR2_TEMP_H	EQU	35
PWMCHANNEL	EQU	37
PWMDUTY	EQU	38
PWMFREQ	EQU	39
SysTemp1	EQU	40
T2_PR	EQU	41
TEMPVAR	EQU	42
TEMPVAR_H	EQU	43

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
	call	INITPWM

;Start of the main program
;#define LED PORTC.2
;#define Max_Duty 120    '~50 %
;#define Flash_ON 110    ' ms
;#define Flash_OFF 110   ' ms
;#define Delay 20        ' ramp delay
;DIR LED OUT  'CCP1/P1A ( Pin 13)
	banksel	TRISC
	bcf	TRISC,2
;Dim Duty as Byte
;Duty = 10  'starting Value
	movlw	10
	banksel	DUTY
	movwf	DUTY
;DO Forever
SysDoLoop_S1
;Do while duty  < Max_Duty
SysDoLoop_S2
	movlw	120
	subwf	DUTY,W
	btfsc	STATUS, C
	goto	SysDoLoop_E2
;Duty = Duty + 4
	movlw	4
	addwf	DUTY,F
;HPWM 1,2, duty
	movlw	1
	movwf	PWMCHANNEL
	movlw	2
	movwf	PWMFREQ
	movf	DUTY,W
	movwf	PWMDUTY
	call	HPWM
;wait delay ms
	movlw	20
	movwf	SysWaitTempMS
	clrf	SysWaitTempMS_H
	call	Delay_MS
;loop
	goto	SysDoLoop_S2
SysDoLoop_E2
;HPWM 1,2, 255
	movlw	1
	movwf	PWMCHANNEL
	movlw	2
	movwf	PWMFREQ
	movlw	255
	movwf	PWMDUTY
	call	HPWM
;Wait FLASH_ON Ms
	movlw	110
	movwf	SysWaitTempMS
	clrf	SysWaitTempMS_H
	call	Delay_MS
;HPWM 1,2,max_duty
	movlw	1
	movwf	PWMCHANNEL
	movlw	2
	movwf	PWMFREQ
	movlw	120
	movwf	PWMDUTY
	call	HPWM
;Wait FLASH_OFF ms
	movlw	110
	movwf	SysWaitTempMS
	clrf	SysWaitTempMS_H
	call	Delay_MS
;HPWM 1, 2, 255
	movlw	1
	movwf	PWMCHANNEL
	movlw	2
	movwf	PWMFREQ
	movlw	255
	movwf	PWMDUTY
	call	HPWM
;Wait FLASH_ON Ms
	movlw	110
	movwf	SysWaitTempMS
	clrf	SysWaitTempMS_H
	call	Delay_MS
;HPWM 1, 2, max_duty
	movlw	1
	movwf	PWMCHANNEL
	movlw	2
	movwf	PWMFREQ
	movlw	120
	movwf	PWMDUTY
	call	HPWM
;Wait Flash_OFF ms
	movlw	110
	movwf	SysWaitTempMS
	clrf	SysWaitTempMS_H
	call	Delay_MS
;Do while duty  > 10
SysDoLoop_S3
	movf	DUTY,W
	sublw	10
	btfsc	STATUS, C
	goto	SysDoLoop_E3
;Duty = Duty - 4
	movlw	4
	subwf	DUTY,F
;HPWM 1,2, duty
	movlw	1
	movwf	PWMCHANNEL
	movlw	2
	movwf	PWMFREQ
	movf	DUTY,W
	movwf	PWMDUTY
	call	HPWM
;wait delay ms
	movlw	20
	movwf	SysWaitTempMS
	clrf	SysWaitTempMS_H
	call	Delay_MS
;loop
	goto	SysDoLoop_S3
SysDoLoop_E3
;LOOP
	goto	SysDoLoop_S1
SysDoLoop_E1
;end
	goto	BASPROGRAMEND
;#define LCD_I2C_Address_1 0x4E
;#define slow_us 40
;#define medium_us 20
;#define fast_us 10
BASPROGRAMEND
	sleep
	goto	BASPROGRAMEND

;********************************************************************************

Delay_MS
	incf	SysWaitTempMS_H, F
DMS_START
	movlw	4
	movwf	DELAYTEMP2
DMS_OUTER
	movlw	165
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

HPWM
;T2_PR = 1
	movlw	1
	movwf	T2_PR
;PR2_Temp = PWMOsc1 / PWMFreq
	movlw	208
	movwf	SysWORDTempA
	movlw	7
	movwf	SysWORDTempA_H
	movf	PWMFREQ,W
	movwf	SysWORDTempB
	clrf	SysWORDTempB_H
	call	SysDivSub16
	movf	SysWORDTempA,W
	movwf	PR2_TEMP
	movf	SysWORDTempA_H,W
	movwf	PR2_TEMP_H
;IF PR2_Temp_H > 0 then
	sublw	0
	btfsc	STATUS, C
	goto	ENDIF1
;T2_PR = 4
	movlw	4
	movwf	T2_PR
;set STATUS.C off
	bcf	STATUS,C
;rotate PR2_Temp right
	rrf	PR2_TEMP_H,F
	rrf	PR2_TEMP,F
;set STATUS.C off
	bcf	STATUS,C
;rotate PR2_Temp right
	rrf	PR2_TEMP_H,F
	rrf	PR2_TEMP,F
;end if
ENDIF1
;IF PR2_Temp_H > 0 then
	movf	PR2_TEMP_H,W
	sublw	0
	btfsc	STATUS, C
	goto	ENDIF2
;T2_PR = 16
	movlw	16
	movwf	T2_PR
;set STATUS.C off
	bcf	STATUS,C
;rotate PR2_Temp right
	rrf	PR2_TEMP_H,F
	rrf	PR2_TEMP,F
;set STATUS.C off
	bcf	STATUS,C
;rotate PR2_Temp right
	rrf	PR2_TEMP_H,F
	rrf	PR2_TEMP,F
;end if
ENDIF2
;PR2 = PR2_Temp
	movf	PR2_TEMP,W
	movwf	PR2
;SET T2CON.T2CKPS0 OFF
	bcf	T2CON,T2CKPS0
;SET T2CON.T2CKPS1 OFF
	bcf	T2CON,T2CKPS1
;if T2_PR = 4 then SET T2CON.T2CKPS0 ON
	movlw	4
	subwf	T2_PR,W
	btfsc	STATUS, Z
	bsf	T2CON,T2CKPS0
;if T2_PR = 16 then SET T2CON.T2CKPS1 ON
	movlw	16
	subwf	T2_PR,W
	btfsc	STATUS, Z
	bsf	T2CON,T2CKPS1
;SET T2CON.TMR2ON ON
	bsf	T2CON,TMR2ON
;if PWMChannel = 1 then
	decf	PWMCHANNEL,W
	btfss	STATUS, Z
	goto	ENDIF5
;PR2_Temp = PWMDuty * (PR2 + 2)  'Correction  - WMR
	movlw	2
	addwf	PR2,W
	movwf	SysTemp1
	movf	PWMDUTY,W
	movwf	SysWORDTempA
	clrf	SysWORDTempA_H
	movf	SysTemp1,W
	movwf	SysWORDTempB
	clrf	SysWORDTempB_H
	call	SysMultSub16
	movf	SysWORDTempX,W
	movwf	PR2_TEMP
	movf	SysWORDTempX_H,W
	movwf	PR2_TEMP_H
;CCPR1L = PR2_Temp_H
	banksel	CCPR1L
	movwf	CCPR1L
;If PWMDuty = 0 Then CCPR1L = 0  ' Assure OFF at Zero - WMR
	banksel	PWMDUTY
	movf	PWMDUTY,F
	btfss	STATUS, Z
	goto	ENDIF8
	banksel	CCPR1L
	clrf	CCPR1L
ENDIF8
;SET CCP1CON.CCP1M3 ON
	banksel	CCP1CON
	bsf	CCP1CON,CCP1M3
;SET CCP1CON.CCP1M2 ON
	bsf	CCP1CON,CCP1M2
;SET CCP1CON.CCP1M1 OFF
	bcf	CCP1CON,CCP1M1
;SET CCP1CON.CCP1M0 OFF
	bcf	CCP1CON,CCP1M0
;end if
ENDIF5
;if PWMChannel = 2 then
	movlw	2
	banksel	PWMCHANNEL
	subwf	PWMCHANNEL,W
	btfss	STATUS, Z
	goto	ENDIF6
;PR2_Temp = PWMDuty * (PR2 + 2)  'Correction  - WMR
	movlw	2
	addwf	PR2,W
	movwf	SysTemp1
	movf	PWMDUTY,W
	movwf	SysWORDTempA
	clrf	SysWORDTempA_H
	movf	SysTemp1,W
	movwf	SysWORDTempB
	clrf	SysWORDTempB_H
	call	SysMultSub16
	movf	SysWORDTempX,W
	movwf	PR2_TEMP
	movf	SysWORDTempX_H,W
	movwf	PR2_TEMP_H
;CCPR1L = PR2_Temp_H
	banksel	CCPR1L
	movwf	CCPR1L
;If PWMDuty = 0 Then CCPR1L = 0  ' Assure OFF at Zero - WMR
	banksel	PWMDUTY
	movf	PWMDUTY,F
	btfss	STATUS, Z
	goto	ENDIF9
	banksel	CCPR1L
	clrf	CCPR1L
ENDIF9
;SET CCP2CON.CCP1M3 ON
	banksel	CCP2CON
	bsf	CCP2CON,CCP1M3
;SET CCP2CON.CCP1M2 ON
	bsf	CCP2CON,CCP1M2
;SET CCP2CON.CCP1M1 OFF
	bcf	CCP2CON,CCP1M1
;SET CCP2CON.CCP1M0 OFF
	bcf	CCP2CON,CCP1M0
;end if
ENDIF6
;if PWMChannel = 3 then
	movlw	3
	banksel	PWMCHANNEL
	subwf	PWMCHANNEL,W
	btfss	STATUS, Z
	goto	ENDIF7
;	PR2_Temp = PWMDuty * (PR2 + 2)  'Correction  - WMR
	movlw	2
	addwf	PR2,W
	movwf	SysTemp1
	movf	PWMDUTY,W
	movwf	SysWORDTempA
	clrf	SysWORDTempA_H
	movf	SysTemp1,W
	movwf	SysWORDTempB
	clrf	SysWORDTempB_H
	call	SysMultSub16
	movf	SysWORDTempX,W
	movwf	PR2_TEMP
	movf	SysWORDTempX_H,W
	movwf	PR2_TEMP_H
;CCPR1L = PR2_Temp_H
	banksel	CCPR1L
	movwf	CCPR1L
;If PWMDuty = 0 Then CCPR1L = 0  ' Assure OFF at Zero - WMR
	banksel	PWMDUTY
	movf	PWMDUTY,F
	btfss	STATUS, Z
	goto	ENDIF10
	banksel	CCPR1L
	clrf	CCPR1L
ENDIF10
;SET CCP3CON.CCP1M3 ON
	banksel	CCP3CON
	bsf	CCP3CON,CCP1M3
;SET CCP3CON.CCP1M2 ON
	bsf	CCP3CON,CCP1M2
;SET CCP3CON.CCP1M1 OFF
	bcf	CCP3CON,CCP1M1
;SET CCP3CON.CCP1M0 OFF
	bcf	CCP3CON,CCP1M0
;end if
ENDIF7
	banksel	STATUS
	return

;********************************************************************************

INITPWM
;PR2 = PR2Temp
	movlw	52
	movwf	PR2
;SET T2CON.T2CKPS0 OFF
	bcf	T2CON,T2CKPS0
;SET T2CON.T2CKPS1 OFF
	bcf	T2CON,T2CKPS1
;CCPR1L = DutyCycleH
	movlw	26
	banksel	CCPR1L
	movwf	CCPR1L
;SET CCPCONCache.DC1B1 ON
	banksel	CCPCONCACHE
	bsf	CCPCONCACHE,DC1B1
;SET CCPCONCache.DC1B0 OFF
	bcf	CCPCONCACHE,DC1B0
;SET CCPCONCache.CCP1M3 ON
	bsf	CCPCONCACHE,CCP1M3
;SET CCPCONCache.CCP1M2 ON
	bsf	CCPCONCACHE,CCP1M2
;SET CCPCONCache.CCP1M1 OFF
	bcf	CCPCONCACHE,CCP1M1
;SET CCPCONCache.CCP1M0 OFF
	bcf	CCPCONCACHE,CCP1M0
;SET T2CON.TMR2ON ON
	bsf	T2CON,TMR2ON
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
;Set SPLLEN Off
	bcf	OSCCON,SPLLEN
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

SYSCOMPEQUAL16
;dim SysWordTempA as word
;dim SysWordTempB as word
;dim SysByteTempX as byte
;clrf SysByteTempX
	clrf	SYSBYTETEMPX
;movf SysWordTempA, W
	movf	SYSWORDTEMPA, W
;subwf SysWordTempB, W
	subwf	SYSWORDTEMPB, W
;btfss STATUS, Z
	btfss	STATUS, Z
;return
	return
;movf SysWordTempA_H, W
	movf	SYSWORDTEMPA_H, W
;subwf SysWordTempB_H, W
	subwf	SYSWORDTEMPB_H, W
;btfss STATUS, Z
	btfss	STATUS, Z
;return
	return
;comf SysByteTempX,F
	comf	SYSBYTETEMPX,F
	return

;********************************************************************************

SYSCOMPLESSTHAN16
;dim SysWordTempA as word
;dim SysWordTempB as word
;dim SysByteTempX as byte
;clrf SysByteTempX
	clrf	SYSBYTETEMPX
;movf SysWordTempA_H,W
	movf	SYSWORDTEMPA_H,W
;subwf SysWordTempB_H,W
	subwf	SYSWORDTEMPB_H,W
;btfss STATUS,C
	btfss	STATUS,C
;return
	return
;movf SysWordTempB_H,W
	movf	SYSWORDTEMPB_H,W
;subwf SysWordTempA_H,W
	subwf	SYSWORDTEMPA_H,W
;btfss STATUS,C
	btfss	STATUS,C
;goto SCLT16True
	goto	SCLT16TRUE
;movf SysWordTempB,W
	movf	SYSWORDTEMPB,W
;subwf SysWordTempA,W
	subwf	SYSWORDTEMPA,W
;btfsc STATUS,C
	btfsc	STATUS,C
;return
	return
SCLT16TRUE
;comf SysByteTempX,F
	comf	SYSBYTETEMPX,F
	return

;********************************************************************************

SYSDIVSUB16
;dim SysWordTempA as word
;dim SysWordTempB as word
;dim SysWordTempX as word
;dim SysDivMultA as word
;dim SysDivMultB as word
;dim SysDivMultX as word
;SysDivMultA = SysWordTempA
	movf	SYSWORDTEMPA,W
	movwf	SYSDIVMULTA
	movf	SYSWORDTEMPA_H,W
	movwf	SYSDIVMULTA_H
;SysDivMultB = SysWordTempB
	movf	SYSWORDTEMPB,W
	movwf	SYSDIVMULTB
	movf	SYSWORDTEMPB_H,W
	movwf	SYSDIVMULTB_H
;SysDivMultX = 0
	clrf	SYSDIVMULTX
	clrf	SYSDIVMULTX_H
;if SysDivMultB = 0 then
	movf	SYSDIVMULTB,W
	movwf	SysWORDTempA
	movf	SYSDIVMULTB_H,W
	movwf	SysWORDTempA_H
	clrf	SysWORDTempB
	clrf	SysWORDTempB_H
	call	SysCompEqual16
	btfss	SysByteTempX,0
	goto	ENDIF13
;SysWordTempA = 0
	clrf	SYSWORDTEMPA
	clrf	SYSWORDTEMPA_H
;exit sub
	return
;end if
ENDIF13
;SysDivLoop = 16
	movlw	16
	movwf	SYSDIVLOOP
SYSDIV16START
;set C off
	bcf	STATUS,C
;Rotate SysDivMultA Left
	rlf	SYSDIVMULTA,F
	rlf	SYSDIVMULTA_H,F
;Rotate SysDivMultX Left
	rlf	SYSDIVMULTX,F
	rlf	SYSDIVMULTX_H,F
;SysDivMultX = SysDivMultX - SysDivMultB
	movf	SYSDIVMULTB,W
	subwf	SYSDIVMULTX,F
	movf	SYSDIVMULTB_H,W
	subwfb	SYSDIVMULTX_H,F
;Set SysDivMultA.0 On
	bsf	SYSDIVMULTA,0
;If C Off Then
	btfsc	STATUS,C
	goto	ENDIF14
;Set SysDivMultA.0 Off
	bcf	SYSDIVMULTA,0
;SysDivMultX = SysDivMultX + SysDivMultB
	movf	SYSDIVMULTB,W
	addwf	SYSDIVMULTX,F
	movf	SYSDIVMULTB_H,W
	addwfc	SYSDIVMULTX_H,F
;End If
ENDIF14
;decfsz SysDivLoop, F
	decfsz	SYSDIVLOOP, F
;goto SysDiv16Start
	goto	SYSDIV16START
;SysWordTempA = SysDivMultA
	movf	SYSDIVMULTA,W
	movwf	SYSWORDTEMPA
	movf	SYSDIVMULTA_H,W
	movwf	SYSWORDTEMPA_H
;SysWordTempX = SysDivMultX
	movf	SYSDIVMULTX,W
	movwf	SYSWORDTEMPX
	movf	SYSDIVMULTX_H,W
	movwf	SYSWORDTEMPX_H
	return

;********************************************************************************

SYSMULTSUB16
;dim SysWordTempA as word
;dim SysWordTempB as word
;dim SysWordTempX as word
;dim SysDivMultA as word
;dim SysDivMultB as word
;dim SysDivMultX as word
;SysDivMultA = SysWordTempA
	movf	SYSWORDTEMPA,W
	movwf	SYSDIVMULTA
	movf	SYSWORDTEMPA_H,W
	movwf	SYSDIVMULTA_H
;SysDivMultB = SysWordTempB
	movf	SYSWORDTEMPB,W
	movwf	SYSDIVMULTB
	movf	SYSWORDTEMPB_H,W
	movwf	SYSDIVMULTB_H
;SysDivMultX = 0
	clrf	SYSDIVMULTX
	clrf	SYSDIVMULTX_H
MUL16LOOP
;IF SysDivMultB.0 ON then SysDivMultX += SysDivMultA
	btfss	SYSDIVMULTB,0
	goto	ENDIF11
	movf	SYSDIVMULTA,W
	addwf	SYSDIVMULTX,F
	movf	SYSDIVMULTA_H,W
	addwfc	SYSDIVMULTX_H,F
ENDIF11
;set STATUS.C OFF
	bcf	STATUS,C
;rotate SysDivMultB right
	rrf	SYSDIVMULTB_H,F
	rrf	SYSDIVMULTB,F
;set STATUS.C off
	bcf	STATUS,C
;rotate SysDivMultA left
	rlf	SYSDIVMULTA,F
	rlf	SYSDIVMULTA_H,F
;if SysDivMultB > 0 then goto MUL16LOOP
	movf	SYSDIVMULTB,W
	movwf	SysWORDTempB
	movf	SYSDIVMULTB_H,W
	movwf	SysWORDTempB_H
	clrf	SysWORDTempA
	clrf	SysWORDTempA_H
	call	SysCompLessThan16
	btfsc	SysByteTempX,0
	goto	MUL16LOOP
;SysWordTempX = SysDivMultX
	movf	SYSDIVMULTX,W
	movwf	SYSWORDTEMPX
	movf	SYSDIVMULTX_H,W
	movwf	SYSWORDTEMPX_H
	return

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
