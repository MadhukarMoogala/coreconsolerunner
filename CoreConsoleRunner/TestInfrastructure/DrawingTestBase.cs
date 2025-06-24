using Autodesk.AutoCAD.DatabaseServices;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace CoreConsoleRunner.TestInfrastructure
{
    [TestFixture]
    public abstract class DrawingTestBase
    {
        protected Database testDb;
        protected Transaction trans;

        [OneTimeSetUp]
        public void Init()
        {
            string dwgPath = GetDrawingPathFromArgs();
            testDb = HostApplicationServices.WorkingDatabase ??
                     DatabaseUtility.WorkingDatabaseManager.InitializeWorkingDatabase(dwgPath);
        }

        [SetUp]
        public void SetUp() =>
            trans = testDb.TransactionManager.StartTransaction();

        [TearDown]
        public void TearDown() =>
            trans.Dispose();

        [OneTimeTearDown]
        public void Cleanup() =>
            testDb.Dispose();

        private static string GetDrawingPathFromArgs()
        {
            string[] args = Environment.GetCommandLineArgs();
            return args.FirstOrDefault(arg => arg.EndsWith(".dwg", StringComparison.OrdinalIgnoreCase))
                ?? throw new ArgumentException("DWG path not provided.");
        }
    }
}
