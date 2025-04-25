using Proj4Net.Core.Abstraction;
using Proj4Net.Core.Datum.Grids;
using Proj4Net.Core.Datum.ShiftTransforms;
using System;
using System.Collections.Generic;
using System.IO;

namespace Proj4Net.Core.Datum;
internal class DatumShiftTransformationFactory
{
    private static readonly object DatumShiftTransformationLoadLock = new object();
    private static readonly Dictionary<string, IDatumShiftTransformation> _shiftTransformations = new();

    internal static IDatumShiftTransformation Load(string grid)
    {
        var gridOptional = grid.StartsWith("@");

        IDatumShiftTransformation datumShiftTransformation = null;

        if (!_shiftTransformations.TryGetValue(grid, out datumShiftTransformation))
        {
            lock (DatumShiftTransformationLoadLock)
            {
                if (!_shiftTransformations.TryGetValue(grid, out datumShiftTransformation))
                {
                    var gridName = gridOptional ? grid.Substring(1) : grid;

                    #region direct ellipsod transformation: eg. ellips:GRS80 (for ETRS89)

                    if (gridName.StartsWith("ellps:"))
                    {
                        string ellipsoidName = gridName.Substring("ellps:".Length);

                        datumShiftTransformation = CoordinateTransformShift.Create(grid, $"+proj=longlat +ellps={ellipsoidName} +datum +towgs84=0,0,0,0,0,0,0");
                        _shiftTransformations.Add(grid, datumShiftTransformation);

                        return datumShiftTransformation;
                    }

                    #endregion

                    var location = new Uri(System.IO.Path.Combine(IO.Paths.PROJ_LIB, gridName));

                    
                    if (!location.IsFile)
                    {
                        // TODO: Load from an url?? https://epsg.io??
                        _shiftTransformations.Add(grid, datumShiftTransformation);

                        return datumShiftTransformation;
                    }

                    var ext = Path.GetExtension(location.LocalPath)?.ToLowerInvariant() ?? string.Empty;

                    if (ext == ".proj")
                    {
                        datumShiftTransformation = CoordinateTransformShift.Create(grid, File.ReadAllText(location.LocalPath).Trim());

                        _shiftTransformations.Add(grid, datumShiftTransformation);

                        return datumShiftTransformation;
                    }

                    #region Grid File

                    Console.WriteLine($"Load datum shift transformation '{grid}': {location}");

                    GridTableLoader loader;

                    switch (ext)
                    {
                        case "":
                        case ".lla":
                            loader = new LlaGridTableLoader(location);
                            break;
                        case ".gsb":
                            loader = new GsbGridTableLoader(location);
                            break;
                        case ".dat":
                            loader = new DatGridTableLoader(location);
                            break;
                        case ".los":
                            loader = new LasLosGridTableLoader(location);
                            break;
                        default:
                            throw new ArgumentException();
                    }

                    datumShiftTransformation = new GridTable(loader);

                    Console.WriteLine($"Load atum shift transformation '{grid}' succeeded");

                    _shiftTransformations.Add(grid, datumShiftTransformation);
                    return datumShiftTransformation;

                    #endregion
                }
            }
        }

        return datumShiftTransformation;
    }
}
