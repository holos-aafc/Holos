using System.Collections.Generic;
using System.Linq;

namespace H.Core.Emissions
{
    public class SummationsCalculator
    {
        /// <summary>
        /// Equation 6.1.1-1
        /// </summary>
        /// <param name="directNitrousOxideEmissionsFromSoilsKg">Direct N2O emissions from soils (kg N2O year^-1)</param>
        /// <returns>Direct N2O emissions from soils (Mg CO2 eq year^-1)</returns>
        public double CalculateDirectNitrousOxideEmissionsFromSoilsToMgGlobalWarming(double directNitrousOxideEmissionsFromSoilsKg)
        {
            return directNitrousOxideEmissionsFromSoilsKg * 296.0 / 1000.0;

        }

        /// <summary>
        /// Equation 6.1.2-1
        /// </summary>
        /// <param name="indirectNitrousOxideEmissionsFromSoilsKg">Indirect N2O emissions from soils (kg N2O year^-1)</param>
        /// <returns>Indirect N2O emissions from soils (Mg CO2 eq year^-1)</returns>
        public double CalculateIndirectNitrousOxideEmissionsFromSoilsToMgGlobalWarming(double indirectNitrousOxideEmissionsFromSoilsKg)
        {
            return indirectNitrousOxideEmissionsFromSoilsKg * 296.0 / 1000.0;
        }

        /// <summary>
        /// Equation 6.2-1
        /// </summary>
        /// <param name="carbonDioxideEmissionsFromSoilsKg">CO2 emissions from soils (kg CO2 year^-1)</param>
        /// <returns>CO2 emissions from soils (Mg CO2 eq year^-1)</returns>
        public double CalculateCarbonDioxideEmissionsFromSoilsToMgGlobalWarming(double carbonDioxideEmissionsFromSoilsKg)
        {
            return carbonDioxideEmissionsFromSoilsKg / 1000.0;
        }

        /// <summary>
        /// Equation 6.3-1
        /// </summary>
        /// <param name="totalCarbonDioxideEmissionsFromTreePlantingsAndShelterbelt">Total CO2 emissions from tree plantings/shelterbelt (kg CO2 year^-1)</param>
        /// <returns>CO2 emissions from tree plantings/shelterbelt (Mg CO2 eq year^-1)</returns>
        public double CalculateCarbonDioxideEmissionsFromTreePlantingsAndShelterbeltToMgGlobalWarming(double totalCarbonDioxideEmissionsFromTreePlantingsAndShelterbelt)
        {
            return totalCarbonDioxideEmissionsFromTreePlantingsAndShelterbelt / 1000.0;
        }

        /// <summary>
        /// Equation 6.4-1
        /// </summary>
        /// <param name="totalCarbonDioxideEmissionsFromEnergyUseKg">Total enteric CH4 emission from livestock (kg CH4 year^-1)</param>
        /// <returns>Enteric CH4 emission from livestock (Mg CO2 eq year^-1)</returns>
        public double CalculateTotalCarbonDioxideEmissionsFromEnergyUseToMgGlobalWarming(double totalCarbonDioxideEmissionsFromEnergyUseKg)
        {
            return totalCarbonDioxideEmissionsFromEnergyUseKg / 1000.0;
        }

        /// <summary>
        /// Equation 6.5-1
        /// </summary>
        /// <param name="totalEntericMethaneEmissionsFromLivestock">Total manure CH4 emission from livestock (kg CH4 year^-1)</param>
        /// <returns>Manure CH4 emission from livestock (Mg CO2 eq year^-1)</returns>
        public double CalculateEntericMethaneEmissionFromLivestockToMgGlobalWarming(List<double> totalEntericMethaneEmissionsFromLivestock)
        {
            return totalEntericMethaneEmissionsFromLivestock.Sum() * 23.0 / 1000;
        }


        /// <summary>
        /// Equation 6.6-1
        /// </summary>
        /// <param name="totalManureMethaneEmissionsFromLivestock">Total manure CH4 emission from livestock (kg CH4 year^-1)</param>
        /// <returns>Manure CH4 emission from livestock (Mg CO2 eq year^-1)</returns>
        public double CalculateManureMethaneEmissionFromLivestockToMgGlobalWarming(List<double> totalManureMethaneEmissionsFromLivestock)
        {
            return totalManureMethaneEmissionsFromLivestock.Sum() * 23.0 / 1000;
        }

        /// <summary>
        /// Equation 6.7.1-1
        /// </summary>
        /// <param name="totalManureDirectNitrousOxideEmissionsFromLivestock">Total manure direct N2O emission from livestock (kg N2O year^-1)</param>
        /// <returns>Manure direct N2O emission from livestock (Mg CO2 eq year^-1)</returns>
        public double CalculateManureDirectNitrousOxideEmissionFromLivestockToMgGlobalWarming(IEnumerable<double> totalManureDirectNitrousOxideEmissionsFromLivestock)
        {
            return totalManureDirectNitrousOxideEmissionsFromLivestock.Sum() * 296.0 / 1000.0;
        }

        /// <summary>
        /// Equation 6.7.2-1
        /// </summary>
        /// <param name="totalManureIndirectNitrousOxideEmissionsFromLivestock">Total manure direct N2O emission from livestock (kg N2O year^-1)</param>
        /// <returns>Manure direct N2O emission from livestock (Mg CO2 eq year^-1)</returns>
        public double CalculateManureIndirectNitrousOxideEmissionFromLivestockToMgGlobalWarming(IEnumerable<double> totalManureIndirectNitrousOxideEmissionsFromLivestock)
        {
            return totalManureIndirectNitrousOxideEmissionsFromLivestock.Sum() * 296.0 / 1000.0;
        }

        /// <summary>
        /// Equation 6.8-1
        /// </summary>
        /// <param name="indirectNitrousOxideEmissionsFromSoilsMg">Indirect N2O emissions from soils (Mg CO2 eq year^-1)</param>
        /// <param name="indirectManureNitrousOxideEmissionsFromLivestockMg">Manure indirect N2O emission from livestock (Mg CO2 eq year^-1)</param>
        /// <returns>Indirect N2O emissions from farm (Mg CO2 eq year^-1)</returns>
        public double CalculateIndirectNitrousOxideEmissionsFromFarmToMgGlobalWarming(double indirectNitrousOxideEmissionsFromSoilsMg,
            double indirectManureNitrousOxideEmissionsFromLivestockMg)
        {
            return indirectManureNitrousOxideEmissionsFromLivestockMg + indirectNitrousOxideEmissionsFromSoilsMg;
        }

        /// <summary>
        /// Equation 6.9-1
        /// </summary>
        /// <param name="directN2OEmissionsFromSoilsMg">Direct N2O emissions from soils (Mg CO2 eq year^-1)</param>
        /// <param name="CO2EmissionsFromSoilsMg">CO2 emissions from soils (Mg CO2 eq year^-1)</param>
        /// <param name="CO2EmissionsFromTreePlantingsAndShelterbeltMg">CO2 emissions from tree plantings/shelterbelt (Mg CO2 eq year^-1)</param>
        /// <param name="CO2EmissionsFromEnergyUseMg">CO2 emissions from energy use (Mg CO2 eq year^-1)</param>
        /// <param name="entericMethaneEmissionsFromLivestockMg">Enteric CH4 emission from livestock (Mg CO2 eq year^-1)</param>
        /// <param name="manureMethaneEmissionsFromLivestockMg">Manure CH4 emission from livestock (Mg CO2 eq year^-1)</param>
        /// <param name="manureDirectN2OEmissionsFromLivestockMg">Manure direct N2O emission from livestock (Mg CO2 eq year^-1)</param>
        /// <param name="indirectN2OEmissionsFromFarmMg">Indirect N2O emissions from farm (Mg CO2 eq year^-1)</param>
        /// <returns>Total annual farm CO2 eq emissions (Mg CO2 eq year^-1)</returns>
        public double CalculateTotalAnnualFarmCarbonDioxideEQEmissionsMg(double directN2OEmissionsFromSoilsMg,
                                                                         double CO2EmissionsFromSoilsMg,
                                                                         double CO2EmissionsFromTreePlantingsAndShelterbeltMg,
                                                                         double CO2EmissionsFromEnergyUseMg,
                                                                         double entericMethaneEmissionsFromLivestockMg,
                                                                         double manureMethaneEmissionsFromLivestockMg,
                                                                         double manureDirectN2OEmissionsFromLivestockMg,
                                                                         double indirectN2OEmissionsFromFarmMg)
        {
            return directN2OEmissionsFromSoilsMg +
                CO2EmissionsFromSoilsMg +
                CO2EmissionsFromTreePlantingsAndShelterbeltMg +
                CO2EmissionsFromEnergyUseMg +
                entericMethaneEmissionsFromLivestockMg +
                manureMethaneEmissionsFromLivestockMg +
                manureDirectN2OEmissionsFromLivestockMg +
                indirectN2OEmissionsFromFarmMg;
        }

    }
}
