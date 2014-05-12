'    Maths outines for the GCBASIC compiler
'    Copyright (C) 2014 Thomas Henry and Evan Venn

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

;-----

;Square root function for Great Cow Basic.
;Thomas Henry --- 4/18/2014

;This uses a binary approach involving only bit-shifting,
;addition and subtraction for greatest speed. The inputs
;and output are Words, but the internal computation is
;carried out on Longs for best accuracy. The returned
;value is accurate to 2 decimal places. The maximum
;argument is 4294.

;input parameter: word
;output value: word
;local variables: sqrt_result, sqrt_bitFlag, sqrt_num: long

function sqrt(in sqrt_arg as word) as word
  dim sqrt_result, sqrt_bitFlag, sqrt_num as long

  sqrt_num = [long]sqrt_arg*1000000   ;scale up by 1 million
  sqrt_result = 0                     ;result built up here

  sqrt_bitFlag = 0                    ;decision flag
  set sqrt_bitFlag.30 on

  do while (sqrt_bitFlag > sqrt_num)  ;bring flag down for start
    sqrt_bitFlag = sqrt_bitFlag/4     ;shift two bits right
  loop

  do while sqrt_bitFlag <> 0          ;repeat until zero
    if (sqrt_num >= sqrt_result + sqrt_bitFlag) then
      sqrt_num = sqrt_num - sqrt_result - sqrt_bitFlag
      sqrt_result = (sqrt_result/2) + sqrt_bitFlag
    else
      sqrt_result = sqrt_result / 2
    end if

    sqrt_bitFlag = sqrt_bitFlag/4     ;shift two bits right
  loop

  sqrt = [word](sqrt_result+5)/10     ;done: round to two places
end function

;-----

;Logarithmic Functions for Great Cow Basic
;Thomas Henry --- 5/1/2014

;These functions compute base 2, base e and base 10 logarithms
;accurate to 2 decimal places, +/- 0.01. The values returned are
;fixed-point numbers, with two decimal places assumed on the right.
;Or if you prefer, think of the values as being scaled up by 100.

;The input arguments are word-sized integers, 1 to 65535.
;Remember, logarithms are not defined for non-positive numbers.
;It is the calling program's responsibility to avoid these.
;Output values are also word-sized.

;Local variables consume 9 bytes, while the function parameters
;consume another 4 bytes, for a grand total of 13 bytes of RAM
;used. The lookup table takes 35 words of program memory.

;----- Functions

function log2_full(log_arg as word) as word
  ;This local function is the heart of all three logarithmic
  ;functions. It returns a base-2 logarithm, to 3 decimal places.

  dim log_int as byte
  dim log_x1, log_y1, log_copy, log_frac as word

  dim log_pnt alias log_x1 as byte            ;reuse these to save space
  dim log_count alias log_x1_H as byte
  dim log_pow alias log_y1, log_y1_H as word
  dim log_y2 alias log_copy, log_copy_H  as word

  log_copy = log_arg                          ;leave argument intact
  log_int = 16                                ;count down to high bit
  set status.c off
    do
    ;in effect, shift left until the first 1 is found
    rotate log_copy left                      ;carry is 0, so just a shift
    log_int--                                 ;keep track of # of shifts
  loop until status.c

  log_count = log_int                         ;leave integer part intact
  log_pow = 0
  set status.c on                             ;move this bit in
  do                                          ;find power less than argument
    rotate log_pow left
    log_count--
  loop until log_count = 255                  ;think of this as -1

  log_frac = [long]10000*log_arg / log_pow    ;10000 times mantissa

  if log_copy = 0 then                        ;perfect power of 2
    log_frac = 0
  else
    log_pnt = log_copy_H / 16 + 1             ;pointer to table
    readTable log_Ordinate, log_pnt, log_y1   ;y1
    readTable log_Ordinate, log_pnt+1, log_y2 ;y2
    log_x1 = 10000 + (log_pnt-1) * 625        ;x1

    ;interpolate: (x-x1)*(y2-y1)/(x2-x1)+y1
    log_frac = [long](log_frac-log_x1)*(log_y2-log_y1)/625+log_y1
  end if

  log2_full = [long]1000*log_int+(log_frac+5)/10  ;round to three places
end function

;----- Table

table log_ordinate as word
  ;log values between 1 and 2, inclusive, scaled up by 10000. The
  ;associated abscissas are shown in the comments. Delta-x is 0.0625.

  0              ;1.0000
  875            ;1.0625
  1699           ;1.1250
  2479           ;1.1875
  3219           ;1.2500
  3923           ;1.3125
  4594           ;1.3750
  5236           ;1.4375
  5850           ;1.5000
  6439           ;1.5625
  7004           ;1.6250
  7549           ;1.6875
  8074           ;1.7500
  8580           ;1.8125
  9069           ;1.8750
  9542           ;1.9375
  10000          ;2.0000
end table

function log2(log_arg1 as word) as word
  ;return base-2 logarithm rounded to 2 decimal places
  log2 = (log2_full(log_arg1)+5)/10
end function

function loge(log_arg1 as word) as word
  ;return base-e logarithm rounded to 2 decimal places
  loge = ([long]log2_full(log_arg1)*45426/65536+5)/10
end function

function log10(log_arg1 as word) as word
  ;return base-10 logarithm rounded to 2 decimal places
  log10 = ([long]log2_full(log_arg1)*19728/65536+5)/10
end function

;-----

;The power function for Great Cow Basic
;Thomas Henry -- 5/2/2014

;This function raises a base to a power. Remember, powers
;can easily become huge, so it is up to the calling program
;to make certain your numbers remain within range. The
;base and exponent are Byte sized numbers in any event.
;The returned result is a Long. Non-negative numbers are
;assumed throughout.

;Keep in mind that 0 raised to 0 is meaningless and should be
;avoided. But any other non-zero base raised to 0 is handled okay.

;Local variables consume 1 byte, while the function parameters
;consume another 6 bytes, for a grand total of 7 bytes of RAM
;used.

function power(p_base as byte, p_exp as byte) as long
  ;return p_base raised to the p_exp power
  dim p_copy as byte

  p_copy = p_exp
  power = 1
  do while p_copy
    power = power * p_base
    p_copy--
  loop
end function

;-----

;The right-shift function for Great Cow Basic
;Thomas Henry -- 5/9/2014

;This function accepts a Word value to be shifted and
;a Byte indicating the number of places it should be
;shifted. The returned value is a Word also. During
;operation, zeros are shifted in on the left, while
;the low order bits roll off on the right.

;No local variables are used. The function parameters consume
;5 bytes.

function shiftR (in sh_arg1 as word, in sh_arg2 as byte) as word
  ;shift sh_arg1, sh_arg2 places to the right

  repeat sh_arg2
    status.c = 0
    rotate sh_arg1 right
  end repeat
  shiftR = sh_arg1
end function
