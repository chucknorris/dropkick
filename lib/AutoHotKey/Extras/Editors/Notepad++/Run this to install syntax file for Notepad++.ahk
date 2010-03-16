msgbox

;***** AHK-Syntaxfile-Installer for Notepad++
;***** inkl. Auto-completion-file

RegRead, AppData, HKEY_CURRENT_USER, Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders, AppData
RegRead, Notepadpath, HKEY_LOCAL_MACHINE, SOFTWARE\Notepad++
If (Errorlevel=1)
	FileSelectFolder, Notepadpath,, 0, Choose folder: 

IfNotExist, %AppData%\Notepad++\
	FileCreateDir, %AppData%\Notepad++
else
	IfNotExist, %AppData%\Notepad++\Backup\
	{
		; MAKE BACKUP OF EXISTING FILE because there is currently no easy way to avoiding ovewriting the user's
		; settings with userDefineLang.xml.  This is done only the FIRST time (i.e. if the Backup folder doesn't yet
		; exist), since subsequent times will just be updates to the files created by a previous run of this script.
		IfExist, %AppData%\Notepad++\userDefineLang.xml
			Addendum = `n`nNote: Notepad++'s *previous* userDefineLang.xml file has been moved into "%AppData%\Notepad++\Backup\" in case you ever want to revert to it.
		FileCreateDir, %AppData%\Notepad++\Backup
		Filecopy, %AppData%\Notepad++\*.*, %AppData%\Notepad++\Backup\
	}

Filecopy, %A_ScriptDir%\userDefineLang.xml, %AppData%\Notepad++\, 1
Filecopy, %A_ScriptDir%\AHK Autohotkey.api, %Notepadpath%\plugins\APIs\, 1

MsgBox, AHK-Syntaxfile successfully installed!%Addendum%