# Unit Testing AutoCAD with NUnit + AcCoreConsole

This project demonstrates how to write and execute automated **unit tests for AutoCAD drawings** using:

- [`NUnit`](https://nunit.org/)

- `accoreconsole.exe` — AutoCAD’s headless scripting engine

- AutoCAD .NET API

- AutoCAD 2026 (latest as of this writing — works for older and future releases too)

---

## 🔧 Project Setup

### 1. Prerequisites

- **AutoCAD 2026 installed**

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

- Visual Studio 2022+ with desktop development workload

- Access to required AutoCAD .NET reference assemblies (`acdbmgd.dll`, `acmgd.dll` etc.)

---

### 2. Clone and Build

Before building, make sure AutoCAD assemblies are referenced correctly in the `.csproj`.



```bash
git clone https://github.com/MadhukarMoogala/coreconsolerunner.git
cd coreconsolerunner
msbuild /t:build coreconsolerunner.sln

```

## Running Tests

### 1. Prepare

- Make sure `TestRun.scr` points to your compiled test DLL.

- Verify the path to `accoreconsole.exe` (AutoCAD 2026).

### 2. Run

Use the provided batch file to launch tests in **headless AutoCAD**:

`RunTest.bat`

---

## 📄 Sample Output

```log
Current dir is now: D:\Work\repo\CoreConsoleTestRunner
Using accoreconsole at: D:\ACAD\watt\AutoCAD 2026\accoreconsole.exe
Running in AutoCAD mode...
Redirect stdout (file: C:\Users\moogalm\AppData\Local\Temp\accc300202).
AcCoreConsole: StdOutConsoleMode: processed-output: enabled,auto
AutoCAD Core Engine Console - Copyright 2025 Autodesk, Inc.  All rights reserved. (W.74.0.0)

Execution Path:
D:\ACAD\watt\AutoCAD 2026\accoreconsole.exe
Current Directory: D:\Work\repo\CoreConsoleTestRunner

Version Number: W.74.0.0 (UNICODE)
LogFilePath has been set to the working folder.
Regenerating model.
Drawing created using acadiso.dwt from AutoCAD profile: <<Unnamed Profile>>
**** System Variable Changed ****
1 of the monitored system variables has changed from the preferred value. Use SYSVARMONITOR command to view changes.


AutoCAD menu utilities loaded.
Command:
Command:

Command:
Command: SECURELOAD

Enter new value for SECURELOAD <0>: 0

Command: netload Assembly file name: "D:\Work\repo\CoreConsoleTestRunner\CoreConsoleRunner\bin\x64\Debug\net8.0-windows\win-x64\CoreConsoleRunner.dll"

Command: RunCADtests

Running NUnit tests...

Command: QUIT
_Y
Really want to discard all changes to drawing? <N> _Y

Command:
QUIT

LogFilePath has been restored to ''.
Exit Code: 0
```

---

## Test Reports

- XML results saved to:  
  `TestResult.xml`

- Optional: Generate **HTML reports** using:
  
  - [ExtentReports](https://extentreports.com/)
    
    ![SampleReport](report.png)

---

## Project Structure

 ![ProjectStructure](projectstructure.png)

---

## 

### Adding New Tests

To add a new AutoCAD entity test, create a class under the `Tests/` folder using the same pattern as existing ones. Each test class must:

- Inherit from `DrawingTestBase`

- Be decorated with `[TestFixture, Apartment(ApartmentState.STA), Category("EntityName")]`

🆕 Example: `LineTests`



```csharp
[TestFixture, Apartment(ApartmentState.STA), Category("Line")]
public class LineTests : DrawingTestBase
{
    private Line _line;

    public void GetFirstLine()
    {
        if (_line != null) return;

        var modelSpace = (BlockTableRecord)trans.GetObject(
            SymbolUtilityServices.GetBlockModelSpaceId(testDb),
            OpenMode.ForRead);

        foreach (ObjectId entId in modelSpace)
        {
            if (entId.ObjectClass.Name == "AcDbLine")
            {
                _line = trans.GetObject(entId, OpenMode.ForRead) as Line;
                break;
            }
        }

        if (_line == null)
            Assert.Fail("No line entity found in ModelSpace.");
    }

    [Test]
    public void LineLengthTest()
    {
        GetFirstLine();
        var test = TestReport.Extent.CreateTest(nameof(LineLengthTest));
        Assert.That(_line.Length, Is.EqualTo(100).Within(0.001));
        test.Pass($"Length: {_line.Length}");
    }

    [Test]
    public void LineStartPointTest()
    {
        GetFirstLine();
        var test = TestReport.Extent.CreateTest(nameof(LineStartPointTest));
        Assert.That(_line.StartPoint, Is.EqualTo(new Point3d(0, 0, 0)).Using<Point3d>((a, b) => a.IsEqualTo(b, new Tolerance(1e-6, 1e-6)) ? 0 : 1));
        test.Pass($"Start Point: {_line.StartPoint}");
    }
}

```



## 💡 Notes

- `accoreconsole.exe` is headless: ideal for CI/CD pipelines and regression testing.

- Ensure that `SECURELOAD` is set to `0` in script to allow `.dll` loading.

- Tests can inspect entities (e.g. `Circle`, `BlockReference`) using AutoCAD API.