using System;
using System.Collections.Generic;
using System.Text;

namespace Proj4Net
{
    public class Coordinate
    {
        public Coordinate()
           : this(0.0, 0.0, 0.0)
        {

        }

        public Coordinate(double x, double y, double z=0.0)
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
    }
}
