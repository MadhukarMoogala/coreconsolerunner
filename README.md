# Accoreconsole NUnit Test

### To Build

Before build ensure assemblies are repathed correctly.

```bash
git clone https://git.autodesk.com/moogalm/AccoreconsoleBug.git
cd AcCoreConsoleBug
msbuild /t:build AccoreconsoleBug.sln
```

### To Test

Copy the dll path and put in the script file, AutoNetCoreConsole2023.scr

```bash
run C3D
```

### Output

```bash
D:\Work\Cases\19462035\AccoreconsoleBug>run C3D
ErrorLevel 0
---------------Civil is running--------------------------------
"D:\turing\AutoCAD 2023\accoreconsole.exe" /i D:\Work\Cases\19462035\AccoreconsoleBug\AccoreconsoleBug\Metric.dwt /p "<<C3D_Metric>>" /product "C3D" /s "D:\Work\Cases\19462035\AccoreconsoleBug\AccoreconsoleBug\AutoNetCoreConsole2023.scr" /loadmodule "D:\turing\AutoCAD 2023\AecBase.dbx"
Redirect stdout (file: C:\Users\moogalm\AppData\Local\Temp\accc389522).
AcCoreConsole: StdOutConsoleMode: processed-output: enabled,auto
AutoCAD Core Engine Console - Copyright 2022 Autodesk, Inc.  All rights reserved. (T.114.0.0)

Execution Path:
D:\turing\AutoCAD 2023\accoreconsole.exe
Current Directory: D:\Work\Cases\19462035\AccoreconsoleBug

Version Number: T.114.0.0 (UNICODE)
LogFilePath has been set to the working folder.
Regenerating layout.
.
**** System Variable Changed ****
1 of the monitored system variables has changed from the preferred value. Use SYSVARMONITOR command to view changes.


AutoCAD menu utilities loaded.
Command:
Command:

Command:
Command: SECURELOAD

Enter new value for SECURELOAD <0>: 0

Command: netload Assembly file name: "D:\Work\Cases\19462035\AccoreconsoleBug\AccoreconsoleBug\bin\x64\Debug\AccoreconsoleBug.dll"

Command: RunCADtests

Command: QUIT

************************MessageBox****************************
AutoCAD Error Aborting


FATAL ERROR:  Unhandled Access Violation Reading 0x0126 Exception at 4CC945C9h


>>>Responding: OK.
**************************************************************
ErrorLevel 1
```

###  