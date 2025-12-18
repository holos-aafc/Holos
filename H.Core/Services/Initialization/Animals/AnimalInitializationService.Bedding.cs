using System.Collections.ObjectModel;
using H.Core.Enumerations;
using H.Core.Models.Animals;
using H.Core.Models;
using H.Core.Providers.Animals;
using System.Collections.Generic;

namespace H.Core.Services.Initialization.Animals
{
    public partial class AnimalInitializationService
    {
        #region Public Methods

        /// <summary>
        /// Initialize the <see cref="Table_30_Default_Bedding_Material_Composition_Data"/> for all <see cref="ManagementPeriod"/>s for the <see cref="Farm"/>.
        /// </summary>
        /// <param name="farm">The farm containing the <see cref="ManagementPeriod"/>s to initialize with new defaults</param>
        public void InitializeBeddingMaterial(Farm farm)
        {
            if (farm != null)
            {
                var data = _beddingMaterialCompositionProvider.Data;
                farm.DefaultsCompositionOfBeddingMaterials.Clear();
                farm.DefaultsCompositionOfBeddingMaterials = new ObservableCollection<Table_30_Default_Bedding_Material_Composition_Data>(SeedDefaultBeddingCompositionData(data));

                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                {
                    var beddingMaterialComposition = farm.GetBeddingMaterialComposition(
                        beddingMaterialType: managementPeriod.HousingDetails.BeddingMaterialType,
                        animalType: managementPeriod.AnimalType);

                    this.InitializeBeddingMaterial(managementPeriod, beddingMaterialComposition);
                }
            }
        }

        public void ReinitializeBeddingMaterial(Farm farm)
        {
            if (farm != null)
            {
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                {
                    this.InitializeBeddingMaterial(managementPeriod, farm);
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

        /// <summary>
        /// Reinitialize the <see cref="Table_30_Default_Bedding_Material_Composition_Data"/> for the <see cref="ManagementPeriod"/>.
        /// </summary>
        /// <param name="managementPeriod">The <see cref="ManagementPeriod"/> that will be reinitialized to new default values</param>
        /// <param name="farm"></param>
        public void InitializeBeddingMaterial(ManagementPeriod managementPeriod, Farm farm)
        {
            if (managementPeriod != null && farm != null)
            {
                var beddingMaterialComposition = farm.GetBeddingMaterialComposition(
                    beddingMaterialType: managementPeriod.HousingDetails.BeddingMaterialType,
                    animalType: managementPeriod.AnimalType);

                this.InitializeBeddingMaterial(managementPeriod, beddingMaterialComposition);
            }
        }

        public void InitializeBeddingMaterialRate(Farm farm)
        {
            if (farm != null)
            {
                foreach (var managementPeriod in farm.GetAllManagementPeriods())
                {
                    this.InitializeBeddingMaterialRate(managementPeriod);
                }
            }
        }

        /// <summary>
        /// Reinitialize the <see cref="Table_30_Default_Bedding_Material_Composition_Data"/> for the <see cref="ManagementPeriod"/>.
        /// </summary>
        /// <param name="managementPeriod">The <see cref="ManagementPeriod"/> that will be reinitialized to new default values</param>
        public void InitializeBeddingMaterialRate(ManagementPeriod managementPeriod)
        {
            if (managementPeriod != null && managementPeriod.HousingDetails != null)
            {
                var beddingType = managementPeriod.HousingDetails.BeddingMaterialType;

                var beddingRate = _beddingMaterialCompositionProvider.GetDefaultBeddingRate(
                    managementPeriod.HousingDetails.HousingType,
                    managementPeriod.HousingDetails.BeddingMaterialType,
                    managementPeriod.AnimalType);

                managementPeriod.HousingDetails.UserDefinedBeddingRate = beddingRate;
            }
        }

        #endregion

        #region Private Methods
        private List<Table_30_Default_Bedding_Material_Composition_Data> SeedDefaultBeddingCompositionData(List<Table_30_Default_Bedding_Material_Composition_Data> source)
        {
            var beddingMaterialCompositionData = new List<Table_30_Default_Bedding_Material_Composition_Data>();

            foreach (var item in source)
            {
                var copiedInstance = _defaultBeddingCompositionDataMapper.Map<Table_30_Default_Bedding_Material_Composition_Data, Table_30_Default_Bedding_Material_Composition_Data>(item);
                beddingMaterialCompositionData.Add(copiedInstance);
            }
            return beddingMaterialCompositionData;
        }

        #endregion
    }
}