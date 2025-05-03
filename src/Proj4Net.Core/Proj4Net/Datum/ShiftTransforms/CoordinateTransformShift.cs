using Proj4Net.Core.Abstraction;
using Proj4Net.Core.Datum.Grids;
using Proj4Net.Core.Utility;

namespace Proj4Net.Core.Datum.ShiftTransforms;
internal class CoordinateTransformShift : IDatumShiftTransformation
{
    private static CoordinateReferenceSystemFactory CrsFactory = new();
    private static CoordinateTransformFactory CtFactory = new();

    protected const double RTD = ProjectionMath.RadiansToDegrees;
    protected const double DTR = ProjectionMath.DegreesToRadians;

    private readonly ICoordinateTransform _transform, _transformInverse;

    private CoordinateTransformShift(string name, string parameters)
    {
        var toCrs = CrsFactory.CreateFromParameters("wgs84", "+proj=longlat +ellps=WGS84 +datum=WGS84 +towgs84=0,0,0,0,0,0,0");
        var fromCrs = CrsFactory.CreateFromParameters(name, parameters);

        _transform = CtFactory.CreateTransform(fromCrs, toCrs);
        _transformInverse = CtFactory.CreateTransform(toCrs, fromCrs);
    }

    static public IDatumShiftTransformation Create(string name, string parameters)
    {
        return new CoordinateTransformShift(name, parameters);
    }

    #region IDatumShiftTransformation

    public bool Applies(PhiLambda location, out IDatumShiftTransformation datumShiftTransformation)
    {
        datumShiftTransformation = this;
        return true;
    }

    public void Apply(Coordinate geoCoord, bool inverse)
    {
        geoCoord.X *= RTD;
        geoCoord.Y *= RTD;

        var transformedGeoCoord = inverse
            ? _transformInverse.Transform(geoCoord)
            : _transform.Transform(geoCoord);

        geoCoord.X = transformedGeoCoord.X * DTR;
        geoCoord.Y = transformedGeoCoord.Y * DTR;
    }

    #endregion
}
