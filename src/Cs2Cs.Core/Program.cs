using Proj4Net;

string from = String.Empty, to = String.Empty, coords = String.Empty;
CoordinateReferenceSystemFactory crsFactory = new CoordinateReferenceSystemFactory();
CoordinateTransformFactory ctFactory = new CoordinateTransformFactory();

for (int i = 0; i < args.Length - 1; i++)
{
    switch(args[i])
    {
        case "-f":
        case "--from":
            from = args[++i];
            break;
        case "-t":
        case "--to":
            to = args[++i];
            break;
        case "-c":
        case "--coords":
            coords = args[++i];
            break;
    }
}

if (String.IsNullOrEmpty(from) || String.IsNullOrEmpty(to))
{
    Console.WriteLine("Usage:");
    Console.WriteLine("cs2cs.core.exe --from <source-coordssys> --to <target-coordsys> [--coords <x,y>]");
    Console.WriteLine("Examples:");
    Console.WriteLine("cs2cs.core.exe --from EPSG:4326 --to EPSG:31256 --coords 15,47");
    Console.WriteLine("cs2cs.core.exe -f \"+proj=longlat +ellps=clrk66 +towgs84=-10,158,187,0,0,0,0 +no_defs \" -to ...");
    return;
}

var sourceCRS = CreateCRS(from);  // "EPSG:4326", "+proj=longlat +datum=WGS84 +no_defs"
var targetCRS = CreateCRS(to);

var transform = ctFactory.CreateTransform(sourceCRS, targetCRS);
var interactive = String.IsNullOrEmpty(coords);

if(interactive)
{
    Console.WriteLine("input coordinates:");
    Console.WriteLine("examples:");
    Console.WriteLine(">> 15 47");
    Console.WriteLine(">> 15.12 48.1");
    Console.WriteLine(">> 15.11,47.3");
}

while (true)
{
    if (interactive)
    {
        Console.Write(">> ");
        coords = Console.ReadLine() ?? String.Empty;
        if (coords == "exit")
        {
            break;
        }
    }

    try
    {
        var coordinate = ParseCoodinateString(coords);

        var sourceCoord = new ProjCoordinate(coordinate.x, coordinate.y);
        var targetCoords = new ProjCoordinate();

        transform.Transform(sourceCoord, targetCoords);

        Console.WriteLine(targetCoords);
    }
    catch
    {
        Console.WriteLine($"Sorry, can't perform transformation. Wrong input? { coords }");
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
    if(coords.Length>=2)
    {
        return new(double.Parse(coords[0], System.Globalization.NumberStyles.Any),
                   double.Parse(coords[1], System.Globalization.NumberStyles.Any));
    }

    throw new Exception("Invalid coordinate string");
}

#endregion
