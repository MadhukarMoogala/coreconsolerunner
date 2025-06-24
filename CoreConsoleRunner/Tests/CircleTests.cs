using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CoreConsoleRunner.TestInfrastructure;
using NUnit.Framework;
using System.Threading;

[TestFixture, Apartment(ApartmentState.STA),Category("Circle")]
public class CircleTests : DrawingTestBase
{
    private Circle _circle;

    public void GetFirstCircle()
    {
        if (_circle != null)
            return; // Already loaded

        var modelSpace = (BlockTableRecord)trans.GetObject(
            SymbolUtilityServices.GetBlockModelSpaceId(testDb),
            OpenMode.ForRead);

        foreach (ObjectId entId in modelSpace)
        {
            if (entId.ObjectClass.Name == "AcDbCircle")
            {
                _circle = trans.GetObject(entId, OpenMode.ForRead) as Circle;
                break;
            }
        }

        if (_circle == null)
            Assert.Fail("No circle entity found in ModelSpace.");
    }

    [Test]
    public void CircleRadiusTest()
    {
        GetFirstCircle();
        var test = TestReport.Extent.CreateTest(nameof(CircleRadiusTest));

        double expected = 50;
        double actual = _circle.Radius;

        test.Info($"Radius: {actual}");
        Assert.That(actual, Is.EqualTo(expected).Within(0.001));
        test.Pass("Radius is correct.");
    }

    [Test]
    public void CircleCenterTest()
    {
        GetFirstCircle();
        var test = TestReport.Extent.CreateTest(nameof(CircleCenterTest));

        Point3d expected = new Point3d(10, 10, 0);
        Point3d actual = _circle.Center;

        test.Info($"Center: {actual}");
        Assert.That(actual.IsEqualTo(expected, new Tolerance(1e-6, 1e-6)));
        test.Pass("Center is correct.");
    }

    [Test]
    public void CircleAreaTest()
    {
        GetFirstCircle();
        var test = TestReport.Extent.CreateTest(nameof(CircleAreaTest));

        double expected = System.Math.PI * 50 * 50;
        double actual = _circle.Area;

        test.Info($"Area: {actual}");
        Assert.That(actual, Is.EqualTo(expected).Within(0.01));
        test.Pass("Area is correct.");
    }
}
