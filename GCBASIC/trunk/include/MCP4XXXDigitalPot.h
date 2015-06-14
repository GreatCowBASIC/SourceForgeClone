'''A program  for GCGB and GCB.
'''--------------------------------------------------------------------------------------------------------------------------------
'''This program is for the Digital potentiometer 2.7 to 5.5V 7-bit linear digital potentiometer with SPI interface.
'''Wiper commands can be write or read. This program is write and reads commands.
'''MOSI/MISO are multiplexed in the Digital potentiometer side.
'''
'''This library is broadly compatible with the other devices in this series.
'''They are: Microchip MCP4131, MCP4132, MCP4231, MCP4232, MCP4141, MCP4142, MCP4241, MCP4242, MCP4151, MCP4152, MCP4251, MCP4252, MCP4161, MCP4162, MCP4261, MCP4262.
'''
'''  #define _MCP4XXXHardwareSPI     ' remove or comment out if you want to use software SPI.
'''	 #define _MCP4XXXSoftwareSPI    ' remove or comment out if you want to use software SPI.
'''	 You need to define for hardware or software:
'''				DigitalPot_CS  <port>
'''				DigitalPot_SCK  <port>
'''				DigitalPot_DO  <port>
'''				DigitalPot_DI  <port>
'''	You need to include tis file
'''		   #include <MCP4XXXDigitalPot.h>
'''	And, you must call for either hardware or software:
'''			MCP4XXXInit
'''	And, for hardware SPI support you must call:
'''				SPIMode MasterFast, 0
''':
''':
'''@author 		EvanV
'''@licence		GPL
'''@version		1.00
'''@date   		June 2015
'''********************************************************************************




  #ifdef AVR
    'For AVR
    #define DigitalPot_CS 	DIGITAL_10
    #define DigitalPot_SCK 	DIGITAL_13
    #define DigitalPot_DO 	DIGITAL_11
    #define DigitalPot_DI 	DIGITAL_12
  #endif

  #ifdef PIC
    'For PIC
    #define DigitalPot_CS 	portd.3
    #define DigitalPot_SCK 	portc.3
    #define DigitalPot_DO 	portc.5
    #define DigitalPot_DI 	portc.4
  #endif

	#define DigitalPot_ADR_WIPER0       0B00000000
  #define DigitalPot_ADR_WIPER1       0B00010000

  #define DigitalPot_CMD_READ         0B00001100
  #define DigitalPot_CMD_WRITE        0B00000000

  #define DigitalPot_ADR_VOLATILE     0B00000000
  #define DigitalPot_ADR_NON_VOLATILE 0B00000010

  #define DigitalPot_TCON_REGISTER    0B00000100
  #define DigitalPot_STATUS_REGISTER  0B00000101

  #define DigitalPot_VOLATILE_INC   	0B00000100
	#define DigitalPot_VOLATILE_DEC   	0B00001000


sub MCP4XXXInit

  #ifdef _MCP4XXXSoftwareSPI

	' Four ports are required for SW SPI
    Dir DigitalPot_DI in
    Dir DigitalPot_DO Out
    Dir DigitalPot_SCK Out
    Dir DigitalPot_CS out
	' Initial state of ports required for SW SPI
    set DigitalPot_DO off
    set DigitalPot_SCK off
    set DigitalPot_CS off

  #endif


  ' Required if you are using HARDWARE SPI.
  #ifdef _MCP4XXXHardwareSPI
    Dir DigitalPot_DI in
    Dir DigitalPot_DO Out
    Dir DigitalPot_SCK Out
    Dir DigitalPot_CS out
    SPIMode MasterFast, 0
  #endif



end sub

'Returns a 9bit value
function DigitalReadMemory ( optional in MCPValue = DigitalPot_ADR_WIPER0 ) as Word

    MCPTemp = DigitalPot_CMD_READ + ( MCPValue * 16 )

    if ( ( MCPValue <> DigitalPot_TCON_REGISTER ) and  ( MCPValue <> DigitalPot_STATUS_REGISTER ) ) then
      MCPTemp = MCPTemp or 0x01
    end if

    Set DigitalPot_CS Off
    SPITransfer( MCPTemp, DigitalReadMemory_h )  'Send command
    SPITransfer( 0xff, [BYTE]DigitalReadMemory ) 'Data
    Set DigitalPot_CS On
    'Return value
    DigitalReadMemory   = DigitalReadMemory & 0x01ff

end Function


#define DigitalPotSetWiperPosition DigitalWriteMemory
sub  DigitalWriteMemory ( in MCPValue as Word, optional MCPAddress = DigitalPot_ADR_VOLATILE )

    Dim MCPTemp as Word
    MCPTemp = 0
    MCPTemp_h = ( MCPAddress *16 )
    MCPTemp =  MCPTemp + ( MCPValue & 0x1FF )

    Set DigitalPot_CS Off
    SPITransfer( ( MCPTemp_h ) , temp )
    SPITransfer( [byte]MCPTemp , temp )
    Set DigitalPot_CS On
    'Delaying when  eeprom address memory
    if ( ( MCPAddress = 2 ) or ( MCPAddress = 3 ) or ( MCPAddress > 5 ) ) then
        wait 3 ms
        wait while DigitalReadMemory ( 5 ).4 = 1
    end if
end sub


sub  DigitalPotIncrement
    Set DigitalPot_CS Off
    SPITransfer(DigitalPot_VOLATILE_INC, MCPValue)
    Set DigitalPot_CS On
end sub

sub  DigitalPotDecrement
    Set DigitalPot_CS Off
    SPITransfer(DigitalPot_VOLATILE_DEC, MCPValue)
    Set DigitalPot_CS On
end sub


#define SPITransfer SWSPITransfer
sub  SWSPITransfer( IN DigitalPotSendByte as byte, OUT DigitalPotTempOut as byte )

  #ifdef _MCP4XXXHardwareSPI
     HWSPITransfer  DigitalPotSendByte,  DigitalPotTempOut
     exit sub
  #endif

  #ifdef _MCP4XXXSoftwareSPI
    Dir DigitalPot_DI In
    Dir DigitalPot_DO Out
    Dir DigitalPot_SCK Out
    Dir DigitalPot_CS out

  	DigitalPotTempOut = 0
    set DigitalPot_SCK Off

    repeat 8

      if DigitalPotSendByte.7 = ON  then
        set DigitalPot_DO ON
      else
        set DigitalPot_DO OFF
      end if

      'Device is cpol = 0. Invert SCK if cpol = 1
      SET DigitalPot_SCK ON
      rotate DigitalPotSendByte left


			rotate DigitalPotTempOut left
      if DigitalPot_DI = On then
        set DigitalPotTempOut.0 On
      Else
      	set DigitalPotTempOut.0 Off
      end if

      SET DigitalPot_SCK OFF

    end repeat

  #endif

end Sub
