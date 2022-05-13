using System.Collections.Generic;
using System.Linq;
using H.Core.Emissions.Results;

namespace H.Core.Emissions.Results
{
    public static class FieldComponentResultsExtensions
    {
        /*
         * Equation 6.1.5-1
         */
        public static double TotalOnFarmCroppingEnergyEmissions(this IEnumerable<FieldComponentEmissionResults> results)
        {
            return results.Sum(result => result.CropEnergyResults.TotalOnFarmCroppingEnergyEmissions);
        }

        /*
         * No equation number
         */
        public static double TotalUpstreamCroppingEnergyEmissions(this IEnumerable<FieldComponentEmissionResults> results)
        {
            return results.Sum(result => result.CropEnergyResults.TotalUpstreamCroppingEnergyEmissions);
        }
    }
}