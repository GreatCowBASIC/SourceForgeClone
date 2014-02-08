'
'    Created Evan R Venn - Oct 2013
'    Beta v0.84
'
'    0.81    Initial release
'    0.82    Corrected ACKPOLLCALL. Should be the device not some random constant.
'    0.83    Correct page swaps and correct read operations when using rd_page
'    0.84    Change PAGE_SIZE to EEPROM_PAGE_SIZE to ensure other devices do not clash
'    0.85    Corrected page variables names.




'EEPROM Page Size in bytes, this is stated in decimal
#define EEPROM_PAGE_SIZE          64
'EEPROM Number of Pages, this is stated in decimal
#define EEPROM_PAGE_NUMBER         512
'WRITE instruction
#define EEPROM_DEVICE_1           0xA0


'.............................................................................
'          Writes data to the EEPROM address
'.............................................................................
Sub eeprom_wr_byte ( in eeprom_device as byte, in w_addr as word, In data_in as byte)
      'generate Start COndition
        I2CSTART
       'send WRITE command
        I2CSEND( eeprom_device , ACK)
       'comment the next line if a Low-Density (<=2kB) device is used
        w_addrH = uf_shift ( w_addr, 8, >> )
        w_addrL = w_addr & 0xff

        I2CSEND(w_addrH, ACK)
        I2CSEND(w_addrL, ACK)
        I2CSEND( data_in , ACK)
        I2CSTOP
        ' ACK polling removes the need to for the 24xxxxx device to have a 5ms write time - minimum
        ' you can remove the ICKACKPOLL and add a deley... your choice
        I2CACKPOLL( eeprom_device )

end sub




'.............................................................................
'          Reads a byte from the EEPROM address
'.............................................................................
Sub eeprom_rd_byte( in eeprom_device as byte, w_addr as word , Out ul_returnedbyte as byte )

      I2CSTART
      I2CSEND( eeprom_device )
      w_addrH = uf_shift ( w_addr, 8, >> )
      w_addrL = w_addr & 0xff
      I2CSEND(w_addrH)
      I2CSEND(w_addrL)
      I2CSTART
      I2CSEND( eeprom_device + 1 )
      ul_returnedbyte  = 0
      I2CRECEIVE ul_returnedbyte  , NACK
      I2CSTOP

end sub




'.............................................................................
'         Writes a page to the EEPROM address
'.............................................................................
sub eeprom_wr_page( in eeprom_device, in w_addr as word, in ul_data() as Byte, in datalength as byte )
    dim ul_j as byte
    dim ul_i as byte

    ul_i = 0
    ul_j = 1
    ul_page_nr = 0
    ' generate Start COndition
    i2cstart
    ' send WRITE command
    I2CSend (eeprom_device)
    'comment the next line if a Low-Density (<=2kB) device is used
    addrh = uf_shift ( w_addr, 8, >> )
    addrl = w_addr & 0xff
    I2CSEND(addrh)
    I2CSEND(addrl)

    ul_mod = w_addr%EEPROM_PAGE_SIZE

    ul_pagespace =  EEPROM_PAGE_SIZE - ul_mod

if ( ul_mod <> 0 ) then

    if ( datalength > ul_pagespace ) then
'       ' Loop through number of bytes
       ul_i = 0
       while ( ul_i < ul_pagespace )
          ul_i++
          ' write next byte from array
          I2CSend ( ul_data(ul_j) )
          ' 1) inc array index
          ul_j++
       Loop
       ' initiate Stop Condition
       I2CStop
       ' ACK polling removes the need to for the 24xxxxx device to have a 5ms write time - minimum
       ' you can remove the ICKACKPOLL and add a delay... your choice
       I2CACKPOLL( eeprom_device )
       ' You check the error state, see I2CAckPollState
       ' go to the start of the next page
       w_addr = w_addr + (EEPROM_PAGE_SIZE-(w_addr%EEPROM_PAGE_SIZE))

       'calculate the number of pages
       ul_page_nr =  ( datalength - ul_j  ) / EEPROM_PAGE_SIZE
       ' write the address of the next page
       eeprom_next_page( w_addr )
       '
       '  loop until all pages are written
       Do while ( ul_page_nr )
            for ul_i = 1 to EEPROM_PAGE_SIZE
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
             I2CACKPOLL( eeprom_device )
             ' You check the error state, see I2CAckPollState

             ' dec the number of pages
             ul_page_nr--
             ' inc address with 1 page
             w_addr = w_addr + EEPROM_PAGE_SIZE

             ' write the address of the next page
             eeprom_next_page( w_addr )
       Loop
       ' loop through the remaining number of bytes
       Do while  ( ul_j < datalength + 1 )
        ' write next byte from array
        I2CSend ( ul_data(ul_j) )
        ul_j++
       Loop
    else
       ' write some bytes from an address that is not at a page break, but, is NOT longer than a page
       ' loop through number of bytes
       for ul_i = 1 to  datalength
           ' write next byte from array
            I2CSend ( ul_data(ul_i) )
       next
    end if
else
    if (datalength > EEPROM_PAGE_SIZE) then
       ' calculate the number of pages
       ul_page_nr =  ( datalength - ul_j  ) / EEPROM_PAGE_SIZE
       ' loop until all pages are written
       Do while ( ul_page_nr )
            for ul_i = 1 to EEPROM_PAGE_SIZE
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
           I2CACKPOLL( eeprom_device )
           ' You check the error state, see I2CAckPollState

           ' dec the number of pages
           ul_page_nr--
           ' inc address with 1 page
           w_addr = w_addr + EEPROM_PAGE_SIZE

           ' write the address of the next page
           eeprom_next_page( w_addr )

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
      ' ACK polling
      I2CACKPOLL( eeprom_device )
      ' You check the error state, see I2CAckPollState

End Sub


'.............................................................................
'          Reads a page from the EEPROM address
'.............................................................................
sub eeprom_rd_page( in eeprom_device, in w_addr as word,  out ul_data() as Byte, in datalength as Byte )
			
        dim ul_loopx as Word
        Dim ul_p as Word
        I2CSTART
        I2CSend( eeprom_device )
        'comment the next line if a Low-Density (<=2kB) device is used
        addrh = uf_shift ( w_addr, 8, >> )
        addrl = w_addr & 0xff
        I2CSEND(addrh)
        I2CSEND(addrl)
        I2CSTART
        I2CSend( eeprom_device + 1 )

        ' loop through number of bytes
        ul_loopx = 0
        do while ( ul_loopx < datalength )
                 ' write next byte to array & send ACK bit
                 ' add one to the array to ensure we dont start at element zero
                 ul_p = ul_loopx + 1
                  if ( ul_loopx  < (datalength - 1 ) ) then
                      I2CRECEIVE( ul_data(ul_p), ACK )
                  else
                      'send NACK bit
                      I2CRECEIVE( ul_data(ul_p) , NACK )
                  end if
                  ul_loopx++
        loop
        I2CSTOP
End Sub

'.............................................................................
'          Writes the start address of the nexr page
'.............................................................................
Sub eeprom_next_page( in w_addr as word )

        wait 10 ms
        I2CSTART()
        I2CSend( eeprom_device )
        'comment the next line if a Low-Density (<=2kB) device is used
        addrh = uf_shift ( w_addr, 8, >> )
        addrl = w_addr & 0xff
        I2CSEND(addrh)
        I2CSEND(addrl)

End Sub

