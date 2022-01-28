using System.IO;
using System.Reflection;

namespace Proj4Net.Tests.Resources
{
    public static class Repository
    {
        private static readonly Assembly _asm = Assembly.GetExecutingAssembly();

        public static Stream Open(string resourceName)
        {
            var mrs = "Proj4Net.Tests.Resources." + resourceName;
            var s = _asm.GetManifestResourceStream(mrs);
            if (s == null)
                return new MemoryStream();
            using (var sr = new BinaryReader(s))
            {
                return new MemoryStream(sr.ReadBytes((int) s.Length));
            }
        }
    }
}
