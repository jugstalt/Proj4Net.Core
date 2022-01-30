using System;

namespace Proj4Net.Tests.Projection
{
    public class ProjectionGridRoundTripper
    {
        private static readonly CoordinateTransformFactory CtFactory = new CoordinateTransformFactory();
        private static readonly CoordinateReferenceSystemFactory CsFactory = new CoordinateReferenceSystemFactory();
        private const String WGS84_PARAM = "+title=long/lat:WGS84 +proj=longlat +datum=WGS84 +units=degrees";
        CoordinateReferenceSystem WGS84 = CsFactory.CreateFromParameters("WGS84", WGS84_PARAM);

        
        private readonly CoordinateReferenceSystem _cs;
        private readonly ICoordinateTransform _transInverse;
        private readonly ICoordinateTransform _transForward;
        private const int GridSize = 4;
        private Boolean _debug;
        private int _transformCount;
        private double[] _gridExtent;

        public ProjectionGridRoundTripper(CoordinateReferenceSystem cs)
        {
            _cs = cs;
            _transInverse = CtFactory.CreateTransform(cs, WGS84);
            _transForward = CtFactory.CreateTransform(WGS84, cs);
        }

        public Boolean LevelDebug
        {
            get { return _debug; }
            set { _debug = value; }
        }

        public int TransformCount
        {
            get { return _transformCount; }
        }

        public double[] Extent
        {
            get { return _gridExtent; }
        }
        public Boolean RunGrid(double tolerance)
        {
            Boolean isWithinTolerance = true;

            _gridExtent = GridExtent(_cs.Projection);
            double minx = _gridExtent[0];
            double miny = _gridExtent[1];
            double maxx = _gridExtent[2];
            double maxy = _gridExtent[3];

            ProjCoordinate p = new ProjCoordinate();
            double dx = (maxx - minx) / GridSize;
            double dy = (maxy - miny) / GridSize;
            Int32 tests = 0, testsPassed = 0;
            for (int ix = 0; ix <= GridSize; ix++)
            {
                for (int iy = 0; iy <= GridSize; iy++)
                {
                    p.X = ix == GridSize ?
                               maxx
                               : minx + ix * dx;

                    p.Y = iy == GridSize ?
                               maxy
                               : miny + iy * dy;

                    tests++;
                    Boolean isWithinTol = RoundTrip(p, tolerance);
                    if (isWithinTol)
                        testsPassed++;
                }
            }
            return tests == testsPassed;
        }

        readonly ProjCoordinate _p2 = new ProjCoordinate();
        readonly ProjCoordinate _p3 = new ProjCoordinate();

        private Boolean RoundTrip(Coordinate p, double tolerance)
        {
            _transformCount++;

            _transForward.Transform(p, _p2);
            _transInverse.Transform(_p2, _p3);

            if (_debug)
                Console.WriteLine(p + " -> " + _p2 + " ->  " + _p3);

            double dx = Math.Abs(_p3.X - p.X);
            double dy = Math.Abs(_p3.Y - p.Y);

            Boolean isInTol = dx <= tolerance && dy <= tolerance;

            if (!isInTol)
                Console.WriteLine("FAIL: " + p + " -> " + _p2 + " ->  " + _p3);


            return isInTol;
        }

        public static double[] GridExtent(Proj4Net.Projection.Projection proj)
        {
            // scan all lat/lon params to try and determine a reasonable extent

            double lon = proj.ProjectionLongitudeDegrees;

            double[] latExtent = new double[] { Double.MaxValue, Double.MinValue };
            UpdateLat(proj.ProjectionLatitudeDegrees, latExtent);
            UpdateLat(proj.ProjectionLatitude1Degrees, latExtent);
            UpdateLat(proj.ProjectionLatitude2Degrees, latExtent);

            double centrex = lon;
            double centrey = 0.0;
            double gridWidth = 10;

            if (latExtent[0] < Double.MaxValue && latExtent[1] > Double.MinValue)
            {
                // got a good candidate

                double dlat = latExtent[1] - latExtent[0];
                if (dlat > 0) gridWidth = 2 * dlat;
                centrey = (latExtent[1] + latExtent[0]) / 2;
            }
            double[] extent = new double[4];
            extent[0] = centrex - gridWidth / 2;
            extent[1] = centrey - gridWidth / 2;
            extent[2] = centrex + gridWidth / 2;
            extent[3] = centrey + gridWidth / 2;
            return extent;
        }

        private static void UpdateLat(double lat, double[] latExtent)
        {
            // 0.0 indicates not set (for most projections?)
            if (lat == 0.0) return;
            if (lat < latExtent[0])
                latExtent[0] = lat;
            if (lat > latExtent[1])
                latExtent[1] = lat;
        }
    }
}