using System;
using System.Collections.Generic;
using System.Linq;

namespace H.Core.Emissions
{
    /// <summary>
    /// Tests were not written on October 10, 2018
    /// </summary>
    public class LiquidManureMethaneEquationsCalculator
    {
        /// <summary>
        /// Equation 9.1-1
        /// Converts from Celsius to Kelvin, Kelvin is used in the other equations
        /// </summary>
        /// <param name="temperatureAverageByProvinceAndMonth"></param>
        /// <returns></returns>
        public double CalculateAirTemperature(double temperatureAverageByProvinceAndMonth)
        {
            return temperatureAverageByProvinceAndMonth + 273.15;
        }

        /// <summary>
        /// Equation 9.1-2
        /// </summary>
        /// <param name="activationEnergyConstant"></param>
        /// <param name="airTemperature"></param>
        /// <returns></returns>
        public double CalculateClimateFactor(double activationEnergyConstant, double airTemperature)
        {
            return Math.Exp(activationEnergyConstant * (airTemperature - 303.16) / (1.987 * 303.16 * airTemperature));
        }

        /// <summary>
        /// Equation 9.1-3
        /// </summary>
        /// <param name="volatileSolids"></param>
        /// <param name="numberOfAnimals"></param>
        /// <param name="numberOfDays"></param>
        /// <returns></returns>
        public double CalculateTotalVolatileSolidsProducedByAllAnimalsForTheMonth(double volatileSolids,
                                                                                  double numberOfAnimals, double numberOfDays)
        {
            return volatileSolids * numberOfAnimals * numberOfDays;
        }

        /// <summary>
        ///  Equation 9.1-4
        /// </summary>
        /// <param name="volatileSolidsProducedByAllAnimalsForTheMonth"></param>
        /// <param name="managementAndDesignPracticeFactor"></param>
        /// <returns></returns>
        public double CalculateMonthlyVolatileSolidsLoadedIntoSystem(
            double volatileSolidsProducedByAllAnimalsForTheMonth, double managementAndDesignPracticeFactor)
        {
            return volatileSolidsProducedByAllAnimalsForTheMonth * managementAndDesignPracticeFactor;
        }

        /// <summary>
        ///  Equation 9.1-5
        /// When Liquid Manure is Emptied use Equation 9.1-6
        /// </summary>
        /// <param name="monthlyVolatileSolidsLoadedIntoSystemForThisMonth"></param>
        /// <param name="monthlyVolatileSolidsAvailableThePreviousMonth"></param>
        /// <param name="MonthlyVolatileSolidsConsumedThePreviousMonth"></param>
        /// <returns></returns>
        public double CalculateMonthlyVolatileSolidsAvailableForConversionToMethane(
            double monthlyVolatileSolidsLoadedIntoSystemForThisMonth,
            double monthlyVolatileSolidsAvailableThePreviousMonth, double MonthlyVolatileSolidsConsumedThePreviousMonth)
        {
            return monthlyVolatileSolidsLoadedIntoSystemForThisMonth +
                   (monthlyVolatileSolidsAvailableThePreviousMonth - MonthlyVolatileSolidsConsumedThePreviousMonth);
        }

        /// <summary>
        ///  Equation 9.1-6
        /// When there is still manure from the previous month use Equation 9.1-5
        /// </summary>
        /// <param name="monthlyVolatileSolidsLoadedIntoSystemForThisMonth"></param>
        /// <returns></returns>
        public double CalculateMonthlyVolatileSolidsAvailableForConversionToMethanWhenLiquidManureIsEmptied(
            double monthlyVolatileSolidsLoadedIntoSystemForThisMonth)
        {
            return monthlyVolatileSolidsLoadedIntoSystemForThisMonth;
        }

        /// <summary>
        ///  Equation 9.1-7
        /// </summary>
        /// <param name="climateFactor"></param>
        /// <param name="monthlyVolatileSolidsAvailableForConversionToMethane"></param>
        /// <returns></returns>
        public double CalculateMonthlyVolatileSolidsConsumed(double climateFactor,
                                                             double monthlyVolatileSolidsAvailableForConversionToMethane)
        {
            return climateFactor * monthlyVolatileSolidsAvailableForConversionToMethane;
        }

        /// <summary>
        ///  Equation 9.1-8
        /// </summary>
        /// <param name="monthlyVolatileSolidsConsumed"></param>
        /// <param name="methaneProducingCapacity"></param>
        /// <returns></returns>
        public double CalculateMonthlyMethaneEmission(double monthlyVolatileSolidsConsumed,
                                                      double methaneProducingCapacity)
        {
            return monthlyVolatileSolidsConsumed * methaneProducingCapacity * 0.67;
        }

        /// <summary>
        ///  Equation 9.1-9
        /// </summary>
        /// <param name="monthlyMethaneEmission"></param>
        /// <returns></returns>
        public double CalculateMonthlyMethaneEmissionForACoveredSystem(double monthlyMethaneEmission)
        {
            return monthlyMethaneEmission * 0.60;
        }

        /// <summary>
        /// Equation 9.1-10
        /// Use Equation 9.1-9 or Equation 9.1-8 depending on which one applies
        /// </summary>
        /// <param name="monthlyMethaneEmission"></param>
        /// <returns></returns>
        public double CalculateManureMethaneEmission(List<double> monthlyMethaneEmission)
        {
            return monthlyMethaneEmission.Sum();
        }

        /// <summary>
        /// Equation 9.1-11
        /// </summary>
        /// <param name="totalVolatileSolidsProducedByAllAnimalsForTheMonth"></param>
        /// <returns></returns>
        public double CalculateVolatileSolidsProducedYearly(
            List<double> totalVolatileSolidsProducedByAllAnimalsForTheMonth)
        {
            return totalVolatileSolidsProducedByAllAnimalsForTheMonth.Sum();
        }

        /// <summary>
        /// Equation 9.1-12
        /// </summary>
        /// <param name="manureMethaneEmission"></param>
        /// <param name="methaneProducingCapacity"></param>
        /// <param name="volatileSolidsProducedYearly"></param>
        /// <returns></returns>
        public double CalculateMethaneConversionFactor(double manureMethaneEmission, double methaneProducingCapacity,
                                                       double volatileSolidsProducedYearly)
        {
            var numerator = manureMethaneEmission / 0.67;
            var denominator = methaneProducingCapacity * volatileSolidsProducedYearly;
            return numerator / denominator;
        }

        /// <summary>
        ///    Equation 9.1-13
        /// </summary>
        /// <param name="volatileSolids"></param>
        /// <param name="methaneProducingCapacity"></param>
        /// <param name="methaneConversionFactor"></param>
        /// <returns></returns>
        public double CalculateManureMethaneEmissionRate(double volatileSolids, double methaneProducingCapacity,
                                                         double methaneConversionFactor)
        {
            return volatileSolids * methaneProducingCapacity * methaneConversionFactor * 0.67;
        }

        /// <summary>
        /// Equation 9.1-14
        /// </summary>
        /// <param name="manureMethaneEmissionRate"></param>
        /// <param name="numberOfAnimals"></param>
        /// <param name="numberOfDays"></param>
        /// <returns></returns>
        public double CalculateManureMethaneEmission(double manureMethaneEmissionRate, double numberOfAnimals,
                                                     double numberOfDays)
        {
            return manureMethaneEmissionRate * numberOfAnimals * numberOfDays;
        }
    }
}