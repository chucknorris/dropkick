;;; ============================================================================
;;;   FILENAME: Run this to install syntax and clip files for ahk.xml
;;; ============================================================================
;;;   Install AutoHotkey syntax highlighting and clip files in jEdit
;;; ============================================================================

;;;   AUTHOR:  Andreas Gleichmann
;;;   VERSION: 1.0.0, 26.05.2005
;;;   	Based on installscript 'Run this to install syntax and clip files for PSPad.ahk' from Scott Greenberg
;;;
;;; ============================================================================
;;;   HISTORY:
;;; ============================================================================
;;;   DISCLAIMER:
;;;   Permission to use, copy, modify, and distribute this software 
;;;   for any purpose and without fee is hereby granted, provided 
;;;   that the above copyright notice appears in all copies and that 
;;;   both that copyright notice and this permission notice appear in 
;;;   all supporting documentation.
;;;
;;;   THIS SOFTWARE IS PROVIDED "AS IS" WITHOUT EXPRESS OR IMPLIED
;;;   WARRANTY.  ALL IMPLIED WARRANTIES OF FITNESS FOR ANY PARTICULAR
;;;   PURPOSE AND OF MERCHANTABILITY ARE HEREBY DISCLAIMED.
;;; ============================================================================

;;; Directives, required by this script (do not change)
#SingleInstance force
#EscapeChar \
SetBatchLines,-1
ScriptTitle = AutoHotkey jEdit Setup
;;;  Used to aid in bugging script
;; debugging on  = 1
;; debugging off = 0
bDebug = 0
IfNotEqual, bDebug, 1
{
    MsgBox, 4, %ScriptTitle%, This will install file for jEdit to handle AutoHotkey scripts.\nWould you like to continue with the install?
    IfMsgBox, No
        ExitApp
}


jEditTitleName=jEdit - 
jEditUserPath=%USERPROFILE%\\.jedit
jEditCatalogPath=%jEditUserPath%\\modes
jEditCatalogFile=catalog
jEditSyntaxFile=ahk.xml
jEditCatalogPathFile=%jEditCatalogPath%\\%jEditCatalogFile%
;;; Allow user to select from dialog, when unable to locate PSPad
IfNotExist, %jEditCatalogPath%
    FileSelectFile, jEditCatalogPathFile, 1, %jEditCatalogPath%\\%jEditCatalogFile%, Select the jEdit catalog File.
SplitPath, jEditCatalogPathFile, jEditCatalogFile, jEditCatalogPath
IfEqual, bDebug, 1
	MsgBox, 0, %ScriptTitle%, jEditCatalogPathFile:%jEditCatalogPathFile%, jEditCatalogFile:%jEditCatalogFile%, jEditCatalogPath:%jEditCatalogPath%

;;; Discover where AutoHotkey and its related files reside.
RegRead, AhkDir, HKLM, SOFTWARE\\AutoHotkey, InstallDir
if (ErrorLevel or not FileExist(AhkDir . "\\AutoHotkey.exe"))  ; Not found, so try best-guess instead.
	SplitPath, A_AhkPath,, AhkDir
AHKExe = %AhkDir%\\AutoHotkey.exe

;;; Allow user to select from dialog, when unable to locate AutoHotkey
IfNotExist, %AHKExe%
    FileSelectFile, AHKExe, 1, %A_ProgramFiles%\\AutoHotkey\\AutoHotkey.exe, Select the AutoHotkey executable., Executable (*.exe)
IfNotExist, %AHKExe%
    Goto, DownloadAHK

;;; Copy files to jEdit dir
file = %A_WORKINGDIR%\\%jEditSyntaxFile%
IfExist, %file%
{
	; Make backup of old file if it exists and if there isn't already a backup file (this relies
	; on it failing to do anything otherwise):
	FileMove, %jEditCatalogPath%\\%jEditSyntaxFile%, %jEditCatalogPath%\\%jEditSyntaxFile%.backup
	FileCopy, %file%, %jEditCatalogPath%, 1
}

;;; add the syntax file to the jEdit catalog
;;search <MODE NAME="ahk" FILE="ahk.xml" FILE_NAME_GLOB="*.{ahk}"/> in catalog file
Found=0
IfEqual, bDebug, 1
    MsgBox, 0, %ScriptTitle%, Search %jEditCatalogPathFile%.
Loop, Read, %jEditCatalogPathFile%
{
    ifInString, A_LoopReadLine, NAME="ahk"
    {
	IfEqual, bDebug, 1
		MsgBox, 0, %ScriptTitle%, AHK Syntaxfile already exist in the catalog file. Nothing to do
	Found=1
	break
    }
}

if Found=0
{
    ;;; Close jEdit
    IfWinExist, %jEditTitleName%
    {
	If bDebug <> 1
	    WinClose ; use the window found above
    }
    
   jEditCatalogPathFile_new=%jEditCatalogPathFile%_new
    IfEqual, bDebug, 1
	    MsgBox, 0, %ScriptTitle%, Add AHK Syntaxfile to the catalog file %jEditCatalogPathFile_new%.

    IfExist, %jEditCatalogPathFile_new%, FileDelete, %jEditCatalogPathFile_new%
    
    Loop, Read, %jEditCatalogPathFile%, %jEditCatalogPathFile_new% 
    {
	ifInString, A_LoopReadLine, </MODES>
	{
	    IfEqual, bDebug, 1
		    MsgBox, 0, %ScriptTitle%, Add definition line to the catalog file.
		FileAppend, <MODE NAME="ahk" FILE="ahk.xml" FILE_NAME_GLOB="*.{ahk}"/>\n
	}
	FileAppend, %A_LoopReadLine%\n
    }
    IfExist, %jEditCatalogPathFile%.backup, FileDelete, %jEditCatalogPathFile%.backup
    FileMove, %jEditCatalogPathFile%, %jEditCatalogPathFile%.backup
    FileMove, %jEditCatalogPathFile_new%, %jEditCatalogPathFile%
}

ExitApp


DownloadAHK:
url = http://www.autohotkey.com/
valSet := 0
Gui, Font, norm
Gui, Add, Text,, We sure AutoHotkey is installed but we are unable to find it. Unable to continue.
Gui, Font, underline
Gui, Add, Text, cBlue gOpenLink, Click here to Download AutoHotkey.
Gui, Font, norm
Gui, Add, Button, Default, &OK
Gui, Show, AutoSize, ScriptTitle
Loop
{
    if valSet
        break
}
ExitApp -1

OpenLink: 
Run, %url%
ExitApp -1
return
