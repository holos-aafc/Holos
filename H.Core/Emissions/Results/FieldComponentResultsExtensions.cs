using System.Collections.Generic;
using System.Linq;
using H.Core.Emissions.Results;

namespace H.Core.Emissions.Results
{
    public static class FieldComponentResultsExtensions
    {
        /*
         * Equation 4.1.5-1
         */
        public static double TotalCroppingEnergyEmissions(this IEnumerable<FieldComponentEmissionResults> results)
        {
            return results.Sum(result => result.CropEnergyResults.TotalCroppingEnergyEmissions);
        }
    }
}