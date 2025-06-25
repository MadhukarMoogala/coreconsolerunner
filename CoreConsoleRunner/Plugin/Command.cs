using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using CoreConsoleRunner.Plugin;
using CoreConsoleRunner.TestInfrastructure;
using NUnit.Framework;
using NUnitLite;
using System;
using System.IO;
using System.Linq;
using System.Threading;

[assembly: CommandClass(typeof(Command))]
[assembly: Apartment(ApartmentState.STA)]

namespace CoreConsoleRunner.Plugin
{
    public static class Utils
    {
        private static bool IsCoreConsole()
        {
            var flag = Environment.GetEnvironmentVariable("accoreconsole");
            if(flag == null)
            {
                
                flag = Environment.GetCommandLineArgs().FirstOrDefault(a => a.Equals("acad", StringComparison.OrdinalIgnoreCase));
                Console.WriteLine($"Running from AutoCAD GUI");
            }
            else
            {
                Console.WriteLine($"Running from AutoCAD Core Console: {flag}");
            }
            return flag?.Equals("Y", StringComparison.OrdinalIgnoreCase) == true;
        }
        public static string ResolveDrawingPath(Editor ed)
        {
            // Case 1: accoreconsole with /i argument
            if (IsCoreConsole())
            {
                return Environment.GetCommandLineArgs()
                    .FirstOrDefault(a => a.EndsWith(".dwg", StringComparison.OrdinalIgnoreCase));
            }

            // Case 2: UI mode, use currently open document if saved
            var doc = Application.DocumentManager.MdiActiveDocument;            
            if (doc != null && doc.IsNamedDrawing)
            {
                return doc.Name;
            }

            // Case 3: Ask user for DWG via dialog prompt
            var promptFileOptions = new PromptOpenFileOptions("Select a drawing file to run tests against:")
            {
                Filter = "Drawing Files (*.dwg)|*.dwg|All Files (*.*)|*.*",
                TransferRemoteFiles = true,
                SearchPath = true,
                PreferCommandLine = true
            };

            var fileNameResult = ed?.GetFileNameForOpen(promptFileOptions);
            return fileNameResult?.Status == PromptStatus.OK ? fileNameResult.StringResult : null;
        }

    }

    public class Command
    {
        [CommandMethod("RunCADtests", CommandFlags.Session)]
        public static void RunCADTests()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var ed = doc?.Editor;
            ed?.WriteMessage("\nRunning NUnit tests...\n");

            TestReport.Extent.AddSystemInfo("AutoCAD Version", HostApplicationServices.Current.releaseMarketVersion.ToString());
            TestReport.Extent.AddSystemInfo("Machine", Environment.MachineName);
            TestReport.Extent.AddSystemInfo("User", Environment.UserName);

            string drawingPath = Utils.ResolveDrawingPath(ed);

            if (string.IsNullOrEmpty(drawingPath) || !File.Exists(drawingPath))
            {
                ed?.WriteMessage($"\nInvalid or missing drawing path: {drawingPath}");
                return;
            }

            DrawingTestBase.DrawingFilePath = drawingPath;

            string[] nunitArgs = ["--trace=verbose"];

            new AutoRun().Execute(nunitArgs);
            TestReport.Flush();

            ed?.WriteMessage("\nNUnit tests completed.\n");
            ed?.WriteMessage($"Test report saved to: {TestReport.ReportPath}\n");
            ed?.WriteMessage($"You can QUIT AutoCAD\n");
        }
    }
}