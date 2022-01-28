using System;

namespace GeoAPI.Geometries
{
#if HAS_SYSTEM_ICLONEABLE
    using ICloneable = System.ICloneable;
#else
    using ICloneable = GeoAPI.ICloneable;
#endif

    [Obsolete("Use Coordinate class instead")]
    public interface ICoordinate :
        ICloneable,
        IComparable, IComparable<ICoordinate>
    {
        double X { get; set; }

        double Y { get; set; }

        double Z { get; set; }

        double M { get; set; }

        ICoordinate CoordinateValue { get; set; }

        double this[Ordinate index] { get; set; }

        double Distance(ICoordinate other);

        bool Equals2D(ICoordinate other);

        bool Equals3D(ICoordinate other);
    }
}
