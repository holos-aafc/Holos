﻿using System;
using System.Collections.Generic;
using System.Linq;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Irrigation;

namespace H.Core.Services.LandManagement
{
    public class IrrigationService
    {
        #region Fields

        private readonly Table_4_Monthly_Irrigation_Water_Application_Provider _irrigationProvider =
            new Table_4_Monthly_Irrigation_Water_Application_Provider();

        #endregion

        #region Public Methods

        /// <summary>
        ///     Equation 2.1.1-17
        /// </summary>
        public double GetDefaultIrrigationForYear(Farm farm, int year)
        {
            var annualPrecipitation = farm.ClimateData.GetTotalPrecipitationForYear(year);
            var potentialEvapotranspiration = farm.ClimateData.GetTotalEvapotranspirationForYear(year);

            if (potentialEvapotranspiration > annualPrecipitation)
                return potentialEvapotranspiration - annualPrecipitation;

            return 0;
        }

        /// <summary>
        ///     Equation 2.1.1-48
        /// </summary>
        public double GetTotalWaterInputs(Farm farm, CropViewItem viewItem)
        {
            var climateDataGroupedByYear =
                farm.ClimateData.DailyClimateData.GroupBy(userClimateData => userClimateData.Year);
            var climateDataForYear =
                climateDataGroupedByYear.SingleOrDefault(groupingByYear => groupingByYear.Key == viewItem.Year);
            var result = 0d;

            if (climateDataForYear != null && climateDataForYear.Count() == 365)
            {
                // Use daily climate data
                var precipitationList = climateDataForYear.OrderBy(climateData => climateData.JulianDay)
                    .Select(climateData => climateData.MeanDailyPrecipitation).ToList();

                // Add irrigation amounts to daily precipitations
                var totalPrecipitationList = AddIrrigationToDailyPrecipitations(precipitationList, farm, viewItem);

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
        ///     Equation 2.1.1-18
        /// </summary>
        public double GetTotalDailyPrecipitation(int julianDay, double precipitation, Province province, int year,
            double annualIrrigation)
        {
            var month = GetMonthFromJulianDay(julianDay);

            double irrigationPercentage = 0;
            var data = _irrigationProvider.GetMonthlyAverageIrrigationDataInstance(month, province);
            if (data != null) irrigationPercentage = data.IrrigationVolume;

            // Lookup value will be null for January, February, March, November, and December

            var fraction = irrigationPercentage / 100.0;
            double daysInMonth = DateTime.DaysInMonth(year, (int)month);

            var result = precipitation + annualIrrigation * fraction / daysInMonth;

            return result;
        }

        public double GetIrrigationForDay(int julianDay, Province province, int year, double annualIrrigation)
        {
            var month = GetMonthFromJulianDay(julianDay);

            double irrigationPercentage = 0;
            var data = _irrigationProvider.GetMonthlyAverageIrrigationDataInstance(month, province);
            if (data != null) irrigationPercentage = data.IrrigationVolume;

            // Lookup value will be null for January, February, March, November, and December

            var fraction = irrigationPercentage / 100.0;
            double daysInMonth = DateTime.DaysInMonth(year, (int)month);

            var result = annualIrrigation * fraction / daysInMonth;

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
                var dailyPrecipitationAndIrrigation = GetTotalDailyPrecipitation(
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

        public Months GetMonthFromJulianDay(int julianDay)
        {
            if (julianDay >= 1 && julianDay <= 31) return Months.January;

            if (julianDay >= 32 && julianDay <= 59) return Months.February;

            if (julianDay >= 60 && julianDay <= 90) return Months.March;

            if (julianDay >= 91 && julianDay <= 120) return Months.April;

            if (julianDay >= 121 && julianDay <= 151) return Months.May;

            if (julianDay >= 152 && julianDay <= 181) return Months.June;

            if (julianDay >= 182 && julianDay <= 212) return Months.July;

            if (julianDay >= 213 && julianDay <= 243) return Months.August;

            if (julianDay >= 244 && julianDay <= 273) return Months.September;

            if (julianDay >= 274 && julianDay <= 304) return Months.October;

            if (julianDay >= 305 && julianDay <= 334) return Months.November;

            if (julianDay >= 335 && julianDay <= 366) // Include 366 to account for leap years
                return Months.December;

            throw new Exception($"Julian day out of range: {julianDay}");
        }

        #endregion
    }
}