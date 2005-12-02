; Gpremacy Installer for Windows
; Note: look for Mono's directory in VERSION
; where VERSION = HKEY_LOCAL_MACHINE\SOFTWARE\Novell\Mono\DefaultCLR
; (such as HKEY_LOCAL_MACHINE\SOFTWARE\Novell\Mono\1.1.9.2)
;--------------------------------

!define VERSION "Multiplayer Release 3"

; The name of the installer
Name "Gpremacy $(VERSION)"

; The file to write
OutFile "gpremacy-win.exe"

; The default installation directory
InstallDir $PROGRAMFILES\Gpremacy

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\Gpremacy" "Install_Dir"

ShowInstDetails show
XPStyle on
Var MonoPath


;--------------------------------
; Pages

Page components
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

;--------------------------------
; Functions

; getMonoDirectory
; Usage:
;    Push $0
;    Call getMonoDirectory
;    StrCmp $0 1 found.NETFramework no.NETFramework
;    Pop $0

Function getMonoDirectory
; $0 will be clobbered
Push $1
Push $2
Push $3

ReadRegStr $2 HKEY_LOCAL_MACHINE "SOFTWARE\Novell\Mono" "DefaultCLR"
StrLen $1 $2 ; if > 0, it's there
IntCmp $1 0 notInstalled notInstalled

isInstalled:
    StrCpy $3 Software\Novell\Mono\$2
    ReadRegStr $3 HKEY_LOCAL_MACHINE $3 SdkInstallRoot
        
    StrLen $1 $3 ; if > 0, it's there
    IntCmp $1 0 notInstalled notInstalled
    goto finish

finish:
    ; Copy the path
    StrCpy $0 $3
    goto done
    
notInstalled:
        MessageBox MB_OK "It doesn't appear that you have Mono installed. Please go to http://go-mono.org/ and get Mono before installing Gpremacy!"
        Quit

done:
    Pop $3
    Pop $2
    Pop $1
FunctionEnd

;--------------------------------
; The stuff to install
Section "Gpremacy (required)"

  SectionIn RO
  
  ; Check for Mono, find its path 
  Push $0    
  Call getMonoDirectory
  StrCpy $MonoPath $0
  Pop $0  
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  ; Put main files
  File "countries.csv"
  File "resource_cards.csv"
  File "countries.csv"
  File "TODO"
  File "README"
  File "LICENSE"
  File "bin\Debug\gpremacy-mono.xml"
  File "bin\Debug\gpremacy-mono.exe"
  
  ; Now get the graphics dir
  File /r /x CVS "Graphics"
  
  ; And the GUI file
  SetOutPath "$INSTDIR\gpremacy_gui"
  File "gpremacy_gui\gpremacy_gui.glade"  
  
  ; Create the batch file
  FileOpen $0 "$INSTDIR\gpremacy-win.bat" w
  FileWrite $0 "@ECHO OFF$\r$\n"
  FileWrite $0 "cd $\"$INSTDIR$\"$\r$\n"
  FileWrite $0 "$\"$MonoPath\bin\mono.exe$\" gpremacy-mono.exe$\r$\n"
  FileWrite $0 "pause"  
  FileClose $0
  
  ; Write the installation path into the registry
  WriteRegStr HKLM SOFTWARE\Gpremacy "Install_Dir" "$INSTDIR"
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Gpremacy" "DisplayName" "Gpremacy"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Gpremacy" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Gpremacy" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Gpremacy" "NoRepair" 1
  WriteUninstaller "uninstall.exe"
  
SectionEnd

; Optional section (can be disabled by the user)
Section "Start Menu Shortcuts"

  CreateDirectory "$SMPROGRAMS\Gpremacy\"
  CreateShortCut "$SMPROGRAMS\Gpremacy\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  CreateShortCut "$SMPROGRAMS\Gpremacy\Gpremacy.lnk" "$INSTDIR\gpremacy-win.bat" "" "$INSTDIR\gpremacy.ico" 0
  
SectionEnd

;--------------------------------

; Uninstaller

Section "Uninstall"
  
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Gpremacy"
  DeleteRegKey HKLM SOFTWARE\Gpremacy

  ; Remove files and uninstaller
  Delete "$INSTDIR\uninstall.exe"
  Delete "$INSTDIR\countries.csv"
  Delete "$INSTDIR\resource_cards.csv"
  Delete "$INSTDIR\countries.csv"
  Delete "$INSTDIR\gpremacy-win.bat"
  Delete "$INSTDIR\TODO"
  Delete "$INSTDIR\README"
  Delete "$INSTDIR\LICENSE"
  Delete "$INSTDIR\gpremacy-mono.exe"
  Delete "$INSTDIR\gpremacy-mono.xml"
  Delete "$INSTDIR\gpremacy_gui.glade"
  Delete "$INSTDIR\Graphics\Flags\*.*"  
  Delete "$INSTDIR\Graphics\*.*"

  ; Remove shortcuts, if any
  Delete "$SMPROGRAMS\Gpremacy\*.*"

  ; Remove directories used  
  RMDir "$SMPROGRAMS\Gpremacy"
  ; Remove this recursively 
  RMDir /r "$INSTDIR\Graphics"
  RMDir /r "$INSTDIR\gpremacy_gui"  
  RMDir "$INSTDIR"

SectionEnd
