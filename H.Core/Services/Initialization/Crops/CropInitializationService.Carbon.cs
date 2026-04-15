using H.Core.Enumerations;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models;
using H.Core.Providers.Carbon;

namespace H.Core.Services.Initialization.Crops
{
    public partial class CropInitializationService
    {
        #region Public Methods

        /// <summary>
        /// Initialize the carbon concentration of each <see cref="CropViewItem"/> within a farm
        /// </summary>
        /// <param name="farm">The <see cref="Farm"/> containing the <see cref="CropViewItem"/>s that will have reset their carbon concentrations reset</param>
        public void InitializeCarbonConcentration(Farm farm)
        {
            var defaults = new Defaults();
            var viewItems = farm.GetAllCropViewItems();
            foreach (var viewItem in viewItems)
            {
                InitializeCarbonConcentration(viewItem, defaults);
            }
        }

        /// <summary>
        /// Initialize the carbon concentration of the <see cref="CropViewItem"/> with the values in the <see cref="Defaults"/> parameter
        /// </summary>
        /// <param name="viewItem">The <see cref="CropViewItem"/> to have it's carbon concentration reset with default value</param>
        /// <param name="defaults">The <see cref="Defaults"/> containing the default carbon concentration</param>
        public void InitializeCarbonConcentration(CropViewItem viewItem, Defaults defaults)
        {
            viewItem.CarbonConcentration = defaults.CarbonConcentration;
        }

        public void InitializeBiomassCoefficients(Farm farm)
        {
            foreach (var viewItem in farm.GetAllCropViewItems())
            {
                InitializeBiomassCoefficients(viewItem, farm);
            }
        }

        public void InitializeBiomassCoefficientProduct(CropViewItem viewItem, Farm farm)
        {
            var residueData = this.GetResidueData(farm, viewItem);
            if (residueData != null)
            {
                viewItem.BiomassCoefficientProduct = residueData.RelativeBiomassProduct;

                this.SetBasedOnHarvestMethod(viewItem, residueData);
            }
        }

        public void InitializeBiomassCoefficientStraw(CropViewItem viewItem, Farm farm)
        {
            var residueData = this.GetResidueData(farm, viewItem);
            if (residueData != null)
            {
                viewItem.BiomassCoefficientStraw = residueData.RelativeBiomassStraw;

                this.SetBasedOnHarvestMethod(viewItem, residueData);
            }
        }

        public void InitializeBiomassCoefficientRoots(CropViewItem viewItem, Farm farm)
        {
            var residueData = this.GetResidueData(farm, viewItem);
            if (residueData != null)
            {
                viewItem.BiomassCoefficientRoots = residueData.RelativeBiomassRoot;

                this.SetBasedOnHarvestMethod(viewItem, residueData);
            }
        }

        public void InitializeBiomassCoefficientExtraroots(CropViewItem viewItem, Farm farm)
        {
            var residueData = this.GetResidueData(farm, viewItem);
            if (residueData != null)
            {
                viewItem.BiomassCoefficientExtraroot = residueData.RelativeBiomassExtraroot;

                this.SetBasedOnHarvestMethod(viewItem, residueData);
            }
        }

        private void SetBasedOnHarvestMethod(CropViewItem viewItem, Table_7_Relative_Biomass_Information_Data residueData)
        {
            if (viewItem.HarvestMethod == HarvestMethods.Swathing || viewItem.HarvestMethod == HarvestMethods.GreenManure || viewItem.HarvestMethod == HarvestMethods.Silage)
            {
                viewItem.BiomassCoefficientProduct = residueData.RelativeBiomassProduct + residueData.RelativeBiomassStraw;
                viewItem.BiomassCoefficientStraw = 0;
                viewItem.BiomassCoefficientRoots = residueData.RelativeBiomassRoot;
                viewItem.BiomassCoefficientExtraroot = residueData.RelativeBiomassExtraroot;
            }
        }

        public void InitializeBiomassCoefficients(CropViewItem viewItem, Farm farm)
        {
            var residueData = this.GetResidueData(farm, viewItem);
            if (residueData != null)
            {
                this.InitializeBiomassCoefficientProduct(viewItem, farm);
                this.InitializeBiomassCoefficientStraw(viewItem, farm);
                this.InitializeBiomassCoefficientRoots(viewItem, farm);
                this.InitializeBiomassCoefficientExtraroots(viewItem, farm);

                this.SetBasedOnHarvestMethod(viewItem, residueData); 
            }
        }

        public void InitializeLumCMaxValues(CropViewItem cropViewItem, Farm farm)
        {
            if (!cropViewItem.CropType.IsPerennial() && !cropViewItem.CropType.IsGrassland() && !cropViewItem.CropType.IsFallow() && !cropViewItem.IsBrokenGrassland)
            {
                return;
            }

            var lumCMax = 0d;
            var kValue = 0d;

            var ecozone = _ecodistrictDefaultsProvider.GetEcozone(farm.GeographicData.DefaultSoilData.EcodistrictId);

            if (cropViewItem.CropType.IsPerennial() || cropViewItem.IsBrokenGrassland)
            {
                var changeType = _landManagementChangeHelper.GetPerennialCroppingChangeType(cropViewItem.PastPerennialArea, cropViewItem.Area);
                if (cropViewItem.IsBrokenGrassland)
                {
                    // From v3, if is broken grassland then use values for decrease in area when looking up lumc and k
                    changeType = PerennialCroppingChangeType.DecreaseInPerennialCroppingArea;
                }

                lumCMax = _lumCMaxKValuesPerennialCroppingChangeProvider.GetLumCMax(ecozone, farm.GeographicData.DefaultSoilData.SoilTexture, changeType);
                kValue = _lumCMaxKValuesPerennialCroppingChangeProvider.GetKValue(ecozone, farm.GeographicData.DefaultSoilData.SoilTexture, changeType);
            }
            else if (cropViewItem.CropType.IsFallow())
            {
                var changeType = _landManagementChangeHelper.GetFallowPracticeChangeType(cropViewItem.PastFallowArea, cropViewItem.Area);

                lumCMax = _lumCMaxKValuesFallowPracticeChangeProvider.GetLumCMax(ecozone, farm.GeographicData.DefaultSoilData.SoilTexture, changeType);
                kValue = _lumCMaxKValuesFallowPracticeChangeProvider.GetKValue(ecozone, farm.GeographicData.DefaultSoilData.SoilTexture, changeType);
            }

            cropViewItem.LumCMax = lumCMax;
            cropViewItem.KValue = kValue;
        }

        #endregion
    }
}