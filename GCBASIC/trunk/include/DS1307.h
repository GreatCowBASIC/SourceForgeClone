'
'    Created Evan R Venn - Oct 2013
'    Beta v0.81
'    Beta v0.82 - completed routines, all tested
'    Beta v0.83 - correct NACK in Enab_Osc
'    0.84 corrected errors in ENABLE and DISABLE clock routines

 ' This is the address of the generic DS1307
 #define DS1307_AddrWrite 0xD0
 #define DS1307_AddrRead 0xD1





' Examples #1
'DS1307_Dis_Osc
'DS1307_Enab_Osc
'DS1307_ResetClock
'DS1307_SetClock ( 10, 0, 0 , 7, 5, 10, 13 )
'DS1307_SetTime ( 23, 59, 57 )
'DS1307_SetDate ( 5, 10, 13 )
'DS1307_ReadTime( Hour , Min , Sec)




' 0x71 in the Hours register would make it 11 PM
' 0x51 in the Hours register would make it 11 AM
'
' More detail:
' 0x51 = %0101 0001 <-- 1's position of hour
'        ||||_________ 10's position of hour. 10+1 = 11 O-clock
'        |||__________ 1 = PM, 0 = AM (in 12-hour mode)
'        ||___________ 1 = 12-hour mode, 0 = 24-hour mode
'        |____________ ignore this bit
'
' Set time & date to  11:59:00, Day 2, Date:Month:Year 30:08:2007

' Examples #2
'DS1307_SetTime ( 0x51, 59, 57 )
'DS1307_ReadTime( Hour , Min , Sec)
'DS1307_SQWEnable ( 1 )


' DS1307_Enab_Osc
' DS1307_Dis_Osc
' DS1307_ResetClock
' DS1307_SetClock ( 10, 0, 0 , 7, 5, 10, 13 )
' DS1307_SetTime ( 23, 59, 57 )
' DS1307_SetDate ( 5, 10, 13 )
' DS1307_ReadClock (out Hour , out Min , out Sec , out DayOfWeek , out CDate , out CMonth , out CYear)
' DS1307_ReadTime( Hour , Min , Sec)
' DS1307_ReadDate ( out CDate , out CMonth , out CYear)


' DS1307_SQWEnable (Optional RateSelect as Byte = 1 )
 ' Enables The SQW/OUT pin of chip
 ' If RateSelect=1 then the outpout Frequency is 1Hz
 ' If RateSelect=4 then the outpout Frequency is 4.096KHz
 ' If RateSelect=8 then the outpout Frequency is 8.192KHz
 ' If RateSelect=32 then the outpout Frequency is 32.768KHz

' DS1307_SQDisable

' DS1307_WriteReg ( DS1307Reg, DS1307Value )
' DS1307_ReadRam  ( DS1307Reg, DS1307Value )

















;Variables
 Dim Sec As byte
 Dim Min As byte
 Dim Hour As byte
 Dim DayOfWeek as byte
 Dim CDate as byte
 Dim CMonth as byte
 Dim CYear as Byte
 Dim va as byte
 va=0
 Sec=0
 Min=0
 Hour=0
 CDate=0
 CMonth=0
 CYear=0




 'Clear Bit 7 of 00h register. Oscillator is enabled, clock start
 'On first application of power to the device the time and date registers
 'are typically reset to 01/01/00 01 00:00:00 (MM/DD/YY DOW HH:MM:SS).
 'The CH bit in the seconds register will be set to a 1
 Sub DS1307_Enab_Osc

     I2CSTART
     I2CSEND ( DS1307_AddrWrite )
     I2CSEND ( 0 )
     I2CSTART
     I2CSEND ( DS1307_AddrRead )
     I2CReceive ( sec, NACK )
     Sec=BcdToDec( Sec )
     Sec=  Sec & 127
     I2CSTART
     I2CSEND ( DS1307_AddrWrite )
     I2CSEND ( 0 )
     I2CSEND ( DecToBcd(sec) )
     I2CStop


 end sub

'Set Bit 7 of 00h register. Oscillator is disabled, clock stop
 Sub DS1307_Dis_Osc

     I2CSTART
     I2CSEND ( DS1307_AddrWrite )
     I2CSEND ( 0 )
     I2CSTART
     I2CSEND ( DS1307_AddrRead )
     I2CReceive ( sec , NACK)
     Sec= BcdToDec(Sec)
     Sec = Sec | 128
     I2CSTART
     I2CSEND ( DS1307_AddrWrite )
     I2CSEND ( 0 )
     I2CSEND ( DecToBcd(sec) )
     I2CStop


 End sub

' Resets the clock to 00:00:00 01 01/01/00
 Sub DS1307_ResetClock
     I2CSTART
     I2CSEND( DS1307_AddrWrite )
     I2CSEND( 0 )
     I2CSEND( 0 )
     I2CSEND( 0 )
     I2CSEND( 0 )
     I2CSEND( 1 )
     I2CSEND( 1 )
     I2CSEND( 1 )
     I2CSEND( 0 )
     I2CStop

 end sub

'Self explained
 Sub DS1307_SetClock (In Hour , In Min , In Sec , In DayOfWeek , In CDate , in CMonth , in CYear)

     I2CSTART
     I2CSEND( DS1307_AddrWrite )
     I2CSEND( 0 )
     I2CSEND( DecToBcd(Sec) )
     I2CSEND( DecToBcd(Min) )
     I2CSEND( DecToBcd(Hour )
     I2CSEND( DecToBcd(DayOfWeek) )
     I2CSEND( DecToBcd(CDate) )
     I2CSEND( DecToBcd(CMonth) )
     I2CSEND( DecToBcd(CYear) )
     I2CStop


 End sub

'Self explained
 Sub DS1307_SetTime (in Hour as byte , in Min as byte ,  l_Sec as byte)

     I2CSTART
     I2CSEND( DS1307_AddrWrite )
     I2CSEND( 0 )
     I2CSEND( DecToBcd(l_Sec) )
     I2CSEND( DecToBcd(Min) )
     I2CSEND( DecToBcd(Hour) )
     I2CStop

 End Sub

'Self explained
 Sub DS1307_SetDate (In CDate , in CMonth , in CYear)

     I2CSTART
     I2CSEND( DS1307_AddrWrite )
     I2CSEND( 4 )
     I2CSEND( DecToBcd(CDate) )
     I2CSEND( DecToBcd(CMonth) )
     I2CSEND( DecToBcd(CYear) )
     I2CStop

 End Sub

'Self explained
 Sub DS1307_ReadClock (out Hour , out Min , out Sec , out DayOfWeek , out CDate , out CMonth , out CYear)

     I2CStart
     I2CSEND( DS1307_AddrWrite )
     I2CSEND( 0 )
     I2CStop
     I2CStart
     I2CSEND( DS1307_AddrRead )
     I2CReceive ( Sec )
     Sec=BcdToDec(Sec & 127)
     I2CReceive ( Min )
     Min=BcdToDec(Min & 127)
     I2CReceive ( Hour )

     if Hour.6 then
     		Hour=BcdToDec(Hour &31)
     Else
     		Hour=BcdToDec(Hour & 63)
     End if

     I2CReceive ( DayOfWeek )
     DayOfWeek=BcdToDec(DayOfWeek & 7)
     I2CReceive ( CDate )
     CDate=BcdToDec(CDate & 63)
     I2CReceive ( CMonth )
     CMonth=BcdToDec(CMonth & 31)
     I2CReceive ( CYear , NACK )
     CYear=BcdToDec(CYear)
     I2CStop
 End Sub

'Self explained
 Sub DS1307_ReadTime (Out Hour , Out Min , Out Sec)

     I2CStart
     I2CSEND( DS1307_AddrWrite )
     I2CSEND( 0 )
     I2CStart
     I2CSEND( DS1307_AddrRead )
     I2CReceive ( Sec )
     Sec=BcdToDec(Sec & 127)
     I2CReceive ( Min )
     Min=BcdToDec(Min & 127)
     I2CReceive ( Hour , NACK )

     if Hour.6 then
     		Hour=BcdToDec(Hour &31)
     Else
     		Hour=BcdToDec(Hour & 63)
     End if
     I2CStop


 End sub

'Self explained
 Sub DS1307_ReadDate ( out CDate , out CMonth , out CYear)

     I2CStart
     I2CSEND( DS1307_AddrWrite )
     I2CSEND( 4 )
     I2CStart
     I2CSEND( DS1307_AddrRead )
     I2CReceive ( CDate )
     CDate=BcdToDec(CDate & 63)
     I2CReceive ( CMonth )
     CMonth=BcdToDec(CMonth & 31)
     I2CReceive ( CYear , NACK )
     CYear=BcdToDec(CYear)
     I2CStop

 end sub

 'Set Bit 6 of 02h register. The DS1307 run in 12-hour mode
 'The hours value must be re-entered whenever the 12/24-hour mode bit is changed.
 Sub DS1307_Set12HourMode

     I2CStart
     I2CSEND( DS1307_AddrWrite )
     I2CSend ( 2 )
     I2CStart
     I2CSEND( DS1307_AddrRead )
     I2CReceive ( Hour , NACK )
     Hour.6=1
     I2CStart
     I2CSEND( DS1307_AddrWrite )
     I2CSend ( 2 )
     I2CSend ( Hour )
     I2CStop

 End sub

 'Clear Bit 6 of 02h register. The DS1307 run in 24-hour mode
 'The hours value must be re-entered whenever the 12/24-hour mode bit is changed.
 Sub DS1307_Set24HourMode

     I2CStart
     I2CSEND( DS1307_AddrWrite )
     I2CSend ( 2 )
     I2CStart
     I2CSEND( DS1307_AddrRead )
     I2CReceive ( Hour, NACK )
     Hour.6=0
     I2CStart
     I2CSEND( DS1307_AddrWrite )
     I2CSend ( 2 )
     I2CSend ( Hour )
     I2CStop

 End sub

 ' Enables The SQW/OUT pin of chip
 ' If RateSelect=1 then the outpout Frequency is 1Hz
 ' If RateSelect=4 then the outpout Frequency is 4.096KHz
 ' If RateSelect=8 then the outpout Frequency is 8.192KHz
 ' If RateSelect=32 then the outpout Frequency is 32.768KHz
 Sub DS1307_SQWEnable (Optional RateSelect as Byte = 1 )

     I2CStart
     I2CSEND( DS1307_AddrWrite )
     I2CSend ( 7 )
     Select case RateSelect
         Case 0
              I2CSend ( 0 )
         Case 1
              I2CSend ( 16 )
         Case 4
              I2CSend ( 17 )
         Case 8
              I2CSend ( 18 )
         Case 32
              I2CSend ( 19 )
         case else
              I2CSend ( 16 )
     End Select
     I2CStop

 End sub

 'Disable the SQW/OUT pin of chip (pin=off)
 Sub DS1307_SQDisable

     I2CStart
     I2CSEND( DS1307_AddrWrite )
     I2CSend ( 7 )
     I2CSend ( 0 )
     I2CStop

 End Sub


' 56 bytes of NV SRAM from 0x08 to 0x3f
' 56-Byte, Battery-Backed, General-Purpose RAM with Unlimited Writes
'The RTC registers are located in address locations 00h to 07h. The RAM registers are located in address locations 08h to 3Fh. During a multibyte access, when the address pointer reaches 3Fh, the end of RAM space, it wraps around to location 00h, the beginning of the clock space.

 Sub DS1307_WriteRam (  in l_DS1307Reg, in l_DS1307Value)

      I2CStart
      I2CSEND( DS1307_AddrWrite )
      I2CSend ( l_DS1307Reg )
      I2CSend ( l_DS1307Value )
      I2CStop

 End Sub



Sub DS1307_ReadRam ( in DS1307Reg, Out l_DS1307Value )

      I2CStart
      I2CSEND( DS1307_AddrWrite )
      I2CSend ( DS1307Reg )

      I2CStart
      I2CSEND( DS1307_AddrRead )
      I2CReceive ( l_DS1307Value, NACK )
      I2CStop

End Sub

 'The contents of the time and calendar registers are in the BCD format.
 Function DecToBcd(va) as Byte
 		DecToBcd=(va/10)*16+va%10
 End Function

 Function BcdToDec(va) as byte
 		BcdToDec=(va/16)*10+va%16
 End Function



 Sub DS1307_WriteReg (DS1307Reg, DS1307Value)

     I2CSTART
     I2CSEND( DS1307_AddrWrite )
     I2CSEND( DS1307Reg )
     I2CSEND( DS1307Value )
     I2CStop

 End Sub





