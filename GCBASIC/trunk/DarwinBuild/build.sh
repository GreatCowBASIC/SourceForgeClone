#!/bin/sh

echo
echo "Compiling Great Cow BASIC for macOS with FreeBASIC"
echo

# You need to install the GNU gcc compiler
#
# Failure to use the gcc compiler will result in the following multiple
# errors from Apple's clang compiler:
# error: indirect goto in function with no address-of-label expressions
#
# Correct the path below for your gcc version and location
export PATH=/usr/local/gcc-8.1/bin:$PATH

# You need to install Xcode from the Apple App Store
#
# Set the SDK library path for your Xcode version
export LIBRARY_PATH=/Applications/Xcode.app/Contents/Developer/Platforms/MacOSX.platform/Developer/SDKs/MacOSX10.14.sdk/usr/lib

# Set the SDK include paths for your Xcode version
export C_INCLUDE_PATH=/Applications/Xcode.app/Contents/Developer/Platforms/MacOSX.platform/Developer/SDKs/MacOSX10.14.sdk/usr/include

# FreeBASIC has a hardcoded OS target version; this needs to match below
# or you will get a linker error similar to the following:
# ld: warning: object file (gcbasic.o) was built for newer OSX version (10.13) than being linked (10.1)
export MACOSX_DEPLOYMENT_TARGET=10.1

# Change to the Great Cow BASIC source directory
cd ~/GreatCowBasic/Sources

# Now compile!
#
# Correct the path for your FreeBASIC version and location
~/fbc-1.06/bin/fbc -exx -v -arch native gcbasic.bas

echo
echo "Finished"
echo
