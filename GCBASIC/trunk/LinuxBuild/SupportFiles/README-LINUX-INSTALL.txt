*Introduction*

Great Cow BASICcan be used when using the Linux operating system.

This instructions are not distribution specific, but are for Linux only (not Windows).

*Instructions*

Complete the following steps to compile and install Great Cow BASICfor Linux:

1. Install FreeBasic from your distributions repository or http://www.freebasic.net/wiki/CompilerInstalling
   If your Distribution eg. Debian are not provide Freebasic download it first from https://sourceforge.net/projects/fbc/
   and follow the instructions for compiling and installing Freebasic 

2. Download the "GCBasic - Sources" from SourceForge at http://sourceforge.net/projects/gcbasic/files/ 

3. Unrar/unpack GCB@Syn.rar to a location of your choice within your home directory (eg. within Downloads) with either a file manager or from a console.

4. From a console, change to the GCBASIC Sources in the unpacked directory:
   eg. cd ~/Downloads/GreatCowBasic/Sources

5. Make sure that 'install.sh' is set as executable (ie. 'chmod +x install.sh'), and then execute: ./install.sh build

6. You'll need root privileges for the next step. You can switch user (su) to root, or optionally use 'sudo'.
 
7. Do you have an old Installation of GCBasic?
   If so, it would be wise to mv it away: Execute [sudo]  /opt/GCBASIC /opt/GCBASIC_old
   
   Execute: [sudo] pwd  (it should still show the path like it was in Step 4.
   
   Now you are ready fo Install
   Execute: [sudo] ./install.sh install


8. If you su'd to root, use 'exit' to drop back to your normal user. Then, be sure to follow the instructions given by the script for updating your path.

9. Confirm proper execution, and the version, of GCBASIC by executing: gcbasic /version

Now you can create and compile GCB source files.


To program your microprocessor with your GCBASIC-created hex file, you'll need additional software.

For PIC programming, you might find what you need at:
http://www.microchip.com/DevelopmentTools/ProductDetails.aspx?PartNO=pg164120

For AVR programming, you'll need 'avrdude'. It should be available in your distributions repository. If not, check here: http://www.nongnu.org/avrdude/

Do not forget to look into the folder Documentation/

