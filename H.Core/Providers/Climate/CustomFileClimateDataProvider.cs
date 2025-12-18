using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using H.Core.Tools;
using H.Infrastructure;

namespace H.Core.Providers.Climate
{
    /// <summary>
    ///     Reads a user-specified input file containing daily climate data
    /// </summary>
    public class CustomFileClimateDataProvider : ICustomFileClimateDataProvider
    {
        #region Constructors

        public CustomFileClimateDataProvider()
        {
            HTraceListener.AddTraceListener();
        }

        #endregion

        #region Methods

        public bool HasExpectedInputFormat(string fileName)
        {
            IEnumerable<string[]> lines;

            try
            {
                lines = from line in File.ReadAllLines(fileName)
                    select line.Split(',');
            }
            catch (Exception e)
            {
                Trace.TraceError(
                    $"{nameof(CustomFileClimateDataProvider)}.{nameof(HasExpectedInputFormat)}. Error reading input file: {e}");

                return false;
            }

            var fileHeaders =
                from word in lines.First()
                select word.ToLower().Replace(" ", string.Empty);

            var fileHeaderList = fileHeaders.ToList();

            // Accept either the original 5-column format or the extended 7-column format (with max/min temperatures)
            var expectedBase = this.GetExpectedFileHeaderList();
            var expectedExtended = this.GetExtendedFileHeaderList();

            if (fileHeaderList.SequenceEqual(expectedBase) == false && fileHeaderList.SequenceEqual(expectedExtended) == false)
            {
                return false;
            }

            return true;
        }

        public List<string> GetExpectedFileHeaderList()
        {
            return new List<string>
                { "year", "julianday", "meandailyairtemperature", "meandailyprecipitation", "meandailypet" };
        }

        public List<string> GetExtendedFileHeaderList()
        {
            return new List<string> { "year", "julianday", "meandailyairtemperature", "maximumairtemperature", "minimumairtemperature", "meandailyprecipitation", "meandailypet" };
        }

        public List<DailyClimateData> ParseFileLines(List<string> lines)
        {
            var result = new List<DailyClimateData>();

            if (lines == null || lines.Count == 0)
            {
                return result;
            }

            // Determine header indices from the first line so we can support both 5- and 7-column files
            var headerTokens = lines[0].Split(',').Select(h => h.ToLower().Replace(" ", String.Empty)).ToList();

            int idxYear = headerTokens.IndexOf("year");
            int idxJulian = headerTokens.IndexOf("julianday");
            int idxMeanTemp = headerTokens.IndexOf("meandailyairtemperature");
            int idxMaxTemp = headerTokens.IndexOf("maximumairtemperature");
            int idxMinTemp = headerTokens.IndexOf("minimumairtemperature");
            int idxPrecip = headerTokens.IndexOf("meandailyprecipitation");
            int idxPET = headerTokens.IndexOf("meandailypet");

            // Skip header row
            for (var i = 1; i < lines.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;

                var tokens = lines[i].Split(',');
                if (tokens.All(x => string.IsNullOrWhiteSpace(x))) continue;

                // helper to safely parse a token by index
                double ParseDoubleAt(int idx)
                {
                    if (idx < 0 || idx >= tokens.Length)
                    {
                        return 0d;
                    }

                    var s = tokens[idx];
                    return string.IsNullOrWhiteSpace(s) ? 0d : double.Parse(s, InfrastructureConstants.EnglishCultureInfo);
                }

                int ParseIntAt(int idx)
                {
                    if (idx < 0 || idx >= tokens.Length)
                    {
                        return 0;
                    }

                    var s = tokens[idx];
                    return string.IsNullOrWhiteSpace(s) ? 0 : int.Parse(s, InfrastructureConstants.EnglishCultureInfo);
                }

                var year = ParseIntAt(idxYear);
                var julianDay = ParseIntAt(idxJulian);

                // Mean temperature: prefer explicit mean if present; otherwise compute from max/min when available
                double temperature = 0d;
                var meanPresent = idxMeanTemp >= 0 && idxMeanTemp < tokens.Length && !string.IsNullOrWhiteSpace(tokens[idxMeanTemp]);
                var maxPresent = idxMaxTemp >= 0 && idxMaxTemp < tokens.Length && !string.IsNullOrWhiteSpace(tokens[idxMaxTemp]);
                var minPresent = idxMinTemp >= 0 && idxMinTemp < tokens.Length && !string.IsNullOrWhiteSpace(tokens[idxMinTemp]);

                if (meanPresent)
                {
                    temperature = ParseDoubleAt(idxMeanTemp);
                }
                else if (maxPresent && minPresent)
                {
                    var maxT = ParseDoubleAt(idxMaxTemp);
                    var minT = ParseDoubleAt(idxMinTemp);
                    temperature = (maxT + minT) / 2.0;
                }

                var maxTemperature = maxPresent ? ParseDoubleAt(idxMaxTemp) : 0d;
                var minTemperature = minPresent ? ParseDoubleAt(idxMinTemp) : 0d;

                var precipitation = idxPrecip >= 0 ? ParseDoubleAt(idxPrecip) : 0d;
                var evapotranspiration = idxPET >= 0 ? ParseDoubleAt(idxPET) : 0d;

                var date = FromJulianDay(julianDay, year);

                result.Add(new DailyClimateData
                {
                    Year = year,
                    Date = date,
                    JulianDay = julianDay,
                    MeanDailyAirTemperature = temperature,
                    MaximumAirTemperature = maxTemperature,
                    MinimumAirTemperature = minTemperature,
                    MeanDailyPrecipitation = precipitation,
                    MeanDailyPET = evapotranspiration
                });
            }

            return result;
        }

        public List<DailyClimateData> ParseText(string text)
        {
            var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();

            return ParseFileLines(lines);
        }

        /// <summary>
        ///     Use by both the CLI, and GUI to load a file that contains custom climate data.
        /// </summary>
        public List<DailyClimateData> GetDailyClimateData(string filePath)
        {
            var lines = new List<string>();
            try
            {
                lines = File.ReadAllLines(filePath).ToList();
            }
            catch (Exception e)
            {
                Trace.TraceError($"{nameof(CustomFileClimateDataProvider)}.{nameof(GetDailyClimateData)}");
                Trace.TraceError($"{e.Message}");
                Trace.TraceError($"{e.InnerException}");

                return new List<DailyClimateData>();
            }

            var dailyClimateData = ParseFileLines(lines);

            return dailyClimateData;
        }

        public static DateTime FromJulianDay(double julianDay, int year)
        {
            var epoch = new DateTime(year, 1, 1);

            return epoch.AddDays(julianDay - 1);
        }

        #endregion
    }
}