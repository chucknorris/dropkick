@echo off

::Project UppercuT - http://uppercut.googlecode.com
::No edits to this file are required - http://uppercut.pbwiki.com

if '%1' == '/?' goto usage
if '%1' == '-?' goto usage
if '%1' == '?' goto usage
if '%1' == '/help' goto usage

SET DIR=%~d0%~p0%
SET NANT="%DIR%lib\Nant\nant.exe"
SET build.config.settings="%DIR%settings\UppercuT.config"

%NANT% /f:.\build\default.build -D:build.config.settings=%build.config.settings% %*
if %ERRORLEVEL% NEQ 0 goto errors

SET nunitresults=%DIR%build_output\build_artifacts\nunit\nunit-results.xml
echo ##teamcity[importData type='nunit' path='%nunitresults%']

goto finish

:usage
echo.
echo Usage: build.bat
echo.
goto finish

:errors
EXIT /B %ERRORLEVEL%

:finish