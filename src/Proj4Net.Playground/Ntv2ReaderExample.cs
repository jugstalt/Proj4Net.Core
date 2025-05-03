using System.Text;

public class Ntv2File
{
    public GlobalHeader GlobalHeader { get; set; }
    public List<SubGrid> SubGrids { get; set; } = new List<SubGrid>();
}

public class GlobalHeader
{
    public string NUM_OREC { get; set; }
    public string NUM_SREC { get; set; }
    public int NUM_FILE { get; set; }
    public string GS_TYPE { get; set; }
    public string VERSION { get; set; }
    public string SYSTEM_F { get; set; }
    public string SYSTEM_T { get; set; }
    public int MAJOR_F { get; set; }
    public int MINOR_F { get; set; }
    public int MAJOR_T { get; set; }
    public int MINOR_T { get; set; }
}

public class SubGridHeader
{
    public string SUB_NAME { get; set; }
    public string PARENT { get; set; }
    public string CREATED { get; set; }
    public string UPDATED { get; set; }
    public double S_LAT { get; set; }
    public double N_LAT { get; set; }
    public double E_LONG { get; set; }
    public double W_LONG { get; set; }
    public double LAT_INC { get; set; }
    public double LONG_INC { get; set; }
    public long GS_COUNT { get; set; }
}

public class SubGrid
{
    public SubGridHeader Header { get; set; } = new SubGridHeader();
    public List<Ntv2GridPoint> Points { get; set; } = new List<Ntv2GridPoint>();
}

public class Ntv2GridPoint
{
    public double LatShift { get; set; }
    public double LonShift { get; set; }
    public double LatAccuracy { get; set; }
    public double LonAccuracy { get; set; }
}

public static class Ntv2Reader
{
    /// <summary>
    /// Liest das File, das zuerst "Key" (8 Byte ASCII) und "Value" (8 Byte ASCII/Double) enthält.
    /// </summary>
    public static Ntv2File ReadFile(string filePath)
    {
        Ntv2File result = new Ntv2File();
        using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        using (var br = new BinaryReader(fs))
        {
            // 1) Global Header einlesen:
            result.GlobalHeader = ReadGlobalHeader(br);

            // 2) Anzahl Sub-Grids aus GlobalHeader ermitteln
            int subGridCount = result.GlobalHeader.NUM_FILE;

            // 3) Sub-Grids einlesen
            for (int i = 0; i < subGridCount; i++)
            {
                var sub = new SubGrid();
                // Sub-Grid-Header
                sub.Header = ReadSubGridHeader(br);

                // Dann Gitterpunkte einlesen
                // "GS_COUNT" => Anzahl der Punkte
                long gsCount = sub.Header.GS_COUNT;

                // Prüfe, wie viele Werte pro Zeile => "NUM_SREC"
                // Falls wir im globalen Header oder Sub-Grid-Header noch so was finden,
                // kann man es anpassen. Ansonsten 4 Double-Werte annehmen
                int doublesPerPoint = 4;
                long bytesPerPoint = doublesPerPoint * 8;

                for (long g = 0; g < gsCount; g++)
                {
                    Ntv2GridPoint p = new Ntv2GridPoint
                    {
                        LatShift = br.ReadSingle(),
                        LonShift = br.ReadSingle(),
                        LatAccuracy = br.ReadSingle(),
                        LonAccuracy = br.ReadSingle()
                    };
                    sub.Points.Add(p);
                }

                result.SubGrids.Add(sub);
            }
        }

        return result;
    }

    /// <summary>
    /// Liest den GlobalHeader, bei dem pro 8-Byte-Abschnitt "Key" und "Value" kommen.
    /// Wir lesen so viele Felder, bis wir unsere 11 Standard-Felder haben.
    /// </summary>
    private static GlobalHeader ReadGlobalHeader(BinaryReader br)
    {
        GlobalHeader gh = new GlobalHeader();
        // Wir wissen, dass der "klassische" NTv2 Global Header 11 Felder hat.
        // In dieser Variation hat jedes Feld: 8 Byte Key + 8 Byte Value = 16 Byte pro Feld
        // => 11 * 16 Byte = 176 Byte
        // Aber wir müssen ggf. dynamisch erkennen.
        // Hier: wir lesen 11 Felder in einer Schleife:

        for (int i = 0; i < 11; i++)
        {
            string key = ReadAscii(br, 8);
            byte[] valueBytes = br.ReadBytes(8);

            AssignGlobalHeaderValue(gh, key.Trim(), valueBytes);
        }
        return gh;
    }

    /// <summary>
    /// Ordnet den gelesenen Value dem passenden Feld im GlobalHeader zu.
    /// </summary>
    private static void AssignGlobalHeaderValue(GlobalHeader gh, string key, byte[] valueBytes)
    {
        // Manche Felder sind ASCII (z.B. NUM_OREC), andere Double (z.B. MAJOR_F).
        // Wir entscheiden basierend auf dem Feldnamen:
        switch (key)
        {
            case "NUM_OREC":
            case "NUM_SREC":
            case "GS_TYPE":
            case "VERSION":
            case "SYSTEM_F":
            case "SYSTEM_T":
                // ASCII-Feld
                gh.GetType().GetProperty(key, typeof(string))?
                  .SetValue(gh, Encoding.ASCII.GetString(valueBytes).Trim());
                break;
            case "NUM_FILE":
            case "MAJOR_F":
            case "MINOR_F":
            case "MAJOR_T":
            case "MINOR_T":
                var ival = BitConverter.ToInt32(valueBytes, 0);
                gh.GetType().GetProperty(key, typeof(int))?
                  .SetValue(gh, ival);
                break;
            case "_":
                // Double-Feld
                double dval = BitConverter.ToDouble(valueBytes, 0);
                gh.GetType().GetProperty(key, typeof(double))?
                  .SetValue(gh, dval);
                break;

            default:
                // Unbekannt -> ignoriere oder logge
                Console.WriteLine($"Unbekannter Key im GlobalHeader: {key}");
                break;
        }
    }

    private static SubGridHeader ReadSubGridHeader(BinaryReader br)
    {
        // Analog: 11 Felder * (8+8 Byte)
        SubGridHeader sh = new SubGridHeader();
        for (int i = 0; i < 11; i++)
        {
            string key = ReadAscii(br, 8);
            byte[] val = br.ReadBytes(8);
            AssignSubGridHeaderValue(sh, key.Trim(), val);
        }
        return sh;
    }

    private static void AssignSubGridHeaderValue(SubGridHeader sh, string key, byte[] valueBytes)
    {
        switch (key)
        {
            case "SUB_NAME":
            case "PARENT":
            case "CREATED":
            case "UPDATED":
                sh.GetType().GetProperty(ToPropertyName(key), typeof(string))?
                  .SetValue(sh, Encoding.ASCII.GetString(valueBytes).Trim());
                break;

            case "S_LAT":
            case "N_LAT":
                double d1 = BitConverter.ToDouble(valueBytes, 0);
                sh.GetType().GetProperty(key, typeof(double))?
                  .SetValue(sh, d1 / 3600);
                break;
            case "E_LONG":
            case "W_LONG":
                double d2 = BitConverter.ToDouble(valueBytes, 0);
                sh.GetType().GetProperty(key, typeof(double))?
                  .SetValue(sh, -d2 / 3600);
                break;
            case "LAT_INC":
            case "LONG_INC":
                double d = BitConverter.ToDouble(valueBytes, 0);
                sh.GetType().GetProperty(key, typeof(double))?
                  .SetValue(sh, d / 3600);
                break;
            case "GS_COUNT":
                long l = BitConverter.ToInt64(valueBytes, 0);
                sh.GetType().GetProperty(key, typeof(long))?
                  .SetValue(sh, l);
                break;
            default:
                Console.WriteLine($"Unbekannter Key im SubGridHeader: {key}");
                break;
        }
    }

    /// <summary>
    /// Kleine Hilfsmethode, um z.B. "S_LAT" auf "SouthLat" zu mappen. 
    /// Du kannst es anpassen wie du möchtest. 
    /// </summary>
    private static string ToPropertyName(string key)
    {
        switch (key)
        {
            case "SUB_NAME": return "SubName";
            case "PARENT": return "Parent";
            case "CREATED": return "Created";
            case "UPDATED": return "Updated";
            case "S_LAT": return "SouthLat";
            case "N_LAT": return "NorthLat";
            case "E_LONG": return "EastLong";
            case "W_LONG": return "WestLong";
            case "LAT_INC": return "LatInc";
            case "LONG_INC": return "LongInc";
            case "GS_COUNT": return "GsCount";
            default: return key; // fallback
        }
    }

    private static string ReadAscii(BinaryReader br, int count)
    {
        return Encoding.ASCII.GetString(br.ReadBytes(count));
    }

    private static int ParseIntSafe(string s)
    {
        if (int.TryParse(s, out int val)) return val;
        return 0;
    }
}