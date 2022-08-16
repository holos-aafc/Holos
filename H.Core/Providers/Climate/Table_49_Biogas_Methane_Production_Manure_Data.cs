using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Core.Enumerations;

namespace H.Core.Providers.Climate
{
    public class Table_49_Biogas_Methane_Production_Manure_Data : BiogasAndMethaneProductionParametersData
    {
        /// <summary>
        /// The animal type producing the manure.
        /// </summary>
        public AnimalType AnimalType { get; set; }

        /// <summary>
        /// Bedding type of the animal producing the manure.
        /// </summary>
        public BeddingMaterialType BeddingType { get; set; }
    }
}
