@echo off
setlocal EnableDelayedExpansion

REM Change to directory of the batch script
cd /d "%~dp0"
echo Current dir is now: %CD%


REM === Registry key for AutoCAD 2026 English version ===
set "ACAD_KEY=HKLM\SOFTWARE\Autodesk\AutoCAD\R25.1\ACAD-9101:409"
REM === Try to get the installation path ===
for /f "tokens=2,*" %%a in ('reg query "%ACAD_KEY%" /v AcadLocation 2^>nul') do (
    set "ACAD_PATH=%%b"
)


if not defined ACAD_PATH (
    echo ERROR: AutoCAD 2026 not found at registry key %ACAD_KEY%
    exit /b 1
)

set "CoreConsole=%ACAD_PATH%accoreconsole.exe"

if not exist "!CoreConsole!" (
    echo ERROR: accoreconsole.exe not found at !CoreConsole!
    exit /b 1
)

echo Using accoreconsole at: !CoreConsole!

REM === Define script and template paths relative to batch file ===
set "SCRIPT=%~dp0TestRunner.scr"
set "TEMPLATE=%~dp0C3D_METRIC.dwt"

REM === Optional: support Civil 3D launch ===
set "IS_C3D=%~1"

if /I "!IS_C3D!"=="C3D" (
    echo Running in Civil 3D mode...
    "!CoreConsole!" /i "%TEMPLATE%" /p "<<C3D_Metric>>" /product "C3D" /s "%SCRIPT%"
) else (
    echo Running in AutoCAD mode...
    "!CoreConsole!" /product "ACAD" /s "%SCRIPT%"
)

echo Exit Code: %ERRORLEVEL%
exit /b %ERRORLEVEL%
