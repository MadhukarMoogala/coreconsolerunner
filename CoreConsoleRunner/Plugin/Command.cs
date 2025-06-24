using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using CoreConsoleRunner.Plugin;
using CoreConsoleRunner.TestInfrastructure;
using NUnit.Framework;
using NUnitLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

[assembly: CommandClass(typeof(Command))]
[assembly: Apartment(ApartmentState.STA)]

namespace CoreConsoleRunner.Plugin
{
    
    public class Command
    {
        [CommandMethod("RunCADtests", CommandFlags.Session)]
        public void RunCADTests()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var ed = doc?.Editor;
            ed?.WriteMessage("\nRunning NUnit tests...\n");
            TestReport.Extent.AddSystemInfo("AutoCAD Version", HostApplicationServices.Current.releaseMarketVersion.ToString());
            TestReport.Extent.AddSystemInfo("Machine", Environment.MachineName);
            TestReport.Extent.AddSystemInfo("User", Environment.UserName);

            string[] nunitArgs = [
                "--trace=verbose"
            ];
            new AutoRun().Execute(nunitArgs);   
            TestReport.Flush();
        }

    }
}