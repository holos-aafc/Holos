using System;
using System.Collections.Generic;
using System.Windows.Media;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Climate;
using H.Core.Providers.Irrigation;

namespace H.Core.Services.LandManagement
{
    public class IrrigationService
    {
        #region Fields

        private readonly Table_7_Monthly_Irrigation_Water_Application_Provider _irrigationProvider = new Table_7_Monthly_Irrigation_Water_Application_Provider();
        

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
        /// Equation 2.1.1-18
        /// </summary>
        public double GetTotalDailyPrecipitation(int julianDay, double precipitation, Province province, int year, double annualIrrigation)
        {
            var month = this.GetMonthFromJulianDay(julianDay);

            double irrigationPercentage = 0;
            var data = _irrigationProvider.GetMonthlyAverageIrrigationDataInstance(month, province);
            if (data != null)
            {
                irrigationPercentage = data.IrrigationVolume;
            }

            // Lookup value will be null for January, February, March, November, and December

            var fraction = irrigationPercentage / 100.0;
            double daysInMonth = DateTime.DaysInMonth(year, (int) month);

            var result = precipitation + ((annualIrrigation * fraction) / daysInMonth);

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

        public Months GetMonthFromJulianDay(int julianDay)
        {
            if (julianDay >= 1 && julianDay <= 31)
            {
                return Months.January;
            }
            else if (julianDay >= 32 && julianDay <= 59)
            {
                return Months.February;
            }
            else if (julianDay >= 60 && julianDay <= 90)
            {
                return Months.March;
            }
            else if (julianDay >= 91 && julianDay <= 120)
            {
                return Months.April;
            }
            else if (julianDay >= 121 && julianDay <= 151)
            {
                return Months.May;
            }
            else if (julianDay >= 152 && julianDay <= 181)
            {
                return Months.June;
            }
            else if (julianDay >= 182 && julianDay <= 212)
            {
                return Months.July;
            }
            else if (julianDay >= 213 && julianDay <= 243)
            {
                return Months.August;
            }
            else if (julianDay >= 244 && julianDay <= 273)
            {
                return Months.September;
            }
            else if (julianDay >= 274 && julianDay <= 304)
            {
                return Months.October;
            }
            else if (julianDay >= 305 && julianDay <= 334)
            {
                return Months.November;
            }
            else if (julianDay >= 335 && julianDay <= 366) // Include 366 to account for leap years
            {
                return Months.December;
            }
            else
            {
                throw new Exception($"Julian day out of range: {julianDay}");
            }
        }

        #endregion

        #region Private Methods

        #endregion
    }
}