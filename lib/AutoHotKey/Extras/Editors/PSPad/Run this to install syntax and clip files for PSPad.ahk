;;; ============================================================================
;;;   FILENAME: Run this to install syntax and clip files for PSPad.ahk
;;; ============================================================================
;;;   Install AutoHotkey syntax highlighting and clip files in PSPad
;;; ============================================================================

;;;   AUTHOR:  Scott Greenberg  
;;;   COMPANY: SG Technology
;;;   VERSION: 1.0.1, 02/05/2005 - 02/08/2005
;;;   WEBSITE: http://gogogadgetscott.info/
;;;   Copyright 2005. SG Technology. All rights reserved.
;;;
;;; ============================================================================
;;;   HISTORY:
;;;    1.0.3
;;;     - The "remove the [ ] brackets" mode overwrites the DEF file (not append).
;;;    1.0.2
;;;     - The INI file is copied to PSPad's Syntax folder, not its Context folder.
;;;     - Fixed the "remove the [ ] brackets" mode to use backslash as escape char.
;;;     - Original ini file is backed up prior to being overwritten.
;;;    1.0.1
;;;     - Fixed some spelling errors.
;;;     - Removed ] (typo) from line No 42.
;;;     - Changed "clip library files" to "clip files".
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
ScriptTitle = AutoHotkey PSPpad Setup
;;;  Used to aid in bugging script
;; debugging on  = 1
;; debugging off = 0
bDebug = 0
IfNotEqual, bDebug, 1
{
    MsgBox, 4, %ScriptTitle%, This will install file for PSPad to handle AutoHotkey scripts.\nWould you like to continue with the install?
    IfMsgBox, No
        ExitApp
}
;;; Get PSPad exe path from registry.
RegRead, PSPadExe, HKCU, Software\\PSPad, PSPadExe
;;; Remove quotes from path
StringReplace, PSPadExe, PSPadExe, ",, A
IfNotEqual, bDebug, 0
    PSPadExe = C:\\Program Files\\PSPad\\PSPad.exe
;;; Allow user to select from dialog, when unable to locate PSPad
IfNotExist, %PSPadExe%
    FileSelectFile, PSPadExe, 1, %A_ProgramFiles%\\PSPad\\PSPad.exe, Select the PSPad executable., Executable (*.exe)
IfNotExist, %PSPadExe%
    Goto, DownloadPSPad
SplitPath, PSPadExe, PSPadName, PSPadDir

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

;;; Close PSPad
Process, Exist, %PSPadName%
if ErrorLevel <> 0
{
    If bDebug <> 1
        Process, Close, %PSPadName%
}
;;; Copy files to PSPad dir
file = %A_WORKINGDIR%\\AutoHotkey.def
IfExist, %file%
{
	;;; Added by beardboy:
	MsgBox, 4, Customize the clip file (.DEF), Would you like to remove the [ ] brackets from optional parameters of commands in the clip file?, 30
	IfMsgBox, Yes
	{
		FileDelete, %PSPadDir%\\Context\\AutoHotkey.def  ; If it exists, overwrite.
		Loop, read, %file%, %PSPadDir%\\Context\\AutoHotkey.def
		{
			IfInString, A_LoopReadLine, [,
			{
				StringReplace, temp, a_loopreadline, %a_space%[,, All
				StringReplace, temp, temp, ],, All
				StringReplace, temp, temp, %a_space%\,, \,, All
				FileAppend, %temp%\n
			}
			else
				FileAppend, %A_LoopReadLine%\n
		}
	}
	else
		FileCopy, %file%, %PSPadDir%\\Context\\, 1
}

;;; End

file = %A_WORKINGDIR%\\AutoHotkey.ini
IfExist, %file%
{
	; Make backup of old file if it exists and if there isn't already a backup file (this relies
	; on it failing to do anything otherwise):
	FileMove, %PSPadDir%\\Syntax\\AutoHotkey.ini, %PSPadDir%\\Syntax\\AutoHotkey (previous).ini
    FileCopy, %file%, %PSPadDir%\\Syntax\\, 1
}
;;; Get current user highlighter settings from ini
PSPadINI = %PSPadDir%\\PSPad.INI
IniRead, UserHighLighterName1, %PSPadINI%, Config, UserHighLighterName, General
IniRead, UserHighLighterName2, %PSPadINI%, Config, UserHighLighterName1, General
IniRead, UserHighLighterName3, %PSPadINI%, Config, UserHighLighterName2, General
IniRead, UserHighLighterName4, %PSPadINI%, Config, UserHighLighterName3, General
IniRead, UserHighLighterName5, %PSPadINI%, Config, UserHighLighterName4, General
FoundE = 0
FoundP = 0
Loop, 5
{
    value := UserHighLighterName%a_index%
    If value =
    {
        FoundE = %a_index%
        break
    }
    StringGetPos, iPos, value, General
    IfGreaterOrEqual, iPos, 0
    {
        FoundE = %a_index%
        break
    }
    StringGetPos, iPos, value, AutoHotkey
    IfGreaterOrEqual, iPos, 0
    {
        FoundP = %a_index%
        break
    }
}
If FoundP
{
    MsgBox, 4, %ScripT%, PSPad is already setup to handle AutoHotkey scripts.\nWould you like to continue anyways?,
    IfMsgBox, No
        ExitApp, -1  
    FoundE = %FoundP%
}
;;; Setup AutoHotkey as an user highlighter in ini
If FoundE <> 0
{
    EnvSub, FoundE, 1
    IfLessOrEqual, FoundE, 0
        FoundE = 
    Key = UserHighLighterName%FoundE%
    IniWrite, AutoHotkey, %PSPadINI%, Config, %Key%
}
else
{
    Gosub, SelectUserHighLighter
    EnvSub, FoundE, 1
    IfLessOrEqual, FoundE, 0
        FoundE = 
    Key = UserHighLighterName%FoundE%
    IniWrite, AutoHotkey, %PSPadINI%, Config, %Key%
}
;;; Added configuration for highlighter to ini
IniWrite, AutoHotkey (*.ahk)|*.ahk, %PSPadINI%, AutoHotkey, Filter
value = %AHKDir%\\AutoHotkey.chm
IniWrite, %value%, %PSPadINI%, AutoHotkey, Compilator Help
value = %AHKDir%\\Compiler\\Ahk2Exe.exe
IniWrite, %value%, %PSPadINI%, AutoHotkey, Compilator File
IniWrite, /in "\%File\%", %PSPadINI%, AutoHotkey, Compilator Param
value = Run,|%AHKDir%\\AutoHotkey.exe| |\%File\%|§
IniWrite, %value%, %PSPadINI%, AutoHotkey, Prog0
value = AU3_Spy,|%AHKDir%\\AU3_Spy.exe|
IniWrite, %value%, %PSPadINI%, AutoHotkey, Prog1
MsgBox, 4, ScriptTitle, Whould you like to setup custom colors w/ light blue background?, 30
IfMsgBox, Yes
{
    IniWrite, 0000800000FFF0DC010, %PSPadINI%, AutoHotkey, Comment
    IniWrite, 1FFFFFFF00FFF0DC000, %PSPadINI%, AutoHotkey, Identifier
    IniWrite, 0080400000FFF0DC100, %PSPadINI%, AutoHotkey, Key
    IniWrite, 000080FF00FFF0DC100, %PSPadINI%, AutoHotkey, Key words 2
    IniWrite, 0000800000FFF0DC100, %PSPadINI%, AutoHotkey, Key words 3
    IniWrite, 000000FF00FFF0DC000, %PSPadINI%, AutoHotkey, Label
    IniWrite, 1FFFFFFF00FFF0DC000, %PSPadINI%, AutoHotkey, Number
    IniWrite, 000000FF00FFF0DC010, %PSPadINI%, AutoHotkey, Preprocessor
    IniWrite, 0000008000FFF0DC100, %PSPadINI%, AutoHotkey, Reserved word
    IniWrite, 0080800000FFF0DC000, %PSPadINI%, AutoHotkey, Space
    IniWrite, 000000FF00FFF0DC000, %PSPadINI%, AutoHotkey, String
    IniWrite, 0000000000FFF0DC000, %PSPadINI%, AutoHotkey, Symbol
}

;;; Open PSPad
If bDebug <> 1
    Run, %PSPadExe%

ExitApp


SelectUserHighLighter:
valSet := 0
Gui, Font, s10 cBlue, Lucida Console
Gui, Add, Text,, Select a User HighLighter to replace:
Gui, Add, Radio, checked vFoundE, &1 %UserHighLighterName1%
Loop, 4
{
    index := a_index + 1
    value := UserHighLighterName%index%
    Gui, Add, Radio,, &%index% %value%
}
Gui, Add, Button, Default, &OK
Gui, Show, AutoSize, ScriptTitle
Loop
{
    if valSet
        break
}
Gui, Submit
Gui, Destroy
Return

ButtonOK:
valSet = 1
Return

GuiClose:
Gui, Submit
Gui, Destroy
valSet = 1
Return

DownloadPSPad:
url = http://www.pspad.com/
valSet := 0
Gui, Font, norm
Gui, Add, Text,, It appears PSPad is not installed. Unable to continue.
Gui, Font, underline 
Gui, Add, Text, cBlue gOpenLink, Click here to Download PSPad.
Gui, Font, norm
Gui, Add, Button, Default, &OK
Gui, Show, AutoSize, ScriptTitle
Loop
{
    if valSet
        break
}
ExitApp -1

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
