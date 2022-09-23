@echo off
@echo ErrorLevel %ERRORLEVEL%
SET CoreConsole="D:\turing\AutoCAD 2023\accoreconsole.exe"
if /I "%~1" == "C3D" goto C3D
echo ---------------AutoCAD is running-------------------------
%CoreConsole% /product "ACAD" /s "D:\Work\Cases\19462035\AccoreconsoleBug\AccoreconsoleBug\AutoNetCoreConsole2023.scr"
goto :end
:C3D
echo ---------------Civil is running--------------------------------
echo %CoreConsole% /i D:\Work\Cases\19462035\AccoreconsoleBug\AccoreconsoleBug\Metric.dwt /p "<<C3D_Metric>>" /product "C3D" /s "D:\Work\Cases\19462035\AccoreconsoleBug\AccoreconsoleBug\AutoNetCoreConsole2023.scr" /loadmodule "D:\turing\AutoCAD 2023\AecBase.dbx"
%CoreConsole% /i D:\Work\Cases\19462035\AccoreconsoleBug\AccoreconsoleBug\Metric.dwt /p "<<C3D_Metric>>" /product "C3D" /s "D:\Work\Cases\19462035\AccoreconsoleBug\AccoreconsoleBug\AutoNetCoreConsole2023.scr" /loadmodule "D:\turing\AutoCAD 2023\AecBase.dbx"
goto :end
:end
@echo ErrorLevel %ERRORLEVEL%

::330798460