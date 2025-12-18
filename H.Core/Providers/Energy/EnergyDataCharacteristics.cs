using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Enumerations;

namespace H.Core.Providers.Energy
{
    public class EnergyDataCharacteristics
    {
        /// <summary>
        /// The province for which crop energy estimate data is required.
        /// </summary>
        public Province Province { get; set; }

        /// <summary>
        /// The functional soil category of the crop.
        /// </summary>
        public SoilFunctionalCategory SoilFunctionalCategory { get; set; }

        /// <summary>
        /// The tillage type used for the crop.
        /// </summary>
        public TillageType TillageType { get; set; }
    }
}
