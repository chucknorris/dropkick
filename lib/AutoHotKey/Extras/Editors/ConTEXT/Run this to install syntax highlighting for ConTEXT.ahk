; Installs AutoHotkey syntax highlighting for the ConTEXT editor.
;
; Created by foom
; Color theme is based on the UnrealScript highlighter found on:
; http://wiki.beyondunreal.com/wiki/ConTEXT/UnrealScript
; Which itself resembles the highlighting syntax/theme of the UScript Editor in UnrealEd© by Epic Games Inc.
;
; Report Problems at:
; http://www.autohotkey.com/forum/topic10895.html
;
; 10.29.2006
; - Initial release.
; 11.09.2006
; - Fixed "if" not beeing highlighted. Maybe other words too.
; - Fixed don't display the succesfully installed msgbox if user cancels contextdir selection.
; 05.11.2006
; - Fixed a typo which prevented the codetemplate from being installed if it allready existed.

SetBatchLines -1
#SingleInstance force


; Discover where AutoHotkey and its related files reside:
RegRead, AhkDir, HKLM, SOFTWARE\AutoHotkey, InstallDir
if (ErrorLevel or not FileExist(AhkDir . "\AutoHotkey.exe"))  ; Not found, so try best-guess instead.
	SplitPath, A_AhkPath,, AhkDir

; Get path to ConTEXT:
RegRead, contextDir, HKEY_LOCAL_MACHINE, SOFTWARE\Classes\Applications\ConTEXT.exe\shell\edit\command
if contextDir
	SplitPath, contextDir,, contextDir  ; This removes the following string from the end: \ConTEXT.exe "%1"
else  ; Try a best-guess location.
	contextDir = %A_ProgramFiles%\ConTEXT
IfNotExist, %contextDir%
    Gosub, selectcontextdir

;Do not edit the settings below. You might, but don't! Edit the values in ConTEXT settings instead. You've been warned.
highlightsettings=
(
Language:            AutoHotkey
Filter:              AutoHotkey (*.ahk)|*.ahk
HelpFile:            %AhkDir%\AutoHotkey.chm
CaseSensitive:       0
LineComment:         ;

//dont use blockcomments, context blockcomments are not ahk conform
BlockCommentBeg:
BlockCommentEnd:

IdentifierBegChars:  a..z A..Z _#
IdentifierChars:     a..z A..Z _# 0..9
NumConstBegChars:    0..9
NumConstChars:       0..9 .abcdefhABCDEFH

EscapeChar:          `
StringBegChar:       "
StringEndChar:       "
MultilineStrings:    0
UsePreprocessor:     0
CurrLineHighlighted: 1

SpaceCol:            clWhite $00400000
Keyword1Col:         clAqua $00400000
Keyword2Col:         clWhite $00320000 B
Keyword3Col:         $00FFBC79 $00400000
Keyword4Col:         clWhite $00620000

// clAqua $00400000
Keyword5Col:         $00FFC0C0 $00400000
IdentifierCol:       clWhite $00400000
CommentCol:          $00909090 $00410000
NumberCol:           clWhite $00400000
StringCol:           clLime $00400000
SymbolCol:           clWhite $00400000

// clGray $00400000
PreprocessorCol:     $00FFC0C0 $00400000
SelectionCol:        $00400000 $00BFFFFF
CurrentLineCol:      clWhite $00600000
MatchedBracesCol:    clYellow $00400000

OverrideTxtFgColor:  0
BlockAutoindent:     1
BlockBegStr:         {
BlockEndStr:         }

)

commandstxt=%ahkdir%\extras\editors\syntax\commands.txt
commandnamestxt=%ahkdir%\extras\editors\syntax\commandnames.txt
functionstxt=%ahkdir%\extras\editors\syntax\functions.txt
variablestxt=%ahkdir%\extras\editors\syntax\variables.txt
keystxt=%ahkdir%\extras\editors\syntax\keys.txt
keywordstxt=%ahkdir%\extras\editors\syntax\keywords.txt

AutoHotkeyctpl=%ahkdir%\extras\editors\syntax\AutoHotkey.ctpl
AutoHotkeychl =%ahkdir%\extras\editors\syntax\AutoHotkey.chl



if (!FileExist(commandstxt) or !FileExist(functionstxt) or !FileExist(variablestxt) or !FileExist(keywordstxt) or !FileExist(keystxt) or !FileExist(commandnamestxt))
{
    MsgBox,16,Error, Couldn't find syntax files. Exiting.
    exitapp
}

Gosub, deletefiles

Loop, Read, %commandstxt%
{
    Command=
    Desc=
    Out=%A_LoopReadLine%
    parse_template(Command, Desc, Out)
    
    loop
    {
        if command in %commandlist%
            command:=command . "_"
        else
            break
    }

    if commandlist
        commandlist:=commandlist . "," . Command
    else
        commandlist:=command

    FullOut=[%Command% | %Desc%]`n%Out%`n`r`n
    ahktemplate=%ahktemplate%%FullOut%
}

commandlist=

Loop, Read, %functionstxt%
{
    Command=
    Desc=
    Out=%A_LoopReadLine%
    parse_template(Command, Desc, Out)

    loop
    {
        if command in %commandlist%
            command:=command . "_"
        else
            break
    }

    if commandlist
        commandlist:=commandlist . "," . Command
    else
        commandlist:=command

    FullOut=[%Command% | %Desc%]`n%Out%`n`r`n
    ahktemplate=%ahktemplate%%FullOut%
}

commandlist=

Loop, Read, %variablestxt%
{
    Command=
    Desc=
    Out=%A_LoopReadLine%
    parse_template(Command, Desc, Out)

    FullOut=[%Command% | A_BuildInVariable]`n%Out%`n`r`n
    ahktemplate=%ahktemplate%%FullOut%
}

FileAppend, %ahktemplate%, %AutoHotkeyctpl%



ahkchl=

;functions
FileRead, syntaxfile, %functionstxt%
ahkchl:="KeyWords1:" . parse_hl(syntaxfile, ",,," . a_space . "," . a_tab  . ",(,[,{,;,`n") . "`n`n"

;commandnames (no preprozessor)
FileRead, syntaxfile, %commandnamestxt%
ahkchl:=ahkchl . parse_hl(syntaxfile, ",,," . a_space . "," . a_tab  . ",(,[,{,;,`n" ,"#",0) . "`n`n"

;keys
FileRead, syntaxfile, %keystxt%
ahkchl:=ahkchl . "KeyWords2:" . parse_hl(syntaxfile, ",,," . a_space . "," . a_tab  . ",(,[,{,;,`n") . "`n`n"

;keywords
FileRead, syntaxfile, %keywordstxt%
ahkchl:=ahkchl . "KeyWords3:" . parse_hl(syntaxfile, ",,," . a_space . "," . a_tab  . ",(,[,{,;,`n") . "`nforce`n`n`n"

;built-in variables
FileRead, syntaxfile, %variablestxt%
ahkchl:=ahkchl . "KeyWords4:" . parse_hl(syntaxfile, ",,," . a_space . "," . a_tab  . ",(,[,{,;,`n") . "`n`n"

;commandnames (only preprozessor)
FileRead, syntaxfile, %commandnamestxt%
ahkchl:=ahkchl . "KeyWords5:" . parse_hl(syntaxfile, ",,," . a_space . "," . a_tab  . ",(,[,{,;,`n" ,"#",1) . "`n`n"

ahkchl:=highlightsettings . "`n`n" . ahkchl 

FileAppend, %ahkchl%, %AutoHotkeychl%


if FileExist(contextDir . "\Template\AutoHotkey.ctpl")
{
    MsgBox,36,Warning,%contextDir%\Template\AutoHotkey.ctpl already exists. Overwrite that file?
    IfMsgBox, No
        FileCopy, %AutoHotkeyctpl%, %contextDir%\Template\AutoHotkey.ctpl, 0
    IfMsgBox, Yes
        FileCopy, %AutoHotkeyctpl%, %contextDir%\Template\AutoHotkey.ctpl, 1
}
else
    FileCopy, %AutoHotkeyctpl%, %contextDir%\Template\AutoHotkey.ctpl, 0

if FileExist(contextDir . "\Highlighters\AutoHotkey.chl")
{
    MsgBox,36,Warning,%contextDir%\Highlighters\AutoHotkey.chl already exists. Overwrite that file?
    IfMsgBox, No
        FileCopy, %AutoHotkeychl%, %contextDir%\Highlighters\AutoHotkey.chl, 0
    IfMsgBox, Yes
        FileCopy, %AutoHotkeychl%, %contextDir%\Highlighters\AutoHotkey.chl, 1
}
else
    FileCopy, %AutoHotkeychl%, %contextDir%\Highlighters\AutoHotkey.chl, 0

exit:
    gosub, deletefiles
    MsgBox AutoHotkey syntax highlighting for ConTEXT has been successfully installed.
    exitapp
return

selectcontextdir:
    FileSelectFolder, contextDir ,,0, Select the folder ConTEXT is installed in.
    if errorlevel
        goto exit
    
    if !FileExist(contextDir . "\conTEXT.exe")
    {
        msgbox,18,Error,This is not the folder context is installed in. Install anyway? ;`n.Abort - Exit.`nRetry - Select again.`nIgnore - Install files anyway.`n
        IfMsgBox, Abort
        {
         gosub, deletefiles
         exitapp
        }

        IfMsgBox, Retry
            goto selectcontextdir
        IfMsgBox, Ignore
        {
            FileCreateDir, %contextDir%\Template
            FileCreateDir, %contextDir%\Highlighters
        }
    }
return

deletefiles:
    if FileExist(autohotkeyctpl)
    	FileDelete, %autohotkeyctpl%
    if FileExist(autohotkeychl)
    	FileDelete, %autohotkeychl%
return

parse_hl(file,lastcharlist,firstcharlist="",bIfFirstChar=0 )
{
    Loop, Parse, file, `n, `n
    {
        firstword=
        Loop, Parse, A_LoopField
        {
            if (firstcharlist and a_index = 1)
            {
                if bIfFirstChar
                {
                    if a_loopfield not in %firstcharlist%
                        break
                }
                else if !bIfFirstChar
                {
                    if a_loopfield in %firstcharlist%
                        break
                }

            }
            if A_LoopField not in %lastcharlist%
                firstword=%firstword%%A_LoopField%
            else
                break
        }
    if firstword
    out=%out%`n%firstword%
    }
    return out
}

parse_template(ByRef Command, ByRef Desc, ByRef Out)
{
    Loop, parse, Out
    {
    
        if bDesc
        {
            if (A_LoopField = "|")
                Desc:=Desc . " or "
            else if (A_LoopField = "[" or a_loopfield = "]")
                continue
            else if A_LoopField = {        ;,},`t
                      Desc:=Desc . "("
            else if A_LoopField = }        ;,},`t
                      Desc:=Desc . ")"
            else if A_LoopField = %a_tab%        ;,},`t
                      Desc:=Desc . "  "
            else if A_loopfield = ``
            {
                      prevloopfield:=a_loopfield
                      continue
            }
            else if (A_loopfield = "n" and prevloopfield = "``")
                      continue
            else
                Desc:=Desc . A_LoopField
            
            prevloopfield:=a_loopfield

            continue
        }
        
        if (A_LoopField = "," OR A_LoopField = " " OR A_LoopField = "(")
        {
        bDesc=1
        Desc:=Desc . A_LoopField
        }
        
;         MsgBox, % Command . a_loop Field
        if !bDesc
            Command=%Command%%A_LoopField%
    ;   MsgBox, 4, , File number %A_Index% is %A_LoopField%.`n`nContinue?
    ;   IfMsgBox, No, break
    }
    StringReplace, Out, Out, | ,%a_space%or%a_space%, All
    StringReplace, Out, Out,[,, All
    StringReplace, Out, Out,],, All
    StringReplace, Out, Out,{,(, All
    StringReplace, Out, Out,},), All
    StringReplace, Out, Out,%a_tab%,%a_space%%a_space%, All
    StringReplace, Out, Out,``n,, All
}
