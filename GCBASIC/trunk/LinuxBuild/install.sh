#!/bin/bash

#2016-07-20: Make grep for version number from $VersionFile case-insensitive
#2017-01-16: Move system-wide path config from /etc/profile to /etc/profile.d/gcbasic.sh
#2017-12-09: Corrected Check for Freebasic, added check for rsync
#2018-10-30: Updated to run from the Sources directory now there is a LinuxBuild directory
#    : Fixed comment typo in check for FreeBASIC
#    : Added script name & version output
#    : Incremented version number
#2018-11-14: Fixed update comments, echo in Greetings,chmod +x was in wrong dir, useless
#2018-11-20: install exits if $installdir already exists
#2018-11-25: install copies SupportFiles, too, chmod enabled again
#
# NOT to be used on any other operating system (eg. Windows/macOS/FreeBSD).

echo -e "\nGreat Cow Basic generic Linux installer, Version 0.6\n"

# Change to the Sources directory immediately above this directory
cd ..

set -e # Halt on errors

# What's my name?
myName=${0##*/}

# File containing Version and Release data
VersionReleaseFile=../version.txt

# Compiler vars
FBC=fbc # Compiler
CC="$FBC -exx -v -arch native gcbasic.bas" # Compile Command
REQUIRES="gcbasic.bas assembly.bi preprocessor.bi utils.bi variables.bi"

# Install vars
exefile="gcbasic"
installdir=/opt/GCBASIC
rsyncexclude="--exclude=Sources"

#--- Subs ---
usage()
# Outputs usage info if number of command parameters is not 1
{
  cat <<EOM
Usage: $myName build|install
  build:    Compile the package. (Do this first.)
  install:  Install the package. (Must be done as root.)

EOM
  exit 1
}

build()
# Compiles gcbasic.bas to gcbasic with appropriate architecture option
{
  # Check for required source and library files
  for file in $REQUIRES; do
    if [ ! -f "$file" ]; then
      echo "$file does not exist. Cannot continue."
      echo -e "Files required are:\n$REQUIRES"
      exit 1
    fi
  done

  # Get version number & release date from Version/Release file, "version.txt"
  echo -e "Extracting release, version information...\n"
  read VersionRelease <$VersionReleaseFile
  VersionRelease=${VersionRelease%$'\r'}
  Release="$( cut -d ':' -f 1 <<< "$VersionRelease" )"
  Version="$( cut -d ':' -f 2 <<< "$VersionRelease" )"

  echo -e "\nCompiling GCBASIC Version $Version, released $Release\n"
  $CC
  if [ $? -ne 0 ]; then
    echo "There was an error executing: $CC"
  else
    echo -e "\nCompiled successfully.\n"
    existing=`which $exefile 2>/dev/null` # Is there an existing gcbasic found in the PATH?
    if [[ $? -eq 0  && $existing != "$installdir/$exefile" ]] ; then
      echo "Note: You already have a version of gcbasic installed at '$existing'."
      echo "This could cause usage confusion between the two versions."
      echo -e "You may wish to delete or rename the existing version before trying to use this version.\n"
    fi
    echo "Continue with '$myName install' (as root)"
    echo -e "Example: 'sudo $myName install'\n"
  fi
}

setpath()
# Adds installdir to system PATH to serve all users
# Used by install() internally. Can also be used externally as root.
{
  if [[ -f /etc/profile && -n $(grep "profile.d" /etc/profile) ]]; then
    echo "export PATH=\$PATH:$installdir" > /etc/profile.d/gcbasic.sh
    echo "Your PATH should be automatically updated the next time you login."
  else
    echo "Cannot set system environment to automatically add $installdir to PATH upon login. You'll need to do that manually. (Perhaps via '/etc/profile' or '/etc/environment'.)"
  fi
  echo -e "\nTo use 'gcbasic' before your next login (ie. now), you'll need to ensure that '$installdir' is in your normal user (not root) path. Check if it's there with, 'echo \$PATH'. If it's not there, add it with, 'PATH=\$PATH:$installdir'.\n"
}

install()
{
# Copy exefile to parent (GreatCowBasic) directory, and Sync with installdir.
  if [ ! -f "$exefile" ]; then
    echo "$exefile does not exist. Cannot install."
    exit 1
  fi
  if [[ $UID != 0 ]]; then
    echo "'$myName install' must be run as root."
    exit
  fi
  if [ ! -d $installdir ]; then
    mkdir $installdir #create target
    if [ $? -ne 0 ]; then
      echo "Could not create installation directory. Exiting."
      exit 1
    fi
  else
    echo "The Target Directory \"$installdir\" already exists."
    echo "Please read \"../README-LINUX-INSTALL.txt\" Step 7 carefully."
    echo "Exiting."
    exit 1
  fi

  cp -p $exefile .. #Copy compiled executable to parent (GreatCowBasic) directory
  # in SupportFiles are the Scripts which essential for compile, make hex, and flash
  cp -p LinuxBuild/SupportFiles/* ..
  cd ..
  chmod +x *.sh # Set scripts as executable
  rsync -rv $rsyncexclude * $installdir #install
  if [ $? -eq 0 ]; then
    echo -e "\n*** Installed successfully to $installdir !\n"
    echo -e "Check there for some helper shell scripts (*.sh). You can check the GCBASIC version number and release date with, 'gcbasic /version'.\n"
    setpath
    echo -e "To view the local HTML help files, point your web browser to $installdir/Help/index.html\n\n"
    echo -e "We hope you enjoy using GCBASIC. Please let us know if you need any help:\nhttp://sourceforge.net/p/gcbasic/discussion/\n"
  else  echo -e "\nThere was a problem during install. Please manually copy the contents of the GreatCowBasic directory to $installdir, and add $installdir to your PATH.\n"
  fi
}
checkrsync()
{
# Check for rsync
RSYNC=rsync
       if [ -z "$(which $RSYNC)"  ]; then
                echo "### rsync not found in path.         ###"
                echo "###         Cannot continue.         ###"
                echo "###   Please install rsync for your  ###"
    echo "###   Distribution and try again.    ###"
                exit 1
        fi

}

checkfbc()
{
# Check for FreeBASIC
FBC=fbc
       if [ -z "$(which $FBC)"  ]; then
                echo "### Freebasic Compiler not found in path. ###"
                echo "###         Cannot continue.              ###"
                echo "###   Please install Freebasic for your   ###"
                echo "###   Distribution and try again.         ###"
                exit 1
        fi

}



#--- Main Event ---
if [ $# -ne 1 ]; then
  usage
  exit
fi

case $1 in
  "build") checkfbc; build;;
  "install") checkrsync; install;;
  *) echo "Invalid option: '$1'. Type '$myName' for help."
esac
