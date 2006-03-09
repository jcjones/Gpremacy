; Gpremacy Installer for Windows
; Note: look for Mono's directory in VERSION
; where VERSION = HKEY_LOCAL_MACHINE\SOFTWARE\Novell\Mono\DefaultCLR
; (such as HKEY_LOCAL_MACHINE\SOFTWARE\Novell\Mono\1.1.9.2)
;--------------------------------

!define VERSION "0.4.0 alpha"

;--------------------------------
;Include Modern UI

  !include "MUI.nsh"

; The name of the installer
  Name "Gpremacy ${VERSION}"

; The file to write
  OutFile "Gpremacy-${VERSION}-Windows-Setup.exe"

; The default installation directory
  InstallDir $PROGRAMFILES\Gpremacy

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
  InstallDirRegKey HKLM "Software\Gpremacy" "Install_Dir"

  ShowInstDetails show
  Var MonoPath

  !define MUI_HEADERIMAGE
  !define MUI_HEADERIMAGE_BITMAP "InstallerExtras\logo.bmp"

;--------------------------------
;Interface Settings

  !define MUI_ABORTWARNING

;--------------------------------
; Pages

  !insertmacro MUI_PAGE_LICENSE "LICENSE"
  !insertmacro MUI_PAGE_COMPONENTS  
  !insertmacro MUI_PAGE_DIRECTORY
  !insertmacro MUI_PAGE_INSTFILES
  
  !insertmacro MUI_UNPAGE_CONFIRM
  !insertmacro MUI_UNPAGE_INSTFILES
  
;--------------------------------
;Languages
 
  !insertmacro MUI_LANGUAGE "English"


;--------------------------------
; Functions

; Uses $0 for the URL
Function openLinkNewWindow
  Push $3 
  Push $2
  Push $1
  Push $0
  ReadRegStr $0 HKCR "http\shell\open\command" ""
# Get browser path
    DetailPrint $0
  StrCpy $2 '"'
  StrCpy $1 $0 1
  StrCmp $1 $2 +2 # if path is not enclosed in " look for space as final char
    StrCpy $2 ' '
  StrCpy $3 1
  loop:
    StrCpy $1 $0 1 $3
    DetailPrint $1
    StrCmp $1 $2 found
    StrCmp $1 "" found
    IntOp $3 $3 + 1
    Goto loop
 
  found:
    StrCpy $1 $0 $3
    StrCmp $2 " " +2
      StrCpy $1 '$1"'
 
  Pop $0
  Exec '$1 $0'
  Pop $1
  Pop $2
  Pop $3
FunctionEnd

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

;isInstalled:
    DetailPrint "Found Mono version $2"
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
    MessageBox MB_YESNO "It doesn't appear that you have Mono installed, yet it is required for Gpremacy. Would you like to visit the Mono website now so that you can download Mono? (Once you've installed Mono you should then re-run this installer for Gpremacy)" IDYES openWeb IDNO exitInstall
    openWeb:        
        StrCpy $0 "http://www.mono-project.com/Downloads"
        Call openLinkNewWindow
    exitInstall:
        Quit

done:
    Pop $3
    Pop $2
    Pop $1
FunctionEnd

;--------------------------------
; The stuff to install
Section "Gpremacy (required)" SecGpremacy

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
  DetailPrint "Creating customized Gpremacy batch file for Mono install in $MonoPath..."
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
Section "Start Menu Shortcuts" SecShortCuts

  CreateDirectory "$SMPROGRAMS\Gpremacy\"
  CreateShortCut "$SMPROGRAMS\Gpremacy\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  CreateShortCut "$SMPROGRAMS\Gpremacy\Gpremacy.lnk" "$INSTDIR\gpremacy-win.bat" "" "$INSTDIR\Graphics\gpremacy.ico" 0
  
SectionEnd

Section "Desktop Shortcut" DesktopShortCut
  CreateShortCut "$DESKTOP\Gpremacy.lnk" "$INSTDIR\gpremacy-win.bat" "" "$INSTDIR\Graphics\gpremacy.ico" 0
SectionEnd

;--------------------------------
;Descriptions

  ;Language strings
  LangString DESC_SecGpremacy ${LANG_ENGLISH} "Gpremacy, the Game of the Superpowers main files"
  LangString DESC_SecShortCuts ${LANG_ENGLISH} "Add short cuts to the Start Menu to simplify starting Gpremacy."  
  LangString DESC_DesktopShortCut ${LANG_ENGLISH} "Add a shortcut to the game on your Desktop."    

  ;Assign language strings to sections
  !insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
    !insertmacro MUI_DESCRIPTION_TEXT ${SecGpremacy} $(DESC_SecGpremacy)
    !insertmacro MUI_DESCRIPTION_TEXT ${SecShortCuts} $(DESC_SecShortCuts)
    !insertmacro MUI_DESCRIPTION_TEXT ${DesktopShortCut} $(DESC_DesktopShortCut)        
  !insertmacro MUI_FUNCTION_DESCRIPTION_END


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
  Delete "$DESKTOP\Gpremacy.lnk"

  ; Remove directories used  
  RMDir "$SMPROGRAMS\Gpremacy"
  ; Remove this recursively 
  RMDir /r "$INSTDIR\Graphics"
  RMDir /r "$INSTDIR\gpremacy_gui"  
  RMDir "$INSTDIR"

SectionEnd
