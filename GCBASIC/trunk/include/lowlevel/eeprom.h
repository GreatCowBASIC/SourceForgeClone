'    EEPROM routines for Great Cow BASIC
'    Copyright (C) 2006 - 2017 Hugh Considine, William Roth and Evan R. Venn

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
' 3/5/2006: Program EEPROM read/write added
' 16/5/2006: Write mode disabled at end of EPWrite
' 23/5/2006: EPWrite and EPRead changed to stop use of temporary vars
' 19/8/2006: Program EEPROM altered to take input as word
' 9/2/2007: Bug fix: EEPGD bit not set if not present
' 5/8/2007: Altered to use alias to access address, data vars
' 4/9/2007: Data EEPROM code altered to support AVR
' 15/10/2007: Bug fix: FREE bit not set if not present, altered to use IntOn/IntOff to control interrupt
' 4/12/2007: Added In/Out to parameters
' 17/6/2008: Altered to make EPRead a system sub, to allow use in ReadTable
' 19/4/2009: Bugfixes for AVR, allow EEPE instead of EEWE
' 4/3/2013: Corrections for PIC16F1847 (EEDAT instead of EEDATA)
' 23/2/2016: Use EEDATL instead of EEDATA (16F1825, possibly others)
' 16/3/16: Added supported for NVMADRH:NVMADRL
' 10/9/17: Removed depencies on Register.Bits - just use Bits. This prevents variables being created. And, change Script from NVMADRH to NVMADRL test.
' 21/10/18: Added EEPROM support for 18fxxq10 the   'Select DATA EE section (0x310000 - 0x3100FF) section
' 20/02/19: Revised bit check to ensure bit exists in ProgWrite - Bit(EEPGD)
' 20/04/19: Revised bit check to ensure bit exists in EPWrite - NVMEN and added cleared NVCON1
'           Set EEDATL_REF to whatever it is actually called (EEDAT, EEDATA or EEDATL)
' 23/09/19: Revised to handle K40 Per  Chip Errata Sheets to correctly support table reads on specific chips.

#script
  If Var(EEDATA) Then
    EEDATL_REF = EEDATA
  End If

  If Var(EEDAT) Then
    EEDATL_REF = EEDAT
  End If

  If Var(EEDATL) Then
    EEDATL_REF = EEDATL
  End If

  'Change from NVMADRH test as NVMADRH does not exist on every chip. The 18F25K42 for instance. 10/9/17
  If var(NVMADRL) Then
    EPWrite = NVMADR_EPWrite
    EPRead  = NVMADR_EPRead
  End If


#endscript

sub EPWrite(In EEAddress, In EEDataValue)

#IFDEF PIC

  'Variable alias
  #IFNDEF Var(EEADRH)
    Dim EEAddress Alias EEADR  'erv
  #ENDIF

  #IFDEF Var(EEADRH)
    Dim EEAddress As Word Alias EEADRH, EEADR
  #ENDIF

  #ifdef EEDATL_REF
    Dim EEDataValue Alias EEDATL_REF
  #endif

  'Disable interrupt
  IntOff

  'Select data memory
  #IFDEF Bit(EEPGD)
    SET EEPGD OFF
  #ENDIF

 #IFDEF Bit(CFGS)
    Set CFGS OFF
  #ENDIF

  'Start write
  SET WREN ON
  EECON2 = 0x55
  EECON2 = 0xAA
  SET WR ON
  SET WREN OFF

  'Wait for write to complete
  WAIT WHILE WR ON
  SET WREN OFF

  'Restore interrupt
  IntOn

#ENDIF

#IFDEF AVR
  'Variable alias
  #IFDEF Var(EEARH)
    Dim EEAddress As Word Alias EEARH, EEARL
  #ENDIF
  #IFNDEF Var(EEARH)
    #IFDEF Var(EEAR)
      Dim EEAddress Alias EEAR
    #ENDIF
  #ENDIF
  Dim EEDataValue Alias EEDR

  'Enable write
  #IFDEF Bit(EEMWE)
    Set EEMWE On
  #ENDIF
  #IFNDEF Bit(EEMWE)
    #IFDEF Bit(EEMPE)
      Set EEMPE On
    #ENDIF
  #ENDIF
  'Start write, wait for it to complete
  #IFDEF Bit(EEWE)
    Set EEWE On
    Wait Until EEWE Off
  #ENDIF
  #IFNDEF Bit(EEWE)
    #IFDEF Bit(EEPE)
      Set EEPE On
      Wait Until EEPE Off
    #ENDIF
  #ENDIF

#ENDIF

end sub

'EPRead made into system subroutine
'Constant added to allow it to still be referred to as EPRead
#define EPRead SysEPRead
sub SysEPRead(In EEAddress, Out EEDataValue)

#IFDEF PIC

  'Variable alias
  #IFNDEF Var(EEADRH)
    Dim EEAddress Alias EEADR
  #ENDIF
  #IFDEF Var(EEADRH)
    Dim EEAddress As Word Alias EEADRH, EEADR
  #ENDIF
  #ifdef EEDATL_REF
    Dim EEDataValue Alias EEDATL_REF
  #endif

  'Disable interrupt
  IntOff

  'Select data memory
  #IFDEF Bit(EEPGD)
    SET EEPGD OFF
  #ENDIF
  #IFDEF Bit(CFGS)
    Set CFGS OFF
  #ENDIF

  'Read
  SET RD ON

  #IFDEF Oneof(CHIP_18F24K40,CHIP_18F25K40,CHIP_18F26K40,CHIP_18F27K40,CHIP_18F45K40,CHIP_18F46K40,CHIP_18F47K40,CHIP_18F65K40,CHIP_18F66K40,CHIP_18LF24K40, CHIP_18LF25K40, CHIP_18LF26K40, CHIP_18LF27K40, CHIP_18LF45K40, CHIP_18LF46K40, CHIP_18LF47K40, CHIP_18LF65K40, CHIP_18LF66K40)
     ' Added (Per  Chip Errata Sheets) to correctly support table reads on specific chips)
     ' Sets NVRAM pointer to Static RAM as default location.
      #Ifdef Var(NVMCON1)
         NVMCON1.7 = 1
         NVMCON1.6 = 0
      #endif
  #ENDIF


  'Restore interrupt
  IntOn
#ENDIF

#IFDEF AVR
  'Variable alias
  #IFDEF Var(EEARH)
    Dim EEAddress As Word Alias EEARH, EEARL
  #ENDIF
  #IFNDEF Var(EEARH)
    #IFDEF Var(EEAR)
      Dim EEAddress Alias EEAR
    #ENDIF
  #ENDIF
  Dim EEDataValue Alias EEDR

  'Start read
  Set EERE On



#ENDIF

end sub

function ReadEP(EEAddress)

#IFDEF PIC
  'Variable alias
  #IFNDEF Var(EEADRH)
    Dim EEAddress Alias EEADR
  #ENDIF
  #IFDEF Var(EEADRH)
    Dim EEAddress As Word Alias EEADRH, EEADR
  #ENDIF
  #ifdef EEDATL_REF
    Dim EEDataValue Alias EEDATL_REF
  #endif

  'Disable interrupt
  IntOff

  'Select data memory
  #IFDEF Bit(EEPGD)
    SET EEPGD OFF
  #ENDIF
  #IFDEF Bit(CFGS)
    Set CFGS OFF
  #ENDIF

  'Read
  SET RD ON

  'Restore interrupt
  IntOn
#ENDIF

#IFDEF AVR
  'Variable alias
  #IFDEF Var(EEARH)
    Dim EEAddress As Word Alias EEARH, EEARL
  #ENDIF
  #IFNDEF Var(EEARH)
    #IFDEF Var(EEAR)
      Dim EEAddress Alias EEAR
    #ENDIF
  #ENDIF
  Dim EEDataValue Alias EEDR

  'Start read
  Set EERE On
#ENDIF

end function

sub ProgramWrite(In EEAddress, In EEDataWord)

#IFDEF PIC


  Dim EEAddress As Word Alias EEADRH, EEADR
  Dim EEDataWord As Word Alias EEDATH, EEDATL_REF

  'Disable Interrupt
  IntOff

  'Select program memory
  #IFDEF Bit(EEPGD)
    SET EEPGD OFF
  #ENDIF  SET EEPGD ON
  #IFDEF Bit(CFGS)
    Set CFGS OFF
  #ENDIF

  'Enable write
  SET WREN ON
  #ifdef bit(FREE)
    SET FREE OFF
  #endif

  'Write enable code
  EECON2 = 0x55
  EECON2 = 0xAA

  'Start write, wait for it to finish
  SET WR ON
  NOP
  NOP
  SET WREN OFF

  'Enable Interrupt
  IntOn
#ENDIF

end sub

sub ProgramRead(In EEAddress, Out EEDataWord)
  Dim EEAddress As Word Alias EEADRH, EEADR
  Dim EEDataWord As Word Alias EEDATH, EEDATL_REF

  'Disable Interrupt
  IntOff

  'Select program memory
  SET EEPGD ON
  #IFDEF Bit(CFGS)
    Set CFGS OFF
  #ENDIF

  'Start read, wait for it to finish
  SET RD ON
  NOP
  NOP

  'Enable interrupt
  IntOn
end sub

sub ProgramErase(In EEAddress)
  Dim EEAddress As Word Alias EEADRH, EEADR

  'Disable Interrupt
  IntOff

  'Select program memory
  SET EEPGD ON
  #IFDEF Bit(CFGS)
    Set CFGS OFF
  #ENDIF

  SET WREN ON
  #ifdef bit(FREE)
    SET FREE ON
  #endif
  EECON2 = 0x55
  EECON2 = 0xAA
  SET WR ON
  NOP
  NOP
  #ifdef bit(FREE)
    SET FREE OFF
  #endif
  SET WREN OFF

  'Enable interrupt
  IntOn
end sub

'  Section: Data EEPROM Module APIs

Sub NVMADR_EPWrite(IN SysEEAddress as WORD , in EEData)

    dim IntState as bit
    IntState = GIE
    Dim SysEEPromAddress As Word

    #IFDEF Oneof(CHIP_18F24K40,CHIP_18F25K40,CHIP_18F26K40,CHIP_18F27K40,CHIP_18F45K40,CHIP_18F46K40,CHIP_18F47K40,CHIP_18F65K40,CHIP_18F66K40,CHIP_18LF24K40, CHIP_18LF25K40, CHIP_18LF26K40, CHIP_18LF27K40, CHIP_18LF45K40, CHIP_18LF46K40, CHIP_18LF47K40, CHIP_18LF65K40, CHIP_18LF66K40)
       ' Added (Per  Chip Errata Sheets) to correctly support table reads on specific chips)
       ' Sets NVRAM pointer to Static RAM as default location.
        #Ifdef Var(NVMCON1)
           Dim  NVMCON1_7_State, NVMCON1_6_State as bit
           NVMCON1_7_State = NVMCON1.7
           NVMCON1_6_State = NVMCON1.6
        #endif
    #ENDIF

   #IFDEF BIT(NVMREGS)
  'ALL 16F NVMREGS Devices Except 18FxxK40/K42
       SysEEPromAddress = SysEEAddress + 0x7000
       NVMADRH =SysEEPromAddress_h
       NVMADRL =SysEEPromAddress
       NVMDATL = EEData
       NVMREGS = 1
    #ENDIF

   #IFDEF BIT(NVMREG0)
   '' 18FXXk40/K42 and others with NVMREG0/NVMREG1
       NVMCON1 = 0
       SysEEPromAddress = SysEEAddress
       NVMADRH =SysEEPromAddress_h & 0X03  'ERV
       NVMADRL =SysEEPromAddress
        'Access EEPROM LOcations
        NVMREG0 = 0
        NVMREG1 = 0
        NVMDAT = EEData
   #ENDIF

    #IFDEF VAR(NVMADRU)
      'Select DATA EE section (0x310000 - 0x3100FF)
       NVMADRU = 0x31
       NVMADRH =SysEEAddress_h
       NVMADRL =SysEEAddress
       NVMDATL = EEData

    #ENDIF



    [canskip]FREE =0b'0'
    [canskip]WREN= 0b'1'
    #if bit(NVMEN)
      NVMEN=0b'1'
    #endif
    GIE = 0
    NVMCON2 = 0x55
    NVMCON2 = 0xAA
    WR = 1
    NOP     ' NOPs may be required for latency at high frequencies
    NOP
    NOP
    NOP
    NOP
    wait while WR = 1
    [canskip]WREN= 0b'0'
    #if bit(NVMEN)
      NVMEN=0b'0'
    #endif

    #IFDEF Oneof(CHIP_18F24K40,CHIP_18F25K40,CHIP_18F26K40,CHIP_18F27K40,CHIP_18F45K40,CHIP_18F46K40,CHIP_18F47K40,CHIP_18F65K40,CHIP_18F66K40,CHIP_18LF24K40, CHIP_18LF25K40, CHIP_18LF26K40, CHIP_18LF27K40, CHIP_18LF45K40, CHIP_18LF46K40, CHIP_18LF47K40, CHIP_18LF65K40, CHIP_18LF66K40)
       ' Added (Per  Chip Errata Sheets) to correctly support table reads on specific chips)
       ' Sets NVRAM pointer to Static RAM as default location.
        #Ifdef Var(NVMCON1)
           Dim  NVMCON1_7_State, NVMCON1_6_State as bit
           NVMCON1.7 = NVMCON1_7_State
           NVMCON1.6 = NVMCON1_6_State
        #endif
    #ENDIF

    'Restore interrupt to initial  State
    GIE = IntState
end sub

Sub NVMADR_EPRead(IN SysEEAddress AS word  , out EEDataValue )

  #IFDEF Oneof(CHIP_18F24K40,CHIP_18F25K40,CHIP_18F26K40,CHIP_18F27K40,CHIP_18F45K40,CHIP_18F46K40,CHIP_18F47K40,CHIP_18F65K40,CHIP_18F66K40,CHIP_18LF24K40, CHIP_18LF25K40, CHIP_18LF26K40, CHIP_18LF27K40, CHIP_18LF45K40, CHIP_18LF46K40, CHIP_18LF47K40, CHIP_18LF65K40, CHIP_18LF66K40)
     ' Added (Per  Chip Errata Sheets) to correctly support table reads on specific chips)
     ' Sets NVRAM pointer to Static RAM as default location.
      #Ifdef Var(NVMCON1)
         Dim  NVMCON1_7_State, NVMCON1_6_State as bit
         NVMCON1_7_State = NVMCON1.7
         NVMCON1_6_State = NVMCON1.6
      #endif
  #ENDIF

  #IFDEf bit(NVMREGS)
      Dim SysEEPromAddress As Word
      SysEEPromAddress = SysEEAddress + 0x7000
      NVMADRH = SysEEPromAddress_h
      NVMADRL = SysEEPromAddress
      NVMREGS = 1
      RD = 1
      NOP     ' NOPs may be required for latency at high frequencies
      NOP
      NOP
      NOP
      EEDataValue = NVMDATL
  #ENDIF

  #IFDEf bit(NVMREG0)
      Dim SysEEPromAddress As Word
      SysEEPromAddress = SysEEAddress
      NVMADRH =SysEEPromAddress_h
      NVMADRL =SysEEPromAddress
      NVMREG0 = 0
      NVMREG1 = 0
      RD = 1
      NOP     ' NOPs may be required for latency at high frequencies
      NOP
      NOP
      NOP
      EEDataValue = NVMDAT
  #ENDIF


    #IFDEF VAR(NVMADRU)
        'Select DATA EE section (0x310000 - 0x3100FF)
        NVMADRU = 0x31
        NVMADRH =SysEEAddress_h
        NVMADRL =SysEEAddress
        RD = 1
        NOP     ' NOPs may be required for latency at high frequencies
        NOP
        NOP
        NOP
        EEDataValue = NVMDATL
    #ENDIF

    #IFDEF Oneof(CHIP_18F24K40,CHIP_18F25K40,CHIP_18F26K40,CHIP_18F27K40,CHIP_18F45K40,CHIP_18F46K40,CHIP_18F47K40,CHIP_18F65K40,CHIP_18F66K40,CHIP_18LF24K40, CHIP_18LF25K40, CHIP_18LF26K40, CHIP_18LF27K40, CHIP_18LF45K40, CHIP_18LF46K40, CHIP_18LF47K40, CHIP_18LF65K40, CHIP_18LF66K40)
       ' Added (Per  Chip Errata Sheets) to correctly support table reads on specific chips)
       ' Sets NVRAM pointer to Static RAM as default location.
        #Ifdef Var(NVMCON1)
           Dim  NVMCON1_7_State, NVMCON1_6_State as bit
           NVMCON1.7 = NVMCON1_7_State
           NVMCON1.6 = NVMCON1_6_State
        #endif
    #ENDIF


End Sub
