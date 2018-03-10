'    MicroChip specific methods to support MCP23017 device for Great Cow BASIC
'    Copyright (C) 2013 .. 2018.  Perry, Evan Venn and Jack James

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

'
'    Created Evan R Venn - Oct 2013 - 2017
'    Thanks to Perry'
'    10.03.2018     Updated to correct constants MCP23017_IOCONA and MCP23017_IOCONB




#define MCP23017_DEVICE_1 0x48

#define MCP23017_IODIRA 0x00  ' default is INput  0= out 1= in
#define MCP23017_IODIRB 0x01
#define MCP23017_IPOLA  0x02  ' input polarity 0= normal, 1= inverted
#define MCP23017_IPOLB  0x03
#define MCP23017_GPINTENA 0x04  ' Allow interrupt on change
#define MCP23017_GPINTENB 0x05
#define MCP23017_DEFVALA  0x06  ' default value BEFORE interrupt [for easy compare]
#define MCP23017_DEFVALB  0x07
#define MCP23017_INTCONA  0x08 ' I/O EXPANDER CONFIGURATION .2 1= Open Drain [wire together interrupts]
#define MCP23017_INTCONB  0x09
#define MCP23017_IOCONA  0x0A ' 0= Interrupt on change , 1= Interrupt on Difference
#define MCP23017_IOCONB  0x0B
#define MCP23017_GPPUA  0x0C ' weak pull up resistors [~100kOhhm] very handy
#define MCP23017_GPPUB  0x0D
#define MCP23017_ITFA 0x0E ' who done it, the interrupt
#define MCP23017_ITFB 0x0F
#define MCP23017_INTCAPA  0x10 ' Saves the event that caused the interrupt
#define MCP23017_INTCAPB  0x11
#define MCP23017_GPIOA  0x12 ' the IO port
#define MCP23017_GPIOB  0x13
#define MCP23017_OLATA  0x14 ' the LATCH of the IO port see docs
#define MCP23017_OLATB  0x15


sub MCP23017_sendbyte( in MCP23017_device, in MCP23017_instr , in MCP23017_data )

    ' see the Data sheet for this!!! FIGURE 1-1: MCP23017 I2C DEVICE PROTOCOL
    I2CSTART
    I2CSEND( MCP23017_device )
    I2CSEND( MCP23017_instr )
    I2CSEND( MCP23017_data )
    'initiate Stop Condition
    I2CSTOP

end sub



sub MCP23017_readbyte( in MCP23017_device, in MCP23017_instr , Out MCP23017_data )

    ' see the Data sheet for this!!! FIGURE 1-1: MCP23017 I2C DEVICE PROTOCOL
    I2CSTART
    I2CSEND( MCP23017_device )
    I2CSEND( MCP23017_instr )
    I2CSTART
    I2CSEND( MCP23017_device + 1)
    I2CReceive( MCP23017_data , NACK )
    'initiate Stop Condition
    I2CSTOP
end sub
