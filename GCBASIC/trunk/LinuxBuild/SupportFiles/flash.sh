##  Version 0.95.001
##  Adapted for use on Linux from flashAVR.bat and flashPIC.bat. Not to be used on Windows.
##  WARNING: Not tested!
##  You can adapt this file to your needs including removing the .sh extension for convenience.
## In this script, "$1" gets replaced with whatever you enter for <hexfile> on the command line. $2 gets replaced with the value entered for <chipmodel>.

## Be sure to uncomment/edit the appropriate flash command line for your programmer.

## Changes @ August 2015
## Changed AVRDude calls.  The first call prevents errors when trying to flash AVRs in delivery status (1MHz).
## The second gives the maximum speed.
## Both does not affect the speed of the firmware in most cheapy china programmers. But those with the actual
## USBASP firmware (2011-05-28) benefit from. Actually most of the chinese USBASPs can be updated (needs two programmers).

basename=${0##*/}

if [ $# -ne 2 ]; then
	echo "Usage: $basename <hexfile> <chipmodel>"
	echo "Example: $basename myfile.hex tiny44"
	echo
	echo "Note: If calling this script from the gcbasic command line"
	echo "using the %FILENAME% and %CHIPMODEL% placeholders,"
	echo "the appropriate values will be populated automatically."
	exit
fi

##  --- Your code here: ---------------------------------------

## *** For AVR ***

##  Call AVRdude for USBASP programmer - moderate speed (187.5 kHz) for 1 MHz devices:
#avrdude -B 5 -c usbasp -P /dev/cuaa0 -p $2 -U flash:w:"$1":i

##  Call AVRdude for USBASP programmer - fastest mode (1500 kHz):
 # avrdude -B 0.5 -c usbasp -P /dev/cuaa0 -p $2 -U flash:w:"$1":i

##  Call AVRdude for STK500v2 programmer:
 # avrdude -c stk500v2 -P com4 -p $2 -U flash:w:"$1":i

##  Call AVRdude for DASA3 programmer:
 # avrdude -i 1 -c dasa3 -P com1 -p $2 -U flash:w:"$1":i

## Call AVRdude for Arduino_Uno bootloader:
avrdude -c arduino -P /dev/ttyUSB0 -b 115200 -p $2 -U flash:w:"$1":i

## Call AVRdude for Arduino_Pro_Mini bootloader:
 # avrdude -c arduino -P com6 -b 57600 -p $2 -U flash:w:"$1":i

##  Call AVRdude for Arduino_Mega2560 bootloader:
 # avrdude -c wiring -P com7 -b 115200 -p $2 -U flash:w:"$1":i

## Call for Digispark bootloader:
## Replace the command name appropriately, and
## remove the '#' from the next three lines
 # Echo "ReInsert Digispark USB now"
 # "micronucleus.exe" -run  "$1"
 # timeout 5 > nul


 ## *** For PIC ***
## ! Not tested. Be sure to edit the commands appropriately. You may need to add or remove double-quotes around the $ variables.

## Call uploader for TinyBootLoader+ bootloader:
 # TinyMultiBootloader+ "$1" exit

##  Call PICpgm-programming software:
 # picpgm -p "$1"
 # picpgm -delay 1 -if "5PiP" -port COM1 -pic PIC$2 -p "$1"
 # picpgm -delay 1 -if "JDM Programmer" -port COM1 -pic PIC$2 -p "$1"
 # picpgm -delay 1 -if "FT232Board-Programmer" -port USB -pic PIC$2 -p "$1"
 # picpgm -delay 1 -if "FT232Adapter-Programmer" -port USB -pic PIC$2 -p "$1"

##  Call PicKit2 command-line programming Software leaving power on the chip
##  and always exit with MCLR on. Optionally #ove -r for MCLR off
 #pk2cmd -bPicKit2 -pPIC$2 -f"$1" -m -j -t -r

##  Call PicKit2 command-line programming Software without leaving the power on the chip
##  and always exit with MCLR on. Optionally #ove -r for MCLR off
 # pk2cmd -bPicKit2 -pPIC$2 -f"$1" -m -r

##  Call PicKit3 command-line programming software:
##  note, that the Java Runtime Environment (jre) has to be installed!
 # java -jar ipecmd.jar -TPPK3 -P$2 -M -F"$1"

##  Use PicKit3 Integrated Programming Environment GUI (IPE):
##  note, that IPE must run and a programmer and a PIC is connected!
 #  IPE-Pusher "$1" 0

##  Call uploader for diolan bootloader:
 #  picboot -r "$1"

##  Call MicroBrn K150 programming software:
 # microbrn "$1" $2 /f /Q 1

##  --- End of your code. ------------------------------------

if [ $? -ne 0 ]; then
	echo "ERROR $? running: $0 with parameters $*"
fi
