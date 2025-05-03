using NUnit.Framework;
using Proj4Net.Core.Units;
using System;

namespace Proj4Net.Core.Tests
{
    [TestFixture]
    public class Proj4NetTests
    {
        [Test]
        public void AngleFormatTest()
        {
            AngleFormat af = new AngleFormat(AngleFormat.PatternLatitudeDDMMSS);
            String s1 = String.Format(af, "{0}", Math.PI);
            s1 = String.Format(af, "{0:DdMmSs}", Math.PI);
            Double v1 = af.Parse(s1);
            Assert.AreEqual(Math.PI, v1);

            s1 = String.Format(af, "{0:D°M'S\"N}", -0.9 * Math.PI);
            v1 = af.Parse(s1);
            Assert.AreEqual(-0.9 * Math.PI, v1);

        }

        [Test]
        public void CRSFactoryTest()
        {
            CoordinateReferenceSystemFactory crsFactory = new CoordinateReferenceSystemFactory();
            CoordinateReferenceSystem crsSource = crsFactory.CreateFromName("EPSG:4326");
            Assert.IsNotNull(crsSource);
            Assert.AreEqual("EPSG:4326", crsSource.Name);
            CoordinateReferenceSystem crsTarget = crsFactory.CreateFromParameters("EPSG:3875", "+proj=merc +lon_0=0 +k=1 +x_0=0 +y_0=0 +a=6378137 +b=6378137 +towgs84=0,0,0,0,0,0,0 +units=m +no_defs");
            Assert.IsNotNull(crsTarget);
            Assert.AreEqual("EPSG:3875", crsTarget.Name);
            BasicCoordinateTransform t = new BasicCoordinateTransform(crsSource, crsTarget);

            ProjCoordinate prjSrc = new ProjCoordinate(0, 0);
            ProjCoordinate prjTgt = new ProjCoordinate();
            t.Transform(prjSrc, prjTgt);

            BasicCoordinateTransform t2 = new BasicCoordinateTransform(crsTarget, crsSource);
            ProjCoordinate prjTgt2 = new ProjCoordinate();
            t2.Transform(prjTgt, prjTgt2);

            Assert.AreEqual(0d, prjSrc.Distance(prjTgt2));
        }
    }
}
