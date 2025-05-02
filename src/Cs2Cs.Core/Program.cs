using Proj4Net.Core;
using RTools.Util;

string from = String.Empty, to = String.Empty, coords = String.Empty;
CoordinateReferenceSystemFactory crsFactory = new CoordinateReferenceSystemFactory();
CoordinateTransformFactory ctFactory = new CoordinateTransformFactory();

#if DEBUG
Logger.Log.Verbosity = VerbosityLevel.Debug;
#else
Logger.Log.Verbosity = VerbosityLevel.Info;
#endif

for (int i = 0; i < args.Length; i++)
{
    switch (args[i])
    {
        case "-f":
        case "--from":
            from = args.Length > i ? args[++i] : "";
            break;
        case "-t":
        case "--to":
            to = args.Length > i ? args[++i] : "";
            break;
        case "-c":
        case "--coords":
            coords = args.Length > i ? args[++i] : "";
            break;
        case "-v":
        case "--version":
            PrintVersion();
            return;
    }
}

if (String.IsNullOrEmpty(from) || String.IsNullOrEmpty(to))
{
    Console.WriteLine("Usage:");
    Console.WriteLine("cs2cs.core.exe --from <source-coordssys> --to <target-coordsys> [--coords <x,y>]");
    Console.WriteLine();
    Console.WriteLine("Examples:");
    Console.WriteLine("cs2cs.core.exe --from EPSG:4326 --to EPSG:31256 --coords 15,47");
    Console.WriteLine("cs2cs.core.exe -f \"+proj=longlat +ellps=clrk66 +towgs84=-10,158,187,0,0,0,0 +no_defs \" -to ...");
    Console.WriteLine("");
    Console.WriteLine("cs2cs.core.exe --version");
    Console.WriteLine("Shows Proj4Net.Core Version");   

    return;
}

var sourceCRS = CreateCRS(from);  // "EPSG:4326", "+proj=longlat +datum=WGS84 +no_defs"
var targetCRS = CreateCRS(to);

var transform = ctFactory.CreateTransform(sourceCRS, targetCRS);
var invTransform = ctFactory.CreateTransform(targetCRS, sourceCRS);
var interactive = String.IsNullOrEmpty(coords);

if (interactive)
{
    Console.WriteLine($"From: {sourceCRS.GetParameterString()}");
    Console.WriteLine($"To: {targetCRS.GetParameterString()}");
    Console.WriteLine("input coordinates:");
    Console.WriteLine("examples:");
    Console.WriteLine(">> 15 47");
    Console.WriteLine(">> 15.12 48.1");
    Console.WriteLine(">> 15.11,47.3");
    Console.WriteLine(">> exit  => ends program");
    Console.WriteLine(">> version  => show Proj4Net.Core Version");
    Console.WriteLine();
    Console.WriteLine();
}

while (true)
{
    if (interactive)
    {
        Console.Write(">> ");
        coords = Console.ReadLine() ?? String.Empty;
        if ("exit".Equals(coords, StringComparison.OrdinalIgnoreCase))
        {
            break;
        }

        if("version".Equals(coords, StringComparison.OrdinalIgnoreCase))
        {
            PrintVersion();
            continue;
        }
    }

    try
    {
        bool inverse = false;
        coords = coords.Trim();
        if (coords.StartsWith("!"))
        {
            coords = coords.Substring(1).Trim();
            inverse = true;
        }

        var coordinate = ParseCoodinateString(coords);

        var sourceCoord = new Coordinate(coordinate.x, coordinate.y);
        var targetCoords = new Coordinate();


        targetCoords = inverse
            ? invTransform.Transform(sourceCoord)
            : transform.Transform(sourceCoord);

        Console.WriteLine(targetCoords);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Sorry, can't perform transformation.");
        Console.WriteLine(ex.Message);
#if DEBUG
        Console.WriteLine("Stacktrace:");
        Console.WriteLine(ex.StackTrace);
#endif
    }

    if (!interactive)
    {
        break;
    }
}

#region Functions

CoordinateReferenceSystem CreateCRS(String crsSpec)
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

(double x, double y) ParseCoodinateString(string coordString)
{
    var coords = coordString.Trim().Replace(" ", ",").Replace(";", ",").Split(',');
    if (coords.Length >= 2)
    {
        return new(double.Parse(coords[0], System.Globalization.CultureInfo.InvariantCulture),
                   double.Parse(coords[1], System.Globalization.CultureInfo.InvariantCulture));
    }

    throw new Exception("Invalid coordinate string");
}

void PrintVersion()
{
    Console.WriteLine($"Proj4Net.Core Version: {typeof(CoordinateReferenceSystem).Assembly.GetName().Version}");
}

#endregion
