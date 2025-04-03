using System.IO;
using System.Reflection;

namespace Proj4Net.Core.IO
{
    public class Paths
    {
        static public string GridRootPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "grids");
    }
}
