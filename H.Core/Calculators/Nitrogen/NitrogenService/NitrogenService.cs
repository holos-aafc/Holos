using System.Collections.Generic;
using System.Linq;
using H.Core.Calculators.Carbon;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Services;
using H.Core.Services.Animals;
using H.Core.Services.Initialization.Crops;

namespace H.Core.Calculators.Nitrogen.NitrogenService
{
    /// <summary>
    /// A service class to calculate inputs related to N. This will route calculations to the IPCC Tier 2 or ICBM methodology as required
    /// </summary>
    public partial class NitrogenService : INitrogenService
    {
        #region Fields

        private IFieldComponentHelper _fieldComponentHelper;
        private readonly ICarbonService _carbonService;
        private readonly IAnimalService _animalService;

        private readonly IICBMNitrogenInputCalculator _icbmNitrogenInputCalculator;
        private readonly IIPCCNitrogenInputCalculator _ipccNitrogenInputCalculator;

        private readonly ICropInitializationService _cropInitializationService;

        #endregion

        #region Constructors

        public NitrogenService()
        {
            _carbonService = new CarbonService();
            _animalService = new AnimalResultsService();
            _fieldComponentHelper = new FieldComponentHelper();

            _icbmNitrogenInputCalculator = new ICBMNitrogenInputCalculator();
            _ipccNitrogenInputCalculator = new IPCCNitrogenInputCalculator();

            _cropInitializationService = new CropInitializationService();
        }

        #endregion

        #region Public Methods

        public void AssignNitrogenInputs(List<CropViewItem> viewItems, Farm farm)
        {
            if (farm.IsCommandLineMode)
            {
                this.ProcessCommandLineItems(viewItems, farm);
            }
            else
            {
                for (int i = 0; i < viewItems.Count; i++)
                {
                    var itemAtIndex = viewItems.ElementAt(i);
                    var year = itemAtIndex.Year;
                    var tuple = _fieldComponentHelper.GetAdjoiningYears(viewItems, year);

                    this.AssignNitrogenInputs(tuple.CurrentYearViewItem, farm, tuple.PreviousYearViewItem);
                }
            }
        }

        public void AssignNitrogenInputs(AdjoiningYears adjoiningYears, Farm farm)
        {
            this.AssignNitrogenInputs(adjoiningYears.CurrentYearViewItem, farm, adjoiningYears.PreviousYearViewItem);
        }

        public void AssignNitrogenInputs(CropViewItem currentYearViewItem, Farm farm, CropViewItem previousYearViewItem)
        {
            currentYearViewItem.AbovegroundNitrogenInputs = this.CalculateAboveGroundResidueNitrogen(farm, currentYearViewItem, previousYearViewItem);
            currentYearViewItem.BelowgroundNitrogenInputs = this.CalculateBelowGroundResidueNitrogen(farm, currentYearViewItem, previousYearViewItem);
        }

        public double CalculateAboveGroundResidueNitrogen(
            Farm farm, 
            CropViewItem currentYearViewItem,
            CropViewItem previousYearViewItem)
        {


            if (this.CanCalculateNitrogenInputsUsingIpccTier2(currentYearViewItem) && farm.Defaults.CarbonModellingStrategy == CarbonModellingStrategies.IPCCTier2)
            {
                return _ipccNitrogenInputCalculator.CalculateTotalAboveGroundResidueNitrogenUsingIpccTier2(
                    currentYearViewItem.AboveGroundResidueDryMatter,
                    currentYearViewItem.CarbonConcentration,
                    currentYearViewItem.NitrogenContentInStraw);
            }
            else
            {
                return _icbmNitrogenInputCalculator.CalculateTotalAboveGroundResidueNitrogenUsingIcbm(
                    previousYearViewItem, currentYearViewItem);
            }
        }

        public double CalculateBelowGroundResidueNitrogen(
            Farm farm, 
            CropViewItem currentYearViewItem,
            CropViewItem previousYearViewItem)
        {
            if (this.CanCalculateNitrogenInputsUsingIpccTier2(currentYearViewItem) && farm.Defaults.CarbonModellingStrategy == CarbonModellingStrategies.IPCCTier2)
            {
                return _ipccNitrogenInputCalculator.CalculateTotalBelowGroundResidueNitrogenUsingIpccTier2(currentYearViewItem, previousYearViewItem);
            }
            else
            {
                return _icbmNitrogenInputCalculator.CalculateTotalBelowGroundResidueNitrogenUsingIcbm(currentYearViewItem, previousYearViewItem);
            }
        }

        public double CalculateCropResidueExportNitrogen(CropViewItem cropViewItem, HarvestViewItem harvestViewItem)
        {
            var result = 0d;

            return result;
        }

        public double CalculateCropResidueExportNitrogen(CropViewItem cropViewItem)
        {
            if (cropViewItem.CropType.IsPerennial())
            {
                // Convert to per ha
                var dryMatterPerHectare = cropViewItem.TotalDryMatterLostFromBaleExports / cropViewItem.Area;

                // Use N content in product since N content in straw is 0 for perennials

                // Use IPCC Tier 2 for now until there is a method in algorithm document for ICBM approach
                return _ipccNitrogenInputCalculator.CalculateTotalNitrogenFromExportedResidues(dryMatterPerHectare, cropViewItem.NitrogenContentInProduct);
            }
            else
            {
                // Use IPCC Tier 2
                return _ipccNitrogenInputCalculator.CalculateTotalNitrogenFromExportedResidues(cropViewItem.AboveGroundResidueDryMatterExported, cropViewItem.NitrogenContentInStraw);
            }
        }

        /// <summary>
        /// Once N inputs have been determined for all crops, this method will check if there are one or more crops grown in the same year. If there is, the total N
        /// inputs from the crops being grown in the same year will be combined and assigned to the main crop for that year since it is the inputs from the main crop
        /// that are used in the N models
        /// </summary>
        public void CombineNitrogenInputs(Farm farm, List<CropViewItem> viewItems)
        {
            var distinctYears = viewItems.GetDistinctYears();
            foreach (var year in distinctYears)
            {
                var viewItemsForYear = viewItems.GetItemsByYear(year);
                var mainCropForYear = _fieldComponentHelper.GetMainCropForYear(viewItemsForYear, year);
                var secondaryCropsForYear = viewItemsForYear.GetSecondaryCrops(mainCropForYear);

                var totalAbovegroundNitrogenFromCoverCrops = this.SumAbovegroundNitrogen(secondaryCropsForYear);
                var totalBelowgroundNitrogenFromCoverCrops = this.SumBelowgroundNitrogen(secondaryCropsForYear);

                /*
                 * Sum up the main crop and cover crop N inputs
                 */

                mainCropForYear.CombinedAboveGroundResidueNitrogen = (mainCropForYear.AbovegroundNitrogenInputs + totalAbovegroundNitrogenFromCoverCrops);
                mainCropForYear.CombinedBelowGroundResidueNitrogen = (mainCropForYear.BelowgroundNitrogenInputs + totalBelowgroundNitrogenFromCoverCrops);
            }
        }

        public double SumAbovegroundNitrogen(List<CropViewItem> viewItems)
        {
            return viewItems.Sum(x => x.AbovegroundNitrogenInputs);
        }

        public double SumBelowgroundNitrogen(List<CropViewItem> viewItems)
        {
            return viewItems.Sum(x => x.BelowgroundNitrogenInputs);
        }

        #endregion

        #region Private Methods

        private bool CanCalculateNitrogenInputsUsingIpccTier2(CropViewItem cropViewItem)
        {
            return _carbonService.CanCalculateInputsUsingIpccTier2(cropViewItem);
        }

        #endregion
    }
}