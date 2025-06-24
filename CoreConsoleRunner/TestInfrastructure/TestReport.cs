using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Config;
using System.IO;

namespace CoreConsoleRunner.TestInfrastructure
{
    /// <summary>
    /// Class for managing test reports using ExtentReports.
    /// </summary>
    /// <remarks>
    /// This class initializes  

    public static class TestReport
    {
        private static ExtentReports _extent;
        public static ExtentTest CurrentTest { get; set; }

        static TestReport()
        {
            string reportPath = Path.Combine(Directory.GetCurrentDirectory(), "Report.html");
            var htmlReporter = new ExtentSparkReporter(reportPath);

            htmlReporter.Config.DocumentTitle = "AutoCAD Core Console Tests";
            htmlReporter.Config.ReportName = "Regression Report";
            htmlReporter.Config.Theme = Theme.Standard;

            _extent = new ExtentReports();
            _extent.AttachReporter(htmlReporter);
        }

        public static ExtentReports Extent => _extent;

        public static void Flush() => _extent.Flush();
    }
}
