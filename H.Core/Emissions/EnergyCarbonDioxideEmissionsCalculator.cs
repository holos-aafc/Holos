using System.Collections.Generic;
using System.Linq;

namespace H.Core.Emissions
{
    public class EnergyCarbonDioxideEmissionsCalculator
    {
        /// <summary>
        /// Equation 4.3.1-3
        /// Sum from all animal types
        /// </summary>
        /// <param name="carbonDioxideEmissionsFromLiquidManureSpreading">CO2 emissions from liquid manure spreading (kg CO2 year^-1)</param>
        /// <returns>Total CO2 emissions from liquid manure spreading (kg CO2 year^-1)</returns>
        public double CalculateTotalCarbonDioxideEmissionsFromLiquidManureSpreading(
            List<double> carbonDioxideEmissionsFromLiquidManureSpreading)
        {
            return carbonDioxideEmissionsFromLiquidManureSpreading.Sum();
        }



        /// <summary>
        /// Equation 4.3.1-6
        /// Sum from all animal types
        /// </summary>
        /// <param name="carbonDioxideEmissionsFromSolidManureSpreading">CO2 emissions from solid manure spreading (kg CO2 year^-1)</param>
        /// <returns>Total CO2 emissions from solid manure spreading (kg CO2 year^-1)</returns>
        public double CalculateTotalCarbonDioxideEmissionsFromSolidManureSpreading(
            List<double> carbonDioxideEmissionsFromSolidManureSpreading)
        {
            return carbonDioxideEmissionsFromSolidManureSpreading.Sum();
        }



        /// <summary>
        /// Equation 4.4.1-2
        /// </summary>
        /// <param name="totalCarbonDioxideEmissionsFromDairyOperations"></param>
        /// <param name="totalCarbonDioxideEmissionsFromSwineOperations"></param>
        /// <param name="totalCarbonDioxideEmissionsFromPoultryOperations"></param>
        /// <param name="totalCarbonDioxideEmissionsFromHousedBeefOperations"></param>
        /// <param name="totalCarbonDioxideEmissionsFromLiquidManureSpreading"></param>
        /// <param name="totalCarbonDioxideEmissionsFromSolidManureSpreading"></param>
        /// <returns></returns>
        public double CalculateTotalCarbonDioxideEmissionsFromLivestockEnergyUse(double totalCarbonDioxideEmissionsFromDairyOperations,
                                                                              double totalCarbonDioxideEmissionsFromSwineOperations,
                                                                              double totalCarbonDioxideEmissionsFromPoultryOperations,
                                                                              double totalCarbonDioxideEmissionsFromHousedBeefOperations,
                                                                              double totalCarbonDioxideEmissionsFromLiquidManureSpreading,
                                                                              double totalCarbonDioxideEmissionsFromSolidManureSpreading)
        {
            return totalCarbonDioxideEmissionsFromDairyOperations +
                   totalCarbonDioxideEmissionsFromSwineOperations +
                   totalCarbonDioxideEmissionsFromPoultryOperations +
                   totalCarbonDioxideEmissionsFromHousedBeefOperations +
                   totalCarbonDioxideEmissionsFromLiquidManureSpreading +
                   totalCarbonDioxideEmissionsFromSolidManureSpreading;
        }


        /// <summary>
        /// Equation 4.4.1-3
        /// </summary>
        /// <param name="totalCarbonDioxideEmissionsFromCroppingEnergyUse"></param>
        /// <param name="totalCarbonDioxideEmissionsFromLivestockEnergyUse"></param>
        /// <returns></returns>
        public double CalculateTotalCarbonDioxideEmissionsFromEnergyUse(double totalCarbonDioxideEmissionsFromCroppingEnergyUse,
                                                                        double totalCarbonDioxideEmissionsFromLivestockEnergyUse)
        {
            return totalCarbonDioxideEmissionsFromCroppingEnergyUse + totalCarbonDioxideEmissionsFromLivestockEnergyUse;
        }
    }
}