﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using H.Infrastructure;

namespace H.Core.Providers.Soil
{
    /// <summary>
    ///     Reads a user-specified input file containing yield data
    /// </summary>
    public class CustomFileYieldProvider : ICustomFileYieldProvider
    {
        #region Fields

        private readonly Dictionary<string, List<CustomUserYieldData>> _cache;

        #endregion

        #region Constructors

        public CustomFileYieldProvider()
        {
            _cache = new Dictionary<string, List<CustomUserYieldData>>();
        }

        #endregion

        #region Public Methods

        public bool HasExpectedInputFormat(string filename)
        {
            IEnumerable<string[]> lines;

            try
            {
                lines = from line in File.ReadAllLines(filename)
                    select line.Split(',');
            }
            catch (Exception e)
            {
                Trace.TraceError(
                    $"{nameof(CustomFileYieldProvider)}.{nameof(HasExpectedInputFormat)}. Error reading input file: {e}");

                return false;
            }

            var fileHeaders =
                from word in lines.First()
                select word.ToLower().Replace(" ", string.Empty);

            var fileHeaderStrings = fileHeaders.ToList();
            if (fileHeaderStrings[0] == "year" && fileHeaderStrings[1].Contains("-"))
            {
                //the first two columns are perhaps formatted correctly
                foreach (var header in fileHeaderStrings.Skip(2))
                    //Any position will technically work we need to be more specific about where it is
                    if (!header.Contains("-"))
                        return false;

                return true;
            }

            return false;
        }

        // Change to new format
        public List<string> GetExpectedFileHeaderList()
        {
            return new List<string> { "year" };
        }

        public List<CustomUserYieldData> ParseLines(IEnumerable<string[]> lines)
        {
            var customUserYieldData = new List<CustomUserYieldData>();
            var cultureInfo = InfrastructureConstants.EnglishCultureInfo;

            // Determine how many columns we have.
            var firstLine = lines.ElementAt(0);

            // First column is for the year, the rest of the columns are for each field of each rotation
            var fieldOfRotation = firstLine.Count() - 1;
            var fieldNames = new List<string>();
            var rotationNames = new List<string>();
            for (var i = 1; i <= fieldOfRotation; i++)
            {
                var delimiter = firstLine[i].IndexOf("-");
                fieldNames.Add(firstLine[i].Substring(0, delimiter));
                rotationNames.Add(firstLine[i].Substring(delimiter + 1));
            }

            foreach (var line in lines.Skip(1))
            {
                if (line.All(string.IsNullOrWhiteSpace)) continue;

                var year = int.Parse(line[0], cultureInfo);
                for (var i = 1; i < line.Count(); i++)
                {
                    var yieldString = line[i];
                    var yield = 0d;
                    if (string.IsNullOrWhiteSpace(yieldString) == false)
                        yield = double.Parse(yieldString, cultureInfo);
                    else
                        yield = 0;

                    var entry = new CustomUserYieldData
                    {
                        Year = year,
                        FieldName = fieldNames.ElementAt(i - 1),
                        Yield = Math.Round(yield, 1),
                        RotationName = rotationNames.ElementAt(i - 1)
                    };

                    customUserYieldData.Add(entry);
                }
            }

            return customUserYieldData;
        }

        public List<CustomUserYieldData> GetYieldData(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                Trace.TraceError(
                    $"{nameof(CustomFileYieldProvider)}.{nameof(GetYieldData)}: file path cannot be empty.");

                return new List<CustomUserYieldData>();
            }

            if (IsCached(filePath)) return GetCachedData(filePath);

            IEnumerable<string[]> lines;
            lines = File.ReadAllLines(filePath).Select(x => x.Split(','));

            var result = ParseLines(lines);

            CacheData(filePath, result);

            return result;
        }

        #endregion

        #region Private Methods

        private bool IsCached(string filePath)
        {
            return GetCachedData(filePath).Any();
        }

        private List<CustomUserYieldData> GetCachedData(string filePath)
        {
            if (_cache.ContainsKey(filePath)) return _cache[filePath];

            return new List<CustomUserYieldData>();
        }

        private void CacheData(string filePath, List<CustomUserYieldData> customUserYieldDatas)
        {
            if (_cache.ContainsKey(filePath))
                _cache[filePath] = customUserYieldDatas;
            else
                _cache.Add(filePath, customUserYieldDatas);

            Trace.TraceInformation(
                $"{nameof(CustomFileYieldProvider)}.{nameof(CacheData)}: yield data file was cached");
        }

        #endregion
    }
}