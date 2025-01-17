#region Imports

using System.Collections.Generic;
using H.Core.Enumerations;
using H.Core.Providers.AnaerobicDigestion;

#endregion

namespace H.Core.Providers.Carbon
{
    /// <summary>
    /// </summary>
    public class Table_7_Relative_Biomass_Information_Data
    {
        #region Constructors

        #endregion

        #region Properties        

        public CropType CropType { get; set; }
        public IrrigationType? IrrigationType { get; set; }
        public double MoistureContentOfProduct { get; set; }

        /// <summary>
        /// (Unitless)
        /// </summary>
        public double RelativeBiomassProduct { get; set; }

        /// <summary>
        /// (Unitless)
        /// </summary>
        public double RelativeBiomassStraw { get; set; }

        /// <summary>
        /// (Unitless)
        /// </summary>
        public double RelativeBiomassRoot { get; set; }

        /// <summary>
        /// (Unitless)
        /// </summary>
        public double RelativeBiomassExtraroot { get; set; }

        /// <summary>
        /// Nitrogen content of product (g N/kg)
        /// </summary>
        public double NitrogenContentProduct { get; set; }

        /// <summary>
        /// Nitrogen content of straw (g N/kg)
        /// </summary>
        public double NitrogenContentStraw { get; set; }

        /// <summary>
        /// Nitrogen content of root (g N/kg)
        /// </summary>
        public double NitrogenContentRoot { get; set; }

        /// <summary>
        /// Nitrogen content of extraroot (g N/kg)
        /// </summary>
        public double NitrogenContentExtraroot { get; set; }

        public double IrrigationLowerRangeLimit { get; set; }
        public double IrrigationUpperRangeLimit { get; set; }

        public double LigninContent { get; set; }

        public Dictionary<Province, Dictionary<SoilFunctionalCategory, double>> NitrogenFertilizerRateTable { get; set; } = new Dictionary<Province, Dictionary<SoilFunctionalCategory, double>>();
        public Dictionary<Province, Dictionary<SoilFunctionalCategory, double>> PhosphorusFertilizerRateTable { get; set; } = new Dictionary<Province, Dictionary<SoilFunctionalCategory, double>>();
        public Dictionary<Province, TillageType> TillageTypeTable { get; set; } = new Dictionary<Province, TillageType>();
        public Province? Province { get; set; }
        public Table_46_Biogas_Methane_Production_CropResidue_Data BiomethaneData { get; set; } = new Table_46_Biogas_Methane_Production_CropResidue_Data();

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return $"{nameof(CropType)}: {CropType}, {nameof(MoistureContentOfProduct)}: {MoistureContentOfProduct}, {nameof(RelativeBiomassProduct)}: {RelativeBiomassProduct}, {nameof(RelativeBiomassStraw)}: {RelativeBiomassStraw}, {nameof(RelativeBiomassRoot)}: {RelativeBiomassRoot}, {nameof(RelativeBiomassExtraroot)}: {RelativeBiomassExtraroot}";
        }

        #endregion
    }
}
