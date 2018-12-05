#!/bin/sh

###################################################
# FreeBSD installation script for Great Cow BASIC #
###################################################
#
# v1.00, 29 Sep 2018 - initial version
# v1.01, 1  Oct 2018 - revised file layout for Linux distribution  
# v1.02, 30 Oct 2018 - fixed cosmetic typo  
# v1.03, 31 Oct 2018 - fixed version string parsing  
# v2.00, 04 Dec 2018 - enforce case on dirs and files
#
###################################################

echo 
echo "FreeBSD Great Cow BASIC installation v2.00"
echo

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
    do_exit "You do not have permission to write to ${1} - try running as root"
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
release=`cat ../version.txt | cut -f1 -d":" | tr -d "[\r]"`
version=`cat ../version.txt | cut -f2 -d":" | tr -d "[\r]" | tr -d "[ ]"`

return
}

#
# Enforce case of files and directories
# - note excludes the elements of the path GreatCowBasic/Sources/FreeBSDBuild
# - note excludes the converters directory contents (spaced directory names, mixed case filenames)
#
enforce_case()
{
cd ..   # change from Source to above (ie GreatCowBasic)

echo "Checking directories with initial capital letter ..."
for dir in ${UCDIRS}    # check for dirs with initial cap
        do
            if [ ! -d ${dir} ]
                then
                        found_dir=`ls | grep -i ${dir}`
                        mv ${found_dir} ${dir}
                        echo "Case of ${found_dir} corrected to ${dir}"
            fi
        done

echo "Checking directories that should be all lowercase ..."
for dir in ${LCDIRS}    # check for dirs with all lowercase
        do
                if [ ! -d ${dir} ]
                  then
                        found_dir=`ls | grep -i ${dir}`
                        mv ${found_dir} ${dir}
                        echo "Case of ${found_dir} corrected to ${dir}"
                fi
        done

echo "Checking files - should be all lowercase ..."

start_dir=`pwd`
all_subdirs=`find . -type d -print`
selected_dirs=`echo "${all_subdirs}" | grep -v "Documentation" | grep -v converters | grep -v "Build"`

for dir in ${selected_dirs}
        do
	    if [ "${dir}" = "." ]
	      then
	        cd ${start_dir}	
		continue
	    fi

	    echo "Checking ${dir} ..."
	    cd ${dir}

	    files=`ls | grep -v Build`
	    #echo ${files}
	    
	    for file in ${files}
	        do
		    #echo "File: ${file}"
		
		    if [ -d ${file} ]
		       then
			   cd ${start_dir}
			   continue
		    fi

		    found_file=`echo "${file}" | grep "[A-Z]"`
                    if [ "${found_file}" ]
                       then
			   correct_file=`echo "${file}" | tr "[A-Z]" "[a-z]"`
			   mv "${found_file}" "${correct-file}"
                           echo "Case of ${found_file} corrected to ${correct_file}"
                    fi

	        done
	    
	    cd ${start_dir}   # return to starting directory
	 done

cd Sources   # change back to Sources directory
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
UCDIRS="Documentation Sources"
LCDIRS="demos include chipdata converters" 
REQUIRED="gcbasic.bas assembly.bi preprocessor.bi utils.bi variables.bi ../version.txt"
INSTALL_FILES="Sources/gcbasic messages.dat messages-br.dat messages-de.dat messages-es.dat messages-fr.dat messages-it.dat messages-tk.dat include chipdata demos Documentation converters"

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
# enforce case
#
enforce_case

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
	echo ">>>> Great Cow BASIC compiled successfully! <<<<"
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

    # copy executable and support files and dirs
    cd ..
    cp -R ${INSTALL_FILES} ${install_dir}/
    if [ ${?} -eq 0 ]
      then
        echo ">>>> Great Cow BASIC installation successful :-) <<<<"
	echo
	echo "Do not forget to add ${install_dir} to your path"
    else
        echo "Great Cow BASIC installation FAILED :-("
	echo
	echo "Find help at https://sourceforge.net/p/gcbasic/discussion/ :-)"
    fi

fi
