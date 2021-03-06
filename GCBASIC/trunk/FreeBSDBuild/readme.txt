Overview - FreeBSD Great Cow BASIC

*Introduction*

Great Cow BASIC can be used with the FreeBSD operating system.

These instructions are for FreeBSD only (not Windows).

*Instructions for using the Great Cow BASIC install.sh script*

Complete the following steps to compile and install Great Cow BASIC:

 1. Download one of the nightly builds of FreeBASIC 1.XX for the FreeBSD 32 bit
or 64 bit binary compilation from: 
http://users.freebasic-portal.de/stw/builds/freebsd32/ (32 bit) or 
http://users.freebasic-portal.de/stw/builds/freebsd64/ (64 bit) 
The filenames are in the format fbc_freebsd[32|64]\_[BuildNumber]_[Date].zip.

 2. Download the Great Cow BASIC UNIX Source Distribution from SourceForge at 
https://sourceforge.net/projects/gcbasic/files/gcb_unix_distribution.rar/download

 3. Move the FreeBASIC ZIP file from your download directory to your home 
directory.

 4. Unzip the FreeBASIC ZIP file which will produce a new directory called 
fbc_freebsd[32|64]. The FreeBASIC compiler "fbc" is in the `bin` subdirectory. 
You should  add the path to `fbc` to your path so that the installation script
can find it.

 5. Move the Great Cow BASIC compressed tar file from your download directory 
to your home directory.

 6. Unpack the Great Cow BASIC compressed tar file by typing these commands:
----
   unrar x gcbasic_unix_distribution.rar
----
This will produce a new directory called GreatCowBASIC. *Note:* If you do not already have the unrar program installed you can either compile it from the ports collection or use the pkg command to install the binary and any required dependancies.

 7. Change to the `GreatCowBASIC/sources` directory.

 8. Execute the freebsdbuild/install.sh shell script from the sources directory.

----
  sh freebsdbuild/install.sh [all | build | install]
----

The build script arguments are:

  * all     - will compile *and* install the Great Cow BASIC compiler and its 
support files.
  * build   - will just compile the binary for the Great Cow BASIC compiler.
  * install - will install the Great Cow BASIC compiler and its support files.

When choosing all or install you will be prompted for an installation directory.
The default is /usr/local/gcb-[version] for which you will need to run the 
installation script as root. Alternatively, you can choose to install in your 
home directory (eg ``~/bin/gcb`). The installation script will automatically 
append the Great Cow BASIC version so that directory would become 
~/bin/gcb-[version].

 9. Add the directory where you installed gcbasic to your path, or use the full
path to the gcbasic installation directory and confirm the proper execution, 
and the version, of Great Cow BASIC by executing gcbasic.

Now you should be able create GCB source files with your favourite editor and 
compile those files with the Great Cow BASIC compiler.

*Programming microcontrollers*

To program your microcontroller with your Great Cow BASIC-created hex file, you
will need additional hardware and software.

. For Microchip PIC microcontroller programming, you may find what you need at:
https://www.microchip.com/DevelopmentTools/ProductDetails.aspx?PartNO=pg164120 
and the FreeBSD version of the `pk2cmd` v1.2 command line programming software.

. For Atmel AVR microcontroller programming, you will need the avrdude 
programming software. avrdude can be compiled and installed from the FreeBSD 
ports directory or the precompiled binary and any missing dependancies can be 
installed using `pkg install avrdude`.

Alternatively, if you use Virtual Machine software such as Virtual Box to run 
Windows programs, you may be able to use Windows GUI programming software.

* For Microchip, the PICKit 2 and PICkit 3 standalone GUI software or even 
better the PICkitPlus software (https://sourceforge.net/projects/pickit3plus/) 
for both the PICkit 2 
(https://www.microchip.com/DevelopmentTools/ProductDetails.aspx?PartNO=pg164120)
and PICkit 3 
(https://www.microchip.com/Developmenttools/ProductDetails/PG164130)
which has fixed various bugs in those programs and been updated to program the 
latest Microchip 8 bit microcontrollers.

*Help*

Great Cow BASIC Help documentation is installed in the documentation 
subdirectory in your GreatCowBASIC directory.

If at any time you encounter an issue and need help, you will find it over at
the friendly Great Cow BASIC discussion forums at:
https://sourceforge.net/p/gcbasic/discussion/
