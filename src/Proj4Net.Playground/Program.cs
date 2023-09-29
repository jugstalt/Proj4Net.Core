using Proj4Net.Core;

string from = "EPSG:4326", to = "EPSG:7856", coords = String.Empty;
CoordinateReferenceSystemFactory crsFactory = new CoordinateReferenceSystemFactory();
CoordinateTransformFactory ctFactory = new CoordinateTransformFactory();

var fromCrs = crsFactory.CreateFromName(from);
var toCrs = crsFactory.CreateFromName(to);
//var toCrs = crsFactory.CreateFromParameters(to, "+proj=utm +zone=56 +south +ellps=GRS80 +towgs84=0,0,0,0,0,0,0 +units=m +no_defs +type=crs");

