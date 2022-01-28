using System;

namespace Proj4Net.Tests
{

    public class CoordinateTransformTester
    {
        readonly Boolean _verbose = true;

        private readonly static CoordinateTransformFactory ctFactory = new CoordinateTransformFactory();
        private readonly static CoordinateReferenceSystemFactory crsFactory = new CoordinateReferenceSystemFactory();

        static readonly String WGS84_PARAM = "+title=long/lat:WGS84 +proj=longlat +ellps=WGS84 +datum=WGS84 +units=degrees";
        CoordinateReferenceSystem WGS84 = crsFactory.CreateFromParameters("WGS84", WGS84_PARAM);

        public CoordinateTransformTester(Boolean verbose)
        {
            _verbose = verbose;
        }

        private ProjCoordinate p = new ProjCoordinate();
        private ProjCoordinate p2 = new ProjCoordinate();

        public Boolean CheckTransformFromWGS84(String name, double lon, double lat, double x, double y)
        {
            return CheckTransformFromWGS84(name, lon, lat, x, y, 0.0001);
        }

        public Boolean CheckTransformFromWGS84(String name, double lon, double lat, double x, double y, double tolerance)
        {
            return CheckTransform(WGS84, lon, lat, CreateCRS(name), x, y, tolerance);
        }

  public bool CheckTransformToWGS84(String name, double x, double y, double lon, double lat, double tolerance)
  {
    return CheckTransform(CreateCRS(name), x, y, WGS84, lon, lat, tolerance);
  }
  
  public bool CheckTransformFromGeo(String name, double lon, double lat, double x, double y, double tolerance)
  {
    CoordinateReferenceSystem  crs = CreateCRS(name);
    CoordinateReferenceSystem geoCRS = crs.CreateGeographic();
    return CheckTransform(geoCRS, lon, lat, crs, x, y, tolerance);
  }
  
  public bool CheckTransformToGeo(String name, double x, double y, double lon, double lat, double tolerance)
  {
    CoordinateReferenceSystem  crs = CreateCRS(name);
    CoordinateReferenceSystem geoCRS = crs.CreateGeographic();
    return CheckTransform(crs, x, y, geoCRS, lon, lat, tolerance);
  }
  
        private static CoordinateReferenceSystem CreateCRS(String crsSpec)
        {
            CoordinateReferenceSystem cs;
            // test if name is a PROJ4 spec
            if (crsSpec.IndexOf("+") >= 0)
            {
                cs = crsFactory.CreateFromParameters("Anon", crsSpec);
            }
            else
            {
                cs = crsFactory.CreateFromName(crsSpec);
            }
            return cs;
        }

        public Boolean CheckTransform(
              String srcCRS, double x1, double y1,
              String tgtCRS, double x2, double y2, double tolerance)
        {
            return CheckTransform(
                    CreateCRS(srcCRS), x1, y1,
                    CreateCRS(tgtCRS), x2, y2, tolerance);
        }

        public Boolean CheckTransform(
              CoordinateReferenceSystem srcCRS, double x1, double y1,
              CoordinateReferenceSystem tgtCRS, double x2, double y2,
              double tolerance)
        {
            p.X = x1;
            p.Y = y1;
            var trans = ctFactory.CreateTransform(srcCRS, tgtCRS);
            trans.Transform(p, p2);

            var dx = Math.Abs(p2.X - x2);
            var dy = Math.Abs(p2.Y - y2);
            var delta = Math.Max(dx, dy);

            if (_verbose)
            {
                Console.Write("{0} => {1}", srcCRS.Name, tgtCRS.Name);
            }

            var isInTol = delta <= tolerance;

            if (_verbose)
            {
                if (!isInTol)
                {
                    Console.WriteLine(" ... FAILED");
                    var source = p.ToShortString();
                    Console.WriteLine("\t{0} -> {1}", source, p2.ToShortString());

                    var result = new ProjCoordinate(x2, y2);
                    var offset = new ProjCoordinate(p2.X - x2, p2.Y - y2);
                    Console.WriteLine("\t{0}    {1},  (tolerance={2}, max delta={3})",
                                      new string(' ', source.Length), result.ToShortString(),
                                      tolerance, delta);
                    Console.WriteLine("\tSource CRS: " + srcCRS.GetParameterString());
                    Console.WriteLine("\tTarget CRS: " + tgtCRS.GetParameterString());
                }
                else
                    Console.WriteLine(" ... PASSED");
            }


            return isInTol;
        }

        /// <summary>
        /// Checks forward and inverse transformations between
        /// two coordinate systems for a given pair of points.
        /// </summary>
        /// <param name="cs1"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="cs2"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="tolerance"></param>
        /// <param name="inverseTolerance"></param>
        /// <param name="checkInverse"></param>
        /// <returns></returns>
        public Boolean CheckTransform(
              String cs1, double x1, double y1,
              String cs2, double x2, double y2,
              double tolerance,
              double inverseTolerance,
              Boolean checkInverse)
        {
            var isOkForward = CheckTransform(cs1, x1, y1, cs2, x2, y2, tolerance);
            var isOkInverse = true;
            if (checkInverse)
                isOkInverse = CheckTransform(cs2, x2, y2, cs1, x1, y1, inverseTolerance);

            return isOkForward && isOkInverse;
        }
    }
}