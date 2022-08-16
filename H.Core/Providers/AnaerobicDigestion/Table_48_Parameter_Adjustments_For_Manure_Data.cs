using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Core.Providers.AnaerobicDigestion
{
    public class Table_48_Parameter_Adjustments_For_Manure_Data
    {
        /// <summary>
        /// Table 48
        /// Fixed reduction in VS in stored solid manure entering the digester following a pre-digester storage period. This reduction depends on the storage method and is applied to all animal groups
        /// </summary>
        public double VolatileSolidsReductionFactor { get; set; }

        /// <summary>
        /// Unit of Measurement = Per Day
        /// Hydrolysis rate of substrate i during digestion (day^-1).
        /// </summary>
        public double HydrolysisRateOfSubstrate { get; set; }

    }
}
