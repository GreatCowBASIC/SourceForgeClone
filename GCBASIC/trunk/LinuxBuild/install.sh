#!/bin/sh

#2016-07-20: Make grep for version number from $VersionFile case-insensitive

set -e # Halt on error

# Great Cow Basic generic Linux installer, Version 0.1
# NOT to be used on any other operating system (eg. Windows).

# What's my name?
myName=${0##*/}

# Files containing Version and Release date
VersionFile=../SynToolbars.ini
ReleaseFile=gcbasic.bas
VersionReleaseFile=../VersionRelease.txt

# Compiler vars
CC="fbc -exx -v -arch native gcbasic.bas" # Compile Command
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
	build:		Compile the package. (Do this first.)
	install:	Install the package. (Must be done as root.)

EOM
	exit 1
}

build()
# Compiles gcbasic.bas to gcbasic with appropriate architecture option
{
	for file in $REQUIRES; do
		if [ ! -f "$file" ]; then
			echo "$file does not exist. Cannot continue."
			echo -e "Files required are:\n$REQUIRES"
			exit 1
		fi
	done

	# Get version number, release date from SynToolbars.ini, gcbasic.bas
	Version=$(expr "$(grep -i '3h=Show GCBASIC Compiler' $VersionFile)" : '.* v\(.*\) b')
	Release=$(expr "$(grep "Version =" $ReleaseFile)" : '.*".* \(.*\)"')
	# Write to text file to facilitate support
	echo -e "Version: $Version\nRelease: $Release\n" > $VersionReleaseFile

	echo -e "\nCompiling GCBASIC Version $Version, Release $Release\n"
	$CC
	if [ $? -ne 0 ]; then
		echo "There was an error executing: $CC"
	else
		echo -e "\nCompiled successfully.\n"
		set +e # Do not halt on error
		existing=`which $exefile 2>/dev/null` # Is there an existing gcbasic found in the PATH?
		set -e # Resume halt on error
		if [[ $? -eq 0  && $existing != "$installdir/$exefile" ]] ; then
			[ -f $VersionReleaseFile ] && cat $VersionReleaseFile
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
	if [ ! -f /etc/profile ]; then
		echo "Cannot set system environment to automatically add $installdir to PATH upon login. You'll need to do that manually."
	else
		if [ -z $(grep "$installdir" /etc/profile) ]
		then
			echo "PATH=\$PATH:$installdir" >> /etc/profile
			echo "Your PATH should be automatically updated the next time you login."
		fi
	fi
	echo -e "To use 'gcbasic' before your next login (ie. now), you'll need to ensure that '$installdir' is in your normal user (not root) path. Check if it's there with, 'echo \$PATH'. If it's not there, add it with, 'PATH=\$PATH:$installdir'.\n"
}

install()
# Copy exefile to parent (GreatCowBasic) directory, and Sync with installdir.
{
	if [ `whoami` != "root" ]; then
		echo "'$myName install' must be run as root."
		exit
	fi
	if [ ! -f "$exefile" ]; then
		echo "$exefile does not exist. Cannot install."
		exit 1
	fi
	if [ ! -d $installdir ]; then
		mkdir $installdir #create target
		if [ $? -ne 0 ]; then
			echo "Could not create installation directory. Exiting."
			exit 1
		fi
	fi

	cp -p $exefile .. #Copy compiled executable to parent (GreatCowBasic) directory
	cd ..
	chmod +x *.sh # Set scripts as executable
	rsync -rv $rsyncexclude * $installdir #install
	echo -e "\n"
	if [ $? -eq 0 ]; then
		echo -e "*** Installed successfully to $installdir !\n"
		echo -e "Check there for some helper shell scripts (*.sh). You can check the GCBASIC version number and release date with, 'gcbasic /version'.\n"
		setpath
		echo -e "To view the local HTML help files, point your web browser to $installdir/Help/index.html\n\n"
		echo -e "We hope you enjoy using GCBASIC. Please let us know if you need any help:\nhttp://sourceforge.net/p/gcbasic/discussion/\n"
	else	echo -e "\nThere was a problem during install. Please manually copy the contents of the GreatCowBasic directory to $installdir, and add $installdir to your PATH.\n"
	fi
}

#--- Main Event ---
if [ $# -ne 1 ]; then
  usage
  exit
fi

case $1 in
	"build") build;;
	"install") install;;
	*) echo "Invalid option: '$1'. Type '$myName' for help."
esac
