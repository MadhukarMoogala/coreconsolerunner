using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreConsoleRunner
{
    public static class TestReport
    {
        public static ExtentReports Extent;
        public static ExtentTest CurrentTest;

        public static void Init(string reportPath)
        {
            var spark = new ExtentSparkReporter(reportPath);
            spark.Config.DocumentTitle = "AutoCAD Core Console Tests";
            spark.Config.ReportName = "Regression Report";
            spark.Config.Theme = Theme.Standard;

            Extent = new ExtentReports();
            Extent.AttachReporter(spark);
        }

        public static void Flush()
        {
            Extent.Flush();
        }
    }
}
