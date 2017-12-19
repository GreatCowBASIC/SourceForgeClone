* About this program *
This is a basic tool for editing GCBASIC ini files. It is derived (hacked together would be more appropriate) from Great Cow Graphical BASIC, and requires some files from Great Cow Graphical BASIC to compile.

* Compiling *
Ensure that the "Programmer Editor" folder is in the same folder as a "Great Cow Graphical BASIC" folder. The "Great Cow Graphical BASIC" folder must contain the following files from GCGB:
 - Preferences.vb
 - PreferencesWindow.vb
 - ProgrammerEditor.vb
 - Translator.vb
 - ProgrammerEditor.resources

Once these files are in place, it should be possible to compile the Programmer Editor using SharpDevelop 3.2 (or similar).

* Using *
If run without any parameters, this program will create an ini file in whatever directory it is located in. If it is given the name of an ini file as a command line parameter, it will use that file.

As well as the ini file it is told to load, this program will also read any files that are included from that file. This makes it possible to keep the settings file in the Application Data folder if GCBASIC is installed in the Program Files directory. To put the settings file into the Application Data folder, create a small ini file containing the following 3 lines and place it in the same directory as this program:

include %appdata%\gcgb.ini
[gcgb]
useappdata = true

The include line tells the program (and GCBASIC) to read from the Application Data folder. The useappdata=true line in the [gcgb] section will cause this program to write any output to a file in Application Data called gcgb.ini. (The hard coding of GCGB is because this program is based on GCGB. It will result in programmer definitions being shared between GCGB and any other environment using this editor, which may be a positive side effect.