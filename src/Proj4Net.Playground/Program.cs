using Proj4Net.Core;

string from = "EPSG:4326", to = "EPSG:31256", coords = String.Empty;
CoordinateReferenceSystemFactory crsFactory = new CoordinateReferenceSystemFactory();
CoordinateTransformFactory ctFactory = new CoordinateTransformFactory();

//var fromCrs = crsFactory.CreateFromName(from);
//var toCrs = crsFactory.CreateFromName(to);

var fromCrs = crsFactory.CreateFromParameters(from, "+proj=longlat +ellps=WGS84 +datum=WGS84 +towgs84=0,0,0,0,0,0,0");
var toCrs = crsFactory.CreateFromParameters(to, "+proj=tmerc +lat_0=0 +lon_0=16.33333333333333 +k=1.000000 +x_0=0 +y_0=-5000000 +ellps=bessel +units=m +nadgrids=AT_GIS_GRID_2021_09_28.gsb +towgs84=577.326,90.129,463.919,5.137,1.474,5.297,2.4232");
//var toCrs = crsFactory.CreateFromParameters(to, "+proj=tmerc +lat_0=0 +lon_0=16.33333333333333 +k=1.000000 +x_0=0 +y_0=-5000000 +ellps=bessel +units=m +towgs84=577.326,90.129,463.919,5.137,1.474,5.297,2.4232");
//var toCrs = crsFactory.CreateFromParameters(to, "+proj=tmerc +lat_0=0 +lon_0=16.33333333333333 +k=1.000000 +x_0=0 +y_0=-5000000 +ellps=bessel +units=m");

ICoordinateTransform trans = ctFactory.CreateTransform(toCrs, fromCrs);

/*
 * Create input and output points.
 * These can be constructed once per thread and reused.
 */
ProjCoordinate p = new ProjCoordinate();
ProjCoordinate p2 = new ProjCoordinate();
p.X = -67911.14;
p.Y = 215046.37;

/*
 * Transform point
 */
trans.Transform(p, p2);

Console.WriteLine($"Transformed coordinates: {p2.X}, {p2.Y}");

Console.ReadLine();