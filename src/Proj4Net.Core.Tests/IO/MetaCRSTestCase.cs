using System;
using System.IO;
using Proj4Net.Core.Utility;

namespace Proj4Net.Core.Tests.IO
{
    public class MetaCRSTestCase
    {
        private static readonly CoordinateTransformFactory CtFactory = new CoordinateTransformFactory();

        private static Boolean _verbose = true;

        private readonly String _testName;
        private readonly String _testMethod;

        private readonly String _srcCrsAuth;
        private readonly String _srcCrs;

        private readonly String _tgtCrsAuth;
        private readonly String _tgtCrs;

        private readonly double _srcOrd1;
        private readonly double _srcOrd2;
        private readonly double _srcOrd3;

        private readonly double _tgtOrd1;
        private readonly double _tgtOrd2;
        private readonly double _tgtOrd3;

        private readonly double _tolOrd1;
        private readonly double _tolOrd2;
        private readonly double _tolOrd3;

        private String _using;
        private String _dataSource;
        private String _dataCmnts;
        private String _maintenanceCmnts;

        private CoordinateReferenceSystem _srcCRS;
        private CoordinateReferenceSystem _tgtCRS;

        private readonly ProjCoordinate _srcPt = new ProjCoordinate();
        private readonly ProjCoordinate _resultPt = new ProjCoordinate();

        private Boolean _isInTol;
        private CoordinateReferenceSystemCache _crsCache;

        public MetaCRSTestCase(
            String testName,
            String testMethod,
            String srcCrsAuth,
            String srcCrs,
            String tgtCrsAuth,
            String tgtCrs,
            double srcOrd1,
            double srcOrd2,
            double srcOrd3,
            double tgtOrd1,
            double tgtOrd2,
            double tgtOrd3,
            double tolOrd1,
            double tolOrd2,
            double tolOrd3,
            String susing,
            String dataSource,
            String dataCmnts,
            String maintenanceCmnts
            )
        {
            _testName = testName;
            _testMethod = testMethod;
            _srcCrsAuth = srcCrsAuth;
            _srcCrs = srcCrs;
            _tgtCrsAuth = tgtCrsAuth;
            _tgtCrs = tgtCrs;
            _srcOrd1 = srcOrd1;
            _srcOrd2 = srcOrd2;
            _srcOrd3 = srcOrd3;
            _tgtOrd1 = tgtOrd1;
            _tgtOrd2 = tgtOrd2;
            _tgtOrd3 = tgtOrd3;
            _tolOrd1 = tolOrd1;
            _tolOrd2 = tolOrd2;
            _tolOrd3 = tolOrd3;
            _using = susing;
            _dataSource = dataSource;
            _dataCmnts = dataCmnts;
            _maintenanceCmnts = maintenanceCmnts;
        }

        public String Name
        {
            get { return _testName; }
        }

        public String Method
        {
            get { return _testMethod; }
        }

        public String SourceCrsName
        {
            get { return CRSName(_srcCrsAuth, _srcCrs); }
        }

        public String TargetCrsName
        {
            get { return CRSName(_tgtCrsAuth, _tgtCrs); }
        }

        public CoordinateReferenceSystem SourceCRS
        {
            get { return _srcCRS; }
        }

        public CoordinateReferenceSystem TargetCRS
        {
            get { return _tgtCRS; }
        }

        public Coordinate SourceCoordinate
        {
            get {return new ProjCoordinate(_srcOrd1, _srcOrd2, _srcOrd3);}
        }

        public Coordinate TargetCoordinate
        {
            get { return new ProjCoordinate(_tgtOrd1, _tgtOrd2, _tgtOrd3); }
        }

        public Coordinate ResultCoordinate
        {
            get { return new ProjCoordinate(_resultPt.X, _resultPt.Y); }
        }

        public CoordinateReferenceSystemCache Cache
        {
            set { _crsCache = _crsCache; }
        }

        public Boolean Execute(CoordinateReferenceSystemFactory csFactory)
        {
            _srcCRS = CreateCRS(csFactory, _srcCrsAuth, _srcCrs);
            _tgtCRS = CreateCRS(csFactory, _tgtCrsAuth, _tgtCrs);
            var isOK = ExecuteTransform(_srcCRS, _tgtCRS);
            return isOK;
        }

        public static String CRSName(String auth, String code)
        {
            return auth + ":" + code;
        }

        public CoordinateReferenceSystem CreateCRS(CoordinateReferenceSystemFactory crsFactory, String auth, String code)
        {
            var name = CRSName(auth, code);

            if (_crsCache != null)
            {
                return _crsCache.CreateFromName(name);
            }
            var cs = crsFactory.CreateFromName(name);
            return cs;
        }

        private Boolean ExecuteTransform(
            CoordinateReferenceSystem srcCRS,
            CoordinateReferenceSystem tgtCRS)
        {
            _srcPt.X = _srcOrd1;
            _srcPt.Y = _srcOrd2;
            // Testing: flip axis order to test SS sample file
            // srcPt.x = srcOrd2;
            // srcPt.y = srcOrd1;

            var trans = CtFactory.CreateTransform(srcCRS, tgtCRS);

            trans.Transform(_srcPt, _resultPt);

            var dx = Math.Abs(_resultPt.X - _tgtOrd1);
            var dy = Math.Abs(_resultPt.Y - _tgtOrd2);
            var dz = 0d;
            if (!Double.IsNaN(_tolOrd3))
            {
                dz = Math.Abs(_resultPt.Z - _tgtOrd3);
            }

            _isInTol = dx <= _tolOrd1 && dy <= _tolOrd2;
            if (dz != 0)
                _isInTol |= (!double.IsNaN(dz) && dz <= _tolOrd3);

            return _isInTol;
        }

        public void Print(TextWriter os)
        {
            os.Write(_testName);
            if (_verbose)
            {
                os.WriteLine();
                os.WriteLine("{0} -> {1} (expected {2})", _srcPt.ToShortString(), _resultPt.ToShortString(),
                                  new ProjCoordinate(_tgtOrd1, _tgtOrd2, _tgtOrd3).ToShortString());
            }

            if (!_isInTol)
            {
                os.WriteLine(@" ... FAIL");
                if (_verbose)
                {
                    os.WriteLine(@"  Source CRS ({0}:{1}): {2}", _srcCrsAuth, _srcCrs, _srcCRS.GetParameterString());
                    os.WriteLine(@"  Target CRS ({0}:{1}): {2}", _tgtCrsAuth, _tgtCrs, _tgtCRS.GetParameterString());
                }
            }
            else
            {
                os.WriteLine(" ... PASSED");
            }
        }
    }
}