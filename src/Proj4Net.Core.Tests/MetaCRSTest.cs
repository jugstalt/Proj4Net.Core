using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Proj4Net.Core.Tests.IO
{
    /// <summary>
    /// Test which serves as an example of using Proj4J.
    /// </summary>
    /// <author>mbdavis</author>
    [TestFixture]
    public class MetaCRSTest
    {
        static readonly CoordinateReferenceSystemFactory CRSFactory = new CoordinateReferenceSystemFactory();

        [Test]//[Ignore]
        public void TestMetaCRSExample()
        {
            var passed = 0;
            var tests = new List<MetaCRSTestCase>();
            using (var file = Resources.Repository.Open(/*"TestData.csv"*/Properties.Resources.TestData))
            {
                var reader = new MetaCRSTestFileReader(file);

                tests.AddRange(reader.ReadTests());
                foreach (var test in tests)
                {
                    passed += RunTest(test);
                }
            }
            Assert.AreEqual(tests.Count, passed);
        }

        [Test]
        public void TestPROJ4_SPCS_ESRI_nad83()
        {
            var passed = 0;
            var tests = new List<MetaCRSTestCase>();
            using (var file = Resources.Repository.Open(/*"PROJ4_SPCS_ESRI_nad83_modified.csv"*/Proj4Net.Core.Tests.Properties.Resources.PROJ4_SPCS_ESRI_nad83_modified))
            {
                var reader = new MetaCRSTestFileReader(file);
                tests.AddRange(reader.ReadTests());
                foreach (var test in tests)
                {
                    passed += RunTest(test);
                }
            }
            Assert.AreEqual(tests.Count, passed);
        }

        [Test]
        public void TestPROJ4_SPCS_EPSG_nad83()
        {
            var passed = 0;
            var tests = new List<MetaCRSTestCase>();
            using (var file = Resources.Repository.Open(/*"PROJ4_SPCS_EPSG_nad83_modified"*/Proj4Net.Core.Tests.Properties.Resources.PROJ4_SPCS_EPSG_nad83_modified))
            {
                var reader = new MetaCRSTestFileReader(file);
                tests.AddRange(reader.ReadTests());
                foreach (var test in tests)
                {
                    passed += RunTest(test);
                }
            }
            Assert.AreEqual(tests.Count, passed);
        }

        [Test]
        public void TestPROJ4_SPCS_nad27()
        {
            var passed = 0;
            var tests = new List<MetaCRSTestCase>();
            using (var file = Resources.Repository.Open(/*"PROJ4_SPCS_nad27_modified.csv"*/ Properties.Resources.PROJ4_SPCS_nad27_modified))
            {
                var reader = new MetaCRSTestFileReader(file);
                tests.AddRange(reader.ReadTests());
                foreach (var test in tests)
                {
                    passed += RunTest(test);
                }
            }
            Assert.AreEqual(tests.Count, passed);
        }

        static Int32 RunTest(MetaCRSTestCase crsTest)
        {
            Int32 result = 0;
            try
            {
                result = crsTest.Execute(CRSFactory) ? 1 : 0;
                crsTest.Print(Console.Out);
            }
            catch (Proj4NetException ex)
            {
                Console.WriteLine(ex);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

    }
}