'
'    Created Evan R Venn - Oct 2013
'    Beta v0.80
'
'    0.80    Test release

' Library for a Ramtron I2C FRAM serial memory
' write_fram(a,d);  Write the int8 d to the address a         ////
' //d = read_fram(a);  Read the int8 d from the address a       ////
' //multi_write_fram(a,char *d,size);//can cross chip boundry ////
' //multi_read_fram(a,char *d,size);//can cross chip boundry  ////
' //The main program may define
' // 2^15=32768 address space.  2^3=8   3 bits for 8 chips      ////
' // For a total of 18 bits



'	Device	Page			Device
'	1	0	0x00000	0x0FFFF	A0
'	1	1	0x10000	0x1FFFF	A2
'	2	0	0x20000	0x2FFFF	A4
'	2	1	0x30000	0x3FFFF	A6
'	3	0	0x40000	0x4FFFF	A8
'	3	1	0x50000	0x5FFFF	AA
'	4	0	0x60000	0x6FFFF	AC
'	4	1	0x70000	0x7FFFF	AE




'YOU TO HAVE A DEVICE CALLED "FRAM_DEVICE_1"!!
#define FRAM_DEVICE_1           0xAC


#define FRAM_CHIPS   1
' 4 chips 0,1,2,3
#define FRAM_PAGE_SIZE    0x10000
#script
'        FRAM_MAX_ADDRESS = (FRAM_PAGE_SIZE*FRAM_CHIPS)-1
#endscript
' this is the TOTAL of all FRAM Devices
#DEFINE FRAM_MAX_ADDRESS = 0x20000




'
' DO NOT CHANGE variable defintions
  dim target_address as long
  Dim ul_i as Long
  Dim ul_j as Long
'.............................................................................
'          Writes data to the FRAM address
'.............................................................................
Sub fram_wr_byte ( in addr as long , In data_out)
      'fram_wr_byte(addr,data_out);         ' write data to addr FRAM address

      'generate Start COndition
        I2CSTART
      ' calculate Page Select
      ' ((0xA0|(int8)(address>>14))&0xFE);//&0xFE forces to write


      target_address = uf_shift_long ( addr, 15, >> )
      target_address = target_address  | FRAM_DEVICE_1
      target_address = target_address & 0xFE

'      crlf
'      SerPrint 1, "wr_byte) Target Address = "
'      SerPrint 1, hex(target_address )
'      crlf

      'send WRITE command
        I2CSEND( target_address )

'      SerPrint 1, "Address = "
'      SerPrint 1, hex(addr_H )
'      SerPrint 1, hex([byte]addr)
'      crlf

       addrh = uf_shift ( addr, 8, >> )
       addrl = addr & 0xff
      'address MSB
      'comment the next line if a Low-Density (<=2kB) device is used
        I2CSEND(addrh)
      'address LSB
        I2CSEND(addrl)

      'send data
        I2CSEND( data_out )
      'initiate Stop Condition
        I2CSTOP

end sub




'.............................................................................
'          Reads a byte from the FRAM address
'.............................................................................
Sub fram_rd_byte( In addr as Long , Out ul_returnedbyte )

      ' generate Start COndition
      I2CSTART

      ' ((0xA0|(int8)(address>>14))&0xFE);//&0xFE forces to write

      target_address = uf_shift_long ( addr, 15, >> )
      target_address = target_address  | FRAM_DEVICE_1
      target_address = target_address & 0xFE

'      crlf
'      SerPrint 1, "rd_byte) Target Address = "
'      SerPrint 1, hex(target_address )
'      crlf

      ' send WRITE command
      I2CSEND( target_address )


       addrh = uf_shift ( addr, 8, >> )
       addrl = addr & 0xff
      'address MSB
      'comment the next line if a Low-Density (<=2kB) device is used
        I2CSEND(addrh)
      'address LSB
        I2CSEND(addrl)

        ' generate Start COndition
      I2CSTART
' faster to reuse the address above and add 1
'      target_address = uf_shift_long ( addr, 15, >> )
'      target_address = target_address  | FRAM_DEVICE_1
'      target_address = target_address | 0x01

       target_address = target_address | 0x01
'      crlf
'      SerPrint 1, "3) Target Address = "
'      SerPrint 1, hex(target_address )
'      crlf

      ' send READ command
      I2CSEND( target_address  )
      ul_returnedbyte  = 0
      ' read data from buffer
      I2CRECEIVE ul_returnedbyte  , NACK
      ' send Stop Condition
      I2CSTOP
      ' return data from I2C buffer

end sub

Sub fram_rd_deviceID( IN fram_device, out deviceID_1, out deviceID_2, out deviceID_3   )
      dim ul_returnedbyte as byte
      ' generate Start COndition
      I2CSTART
      I2CSend ( 0xF8 )

      Dim deviceID_1 As Byte Alias  target_address_E
      Dim deviceID_2 As Byte Alias  target_address_U
      Dim deviceID_3 As Byte Alias  target_address_H

      ' send WRITE command
      I2CSEND( fram_device , ACK)
      I2CStart
      I2CSend ( 0xF9 , ACK)
      I2CRECEIVE ( deviceID_1  , ACK )
      I2CRECEIVE ( deviceID_2  , ACK )
      I2CRECEIVE ( deviceID_3  , NACK )



      I2CSTOP


End Function


Sub fram_sleep( in fram_device   )
      dim ul_returnedbyte as byte
      ' generate Start COndition
      I2CSTART
      I2CSend ( 0xF8 )

      ' ((0xA0|(int8)(address>>14))&0xFE);//&0xFE forces to write
      ' use target address to return the values to the calling

      Dim deviceID_1 As Byte Alias  target_address_E
      Dim deviceID_2 As Byte Alias  target_address_U
      Dim deviceID_3 As Byte Alias  target_address_H

      ' send WRITE command
      I2CSEND( fram_device , ACK)
      I2CStart
      I2CSend ( 0x86 , ACK)
      I2CSTOP

End Function


sub fram_wake ( in fram_device  )
    ' sets I2CReturnState to the currect state of the device
    I2CACKPOLL ( fram_device )
end sub
'.............................................................................
'         Writes a page to the FRAM address
'.............................................................................
sub fram_wr_page( In addr as Long, ul_data() as Byte, datalength as byte )


'        crlf
'        SerPrint 1, "fram_wr_page"
'        crlf
'        SerSend 1,9
'        SerPrint 1, "Addr = 0x"
'        SerPrint 1, hex( addr_E )
'        SerPrint 1, hex( addr_U )
'        SerPrint 1, hex( addr_H )
'        SerPrint 1, hex( addr )

      ul_i = 0
      ul_j = 1
      ul_page_nr = 0
      ' generate Start COndition
      i2cstart

      ' ((0xA0|(int8)(address>>14))&0xFE);//&0xFE forces to write

      target_address = uf_shift_long ( addr, 15, >> )
      target_address = target_address  | FRAM_DEVICE_1
      target_address = target_address & 0xFE

      ' send WRITE command
      I2CSend ( target_address )

      addrh = uf_shift ( ( addr & 0xffff) , 8, >> )
      addrl = ( addr & 0xffff)  & 0xff


'        SerSend  1, 9
'        SerPrint 1, "Addr = 0x"
'        SerPrint 1, hex( addrh )
'        SerPrint 1, hex( addrl )
'        crlf

      'address MSB
      I2CSEND(addrh)
      'address LSB
      I2CSEND(addrl)

      ul_mod = addr%FRAM_PAGE_SIZE

      ul_pagespace =  FRAM_PAGE_SIZE - ul_mod

if ( ul_mod <> 0 ) then

    if ( datalength > ul_pagespace ) then
       ' Loop through number of bytes
       ul_i = 0
       while ( ul_i < ul_pagespace )
          ul_i++
          ' write next byte from array
          I2CSend ( ul_data(ul_j) )
          ' inc array index
          ul_j++
       Loop
       ' initiate Stop Condition
       I2CStop

       ' ACK polling removes the need to for the 24xxxxx device to have a 5ms write time - minimum
       ' No need to ACKPOLL. This device is SO fast!
'       I2CACKPOLL( fram_device )

       ' go to the start of the next page
       addr = addr + (FRAM_PAGE_SIZE-(addr%FRAM_PAGE_SIZE))

       'calculate the number of pages
       ul_page_nr =  ( datalength - ul_j ) / FRAM_PAGE_SIZE
       ' ul_page_nr =  datalength / FRAM_PAGE_SIZE
       ' write the address of the next page
       fram_next_page( addr )
       '  loop until all pages are written
       Do while ( ul_page_nr )
            for ul_i = 1 to FRAM_PAGE_SIZE
              ' write next byte from array
              I2CSend ( ul_data(ul_j) )
              ' inc array index
              ul_j++
            next
             ' initiate Stop Condition
             I2CStop

             ' wait 10 ms
             ' ACK polling removes the need to for the 24xxxxx device to have a 5ms write time - minimum
             ' not required
             ' I2CACKPOLL( fram_device )
             ' dec the number of pages
             ul_page_nr--
             ' inc address with 1 page
             addr = addr + FRAM_PAGE_SIZE
             ' write the address of the next page
             fram_next_page( addr )

       Loop
       ' loop through the remaining number of bytes
       Do while  ( ul_j < datalength + 1 )
        ' write next byte from array
        I2CSend ( ul_data(ul_j) )
        ul_j++
       Loop
    else

       ' write some bytes from an address that is not at a page break, but, is NOT at the page break
       ' loop through number of bytes
       for ul_i = 1 to  datalength
           ' write next byte from array
            I2CSend ( ul_data(ul_i) )
       next
    end if
else

    if (datalength > FRAM_PAGE_SIZE) then
       ' calculate the number of pages
       ul_page_nr =  ( datalength - ul_j ) / FRAM_PAGE_SIZE
       ' ul_page_nr = datalength/FRAM_PAGE_SIZE
       ' loop until all pages are written
       Do while ( ul_page_nr )
            for ul_i = 1 to FRAM_PAGE_SIZE
              ' write next byte from array
              I2CSend ( ul_data(ul_j) )
              ' inc array index
              ul_j++
            next
           ' initiate Stop Condition
           I2CStop

           ' wait 10 ms
           ' ACK polling removes the need to for the 24xxxxx device to have a 5ms write time - minimum
           ' you can remove the ICKACKPOLL and add a deley... your choice
'           I2CACKPOLL( fram_device )
           ' dec the number of pages
           ul_page_nr--
           ' inc address with 1 page
           addr = addr + FRAM_PAGE_SIZE
           ' write the address of the next page
           fram_next_page( addr )

       Loop

       ' loop through number of bytes
       Do while  ( ul_j < datalength + 1 )
        ' write next byte from array
        I2CSend ( ul_data(ul_j) )
        ul_j++
       Loop

    else
       ' execute a write of a few bytes at a page address
       ' loop through number of bytes
       for ul_i = 1 to  datalength
           ' write next byte from array
           I2CSend ( ul_data(ul_i) )
       next
    end if
end if

      ' send Stop condition
      i2cstop

End Sub


'.............................................................................
'          Reads a page from the FRAM address
'.............................................................................
sub fram_rd_page( addr as long,  out ul_data() as Byte, datalength as Byte )

'          crlf
'          SerPrint 1, "fram_rd_page"
'          crlf
'          sersend  1, 9
'          SerPrint 1, "Addr = 0x"
'          SerPrint 1, hex( addr_E )
'          SerPrint 1, hex( addr_U )
'          SerPrint 1, hex( addr_H )
'          SerPrint 1, hex( addr )
			
        dim ul_loopx as Word
        ' generate Start COndition
        I2CSTART

        ' ((0xA0|(int8)(address>>14))&0xFE);//&0xFE forces to write
        target_address = uf_shift_long ( addr, 15, >> )
        target_address = target_address  | FRAM_DEVICE_1
        target_address = target_address & 0xFE

        ' send WRITE command
        I2CSend ( target_address )
'          crlf
'          SerSend  1, 9
'          SerPrint 1, "Target = "
'          SerPrint 1, hex( target_address )
'          crlf

        addrh = uf_shift ( ( addr & 0xffff) , 8, >> )
        addrl = ( addr & 0xffff)  & 0xff

'          SerSend  1, 9
'          SerPrint 1, "Addr = 0x"
'          SerPrint 1, hex( addrh )
'          SerPrint 1, hex( addrl )
'          crlf

        'address MSB
        'comment the next line if a Low-Density (<=2kB) device is used
        I2CSEND(addrh)
        'address LSB
        I2CSEND(addrl)

        ' generate Start COndition
        I2CSTART

        'send READ command
        target_address = target_address | 0x01
        I2CSend( target_address )

        ' loop through number of bytes
        ul_i = 0
        do while ( ul_i < datalength )
                 ' write next byte to array & send ACK bit
                 ' add one to the array to ensure we dont start at element zero
                 ul_p = ul_i + 1
                 ' more date to come 'AND' NOT at end of device page
                   if ( ul_i  < (datalength ) & ( (addr & 0xFFFF ) < 0xFFFF ) ) then
                      I2CRECEIVE( ul_data(ul_p), ACK )
                  else
                      'send NACK bit
                      I2CRECEIVE( ul_data(ul_p) , NACK )
                  end if

                  ul_i++
                  addr++
                  if (addr%FRAM_PAGE_SIZE) = 0 then

'                     sersend  1,9:SerPrint 1, "Page/Device Change": crlf

                     I2CStop
                     ' write the address of the next page
                     fram_next_page( addr )
                     ' execute restart
                     I2CStart
                     'send READ command
                     target_address = target_address | 0x01
                     I2CSend( target_address )
                  end if
        loop
        ' send Stop Condition
        I2CSTOP
End Sub

'.............................................................................
'          Writes the start address of the next page
'.............................................................................
Sub fram_next_page( addr as Long )

        ' send Start condition
        I2CSTART
'                crlf
'                SerPrint 1, "fram_next_page"
'                crlf
'                SerSend  1, 9
'                SerPrint 1, "start addr = 0x"
'                SerPrint 1, hex( addr_E )
'                SerPrint 1, hex( addr_U )
'                SerPrint 1, hex( addr_H )
'                SerPrint 1, hex( addr )
'            ' ((0xA0|(int8)(address>>14))&0xFE);//&0xFE forces to write

        target_address = uf_shift_long ( addr, 15, >> )
'              crlf
'              SerPrint 1, "target_address >> 0x"
'              SerPrint 1, hex( target_address_E )
'              SerPrint 1, hex( target_address_U )
'              SerPrint 1, hex( target_address_H )
'              SerPrint 1, hex( target_address )


        target_address = target_address  | FRAM_DEVICE_1
        target_address = target_address & 0xFE

'                crlf
'                SerPrint 1, "Adress Pointer = "
'                SerPrint 1, hex( addr_E )
'                SerPrint 1, hex( addr_U )
'                SerPrint 1, hex( addr_H )
'                SerPrint 1, hex( addr )


'                  crlf
'                  SerSend  1, 9
'                  SerPrint 1, "New Target Device/Page = "
'                  SerPrint 1, hex( target_address )
'                  crlf

        ' send WRITE command
        I2CSend ( target_address )

        'comment the next line if a Low-Density (<=2kB) device is used
        addrh = uf_shift ( ( addr & 0xffff) , 8, >> )
        addrl = ( addr & 0xffff)  & 0xff

'                  SerSend  1, 9
'                  SerPrint 1, "Addr = 0x"
'                  SerPrint 1, hex( addrh )
'                  SerPrint 1, hex( addrl )
'                  crlf

        'address MSB
        'comment the next line if a Low-Density (<=2kB) device is used
        I2CSEND(addrh)
        'address LSB
        I2CSEND(addrl)

End Sub

