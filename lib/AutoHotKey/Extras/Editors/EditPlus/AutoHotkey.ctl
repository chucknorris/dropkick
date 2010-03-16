#TITLE=AutoHotkey
#INFO
Cliptext Library for AutoHotkey
#SORT=y

#T=#AllowSameLineComments

#T=#ClipboardTimeout
#ClipboardTimeout milliseconds

#T=#CommentFlag
#CommentFlag NewString

#T=#ErrorStdOut

#T=#EscapeChar
#EscapeChar NewChar

#T=#HotkeyInterval
#HotkeyInterval Value

#T=#HotkeyModifierTimeout
#HotkeyModifierTimeout milliseconds

#T=#Hotstring
#Hotstring NewOptions

#T=#IfWinActive
#IfWinActive [,WinTitle,WinText]

#T=#IfWinExist
#IfWinExist [,WinTitle,WinText]

#T=#IfWinNotActive
#IfWinNotActive [,WinTitle,WinText]

#T=#IfWinNotExist
#IfWinNotExist [,WinTitle,WinText]

#T=#Include
#Include FileName

#T=#IncludeAgain
#IncludeAgain FileName

#T=#InstallKeybdHook

#T=#InstallMouseHook

#T=#KeyHistory
#KeyHistory MaxEvents

#T=#LTrim
#LTrim On|Off

#T=#MaxHotkeysPerInterval
#MaxHotkeysPerInterval Value

#T=#MaxMem
#MaxMem ValueInMegabytes

#T=#MaxThreads
#MaxThreads Value

#T=#MaxThreadsBuffer
#MaxThreadsBuffer On|Off

#T=#MaxThreadsPerHotkey
#MaxThreadsPerHotkey Value

#T=#NoEnv

#T=#NoTrayIcon

#T=#Persistent

#T=#SingleInstance
#SingleInstance [force|ignore|off]

#T=#UseHook
#UseHook [On|Off]

#T=#WinActivateForce

#T=AutoTrim
AutoTrim,On|Off

#T=BlockInput
BlockInput,On|Off|Send|Mouse|SendAndMouse|Default|MouseMove|MouseMoveOff

#T=Break

#T=Click

#T=ClipWait
ClipWait [,SecondsToWait,1]

#T=Continue

#T=Control
Control,Cmd [,Value,Control,WinTitle,WinText,ExcludeTitle,ExcludeText]

#T=ControlClick
ControlClick [,Control-or-Pos,WinTitle,WinText,WhichButton,ClickCount,Options,ExcludeTitle,ExcludeText]

#T=ControlFocus
ControlFocus [,Control,WinTitle,WinText,ExcludeTitle,ExcludeText]

#T=ControlGet
ControlGet,OutputVar,Cmd [,Value,Control,WinTitle,WinText,ExcludeTitle,ExcludeText]

#T=ControlGetFocus
ControlGetFocus,OutputVar [WinTitle,WinText,ExcludeTitle,ExcludeText]

#T=ControlGetPos
ControlGetPos [,X,Y,Width,Height,Control,WinTitle,WinText,ExcludeTitle,ExcludeText]

#T=ControlGetText
ControlGetText,OutputVar [,Control,WinTitle,WinText,ExcludeTitle,ExcludeText]

#T=ControlMove
ControlMove,Control,X,Y,Width,Height [,WinTitle,WinText,ExcludeTitle,ExcludeText]

#T=ControlSend
ControlSend [,Control,Keys,WinTitle,WinText,ExcludeTitle,ExcludeText]

#T=ControlSendRaw
ControlSendRaw [,Control,Keys,WinTitle,WinText,ExcludeTitle,ExcludeText]

#T=ControlSetText
ControlSetText,Control,NewText [,WinTitle,WinText,ExcludeTitle,ExcludeText]

#T=CoordMode
CoordMode,ToolTip|Pixel|Mouse [,Screen|Relative]

#T=Critical
Critical [,Off]

#T=DetectHiddenText
DetectHiddenText,On|Off

#T=DetectHiddenWindows
DetectHiddenWindows,On|Off

#T=Drive
Drive,Sub-command [,Drive ,Value]

#T=DriveGet
DriveGet,OutputVar,Cmd [,Value]

#T=DriveSpaceFree
DriveSpaceFree,OutputVar,C:\

#T=Edit

#T=Else

#T=EnvAdd
EnvAdd,Var,Value [,TimeUnits]

#T=EnvDiv
EnvDiv,Var,Value

#T=EnvGet
EnvGet,OutputVar,EnvVarName

#T=EnvMult
EnvMult,Var,Value

#T=EnvSet
EnvSet,EnvVar,Value

#T=EnvSub
EnvSub,Var,Value [,TimeUnits]

#T=EnvUpdate

#T=Exit
Exit [,ExitCode]

#T=ExitApp
ExitApp [,ExitCode]

#T=FileAppend
FileAppend [,Text,Filename]

#T=FileCopy
FileCopy,Source,Dest [,Flag (1 = overwrite)]

#T=FileCopyDir
FileCopyDir,Source,Dest [,Flag]

#T=FileCreateDir
FileCreateDir,Path

#T=FileCreateShortcut
FileCreateShortcut,Target,C:\My Shortcut.lnk [,WorkingDir,Args,Description,IconFile,ShortcutKey,IconNumber,RunState]

#T=FileDelete
FileDelete,FilePattern

#T=FileGetAttrib
FileGetAttrib,OutputVar(RASHNDOCT) [,Filename]

#T=FileGetShortcut
FileGetShortcut,LinkFile [,OutTarget,OutDir,OutArgs,OutDescription,OutIcon,OutIconNum,OutRunState]

#T=FileGetSize
FileGetSize,OutputVar [,Filename,Units]

#T=FileGetTime
FileGetTime,OutputVar [,Filename,WhichTime (M,C,or A -- default is M)]

#T=FileGetVersion
FileGetVersion,OutputVar [,Filename]

#T=FileInstall
FileInstall,Source,Dest [,Flag (1 = overwrite)]

#T=FileMove
FileMove,Source,Dest [,Flag (1 = overwrite)]

#T=FileMoveDir
FileMoveDir,Source,Dest [,Flag (2 = overwrite)]

#T=FileRead
FileRead,OutputVar,Filename

#T=FileReadLine
FileReadLine,OutputVar,Filename,LineNum

#T=FileRecycle
FileRecycle,FilePattern

#T=FileRecycleEmpty
FileRecycleEmpty [,C:\]

#T=FileRemoveDir
FileRemoveDir,Path [,Recurse? (1 = yes)]

#T=FileSelectFile
FileSelectFile,OutputVar [,Options,RootDir[\DefaultFilename],Prompt,Filter]

#T=FileSelectFolder
FileSelectFolder,OutputVar [,*StartingFolder,Options,Prompt]

#T=FileSetAttrib
FileSetAttrib,Attributes(+-^RASHNOT) [,FilePattern,OperateOnFolders?,Recurse?]

#T=FileSetTime
FileSetTime [,YYYYMMDDHH24MISS,FilePattern,WhichTime (M|C|A),OperateOnFolders?,Recurse?]

#T=FormatTime
FormatTime,OutputVar [,YYYYMMDDHH24MISS,Format]

#T=GetKeyState
GetKeyState,OutputVar,WhichKey [,Mode (P|T)]

#T=Gosub
Gosub,Label

#T=Goto
Goto,Label

#T=GroupActivate
GroupActivate,GroupName [,R]

#T=GroupAdd
GroupAdd,GroupName,WinTitle [,WinText,Label,ExcludeTitle,ExcludeText]

#T=GroupClose
GroupClose,GroupName [,A|R]

#T=GroupDeactivate
GroupDeactivate,GroupName [,R]

#T=Gui
Gui,sub-command [,Param2,Param3,Param4]

#T=GuiControl
GuiControl,Sub-command,ControlID [,Param3]

#T=GuiControlGet
GuiControlGet,OutputVar [,Sub-command,ControlID,Param4]

#T=Hotkey
Hotkey,KeyName [,Label,Options]

#T=If Else
If (var = "Value") | IfWinExist,WinTitle | etc.
{
  command1
  command2
}
Else
{
  command1
  command2
}

#T=If Var [not] between Low and High

#T=If Var [not] contains value1,value2,...

#T=If Var [not] in value1,value2,...

#T=If Var is [not] integer|float|number|digit|xdigit|alpha|upper|lower|alnum|space|time

#T=IfEqual
IfEqual,var,value

#T=IfExist
IfExist,File|Dir|Pattern

#T=IfGreater
IfGreater,var,value

#T=IfGreaterOrEqual
IfGreaterOrEqual,var,value

#T=IfInString
IfInString,Var,SearchString

#T=IfLess
IfLess,var,value

#T=IfLessOrEqual
IfLessOrEqual,var,value

#T=IfMsgBox
IfMsgBox,Yes|No|OK|Cancel|Abort|Ignore|Retry|Timeout

#T=IfNotEqual
IfNotEqual,var,value

#T=IfNotExist
IfNotExist,File|Dir|Pattern

#T=IfNotInString
IfNotInString,Var,SearchString

#T=IfWinActive
IfWinActive [,WinTitle,WinText,ExcludeTitle,ExcludeText]

#T=IfWinExist
IfWinExist [,WinTitle,WinText,ExcludeTitle,ExcludeText]

#T=IfWinNotActive
IfWinNotActive [,WinTitle,WinText,ExcludeTitle,ExcludeText]

#T=IfWinNotExist
IfWinNotExist [,WinTitle,WinText,ExcludeTitle,ExcludeText]

#T=ImageSearch
ImageSearch,OutputVarX,OutputVarY,X1,Y1,X2,Y2,ImageFile

#T=IniDelete
IniDelete,Filename,Section [,Key]

#T=IniRead
IniRead,OutputVar,Filename,Section,Key [,Default]

#T=IniWrite
IniWrite,Value,Filename,Section,Key

#T=Input
Input [,OutputVar,Options,EndKeys,MatchList]

#T=InputBox
InputBox,OutputVar [,Title,Prompt,HIDE,Width,Height,X,Y,Font,Timeout,Default]

#T=KeyHistory

#T=KeyWait
KeyWait,KeyName [,Options]

#T=ListHotkeys

#T=ListLines

#T=ListVars

#T=Loop
Loop [,Count]
{
  ID := A_Index
  If var%A_Index% =
  break
}

#T=Loop,FilePattern
Loop,FilePattern [,IncludeFolders?,Recurse?]
{
  FileName := A_LoopFileName
  FileFullPath := A_LoopFileLongPath
  FileRelativeDir := A_LoopFileDir
  command2
}

#T=Loop,Parse
Loop,Parse,InputVar [,Delimiters|CSV,OmitChars]
{
  Line := A_LoopField
  command2
}

#T=Loop,Read
Loop,Read,InputFile [,OutputFile]
{
  Line := A_LoopReadLine
  command2
}

#T=Loop,Reg
Loop,HKLM|HKU|HKCU|HKCR|HKCC [,Key,IncludeSubkeys?,Recurse?]
{
  RegName := A_LoopRegName
  RegType := A_LoopRegType
  command2
}

#T=Menu
Menu,MenuName,Cmd [,P3,P4,P5]

#T=MouseClick
MouseClick,WhichButton [,X,Y,ClickCount,Speed,D|U,R]

#T=MouseClickDrag
MouseClickDrag,WhichButton,X1,Y1,X2,Y2 [,Speed,R]

#T=MouseGetPos
MouseGetPos [,OutputVarX,OutputVarY,OutputVarWin,OutputVarControl,1|2|3]

#T=MouseMove
MouseMove,X,Y [,Speed,R]

#T=MsgBox
MsgBox [,Options,Title,Text,Timeout]

#T=OnExit
OnExit [,Label]

#T=OutputDebug
OutputDebug,Text

#T=Pause
Pause [,On|Off|Toggle,OperateOnUnderlyingThread?]

#T=PixelGetColor
PixelGetColor,OutputVar,X,Y [,Alt|Slow|RGB]

#T=PixelSearch
PixelSearch,OutputVarX,OutputVarY,X1,Y1,X2,Y2,ColorID [,Variation,Fast|RGB]

#T=PostMessage
PostMessage,Msg [,wParam,lParam,Control,WinTitle,WinText,ExcludeTitle,ExcludeText]

#T=Process
Process,Cmd,PID-or-Name [,Param3]

#T=Progress
Progress,Param1 [,SubText,MainText,WinTitle,FontName]

#T=Random
Random,OutputVar [,Min,Max]

#T=RegDelete
RegDelete,HKLM|HKU|HKCU|HKCR|HKCC,SubKey [,ValueName]

#T=RegRead
RegRead,OutputVar,HKLM|HKU|HKCU|HKCR|HKCC,SubKey [,ValueName]

#T=RegWrite
RegWrite,REG_SZ|REG_EXPAND_SZ|REG_MULTI_SZ|REG_DWORD|REG_BINARY,HKLM|HKU|HKCU|HKCR|HKCC,SubKey [,ValueName,Value]

#T=Reload

#T=Repeat

#T=Return
Return [,Expression]

#T=Run
Run,Target [,WorkingDir,Max|Min|Hide|UseErrorLevel,OutputVarPID]

#T=RunAs
RunAs [,User,Password,Domain] 

#T=RunWait
RunWait,Target [,WorkingDir,Max|Min|Hide|UseErrorLevel,OutputVarPID]

#T=Send
Send,Keys

#T=SendEvent
SendEvent,Keys

#T=SendInput
SendInput,Keys

#T=SendMessage
SendMessage,Msg [,wParam,lParam,Control,WinTitle,WinText,ExcludeTitle,ExcludeText]

#T=SendMode
SendMode,Event|Play|Input|InputThenPlay

#T=SendPlay
SendPlay,Keys

#T=SendRaw
SendRaw,Keys

#T=SetBatchLines
SetBatchLines,-1 | 20ms | LineCount

#T=SetCapslockState
SetCapsLockState,On|Off|AlwaysOn|AlwaysOff

#T=SetControlDelay
SetControlDelay,Delay

#T=SetDefaultMouseSpeed
SetDefaultMouseSpeed,Speed

#T=SetEnv
SetEnv,Var,Value

#T=SetFormat
SetFormat,float|integer,TotalWidth.DecimalPlaces|hex|d

#T=SetKeyDelay
SetKeyDelay [,Delay,PressDuration]

#T=SetMouseDelay
SetMouseDelay,Delay

#T=SetNumlockState
SetNumLockState,On|Off|AlwaysOn|AlwaysOff

#T=SetScrollLockState
SetScrollLockState,On|Off|AlwaysOn|AlwaysOff

#T=SetStoreCapslockMode
SetStoreCapslockMode,On|Off

#T=SetTimer
SetTimer,Label [,Period|On|Off]

#T=SetTitleMatchMode
SetTitleMatchMode,Fast|Slow|RegEx|1|2|3

#T=SetWinDelay
SetWinDelay,Delay

#T=SetWorkingDir
SetWorkingDir,DirName

#T=Shutdown
Shutdown,Code

#T=Sleep
Sleep,Delay

#T=Sort
Sort,VarName [,Options]

#T=SoundBeep
SoundBeep [,Frequency,Duration]

#T=SoundGet
SoundGet,OutputVar [,ComponentType,ControlType,DeviceNumber]

#T=SoundGetWaveVolume
SoundGetWaveVolume,OutputVar [,DeviceNumber]

#T=SoundPlay
SoundPlay,Filename [,wait]

#T=SoundSet
SoundSet,NewSetting [,ComponentType,ControlType,DeviceNumber]

#T=SoundSetWaveVolume
SoundSetWaveVolume,Percent [,DeviceNumber]

#T=SplashImage
SplashImage [,ImageFile,Options,SubText,MainText,WinTitle,FontName]

#T=SplashTextOff

#T=SplashTextOn
SplashTextOn [,Width,Height,Title,Text]

#T=SplitPath
SplitPath,InputVar [,OutFileName,OutDir,OutExtension,OutNameNoExt,OutDrive]

#T=StatusBarGetText
StatusBarGetText,OutputVar [,Part#,WinTitle,WinText,ExcludeTitle,ExcludeText]

#T=StatusBarWait
StatusBarWait [,BarText,Seconds,Part#,WinTitle,WinText,Interval,ExcludeTitle,ExcludeText]

#T=StringCaseSense
StringCaseSense,On|Off|Locale

#T=StringGetPos
StringGetPos,OutputVar,InputVar,SearchText [,L#|R#,Offset]

#T=StringLeft
StringLeft,OutputVar,InputVar,Count

#T=StringLen
StringLen,OutputVar,InputVar

#T=StringLower
StringLower,OutputVar,InputVar [,T]

#T=StringMid
StringMid,OutputVar,InputVar,StartChar [,Count,L]

#T=StringReplace
StringReplace,OutputVar,InputVar,SearchText [,ReplaceText,All]

#T=StringRight
StringRight,OutputVar,InputVar,Count

#T=StringSplit
StringSplit,OutputArray,InputVar [,Delimiters,OmitChars]

#T=StringTrimLeft
StringTrimLeft,OutputVar,InputVar,Count

#T=StringTrimRight
StringTrimRight,OutputVar,InputVar,Count

#T=StringUpper
StringUpper,OutputVar,InputVar [,T]

#T=Suspend
Suspend [,On|Off|Toggle|Permit]

#T=SysGet
SysGet,OutputVar,Sub-command [,Param3]

#T=Thread
Thread,Setting,P2 [,P3]

#T=ToolTip
ToolTip [,Text,X,Y,WhichToolTip]

#T=Transform
Transform,OutputVar,Cmd,Value1 [,Value2]

#T=TrayTip
TrayTip [,Title,Text,Seconds,Options]

#T=URLDownloadToFile
URLDownloadToFile,URL,Filename

#T=While
While,Expression

#T=WinActivate
WinActivate [,WinTitle,WinText,ExcludeTitle,ExcludeText]

#T=WinActivateBottom
WinActivateBottom [,WinTitle,WinText,ExcludeTitle,ExcludeText]

#T=WinClose
WinClose [,WinTitle,WinText,SecondsToWait,ExcludeTitle,ExcludeText]

#T=WinGet
WinGet,OutputVar [,Cmd,WinTitle,WinText,ExcludeTitle,ExcludeText]

#T=WinGetActiveStats
WinGetActiveStats,Title,Width,Height,X,Y

#T=WinGetActiveTitle
WinGetActiveTitle,OutputVar

#T=WinGetClass
WinGetClass,OutputVar [,WinTitle,WinText,ExcludeTitle,ExcludeText]

#T=WinGetPos
WinGetPos [,X,Y,Width,Height,WinTitle,WinText,ExcludeTitle,ExcludeText]

#T=WinGetText
WinGetText,OutputVar [,WinTitle,WinText,ExcludeTitle,ExcludeText]

#T=WinGetTitle
WinGetTitle,OutputVar [,WinTitle,WinText,ExcludeTitle,ExcludeText]

#T=WinHide
WinHide [,WinTitle,WinText,ExcludeTitle,ExcludeText]

#T=WinKill
WinKill [,WinTitle,WinText,SecondsToWait,ExcludeTitle,ExcludeText]

#T=WinMaximize
WinMaximize [,WinTitle,WinText,ExcludeTitle,ExcludeText]

#T=WinMenuSelectItem
WinMenuSelectItem,WinTitle,WinText,Menu [,SubMenu1,SubMenu2,SubMenu3,SubMenu4,SubMenu5,SubMenu6,ExcludeTitle,ExcludeText]

#T=WinMinimize
WinMinimize [,WinTitle,WinText,ExcludeTitle,ExcludeText]

#T=WinMinimizeAll

#T=WinMinimizeAllUndo

#T=WinMove
WinMove,WinTitle,WinText,X,Y [,Width,Height,ExcludeTitle,ExcludeText]

#T=WinRestore
WinRestore [,WinTitle,WinText,ExcludeTitle,ExcludeText]

#T=WinSet
WinSet,AlwaysOnTop|Trans,On|Off|Toggle|Value(0-255) [,WinTitle,WinText,ExcludeTitle,ExcludeText]

#T=WinSetTitle
WinSetTitle,WinTitle,WinText,NewTitle [,ExcludeTitle,ExcludeText]

#T=WinShow
WinShow [,WinTitle,WinText,ExcludeTitle,ExcludeText]

#T=WinWait
WinWait,WinTitle,WinText,Seconds [,ExcludeTitle,ExcludeText]

#T=WinWaitActive
WinWaitActive [,WinTitle,WinText,Seconds,ExcludeTitle,ExcludeText]

#T=WinWaitClose
WinWaitClose,WinTitle,WinText,Seconds [,ExcludeTitle,ExcludeText]

#T=WinWaitNotActive
WinWaitNotActive [,WinTitle,WinText,Seconds,ExcludeTitle,ExcludeText]

#T=A_AhkPath

#T=A_AhkVersion

#T=A_AppData

#T=A_AppDataCommon

#T=A_AutoTrim

#T=A_BatchLines

#T=A_CaretX

#T=A_CaretY

#T=A_ComputerName

#T=A_ControlDelay

#T=A_Cursor

#T=A_DD

#T=A_DDD

#T=A_DDDD

#T=A_DefaultMouseSpeed

#T=A_Desktop

#T=A_DesktopCommon

#T=A_DetectHiddenText

#T=A_DetectHiddenWindows

#T=A_EndChar

#T=A_EventInfo

#T=A_ExitReason

#T=A_FormatFloat

#T=A_FormatInteger

#T=A_Gui

#T=A_GuiEvent

#T=A_GuiControl

#T=A_GuiControlEvent

#T=A_GuiHeight

#T=A_GuiWidth

#T=A_GuiX

#T=A_GuiY

#T=A_Hour

#T=A_IconFile

#T=A_IconHidden

#T=A_IconNumber

#T=A_IconTip

#T=A_Index

#T=A_IPAddress1

#T=A_IPAddress2

#T=A_IPAddress3

#T=A_IPAddress4

#T=A_ISAdmin

#T=A_IsCompiled

#T=A_IsCritical

#T=A_IsPaused

#T=A_IsSuspended

#T=A_KeyDelay

#T=A_Language

#T=A_LastError

#T=A_LineFile

#T=A_LineNumber

#T=A_LoopField

#T=A_LoopFileAttrib

#T=A_LoopFileDir

#T=A_LoopFileExt

#T=A_LoopFileFullPath

#T=A_LoopFileLongPath

#T=A_LoopFileName

#T=A_LoopFileShortName

#T=A_LoopFileShortPath

#T=A_LoopFileSize

#T=A_LoopFileSizeKB

#T=A_LoopFileSizeMB

#T=A_LoopFileTimeAccessed

#T=A_LoopFileTimeCreated

#T=A_LoopFileTimeModified

#T=A_LoopReadLine

#T=A_LoopRegKey

#T=A_LoopRegName

#T=A_LoopRegSubkey

#T=A_LoopRegTimeModified

#T=A_LoopRegType

#T=A_MDAY

#T=A_Min

#T=A_MM

#T=A_MMM

#T=A_MMMM

#T=A_Mon

#T=A_MouseDelay

#T=A_MSec

#T=A_MyDocuments

#T=A_Now

#T=A_NowUTC

#T=A_NumBatchLines

#T=A_OSType

#T=A_OSVersion

#T=A_PriorHotkey

#T=A_ProgramFiles

#T=A_Programs

#T=A_ProgramsCommon

#T=A_ScreenHeight

#T=A_ScreenWidth

#T=A_ScriptDir

#T=A_ScriptFullPath

#T=A_ScriptName

#T=A_Sec

#T=A_Space

#T=A_StartMenu

#T=A_StartMenuCommon

#T=A_Startup

#T=A_StartupCommon

#T=A_StringCaseSense

#T=A_Tab

#T=A_Temp

#T=A_ThisFunc

#T=A_ThisHotkey

#T=A_ThisLabel

#T=A_ThisMenu

#T=A_ThisMenuItem

#T=A_ThisMenuItemPos

#T=A_TickCount

#T=A_TimeIdle

#T=A_TimeIdlePhysical

#T=A_TimeSincePriorHotkey

#T=A_TimeSinceThisHotkey

#T=A_TitleMatchMode

#T=A_TitleMatchModeSpeed

#T=A_UserName

#T=A_WDay

#T=A_WinDelay

#T=A_WinDir

#T=A_WorkingDir

#T=A_YDay

#T=A_YEAR

#T=A_YWeek

#T=A_YYYY

#T=Clipboard

#T=ClipboardAll

#T=ComSpec

#T=ErrorLevel

#T=ProgramFiles

#T=True

#T=False


#
