using Proj4Net.Core.Datum;
using RTools.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proj4Net.Core.Tests;

internal class BaseDatumsTransformationTest
{
    protected void DatumTransform(
            Datum.Datum source, 
            Datum.Datum target,
            Coordinate pt)
    {
        var sourceGeoConv = new GeocentricConverter(source.Ellipsoid);
        var targetGeoConv = new GeocentricConverter(target.Ellipsoid);

        if(source.Equals(target))
        {
            return;
        }

        sourceGeoConv.ConvertGeodeticToGeocentric(pt);

        if (source.HasTransformToWGS84)
        {
            source.TransformFromGeocentricToWgs84(pt);
        }
        if (target.HasTransformToWGS84)
        {
            target.TransformToGeocentricFromWgs84(pt);
        }

        targetGeoConv.ConvertGeocentricToGeodetic(pt);
    }
}
