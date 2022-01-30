using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Proj4Net.Core.Tests.IO
{
    /// <summary>
    /// Reads a file in MetaCRS Test format
    /// into a list of <see cref="MetaCRSTestCase"/>.
    /// This format is a CSV file with a standard set of columns.
    /// Each record defines a transformation from one coordinate system
    /// to another.
    /// For full details on the file format, see http://trac.osgeo.org/metacrs/
    /// </summary>
    /// <author>Martin Davis</author>
    public class MetaCRSTestFileReader
    {
        public const int ColumnCount = 19;

        private readonly Stream _file;
        private readonly CsvRecordParser _lineParser = new CsvRecordParser();

        public MetaCRSTestFileReader(Stream file)
        {
            _file = file;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="IOException"></exception>
        public IEnumerable<MetaCRSTestCase> ReadTests()
        {
            List<MetaCRSTestCase> tests;
            using (var lineReader = new StreamReader(_file))
            {
                try
                {
                    tests = ParseFile(lineReader);
                }
                finally
                {
                }
            }
            return tests;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lineReader"></param>
        /// <returns></returns>
        /// <exception cref="IOException"/>
        private List<MetaCRSTestCase> ParseFile(TextReader lineReader)
        {
            var tests = new List<MetaCRSTestCase>();
            var isHeaderRead = false;
            while (true)
            {
                var line = lineReader.ReadLine();
                if (line == null)
                    break;
                // skip comments
                if (line.StartsWith("#"))
                    continue;
                // skip header
                if (!isHeaderRead)
                {
                    // TODO: validate header line to have correct set of columns
                    isHeaderRead = true;
                    continue;
                }
                tests.Add(ParseTest(line));
            }
            return tests;
        }

        private MetaCRSTestCase ParseTest(String line)
        {
            var cols = _lineParser.Parse(line);
            if (cols.Length != ColumnCount)
                throw new InvalidDataException("Expected " + ColumnCount + " columns in _file, but found " + cols.Length);
            var testName = cols[0];
            var testMethod = cols[1];
            var srcCrsAuth = cols[2];
            var srcCrs = cols[3];
            var tgtCrsAuth = cols[4];
            var tgtCrs = cols[5];
            var srcOrd1 = ParseNumber(cols[6]);
            var srcOrd2 = ParseNumber(cols[7]);
            var srcOrd3 = ParseNumber(cols[8]);
            var tgtOrd1 = ParseNumber(cols[9]);
            var tgtOrd2 = ParseNumber(cols[10]);
            var tgtOrd3 = ParseNumber(cols[11]);
            var tolOrd1 = ParseNumber(cols[12]);
            var tolOrd2 = ParseNumber(cols[13]);
            var tolOrd3 = ParseNumber(cols[14]);
            var susing = cols[15];
            var dataSource = cols[16];
            var dataCmnts = cols[17];
            var maintenanceCmnts = cols[18];

            return new MetaCRSTestCase(testName, testMethod, srcCrsAuth, srcCrs, tgtCrsAuth, tgtCrs, srcOrd1, srcOrd2,
                                       srcOrd3, tgtOrd1, tgtOrd2, tgtOrd3, tolOrd1, tolOrd2, tolOrd3, susing, dataSource,
                                       dataCmnts, maintenanceCmnts);
        }

        /// <summary>
        /// Parses a number from a String.
        /// If the string is empty returns <see cref="Double.NaN"/>.
        /// </summary>
        /// <param name="numStr">The number string</param>
        /// <returns>The number</returns>
        /// <returns><see cref="Double.NaN"/> if the <paramref name="numStr"/> is null or empty</returns>
        /// <exception cref="FormatException"> thrown if the number can't be parsed.</exception>
        private static double ParseNumber(String numStr)
        {
            if (String.IsNullOrEmpty(numStr))
            {
                return Double.NaN;
            }
            return Double.Parse(numStr, CultureInfo.InvariantCulture);
        }
    }
}