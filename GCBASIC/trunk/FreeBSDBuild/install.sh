#!/bin/sh

###################################################
# FreeBSD installation script for Great Cow BASIC #
###################################################
#
# version 1.00, 29 Sep 2018 - initial version
#
###################################################

## FUNCTIONS

#
# Check for executable
#
exec_check()
{
check=`which ${1}`

if [ "${check}" = "" ]
  then
    do_exit "${1} executable not found in your path!"
  else
    echo "${1} executable found"
fi

return
}

#
# Input dir
#
dir_input()
{
echo -n "${1}: "; read dir
dir=`echo ${dir} | sed "s!~!${HOME}!" | sed "s/\/$//"`

return
}

#
# Check requirements
#
req_check()
{
for file in ${REQUIRED}
  do
    if [ ! -r "${file}" ]
      then
	 do_exit "Required file: ${file} not found!"
    fi
  done

echo "source files present and correct"

return
}


#
# Check dir exists and writable
#
write_dir()
{
if [ ! -d ${1} ]
  then
    echo
    echo "Attempting to create ${1}"
    mkdir ${1} > /dev/null 2>&1
    if [ ! -d ${1} ]
      then
    	do_exit "You do not have permission to create ${1} - try running as root"
    fi
fi
	 
touch ${1}/deleteme > /dev/null 2>&1
if [ ${?} -gt 0 ]
  then
    do_exit "You do not have permission to write to ${1} - try runnign as root"
else 
    rm -f ${1}/deleteme
fi

return
}

#
# Extract version and release from version.txt file
#
ver_rel()
{
release=`cat version.txt | cut -f1 -d":" | tr -d "[\r]"`
version=`cat version.txt | cut -f2 -d":" | tr -d "[\r]"`

return
}

#
# Usage
#
usage()
{
cmd=`basename ${0}`
echo
echo "Usage: ${cmd} [all | build | install]"
echo "       all      - compile and install gcbasic"
echo "       build    - compile gcbasic"
echo "       install  - install gcbasic"
echo

exit 1
}

#
# Exit with message
#
do_exit()
{
echo
echo "${1}"
echo
echo "Exiting until fixed"
exit 1
}

## END FUNCTIONS

## DEFINES

CC="fbc -exx -v -arch native gcbasic.bas"
REQUIRED="gcbasic.bas assembly.bi preprocessor.bi utils.bi variables.bi version.txt"
INSTALL_FILES="gcbasic messages.* include chipdata Demos Documentation converters"

## END DEFINES

## VARS

install_dir="/usr/local/gcb"

## END VARS

#
# Check command line
#
if [ "${1}" = "" ]
  then usage
fi

#
# Check location script called from
#
pwd=`pwd`
pwd=`basename ${pwd}`
if [ "${pwd}" != "Sources" ]
  then
    echo ${pwd}
    do_exit "${0} must be executed in the GreatCowBasic/Sources directory"
fi

#
# target all or build
#
if [ "${1}" = "all" -o "${1}" = "build" ]
  then
    ver_rel
    echo
    echo "Building Great Cow BASIC ${version}, released ${release}"
    echo
    req_check
    exec_check fbc
    exec_check gcc
    echo "compiling ..."
    echo
    ${CC}
    if [ ${?} -eq 0 ]
      then
	echo
	echo "Great Cow BASIC compiled successfully!"
	echo
    fi
fi


#
# target all or install
#
if [ "${1}" = "all" -o "${1}" = "install" ]
  then
    ver_rel
    echo
    echo "Great Cow BASIC version will be added to installation directory"
    echo "eg /usr/local/GCB -> /usr/local/GCB-${version}"
    echo
    dir_input "Installation directory (return for ${install_dir}-${version})"

    if [ "${dir}" != "" ]
      then
	install_dir=${dir}-${version}
    else
	install_dir=${install_dir}-${version}
    fi
        
    write_dir ${install_dir}
    echo
    echo "Installing in ${install_dir} ..."
    echo

    # executable and support files and dirs
    cp -R ${INSTALL_FILES} ${install_dir}/
    if [ ${?} -eq 0 ]
      then
        echo "Great Cow BASIC installation successful :-)"
	echo
	echo "Do not forget to add ${install_dir} to your path"
    else
        echo "Great Cow BASIC installation FAILED :-("
	echo
	echo "Find help at https://sourceforge.net/p/gcbasic/discussion/ :-)"
    fi

fi
