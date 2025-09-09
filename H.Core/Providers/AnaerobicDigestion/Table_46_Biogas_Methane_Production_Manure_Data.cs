using H.Core.Enumerations;
using H.Core.Providers.Climate;

namespace H.Core.Providers.AnaerobicDigestion
{
    public class Table_46_Biogas_Methane_Production_Manure_Data : BiogasAndMethaneProductionParametersData
    {
        /// <summary>
        ///     The animal type producing the manure.
        /// </summary>
        public AnimalType AnimalType { get; set; }

        /// <summary>
        ///     Bedding type of the animal producing the manure.
        /// </summary>
        public BeddingMaterialType BeddingType { get; set; }
    }
}