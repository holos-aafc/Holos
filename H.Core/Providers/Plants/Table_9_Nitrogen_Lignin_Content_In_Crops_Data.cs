using H.Core.Enumerations;
using H.Core.Providers.AnaerobicDigestion;

namespace H.Core.Providers.Plants
{
    public class Table_9_Nitrogen_Lignin_Content_In_Crops_Data
    {
        #region Constructors

        public Table_9_Nitrogen_Lignin_Content_In_Crops_Data()
        {
            BiomethaneData = new Table_46_Biogas_Methane_Production_CropResidue_Data();
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return
                $"{nameof(CropType)}: {CropType}, {nameof(InterceptValue)}: {InterceptValue}, {nameof(SlopeValue)}: {SlopeValue}, {nameof(RSTRatio)}: {RSTRatio}, {nameof(NitrogenContentResidues)}: {NitrogenContentResidues}, {nameof(LigninContentResidues)}: {LigninContentResidues}, {nameof(MoistureContent)}: {MoistureContent}, {nameof(BiomethaneData)}: {BiomethaneData}";
        }

        #endregion

        #region Properties

        /// <summary>
        ///     The crop type for which we need the various information and values.
        /// </summary>
        public CropType CropType { get; set; }

        /// <summary>
        ///     The intercept value given the crop type. Taken from national inventory numbers.
        /// </summary>
        public double InterceptValue { get; set; }

        /// <summary>
        ///     The slop value given the crop type. Taken from national inventory numbers.
        /// </summary>
        public double SlopeValue { get; set; }

        /// <summary>
        ///     Shoot to root ratio of the crop. The ratio of below-ground root biomass to above-ground shoot
        /// </summary>
        public double RSTRatio { get; set; }

        /// <summary>
        ///     Nitrogen Content of residues. Unit of measurement = Proportion of Carbon content
        /// </summary>
        public double NitrogenContentResidues { get; set; }

        /// <summary>
        ///     Lignin content of residue. Unit of measurement = Proportion of Carbon content
        /// </summary>
        public double LigninContentResidues { get; set; }

        /// <summary>
        ///     Moisture content of crop
        ///     (%)
        /// </summary>
        public double MoistureContent { get; set; }

        public Table_46_Biogas_Methane_Production_CropResidue_Data BiomethaneData { get; set; }

        #endregion
    }
}