using System;
using H.Core.Enumerations;
using H.Core.Models;

namespace H.Core.Providers.Climate
{
    public interface IClimateProvider
    {
        void OutputDailyClimateData(Farm farm, string outputPath);
        ClimateData Get(double latitude, double longitude, TimeFrame climateNormalTimeFrame);
        double GetMeanTemperatureForDay(Farm farm, DateTime dateTime);
        double GetAnnualEvapotranspiration(Farm farm, DateTime dateTime);
        double GetAnnualPrecipitation(Farm farm, DateTime dateTime);
        double GetGrowingSeasonPrecipitation(Farm farm, DateTime dateTime);
        double GetGrowingSeasonEvapotranspiration(Farm farm, DateTime dateTime);
        double GetAnnualPrecipitation(Farm farm, int year);
        double GetAnnualEvapotranspiration(Farm farm, int year);
        double GetGrowingSeasonPrecipitation(Farm farm, int year);
        double GetGrowingSeasonEvapotranspiration(Farm farm, int year);
        ClimateData Get(double latitude, double longitude, TimeFrame climateNormalTimeFrame, Farm farm);
        ClimateData Get(string filepath, TimeFrame normalCalculationTimeFrame);
        ClimateData GetClimateData(int polygonId, TimeFrame timeFrame);
    }
}