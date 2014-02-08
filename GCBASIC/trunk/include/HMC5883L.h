  '
  '    Created Evan R Venn - Jan 2014
  '    Beta v0.91

  ' This is the address of the generic device
  ' this is 0011110b, I2C 7bit address of HMC5883L
  #define HMC5883L_address 0x1E




  '#define HMC5843_CONFIG_REG_A                    0x00
  '#define HMC5843_CONFIG_REG_B                    0x01
  #define HMC5843_MODE_REG                        0x02
  '#define HMC5843_DATA_OUT_X_MSB_REG              0x03
  '#define HMC5843_DATA_OUT_X_LSB_REG              0x04
  '#define HMC5843_DATA_OUT_Y_MSB_REG              0x05
  '#define HMC5843_DATA_OUT_Y_LSB_REG              0x06
  '#define HMC5843_DATA_OUT_Z_MSB_REG              0x07
  '#define HMC5843_DATA_OUT_Z_LSB_REG              0x08
  ' Note: Y and Z are exchanged on HMC5883 */
  #define HMC5883_DATA_OUT_Z_MSB_REG              0x05
  #define HMC5883_DATA_OUT_Z_LSB_REG              0x06
  #define HMC5883_DATA_OUT_Y_MSB_REG              0x07
  #define HMC5883_DATA_OUT_Y_LSB_REG              0x08
  '#define HMC5843_STATUS_REG                      0x09

  ' Range gain settings in (+-)Ga
  ' Note: HMC5843 and HMC5883 have different recommended sensor field
  ' ranges; default corresponds to +-1.0 Ga and +-1.3 Ga, respectively
  '
  '#define HMC5843_RANGE_GAIN_OFFSET               0x05
  '#define HMC5843_RANGE_GAIN_DEFAULT              0x01
  '#define HMC5843_RANGE_GAIN_MAX                  0x07
  '
  '' Device status
  '#define HMC5843_DATA_READY                      0x01
  '#define HMC5843_DATA_OUTPUT_LOCK                0x02
  '' Does not exist on HMC5883, not used */
  ' #define HMC5843_VOLTAGE_REGULATOR_ENABLED       0x04
  '
  '
  '' Mode register configuration
  #define HMC5843_MODE_CONVERSION_CONTINUOUS      0x00
  '#define HMC5843_MODE_CONVERSION_SINGLE          0x01
  '#define HMC5843_MODE_IDLE                       0x02
  '#define HMC5843_MODE_SLEEP                      0x03
  '#define HMC5843_MODE_MASK                       0x03
  '
  '' HMC5843: Minimum data output rate
  '' HMC5883: Typical data output rate
  '#define HMC5843_RATE_OFFSET                     0x02
  '#define HMC5843_RATE_BITMASK                    0x1C
  '#define HMC5843_RATE_NOT_USED                   0x07
  '
  '' Device measurement configuration
  '#define HMC5843_MEAS_CONF_NORMAL                0x00
  '#define HMC5843_MEAS_CONF_POSITIVE_BIAS         0x01
  '#define HMC5843_MEAS_CONF_NEGATIVE_BIAS         0x02
  '#define HMC5843_MEAS_CONF_NOT_USED              0x03
  '#define HMC5843_MEAS_CONF_MASK                  0x03





  ' Examples #1
  ' to be done



  ' Examples #2
  ' to be done









  ;Variables
  Dim HMC5883L_x As word
  Dim HMC5883L_y As word
  Dim HMC5883L_z As word



  Sub Init_HMC5883L ( Optional DeviceOpMode As Byte = HMC5843_MODE_CONVERSION_CONTINUOU )

     I2CSTART
     I2CSEND ( HMC5883L_address )
     I2CSEND ( HMC5843_MODE_REG )
     I2CSEND ( DeviceOpMode )
     I2CSTART

  end sub



  Sub Read_HMC5883L ( out HMC5883L_x, Out HMC5883L_y, Out HMC5883L_z)

     I2CSTART
     I2CSEND ( HMC5883L_address )
     I2CSEND ( HMC5843_MODE_CONVERSION_CONTINUOUS )
     I2CSTART

     I2CReceive ( HMC5883L_x_h )
     I2CReceive ( HMC5883L_x   )
     I2CReceive ( HMC5883L_z_h )
     I2CReceive ( HMC5883L_z   )
     I2CReceive ( HMC5883L_y_h )
     I2CReceive ( HMC5883L_y, NACK )
     I2CStop

  end sub
