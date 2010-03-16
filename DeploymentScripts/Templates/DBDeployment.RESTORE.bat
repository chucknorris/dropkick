@echo off

SET DIR=%~d0%~p0%

SET file.settings="%DIR%..\..\Settings\${environment}.settings"

::"%DIR%deployment\nant.exe" /f:"%DIR%scripts\db.deploy" -D:file.settings=%file.settings%