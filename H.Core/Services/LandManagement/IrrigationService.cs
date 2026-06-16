using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Irrigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace H.Core.Services.LandManagement
{
    public class IrrigationService
    {
        #region Fields

        private readonly Table_4_Monthly_Irrigation_Water_Application_Provider _irrigationProvider = new Table_4_Monthly_Irrigation_Water_Application_Provider();


        #endregion

        #region Constuctors


        #endregion

        #region Public Methods

        /// <summary>
        /// Equation 2.1.1-17
        /// </summary>
        public double GetDefaultIrrigationForYear(Farm farm, int year)
        {
            var annualPrecipitation = farm.ClimateData.GetTotalPrecipitationForYear(year);
            var potentialEvapotranspiration = farm.ClimateData.GetTotalEvapotranspirationForYear(year);

            if (potentialEvapotranspiration > annualPrecipitation)
            {
                return potentialEvapotranspiration - annualPrecipitation;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Equation 2.1.1-48
        /// </summary>
        public double GetTotalWaterInputs(Farm farm, CropViewItem viewItem)
        {
            var climateDataGroupedByYear = farm.ClimateData.DailyClimateData.GroupBy(userClimateData => userClimateData.Year);
            var climateDataForYear = climateDataGroupedByYear.SingleOrDefault(groupingByYear => groupingByYear.Key == viewItem.Year);
            var result = 0d;

            if (climateDataForYear != null && climateDataForYear.Count() == 365)
            {
                // Use daily climate data
                var precipitationList = climateDataForYear.OrderBy(climateData => climateData.JulianDay).Select(climateData => climateData.MeanDailyPrecipitation).ToList();

                // Add irrigation amounts to daily precipitations
                var totalPrecipitationList = this.AddIrrigationToDailyPrecipitations(precipitationList, farm, viewItem);

                result = totalPrecipitationList.Sum();
            }
            else
            {
                // We don't have a complete set of daily values, use normals as a fallback
                var totalPrecipitation = farm.ClimateData.GetTotalPrecipitationForYear(viewItem.Year);
                var totalIrrigation = viewItem.AmountOfIrrigation;

                result = totalPrecipitation + totalIrrigation;
            }

            return result;
        }

        public double GetGrowingSeasonIrrigation(Farm farm, CropViewItem cropViewItem)
        {
            return _irrigationProvider.GetTotalGrowingSeasonIrrigation(farm.DefaultSoilData.Province);
        }

        /// <summary>
        /// Equation 2.1.1-18
        /// </summary>
        public double GetTotalDailyPrecipitation(int julianDay, double precipitation, Province province, int year, double annualIrrigation)
        {
            var month = this.GetMonthFromJulianDay(julianDay, year);

            double irrigationPercentage = 0;
            var data = _irrigationProvider.GetMonthlyAverageIrrigationDataInstance(month, province);
            if (data != null)
            {
                irrigationPercentage = data.IrrigationVolume;
            }

            // Lookup value will be null for January, February, March, November, and December

            var fraction = irrigationPercentage / 100.0;
            double daysInMonth = DateTime.DaysInMonth(year, (int)month);

            var result = precipitation + ((annualIrrigation * fraction) / daysInMonth);

            return result;
        }

        public double GetIrrigationForDay(int julianDay, Province province, int year, double annualIrrigation)
        {
            var month = this.GetMonthFromJulianDay(julianDay, year);

            double irrigationPercentage = 0;
            var data = _irrigationProvider.GetMonthlyAverageIrrigationDataInstance(month, province);
            if (data != null)
            {
                irrigationPercentage = data.IrrigationVolume;
            }

            // Lookup value will be null for January, February, March, November, and December

            var fraction = irrigationPercentage / 100.0;
            double daysInMonth = DateTime.DaysInMonth(year, (int)month);

            var result = ((annualIrrigation * fraction) / daysInMonth);

            return result;
        }

        public List<double> AddIrrigationToDailyPrecipitations(
            List<double> dailyPrecipitationList,
            Farm farm,
            CropViewItem cropViewItem)
        {
            var result = new List<double>();

            var julianDay = 1;

            foreach (var dailyPrecipitation in dailyPrecipitationList)
            {
                var dailyPrecipitationAndIrrigation = this.GetTotalDailyPrecipitation(
                    julianDay,
                    dailyPrecipitation,
                    farm.Province,
                    cropViewItem.Year,
                    cropViewItem.AmountOfIrrigation);

                result.Add(dailyPrecipitationAndIrrigation);

                julianDay++;
            }

            return result;
        }

        public Months GetMonthFromJulianDay(int julianDay, int year)
        {
            if (julianDay > (DateTime.IsLeapYear(year) ? 366 : 365))
            {
                throw new Exception($"Julian day out of range: {julianDay}");
            }

            return (Months)new DateTime(year, 1, 1)
                .AddDays(julianDay - 1) // Get datetime for the julian day
                .Month; // Extract month from datetime
        }

        #endregion

        #region Private Methods

        #endregion
    }
}