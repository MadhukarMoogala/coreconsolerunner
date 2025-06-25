using Autodesk.AutoCAD.DatabaseServices;
using NUnit.Framework;
using System;
using System.IO;


namespace CoreConsoleRunner.TestInfrastructure
{
    [TestFixture]
    public abstract class DrawingTestBase
    {
        public static string DrawingFilePath { get; set; }
        protected Database testDb;
        protected Transaction trans;

        [OneTimeSetUp]
        public void Init()
        {
            if (string.IsNullOrEmpty(DrawingFilePath) || !File.Exists(DrawingFilePath))
                throw new FileNotFoundException("Drawing file not provided or invalid.");

            //check if the working database has valid named drawing
            if (HostApplicationServices.WorkingDatabase != null &&                 
                HostApplicationServices.WorkingDatabase.Filename.Equals(DrawingFilePath, StringComparison.OrdinalIgnoreCase))
            {
                //use the existing working database
                testDb = HostApplicationServices.WorkingDatabase;
            }
            else
            {
                //initialize a new working database from the provided drawing file
               testDb = DatabaseUtility.WorkingDatabaseManager.InitializeWorkingDatabase(DrawingFilePath);
            }
            
        }

        [SetUp]
        public void SetUp() =>
            trans = testDb.TransactionManager.StartTransaction();

        [TearDown]
        public void TearDown() =>
            trans.Dispose();

        [OneTimeTearDown]
        public void Cleanup() =>
            testDb?.Dispose();
    }
}
