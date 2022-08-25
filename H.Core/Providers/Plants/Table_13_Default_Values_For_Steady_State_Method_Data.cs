using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Enumerations;

namespace H.Core.Providers.Plants
{
    public class Table_13_Default_Values_For_Steady_State_Method_Data
    {
        /// <summary>
        /// The livestock manure type represented by the animal.
        /// </summary>
        public AnimalType AnimalType { get; set; }

        /// <summary>
        /// The carbon to nitrogen ratio of Manure.
        /// </summary>
        public double CarbonToNitrogenRatio { get; set; }
        
        /// <summary>
        /// Nitrogen content of Manure (% dry basis)
        /// </summary>
        public double NitrogenContentManure { get; set; }

        /// <summary>
        /// Lignin Content of Manure (% dry basis)
        /// </summary>
        public double LigninContentManure { get; set; }
    }
}
