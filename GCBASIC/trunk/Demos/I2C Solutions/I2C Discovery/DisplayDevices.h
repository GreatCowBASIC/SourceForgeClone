;   FILE:           DisplayDevices.h
;   DATE:           14/2/2015
;   VERSION:        1.0a
;   AUTHOR:         Evan R Venn
;
;    Description.
'    A support method for GCGB and GCB.
'    Edit this file to add more devices.  This is used by the I2C discovery routines.
'    Supports the hardware and software discovery routines.	
'

;    This file is part of the Great Cow Basic compiler.
;
;    This demonstration code is distributed in the hope that it will be useful,
;    but WITHOUT ANY WARRANTY; without even the implied warranty of
;    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
;    GNU General Public License for more details.
;

Sub DisplayDevices
  select case testid
         case 49
              Hserprint "DS2482_1Channel_1Wire Master"
         case 65
              Hserprint "Serial_Expander_Device"
         case 73
              Hserprint "IC2/Serial_Expander_Device"
         case 75
              Hserprint "IC2/Serial_Expander_Device"
         case 77
              Hserprint "IC2/Serial_Expander_Device"
         case 79
              Hserprint "IC2/Serial_Expander_Device"
         case 97
              Hserprint "GCB Slave Device"
         case 161
              Hserprint "EEProm_Device_Device"
         case 163
              Hserprint "EEProm_Device_Device"
         case 165
              Hserprint "EEProm_Device_Device"
         case 167
              Hserprint "EEProm_Device_Device"
         case 169
              Hserprint "EEProm_Device_Device"
         case 171
              Hserprint "EEProm_Device_Device"
         case 173
              Hserprint "EEProm_Device_Device"
         case 175
              Hserprint "EEProm_Device_Device"
         case 199
              Hserprint "AXE033 I2C LCD Device"
         case 209
              Hserprint "DS1307_RTC_Device"
         case 249
              Hserprint "FRAM_Device"
         case else
              Hserprint "Unknown_Device"
  end select
end sub