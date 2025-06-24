using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Autodesk.AutoCAD.Runtime;
using CoreConsoleRunner;
using NUnit.Framework;
using NUnitLite;

[assembly: CommandClass(typeof(NUnitLiteTestRunner))]
[assembly: Apartment(ApartmentState.STA)]

namespace CoreConsoleRunner
{
    
    public class NUnitLiteTestRunner
    {
        public static  string drawingPath = null;
        [CommandMethod("RunCADtests", CommandFlags.Session)]
        public void RunCADTests()
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            if (doc == null)
            {
                return;
            }
            var ed = doc.Editor;
            ed.WriteMessage("\nRunning NUnit tests...\n");

            string isCoreConsole = Environment.GetEnvironmentVariable("accoreconsole");
            if (isCoreConsole == null)
            {
                var promptFileNameResult = ed.GetFileNameForOpen("Open the drawing for test");
                if (promptFileNameResult.Status != Autodesk.AutoCAD.EditorInput.PromptStatus.OK)
                {
                    ed.WriteMessage("\nNo drawing file selected. Exiting tests.\n");
                    return;
                }
                drawingPath = promptFileNameResult.StringResult;
            }
           
            string directoryPlugin = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string directoryReportUnit = Path.Combine(directoryPlugin, @"ReportUnit");
            Directory.CreateDirectory(directoryReportUnit);
            string fileInputXML = Path.Combine(directoryReportUnit, @"Report-NUnit.xml");


            string[] nunitArgs =
            [
                // for details of options see  https://github.com/nunit/docs/wiki/Console-Command-Line
                "--trace=verbose" // Tell me everything
                ,"--result=" + fileInputXML
            ];

            new AutoRun().Execute(nunitArgs);
        }
    }
}