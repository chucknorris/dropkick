; Done by Rajat for AutoHotkey
;
; Script Function:
;	Install syntax highlighting and clip library files.
;	Add hotkeys to TextPad for Launching Script and Context Sensitive Help

; August 29, 2006
; - Uses A_AhkPath as a new fallback method to find where AutoHotkey is installed.

; June 2, 2005:
; - Uses a different method to find where AutoHotkey is installed.
; - Avoids using SetWorkingDir so that this script can be started in any directory.
; - Improved the "Run script" shortcut to use ErrStdOut on Windows NT/2k/XP or later.
; - Added a simple GUI to prompt for extra options at the end of the installation.


; Discover where AutoHotkey and its related files reside:
RegRead, AhkDir, HKLM, SOFTWARE\AutoHotkey, InstallDir
if (ErrorLevel or not FileExist(AhkDir . "\AutoHotkey.exe"))  ; Not found, so try best-guess instead.
	SplitPath, A_AhkPath,, AhkDir

; Get path to TextPad:
RegRead, TextPadDir, HKEY_CLASSES_ROOT, TextPad.tws\shell\open\Command,
if TextPadDir =
	RegRead, TextPadDir, HKEY_CLASSES_ROOT, Applications\TextPad.exe\shell\edit\Command,
if TextPadDir =
	TextPadDir = %A_ProgramFiles%\TextPad 4  ; Try a best-guess location.
else
{
	StringReplace, TextPadDir, TextPadDir, ", , all  ; Remove all double quotes.
	SplitPath, TextPadDir, , TextPadDir
}
IfNotExist, %TextPadDir%
{
	MsgBox, TextPad could not be found (the directory "%TextPadDir%" does not exist).`n`nThis script will now exit.
	ExitApp
}

Gui -SysMenu
Gui, Font, s12
Gui, Add, Text, , This script will install syntax highlighting and clip library files for TextPad v4.`nSelect any extra options you want enabled:
Gui, Font, s10
Gui, Add, Checkbox, checked vDefaultEditor, Make TextPad the default editor for AutoHotkey scripts.
Gui, Font, cRed
Gui, Add, Text, y+20, Warning:%A_Space%
Gui, Font, cDefault
Gui, Add, Text, x+0, If you have any custom tool items in your TextPad Tools menu,`nchecking any of the following might replace/overwrite them.
Gui, Add, Checkbox, vShortcutRun xm, Make 'Ctrl+1' a shortcut for running the script currently loaded in TextPad.
Gui, Add, Checkbox, vShortcutHelp, Make 'Ctrl+2' a shortcut for AutoHotkey context sensitive help in TextPad.
Gui, Add, Checkbox, vShortcutIntelliSense, Make 'Ctrl+3' a shortcut for starting AutoHotkey IntelliSense in TextPad.
Gui, Add, Text, , Note: The shortcut keys above can be changed in TextPad 'Configure Menu > Preferences > KeyBoard'
Gui, Add, Button, y+20 w100 Default, &Install
Gui, Add, Button, x+20 wp, &Cancel
Gui, Show, , Configure TextPad for AutoHotkey
return



GuiClose:
ButtonCancel:
ExitApp



ButtonInstall:
Gui Submit
ExtrasPath = %AhkDir%\Extras\Editors\TextPad

FileCopy, %ExtrasPath%\AutoHotkey.syn, %TextPadDir%\system, 1
if ErrorLevel <> 0
{
	MsgBox, Could not copy %ExtrasPath%\AutoHotkey.syn
	ExitApp
}
FileCopy, %ExtrasPath%\AutoHotkey.tcl, %TextPadDir%\Samples, 1
if ErrorLevel <> 0
{
	MsgBox, Could not copy %ExtrasPath%\AutoHotkey.tcl
	ExitApp
}

; Now write the reg keys (use the non-abbreviated root key names in case user has an older version of AutoHotkey)
RegWrite, REG_DWORD, HKEY_CURRENT_USER, Software\Helios\TextPad 4\Document Classes\AutoHotkey, Type, 2
RegWrite, REG_MULTI_SZ, HKEY_CURRENT_USER, Software\Helios\TextPad 4\Document Classes\AutoHotkey, Members, *.ahk
RegWrite, REG_BINARY, HKEY_CURRENT_USER, Software\Helios\TextPad 4\Document Classes\AutoHotkey, Properties, 46000000010000000100000001000000
RegWrite, REG_BINARY, HKEY_CURRENT_USER, Software\Helios\TextPad 4\Document Classes\AutoHotkey, Colors, 010000000100000001000000010000000100000001000000010000000100000001000000010000000100000001000000010000000100000001000000010000008006c800010000000100000001000000010000000100000001000000010000000100000001000000010000000100000001000000010000000100000001000000010000000100000001000000010000000100000001000000010000000100000001000000010000008080800001000000
RegWrite, REG_BINARY, HKEY_CURRENT_USER, Software\Helios\TextPad 4\Document Classes\AutoHotkey, SyntaxProps, 01000000
Regwrite, REG_BINARY, HKEY_CURRENT_USER, Software\Helios\TextPad 4\Document Classes\AutoHotkey,TabStops,00000000000000000400040000000000406CB800C40306004AD85000FFFFFFFF18EE1200C805B80000010000FE32470018EE12008D000000C805B800904E1500E803B80044EE12000000000078DD4F0018EE120001000000E803B800ACF91200BE02050001000000E803B8008D00000001000000ACF9120000000000904E150008EE120004F41200CFE7520000000000
RegWrite, REG_SZ, HKEY_CURRENT_USER, Software\Helios\TextPad 4\Document Classes\AutoHotkey, SyntaxFile, AutoHotkey.syn
RegWrite, REG_SZ, HKEY_CURRENT_USER, Software\Helios\TextPad 4\Document Classes\AutoHotkey, WordChars, _

if DefaultEditor = 1
	RegWrite, REG_SZ, HKEY_CLASSES_ROOT, AutoHotkeyScript\Shell\Edit\Command, , "%TextPadDir%\TextPad.exe" "`%1"

if ShortcutRun = 1
{
	; Tried using the following, but it doesn't seem to add much value in TextPad because unlike EditPlus,
	; it put the caret/focus to the line indicated in the syntax error message.  In other words, the standard
	; syntax-error dialog is probably more useful since it doesn't open a secondary TextPad window containing
	; the results of every script launch:
	; Command: %comspec%
	; Parameters: /c ""%AhkDir%\AutoHotkey.exe" /ErrorStdOut "$File""
	RegWrite, REG_BINARY, HKEY_CURRENT_USER, Software\Helios\TextPad 4\Tools\0, Properties, 000000005680000044000000
	RegWrite, REG_SZ, HKEY_CURRENT_USER, Software\Helios\TextPad 4\Tools\0, MenuText, Run Script
	RegWrite, REG_SZ, HKEY_CURRENT_USER, Software\Helios\TextPad 4\Tools\0, Command, %AhkDir%\AutoHotkey.exe
	RegWrite, REG_SZ, HKEY_CURRENT_USER, Software\Helios\TextPad 4\Tools\0, Parameters, "$File"
	RegWrite, REG_SZ, HKEY_CURRENT_USER, Software\Helios\TextPad 4\Tools\0, Folder, $FileDir
	RegWrite, REG_SZ, HKEY_CURRENT_USER, Software\Helios\TextPad 4\Tools\0, RE,
}


if ShortcutHelp = 1
{
	RegWrite, REG_BINARY, HKEY_CURRENT_USER, Software\Helios\TextPad 4\Tools\1, Properties, 020000005780000004000000
	RegWrite, REG_SZ, HKEY_CURRENT_USER, Software\Helios\TextPad 4\Tools\1, MenuText, AutoHotkey Help
	RegWrite, REG_SZ, HKEY_CURRENT_USER, Software\Helios\TextPad 4\Tools\1, Command, HH.EXE
	RegWrite, REG_SZ, HKEY_CURRENT_USER, Software\Helios\TextPad 4\Tools\1, Parameters, %AhkDir%\AutoHotkey.chm
	RegWrite, REG_SZ, HKEY_CURRENT_USER, Software\Helios\TextPad 4\Tools\1, Folder,
	RegWrite, REG_SZ, HKEY_CURRENT_USER, Software\Helios\TextPad 4\Tools\1, RE,
}


if ShortcutIntelliSense = 1
{
	Regwrite, REG_BINARY, HKEY_CURRENT_USER, Software\Helios\TextPad 4\Tools\2, Properties, 000000005880000044000000
	Regwrite, REG_SZ, HKEY_CURRENT_USER, Software\Helios\TextPad 4\Tools\2, MenuText, IntelliSense for AutoComplete
	Regwrite, REG_SZ, HKEY_CURRENT_USER, Software\Helios\TextPad 4\Tools\2, Command, %AhkDir%\AutoHotkey.exe
	Regwrite, REG_SZ, HKEY_CURRENT_USER, Software\Helios\TextPad 4\Tools\2, Parameters, "%TextPadDir%\system\IntelliSense.ahk"
	Regwrite, REG_SZ, HKEY_CURRENT_USER, Software\Helios\TextPad 4\Tools\2, Folder, $FileDir
	Regwrite, REG_SZ, HKEY_CURRENT_USER, Software\Helios\TextPad 4\Tools\2, RE,

	FileCopy, %AhkDir%\Extras\Scripts\IntelliSense.ahk, %TextPadDir%\system, 1
	FileCopy, %AhkDir%\Extras\Editors\Syntax\Commands.txt, %TextPadDir%\system\AHKCommands.txt, 1
}

SplashTextOn,,, Installation Complete
Sleep 2000
SplashTextOff

ExitApp  ; Must do this since GUI scripts are persistent.
