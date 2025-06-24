using Autodesk.AutoCAD.DatabaseServices;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using static CoreConsoleRunner.RegressionTest;

namespace CoreConsoleRunner
{
    [TestFixture, Apartment(ApartmentState.STA)]
    internal class RegressionTest
    {
        private Database testDb = null;
        private Transaction transInMethod = null;
        private Params mParams = null;

        public class Params
        {
            public double Width { get; set; }
            public double Height { get; set; }
        }
        private static Params GetDynamicProperties(BlockReference br)
        {
            Params p = new Params();
            // Only continue is we have a valid dynamic block
            if (br != null && br.IsDynamicBlock)
            {
                
                // Get the dynamic block's property collection
                DynamicBlockReferencePropertyCollection pc = br.DynamicBlockReferencePropertyCollection;
                foreach (DynamicBlockReferenceProperty prop in pc)
                {
                    switch (prop.PropertyName)
                    {
                        case "Width":
                            p.Width = Convert.ToDouble(prop.Value);
                            break;
                        case "Height":
                            p.Height = Convert.ToDouble(prop.Value);
                            break;
                        default:
                            break;
                    }
                }
            }
            return p;
        }

        [OneTimeSetUp]
        public void Init()
        {
            string htmlReportPath = Path.Combine(Directory.GetCurrentDirectory(), "Report.html");
            TestReport.Init(htmlReportPath);
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length < 2)
            {
                throw new ArgumentException("Please provide the path to the drawing file as a command line argument.");
            }
            // Get the command line arguments
            var getCmdLine = args.ToArray();

            //Check for "/i <dwg path>"
            if (getCmdLine[0].EndsWith("accoreconsole.exe", StringComparison.OrdinalIgnoreCase) && getCmdLine[1].StartsWith("/i", StringComparison.OrdinalIgnoreCase))
            {
                var dwgPath = getCmdLine[2];
                if (!System.IO.File.Exists(dwgPath))
                {
                    throw new ArgumentException($"The specified drawing file does not exist: {dwgPath}");
                }
                testDb = HostApplicationServices.WorkingDatabase;

            }
            else
              testDb = DatabaseUtility.WorkingDatabaseManager.InitializeWorkingDatabase(NUnitLiteTestRunner.drawingPath);
        }
        [OneTimeTearDown]
        public void CleanUp()
        {
            TestReport.Flush();
            testDb.Dispose();
            
        }
        [SetUp]
        public void MethodSetup()
        {
            transInMethod = testDb.TransactionManager.StartTransaction();
            // Get an instance of the object.
            var bt = transInMethod.GetObject(testDb.BlockTableId, OpenMode.ForRead) as BlockTable;

            foreach (ObjectId btrId in bt)
            {
                //get the blockDef and check if is anonymous
                BlockTableRecord btr = (BlockTableRecord)transInMethod.GetObject(btrId, OpenMode.ForRead);
                if (btr.IsDynamicBlock)
                {
                    //get all anonymous blocks from this dynamic block
                    ObjectIdCollection anonymousIds = btr.GetAnonymousBlockIds();
                    ObjectIdCollection dynBlockRefs = new ObjectIdCollection();
                    foreach (ObjectId anonymousBtrId in anonymousIds)
                    {
                        //get the anonymous block
                        BlockTableRecord anonymousBtr = (BlockTableRecord)transInMethod.GetObject(anonymousBtrId, OpenMode.ForRead);
                        //and all references to this block
                        ObjectIdCollection blockRefIds = anonymousBtr.GetBlockReferenceIds(true, true);
                        foreach (ObjectId id in blockRefIds)
                        {
                            dynBlockRefs.Add(id);
                        }
                    }
                    if (dynBlockRefs.Count > 0)
                    {
                        //Get the first dynamic block reference, we have only one Dynamic Block reference in Drawing
                        var dBref = transInMethod.GetObject(dynBlockRefs[0], OpenMode.ForRead) as BlockReference;
                        mParams = GetDynamicProperties(dBref);
                    }
                }
            }
        }

        [TearDown]
        public void MethodTearDown()
        {
            transInMethod.Dispose();
        }

        [Test]
        [TestCase(TestName = "WidthTest", ExpectedResult = 20)]
        public double WidthTest()
        {
            TestReport.CurrentTest = TestReport.Extent.CreateTest("WidthTest");

            double actual = mParams.Width;
            TestReport.CurrentTest.Info($"Width from dynamic block: {actual}");
            if (actual == 20)
                TestReport.CurrentTest.Pass("Width is correct.");
            else
                TestReport.CurrentTest.Fail($"Expected 20 but got {actual}");

            return actual;
        }

        [Test]
        [TestCase(TestName = "HeightTest", ExpectedResult = 40)]
        public double HeightTest()
        {
            TestReport.CurrentTest = TestReport.Extent.CreateTest("HeightTest");

            double actual = mParams.Height;
            TestReport.CurrentTest.Info($"Height from dynamic block: {actual}");
            if (actual == 40)
                TestReport.CurrentTest.Pass("Height is correct.");
            else
                TestReport.CurrentTest.Fail($"Expected 40 but got {actual}");

            return actual;
        }
    }
}
