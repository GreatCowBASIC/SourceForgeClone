'    Software I2C routines for the GCBASIC compiler
'    Copyright (C) 2013 Evan R. Venn
'    Copyright (C) 2014 Thomas Henry
'    Copyright (C) 2014 Evan R. Venn
'
'    This library is free software' you can redistribute it and/or
'    modify it under the terms of the GNU Lesser General Public
'    License as published by the Free Software Foundation' either
'    version 2.1 of the License, or (at your option) any later version.
'
'    This library is distributed in the hope that it will be useful,
'    but WITHOUT ANY WARRANTY' without even the implied warranty of
'    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
'    Lesser General Public License for more details.
'
'    You should have received a copy of the GNU Lesser General Public
'    License along with this library' if not, write to the Free Software
'    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
'
'    Created Evan R Venn - Oct 2013
'    Revised Evan R Venn - Beta v0.81
'    Revised Evan R Venn - Beta v0.82 - completed routines, all tested
'    Revised Evan R Venn - Beta v0.83 - correct NACK in Enab_Osc
'    Revised Evan R Venn - Beta 0.84 corrected errors in ENABLE and DISABLE clock routines
'    Revised Evan R Venn - Oct 2013, thanks go to Microchip and then Application Note AN1488 @ Beta v0.90
'    Revised Thomas Henry, May 25, 2014
'    edited for consistency and commented code
'    removed several redundancies
'    combined DS1307_Enab_Osc and DS1307_Dis_Osc
'    combined DS1307_Set12HourMode and DS1307_Set24HourMode
'    combined DS1307_SQWEnable and DS1307_SQWDisable
'    made parameters private so not to conflict with user's variables,
'    reused parameters where reasonable to save memory.
'    amplified hour-mode to work under all conditions
'    added a.m./p.m. flag (DS_A_P).
'    renamed read and writer commands to indicate generality
'    Revised Thomas Henry, May 26, 2014
'    added DOW to DS1307_ReadDate and DS1307_SetDate
'    added DS1307_ReadHourMode
'    tweaked the comments and external documentation
'
'    10 bytes are used as input and output parameters. They are:
'    DS_Value, DS_Addr, DS_Hour, DS_Min, DS_Sec, DS_A_P, DS_Date, DS_Month, DS_Year and DS_DOW.

'    Quick Command Reference:

'    DS1307_Enable(flag)
     'DS1307_ResetClock
'    DS1307_SetClock(hour, minute, second, DOW, date, month, year)
'    DS1307_SetTime(hour, minute, second)
'    DS1307_SetDate(DOW, date, month, year)
'    DS1307_ReadClock(hour, minute, second, a.m.|p.m., DOW, date, month, year)
'    DS1307_ReadTime(hour, minute, second, a.m.|p.m.)
'    DS1307_ReadDate(DOW, date, month, year)
'    DS1307_SetHourMode(12|24)
'    DS1307_ReadHourMode(12|24)
'    DS1307_SetSQW(rate)
'    DS1307_Write(address, value)
'    DS1307_Read(address, value)

;See the subroutine definitions below for full details on each.

;----- Constants

 ' These are the addresses of the generic DS1307 Real-Time Clock
 #define DS_AddrWrite 0xD0
 #define DS_AddrRead  0xD1

;-----

function DecToBcd(in DS_Value) as byte
  ;Convert pure decimal number to binary coded decimal
  DecToBcd=(DS_Value/10)*16+DS_Value%10
end function

;-----

function BcdToDec(in DS_Value) as byte
  ;Convert binary coded decimal to pure decimal
  BcdToDec=(DS_Value/16)*10+DS_Value%16
end function

;-----

sub DS1307_Enable(in DS_Value)
  ;Enables clock if DS_Value = TRUE, disables if DS_Value = FALSE
   I2CStart
   I2CSend(DS_AddrWrite)
   I2CSend(0)                     ;indicate register 0
   I2CStart
   I2CSend(DS_AddrRead)
   I2CReceive(DS_Sec, NACK)       ;get the current seconds
   if DS_Value then
    set DS_Sec.7 off              ;CH bit = 0 enables
   else
    set DS_Sec.7 on               ;CH bit = 1 disables
   end if
   I2CStart
   I2CSend(DS_AddrWrite)
   I2CSend(0)                     ;indicate register 0
   I2CSend(DS_Sec)                ;now send updated value
   I2CStop
end sub

;-----

sub DS1307_ResetClock
  ;Reset clock to 00:00:00 01 01/01/00.
  ;Also sets 24-hour mode and enables the clock.
  I2CStart
  I2CSend(DS_AddrWrite)
  I2CSend(0)                      ;begin with address 0
  I2CSend(0)                      ;then set the seven
  I2CSend(0)                      ;consecutive locations
  I2CSend(0)
  I2CSend(1)
  I2CSend(1)
  I2CSend(1)
  I2CSend(0)
  I2CStop
end sub

;-----

sub DS1307_SetClock(in DS_Hour, in DS_Min, in DS_Sec, in DS_DOW, in DS_Date, in DS_Month, in DS_Year)
  ;Set entire clock: hours, minutes, seconds, day of week, date, month, year
  ;Also sets 24-hour mode and enables the clock.
  I2CStart
  I2CSend(DS_AddrWrite)
  I2CSend(0)                      ;begin with address 0
  I2CSend(DecToBcd(DS_Sec))       ;then set the seven
  I2CSend(DecToBcd(DS_Min))       ;consecutive values
  I2CSend(DecToBcd(DS_Hour)
  I2CSend(DecToBcd(DS_DOW))
  I2CSend(DecToBcd(DS_Date))
  I2CSend(DecToBcd(DS_Month))
  I2CSend(DecToBcd(DS_Year))
  I2CStop
end sub

;-----

sub DS1307_SetTime(in DS_Hour, in DS_Min, in DS_Sec)
  ;Set time only: hours, minutes, seconds.
  ;Also sets 24-hour mode and enables the clock.
  I2CStart
  I2CSend(DS_AddrWrite)
  I2CSend(0)                      ;begin with address 0
  I2CSend(DecToBcd(DS_Sec))       ;then set the three
  I2CSend(DecToBcd(DS_Min))       ;consecutive values
  I2CSend(DecToBcd(DS_Hour))
  I2CStop
end sub

;-----

sub DS1307_SetDate(in DS_DOW, DS_Date, in DS_Month, in DS_Year)
  ;Set date only: day of week, date, month, year
  ;There is no error detection for out-of-range dates.
  I2CStart
  I2CSend(DS_AddrWrite)
  I2CSend(3)                      ;begin with address 3
  I2CSend(DecToBcd(DS_DOW))       ;then set the four
  I2CSend(DecToBcd(DS_Date))      ;consecutive values
  I2CSend(DecToBcd(DS_Month))
  I2CSend(DecToBcd(DS_Year))
  I2CStop
 end sub

;-----

sub DS1307_ReadClock(out DS_Hour, out DS_Min, out DS_Sec, out DS_A_P, out DS_DOW, out DS_Date, out DS_Month, out DS_Year)
;Read entire clock: hours, minutes, seconds, a.m. or p.m., day of week, date, month, year
;DS_A_P = 0 means a.m. DS_A_P = 1 means p.m.
  I2CStart
  I2CSend(DS_AddrWrite)
  I2CSend(0)                      ;begin with address 0
  I2CStop
  I2CStart
  I2CSend(DS_AddrRead)
  I2CReceive(DS_Sec)              ;get seconds
  DS_Sec = BcdToDec(DS_Sec & 127) ;strip off CH bit

  I2CReceive(DS_Min)              ;get minutes
  DS_Min = BcdToDec(DS_Min)       ;bit 7 is always 0

  I2CReceive(DS_Hour)             ;get hours
  if DS_Hour.6 then               ;12-hour mode
    DS_A_P = DS_Hour.5            ;a.m. or p.m.
    DS_Hour = BcdToDec(DS_Hour & 31)
  else
    DS_Hour = BcdToDec(DS_Hour)   ;24-hour mode
    DS_A_P = (DS_Hour > 11)       ;a.m. or p.m.
  end if

  I2CReceive(DS_DOW)              ;get day of week
  DS_DOW = BcdToDec(DS_DOW)

  I2CReceive(DS_Date)             ;get date
  DS_Date = BcdToDec(DS_Date)

  I2CReceive(DS_Month)            ;get month
  DS_Month = BcdToDec(DS_Month)

  I2CReceive(DS_Year, NACK)       ;get year
  DS_Year = BcdToDec(DS_Year)
  I2CStop
 end sub

;-----

sub DS1307_ReadTime(out DS_Hour, out DS_Min, out DS_Sec, out DS_A_P)
  ;Read time only: hours, minutes, seconds, a.m. or p.m.
  I2CStart
  I2CSend(DS_AddrWrite)
  I2CSend(0)                      ;begin with address 0
  I2CStart
  I2CSend(DS_AddrRead)
  I2CReceive(DS_Sec)              ;get the seconds
  DS_Sec = BcdToDec(DS_Sec & 127) ;strip off CH bit

  I2CReceive(DS_Min)              ;get the minutes
  DS_Min = BcdToDec(DS_Min)

  I2CReceive(DS_Hour, NACK)       ;get the hours
  if DS_Hour.6 then               ;12-hour mode
    DS_A_P = DS_Hour.5            ;a.m. or p.m.
    DS_Hour = BcdToDec(DS_Hour & 31)
  else
    DS_Hour = BcdToDec(DS_Hour)   ;24-hour mode
    DS_A_P = (DS_Hour > 11)       ;a.m. or p.m.
  end if
  I2CStop
end sub

;-----

sub DS1307_ReadDate(out DS_DOW, out DS_Date, out DS_Month, out DS_Year)
;Get date only: day of week, date, month, year
  I2CStart
  I2CSend(DS_AddrWrite)
  I2CSend(3)                      ;begin with address 3
  I2CStart
  I2CSend(DS_AddrRead)
  I2CReceive(DS_DOW)              ;get day of week
  DS_Date = BcdToDec(DS_DOW)
  I2CReceive(DS_Date)             ;get date
  DS_Date = BcdToDec(DS_Date)
  I2CReceive(DS_Month)            ;get month
  DS_Month = BcdToDec(DS_Month)
  I2CReceive(DS_Year, NACK)       ;get year
  DS_Year = BcdToDec(DS_Year)
  I2CStop
end sub

;-----

sub DS1307_SetHourMode(in DS_A_P)
  ;Set hour mode. 12 = 12-hour, all else = 24-hour
  I2CStart
  I2CSend(DS_AddrWrite)
  I2CSend(2)                      ;located in address 2
  I2CStart
  I2CSend(DS_AddrRead)
  I2CReceive(DS_Hour, NACK)       ;get current hours and mode
  if DS_A_P = 12 then             ;change to 12-hour mode
    if !DS_Hour.6 then            ;only if in 24-hour mode
      DS_Hour = BcdToDec(DS_Hour) ;read current hour
      if DS_Hour > 12 then        ;it's a p.m.
        DS_Hour -= 12             ;reduce to 12-hour mode
        DS_Hour = DecToBcd(DS_Hour) ;convert to BCD
        set DS_Hour.5 on          ;indicate p.m.
      else
        DS_Hour = DecToBcd(DS_Hour)
      end if
      set DS_Hour.6 on            ;indicate 12-hour mode now
    end if
  else                            ;change to 24-hour mode
    if DS_Hour.6 then             ;only if in 12-hour mode
      DS_A_P = DS_Hour & 32       ;save am/pm flag
      DS_Hour = BcdToDec(DS_Hour & 31)  ;strip it out
      if DS_A_P then              ;p.m. time
        DS_Hour += 12             ;convert to 24-hour format
      end if
      DS_Hour = DecToBcd(DS_Hour) ;top 2 bits are 0
    end if
  end if
  I2CStart
  I2CSend(DS_AddrWrite)
  I2CSend(2)
  I2CSend(DS_Hour)                ;send hours and flag back again
  I2CStop
end sub

;-----

sub DS1307_ReadHourMode(out DS_A_P)
  ;Return the current hour mode, 12 or 24
  I2CStart
  I2CSend(DS_AddrWrite)
  I2CSend(2)                      ;go to address 2
  I2CStart
  I2CSend(DS_AddrRead)
  I2CReceive(DS_A_P, NACK)        ;get entire hour byte
  I2CStop

  if DS_A_P.6 then                ;read the mode bit
    DS_A_P = 12                   ;and translate
  else
    DS_A_P = 24
  end if
end sub

;-----

sub DS1307_SetSQW(in DS_Value)
  ;Set square wave output pin mode
  I2CStart
  I2CSend(DS_AddrWrite)
  I2CSend(7)                      ;location is address 7
  select case DS_Value
     case 0                       ;0 = disable completely
          I2CSend(0)
     case 4                       ;4 = 4.096kHz
          I2CSend(17)
     case 8                       ;8 = 8.192kHz
          I2CSend(18)
     case 32                      ;32 = 32.768kHz
          I2CSend(19)
     case else                    ;anything else defaults to 1 Hz
          I2CSend(16)
  end select
  I2CStop
end sub

;-----

sub DS1307_Write(in DS_Addr, in DS_Value)
  ;Write to the internal RAM. Use addresses 0x08 to 0x3F only,
  ;else there will be wraparound to the register space of
  ;0x00 to 0x07. This is especially important when doing
  ;multibyte writes. But note that this subroutine could
  ;also be used to write to clock registers 0 though 7 if
  ;that's really what you want, for low level access.
  I2CStart
  I2CSend(DS_AddrWrite)           ;indicate write mode
  I2CSend(DS_Addr)                ;send address
  I2CSend(DS_Value)               ;send value
  I2CStop
end sub

;-----

sub DS1307_Read(in DS_Addr, out DS_Value)
  ;Read from the internal RAM. See the notes, above.
  I2CStart
  I2CSend(DS_AddrWrite)           ;indicate write mode
  I2CSend(DS_Addr)                ;send the address
  I2CStart
  I2CSend(DS_AddrRead)            ;then read mode
  I2CReceive(DS_Value, NACK)      ;and get value
  I2CStop
end sub





