using NUnit.Framework;
using Proj4Net.Core.Datum;
using Proj4Net.Core.Utility;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Proj4Net.Core.Tests;

[TestFixture]
internal class DatumsTransformationTest : BaseDatumsTransformationTest
{
    private const double Tolerance = 3e-7;
    protected const double RTD = ProjectionMath.RadiansToDegrees;
    protected const double DTR = ProjectionMath.DegreesToRadians;

    [Test]
    public void TestAllDatums()
    {
        // Experiment. Dont shure if this test is realy useful...

        foreach (var sourceDatum in Registry.Datums)
        {
            foreach (var targetDatum in Registry.Datums)
            {
                Console.WriteLine($"Test Datum Transformation {sourceDatum.Name} => {targetDatum.Name}");

                for (double lat = -80.0; lat <= 80.0; lat += 10.0)
                {
                    for (double lon = -170.0; lon <= 170.0; lon += 10.0)
                    {
                        var pt = new Coordinate(lon * DTR, lat * DTR);
                        base.DatumTransform(sourceDatum, targetDatum, pt);
                        Console.WriteLine($"  {targetDatum.Name}: {pt.ToString(false, true)}");
                        base.DatumTransform(targetDatum, sourceDatum, pt);
                        Console.WriteLine($"  {sourceDatum.Name}: {pt.ToString(false, true)}");

                        double dx = Math.Abs(lon - pt.X * RTD);
                        double dy = Math.Abs(lat - pt.Y * RTD);

                        Console.WriteLine($"  dx={dx} dy={dy}");

                        Assert.IsTrue(
                            dx < Tolerance &&
                            dy < Tolerance);

                        double dx_meter = dx * 111_000.0; // 1 degree is approx. 111 km
                        double dy_meter = dy * 111_000.0; // 1 degree is approx. 111 km

                        Console.WriteLine($"  dx_meter={dx_meter} dy_meter={dy_meter}");

                        Assert.IsTrue(
                            dx_meter < 0.05 &&
                            dy_meter < 0.05);
                    }
                }
            }
        }
    }

    [Test]
    public void TestAllGeocentricConvert()
    {
        foreach (var datum in Registry.Datums)
        {
            Console.WriteLine($"Test Datum Convert {datum.Name}");

            var geodeticConverter = new GeocentricConverter(datum.Ellipsoid);

            for (double h = 0; h < 10_000; h += 1000)
            {
                for (double lat = -89.0; lat <= 89.0; lat += 2.0)
                {
                    for (double lon = -179.0; lon <= 179.0; lon += 2.0)
                    {
                        var pt = new Coordinate(lon * DTR, lat * DTR, h);

                        geodeticConverter.ConvertGeodeticToGeocentric(pt);
                        var pt2 = new Coordinate(pt.X, pt.Y, pt.Z);

                        geodeticConverter.ConvertGeocentricToGeodetic(pt, GeocentricToGeodeticAlgorithm.Iterative);
                        geodeticConverter.ConvertGeocentricToGeodetic(pt2, GeocentricToGeodeticAlgorithm.Vermeille);

                        double dx = Math.Abs(pt2.X * RTD - pt.X * RTD);
                        double dy = Math.Abs(pt2.Y * RTD - pt.Y * RTD);

                        Console.WriteLine($"{lon},{lat}:  dx={dx} dy={dy}");

                        Assert.IsTrue(
                                dx < 3e-14 &&
                                dy < 3e-14);
                    }
                }
            }
        }
    }

    [Test]
    public void TestGeocentricConvert_WGS84()
    {
        var geodeticConverter = new GeocentricConverter(Ellipsoid.WGS84);

        var coord = new Coordinate(14.999999706036 * DTR, 48.000594094561 * DTR);
        
        geodeticConverter.ConvertGeodeticToGeocentric(coord);
        Console.WriteLine($"Geocentric: {coord.ToString(true)}");
        geodeticConverter.ConvertGeocentricToGeodetic(coord);

        var dx= Math.Abs(coord.X * RTD - 14.999999706036);
        var dy= Math.Abs(coord.Y * RTD - 48.000594094561);

        Console.WriteLine($"dx={dx} dy={dy}");

        Assert.IsTrue(dx < 2e-15 && dy < 2e-15);
    }

    [Test]
    public void TestGeocentricConvert_WGS84_Bessel()
    {
        var geodeticConverterWGS84 = new GeocentricConverter(Ellipsoid.WGS84);
        var geodeticConverterBessel = new GeocentricConverter(Ellipsoid.BESSEL);

        var coord = new Coordinate(14.999999706036 * DTR, 47.999999086691 * DTR);

        geodeticConverterBessel.ConvertGeodeticToGeocentric(coord);
        Console.WriteLine($"Geocentric: {coord.ToString(true)}");

        geodeticConverterWGS84.ConvertGeocentricToGeodetic(coord);
        Console.WriteLine($"Geodetic (WGS84): {coord.ToString(true, true)}");
        //coord.Z = 0;

        geodeticConverterWGS84.ConvertGeodeticToGeocentric(coord);
        Console.WriteLine($"Geocentric: {coord.ToString(true)}");

        geodeticConverterBessel.ConvertGeocentricToGeodetic(coord);
        Console.WriteLine($"Geodetic (Bessel): {coord.ToString(true, true)}");

        var dx = Math.Abs(coord.X * RTD - 14.999999706036);
        var dy = Math.Abs(coord.Y * RTD - 47.999999086691);

        Console.WriteLine($"dx={dx} dy={dy}");

        Assert.IsTrue(dx < 1e-14 && dy < 1e-14);
    }
}