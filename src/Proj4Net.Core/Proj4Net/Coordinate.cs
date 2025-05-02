using Proj4Net.Core.Utility;
using System;

namespace Proj4Net.Core
{
    public class Coordinate
    {
        public Coordinate()
           : this(0.0, 0.0, 0.0)
        {

        }

        public Coordinate(double x, double y, double z = 0.0)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public double X { get; set; }
        public double Y { get; set; }
        public Double Z { get; set; }

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


        protected const double RTD = ProjectionMath.RadiansToDegrees;
        protected const double DTR = ProjectionMath.DegreesToRadians;

        public override string ToString() => ToString(false);
        public string ToString(bool printZ, bool radiansToDegrees = false, int round = 12)
        {
            double x = Math.Round( X * (radiansToDegrees ? RTD : 1.0), round);
            double y = Math.Round(Y * (radiansToDegrees ? RTD : 1.0), round);
            double z = Math.Round(Z, round);

            if (printZ)
            {
                return $"{x.ToString(System.Globalization.CultureInfo.InvariantCulture)},{y.ToString(System.Globalization.CultureInfo.InvariantCulture)},{z.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
            }
            return $"{x.ToString(System.Globalization.CultureInfo.InvariantCulture)},{y.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
        }
    }
}
