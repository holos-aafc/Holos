#region Imports

using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using H.Core.Providers.Climate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace H.Core.Test.Providers.Climate
{
    [TestClass]
    public class CustomClimateDataProviderTest
    {
        #region Fields

        private CustomFileClimateDataProvider _provider;

        #endregion

        #region Initialization

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new CustomFileClimateDataProvider();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        #endregion

        #region Tests

        [TestMethod]
        public void ReadUserClimateFileReturnsNonEmptyList()
        {
            var lines = Resource1.custom_climate_data.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.None).ToList();

            var result = _provider.ParseFileLines(lines);

            Assert.AreEqual(38717, result.Count);
        }

        [TestMethod]
        public void ParseExtendedFileLines_ComputesMeanWhenMissing()
        {
            var lines = new List<string>
            {
                "Year,JulianDay,MaximumAirTemperature,MinimumAirTemperature,MeanDailyPrecipitation,MeanDailyPET",
                "2020,1,10,2,0,1"
            };

            var result = _provider.ParseFileLines(lines);

            Assert.AreEqual(1, result.Count);
            var first = result.First();
            Assert.AreEqual(2020, first.Year);
            Assert.AreEqual(1, first.JulianDay);
            Assert.AreEqual(6.0, first.MeanDailyAirTemperature,0.0001);
            Assert.AreEqual(10.0, first.MaximumAirTemperature,0.0001);
            Assert.AreEqual(2.0, first.MinimumAirTemperature,0.0001);
            Assert.AreEqual(0.0, first.MeanDailyPrecipitation,0.0001);
            Assert.AreEqual(1.0, first.MeanDailyPET,0.0001);
        }

        [TestMethod]
        public void ParseExtendedFileLines_UsesMeanWhenPresent()
        {
            var lines = new List<string>
            {
                "Year,JulianDay,MeanDailyAirTemperature,MaximumAirTemperature,MinimumAirTemperature,MeanDailyPrecipitation,MeanDailyPET",
                "2020,2,7,11,3,2,1.5"
            };

            var result = _provider.ParseFileLines(lines);

            Assert.AreEqual(1, result.Count);
            var first = result.First();
            Assert.AreEqual(2020, first.Year);
            Assert.AreEqual(2, first.JulianDay);
            Assert.AreEqual(7.0, first.MeanDailyAirTemperature,0.0001);
            Assert.AreEqual(11.0, first.MaximumAirTemperature,0.0001);
            Assert.AreEqual(3.0, first.MinimumAirTemperature,0.0001);
            Assert.AreEqual(2.0, first.MeanDailyPrecipitation,0.0001);
            Assert.AreEqual(1.5, first.MeanDailyPET,0.0001);
        }

        [TestMethod]
        public void HasExpectedInputFormat_AcceptsBaseAndExtendedHeaders()
        {
            var baseHeader = "Year,JulianDay,MeanDailyAirTemperature,MeanDailyPrecipitation,MeanDailyPET";
            var extendedHeader = "Year,JulianDay,MeanDailyAirTemperature,MaximumAirTemperature,MinimumAirTemperature,MeanDailyPrecipitation,MeanDailyPET";

            var tempBase = Path.GetTempFileName();
            var tempExt = Path.GetTempFileName();

            try
            {
                File.WriteAllText(tempBase, baseHeader + Environment.NewLine + "2020,1,5,0,1");
                File.WriteAllText(tempExt, extendedHeader + Environment.NewLine + "2020,1,5,10,0,1");

                Assert.IsTrue(_provider.HasExpectedInputFormat(tempBase));
                Assert.IsTrue(_provider.HasExpectedInputFormat(tempExt));
            }
            finally
            {
                try { File.Delete(tempBase); } catch { }
                try { File.Delete(tempExt); } catch { }
            }
        }

        [TestMethod]
        public void ParseRealClimateFile_ReadsLethbridgeFileAndVerifiesParsing_FromResources()
        {
            // Use embedded resource entry instead of file system lookup
            var csv = Resource1.lethbridge_daily_variable;
            Assert.IsFalse(string.IsNullOrWhiteSpace(csv), "Resource for Lethbridge CSV is missing or empty.");

            var fileLines = csv.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.None).Where(l => !string.IsNullOrWhiteSpace(l)).ToList();
            Assert.IsTrue(fileLines.Count >1, "Test resource appears to be empty or only contains a header.");

            var parsed = _provider.ParseFileLines(fileLines);
            var expectedDataLines = fileLines.Count -1;
            Assert.AreEqual(expectedDataLines, parsed.Count, "Parsed record count does not match number of data lines in resource.");

            var firstDataLine = fileLines[1];
            var fields = firstDataLine.Split(',');
            int expectedYear;
            int expectedJulianDay =0;
            if (!int.TryParse(fields[0], out expectedYear) || !int.TryParse(fields[1], out expectedJulianDay))
            {
                Assert.Fail("Could not parse Year/JulianDay from first data line of the test CSV resource.");
            }

            var first = parsed.First();
            Assert.AreEqual(expectedYear, first.Year, "Year from parsed record does not match CSV resource.");
            Assert.AreEqual(expectedJulianDay, first.JulianDay, "JulianDay from parsed record does not match CSV resource.");
        }

        [TestMethod]
        public void ParseExtendedResource_WithMaxMin_ParsesAllFields()
        {
            var csv = Resource1.extended_climate_with_max_min;
            Assert.IsFalse(string.IsNullOrWhiteSpace(csv), "Extended climate resource is missing or empty.");

            var lines = csv.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.None)
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .ToList();

            var parsed = _provider.ParseFileLines(lines);
            // Expect full year 2025 dataset
            Assert.AreEqual(365, parsed.Count, "Expected 365 daily records for 2025.");
            Assert.IsTrue(parsed.All(p => p.Year == 2025), "All records should be for year 2025.");
            Assert.AreEqual(1, parsed.First().JulianDay, "First record should be day 1.");
            Assert.AreEqual(365, parsed.Last().JulianDay, "Last record should be day 365.");

            // Basic invariants on temperatures and non-negative water variables
            Assert.IsTrue(parsed.All(p => p.MaximumAirTemperature >= p.MeanDailyAirTemperature && p.MeanDailyAirTemperature >= p.MinimumAirTemperature),
                "Each record should satisfy Max >= Mean >= Min.");
            Assert.IsTrue(parsed.All(p => p.MeanDailyPrecipitation >= 0 && p.MeanDailyPET >= 0),
                "Precipitation and PET should be non-negative.");

            // Ensure we have some precipitation and some PET >0 days
            Assert.IsTrue(parsed.Any(p => p.MeanDailyPrecipitation > 0), "Expected some days with precipitation.");
            Assert.IsTrue(parsed.Any(p => p.MeanDailyPET > 0), "Expected some days with PET > 0.");
        }

        [TestMethod]
        public void ParseGeneratedSinusoidalDataset_ParsesAllFieldsAndSeasonality()
        {
            // Build sinusoidal dataset for 2025 in-memory
            var lines = new List<string>();
            lines.Add("Year,JulianDay,MeanDailyAirTemperature,MaximumAirTemperature,MinimumAirTemperature,MeanDailyPrecipitation,MeanDailyPET");

            for (int d = 1; d <= 365; d++)
            {
                // Mean = 5 + 15*sin(2*pi*(d-80)/365)
                var mean = Math.Round(5 + 15 * Math.Sin(2 * Math.PI * (d - 80) / 365.0), 1);
                var max = Math.Round(mean + 5, 1);
                var min = Math.Round(mean - 5, 1);
                double precip = (d % 10 == 0) ? 5.0 : (d % 7 == 0 ? 2.0 : 0.0);
                var pet = Math.Round(Math.Max(0.0, mean) * 0.12, 3);
                lines.Add($"2025,{d},{mean},{max},{min},{precip},{pet}");
            }

            var parsed = _provider.ParseFileLines(lines);
            Assert.AreEqual(365, parsed.Count);
            Assert.IsTrue(parsed.All(p => p.Year == 2025));
            Assert.AreEqual(1, parsed.First().JulianDay);
            Assert.AreEqual(365, parsed.Last().JulianDay);
            Assert.IsTrue(parsed.All(p => p.MaximumAirTemperature >= p.MeanDailyAirTemperature && p.MeanDailyAirTemperature >= p.MinimumAirTemperature));
            Assert.IsTrue(parsed.All(p => p.MeanDailyPrecipitation >= 0 && p.MeanDailyPET >= 0));

            // Seasonal extremes expected roughly mid-summer and mid-winter
            var maxMean = parsed.Max(p => p.MeanDailyAirTemperature);
            var minMean = parsed.Min(p => p.MeanDailyAirTemperature);
            Assert.IsTrue(maxMean >= 19.0 && maxMean <= 20.1, $"Unexpected summer peak mean: {maxMean}");
            Assert.IsTrue(minMean <= -9.0 && minMean >= -10.5, $"Unexpected winter trough mean: {minMean}");

            // PET should correlate with positive mean temperatures
            Assert.IsTrue(parsed.Any(p => p.MeanDailyPET > 2.0), "Expect some higher PET values in warm season.");
        }

        #endregion
    }
}