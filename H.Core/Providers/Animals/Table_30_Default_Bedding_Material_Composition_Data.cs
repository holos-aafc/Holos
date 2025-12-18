using H.Core.Enumerations;
using H.Core.Models;
using H.Infrastructure;

namespace H.Core.Providers.Animals
{
    public class Table_30_Default_Bedding_Material_Composition_Data
    {
        #region Methods

        public override string ToString()
        {
            return $"{nameof(BeddingMaterial)}: {BeddingMaterial}";
        }

        #endregion

        #region Properties

        public ComponentCategory ComponentCategory { get; set; }

        public AnimalType AnimalType { get; set; }

        public string ComponentCategoryString => ComponentCategory.GetDescription();

        public BeddingMaterialType BeddingMaterial { get; set; }

        public string BeddingMaterialString => BeddingMaterial.GetDescription();

        /// <summary>
        ///     Unit of Measurement: %
        /// </summary>
        public double MoistureContent { get; set; }

        /// <summary>
        ///     Unit of Measurement: (kg N/kg DM)
        /// </summary>
        public double TotalNitrogenKilogramsDryMatter { get; set; }

        /// <summary>
        ///     Unit of Measurement: (kg C/kg DM)
        /// </summary>
        public double TotalCarbonKilogramsDryMatter { get; set; }

        public double TotalPhosphorusKilogramsDryMatter { get; set; }

        /// <summary>
        ///     Unit of Measurement: (unitless)
        /// </summary>
        public double CarbonToNitrogenRatio { get; set; }

        #endregion
    }
}