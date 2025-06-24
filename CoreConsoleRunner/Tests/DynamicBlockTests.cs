using Autodesk.AutoCAD.DatabaseServices;
using CoreConsoleRunner.TestInfrastructure;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;

[TestFixture, Apartment(ApartmentState.STA), Category("DynamicBlocks")]
public class DynamicBlockTests : DrawingTestBase
{
    [Test]
    public void WidthTest()
    {
        var blockRef = FindFirstDynamicBlock();
        var width = GetProp(blockRef, "Width");
        Assert.That(width, Is.EqualTo(20));
        TestReport.Extent.CreateTest("WidthTest").Pass($"Width: {width}");
    }

    [Test]
    public void HeightTest()
    {
        var blockRef = FindFirstDynamicBlock();
        var height = GetProp(blockRef, "Height");
        Assert.That(height, Is.EqualTo(40));
        TestReport.Extent.CreateTest("HeightTest").Pass($"Height: {height}");
    }

    private BlockReference FindFirstDynamicBlock()
    {
        var bt = (BlockTable)trans.GetObject(testDb.BlockTableId, OpenMode.ForRead);
        foreach (ObjectId btrId in bt)
        {
            var btr = (BlockTableRecord)trans.GetObject(btrId, OpenMode.ForRead);
            if (!btr.IsDynamicBlock) continue;
            foreach (ObjectId anonId in btr.GetAnonymousBlockIds())
            {
                var anonBtr = (BlockTableRecord)trans.GetObject(anonId, OpenMode.ForRead);
                var refIds = anonBtr.GetBlockReferenceIds(true, true);
                if (refIds.Count > 0)
                    return (BlockReference)trans.GetObject(refIds[0], OpenMode.ForRead);
            }
        }
        throw new InvalidOperationException("No dynamic block reference found.");
    }

    private double GetProp(BlockReference br, string name)
    {
        var props = br.DynamicBlockReferencePropertyCollection;
        return props.OfType<DynamicBlockReferenceProperty>()
                    .Where(p => p.PropertyName == name)
                    .Select(p => Convert.ToDouble(p.Value))
                    .FirstOrDefault();
    }
}
