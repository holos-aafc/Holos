using System.Collections.Generic;
using System.Linq;

namespace H.Core.Emissions.Results
{
    public static class FarmEmissionResultsExtensions
    {
        /// <summary>
        /// Returns the total enteric methane produced from all farms.
        ///
        /// (kg CH4)
        /// </summary>
        public static double TotalEntericMethaneForAllFarms(this IEnumerable<FarmEmissionResults> farmEmissionResults)
        {
            return farmEmissionResults.Sum(result => result.TotalEntericMethaneFromFarm);
        }

        /// <summary>
        /// Returns the total manure methane produced from all farms.
        ///
        /// (kg CH4)
        /// </summary>
        public static double TotalManureMethaneForAllFarms(this IEnumerable<FarmEmissionResults> farmEmissionResults)
        {
            return farmEmissionResults.Sum(result => result.TotalManureMethaneFromFarm);
        }

        /// <summary>
        /// Returns the total direct nitrous oxide produced from all farms.
        ///
        /// (kg N2O)
        /// </summary>
        public static double TotalDirectNitrousOxideForAllFarms(this IEnumerable<FarmEmissionResults> farmEmissionResults)
        {
            return farmEmissionResults.Sum(result => result.TotalDirectNitrousOxideFromFarm);
        }

        /// <summary>
        /// Returns the total indirect nitrous oxide produced from all farms.
        ///
        /// (kg N2O)
        /// </summary>
        public static double TotalIndirectNitrousOxideForAllFarms(this IEnumerable<FarmEmissionResults> farmEmissionResults)
        {
            return farmEmissionResults.Sum(result => result.TotalIndirectNitrousOxideFromFarm);
        }

        /// <summary>
        /// Returns the total energy carbon dioxide produced from all farms.
        ///
        /// (kg CO2)
        /// </summary>
        public static double TotalEnergyCarbonDioxideForAllFarms(this IEnumerable<FarmEmissionResults> farmEmissionResults)
        {
            return farmEmissionResults.Sum(result => result.TotalEnergyCarbonDioxideFromFarm);
        }

        /// <summary>
        /// Returns the total carbon dioxide produced from all farms.
        ///
        /// (kg CO2)
        /// </summary>
        public static double TotalCarbonDioxideForAllFarms(this IEnumerable<FarmEmissionResults> farmEmissionResults)
        {
            return farmEmissionResults.Sum(result => result.TotalCarbonDioxideFromFarm);
        }

        /// <summary>
        /// Returns the total carbon dioxide equivalents produced from all farms.
        ///
        /// (kg CO2e)
        /// </summary>
        public static double TotalCarbonDioxideEquivalentsForAllFarms(this IEnumerable<FarmEmissionResults> farmEmissionResults)
        {
            return farmEmissionResults.Sum(result => result.TotalCarbonDioxideEquivalentsFromFarm);
        }
    }
}