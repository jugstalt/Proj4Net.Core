using System;
using NUnit.Framework;

namespace Proj4Net.Core.Tests
{
    /// <summary>
    /// Tests correctness and accuracy of Coordinate System transformations.
    /// </summary>
    /// <author>Martin Davis</author>
    public abstract class BaseCoordinateTransformTest
    {
        // ~= 1 / (2Pi * Earth radius) 
        // in code: 1.0 / (2.0 * Math.PI * 6378137.0);
        public const double ApproximateMeterInDegrees = 2.0e-8; 

        //private static bool debug = true;
        private readonly string _name;

        private static readonly CoordinateTransformTester Tester = new CoordinateTransformTester(true);

        protected BaseCoordinateTransformTest(String name)
        {
            _name = name;
        }

        protected void CheckTransformFromWGS84(String code, double lon, double lat, double x, double y)
        {
            Assert.IsTrue(Tester.CheckTransformFromWGS84(code, lon, lat, x, y, 0.0001));
        }

        protected void CheckTransformFromWGS84(String code, double lon, double lat, double x, double y, double tolerance)
        {
            Assert.IsTrue(Tester.CheckTransformFromWGS84(code, lon, lat, x, y, tolerance));
        }

        protected void CheckTransformToWGS84(String code, double x, double y, double lon, double lat, double tolerance)
        {
            Assert.IsTrue(Tester.CheckTransformToWGS84(code, x, y, lon, lat, tolerance));
        }

        protected void CheckTransformFromGeo(String code, double lon, double lat, double x, double y)
        {
            Assert.IsTrue(Tester.CheckTransformFromGeo(code, lon, lat, x, y, 0.0001));
        }

        protected void CheckTransformFromGeo(String code, double lon, double lat, double x, double y, double tolerance)
        {
            Assert.IsTrue(Tester.CheckTransformFromGeo(code, lon, lat, x, y, tolerance));
        }

        protected void CheckTransformToGeo(String code, double x, double y, double lon, double lat, double tolerance)
        {
            Assert.IsTrue(Tester.CheckTransformToGeo(code, x, y, lon, lat, tolerance));
        }

        protected void CheckTransformFromAndToGeo(String code, double lon, double lat, double x, double y, double tolProj, double tolGeo)
        {
            Assert.IsTrue(Tester.CheckTransformFromGeo(code, lon, lat, x, y, tolProj));
            Assert.IsTrue(Tester.CheckTransformToGeo(code, x, y, lon, lat, tolGeo));
        }


        protected void CheckTransform(
            String cs1, double x1, double y1,
            String cs2, double x2, double y2,
            double tolerance)
        {
            Assert.IsTrue(Tester.CheckTransform(cs1, x1, y1, cs2, x2, y2, tolerance));
        }

        protected void CheckTransformAndInverse(
            String cs1, double x1, double y1,
            String cs2, double x2, double y2,
            double tolerance,
            double inverseTolerance)
        {
            Assert.IsTrue(Tester.CheckTransform(cs1, x1, y1, cs2, x2, y2, tolerance, inverseTolerance, true));
        }

    }
}