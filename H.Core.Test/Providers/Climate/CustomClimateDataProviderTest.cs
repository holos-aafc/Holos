#region Imports

using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using H.Core.Providers.Climate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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
            var lines = Resource1.custom_climate_data.Split(new []{Environment.NewLine, "\n"}, StringSplitOptions.None).ToList();

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
            int expectedJulianDay = 0;
            if (!int.TryParse(fields[0], out expectedYear) || !int.TryParse(fields[1], out expectedJulianDay))
            {
                Assert.Fail("Could not parse Year/JulianDay from first data line of the test CSV resource.");
            }

            var first = parsed.First();
            Assert.AreEqual(expectedYear, first.Year, "Year from parsed record does not match CSV resource.");
            Assert.AreEqual(expectedJulianDay, first.JulianDay, "JulianDay from parsed record does not match CSV resource.");
        }

        #endregion
    }
}