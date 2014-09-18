;Program compiled by Great Cow BASIC (0.9 10/9/2014)
;Need help? See the GCBASIC forums at http://sourceforge.net/projects/gcbasic/forums,
;check the documentation or email w_cholmondeley at users dot sourceforge dot net.

;********************************************************************************

;Set up the assembler options (Chip type, clock source, other bits and pieces)
 LIST p=16F877A, r=DEC
#include <P16F877A.inc>
 __CONFIG _HS_OSC & _WDT_OFF & _LVP_OFF

;********************************************************************************

;Set aside memory locations for variables
DELAYTEMP	EQU	112
DELAYTEMP2	EQU	113
SYSSTRINGA	EQU	119
SYSSTRINGA_H	EQU	120
SysBYTETempA	EQU	117
SysBYTETempB	EQU	121
SysByteTempX	EQU	112
SysDivLoop	EQU	116
SysWORDTempA	EQU	117
SysWORDTempA_H	EQU	118
SysWORDTempB	EQU	121
SysWORDTempB_H	EQU	122
SysWaitTempMS	EQU	114
SysWaitTempMS_H	EQU	115
SysWaitTempUS	EQU	117
SysWaitTempUS_H	EQU	118
CURRCHARROW	EQU	32
CURRCOL	EQU	33
CURRPAGE	EQU	34
GLCDBACKGROUND	EQU	35
GLCDBACKGROUND_H	EQU	36
GLCDBITNO	EQU	37
GLCDCHANGE	EQU	38
GLCDCOLOUR	EQU	39
GLCDCOLOUR_H	EQU	40
GLCDDATATEMP	EQU	41
GLCDFOREGROUND	EQU	42
GLCDFOREGROUND_H	EQU	43
GLCDREADBYTE	EQU	44
GLCDX	EQU	45
GLCDXPOS	EQU	46
GLCDY	EQU	47
GLCDYPOS	EQU	48
GLCD_COUNT	EQU	49
GLCD_YORDINATE	EQU	50
GLCD_YORDINATE_H	EQU	51
HCOUNT	EQU	52
LCDBYTE	EQU	53
MAXHEIGHT	EQU	54
OBJHEIGHT	EQU	55
OBJWIDTH	EQU	56
OLDGLCDXPOS	EQU	57
ONPAGEBOUNDARY	EQU	58
OPTMISEGLCDDRAW	EQU	59
SYSBITVAR0	EQU	60
SysRepeatTemp1	EQU	61
SysTemp1	EQU	62
SysTemp2	EQU	63
TABLELEN	EQU	64
TABLELEN_H	EQU	65
TABLEREADPOSITION	EQU	66
TABLEREADPOSITION_H	EQU	67
WBYTE	EQU	68
WHOLEYBYTES	EQU	69
WIDTHCOUNT	EQU	70

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
	call	INITGLCD

;Start of the main program
;Graphical BMP to GCB routines for the GCBASIC compiler
;Copyright (C) 2014 Evan Venn
;This library is free software; you can redistribute it and/or
;modify it under the terms of the GNU Lesser General Public
;License as published by the Free Software Foundation; either
;version 2.1 of the License, or (at your option) any later version.
;This library is distributed in the hope that it will be useful,
;but WITHOUT ANY WARRANTY; without even the implied warranty of
;MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
;Lesser General Public License for more details.
;You should have received a copy of the GNU Lesser General Public
;License along with this library; if not, write to the Free Software
;Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
;
;Initial test release 14092014
;You can choose two Pics... or change to your own chip.
;2024 bytes max in a table - why?
;Setup GLCD
;#define GLCD_TYPE GLCD_TYPE_KS0108
;#define GLCD_WIDTH 128
;#define GLCD_HEIGHT 64
;#define GLCD_CS1 PORTa.4'D12 to actually since CS1, CS2 can be reversed on some devices
;#define GLCD_CS2 PORTa.5
;#define GLCD_DB0 PORTD.0 'D0 to pin 7 on LCD
;#define GLCD_DB1 PORTD.1 'D1 to pin 8 on LCD
;#define GLCD_DB2 PORTD.2 'D2 to pin 9 on LCD
;#define GLCD_DB3 PORTD.3 'D3 to pin 10 on LCD
;#define GLCD_DB4 PORTD.4 'D4 to pin 11 on LCD
;#define GLCD_DB5 PORTD.5 'D5 to pin 12 on LCD
;#define GLCD_DB6 PORTD.6 'D6 to pin 13 on LCD
;#define GLCD_DB7 PORTD.7 'D7 to pin 14 on LCD
;#define GLCD_RS PORTa.0
;#define GLCD_Enable PORTA.1
;#define GLCD_RW PORTA.2
;#define GLCD_RESET PORTA.3
;#chip 16f1939, 32
;#config mclr_on, PLLEN_ON
;
;#define GLCD_TYPE GLCD_TYPE_KS0108
;#define GLCD_WIDTH 128
;#define GLCD_HEIGHT 63
;' #define GLCDFontWidth 6
;
;#define GLCD_CS1 PORTa.7'D12 to actually since CS1, CS2 can be reversed on some devices
;#define GLCD_CS2 PORTa.6
;
;#define GLCD_DB0 PORTD.0 'D0 to pin 7 on LCD
;#define GLCD_DB1 PORTD.1 'D1 to pin 8 on LCD
;#define GLCD_DB2 PORTD.2 'D2 to pin 9 on LCD
;#define GLCD_DB3 PORTD.3 'D3 to pin 10 on LCD
;#define GLCD_DB4 PORTD.4 'D4 to pin 11 on LCD
;#define GLCD_DB5 PORTD.5 'D5 to pin 12 on LCD
;#define GLCD_DB6 PORTD.6 'D6 to pin 13 on LCD
;#define GLCD_DB7 PORTD.7 'D7 to pin 14 on LCD
;
;
;#define GLCD_RS PORTa.0
;#define GLCD_Enable PORTA.1
;#define GLCD_RW PORTA.2
;#define GLCD_RESET PORTA.3
;redefine the BMP to minimise changes to code.
;#define TestDataSource Anobium
;These must be WORDs as this a large table.
;dim TableReadPosition, TableLen as word
;OptmiseGLCDDraw  = 0            ; either 0 or 1.  Needs to match the background pixel state.
	clrf	OPTMISEGLCDDRAW
;Start of code
;TableReadPosition = 1
	movlw	1
	movwf	TABLEREADPOSITION
	clrf	TABLEREADPOSITION_H
;ReadTable TestDataSource, TableReadPosition, objwidth: TableReadPosition++
	movf	TABLEREADPOSITION,W
	movwf	SysStringA
	movf	TABLEREADPOSITION_H,W
	movwf	SysStringA_H
	call	ANOBIUM
	movwf	OBJWIDTH
	incf	TABLEREADPOSITION,F
	btfsc	STATUS,Z
	incf	TABLEREADPOSITION_H,F
;ReadTable TestDataSource, TableReadPosition, objHeight: TableReadPosition++
	movf	TABLEREADPOSITION,W
	movwf	SysStringA
	movf	TABLEREADPOSITION_H,W
	movwf	SysStringA_H
	call	ANOBIUM
	movwf	OBJHEIGHT
	incf	TABLEREADPOSITION,F
	btfsc	STATUS,Z
	incf	TABLEREADPOSITION_H,F
;draw at the upper left corner is set with GLCDXPos, GLCDYPos
;GLCDXPos = 0
	clrf	GLCDXPOS
;GLCDYPos = 0
	clrf	GLCDYPOS
;oldGLCDXPos = GLCDXPos
	movf	GLCDXPOS,W
	movwf	OLDGLCDXPOS
;TableReadPosition = 3
	movlw	3
	movwf	TABLEREADPOSITION
	clrf	TABLEREADPOSITION_H
;WholeYBytes = objHeight /  8               ; Number of whole bytes within Y Axis.  This is integer maths!
	movf	OBJHEIGHT,W
	movwf	SysBYTETempA
	movlw	8
	movwf	SysBYTETempB
	call	SysDivSub
	movf	SysBYTETempA,W
	movwf	WHOLEYBYTES
;onPageBoundary = ( GLCDYPos % 8 ) = 0      ; If GLCDYPos as a memory page boundary.  Used to fast write the byte
	movf	GLCDYPOS,W
	movwf	SysBYTETempA
	movlw	8
	movwf	SysBYTETempB
	call	SysDivSub
	movf	SysBYTETempX,W
	movwf	SysTemp1
	movwf	SysBYTETempA
	clrf	SysBYTETempB
	call	SysCompEqual
	movf	SysByteTempX,W
	movwf	ONPAGEBOUNDARY
;if WholeYBytes <> 0 then
	movf	WHOLEYBYTES,F
	btfsc	STATUS, Z
	goto	ENDIF1
;for hCount = 0 to (WholeYBytes - 1)  ; counter to number of whole bytes
	movlw	255
	movwf	HCOUNT
	decf	WHOLEYBYTES,W
	movwf	SysTemp1
	clrf	SysBYTETempB
	movf	SysTemp1,W
	movwf	SysBYTETempA
	call	SysCompLessThan
	btfsc	SysByteTempX,0
	goto	SysForLoopEnd1
SysForLoop1
	incf	HCOUNT,F
;GLCDY = GLCDYPos+hCount
	movf	HCOUNT,W
	addwf	GLCDYPOS,W
	movwf	GLCDY
;for widthCount = 0 to (objwidth - 1)  ; increment thru bytes horizontally
	movlw	255
	movwf	WIDTHCOUNT
	decf	OBJWIDTH,W
	movwf	SysTemp1
	clrf	SysBYTETempB
	movf	SysTemp1,W
	movwf	SysBYTETempA
	call	SysCompLessThan
	btfsc	SysByteTempX,0
	goto	SysForLoopEnd2
SysForLoop2
	incf	WIDTHCOUNT,F
;ReadTable TestDataSource, TableReadPosition, wByte: TableReadPosition++   ; get the next byte
	movf	TABLEREADPOSITION,W
	movwf	SysStringA
	movf	TABLEREADPOSITION_H,W
	movwf	SysStringA_H
	call	ANOBIUM
	movwf	WBYTE
	incf	TABLEREADPOSITION,F
	btfsc	STATUS,Z
	incf	TABLEREADPOSITION_H,F
;if wByte <> OptmiseGLCDDraw then             ; if the incoming byte is the same as the background, dont draw
	movf	OPTMISEGLCDDRAW,W
	subwf	WBYTE,W
	btfsc	STATUS, Z
	goto	ENDIF5
;if onPageBoundary = true then
	incf	ONPAGEBOUNDARY,W
	btfss	STATUS, Z
	goto	ELSE13_1
;GLCDX = GLCDXPos+widthCount
	movf	WIDTHCOUNT,W
	addwf	GLCDXPOS,W
	movwf	GLCDX
;If GLCDX.6 = Off Then
	btfsc	GLCDX,6
	goto	ENDIF14
;Set GLCD_CS2 On
	bsf	PORTA,5
;Set GLCD_CS1 off
	bcf	PORTA,4
;end if
ENDIF14
;If GLCDX.6 = On Then
	btfss	GLCDX,6
	goto	ENDIF15
;Set GLCD_CS1 On
	bsf	PORTA,4
;GLCDX -= 64
	movlw	64
	subwf	GLCDX,F
;Set GLCD_CS2 off
	bcf	PORTA,5
;end if
ENDIF15
;CurrPage = GLCDY / 8
	movf	GLCDY,W
	movwf	SysBYTETempA
	movlw	8
	movwf	SysBYTETempB
	call	SysDivSub
	movf	SysBYTETempA,W
	movwf	CURRPAGE
;Set GLCD_RS Off
	bcf	PORTA,0
;GLCDWriteByte b'10111000' Or CurrPage
	movlw	184
	iorwf	CURRPAGE,W
	movwf	LCDBYTE
	call	GLCDWRITEBYTE
;GLCDWriteByte 64 Or GLCDX
	movlw	64
	iorwf	GLCDX,W
	movwf	LCDBYTE
	call	GLCDWRITEBYTE
;Set GLCD_RS On
	bsf	PORTA,0
;GLCDWriteByte wByte
	movf	WBYTE,W
	movwf	LCDBYTE
	call	GLCDWRITEBYTE
;else
	goto	ENDIF13
ELSE13_1
;For CurrCharRow = 0 to 7
	movlw	255
	movwf	CURRCHARROW
SysForLoop3
	incf	CURRCHARROW,F
;If wByte.0 = 0 Then
	btfsc	WBYTE,0
	goto	ELSE16_1
;PSet GLCDXPos+widthCount, CurrCharRow + GLCDYPos, GLCDBackground
	movf	WIDTHCOUNT,W
	addwf	GLCDXPOS,W
	movwf	GLCDX
	movf	GLCDYPOS,W
	addwf	CURRCHARROW,W
	movwf	GLCDY
	movf	GLCDBACKGROUND,W
	movwf	GLCDCOLOUR
	movf	GLCDBACKGROUND_H,W
	movwf	GLCDCOLOUR_H
	call	PSET
;Else
	goto	ENDIF16
ELSE16_1
;PSet GLCDXPos+widthCount, CurrCharRow + GLCDYPos, GLCDForeground
	movf	WIDTHCOUNT,W
	addwf	GLCDXPOS,W
	movwf	GLCDX
	movf	GLCDYPOS,W
	addwf	CURRCHARROW,W
	movwf	GLCDY
	movf	GLCDFOREGROUND,W
	movwf	GLCDCOLOUR
	movf	GLCDFOREGROUND_H,W
	movwf	GLCDCOLOUR_H
	call	PSET
;End If
ENDIF16
;Rotate wByte Right
	rrf	WBYTE,F
;Next
	movlw	7
	subwf	CURRCHARROW,W
	btfss	STATUS, C
	goto	SysForLoop3
SysForLoopEnd3
;end if
ENDIF13
;end if
ENDIF5
;next widthCount
	decf	OBJWIDTH,W
	movwf	SysTemp1
	movf	WIDTHCOUNT,W
	movwf	SysBYTETempA
	movf	SysTemp1,W
	movwf	SysBYTETempB
	call	SysCompLessThan
	btfsc	SysByteTempX,0
	goto	SysForLoop2
SysForLoopEnd2
;GLCDXPos =  oldGLCDXPos
	movf	OLDGLCDXPOS,W
	movwf	GLCDXPOS
;GLCDYPos = GLCDYPos + 8
	movlw	8
	addwf	GLCDYPOS,F
;next hCount
	decf	WHOLEYBYTES,W
	movwf	SysTemp1
	movf	HCOUNT,W
	movwf	SysBYTETempA
	movf	SysTemp1,W
	movwf	SysBYTETempB
	call	SysCompLessThan
	btfsc	SysByteTempX,0
	goto	SysForLoop1
SysForLoopEnd1
;end if
ENDIF1
;maxHeight = objHeight % 8       ; calculate the remaining bits
	movf	OBJHEIGHT,W
	movwf	SysBYTETempA
	movlw	8
	movwf	SysBYTETempB
	call	SysDivSub
	movf	SysBYTETempX,W
	movwf	MAXHEIGHT
;if maxHeight <> 0 then
	movf	MAXHEIGHT,F
	btfsc	STATUS, Z
	goto	ENDIF2
;for widthCount = 0 to objwidth - 1
	movlw	255
	movwf	WIDTHCOUNT
	decf	OBJWIDTH,W
	movwf	SysTemp1
	clrf	SysBYTETempB
	movf	SysTemp1,W
	movwf	SysBYTETempA
	call	SysCompLessThan
	btfsc	SysByteTempX,0
	goto	SysForLoopEnd4
SysForLoop4
	incf	WIDTHCOUNT,F
;ReadTable TestDataSource, TableReadPosition, wByte: TableReadPosition++
	movf	TABLEREADPOSITION,W
	movwf	SysStringA
	movf	TABLEREADPOSITION_H,W
	movwf	SysStringA_H
	call	ANOBIUM
	movwf	WBYTE
	incf	TABLEREADPOSITION,F
	btfsc	STATUS,Z
	incf	TABLEREADPOSITION_H,F
;For CurrCharRow = 0 to maxHeight-1
	movlw	255
	movwf	CURRCHARROW
	decf	MAXHEIGHT,W
	movwf	SysTemp1
	clrf	SysBYTETempB
	movf	SysTemp1,W
	movwf	SysBYTETempA
	call	SysCompLessThan
	btfsc	SysByteTempX,0
	goto	SysForLoopEnd5
SysForLoop5
	incf	CURRCHARROW,F
;If wByte.0 = 0 Then
	btfsc	WBYTE,0
	goto	ELSE10_1
;PSet GLCDXPos+widthCount, ( CurrCharRow + GLCDYPos ), GLCDBackground
	movf	WIDTHCOUNT,W
	addwf	GLCDXPOS,W
	movwf	GLCDX
	movf	GLCDYPOS,W
	addwf	CURRCHARROW,W
	movwf	GLCDY
	movf	GLCDBACKGROUND,W
	movwf	GLCDCOLOUR
	movf	GLCDBACKGROUND_H,W
	movwf	GLCDCOLOUR_H
	call	PSET
;Else
	goto	ENDIF10
ELSE10_1
;PSet GLCDXPos+widthCount, ( CurrCharRow + GLCDYPos ), GLCDForeground
	movf	WIDTHCOUNT,W
	addwf	GLCDXPOS,W
	movwf	GLCDX
	movf	GLCDYPOS,W
	addwf	CURRCHARROW,W
	movwf	GLCDY
	movf	GLCDFOREGROUND,W
	movwf	GLCDCOLOUR
	movf	GLCDFOREGROUND_H,W
	movwf	GLCDCOLOUR_H
	call	PSET
;End If
ENDIF10
;Rotate wByte Right
	rrf	WBYTE,F
;Next
	decf	MAXHEIGHT,W
	movwf	SysTemp1
	movf	CURRCHARROW,W
	movwf	SysBYTETempA
	movf	SysTemp1,W
	movwf	SysBYTETempB
	call	SysCompLessThan
	btfsc	SysByteTempX,0
	goto	SysForLoop5
SysForLoopEnd5
;next
	decf	OBJWIDTH,W
	movwf	SysTemp1
	movf	WIDTHCOUNT,W
	movwf	SysBYTETempA
	movf	SysTemp1,W
	movwf	SysBYTETempB
	call	SysCompLessThan
	btfsc	SysByteTempX,0
	goto	SysForLoop4
SysForLoopEnd4
;end if
ENDIF2
;Set GLCD_CS1 Off
	bcf	PORTA,4
;Set GLCD_CS2 Off
	bcf	PORTA,5
BASPROGRAMEND
	sleep
	goto	BASPROGRAMEND

;********************************************************************************

ANOBIUM
	movf	SYSSTRINGA,W
	movwf	SysWORDTempA
	movf	SYSSTRINGA_H,W
	movwf	SysWORDTempA_H
	movlw	227
	movwf	SysWORDTempB
	movlw	1
	movwf	SysWORDTempB_H
	call	SysCompLessThan16
	btfss	SysByteTempX,0
	retlw	0
	movf	SysStringA, W
	addlw	low (1 + TableANOBIUM)
	movwf	SysStringA
	movlw	high TableANOBIUM
	btfsc	STATUS, C
	addlw	1
	addwf	SysStringA_H, W
	movwf	PCLATH
	movf	SysStringA, W
TableANOBIUM
	movwf	PCL
	retlw	226
	retlw	60
	retlw	64
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	128
	retlw	224
	retlw	0
	retlw	192
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	64
	retlw	192
	retlw	176
	retlw	96
	retlw	192
	retlw	128
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	192
	retlw	112
	retlw	222
	retlw	115
	retlw	28
	retlw	7
	retlw	1
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	1
	retlw	1
	retlw	3
	retlw	6
	retlw	13
	retlw	27
	retlw	62
	retlw	236
	retlw	24
	retlw	16
	retlw	16
	retlw	16
	retlw	24
	retlw	24
	retlw	8
	retlw	8
	retlw	12
	retlw	12
	retlw	12
	retlw	4
	retlw	4
	retlw	4
	retlw	6
	retlw	6
	retlw	2
	retlw	2
	retlw	2
	retlw	3
	retlw	3
	retlw	1
	retlw	1
	retlw	3
	retlw	15
	retlw	28
	retlw	112
	retlw	224
	retlw	128
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	255
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	1
	retlw	7
	retlw	14
	retlw	56
	retlw	112
	retlw	192
	retlw	128
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	255
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	128
	retlw	192
	retlw	99
	retlw	54
	retlw	28
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	7
	retlw	12
	retlw	8
	retlw	24
	retlw	16
	retlw	48
	retlw	48
	retlw	32
	retlw	96
	retlw	64
	retlw	192
	retlw	192
	retlw	128
	retlw	128
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	128
	retlw	192
	retlw	112
	retlw	56
	retlw	28
	retlw	14
	retlw	3
	retlw	1
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	192
	retlw	32
	retlw	192
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	1
	retlw	1
	retlw	3
	retlw	3
	retlw	2
	retlw	6
	retlw	4
	retlw	12
	retlw	236
	retlw	8
	retlw	12
	retlw	6
	retlw	3
	retlw	1
	retlw	0
	retlw	0
	retlw	32
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	0
	retlw	192
	retlw	56
	retlw	22
	retlw	17
	retlw	16
	retlw	17
	retlw	22
	retlw	56
	retlw	192
	retlw	0
	retlw	255
	retlw	2
	retlw	1
	retlw	1
	retlw	1
	retlw	254
	retlw	0
	retlw	0
	retlw	126
	retlw	129
	retlw	129
	retlw	129
	retlw	129
	retlw	126
	retlw	0
	retlw	0
	retlw	255
	retlw	66
	retlw	129
	retlw	129
	retlw	129
	retlw	126
	retlw	0
	retlw	0
	retlw	255
	retlw	0
	retlw	0
	retlw	127
	retlw	128
	retlw	128
	retlw	128
	retlw	64
	retlw	255
	retlw	0
	retlw	0
	retlw	255
	retlw	2
	retlw	1
	retlw	1
	retlw	1
	retlw	254
	retlw	2
	retlw	1
	retlw	1
	retlw	1
	retlw	254

;********************************************************************************

ANOBIUM_H
	movf	SYSSTRINGA,W
	movwf	SysWORDTempA
	movf	SYSSTRINGA_H,W
	movwf	SysWORDTempA_H
	movlw	1
	movwf	SysWORDTempB
	clrf	SysWORDTempB_H
	call	SysCompLessThan16
	btfss	SysByteTempX,0
	retlw	0
	retlw	1

;********************************************************************************

Delay_MS
	incf	SysWaitTempMS_H, F
DMS_START
	movlw	227
	movwf	DELAYTEMP2
DMS_OUTER
	movlw	6
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

GLCDCLS
;GLCD_yordinate = 0
	clrf	GLCD_YORDINATE
	clrf	GLCD_YORDINATE_H
;Set GLCD_CS1 On
	bsf	PORTA,4
;Set GLCD_CS2 Off
	bcf	PORTA,5
;for GLCD_Count = 1 to 2
	clrf	GLCD_COUNT
SysForLoop6
	incf	GLCD_COUNT,F
;For CurrPage = 0 to 7
	movlw	255
	movwf	CURRPAGE
SysForLoop7
	incf	CURRPAGE,F
;Set GLCD_RS Off
	bcf	PORTA,0
;GLCDWriteByte b'10111000' Or CurrPage
	movlw	184
	iorwf	CURRPAGE,W
	movwf	LCDBYTE
	call	GLCDWRITEBYTE
;For CurrCol = 0 to 63
	movlw	255
	movwf	CURRCOL
SysForLoop8
	incf	CURRCOL,F
;Set GLCD_RS Off
	bcf	PORTA,0
;GLCDWriteByte 64 Or CurrCol
	movlw	64
	iorwf	CURRCOL,W
	movwf	LCDBYTE
	call	GLCDWRITEBYTE
;Set GLCD_RS On
	bsf	PORTA,0
;GLCDWriteByte 0
	clrf	LCDBYTE
	call	GLCDWRITEBYTE
;Next
	movlw	63
	subwf	CURRCOL,W
	btfss	STATUS, C
	goto	SysForLoop8
SysForLoopEnd8
;Next
	movlw	7
	subwf	CURRPAGE,W
	btfss	STATUS, C
	goto	SysForLoop7
SysForLoopEnd7
;Set GLCD_CS1 Off
	bcf	PORTA,4
;Set GLCD_CS2 On
	bsf	PORTA,5
;next
	movlw	2
	subwf	GLCD_COUNT,W
	btfss	STATUS, C
	goto	SysForLoop6
SysForLoopEnd6
;Set GLCD_CS1 OFF
	bcf	PORTA,4
;Set GLCD_CS2 Off
	bcf	PORTA,5
	return

;********************************************************************************

FN_GLCDREADBYTE
;Dir GLCD_DB7 In
	banksel	TRISD
	bsf	TRISD,7
;Dir GLCD_DB6 In
	bsf	TRISD,6
;Dir GLCD_DB5 In
	bsf	TRISD,5
;Dir GLCD_DB4 In
	bsf	TRISD,4
;Dir GLCD_DB3 In
	bsf	TRISD,3
;Dir GLCD_DB2 In
	bsf	TRISD,2
;Dir GLCD_DB1 In
	bsf	TRISD,1
;Dir GLCD_DB0 In
	bsf	TRISD,0
;Set GLCD_RW On
	banksel	PORTA
	bsf	PORTA,2
;Set GLCD_ENABLE On
	bsf	PORTA,1
;Wait 2 us
	movlw	3
	movwf	DELAYTEMP
DelayUS4
	decfsz	DELAYTEMP,F
	goto	DelayUS4
;GLCDReadByte.7 = GLCD_DB7
	bcf	GLCDREADBYTE,7
	btfsc	PORTD,7
	bsf	GLCDREADBYTE,7
;GLCDReadByte.6 = GLCD_DB6
	bcf	GLCDREADBYTE,6
	btfsc	PORTD,6
	bsf	GLCDREADBYTE,6
;GLCDReadByte.5 = GLCD_DB5
	bcf	GLCDREADBYTE,5
	btfsc	PORTD,5
	bsf	GLCDREADBYTE,5
;GLCDReadByte.4 = GLCD_DB4
	bcf	GLCDREADBYTE,4
	btfsc	PORTD,4
	bsf	GLCDREADBYTE,4
;GLCDReadByte.3 = GLCD_DB3
	bcf	GLCDREADBYTE,3
	btfsc	PORTD,3
	bsf	GLCDREADBYTE,3
;GLCDReadByte.2 = GLCD_DB2
	bcf	GLCDREADBYTE,2
	btfsc	PORTD,2
	bsf	GLCDREADBYTE,2
;GLCDReadByte.1 = GLCD_DB1
	bcf	GLCDREADBYTE,1
	btfsc	PORTD,1
	bsf	GLCDREADBYTE,1
;GLCDReadByte.0 = GLCD_DB0
	bcf	GLCDREADBYTE,0
	btfsc	PORTD,0
	bsf	GLCDREADBYTE,0
;Set GLCD_ENABLE Off
	bcf	PORTA,1
;Wait 2 us
	movlw	3
	movwf	DELAYTEMP
DelayUS5
	decfsz	DELAYTEMP,F
	goto	DelayUS5
	return

;********************************************************************************

GLCDWRITEBYTE
;Dim GLCDSaveRS As Bit
;Dim GLCDSaveCS2 As Bit
;GLCDSaveRS = GLCD_RS
	bcf	SYSBITVAR0,0
	btfsc	PORTA,0
	bsf	SYSBITVAR0,0
;GLCDSaveCS2 = GLCD_CS2
	bcf	SYSBITVAR0,1
	btfsc	PORTA,5
	bsf	SYSBITVAR0,1
;If GLCD_CS1 = 1 Then
	btfsc	PORTA,4
;GLCD_CS2 = 0
	bcf	PORTA,5
;End If
;Set GLCD_RS Off
	bcf	PORTA,0
;Wait Until GLCDReadByte.7 = Off
SysWaitLoop1
	call	FN_GLCDREADBYTE
	btfsc	GLCDREADBYTE,7
	goto	SysWaitLoop1
;GLCD_RS = GLCDSaveRS
	bcf	PORTA,0
	btfsc	SYSBITVAR0,0
	bsf	PORTA,0
;GLCD_CS2 = GLCDSaveCS2
	bcf	PORTA,5
	btfsc	SYSBITVAR0,1
	bsf	PORTA,5
;Set GLCD_RW Off
	bcf	PORTA,2
;DIR GLCD_DB0 OUT
	banksel	TRISD
	bcf	TRISD,0
;DIR GLCD_DB1 OUT
	bcf	TRISD,1
;DIR GLCD_DB2 OUT
	bcf	TRISD,2
;DIR GLCD_DB3 OUT
	bcf	TRISD,3
;DIR GLCD_DB4 OUT
	bcf	TRISD,4
;DIR GLCD_DB5 OUT
	bcf	TRISD,5
;DIR GLCD_DB6 OUT
	bcf	TRISD,6
;DIR GLCD_DB7 OUT
	bcf	TRISD,7
;GLCD_DB7 = LCDByte.7
	banksel	PORTD
	bcf	PORTD,7
	btfsc	LCDBYTE,7
	bsf	PORTD,7
;GLCD_DB6 = LCDByte.6
	bcf	PORTD,6
	btfsc	LCDBYTE,6
	bsf	PORTD,6
;GLCD_DB5 = LCDByte.5
	bcf	PORTD,5
	btfsc	LCDBYTE,5
	bsf	PORTD,5
;GLCD_DB4 = LCDByte.4
	bcf	PORTD,4
	btfsc	LCDBYTE,4
	bsf	PORTD,4
;GLCD_DB3 = LCDByte.3
	bcf	PORTD,3
	btfsc	LCDBYTE,3
	bsf	PORTD,3
;GLCD_DB2 = LCDByte.2
	bcf	PORTD,2
	btfsc	LCDBYTE,2
	bsf	PORTD,2
;GLCD_DB1 = LCDByte.1
	bcf	PORTD,1
	btfsc	LCDBYTE,1
	bsf	PORTD,1
;GLCD_DB0 = LCDByte.0
	bcf	PORTD,0
	btfsc	LCDBYTE,0
	bsf	PORTD,0
;Wait 2 us
	movlw	3
	movwf	DELAYTEMP
DelayUS1
	decfsz	DELAYTEMP,F
	goto	DelayUS1
;Set GLCD_ENABLE On
	bsf	PORTA,1
;Wait 2 us
	movlw	3
	movwf	DELAYTEMP
DelayUS2
	decfsz	DELAYTEMP,F
	goto	DelayUS2
;Set GLCD_ENABLE Off
	bcf	PORTA,1
;Wait 2 us
	movlw	3
	movwf	DELAYTEMP
DelayUS3
	decfsz	DELAYTEMP,F
	goto	DelayUS3
	return

;********************************************************************************

INITGLCD
;Dir GLCD_RS Out
	banksel	TRISA
	bcf	TRISA,0
;Dir GLCD_RW Out
	bcf	TRISA,2
;Dir GLCD_ENABLE Out
	bcf	TRISA,1
;Dir GLCD_CS1 Out
	bcf	TRISA,4
;Dir GLCD_CS2 Out
	bcf	TRISA,5
;Dir GLCD_RESET Out
	bcf	TRISA,3
;Set GLCD_RESET Off
	banksel	PORTA
	bcf	PORTA,3
;Wait 1 ms
	movlw	1
	movwf	SysWaitTempMS
	clrf	SysWaitTempMS_H
	call	Delay_MS
;Set GLCD_RESET On
	bsf	PORTA,3
;Wait 1 ms
	movlw	1
	movwf	SysWaitTempMS
	clrf	SysWaitTempMS_H
	call	Delay_MS
;Set GLCD_CS1 On
	bsf	PORTA,4
;Set GLCD_CS2 On
	bsf	PORTA,5
;Set GLCD_RS Off
	bcf	PORTA,0
;GLCDWriteByte 63
	movlw	63
	movwf	LCDBYTE
	call	GLCDWRITEBYTE
;GLCDWriteByte 192
	movlw	192
	movwf	LCDBYTE
	call	GLCDWRITEBYTE
;Set GLCD_CS1 Off
	bcf	PORTA,4
;Set GLCD_CS2 Off
	bcf	PORTA,5
;GLCDBackground = 0
	clrf	GLCDBACKGROUND
	clrf	GLCDBACKGROUND_H
;GLCDForeground = 1
	movlw	1
	movwf	GLCDFOREGROUND
	clrf	GLCDFOREGROUND_H
;GLCDCLS
	goto	GLCDCLS

;********************************************************************************

INITSYS
;SET ADCON0.ADON OFF
	bcf	ADCON0,ADON
;SET ADFM OFF
	banksel	ADCON1
	bcf	ADCON1,ADFM
;SET PCFG3 OFF
	bcf	ADCON1,PCFG3
;SET PCFG2 ON
	bsf	ADCON1,PCFG2
;SET PCFG1 ON
	bsf	ADCON1,PCFG1
;SET PCFG0 OFF
	bcf	ADCON1,PCFG0
;CMCON = 7
	movlw	7
	movwf	CMCON
;PORTA = 0
	banksel	PORTA
	clrf	PORTA
;PORTB = 0
	clrf	PORTB
;PORTC = 0
	clrf	PORTC
;PORTD = 0
	clrf	PORTD
;PORTE = 0
	clrf	PORTE
	return

;********************************************************************************

PSET
;If GLCDX.6 = Off Then Set GLCD_CS2 On:Set GLCD_CS1 off
	btfsc	GLCDX,6
	goto	ENDIF18
	bsf	PORTA,5
	bcf	PORTA,4
ENDIF18
;If GLCDX.6 = On Then Set GLCD_CS1 On: GLCDX -= 64: Set GLCD_CS2 off
	btfss	GLCDX,6
	goto	ENDIF19
	bsf	PORTA,4
	movlw	64
	subwf	GLCDX,F
	bcf	PORTA,5
ENDIF19
;CurrPage = GLCDY / 8
	movf	GLCDY,W
	movwf	SysBYTETempA
	movlw	8
	movwf	SysBYTETempB
	call	SysDivSub
	movf	SysBYTETempA,W
	movwf	CURRPAGE
;Set GLCD_RS Off
	bcf	PORTA,0
;GLCDWriteByte b'10111000' Or CurrPage
	movlw	184
	iorwf	CURRPAGE,W
	movwf	LCDBYTE
	call	GLCDWRITEBYTE
;Set GLCD_RS Off
	bcf	PORTA,0
;GLCDWriteByte 64 Or GLCDX
	movlw	64
	iorwf	GLCDX,W
	movwf	LCDBYTE
	call	GLCDWRITEBYTE
;Set GLCD_RS On
	bsf	PORTA,0
;GLCDDataTemp = GLCDReadByte
	call	FN_GLCDREADBYTE
	movf	GLCDREADBYTE,W
	movwf	GLCDDATATEMP
;Set GLCD_RS On
	bsf	PORTA,0
;GLCDDataTemp = GLCDReadByte
	call	FN_GLCDREADBYTE
	movf	GLCDREADBYTE,W
	movwf	GLCDDATATEMP
;GLCDBitNo = GLCDY And 7
	movlw	7
	andwf	GLCDY,W
	movwf	GLCDBITNO
;If GLCDColour.0 = 0 Then
	btfsc	GLCDCOLOUR,0
	goto	ELSE20_1
;GLCDChange = 254
	movlw	254
	movwf	GLCDCHANGE
;Set C On
	bsf	STATUS,C
;Else
	goto	ENDIF20
ELSE20_1
;GLCDChange = 1
	movlw	1
	movwf	GLCDCHANGE
;Set C Off
	bcf	STATUS,C
;End If
ENDIF20
;Repeat GLCDBitNo
	movf	GLCDBITNO,W
	movwf	SysRepeatTemp1
	btfsc	STATUS,Z
	goto	SysRepeatLoopEnd1
SysRepeatLoop1
;Rotate GLCDChange Left
	rlf	GLCDCHANGE,F
;End Repeat
	decfsz	SysRepeatTemp1,F
	goto	SysRepeatLoop1
SysRepeatLoopEnd1
;If GLCDColour.0 = 0 Then
	btfsc	GLCDCOLOUR,0
	goto	ELSE21_1
;GLCDDataTemp = GLCDDataTemp And GLCDChange
	movf	GLCDDATATEMP,W
	andwf	GLCDCHANGE,W
	movwf	GLCDDATATEMP
;Else
	goto	ENDIF21
ELSE21_1
;GLCDDataTemp = GLCDDataTemp Or GLCDChange
	movf	GLCDDATATEMP,W
	iorwf	GLCDCHANGE,W
	movwf	GLCDDATATEMP
;End If
ENDIF21
;Set GLCD_RS Off
	bcf	PORTA,0
;GLCDWriteByte 64 Or GLCDX
	movlw	64
	iorwf	GLCDX,W
	movwf	LCDBYTE
	call	GLCDWRITEBYTE
;Set GLCD_RS On
	bsf	PORTA,0
;GLCDWriteByte GLCDDataTemp
	movf	GLCDDATATEMP,W
	movwf	LCDBYTE
	call	GLCDWRITEBYTE
;Set GLCD_CS1 Off
	bcf	PORTA,4
;Set GLCD_CS2 Off
	bcf	PORTA,5
	return

;********************************************************************************

SYSCOMPEQUAL
;Dim SysByteTempA, SysByteTempB, SysByteTempX as byte
;clrf SysByteTempX
	clrf	SYSBYTETEMPX
;movf SysByteTempA, W
	movf	SYSBYTETEMPA, W
;subwf SysByteTempB, W
	subwf	SYSBYTETEMPB, W
;btfsc STATUS, Z
	btfsc	STATUS, Z
;comf SysByteTempX,F
	comf	SYSBYTETEMPX,F
	return

;********************************************************************************

SYSCOMPLESSTHAN
;Dim SysByteTempA, SysByteTempB, SysByteTempX as byte
;clrf SysByteTempX
	clrf	SYSBYTETEMPX
;bsf STATUS, C
	bsf	STATUS, C
;movf SysByteTempB, W
	movf	SYSBYTETEMPB, W
;subwf SysByteTempA, W
	subwf	SYSBYTETEMPA, W
;btfss STATUS, C
	btfss	STATUS, C
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

SYSDIVSUB
;dim SysByteTempA as byte
;dim SysByteTempB as byte
;dim SysByteTempX as byte
;SysByteTempX = 0
	clrf	SYSBYTETEMPX
;SysDivLoop = 8
	movlw	8
	movwf	SYSDIVLOOP
SYSDIV8START
;bcf STATUS, C
	bcf	STATUS, C
;rlf SysByteTempA, F
	rlf	SYSBYTETEMPA, F
;rlf SysByteTempX, F
	rlf	SYSBYTETEMPX, F
;movf SysByteTempB, W
	movf	SYSBYTETEMPB, W
;subwf SysByteTempX, F
	subwf	SYSBYTETEMPX, F
;bsf SysByteTempA, 0
	bsf	SYSBYTETEMPA, 0
;btfsc STATUS, C
	btfsc	STATUS, C
;goto Div8NotNeg
	goto	DIV8NOTNEG
;bcf SysByteTempA, 0
	bcf	SYSBYTETEMPA, 0
;movf SysByteTempB, W
	movf	SYSBYTETEMPB, W
;addwf SysByteTempX, F
	addwf	SYSBYTETEMPX, F
DIV8NOTNEG
;decfsz SysDivLoop, F
	decfsz	SYSDIVLOOP, F
;goto SysDiv8Start
	goto	SYSDIV8START
	return

;********************************************************************************

;Start of program memory page 1
	ORG	2048
;Start of program memory page 2
	ORG	4096
;Start of program memory page 3
	ORG	6144

 END
