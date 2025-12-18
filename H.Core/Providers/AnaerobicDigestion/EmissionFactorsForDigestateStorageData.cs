using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Enumerations;

namespace H.Core.Providers.AnaerobicDigestion
{
    public class EmissionFactorsForDigestateStorageData
    {
        /// <summary>
        /// The type of emission from the digestate
        /// </summary>
        public EmissionTypes EmissionType { get; set; }

        /// <summary>
        /// The source/origin of the emission during storage. Defined by the state of the digestate, Raw, Liquid or Solid.
        /// </summary>
        public DigestateState EmissionOrigin { get; set; }

        /// <summary>
        /// The Emission Factor(EF) of the digestate based on the state of the digestate and the type of emission.
        /// Denotes the fraction of a parameter in digestate during storage. Refer to description property for per item description.
        /// </summary>
        public double EmissionFactor { get; set; }

        /// <summary>
        /// Provides an explanation regarding the emission factor e.g. what it is a fraction of.
        /// </summary>
        public string Description { get; set; }
    }
}
