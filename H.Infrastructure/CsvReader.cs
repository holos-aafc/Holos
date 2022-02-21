#region Imports

using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

#endregion

namespace H.Infrastructure
{
    /// <summary>
    /// This custom "CsvReader" class is used to deal with csv files that have commas within quotes i.e. \"Barley, Dry\".
    /// Splitting on commas would cause problems in these cases. The "CsvReader" deals
    /// with these cases nicely.
    /// </summary>
    public sealed class CsvReader : IDisposable
    {
        private static readonly Regex rexCsvSplitter = new Regex(@",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))");
        private static readonly Regex rexRunOnLine = new Regex(@"^[^""]*(?:""[^""]*""[^""]*)*""[^""]*$");
        private readonly TextReader __reader;

        //============================================


        public CsvReader(string fileName)
            : this(new FileStream(fileName, FileMode.Open, FileAccess.Read))
        {
        }

        public CsvReader(Stream stream)
        {
            __reader = new StreamReader(stream);
        }

        public IEnumerable RowEnumerator
        {
            get
            {
                if (null == __reader)
                {
                    throw new ApplicationException("I can't start reading without CSV input.");
                }

                this.RowIndex = 0;
                string sLine;
                string sNextLine;

                while (null != (sLine = __reader.ReadLine()))
                {
                    while (rexRunOnLine.IsMatch(sLine) && null != (sNextLine = __reader.ReadLine()))
                        sLine += "\n" + sNextLine;

                    this.RowIndex++;
                    var values = rexCsvSplitter.Split(sLine);

                    for (var i = 0; i < values.Length; i++)
                    {
                        values[i] = Csv.Unescape(values[i]);
                    }

                    yield return values;
                }

                __reader.Close();
            }
        }

        public long RowIndex { get; private set; }

        public void Dispose()
        {
            if (null != __reader)
            {
                __reader.Dispose();
            }
        }
    }
}