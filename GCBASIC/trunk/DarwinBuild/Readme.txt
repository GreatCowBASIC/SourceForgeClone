Overview - Apple macOS Great Cow BASIC

*Introduction*

Great Cow BASIC can be used with the Apple macOS operating system.

These instructions are for Apple macOS only (not Windows).

You have a choice to make. You can either:

. download a macOS installer which will install a precompiled binary for Great 
Cow BASIC along with support files and some optional components; or
. download, compile and install Great Cow BASIC from the source files.

There are instructions below for both choices. If I was you, I would use the 
Great Cow BASIC Installer and save valuable programming time :-)

*Instructions for using the Great Cow BASIC Installer*

 1. Download the Great Cow BASIC - macOS Installer disk image (.dmg) file from 
https://sourceforge.net/projects/gcbasic/files/GCBasic-macOS-Installer.dmg/download

 2. Double click the .dmg file to mount it on your Desktop and a window will 
open which contains the Installer.

 3. Double click the Great Cow BASIC icon and follow the installer prompts. 
Note: Depending on your macOS version, you may need to right click the Great 
Cow BASIC Icon and select Open as it is from an unidentified source.

*Instructions for building your own Great Cow BASIC binary*

Complete the following steps to compile and install Great Cow BASIC:

 1. Download the FreeBASIC 1.06 macOS binary compilation from: 
http://tmc.castleparadox.com/temp/fbc-1.06-darwin-wip20160505.tar.bz2

 2. Download the Great Cow BASIC macOS Source Distribution from SourceForge at 
https://sourceforge.net/projects/gcbasic/files/gcb_unix_distribution.rar/download

 3. Unfortunately Apple replaced the gcc compiler with the clang compiler and 
 FreeBASIC needs the real gcc due to a certain use of goto... so, you can 
 compile your own version of gcc following the instructions at: 
 https://bitbucket.org/sol_prog/macos-gcc-binary 
 or you can take the low road and just download the pre-compiled binary version from:
https://bitbucket.org/sol_prog/macos-gcc-binary/raw/38724f2d24aa5b7944d2227d8098b1d30ac5d7b5/gcc-8.1.tar.bz2

 4. Open a Terminal window (Terminal can be found in Applications > Utilities).

 5. Move gcc-8.1.tar.bz2 from your Downloads directory to your Home directory 
by typing the following command in your Terminal window:
----
   mv ~/Downloads/gcc-8.1.tar.bz2 ~/
----

 6. Unpack the gcc-8.1.tar.bz2 compressed tar file by typing the following 
command into your Terminal window:
----
   gzcat gcc-8.1.tar.bz2 | tar xvf -
----
This will produce a new directory called gcc-8.1.

 7. Move the gcc-8.1 directory to the /usr/local/ directory by typing the 
following commands into your Terminal window:
----
   sudo mv gcc-8.1 /usr/local
----
Note: You will be asked for your password to execute the above command.

 8. Ensure that the Apple Developer Xcode app is installed. Xcode can be 
downloaded and installed from the App Store for free.

 9. Ensure that the Xcode command line tools are installed by typing the 
following command in your Terminal window:
----
   xcode-select --install.
----

 10. Move the FreeBASIC compressed tar file from your Downloads directory to 
your home directory by typing the following command in your Terminal window:
----
   mv ~/Downloads/fbc-1.06-darwin-wip20160505.tar.bz2 ~/
----

 11. Unpack the FreeBASIC compressed tar file by typing these commands in your 
Terminal window:
----
   gzcat fbc-1.06-darwin-wip20160505.tar.bz2 | tar xvf -
----
This will produce a new directory called fbc-1.06.

 12. Move the Great Cow BASIC compressed tar file from your Downloads directory
to your home directory by typing the following command in your Terminal window:
----
   mv ~/Downloads/gcb-unix-distribution.rar ~/
----

 13. Unpack the Great Cow BASIC compressed tar file by typing these commands in your Terminal window:
----
   unrar x gcb-unix-distribution.rar
----
This will produce a new directory called GreatCowBASIC. *Note:* If you do not 
currently have the unrar program which can extract rar file archives you can 
download and install it for free from the App Store.

 14. Change to the GreatCowBASIC/sources directory by typing this command in 
your Terminal window:
----
   cd ~/GreatCowBASIC/sources
----

 15. Compile the Great Cow BASIC binary (gcbasic) by typing the following 
command into your Terminal window:
----
  sh darwinbuild/build.sh
----
Note 1: If you did not install the various files with the same names as in the 
instructions above into your Home directory, you will need to edit the build.sh
script file and change the file paths and filenames to the appropriate values.

Note 2: You may need to alter the library and include paths in the build.sh 
script depending on your version of macOS (it is currently setup for the Xcode 
High Sierra 10.13 and Mohave 10.14 versions of macOS).

 16. Confirm the proper execution, and the version, of Great Cow BASIC by 
typing the following command in the Terminal window:
----
   gcbasic
----

Now you should be able create GCB source files with your favourite editor and 
compile those files with the gcbasic compiler.

*Programming microcontrollers*

To program your microcontroller with your Great Cow BASIC-created hex file, you
will need additional hardware and software.

. For Microchip PIC microcontroller programming, you may find what you need at:
https://www.microchip.com/DevelopmentTools/ProductDetails.aspx?PartNO=pg164120 
and the macOS version of the `pk2cmd` v1.2 command line programming software.

. For Atmel AVR microcontroller programming, you will need the avrdude 
programming software. Check here: http://www.nongnu.org/avrdude/ for it.

Alternatively, if you use Virtual Machine software such as Parallels or VMWare 
Fusion_ to run Windows programs, you can use Windows GUI programming software.

* For Microchip, the PICKit 2 and PICkit 3 standalone GUI software or even 
better the PICkitPlus software 
(https://sourceforge.net/projects/pickit3plus/) 
for both the PICkit 2 
(https://www.microchip.com/DevelopmentTools/ProductDetails.aspx?PartNO=pg164120)
and PICkit 3
(https://www.microchip.com/Developmenttools/ProductDetails/PG164130) 
which has fixed various bugs in those programs and been updated to program 
the latest Microchip 8 bit microcontrollers.

*Help*

Great Cow BASIC Help documentation is installed in the documentation 
subdirectory in your GreatCowBASIC directory.

If at any time you encounter an issue and need help, you will find it over at 
the friendly Great Cow BASIC discussion forums at:
https://sourceforge.net/p/gcbasic/discussion/

