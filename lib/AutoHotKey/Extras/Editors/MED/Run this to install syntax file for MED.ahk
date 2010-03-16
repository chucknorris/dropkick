; v0.4 by Wolfgang Reszel (Tekl)
; works with MED v3.x and MED CX v4.x

; set language-specific strings
StringRight, Lng, A_Language, 2
if Lng = 07  ; = Deutsch (0407, 0807, 0c07 ...)
{
   lng_close         = Bitte MED vorher schließen!
   lng_installed     = Die AutoHotkey-Syntax und die TextLib wurden für MED installiert.
   lng_noMed         = MED ist nicht richtig installiert worden.`nGgf. müssen Sie MED einmal starteten und wieder beenden.
   lng_Processing    = Dateien werden erstellt ...
   lng_RegistryFailed= MED wurde nicht als Standard-Editor für AHK-Dateien eingetragen,`nda dieses Skript keine Admin-Rechte hat.
}
else        ; = other languages (english)
{
   lng_close         = Please close MED!
   lng_installed     = The AutoHotkey-Syntax and the TextLib have been installed to MED!
   lng_noMed         = MED is not properly installed.`nMaybe you should start and quit MED once.
   lng_Processing    = Creating files ...
   lng_RegistryFailed= The standard editor for AHK files is not set to MED`nbecause this script does not have admin rights.
}

If A_IsAdmin = 0
{
   DllCall("shell32\ShellExecuteA", uint, 0, str, "RunAs", str, A_AhkPath,str, "/r """ . A_ScriptFullPath . """", str, A_WorkingDir, int, 1)  ; Last parameter: SW_SHOWNORMAL = 1
   ExitApp
}

; Get the real OS Version
If A_IsCompiled = 1
   RegRead, A_RealOSVersion, HKEY_CURRENT_USER, Software\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers, %A_ScriptFullPath%
Else
   RegRead, A_RealOSVersion, HKEY_CURRENT_USER, Software\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers, %A_AhkPath%
If A_RealOSVersion <>
   A_RealOSVersion = WIN_VISTA
Else
   A_RealOSVersion = %A_OSVersion%

; Get path where MED is installed
MedVersion = 3
RegRead, medpathroot, HKEY_LOCAL_MACHINE, SOFTWARE\Utopia Planitia\MEDCX,Home
IfExist, %medpathroot%
   MedVersion = 4
Else
   RegRead, medpathroot, HKEY_LOCAL_MACHINE, SOFTWARE\Utopia Planitia\MED,Home

; Set users syntax-folder
If MedVersion = 4
   If A_RealOSVersion = WIN_VISTA
   {
      EnvGet, LocalAppData, LocalAppData
      medpath= %LocalAppData%\UtopiaPlanitia\MedCX
   }
   Else
      medpath= %medpathroot%\medcx\profiles\medcx\%Username%
Else
   medpath= %medpathroot%\med\profiles\%Username%

; Get path of AHKs help-file
ahkpath = %A_WorkingDir%
StringGetPos, pos, ahkpath, \, R3
StringLeft, ahkpath,ahkpath, %pos%

; MED does not like whitespace
StringReplace, ahkpath,ahkpath, %A_Space%,|

; Checks if the syntax-folder exist
IfExist, %medpath%
{
   ; Checks if MED is running
   IfWinExist, ahk_class MRED_MAINWIN_CLASS
   {
      MsgBox, 48,, %lng_close%
   }
   IfWinExist, ahk_class MRED_UNICODE_MAINWIN_CLASS
   {
      MsgBox, 48,, %lng_close%
   }
   ; Checks again if MED is running
   IfWinExist, ahk_class MRED_MAINWIN_CLASS
   {
      MsgBox, 48,, %lng_close%
      ExitApp
   }
   IfWinExist, ahk_class MRED_UNICODE_MAINWIN_CLASS
   {
      MsgBox, 48,, %lng_close%
      ExitApp
   }
   SplashTextOn,,,%lng_Processing%

   ; Read the header for the syntax-file
   FileRead, SyntaxFile, MEDSynHeader.txt

   ; Set the path to the Helpfile
   StringReplace, SyntaxFile, SyntaxFile, C:\Program|Files\AutoHotkey\, %ahkpath%\

   ; Commands
   SyntaxFile = %SyntaxFile%`ncolor: darkblue, normal, black, bold
   Loop, 2
   {
      CommandCount = 0
      If A_Index = 1
         File = CommandNames.txt
      If A_Index = 2
         File = Functions.txt

      Loop, Read, ..\Syntax\%File%
      {
         IfInString, A_LoopReadLine, `;
            continue
         If A_LoopReadLine =
            continue

         If CommandCount = 0
            SyntaxFile = %SyntaxFile%`ntoken:

         StringReplace, ReadLine, A_LoopReadLine, %A_Space%, _, A

         IfInString, ReadLine, If_Else
            ReadLine = If
         IfInString, ReadLine, If_Var
            continue
         If ReadLine = Loop`,_
            continue

         ReadLine := RegExReplace( ReadLine, "i)^([a-z_]+\().*\)$", "$1" )

         SyntaxFile := SyntaxFile " " ReadLine

         CommandCount++
         If CommandCount > 4
            CommandCount = 0
      }
   }

   ; Variables
   SyntaxFile = %SyntaxFile%`n`ncolor: red, normal, black, italic
   CommandCount = 0
   Loop, Read, ..\Syntax\Variables.txt
   {
      IfInString, A_LoopReadLine, `;
         continue
      If A_LoopReadLine =
         continue

      If CommandCount = 0
         SyntaxFile = %SyntaxFile%`ntoken:

      StringReplace, ReadLine, A_LoopReadLine, %A_Space%, _, A
      SyntaxFile := SyntaxFile " " ReadLine
      ;SyntaxFile := SyntaxFile " %" ReadLine "%"

      CommandCount++
      If CommandCount > 4
         CommandCount = 0
   }

   ; Keywords
   SyntaxFile = %SyntaxFile%`n`ncolor: darkpink, normal, black, normal
   CommandCount = 0
   Loop, Read, ..\Syntax\Keywords.txt
   {
      IfInString, A_LoopReadLine, `;
         continue
      If A_LoopReadLine =
         continue

      If CommandCount = 0
         SyntaxFile = %SyntaxFile%`ntoken:

      StringReplace, ReadLine, A_LoopReadLine, %A_Space%, _, A
      SyntaxFile := SyntaxFile " " ReadLine

      CommandCount++
      If CommandCount > 4
         CommandCount = 0
   }

   ; Keys
   SyntaxFile = %SyntaxFile%`n`ncolor: darkgreen, normal, black, underline
   CommandCount = 0
   Loop, Read, ..\Syntax\Keys.txt
   {
      IfInString, A_LoopReadLine, `;
         continue
      If A_LoopReadLine =
         continue

      If CommandCount = 0
         SyntaxFile = %SyntaxFile%`ntoken:

      StringReplace, ReadLine, A_LoopReadLine, %A_Space%, _, A
      SyntaxFile := SyntaxFile " " ReadLine
      ;SyntaxFile := SyntaxFile " {" ReadLine "}"

      CommandCount++
      If CommandCount > 4
         CommandCount = 0
   }

   ; Last line
   SyntaxFile = %SyntaxFile%`n`n#===========================================================

   If MedVersion = 4
   {
      ; Delete old syntax-file
      FileDelete, %medpath%\med-syn\AutoHotkey.syn
      FileDelete, %medpathroot%\medcx\profile_skeleton\med-syn\AutoHotkey.syn
      ; Write new syntax-file
      Ansi2Unicode(SyntaxFile,SyntaxFileU)
      Bin2Hex(SyntaxFileH, SyntaxFileU)
      BinWrite( medpath "\med-syn\AutoHotkey.syn", "FFFE" SyntaxFileH )
      BinWrite( medpathroot "\medcx\profile_skeleton\med-syn\AutoHotkey.syn", "FFFE"  SyntaxFileH )
   }
   Else
   {
      ; Delete old syntax-file
      FileDelete, %medpath%\med-syn\AutoHotkey.syn
      FileDelete, %medpathroot%\med\profile_skeleton\med-syn\AutoHotkey.syn
      ; Write new syntax-file
      FileAppend, %SyntaxFile%, %medpath%\med-syn\AutoHotkey.syn
      FileAppend, %SyntaxFile%, %medpathroot%\med\profile_skeleton\med-syn\AutoHotkey.syn
   }

   ; Read the header for the TextLib-file
   FileRead, LibFile, MEDMclHeader.txt

   Loop, 2
   {
      If A_Index = 1
         File = Commands.txt
      If A_Index = 2
         File = Functions.txt
      Loop, Read, ..\Syntax\%File%
      {
         StringReplace, ReadLine, A_LoopReadLine, ``n, `n , A
         StringReplace, ReadLine, ReadLine, ``t, % "   ", A
         StringGetPos, Pos, ReadLine, %A_Space%
         StringLeft, CommandOnly, ReadLine, %Pos%

         IfInString, ReadLine, If Var = Value
            CommandOnly = If =
         IfInString, ReadLine, If Var [not] between
            CommandOnly = If between
         IfInString, ReadLine, If Var [not] contains
            CommandOnly = If contains
         IfInString, ReadLine, If Var [not] in
            CommandOnly = If in
         IfInString, ReadLine, If Var is [not] type
            CommandOnly = If is type
         IfInString, ReadLine, Loop`, FilePattern
            CommandOnly = Loop FilePattern
         IfInString, ReadLine, Loop`, Parse
            CommandOnly = Loop Parse
         IfInString, ReadLine, Loop`, Read
            CommandOnly = Loop Read
         IfInString, ReadLine, Loop`, Reg
            CommandOnly = Loop Reg

         StringReplace, CommandOnly, CommandOnly, `,,
         If CommandOnly =
            continue

         CommandOnly := RegExReplace( CommandOnly, "i)([a-z_]+\().*$", "$1" )

         LibFile := LibFile "`n!TEXT=" CommandOnly "`n" ReadLine "`n!"
      }
   }

   If MedVersion = 4
   {
      ; Delete old TextLib-file
      FileDelete, %medpath%\med-mcl\AutoHotkey.mcl
      FileDelete, %medpathroot%\medcx\profile_skeleton\med-mcl\AutoHotkey.mcl
      ; Write new TextLib-file
      Ansi2Unicode(LibFile,LibFileU)
      Bin2Hex(LibFileH, LibFileU)
      BinWrite( medpath "\med-mcl\AutoHotkey.mcl", "FFFE" LibFileH )
      BinWrite( medpathroot "\medcx\profile_skeleton\med-mcl\AutoHotkey.mcl", "FFFE" LibFileH )
      ;FileAppend, %LibFile%, %medpath%\med-mcl\AutoHotkey.mcl
      ;FileAppend, %LibFile%, %medpathroot%\medcx\profile_skeleton\med-mcl\AutoHotkey.mcl
   }
   Else
   {
      ; Delete old TextLib-file
      FileDelete, %medpath%\med-mcl\AutoHotkey.mcl
      FileDelete, %medpathroot%\med\profile_skeleton\med-mcl\AutoHotkey.mcl
      ; Write new TextLib-file
      FileAppend, %LibFile%, %medpath%\med-mcl\AutoHotkey.mcl
      FileAppend, %LibFile%, %medpathroot%\med\profile_skeleton\med-mcl\AutoHotkey.mcl
   }
   SplashTextOff

   ; Set 'Edit Script' to MED
   If MedVersion = 4
      RegWrite, REG_SZ,HKEY_CLASSES_ROOT,AutoHotkeyScript\Shell\Edit\Command,, "%medpathroot%\medcx.exe" "`%1"
   Else
      RegWrite, REG_SZ,HKEY_CLASSES_ROOT,AutoHotkeyScript\Shell\Edit\Command,, "%medpathroot%\med.exe" "`%1"
   If ErrorLevel
      MsgBox, 64,, %lng_installed% (MED v%MedVersion%)`n`n%lng_RegistryFailed%
   Else
      MsgBox, 64,, %lng_installed% (MED v%MedVersion%)
}
else
{
   MsgBox, 16,, %lng_noMed%
}

; == FUNCTIONS ============================================================

/*
CP_ACP   = 0
CP_OEMCP = 1
CP_MACCP = 2
CP_UTF7  = 65000
CP_UTF8  = 65001
*/

Ansi2Oem(sString) {
   Ansi2Unicode(sString, wString, 0)
   Unicode2Ansi(wString, zString, 1)
   Return zString
}

Oem2Ansi(zString) {
   Ansi2Unicode(zString, wString, 1)
   Unicode2Ansi(wString, sString, 0)
   Return sString
}

Ansi2UTF8(sString) {
   Ansi2Unicode(sString, wString, 0)
   Unicode2Ansi(wString, zString, 65001)
   Return zString
}

UTF82Ansi(zString) {
   Ansi2Unicode(zString, wString, 65001)
   Unicode2Ansi(wString, sString, 0)
   Return sString
}

Ansi2Unicode(ByRef sString, ByRef wString, CP = 0) {
     nSize := DllCall("MultiByteToWideChar"
      , "Uint", CP
      , "Uint", 0
      , "Uint", &sString
      , "int",  -1
      , "Uint", 0
      , "int",  0)

   VarSetCapacity(wString, nSize * 2)

   DllCall("MultiByteToWideChar"
      , "Uint", CP
      , "Uint", 0
      , "Uint", &sString
      , "int",  -1
      , "Uint", &wString
      , "int",  nSize)
}

Unicode2Ansi(ByRef wString, ByRef sString, CP = 0) {
     nSize := DllCall("WideCharToMultiByte"
      , "Uint", CP
      , "Uint", 0
      , "Uint", &wString
      , "int",  -1
      , "Uint", 0
      , "int",  0
      , "Uint", 0
      , "Uint", 0)

   VarSetCapacity(sString, nSize)

   DllCall("WideCharToMultiByte"
      , "Uint", CP
      , "Uint", 0
      , "Uint", &wString
      , "int",  -1
      , "str",  sString
      , "int",  nSize
      , "Uint", 0
      , "Uint", 0)
}

BinWrite(file, data, n=0, offset=0) {
   ; Open file for WRITE (0x40..), OPEN_ALWAYS (4): creates only if it does not exists
   h := DllCall("CreateFile","str",file,"Uint",0x40000000,"Uint",0,"UInt",0,"UInt",4,"Uint",0,"UInt",0)
   IfEqual h,-1, SetEnv, ErrorLevel, -1
   IfNotEqual ErrorLevel,0,Return,0 ; couldn't create the file

   m = 0                            ; seek to offset
   IfLess offset,0, SetEnv,m,2
   r := DllCall("SetFilePointerEx","Uint",h,"Int64",offset,"UInt *",p,"Int",m)
   IfEqual r,0, SetEnv, ErrorLevel, -3
   IfNotEqual ErrorLevel,0, {
      t = %ErrorLevel%              ; save ErrorLevel to be returned
      DllCall("CloseHandle", "Uint", h)
      ErrorLevel = %t%              ; return seek error
      Return 0
   }

   TotalWritten = 0
   m := Ceil(StrLen(data)/2)
   If (n <= 0 or n > m)
       n := m
   Loop %n%
   {
      StringLeft c, data, 2         ; extract next byte
      StringTrimLeft data, data, 2  ; remove  used byte
      c = 0x%c%                     ; make it number
      result := DllCall("WriteFile","UInt",h,"UChar *",c,"UInt",1,"UInt *",Written,"UInt",0)
      TotalWritten += Written       ; count written
      if (!result or Written < 1 or ErrorLevel)
         break
   }

   IfNotEqual ErrorLevel,0, SetEnv,t,%ErrorLevel%

   h := DllCall("CloseHandle", "Uint", h)
   IfEqual h,-1, SetEnv, ErrorLevel, -2
   IfNotEqual t,,SetEnv, ErrorLevel, %t%

   Return TotalWritten
}

Bin2Hex(ByRef h, ByRef b, n=0) {    ; n bytes binary data -> stream of 2-digit hex
                                    ; n = 0: all (SetCapacity can be larger than used!)
   format = %A_FormatInteger%       ; save original integer format
   SetFormat Integer, Hex           ; for converting bytes to hex

   m := VarSetCapacity(b)
   If (n < 1 or n > m)
       n := m
   Address := &b
   h =
   Loop %n%
   {
      x := *Address                 ; get byte in hex
      StringTrimLeft x, x, 2        ; remove 0x
      x = 0%x%                      ; pad left
      StringRight x, x, 2           ; 2 hex digits
      h = %h%%x%
      Address++
   }
   SetFormat Integer, %format%      ; restore original format
}

