;I2C EEPROM Include File
;Thomas Henry                 --- 5/29/2014
;Evan R.  Venn Revised        --- 6/28/2014 - Correct parameter    eeprom_rd _array to IN
;Evan R.  Venn Revised        --- 7/3/2014 - Added Hardware I2C Support.  This simply detects your configuration variable I2C_DATA or HI2C_DATA
;Evan R.  Venn Revised        --- 7/12/2014 - Added Support for Byte address EEPROMs

'    added references to function to add wr_string and rd_string
'    eeprom_wr_array
'    eeprom_wr_string
'    eeprom_rd_array
'    eeprom_rd_string
'
'    and, the Variables were removed to ensure stability in all cases.

;This include file began life as Evan R. Venn's "24LCxxxx.h"
;include file (version 0.85).

;The original intent was simply to comment his code for my
;own understanding. But then, I hit upon a number of ways
;to condense things while making the commands more general.
;So, rather than cloud the issue, I decided to start from
;scratch and assign the result an entirely new name. But it
;is clear that Evan's code was definitely the progenitor
;of what follows. I thank him for that.

;Here are the changes:

; removed unneeded or redundant code
; renamed page commands to reflect they are actually
;   array/string commands (not page commands)
; greatly condensed the array write command
; reduced local variables to only two bytes
; commented the code throughout
; generalized with parameters to allow connection to
;   multiple EEPROMs simultaneously
; eliminated reliance on the "Utils.h" include file
; renamed parameters to be consistent, but still not
;   clash with user program variables

;This has been tested on the 24LC256 and AT24C32 which have
;page sizes of 64 and 32 bytes, respectively.

;There are two local byte variables, and eight bytes are
;consumed in the input/output parameters, for a grand
;total of ten bytes.

;Quick Command Reference:

;eeprom_wr_byte(device_number, address, value)
;eeprom_rd_byte(device number, address, value)
;eeprom_wr_array(device_number, page_size, address, array_name, number_of_bytes)
;eeprom_rd_array(device_number, address, array_name, number_of_bytes)

;It is assumed that the arrays/strings are 256 bytes long
;or less, but this could easily be changed if needed.

;There is no error detection. It is the duty of the
;calling program to specify valid addresses within
;the Eeprom and not let array/strings run off the end.

;----- Subroutines

#define ACK       0x00
#define NACK      0x80

;------ Word support for larger memory devices
sub eeprom_wr_byte (in eepDev as byte, in eepAddr as word, in eepromVal as byte)
  ;write a single byte to an address

  #ifdef HI2C_DATA
      do
        HI2CReStart                              ;generate a start signal
        HI2CSend(eepDev)                       ;inidcate a write
      loop While HI2CAckPollState
      HI2CSend(eepAddr_H)                    ;send address as two bytes
      HI2CSend(eepAddr)
      HI2CSend(eepromVal)                    ;then the value
      HI2CStop
  #endif

  #ifdef I2C_DATA

      I2CStart                              ;generate a start signal
      I2CSend(eepDev)                       ;indicate a write
      I2CSend(eepAddr_H)                    ;send address as two bytes
      I2CSend(eepAddr)
      I2CSend(eepromVal)                    ;then the value
      I2CStop
      I2CAckPoll(eepDev)
  #endif
end sub

;------ Byte support for smaller memory devices
sub eeprom_wr_byte (in eepDev as byte, in eepAddr as byte, in eepromVal as byte)
  ;write a single byte to an address

  #ifdef HI2C_DATA
      do
        HI2CReStart                              ;generate a start signal
        HI2CSend(eepDev)                       ;inidcate a write
      loop While HI2CAckPollState
      HI2CSend(eepAddr)
      HI2CSend(eepromVal)                    ;then the value
      HI2CStop
  #endif

  #ifdef I2C_DATA

      I2CStart                              ;generate a start signal
      I2CSend(eepDev)                       ;indicate a write
      I2CSend(eepAddr)
      I2CSend(eepromVal)                    ;then the value
      I2CStop
      I2CAckPoll(eepDev)
  #endif
end sub


;-----

;------ Word support for larger memory devices
sub eeprom_rd_byte(in eepDev as byte, in eepAddr as word, out eepromVal as byte )
  ;read a single byte from an address
  #ifdef HI2C_DATA
      do
        HI2CReStart                              ;generate a start signal
        HI2CSend(eepDev)                       ;inidcate a write
      loop While HI2CAckPollState
      HI2CSend(eepAddr_H)                    ;send address as two bytes
      HI2CSend(eepAddr)
      HI2CReStart
      HI2CSend(eepDev + 1)                   ;set the read flag
      HI2CReceive(eepromVal, NACK)           ;read one byte and conclude
      HI2CStop
  #endif

  #ifdef I2C_DATA
      I2CStart                              ;generate a start signal
      I2CSend(eepDev)                       ;inidcate a write
      I2CSend(eepAddr_H)                    ;send address as two bytes
      I2CSend(eepAddr)
      I2CReStart
      I2CSend(eepDev + 1)                   ;set the read flag
      I2CReceive(eepromVal, NACK)           ;read one byte and conclude
      I2CStop
  #endif
end sub

;------ Byte support for larger memory devices
sub eeprom_rd_byte(in eepDev as byte, in eepAddr as byte, out eepromVal as byte )
  ;read a single byte from an address
  #ifdef HI2C_DATA
      do
        HI2CReStart                              ;generate a start signal
        HI2CSend(eepDev)                       ;inidcate a write
      loop While HI2CAckPollState
      HI2CSend(eepAddr)
      HI2CReStart
      HI2CSend(eepDev + 1)                   ;set the read flag
      HI2CReceive(eepromVal, NACK)           ;read one byte and conclude
      HI2CStop
  #endif

  #ifdef I2C_DATA
      I2CStart                              ;generate a start signal
      I2CSend(eepDev)                       ;inidcate a write
      I2CSend(eepAddr)
      I2CReStart
      I2CSend(eepDev + 1)                   ;set the read flag
      I2CReceive(eepromVal, NACK)           ;read one byte and conclude
      I2CStop
  #endif
end sub
;-----
#define eeprom_wr_string eeprom_wr_array
;------ Word support for larger memory devices
sub eeprom_wr_array(in eepDev as byte, in eepPageSize as byte, in eepAddr as word, in eepArray() as byte, in eepLen as byte)
  ;write array/string to EEPROM, starting at specified address

  #ifdef HI2C_DATA

      do
        HI2CReStart                              ;generate a start signal
        HI2CSend(eepDev)                       ;inidcate a write
      loop While HI2CAckPollState
      HI2CSend(eepAddr_H)                    ;and the start address
      HI2CSend(eepAddr)                      ;as two bytes

      for eep_i = 1 to eepLen
        HI2CSend(eepArray(eep_i))            ;write next byte from array
        eepAddr++                           ;prep for next one
        if (eepAddr mod eepPageSize) = 0 then ;end of page
          HI2CStop                           ;so, lock in place
          do
            HI2CReStart                              ;generate a start signal
            HI2CSend(eepDev)                       ;inidcate a write
          loop While HI2CAckPollState
          HI2CSend(eepAddr_H)                ;send next page address
          HI2CSend(eepAddr)
        end if
      next i

      HI2CStop                               ;could be redundant in one
  #endif

  #ifdef I2C_DATA

      I2CStart                              ;generate a start signal
      I2CSend(eepDev)                       ;indicate the device
      I2CSend(eepAddr_H)                    ;and the start address
      I2CSend(eepAddr)                      ;as two bytes

      for eep_i = 1 to eepLen
        I2CSend(eepArray(eep_i))            ;write next byte from array
        eepAddr++                           ;prep for next one
        if (eepAddr mod eepPageSize) = 0 then ;end of page
          I2CStop                           ;so, lock in place
          I2CAckPoll(eepDev)                ;wait for buffer write
          I2CReStart                          ;generate a start signal
          I2CSend(eepDev)                   ;indicate a write
          I2CSend(eepAddr_H)                ;send next page address
          I2CSend(eepAddr)
        end if
      next i

      I2CStop                               ;could be redundant in one

  #endif
end sub

;------ Byte support for larger memory devices
sub eeprom_wr_array(in eepDev as byte, in eepPageSize as byte, in eepAddr as byte, in eepArray() as byte, in eepLen as byte)
  ;write array/string to EEPROM, starting at specified address

  #ifdef HI2C_DATA

      do
        HI2CReStart                              ;generate a start signal
        HI2CSend(eepDev)                       ;inidcate a write
      loop While HI2CAckPollState
      HI2CSend(eepAddr)                      ;as two bytes

      for eep_i = 1 to eepLen
        HI2CSend(eepArray(eep_i))            ;write next byte from array
        eepAddr++                           ;prep for next one
        if (eepAddr mod eepPageSize) = 0 then ;end of page
          HI2CStop                           ;so, lock in place
          do
            HI2CReStart                              ;generate a start signal
            HI2CSend(eepDev)                       ;inidcate a write
          loop While HI2CAckPollState
          HI2CSend(eepAddr)
        end if
      next i

      HI2CStop                               ;could be redundant in one
  #endif

  #ifdef I2C_DATA

      I2CStart                              ;generate a start signal
      I2CSend(eepDev)                       ;indicate the device
      I2CSend(eepAddr)                      ;as two bytes

      for eep_i = 1 to eepLen
        I2CSend(eepArray(eep_i))            ;write next byte from array
        eepAddr++                           ;prep for next one
        if (eepAddr mod eepPageSize) = 0 then ;end of page
          I2CStop                           ;so, lock in place
          I2CAckPoll(eepDev)                ;wait for buffer write
          I2CReStart                          ;generate a start signal
          I2CSend(eepDev)                   ;indicate a write
          I2CSend(eepAddr_H)                ;send next page address
          I2CSend(eepAddr)
        end if
      next i

      I2CStop                               ;could be redundant in one

  #endif
end sub

;-----
;------ Word support for larger memory devices
#define eeprom_rd_string eeprom_rd_array
sub eeprom_rd_array(in eepDev as byte, in eepAddr as word, out eepArray() as byte, in eepLen as byte)
  ;Read an entire array/string from EEPROM
    #ifdef HI2C_DATA
        do
          HI2CReStart                              ;generate a start signal
          HI2CSend(eepDev)                       ;inidcate a write
        loop While HI2CAckPollState
        HI2CSend(eepAddr_H)                    ;as two bytes
        HI2CSend(eepAddr)
        HI2CReStart
        HI2CSend(eepDev + 1)                   ;indicate a read

        eep_i = 0                             ;loop consecutively
        do while (eep_i < eepLen)             ;these many bytes
          eep_j = eep_i + 1                   ;arrays begin at 1 not 0
          if (eep_i  < (eepLen - 1)) then
            HI2CReceive(eepArray(eep_j), ACK)  ;more data to get
          else
            HI2CReceive(eepArray(eep_j), NACK ) ;send NACK on last byte
          end if
          eep_i++                             ;get set for next
        loop
        HI2CStop
    #endif

    #ifdef I2C_DATA
        I2CStart                              ;generate a start signal
        I2CSend(eepDev)                       ;send address of start
        I2CSend(eepAddr_H)                    ;as two bytes
        I2CSend(eepAddr)
        I2CReStart
        I2CSend(eepDev + 1)                   ;indicate a read

        eep_i = 0                             ;loop consecutively
        do while (eep_i < eepLen)             ;these many bytes
          eep_j = eep_i + 1                   ;arrays begin at 1 not 0
          if (eep_i  < (eepLen - 1)) then
            I2CReceive(eepArray(eep_j), ACK)  ;more data to get
          else
            I2CReceive(eepArray(eep_j), NACK) ;send NACK on last byte
          end if
          eep_i++                             ;get set for next
        loop
        I2CStop
    #endif
end sub

;------ Byte support for larger memory devices
sub eeprom_rd_array(in eepDev as byte, in eepAddr as byte, out eepArray() as byte, in eepLen as byte)
  ;Read an entire array/string from EEPROM
    #ifdef HI2C_DATA
        do
          HI2CReStart                              ;generate a start signal
          HI2CSend(eepDev)                       ;inidcate a write
        loop While HI2CAckPollState
        HI2CSend(eepAddr)
        HI2CReStart
        HI2CSend(eepDev + 1)                   ;indicate a read

        eep_i = 0                             ;loop consecutively
        do while (eep_i < eepLen)             ;these many bytes
          eep_j = eep_i + 1                   ;arrays begin at 1 not 0
          if (eep_i  < (eepLen - 1)) then
            HI2CReceive(eepArray(eep_j), ACK)  ;more data to get
          else
            HI2CReceive(eepArray(eep_j), NACK ) ;send NACK on last byte
          end if
          eep_i++                             ;get set for next
        loop
        HI2CStop
    #endif

    #ifdef I2C_DATA
        I2CStart                              ;generate a start signal
        I2CSend(eepDev)                       ;send address of start
        I2CSend(eepAddr)
        I2CReStart
        I2CSend(eepDev + 1)                   ;indicate a read

        eep_i = 0                             ;loop consecutively
        do while (eep_i < eepLen)             ;these many bytes
          eep_j = eep_i + 1                   ;arrays begin at 1 not 0
          if (eep_i  < (eepLen - 1)) then
            I2CReceive(eepArray(eep_j), ACK)  ;more data to get
          else
            I2CReceive(eepArray(eep_j), NACK) ;send NACK on last byte
          end if
          eep_i++                             ;get set for next
        loop
        I2CStop
    #endif
end sub

