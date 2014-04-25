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
