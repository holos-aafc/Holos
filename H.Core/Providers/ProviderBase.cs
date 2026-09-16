using H.Infrastructure;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace H.Core.Providers
{
    public abstract class ProviderBase
    {
        #region Properties
        public bool IsInitialized { get; set; } 
        #endregion

        #region Public Methods

        public double ParseDouble(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return 0;
            }

            var culture = InfrastructureConstants.EnglishCultureInfo;

            var result = double.Parse(value, culture);

            return result;
        }

        public int ParseInt(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return 0;
            }

            var culture = InfrastructureConstants.EnglishCultureInfo;

            var result = int.Parse(value, culture);

            return result;
        }

        /// <summary>
        /// True when a row of a csv file holds nothing - every cell is empty or whitespace. Spacer rows are common
        /// in these files, where they separate groups of data, and a reader is expected to skip them.
        /// </summary>
        /// <param name="line">A row as the csv reader splits it. A null or empty row counts as blank.</param>
        public bool IsBlank(string[] line)
        {
            return line == null || line.All(string.IsNullOrWhiteSpace);
        }

        /// <summary>
        /// True when a row of a csv file is a comment - its first cell starts with '#'. A file can then carry its
        /// own citation and editing notes for whoever opens it, which a reader is expected to skip along with blank
        /// rows. The same marker is used for the settings files the command line interface reads.
        /// </summary>
        /// <param name="line">A row as the csv reader splits it. A null or empty row is not a comment.</param>
        public bool IsComment(string[] line)
        {
            if (line == null || line.Length == 0 || string.IsNullOrWhiteSpace(line[0]))
            {
                return false;
            }

            return line[0].TrimStart().StartsWith("#");
        }

        #endregion
    }
}