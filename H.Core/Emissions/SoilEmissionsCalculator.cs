using System;

namespace H.Core.Emissions
{
    public class SoilEmissionsCalculator
    {
        /// <summary>
        /// Equation 2.1.1-1
        /// Equation 2.1.2-1
        /// Equation 2.1.3-1
        /// Equation 2.1.3-5
        /// Equation 2.1.4-1
        /// Equation 2.1.4-5
        /// For: Tillage, Fallow, Current Perennial, Past Perennial, Seeded Grassland, Broken Grassland
        /// </summary>
        /// <param name="currentYear">Current year</param>
        /// <param name="yearOfChange">Year of tillage change</param>
        /// <returns>Time since management change</returns>
        public double CalculateTimeSinceManagementChangeInYears(double currentYear, double yearOfChange)
        {
            return currentYear - yearOfChange;
        }

        /// <summary>
        /// Equation 2.1.1-2
        /// Equation 2.1.2-2
        /// Equation 2.1.3-2
        /// Equation 2.1.3-6
        /// Equation 2.1.4-2
        /// Equation 2.1.4-6   
        /// For: Tillage, Fallow, Current Perennial, Past Perennial, Seeded Grassland, Broken Grassland
        /// </summary>
        /// <param name="maximumCarbonProducedByManagementChange">Maximum C produced by management change (g m^-2)</param>
        /// <param name="rateConstant">Rate constant</param>
        /// <param name="timeSinceManagementChangeInYears">Time since management change (years)</param>
        /// <returns>C change rate for grassland (g m^-2 year^-1)</returns>
        public double CalculateCarbonChangeRate(double maximumCarbonProducedByManagementChange, 
                                                double rateConstant,
                                                double timeSinceManagementChangeInYears)
        {
            var negativek = -rateConstant;
            var e1 = negativek * (timeSinceManagementChangeInYears - 1);
            var e2 = negativek * timeSinceManagementChangeInYears;
            var factor = Math.Exp(e1) - Math.Exp(e2);
            var carbonChangeRate = maximumCarbonProducedByManagementChange * factor;
            return carbonChangeRate;
        }

        /// <summary>
        /// Equation 2.1.1-3
        /// Equation 2.1.2-3
        /// Equation 2.1.3-3
        /// Equation 2.1.3-7
        /// Equation 2.1.4-3
        /// Equation 2.1.4-7
        /// For: Tillage, Fallow, Current Perennial, Past Perennial, Seeded Grassland, Broken Grassland
        /// </summary>
        /// <param name="carbonChangeRate">C change rate for grassland (g m^-2 year^-1)</param>
        /// <param name="areaOfManagementChange">Area of management change (ha)</param>
        /// <returns>C change (kg C year^-1)</returns>
        public double CalculateCarbonChange(double carbonChangeRate, double areaOfManagementChange)
        {
            return carbonChangeRate * areaOfManagementChange * 10;
        }

        /// <summary>
        /// Equation 2.1.1-4
        /// Equation 2.1.2-4
        /// Equation 2.1.3-4
        /// Equation 2.1.3-8
        /// Equation 2.1.4-4
        /// Equation 2.1.4-8
        /// For: Tillage, Fallow, Current Perennial, Past Perennial, Seeded Grassland, Broken Grassland
        /// </summary>
        /// <param name="carbonChange">C change (kg C year^-1)</param>
        /// <returns>CO2 change (kg CO2 year^-1)</returns>
        public double CalculateCarbonDioxideChange(double carbonChange)
        {
            var constant = (-1) * CoreConstants.ConvertFromCToCO2;
            return carbonChange * constant;
        }

        /// <summary>
        /// Equation 2.1.5-1
        /// </summary>
        /// <param name="carbonChangeForTillage">C change for tillage (kg C year^-1)</param>
        /// <param name="carbonChangeForFallow">C change for fallow (kg C year^-1)</param>
        /// <param name="carbonChangeForPerennialSeeded">C change for perennial seeded (kg C year^-1)</param>
        /// <param name="carbonChangeForPerennialPast">C change for perennial past (kg C year^-1)</param>
        /// <param name="carbonChangeForGrasslandSeeded">C change for grassland seeded (kg C year^-1)</param>
        /// <param name="carbonChangeForGrasslandBroken">C change for grassland broken (kg C year^-1)</param>
        /// <returns>C change for soils (kg C year^-1)</returns>
        public double CalculateTotalCarbonChangeForSoils(double carbonChangeForTillage, double carbonChangeForFallow,
                                                         double carbonChangeForPerennialSeeded, double carbonChangeForPerennialPast,
                                                         double carbonChangeForGrasslandSeeded, double carbonChangeForGrasslandBroken)
        {
            var sum = carbonChangeForTillage + carbonChangeForFallow + carbonChangeForPerennialSeeded +
                      carbonChangeForPerennialPast + carbonChangeForGrasslandSeeded + carbonChangeForGrasslandBroken;
            return -sum;
        }

        /// <summary>
        /// Equation 2.1.5-2
        /// </summary>
        /// <param name="CarbonDioxideChangeForTillage">CO2 change for tillage (kg CO2 year^-1)</param>
        /// <param name="CarbonDioxideChangeForFallow">CO2 change for fallow (kg CO2 year^-1)</param>
        /// <param name="CarbonDioxideChangeForPerennial">CO2 change for perennial (kg CO2 year^-1)</param>
        /// <param name="CarbonDioxideChangeForGrassland">CO2 change for grassland (kg CO2 year^-1)</param>
        /// <returns>CO2 change for soil (kg CO2 year^-1)</returns>
        public double CalculateTotalCarbonDioxideChangeForSoils(double CarbonDioxideChangeForTillage,
                                                                double CarbonDioxideChangeForFallow, double CarbonDioxideChangeForPerennial,
                                                                double CarbonDioxideChangeForGrassland)
        {
            return CarbonDioxideChangeForTillage + CarbonDioxideChangeForFallow + CarbonDioxideChangeForPerennial +
                   CarbonDioxideChangeForGrassland;
        }

        /// <summary>
        /// Equation 2.1.6-1
        /// </summary>
        /// <param name="carbonDioxideChangeForSoilsPerYear">CO2 change for soils (kg CO2 year^-1)</param>
        /// <returns>CO2 change for soils (kg CO2 month^-1) – by month</returns>
        public double CalculateCarbonDioxideChangeForSoilsPerMonth(double carbonDioxideChangeForSoilsPerYear)
        {
            return carbonDioxideChangeForSoilsPerYear / 12;
        }
    }
}