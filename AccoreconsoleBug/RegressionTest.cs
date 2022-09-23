using Autodesk.AutoCAD.DatabaseServices;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static AccoreconsoleBug.DatabaseUtility;
using static AccoreconsoleBug.RegressionTest;

namespace AccoreconsoleBug
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
            testDb = WorkingDatabaseManager.InitializeWorkingDatabase("D:\\Work\\Cases\\19462035\\AccoreconsoleBug\\AccoreconsoleBug\\TestDyn.dwg");           

        }
        [OneTimeTearDown]
        public void CleanUp()
        {
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
        [TestCase(TestName="WidthTest",ExpectedResult=20)]
        public double WidthTest()
        {

            return mParams.Width;
            
        }
        [Test]
        [TestCase(TestName = "HeightTest",ExpectedResult = 40)]
        public double HeightTest()
        {

            return mParams.Height;

        }
    }
}
