#!/bin/bash
##  Version 0.95.006
##  Adapted for use on Linux from makeASM.bat. Not to be used on Windows.
##  You can adapt this file to your needs including removing the .sh extension for convenience.
## When executing this script, $1 gets replaced with whatever you enter for <sourcefile> on the command line.

basename=${0##*/}
installdir=$(dirname $(which gcbasic))
proc=awk
export LC_ALL=C
if [ $# -ne 1 ]; then
  echo "Usage: $basename <sourcefile>"
  echo "Example: $basename myfile.gcb"
  echo
  exit
fi

if [ -f $installdir/preprocess.awk ]; then # We need preprocess.awk to do any preprocessing

  # process any header files newer than current ASM file
  # ! There can be only one dot in the filename, or this will fail
  out_name=$(basename "$1" | cut -d. -f1).asm # Note ASM suffix

  if [ -f "$out_name" ]; then # # If there is no current ASM file, don't bother.
    # Check installed header files
    find $installdir -type f -name '*.h' -newer "$out_name" | while IFS= read h; do
      $proc -v NoHeaderMessage=1 -f $installdir/preprocess.awk "$h"
      if [ $? -eq 2 ]; then
        echo ""
        echo "Fatal error executing:"
        echo "  $proc -f $installdir/preprocess.awk $h"
        exit
      fi
    done

    # Check header files in current working directory
    find . -type f -name '*.h' -newer "$out_name" | while IFS= read h; do
      $proc -v NoHeaderMessage=1 -f $installdir/preprocess.awk "$h"
      if [ $? -eq 2 ]; then
        echo ""
        echo "Fatal error executing:"
        echo "  $proc -f $installdir/preprocess.awk $h"
        exit
      fi
    done

  fi

  # process current source file
  $proc -v NoHeaderMessage=1 -f $installdir/preprocess.awk "$1"
  if [ $? -eq 2 ]; then
    echo ""
    echo "Fatal error executing:"
    echo "  $proc -f $installdir/preprocess.awk $1"
    exit
  fi
else
  echo "$installdir/preprocess.awk not found. '$1' not preprocessed."
fi

##  Call GCBasic to make an asm-file only:

##  --- edit command below (don't delete /NP) -----------------

gcbasic /NP /K:A "$1"

##  --- edit END ----------------------------------------------

if [ $? -ne 0 ]; then
  echo "ERROR $? running: $0 with parameters $*"
fi
