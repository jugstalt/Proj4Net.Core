using System;
using System.Collections.Generic;
using System.Text;

namespace Proj4Net.Tests.IO
{
    public class CsvRecordParser
    {
        private enum SpecialCharacters
        {
            Quote = 1,
            WhiteSpace = 2,
            Data = 3,
            Separator = 4,
            Eol = 5
        }

        private enum States
        {
            Data = 1,
            Before = 2,
            QuotedData = 3,
            SeenQuote = 4,
            After = 5
        }

        //private static final String[] strArrayType = new String[0];
        private const char Quote = '"';
        private const char Separator = ',';

        private int _loc;

        private Boolean _isStrictMode;

        /// <summary>
        /// Controls whether the parsing strictly follows the CSV specification.
        /// If not in strict mode:
        /// <list type="Bulltet">
        /// <item>quotes which occur in the middle of fields are simply scanned as data</item>
        /// </list>
        /// </summary>
        public Boolean StrictMode
        {
            get { return _isStrictMode; }
            set { _isStrictMode = value; }
        }

        ///<summary>Creates a new parser, which is not in <see cref="StrictMode"/></summary>
        public CsvRecordParser()
            : this(false)
        {
        }

        /// <summary>
        /// Creates a new parser
        /// </summary>
        /// <param name="strictMode">Sets <see cref="StrictMode"/> of parser</param>
        public CsvRecordParser(Boolean strictMode)
        {
            _isStrictMode = strictMode;
        }

        ///<summary>
        /// Parses a single record of a CSV file.
        ///</summary>
        /// <param name="record">record</param>
        /// <returns>An array of the fields in the record</returns>
        /// <exception cref="ArgumentException">If parsing fails</exception>
        public String[] Parse(String record)
        {
            _loc = 0;
            List<String> vals = new List<String>();
            int lineLen = record.Length;
            while (_loc < lineLen)
            {
                vals.Add(ParseField(record));
            }
            return vals.ToArray();
        }

        private String ParseField(String line)
        {
            StringBuilder data = new StringBuilder();

            States state = States.Before;
            while (true)
            {
                SpecialCharacters category = SpecialCharacters.Eol;
                if (_loc < line.Length)
                    category = Categorize(line[_loc]);

                switch (state)
                {
                    case States.Before:
                        switch (category)
                        {
                            case SpecialCharacters.WhiteSpace:
                                _loc++;
                                break;
                            case SpecialCharacters.Quote:
                                _loc++;
                                state = States.QuotedData;
                                break;
                            case SpecialCharacters.Separator:
                                _loc++;
                                return "";
                            case SpecialCharacters.Data:
                                data.Append(line[_loc]);
                                state = States.Data;
                                _loc++;
                                break;
                            case SpecialCharacters.Eol:
                                return null;
                        }
                        break;
                    case States.Data:
                        switch (category)
                        {
                            case SpecialCharacters.Separator:
                            case SpecialCharacters.Eol:
                                _loc++;
                                return data.ToString();
                            case SpecialCharacters.Quote:
                                if (_isStrictMode)
                                    throw new ArgumentException("Malformed field - quote not at beginning of field");
                                data.Append(line[_loc]);
                                _loc++;
                                break;
                            case SpecialCharacters.WhiteSpace:
                            case SpecialCharacters.Data:
                                data.Append(line[_loc]);
                                _loc++;
                                break;
                        }
                        break;
                    case States.QuotedData:
                        switch (category)
                        {
                            case SpecialCharacters.Quote:
                                _loc++;
                                state = States.SeenQuote;
                                break;
                            case SpecialCharacters.Separator:
                            case SpecialCharacters.WhiteSpace:
                            case SpecialCharacters.Data:
                                data.Append(line[_loc]);
                                _loc++;
                                break;
                            case SpecialCharacters.Eol:
                                return data.ToString();
                        }
                        break;
                    case States.SeenQuote:
                        switch (category)
                        {
                            case SpecialCharacters.Quote:
                                // double quote - add to value
                                _loc++;
                                data.Append('"');
                                state = States.QuotedData;
                                break;
                            case SpecialCharacters.Separator:
                            case SpecialCharacters.Eol:
                                // at end of field
                                _loc++;
                                return data.ToString();
                            case SpecialCharacters.WhiteSpace:
                                _loc++;
                                state = States.After;
                                break;
                            case SpecialCharacters.Data:
                                throw new ArgumentException("Malformed field - quote not at end of field");
                        }
                        break;
                    case States.After:
                        switch (category)
                        {
                            case SpecialCharacters.Quote:
                                throw new ArgumentException("Malformed field - unexpected quote");
                            case SpecialCharacters.Eol:
                            case SpecialCharacters.Separator:
                                // at end of field
                                _loc++;
                                return data.ToString();
                            case SpecialCharacters.WhiteSpace:
                                // skip trailing whitespace
                                _loc++;
                                break;
                            case SpecialCharacters.Data:
                                throw new ArgumentException("Malformed field - unexpected data after quote");
                        }
                        break;
                }
            }
        }

        ///<summary>
        /// Categorizes a character into a lexical category.
        /// </summary>
        /// <param name="c">The character to categorize</param>
        /// <returns>The lexical category</returns>
        private static SpecialCharacters Categorize(char c)
        {
            switch (c)
            {
                case ' ':
                case '\r':
                case (char)0xff:
                case '\n':
                    return SpecialCharacters.WhiteSpace;
                default:
                    if (c == Quote)
                        return SpecialCharacters.Quote;

                    if (c == Separator)
                        return SpecialCharacters.Separator;

                    if ('!' <= c && c <= '~')
                        return SpecialCharacters.Data;

                    if (0x00 <= c && c <= 0x20)
                        return SpecialCharacters.WhiteSpace;

                    if (char.IsWhiteSpace(c))
                        return SpecialCharacters.WhiteSpace;

                    return SpecialCharacters.Data;
            }
        }
    }
}