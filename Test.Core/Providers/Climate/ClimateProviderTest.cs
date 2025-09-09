﻿using System.Collections.ObjectModel;
using System.Text;
using H.Content;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Climate;

namespace H.Core.Test.Providers.Climate;

[TestClass]
public class ClimateProviderTest : UnitTestBase
{
    #region Fields

    private NasaClimateProvider _nasaClimateProvider;

    #endregion

    [TestInitialize]
    public void Initialize()
    {
        _nasaClimateProvider = new NasaClimateProvider();
    }

    #region Tests

    [TestMethod]
    [Ignore]
    public void TameYieldRun2()
    {
        var lines = CsvResourceReader.SplitFileIntoLinesUsingRegex(
            File.ReadAllText("Climate\\TameYieldData_SiteCoords_16Feb2023.csv"));

        var stringBuilder = new StringBuilder();

        stringBuilder.Append("Author");
        stringBuilder.Append(",");
        stringBuilder.Append("Title_of_publication");
        stringBuilder.Append(",");
        stringBuilder.Append("Year_of_publication");
        stringBuilder.Append(",");
        stringBuilder.Append("Location");
        stringBuilder.Append(",");
        stringBuilder.Append("Sub_location");
        stringBuilder.Append(",");
        stringBuilder.Append("Measurement_period");
        stringBuilder.Append(",");
        stringBuilder.Append("Latitude (decimal degrees)");
        stringBuilder.Append(",");
        stringBuilder.Append("Longitude (decimal degrees)");
        stringBuilder.Append(",");
        stringBuilder.Append("Polygon");
        stringBuilder.Append(",");
        stringBuilder.Append("Total annual precipitation");
        stringBuilder.Append(",");
        stringBuilder.Append("Total annual PET");
        stringBuilder.Append(",");
        stringBuilder.Append("Total growing season precipitation");
        stringBuilder.Append(",");
        stringBuilder.AppendLine();

        foreach (var line in lines.Skip(1))
        {
            var author = line[0];
            var titleOfPublication = line[1].Replace(",", "");
            var yearOfPublication = line[2].Replace(",", "");
            var location = line[3];
            var subLocation = line[4];
            var meaurementPeriod = int.Parse(line[5]);
            var latitude = double.Parse(line[6]);
            var longitude = double.Parse(line[7]);
            var polygon = string.IsNullOrWhiteSpace(line[8]) ? 0 : int.Parse(line[8]);

            var totalAnnualPrecipitation = 0d;
            var totalAnnualEvapotranspiration = 0d;
            var totalGrowingSeasonPrecipitation = 0d;

            List<DailyClimateData> dataForYear;
            if (polygon == 0)
            {
                var dailyClimateData = _nasaClimateProvider.GetCustomClimateData(latitude, longitude);
                dataForYear = dailyClimateData.Where(x => x.Year == meaurementPeriod).ToList();
            }
            else
            {
                var path =
                    "C:\\Users\\mcphersonaa\\OneDrive - AGR-AGR\\Documents\\Work\\Projects\\Holos\\Model\\Climate\\Daily SLC Climate Data Files (By Polygon)\\roland_2\\" +
                    polygon + ".csv";

                var fileLines = File.ReadAllLines(path);
                var data = MakeDailyClimateData(fileLines);
                dataForYear = data.Where(x => x.Year == meaurementPeriod).ToList();
            }

            if (dataForYear.Any())
            {
                totalAnnualPrecipitation = dataForYear.Sum(x => x.MeanDailyPrecipitation);
                totalAnnualEvapotranspiration = dataForYear.Sum(x => x.MeanDailyPET);

                var growingSeasonDays = dataForYear.Where(x =>
                    x.JulianDay >= CoreConstants.GrowingSeasonJulianStartDay &&
                    x.JulianDay <= CoreConstants.GrowingSeasonJulianEndDaySeptember);

                totalGrowingSeasonPrecipitation = growingSeasonDays.Sum(x => x.MeanDailyPrecipitation);
            }

            stringBuilder.Append(author);
            stringBuilder.Append(",");
            stringBuilder.Append(titleOfPublication);
            stringBuilder.Append(",");
            stringBuilder.Append(yearOfPublication);
            stringBuilder.Append(",");
            stringBuilder.Append(location);
            stringBuilder.Append(",");
            stringBuilder.Append(subLocation);
            stringBuilder.Append(",");
            stringBuilder.Append(meaurementPeriod);
            stringBuilder.Append(",");
            stringBuilder.Append(latitude);
            stringBuilder.Append(",");
            stringBuilder.Append(longitude);
            stringBuilder.Append(",");
            stringBuilder.Append(polygon);
            stringBuilder.Append(",");
            stringBuilder.Append(totalAnnualPrecipitation);
            stringBuilder.Append(",");
            stringBuilder.Append(totalAnnualEvapotranspiration);
            stringBuilder.Append(",");
            stringBuilder.Append(totalGrowingSeasonPrecipitation);
            stringBuilder.Append(",");
            stringBuilder.AppendLine();
        }

        File.WriteAllText("Climate\\Output_2.csv", stringBuilder.ToString());
    }

    private List<DailyClimateData> MakeDailyClimateData(string[] fileLines)
    {
        List<DailyClimateData> listOfDailyClimateData = new List<DailyClimateData>();
        foreach (var line in fileLines.Skip(1))
        {
            var splitLine = line.Split(',');
            var dailyClimateData = new DailyClimateData
            {
                Year = int.Parse(splitLine[0]),
                JulianDay = int.Parse(splitLine[1]),
                MeanDailyAirTemperature = double.Parse(splitLine[2]),
                MeanDailyPrecipitation = double.Parse(splitLine[3]),
                MeanDailyPET = double.Parse(splitLine[4])
            };
            listOfDailyClimateData.Add(dailyClimateData);
        }

        return listOfDailyClimateData;
    }

    [TestMethod]
    public void ChangeNormals()
    {
        var climateData = _climateProvider.Get(50.99, -80.00, TimeFrame.NineteenEightyToNineteenNinety);

        // If Nasa service is offline we will get null and a failing test. Return from test in this case since we need data to test normals
        if (climateData == null) return;

        var changedClimateData = _climateProvider.AdjustClimateNormalsForTimeFrame(climateData.DailyClimateData,
            TimeFrame.NineteenNinetyToTwoThousand);

        Assert.AreNotEqual(climateData.EvapotranspirationData, changedClimateData.EvapotranspirationData);
        Assert.AreNotEqual(climateData.PrecipitationData, changedClimateData.PrecipitationData);
        Assert.AreNotEqual(climateData.TemperatureData, changedClimateData.TemperatureData);
    }

    [TestMethod]
    public void TestFileExport()
    {
        // Setup
        var somePath = Directory.GetCurrentDirectory();
        somePath = somePath + "test.csv";

        var dailyClimateData = new List<DailyClimateData>
            { new() { Date = DateTime.Now, MeanDailyAirTemperature = 10 } };
        var dailyClimateData2 = new DailyClimateData
            { Date = DateTime.Now, MeanDailyAirTemperature = 12, SolarRadiation = 2 };
        var dailyClimateData3 = new DailyClimateData
        {
            Year = 2023, JulianDay = 5, MeanDailyAirTemperature = 3, MeanDailyPrecipitation = 4,
            MeanDailyPET = 5, RelativeHumidity = 5, SolarRadiation = 3, Date = DateTime.Now
        };
        var farm = new Farm
        {
            ClimateData = new ClimateData
                { DailyClimateData = new ObservableCollection<DailyClimateData>(dailyClimateData) }
        };
        farm.ClimateData.DailyClimateData.Add(dailyClimateData2);
        farm.ClimateData.DailyClimateData.Add(dailyClimateData3);

        _climateProvider.OutputDailyClimateData(farm, somePath);

        // Verify
        File.Exists(somePath);
        File.Delete(somePath); // Delete file test
    }

    [TestMethod]
    public void TestOutputMonthlyClimateData()
    {
        // Setup
        var somePath = Directory.GetCurrentDirectory();
        somePath = somePath + "test.csv";

        var dailyClimateData = new List<DailyClimateData>
            { new() { Date = DateTime.Now, MeanDailyAirTemperature = 10 } };
        var dailyClimateData2 = new DailyClimateData
            { Date = DateTime.Now, MeanDailyAirTemperature = 12, SolarRadiation = 2 };
        var dailyClimateData3 = new DailyClimateData
        {
            Year = 2023,
            JulianDay = 5,
            MeanDailyAirTemperature = 3,
            MeanDailyPrecipitation = 4,
            MeanDailyPET = 5,
            RelativeHumidity = 5,
            SolarRadiation = 3,
            Date = DateTime.Now
        };
        var farm = new Farm
        {
            ClimateData = new ClimateData
                { DailyClimateData = new ObservableCollection<DailyClimateData>(dailyClimateData) }
        };

        var viewItem1 = new CropViewItem { Year = 1985 };
        var viewItem2 = new CropViewItem { Year = 2002 };

        farm.StageStates.Add(new FieldSystemDetailsStageState
        {
            DetailsScreenViewCropViewItems =
                new ObservableCollection<CropViewItem>(new List<CropViewItem> { viewItem1, viewItem2 })
        });

        farm.ClimateData.DailyClimateData.Add(dailyClimateData2);
        farm.ClimateData.DailyClimateData.Add(dailyClimateData3);

        _climateProvider.OutputMonthlyClimateData(farm, somePath);

        // Verify
        File.Exists(somePath);
        File.Delete(somePath); // Delete file test
    }

    #endregion
}