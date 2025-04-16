using Proj4Net.Core;

string from = "EPSG:4326", to = "EPSG:31256", coords = String.Empty;
CoordinateReferenceSystemFactory crsFactory = new CoordinateReferenceSystemFactory();
CoordinateTransformFactory ctFactory = new CoordinateTransformFactory();

//var fromCrs = crsFactory.CreateFromName(from);
//var toCrs = crsFactory.CreateFromName(to);


var toCrs = crsFactory.CreateFromParameters(from, "+proj=longlat +ellps=WGS84 +datum=WGS84 +towgs84=0,0,0,0,0,0,0");
//var toCrs = crsFactory.CreateFromParameters(from, "+proj=longlat +ellps=WGS84 +datum=WGS84");
//var toCrs = crsFactory.CreateFromParameters(to, "+proj=tmerc +lat_0=0 +lon_0=16.33333333333333 +k=1.000000 +x_0=0 +y_0=-5000000 +ellps=bessel +units=m +towgs84=577.326,90.129,463.919,5.137,1.474,5.297,2.4232");
//var toCrs = crsFactory.CreateFromParameters(to, "+proj=merc +a=6378137 +b=6378137 +lat_ts=0.0 +lon_0=0.0 +x_0=0.0 +y_0=0 +k=1.0 +units=m +nadgrids=@null +wktext +towgs84=0,0,0,0,0,0,0");
/*
 * Create input and output points.
 * These can be constructed once per thread and reused.
 */
ProjCoordinate p = new ProjCoordinate();
ProjCoordinate p2 = new ProjCoordinate();
ProjCoordinate p0 = new ProjCoordinate();

// 15 48 with grid
//var fromCrs = crsFactory.CreateFromParameters(to, "+proj=tmerc +lat_0=0 +lon_0=16.33333333333333 +k=1.000000 +x_0=0 +y_0=-5000000 +ellps=bessel +units=m +nadgrids=AT_GIS_GRID_2021_09_28.gsb");
//p.X = -99411.663305323091;
//p.Y = 318802.50516882259;

// 15 48 with +towgs84
var fromCrs = crsFactory.CreateFromParameters(to, "+proj=tmerc +lat_0=0 +lon_0=16.33333333333333 +k=1.000000 +x_0=0 +y_0=-5000000 +ellps=bessel +units=m +towgs84=577.326,90.129,463.919,5.137,1.474,5.297,2.4232");
p.X = -99411.687267207672;
p.Y = 318802.40399568342;


// 15 48 without datum
//var fromCrs = crsFactory.CreateFromParameters(to, "+proj=tmerc +lat_0=0 +lon_0=16.33333333333333 +k=1.000000 +x_0=0 +y_0=-5000000 +ellps=bessel +units=m");
//p.X = -99487.450673206578;
//p.Y = 318745.56258221995;

Console.WriteLine($"Original coordinates   : {Math.Round(p.X, 8)}, {Math.Round(p.Y, 8)}");

/*
 * Transform point
 */
ICoordinateTransform trans = ctFactory.CreateTransform(fromCrs, toCrs);

int interations = 10_000_000;

var helper = new Coordinate();
// Warmup
//for (int i = 0; i < interations; i++)
//{
//    trans.Transform(p, p2, helper);
//}

Parallel.For(0, interations, i =>
{
    trans.Transform(p, p2);
});

var dtStart = DateTime.Now;

trans.Transform(interations, 
    (i, from) => {
        from.X = p.X;
        from.Y = p.Y;
    },
    (i, to) => {
        p2.X = to.X;
        p2.Y = to.Y;
    }, 
    true);

//Parallel.For(0, interations, i =>
//{
//    trans.Transform(p, p2);
//});

//for (int i = 0; i < interations; i++)
//{
//    trans.Transform(p, p2);
//}
var dtStop = DateTime.Now;

Console.WriteLine("Made {0} iterations in {1} ms", interations, (dtStop - dtStart).TotalMilliseconds);

Console.WriteLine($"Transformed coordinates: {Math.Round(p2.X, 8)}, {Math.Round(p2.Y, 8)}");
/*
 * Inv Transform point
 */
trans = ctFactory.CreateTransform(toCrs, fromCrs);
trans.Transform(p2, p0);

Console.WriteLine($"Transformed coordinates: {Math.Round(p0.X, 8)}, {Math.Round(p0.Y, 8)}");

return;

string path = @"G:\github\jugstalt\Proj4Net.Core\src\Proj4Net.Playground\bin\Debug\net6.0\share\proj";

foreach (var fileInfo in new DirectoryInfo(path).GetFiles("*.gsb"))
{
    Console.WriteLine(fileInfo.FullName);


    //string filePath = @"G:\github\jugstalt\Proj4Net.Core\src\Proj4Net.Playground\bin\Debug\net6.0\share\proj\AT_GIS_GRID_2021_09_28.gsb";
    string filePath = fileInfo.FullName;    

    var ntv2 = Ntv2Reader.ReadFile(filePath);

    Console.WriteLine("== Global Header ==");
    Console.WriteLine($"  NumORec  = {ntv2.GlobalHeader.NUM_OREC}");
    Console.WriteLine($"  NumSRec  = {ntv2.GlobalHeader.NUM_SREC}");
    Console.WriteLine($"  NumFile  = {ntv2.GlobalHeader.NUM_FILE}");
    Console.WriteLine($"  GsType   = {ntv2.GlobalHeader.GS_TYPE}");
    Console.WriteLine($"  Version  = {ntv2.GlobalHeader.VERSION}");
    Console.WriteLine($"  SystemF  = {ntv2.GlobalHeader.SYSTEM_F}");
    Console.WriteLine($"  SystemT  = {ntv2.GlobalHeader.SYSTEM_T}");
    Console.WriteLine($"  MajorF   = {ntv2.GlobalHeader.MAJOR_F}");
    Console.WriteLine($"  MinorF   = {ntv2.GlobalHeader.MINOR_F}");
    Console.WriteLine($"  MajorT   = {ntv2.GlobalHeader.MAJOR_F}");
    Console.WriteLine($"  MinorT   = {ntv2.GlobalHeader.MINOR_T}");

    for (int i = 0; i < ntv2.SubGrids.Count; i++)
    {
        var sg = ntv2.SubGrids[i];
        Console.WriteLine($"\n== Sub-Grid #{i + 1} ==");
        Console.WriteLine($"  Name     = {sg.Header.SUB_NAME}");
        Console.WriteLine($"  Parent   = {sg.Header.PARENT}");
        Console.WriteLine($"  Created  = {sg.Header.CREATED}");
        Console.WriteLine($"  Updated  = {sg.Header.UPDATED}");
        Console.WriteLine($"  SouthLat = {sg.Header.S_LAT}");
        Console.WriteLine($"  NorthLat = {sg.Header.N_LAT}");
        Console.WriteLine($"  EastLong = {sg.Header.E_LONG}");
        Console.WriteLine($"  WestLong = {sg.Header.W_LONG}");
        Console.WriteLine($"  LatInc   = {sg.Header.LAT_INC}");
        Console.WriteLine($"  LongInc  = {sg.Header.LONG_INC}");
        Console.WriteLine($"  GsCount  = {sg.Header.GS_COUNT}");
        Console.WriteLine($"  -> #Points read: {sg.Points.Count}");
    }
}

Console.ReadLine();