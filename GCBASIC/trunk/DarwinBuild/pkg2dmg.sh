#!/bin/sh  
if [ -r GCBasic-macOS-Installer.dmg.sparseimage ]
  then
    rm GCBasic-macOS-Installer.dmg.sparseimage
fi

if [ -r GCBasic-macOS-Installer.dmg ]
  then
    rm GCBasic-macOS-Installer.dmg
fi

hdiutil create -size 100M -type SPARSE -volname "GCBasic macOS Installer" -fs HFS+ GCBasic-macOS-Installer.dmg
hdiutil attach GCBasic-macOS-Installer.dmg.sparseimage
cp -R GCB_Installer/build/Great\ Cow\ BASIC.pkg '/Volumes/GCBasic macOS Installer/'
hdiutil detach -force '/Volumes/GCBasic macOS Installer'
hdiutil convert "GCBasic-macOS-Installer.dmg.sparseimage" -format UDBZ -o "GCBasic-macOS-Installer.dmg" -ov -imagekey zlib-level=9
rm GCBasic-macOS-Installer.dmg.sparseimage
open GCBasic-macOS-Installer.dmg
