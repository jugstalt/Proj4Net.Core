using NUnit.Framework;
using System;

namespace Proj4Net.Core.Tests.Projection
{
    ///<summary>
    /// Tests accuracy and correctness of projecting and reprojecting a grid of geographic coordinates.</summary>
    ///<author>Martin Davis</author>
    [TestFixture]
    public class ProjectionGridTest
    {
        const double Tolerance = 0.00001;

        [Test]
        public void TestAlbers()
        {
            RunEPSG(3005);
        }
        [Test]
        public void TestStatePlane()
        {
            // State-plane EPSG defs
            RunEPSG(2759, 2930);
        }

        [Test]
        public void TestStatePlaneND()
        {
            RunEPSG(2265);
        }

        void RunEPSG(int codeStart, int codeEnd)
        {
            for (int i = codeStart; i <= codeEnd; i++)
            {
                RunEPSG(i);
            }
        }
        void RunEPSG(int code)
        {
            Run("epsg:" + code);
        }

        static void Run(String code)
        {
            CoordinateReferenceSystemFactory csFactory = new CoordinateReferenceSystemFactory();
            CoordinateReferenceSystem cs = csFactory.CreateFromName(code);
            if (cs == null)
                return;
            ProjectionGridRoundTripper tripper = new ProjectionGridRoundTripper(cs);
            //tripper.setLevelDebug(true);
            Boolean isOK = tripper.RunGrid(Tolerance);
            double[] extent = tripper.Extent;

            Console.WriteLine(code + " - " + cs.GetParameterString());
            Console.WriteLine(
                    @" - extent: [ " + extent[0] + @", " + extent[1] + @" : " + extent[2] + @", " + extent[3] + @" ]" +
                    @" - tol: " + Tolerance +
                    @" - # pts run = " + tripper.TransformCount);

            Assert.IsTrue(isOK);
        }
    }
}