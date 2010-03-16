;##############################################################################
;#
;#  Add or update syntax highlighting for AutoHotKey scripts in UltraEdit
;#
;#  Mod of a script done by Tekl (although not much has survived)
;#  Mod done by toralf, 2005-11-14
;#
;#  Tested with: AHK 1.0.40.06, UltraEdit 11.10a
;#
;#  Requirements
;#    - Syntax files for AHK in one directory
;#    - UltraEdit uses standard file for highlighting => wordfile.txt
;#
;#  Customize:
;#    - The default color for strings is gray, change it to any color
;#         you want to have "string" to appeer => Extra->Option->syntaxhiglighting
;#    - Change the default color for up to 8 keyword groups
;#         => Extra->Option->syntaxhiglighting
;#    -specify up to 8 syntax files, each containing one keyword per line
;#         => you can add your own files, for keywords that you want to highlight
;#            Personally I use 3 additional: Operators, Separators and Special
;#            The content of these files is posted further down
;#

; Specify a list of up to 8 syntax files; the order influences the color given to them by UE by default
SyntaxFileNameList = CommandNames|Keywords|Variables|Functions|Keys|Operators|Separators|Special
;Default colors in UE:  blue     |red     |orange   |green    |brown|blue    |blue      |blue

SyntaxExtention = .txt

;#############   END of Customization Area   ##################################

;#############   Ask and Check for valid input  ###############################

RegRead, UeditPath, HKEY_LOCAL_MACHINE, SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\UEDIT32.exe,Path
IfNotExist, %UeditPath%\uedit32.exe
  {
    UeditPath = %A_ProgramFiles%\UltraEdit
    IfNotExist, %UeditPath%\uedit32.exe
      {
        FileSelectFolder, UeditPath,*%A_ProgramFiles%, 0, Select UltraEdit program-folder
        IfNotExist, %UeditPath%\uedit32.exe
          {
            MsgBox UltraEdit cannot be found.
            ExitApp
          }
      }
  }

UEini = %APPDATA%\IDMComp\UltraEdit\uedit32.ini
IfNotExist, %UEini%
  {
    UEini = %A_WinDir%\uedit32.ini
    IfNotExist, %UEini%
        FileSelectFile, UEini, 1, %A_ProgramFiles%\UltraEdit, Select UltraEdit INI-File, *.ini
  }
IniRead, UEwordfile, %UEini%, Settings, Language File
If UEwordfile = Error
  {
    MsgBox INI-File "%UEini%" is missing the Key "Language File".
    ExitApp
  }

;Search or ask for Wordfile, when it doesn't exist -> exit
UEwordfile = %UeditPath%\wordfile.txt
IfNotExist, %UEwordfile%
  {
    FileSelectFile, UEwordfile, 1, %A_ProgramFiles%, Select UltraEdit wordfile, *.txt
    IfNotExist, %UEwordfile%
      {
        MsgBox, 16,, UltraEdit Wordfile cannot be found.
        ExitApp
      }
  }

; Discover where AutoHotkey and its related files reside:
RegRead, ahkpath, HKLM, SOFTWARE\AutoHotkey, InstallDir
if (ErrorLevel or not FileExist(ahkpath . "\AutoHotkey.exe"))  ; Not found, so try best-guess instead.
	SplitPath, A_AhkPath,, ahkpath

PathSyntaxFiles = %ahkpath%\AutoHotkey\Extras\Editors\Syntax
IfNotExist, %PathSyntaxFiles%
  {
    PathSyntaxFiles = %A_ProgramFiles%\AutoHotkey\Extras\Editors\Syntax
    IfNotExist, %PathSyntaxFiles%
      {
        FileSelectFolder, PathSyntaxFiles, *%A_ProgramFiles%,0, Select Folder "AutoHotkey\Extras\Editors\Syntax"
        IfNotExist, %PathSyntaxFiles%
          {
            MsgBox, 16,, Folder containing syntax files not found.
            ExitApp
          }
      }
  }

MissingFile =
FileCount = 0
Loop, Parse, SyntaxFileNameList, |
  {
    FileCount += 1
    IfNotExist, %PathSyntaxFiles%\%A_LoopField%%SyntaxExtention%
        MissingFile = %MissingFile%`n%A_LoopField%%SyntaxExtention%
  }
If MissingFile is not Space
  {
    MsgBox, 16,, AHK Syntax file(s)`n%MissingFile%`n`ncannot be found in`n`n%PathSyntaxFiles%\.
    ExitApp
  }
If FileCount > 8
  {
    MsgBox, 16,, You have specified %FileCount% Syntax files.`nOnly 8 are supported be UltraEdit.`nPlease shorten the list.
    ExitApp
  }

;Check the number of languages in the current wordfile, if more than 19 without AHK -> exit
NumberOfLanguages = 0
Loop, Read, %UEwordfile%
  {
    StringLeft, WFdef, A_LoopReadLine, 2
    If WFdef = /L
      {
        StringSplit, WFname, A_LoopReadLine, "      ;"
        LanguageName = %WFname2%
        If LanguageName <> AutoHotkey
            NumberOfLanguages += 1
      }
  }
If NumberOfLanguages > 19
  {
    MsgBox, 48,, The wordfile has %NumberOfLanguages% syntax-schemes. UltraEdit does only support 20 schemes.`nPlease Delete schemes from the file!
    ExitApp
  }

;#############   Read keywords from syntax files into arrays   ################

Loop, Parse, SyntaxFileNameList, |  ;Read all syntax files
  {
    SyntaxFileName = %A_LoopField%
    Gosub, ReadSyntaxFromFile       ;SyntaxFileName will become string with keywords
  }

;#############   Build language specific highlight for AHK   ##################

StrgAHKwf = "AutoHotkey" Nocase
StrgAHKwf = %StrgAHKwf% Line Comment = `;
StrgAHKwf = %StrgAHKwf% Line Comment Preceding Chars = [~``]     ;to Escape Escaped ;
StrgAHKwf = %StrgAHKwf% Escape Char = ``
StrgAHKwf = %StrgAHKwf% String Chars = "                                                   ;"
StrgAHKwf = %StrgAHKwf% Block Comment On = /*
StrgAHKwf = %StrgAHKwf% Block Comment Off = */
StrgAHKwf = %StrgAHKwf% File Extensions = ahk`n
StrgAHKwf = %StrgAHKwf%/DeLimiters = *~`%+-!^&(){}=|\/:"'``;<>%A_Tab%,%A_Space%.`n         ;"
StrgAHKwf = %StrgAHKwf%/Indent Strings = "{" ":" "("`n
StrgAHKwf = %StrgAHKwf%/Unindent Strings = "}" "Return" "Else" ")"`n
StrgAHKwf = %StrgAHKwf%/Open Fold Strings = "{"`n
StrgAHKwf = %StrgAHKwf%/Close Fold Strings = "}"`n
StrgAHKwf = %StrgAHKwf%/Function String = "`%[^t ]++^(:[^*^?BbCcKkOoPpRrZz0-9- ]++:*`::^)"`n   ; Hotstrings
StrgAHKwf = %StrgAHKwf%/Function String 1 = "`%[^t ]++^([a-zA-Z0-9 #!^^&<>^*^~^$]+`::^)"`n     ; Hotkeys
StrgAHKwf = %StrgAHKwf%/Function String 2 = "`%[^t ]++^([a-zA-Z0-9äöüß#_@^$^?^[^]]+:^)"`n      ; Subroutines
StrgAHKwf = %StrgAHKwf%/Function String 3 = "`%[^t ]++^([a-zA-Z0-9äöüß#_@^$^?^[^]]+(*)^)"`n    ; Functions
StrgAHKwf = %StrgAHKwf%/Function String 4 = "`%[^t ]++^(#[a-zA-Z]+ ^)"`n                       ; Directives

Loop, Parse, SyntaxFileNameList, |      ;Add the keywords from syntax strings into their Sections
  {
    StrgAHKwf = %StrgAHKwf%/C%A_Index%"%A_LoopField%"   ;Section definition
    SyntaxString = %A_LoopField%             ;which Section/syntax
    Gosub, ParseSyntaxString                 ;Parse through string and add to list
  }

;#############   Add or Update Wordfile   #####################################

;Name of a file for temporary store the word file
TemporaryUEwordFile = TempUEwordFile.txt
FileDelete, %TemporaryUEwordFile%

Loop, Read, %UEwordfile%, %TemporaryUEwordFile%   ;Read through Wordfile
  {
    StringLeft, WFdef, A_LoopReadLine, 2
    If WFdef = /L
      {
        StringSplit, WFname, A_LoopReadLine, "                  ;"
        LanguageName = %WFname2%
        LanguageNumber = %WFname1%
        StringTrimLeft,LanguageNumber,LanguageNumber,2
        If LanguageName = AutoHotkey         ;when AHK Section found, place new Section at same location
          {
            FileAppend, /L%LanguageNumber%%StrgAHKwf%
            AHKLanguageFound := True
          }
      }
    If LanguageName <> AutoHotkey            ;everything that does not belong to AHK, gets unchanged to file
        FileAppend, %A_LoopReadLine%`n
  }

If not AHKLanguageFound                      ;when AHK Section not found, append AHK Section
  {
    LanguageNumber += 1
    FileAppend, /L%LanguageNumber%%StrgAHKwf%, %TemporaryUEwordFile%
  }

FileCopy, %UEwordfile%, %UEwordfile%.ahk.bak, 1    ;Create Backup of current wordfile
FileMove, %TemporaryUEwordFile%, %UEwordfile%, 1       ;Replace wordfile with temporary file

; Tell user what has been done
Question = `n`nWould you like to make UltraEdit the Default editor for AutoHotkey scripts (.ahk files)?
If AHKLanguageFound
    MsgBox, 4,, The AutoHotkey-Syntax for UltraEdit has been updated in your wordfile:`n`n%UEwordfile%`n`nA backup has been created in the same folder.%Question%
Else
    MsgBox, 4,, The AutoHotkey-Syntax for UltraEdit has been added to your wordfile:`n`n%UEwordfile%`n`nA backup has been created in the same folder.%Question%

IfMsgBox, Yes
    RegWrite, REG_SZ, HKEY_CLASSES_ROOT, AutoHotkeyScript\Shell\Edit\Command,, %UeditPath%\uedit32.exe "`%1"

ExitApp  ; That's it, exit

;#############   SubRoutines   ################################################

ReadSyntaxFromFile:
  TempString =
  Loop, Read , %PathSyntaxFiles%\%SyntaxFileName%%SyntaxExtention%   ;read syntax file
    {
      StringLeft,Char, A_LoopReadLine ,1
      ;if line is comment, don't bother, otherwise add keyword to string
      If Char <> `;
        {
          ;only add first word in line
          Loop Parse, A_LoopReadLine, `,%A_Tab%%A_Space%(
            {
              TempString = %TempString%%A_LoopField%`n
              Break
            }
        }
    }
  %SyntaxFileName% = %TempString%                          ;Assign string to syntax filename
  Sort, %SyntaxFileName%, U                                ;Sort keywords in string
Return

ParseSyntaxString:
  Loop, Parse, %SyntaxString%, `n                 ;Parse through syntax string
    {
      StringLeft, Char, A_LoopField,1
      If (Char = PrevChar)                       ;add keyword to line when first character is same with previous keyword
          StrgAHKwf = %StrgAHKwf% %A_LoopField%
      Else                                       ;for every keyword with a new first letter, start a new row
          StrgAHKwf = %StrgAHKwf%`n%A_LoopField%
      PrevChar = %Char%                          ;remember first character of keyword
    }
Return

;#############   END of File   ################################################
