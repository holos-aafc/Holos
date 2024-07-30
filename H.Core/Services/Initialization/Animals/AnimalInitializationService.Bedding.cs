using System.Collections.ObjectModel;
using H.Core.Models.Animals;
using H.Core.Models;
using H.Core.Providers.Animals;

namespace H.Core.Services.Initialization.Animals
{
    public partial class AnimalInitializationService
    {
        #region Public Methods

        /// <summary>
        /// Reinitialize the <see cref="Table_30_Default_Bedding_Material_Composition_Data"/> for all <see cref="ManagementPeriod"/>s for the <see cref="Farm"/>.
        /// </summary>
        /// <param name="farm">The farm containing the <see cref="ManagementPeriod"/>s to initialize with new defaults</param>
        public void ReinitializeBeddingMaterial(Farm farm)
        {
            if (farm != null)
            {
                var data = _beddingMaterialCompositionProvider.Data;
                farm.DefaultsCompositionOfBeddingMaterials.Clear();
                farm.DefaultsCompositionOfBeddingMaterials.AddRange(data);

                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                {
                    var beddingMaterialComposition = farm.GetBeddingMaterialComposition(
                        beddingMaterialType: managementPeriod.HousingDetails.BeddingMaterialType,
                        animalType: managementPeriod.AnimalType);

                    this.InitializeBeddingMaterial(managementPeriod, beddingMaterialComposition);
                }
            }
        }

        /// <summary>
        /// Reinitialize the <see cref="Table_30_Default_Bedding_Material_Composition_Data"/> for the <see cref="ManagementPeriod"/>.
        /// </summary>
        /// <param name="managementPeriod">The <see cref="ManagementPeriod"/> that will be reinitialized to new default values</param>
        /// <param name="data">The defaults to use for the initialization</param>
        public void InitializeBeddingMaterial(ManagementPeriod managementPeriod, Table_30_Default_Bedding_Material_Composition_Data data)
        {
            if (managementPeriod != null && managementPeriod.HousingDetails != null && data != null)
            {
                managementPeriod.HousingDetails.TotalCarbonKilogramsDryMatterForBedding = data.TotalCarbonKilogramsDryMatter;
                managementPeriod.HousingDetails.TotalNitrogenKilogramsDryMatterForBedding = data.TotalNitrogenKilogramsDryMatter;
                managementPeriod.HousingDetails.TotalPhosphorusKilogramsDryMatterForBedding = data.TotalPhosphorusKilogramsDryMatter;
                managementPeriod.HousingDetails.MoistureContentOfBeddingMaterial = data.MoistureContent;
            }
        }

        #endregion
    }
}