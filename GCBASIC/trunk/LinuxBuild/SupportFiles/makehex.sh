##  Version 0.95.002
##  Adapted for use on Linux from makeHEX.bat. Not to be used on Windows.
##  You can adapt this file to your needs including removing the .sh extension for convenience.
## In this script, $1 gets replaced with whatever you enter for <sourcefile> on the command line.

basename=${0##*/}
installdir=$(dirname $(which gcbasic))
proc=awk

if [ $# -ne 1 ]; then
	echo "Usage: $basename <sourcefile>"
	echo "Example: $basename myfile.gcb"
	echo
	exit
fi

if [ -f $installdir/preprocess.awk ]; then
	$proc -f $installdir/preprocess.awk $1
	if [ $? -eq 2 ]; then
		echo ""
		echo "Fatal error executing:"
		echo "	$proc -f $installdir/preprocess.awk $1"
		exit
	fi
else
	echo "$installdir/preprocess.awk not found. '$1' not preprocessed."
fi

##  Call GCBasic to make an asm-file and assembling to hex-file:

##  --- edit command below (don't delete /NP) -----------------
gcbasic /NP /K:A /A:GCASM $1

##  --- edit END ----------------------------------------------

if [ $? -ne 0 ]; then
	echo "ERROR $? running: $0 with parameters $*"
fi
