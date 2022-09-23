using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using AccoreconsoleBug;
using Autodesk.AutoCAD.Runtime;
using NUnit.Framework;
using NUnitLite;

[assembly: CommandClass(typeof(NUnitLiteTestRunner))]
[assembly: Apartment(ApartmentState.STA)]

namespace AccoreconsoleBug
{
    public class NUnitLiteTestRunner
    {
        [CommandMethod("RunCADtests", CommandFlags.Session)]
        public void RunCADTests()
        {
           
            string directoryPlugin = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string directoryReportUnit = Path.Combine(directoryPlugin, @"ReportUnit");
            Directory.CreateDirectory(directoryReportUnit);
            string fileInputXML = Path.Combine(directoryReportUnit, @"Report-NUnit.xml");


            string[] nunitArgs = new List<string>
            {
                // for details of options see  https://github.com/nunit/docs/wiki/Console-Command-Line
                "--trace=verbose" // Tell me everything
                ,"--result=" + fileInputXML
            }.ToArray();

            new AutoRun().Execute(nunitArgs);
        }
    }
}