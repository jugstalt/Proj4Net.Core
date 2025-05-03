using NUnit.Framework;
using System;

namespace Proj4Net.Core.Tests
{
    [TestFixture]
    public class ExampleTest
    {
        [Test]
        public void TransformToGeographic()
        {
            Assert.IsTrue(CheckTransform("EPSG:2227", -121.3128278, 37.95657778, 6327319.23, 2171792.15, 0.01));
        }

        private static Boolean CheckTransform(String csName, double lon, double lat, double expectedX, double expectedY, double tolerance)
        {
            CoordinateTransformFactory ctFactory = new CoordinateTransformFactory();
            CoordinateReferenceSystemFactory csFactory = new CoordinateReferenceSystemFactory();
            /*
             * Create {@link CoordinateReferenceSystem} & CoordinateTransformation.
             * Normally this would be carried out once and reused for all transformations
             */
            CoordinateReferenceSystem crs = csFactory.CreateFromName(csName);

            const String wgs84Param = "+title=long/lat:WGS84 +proj=longlat +ellps=WGS84 +datum=WGS84 +units=degrees";
            CoordinateReferenceSystem wgs84 = csFactory.CreateFromParameters("WGS84", wgs84Param);

            ICoordinateTransform trans = ctFactory.CreateTransform(wgs84, crs);

            /*
             * Create input and output points.
             * These can be constructed once per thread and reused.
             */
            ProjCoordinate p = new ProjCoordinate();
            ProjCoordinate p2 = new ProjCoordinate();
            p.X = lon;
            p.Y = lat;

            /*
             * Transform point
             */
            trans.Transform(p, p2);


            return IsInTolerance(p2, expectedX, expectedY, tolerance);
        }
        [Test]
        public void TestExplicitTransform()
        {
            const String csName1 = "EPSG:32636";
            const String csName2 = "EPSG:4326";

            CoordinateTransformFactory ctFactory = new CoordinateTransformFactory();
            CoordinateReferenceSystemFactory csFactory = new CoordinateReferenceSystemFactory();
            /*
             * Create {@link CoordinateReferenceSystem} & CoordinateTransformation.
             * Normally this would be carried out once and reused for all transformations
             */
            CoordinateReferenceSystem crs1 = csFactory.CreateFromName(csName1);
            CoordinateReferenceSystem crs2 = csFactory.CreateFromName(csName2);

            ICoordinateTransform trans = ctFactory.CreateTransform(crs1, crs2);

            /*
             * Create input and output points.
             * These can be constructed once per thread and reused.
             */
            ProjCoordinate p1 = new ProjCoordinate();
            ProjCoordinate p2 = new ProjCoordinate();
            p1.X = 500000;
            p1.Y = 4649776.22482;

            /*
             * Transform point
             */
            trans.Transform(p1, p2);

            Assert.IsTrue(IsInTolerance(p2, 33, 42, 0.000001));
        }

        private static Boolean IsInTolerance(Coordinate p, double x, double y, double tolerance)
        {
            /*
             * Compare result to expected, for test purposes
             */
            double dx = Math.Abs(p.X - x);
            double dy = Math.Abs(p.Y - y);
            Boolean isInTol = dx <= tolerance && dy <= tolerance;
            return isInTol;
        }
    }
}