using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using H.Core.Models;
using H.Core.Tools;
using H.Infrastructure;

namespace H.Core.Providers.Climate
{
    /// <summary>
    /// Reads a user-specified input file containing daily climate data
    /// </summary>
    public class CustomFileClimateDataProvider : ICustomFileClimateDataProvider
    {
        #region Fields

        #endregion

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
                Trace.TraceError($"{nameof(CustomFileClimateDataProvider)}.{nameof(CustomFileClimateDataProvider.HasExpectedInputFormat)}. Error reading input file: {e.ToString()}");

                return false;
            }

            var fileHeaders =
                from word in lines.First()
                select word.ToLower().Replace(" ", String.Empty);

            if (fileHeaders.SequenceEqual(this.GetExpectedFileHeaderList()) == false)
            {
                return false;
            }

            return true;
        }

        public List<string> GetExpectedFileHeaderList()
        {
            return new List<string> { "year", "julianday", "meandailyairtemperature", "meandailyprecipitation", "meandailypet" };
        }

        public List<DailyClimateData> ParseFileLines(List<string> lines)
        {
            var result = new List<DailyClimateData>();

            // Skip header row
            for (int i = 1; i < lines.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                {
                    continue;
                }

                var tokens = lines[i].Split(',');

                var year = int.Parse(tokens[0], InfrastructureConstants.EnglishCultureInfo);
                var julianDay = int.Parse(tokens[1], InfrastructureConstants.EnglishCultureInfo);
                var temperature = string.IsNullOrWhiteSpace(tokens[2]) ? 0 : double.Parse(tokens[2], InfrastructureConstants.EnglishCultureInfo);
                var precipitation = string.IsNullOrWhiteSpace(tokens[3]) ? 0 : double.Parse(tokens[3], InfrastructureConstants.EnglishCultureInfo);
                var evapotranspiration = string.IsNullOrWhiteSpace(tokens[4]) ? 0 : double.Parse(tokens[4], InfrastructureConstants.EnglishCultureInfo);

                result.Add(new DailyClimateData()
                {
                    Year = year,
                    JulianDay = julianDay,
                    MeanDailyAirTemperature = temperature,
                    MeanDailyPrecipitation = precipitation,
                    MeanDailyPET = evapotranspiration,
                });
            }

            return result;
        }

        public List<DailyClimateData> ParseText(string text)
        {
            var lines = text.Split(new[] {Environment.NewLine}, StringSplitOptions.None).ToList();

            return this.ParseFileLines(lines);
        }

        /// <summary>
        /// Use by both the CLI, and GUI to load a file that contains custom climate data.
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

            var dailyClimateData = this.ParseFileLines(lines);

            return dailyClimateData;
        }

        #endregion
    }
}