﻿using System;
using System.Collections.Generic;

namespace H.Core.Calculators.Climate
{
    public class EvapotranspirationCalculator
    {
        #region Fields

        private readonly Dictionary<Tuple<double, double, double>, double> _cache =
            new Dictionary<Tuple<double, double, double>, double>();

        #endregion

        #region Methods

        /// <summary>
        ///     Eq. 1.6.2-1
        ///     Eq. 1.6.2-2
        /// </summary>
        /// <param name="meanDailyTemperature">Mean daily temp (C)</param>
        /// <param name="solarRadiation">Solar radiation (MJ m^-2 day^-1)</param>
        /// <param name="relativeHumidity">Relative humidity (%)</param>
        /// <returns>Reference evapotranspiration (mm day^-1)</returns>
        public double CalculateReferenceEvapotranspiration(double meanDailyTemperature, double solarRadiation,
            double relativeHumidity)
        {
            var key = new Tuple<double, double, double>(meanDailyTemperature, solarRadiation, relativeHumidity);
            if (_cache.ContainsKey(key)) return _cache[key];

            var term1 = 0.013;

            // As per Roland chat on December 9 in Discord (wasn't in algorithm document). Evapotranspiration will be zero when temperature is < 0 for the day.
            if (meanDailyTemperature <= 0) return 0;

            if (Math.Abs(meanDailyTemperature - -15) < double.Epsilon)
                // Have to return 0 since the next term would be a division by 0 otherwise
                return 0;

            var term2 = meanDailyTemperature / (meanDailyTemperature + 15);
            var term3 = 23.8856 * solarRadiation + 50;
            var term4 = 1 + (50 - relativeHumidity) / 70;

            var result = 0d;
            if (relativeHumidity >= 50)
                result = term1 * term2 * term3;
            else
                result = term1 * term2 * term3 * term4;

            // Evapotranspiration can never be < 0
            if (result < 0) return 0;

            _cache[key] = result;

            return result;
        }

        #endregion
    }
}