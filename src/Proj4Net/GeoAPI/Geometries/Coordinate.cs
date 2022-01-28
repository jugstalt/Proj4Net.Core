using System;
using System.Globalization;

namespace GeoAPI.Geometries
{
#if HAS_SYSTEM_SERIALIZABLEATTRIBUTE
    [Serializable]
#endif
#pragma warning disable 612, 618
    public class Coordinate : ICoordinate, IComparable<Coordinate>
#pragma warning restore 612,618
    {
        public const double NullOrdinate = Double.NaN;

        public double X; // = Double.NaN;
        public double Y; // = Double.NaN;
        public double Z; // = Double.NaN;

        public Coordinate(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double this[Ordinate ordinateIndex]
        {
            get
            {
                switch (ordinateIndex)
                {
                    case Ordinate.X:
                        return X;
                    case Ordinate.Y:
                        return Y;
                    case Ordinate.Z:
                        return Z;
                }
                throw new ArgumentOutOfRangeException("ordinateIndex");
            }
            set
            {
                switch (ordinateIndex)
                {
                    case Ordinate.X:
                        X = value;
                        return;
                    case Ordinate.Y:
                        Y = value;
                        return;
                    case Ordinate.Z:
                        Z = value;
                        return;
                }
                throw new ArgumentOutOfRangeException("ordinateIndex");
            }
        }

        public Coordinate() : this(0.0, 0.0, NullOrdinate) { }

        [Obsolete]
        public Coordinate(ICoordinate c) : this(c.X, c.Y, c.Z) { }

        public Coordinate(Coordinate c) : this(c.X, c.Y, c.Z) { }

        public Coordinate(double x, double y) : this(x, y, NullOrdinate) { }

        public Coordinate CoordinateValue
        {
            get { return this; }
            set
            {
                X = value.X;
                Y = value.Y;
                Z = value.Z;
            }
        }

        public bool Equals2D(Coordinate other)
        {
            return X == other.X && Y == other.Y;
        }

        public bool Equals2D(Coordinate c, double tolerance)
        {
            if (!EqualsWithTolerance(X, c.X, tolerance))
                return false;
            if (!EqualsWithTolerance(Y, c.Y, tolerance))
                return false;
            return true;
        }

        private static bool EqualsWithTolerance(double x1, double x2, double tolerance)
        {
            return Math.Abs(x1 - x2) <= tolerance;
        }

        public override bool Equals(object other)
        {
            if (other == null)
                return false;
            var otherC = other as Coordinate;
            if (otherC != null)
                return Equals(otherC);
#pragma warning disable 612,618
            if (!(other is ICoordinate))
                return false;
            return ((ICoordinate)this).Equals((ICoordinate)other);
#pragma warning restore 612,618
        }

        public Boolean Equals(Coordinate other)
        {
            return Equals2D(other);
        }

        public int CompareTo(object o)
        {
            var other = (Coordinate)o;
            return CompareTo(other);
        }

        public int CompareTo(Coordinate other)
        {
            if (X < other.X)
                return -1;
            if (X > other.X)
                return 1;
            if (Y < other.Y)
                return -1;
            return Y > other.Y ? 1 : 0;
        }

        public bool Equals3D(Coordinate other)
        {
            return (X == other.X) && (Y == other.Y) &&
                ((Z == other.Z) || (Double.IsNaN(Z) && Double.IsNaN(other.Z)));
        }

        public bool EqualInZ(Coordinate c, double tolerance)
        {
            return EqualsWithTolerance(this.Z, c.Z, tolerance);
        }

        public override string ToString()
        {
            return "(" + X.ToString("R", NumberFormatInfo.InvariantInfo) + ", " +
                         Y.ToString("R", NumberFormatInfo.InvariantInfo) + ", " +
                         Z.ToString("R", NumberFormatInfo.InvariantInfo) + ")";
        }

        public virtual Coordinate Copy()
        {
            return new Coordinate(X, Y, Z);
        }

        [Obsolete("Use Copy")]
        public object Clone()
        {
            return MemberwiseClone();
        }

        public double Distance(Coordinate c)
        {
            var dx = X - c.X;
            var dy = Y - c.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public double Distance3D(Coordinate c)
        {
            double dx = X - c.X;
            double dy = Y - c.Y;
            double dz = Z - c.Z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public override int GetHashCode()
        {
            var result = 17;
            // ReSharper disable NonReadonlyFieldInGetHashCode
            result = 37 * result + GetHashCode(X);
            result = 37 * result + GetHashCode(Y);
            // ReSharper restore NonReadonlyFieldInGetHashCode
            return result;
        }

        public static int GetHashCode(double value)
        {
            return value.GetHashCode();
        }

        #region ICoordinate

        [Obsolete]
        double ICoordinate.X
        {
            get { return X; }
            set { X = value; }
        }

        [Obsolete]
        double ICoordinate.Y
        {
            get { return Y; }
            set { Y = value; }
        }

        [Obsolete]
        double ICoordinate.Z
        {
            get { return Z; }
            set { Z = value; }
        }

        [Obsolete]
        double ICoordinate.M
        {
            get { return NullOrdinate; }
            set { }
        }

        [Obsolete]
        ICoordinate ICoordinate.CoordinateValue
        {
            get { return this; }
            set
            {
                X = value.X;
                Y = value.Y;
                Z = value.Z;
            }
        }

        [Obsolete]
        Double ICoordinate.this[Ordinate index]
        {
            get
            {
                switch (index)
                {
                    case Ordinate.X:
                        return X;
                    case Ordinate.Y:
                        return Y;
                    case Ordinate.Z:
                        return Z;
                    default:
                        return NullOrdinate;
                }
            }
            set
            {
                switch (index)
                {
                    case Ordinate.X:
                        X = value;
                        break;
                    case Ordinate.Y:
                        Y = value;
                        break;
                    case Ordinate.Z:
                        Z = value;
                        break;
                }
            }
        }

        [Obsolete]
        bool ICoordinate.Equals2D(ICoordinate other)
        {
            return X == other.X && Y == other.Y;
        }

        [Obsolete]
        int IComparable<ICoordinate>.CompareTo(ICoordinate other)
        {
            if (X < other.X)
                return -1;
            if (X > other.X)
                return 1;
            if (Y < other.Y)
                return -1;
            return Y > other.Y ? 1 : 0;
        }

        int IComparable.CompareTo(object o)
        {
            var other = (Coordinate)o;
            return CompareTo(other);
        }

        [Obsolete]
        bool ICoordinate.Equals3D(ICoordinate other)
        {
            return (X == other.X) && (Y == other.Y) &&
                ((Z == other.Z) || (Double.IsNaN(Z) && Double.IsNaN(other.Z)));
        }

        [Obsolete]
        double ICoordinate.Distance(ICoordinate p)
        {
            var dx = X - p.X;
            var dy = Y - p.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        #endregion ICoordinate
    }
}