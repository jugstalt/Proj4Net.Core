using Proj4Net.Core.Datum.Grids;

namespace Proj4Net.Core.Abstraction;
public interface IDatumShiftTransformation
{
    bool Applies(PhiLambda location, out IDatumShiftTransformation datumShiftTransformation);

    Coordinate Apply(Coordinate geoCoord, bool inverse);
}
