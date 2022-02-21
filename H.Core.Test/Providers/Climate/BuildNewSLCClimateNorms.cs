using H.Core.Calculators.Climate;
using H.Core.Enumerations;
using H.Core.Providers.Climate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace H.Core.Test
{
    [TestClass]
    public class BuildNewSLCClimateNorms
    {
        /// <summary>
        /// Where the csv files with the SLC data exists
        /// </summary>
        private string workingDirectory = @"C:\Users\bigbe\Holos\slcDaily\additionalSLC";
        private string outPutDirectory = @"C:\Users\bigbe\Holos\slcDaily\2021_new_climate_normals";

        // Need to check that our new way of calculating normals is correct. Best to check on one file than working on all 2669 to see if they are correct
        [TestMethod]
        [Ignore]
        public void BuildUpdatedLethbridgeClimateNorms()
        {
            var lethbridgePolygonFile = workingDirectory + @"\793011.csv";
            var climateNormalCalculator = new ClimateNormalCalculator();
            int customTimeFrameStart = 1980;
            int customTimeFrameEnd = 2010;
            int polygonId = 793011;

            var fileLines = File.ReadAllLines(lethbridgePolygonFile);

            var listOfDailyClimateData = MakeDailyClimateData(fileLines);

            var temperatureNormals = climateNormalCalculator.GetTemperatureDataByDailyValues(listOfDailyClimateData, customTimeFrameStart, customTimeFrameEnd);
            var precipiationNormals = climateNormalCalculator.GetPrecipitationDataByDailyValues(listOfDailyClimateData, customTimeFrameStart, customTimeFrameEnd);
            var evapotranspirationNormals = climateNormalCalculator.GetEvapotranspirationDataByDailyValues(listOfDailyClimateData, customTimeFrameStart, customTimeFrameEnd);

            var lines = new List<string>();
            foreach (var month in Enum.GetValues(typeof(Months)).Cast<Months>())
            {
                string line = $"{polygonId},{(int)month},{temperatureNormals.GetMeanTemperatureForMonth((int)month)},{precipiationNormals.GetValueByMonth(month)},{evapotranspirationNormals.GetValueByMonth(month)}";
                lines.Add(line);
            }
            File.WriteAllLines(outPutDirectory + "\\test_fix_for_normals.csv", lines);

        }
        [TestMethod]
        [Ignore]
        public void BuildUpdatedClimateNormsByPolygon()
        {
            var climateNormalCalculator = new ClimateNormalCalculator();
            var fileExtension = ".csv";
            int customTimeFrameStart;
            int customTimeFrameEnd;

            var allFileStrings = Directory.GetFiles(workingDirectory, $"*{fileExtension}");

            //for every file we must have the rolling average of climate normals from 1950 to 2017
            for (int i = 1950; i <= 1990; i += 10)
            {
                //assign correct timeframe
                customTimeFrameStart = i;
                if (i == 1990)
                {
                    customTimeFrameEnd = 2017;
                }
                else
                {
                    customTimeFrameEnd = i + 30;
                }
                foreach (var fileString in allFileStrings)
                {
                    var fileLines = File.ReadAllLines(fileString);

                    //make the daily climate data from file
                    var listOfDailyClimateData = MakeDailyClimateData(fileLines);

                    //the climate normals for that file for the given timeframe
                    var temperatureNormals = climateNormalCalculator.GetTemperatureDataByDailyValues(listOfDailyClimateData, customTimeFrameStart, customTimeFrameEnd);
                    var precipiationNormals = climateNormalCalculator.GetPrecipitationDataByDailyValues(listOfDailyClimateData, customTimeFrameStart, customTimeFrameEnd);
                    var evapotranspirationNormals = climateNormalCalculator.GetEvapotranspirationDataByDailyValues(listOfDailyClimateData, customTimeFrameStart, customTimeFrameEnd);

                    //Just give me the filename as the polygon id
                    var splitFileString = fileString.Split('\\');
                    var polygonId = splitFileString.ElementAt(splitFileString.Length - 1).Trim(fileExtension.ToCharArray());

                    var lines = new List<string>();
                    foreach (var month in Enum.GetValues(typeof(Months)).Cast<Months>())
                    {
                        string line = $"{polygonId},{(int)month},{temperatureNormals.GetMeanTemperatureForMonth((int)month)},{precipiationNormals.GetValueByMonth(month)},{evapotranspirationNormals.GetValueByMonth(month)}";
                        lines.Add(line);
                    }

                    Directory.CreateDirectory(outPutDirectory + $"\\{customTimeFrameStart}-{customTimeFrameEnd}");
                    File.WriteAllLines(outPutDirectory + $"\\{customTimeFrameStart}-{customTimeFrameEnd}\\normalized_{polygonId}{fileExtension}", lines);
                }
            }

        }

        private List<DailyClimateData> MakeDailyClimateData(string[] fileLines)
        {
            List<DailyClimateData> listOfDailyClimateData = new List<DailyClimateData>();
            foreach (var line in fileLines.Skip(1))
            {
                var splitLine = line.Split(',');
                var dailyClimateData = new DailyClimateData()
                {
                    Year = int.Parse(splitLine[0]),
                    JulianDay = int.Parse(splitLine[1]),
                    MeanDailyAirTemperature = double.Parse(splitLine[2]),
                    MeanDailyPrecipitation = double.Parse(splitLine[3]),
                    MeanDailyPET = double.Parse(splitLine[4]),
                };
                listOfDailyClimateData.Add(dailyClimateData);
            }
            return listOfDailyClimateData;
        }
    }
}
