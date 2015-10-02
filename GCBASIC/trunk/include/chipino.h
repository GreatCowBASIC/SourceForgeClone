'CHIPINOIO.h
'CHIPINO I/O Header File
#IFNDEF LCD_LAT
  #define D0 portc.7
  #define D1 portc.6
  #define D2 portc.5
  #define D3 portc.4
  #define D4 portc.3
  #define D5 portc.2
  #define D6 portc.1
  #define D7 portc.0
  #define D8 portb.0
  #define D9 portb.1
  #define D10 portb.2
  #define D11 portb.3
  #define D12 portb.4
  #define D13 portb.5
#ENDIF

#IFDEF LCD_LAT

  #define D0 LATc.7
  #define D1 LATc.6
  #define D2 LATc.5
  #define D3 LATc.4
  #define D4 LATc.3
  #define D5 LATc.2
  #define D6 LATc.1
  #define D7 LATc.0
  #define D8 LATb.0
  #define D9 LATb.1
  #define D10 LATb.2
  #define D11 LATb.3
  #define D12 LATb.4
  #define D13 LATb.5

  #define _D0 portc.7
  #define _D1 portc.6
  #define _D2 portc.5
  #define _D3 portc.4
  #define _D4 portc.3
  #define _D5 portc.2
  #define _D6 portc.1
  #define _D7 portc.0
  #define _D8 portb.0
  #define _D9 portb.1
  #define _D10 portb.2
  #define _D11 portb.3
  #define _D12 portb.4
  #define _D13 portb.5

#ENDIF


#define A0 porta.0
#define A1 porta.1
#define A2 porta.2
#define A3 porta.3
#define A4 porta.5
#define A5 porta.4

