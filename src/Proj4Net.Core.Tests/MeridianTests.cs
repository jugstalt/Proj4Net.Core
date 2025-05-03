using NUnit.Framework;
using Proj4Net.Core.Projection;
using System;

namespace Proj4Net.Core.Tests
{
    public class MeridianTests
    {
        public MeridianTests()
        {
        }

        [Test]
        public void TestNamedMeridians()
        {
            foreach (NamedMeridian nm in Enum.GetValues(typeof(NamedMeridian)))
            {
                if (nm == NamedMeridian.Unknown || nm == NamedMeridian.Undefined)
                    continue;

                var m = Meridian.CreateByNamedMeridian(nm);
                Assert.AreEqual((int)nm, m.Code);
                Assert.AreEqual(nm, m.Name);
                Assert.AreEqual(string.Format(" +pm={0}", nm.ToString().ToLower()), m.Proj4Description);
                var c = new Coordinate(0, 0);
                m.InverseAdjust(c);
                Assert.AreEqual(m.Longitude, c.X, 1e-7);
                m.Adjust(c);
                Assert.AreEqual(0, c.X, 1e-7);

                var m2 = Meridian.CreateByName(nm.ToString().ToLower());
                Assert.AreEqual(m, m2);

                var m3 = Meridian.CreateByDegree(Utility.ProjectionMath.ToDegrees(m.Longitude));
                Assert.AreEqual(m, m3);


            }
        }

        [Test]
        public void TestCustomMeridian()
        {
            var degree = 5.7;
            var m = Meridian.CreateByDegree(degree);
            Assert.AreEqual(NamedMeridian.Unknown, m.Name);
            Assert.AreEqual(Utility.ProjectionMath.ToDegrees(m.Longitude), degree);
            Assert.AreEqual(string.Format(System.Globalization.NumberFormatInfo.InvariantInfo, " +pm={0}", degree), m.Proj4Description);
        }
    }
}

