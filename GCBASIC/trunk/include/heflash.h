'''GCB Library for reading/writing to High Endurance Flash Memory
'''-----------------------------------------------------------------------------
'''The high endurance flash memory (HEFM) is a block of 128 locations found
'''at the top of the Flash program memory. Each location can only be used to
'''hold a byte variable (whereas the standard Flash memory for a mid-range
'''PIC MCU will typically hold 14 bits of information).
'''
'''The main difference between EEPROM and HEFM is that the former does allow
'''byte-by-byte erase whereas the latter does not. With HEFM data must be
'''erased before a write and this can only be performed in blocks
'''(referred as rows) of a fixed size associated with the chip design.
'''
'''The heflash.h library provide the following variables for the current
'''list of chips supporting this design :
'''  FLASH_ROWSIZE : size of a HEFM row. 16 or 32 words (byte variable)
'''  HEFLASH_START : starting address of HEFM (word variable)
'''  HEFLASH_END   : ending address of HEFM (word variable)
'''
'''These are the user's oriented functions/subroutines:
'''  HEFLASH_readBlock  : sub to read a HEFM row into a n bytes buffer array
'''  HEFLASH_writeBlock : sub to write a buffer array of n bytes in a HEFM row
'''  HEFLASH_readByte   : byte function to read a HEFM row at a given offset
'''  HEFRead  : sub equivalent to EPRead sub for EEPROM
'''  HEFWrite : sub equivalent to EPwrite sub for EEPROM
'''
'''WORD OF CAUTION:
'''Whenever you update the hex file of your PIC with your programmer you
'''will erase the data that are stored in HEFM.  If you want to avoid that
'''you will have to flash your PIC with a software that allow memory exclusion
'''when flashing. This is the case of Microchip MPLAB IPE :
'''(Go In Advanced Mode/Enter password/Select Memory/Tick “Preserve Flash
'''on Program”/Enter Start and End address of your HEFM).
'''
'''This program has been adaptated from the Microchip Application note
'''AN1673 "Using the PIC16F1XXX High-Endurance Flash (HEF) block
'''by Lucio Di Jasio, 2014
'''
'''@author 	Jacques Nilo
'''@licence	GPL
'''@version	1.2
'''@date   	31.05.2015
'''*****************************************************************************

; Define HEF Global variables
dim HEFLASH_START,HEFLASH_END as word
dim FLASH_ROWSIZE as Byte
#startup HEFLASH_init

'''@hide
sub HEFLASH_init
#ifdef Chip_12F1501
  FLASH_ROWSIZE = 16
  HEFLASH_START = 0x0380
  HEFLASH_END = 0x03FF
#endif
#ifdef Oneof(Chip_16f1503, Chip_16f1507, Chip_16f1703, Chip_16f1707)
  FLASH_ROWSIZE = 16
  HEFLASH_START = 0x0780
  HEFLASH_END = 0x07FF
#endif
#ifdef Chip_10f320
  FLASH_ROWSIZE = 16
  HEFLASH_START = 0x0080
  HEFLASH_END = 0x00FF
#endif
#ifdef Chip_10f322
  FLASH_ROWSIZE = 16
  HEFLASH_START = 0x0180
  HEFLASH_END = 0x01FF
#endif
#ifdef Oneof(Chip_16f1512, Chip_16f720)
  FLASH_ROWSIZE = 32
  HEFLASH_START = 0x0780
  HEFLASH_END = 0x07FF
#endif
#ifdef Oneof(Chip_16f1508, Chip_16f1513, Chip_16f1704, Chip_16f1708)
  FLASH_ROWSIZE = 32
  HEFLASH_START = 0x0F80
  HEFLASH_END = 0x0FFF
#endif
; ifdef statement cannot be splitted on different lines :-(
#ifdef Oneof(Chip_16f1454, Chip_16f1455, Chip_16f1459, Chip_16f1509, Chip_16f1516, Chip_16f1517, Chip_16f1526, Chip_16f1705, Chip_16f1709)
  FLASH_ROWSIZE = 32
  HEFLASH_START = 0x1F80
  HEFLASH_END = 0x1FFF
#endif
#ifdef Oneof(Chip_16f1518, Chip_16f1519, Chip_16f1527)
  FLASH_ROWSIZE = 32
  HEFLASH_START = 0x3F80
  HEFLASH_END = 0x3FFF
#endif
end sub

'''Read a word from HEFM
'''@param FlashAddress HEFM source absolute address (word)
'''@return memory value stored at absolute address specified by flashaddress (word)
function FLASH_read(in FlashAddress as word) as word
;
; This word function returns the memory value stored at
; absolute address [Flashaddress]
; Used by HEF subs and functions. For experienced programmers only.
;
Dim FlashAddress alias PMADRH,PMADRL
Dim FLASH_read   alias PMDATH,PMDATL
PMCON1.CFGS=0       'Select the Flash address space
PMCON1.RD=1         'Read control bit on
NOP
NOP
end function

'''Read a word from configuration Flash memory
'''@param FlashAddress source absolute address (word)
'''@return memory value stored at absolute address specified by flashaddress (word)
function FLASH_readconfig(in FlashAddress as word) as word
;
; This word function returns the memory value stored at
; absolute address [Flashaddress] above 0x8000 where the
; IDLOC, CONFIG bits and calibration data can be found
; For experienced programmers only.
;
Dim FlashAddress     alias PMADRH,PMADRL
Dim FLASH_readconfig alias PMDATH,PMDATL
PMCON1.CFGS=1       'Select the configuration Flash address space
PMCON1.RD=1         'Read control bit on
NOP
NOP
end function

'''@hide
sub FLASH_unlock
;
; This subroutine unlocks Flash memory to allow erase or write operation
; Used by HEF subs and functions. For experienced programmers only.
;
; Unlock sequence
PMCON2 = 0x55
PMCON2 = 0xAA
PMCON1.WR=1
NOP
NOP
end sub

'''@hide
sub FLASH_write(In FlashAddress as word,In FlashDataValue as word,in Flashlatch)
;
; This subroutine writes the word [FlashDataValue] at absolute address given
; in the word variable [Flashaddress]
; When Flashlatch is 1 no real write is performed.
; When Flashlatch is 0 the real write function is executed
; Used by HEF subs and functions. For experienced programmers only.
;
Dim FlashAddress   alias PMADRH,PMADRL
Dim FlashDataValue alias PMDATH,PMDATL
Dim old_GIE as bit
old_gie = INTCON.GIE    'Save initial GIE state
INTCON.GIE=0            'Disable interrupts
PMCON1.LWLO=Flashlatch  '1=latch 0=write row
PMCON1.CFGS=0           'Select the Flash address space
PMCON1.FREE=0           'Next operation will be a write
PMCON1.WREN=1           'Enable Flash Memory write/erase
Flash_unlock            'Unlock memory
PMCON1.WREN=0           'Enable Flash Memory write/erase
INTCON.GIE=old_gie      'Restore initial GIE state
end sub

'''@hide
sub FLASH_erase(In FlashAddress as word)
;
; This subroutine erase the full HEM row to which belongs the given
; address [Flashaddress]
; Flashaddress must always be comprised betwwen HEFLASH_START & _END
; Used by HEF subs and functions. For experienced programmers only.
;
Dim FlashAddress alias PMADRH,PMADRL
Dim old_GIE as bit
old_gie = INTCON.GIE    'Save initial GIE state
INTCON.GIE=0            'Disable interrupts
PMCON1.CFGS=0           'Select the Flash address space
PMCON1.FREE=1           'Next operation will be a write
PMCON1.WREN=1           'Enable Flash Memory write/erase
Flash_unlock            'Unlock memory
PMCON1.WREN=0           'Enable Flash Memory write/erase
INTCON.GIE=old_gie      'Restore initial GIE state
end sub

'''Write a block of data to HEFM. (Lenght should be < row size)
'''@param Flash_rel_addr HEFM block number (0,...,FLASH_ROWSIZE-1)
'''@param Flash_buffer HEFM block source buffer
'''@param Flashcount # of bytes to write to HEFM block (<FLASH_ROWSIZE)
sub HEFLASH_writeBlock(in Flash_rel_addr, in Flash_buffer(),in FlashCount)
;
; This subroutine writes Flashcount bytes from the Flash_buffer() array
; at a given row # of the HEM
; row [Flash_rel_addr] is generally comprised between 0 and 3
; The writing starts at the begining at the first byte of the row
; Flashcount must always be lower than FLASH_ROWSIZE
;
Dim Flash_abs_addr as word
; Obtain absolute address if HEFlash row
Flash_abs_addr=Flash_rel_addr*FLASH_ROWSIZE+HEFLASH_START
; Erase the entire row
FLASH_erase(Flash_abs_addr)
index=0
; Fill the latches with data
do while FlashCount > 1
; Load data in latches without writing
  FLASH_Write(Flash_abs_addr+index,Flash_buffer(index+1),1)
  index = index + 1
  FlashCount = FlashCount - 1
loop
; Last write of data --> write
FLASH_Write(Flash_abs_addr+index,Flash_buffer(index+1),0)
end sub

'''Read a block of data from HEFM. (Lenght should be < row size)
'''@param Flash_buffer destination buffer (should be large enough)
'''@param Flash_rel_addr HEFM block number (0,...,FLASH_ROWSIZE-1)
'''@param Flashcount # of bytes to read from HEFM block (<FLASH_ROWSIZE)
sub HEFLASH_readBlock(in Flash_buffer(),in Flash_rel_addr,in FlashCount)
;
; This subroutine returns in Flash_buffer array the [Flashcount] byte values
; stored in HEM given its row #
; row [Flash_rel_addr] is generally comprised between 0 and 3
; The reading starts at the begining at the first byte of the row
; Flashcount must therefore be always lower than FLASH_ROWSIZE
;
Dim Flash_abs_addr as word
; Obtain absolute address if HEFlash row
Flash_abs_addr=Flash_rel_addr*FLASH_ROWSIZE+HEFLASH_START
index=1
; Read content
do while FlashCount > 0
  Flash_buffer(index)=FLASH_Read(Flash_abs_addr+index-1)
  index=index+1
  FlashCount=FlashCount-1
loop
end sub

'''Read a byte of data from HEFM 
'''@param Flash_rel_addr HEFM block number (0,...,FLASH_ROWSIZE-1)
'''@param Flash_offset offset within the HEFM block (<FLASH_ROWSIZE)
'''@return Byte of data retrieved 
function HEFLASH_readByte(in Flash_rel_addr, in Flash_offset) as byte
;
; Returns the byte value stored in HEM given row # and offset #
; row [Flash_rel_addr] is generally comprised between 0 and 3
; offset [Flash_offset] cannot be greater than FLASH_ROWSIZE
;
Dim Flash_abs_addr as word
; Add offset into HEFlash memory
Flash_abs_addr=Flash_rel_addr*FLASH_ROWSIZE+HEFLASH_START+Flash_offset
; Read content
HEFLASH_readByte=FLASH_Read(Flash_abs_addr)
end function

'''Read a byte of data from HEFM 
'''@param location relative adress within global HEFM (0,...,127)
'''@param Flash_data Byte of data retrieved
sub HEFRead(in location, out Flash_data)
;
; This subroutine returns in Flash_data the byte value stored in HEM
; at relative location [location] with 0 being the first HEM location
; location must generally be comprised between 0 and 127
; The syntax is therefore similar to the EEPROM subroutine
;
Flash_rel_addr = location / FLASH_ROWSIZE
Flash_offset = location % FLASH_ROWSIZE
Flash_data = HEFLASH_readByte(Flash_rel_addr,Flash_offset)
end sub

'''Write a byte of data to HEFM. Remaining block data kept unchanged
'''@param location relative adress within global HEFM (0,...,127)
'''@param Flash_data Byte of data to be written
sub HEFwrite(in location, in Flash_data)
;
; This subroutine write the byte data [Flash_data] in HEM
; at relative location [location] with 0 being the first HEM location
; location must generally be comprised between 0 and 127
; The syntax is therefore similar to the EEPROM subroutine
; The data already existing in the row of [location] are preserved
; If you need to save several data at once in the same row the
; HEFLASH_writeBlock subroutine will be more efficient
;
; It would be good to be able to declare dim Flash_TempBuffer(FLASH_ROWSIZE)...
dim Flash_TempBuffer(32)
; Compute row #
Flash_rel_addr = location / FLASH_ROWSIZE
; Compute offset # in row
Flash_offset = location % FLASH_ROWSIZE
; Save data previously stored in row #
HEFLASH_readBlock(Flash_TempBuffer(),Flash_rel_addr,FLASH_ROWSIZE)
; Update row buffer with Flash_data at offset
Flash_TempBuffer(Flash_offset+1)=Flash_data
; Write back the updated row buffer in HEFM
HEFLASH_writeBlock(Flash_rel_addr,Flash_TempBuffer(),FLASH_ROWSIZE)
end sub