BEGIN {
# store store = " Store Data" for EEPROM within PC
# store = " Store Data"
# store = "" is you are going to store in External EEPROM
Array[0]="Max Records";
FS=","
charcount=1
_ord_init()
}
{
         gsub($0,"\n","")
         if ( trim($0) != "" && length(trim($0)) > 0 ) {
            numofrecords = length(Array)
	  Array[numofrecords ] = sprintf("%02X",numofrecords)","sprintf("%02X",$1)","sprintf("%02X",length($2))","$2;
#            Array[numofrecords ] = sprintf("%02X",numofrecords)","sprintf("%02X",$1)","sprintf("%02X",length(trim($2)))","trim($2);
         }
}
END {

          tablecount = 1
          printf ("Table StringDataSource%s\n", tablecount, store )

	numofrecords = length(Array)-1;
          if ( numofrecords > 255 ) {
             printf "Error to many strings\n"
          }
          Array[0]=sprintf("%02X",numofrecords)
	# This is the main processing loop
	for ( ptr=0; ptr <= numofrecords; ptr++) {

          #    printf ("%s\n", Array[ptr] );
               if ( ptr == 0 ) {
                  printf ("0x%s 'Total number of strings processed\n", Array[ptr] );
                  charcount = 1
                  totalcharcount = 1
               }
               else {
                    # calc the length of the next string to be processed
                    stringl = sprintf ("0x%s\n",substr(Array[ptr],7,2))
                    stringl= strtonum ( stringl )

                     # output new/another table header as the will be 250 chars MAX
                     if ( (stringl + charcount) > 248 ) {
                          printf "End Table\n"
                          printf ("'\t Number of chars in table = %s\n\n", charcount)
                          charcount = 1
                          # increment table count
                          tablecount++
                          printf ("Table StringDataSource%s %s\n", tablecount, store )
                     }



                     # outout the string index and then the string length
                    printf ("0x%s ' Index number\n", substr(Array[ptr],1,2) )
                    printf ("0x%s ' Length\n", substr(Array[ptr],7,2) )
                    charcount = charcount + 2
                    totalcharcount = totalcharcount + 2

                    for (loopx=0;loopx < stringl ; loopx++) {
                         #printf ("0x%s\n", ord(substr(Array[ptr],10+loopx,1) ) )
                         #printf ("0x%s\n", ord( "A") )

                         printf ("0x%02X' %s\n", ord(substr(Array[ptr],10+loopx,1)), substr(Array[ptr],10+loopx,1)  )
                         charcount++
                         totalcharcount++
                    }

               }
          }
          printf "End Table\n"
          printf "'Number of chars in table = "charcount-1"\n"
          if ( tablecount > 1 ) {
             printf "'Multiple Tables created\n"
          }

          printf "'Number of chars in all tables = "totalcharcount-1"\n"

#            table1exists = 0
#            ReadTable StringDataSource1, 0, table1exists
#
#            if table1exists <> 0 then
#              ReadTable StringDataSource1, 0, tablelength
#              for cc = 0 to tablelength
#                ReadTable StringDataSource1, cc, char
#                eeprom_wr_byte( EEPROM_DEVICE_1, cc, char)
#              next
#            end if


          printf "dim cc as word\n"
          printf "dim eepromaddress as word\n"
          printf "eepromaddress = 0 \n"

          for ( loopx = 1;loopx <= tablecount; loopx++) {
              # output GCB code
              printf ("table%sexists = 0\n",loopx)
              printf ("ReadTable StringDataSource%s, 0, table%sexists\n",loopx ,loopx )
              printf ("if table%sexists <> 0 then\n",loopx)
              printf ("\tReadTable StringDataSource%s, 0, tablelength\n",loopx)
              printf ("\tfor cc = 0 to tablelength\n")
              printf ("\t\tReadTable StringDataSource%s, cc, char\n",loopx)
              printf ("\t\teeprom_wr_byte( EEPROM_DEVICE_1, eepromaddress, char)\n")
              printf "\t\teepromaddress++\n"
              printf ("\tnext\n")
              printf ("end if\n")
          }
              printf ("\n\nEND\n")
              printf "'Number of chars in all tables = "totalcharcount-1"\n"










}


# general string handling functions
function ltrim(s){ sub(/^[ \t]+/, "", s); return s }
function rtrim(s){ sub(/[ \t]+$/, "", s); return s }
function trim(s){ return rtrim(ltrim(s)); }

func _ord_init(    low, high, i, t) {
      low = sprintf("%c", 7) # BEL is ascii 7
      if (low == "\a") {     # regular ascii
        low = 0
        high = 127
       }
      else if (sprintf("%c", 128 + 7) == "\a") {
        # ascii, mark parity
        low = 128
        high = 255
        }
      else {                  # ebcdic(!)
        low = 0
        high = 255
       }

    for (i = low; i <= high; i++) {
      t = sprintf("%c", i)
      _ord_[t] = i
       }
    }

  func ord(str,    c) {
    # only first character is of interest
    c = substr(str, 1, 1)
    return _ord_[c]
    }
