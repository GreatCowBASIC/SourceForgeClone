'
'    Created Evan R Venn - Feb 2015

'    PCF8574.h




#define PCF8574_DEVICE_1 0x4E


sub PCF8574_sendbyte( in PCF8574_device, in PCF8574_data )

    I2CSTART
    I2CSEND( PCF8574_device )
    I2CSEND( PCF8574_data )
    'initiate Stop Condition
    I2CSTOP

end sub



sub PCF8574_readbyte( in PCF8574_device, Out PCF8574_data )

    ' see the Data sheet for this!!! FIGURE 1-1: PCF8574 I2C DEVICE PROTOCOL
    I2CSTART
    I2CSEND( PCF8574_device )
    I2CSTART
    I2CSEND( PCF8574_device + 1)
    I2CReceive( PCF8574_data , NACK )
    'initiate Stop Condition
    I2CSTOP
end sub
