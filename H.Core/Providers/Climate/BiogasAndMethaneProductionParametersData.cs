using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Core.Providers.Climate
{
    public class BiogasAndMethaneProductionParametersData
    {
        /// <summary>
        /// BMP. Biomethane potential given a subtrate type. 
        /// Unit of measurement: Nm3 ton-1 VS
        /// </summary>
        public double BioMethanePotential { get; set; }

        /// <summary>
        /// f_CH4. Fraction of methane in biogas
        /// </summary>
        public double MethaneFraction { get; set; }

        /// <summary>
        /// Defined as percentage of total solids.
        /// Unit of measurement: %
        /// </summary>
        public double VolatileSolids { get; set; }

        /// <summary>
        /// TS. The amount of total solids in the substrate type
        /// Unit of measurement: (kg t^-1)^3
        /// </summary>
        public double TotalSolids { get; set; }

        /// <summary>
        /// Total Nitrogen in the substrate
        /// Unit of measurement: (KG N t^-1)^5
        /// </summary>
        public double TotalNitrogen{ get; set; }
    }
}
